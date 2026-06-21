# RC-07 — Confidencialidad del token del servidor

**Proyecto:** discord-bots-admin
**Documento:** RC-07-confidencialidad-del-token_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

El token de bot de un Servidor se conserva siempre en forma cifrada en el modelo; no existe ninguna representación del token en texto claro persistida, y el descifrado ocurre solo en memoria para operar.

## 2. Entidades involucradas

Servidor.

## 3. Tipo de restricción

Valor permitido (confidencialidad del atributo).

## 4. Mecanismo de verificación conceptual

El atributo de token solo admite el valor cifrado producido con la clave maestra externa a la base; cualquier acceso al token para conectarse a la plataforma lo descifra en memoria sin persistir el resultado en texto claro.

## 5. RN o CU que la justifican

RN-14; CU-10, CU-12, CU-13.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
