# ADR-06 — Cumplimiento de la Ley 25.326 de Protección de Datos Personales

**Proyecto:** discord-bots-admin
**Documento:** ADR-06-compliance-ley-25326-proteccion-datos_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Seguridad
**Compliance:** requiere_compliance = true

## 1. Contexto

El sistema almacena datos que identifican a personas en la plataforma de mensajería: identificadores de usuario emisor, copias de mensajes accionados y canales afectados, conservados para la revisión de incidentes (RN-11). El marco legal local aplicable es la Ley 25.326 de Protección de Datos Personales, cuya autoridad de aplicación es la AAIP, con proyectos de reforma en trámite parlamentario que conviene monitorear (`SOLUTION-INTAKE §17 P.5`). Esto impone residencia local de los datos, minimización (guardar solo lo necesario) y retención acotada. Lo motivan el alcance de tratamiento de datos de 00 (`vision-producto §7`, `alcance-proyecto §7`) y RN-11.

## 2. Decisión

Se adopta el cumplimiento de la Ley 25.326 como restricción transversal de diseño: residencia local de los datos en el dispositivo del cliente sin transferencia a terceros; minimización (se guardan solo los identificadores necesarios y la copia de mensajes estrictamente requerida para la revisión); y retención acotada de la evidencia de incidentes. El cifrado del token en reposo (ADR-07) y el hash de la credencial del administrador (ADR-03) complementan esta postura de protección.

## 3. Estado

Aceptado el 2026-06-20. Derivada de la restricción legal declarada en `SOLUTION-INTAKE §17 P.5` y en la categoría 00.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Cumplimiento local con minimización y retención acotada (elegida) | Residencia local sin terceros; superficie de datos mínima; alineada con el marco legal y con el auto-hospedaje | Requiere definir y aplicar una política de retención y purga |
| Conservar todo el historial sin política de retención | Máxima evidencia para auditoría | Excede la minimización; aumenta el riesgo y la exposición legal ante una filtración |
| Externalizar datos a un servicio de terceros (analítica/almacenamiento) | Capacidades adicionales | Rompe la residencia local sin terceros; contradice el alcance y la restricción legal |

## 5. Consecuencias positivas

1. Los datos personales no salen del dispositivo del cliente (residencia local, sin terceros).
2. La superficie de datos personales es mínima: solo identificadores necesarios y la copia de mensajes para revisión.
3. La retención acotada reduce el impacto de una eventual filtración y facilita el cumplimiento.
4. Refuerza la confianza del operador y alinea el diseño con el marco legal vigente.

## 6. Consecuencias negativas y trade-offs

1. La retención acotada implica purgar evidencia antigua, que deja de estar disponible para revisión: aceptado como balance entre auditoría y minimización.
2. Exige mantener una política de retención y su mecanismo de purga: aceptado; se define como configuración y se profundiza en 08/09.
3. Monitoreo del trámite de reforma legal: aceptado como tarea continua, sin impacto estructural inmediato.

## 7. Implementación

La persistencia guarda únicamente los identificadores requeridos (snowflakes como texto) y la copia de mensajes del incidente (RN-11). Se define una política de retención de incidentes (ventana de conservación) y un mecanismo de purga. No hay exportación a terceros. El cifrado de token (ADR-07) y el hash de credencial (ADR-03) protegen los secretos. El detalle de la política de retención y de los procedimientos se trabaja en las categorías 08 (calidad) y 09 (DevOps).

## 8. Métricas de validación

- Cero transferencias de datos a terceros (verificación de red y de configuración).
- La evidencia de incidentes se purga conforme a la ventana de retención configurada.
- Solo se persisten los campos mínimos declarados en `modelo-datos-logico_v1.0.md`.

## 9. Referencias

- RN-11 (integridad de la evidencia del incidente), RN-13, RN-14 (protección de secretos).
- CU-05, CU-06 (evidencia de incidentes).
- `SOLUTION-INTAKE §17 P.5`; `00_contexto/vision-producto_v1.0.md §7`; `00_contexto/alcance-proyecto_v1.0.md §7`.
- ADR-03 (hash de credencial), ADR-07 (cifrado de token).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
