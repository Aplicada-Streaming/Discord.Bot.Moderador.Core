# RN-14 — Cifrado del token de bot en reposo

**Proyecto:** discord-bots-admin
**Documento:** RN-14-cifrado-del-token-en-reposo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

El token de bot de cada servidor se almacena cifrado en reposo con una clave maestra que vive fuera de la base de datos; nunca se persiste en texto claro y solo se descifra en memoria para operar contra la plataforma.

## 2. Justificación

Un token filtrado otorga control total del bot sobre el servidor. Cifrar el token en reposo con una clave maestra externa a la base limita el daño de un acceso indebido a la base de datos y respeta el resguardo de secretos exigido por el negocio.

## 3. Ámbito de aplicación

Se evalúa al registrar o reemplazar el token de un servidor, al persistirlo y al usarlo para conectarse a la plataforma o probar la configuración.

## 4. Consecuencia si se viola

Si un token se persistiera en texto claro, su filtración comprometería el control del bot. La regla obliga a cifrar siempre antes de persistir y a descifrar solo en memoria; un token no se expone nunca en texto claro en la base ni en los reportes.

## 5. CU afectados

CU-10, CU-12, CU-13.

## 6. Pruebas que la verifican

- Prueba de que el token persistido está cifrado y no aparece en texto claro en la base (referencia a 08).
- Prueba de descifrado en memoria para conectarse a la plataforma sin exponer el token (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del cifrado de tokens en reposo del intake |
