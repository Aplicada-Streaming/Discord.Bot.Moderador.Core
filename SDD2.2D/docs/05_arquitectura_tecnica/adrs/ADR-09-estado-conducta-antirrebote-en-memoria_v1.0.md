# ADR-09 — Estado de conducta y antirrebote en memoria, no persistidos

**Proyecto:** discord-bots-admin
**Documento:** ADR-09-estado-conducta-antirrebote-en-memoria_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Persistencia

## 1. Contexto

Las reglas de conducta evalúan la actividad reciente del usuario mediante ventanas deslizantes (canales distintos, frecuencia) que se actualizan con cada mensaje. El antirrebote por usuario evita acciones repetidas durante una ráfaga (RN-06). Este estado es de alta frecuencia de escritura y de vida corta (la ventana de detección es del orden de segundos). Persistirlo en la base relacional embebida del dispositivo de 32 bits añadiría escrituras constantes y latencia, comprometiendo el NFR de latencia p95 < 200 ms y de throughput ≥ 50 mensajes/s (`SOLUTION-INTAKE §17 P.10`). Lo motivan CU-01, CU-16, RN-06, RN-10 y el trade-off de §17 P.12.

## 2. Decisión

Se adopta mantener el estado de conducta (ventanas deslizantes de actividad reciente por usuario) y el estado de antirrebote por usuario en memoria, particionados por contexto (servidor), sin persistirlos. Ante un reinicio del servicio, este estado se pierde y se reconstruye con el tráfico entrante; el borrado retroactivo al banear cubre los mensajes previos dentro de la ventana de borrado. La eventual persistencia de algún contador de conducta para sobrevivir reinicios queda abierta a Sprint 0 (`SOLUTION-INTAKE §17 P.11`).

## 3. Estado

Aceptado el 2026-06-20. Trade-off declarado y aceptado en `SOLUTION-INTAKE §17 P.12`; la persistencia eventual de contadores es la única dimensión abierta a Sprint 0.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Estado en memoria, no persistido (elegida) | Latencia mínima; no recarga la base en el dispositivo de 32 bits; cumple latencia y throughput | Se pierde ante un reinicio; una ráfaga en curso podría no cortarse hasta reconstruir la ventana |
| Persistir el estado en la base relacional | Sobrevive reinicios | Escrituras de alta frecuencia; latencia y desgaste del almacenamiento; compromete los NFR |
| Cache externa en proceso aparte | Sobrevive reinicios del proceso de aplicación | Otro proceso/servicio en el dispositivo; contradice el monolito único (ADR-01) |

## 5. Consecuencias positivas

1. La latencia del pipeline se mantiene mínima, dentro del p95 < 200 ms.
2. No se recarga la base ni el almacenamiento del dispositivo con escrituras de alta frecuencia.
3. El particionamiento por contexto encaja con el firewall multi-contexto (ADR-13) y con el presupuesto de memoria por conexión.
4. La simplicidad reduce la superficie de fallos del núcleo crítico.

## 6. Consecuencias negativas y trade-offs

1. El estado se pierde ante un reinicio: trade-off aceptado a cambio de simplicidad y de no recargar la base (`SOLUTION-INTAKE §17 P.12`); el borrado retroactivo limpia lo previo dentro de la ventana.
2. Tras un reinicio, una ráfaga en curso podría no cortarse hasta reconstruir la ventana: mitigado por el borrado retroactivo y por la ventana corta de detección.
3. Persistir contadores queda como posible evolución: documentado como abierto a Sprint 0; si se decide, será una ADR nueva que supere a esta.

## 7. Implementación

Los componentes Estado de conducta en memoria y Antirrebote por usuario (Dominio) mantienen estructuras en memoria particionadas por contexto, con expiración por ventana. No se modelan como tablas (`modelo-datos-logico_v1.0.md` lo documenta como nota en memoria, coherente con el modelo conceptual de 02). El presupuesto de memoria por conexión se respeta limpiando entradas vencidas.

## 8. Métricas de validación

- Latencia p95 < 200 ms y throughput ≥ 50 mensajes/s en el hardware de referencia.
- Memoria ≤ 8 MB por conexión de gateway activa.
- Pruebas unitarias de la ventana deslizante y del antirrebote (supresión dentro de la ventana, expiración fuera de ella).

## 9. Referencias

- CU-01, CU-16; relacionada con CU-02 (antirrebote en el baneo) y CU-14 (simulación).
- RN-06, RN-10.
- NFR de latencia, throughput y memoria (`arquitectura-solucion_v1.0.md §8`).
- `SOLUTION-INTAKE §17 P.10, P.11, P.12`; `modelo-conceptual_v1.0.md` (nota sobre estado en memoria).
- ADR-01 (monolito), ADR-02 (persistencia), ADR-13 (firewall multi-contexto).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
