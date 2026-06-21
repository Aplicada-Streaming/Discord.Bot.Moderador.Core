# 11 Examples — discord-bots-admin

**Proyecto:** discord-bots-admin
**Sección:** 11_examples
**Variante aplicada:** Sample Engineer (tipo de proyecto web-monolith; samples de capacidad sobre el proyecto único `DiscordModeradorBot.Servicio`)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11)

---

## 1. Propósito de la carpeta

Esta carpeta es la especificación de los samples ejecutables del servicio monolítico de administración y moderación (`DiscordModeradorBot.Servicio`). En `docs/11_examples/` vive la documentación explicativa de cada sample: objetivo, nivel, prerequisites, comandos de arranque, estructura de código planificada, output esperado, variaciones y trazabilidad a los casos de uso, ADR y NFR upstream.

El código ejecutable de cada sample vive en `/samples/<carpeta>/` en la raíz del repositorio (`SOLUTION-INTAKE §16`, §16.1). Hay correspondencia uno a uno entre cada `ejemplo-XX-<kebab>_v1.0.md` de esta carpeta y una carpeta ejecutable de `/samples/`. Cada sample es autocontenido, reproducible en cinco pasos o menos, y ejercita partes reales de `/src`.

Caso degenerado de layout aplanado: la solución es un único proyecto (`SOLUTION-INTAKE §13`), por lo que los markdown viven en `docs/11_examples/` y no bajo `proyectos/<kebab>/11_examples/`.

### 1.1 Estado de materialización

Esta sección se redacta en la etapa de documentación. La carpeta `/src` aún no existe; la generación de código (incluido el código ejecutable de `/samples`) es posterior al handoff de la cadena SDD. En consecuencia:

- Los markdown de esta carpeta son la especificación vigente y completa de cada sample (los nueve apartados obligatorios por sample).
- La estructura de `/samples/XX-<kebab>/` que describe cada markdown es planificada: documenta el árbol de archivos, los comandos de arranque y el output esperado como contrato, no como código ya presente.
- El código ejecutable de `/samples`, sus tests de verificación y el job de integración continua que los compila y corre se materializan en la fase de codificación (post-handoff), conforme a `SOLUTION-INTAKE §16.1`. Hasta entonces figuran como planificados.

## 2. Tabla maestra de samples

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| [ejemplo-01-basico-conexion-gateway_v1.0.md](ejemplo-01-basico-conexion-gateway_v1.0.md) | Básico | < 5 min | CU-01 (ingreso de mensajes al pipeline, previo a la evaluación de la regla: pasos 1 y 3 del flujo, sin evaluar) | `/samples/01-basico-conexion-gateway/` |
| [ejemplo-02-intermedio-configuracion-por-descriptores_v1.0.md](ejemplo-02-intermedio-configuracion-por-descriptores_v1.0.md) | Intermedio | 10-15 min | CU-11, CU-15 | `/samples/02-intermedio-configuracion-por-descriptores/` |
| [ejemplo-03-avanzado-deteccion-rafaga_v1.0.md](ejemplo-03-avanzado-deteccion-rafaga_v1.0.md) | Avanzado | 15-25 min | CU-01, CU-02, CU-14 | `/samples/03-avanzado-deteccion-rafaga/` |

La progresión es por nivel con descriptor de capacidad en el slug, de menor a mayor complejidad: sample 01 establece el ingreso de mensajes al pipeline (capa de integración con el canal de eventos); sample 02 agrega la capa de configuración dirigida por descriptores que el administrador opera desde el panel; sample 03 ejercita el núcleo del producto, la detección de ráfaga distribuida con su contención y su modo simulación. El slug nombra la capacidad demostrada (conexión al gateway, configuración por descriptores, detección de ráfaga), nunca una entidad del dominio fuente.

## 3. Convenciones de los samples

