# Roadmap de Producto

**Proyecto:** discord-bots-admin
**Documento:** roadmap-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Product Manager Senior (AG-00)
**Trazabilidad upstream:** SOLUTION-INTAKE §4, §6, §15
**Trazabilidad downstream:** 06_backlog, 07_plan-sprint

## 1. Propósito

Este documento ordena la construcción del producto en fases que entregan valor demostrable de punta a punta. El criterio rector, tomado del esquema de delivery del intake, es el corte vertical: cada fase entrega una rebanada funcional completa que atraviesa panel, persistencia, bot y motor de evaluación, en lugar de construir por capas horizontales. La primera fase es un walking skeleton end-to-end demostrable y ninguna fase posterior rompe el camino completo. Las fases se derivan del orden de rebanadas declarado en SOLUTION-INTAKE §15 y de la priorización MoSCoW de §4.

## 2. Fases del producto

| Fase | Objetivo | Épicas asociadas | Sprints estimados | Entregable | Release target |
| --- | --- | --- | --- | --- | --- |
| F1 — Walking skeleton | Registrar un servidor con su credencial, recibir un mensaje del canal de eventos, evaluar la regla de ráfaga distribuida y, en modo simulación, reportar la acción que se ejecutaría | Alta de servidor; conexión al canal de eventos; motor de evaluación inicial; detección de ráfaga distribuida; modo simulación | 1 a 2 | Camino end-to-end demostrable en simulación | v0.1 |
| F2 — Baneo con borrado retroactivo | Ejecutar el baneo real con borrado retroactivo de los mensajes del usuario dentro de la ventana configurable, y dejar el incidente registrado | Acción de baneo; borrado retroactivo; registro de incidentes; reporte a canal privado | 1 | Corte real de la ráfaga con limpieza de mensajes | v0.2 |
| F3 — Reglas de contenido | Detectar contenido no deseado en un mensaje por expresión regular y disparar la acción | Reglas de contenido; predicados sin estado; integración con el pipeline de evaluación | 1 | Moderación por contenido operativa | v0.3 |
| F4 — Revisión de incidentes y desbaneo | Revisar desde el panel los incidentes y los mensajes accionados, y revertir un baneo (desbaneo) | Panel de incidentes; copia de mensajes para revisión; desbaneo | 1 | Ciclo de revisión y reversión de falsos positivos | v0.4 |
| F5 — Exenciones | Excluir de la moderación a roles, usuarios y canales de confianza | Exenciones por rol, usuario y canal; filtro previo en el pipeline | 1 | Staff y canales de confianza protegidos | v0.5 |
| F6 — Acciones adicionales | Aplicar timeout, expulsión y asignación o quita de rol como alternativas al baneo | Catálogo de acciones; reglas de conducta por volumen; antirrebote por usuario | 1 a 2 | Repertorio de acciones de moderación completo | v0.6 |
| F7 — Configuración por descriptores | Administrar todos los parámetros desde el panel con valor por defecto, leyenda y ejemplos, y prueba de configuración al registrar un servidor | Configuración dirigida por descriptores; ayuda contextual; prueba de configuración | 1 a 2 | Configuración asistida completa y validación previa a la activación | v1.0 |

Nota: las estimaciones de sprints son orientativas; el dimensionamiento fino se realiza en la categoría 07 plan de sprint. No hay fecha objetivo contractual; el avance se mide por iteración y por criterios de transición, no por calendario.

## 3. Matriz fase → épica → sprint → release

| Fase | Épica representativa | Sprint (orientativo) | Release |
| --- | --- | --- | --- |
| F1 | Walking skeleton de detección en simulación | S1 | v0.1 |
| F2 | Baneo con borrado retroactivo y registro de incidente | S2 | v0.2 |
| F3 | Reglas de contenido por expresión regular | S3 | v0.3 |
| F4 | Panel de incidentes y desbaneo | S4 | v0.4 |
| F5 | Exenciones de confianza | S5 | v0.5 |
| F6 | Acciones adicionales y reglas de conducta | S6 | v0.6 |
| F7 | Configuración por descriptores y prueba de configuración | S7 | v1.0 |

