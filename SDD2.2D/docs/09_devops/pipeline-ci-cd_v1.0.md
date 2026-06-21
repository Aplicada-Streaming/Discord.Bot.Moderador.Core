# Pipeline CI/CD — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero DevOps Senior (AG-09) — variante DevOps + Deploy Engineer (web-monolith)

Este documento define el pipeline de integración y entrega continua del servicio monolítico de administración y moderación. El pipeline trata el build, la validación, el empaquetado, la firma, la publicación y el rollback del único artefacto del proyecto: un paquete self-contained para `linux-arm` (ARM de 32 bits, armv7l) que se instala como servicio gestionado por systemd, gobernado por ADR-05. La plataforma de integración continua y la herramienta de auto-versioning no se nombran en el cuerpo; se anclan por capacidad a `SOLUTION-INTAKE §17 P.8` (plataforma de CI) y `§17 P.7` (auto-versioning). Los quality gates del pipeline ejecutan, sin redefinirla, la Definition of Done de 08 (`definition-of-done_v1.0.md`) a través de los gates G1–G5 de `estrategia-calidad_v1.0.md §3`.

Divergencia respecto del default §2.2 de las reglas (web-monolith → `image-docker`): el cliente prohíbe contenedores (`SOLUTION-INTAKE §10`). El artefacto publicable no es una imagen de contenedor sino el paquete self-contained `linux-arm` instalable como servicio del sistema. La divergencia está gobernada por ADR-05 (despliegue self-contained ARM con servicio del sistema) y se documenta como decisión arquitectónica, no como omisión. El detalle del empaquetado vive en `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md`.

Distinción publicación vs despliegue (anti-patrón §4.8): este pipeline construye, valida, firma y publica el paquete (build/publicación). La instalación del paquete en el dispositivo objetivo y su registro como servicio systemd es el despliegue/instalación, descrito en `entornos-deploy_v1.0.md` y en la guía de publicación. No se confunden.

## 1. Triggers

Triggers explícitos por evento, para distinguir validación de PR de la entrega de release (anti-patrón §4.8 "trigger único y opaco"):

| Trigger | Evento | Stages que ejecuta | Resultado |
| --- | --- | --- | --- |
| PR a `main` | Apertura o actualización de un pull request hacia `main` | Lint, Build, Test (los tres niveles), SCA, SBOM, SAST | Gate bloqueante para mergear; no publica |
| Push a `main` (merge) | Merge de un PR a `main` | Lint, Build, Test, SCA, SBOM, SAST | Calcula la versión candidata (auto-versioning, `§17 P.7`); no publica artefacto de release |
| Tag de versión `v<X.Y.Z>` | Creación de un tag SemVer estable, sin sufijo | Build de release, Test, SCA, SBOM, Firma, Package, Publish (release) | Publica el paquete self-contained `linux-arm` como release |
| Tag de prerelease `v<X.Y.Z>-rc.N` (y `-alpha.N`, `-beta.N`) | Creación de un tag SemVer con sufijo de prerelease | Build de release, Test, SCA, SBOM, Firma, Package, Publish (prerelease) | Publica el paquete como release de prerelease para validación en pre-producción |
| Schedule (diario) | Programado | SCA, dependency scanning | Re-escaneo de dependencias del último release publicado; alerta por CVE nueva (ver `supply-chain-seguridad_v1.0.md`) |

GitHub Flow (`estrategia-versionado_v1.0.md §4`): trunk `main` protegido con ramas de feature de vida corta; el merge a `main` requiere PR con los gates G1–G5 en verde.

## 2. Stages obligatorios

Cada stage declara su comando representativo, su tooling por capacidad y su criterio de éxito. El producto concreto de cada herramienta vive en `SOLUTION-INTAKE §17 P.6` (frameworks de test, cobertura) y `§17 P.8` (compilador, formato, análisis estático, plataforma de CI); el cuerpo no lo hardcodea. Todos los comandos son reproducibles localmente (anti-patrón §4.8 "pipeline irreproducible localmente"); su equivalente local se documenta en el README del repositorio (categoría 10 colapsada en READMEs, ADR-11).

