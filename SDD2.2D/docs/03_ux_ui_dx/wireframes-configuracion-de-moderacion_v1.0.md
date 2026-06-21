# Wireframes — Configuración de moderación dirigida por descriptores

**Proyecto:** discord-bots-admin
**Documento:** wireframes-configuracion-de-moderacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Pantalla y propósito

Superficie de configuración de la moderación de un servidor. El administrador crea y ajusta reglas de contenido y de conducta, las agrupa, compone eventos con sus acciones, define exenciones y configura los parámetros (umbrales, ventanas, modos) apoyándose en el valor por defecto, la leyenda y los ejemplos de cada parámetro, sin conocimiento técnico profundo. Es la superficie de configuración del producto: cada campo se dirige por su descriptor (fuente única), todo cambio se previsualiza y se confirma, y todo evento nuevo nace en modo simulación.

## 2. Layout

Shell de aplicación del catálogo. El contenido se agrupa por módulo de configuración (Reglas, Grupos, Eventos, Acciones, Exenciones), cada uno con su barra de acento vertical, título, subtítulo y su ABM. La cabecera de la superficie lleva el indicador de modo simulación. Una franja inferior fija sostiene la explicación en palabras y la acción de previsualizar y aplicar. La ranura del asistente de IA queda reservada y deshabilitada en un lateral o al pie, sin competir con la configuración manual.

```
+-----------+-------------------------------------------------------+
| barra     | Configuracion - Servidor "Comunidad"  [Modo simulacion]|
| lateral   | Ajusta la moderacion. Los eventos nuevos simulan.     |
|           |                                                       |
|           | | Reglas                          [ Nueva regla ]    |
|           | +-------------------------------------------------+  |
|           | | Nombre        | Clase     | Criterio   | Acciones|  |
|           | | Rafaga distr. | Conducta  | canales>=N | edit del|  |
|           | | Enlaces       | Contenido | regex      | edit del|  |
|           | +-------------------------------------------------+  |
|           |                                                       |
|           | | Grupos                          [ Nuevo grupo ]    |
|           | | Eventos                         [ Nuevo evento ]   |
|           | | Acciones / Exenciones ...                          |
|           |                                                       |
|           | +-- En palabras -----------------------------------+  |
|           | | Cuando un usuario publica en 4 o mas canales      | |
|           | | distintos en 2 s, el evento "Corte de rafaga"     | |
|           | | banearia y borraria 1 dia de mensajes. (simulado) | |
|           | +---------------------------------------------------+  |
|           |               [ Previsualizar ]   [ Aplicar ]        |
|           |                                                       |
|           | +-- Asistente (proximamente, deshabilitado) --------+ |
|           | : Sugerira configuraciones que vos confirmas.      : |
|           | +...................................................+ |
+-----------+-------------------------------------------------------+
```

Formulario de edición de una regla de conducta (panel o modal), con campos dirigidos por descriptor:

```
+-------------------------------------------------------+
| Editar regla - Rafaga distribuida          [Conducta] |
|                                                       |
| Nombre                                                |
| [__________________________]                          |
|                                                       |
| Umbral de canales distintos            (i)            |
| [  4  ] canales                                       |
| por defecto 3; entre 2 y 10                            |
|                                                       |
| Ventana de detección                   (i)            |
| [  2  ] segundos                                      |
| por defecto 2; entre 1 y 30                            |
|                                                       |
| > Opciones avanzadas  (colapsado)                     |
|                                                       |
| [Estado: activo]            [ Cancelar ] [ Guardar ]  |
+-------------------------------------------------------+
```

Tarjeta de ayuda contextual (al abrir el ícono de info del campo "Umbral de canales distintos"):

```
+-- Que es el umbral de canales distintos? --------------+
| <leyenda del descriptor: en una sola linea o dos>      |
| Ejemplos (del descriptor):                             |
|  - con 3: corta antes, mas sensible                    |
|  - con 6: corta solo rafagas amplias, menos sensible   |
+--------------------------------------------------------+
```

Todos los valores mostrados arriba (3, 2, 10, los textos de leyenda y ejemplos) son ilustrativos de cómo la pantalla refleja un descriptor; provienen del descriptor del parámetro, no se hardcodean en la pantalla.

## 3. Componentes principales

