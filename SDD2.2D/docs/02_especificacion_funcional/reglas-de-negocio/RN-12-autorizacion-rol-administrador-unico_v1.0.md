# RN-12 — Autorización por rol administrador único

**Proyecto:** discord-bots-admin
**Documento:** RN-12-autorizacion-rol-administrador-unico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Solo el administrador autenticado, único rol del sistema, está autorizado a registrar servidores, configurar reglas, grupos, eventos, acciones y parámetros, definir exenciones, revisar incidentes y revertir baneos; toda acción administrativa requiere una sesión válida de ese rol.

## 2. Justificación

El sistema tiene un único operador y un único punto de entrada seguro; concentrar la autorización en un rol administrador evita accesos no autorizados a funciones sensibles como la gestión de tokens y la reversión de baneos. No hay multi-tenant ni roles adicionales.

## 3. Ámbito de aplicación

Se evalúa en cada acción administrativa del panel: registro y edición de servidores, configuración de la moderación, exenciones, revisión de incidentes y reversión de baneos.

## 4. Consecuencia si se viola

Si una acción administrativa se ejecutara sin una sesión válida del administrador, se comprometería la seguridad del sistema. La regla rechaza la operación, redirige a la autenticación y no aplica el cambio solicitado.

## 5. CU afectados

CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12, CU-15.

## 6. Pruebas que la verifican

- Prueba de acceso a una función administrativa sin sesión válida, que se rechaza y redirige a la autenticación (referencia a 08).
- Prueba de que una sesión válida del administrador habilita las funciones administrativas (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la autorización por rol administrador único del intake |