| Stage | Tooling (por capacidad) | Comando representativo | Criterio de éxito (gate) | Bloqueante | DoD/NFR que verifica |
| --- | --- | --- | --- | --- | --- |
| STAGE-01 Lint / formato | Verificador de formato del runtime (`§17 P.8`) | `dotnet format --verify-no-changes` | Sin diferencias con el formato canónico | Sí en PR | Gate G4 (`estrategia-calidad §3`); DoD US/BT "formato sin diferencias" |
| STAGE-02 Build | Compilador del runtime en configuración release (`§17 P.8`) | `dotnet build -c Release` | 0 errores de compilación | Sí | Gate G1; DoD US/BT/release "compila sin errores" |
| STAGE-03 Análisis estático | Analizador estático del runtime (`§17 P.8`) | `dotnet build -c Release -warnaserror` sobre analizadores | Sin warnings nuevos respecto de la rama base | Sí | Gate G5; DoD US/BT/release "análisis estático sin warnings nuevos" |
| STAGE-04 Test unitario | Framework unitario + aserciones fluidas + dobles (`§17 P.6`) | `dotnet test --filter Category=Unit` | Suite unitaria 100 % verde | Sí | Gate G2; pirámide 08 §1 nivel Unit (70 %) |
| STAGE-05 Test integración | Factory de aplicación web + base relacional embebida en archivo (`§17 P.6`) | `dotnet test --filter Category=Integration` | Suite de integración 100 % verde | Sí | Gate G2; pirámide 08 §1 nivel Integration (20 %) |
| STAGE-06 Test e2e crítico | Conducción headless del navegador (`§17 P.6`) | `dotnet test --filter Category=E2E` | E2E crítica del panel 100 % verde | Sí | Gate G2; pirámide 08 §1 nivel E2E (10 %) |
| STAGE-07 Cobertura | Recolector de cobertura por capa (`§17 P.6`) | `dotnet test --collect:"XPlat Code Coverage"` + reporte por capa | Global líneas ≥ 75 %, branches ≥ 65 %; módulo de detección ≥ 90 % líneas; por capa según `estrategia-testing §2` | Sí | Gate G3; NFR "Cobertura de tests del módulo de detección" (05 §8) |
| STAGE-08 SCA | Herramienta de composition analysis (`supply-chain-seguridad §4`) | Escaneo de dependencias y vulnerabilidades | 0 CVE críticas, 0 altas sin excepción con ADR | Sí | NFR seguridad (05 §8 — token cifrado, hash del administrador); política de CVE de `supply-chain-seguridad §6` |
| STAGE-09 SBOM | Generador CycloneDX o SPDX (`supply-chain-seguridad §1`) | Generación del SBOM en JSON | SBOM generado y adjunto al artefacto | Sí (en release) | OWASP SCVS; `supply-chain-seguridad §1` |
| STAGE-10 Firma | Herramienta de firma con transparency log (`supply-chain-seguridad §2`) | Firma del paquete y del SBOM | Firma válida y registrada en transparency log | Sí (en release) | SLSA; `supply-chain-seguridad §2/§3` |
| STAGE-11 Package | Publicación self-contained por cross-compile (`guia-publicacion §2`) | `dotnet publish -c Release -r linux-arm --self-contained true` + empaquetado | Paquete self-contained `linux-arm` generado con checksum | Sí (en release) | ADR-05; DoD release "artefacto self-contained se publica" |
| STAGE-12 Publish | Adjunto del paquete al release del repositorio (`guia-publicacion §2`) | Subida del paquete + SBOM + firma + checksum al release | Artefacto disponible y consumible en el canal de release | Sí (en release) | ADR-05; DoD release |

Notas de stages:

- La compilación cruzada (cross-compile) a `linux-arm` se ejecuta en STAGE-11 desde un runner x64, sin necesidad de un runner ARM (`SOLUTION-INTAKE §17 P.8`, ADR-05). El runtime se incluye en el paquete (self-contained), por lo que el dispositivo no necesita un runtime instalado.
- STAGE-04..STAGE-06 representan los tres niveles de la pirámide de 08 §1 (unit, integration, e2e), uno por nivel, ejecutando exactamente la suite de `casos-prueba-referenciales_v1.0.md`. La cobertura de STAGE-07 ejecuta el gate G3 que reconcilia el piso global con la cobertura por capa de `estrategia-testing_v1.0.md §2`.
- Los gates no se redefinen acá: STAGE-01..STAGE-07 ejecutan literalmente G1–G5 de 08; su excepción solo se admite con ADR explícita y BT de remediación (`definition-of-done §2`, `estrategia-calidad §3`).

## 3. Matriz de sistema operativo y runtime