El mapeo definitivo de épicas a sprints y la apertura de cada épica en historias se trabajan en las categorías 06 backlog y 07 plan de sprint.

## 4. Dependencias entre fases

- F1 es prerequisito de todas: establece el camino completo (alta de servidor, canal de eventos, motor, simulación) sobre el que se montan las demás rebanadas.
- F2 depende de F1: sin el motor de detección y el registro de incidentes no hay nada que banear ni que registrar.
- F3 depende de F1 (pipeline de evaluación) y se apoya en F2 para ejecutar acciones reales sobre el contenido detectado.
- F4 depende de F2: revisa y revierte los incidentes y baneos que F2 produce.
- F5 se apoya en F1 (pipeline) y aplica antes de F2 y F3 en tiempo de ejecución, pero se construye después porque no bloquea el camino completo.
- F6 depende de F1 (motor y pipeline) y reutiliza el registro de incidentes de F2.
- F7 depende de todas las anteriores: consolida la administración por descriptores de los parámetros que cada fase fue introduciendo y agrega la prueba de configuración previa a la activación.

El orden topológico de construcción es trivial al tratarse de un único proyecto; el criterio bloqueante es que cada fase entregue valor demostrable end-to-end y ninguna rompa el camino completo.

## 5. Criterios de transición entre fases

| Fase origen | Fase destino | Criterios verificables |
| --- | --- | --- |
| F1 | F2 | - [ ] Se registra un servidor con su credencial desde el panel<br>- [ ] El sistema recibe mensajes del canal de eventos de un servidor registrado<br>- [ ] El motor evalúa la regla de ráfaga distribuida sobre mensajes simulados<br>- [ ] En modo simulación, el sistema reporta la acción que ejecutaría sin ejecutarla |
| F2 | F3 | - [ ] El baneo real corta al emisor de la ráfaga<br>- [ ] El borrado retroactivo elimina los mensajes del usuario dentro de la ventana configurada<br>- [ ] El incidente queda registrado con su copia de mensajes y canales afectados<br>- [ ] El reporte llega al canal privado configurado |
| F3 | F4 | - [ ] Una regla de contenido por expresión regular detecta un mensaje no deseado<br>- [ ] La detección de contenido dispara la acción configurada<br>- [ ] La regla de contenido convive con la de ráfaga sin romper el pipeline |
| F4 | F5 | - [ ] El panel lista los incidentes con su copia de mensajes y canales afectados<br>- [ ] El administrador revierte un baneo (desbaneo) desde el panel<br>- [ ] El desbaneo queda registrado sin restaurar los mensajes borrados |
| F5 | F6 | - [ ] Una exención por rol, usuario o canal excluye al sujeto de la moderación<br>- [ ] Un sujeto exento no dispara baneo aunque cumpla el patrón<br>- [ ] Las exenciones se administran desde el panel |
| F6 | F7 | - [ ] Las acciones de timeout, expulsión y rol se ejecutan correctamente<br>- [ ] La regla de conducta por volumen en un canal dispara la acción configurada<br>- [ ] El antirrebote evita repetir acciones sobre el mismo usuario durante una ráfaga |
| F7 | Cierre v1.0 | - [ ] Cada parámetro se administra desde el panel con valor por defecto, leyenda y ejemplos<br>- [ ] La prueba de configuración valida credencial, permisos y canales antes de activar<br>- [ ] La prueba advierte sobre jerarquía de roles o permisos faltantes<br>- [ ] Todas las capacidades Must Have del alcance están operativas end-to-end |

## 6. Trazabilidad

- Upstream: SOLUTION-INTAKE §4 (alcance funcional MoSCoW que prioriza las capacidades por fase), §6 (flujos típicos de moderación, configuración y revisión que ordenan las rebanadas), §15 (esquema de descomposición y delivery por rebanadas verticales del que se derivan las fases).
- Downstream: alimenta 06_backlog (apertura de cada fase en épicas e historias y su Definition of Ready) y 07_plan-sprint (asignación de fases a sprints, dimensionamiento y secuencia de release). Cada fase de este roadmap se mapea a épicas conocidas o pendientes de definir en la categoría 06.
