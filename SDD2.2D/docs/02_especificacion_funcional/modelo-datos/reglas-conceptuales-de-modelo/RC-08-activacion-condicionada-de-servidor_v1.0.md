# RC-08 — Activación de un servidor condicionada a la prueba

**Proyecto:** discord-bots-admin
**Documento:** RC-08-activacion-condicionada-de-servidor_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Un Servidor solo puede tener estado de activación activo si su prueba de configuración más reciente no arrojó faltantes bloqueantes; un servidor con credencial inválida o permisos requeridos ausentes no puede estar en estado activo.

## 2. Entidades involucradas

Servidor.

## 3. Tipo de restricción

Derivación y valor permitido del estado.

## 4. Mecanismo de verificación conceptual

El estado de activación se deriva del resultado de la prueba de configuración: solo se admite el valor activo cuando la última prueba no tiene faltantes bloqueantes; ante un token invalidado en operación, el estado pasa a desconectado.

## 5. RN o CU que la justifican

RN-16; CU-12, CU-13.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
