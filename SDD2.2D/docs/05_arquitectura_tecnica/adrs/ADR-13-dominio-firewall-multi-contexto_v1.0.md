# ADR-13 — Dominio como firewall multi-contexto (un token y una conexión por servidor)

**Proyecto:** discord-bots-admin
**Documento:** ADR-13-dominio-firewall-multi-contexto_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Estilo

## 1. Contexto

El sistema debe poder operar varios servidores de la plataforma desde una sola instancia, pero con un único administrador (`SOLUTION-INTAKE §3, §17 P.4`). En la plataforma, el token identifica a una aplicación-bot por servidor, de modo que cada servidor registrado aporta su propio token y su propia conexión al canal de eventos (§17 P.11). Cada servidor tiene además sus propias reglas, políticas, exenciones y estado de conducta, que no deben mezclarse entre servidores. El límite de memoria del proceso de 32 bits acota cuántas conexiones concurrentes caben (§17 P.12). Lo motivan NB-01, CU-01, CU-10, CU-13, RN-08, RN-14 y RC-01/RC-02.

## 2. Decisión

Se adopta el dominio modelado como firewall multi-contexto: cada servidor registrado es un contexto independiente con su token cifrado, su conexión propia al canal de eventos, y su conjunto aislado de reglas, políticas, exenciones y estado de conducta en memoria. La operación multi-servidor es multi-contexto dentro de una misma instancia, no multi-tenant: hay un solo administrador. En v1 se opera un solo servidor; cada token adicional implica una conexión de gateway concurrente sujeta al presupuesto de memoria.

## 3. Estado

Aceptado el 2026-06-20. Decisión cerrada pre-Sprint 0 según `SOLUTION-INTAKE §17 P.11`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Firewall multi-contexto, una conexión por servidor (elegida) | Aislamiento natural por servidor; encaja con un token por aplicación-bot; estado de conducta particionado; presupuesto de memoria por conexión | Cada servidor suma una conexión concurrente, limitada por la memoria del dispositivo |
| Multi-tenant con discriminador por inquilino | Modelo clásico de aislamiento por cliente | No aplica: hay un único administrador; introduce complejidad sin beneficio (ver `modelo-datos-logico` no multi-tenant) |
| Una sola conexión compartida para varios servidores | Menos conexiones | La plataforma asocia el token a una aplicación-bot por servidor; no representa fielmente el modelo de la plataforma |

## 5. Consecuencias positivas

1. Las reglas, políticas, exenciones y estado de conducta quedan aislados por servidor, sin contaminación cruzada.
2. El modelo encaja con el token por aplicación-bot de la plataforma (RN-14, ADR-07).
3. El estado de conducta en memoria se particiona por contexto, alineado con el presupuesto de memoria (ADR-09).
4. El estado de conexión se gestiona y se muestra por contexto en el panel (CU-13).

## 6. Consecuencias negativas y trade-offs

1. Cada servidor adicional suma una conexión concurrente: trade-off aceptado; v1 opera un solo servidor por el límite de memoria de 32 bits (`SOLUTION-INTAKE §17 P.12`).
2. La operación multi-servidor a escala masiva queda fuera de v1: aceptado por alcance (`00/alcance §5`).
3. El presupuesto de ≤ 8 MB por conexión debe verificarse en hardware real: validado por NFR de memoria.

## 7. Implementación

El Registro de servidores (Aplicación) administra los contextos; el Adaptador del gateway (Infraestructura) mantiene una conexión por contexto con reconexión automática y estado por contexto (CU-13). El motor de moderación evalúa con las reglas y el estado del contexto correspondiente. Los snowflakes se almacenan como texto por contexto (RN-08, RC-02). No hay discriminador multi-tenant en el modelo (`modelo-datos-logico_v1.0.md §6`).

## 8. Métricas de validación

- Memoria ≤ 8 MB por conexión de gateway activa (NFR de memoria).
- Aislamiento verificado: una regla de un servidor no afecta a otro (prueba de integración).
- Estado de conexión correcto por contexto ante caída y reconexión (CU-13).

## 9. Referencias

- NB-01.
- CU-01, CU-10, CU-13; relacionada con CU-12.
- RN-08, RN-14.
- RC-01, RC-02.
- NFR de memoria y disponibilidad (`arquitectura-solucion_v1.0.md §8`).
- `SOLUTION-INTAKE §3, §17 P.4, P.11, P.12`.
- ADR-07 (cifrado de token por contexto), ADR-09 (estado en memoria particionado), ADR-01 (monolito).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
