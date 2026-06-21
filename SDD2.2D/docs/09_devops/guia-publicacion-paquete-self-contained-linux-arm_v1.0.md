# Guía de publicación — paquete self-contained linux-arm

**Proyecto:** discord-bots-admin
**Documento:** guia-publicacion-paquete-self-contained-linux-arm_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero DevOps Senior (AG-09) — variante DevOps + Deploy Engineer (web-monolith)

Esta guía documenta la publicación del único artefacto del proyecto: un paquete self-contained para `linux-arm` (ARM de 32 bits, armv7l) que se instala como servicio gestionado por systemd. El tipo de artefacto sigue el patrón parametrizado de §3.1 de las reglas, familia `paquete-`: `paquete-self-contained-linux-arm`. No es `image-docker` (el cliente prohíbe contenedores, `SOLUTION-INTAKE §10`); la divergencia respecto del default §2.2 de web-monolith está gobernada por ADR-05 y se argumenta en `pipeline-ci-cd_v1.0.md`. No hay feed de paquetes porque el proyecto no es redistribuible (`§17 P.7`): la distribución es el paquete instalable, publicado como release del repositorio.

Distinción publicación vs despliegue (anti-patrón §4.8): esta guía cubre el empaquetado y la publicación del paquete (build/publicación) y, por la naturaleza del artefacto, también el procedimiento de instalación/verificación en el dispositivo (despliegue), que en este proyecto es indisociable del consumo del artefacto. El modelo de ambientes y la configuración del dispositivo viven en `entornos-deploy_v1.0.md`.

## 1. Pre-requisitos

| Pre-requisito | Detalle |
| --- | --- |
| Permiso de publicación de releases | Credencial con permiso de creación de releases en el repositorio (anclada a la plataforma de CI de `SOLUTION-INTAKE §17 P.8`); scope mínimo: crear release y subir adjuntos |
| Runtime de build | .NET 10 en el runner x64 (`§17 P.1`); no se requiere runner ARM (cross-compile, `§17 P.8`, ADR-05) |
| Credencial de firma | Identidad/credencial de firma con transparency log (`supply-chain-seguridad_v1.0.md §2`), inyectada por el ambiente de CI, nunca commiteada |
| Acceso al dispositivo (para instalación) | Acceso al dispositivo objetivo (Raspbian/Raspberry Pi OS 32 bits, armv7l, `§17 P.9`) con privilegios para registrar el servicio systemd |
| Configuración local opcional (publicación manual) | .NET 10 local para reproducir el empaquetado: `dotnet publish` con runtime identifier `linux-arm` |

## 2. Comando o stage de publicación

El stage que publica de forma automatizada es STAGE-11 (Package) + STAGE-12 (Publish) de `pipeline-ci-cd_v1.0.md §2`, disparado por un tag SemVer (`estrategia-versionado_v1.0.md §5`). El empaquetado es self-contained por cross-compile desde el runner x64.

Comando exacto reproducible (equivalente local y del stage):

```bash
# 1. Publicación self-contained linux-arm por cross-compile desde x64
dotnet publish src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj \
  -c Release -r linux-arm --self-contained true \
  -o ./publish/linux-arm

# 2. Empaquetado del paquete (zip con todo lo necesario + scripts de servicio)
#    Incluye el binario self-contained, la unidad systemd y el instalador.
cp -r scripts/servicio ./publish/linux-arm/servicio
zip -r discord-bots-admin_<X.Y.Z>_linux-arm.zip ./publish/linux-arm

# 3. Checksum del paquete
sha256sum discord-bots-admin_<X.Y.Z>_linux-arm.zip > discord-bots-admin_<X.Y.Z>_linux-arm.zip.sha256

# 4. SBOM (CycloneDX o SPDX) y firma del paquete y del SBOM
#    (ver supply-chain-seguridad_v1.0.md §1 y §2; herramientas por capacidad)

# 5. Publicación como release del repositorio (adjunta paquete, checksum, SBOM y firma)
gh release create v<X.Y.Z> \
  discord-bots-admin_<X.Y.Z>_linux-arm.zip \
  discord-bots-admin_<X.Y.Z>_linux-arm.zip.sha256 \
  sbom.json signature.sig \
  --notes-file CHANGELOG-release.md
```

El comando `gh release` corresponde a la CLI de la plataforma de CI declarada en `SOLUTION-INTAKE §17 P.8`; se mantiene como comando concreto reproducible.

Variables de entorno requeridas en CI:

| Variable | Propósito | Origen |
| --- | --- | --- |
| Credencial de publicación de releases | Crear el release y subir adjuntos | Secret del ambiente de CI (`§17 P.8`), nunca en el repositorio |
| Credencial de firma | Firmar el paquete y el SBOM | Secret del ambiente de CI (`supply-chain-seguridad §2`) |

La clave maestra de cifrado de tokens (ADR-07) no participa del build ni del empaquetado: vive solo en el dispositivo (archivo de entorno con permisos restringidos, `entornos-deploy_v1.0.md §4`). El paquete no la contiene.

## 3. Verificación post-publish

Confirma que el artefacto quedó publicado, es íntegro y es instalable y operable:

1. Descargar el paquete del release y verificar checksum y firma:
   ```bash
   gh release download v<X.Y.Z>
   sha256sum -c discord-bots-admin_<X.Y.Z>_linux-arm.zip.sha256
   # verificar la firma del paquete y del SBOM (supply-chain-seguridad §2)
   ```
