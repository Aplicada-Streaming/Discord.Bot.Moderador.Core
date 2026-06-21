# RC-05 — Orden determinista de eventos y de acciones

**Proyecto:** discord-bots-admin
**Documento:** RC-05-orden-determinista-evento-accion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Los Eventos de un Servidor tienen una prioridad que define un orden de evaluación determinista, y las Acciones de un Evento tienen un orden de ejecución que define una secuencia determinista; ni la prioridad de evaluación ni el orden de ejecución pueden quedar indefinidos.

## 2. Entidades involucradas

Evento, Acción.

## 3. Tipo de restricción

Cardinalidad y derivación del orden.

## 4. Mecanismo de verificación conceptual

Cada evento conserva su prioridad y cada acción su posición en el evento; al evaluar, los eventos se recorren por prioridad y las acciones por su orden, de modo que dos ejecuciones sobre la misma entrada producen el mismo resultado.

## 5. RN o CU que la justifican

RN-04, RN-05; CU-02, CU-04, CU-11.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
