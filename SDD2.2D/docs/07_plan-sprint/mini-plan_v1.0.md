# Mini-plan de sprints — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** mini-plan_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Scrum Master (AG-07)

Plan único condensado para un proyecto de un solo desarrollador. Por la regla §2.2 de las reglas de la categoría 07 (equipo de 1 dev), este mini-plan sustituye a los cuatro artefactos completos (`plan-iteracion-sprint-XX`, `template-sprint-review`, `template-sprint-retrospectiva` y `velocidad-equipo`) y condensa el objetivo del sprint, la secuencia de rebanadas verticales, la lista de items del primer sprint, los riesgos, la trazabilidad y la bitácora de avance. El catálogo priorizado de historias vive en 06 (`product-backlog_v1.0.md`), la vista técnica en `backlog-tecnico_v1.0.md` y la condición de entrada al sprint en `definition-of-ready_v1.0.md`.

## 1. Información general

- Composición del equipo: un desarrollador (Fernando) con asistencia de IA. Único rol que diseña, construye, prueba y opera. No hay ceremonias de equipo formales; el refinamiento es de autocuraduría guiada según §5 del product-backlog.
- Unidad de estimación: story points en escala Fibonacci (1, 2, 3, 5, 8, 13), heredada de 06. La primera estimación es de calibración y se reajusta tras el walking skeleton, dado que no hay historial de velocity todavía.
- Capacidad: no se declara capacidad fija en horas. Al ser un único dev sin dedicación contractual, el compromiso de cada rebanada se acota a un alcance demostrable end-to-end antes que a un techo de puntos. La estimación de cada rebanada se usa como referencia relativa de tamaño, no como compromiso de fecha.
- Fecha objetivo: sin fecha objetivo (intake §10). El desarrollo es incremental por sprints, sin compromiso contractual que justifique un deadline fijo.
- Duración de sprint sugerida: el primer sprint (walking skeleton) se acota a una semana, sprint corto justificado por §3.2 (arranque y validación del recorrido completo para reducir riesgo). Las rebanadas siguientes vuelven a la duración estándar de dos semanas, salvo justificación puntual registrada en la bitácora.
- Cadencia: una rebanada vertical por sprint. Cada rebanada atraviesa panel, persistencia, bot y motor de evaluación, y es demostrable de punta a punta (intake §15).

## 2. Objetivo del primer sprint (sprint goal)

Disponer de un walking skeleton end-to-end que permite registrar un servidor con su credencial cifrada, recibir un mensaje del canal de eventos, evaluar la regla de ráfaga distribuida y, en modo simulación, reportar la acción que se ejecutaría.

## 3. Secuencia de rebanadas verticales

Vertical slicing del intake §15: la rebanada 1 es el walking skeleton; las siguientes se agregan una por una, cada una completa y demostrable end-to-end. El orden topológico es trivial (un único proyecto); el criterio rector y bloqueante es que cada rebanada entregue valor demostrable de punta a punta y ninguna rompa el camino completo. Cada rebanada mapea a las US y BT reales de 06 y declara los CU y NB que avanzan.

| Rebanada | Sprint | Foco demostrable end-to-end | US (06) | BT (06) | CU que avanzan | NB que avanzan |
| --- | --- | --- | --- | --- | --- | --- |
| R1 | Sprint 01 | Walking skeleton: registrar servidor con credencial cifrada, recibir un mensaje, evaluar ráfaga distribuida y reportar en modo simulación | US-11, US-01, US-02, US-16 | BT-01, BT-02, BT-13, BT-09, BT-04, BT-06, BT-07 (versión mínima de cada uno), BT-14 | CU-10, CU-01, CU-14 | NB-01, NB-05, NB-07 |
| R2 | Sprint 02 | Baneo automático del emisor con borrado retroactivo y reporte real del incidente a canal privado | US-03, US-04, US-06 | BT-10, BT-08, BT-03 | CU-02, CU-03, CU-05 | NB-01, NB-02, NB-04 |
| R3 | Sprint 03 | Reglas de contenido por expresión regular o palabras clave que contienen al emisor | US-05 | BT-05 | CU-04 | NB-03 |
| R4 | Sprint 04 | Revisión de incidentes desde el panel y desbaneo de un falso positivo, sobre cuenta administradora autenticada | US-09, US-10, US-07, US-08 | BT-12, BT-16 | CU-08, CU-09, CU-06, CU-07 | NB-04, NB-05, NB-07 |
| R5 | Sprint 05 | Exenciones por rol, usuario o canal de confianza que descartan al emisor antes de evaluar reglas | US-17 | BT-15 | CU-15 | NB-07 |
| R6 | Sprint 06 | Acciones adicionales de moderación (timeout, expulsión y rol) ejecutadas en orden por el ejecutor de acciones | parte de US-12 (catálogo de acciones); habilitada por US-03 | BT-10 (extensión del catálogo de acciones) | CU-11, CU-02 | NB-01, NB-05 |
| R7 | Sprint 07 | Configuración dirigida por descriptores con ayuda contextual por parámetro y prueba de configuración previa a activar el servidor | US-12, US-13, US-14, US-15, US-18 | BT-15, BT-14, BT-11, BT-17 | CU-11, CU-12, CU-13, CU-16 | NB-05, NB-06, NB-07 |

