# ADR-05 — Despliegue self-contained para ARM con servicio del sistema

**Proyecto:** discord-bots-admin
**Documento:** ADR-05-despliegue-self-contained-arm-servicio-sistema_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Despliegue

## 1. Contexto

La plataforma de despliegue está impuesta por el cliente: un dispositivo de bajo consumo con sistema operativo de 32 bits (armv7l), auto-hospedado, sin contenedores, donde el sistema debe quedar corriendo como servicio del sistema instalado mediante un paquete con todo lo necesario (`SOLUTION-INTAKE §10, §17 P.9`). No se puede asumir un runtime instalado en el sistema operativo. La publicación se genera por compilación cruzada desde una estación x64 (§17 P.8). Lo motivan las restricciones de plataforma de 00/alcance y el NFR de disponibilidad (§17 P.10).

## 2. Decisión

Se adopta una publicación en modo self-contained para la arquitectura de CPU objetivo (ARM de 32 bits), empaquetada con todo lo necesario, sin dependencia de un runtime instalado. La instalación registra el servicio en el supervisor de servicios del sistema (systemd) con reinicio automático. La publicación se genera por compilación cruzada desde un runner x64.

## 3. Estado

Aceptado el 2026-06-20. Decisión cerrada pre-Sprint 0 según `SOLUTION-INTAKE §17 P.11`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Self-contained para ARM con servicio del sistema (elegida) | No requiere runtime instalado; instalación y rollback como un paquete; reinicio automático del servicio; cumple el auto-hospedaje sin terceros | Paquete más grande; un binario por arquitectura objetivo |
| Publicación dependiente del runtime del framework | Paquete más pequeño | Requiere instalar y mantener el runtime en el dispositivo; riesgo de incompatibilidad de versiones; viola la premisa de paquete autocontenido |
| Contenedores | Aislamiento y reproducibilidad | Prohibido por el cliente; sobrecarga en hardware de 32 bits; viola §10 |

## 5. Consecuencias positivas

1. El sistema se instala con un único paquete y queda corriendo como servicio (criterio de aceptación del proyecto en 00/alcance).
2. El rollback es reinstalar la publicación anterior conservando el archivo de entorno y la clave maestra; los tokens cifrados siguen válidos (ADR-07).
3. El reinicio automático del servicio sostiene el SLO de disponibilidad de 99 % mensual.
4. No hay dependencia de un runtime del sistema, lo que reduce la superficie de mantenimiento.

## 6. Consecuencias negativas y trade-offs

1. El paquete es más grande al incluir el runtime: aceptado a cambio de no depender de un runtime instalado (`SOLUTION-INTAKE §17 P.12`).
2. ARM de 32 bits es un tier deprioritizado sin inversión de performance: trade-off aceptado por reutilizar el hardware (§17 P.12).
3. Se publica un binario por arquitectura objetivo: aceptado; v1 apunta a una sola arquitectura.

## 7. Implementación

El artefacto de release es un paquete (zip con todo lo necesario) generado por compilación cruzada desde x64. El instalador y la unidad de servicio viven en `scripts/servicio/` (`SOLUTION-INTAKE §16`). La unidad de systemd habilita reinicio automático. El archivo de entorno con la clave maestra se conserva entre actualizaciones (ADR-07). El detalle de pipeline y publicación se profundiza en la categoría 09.

## 8. Métricas de validación

- El paquete se instala en el hardware de referencia y queda corriendo como servicio (criterio de aceptación de 00/alcance).
- Disponibilidad mensual ≥ 99 % (con reinicio automático).
- Rollback a la publicación anterior sin pérdida de validez de los tokens cifrados.

## 9. Referencias

- CU-13 (estado de conexión y reconexión), todas las CU (operación del servicio).
- NFR de disponibilidad (`arquitectura-solucion_v1.0.md §8`).
- `SOLUTION-INTAKE §10, §16, §17 P.8, P.9, P.11, P.12`; `00_contexto/alcance-proyecto_v1.0.md §7`.
- ADR-01 (estilo monolítico), ADR-07 (clave maestra preservada en rollback), ADR-13 (disponibilidad).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
