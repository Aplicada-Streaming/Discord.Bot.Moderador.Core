# CU-12 — Probar la configuración de un servidor antes de activarlo

**Proyecto:** discord-bots-admin
**Documento:** CU-12-probar-configuracion-servidor_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Verificar, antes de activar un servidor, que la credencial es válida, que el bot tiene los permisos necesarios, que su jerarquía de roles le permite actuar, que puede recibir eventos y que existen los canales necesarios, advirtiendo de los faltantes, para que el administrador no confíe en una protección que no existe.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Ejecuta la prueba de configuración y corrige los faltantes detectados |
| Servicio de administración | Sistema | Ejecuta las verificaciones contra la plataforma y reporta el resultado |
| Plataforma de mensajería | Sistema | Responde a la validación de credencial, permisos, jerarquía, eventos y canales |

## 3. Precondiciones

- El administrador está autenticado en el panel (CU-09).
- Existe un servidor registrado con su token (CU-10), todavía inactivo.

## 4. Flujo principal

1. El administrador solicita probar la configuración de un servidor registrado.
2. El servicio descifra el token en memoria y lo usa para conectarse a la plataforma (RN-14).
3. El servicio verifica que la credencial es válida y está vigente.
4. El servicio verifica que el bot tiene los permisos necesarios para banear, borrar mensajes, gestionar roles y escribir en el canal de salida.
5. El servicio verifica la jerarquía de roles del bot respecto de los roles del servidor y detecta roles por encima del bot (RN-01).
6. El servicio verifica que puede recibir eventos del canal de eventos y que existen los canales de salida designados.
7. El servicio compone un resultado con las verificaciones superadas y las advertencias de faltantes.
8. Si no hay faltantes bloqueantes, el servicio habilita la activación del servidor; en caso contrario, mantiene el servidor inactivo (RN-16).

## 5. Flujos alternativos

- 5.A Activación tras corrección. Disparador: el administrador corrige los permisos o la jerarquía y vuelve a probar. Acción: el servicio repite las verificaciones. Punto de retorno: retorna al paso 7 con el nuevo resultado.
- 5.B Re-validación periódica. Disparador: el sistema re-valida la credencial de un servidor ya activo. Acción: ejecuta las verificaciones sobre el servidor en operación. Punto de retorno: retorna al paso 7 y, si la credencial dejó de ser válida, marca el servidor como desconectado (CU-13).
- 5.C Advertencia no bloqueante. Disparador: la prueba detecta una jerarquía que impide actuar sobre algunos roles, sin impedir la operación general. Acción: el servicio registra la advertencia y permite activar dejando constancia. Punto de retorno: retorna al paso 8 habilitando la activación con la advertencia visible.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| PRUEBA_TOKEN_INVALIDO | La credencial es inválida o fue revocada | Marca la prueba como fallida, bloquea la activación (RN-16) y deja el servidor como desconectado |
| PRUEBA_PERMISOS_FALTANTES | El bot carece de permisos requeridos para moderar | Reporta los permisos faltantes y bloquea la activación hasta corregirlos |
| PRUEBA_CANAL_SALIDA_AUSENTE | No existe o no es accesible el canal de salida designado | Reporta la ausencia del canal y, según su criticidad, bloquea o advierte antes de activar |

## 7. Postcondiciones

- En caso de éxito sin faltantes bloqueantes: el servidor queda habilitado para activarse con la prueba superada.
- En caso de faltantes bloqueantes: el servidor permanece inactivo; el resultado detalla los faltantes a corregir.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un servidor registrado con token válido, permisos completos y canal de salida accesible | El administrador ejecuta la prueba de configuración | El servicio reporta todas las verificaciones superadas y habilita la activación |
| CA-02 | Un servidor con token válido pero sin permiso de baneo | El administrador ejecuta la prueba | El servicio reporta el permiso faltante con código PRUEBA_PERMISOS_FALTANTES y bloquea la activación |
| CA-03 | Un servidor cuyo token fue revocado | El administrador ejecuta la prueba | El servicio marca la prueba fallida con código PRUEBA_TOKEN_INVALIDO, bloquea la activación y deja el servidor desconectado |
| CA-04 | Un servidor con token y permisos correctos pero con roles de staff por encima del bot | El administrador ejecuta la prueba | El servicio advierte de la jerarquía que impide actuar sobre esos roles y permite activar dejando la advertencia visible |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-01, RN-12, RN-14, RN-16 |
| Historias de usuario a generar | US a generar en 06 (prueba de configuración; advertencia de permisos y jerarquía; bloqueo de activación) |
| Componentes esperados | Servicio de prueba de configuración; integración de validación con la plataforma; estado de servidor (referencia tentativa a 05) |
| Tests previstos | Pruebas de token válido e inválido; pruebas de permisos faltantes; pruebas de advertencia de jerarquía (referencia tentativa a 08) |

## 10. Notas y supuestos

- La prueba previa mitiga los riesgos de credencial inválida, permisos faltantes y jerarquía insuficiente antes de confiar en la protección.
- La activación bloqueada por faltantes evita la falsa sensación de seguridad de un servidor que no puede ser moderado.
- La distinción entre faltante bloqueante y advertencia no bloqueante se apoya en RN-16 y se detalla en la configuración.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-06 y de los riesgos R-03 y R-04 del intake |
