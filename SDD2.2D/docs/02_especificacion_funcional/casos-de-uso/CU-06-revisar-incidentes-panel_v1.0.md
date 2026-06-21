# CU-06 — Revisar incidentes y mensajes accionados desde el panel

**Proyecto:** discord-bots-admin
**Documento:** CU-06-revisar-incidentes-panel_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Permitir que el administrador consulte desde el panel los incidentes registrados, con la copia de los mensajes que dispararon cada acción y la lista de canales afectados, para revisar la evidencia, evaluar si hubo un falso positivo y decidir una eventual reversión, sin necesidad de entrar al servidor.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Consulta y revisa los incidentes y su evidencia desde el panel |
| Servicio de moderación | Sistema | Provee los incidentes registrados y la copia de los mensajes conservados |

## 3. Precondiciones

- El administrador está autenticado en el panel (CU-09).
- Existe al menos un incidente registrado por una política disparada (CU-02, CU-03 o CU-04).
- La copia de los mensajes accionados se conservó al registrar el incidente.

## 4. Flujo principal

1. El administrador abre la sección de incidentes del panel.
2. El servicio presenta la lista de incidentes ordenada por fecha, con emisor, tipo de regla que disparó, acción resultante y modo (real o simulación).
3. El administrador selecciona un incidente para revisar su detalle.
4. El servicio muestra la copia de los mensajes que dispararon la acción, la lista de canales afectados y el resultado de la acción.
5. El administrador evalúa la evidencia y decide si fue un falso positivo.
6. Si concluye que fue un falso positivo y la acción fue un baneo en ejecución real, el administrador puede iniciar la reversión (CU-07) desde el mismo detalle.

## 5. Flujos alternativos

- 5.A Filtro de incidentes. Disparador: el administrador necesita acotar la lista. Acción: aplica filtros por servidor, por modo (real o simulación) o por rango de fechas. Punto de retorno: retorna al paso 2 con la lista filtrada.
- 5.B Incidente simulado. Disparador: el incidente seleccionado proviene de una política en modo simulación. Acción: el detalle indica que la acción no se ejecutó y muestra lo que se habría hecho; no ofrece reversión porque no hubo acción real. Punto de retorno: retorna al paso 4 mostrando el detalle simulado.
- 5.C Incidente no accionable. Disparador: la acción no se ejecutó por jerarquía o permisos. Acción: el detalle muestra la advertencia y no ofrece reversión. Punto de retorno: retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| INCIDENTE_NO_ENCONTRADO | El incidente solicitado no existe o fue purgado por retención acotada | Informa que el incidente no está disponible y vuelve a la lista |
| REVISION_SIN_AUTENTICACION | La sesión del administrador expiró o no está autenticada (RN-12) | Redirige a la autenticación (CU-09) y no muestra los incidentes |
| EVIDENCIA_NO_DISPONIBLE | La copia de mensajes del incidente no pudo recuperarse | Muestra el incidente con los metadatos disponibles e indica que la copia no está accesible |

## 7. Postcondiciones

- En caso de éxito: el administrador visualiza el incidente y su evidencia; queda en condiciones de decidir una reversión.
- En caso de fallo: el incidente o su evidencia no se muestran; el estado del sistema no cambia.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un administrador autenticado y 3 incidentes registrados | El administrador abre la sección de incidentes | El servicio lista los 3 incidentes con emisor, tipo de regla, acción y modo |
| CA-02 | Un incidente de baneo en ejecución real con copia de 6 mensajes en 4 canales | El administrador abre el detalle del incidente | El servicio muestra los 6 mensajes copiados y los 4 canales afectados, y ofrece la opción de revertir |
| CA-03 | Un incidente en modo simulación | El administrador abre su detalle | El servicio muestra la acción que se habría ejecutado y no ofrece reversión |
| CA-04 | Una sesión de administrador expirada | El administrador intenta abrir la sección de incidentes | El servicio redirige a la autenticación con código REVISION_SIN_AUTENTICACION y no muestra incidentes |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-09, RN-11, RN-12 |
| Historias de usuario a generar | US a generar en 06 (listado y detalle de incidentes; filtros; visualización de la copia de mensajes) |
| Componentes esperados | Página de incidentes del panel; servicio de consulta de incidentes; almacenamiento de la copia de mensajes (referencia tentativa a 05) |
| Tests previstos | Pruebas del listado y detalle; pruebas de filtros; pruebas de control de acceso a la sección (referencia tentativa a 08) |

## 10. Notas y supuestos

- La copia de mensajes es la única evidencia disponible una vez removidos del servidor; el panel no restaura mensajes.
- La retención de incidentes es acotada conforme al marco de minimización de datos; un incidente purgado deja de estar disponible.
- El detalle de la presentación visual de la sección corresponde a la categoría 03; aquí solo se define qué información se consulta.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-04 y del riesgo R-01 del intake |
