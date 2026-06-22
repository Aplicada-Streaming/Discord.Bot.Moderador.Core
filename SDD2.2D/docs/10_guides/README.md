# 10 Guías prácticas — discord-bots-admin

**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Versión de la sección:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-22

Guías operativas y de uso, orientadas a la persona que configura y opera el servicio (no a la
especificación). Complementan la documentación formal (00–09) con procedimientos concretos,
ejemplos y solución de problemas. La instalación/operación del artefacto vive en `09_devops`; esta
sección reúne las guías de uso del producto (alta de bots, configuración de moderación, etc.).

## Documentos de la sección

| Documento | Estado | Contenido |
| --- | --- | --- |
| [guia-configuracion-bot-discord_v1.0.md](guia-configuracion-bot-discord_v1.0.md) | Propuesto | Cómo crear la app y el bot en el portal de Discord, obtener el **Bot Token** correcto, habilitar **intents** privilegiados, configurar **permisos** (OAuth2/Installation), invitar el bot, obtener los IDs y dar de alta el servidor en el panel hasta dejarlo **activo**. Incluye ejemplos, enlaces y solución de problemas (token inválido/401, intents, permisos). |

## Relación con otras secciones

- **09_devops** — instalación, ambientes y secretos (clave maestra, token cifrado); modos de
  ejecución (Simulado/Discord) y scripts locales.
- **02_especificacion_funcional** — CU-05 (reporte a canal), CU-10 (registro de servidor), CU-12
  (prueba de configuración), CU-13 (conexión del gateway).
- **scripts/local/** — `run-discord.bat` (modo Discord), `run-all.bat` (Simulado), `README.md`.
