# CU-09 — Autenticar al administrador

**Proyecto:** discord-bots-admin
**Documento:** CU-09-autenticar-administrador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Verificar la identidad del administrador a partir de sus credenciales para abrir una sesión autorizada en el panel, de modo que solo él pueda registrar servidores, configurar la moderación y revisar o revertir incidentes. Es la puerta de acceso a todas las funciones administrativas.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Presenta sus credenciales para iniciar sesión |
| Servicio de administración | Sistema | Verifica las credenciales contra el resguardo almacenado y abre o niega la sesión |

## 3. Precondiciones

- Existe una cuenta de administrador creada en el primer ingreso (CU-08).
- El primer ingreso ya está completado (RN-13).

## 4. Flujo principal

1. El administrador solicita ingresar al panel.
2. El servicio presenta el ingreso de credenciales.
3. El administrador ingresa su identificador y su contraseña.
4. El servicio verifica las credenciales contra el resguardo de hash almacenado, sin comparar texto claro (RN-13).
5. Si las credenciales son válidas, el servicio abre una sesión autorizada para el rol administrador (RN-12).
6. El servicio dirige al administrador al panel con la sesión activa.

## 5. Flujos alternativos

- 5.A Cierre de sesión. Disparador: el administrador solicita cerrar la sesión. Acción: el servicio invalida la sesión vigente. Punto de retorno: vuelve al ingreso de credenciales.
- 5.B Sesión vencida. Disparador: la sesión activa supera su tiempo de vigencia. Acción: el servicio invalida la sesión y exige autenticarse de nuevo. Punto de retorno: retorna al paso 2 al intentar una acción protegida.
- 5.C Sin cuenta creada. Disparador: aún no se completó el primer ingreso. Acción: el servicio redirige al alta de credenciales (CU-08). Punto de retorno: el flujo continúa en CU-08.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| AUTH_CREDENCIALES_INVALIDAS | El identificador o la contraseña no coinciden con la cuenta | No abre sesión; informa credenciales inválidas sin precisar cuál falló y registra el intento |
| AUTH_DEMASIADOS_INTENTOS | Se superó el límite de intentos fallidos consecutivos | Aplica una demora o bloqueo temporal de nuevos intentos y registra la situación |
| AUTH_SIN_CUENTA | No existe cuenta de administrador todavía | Redirige al primer ingreso (CU-08) en lugar de a la autenticación |

## 7. Postcondiciones

- En caso de éxito: existe una sesión autorizada para el administrador; las funciones protegidas quedan accesibles.
- En caso de fallo: no se abre sesión; las funciones protegidas permanecen inaccesibles; el intento queda registrado.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una cuenta de administrador existente y credenciales correctas | El administrador ingresa identificador y contraseña válidos | El servicio abre una sesión autorizada y lo dirige al panel |
| CA-02 | Una cuenta de administrador existente | El administrador ingresa una contraseña incorrecta | El servicio no abre sesión, informa credenciales inválidas con código AUTH_CREDENCIALES_INVALIDAS y registra el intento |
| CA-03 | Una sesión activa que superó su tiempo de vigencia | El administrador intenta una acción protegida | El servicio invalida la sesión y exige autenticarse de nuevo |
| CA-04 | Un sistema sin cuenta de administrador creada | El administrador intenta autenticarse | El servicio redirige al primer ingreso con código AUTH_SIN_CUENTA |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-12, RN-13 |
| Historias de usuario a generar | US a generar en 06 (autenticación; cierre de sesión; vencimiento de sesión; límite de intentos) |
| Componentes esperados | Servicio de autenticación; gestión de sesión; verificación contra resguardo de hash (referencia tentativa a 05) |
| Tests previstos | Pruebas de credenciales válidas e inválidas; pruebas de vencimiento de sesión; pruebas de límite de intentos (referencia tentativa a 08) |

## 10. Notas y supuestos

- La autorización por rol administrador único se especifica como invariante en RN-12; este CU abre la sesión que esa regla protege.
- El identity provider es local, sin proveedor externo.
- El mensaje de error no revela si falló el identificador o la contraseña, para no facilitar la enumeración de cuentas.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-05 y del requisito de autenticación del intake |
