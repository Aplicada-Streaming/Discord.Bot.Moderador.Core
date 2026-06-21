# RN-01 — Jerarquía de roles del bot para accionar

**Proyecto:** discord-bots-admin
**Documento:** RN-01-jerarquia-de-roles-del-bot_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

El bot no puede aplicar una acción de moderación sobre un usuario cuyo rol es jerárquicamente superior o igual al suyo, ni cuando carece del permiso requerido para esa acción; en ese caso la acción no se ejecuta, se registra y se reporta como no accionable.

## 2. Justificación

La plataforma de mensajería impide a una aplicación-bot actuar sobre usuarios con rol superior al propio; intentarlo falla o queda sin efecto. La regla evita acciones imposibles, deja constancia del caso y permite advertir al administrador antes de confiar en una protección inexistente.

## 3. Ámbito de aplicación

Se evalúa en toda acción de contención sobre un usuario (baneo, borrado retroactivo, desbaneo, timeout, expulsión, gestión de roles) y en la prueba de configuración previa a activar un servidor.

## 4. Consecuencia si se viola

La acción no se ejecuta; el sistema registra el incidente como no accionable por jerarquía o permisos con el código correspondiente y lo reporta al canal de incidencias. La prueba de configuración advierte de la jerarquía y de los permisos faltantes antes de activar.

## 5. CU afectados

CU-02, CU-03, CU-04, CU-07, CU-12.

## 6. Pruebas que la verifican

- Prueba de baneo bloqueado por rol superior del usuario objetivo (referencia a 08).
- Prueba de acción bloqueada por permiso faltante del bot (referencia a 08).
- Prueba de advertencia de jerarquía en la prueba de configuración (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del caso límite de jerarquía de roles del intake |
