# RN-05 — Orden de ejecución de las acciones de una política

**Proyecto:** discord-bots-admin
**Documento:** RN-05-orden-de-ejecucion-de-acciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Las acciones asociadas a una política se ejecutan en el orden configurado por el administrador, y la copia de los mensajes involucrados se toma antes de ejecutar cualquier acción que pueda removerlos.

## 2. Justificación

El orden importa: típicamente se reporta y luego se banea con borrado, y la copia de evidencia debe capturarse antes del borrado porque los mensajes removidos no se restauran. La regla garantiza un comportamiento determinista y preserva la evidencia para la trazabilidad de incidentes.

## 3. Ámbito de aplicación

Se evalúa en el ejecutor de acciones cada vez que una política cumplida desencadena una o más acciones, tanto en ejecución real como, en su registro, en simulación.

## 4. Consecuencia si se viola

Si las acciones se ejecutaran en otro orden o la copia se tomara después del borrado, se podría perder la evidencia del incidente o producir resultados no reproducibles. La regla obliga a respetar el orden configurado y a capturar la copia antes de toda remoción.

## 5. CU afectados

CU-02, CU-03, CU-04, CU-11.

## 6. Pruebas que la verifican

- Prueba de una política con acciones reportar y luego banear que se ejecutan en ese orden (referencia a 08).
- Prueba de que la copia de mensajes se toma antes del borrado retroactivo (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del flujo de moderación del intake |
