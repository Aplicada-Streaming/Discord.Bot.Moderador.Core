# CU-02 — Banear automáticamente al emisor de la ráfaga

**Proyecto:** discord-bots-admin
**Documento:** CU-02-banear-emisor-rafaga_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Ejecutar la contención del emisor cuando se cumple una política disparada por la condición de ráfaga distribuida, baneándolo de forma automática y sin intervención humana, para cortar la propagación del spam en su inicio. Es la acción de contención que materializa el valor central del producto sobre el disparador que produce CU-01.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Evalúa la política y ejecuta la acción de baneo contra la plataforma |
| Plataforma de mensajería | Sistema | Recibe y aplica la orden de baneo sobre el usuario en el servidor |
| Administrador del sistema | Secundario | Configura la política y la acción de baneo, y revisa el resultado en los incidentes |

## 3. Precondiciones

- La condición de ráfaga distribuida quedó marcada para el par usuario-servidor (CU-01).
- Existe un evento o política con un grupo de reglas que incluye la regla de ráfaga y una acción de baneo configurada.
- La política no está en modo simulación (RN-09); si lo está, se aplica CU-14.
- El bot dispone de permiso de baneo y su rol es jerárquicamente superior al del usuario objetivo (RN-01).
- El usuario objetivo no fue accionado previamente dentro de la misma ráfaga (RN-06, antirrebote).

## 4. Flujo principal

1. El servicio toma la condición de ráfaga marcada y evalúa las políticas por prioridad con primera coincidencia (RN-04).
2. El servicio verifica que la política aplicable está en modo ejecución real, no simulación.
3. El servicio verifica que el usuario objetivo no fue accionado ya en la ráfaga vigente (antirrebote, RN-06).
4. El servicio verifica la jerarquía de roles y los permisos del bot sobre el usuario objetivo (RN-01).
5. El servicio toma una copia de los mensajes involucrados antes de ejecutar la acción, para la trazabilidad del incidente.
6. El servicio ejecuta la orden de baneo del usuario contra la plataforma.
7. El servicio registra el incidente con la acción ejecutada, los mensajes copiados y los canales afectados (alimenta CU-05 y CU-06).
8. El servicio marca al usuario como ya accionado en la ráfaga vigente para no repetir la acción.

## 5. Flujos alternativos

- 5.A Política en modo simulación. Disparador: la política aplicable tiene activado el modo simulación. Acción: el flujo se desvía a CU-14, que registra lo que se habría hecho sin ejecutarlo. Punto de retorno: termina sin banear; el incidente queda marcado como simulado.
- 5.B Acciones múltiples encadenadas. Disparador: la política define más de una acción (por ejemplo reportar y luego banear). Acción: el servicio ejecuta las acciones en el orden configurado (RN-05). Punto de retorno: tras ejecutar todas las acciones de la política, retorna al paso 7.
- 5.C Continuación a otra política. Disparador: la política tiene la bandera continuar activa. Acción: tras ejecutarla, el servicio sigue evaluando políticas de menor prioridad sobre el mismo mensaje (RN-04). Punto de retorno: vuelve al paso 1 con la siguiente política.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| BANEO_JERARQUIA_INSUFICIENTE | El usuario objetivo tiene un rol superior o igual al del bot, o el bot carece de permiso de baneo (RN-01) | No ejecuta el baneo; registra el incidente como no accionable por jerarquía o permisos y lo reporta al canal de incidencias (CU-05) |
| BANEO_USUARIO_YA_ACCIONADO | El usuario ya fue accionado en la ráfaga vigente (RN-06) | No repite la acción; el intento se descarta silenciosamente y no genera un nuevo incidente |
| BANEO_FALLA_PLATAFORMA | La plataforma rechaza o no confirma la orden de baneo | Reintenta según política de la integración; si persiste, registra el incidente como acción fallida y lo reporta para revisión manual |

## 7. Postcondiciones

- En caso de éxito: el usuario queda baneado en el servidor; existe un incidente con la copia de mensajes y los canales afectados; el usuario queda marcado como accionado en la ráfaga vigente.
- En caso de fallo: el usuario no queda baneado; existe un incidente que registra el motivo del fallo o de la no accionabilidad para revisión del administrador.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una política en ejecución real con acción de baneo y un emisor con rol inferior al bot | La condición de ráfaga se marca para ese emisor | El servicio banea al emisor, registra el incidente y lo marca como accionado |
| CA-02 | Una política en ejecución real y un emisor que es administrador del servidor con rol superior al bot | La condición de ráfaga se marca para ese emisor | El servicio no banea, registra el incidente con código BANEO_JERARQUIA_INSUFICIENTE y lo reporta al canal de incidencias |
| CA-03 | Una política en modo simulación con acción de baneo | La condición de ráfaga se marca para un emisor | El servicio no banea y registra el incidente como simulado con la acción que se habría ejecutado |
| CA-04 | Un emisor ya baneado en la ráfaga vigente por la primera coincidencia | Llega un nuevo mensaje del mismo emisor dentro de la misma ráfaga | El servicio no repite el baneo por el antirrebote (RN-06) |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-04, RN-05, RN-06, RN-07, RN-09 |
| Historias de usuario a generar | US a generar en 06 (baneo automático del emisor; encadenamiento de acciones; manejo de jerarquía insuficiente) |
| Componentes esperados | Ejecutor de acciones; integración con la plataforma de moderación; registro de incidentes (referencia tentativa a 05) |
| Tests previstos | Pruebas del ejecutor de baneo; pruebas de la verificación de jerarquía; pruebas de antirrebote (referencia tentativa a 08) |

## 10. Notas y supuestos

- El baneo es reversible mediante CU-07 (desbaneo); el borrado de mensajes que lo acompaña en CU-03 no lo es.
- La copia de mensajes se toma antes de ejecutar para garantizar que la evidencia sobreviva a la remoción de los mensajes del servidor.
- Este CU describe la acción de baneo como contención; el borrado retroactivo de los mensajes se especifica en CU-03 y suele encadenarse con esta acción.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-01 y de los casos límite del intake |
