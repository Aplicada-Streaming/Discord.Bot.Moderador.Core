# ADR-10 — Omisión de la categoría de contratos de prompts AI (sin LLM en v1)

**Proyecto:** discord-bots-admin
**Documento:** ADR-10-omision-contratos-prompts-ai_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Estilo
**Bandera:** usa_llm = false

## 1. Contexto

La cadena SDD prevé que la categoría 04 (contratos de prompts) sea upstream de 05 cuando algún componente delega lógica en un modelo de lenguaje. Este sistema no usa modelos de lenguaje en v1: el alcance excluye explícitamente el asistente que propone configuraciones por prompt, y solo se reserva la frontera para enchufarlo más adelante (`SOLUTION-INTAKE §4 Won't Have, §9, §17 P.11`; `02_especificacion_funcional §7`; catálogo de NB §4 nota). Para evitar una omisión silenciosa, se registra la decisión de omitir la categoría 04 y de reservar la frontera sin construirla. Lo motivan el alcance excluido de 00/01/02 y la frontera reservada de §17 P.11.

## 2. Decisión

Se omite la categoría de contratos de prompts AI porque el sistema no incorpora modelos de lenguaje en v1 (`usa_llm = false`). Se reserva únicamente la frontera de propuesta de configuración validable (la IA propondría, el sistema valida, el humano previsualiza, simula y confirma antes de aplicar), documentada como punto de extensión en `extensibilidad_v1.0.md`, sin diseño ni construcción en v1. Ninguna ADR ni componente de 05 referencia un contrato de prompt.

## 3. Estado

Aceptado el 2026-06-20. Coherente con el alcance excluido y con la frontera reservada de `SOLUTION-INTAKE §17 P.11`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Omitir 04 y reservar la frontera, documentando la omisión (elegida) | Coherente con el alcance v1; no introduce dependencia de un modelo de lenguaje; deja un punto de extensión limpio | Requiere documentar la omisión para no parecer un olvido |
| Diseñar contratos de prompt en v1 | Adelanta la capacidad futura | Fuera de alcance; introduce dependencia y complejidad no requeridas; viola §4 Won't Have |
| No mencionar la categoría 04 | Menos documentación | Omisión silenciosa; rompe la trazabilidad y la auditabilidad de la decisión |

## 5. Consecuencias positivas

1. El alcance v1 permanece acotado, sin dependencia de un modelo de lenguaje.
2. La frontera reservada queda documentada como punto de extensión, lista para una versión futura.
3. La decisión de omitir es explícita y auditable, no un olvido.

## 6. Consecuencias negativas y trade-offs

1. La capacidad de propuesta asistida no existe en v1: aceptado por alcance (§4 Won't Have).
2. La frontera reservada podría requerir ajustes cuando se diseñe de verdad: aceptado; solo se reserva, no se compromete su forma final.

## 7. Implementación

No se genera ningún artefacto de la categoría 04 ni contrato de prompt. La frontera `PropuestaDeConfiguracion` se describe como punto de extensión reservado en `extensibilidad_v1.0.md`, sin implementación. El modelo de validación de configuración por descriptores (ADR-12) es el sustrato contra el cual se enchufaría a futuro.

## 8. Métricas de validación

- Cero referencias a contratos de prompt en los artefactos de 05.
- La frontera reservada figura en `extensibilidad_v1.0.md` sin código asociado en v1.

## 9. Referencias

- `SOLUTION-INTAKE §4 (Won't Have), §9, §17 P.11`.
- `02_especificacion_funcional §7`; `01_necesidades_negocio §4 (nota: ninguna NB se enlaza a 04)`.
- `extensibilidad_v1.0.md` (frontera reservada).
- ADR-12 (configuración dirigida por esquema, sustrato de la futura propuesta).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
