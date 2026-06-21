#!/usr/bin/env bash
# =============================================================================
# Instalador del servicio discord-bots-admin en el dispositivo (Raspbian armv7l).
#
# Despliega el paquete self-contained linux-arm, registra/actualiza la unidad systemd
# y PRESERVA el archivo de entorno y la clave maestra existentes, de modo que los
# tokens cifrados sigan siendo válidos tras una reinstalación o un rollback
# (SOLUTION-INTAKE §17 P.8, ADR-05, ADR-07; entornos-deploy_v1.0.md §5).
#
# Es idempotente: se puede correr varias veces. La base SQLite y el archivo de
# entorno NUNCA se sobrescriben (rollback no toca datos; pipeline-ci-cd_v1.0.md §6).
#
# Uso:
#   sudo ./instalar.sh --paquete discord-bots-admin_<X.Y.Z>_linux-arm.zip [--conservar-entorno]
#   sudo ./instalar.sh                # instala desde la carpeta donde vive este script
#
# Opciones:
#   --paquete <zip>     Ruta al zip publicado. Si se omite, instala el contenido
#                       ya descomprimido junto a este script (carpeta del paquete).
#   --conservar-entorno Fuerza preservar el .env aunque exista (comportamiento por
#                       defecto; explícito para el rollback de §17 P.8).
#   --no-arrancar       No habilita ni arranca el servicio (sólo instala archivos).
# =============================================================================
set -euo pipefail

NOMBRE_SERVICIO="discord-moderador-bot"
USUARIO_SERVICIO="discordbot"
GRUPO_SERVICIO="discordbot"
DIR_INSTALACION="/opt/discord-bots-admin"
DIR_DATOS="/var/lib/discord-bots-admin"
DIR_ENTORNO="/etc/discord-bots-admin"
ARCHIVO_ENTORNO="${DIR_ENTORNO}/discord-bots-admin.env"
RUTA_UNIDAD="/etc/systemd/system/${NOMBRE_SERVICIO}.service"
EJECUTABLE="DiscordModeradorBot.Servicio"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

PAQUETE=""
NO_ARRANCAR=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    --paquete) PAQUETE="${2:-}"; shift 2 ;;
    --conservar-entorno) shift ;;          # comportamiento por defecto, explícito
    --no-arrancar) NO_ARRANCAR=1; shift ;;
    -h|--help) grep '^#' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
    *) echo "Opción desconocida: $1" >&2; exit 2 ;;
  esac
done

if [[ "$(id -u)" -ne 0 ]]; then
  echo "ERROR: ejecutá el instalador como root (sudo)." >&2
  exit 1
fi

echo "==> Instalando ${NOMBRE_SERVICIO}"

# 1. Usuario y grupo de servicio sin privilegios.
if ! getent group "${GRUPO_SERVICIO}" >/dev/null; then
  groupadd --system "${GRUPO_SERVICIO}"
fi
if ! id "${USUARIO_SERVICIO}" >/dev/null 2>&1; then
  useradd --system --gid "${GRUPO_SERVICIO}" --home-dir "${DIR_DATOS}" \
          --no-create-home --shell /usr/sbin/nologin "${USUARIO_SERVICIO}"
fi

# 2. Directorios. Los datos (SQLite WAL) se preservan si ya existen.
install -d -m 0755 "${DIR_INSTALACION}"
install -d -m 0750 -o "${USUARIO_SERVICIO}" -g "${GRUPO_SERVICIO}" "${DIR_DATOS}"
install -d -m 0750 "${DIR_ENTORNO}"

# 3. Resolver el origen de los binarios (zip o carpeta ya descomprimida).
TMP_EXTRACCION=""
ORIGEN_BIN=""
if [[ -n "${PAQUETE}" ]]; then
  if [[ ! -f "${PAQUETE}" ]]; then
    echo "ERROR: no existe el paquete: ${PAQUETE}" >&2
    exit 1
  fi
  TMP_EXTRACCION="$(mktemp -d)"
  echo "==> Descomprimiendo ${PAQUETE}"
  unzip -q -o "${PAQUETE}" -d "${TMP_EXTRACCION}"
  # El zip contiene la carpeta linux-arm con el binario y la subcarpeta servicio/.
  if [[ -f "${TMP_EXTRACCION}/linux-arm/${EJECUTABLE}" ]]; then
    ORIGEN_BIN="${TMP_EXTRACCION}/linux-arm"
  elif [[ -f "${TMP_EXTRACCION}/${EJECUTABLE}" ]]; then
    ORIGEN_BIN="${TMP_EXTRACCION}"
  else
    ORIGEN_BIN="$(dirname "$(find "${TMP_EXTRACCION}" -name "${EJECUTABLE}" -type f | head -n1)")"
  fi
