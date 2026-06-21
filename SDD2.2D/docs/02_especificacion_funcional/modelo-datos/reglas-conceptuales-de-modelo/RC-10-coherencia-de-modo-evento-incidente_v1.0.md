# RC-10 — Coherencia del modo entre evento e incidente

**Proyecto:** discord-bots-admin
**Documento:** RC-10-coherencia-de-modo-evento-incidente_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

El modo registrado en un Incidente (simulación o ejecución real) es coherente con el modo del Evento que lo disparó al momento del disparo; un incidente en modo simulación nunca registra una acción real ejecutada sobre un usuario.

## 2. Entidades involucradas

Evento, Incidente.

## 3. Tipo de restricción

Derivación y valor permitido.

## 4. Mecanismo de verificación conceptual

Al registrar un incidente se copia el modo vigente del evento; si el modo es simulación, el resultado del incidente solo admite valores que no implican ejecución real, y un evento con modo indefinido se trata como simulación.

## 5. RN o CU que la justifican

RN-09; CU-02, CU-14.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
