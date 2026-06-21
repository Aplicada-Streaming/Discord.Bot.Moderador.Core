# CU-03 — Banear con borrado retroactivo de los mensajes del emisor

**Proyecto:** discord-bots-admin
**Documento:** CU-03-banear-con-borrado-retroactivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Eliminar, en el mismo acto de contención del emisor, los mensajes que ese usuario alcanzó a publicar en todos los canales dentro de una ventana hacia atrás configurable, para que la limpieza del incidente sea una sola operación y el administrador no deba recorrer canal por canal. Acopla la limpieza retroactiva a la acción de baneo.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Ejecuta la acción de baneo con la opción de borrado retroactivo dentro de la ventana configurada |
| Plataforma de mensajería | Sistema | Remueve los mensajes del usuario en el rango solicitado |
| Administrador del sistema | Secundario | Define la ventana de borrado hacia atrás dentro del tope de la plataforma |

## 3. Precondiciones

- Se cumplió una política con una acción de baneo y borrado retroactivo (encadenada con CU-02).
- El bot dispone de permiso para borrar mensajes y banear, y su rol es superior al del usuario objetivo (RN-01).
- La ventana de borrado configurada está dentro del rango permitido por la plataforma, de 0 a 7 días (RN-02).
- Se tomó la copia de los mensajes involucrados antes de ejecutar el borrado.

## 4. Flujo principal

1. El servicio toma la acción de baneo con borrado retroactivo de la política cumplida.
2. El servicio lee la ventana de borrado hacia atrás configurada para la acción.
3. El servicio acota la ventana al tope de la plataforma de 7 días si el valor configurado lo excediera (RN-02).
4. El servicio toma la copia de los mensajes del emisor que caen dentro de la ventana, en todos los canales, antes de removerlos.
5. El servicio ejecuta el baneo del usuario solicitando a la plataforma la remoción de sus mensajes dentro de la ventana de borrado.
6. El servicio registra el incidente con la lista de canales afectados y la copia de los mensajes removidos.
7. El servicio confirma la operación; los canales quedan limpios de la actividad reciente del emisor dentro de la ventana.

## 5. Flujos alternativos

- 5.A Ventana de borrado en cero. Disparador: la acción se configura con ventana de borrado de 0 días. Acción: el servicio banea sin remover mensajes previos. Punto de retorno: retorna al paso 6 registrando el incidente sin canales afectados por borrado.
- 5.B Ráfaga espaciada más allá de la ventana de detección. Disparador: parte de los mensajes quedó fuera de la ventana de detección pero dentro de la ventana de borrado. Acción: el borrado retroactivo limpia esos mensajes previos por estar dentro de la ventana de borrado, aunque no hayan contado para la detección. Punto de retorno: retorna al paso 5 incluyendo esos mensajes en la remoción.
- 5.C Mensajes parcialmente no visibles. Disparador: parte de los mensajes no fueron recibidos por el servicio durante una caída del canal de eventos. Acción: el borrado retroactivo igual los remueve por estar dentro de la ventana de borrado en la plataforma, aunque el servicio no los haya evaluado. Punto de retorno: retorna al paso 6 con los canales efectivamente limpiados.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| BORRADO_VENTANA_FUERA_DE_RANGO | La ventana configurada supera el tope de 7 días de la plataforma (RN-02) | Acota la ventana a 7 días, ejecuta el borrado con el tope y registra el ajuste |
| BORRADO_JERARQUIA_INSUFICIENTE | El bot no puede actuar sobre el usuario por jerarquía o permisos (RN-01) | No ejecuta baneo ni borrado; registra el incidente como no accionable y lo reporta |
| BORRADO_FALLA_PARCIAL | La plataforma remueve solo parte de los mensajes solicitados | Registra el incidente indicando los canales no limpiados por completo para revisión manual; conserva la copia de todos los mensajes involucrados |

## 7. Postcondiciones

- En caso de éxito: el usuario queda baneado y sus mensajes dentro de la ventana de borrado quedan removidos de todos los canales; el incidente conserva la copia de esos mensajes y la lista de canales afectados.
- En caso de fallo: el baneo y el borrado no se completan; el incidente registra el alcance parcial o la no accionabilidad y conserva la copia tomada.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una acción de baneo con ventana de borrado de 1 día y un emisor con mensajes en 5 canales en la última hora | Se ejecuta la acción sobre el emisor | El servicio banea y remueve los mensajes del emisor en los 5 canales dentro del último día, y registra los 5 canales como afectados |
| CA-02 | Una acción configurada con ventana de borrado de 10 días | Se ejecuta la acción sobre el emisor | El servicio acota la ventana a 7 días, ejecuta el borrado con ese tope y registra el ajuste con código BORRADO_VENTANA_FUERA_DE_RANGO |
| CA-03 | Una acción con ventana de borrado de 0 días | Se ejecuta la acción sobre el emisor | El servicio banea sin remover mensajes previos y registra el incidente sin canales afectados por borrado |
| CA-04 | Un emisor con mensajes que el servicio no llegó a evaluar por una caída del canal de eventos, todos dentro de la ventana de borrado | Se ejecuta la acción sobre el emisor | El servicio remueve también esos mensajes por estar dentro de la ventana de borrado |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-05 |
| Historias de usuario a generar | US a generar en 06 (borrado retroactivo acoplado al baneo; configuración de la ventana de borrado) |
| Componentes esperados | Ejecutor de acciones de moderación con opción de purga; integración con la plataforma; registro de incidentes (referencia tentativa a 05) |
| Tests previstos | Pruebas del acotado de la ventana al tope de 7 días; pruebas del borrado multi-canal; pruebas de borrado parcial (referencia tentativa a 08) |

## 10. Notas y supuestos

- Los mensajes removidos no se restauran; el sistema conserva solo la copia tomada para revisión, según el alcance excluido. La reversibilidad alcanza al baneo (CU-07), no al borrado.
- El tope de 7 días lo impone la plataforma; la ventana configurable es el rango de 0 a 7 días.
- La copia previa al borrado es la única evidencia disponible una vez removidos los mensajes; por eso se toma siempre antes de ejecutar.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-02 y de los casos límite del intake |