else
  # Instalación desde la carpeta del paquete ya descomprimido. El binario suele estar
  # un nivel arriba de scripts/servicio/ (la carpeta del paquete linux-arm).
  if [[ -f "${SCRIPT_DIR}/../${EJECUTABLE}" ]]; then
    ORIGEN_BIN="$(cd "${SCRIPT_DIR}/.." && pwd)"
  elif [[ -f "${SCRIPT_DIR}/${EJECUTABLE}" ]]; then
    ORIGEN_BIN="${SCRIPT_DIR}"
  fi
fi

if [[ -z "${ORIGEN_BIN}" || ! -f "${ORIGEN_BIN}/${EJECUTABLE}" ]]; then
  echo "ERROR: no se encontró el ejecutable ${EJECUTABLE}. Pasá --paquete <zip> o corré desde la carpeta del paquete." >&2
  exit 1
fi

# 4. Copiar binarios (sin tocar datos ni entorno). rsync si está, si no cp.
echo "==> Desplegando binarios en ${DIR_INSTALACION}"
if command -v rsync >/dev/null 2>&1; then
  rsync -a --delete --exclude 'servicio' "${ORIGEN_BIN}/" "${DIR_INSTALACION}/"
else
  rm -rf "${DIR_INSTALACION:?}/"*
  cp -a "${ORIGEN_BIN}/." "${DIR_INSTALACION}/"
  rm -rf "${DIR_INSTALACION}/servicio"
fi
chmod 0755 "${DIR_INSTALACION}/${EJECUTABLE}"
chown -R root:root "${DIR_INSTALACION}"

# 5. Archivo de entorno: PRESERVAR si existe (clave maestra + tokens cifrados válidos).
#    Esta es la garantía de rollback de §17 P.8 / ADR-07.
if [[ -f "${ARCHIVO_ENTORNO}" ]]; then
  echo "==> Archivo de entorno existente PRESERVADO (clave maestra y tokens cifrados intactos): ${ARCHIVO_ENTORNO}"
else
  PLANTILLA=""
  if [[ -f "${SCRIPT_DIR}/discord-bots-admin.env.example" ]]; then
    PLANTILLA="${SCRIPT_DIR}/discord-bots-admin.env.example"
  elif [[ -n "${TMP_EXTRACCION}" ]]; then
    PLANTILLA="$(find "${TMP_EXTRACCION}" -name 'discord-bots-admin.env.example' | head -n1)"
  fi
  if [[ -n "${PLANTILLA}" && -f "${PLANTILLA}" ]]; then
    install -m 0600 "${PLANTILLA}" "${ARCHIVO_ENTORNO}"
    echo "==> ATENCIÓN: archivo de entorno creado desde la plantilla en ${ARCHIVO_ENTORNO}"
    echo "    Completá DISCORDMODERADOR_CLAVE_MAESTRA (p.ej. 'openssl rand -base64 32') antes de arrancar."
  else
    echo "ERROR: no se halló la plantilla discord-bots-admin.env.example para crear el entorno inicial." >&2
    exit 1
  fi
fi
chmod 0600 "${ARCHIVO_ENTORNO}"
chown root:"${GRUPO_SERVICIO}" "${ARCHIVO_ENTORNO}" 2>/dev/null || chown root:root "${ARCHIVO_ENTORNO}"

# 6. Instalar/actualizar la unidad systemd.
UNIDAD_ORIGEN="${SCRIPT_DIR}/${NOMBRE_SERVICIO}.service"
if [[ ! -f "${UNIDAD_ORIGEN}" && -n "${TMP_EXTRACCION}" ]]; then
  UNIDAD_ORIGEN="$(find "${TMP_EXTRACCION}" -name "${NOMBRE_SERVICIO}.service" | head -n1)"
fi
echo "==> Instalando unidad systemd en ${RUTA_UNIDAD}"
install -m 0644 "${UNIDAD_ORIGEN}" "${RUTA_UNIDAD}"
systemctl daemon-reload

# 7. Habilitar y arrancar (salvo --no-arrancar).
if [[ "${NO_ARRANCAR}" -eq 0 ]]; then
  echo "==> Habilitando y arrancando el servicio"
  systemctl enable --now "${NOMBRE_SERVICIO}"
  sleep 2
  systemctl status "${NOMBRE_SERVICIO}" --no-pager || true
else
  echo "==> Servicio instalado (no arrancado por --no-arrancar). Arrancá con: systemctl enable --now ${NOMBRE_SERVICIO}"
fi

# 8. Limpieza.
[[ -n "${TMP_EXTRACCION}" ]] && rm -rf "${TMP_EXTRACCION}"

echo "==> Instalación completa de ${NOMBRE_SERVICIO}."
