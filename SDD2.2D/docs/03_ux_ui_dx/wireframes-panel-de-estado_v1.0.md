# Wireframes — Panel de estado

**Proyecto:** discord-bots-admin
**Documento:** wireframes-panel-de-estado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Pantalla y propósito

Pantalla de inicio del panel, primera vista tras autenticarse. Muestra los servidores registrados con su estado de activación y su estado de conexión al canal de eventos, para que el administrador confirme de un vistazo que la moderación está activa y detecte de inmediato una pérdida de protección. Es el centro de monitoreo y el punto de partida hacia el registro de un servidor, su configuración y la revisión de incidentes.

## 2. Layout

Shell de aplicación del catálogo: barra lateral fija de chrome oscuro a la izquierda con la identidad arriba, navegación al medio y cierre de sesión diferenciado al pie; área de contenido a la derecha sobre el lienzo de página.

```
+-----------+-------------------------------------------------------+
| [logo]    |  Estado de los servidores            [ Registrar    ] |
|           |  Resumen de la cobertura de moderación   servidor     |
| Inicio  * |                                                       |
| Servidores|  +-----------+-----------+-----------+                |
| Incidentes|  | Activos N | Conect. N | Desconec. N|  resumen      |
| Config.   |  +-----------+-----------+-----------+                |
|           |                                                       |
|           |  +-------------------------------------------------+  |
|           |  | Servidor        | Activación | Conexión | Accion|  |
|           |  |-----------------+------------+----------+-------|  |
|           |  | [ic] Comunidad  | [Activo]   |[Conectado]| > ... |  |
|           |  | [ic] Pruebas    | [Inactivo] |[--]       | > ... |  |
|           |  | [ic] Soporte    | [Activo]   |[Desconec.]| > ... |  |
|           |  +-------------------------------------------------+  |
|           |                                                       |
| -----     |  [ banda de aviso: 1 servidor desconectado ]          |
| Salir     |                                                       |
+-----------+-------------------------------------------------------+
```

La barra de resumen ofrece los conteos de servidores activos, conectados y desconectados. La grilla lista un servidor por fila. Una banda de aviso aparece sobre o bajo la grilla solo cuando hay al menos un servidor desconectado o con token inválido.

## 3. Componentes principales

| Componente | Patrón del catálogo | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- | --- |
| Navegación lateral | Navegación lateral (4.1) | Moverse entre Inicio, Servidores, Incidentes, Configuración y Salir | Ítems con ícono y etiqueta; ítem activo resaltado | Ítem activo con fondo de marca; Salir separado por hairline en color de atención |
| Encabezado de sección | ABM listado (4.3) | Titular la vista y ofrecer la acción primaria | Título, subtítulo y botón "Registrar servidor" | El botón primario abre el registro de servidor |
| Barra de resumen | Badge de estado (4.8) | Resumir la cobertura | Conteo de activos, conectados y desconectados | Lectura rápida; cada conteo con etiqueta textual |
| Grilla de servidores | ABM listado (4.3) | Listar los servidores registrados | Por fila: nombre o identificador, estado de activación, estado de conexión, acciones | Fila seleccionable; acciones por fila como botones de ícono con rótulo accesible |
| Badge de activación | Badge de estado (4.8) | Indicar activo o inactivo | Texto "Activo" o "Inactivo" con su tint | Estado por color más texto, nunca solo color |
| Badge de conexión | Badge de estado (4.8) | Indicar conectado, desconectado o token inválido | Texto explícito con su tint (éxito, error, atención) | Se refresca dentro del tiempo objetivo de refresco |
| Banda de aviso | Estados y feedback (5) | Alertar de servidores sin cobertura | Cantidad de servidores desconectados y enlace a su detalle | Visible solo cuando hay desconexión; accionable |
| Acciones por fila | Botones (4.9) | Abrir configuración, probar o ver detalle del servidor | Íconos de configurar, probar y ver | Cada botón con rótulo accesible |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Registrar servidor | El administrador activa el botón primario | Abre la superficie de registro de servidor (CU-10) | Sesión activa |
| Abrir detalle de servidor | El administrador selecciona una fila | Despliega el detalle del servidor con su estado, su configuración y la prueba | Existe el servidor |
| Configurar servidor | El administrador activa la acción de configurar de la fila | Navega a la configuración de moderación de ese servidor (CU-11) | El servidor está registrado |
| Probar configuración | El administrador activa la acción de probar de la fila | Abre la prueba de configuración del servidor (CU-12) | El servidor está registrado |
| Re-validar por token inválido | El estado de conexión indica token inválido | Ofrece re-validar la credencial mediante la prueba de configuración (CU-12) | El servidor figura como desconectado por token inválido |
| Observar reconexión | El sistema detecta una caída y reconecta automáticamente | El badge de conexión pasa a desconectado y vuelve a conectado al restablecerse, dentro del tiempo de refresco | El servidor estaba activo y conectado |
| Cerrar sesión | El administrador activa Salir | El sistema invalida la sesión y vuelve al ingreso (CU-09 5.A) | Sesión activa |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | No hay servidores registrados | Bloque centrado con ilustración SVG, texto orientativo y botón "Registrar servidor" |
| Cargando | Consulta del estado de los servidores en curso | Skeleton de filas en la grilla; barra de resumen en placeholder |
| Con datos | Hay servidores registrados | Grilla con cada servidor y sus badges de activación y conexión; barra de resumen con conteos |
| Desconectado | Un servidor activo perdió la conexión al canal de eventos (CU-13) | Badge de conexión en estado de error con texto "Desconectado"; banda de aviso visible |
| Token inválido | La credencial se revocó o venció durante la operación (CONEXION_TOKEN_INVALIDO) | Badge de conexión con texto "Token inválido" y acción de re-validar |
| Error | Falla al obtener el estado de los servidores | Banner con la causa y acción de reintentar; se conserva la última lista conocida |
| Estado no actualizado | El estado no logró refrescarse dentro del tiempo objetivo (CONEXION_ESTADO_NO_ACTUALIZA) | Marca de estado posiblemente desactualizado con hora del último refresco; no se presenta como vigente |
| Éxito | Un servidor recién activado tras superar la prueba | Confirmación sutil; el servidor aparece como activo y conectado |

