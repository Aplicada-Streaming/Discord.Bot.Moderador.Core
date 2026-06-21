# RN-06 — Antirrebote por usuario durante una ráfaga

**Proyecto:** discord-bots-admin
**Documento:** RN-06-antirrebote-por-usuario_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Un mismo usuario no recibe la misma acción de moderación más de una vez dentro de la ventana de antirrebote vigente; las coincidencias adicionales sobre un usuario ya accionado en esa ventana se suprimen.

## 2. Justificación

Durante una ráfaga, un usuario puede disparar la misma regla muchas veces en segundos; repetir la acción genera ruido, incidentes duplicados y reacciones desproporcionadas. La regla acota la acción a una sola vez por ventana para mantener la respuesta proporcionada y la auditoría limpia.

## 3. Ámbito de aplicación

Se evalúa en el ejecutor de acciones antes de aplicar una acción real sobre un usuario, usando el estado de antirrebote por usuario que vive en memoria.

## 4. Consecuencia si se viola

Si no se aplicara el antirrebote, un mismo usuario podría ser accionado repetidamente en una sola ráfaga, multiplicando incidentes y reacciones. La regla obliga a suprimir las repeticiones dentro de la ventana y a no generar incidentes adicionales por ellas.

## 5. CU afectados

CU-02, CU-16.

## 6. Pruebas que la verifican

- Prueba de supresión de una segunda acción sobre el mismo usuario dentro de la ventana (referencia a 08).
- Prueba de que, expirada la ventana, una nueva coincidencia vuelve a ser accionable (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la precisión de antirrebote del intake |