| Trigger | Sistema operativo del runner | Runtime de build | Arquitectura de salida | Justificación |
| --- | --- | --- | --- | --- |
| PR a `main`, push a `main` | Linux x64 | .NET 10 | — (build y test, sin paquete) | El build y la suite de tests corren en el runner x64 estándar; la base de integración es embebida en archivo, sin contenedores (`estrategia-testing §7`), por lo que no requiere runner especial |
| Tag de release / prerelease | Linux x64 (runner de build) | .NET 10 | `linux-arm` (armv7l) self-contained por cross-compile | El paquete objetivo es `linux-arm`, pero se compila cruzado desde x64 (`§17 P.8`, ADR-05); evita mantener un runner ARM, tier deprioritizado de la plataforma (`§17 P.12`) |
| Verificación post-publish | Dispositivo de referencia ARM (armv7l) o equivalente | Runtime incluido en el paquete (self-contained) | — (ejecuta el paquete) | La validación de arranque del servicio y la medición de NFR de latencia/throughput/memoria se hacen sobre el hardware real (05 §8), no sobre el runner de build |

Justificación de la matriz (cobertura de consumidores reales vs costo de minutos de CI): el único destino productivo es `linux-arm` sobre Raspbian de 32 bits (`§17 P.9`). No se construye una matriz cruzada de SO de destino porque hay un solo target soportado; ARMv6 y arquitecturas no listadas quedan fuera (`§17 P.9`). El build x64 + cross-compile es la opción de menor costo coherente con ADR-05.

## 4. Caché y artefactos

Caché:

| Caché | Llave | Expiración | Propósito |
| --- | --- | --- | --- |
| Paquetes del gestor de dependencias del runtime | Hash de los archivos de proyecto y de bloqueo de dependencias | 7 días sin uso o cambio de llave | Acelerar restore de dependencias entre corridas |
| Salida de build incremental | Rama + hash de fuentes | Por corrida; no compartido entre ramas de release | Acelerar build de PR |

Artefactos producidos y su retención:

| Artefacto | Stage productor | Retención | Adjunto al release |
| --- | --- | --- | --- |
| Reporte de cobertura por capa | STAGE-07 | 30 días | No (visible en el resumen de la corrida) |
| Resultados de SCA y dependency scanning | STAGE-08 | 90 días | No |
| SBOM (CycloneDX/SPDX JSON) | STAGE-09 | Permanente (release) | Sí |
| Firma del paquete y del SBOM | STAGE-10 | Permanente (release) | Sí |
| Paquete self-contained `linux-arm` (zip) | STAGE-11 | Permanente (release) | Sí |
| Checksum del paquete | STAGE-11 | Permanente (release) | Sí |

## 5. Promotion rules entre el modelo de ambientes reducido

El modelo de ambientes es reducido y auto-hospedado (ver `entornos-deploy_v1.0.md §1`), justificado por `SOLUTION-INTAKE §10` y ADR-05: no es la escalera cloud DEV/QA/STAGING/PROD. Las transiciones promueven el mismo paquete a través de tres etapas.

| Transición | Trigger | Prerequisitos | Aprobador |
| --- | --- | --- | --- |
| Desarrollo local → Pre-producción (validación) | Tag de prerelease `v<X.Y.Z>-rc.N` | Gates G1–G5 en verde; SBOM y firma presentes; build de release reproducible | Auto (el implementador genera el tag) |
| Pre-producción → Producción (dispositivo objetivo) | Tag de release `v<X.Y.Z>` sin sufijo | Validación en pre-producción superada; instalación de prueba y arranque del servicio verificados; NFR de latencia/throughput/memoria medidos sobre hardware equivalente dentro de SLA o excepción con ADR (DoD release; 05 §8) | Release manager (rol que ejerce el implementador único, `estrategia-calidad §4`) |

Para producción, la promoción exige aprobación humana explícita (el implementador en su rol de release manager) y queda registrada por el tag y el release publicado, satisfaciendo el anti-patrón §4.8 "promotion sin aprobador humano para PROD". Dado que el operador es único (`estrategia-calidad §4`), el aprobador y el ejecutor coinciden; el registro auditable es el tag SemVer y las release notes.

## 6. Rollback

Rollback por tipo de artefacto (paquete self-contained `linux-arm` instalado como servicio systemd). El procedimiento de pipeline marca la versión rota y reinstala la publicación anterior conservando el archivo de entorno y la clave maestra, de modo que los tokens cifrados sigan siendo válidos (`SOLUTION-INTAKE §17 P.8`, ADR-05, ADR-07). Los comandos `gh release` invocan la CLI de la plataforma de CI declarada en `SOLUTION-INTAKE §17 P.8`; se conservan como comandos concretos. Comandos concretos:

