# Wireframes — Revisión de incidentes y desbaneo

**Proyecto:** discord-bots-admin
**Documento:** wireframes-revision-de-incidentes-y-desbaneo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Pantalla y propósito

Superficie de control de falsos positivos. El administrador consulta los incidentes registrados con la copia de los mensajes que dispararon cada acción y la lista de canales afectados, evalúa si hubo un falso positivo y, cuando corresponde a un baneo en ejecución real, revierte el baneo desde el mismo detalle. Se acepta que los mensajes removidos no se restauran; lo reversible es el baneo del usuario. Es la materialización del reporte que llega al canal privado (CU-05) llevado al panel para revisión y reversión.

## 2. Layout

Shell de aplicación del catálogo. Vista maestro-detalle: lista de incidentes a la izquierda o arriba con su barra de filtros, detalle del incidente seleccionado a la derecha o debajo.

Lista de incidentes:

```
+-----------+-------------------------------------------------------+
| barra     | Incidentes                                            |
| lateral   | [Buscar...]  [Servidor v] [Modo v] [Fechas v]         |
|           | +-------------------------------------------------+    |
|           | | Fecha       | Emisor   | Regla    | Accion |Modo |    |
|           | |-------------+----------+----------+--------+-----|    |
|           | | 2026-06-20  | usr#1234 | Conducta | Baneo  |Real |    |
|           | | 2026-06-20  | usr#9876 | Contenido| Reporte|Sim. |    |
|           | | 2026-06-19  | usr#5555 | Conducta | Baneo  |Real*|    |
|           | +-------------------------------------------------+    |
+-----------+-------------------------------------------------------+
```

Detalle del incidente seleccionado (baneo real revisable):

```
+-------------------------------------------------------+
| Incidente 2026-06-20  - "Corte de rafaga"  [Real]     |
| Emisor usr#1234   Resultado: baneo ejecutado          |
|                                                       |
| Canales afectados (4): anuncios, general, ayuda, off  |
|                                                       |
| Mensajes accionados (copia conservada):               |
|  +-------------------------------------------------+  |
|  | [canal] [hora]  <contenido copiado del mensaje> |  |
|  | [canal] [hora]  <contenido copiado del mensaje> |  |
|  +-------------------------------------------------+  |
|                                                       |
| Reversion: ninguna                                    |
|                                  [ Revertir baneo ]   |
+-------------------------------------------------------+
```

Diálogo de confirmación de desbaneo:

```
+-- Revertir el baneo de usr#1234? ----------------------+
| Se desbaneara al usuario en el servidor "Comunidad".   |
| Los mensajes ya removidos no se restauran.             |
|                          [ Cancelar ] [ Revertir ]     |
+--------------------------------------------------------+
```

## 3. Componentes principales

| Componente | Patrón del catálogo | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- | --- |
| Barra de filtros | Búsqueda y filtros (4.10) | Acotar la lista de incidentes | Búsqueda; filtros por servidor, modo y rango de fechas | Filtra la grilla; resultado vacío muestra estado vacío con acción |
| Grilla de incidentes | ABM listado (4.3) | Listar incidentes por fecha | Fecha, emisor, tipo de regla, acción, modo | Filas seleccionables; orden cronológico; badge de modo (real o simulación) |
| Badge de modo | Badge de estado (4.8) | Distinguir real de simulación | Texto "Real" o "Simulación" con su tint | Estado por color más texto |
| Badge de resultado | Badge de estado (4.8) | Indicar el resultado de la acción | Ejecutada, simulada, no accionable o fallida | Texto explícito; el no accionable por jerarquía se rotula |
| Panel de detalle | ABM formulario (4.4, solo lectura) | Mostrar la evidencia del incidente | Emisor, evento, modo, resultado, reversión | Solo lectura; ofrece la reversión cuando corresponde |
| Lista de canales afectados | Estados y feedback (5) | Enumerar los canales alcanzados | Cantidad y nombres de canales | Lectura |
| Copia de mensajes accionados | Representación reutilizada | Conservar la evidencia revisable | Canal, hora y contenido copiado de cada mensaje | Solo lectura; no restaura ni vincula al mensaje original removido |
| Botón Revertir baneo | Botones (4.9, destructivo controlado) | Iniciar la reversión | Verbo de la acción | Visible solo si el resultado fue baneo real; abre la confirmación |
| Diálogo de confirmación | Diálogos / confirmación (catálogo) | Confirmar la reversión antes de ejecutar | Alcance del desbaneo y aviso de que los mensajes no se restauran | Cancelar no ejecuta; Revertir ejecuta el desbaneo |
| Aviso de reversión registrada | Estados y feedback (5) | Dejar constancia de la reversión | Autor y fecha de la reversión | Aparece tras un desbaneo exitoso |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Abrir incidentes | El administrador entra a la sección | Lista de incidentes por fecha con emisor, tipo de regla, acción y modo (CU-06) | Sesión activa |
| Filtrar | El administrador aplica filtros | La grilla se acota por servidor, modo o rango de fechas (CU-06 5.A) | Hay incidentes |
| Ver detalle | El administrador selecciona un incidente | Muestra copia de mensajes, canales afectados y resultado (CU-06) | El incidente existe y su evidencia se conservó |
| Detalle de incidente simulado | Se abre un incidente en modo simulación | Indica que la acción no se ejecutó y muestra lo que se habría hecho; no ofrece reversión (CU-06 5.B) | El incidente es simulado |
| Detalle no accionable | Se abre un incidente que no pudo ejecutarse por jerarquía o permisos | Muestra la advertencia y no ofrece reversión (CU-06 5.C) | El incidente quedó no accionable |
| Iniciar reversión | El administrador activa "Revertir baneo" | Abre el diálogo de confirmación (CU-07) | Baneo en ejecución real; usuario aún baneado |
| Confirmar reversión | El administrador confirma en el diálogo | El sistema desbanea, registra la reversión con autor y fecha e informa que los mensajes no se restauran (CU-07) | El bot tiene permiso de desbanear |
| Cancelar reversión | El administrador cancela el diálogo | No se ejecuta el desbaneo; vuelve al detalle sin cambios (CU-07 5.C) | Diálogo abierto |
| Reversión sin permiso | El bot carece de permiso para desbanear | No desbanea; informa el permiso faltante y registra el intento (DESBANEO_SIN_PERMISO) | — |
| Usuario ya desbaneado | El usuario fue desbaneado por otra vía | Informa que ya no está baneado y marca el incidente como revertido (CU-07 5.B) | — |
| Sesión expirada | La sesión del administrador venció | Redirige al ingreso y no muestra los incidentes (REVISION_SIN_AUTENTICACION) | — |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | No hay incidentes registrados | Bloque centrado con ilustración SVG y texto orientativo; sin acción de creación (los incidentes los genera el motor) |
| Vacío por filtro | Los filtros no arrojan resultados | Estado vacío con acción de limpiar filtros |
| Cargando | Carga del listado o del detalle en curso | Skeleton de filas en la lista; skeleton de bloques en el detalle |
| Con datos | Hay incidentes | Lista con badges de modo y resultado; detalle con evidencia |
| Detalle simulado | El incidente proviene de una política en simulación | Detalle con la acción que se habría ejecutado; sin botón de reversión |
| Detalle no accionable | La acción no se ejecutó por jerarquía o permisos | Detalle con advertencia; sin botón de reversión |
| Confirmación abierta | El administrador inició la reversión | Diálogo con alcance y aviso de no restauración de mensajes |
| Error | Incidente no encontrado o evidencia no disponible (INCIDENTE_NO_ENCONTRADO, EVIDENCIA_NO_DISPONIBLE) | El incidente muestra los metadatos disponibles e indica que la copia no está accesible; vuelve a la lista si no existe |
| Error de reversión | Falla de plataforma o falta de permiso (DESBANEO_FALLA_PLATAFORMA, DESBANEO_SIN_PERMISO) | Mensaje con la causa; el incidente queda sin marcar como revertido y registra el intento |
| Sin autenticación | La sesión expiró (REVISION_SIN_AUTENTICACION) | Redirección al ingreso; no se muestran incidentes |
| Éxito | Desbaneo ejecutado | Confirmación con el aviso de que los mensajes no se restauran; el detalle registra la reversión con autor y fecha |

