# RN-09 — El modo simulación no ejecuta acción real

**Proyecto:** discord-bots-admin
**Documento:** RN-09-modo-simulacion-no-ejecuta_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Una política en modo simulación evalúa sus condiciones y registra la acción que habría ejecutado, pero nunca ejecuta una acción real sobre los usuarios ni sus mensajes; ante un modo indefinido, se asume simulación como modo seguro.

## 2. Justificación

El negocio necesita calibrar reglas con datos reales sin consecuencias para los miembros antes de promoverlas a ejecución. La simulación reduce los falsos positivos por diseño. Asumir simulación ante ambigüedad evita ejecutar acciones reales por una configuración mal definida.

## 3. Ámbito de aplicación

Se evalúa en el motor de evaluación y en el ejecutor de acciones cada vez que una política cumplida tiene su modo en simulación o indefinido.

## 4. Consecuencia si se viola

Si una política en simulación ejecutara una acción real, se contendría a usuarios sin la validación previa que el negocio exige. La regla obliga a que la simulación solo registre, y a aplicar el modo seguro de simulación cuando el modo no esté definido de forma inequívoca.

## 5. CU afectados

CU-02, CU-04, CU-05, CU-11, CU-14.

## 6. Pruebas que la verifican

- Prueba de una política en simulación que registra el incidente pero no banea (referencia a 08).
- Prueba de una política con modo indefinido que aplica simulación por defecto (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del modo simulación del intake |