| Componente | Patrón del catálogo | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- | --- |
| Agrupación por módulo | Agrupación por módulo (3.2) | Separar Reglas, Grupos, Eventos, Acciones, Exenciones | Título, subtítulo y ABM de cada módulo | Barra de acento por módulo; grilla responsiva |
| ABM de listado por módulo | ABM listado (4.3) | Listar y administrar cada tipo de elemento | Filas con nombre, atributos y acciones | Botón "Nuevo ..." por módulo; acciones de editar y eliminar por fila |
| Formulario de edición | ABM formulario (4.4) | Crear o editar un elemento | Campos del elemento | Pie con estado a la izquierda y Cancelar/Guardar a la derecha |
| Campo dirigido por descriptor | Campo configurable dirigido por descriptor (config-esquema 4.1) | Capturar un parámetro | Label de `etiqueta`; control según `tipo` y `unidad`; hint con default y límites | Valor inicial = `default`; validación inline con `min`/`max`/`enum`; nada se escribe a mano por pantalla |
| Ícono de info + ayuda contextual | Tarjeta de ayuda contextual (config-esquema 4.2) | Explicar el parámetro | Título de `etiqueta`, `leyenda` y `ejemplos` | Se abre con el ícono de info; se cierra con la misma tecla o foco fuera; estado info |
| Divulgación progresiva | Divulgación progresiva (config-esquema 4.3) | Ocultar las opciones avanzadas | Expander "Opciones avanzadas" colapsado | La pertenencia a común o avanzado viene del descriptor o de la agrupación; operable por teclado |
| Presets / recetas | Presets (config-esquema 4.4) | Precargar un conjunto coherente de valores | Lista de presets con nombre y propósito | Al elegir uno, precarga valores y entra en modo simulación; pasa por la misma previsualización |
| Selector de modo de coincidencia | Controles de formulario (4.6) | Definir el modo de un grupo | Todas, alguna, o al menos N | Al elegir "al menos N", habilita el campo N dirigido por descriptor (RN-15) |
| Campo prioridad y bandera continuar | Campo dirigido por descriptor + Toggle (4.7) | Configurar el orden y el encadenamiento del evento | Prioridad numérica; bandera continuar | La bandera continuar permite seguir evaluando políticas de menor prioridad (RN-04) |
| Orden de acciones | ABM formulario (4.4) | Ordenar las acciones del evento | Lista ordenable de acciones | El orden determina la secuencia de ejecución (RN-05) |
| Toggle de modo del evento | Toggle (4.7) + Indicador de simulación (config-esquema 4.6) | Conmutar simulación / ejecución real | Estado del modo | Evento nuevo nace en simulación; promover a real requiere previsualización y confirmación (RN-09) |
| Indicador de modo simulación | Indicador de modo simulación (config-esquema 4.6) | Señalar que los cambios se prueban sin aplicarse | Chip "Modo simulación" en la cabecera | Estado de atención; color más texto |
| Explicación en palabras | Explicación en lenguaje natural (config-esquema 4.5) | Describir en prosa la configuración actual o propuesta | Texto generado por plantilla a partir de descriptores y valores | Se regenera al cambiar un valor; no se escribe a mano |
| Botón Previsualizar | Botones (4.9) | Mostrar la propuesta antes de aplicar | Explicación en palabras más alcance afectado | Abre la previsualización de la PropuestaDeConfiguracion |
| Botón Aplicar | Botones (4.9) | Confirmar y aplicar la propuesta | Verbo de la acción | La UI nunca aplica directo: exige confirmación tras previsualizar |
| Ranura del asistente | Ranura del asistente (config-esquema 4.7) | Reservar el lugar del futuro asistente de IA | Contenedor con borde discontinuo y badge "próximamente" | Deshabilitado; no dispara nada hoy; su estado se anuncia a tecnologías asistivas |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Crear regla | El administrador activa "Nueva regla" | Abre el formulario con los campos del descriptor precargados con sus defaults | Servidor registrado; sesión activa |
| Ajustar un parámetro | El administrador cambia un campo dirigido por descriptor | Validación inline contra `min`/`max`/`enum`; la explicación en palabras se regenera | El campo tiene su descriptor |
| Rechazo por fuera de límite | Se ingresa un valor fuera de los límites del descriptor | Mensaje inline con el rango admitido y el valor por defecto ofrecido (CONFIG_VALOR_FUERA_DE_LIMITE) | — |
| Abrir ayuda contextual | El administrador activa el ícono de info de un campo | Se despliega la tarjeta con leyenda y ejemplos del descriptor | El campo tiene su descriptor |
| Expandir opciones avanzadas | El administrador activa el expander | Aparecen los parámetros avanzados; el expander anuncia su estado | — |
| Agrupar reglas | El administrador crea un grupo y elige su modo de coincidencia | El grupo queda con su modo; "al menos N" pide el valor N (RN-15) | Al menos una regla disponible |
| Rechazo de grupo sin reglas | Se guarda un grupo sin ninguna regla | Mensaje de que el grupo debe contener al menos una regla (CONFIG_GRUPO_SIN_REGLAS) | — |
| Componer evento | El administrador asocia grupos, fija prioridad y bandera continuar | El evento queda definido en orden de prioridad con su bandera (RN-04) | Al menos un grupo |
| Ordenar acciones | El administrador asocia acciones y las ordena | Las acciones quedan en el orden de ejecución (RN-05) | Evento existente |
| Aplicar preset | El administrador elige un preset | Precarga el conjunto de valores coherente y entra en modo simulación | Existen presets aplicables |
| Definir exención | El administrador agrega una exención por rol, usuario o canal | Valida el snowflake y persiste la exención (CU-15, RN-07, RN-08) | Servidor registrado |
| Rechazo de exención inválida | El identificador no es un snowflake | Mensaje con el formato esperado (EXENCION_IDENTIFICADOR_INVALIDO) | — |
| Configurar antirrebote | El administrador ajusta la ventana de antirrebote por usuario | Campo dirigido por descriptor; fuera de límites aplica el default (CU-16, ANTIRREBOTE_VENTANA_INVALIDA) | — |
| Configurar borrado retroactivo | El administrador ajusta la ventana de borrado de una acción | Campo dirigido por descriptor acotado al tope de plataforma (RN-02) | Acción de banear con borrado |
| Previsualizar propuesta | El administrador activa Previsualizar | Muestra la explicación en palabras y el alcance afectado antes de aplicar | Hay cambios sin aplicar |
| Aplicar propuesta | El administrador confirma en la previsualización | El sistema valida y persiste; la UI nunca aplica directo | Propuesta previsualizada y confirmada |
| Promover a ejecución real | El administrador conmuta el modo del evento y confirma | El evento pasa de simulación a real tras previsualizar (RN-09, CU-14 5.A) | El evento estaba en simulación |
| Eliminar elemento referenciado | Se intenta eliminar un elemento usado por otro | Bloqueo con la lista de referencias a resolver (CONFIG_REFERENCIA_REQUERIDA) | — |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | El servidor no tiene reglas, grupos ni eventos | Estado vacío por módulo con ilustración SVG, texto orientativo y botón "Nuevo ..." |
| Cargando | Carga de descriptores y de la configuración del servidor | Skeleton en las grillas; los campos esperan sus descriptores antes de renderizar defaults |
| Con datos | Hay configuración definida | Grillas con reglas, grupos, eventos, acciones y exenciones; explicación en palabras al pie |
| Campo válido | Valor dentro de los límites del descriptor | Control normal con hint de default y límites |
| Campo en error | Valor fuera de `min`/`max` o de `enum` (CONFIG_VALOR_FUERA_DE_LIMITE) | Borde de peligro y mensaje inline con el límite violado y el rango admitido |
| Ayuda desplegada / colapsada | El administrador abrió o cerró la ayuda contextual | Tarjeta info visible u oculta con leyenda y ejemplos del descriptor |
| Preset aplicado → simulación | El administrador eligió un preset | Valores precargados y chip "Modo simulación"; aún no aplicado |
| Propuesta en previsualización | Hay cambios sin aplicar | Explicación en palabras más el alcance afectado; resumen de qué cambia y a qué afecta |
| Modo simulación | El evento está en simulación (estado de seguridad, no error) | Chip "Modo simulación" en la cabecera; los cambios se prueban sin ejecutarse |
| Error de conflicto | Grupo sin reglas o eliminación con referencias (CONFIG_GRUPO_SIN_REGLAS, CONFIG_REFERENCIA_REQUERIDA) | Mensaje que explica el conflicto y la salida |
| Ranura del asistente deshabilitada | La IA no está conectada | Contenedor con borde discontinuo y badge "próximamente"; estado anunciado a lectores de pantalla |
| Éxito | Propuesta aplicada tras confirmación | Confirmación sutil; la configuración persistida queda reflejada |

