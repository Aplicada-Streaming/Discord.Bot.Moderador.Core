# CU-16 — Evitar acciones repetidas sobre el mismo usuario durante una ráfaga

**Proyecto:** discord-bots-admin
**Documento:** CU-16-antirrebote-por-usuario_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Impedir que un mismo usuario reciba la misma acción de moderación varias veces durante una única ráfaga, aplicando un antirrebote por usuario, para reducir el ruido, evitar reacciones desproporcionadas y no duplicar incidentes durante un único ataque.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Aplica el antirrebote suprimiendo acciones repetidas sobre un usuario ya accionado |
| Administrador del sistema | Secundario | Configura la ventana de antirrebote por usuario |

## 3. Precondiciones

- Existe una política en ejecución real con una acción configurada (CU-02, CU-03 o CU-04).
- Existe una ventana de antirrebote por usuario configurada (RN-06).
- Un usuario ya fue accionado por la primera coincidencia dentro de la ráfaga vigente.

## 4. Flujo principal

1. El servicio determina que un usuario cumple las condiciones de una política con acción real.
2. El servicio consulta si ese usuario ya fue accionado dentro de la ventana de antirrebote vigente (RN-06).
3. Si el usuario no fue accionado aún, el servicio ejecuta la acción y marca al usuario como accionado con su marca de tiempo.
4. Si el usuario ya fue accionado dentro de la ventana, el servicio suprime la acción repetida y no genera un incidente adicional.
5. Al expirar la ventana de antirrebote, el servicio vuelve a permitir acciones sobre ese usuario.

## 5. Flujos alternativos

- 5.A Acciones distintas en la misma ráfaga. Disparador: una segunda política dispara una acción de tipo distinto sobre el mismo usuario. Acción: el antirrebote aplica según su criterio configurado por usuario; si suprime, no repite; si la acción es de otra naturaleza permitida, la evalúa según RN-06. Punto de retorno: retorna al paso 3 o al 4 según el criterio.
- 5.B Nueva ráfaga tras expirar la ventana. Disparador: el usuario vuelve a disparar una regla después de expirada la ventana. Acción: el servicio trata el caso como una nueva acción permitida. Punto de retorno: retorna al paso 3.
- 5.C Reinicio del servicio durante la ráfaga. Disparador: el servicio se reinicia y pierde el estado de antirrebote en memoria. Acción: tras el reinicio el usuario puede volver a ser accionado, según el trade-off de estado en memoria. Punto de retorno: retorna al paso 1 con el estado reconstruido.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| ANTIRREBOTE_ESTADO_NO_DISPONIBLE | El estado de antirrebote en memoria no está disponible (por ejemplo tras un reinicio) | Trata al usuario como no accionado aún y permite la acción, registrando la situación en el journal |
| ANTIRREBOTE_VENTANA_INVALIDA | La ventana de antirrebote quedó fuera de los límites de su descriptor (RN-10) | Aplica el valor por defecto del descriptor y señala la inconsistencia |
| ANTIRREBOTE_SUPRESION_NO_REGISTRADA | No se pudo registrar la supresión de una acción repetida | Suprime igualmente la acción para no duplicarla y deja constancia en el journal |

## 7. Postcondiciones

- En caso de éxito: el usuario recibe la acción una sola vez por ráfaga dentro de la ventana; las repeticiones quedan suprimidas sin generar incidentes adicionales.
- En caso de fallo del estado: el usuario podría recibir la acción más de una vez, sin afectar la corrección de la contención.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una política en ejecución real y un usuario ya baneado por la primera coincidencia, con ventana de antirrebote vigente | Llega un nuevo mensaje del mismo usuario dentro de la ventana | El servicio suprime la acción repetida y no genera un incidente adicional |
| CA-02 | Un usuario accionado cuya ventana de antirrebote expiró | El usuario vuelve a disparar una regla | El servicio trata el caso como una nueva acción permitida y la ejecuta |
| CA-03 | Una ventana de antirrebote configurada fuera de los límites de su descriptor | Se evalúa una acción | El servicio aplica el valor por defecto con código ANTIRREBOTE_VENTANA_INVALIDA |
| CA-04 | Un usuario nunca accionado en la ráfaga vigente | El usuario dispara la regla por primera vez | El servicio ejecuta la acción y marca al usuario como accionado con su marca de tiempo |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 |
| Reglas de negocio aplicables | RN-06, RN-10 |
| Historias de usuario a generar | US a generar en 06 (antirrebote por usuario; ventana de antirrebote configurable; supresión de acciones repetidas) |
| Componentes esperados | Servicio de antirrebote en memoria; ejecutor de acciones; descriptores de parámetros (referencia tentativa a 05) |
| Tests previstos | Pruebas de supresión dentro de la ventana; pruebas de expiración de la ventana; pruebas de estado no disponible (referencia tentativa a 08) |

## 10. Notas y supuestos

- El estado de antirrebote vive en memoria y se pierde ante un reinicio, según el trade-off aceptado; el objetivo es reducir ruido, no garantizar exactamente una sola acción ante reinicios.
- El antirrebote complementa el discriminador de canales distintos y la trazabilidad de incidentes para reducir reacciones desproporcionadas.
- El indicador de negocio exige cero acciones adicionales repetidas sobre el mismo usuario después de la primera durante una misma ráfaga.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-07 y de la precisión de antirrebote del intake |
