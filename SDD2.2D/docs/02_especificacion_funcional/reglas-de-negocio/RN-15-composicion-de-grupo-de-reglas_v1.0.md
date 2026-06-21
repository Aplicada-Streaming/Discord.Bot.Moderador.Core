# RN-15 — Composición de un grupo de reglas

**Proyecto:** discord-bots-admin
**Documento:** RN-15-composicion-de-grupo-de-reglas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Un grupo de reglas contiene al menos una regla y tiene un modo de coincidencia definido —todas, alguna, o al menos N—; el anidamiento booleano se limita a dos niveles: las reglas dentro de un grupo y la combinación de grupos dentro de un evento.

## 2. Justificación

El grupo de reglas es la unidad de combinación booleana del motor; un grupo vacío no decide nada y un anidamiento ilimitado volvería la configuración inmanejable para el operador no técnico. Acotar a dos niveles mantiene la complejidad bajo control, según el alcance excluido.

## 3. Ámbito de aplicación

Se evalúa al crear o editar un grupo de reglas y al componer un evento a partir de grupos, en la configuración de la moderación.

## 4. Consecuencia si se viola

Si se permitiera un grupo sin reglas o un anidamiento de más de dos niveles, la evaluación sería indefinida o inmanejable. La regla rechaza un grupo sin reglas, exige un modo de coincidencia y no admite niveles adicionales de anidamiento.

## 5. CU afectados

CU-11.

## 6. Pruebas que la verifican

- Prueba de rechazo de un grupo de reglas sin ninguna regla asociada (referencia a 08).
- Prueba de evaluación de un grupo con modo al menos N que coincide al alcanzar N reglas (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del modelo de grupos de reglas y del alcance excluido del intake |
