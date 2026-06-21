# Glosario UX

**Proyecto:** discord-bots-admin
**Documento:** glosario-ux_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Alcance

Este glosario define la terminología propia de la sección 03 (experiencia, layout, estados y patrones de presentación). No redefine los términos del dominio de moderación, que viven en el glosario de la visión de producto (`00_contexto/vision-producto_v1.0.md` §9) y en el modelo conceptual de 02 (`02_especificacion_funcional/modelo-datos/modelo-conceptual_v1.0.md`). Cuando un término del dominio aparece en un wireframe, se usa con la misma semantica que en 02 y no se redefine aquí.

## 2. Términos del dominio reutilizados de 00 y 02 (no se redefinen)

Se referencian con su semántica original: servidor, regla (de contenido y de conducta), grupo de reglas, evento o política, acción, exención, incidente, mensaje accionado, canal afectado, canal de salida, modo simulación, borrado retroactivo, desbaneo, ráfaga distribuida (fan-out), token de bot, snowflake, antirrebote, prueba de configuración, estado de conexión, estado de activación. Para su definición, ver el glosario de 00 §9 y el modelo conceptual de 02.

## 3. Términos nuevos de UX

| Término | Definición | Notas de uso |
| --- | --- | --- |
| Pantalla | Unidad visual completa que ocupa el área de contenido para una tarea o un conjunto de tareas relacionadas. | En esta solución, cada wireframe documenta una pantalla o superficie. |
| Superficie | Sinónimo operativo de pantalla usado en la nomenclatura de los wireframes; una pantalla, un panel maestro-detalle o un wizard con estados propios. | Una superficie por archivo `wireframes-<superficie>`. |
| Vista | Estado renderizado concreto de una pantalla en un momento dado (por ejemplo, la vista de detalle de un incidente). | Una pantalla tiene varias vistas según su estado. |
| Shell de aplicación | Estructura persistente de la interfaz: barra lateral de navegación más área de contenido. | Patrón del catálogo; ausente en la pantalla de ingreso. |
| Barra lateral | Navegación vertical fija con identidad, ítems de navegación y cierre de sesión. | Colapsa a navegación superior o drawer en viewport angosto. |
| Modal | Diálogo que se superpone al contenido y bloquea la interacción con el resto hasta resolverse. | Usado en la confirmación de desbaneo y en la previsualización de la propuesta. |
| Diálogo de confirmación | Modal que pide confirmación explícita antes de una acción de consecuencia. | Atrapa el foco mientras está abierto; Cancelar no ejecuta. |
| Toast | Notificación breve y no bloqueante que confirma el resultado de una acción. | El verbo del toast coincide con el del botón que lo disparó. |
| Banner | Mensaje persistente inline que comunica un estado o un error recuperable dentro del contenido. | Lleva causa y acción de recuperación. |
| Banda de aviso | Banner contextual del panel de estado que alerta de servidores sin cobertura. | Visible solo cuando hay desconexión o token inválido. |
| Estado vacío | Representación de una superficie sin datos, con ilustración e invitación a la acción siguiente. | No es un adorno: orienta a la próxima acción. |
| Skeleton | Marcador de carga que reproduce la silueta del contenido mientras llega el dato. | Se usa por encima de un umbral de espera breve. |
| Badge / chip de estado | Etiqueta compacta que comunica un estado combinando color y texto. | Estado nunca solo por color: siempre con texto o ícono. |
| Stepper / wizard | Indicador y contenedor de un flujo de varios pasos con avance y retroceso. | Usado en el registro y prueba de servidor. |
| Campo dirigido por descriptor | Control de formulario cuya etiqueta, valor por defecto, límites, ayuda y ejemplos provienen de su descriptor, no de la pantalla. | Patrón de la extensión de configuración por esquema. |
| Ayuda contextual | Tarjeta desplegable junto a un campo con su leyenda y ejemplos, derivada del descriptor. | Se abre con el ícono de info; accesible por teclado. |
| Divulgación progresiva | Técnica que oculta las opciones avanzadas en un expander colapsado por defecto. | Acota las opciones simultáneas (ley de Hick). |
| Preset / receta | Conjunto coherente de valores listo para precargar en la configuración. | Aterriza en modo simulación; pasa por previsualización y confirmación. |
| Explicación en palabras | Bloque de prosa que describe la configuración actual o propuesta, generado por plantilla a partir de descriptores y valores. | No se escribe a mano; se regenera al cambiar un valor. |
| Indicador de modo simulación | Chip en la cabecera de la configuración que señala que los cambios se prueban sin aplicarse. | Estado de atención con texto explícito. |
| Previsualización (de la propuesta) | Vista previa de la explicación en palabras más el alcance afectado antes de aplicar una configuración. | Paso obligatorio antes de confirmar; la UI nunca aplica directo. |
| Alcance afectado | Resumen de qué elementos y comportamientos cambian al aplicar una propuesta de configuración. | Acompaña a la previsualización. |
| Ranura del asistente | Hueco de UI reservado y deshabilitado para el futuro asistente de IA. | No dispara nada hoy; anuncia su estado a tecnologías asistivas. |
| Acción primaria | Acción principal de una superficie, destacada por jerarquía visual y posición. | Una por contexto; nombra el verbo exacto. |
| Acción destructiva | Acción de consecuencia (eliminar, revertir) diferenciada y, cuando aplica, con confirmación. | Color de peligro más texto; nunca solo color. |
| Maestro-detalle | Disposición con una lista (maestro) y el detalle del elemento seleccionado. | Usada en la revisión de incidentes; pasa a dos pasos en móvil. |
| Rótulo accesible | Nombre programático de un control de solo ícono, expuesto a tecnologías asistivas. | Obligatorio en los botones de ícono de las filas. |
| Foco visible | Indicación visual del elemento que tiene el foco de teclado. | Anillo de al menos 2px que no depende solo del color. |

## 4. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Glosario UX inicial: términos de presentación de la sección 03, con referencia a los términos de dominio de 00 y 02 sin redefinirlos. |