1. Identificar la última publicación estable previa al release roto:
   `gh release list` y seleccionar el tag `v<X.Y.Z>` anterior.
2. Descargar el paquete previo y verificar su integridad antes de reinstalar:
   `gh release download v<X.Y.Z-previa>` y comprobar checksum y firma (ver `supply-chain-seguridad §2`).
3. Detener el servicio en el dispositivo:
   `sudo systemctl stop discord-moderador-bot`.
4. Reinstalar la publicación previa preservando el archivo de entorno y la clave maestra (no se sobrescribe el archivo de entorno):
   `sudo ./scripts/servicio/instalar.sh --paquete <paquete-previo>.zip --conservar-entorno`.
5. Arrancar el servicio y verificar:
   `sudo systemctl start discord-moderador-bot && systemctl status discord-moderador-bot`.
6. Verificar que el servicio opera sin re-registrar tokens (la clave maestra preservada mantiene válidos los tokens cifrados; ADR-07 §8).

El detalle operativo del rollback (con ventana de gracia y comunicación) vive en `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §4`. El rollback de la publicación no implica restaurar datos; la base SQLite del dispositivo no se toca. Si el defecto es de datos o de esquema, se trata por migración, no por rollback de paquete.

## 7. Notificaciones

| Evento | Canal | Severidad | Escalamiento |
| --- | --- | --- | --- |
| Falla de un gate bloqueante en PR (G1–G5) | Resumen del PR + notificación al autor | Media | Bloquea el merge hasta resolución |
| Falla de build o de tests en `main` | Notificación al implementador | Alta | Revisión inmediata; no se promueve a release |
| CVE crítica o alta detectada (SCA / schedule) | Notificación al implementador + registro en el seguimiento | Alta/Crítica | SLA de remediación de `supply-chain-seguridad §6` |
| Publicación de release exitosa | Release notes + CHANGELOG generado | Informativa | — |
| Falla de publicación o de firma en release | Notificación al implementador | Alta | Re-ejecución del stage; si persiste, no se promueve |

Dashboards visibles al equipo (operador único): el resumen de cada corrida con el estado por gate, el reporte de cobertura por capa y el listado de releases con su SBOM y firma. Al ser un único desarrollador (`estrategia-calidad §4`), las notificaciones convergen en el implementador, que ejerce los roles de autor, QA y release manager.

## 8. Trazabilidad

- Cada gate del pipeline (STAGE-01..STAGE-07) referencia el gate G1–G5 de `estrategia-calidad_v1.0.md §3`, que a su vez materializa la DoD de `definition-of-done_v1.0.md`. La DoD no se redefine acá.
- STAGE-07 (cobertura) verifica el NFR "Cobertura de tests del módulo de detección ≥ 90 %; global ≥ 75/65" de `arquitectura-solucion_v1.0.md §8`.
- Los NFR numéricos de latencia (p95 < 200 ms), throughput (≥ 50 mensajes/s) y memoria (≤ 8 MB por conexión) se verifican en la promoción a producción sobre hardware equivalente (sección 5; DoD release; 05 §8). El NFR de disponibilidad (99 % mensual) se sostiene por el reinicio automático del servicio systemd (ADR-05) y se mide en operación, no en el pipeline.
- El rollback referencia ADR-05 (reinstalación de la publicación previa) y ADR-07 (preservación de la clave maestra y validez de los tokens cifrados).
- Downstream: el README del repositorio (categoría 10 colapsada, ADR-11) cita los comandos locales equivalentes a estos stages; los samples de 11 referencian el artefacto y los canales declarados acá y en `entornos-deploy_v1.0.md`.

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Pipeline CI/CD inicial para `discord-bots-admin`: triggers por evento, doce stages (lint/formato, build, análisis estático, test por nivel, cobertura, SCA, SBOM, firma, package y publish), matriz x64 build + cross-compile a `linux-arm`, caché y artefactos, promotion rules sobre el modelo de ambientes reducido, rollback por reinstalación preservando clave maestra (ADR-05/ADR-07) y notificaciones. Gates G1–G5 de 08 ejecutados sin redefinir. Divergencia respecto del default image-docker gobernada por ADR-05. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: anclaje del comando `gh release` a la CLI de la plataforma de CI declarada en `SOLUTION-INTAKE §17 P.8` en su primera aparición, conservando los comandos concretos. |
