# RN-13 — Resguardo de las credenciales del administrador y cuenta única

**Proyecto:** discord-bots-admin
**Documento:** RN-13-resguardo-de-credenciales-del-administrador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Existe a lo sumo una cuenta de administrador, creada en el primer ingreso, y su contraseña se almacena siempre con un resguardo de hash robusto, nunca en texto claro, y debe cumplir una política mínima de robustez.

## 2. Justificación

El acceso al sistema debe ser un punto de entrada único y seguro; una contraseña en texto claro o débil comprometería todo el sistema, incluida la gestión de tokens. Limitar a una sola cuenta refleja el modelo de un único operador y simplifica la superficie de seguridad.

## 3. Ámbito de aplicación

Se evalúa al crear la cuenta de administrador en el primer ingreso y al verificar las credenciales en cada autenticación.

## 4. Consecuencia si se viola

Si se intentara crear una segunda cuenta, el sistema bloquea el alta y redirige a la autenticación; si una contraseña no cumple la política de robustez, se rechaza; una contraseña nunca se almacena ni se compara en texto claro.

## 5. CU afectados

CU-08, CU-09.

## 6. Pruebas que la verifican

- Prueba de creación de una única cuenta y bloqueo de un segundo alta (referencia a 08).
- Prueba de rechazo de una contraseña que no cumple la política de robustez y de verificación contra hash (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del first-run setup y del resguardo de contraseña del intake |
