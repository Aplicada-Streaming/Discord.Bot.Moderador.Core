# Operación del servicio — discord-bots-admin

Artefactos de despliegue del paquete self-contained `linux-arm` (armv7l) que se instala
como servicio gestionado por systemd en el dispositivo objetivo (Raspbian / Raspberry Pi
OS de 32 bits). Materializa la "provisión como scripts versionados" de
`entornos-deploy_v1.0.md §2` y la guía de
`guia-publicacion-paquete-self-contained-linux-arm_v1.0.md`. Gobernado por ADR-05
(servicio del sistema) y ADR-07 (clave maestra fuera del repo y de la base).

## Contenido

| Archivo | Rol |
| --- | --- |
| `publicar.ps1` / `publicar.sh` | Publica self-contained `linux-arm` por cross-compile desde x64 y arma el zip + checksum (STAGE-11). |
| `instalar.sh` | Instala/actualiza el servicio en el dispositivo; **preserva** el archivo de entorno y la clave maestra (rollback §17 P.8). |
| `discord-moderador-bot.service` | Unidad systemd con `EnvironmentFile`, reinicio automático y endurecimiento básico. |
| `discord-bots-admin.env.example` | Plantilla del archivo de entorno **sin secretos**. La instancia real vive sólo en el dispositivo. |

> La clave maestra `DISCORDMODERADOR_CLAVE_MAESTRA` y los tokens **nunca** se versionan
> ni se guardan en la base (ADR-07). El `.gitignore` ignora `*.env`; sólo se versiona la
> plantilla `*.env.example`.

## 1. Empaquetar (en la estación x64 o en CI)

```bash
# bash (runner Linux / CI)
./scripts/servicio/publicar.sh 1.0.0
# PowerShell (estación Windows)
pwsh scripts/servicio/publicar.ps1 -Version 1.0.0
```

Genera `artefactos/discord-bots-admin_1.0.0_linux-arm.zip` y su `.sha256`. El paso de
publicación verifica que el zip incluye el ejecutable `DiscordModeradorBot.Servicio` y la
nativa `libe_sqlite3.so` para `linux-arm`. En el release del repositorio se le adjuntan
además el SBOM (CycloneDX) y la firma (`supply-chain-seguridad_v1.0.md §1/§2`).

## 2. Instalar en el dispositivo

```bash
# Verificar integridad antes de instalar
sha256sum -c discord-bots-admin_1.0.0_linux-arm.zip.sha256

# Instalación (idempotente; crea usuario de servicio, directorios y la unidad systemd)
sudo ./instalar.sh --paquete discord-bots-admin_1.0.0_linux-arm.zip

# Primera vez: completar la clave maestra y arrancar
sudo openssl rand -base64 32   # generar una clave; pegarla en el .env
sudoedit /etc/discord-bots-admin/discord-bots-admin.env
sudo systemctl enable --now discord-moderador-bot
systemctl status discord-moderador-bot      # debe figurar active (running)
journalctl -u discord-moderador-bot -n 50   # logs al journal (NFR observabilidad)
```

Rutas que usa el servicio:

- Binarios: `/opt/discord-bots-admin` (se reemplazan en cada instalación).
- Datos (SQLite WAL): `/var/lib/discord-bots-admin` (**se preservan**; el rollback no los toca).
- Entorno + clave maestra: `/etc/discord-bots-admin/discord-bots-admin.env` (permisos `600`, **se preserva**).

## 3. Actualizar a una versión nueva

```bash
sudo systemctl stop discord-moderador-bot
sudo ./instalar.sh --paquete discord-bots-admin_<nueva>_linux-arm.zip
sudo systemctl start discord-moderador-bot
```

El instalador **no** sobrescribe el `.env` existente: la clave maestra se conserva y los
tokens cifrados siguen siendo válidos.

## 4. Rollback (reinstalar la publicación previa, §17 P.8 / ADR-05 / ADR-07)

```bash
gh release list                                   # identificar el tag estable anterior
gh release download v<X.Y.Z-previa>               # descargar el paquete previo
sha256sum -c discord-bots-admin_<previa>_linux-arm.zip.sha256

sudo systemctl stop discord-moderador-bot
sudo ./instalar.sh --paquete discord-bots-admin_<previa>_linux-arm.zip --conservar-entorno
sudo systemctl start discord-moderador-bot && systemctl status discord-moderador-bot
```

`--conservar-entorno` es el comportamiento por defecto (explícito acá): preserva el
archivo de entorno y la clave maestra, de modo que el servicio opera **sin re-registrar
tokens**. El rollback de paquete no toca la base SQLite; si el defecto es de datos o de
esquema se trata por migración, no por rollback (`pipeline-ci-cd_v1.0.md §6`).

## 5. Resguardo de la clave maestra

Resguardá `DISCORDMODERADOR_CLAVE_MAESTRA` fuera del dispositivo (gestor de contraseñas
del operador). Si se pierde, los tokens cifrados en la base dejan de poder descifrarse y
hay que re-registrarlos por el panel (ADR-07 §6).