2. Instalación de prueba en el dispositivo de referencia o equivalente (pre-producción, `entornos-deploy_v1.0.md §1`):
   ```bash
   unzip discord-bots-admin_<X.Y.Z>_linux-arm.zip -d /opt/discord-bots-admin
   sudo /opt/discord-bots-admin/servicio/instalar.sh --paquete discord-bots-admin_<X.Y.Z>_linux-arm.zip
   ```
3. Arranque del servicio y verificación de estado:
   ```bash
   sudo systemctl enable --now discord-moderador-bot
   systemctl status discord-moderador-bot   # debe figurar active (running)
   journalctl -u discord-moderador-bot -n 50 # el servicio registra al journal (NFR observabilidad, 05 §8)
   ```
4. Verificación funcional mínima: el panel responde en el dispositivo y el servicio reconecta al gateway con el token cifrado descifrado en memoria (ADR-07). Esta verificación corresponde a la DoD de release de `definition-of-done_v1.0.md` ("el artefacto self-contained se publica, instala como servicio y arranca").
5. Medición de NFR sobre el hardware real antes de promover a producción: latencia p95 < 200 ms, throughput ≥ 50 mensajes/s, memoria ≤ 8 MB por conexión (05 §8; banco de carga de `criterios-validacion`/`matriz-cobertura-pruebas` de 08).

## 4. Rollback

Rollback por reinstalación de la publicación anterior, preservando el archivo de entorno y la clave maestra, de modo que los tokens cifrados sigan siendo válidos (`SOLUTION-INTAKE §17 P.8`, ADR-05, ADR-07). Como el paquete no se distribuye por un feed, no hay "delist": el rollback es reinstalar la versión previa.

1. Seleccionar la última publicación estable previa: `gh release list`.
2. Descargar el paquete previo y verificar checksum y firma: `gh release download v<X.Y.Z-previa>` (`supply-chain-seguridad §2`).
3. Detener el servicio: `sudo systemctl stop discord-moderador-bot`.
4. Reinstalar conservando el archivo de entorno y la clave maestra (no se sobrescribe el archivo de entorno):
   `sudo /opt/discord-bots-admin/servicio/instalar.sh --paquete <paquete-previo>.zip --conservar-entorno`.
5. Arrancar y verificar: `sudo systemctl start discord-moderador-bot && systemctl status discord-moderador-bot`.
6. Confirmar que el servicio opera sin re-registrar tokens (clave maestra preservada; los tokens cifrados siguen válidos, ADR-07 §8).

Ventana de gracia y comunicación: al ser un operador único auto-hospedado, no hay consumidores externos a notificar; la comunicación es el CHANGELOG y las release notes del release de fix. El rollback de paquete no toca la base SQLite del dispositivo; si el defecto es de datos o de esquema, se trata por migración, no por rollback de paquete. El procedimiento debe ensayarse al menos una vez (criterio §5.4 de las reglas; verificable en la DoD de release con un rollback de prueba).

## 5. Métricas

| Métrica | Definición | Observación |
| --- | --- | --- |
| Resultado de la instalación | El paquete se instala y el servicio queda `active (running)` | Criterio de aceptación de 00/alcance y ADR-05 §8 |
| Tiempo de arranque del servicio | Tiempo desde `systemctl start` hasta estado activo | Indicador operativo; soporta el SLO de disponibilidad 99 % mensual (05 §8) |
| Disponibilidad mensual | Tiempo de servicio sobre tiempo total del mes (journal + estado de conexión) | NFR de disponibilidad (05 §8); reinicio automático del servicio (ADR-05) |
| Validez de la firma y del checksum post-publish | Verificación de firma y checksum exitosa antes de instalar | `supply-chain-seguridad §2` |
| Vulnerabilidades detectadas post-publish | CVE nuevas detectadas por el re-escaneo programado sobre el último release | Schedule de `pipeline-ci-cd_v1.0.md §1`; política de CVE de `supply-chain-seguridad §6` |
| Tiempo medio hasta detección de regresión | Tiempo entre publicar un release y detectar una regresión en operación | Indicador de calidad operativa; gatilla rollback (§4) |

## 6. Trazabilidad

- El empaquetado self-contained `linux-arm` por cross-compile y la instalación como servicio systemd referencian ADR-05; la preservación de la clave maestra en rollback referencia ADR-07; la residencia local referencia ADR-06.
- El stage de publicación referencia STAGE-11/STAGE-12 de `pipeline-ci-cd_v1.0.md §2`; el disparo por tag referencia `estrategia-versionado_v1.0.md §5`.
- La verificación post-publish referencia la DoD de release de `definition-of-done_v1.0.md` y los NFR de 05 §8.
- La firma, el SBOM y la verificación de integridad referencian `supply-chain-seguridad_v1.0.md`.
- Downstream: el README del repositorio y el de `scripts/servicio/` (categoría 10 colapsada, ADR-11) detallan la instalación local citando estos comandos; los samples de 11 referencian el artefacto declarado acá.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Guía de publicación inicial del paquete self-contained `linux-arm` para `discord-bots-admin`: nombre parametrizado familia `paquete-` (no hardcodea gestor); pre-requisitos; comando/stage de publicación por cross-compile self-contained con SBOM, firma y checksum; verificación post-publish (integridad, instalación de prueba, arranque del servicio, medición de NFR); rollback por reinstalación preservando entorno y clave maestra (ADR-05/ADR-07); métricas. Divergencia respecto del default image-docker gobernada por ADR-05. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: anclaje del comando `gh release` a la CLI de la plataforma de CI declarada en `SOLUTION-INTAKE §17 P.8` en su primera aparición, conservando el comando concreto. |
