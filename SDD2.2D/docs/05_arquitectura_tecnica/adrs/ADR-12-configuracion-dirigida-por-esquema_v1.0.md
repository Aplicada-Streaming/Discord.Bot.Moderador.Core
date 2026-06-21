# ADR-12 — Configuración dirigida por esquema (descriptores como fuente única)

**Proyecto:** discord-bots-admin
**Documento:** ADR-12-configuracion-dirigida-por-esquema_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Extensibilidad

## 1. Contexto

El administrador debe ajustar la sensibilidad de la moderación (umbrales, ventanas, modos, opciones) sin conocimiento técnico profundo, apoyándose en un valor por defecto, una leyenda y ejemplos por parámetro (`SOLUTION-INTAKE §4, §5, §17 P.2`). La configuración debe validarse contra límites y debe ser extensible: agregar nuevos parámetros, tipos de regla o tipos de acción no debería requerir duplicar la lógica de validación ni la ayuda en pantalla. Lo motivan NB-05, CU-01, CU-11, CU-16, RN-04, RN-09 y RN-10.

## 2. Decisión

Se adopta configuración dirigida por esquema: cada parámetro configurable se describe con un descriptor único que es la fuente de verdad de su default, sus límites, su leyenda y sus ejemplos. La validación, la ayuda contextual del panel y la aplicación de valores por defecto se derivan del descriptor. Agregar un nuevo descriptor, tipo de regla o tipo de acción es el mecanismo de extensión de la configuración. La frontera reservada de propuesta de configuración (ADR-10) se enchufaría contra esta validación por descriptores.

## 3. Estado

Aceptado el 2026-06-20. Decisión cerrada pre-Sprint 0 según `SOLUTION-INTAKE §17 P.11`. El mecanismo exacto de recarga en caliente (invalidación de caché) queda abierto a Sprint 0.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Descriptores como fuente única (elegida) | Una sola definición por parámetro alimenta validación, default y ayuda; extensible; coherente entre panel y motor | Requiere construir el registro de descriptores y derivar la UI de él |
| Validación y ayuda codificadas por separado en panel y motor | Directo al inicio | Duplica reglas; deriva en inconsistencias entre la ayuda y la validación real (anti-patrón) |
| Configuración libre sin límites declarados | Flexibilidad total | Permite valores inválidos; sin ayuda contextual; viola RN-10 |

## 5. Consecuencias positivas

1. Validación, default y ayuda en pantalla provienen de una sola definición por parámetro (RN-10).
2. La ayuda contextual (leyenda y ejemplos) es consistente con la validación real.
3. Agregar parámetros, tipos de regla o tipos de acción es un punto de extensión declarado (`extensibilidad_v1.0.md`).
4. Habilita el modo simulación por defecto y la prueba de configuración como capacidades dirigidas por esquema (RN-09).

## 6. Consecuencias negativas y trade-offs

1. Construir el registro de descriptores tiene un costo inicial: aceptado a cambio de coherencia y extensibilidad.
2. La recarga en caliente de la configuración requiere invalidar caché: mecanismo exacto abierto a Sprint 0 (`SOLUTION-INTAKE §17 P.11`); no altera la decisión de fondo.
3. Un descriptor mal definido afectaría a todo lo que deriva de él: mitigado con pruebas del registro de descriptores.

## 7. Implementación

El Registro de descriptores (Dominio) define cada parámetro. El Servicio de configuración (Aplicación) valida los comandos del panel contra los descriptores y aplica defaults (RN-10). El panel deriva la ayuda contextual del descriptor. Los puntos de extensión (nuevos descriptores, tipos de regla, tipos de acción) se detallan en `extensibilidad_v1.0.md`, que referencia esta ADR.

## 8. Métricas de validación

- Valores fuera de los límites del descriptor se rechazan en el panel (prueba unitaria por descriptor).
- La ayuda mostrada coincide con los límites aplicados (consistencia descriptor↔UI).
- Agregar un descriptor nuevo no requiere modificar la lógica de validación genérica.

## 9. Referencias

- NB-05.
- CU-01, CU-11, CU-16; relacionada con CU-12 (prueba de configuración) y CU-14 (simulación por defecto).
- RN-04, RN-09, RN-10.
- `SOLUTION-INTAKE §4, §5, §17 P.2, P.11`.
- `extensibilidad_v1.0.md`; ADR-04 (separación de capas), ADR-10 (frontera reservada de propuesta).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
