# Catálogo de Necesidades de Negocio

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | necesidades-negocio_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Cantidad de NB | 7 |
| Versión del catálogo de NB | 1.0 |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §3, §4, §8, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU previstas en 02_especificacion_funcional; priorización MoSCoW para 06_backlog-tecnico y 07_plan-sprint |

## 1. Propósito

Este índice consolida las necesidades de negocio del proyecto discord-bots-admin, derivadas del dolor central del solicitante: las ráfagas de spam distribuido que la moderación manual y el filtro nativo de la plataforma no cortan a tiempo. Cada necesidad articula un problema de negocio, su criterio de éxito medible y su prioridad, y se desarrolla en un archivo independiente bajo `necesidades-de-negocio/`. Las necesidades no descienden a flujo funcional ni a decisiones técnicas; eso corresponde a las categorías posteriores.

## 2. Tabla resumen de necesidades de negocio

| ID | Necesidad | Prioridad MoSCoW | CU previstas | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Corte automático de la ráfaga de spam distribuido | Must | CU-01, CU-02 | Propuesto | [NB-01](necesidades-de-negocio/NB-01-corte-automatico-rafaga-distribuida_v1.0.md) |
| NB-02 | Limpieza retroactiva de los mensajes del incidente | Must | CU-03 | Propuesto | [NB-02](necesidades-de-negocio/NB-02-limpieza-retroactiva-mensajes_v1.0.md) |
| NB-03 | Contención de contenido no deseado por patrón | Must | CU-04 | Propuesto | [NB-03](necesidades-de-negocio/NB-03-contencion-contenido-no-deseado_v1.0.md) |
| NB-04 | Trazabilidad de incidentes y control de falsos positivos | Must | CU-05, CU-06, CU-07 | Propuesto | [NB-04](necesidades-de-negocio/NB-04-trazabilidad-incidentes-falsos-positivos_v1.0.md) |
| NB-05 | Configuración autónoma de la moderación por el administrador | Must | CU-08, CU-09, CU-10, CU-11 | Propuesto | [NB-05](necesidades-de-negocio/NB-05-configuracion-autonoma-moderacion_v1.0.md) |
| NB-06 | Operación confiable y validación previa de la moderación | Should | CU-12, CU-13 | Propuesto | [NB-06](necesidades-de-negocio/NB-06-operacion-confiable-validacion-previa_v1.0.md) |
| NB-07 | Mitigación del riesgo de moderación errónea | Should | CU-14, CU-15, CU-16 | Propuesto | [NB-07](necesidades-de-negocio/NB-07-mitigacion-moderacion-erronea_v1.0.md) |

## 3. Mapa de dependencias entre NB

Las dependencias son acíclicas y ninguna NB depende de más de tres otras. NB-01 y NB-05 son las necesidades raíz: la primera aporta el corte y la contención; la segunda, la superficie de administración.

| NB | Depende de | Es prerequisito de |
| --- | --- | --- |
| NB-01 | — (raíz) | NB-02, NB-03, NB-04, NB-07 |
| NB-02 | NB-01 | — |
| NB-03 | NB-01 | — |
| NB-04 | NB-01, NB-05 | — |
| NB-05 | — (raíz) | NB-04, NB-06, NB-07 |
| NB-06 | NB-05 | — |
| NB-07 | NB-01, NB-05 | — |

Orden de lectura sugerido por dependencias: NB-01 y NB-05 primero (raíces), luego NB-02, NB-03 y NB-04 (detección, limpieza y trazabilidad del núcleo), y finalmente NB-06 y NB-07 (confiabilidad operativa y prudencia de la moderación).

## 4. Trazabilidad agregada

- Upstream: las siete NB derivan del problema y el alcance Must Have de SOLUTION-INTAKE §1 y §4, de las métricas de éxito de §8 y de los riesgos de §11, y son coherentes con `vision-producto_v1.0.md` y `alcance-proyecto_v1.0.md` de la categoría 00.
- Downstream: cada NB declara en su §7 las CU previstas que la implementarán en 02_especificacion_funcional, todas con estado `a generar`. La prioridad MoSCoW de cada NB ordena el backlog técnico (06) y el plan de sprint (07). Los criterios de éxito de cada NB alimentan los criterios de aceptación de la categoría 08.
- Nota: este proyecto no declara uso de modelos de lenguaje en v1 (la frontera de propuesta de configuración asistida solo se reserva, no se construye), por lo que ninguna NB se enlaza a la categoría 04.

## 5. Cobertura del alcance MoSCoW del intake

- Must Have v1: cubierto por NB-01 (detección de ráfaga y baneo), NB-02 (borrado retroactivo), NB-03 (contenido por patrón), NB-04 (reporte de incidentes) y NB-05 (panel, cuenta administradora, configuración con ayuda contextual).
- Should Have v1: cubierto por NB-06 (prueba de configuración, reconexión, estado de conexión) y NB-07 (modo simulación, exenciones, antirrebote, y la revisión y desbaneo se trazan también desde NB-04).
- Could Have y Won't Have v1: no generan NB propias en este catálogo; el asistente de propuesta de configuración por modelo de lenguaje queda como frontera reservada y fuera de alcance v1 según el intake §4 y la visión.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Catálogo inicial con siete NB derivadas del intake y de la categoría 00 |
| 1.0 | 2026-06-20 | Alineación de la columna "CU previstas" de la tabla resumen §2 con la numeración consolidada del 02 (NB-05 → CU-08..CU-11; NB-06 → CU-12/CU-13; NB-07 → CU-14..CU-16), corrigiendo el desfasaje detectado por el audit final (H-01) y eliminando la reutilización de CU-11 y CU-13 entre NB |
