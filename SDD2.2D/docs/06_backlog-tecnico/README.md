# 06 Backlog técnico — discord-bots-admin

**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Versión de la sección:** 1.0
**Fecha:** 2026-06-20
**Autor:** Scrum Master (AG-06)

Punto de entrada navegable del backlog del servicio monolítico de administración y moderación. Esta sección es la bisagra entre el diseño (categorías 01, 02 y 05) y la ejecución (07 plan de sprint, 08 acceptance tests).

## Documentos de la sección

| Documento | Contenido |
| --- | --- |
| [product-backlog_v1.0.md](product-backlog_v1.0.md) | Objetivos y MVP, 8 épicas EP-XX, 19 historias inline con MoSCoW y trazabilidad a CU, métricas de avance y política de refinement. |
| [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) | 7 épicas técnicas, 17 tareas técnicas BT-XX con fuente upstream en 05, y matriz BT↔US↔CU. |
| [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) | Criterios DoR para US (7) y para BT (5), excepciones admitidas y aprobador. |

## Modo de US y BT (umbral §3.3)

- Historias de usuario: 19 US, por debajo del umbral de 20. Modo inline en `product-backlog_v1.0.md`; no se generan archivos individuales bajo `historias-usuario/`.
- Tareas técnicas: 17 BT, por debajo del umbral de 30 (y por encima del piso de 8 para web-monolith). Modo inline en `backlog-tecnico_v1.0.md`; no se generan archivos individuales bajo `tareas-tecnicas/`.

En ambos modos, cada US lleva criterios de aceptación, trazabilidad a CU y DoR check; cada BT lleva fuente upstream, US consumidora y criterios técnicos.

## Resumen de épicas

| EP | Nombre | NB | CU |
| --- | --- | --- | --- |
| EP-01 | Detección de ráfaga y baneo automático | NB-01 | CU-01, CU-02 |
| EP-02 | Baneo con borrado retroactivo | NB-02 | CU-03 |
| EP-03 | Contención de contenido por patrón | NB-03 | CU-04 |
| EP-04 | Incidentes, reporte y desbaneo | NB-04 | CU-05, CU-06, CU-07 |
| EP-05 | Autenticación y registro de servidor | NB-05 | CU-08, CU-09, CU-10 |
| EP-06 | Configuración dirigida por descriptores | NB-05 | CU-11 |
| EP-07 | Operación confiable y validación previa | NB-06 | CU-12, CU-13 |
| EP-08 | Exenciones y prudencia de la moderación | NB-07 | CU-14, CU-15, CU-16 |

## US Must Have del MVP

US-01 (detección de ráfaga), US-02 (configurar umbral y ventana), US-03 (baneo automático), US-04 (borrado retroactivo), US-05 (contenido por patrón), US-06 (reporte a canal privado), US-09 (alta de credenciales), US-10 (autenticación), US-11 (registro de servidor), US-12 (administrar reglas, grupos, eventos y acciones).

## BT prioritarias (Must)

BT-01 (esqueleto de capas), BT-02 (esquema y migración inicial), BT-03 (modo WAL), BT-04 (pipeline), BT-05 (evaluador de contenido), BT-06 (estado de conducta y ráfaga), BT-07 (evaluador de políticas), BT-09 (adaptador del canal de eventos), BT-10 (ejecutor de acciones), BT-12 (autenticación), BT-13 (cifrado de tokens), BT-14 (registro de descriptores), BT-15 (CRUD de configuración).

## DoR vigente

Versión 1.0: 7 criterios para US y 5 para BT, con tres excepciones admitidas (spike, walking skeleton, decisión abierta a Sprint 0) y aprobador Scrum Master (AG-06). La DoR fija el umbral de entrada al sprint; la condición de terminado vive en la Definition of Done de la categoría 08.

## Trazabilidad

- Upstream: 01 (NB-01..NB-07), 02 (CU-01..CU-16, RN, modelo conceptual), 05 (arquitectura, ADR-01..ADR-13, modelo lógico, extensibilidad). Cada US traza a ≥1 CU; cada BT a un componente, ADR, modelo lógico o punto de extensión.
- Downstream: 07 (asignación a sprint y velocity) y 08 (acceptance tests Given/When/Then de cada US Must y Should).

## Estimación

Técnica Fibonacci (1, 2, 3, 5, 8, 13), declarada en la cabecera de los tres documentos y mantenida en todas las US y BT.