## 6. Versión móvil o responsive

Las grillas por módulo reflujen a tarjetas apiladas bajo el punto de quiebre del catálogo. El formulario de edición ocupa el ancho disponible; la ayuda contextual se presenta como hoja inferior en lugar de popover lateral. La franja de explicación en palabras y de acciones de previsualizar y aplicar se mantiene accesible al pie. La ranura del asistente pasa al final del flujo. Contenido legible sin scroll horizontal a 320px.

## 7. Notas de implementación

- Configuración dirigida por esquema (obligatorio): cada parámetro toma de su descriptor la etiqueta, la leyenda, el default, los límites y los ejemplos; la pantalla no hardcodea ninguno. La validación inline se deriva del descriptor; la validación que decide si una propuesta se aplica vive en el motor de 05 contra los mismos descriptores. La explicación en palabras se genera por plantilla a partir de descriptores y valores, nunca a mano.
- Frontera PropuestaDeConfiguracion: toda forma de cambiar la configuración (formulario, preset y, en el futuro, sugerencia de IA) llena la misma propuesta, que se previsualiza (explicación en palabras más alcance afectado) y se confirma antes de aplicar. La UI nunca aplica directo. El modo simulación es la red de seguridad.
- Forward-compat de IA: la ranura del asistente se reserva deshabilitada; la IA, cuando se enchufe, llenará una PropuestaDeConfiguracion que pasa por la misma previsualización, simulación y confirmación. La IA propone, no ejecuta. No se construye el panel del asistente en esta versión.
- Accesibilidad: disclosure por teclado en el ícono de info y en el expander de opciones avanzadas, con anuncio de expandido o colapsado; la ayuda contextual se asocia al campo para que el lector de pantalla la anuncie con el control; el mensaje de error inline se asocia al campo e indica el rango admitido; el modo simulación y los estados de campo combinan color con texto e ícono; la ranura deshabilitada anuncia su estado; foco visible y respeto de la preferencia de movimiento reducido al abrir ayuda y expanders.
- Anidamiento booleano limitado a dos niveles: reglas dentro de un grupo y combinación de grupos dentro de un evento (RN-15); la UI no ofrece niveles adicionales.
- Internacionalización: leyendas, ejemplos y explicación en palabras toleran expansión; unidades (segundos, canales, días) se muestran junto al control según el descriptor.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Administrador del sistema, operador único (00) |
| CU origen | CU-11 (administrar reglas, grupos, eventos, acciones y parámetros con ayuda contextual); CU-14 (modo simulación); CU-15 (exenciones); CU-16 (antirrebote por usuario) |
| Reglas de negocio relevantes | RN-02 (tope del borrado retroactivo); RN-04 (evaluación por prioridad y bandera continuar); RN-05 (orden de ejecución de acciones); RN-06 (antirrebote por usuario); RN-07 (descarte previo de exentos); RN-08 (identidad de snowflakes); RN-09 (la simulación no ejecuta); RN-10 (configuración dirigida por descriptor); RN-12 (autorización por rol administrador único); RN-15 (composición de un grupo de reglas) |
| Marco experiencia-de-uso aplicado | experiencia-de-uso_v1.0.md §3.3, §4, §5, §8 |
| US a generar | US a generar en 06 (alta y edición de reglas, grupos, eventos y acciones; ayuda contextual por parámetro; modos de coincidencia; presets; modo simulación y promoción; previsualización y aplicación; exenciones; ventana de antirrebote; ventana de borrado) |
| Tests previstos | Snapshot por estado (vacío, cargando, con datos, campo en error, preset aplicado, previsualización, modo simulación, ranura deshabilitada); tests de accesibilidad WCAG 2.2 AA con foco en disclosure por teclado y ayuda asociada al campo (referencia tentativa a 08) |
| Catálogo de diseño aplicado | design-rules-web-generico_v1.0.md; design-rules-blazor-mudblazor_v1.0.md; design-rules-config-esquema_v1.0.md |
| Configuración dirigida por esquema aplicada (descriptores, presets, modo simulación, ranura del asistente) | Sí |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Wireframe inicial de la configuración de moderación dirigida por descriptores, con campos colgados del descriptor, ayuda contextual, divulgación progresiva, presets, explicación en palabras, indicador de modo simulación, previsualización y confirmación de la PropuestaDeConfiguracion y ranura del asistente reservada y deshabilitada; estados mínimos vacío/cargando/con datos/error más los estados de configuración; anclado en CU-11, CU-14, CU-15 y CU-16. |
