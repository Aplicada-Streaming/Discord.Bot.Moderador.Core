# RN-02 — Tope del borrado retroactivo de mensajes

**Proyecto:** discord-bots-admin
**Documento:** RN-02-tope-borrado-retroactivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

La ventana de borrado retroactivo de mensajes nunca puede exceder los 7 días hacia atrás; cualquier valor configurado por encima se acota a ese tope antes de ejecutar el borrado.

## 2. Justificación

La plataforma de mensajería limita a 7 días el rango de mensajes que una acción de baneo puede remover de forma retroactiva. La regla refleja esa restricción de la plataforma como invariante del dominio para que la configuración nunca prometa una limpieza que no se puede ejecutar.

## 3. Ámbito de aplicación

Se evalúa al configurar la ventana de borrado de una acción y al ejecutar el baneo con borrado retroactivo. El rango válido es de 0 a 7 días.

## 4. Consecuencia si se viola

Si el valor configurado supera el tope, el sistema lo acota a 7 días, ejecuta el borrado con ese tope y registra el ajuste con el código correspondiente. No se rechaza la operación; se limita.

## 5. CU afectados

CU-03, CU-11.

## 6. Pruebas que la verifican

- Prueba de acotado de una ventana configurada en más de 7 días al tope de 7 días (referencia a 08).
- Prueba de borrado con ventana de 0 días que no remueve mensajes previos (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del límite de plataforma de 7 días del intake |
