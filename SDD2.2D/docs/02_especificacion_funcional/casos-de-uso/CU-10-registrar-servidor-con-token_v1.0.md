# CU-10 — Registrar un servidor con su credencial de acceso

**Proyecto:** discord-bots-admin
**Documento:** CU-10-registrar-servidor-con-token_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Permitir que el administrador registre un servidor a moderar junto con su token de bot, para que el sistema pueda conectarse a su canal de eventos y operar la moderación como un contexto independiente. Cada servidor registrado es un contexto del firewall multi-contexto con su propia credencial.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Registra el servidor e ingresa su token de bot |
| Servicio de administración | Sistema | Persiste el servidor y cifra el token en reposo |
| Plataforma de mensajería | Sistema | Provee la identidad del servidor y valida la credencial cuando se prueba |

## 3. Precondiciones

- El administrador está autenticado en el panel (CU-09).
- El administrador dispone de un token de bot válido para el servidor a registrar.
- El servidor no está ya registrado en el sistema (RN-08).

## 4. Flujo principal

1. El administrador inicia el registro de un servidor desde el panel.
2. El administrador ingresa el identificador del servidor y su token de bot.
3. El servicio valida que el identificador del servidor (snowflake) tenga el formato esperado y se almacene como texto (RN-08).
4. El servicio cifra el token en reposo con la clave maestra del servicio, nunca en texto claro (RN-14).
5. El servicio persiste el servidor con su token cifrado y lo deja en estado registrado pero no activo.
6. El servicio ofrece ejecutar la prueba de configuración (CU-12) antes de activar.

## 5. Flujos alternativos

- 5.A Actualización del token. Disparador: el administrador reemplaza un token revocado o vencido. Acción: el servicio cifra y reemplaza el token conservando el resto de la configuración del servidor. Punto de retorno: retorna al paso 5 y vuelve a ofrecer la prueba de configuración.
- 5.B Designación de canal de salida. Disparador: el administrador define el canal de salida de reportes durante el registro. Acción: el servicio asocia el canal de salida lógico al servidor. Punto de retorno: retorna al paso 5 incluyendo el canal de salida.
- 5.C Registro sin activación inmediata. Disparador: el administrador guarda el servidor sin probar todavía. Acción: el servidor queda registrado e inactivo a la espera de la prueba. Punto de retorno: el flujo termina dejando el servidor inactivo.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| SERVIDOR_YA_REGISTRADO | Ya existe un servidor con el mismo identificador (RN-08) | No crea un duplicado; informa que el servidor ya está registrado y ofrece editarlo |
| SERVIDOR_IDENTIFICADOR_INVALIDO | El identificador del servidor no tiene el formato de snowflake esperado (RN-08) | Rechaza el registro e indica el formato esperado |
| SERVIDOR_TOKEN_VACIO | No se ingresó token de bot | Rechaza el registro e indica que el token es obligatorio para operar el servidor |

## 7. Postcondiciones

- En caso de éxito: el servidor queda registrado con su token cifrado en reposo, en estado inactivo hasta superar la prueba de configuración.
- En caso de fallo: el servidor no se registra o no se modifica; el estado anterior se conserva.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un administrador autenticado y un servidor no registrado | El administrador registra el servidor con un identificador válido y un token de bot | El servicio persiste el servidor con el token cifrado y lo deja inactivo, y ofrece la prueba de configuración |
| CA-02 | Un servidor ya registrado con un identificador dado | El administrador intenta registrar otro servidor con el mismo identificador | El servicio rechaza el duplicado con código SERVIDOR_YA_REGISTRADO y ofrece editar el existente |
| CA-03 | Un administrador registrando un servidor | El administrador deja vacío el campo de token | El servicio rechaza el registro con código SERVIDOR_TOKEN_VACIO |
| CA-04 | Un servidor existente con token revocado | El administrador ingresa un token nuevo | El servicio cifra y reemplaza el token, conserva la configuración y vuelve a ofrecer la prueba de configuración |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-08, RN-12, RN-14 |
| Historias de usuario a generar | US a generar en 06 (registro de servidor; cifrado del token; designación de canal de salida) |
| Componentes esperados | Página de registro de servidores; servicio de cifrado de tokens; persistencia de servidores (referencia tentativa a 05) |
| Tests previstos | Pruebas de registro y de unicidad; pruebas de cifrado del token; pruebas de validación del identificador (referencia tentativa a 08) |

## 10. Notas y supuestos

- El token se cifra con una clave maestra que vive fuera de la base de datos; un token nunca se almacena en texto claro.
- El identificador del servidor es un snowflake almacenado como texto para evitar el desborde del entero de 64 bits.
- En v1 la operación se prevé para un servidor; el modelo admite varios contextos, acotado por la memoria del dispositivo de despliegue.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-05 y de la persistencia descrita en el intake |
