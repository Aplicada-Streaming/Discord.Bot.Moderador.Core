# RN-10 — Configuración dirigida por descriptor de parámetro

**Proyecto:** discord-bots-admin
**Documento:** RN-10-configuracion-dirigida-por-descriptor_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Cada parámetro configurable tiene un descriptor único que es la fuente de verdad de su valor por defecto, sus límites, su leyenda y sus ejemplos; un valor solo se acepta si cae dentro de los límites del descriptor, y la ausencia de valor se resuelve con el valor por defecto del descriptor.

## 2. Justificación

El administrador debe poder configurar la moderación sin conocimiento técnico profundo, entendiendo qué ajusta y con qué efecto. Centralizar default, límites, leyenda y ejemplos en un descriptor único evita configuraciones inválidas y asegura ayuda contextual consistente en todo el panel.

## 3. Ámbito de aplicación

Se evalúa al ingresar o editar cualquier parámetro de reglas, grupos, eventos, acciones, umbrales, ventanas y modos desde el panel, y al aplicar valores por defecto cuando no se especifican.

## 4. Consecuencia si se viola

Si se aceptara un valor fuera de los límites del descriptor, la moderación operaría con una configuración inválida. La regla rechaza esos valores mostrando los límites permitidos, ofrece el valor por defecto y exige que cada parámetro exponga su leyenda y sus ejemplos.

## 5. CU afectados

CU-01, CU-11, CU-16.

## 6. Pruebas que la verifican

- Prueba de aceptación de un valor dentro de los límites del descriptor (referencia a 08).
- Prueba de rechazo de un valor fuera de los límites y de aplicación del valor por defecto cuando se omite (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la configuración dirigida por esquema del intake |
