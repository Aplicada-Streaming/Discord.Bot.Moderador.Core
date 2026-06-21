# Estrategia de versionado — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero DevOps Senior (AG-09) — variante DevOps + Deploy Engineer (web-monolith)

Este documento es el documento bisagra entre el código (Conventional Commits, branching) y el artefacto publicado (SemVer, canales, deprecation). Lo consumen tanto el implementador (al hacer commits y tags) como el pipeline (al calcular la versión y publicar). El artefacto del proyecto es un paquete self-contained para `linux-arm` instalable como servicio (ADR-05); no es redistribuible (`SOLUTION-INTAKE §17 identidad, P.7`), por lo que no hay feed de paquetes: la distribución es el paquete instalable, publicado como release del repositorio.

## 1. SemVer 2.0.0

Se adopta SemVer 2.0.0 sin excepciones (`SOLUTION-INTAKE §17 P.7`). Formato: `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`.

| Componente | Cuándo se incrementa |
| --- | --- |
| MAJOR | Cambio incompatible hacia el operador o hacia la integración: cambio que rompe el formato del archivo de entorno o el contrato de la variable de la clave maestra; migración de esquema no reversible que exige intervención manual; cambio en el contrato de instalación del servicio (unidad systemd) que invalida una instalación previa |
| MINOR | Funcionalidad retrocompatible: una rebanada nueva del mini-plan (nuevo tipo de regla, nueva acción, exenciones, revisión de incidentes), nuevos parámetros con valor por defecto compatible |
| PATCH | Corrección retrocompatible: fix de un evaluador, de la prueba de configuración, del cifrado o del panel, sin cambiar contratos ni esquema de forma incompatible |

La versión inicial es `1.0.0`. El identificador de prerelease usa los sufijos `-alpha.N`, `-beta.N`, `-rc.N`. El metadato de build (`+BUILDMETADATA`) puede portar el commit corto para trazabilidad y no afecta la precedencia.

Particularidad del proyecto: la aplicación no expone una API ni un contrato hacia terceros (`SOLUTION-INTAKE §17 P.3`, ADR-11), por lo que MAJOR no se dispara por cambios de API pública; se dispara por los contratos operativos hacia el operador y la instalación enumerados arriba. La compatibilidad de la integración con Discord sigue la versión de la API de la plataforma y de su librería cliente (`§17 P.3`) y no determina por sí sola el bump del producto.

## 2. Conventional Commits 1.0.0

Se adopta Conventional Commits 1.0.0 sin excepciones (`SOLUTION-INTAKE §17 P.7`). Cada commit usa un prefijo semántico; el marcador `!` o un footer `BREAKING CHANGE:` fuerza MAJOR. El mensaje de merge por squash a `main` es el que define el bump (ver §4).

| Prefijo | Significado | Bump SemVer | Ejemplo |
| --- | --- | --- | --- |
| `feat` | Funcionalidad nueva | MINOR | `feat(deteccion): agregar evaluador de conducta por volumen en un canal` |
| `fix` | Corrección de defecto | PATCH | `fix(cifrado): descifrar el token solo en memoria al reconectar` |
| `feat!` o footer `BREAKING CHANGE` | Cambio incompatible | MAJOR | `feat(servicio)!: cambiar el nombre de la variable de la clave maestra` |
| `refactor` | Refactor sin cambio de comportamiento | Ninguno | `refactor(pipeline): extraer el ensamblado de evaluadores` |
| `perf` | Mejora de rendimiento | Ninguno | `perf(ventana): reducir asignaciones en la ventana deslizante` |
| `test` | Tests | Ninguno | `test(deteccion): cubrir el borde de ráfaga fuera de ventana` |
| `docs` | Documentación | Ninguno | `docs(readme): documentar la instalación del servicio` |
| `build` | Build o empaquetado | Ninguno | `build(publish): ajustar el cross-compile a linux-arm` |
| `ci` | Pipeline | Ninguno | `ci(gates): agregar el stage de cobertura por capa` |
| `chore`, `style` | Mantenimiento, formato | Ninguno | `chore(deps): actualizar dependencias` |

El CHANGELOG se genera automáticamente desde los Conventional Commits siguiendo Keep a Changelog 1.1.0 y se publica en cada release (anti-patrón §4.8 "CHANGELOG ausente o no mantenido"). Los `BREAKING CHANGE` se listan en una sección destacada del CHANGELOG.

## 3. Herramienta de versionado

La versión se calcula automáticamente a partir de los tags de Git, no a mano (anti-patrón §4.8 "versionado manual"). La herramienta de auto-versioning está anclada a `SOLUTION-INTAKE §17 P.7`, que fija la capacidad y deja la elección puntual abierta a Sprint 0 entre las dos candidatas declaradas allí. Este documento no hardcodea cuál de las dos se adopta; documenta la decisión como abierta hasta Sprint 0, conforme a `§17 P.11` (decisiones deliberadamente abiertas para Sprint 0).