## 6. Versión móvil o responsive

La vista maestro-detalle pasa a navegación en dos pasos bajo el punto de quiebre del catálogo: primero la lista filtrable, luego el detalle a pantalla completa con retorno a la lista. La copia de mensajes accionados reflujen verticalmente. El diálogo de confirmación se centra y ocupa el ancho disponible. Contenido legible sin scroll horizontal a 320px.

## 7. Notas de implementación

- Accesibilidad: la grilla con encabezados semánticos; cada badge combina color y texto; el detalle como región anunciada al seleccionar; el diálogo de confirmación atrapa el foco mientras está abierto y lo devuelve al cerrarse; el botón de reversión, por ser una acción de consecuencia, exige confirmación explícita; foco visible y navegación por teclado completa.
- La copia de mensajes es la única evidencia disponible: la pantalla no restaura ni enlaza al mensaje original removido; deja claro que el desbaneo no restaura mensajes.
- Performance percibida: skeletons en lista y detalle; la reversión cruza a la plataforma externa y no se declara exitosa hasta confirmar el resultado (no es optimista); el botón queda ocupado durante la operación.
- La retención de incidentes es acotada; un incidente purgado deja de estar disponible y la pantalla lo informa sin inventar evidencia.
- Internacionalización: fechas en formato local con orden año-mes-día para orden cronológico inequívoco; los nombres de canal y el contenido copiado toleran expansión.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Administrador del sistema, operador único (00) |
| CU origen | CU-06 (revisar incidentes y mensajes accionados); CU-07 (revertir una contención / desbaneo); CU-05 (reporte a canal privado, como origen del incidente revisado) |
| Reglas de negocio relevantes | RN-01 (jerarquía de roles del bot, permiso de desbanear); RN-09 (la simulación no ejecuta); RN-11 (integridad de la evidencia del incidente); RN-12 (autorización por rol administrador único) |
| Marco experiencia-de-uso aplicado | experiencia-de-uso_v1.0.md §3.4, §4, §8 |
| US a generar | US a generar en 06 (listado y detalle de incidentes; filtros; copia de mensajes; reversión de baneo con confirmación; registro de la reversión) |
| Tests previstos | Snapshot por estado (vacío, vacío por filtro, cargando, con datos, detalle simulado, detalle no accionable, confirmación, error de reversión, éxito); test de accesibilidad WCAG 2.2 AA con foco en el diálogo y el manejo de foco (referencia tentativa a 08) |
| Catálogo de diseño aplicado | design-rules-web-generico_v1.0.md; design-rules-blazor-mudblazor_v1.0.md |
| Configuración dirigida por esquema aplicada | N/A (superficie de revisión, sin parámetros de moderación) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Wireframe inicial de revisión de incidentes y desbaneo, con vista maestro-detalle, filtros, copia de mensajes accionados, distinción de modo real y simulación, diálogo de confirmación de reversión con aviso de no restauración, estados mínimos vacío/cargando/con datos/error más detalle simulado, no accionable y error de reversión; anclado en CU-06, CU-07 y CU-05. |