## 6. Versión móvil o responsive

Por debajo del punto de quiebre principal del catálogo, la barra lateral colapsa a navegación superior o drawer. La grilla de servidores reflujen a tarjetas apiladas: cada servidor como una tarjeta con nombre, badges de activación y conexión, y un menú de acciones. La barra de resumen apila sus conteos. Contenido legible sin scroll horizontal a 320px.

## 7. Notas de implementación

- Accesibilidad: la grilla con encabezados de columna semánticos; cada badge combina color y texto; las acciones de fila son botones de solo ícono con rótulo accesible; foco visible y navegación por teclado completa en la grilla; la banda de aviso se anuncia como región de estado.
- El estado de conexión nunca se comunica solo por color: el daltónico distingue por el texto del badge.
- Performance percibida: skeletons mientras carga la lista; el refresco del estado de conexión ocurre dentro del tiempo objetivo y, si no lo logra, se rotula la antigüedad del dato en lugar de mostrarlo como vigente.
- La pantalla no usa almacenamiento de navegador improvisado; el estado de conexión proviene del servicio.
- Internacionalización: los textos de los badges (Activo, Inactivo, Conectado, Desconectado, Token inválido) toleran expansión; fechas del último refresco en formato local.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Administrador del sistema, operador único (00) |
| CU origen | CU-13 (reconectar y mostrar el estado de conexión de cada servidor) |
| Reglas de negocio relevantes | RN-14 (cifrado del token en reposo); RN-16 (activación condicionada a la prueba) |
| Marco experiencia-de-uso aplicado | experiencia-de-uso_v1.0.md §3.5, §4, §7 |
| US a generar | US a generar en 06 (estado de conexión por servidor; reconexión automática; aviso de token invalidado; resumen de cobertura) |
| Tests previstos | Snapshot por estado (vacío, cargando, con datos, desconectado, token inválido, error); test de accesibilidad WCAG 2.2 AA (referencia tentativa a 08) |
| Catálogo de diseño aplicado | design-rules-web-generico_v1.0.md; design-rules-blazor-mudblazor_v1.0.md |
| Configuración dirigida por esquema aplicada | N/A (superficie de monitoreo, sin parámetros de moderación) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Wireframe inicial del panel de estado con shell de aplicación, grilla de servidores con badges de activación y conexión, banda de aviso de desconexión, estados mínimos vacío/cargando/con datos/error más desconectado, token inválido y estado no actualizado, anclado en CU-13. |
