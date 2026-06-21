# Supply chain y seguridad — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero DevOps Senior (AG-09) — variante DevOps + Deploy Engineer (web-monolith)

Este documento define la política de cadena de suministro del servicio: SBOM, firma del artefacto, nivel SLSA objetivo, dependency scanning, SAST/DAST y política de CVE. Se alinea con SLSA, NIST SSDF (SP 800-218) y OWASP SCVS (especialidad base AG-09). El artefacto protegido es el paquete self-contained `linux-arm` instalable como servicio (ADR-05). El refuerzo de compliance proviene de ADR-06 (Ley 25.326): residencia local de los datos personales, minimización y retención acotada.

## 1. SBOM

| Aspecto | Decisión |
| --- | --- |
| Formato | CycloneDX o SPDX (estándar abierto de SBOM); elección puntual al adoptar el generador en Sprint 0 |
| Generador | Herramienta de generación de SBOM por capacidad, integrada en STAGE-09 de `pipeline-ci-cd_v1.0.md §2` |
| Formato de salida | JSON |
| Publicación | Adjunto a cada release del repositorio, junto al paquete (`guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §2`) |
| Firma del SBOM | El propio SBOM se firma (sección 2), para que su integridad sea verificable |

El SBOM inventaría las dependencias del paquete self-contained (incluido el runtime embebido) y es la base para responder ante una CVE de dependencias (anti-patrón §4.8 "falta de SBOM"). La generación es automática en el pipeline (criterio §5.4 de las reglas).

## 2. Firma

| Aspecto | Decisión |
| --- | --- |
| Mecanismo | Firma del artefacto con una herramienta de firma con transparency log (capacidad tipo sigstore/cosign u homólogo); STAGE-10 de `pipeline-ci-cd_v1.0.md §2` |
| Objetos firmados | El paquete self-contained `linux-arm` y el SBOM |
| Transparency log | La firma se registra en un transparency log para auditabilidad |
| Verificación | Antes de instalar o de hacer rollback, el operador verifica firma y checksum (`guia-publicacion §3`, `§4`) |

La firma se ejecuta en el stage final, antes del publish, y se verifica antes de instalar (criterio §5.4 de las reglas; anti-patrón §4.8 "falta de firma del artefacto"). El consumidor del artefacto es el propio operador, que verifica autoría e integridad del paquete que instala en su dispositivo.

## 3. SLSA

Nivel SLSA objetivo y plan de elevación, proporcionado a un proyecto auto-hospedado de un único operador:

| Nivel | Estado | Criterios |
| --- | --- | --- |
| SLSA L1 | Objetivo v1 (cumplido por diseño) | Proceso de build con script versionado; el build se ejecuta en el pipeline (`pipeline-ci-cd_v1.0.md`); procedencia básica: el release indica de qué commit y tag proviene |
| SLSA L2 | Objetivo de elevación | Build en servicio de CI gestionado (no en la estación local) con generación de procedencia firmada del artefacto; firma en transparency log (sección 2) |
| SLSA L3 | Futuro (no v1) | Build aislado y procedencia no falsificable; requiere endurecer el runner; se evalúa cuando el riesgo lo justifique |
| SLSA L4 | No previsto | Desproporcionado para un operador único |

Plan de elevación: v1 fija L1 como piso y apunta a L2 mediante la firma con transparency log y la procedencia firmada del release. La elevación a L3 queda como mejora futura, sin bloquear v1.

## 4. Dependency scanning

| Aspecto | Decisión |
| --- | --- |
| Tooling | Herramienta de SCA por capacidad (STAGE-08 de `pipeline-ci-cd_v1.0.md §2`) + actualizador automático de dependencias (capacidad tipo Dependabot/Renovate u homólogo) |
| Frecuencia | En cada PR y push a `main` (STAGE-08); re-escaneo programado diario del último release publicado (schedule de `pipeline-ci-cd_v1.0.md §1`) |
| Política por severidad | Crítica: bloquea el merge y el release, remediación inmediata. Alta: bloquea sin excepción documentada con ADR. Media: no bloquea; se agenda en el backlog de 06. Baja: se registra; se atiende oportunamente |
| Integración | El actualizador automático abre PRs de actualización de dependencias, que pasan por los gates G1–G5 antes de mergear |

