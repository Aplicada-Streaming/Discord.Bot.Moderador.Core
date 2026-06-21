#!/usr/bin/env bash
# =============================================================================
# Publica el paquete self-contained linux-arm (armv7l) y lo empaqueta como zip.
#
# Reproduce STAGE-11 (Package) del pipeline (pipeline-ci-cd_v1.0.md §2;
# guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §2). Cross-compila
# desde x64 (no requiere runner ARM, §17 P.8 / ADR-05), incluye los artefactos de
# despliegue de scripts/servicio/ y genera el checksum SHA-256.
#
# Uso:
#   ./publicar.sh [VERSION] [DIR_SALIDA]
#   ./publicar.sh 1.0.0 ./artefactos
# =============================================================================
set -euo pipefail

VERSION="${1:-1.0.0}"
SALIDA="${2:-./artefactos}"
RID="linux-arm"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO="$(cd "${SCRIPT_DIR}/../.." && pwd)"
PROYECTO="${REPO}/src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj"
DIR_PUBLISH="${REPO}/publish/${RID}"
NOMBRE_PAQUETE="discord-bots-admin_${VERSION}_${RID}"
DIR_SALIDA="${REPO}/${SALIDA#./}"
ZIP="${DIR_SALIDA}/${NOMBRE_PAQUETE}.zip"

echo "==> Publicando self-contained ${RID} (cross-compile desde x64)"
rm -rf "${DIR_PUBLISH}"
dotnet publish "${PROYECTO}" -c Release -r "${RID}" --self-contained true -o "${DIR_PUBLISH}"

# Verificar ejecutable + nativa de SQLite linux-arm.
[[ -f "${DIR_PUBLISH}/DiscordModeradorBot.Servicio" ]] || { echo "ERROR: falta el ejecutable del servicio" >&2; exit 1; }
[[ -f "${DIR_PUBLISH}/libe_sqlite3.so" ]] || { echo "ERROR: falta libe_sqlite3.so (nativa SQLite linux-arm)" >&2; exit 1; }
echo "==> OK: ejecutable y libe_sqlite3.so presentes."

# Incluir artefactos de despliegue.
mkdir -p "${DIR_PUBLISH}/servicio"
cp -f "${SCRIPT_DIR}/instalar.sh" "${DIR_PUBLISH}/servicio/"
cp -f "${SCRIPT_DIR}/discord-moderador-bot.service" "${DIR_PUBLISH}/servicio/"
cp -f "${SCRIPT_DIR}/discord-bots-admin.env.example" "${DIR_PUBLISH}/servicio/"
cp -f "${SCRIPT_DIR}/README.md" "${DIR_PUBLISH}/servicio/" 2>/dev/null || true
chmod +x "${DIR_PUBLISH}/servicio/instalar.sh"

# Empaquetar.
mkdir -p "${DIR_SALIDA}"
rm -f "${ZIP}"
echo "==> Empaquetando ${ZIP}"
( cd "${DIR_PUBLISH}" && zip -q -r "${ZIP}" . )

# Checksum SHA-256.
( cd "${DIR_SALIDA}" && sha256sum "${NOMBRE_PAQUETE}.zip" > "${NOMBRE_PAQUETE}.zip.sha256" )
echo "==> Paquete: ${ZIP}"
echo "==> SHA256 : $(cat "${ZIP}.sha256")"
echo "==> Listo. Adjuntar zip + .sha256 (+ SBOM + firma en release) según guia-publicacion §2."
