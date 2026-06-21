# Criterios de validación — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** criterios-validacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

## 1. Propósito

Este documento define los criterios numéricos y verificables que permiten declarar al sistema validado para release. Un release de `discord-bots-admin` está validado cuando los CU críticos están cubiertos y verdes, cada NFR cumple su SLA medido en ambiente equivalente al productivo, la regresión no introduce roturas y la calidad de código cumple los gates de cobertura por capa. Cualquier criterio no cumplido se acepta solo con ADR explícita y plan de remediación. Estos criterios complementan la DoD de release de `definition-of-done_v1.0.md` §1.4.

## 2. Criterios funcionales

Cada CU crítico debe estar cubierto y verde por cada uno de sus criterios de aceptación (CA-XX de 02), verificable contra la tabla CU↔Tests de `matriz-cobertura-pruebas_v1.0.md` §2. Los CU críticos son los del núcleo de moderación y los resguardos contra la moderación errónea.

| CU crítico | Condición de validación | TC asociados |
| --- | --- | --- |
| CU-01 Detectar ráfaga distribuida | Marca la condición por canales distintos en la ventana; no dispara por volumen en un canal; descarta exentos | TC-01, TC-02, TC-03, TC-04 |
| CU-02 Banear al emisor de la ráfaga | Banea con rol inferior; no banea con rol superior y registra el código; simula sin ejecutar; antirrebote suprime repetición | TC-05, TC-06, TC-07, TC-08 |
| CU-03 Banear con borrado retroactivo | Purga los canales dentro de la ventana; acota a 7 días; ventana 0 no remueve; alcanza mensajes no evaluados | TC-09, TC-10, TC-11, TC-12 |
| CU-04 Detectar contenido no deseado | Contiene por regex; omite patrón inválido y continúa; tope de tiempo de regex; descarta exentos | TC-13, TC-14, TC-15, TC-16 |
| CU-05 Reportar al canal privado | Reporta con canales afectados; etiqueta simulación; conserva incidente sin canal designado | TC-22, TC-25, TC-26 |
| CU-14 Modo simulación | Registra sin ejecutar; etiqueta el reporte; ejecuta tras promoción; modo indefinido cae a simulación segura | TC-07, TC-22, TC-23, TC-24 |
| CU-15 Exenciones | Persiste y excluye por rol; descarta antes de evaluar; rechaza identificador no snowflake; excluye canal de confianza | TC-04, TC-27, TC-28, TC-29 |
| CU-16 Antirrebote por usuario | Suprime dentro de la ventana; permite tras expirar; usa default con ventana inválida | TC-08, TC-20, TC-21 |
| CU-12 Probar configuración | Habilita con todo superado; bloquea por permiso o token inválido; advierte jerarquía no bloqueante | TC-43, TC-44, TC-45, TC-46 |

Los CU no críticos restantes (CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-13) deben estar verdes en su criterio principal para el release; su detalle está en la matriz §2.

## 3. Criterios no funcionales

Cada NFR cumple su SLA medido en el ambiente de pruebas equivalente al productivo. Valores de `arquitectura-solucion_v1.0.md` §8 (`SOLUTION-INTAKE §17 P.10`). El tooling de medición se ancla por capacidad.

| NFR | SLA a validar | Test / medición | Ambiente |
| --- | --- | --- | --- |
| Latencia de procesamiento por mensaje | p95 < 200 ms | TC-55 | Hardware de referencia o ambiente equivalente; confirmación en hardware real |
| Throughput sostenido | ≥ 50 mensajes/s | TC-56 | Banco de carga sobre hardware de referencia (a confirmar por benchmark) |
| Disponibilidad mensual (SLO) | 99 % mensual | Métrica observada en 09 | Operación real; derivada del journal y del estado de conexión |
| Memoria por conexión de gateway activa | ≤ 8 MB por conexión | TC-57 | Perfilado en el dispositivo |
| Cobertura del módulo de detección | ≥ 90 % líneas; global líneas ≥ 75 %, branches ≥ 65 % | Gate G3 | Pipeline de integración continua |
| Limpieza efectiva de la ráfaga | ≥ 98 % de mensajes eliminados dentro de los 10 s | TC-58 | Pruebas con mensajes simulados; registro de incidentes |

Para latencia y throughput, por ser hardware ARM de 32 bits (tier deprioritizado, `SOLUTION-INTAKE §17 P.12`), la validación se hace primero en ambiente equivalente y se confirma en el hardware real; un desvío conocido se registra como excepción con ADR (§6).

## 4. Criterios de regresión

- La suite de regresión completa (unit + integración + e2e crítica) se ejecuta y queda verde antes del release (gate G2).
- Ningún test que estaba verde en la versión anterior pasó a rojo sin justificación documentada; la comparación se hace contra la corrida de la versión previa.
- Todo bug cerrado en el ciclo generó al menos un caso de prueba de regresión nuevo o extendió uno existente (anti-patrón de falta de regresión de la regla §4.10); la numeración continúa a partir de TC-69.

## 5. Criterios de calidad de código

- Cobertura por capa cumplida según `estrategia-testing_v1.0.md` §2: Dominio ≥ 90 / 80, Aplicación ≥ 80 / 70, Infraestructura ≥ 70 / 60, Presentación ≥ 60 / 50; global ≥ 75 / 65 (gate G3). La cobertura se reporta por capa, no como número global único.
- Mutation score: no exigido como gate en v1 (la regla §2.2 solo lo pide para library).
- Análisis estático sin warnings nuevos respecto de la rama base (gate G5).
- Formato canónico sin diferencias (gate G4).
- Build sin errores (gate G1).

## 6. Excepciones documentadas

Cualquier criterio no cumplido se acepta solo con ADR explícita en 05 y un BT de remediación en el backlog de 06. Casos previstos:

- Desvío de latencia o throughput en hardware ARM de 32 bits: aceptable con ADR que lo documente como tier deprioritizado (`SOLUTION-INTAKE §17 P.12`, ADR-01) y un plan de mejora o de ajuste de expectativa.
- Bajada temporal de un umbral de cobertura: solo con ADR que la justifique y un BT que la reponga; sin ADR el gate G3 es bloqueante.
- Decisión abierta a Sprint 0 que afecta un criterio (valores por defecto de detección, familia de hash): el criterio se valida con el valor provisorio documentado y se recalibra al cerrarse la decisión (`SOLUTION-INTAKE §17 P.11`).

Sin la ADR correspondiente, ningún criterio de §2 a §5 puede declararse cumplido por excepción.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Criterios de validación iniciales para `discord-bots-admin`: funcionales por CU crítico, no funcionales con los SLA de 05 §8 medidos en ambiente equivalente, regresión, calidad de código con los gates de cobertura por capa y excepciones admitidas solo con ADR. |
