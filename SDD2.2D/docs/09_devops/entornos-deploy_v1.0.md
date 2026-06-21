# Entornos y despliegue — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero DevOps Senior (AG-09) — variante DevOps + Deploy Engineer (web-monolith)

Este documento declara el modelo de ambientes del servicio, su provisión, su configuración por ambiente, sus secretos y su procedimiento de instalación/promoción. El despliegue es auto-hospedado en un único dispositivo de bajo consumo (`SOLUTION-INTAKE §10`), gobernado por ADR-05 (despliegue self-contained ARM con servicio del sistema). Distingue publicación (build del paquete, en `pipeline-ci-cd_v1.0.md`) de despliegue/instalación (registro del servicio en el dispositivo), conforme al anti-patrón §4.8 "confundir publicación con despliegue".

## 1. Modelo de ambientes (reducido y justificado)

Divergencia respecto del default §2.2 de las reglas (web-monolith → DEV/QA/STAGING/PROD): el despliegue no es una escalera cloud de cuatro ambientes con promoción aprobada en infraestructura gestionada. Es un servicio auto-hospedado en un único dispositivo de bajo consumo de 32 bits, sin redundancia ni terceros (`SOLUTION-INTAKE §10`, `§17 P.9`, `§17 P.12`). Aplicar cuatro ambientes cloud sería desproporcionado y no representaría la realidad operativa. ADR-05 establece el despliegue self-contained de un solo dispositivo; sobre esa base se declara un modelo reducido de tres etapas. La reducción se justifica contra el default y no se hace sin sustento: está respaldada por ADR-05 y por la restricción de plataforma del intake §10.

| Ambiente / etapa | Propósito | Destino | Aprobador | SLA / disponibilidad |
| --- | --- | --- | --- | --- |
| Desarrollo local | Construir, probar y depurar; ejecutar la suite completa y los samples | Estación x64 del implementador | Auto | — (no productivo) |
| Pre-producción (validación, opcional) | Validar el paquete `linux-arm` real: instalación de prueba, arranque del servicio, medición de NFR | Hardware ARM equivalente al de referencia, o el mismo dispositivo objetivo en una ventana de prueba | Auto (el implementador genera el tag de prerelease) | Sin SLO formal; se mide latencia p95 < 200 ms, throughput ≥ 50 mensajes/s, memoria ≤ 8 MB por conexión (05 §8) sobre el hardware real |
| Producción | Operar la moderación de forma continua | Dispositivo objetivo: Raspbian / Raspberry Pi OS de 32 bits (armv7l), servicio systemd | Release manager (implementador único, `estrategia-calidad §4`) | SLO de disponibilidad 99 % mensual (NFR de 05 §8; sostenido por reinicio automático del servicio, ADR-05) |

Referencia a los NFR de disponibilidad y latencia de 05 (`arquitectura-solucion_v1.0.md §8`): producción declara el SLO de disponibilidad de 99 % mensual; pre-producción es donde se miden los NFR numéricos de latencia (p95 < 200 ms), throughput (≥ 50 mensajes/s) y memoria (≤ 8 MB por conexión) sobre el hardware real antes de promover (DoD release de `definition-of-done_v1.0.md`). La promoción entre etapas sigue las promotion rules de `pipeline-ci-cd_v1.0.md §5`.

## 2. Provisión (IaC)

IaC mínima/ausente, justificada para un único dispositivo auto-hospedado. No hay infraestructura cloud que provisionar ni un parque de servidores que reproducir; un Terraform/Pulumi/Bicep sería sobreingeniería para un solo dispositivo físico ya disponible (`SOLUTION-INTAKE §10`, ADR-05). La "provisión" se materializa como scripts de instalación versionados, no como aprovisionamiento declarativo de infraestructura:

| Elemento | Forma | Ubicación |
| --- | --- | --- |
| Instalador del servicio | Script idempotente que despliega el paquete y registra el servicio | `scripts/servicio/` (`SOLUTION-INTAKE §16`, ADR-05 §7) |
| Unidad de servicio del sistema (systemd) | Archivo de unidad con reinicio automático | `scripts/servicio/` |
| Archivo de entorno del servicio | Plantilla con permisos restringidos (la clave maestra se completa en el dispositivo, nunca en el repositorio) | `scripts/servicio/` (plantilla); instancia real solo en el dispositivo |

Los scripts se versionan junto al código y se revisan como cualquier cambio. No se usa state remoto ni `plan/apply` porque no hay infraestructura declarativa; la "aprobación de plan" se sustituye por la revisión del PR del script de instalación y por la verificación post-publish de `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §3`.

## 3. Configuración por ambiente (12-factor)

Configuración en variables de entorno y archivos referenciados, nunca en código (12-factor), conforme a ADR-07 y `SOLUTION-INTAKE §17 P.5`. El archivo de entorno del servicio lleva permisos restringidos del sistema de archivos.

