# ADR-14 — Despliegue en contenedor Docker linux/amd64 gestionado por Portainer

**Proyecto:** discord-bots-admin
**Documento:** ADR-14-despliegue-contenedor-docker-x64-portainer_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-07-10
**Autor:** DevOps / Arquitecto
**Categoría:** Despliegue
**Supercede:** ADR-05

## 1. Contexto

ADR-05 establecía el despliegue como paquete self-contained `linux-arm` instalado como servicio
systemd en una Raspberry Pi (armv7l), conforme a `SOLUTION-INTAKE §10`. El servidor de destino
real es un equipo Debian x64 (Intel i7) con Docker y Portainer ya instalados. El despliegue ARM
sin contenedores ya no representa el entorno operativo real. Se toma la decisión de migrar a un
modelo de contenedor Docker, que:

- Coincide con el servidor de destino efectivo (linux/amd64).
- Permite gestión visual y operación diaria desde Portainer (ya desplegado en `i7infra`).
- Habilita un pipeline CI/CD estándar basado en imagen Docker Hub en lugar de zip adjunto a GitHub Releases.
- El desarrollador no tiene paravirtualización local (Windows 10 sin Hyper-V); el servidor Debian es el entorno de ejecución.

## 2. Decisión

Se adopta un despliegue basado en imagen Docker `linux/amd64` construida por GitHub Actions y
publicada en Docker Hub. La gestión operativa del contenedor se delega a Portainer. La base SQLite
(ADR-02) se persiste en un named volume de Docker. La configuración sensible (clave maestra, tokens)
se inyecta por variables de entorno en Portainer, sin modificar la imagen (ADR-07, 12-factor).

El wizard de primera ejecución (`/configuracion-inicial`) ya implementado en la aplicación
(Blazor + `ServicioAdministrador.ExisteAdministradorAsync`) cubre el bootstrap del sistema en el
contenedor sin cambios adicionales.

## 3. Estado

Aceptado el 2026-07-10. Supercede ADR-05.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
|---|---|---|
| Mantener self-contained ARM (ADR-05) | Sin cambio de pipeline | No coincide con el servidor real (x64); gestión manual |
| Contenedor Docker linux/amd64 (elegida) | Coincide con el servidor; gestión por Portainer; pipeline estándar | Cambia el artefacto de entrega; requiere Docker Hub |
| Imagen multi-arch (amd64 + arm/v7) | Soporta ambos targets | Complejidad innecesaria mientras el único destino es x64 |

## 5. Consecuencias positivas

1. El despliegue y las actualizaciones se gestionan desde Portainer sin SSH al servidor.
2. El pipeline CI/CD produce la imagen Docker como único artefacto; elimina la compilación cruzada ARM.
3. La base SQLite sobrevive actualizaciones y reinstalaciones gracias al named volume de Docker.
4. El wizard de primera ejecución ya implementado cubre el bootstrap del sistema en contenedor.
5. El rollback se realiza cambiando el tag de la imagen en Portainer a la versión anterior.

## 6. Consecuencias negativas y trade-offs

1. Se pierde la compatibilidad ARM directa: aceptado; el único servidor de destino actual es x64.
2. Requiere acceso a Docker Hub para publicar y desplegar la imagen.
3. El desarrollador no puede probar Docker localmente (sin paravirtualización en Windows 10);
   la validación se realiza en el servidor `i7infra` o mediante el pipeline de CI.

## 7. Implementación

| Artefacto | Ubicación |
|---|---|
| `Dockerfile` multi-stage | `Discord.Bot.Moderador.Core/Dockerfile` |
| `docker-compose.yml` del servicio | `Infra/bot/docker-compose.yml` |
| GitHub Actions docker-publish | `.github/workflows/docker-publish.yml` |
| Script backup SQLite | `scripts/servicio/backup-sqlite.sh` |

Variables de entorno requeridas en el contenedor (se configuran en Portainer, nunca en la imagen):

| Variable | Descripción |
|---|---|
| `Moderacion__Gateway` | `Discord` en producción, `Simulado` en dev |
| `Persistencia__RutaBase` | `/app/data/discordmoderador.db` (dentro del volume) |
| `ClaveMaestra` | Clave de cifrado AES de tokens (ADR-07); solo en Portainer |

## 8. Métricas de validación

- La imagen se construye y publica en Docker Hub en el pipeline de CI al crear un tag `v*`.
- El contenedor arranca en el servidor, aplica las migraciones y responde en el puerto configurado.
- El wizard `/configuracion-inicial` aparece en el primer arranque y redirige al panel tras el alta.
- La base SQLite persiste tras `docker compose down && docker compose up`.
- El rollback cambia el tag de imagen en Portainer y el contenedor reinicia con la versión anterior.

## 9. Referencias

- ADR-02 (persistencia SQLite WAL), ADR-03 (hash del administrador), ADR-07 (clave maestra).
- `entornos-deploy_v1.0.md`, `pipeline-ci-cd_v1.0.md`.
- `SOLUTION-INTAKE §10` (restricción original de plataforma, ahora superada por cambio de entorno).

## 10. Control de cambios

| Versión | Fecha | Descripción |
|---|---|---|
| 1.0 | 2026-07-10 | Decisión inicial. Supercede ADR-05. |
