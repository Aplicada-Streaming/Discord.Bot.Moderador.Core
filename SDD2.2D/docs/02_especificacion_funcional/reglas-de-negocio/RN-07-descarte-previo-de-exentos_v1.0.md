# RN-07 — Descarte previo de los sujetos exentos

**Proyecto:** discord-bots-admin
**Documento:** RN-07-descarte-previo-de-exentos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Un sujeto exento por rol, por usuario o por canal de confianza se descarta antes de evaluar cualquier regla de contenido o de conducta, de modo que nunca puede ser alcanzado por una acción de moderación.

## 2. Justificación

El staff y los espacios legítimos deben quedar fuera de la moderación para no ser contenidos por error; el riesgo de mayor impacto es banear a un usuario legítimo. Descartar a los exentos antes de evaluar garantiza por diseño que ninguna regla pueda accionar sobre ellos.

## 3. Ámbito de aplicación

Se evalúa al inicio del pipeline de moderación, antes de registrar la actividad de conducta y antes de evaluar las reglas de contenido, para cada mensaje recibido.

## 4. Consecuencia si se viola

Si un sujeto exento no se descartara antes de evaluar, podría ser alcanzado por una acción, violando una garantía de negocio. La regla exige que el descarte ocurra antes de toda evaluación; el indicador de negocio asociado es de cero sujetos exentos alcanzados por una acción.

## 5. CU afectados

CU-01, CU-02, CU-04, CU-15.

## 6. Pruebas que la verifican

- Prueba de un usuario con rol exento que publica en varios canales y no es contenido (referencia a 08).
- Prueba de actividad en un canal de confianza que no dispara ninguna regla (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del flujo de moderación y de las exenciones del intake |
