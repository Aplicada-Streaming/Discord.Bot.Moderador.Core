# 07 Plan de sprint — discord-bots-admin

Índice navegable de la categoría 07 (planificación de sprints) del proyecto discord-bots-admin. La categoría 07 traduce el backlog priorizado de 06 en un plan de ejecución iterativa, declara qué se construye en la ventana de cada sprint y mantiene la trazabilidad a los CU de 02 y a las necesidades de negocio de 01.

## Modo aplicado: mini-plan (proyecto de 1 dev)

Este proyecto es de un único desarrollador (Fernando, con asistencia de IA). Por la regla §2.2 de `SDD2.2D/devs/rules/07_rules_plan_sprint.md`, se aplica el modo simplificado: un único `mini-plan_v1.0.md` sustituye a los cuatro artefactos completos pensados para equipos de más de un desarrollador.

En consecuencia, en esta sección se omiten de forma explícita y deliberada:

- `plan-iteracion-sprint-XX_v1.0.md` (un plan por sprint),
- `template-sprint-review_v1.0.md`,
- `template-sprint-retrospectiva_v1.0.md`,
- `velocidad-equipo_v1.0.md`.

Esos cuatro artefactos son obligatorios solo para equipos de dos o más desarrolladores. Su contenido relevante para un solo dev (sprint goal, lista de items y bitácora de avance) queda condensado en el mini-plan.

## Documentos de la sección

| Documento | Descripción |
| --- | --- |
| [`mini-plan_v1.0.md`](mini-plan_v1.0.md) | Plan único condensado: información general, sprint goal del primer sprint, secuencia de siete rebanadas verticales mapeadas a US/BT/CU/NB, lista de items del primer sprint, DoD por referencia a 08, riesgos con mitigación, trazabilidad por rebanada y bitácora de avance semanal. |

## Vínculos a upstream y downstream

- Upstream 06 (backlog técnico): [`product-backlog_v1.0.md`](../06_backlog-tecnico/product-backlog_v1.0.md), [`backlog-tecnico_v1.0.md`](../06_backlog-tecnico/backlog-tecnico_v1.0.md), [`definition-of-ready_v1.0.md`](../06_backlog-tecnico/definition-of-ready_v1.0.md).
- Upstream 02 (especificación funcional): casos de uso CU-01..CU-16 en [`casos-de-uso/`](../02_especificacion_funcional/casos-de-uso/).
- Upstream 01 (necesidades de negocio): NB-01..NB-07 en [`necesidades-de-negocio/`](../01_necesidades_negocio/necesidades-de-negocio/).
- Upstream 05 (arquitectura técnica): ADR-01..ADR-13 en [`adrs/`](../05_arquitectura_tecnica/adrs/).
- Downstream 08: acceptance tests de las US comprometidas y DoD canónica (referenciada como pendiente desde el mini-plan).
- Downstream 09: actualizaciones de pipeline o despliegue cuando una rebanada las introduzca (en particular la rebanada de empaquetado y servicio).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | README inicial de la sección 07: declara el modo mini-plan para proyecto de 1 dev, la omisión justificada de los cuatro artefactos de equipo y el índice de documentos con sus vínculos upstream y downstream. |