| Variable / configuración | Desarrollo local | Pre-producción | Producción | Notas |
| --- | --- | --- | --- | --- |
| Clave maestra de cifrado de tokens | Valor no productivo inyectado por el ambiente de pruebas (`estrategia-testing §7`) | Valor de prueba dedicado | Valor real en variable de entorno del servicio (archivo de entorno con permisos restringidos) | ADR-07; nunca en la base ni en el repositorio |
| Ruta de la base SQLite | Local efímera o de desarrollo | Local del dispositivo de validación | Ruta persistente en el dispositivo, modo WAL | ADR-02 (persistencia embebida WAL) |
| Token de bot por servidor | Sintético / no real | De prueba | Real, cifrado en reposo con la clave maestra | ADR-07; se ingresa por el panel, se cifra y se guarda cifrado |
| Parámetros de detección (umbral de canales, ventana) | Defaults de los descriptores | Defaults o valores de calibración | Valores calibrados por el operador | ADR-12 (descriptores como fuente de verdad); valores por defecto exactos abiertos a Sprint 0 (`§17 P.11`) |
| Nivel y destino de logs | Consola | Journal del sistema | Journal del sistema | NFR de observabilidad (05 §8); el servicio registra al journal |

Mapa por ambiente: la única diferencia funcional entre ambientes es el valor de los secretos y rutas; el binario es el mismo paquete promovido (no se recompila por ambiente). El bot y el panel corren en el mismo proceso (ADR-01), por lo que comparten el archivo de entorno.

## 4. Secretos

| Secreto | Almacenamiento | Rotación | Prohibición |
| --- | --- | --- | --- |
| Clave maestra de cifrado de tokens | Variable de entorno del servicio, en un archivo de entorno con permisos restringidos, fuera de la base (ADR-07) | Operación poco frecuente y acotada; rotarla exige re-cifrar los tokens (ADR-07 §6). Procedimiento de resguardo de la clave documentado en el README del dispositivo | Prohibido commitearla; prohibido guardarla en la base |
| Token de bot por servidor | Cifrado en reposo en la base con AES bajo la clave maestra; se descifra solo en memoria al operar (ADR-07) | Se re-ingresa por el panel si el token se revoca o rota en la plataforma (`§7 del intake`) | Prohibido guardarlo en claro; prohibido commitearlo |
| Credencial del administrador | Hash robusto en formato PHC (Argon2 o PBKDF2, elección abierta a Sprint 0) en la base (ADR-03) | El administrador la cambia desde el panel | Prohibido guardarla en claro; prohibido commitearla |

No se usa un gestor de secretos de terceros: contradeciría el auto-hospedaje sin terceros (ADR-07 §4). La protección de la clave maestra y del archivo de entorno se apoya en los permisos del sistema de archivos del dispositivo. El escaneo de commits para detectar secretos filtrados es parte de la supply chain (`supply-chain-seguridad_v1.0.md §4`; anti-patrón §4.8 "secretos en commit"). Compliance: la residencia local de los datos personales y la minimización las gobierna ADR-06 (Ley 25.326); los secretos no salen del dispositivo.

## 5. Promoción / instalación en el dispositivo

La promoción entre etapas la disparan los tags SemVer (`estrategia-versionado_v1.0.md §5`) y la ejecuta el pipeline (`pipeline-ci-cd_v1.0.md §5`). La instalación en el dispositivo es el despliegue propiamente dicho:

1. El pipeline publica el paquete self-contained `linux-arm` firmado y con SBOM (release).
2. En el dispositivo, se descarga el paquete y se verifica checksum y firma (`supply-chain-seguridad §2`).
3. Se ejecuta el instalador, que despliega el paquete, registra/actualiza la unidad systemd y conserva el archivo de entorno y la clave maestra existentes:
   `sudo ./scripts/servicio/instalar.sh --paquete <paquete>.zip --conservar-entorno`.
4. Se habilita y arranca el servicio con reinicio automático:
   `sudo systemctl enable --now discord-moderador-bot`.
5. Se verifica el arranque y la operación (`guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §3`).

Aprobador requerido y registro de auditoría: la promoción a producción la aprueba el release manager (implementador único, `estrategia-calidad §4`); el registro auditable es el tag SemVer y las release notes. El procedimiento detallado de instalación, verificación post-publish y rollback vive en `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md`. La guía de instalación que normalmente viviría en la categoría 10 está colapsada en los READMEs del repositorio y de `scripts/servicio/` (ADR-11) y se resume acá y en la guía de publicación.

## 6. Trazabilidad

- El modelo de ambientes reducido se justifica contra el default §2.2 con `SOLUTION-INTAKE §10` y ADR-05.
- El SLO de disponibilidad de producción (99 % mensual) y los NFR de latencia/throughput/memoria de pre-producción referencian `arquitectura-solucion_v1.0.md §8`.
- La configuración 12-factor y el manejo de la clave maestra y los tokens referencian ADR-07 y `§17 P.5`; la persistencia WAL referencia ADR-02; el hash de la credencial referencia ADR-03.
- La residencia local y la minimización referencian ADR-06 (Ley 25.326).
- La IaC mínima se justifica con ADR-05 y `§10`; los scripts viven en `scripts/servicio/` (`§16`).
- Downstream: los READMEs (ADR-11) detallan la instalación; los samples de 11 referencian las etapas declaradas acá.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Entornos y despliegue inicial para `discord-bots-admin`: modelo de ambientes reducido de tres etapas (desarrollo local, pre-producción opcional, producción en el dispositivo) justificado contra el default DEV/QA/STAGING/PROD por `§10` y ADR-05; IaC mínima como scripts versionados en `scripts/servicio/`; configuración 12-factor con mapa por ambiente; secretos (clave maestra, token cifrado, credencial con hash) según ADR-07/ADR-03 sin gestor de terceros; promoción/instalación en el dispositivo por tag SemVer con aprobación del release manager. |