Nota de secuencia: la rebanada 1 toma cada BT en su versión mínima suficiente para demostrar el recorrido (camino feliz), por la excepción de walking skeleton de la DoR (§3 de `definition-of-ready_v1.0.md`). Las rebanadas posteriores completan los criterios de borde de las mismas BT. La US-19 (Could, en Borrador) no se compromete en ninguna rebanada de esta versión del plan; queda en backlog hasta su refinamiento.

## 4. Lista de items del primer sprint (Rebanada 1 — walking skeleton)

Items comprometidos en su versión mínima demostrable end-to-end. Las BT de fundación entran completas; las BT de pipeline e integración entran en la versión mínima suficiente para el camino feliz, con los criterios de borde diferidos a rebanadas posteriores.

| ID | Tipo | Descripción corta | Prioridad | Estimación | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-01 | Backlog técnico | Esqueleto de capas y composición de dependencias | Alta | 5 | Pendiente |
| BT-02 | Backlog técnico | Esquema relacional embebido y migración inicial | Alta | 5 | Pendiente |
| BT-13 | Backlog técnico | Cifrado de tokens en reposo con clave maestra | Alta | 5 | Pendiente |
| US-11 | Historia | Registrar un servidor con su credencial de acceso | Alta | 5 | Pendiente |
| BT-09 | Backlog técnico | Adaptador del canal de eventos y normalización de mensajes (mínimo) | Alta | 8 | Pendiente |
| BT-14 | Backlog técnico | Registro de descriptores y validación genérica (mínimo para umbral y ventana) | Alta | 5 | Pendiente |
| US-02 | Historia | Configurar umbral de canales y ventana de detección | Alta | 3 | Pendiente |
| BT-04 | Backlog técnico | Pipeline de evaluación y orquestación de etapas (mínimo) | Alta | 8 | Pendiente |
| BT-06 | Backlog técnico | Estado de conducta en memoria y evaluador de ráfaga | Alta | 8 | Pendiente |
| US-01 | Historia | Detectar ráfaga distribuida por canales distintos | Alta | 8 | Pendiente |
| BT-07 | Backlog técnico | Evaluador de políticas por prioridad con primera coincidencia (mínimo) | Alta | 5 | Pendiente |
| US-16 | Historia | Ejecutar una política en modo simulación (reporte de lo que se ejecutaría) | Media | 3 | Pendiente |

Total de puntos referenciales de la rebanada: 68 SP nominales del backlog. Estos 68 SP son los nominales heredados del backlog de 06 (tamaño relativo de cada ítem), no un compromiso de capacidad ni de fecha del sprint. Al tratarse de un walking skeleton con BT de integración y pipeline tomadas en versión mínima, el esfuerzo real comprometido es una fracción del nominal; el remanente de esas BT se traslada a R2 y R3 como criterios de borde. El total se usa como referencia de tamaño relativo, no como techo de velocity (sin historial todavía).

## 5. Definition of Done aplicada

La condición de terminado de cada item se rige por la DoD canónica del proyecto, que vive en la categoría 08 y todavía no está generada. Este mini-plan no redefine la DoD: la referencia como pendiente de 08 y, al cerrar la rebanada 1, cada US y BT comprometida se cierra contra esa DoD canónica una vez disponible. Criterio específico de la rebanada 1, en línea con la excepción de walking skeleton de la DoR: el recorrido completo (registrar servidor, recibir mensaje, evaluar ráfaga, reportar en modo simulación) es demostrable end-to-end por el camino feliz; los criterios de borde de las BT tomadas en versión mínima se completan en rebanadas posteriores y no condicionan el cierre de la rebanada 1.

