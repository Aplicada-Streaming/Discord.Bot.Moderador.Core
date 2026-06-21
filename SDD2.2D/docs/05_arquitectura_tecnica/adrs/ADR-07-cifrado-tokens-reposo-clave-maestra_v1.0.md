# ADR-07 — Cifrado de tokens en reposo con clave maestra por variable de entorno

**Proyecto:** discord-bots-admin
**Documento:** ADR-07-cifrado-tokens-reposo-clave-maestra_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Seguridad

## 1. Contexto

Cada servidor registrado aporta un token de bot que otorga control total de la aplicación-bot en ese servidor; un token filtrado es un riesgo alto (`SOLUTION-INTAKE §17 P.12`). El token debe persistirse para operar (reconexión, acciones), pero nunca puede quedar en texto claro en la base (RN-14, RC-07). El sistema es auto-hospedado, sin un gestor de secretos de terceros. Lo motivan NB-05/NB-06, CU-10, CU-12, CU-13, RN-14 y RC-07.

## 2. Decisión

Se adopta el cifrado simétrico (AES) del token en reposo, con una clave maestra que vive fuera de la base, en una variable de entorno del servicio (archivo de entorno con permisos restringidos). El token se descifra solo en memoria, en el momento de operar contra la plataforma, y nunca se persiste en claro. La clave maestra se conserva entre actualizaciones para que los tokens cifrados sigan siendo válidos tras un rollback.

## 3. Estado

Aceptado el 2026-06-20. Decisión cerrada pre-Sprint 0 según `SOLUTION-INTAKE §17 P.11`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Cifrado AES con clave maestra fuera de la base (elegida) | Token nunca en claro en reposo; clave separada del dato cifrado; sin dependencia de terceros; compatible con rollback | Requiere custodiar la clave maestra y el archivo de entorno con permisos estrictos |
| Token en texto claro en la base | Trivial | Viola RN-14 y RC-07; una copia de la base expone todos los tokens |
| Gestor de secretos externo | Rotación y auditoría gestionadas | Dependencia de un tercero; contradice el auto-hospedaje sin terceros |
| Cifrado con clave derivada de la contraseña del administrador | Sin archivo de clave adicional | Acopla el cifrado al login; el bot no podría operar sin sesión del administrador |

## 5. Consecuencias positivas

1. El token nunca queda en texto claro en reposo (RN-14, RC-07).
2. La clave maestra vive fuera de la base: una copia de la base por sí sola no expone los tokens.
3. El rollback preserva el archivo de entorno y la clave; los tokens cifrados siguen válidos (ADR-05).
4. No introduce dependencia de un gestor de secretos de terceros, coherente con el auto-hospedaje.

## 6. Consecuencias negativas y trade-offs

1. La seguridad depende de custodiar la clave maestra y el archivo de entorno con permisos restringidos: trade-off aceptado y mitigado por permisos del sistema de archivos (`SOLUTION-INTAKE §17 P.12`).
2. La pérdida de la clave maestra inutiliza los tokens cifrados: aceptado; se documenta el procedimiento de resguardo de la clave.
3. La rotación de la clave exige re-cifrar los tokens: aceptado; operación poco frecuente y acotada.

## 7. Implementación

El Servicio de cifrado (Infraestructura) lee la clave maestra de la variable de entorno del servicio, cifra el token al registrarlo (CU-10) y lo descifra solo en memoria para la prueba de configuración (CU-12), la reconexión y las acciones (CU-13). El campo de token en la base almacena únicamente el texto cifrado (`modelo-datos-logico_v1.0.md`, RC-07). El archivo de entorno se conserva en actualizaciones (ADR-05).

## 8. Métricas de validación

- Cero tokens en texto claro en la base (verificación de datos y de esquema).
- El servicio opera tras un rollback sin re-registrar tokens (clave maestra preservada).
- Pruebas unitarias de cifrado/descifrado y de que el descifrado ocurre solo en memoria.

## 9. Referencias

- NB-05, NB-06.
- CU-10, CU-12, CU-13.
- RN-14.
- RC-07.
- `SOLUTION-INTAKE §17 P.5, P.8, P.11, P.12`.
- ADR-05 (rollback preserva la clave), ADR-06 (compliance), ADR-02 (persistencia).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
