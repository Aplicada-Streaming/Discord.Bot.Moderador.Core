# CU-13 — Reconectar automáticamente y mostrar el estado de conexión de cada servidor

**Proyecto:** discord-bots-admin
**Documento:** CU-13-reconectar-y-mostrar-estado-conexion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Recuperar automáticamente la conexión al canal de eventos de cada servidor ante caídas transitorias y mantener visible para el administrador el estado de conexión de cada servidor, para que la moderación no quede inactiva en silencio y una pérdida de protección sea detectable.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Detecta caídas, reconecta y actualiza el estado de conexión de cada servidor |
| Plataforma de mensajería | Sistema | Provee el canal de eventos y acepta la reconexión |
| Administrador del sistema | Secundario | Observa el estado de conexión de cada servidor en el panel |

## 3. Precondiciones

- Existe al menos un servidor activo con su conexión al canal de eventos establecida.
- El servidor superó la prueba de configuración con su token válido (CU-12).

## 4. Flujo principal

1. El servicio mantiene la conexión al canal de eventos de cada servidor activo y refleja su estado como conectado.
2. El servicio detecta una caída de la conexión del canal de eventos de un servidor.
3. El servicio marca al servidor como desconectado y actualiza el estado visible en el panel.
4. El servicio intenta reconectar automáticamente según su política de reintento.
5. Al restablecer la conexión, el servicio vuelve a marcar al servidor como conectado y reanuda la evaluación de mensajes.
6. El servicio mantiene el estado de conexión actualizado dentro del tiempo objetivo de refresco.

## 5. Flujos alternativos

- 5.A Caída prolongada. Disparador: la reconexión no se logra tras varios intentos. Acción: el servicio mantiene al servidor como desconectado, sigue reintentando y deja el estado visible para el administrador. Punto de retorno: al lograr conexión retorna al paso 5.
- 5.B Token invalidado durante la operación. Disparador: la reconexión falla porque la credencial dejó de ser válida. Acción: el servicio marca al servidor como desconectado por token inválido e indica que requiere re-validación (CU-12). Punto de retorno: el flujo deriva a la corrección del token y nueva prueba.
- 5.C Mensajes perdidos durante la caída. Disparador: durante la desconexión llegan mensajes que no se reciben. Acción: esos mensajes no se evalúan; el borrado retroactivo del baneo cubre lo que quede dentro de la ventana de borrado al accionar. Punto de retorno: al reconectar, el servicio reanuda la evaluación de los mensajes nuevos.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CONEXION_RECONEXION_AGOTADA | Se agotan los reintentos de reconexión sin éxito | Mantiene el servidor como desconectado, sigue reintentando con la política definida y conserva el estado visible |
| CONEXION_TOKEN_INVALIDO | La credencial se revocó o venció durante la operación | Marca el servidor como desconectado por token inválido y solicita la re-validación (CU-12) |
| CONEXION_ESTADO_NO_ACTUALIZA | El estado de conexión no logra refrescarse en el panel dentro del tiempo objetivo | Registra la demora y reintenta la actualización del estado |

## 7. Postcondiciones

- En caso de éxito: la conexión queda restablecida y el servidor vuelve a estado conectado, con la evaluación de mensajes reanudada.
- En caso de fallo: el servidor permanece desconectado, con el estado visible para el administrador y el motivo registrado.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un servidor activo conectado al canal de eventos | La conexión cae de forma transitoria | El servicio marca el servidor como desconectado, reconecta automáticamente y vuelve a marcarlo como conectado |
| CA-02 | Un servidor que pasa de conectado a desconectado | Cambia el estado de conexión | El panel refleja el nuevo estado dentro del tiempo objetivo de refresco |
| CA-03 | Un servidor cuyo token se revoca durante la operación | La reconexión falla por credencial inválida | El servicio marca el servidor como desconectado por token inválido con código CONEXION_TOKEN_INVALIDO y solicita re-validación |
| CA-04 | Un servidor que sufre una caída prolongada | Los reintentos no logran reconectar | El servicio mantiene el estado desconectado visible y continúa reintentando |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-14, RN-16 |
| Historias de usuario a generar | US a generar en 06 (reconexión automática; estado de conexión por servidor; aviso de token invalidado) |
| Componentes esperados | Gestor de conexiones de gateway; servicio de estado de servidor; panel de estado (referencia tentativa a 05) |
| Tests previstos | Pruebas de reconexión tras caída; pruebas de actualización del estado; pruebas de token invalidado en operación (referencia tentativa a 08) |

## 10. Notas y supuestos

- Los mensajes no recibidos durante una caída no se evalúan; el borrado retroactivo al banear cubre lo que quede dentro de la ventana de borrado.
- El estado de conexión visible evita la falsa sensación de seguridad de creer protegido un servidor que dejó de recibir eventos.
- Cada servidor activo mantiene su propia conexión de gateway; en v1 la operación se prevé para un servidor por la memoria del dispositivo.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-06 y del riesgo R-04 del intake |