## 6. Riesgos del primer sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El estado de conducta en memoria (BT-06) no detecta de forma confiable la cuenta de canales distintos en la ventana, base del discriminador de ráfaga | Media | Alto | Spike acotado de calibración al inicio de la rebanada con mensajes simulados (DoR criterio 7); operar sobre el descriptor (US-02) para ajustar umbral y ventana sin tocar la lógica; cobertura del módulo de detección verificable de forma aislada (BT-04) |
| Subestimación del esfuerzo del walking skeleton por ser el sprint inaugural sin historial de velocity | Alta | Medio | BT de integración y pipeline tomadas en versión mínima (camino feliz), con criterios de borde diferidos a R2/R3; sprint corto de una semana (§3.2) para obtener feedback temprano; reajuste de la estimación al cierre, registrado en la bitácora |
| Decisión abierta a Sprint 0 sobre la familia de hash y la herramienta de migración bloquea el arranque (DoR §3) | Baja | Medio | El walking skeleton no depende del hash de contraseña (US-09/US-10 entran en R4); la credencial del servidor usa cifrado simétrico (BT-13) cuya elección puntual no bloquea el camino feliz; se registra como supuesto a confirmar dentro del sprint |
| El presupuesto de memoria por conexión de la plataforma objetivo (≤ 8 MB por contexto, BT-09) condiciona el adaptador del canal de eventos | Baja | Alto | Medir el consumo del adaptador en la rebanada 1 con un único contexto (v1 es mono-servidor); diferir la operación multi-contexto y validar el presupuesto antes de habilitar más contextos |

## 7. Trazabilidad

Qué CU y NB avanzan por rebanada y qué ADR gobiernan las decisiones técnicas implicadas. Los CU son de 02; las NB de 01; los ADR de 05. Sin invención de identificadores.

| Rebanada | CU que avanzan | NB que avanzan | ADR que gobiernan |
| --- | --- | --- | --- |
| R1 | CU-10, CU-01, CU-14 | NB-01, NB-05, NB-07 | ADR-01, ADR-04, ADR-02, ADR-07, ADR-13, ADR-09, ADR-12 |
| R2 | CU-02, CU-03, CU-05 | NB-01, NB-02, NB-04 | ADR-08, ADR-13, ADR-09, ADR-02 |
| R3 | CU-04 | NB-03 | ADR-04, ADR-08 |
| R4 | CU-08, CU-09, CU-06, CU-07 | NB-04, NB-05, NB-07 | ADR-03, ADR-06, ADR-01 |
| R5 | CU-15 | NB-07 | ADR-12, ADR-13 |
| R6 | CU-11, CU-02 | NB-01, NB-05 | ADR-08, ADR-13 |
| R7 | CU-11, CU-12, CU-13, CU-16 | NB-05, NB-06, NB-07 | ADR-12, ADR-13, ADR-09, ADR-05, ADR-11 |

Cobertura: las siete NB (NB-01..NB-07) y los dieciséis CU (CU-01..CU-16) quedan cubiertos a lo largo de las siete rebanadas. Cada US y cada BT citada en este plan existe en 06 con el identificador exacto.

Downstream: cada US comprometida dispara la creación o actualización de su caso de aceptación en 08; las rebanadas R7 (BT-17 empaquetado y servicio) y, en menor medida, R4 (panel autenticado) tocan pipeline y despliegue y referencian la actualización prevista en 09 cuando se ejecuten.

## 8. Bitácora de avance semanal

Estructura lista para completar a medida que avanza cada sprint. Una fila por semana; al cierre de cada rebanada se registra el reajuste de estimación.

| Semana | Sprint / Rebanada | Foco de la semana | Hecho | Bloqueos |
| --- | --- | --- | --- | --- |
| S1 | Sprint 01 / R1 | Fundaciones, cifrado de credencial y registro de servidor (BT-01, BT-02, BT-13, US-11) | | |
| S2 | Sprint 01 / R1 | Adaptador mínimo, pipeline y evaluador de ráfaga, reporte en simulación (BT-09, BT-04, BT-06, BT-07, US-01, US-02, US-16) | | |
| ... | ... | ... | | |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Mini-plan inicial para proyecto de 1 dev: sprint goal del walking skeleton, secuencia de siete rebanadas verticales mapeadas a US/BT/CU/NB de 06, lista de items de la rebanada 1, DoD por referencia a 08, cuatro riesgos con mitigación, trazabilidad por rebanada a CU/NB/ADR y bitácora de avance. Sustituye a los cuatro artefactos de equipo por la regla §2.2. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: descripción corta de BT-13 alineada al título canónico del backlog técnico de 06; aclaración de que los SP de §4 son nominales del backlog, no compromiso de capacidad. |