- Capacidad: cálculo de la versión SemVer desde el historial de tags y de Conventional Commits, con prefijo de tag `v`.
- Configuración base: prefijo de tag `v`; versión inicial `1.0.0`; precedencia de prerelease según SemVer 2.0.0.
- Elección puntual: abierta a Sprint 0 entre las dos herramientas candidatas de `§17 P.7`. Al cerrarse la decisión, se registra en el control de cambios de este documento y en el README del repositorio (categoría 10 colapsada, ADR-11), sin cambiar el resto de la estrategia.

## 4. Branching

Estrategia: GitHub Flow (`SOLUTION-INTAKE §17 P.7`), apropiada para un único desarrollador. Es nivel proceso y se nombra como tal.

- Trunk `main` protegido: el merge requiere PR con los gates G1–G5 en verde (`pipeline-ci-cd_v1.0.md §1`, `estrategia-calidad_v1.0.md §3`).
- Ramas de feature de vida corta: `feature/<slug>`, alineadas con la rebanada vertical del mini-plan de 07; vida corta (idealmente menos de una rebanada).
- PR obligatorio hacia `main`. Al ser un operador único, la revisión por pares se sustituye por los gates automáticos y la trazabilidad de cada test a CU/RN/NFR (`estrategia-calidad_v1.0.md §4`).
- Merge por squash con un mensaje Conventional Commits que define el bump (§2). El tag SemVer se crea sobre `main` para disparar el release (`pipeline-ci-cd_v1.0.md §1`).

## 5. Canales y semántica de prerelease

El proyecto no es redistribuible y no tiene feed de paquetes (`SOLUTION-INTAKE §17 P.7`); por lo tanto no aplica el modelo preview/stable sobre un feed. En su lugar, los canales se materializan como el tipo de release del repositorio, alineados con el modelo de ambientes reducido de `entornos-deploy_v1.0.md §1`.

| Canal | Tag | Semántica | Destino |
| --- | --- | --- | --- |
| Prerelease | `v<X.Y.Z>-alpha.N`, `-beta.N`, `-rc.N` | Versión candidata para validación; no productiva | Pre-producción (hardware equivalente o el mismo dispositivo en ventana de prueba) |
| Estable | `v<X.Y.Z>` sin sufijo | Versión productiva, gates completos, firmada y con SBOM | Producción (dispositivo objetivo) |

Semántica de sufijos: `-alpha.N` (en desarrollo, incompleta), `-beta.N` (completa, en validación), `-rc.N` (candidata final, sin defectos conocidos). La promoción de prerelease a estable sigue las promotion rules de `pipeline-ci-cd_v1.0.md §5`, con aprobación del implementador en su rol de release manager.

## 6. Deprecation policy

- Un parámetro de configuración o un descriptor obsoleto se marca como deprecado y vive al menos un MINOR antes de removerse, para no romper configuraciones existentes del operador. El descriptor (ADR-12, fuente única de verdad del parámetro) lleva la marca de deprecación y la leyenda que indica el reemplazo.
- Un cambio que rompa el contrato del archivo de entorno o de la variable de la clave maestra es MAJOR y se acompaña de una guía de migración en las release notes y el CHANGELOG, e instrucciones de preservación del archivo de entorno (ADR-07).
- Toda deprecación y todo breaking change se anuncian en el CHANGELOG (Keep a Changelog) y en el README del repositorio. Los obsoletos en código se marcan con la anotación de obsolescencia del runtime para que el análisis estático (gate G5) los visibilice.
- Al ser un operador único auto-hospedado, no hay consumidores externos a notificar; la comunicación es el CHANGELOG y las release notes, que el implementador consulta antes de actualizar el servicio.

## 7. Trazabilidad

- SemVer 2.0.0, Conventional Commits 1.0.0 y Keep a Changelog 1.1.0 provienen de `SOLUTION-INTAKE §17 P.7` y de la especialidad base AG-09.
- La herramienta de auto-versioning se ancla a `§17 P.7` con elección abierta a Sprint 0 (`§17 P.11`), sin hardcodear el producto.
- El branching GitHub Flow proviene de `§17 P.7`.
- El bump por commit alimenta el cálculo de versión del pipeline (`pipeline-ci-cd_v1.0.md §1`), que dispara la publicación del paquete (`guia-publicacion-paquete-self-contained-linux-arm_v1.0.md`).
- Los canales referencian el modelo de ambientes reducido de `entornos-deploy_v1.0.md §1`.
- Downstream: el README del repositorio (ADR-11) cita el flujo de versionado para el implementador; los samples de 11 referencian la nomenclatura de versiones declarada acá.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Estrategia de versionado inicial para `discord-bots-admin`: SemVer 2.0.0 con reglas de bump adaptadas a un proyecto sin API pública (MAJOR por contratos operativos e instalación), Conventional Commits 1.0.0, CHANGELOG por Keep a Changelog, herramienta de auto-versioning anclada a `§17 P.7` con elección abierta a Sprint 0, branching GitHub Flow, canales prerelease/estable como tipo de release (sin feed, no redistribuible) y deprecation policy sobre descriptores y contrato de entorno. |
