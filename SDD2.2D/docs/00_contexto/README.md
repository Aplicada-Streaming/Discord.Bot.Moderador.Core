# 00 Contexto del producto — discord-bots-admin

Esta carpeta reúne los documentos de contexto a nivel solución del proyecto **Administrador de Bots Moderador para Discord** (`discord-bots-admin`, nombre de código `DiscordModeradorBot.Servicio`, tipo `web-monolith`). Define el porqué del sistema, su alcance y el orden de construcción, y es el inicio de la cadena de trazabilidad: alimenta las categorías 01, 02, 03, 05, 06, 07 y 11.

## Documentos de la sección

| Documento | Propósito | Estado | Orden de lectura |
| --- | --- | --- | --- |
| `vision-producto_v1.0.md` | Problema de negocio, audiencia, propuesta de valor, visión a 3 años, objetivos SMART, métricas, riesgos y glosario | Propuesto | 1 |
| `alcance-proyecto_v1.0.md` | Qué entra y qué no entra, supuestos, restricciones y criterios de aceptación del proyecto | Propuesto | 2 |
| `roadmap-producto_v1.0.md` | Fases de construcción por rebanadas verticales, dependencias y criterios de transición | Propuesto | 3 |

## Documentos omitidos y su motivo

Según la regla §2.2 de `00_rules_contexto.md`, los documentos no aplicables a este proyecto se omiten y el motivo se declara aquí:

- `compatibilidad-plataformas_v1.0.md`: omitido para `web-monolith`. Se omite porque los navegadores target del panel (SOLUTION-INTAKE §17 P.9) son evergreen, no legacy. Las plataformas target del sistema (sistema operativo y runtime de despliegue, navegadores soportados) se documentan en `alcance-proyecto_v1.0.md` (§7 Restricciones) y se detallan en la categoría 09 DevOps.
- `acuerdo-equipo_v1.0.md`: omitido porque el equipo es de un solo desarrollador (`equipo_n = 1`, SOLUTION-INTAKE §2: el implementador único es Fernando). Sin equipo de más de dos personas no hay ceremonias, branching strategy ni SLA de respuesta interno que acordar. Las convenciones técnicas individuales (versionado, branching, commits) viven en el bloque técnico del intake (§17 P.7) y se trabajan en las categorías 09 DevOps y 10 Developer Guide.

## Stakeholders del proyecto

| Rol | Categoría | Responsabilidad principal |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Aprueba el alcance, define la sensibilidad de la moderación y opera el sistema |
| Administrador del sistema (rol único de la aplicación) | Beneficiario / operador | Registra servidores, configura reglas y revisa incidentes |
| Fernando | Implementador | Construcción y mantenimiento del sistema |
| Comunidades de Discord moderadas | Beneficiario indirecto | Reciben un servidor libre de spam |

## Insumos upstream

- `SOLUTION-INTAKE-discord-bots-admin_v1.0.md` (Parte A §1 a §12, §13 composición, §15 delivery, §17 P.9 plataformas).
- `SOLUTION-MANIFEST-discord-bots-admin_v1.0.md` (manifiesto canónico de la solución).
