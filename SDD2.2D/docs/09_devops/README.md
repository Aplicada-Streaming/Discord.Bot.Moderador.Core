# 09 DevOps — discord-bots-admin

**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Versión de la sección:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero DevOps Senior (AG-09) — variante DevOps + Deploy Engineer (web-monolith)

Punto de entrada navegable de la disciplina DevOps del servicio monolítico de administración y moderación: pipeline CI/CD, versionado, ambientes y despliegue, guía de publicación del artefacto y política de cadena de suministro. Recibe upstream de 05 (arquitectura, NFR §8, ADR-05/06/07/11) y de 08 (DoD canónica, quality gates G1–G5, cobertura por capa). Alimenta a 10 (colapsada en READMEs por ADR-11; la guía de instalación local se resume acá y en la guía de publicación) y a 11 (los samples referencian el artefacto y los canales declarados acá).

## Tipo de artefacto y modelo de ambientes (resolución de divergencias §2.2)

- Tipo de artefacto: paquete self-contained para `linux-arm` (ARM de 32 bits, armv7l) instalable como servicio systemd, familia `paquete-` del patrón §3.1. No es `image-docker` (el cliente prohíbe contenedores, `SOLUTION-INTAKE §10`). Divergencia respecto del default §2.2 de web-monolith gobernada por ADR-05. No hay feed de paquetes: el proyecto no es redistribuible (`§17 P.7`); la distribución es el paquete instalable publicado como release.
- Modelo de ambientes: reducido y auto-hospedado en un único dispositivo (`§10`, ADR-05), no la escalera cloud DEV/QA/STAGING/PROD. Tres etapas: desarrollo local (x64), pre-producción opcional (hardware equivalente o el mismo dispositivo) y producción (dispositivo objetivo).
- `pipeline-solucion` ausente: la solución tiene un único proyecto (caso degenerado, `SOLUTION-MANIFEST §3`); el artefacto de nivel solución se omite correctamente.

## Documentos de la sección

| Documento | Estado | Contenido |
| --- | --- | --- |
| [pipeline-ci-cd_v1.0.md](pipeline-ci-cd_v1.0.md) | Propuesto | Triggers por evento; doce stages (lint/formato, build, análisis estático, test por nivel, cobertura, SCA, SBOM, firma, package, publish) con cross-compile a `linux-arm`; matriz SO/runtime; caché y artefactos; promotion rules; rollback; notificaciones. Ejecuta los gates G1–G5 de 08 sin redefinirlos. |
| [estrategia-versionado_v1.0.md](estrategia-versionado_v1.0.md) | Propuesto | SemVer 2.0.0; Conventional Commits 1.0.0; CHANGELOG por Keep a Changelog; herramienta de auto-versioning anclada a `§17 P.7` (abierta a Sprint 0); branching GitHub Flow; canales prerelease/estable; deprecation policy. |
| [entornos-deploy_v1.0.md](entornos-deploy_v1.0.md) | Propuesto | Modelo de ambientes reducido justificado; IaC mínima como scripts versionados; configuración 12-factor; secretos (clave maestra, token cifrado, credencial); promoción/instalación en el dispositivo. |
| [guia-publicacion-paquete-self-contained-linux-arm_v1.0.md](guia-publicacion-paquete-self-contained-linux-arm_v1.0.md) | Propuesto | Pre-requisitos; comando/stage de publicación por cross-compile self-contained; verificación post-publish (integridad, instalación, arranque, NFR); rollback preservando entorno y clave; métricas. |
| [supply-chain-seguridad_v1.0.md](supply-chain-seguridad_v1.0.md) | Propuesto | SBOM (CycloneDX/SPDX); firma con transparency log; SLSA L1 con plan a L2; dependency scanning con política por severidad; SAST/DAST acotado; política de CVE con SLA; refuerzo por compliance (ADR-06). |

## Orden de lectura sugerido

1. `estrategia-versionado_v1.0.md` — frontera entre código y artefacto (SemVer, Conventional Commits, branching).
2. `pipeline-ci-cd_v1.0.md` — stages, gates G1–G5, cross-compile, rollback.
3. `entornos-deploy_v1.0.md` — modelo de ambientes reducido, configuración y secretos.
4. `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md` — empaquetado, publicación, instalación y rollback del artefacto.
5. `supply-chain-seguridad_v1.0.md` — SBOM, firma, SLSA, scanning, SAST/DAST, CVE.

## Quality gates como stages

Los quality gates del pipeline ejecutan la DoD de 08 (`definition-of-done_v1.0.md`) a través de los gates G1–G5 de `estrategia-calidad_v1.0.md §3`, sin redefinirlos:

| Gate | Stage del pipeline | Criterio |
| --- | --- | --- |
| G1 Compilación | STAGE-02 Build | Build sin errores |
| G2 Tests en verde | STAGE-04/05/06 (unit/integración/e2e) | Suite 100 % verde por nivel de la pirámide de 08 |
| G3 Cobertura por capa | STAGE-07 Cobertura | Global líneas ≥ 75 %, branches ≥ 65 %; módulo de detección ≥ 90 % líneas |
| G4 Formato | STAGE-01 Lint/formato | Formato canónico sin diferencias |
| G5 Análisis estático | STAGE-03 Análisis estático | Sin warnings nuevos respecto de la rama base |

## NFR de 05 §8 con stage o gate que los verifica

| NFR (05 §8) | Objetivo | Dónde se verifica |
| --- | --- | --- |
| Latencia de procesamiento por mensaje | p95 < 200 ms | Pre-producción sobre hardware real, antes de promover (`pipeline-ci-cd §5`, DoD release) |
| Throughput sostenido | ≥ 50 mensajes/s | Pre-producción sobre hardware real (banco de carga) |
| Disponibilidad mensual (SLO) | 99 % mensual | Operación; sostenido por reinicio automático del servicio systemd (ADR-05); medido del journal |
| Memoria por conexión de gateway activa | ≤ 8 MB | Pre-producción sobre hardware real (perfilado) |
| Cobertura del módulo de detección | ≥ 90 % líneas; global ≥ 75/65 | STAGE-07 (gate G3) en cada PR/build |
| Limpieza efectiva de la ráfaga | ≥ 98 % en 10 s | Suite de 08 (tests de incidente); verificada en validación |

## Trazabilidad

- Upstream: 05 (`arquitectura-solucion_v1.0.md §8`, ADR-05/06/07/11), 08 (`definition-of-done_v1.0.md`, `estrategia-calidad_v1.0.md §3`, `estrategia-testing_v1.0.md §1/§2`).
- Downstream: 10 colapsada en READMEs (ADR-11); 11 (samples referencian artefacto y canales).
- `pipeline-solucion` ausente por caso degenerado (un único proyecto).
