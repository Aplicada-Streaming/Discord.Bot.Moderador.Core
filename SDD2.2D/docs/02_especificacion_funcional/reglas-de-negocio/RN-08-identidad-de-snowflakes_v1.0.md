# RN-08 — Identidad de los snowflakes de la plataforma

**Proyecto:** discord-bots-admin
**Documento:** RN-08-identidad-de-snowflakes_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Todo identificador de la plataforma (servidor, canal, usuario o mensaje) es un snowflake que se trata y almacena como texto, y un servidor se identifica de forma única por su snowflake dentro del sistema.

## 2. Justificación

El snowflake es un identificador de 64 bits que excede el rango del entero con signo y, tratado como número, se corrompe por desborde. Tratarlo como texto preserva su valor exacto. La unicidad del snowflake del servidor evita registrar el mismo servidor dos veces como contextos separados.

## 3. Ámbito de aplicación

Se evalúa al registrar o editar un servidor, al definir exenciones por rol, usuario o canal, al designar canales de salida y al registrar incidentes con sus mensajes y canales afectados.

## 4. Consecuencia si se viola

Si un identificador se tratara como número, se corromperían las referencias por desborde; si se permitiera un servidor duplicado, la moderación se aplicaría de forma inconsistente. La regla rechaza identificadores con formato inválido y rechaza el registro de un servidor ya existente.

## 5. CU afectados

CU-10, CU-15.

## 6. Pruebas que la verifican

- Prueba de almacenamiento y recuperación de un snowflake de 64 bits sin pérdida de precisión (referencia a 08).
- Prueba de rechazo del registro de un servidor con un snowflake ya existente (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la persistencia de snowflakes como texto del intake |