La política por severidad de dependency scanning es la misma que la de CVE de la sección 6; la diferencia es el origen (escaneo de dependencias vs CVE publicada). Una excepción a "0 altas" solo se admite con ADR explícita y BT de remediación (coherente con la política de excepciones de `estrategia-calidad_v1.0.md §3`).

## 5. SAST y DAST

| Análisis | Herramienta (por capacidad) | Stage del pipeline | Criterio de bloqueo |
| --- | --- | --- | --- |
| SAST (análisis estático de seguridad) | Analizador estático del runtime con reglas de seguridad (STAGE-03, complementado en STAGE-08) | En PR y en `main` | Hallazgo de seguridad crítico o alto nuevo bloquea el merge (gate G5 más reglas de seguridad) |
| DAST (análisis dinámico) | Conducción del panel en una instancia local con datos sintéticos | Sobre el panel en pre-producción (alcance acotado) | Vulnerabilidad explotable confirmada en el panel bloquea la promoción a producción |

Alcance acotado del DAST: el sistema no expone una API ni una superficie pública a terceros (`SOLUTION-INTAKE §17 P.3`, ADR-11); la única superficie expuesta es el panel de administración del operador único. El DAST se limita a esa superficie, sobre una instancia local con datos sintéticos (coherente con el ambiente e2e de `estrategia-testing_v1.0.md §7`). No hay endpoints públicos que sondear, lo que reduce proporcionalmente el alcance del DAST.

## 6. Política de CVE

| Severidad | SLA de remediación | Acción |
| --- | --- | --- |
| Crítica | Inmediata (release de fix prioritario) | Bloquea release; PATCH o MINOR con el fix; rollback si ya está en producción (`guia-publicacion §4`) |
| Alta | Ventana corta acordada en el backlog | Bloquea release sin excepción con ADR; fix planificado con prioridad |
| Media | Ventana media | Se agenda como BT en el backlog de 06; no bloquea |
| Baja | Oportunista | Se registra; se atiende en una actualización rutinaria |

Comunicación: al ser un operador único auto-hospedado, no hay consumidores externos a notificar; la comunicación es el CHANGELOG (Keep a Changelog) y las release notes del release de fix, que el operador consulta antes de actualizar el servicio. Ventana entre detección y publicación de fix: para crítica y alta se publica el fix con prioridad y se ejecuta el rollback si el riesgo en producción lo exige.

Refuerzo por compliance (ADR-06, Ley 25.326): la cadena de suministro protege también los datos personales tratados (identificadores de usuario, copias de mensajes de incidentes). La minimización y la retención acotada reducen la superficie expuesta ante una vulnerabilidad; la residencia local sin terceros (ADR-06) implica que no hay transferencia de datos a servicios externos cuya cadena de suministro debiera auditarse aparte. El cifrado del token en reposo (ADR-07) y el hash de la credencial del administrador (ADR-03) limitan el impacto de una eventual filtración. El escaneo de secretos en commits (sección 4 de `entornos-deploy_v1.0.md`; anti-patrón §4.8 "secretos en commit") evita exponer la clave maestra o los tokens en la historia de Git.

## 7. Trazabilidad

- SBOM, firma, SLSA, dependency scanning, SAST/DAST y política de CVE responden a la estructura §4.6 de las reglas y a SLSA / NIST SSDF / OWASP SCVS de la especialidad base AG-09.
- Los stages de supply chain (SCA, SBOM, firma) referencian STAGE-08..STAGE-10 de `pipeline-ci-cd_v1.0.md §2`.
- La verificación de firma y checksum referencia `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §3/§4`.
- El refuerzo de compliance referencia ADR-06 (Ley 25.326), ADR-07 (cifrado de token) y ADR-03 (hash de credencial).
- La política de excepciones por ADR es coherente con `estrategia-calidad_v1.0.md §3` y `definition-of-done_v1.0.md §2`.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Supply chain y seguridad inicial para `discord-bots-admin`: SBOM CycloneDX/SPDX JSON adjunto y firmado; firma del paquete y del SBOM con transparency log; SLSA L1 piso con plan de elevación a L2; dependency scanning con política por severidad e integración de actualizador automático; SAST en pipeline y DAST acotado al panel (sin API pública, ADR-11); política de CVE con SLA por severidad y comunicación por CHANGELOG; refuerzo de compliance por ADR-06/ADR-07/ADR-03. |