- Autocontenidos: cada sample arranca en un entorno limpio sin más dependencias que las declaradas en sus prerequisites.
- Ejecutables en cinco pasos o menos: cada markdown documenta el camino a la primera ejecución exitosa en un máximo de cinco pasos copiables.
- Trazabilidad obligatoria: cada sample declara en su §8 al menos un caso de uso, ADR, NFR o componente arquitectónico que ilustra, con enlace al artefacto upstream.
- Nivel declarado: cada sample declara su nivel (básico, intermedio o avanzado) en su §2, justificado respecto del sample anterior.
- Sin credenciales reales en los samples 01 y 03: ambos operan con un sustrato de mensajes simulados que no requiere conectar contra la plataforma real; el sample 02 opera enteramente sobre el panel local. Esto los hace reproducibles en CI sin un token de producción.
- Output esperado documentado: cada sample documenta en su §6 el output exacto que el desarrollador verá.

## 4. Cómo agregar un sample nuevo

1. Elegí el siguiente número correlativo y un slug de capacidad en kebab-case que describa qué demuestra respecto del anterior (nunca una entidad del dominio).
2. Copiá la estructura de los nueve apartados obligatorios de un markdown existente (cabecera de metadatos más §1 a §9) y completala.
3. Declará el nivel en §2 y la trazabilidad a CU/ADR/NFR en §8 con al menos una fila.
4. Documentá la estructura planificada de `/samples/XX-<kebab>/`, los comandos de arranque (≤ 5 pasos) y el output esperado.
5. Agregá la fila correspondiente a la tabla maestra de §2 de este README.

El procedimiento y los criterios de aceptación viven en `SDD2.2D/devs/rules/11_rules_examples.md §4` y §6.

## 5. Vínculo con la arquitectura y con la disciplina DevOps

La categoría 10 (developer guide) está colapsada en READMEs por decisión arquitectónica (ADR-11): no hay una guía de desarrollador independiente. Los conceptos que cada sample materializa se consultan en la arquitectura de 05 y en el README de 09:

- Arquitectura de la solución y vista lógica (componentes, pipeline, firewall multi-contexto): `../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md`.
- Puntos de extensión (descriptores, tipos de regla, tipos de acción): `../05_arquitectura_tecnica/extensibilidad_v1.0.md`.
- Flujo de ejecución del pipeline de moderación: `../05_arquitectura_tecnica/flujo-ejecucion_v1.0.md`.
- Artefacto de release y canales declarados (paquete self-contained para `linux-arm`, gates del pipeline): `../09_devops/README.md`.

## 6. Tipo de proyecto y estructura de `/samples`

La estructura de `/samples` se deriva del tipo de proyecto y se ajusta a los tres samples de capacidad que el intake materializa (`SOLUTION-INTAKE §16.1, §18`).

| Tipo D8 | Estructura de `/samples` (ajustada al intake) |
| --- | --- |
| web-monolith (este proyecto) | `01-basico-conexion-gateway/`, `02-intermedio-configuracion-por-descriptores/`, `03-avanzado-deteccion-rafaga/` |
| (resto) | Ver §2.3 de `SDD2.2D/devs/rules/11_rules_examples.md`. |

Nota sobre la matriz §2.3: el default genérico de `web-monolith` (`01-datos-seed/`, `02-tema-custom/`) se reemplaza por estos tres samples de capacidad porque el intake materializa `/samples` con ellos (`SOLUTION-INTAKE §16.1, §18`). Los tres superan el mínimo de dos samples de `web-monolith` (§2.2 de las reglas) y nombran capacidades demostradas, no entidades del dominio.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Índice inicial de la sección 11. Tabla maestra de los tres samples de capacidad (conexión al gateway, configuración por descriptores, detección de ráfaga), convenciones, procedimiento de alta y vínculo con 05 y 09. Estado de materialización declarado: los markdown son la especificación; el código ejecutable de `/samples` y su CI se materializan en la fase de codificación (`SOLUTION-INTAKE §16.1`). |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: en la tabla maestra de §2, la etiqueta de CU de E1 se hace consistente listando el identificador CU-01 y aclarando que el sample ejercita el ingreso previo a la evaluación de la regla. |
