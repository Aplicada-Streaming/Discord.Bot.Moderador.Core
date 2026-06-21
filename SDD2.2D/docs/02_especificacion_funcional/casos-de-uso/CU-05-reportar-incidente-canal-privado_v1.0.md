# CU-05 — Reportar a un canal privado los mensajes accionados y los canales afectados

**Proyecto:** discord-bots-admin
**Documento:** CU-05-reportar-incidente-canal-privado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Enviar a un canal de salida privado, apenas se dispara una política, un reporte con los mensajes que motivaron la acción y la lista de canales afectados, para que el administrador se entere del evento en el momento y pueda detectar a tiempo un posible falso positivo. Es la primera superficie de control de la trazabilidad de incidentes.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Compone y envía el reporte al canal de salida designado |
| Plataforma de mensajería | Sistema | Entrega el reporte al canal de salida privado |
| Administrador del sistema | Secundario | Designa el canal de salida y lee los reportes para detectar falsos positivos |

## 3. Precondiciones

- Existe un canal de salida lógico designado para reportes de moderación en el servidor.
- Se disparó una política (por ráfaga o por contenido) que tomó la copia de los mensajes involucrados.
- El bot dispone de permiso para escribir en el canal de salida designado.

## 4. Flujo principal

1. El servicio toma el incidente recién registrado con la copia de mensajes y la lista de canales afectados.
2. El servicio compone el reporte con el emisor, los mensajes que dispararon la acción, los canales afectados y la acción ejecutada o simulada.
3. El servicio identifica el canal de salida lógico designado para reportes de moderación.
4. El servicio envía el reporte al canal de salida privado a través de la plataforma.
5. El servicio confirma el envío y vincula el reporte enviado con el incidente registrado.

## 5. Flujos alternativos

- 5.A Incidente simulado. Disparador: el incidente proviene de una política en modo simulación. Acción: el reporte se marca explícitamente como simulación, indicando la acción que se habría ejecutado. Punto de retorno: retorna al paso 4 enviando el reporte etiquetado como simulado.
- 5.B Incidente no accionable por jerarquía. Disparador: la acción no pudo ejecutarse por jerarquía o permisos (RN-01). Acción: el reporte incluye la advertencia de jerarquía o permisos faltantes. Punto de retorno: retorna al paso 4 enviando el reporte con la advertencia.
- 5.C Múltiples canales de salida. Disparador: el servidor tiene más de un canal de salida con propósito lógico distinto. Acción: el servicio envía el reporte al canal cuyo propósito corresponde al tipo de reporte. Punto de retorno: retorna al paso 4 dirigiendo el envío al canal adecuado.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| REPORTE_CANAL_NO_DESIGNADO | No hay un canal de salida designado para reportes en el servidor | No envía el reporte; conserva el incidente en el panel y registra la ausencia de canal para que el administrador lo configure |
| REPORTE_SIN_PERMISO_ESCRITURA | El bot no tiene permiso para escribir en el canal de salida designado | No envía el reporte; registra el fallo y lo deja visible en el panel de incidentes |
| REPORTE_FALLA_ENVIO | La plataforma rechaza o no confirma el envío del reporte | Reintenta según la política de la integración; si persiste, registra el fallo y conserva el incidente para revisión desde el panel (CU-06) |

## 7. Postcondiciones

- En caso de éxito: el reporte queda publicado en el canal de salida privado y vinculado al incidente; el administrador puede verlo de inmediato.
- En caso de fallo: el reporte no se publica; el incidente queda igualmente registrado y disponible desde el panel (CU-06), con el motivo del fallo de envío.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un canal de salida privado designado y una política en ejecución real que baneó a un emisor en 4 canales | Se registra el incidente | El servicio publica en el canal de salida un reporte con el emisor, los mensajes accionados y los 4 canales afectados |
| CA-02 | Una política en modo simulación que habría baneado a un emisor | Se registra el incidente simulado | El servicio publica un reporte etiquetado como simulación con la acción que se habría ejecutado |
| CA-03 | Un servidor sin canal de salida designado | Se dispara una política | El servicio no envía reporte, conserva el incidente en el panel y registra código REPORTE_CANAL_NO_DESIGNADO |
| CA-04 | Un incidente no accionable por jerarquía superior del emisor | Se compone el reporte | El reporte incluye la advertencia de jerarquía o permisos faltantes |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-09, RN-11 |
| Historias de usuario a generar | US a generar en 06 (reporte de incidentes a canal privado; etiqueta de simulación; múltiples canales de salida) |
| Componentes esperados | Compositor de reportes; integración de envío a canal; vínculo reporte-incidente (referencia tentativa a 05) |
| Tests previstos | Pruebas de composición del reporte; pruebas de envío con y sin canal designado; pruebas de etiqueta de simulación (referencia tentativa a 08) |

## 10. Notas y supuestos

- El canal de salida es un canal designado con un nombre lógico; el sistema lo referencia por su propósito, no por su nombre visible.
- La existencia de múltiples canales de salida con propósito distinto es una capacidad prevista en el alcance; en el recorte mínimo puede existir un único canal de reporte.
- El reporte no restaura mensajes; solo presenta la copia tomada al momento del incidente.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-04 y del riesgo R-01 del intake |
