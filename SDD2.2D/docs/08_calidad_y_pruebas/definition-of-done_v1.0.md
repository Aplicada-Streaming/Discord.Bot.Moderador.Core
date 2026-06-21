# Definition of Done — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** definition-of-done_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

Este documento es la fuente canónica de la Definition of Done del proyecto. La Definition of Ready de 06 (`definition-of-ready_v1.0.md` §5) y el mini-plan de 07 (`mini-plan_v1.0.md` §5) la referencian como pendiente; este documento la materializa. Los planes de sprint referencian esta DoD; no la redefinen (regla §4.8 de 08). Cada criterio es verificable mecánicamente: responde a "¿cómo se valida?" con un comando, un check del pipeline o una métrica del reporte. Los gates G1–G5 son los de `estrategia-calidad_v1.0.md` §3 (`SOLUTION-INTAKE §17 P.6/P.8`).

## 1. DoD por capa

### 1.1 DoD de una historia de usuario (US)

- [ ] El código que implementa la US compila sin errores (gate G1 del pipeline).
- [ ] Cada criterio de aceptación Given/When/Then de la US (CA-01..CA-04 del CU asociado en 02) tiene al menos un caso de prueba en verde en `casos-prueba-referenciales_v1.0.md`, incluyendo al menos un camino feliz y un borde para US Must y Should (gate G2; verificable contra la matriz CU↔Tests).
- [ ] La cobertura por capa de los componentes tocados cumple su umbral de `estrategia-testing_v1.0.md` §2; si la US toca el módulo de detección, ese módulo queda ≥ 90 % líneas (gate G3; reporte de cobertura por capa).
- [ ] La matriz `matriz-cobertura-pruebas_v1.0.md` queda actualizada: el TC de la US figura con su estado real, sin filas en "Pendiente" que ya estén implementadas (verificable por inspección de la matriz al cierre).
- [ ] El formato del código no presenta diferencias con el canónico (gate G4) y el análisis estático no introduce warnings nuevos (gate G5).
- [ ] La US referencia al menos un CU de 02 y los descriptores de los parámetros que toca, si aplica (heredado de la DoR de 06; verificable en la trazabilidad de la US).

### 1.2 DoD de una tarea técnica (BT)

- [ ] El componente de la BT compila y se prueba sin la capa de la que no debe depender; en particular, los componentes del Dominio se prueban sin infraestructura (gate G1 + ejecución de la suite unitaria del Dominio; criterio de BT-01).
- [ ] Los tests previstos de la BT pasan (gate G2) y verifican el contrato o la restricción declarada (cada test con al menos una aserción explícita; sin tests sin assert).
- [ ] La cobertura de la capa a la que pertenece la BT cumple su umbral de `estrategia-testing_v1.0.md` §2 (gate G3).
- [ ] La BT no baja ningún umbral de cobertura vigente; si lo hiciera, existe una ADR que lo justifica (verificable contra el reporte de cobertura y el registro de ADR de 05).
- [ ] Formato sin diferencias (gate G4) y análisis estático sin warnings nuevos (gate G5).
- [ ] La BT referencia su fuente upstream en 05 (componente, ADR, modelo lógico o punto de extensión) y al menos una US consumidora o una ADR de infraestructura compartida (heredado de la DoR de 06).

### 1.3 DoD de un sprint (rebanada vertical)

- [ ] La rebanada es demostrable end-to-end por el camino completo declarado en `mini-plan_v1.0.md` §3 para ese sprint, sin romper el camino de las rebanadas anteriores (verificable ejecutando el journey de la rebanada; en la rebanada 1 alcanza el camino feliz por la excepción de walking skeleton).
- [ ] Todas las US y BT comprometidas en la rebanada cumplen su DoD de §1.1 / §1.2 (verificable ítem por ítem).
- [ ] La suite completa (unit + integración + e2e crítica) pasa en el pipeline (gates G2) y la cobertura global cumple líneas ≥ 75 % y branches ≥ 65 % (gate G3).
- [ ] Cada CU que la rebanada hace avanzar (columna CU del mini-plan §7) tiene sus criterios de aceptación cubiertos por tests en verde en la matriz CU↔Tests.
- [ ] No hay defectos bloqueantes abiertos sobre la rebanada (verificable en el seguimiento de defectos).
- [ ] La matriz de cobertura y los catálogos de TC quedan actualizados al cierre de la rebanada (anti-patrón de matriz desactualizada de la regla §4.10).

