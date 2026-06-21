# RN-03 — Validez del patrón de una regla de contenido

**Proyecto:** discord-bots-admin
**Documento:** RN-03-validez-patron-de-contenido_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Una regla de contenido solo se aplica si su criterio es válido: una expresión regular que compila o un conjunto no vacío de palabras o frases clave; un criterio inválido nunca se evalúa contra los mensajes.

## 2. Justificación

Un patrón que no compila o un criterio vacío no puede decidir de forma confiable si un mensaje es indeseado, y su aplicación silenciosa generaría falsos negativos o errores en el flujo de mensajes. La regla protege la confiabilidad del eje de contenido, que el negocio quiere bajo control directo del administrador.

## 3. Ámbito de aplicación

Se evalúa al guardar una regla de contenido en la configuración y en cada evaluación de contenido sobre un mensaje.

## 4. Consecuencia si se viola

Si el criterio es inválido, el sistema no evalúa esa regla, registra el criterio inválido con el código correspondiente para que el administrador lo corrija y continúa con las demás reglas, sin interrumpir el procesamiento.

## 5. CU afectados

CU-04, CU-11.

## 6. Pruebas que la verifican

- Prueba de regla con expresión regular que no compila, que se omite y se registra como inválida (referencia a 08).
- Prueba de regla por palabras clave con conjunto vacío, que se rechaza (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la necesidad de contención de contenido confiable del intake |
