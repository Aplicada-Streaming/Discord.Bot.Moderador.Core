# RN-16 — Activación de un servidor condicionada a la prueba de configuración

**Proyecto:** discord-bots-admin
**Documento:** RN-16-activacion-condicionada-a-prueba_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Un servidor no puede activarse si la prueba de configuración detecta un faltante bloqueante —credencial inválida o permisos requeridos ausentes—; la activación solo se habilita cuando la prueba no arroja faltantes bloqueantes, y las advertencias no bloqueantes quedan registradas y visibles.

## 2. Justificación

Activar un servidor que no puede ser moderado genera una falsa sensación de seguridad mientras queda expuesto. Condicionar la activación a la prueba de configuración asegura que el administrador no confíe en una protección inexistente y que las limitaciones conocidas queden a la vista.

## 3. Ámbito de aplicación

Se evalúa al ejecutar la prueba de configuración y al intentar activar un servidor registrado, y en la re-validación periódica de un servidor activo.

## 4. Consecuencia si se viola

Si un servidor se activara con un faltante bloqueante, podría operar sin poder moderar. La regla bloquea la activación ante credencial inválida o permisos ausentes, deja el servidor inactivo o desconectado y registra el motivo; las advertencias no bloqueantes permiten activar dejando constancia.

## 5. CU afectados

CU-12, CU-13.

## 6. Pruebas que la verifican

- Prueba de bloqueo de activación con credencial inválida o permiso de baneo ausente (referencia a 08).
- Prueba de activación habilitada con prueba superada y advertencia de jerarquía no bloqueante registrada (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la prueba de configuración y de los riesgos R-03 y R-04 del intake |
