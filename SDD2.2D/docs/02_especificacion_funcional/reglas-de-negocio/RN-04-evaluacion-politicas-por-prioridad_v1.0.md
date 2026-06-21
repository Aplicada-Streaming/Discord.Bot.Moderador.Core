# RN-04 — Evaluación de políticas por prioridad con primera coincidencia

**Proyecto:** discord-bots-admin
**Documento:** RN-04-evaluacion-politicas-por-prioridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Cuando varias políticas pueden aplicar sobre un mismo mensaje, se evalúan en orden de prioridad y se detiene en la primera coincidencia, salvo que esa política tenga activada la bandera continuar, en cuyo caso la evaluación prosigue con las políticas de menor prioridad.

## 2. Justificación

Distintas políticas pueden coincidir sobre un mismo mensaje; sin un criterio de resolución, el resultado sería ambiguo o se ejecutarían acciones contradictorias. La regla, de estilo firewall, da un orden determinista y previsible y permite, de forma explícita y opcional, que un mismo mensaje dispare más de un evento.

## 3. Ámbito de aplicación

Se evalúa en el motor de evaluación cada vez que se procesa un mensaje contra el conjunto de políticas de un servidor, tanto en ejecución real como en simulación.

## 4. Consecuencia si se viola

Si no se respetara el orden por prioridad o la bandera continuar, el sistema podría ejecutar acciones en un orden no determinista o repetir acciones no deseadas; la regla obliga a un resultado reproducible y a que solo la bandera continuar habilite el encadenamiento de políticas.

## 5. CU afectados

CU-01, CU-02, CU-04, CU-11, CU-14.

## 6. Pruebas que la verifican

- Prueba de dos políticas coincidentes sin bandera continuar, donde solo dispara la de mayor prioridad (referencia a 08).
- Prueba de política con bandera continuar que permite disparar también la siguiente (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del caso límite de coincidencia de políticas del intake |
