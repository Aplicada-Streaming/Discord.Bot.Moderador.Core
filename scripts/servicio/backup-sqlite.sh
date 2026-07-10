#!/usr/bin/env bash
# backup-sqlite.sh — Backup del archivo SQLite del Discord Moderador Bot
# Uso:   ./backup-sqlite.sh [directorio-destino]
# Deps:  docker, zip
# Notas:
#   - Detiene el contenedor antes de copiar para garantizar un checkpoint WAL limpio (ADR-02).
#   - El contenedor reinicia automáticamente tras el backup (restart: unless-stopped).
#   - El archivo de backup se guarda como zip con timestamp en el directorio destino.
#
# Restauración:
#   1. docker stop discord-bot-moderador
#   2. unzip discordmoderador_YYYYMMDD_HHMMSS.zip -d /tmp/
#   3. docker cp /tmp/discordmoderador_backup.db discord-bot-moderador:/app/data/discordmoderador.db
#   4. docker start discord-bot-moderador

set -euo pipefail

CONTAINER="discord-bot-moderador"
DB_CONTAINER_PATH="/app/data/discordmoderador.db"
DEST_DIR="${1:-./backups}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
TMP_FILE="/tmp/discordmoderador_backup_${TIMESTAMP}.db"
BACKUP_ZIP="${DEST_DIR}/discordmoderador_${TIMESTAMP}.zip"

mkdir -p "${DEST_DIR}"

echo "[backup] Deteniendo el contenedor ${CONTAINER}..."
docker stop "${CONTAINER}"

echo "[backup] Copiando base de datos..."
docker cp "${CONTAINER}:${DB_CONTAINER_PATH}" "${TMP_FILE}"

echo "[backup] Reiniciando el contenedor ${CONTAINER}..."
docker start "${CONTAINER}"

echo "[backup] Comprimiendo en ${BACKUP_ZIP}..."
zip -j "${BACKUP_ZIP}" "${TMP_FILE}"
rm -f "${TMP_FILE}"

echo "[backup] Completado: ${BACKUP_ZIP}"
