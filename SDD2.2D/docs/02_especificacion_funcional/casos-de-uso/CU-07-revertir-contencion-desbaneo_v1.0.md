# CU-07 — Revertir una contención (desbaneo) desde el panel

**Proyecto:** discord-bots-admin
**Documento:** CU-07-revertir-contencion-desbaneo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Permitir que el administrador revierta desde el panel un baneo aplicado por la moderación, cuando la revisión del incidente concluye que fue un falso positivo, para corregir el error sin entrar al servidor. Se acepta que los mensajes ya removidos no se restauran; lo reversible es el baneo del usuario.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Decide y ejecuta la reversión del baneo desde el panel |
| Servicio de moderación | Sistema | Ejecuta la orden de desbaneo contra la plataforma y actualiza el incidente |
| Plataforma de mensajería | Sistema | Recibe y aplica la orden de desbaneo sobre el usuario |

## 3. Precondiciones

- El administrador está autenticado en el panel (CU-09).
- Existe un incidente con una acción de baneo ejecutada en modo real (CU-02 o CU-03).
- El usuario figura como baneado en el servidor del incidente.
- El bot dispone de permiso para revertir baneos en ese servidor.

## 4. Flujo principal

1. El administrador abre el detalle de un incidente con baneo real (desde CU-06).
2. El administrador solicita revertir la contención del usuario.
3. El servicio confirma la intención de revertir antes de ejecutar.
4. El servicio ejecuta la orden de desbaneo del usuario contra la plataforma.
5. El servicio actualiza el incidente registrando la reversión, su autor y la fecha.
6. El servicio informa que el usuario fue desbaneado y advierte que los mensajes removidos no se restauran.

## 5. Flujos alternativos

- 5.A Incidente sin baneo real. Disparador: el incidente corresponde a una simulación o a una acción que no fue baneo. Acción: el servicio no ofrece la reversión. Punto de retorno: el flujo termina sin cambios.
- 5.B Usuario ya desbaneado. Disparador: el usuario fue desbaneado por otra vía o en una reversión previa. Acción: el servicio informa que ya no está baneado y marca el incidente como revertido si no lo estaba. Punto de retorno: el flujo termina sin ejecutar un nuevo desbaneo.
- 5.C Cancelación de la confirmación. Disparador: el administrador no confirma la reversión. Acción: el servicio no ejecuta el desbaneo. Punto de retorno: retorna al detalle del incidente sin cambios.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| DESBANEO_SIN_PERMISO | El bot carece de permiso para revertir baneos en el servidor (RN-01) | No ejecuta la reversión; informa el permiso faltante y registra el intento |
| DESBANEO_FALLA_PLATAFORMA | La plataforma rechaza o no confirma el desbaneo | Reintenta según la política de la integración; si persiste, informa el fallo y deja el incidente sin marcar como revertido |
| DESBANEO_SIN_AUTORIZACION | Quien solicita la reversión no es el administrador autenticado (RN-12) | Rechaza la operación; redirige a la autenticación (CU-09) |

## 7. Postcondiciones

- En caso de éxito: el usuario queda desbaneado en el servidor; el incidente registra la reversión con autor y fecha; los mensajes removidos siguen sin restaurarse.
- En caso de fallo: el usuario permanece baneado; el incidente registra el intento fallido para revisión.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un administrador autenticado y un incidente con baneo real de un usuario que sigue baneado | El administrador confirma la reversión | El servicio desbanea al usuario, registra la reversión con autor y fecha e informa que los mensajes no se restauran |
| CA-02 | Un incidente en modo simulación | El administrador abre su detalle | El servicio no ofrece la opción de revertir |
| CA-03 | Un incidente con baneo real cuyo usuario ya fue desbaneado por otra vía | El administrador solicita revertir | El servicio informa que el usuario ya no está baneado y marca el incidente como revertido |
| CA-04 | Un bot sin permiso para revertir baneos en el servidor | El administrador confirma la reversión | El servicio no desbanea, informa el permiso faltante con código DESBANEO_SIN_PERMISO y registra el intento |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-12 |
| Historias de usuario a generar | US a generar en 06 (reversión de baneo desde el panel; confirmación previa; registro de la reversión) |
| Componentes esperados | Acción de desbaneo; integración con la plataforma; actualización del incidente (referencia tentativa a 05) |
| Tests previstos | Pruebas del desbaneo exitoso; pruebas de incidente no reversible; pruebas de permiso faltante (referencia tentativa a 08) |

## 10. Notas y supuestos

- La reversibilidad alcanza solo al baneo; el borrado de mensajes de CU-03 no es reversible por limitación de la plataforma.
- La reversión es una acción del administrador, distinta de las acciones automáticas del motor; queda auditada en el incidente.
- El indicador de negocio de falsos positivos se mide sobre los baneos revertidos por este CU.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-04 y del riesgo R-01 del intake |