### 1.4 DoD de un release

- [ ] Los 16 CU críticos y no críticos están cubiertos por al menos un caso de prueba en verde por cada uno de sus criterios de aceptación (verificable contra la matriz CU↔Tests completa).
- [ ] Cada NFR con objetivo numérico de `arquitectura-solucion_v1.0.md` §8 tiene su medición o test asociado ejecutado y dentro del SLA en ambiente equivalente al productivo, o una excepción con ADR (verificable contra `matriz-cobertura-pruebas_v1.0.md` §3 y `criterios-validacion_v1.0.md` §3).
- [ ] La cobertura por capa cumple todos los umbrales de `estrategia-testing_v1.0.md` §2, con el módulo de detección ≥ 90 % líneas (gate G3; reporte por capa).
- [ ] La suite de regresión completa pasa y ningún test verde de la versión anterior pasó a rojo sin justificación documentada (gate G2 + comparación con la corrida previa).
- [ ] Gates G1, G4 y G5 en verde sobre la rama de release (build, formato, análisis estático sin warnings nuevos).
- [ ] El artefacto self-contained para la arquitectura objetivo se publica, instala como servicio del sistema y arranca; el rollback a la publicación anterior preserva la clave maestra y los tokens cifrados siguen siendo válidos (verificable ejecutando la instalación y un rollback de prueba; `SOLUTION-INTAKE §17 P.8`).
- [ ] Todo bug cerrado en el ciclo generó al menos un caso de prueba de regresión nuevo o extendió uno existente (anti-patrón de falta de regresión de la regla §4.10).

## 2. Excepciones admitidas

- Walking skeleton (rebanada 1): el cierre se admite con el camino feliz demostrable end-to-end; los criterios de borde de las BT tomadas en versión mínima se completan en rebanadas posteriores (en línea con la excepción de la DoR de 06 §3 y el criterio específico del mini-plan §5). No exime de los gates G1, G2 sobre lo implementado, G4 y G5.
- Decisión abierta a Sprint 0: un criterio que depende de una decisión aún abierta (valores por defecto de detección, familia de hash, mecanismo de recarga en caliente; `SOLUTION-INTAKE §17 P.11`) puede declararse Done con el valor provisorio documentado, y se recalibra al cerrarse la decisión.
- Excepción a un gate de cobertura o calidad: solo con ADR explícita en 05 y un BT de remediación en el backlog de 06. Sin ADR, el gate es bloqueante.
- Deuda técnica: se admite declarar Done con deuda solo si queda registrada como BT explícito en el backlog con su plan de remediación.

Toda excepción se documenta en el propio ítem y caduca en el cierre del sprint, salvo la respaldada por ADR.

## 3. Vigencia

Este documento es la fuente canónica de la DoD del proyecto. Una sola versión vigente por nombre lógico (regla §3.4). Cualquier cambio en sus criterios versionables se registra en §9 de este documento (control de cambios) y se comunica en el cierre de rebanada siguiente. Los planes de sprint de 07 y la DoR de 06 referencian esta DoD; ningún otro documento la redefine.

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | DoD canónica inicial para `discord-bots-admin`: cuatro capas (US, BT, sprint, release) con criterios verificables mecánicamente mediante los gates G1–G5 (`SOLUTION-INTAKE §17 P.6/P.8`), excepciones admitidas (walking skeleton, decisión Sprint 0, ADR, deuda con BT) y vigencia como fuente canónica referenciada por 06 y 07. |
