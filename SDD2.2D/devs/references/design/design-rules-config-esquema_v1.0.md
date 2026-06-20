# Reglas de diseño — Configuración dirigida por esquema

**Proyecto:** {{nombre-solucion}}
**Documento:** design-rules-config-esquema_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-20
**Autor:** {{equipo-o-rol}} (AG-03 UX/UI)
**Ámbito:** Capacidad transversal — superficies de configuración (agnóstico de framework)
**Hereda de:** `design-rules-web-generico_v1.0.md`
**Posición:** Insumo normativo de la categoría 03. Extensión por capacidad del catálogo `references/design/`. No es un artefacto operativo de `docs/`.

---

## 0. Propósito y alcance

Este documento codifica el lenguaje de diseño de las superficies de configuración: pantallas donde el usuario fija parámetros que cambian el comportamiento del sistema. Es una extensión por capacidad del catálogo, distinta de las especializaciones por stack: aplica de forma transversal a cualquier proyecto que tenga superficies de configuración, sea cual sea el framework, y no aplica a la UI que no configura nada.

Carga condicional: el subagente AG-03 lo suma al base solo cuando el proyecto declara superficies de configuración (Parte C del intake o casos de uso de 02 con configuración de parámetros). No reemplaza al documento base: hereda sus tokens, su tipografía, su espaciado, sus patrones de formulario y sus estados, y agrega encima los patrones propios de la configuración por esquema.

La capacidad queda preparada para enchufar asistencia de IA en el futuro sin rehacer el dominio. Ese enganche es forward-compatible: hoy se reserva la estructura (ver §7) y la IA se conecta después contra la misma frontera, sin tocar reglas, eventos ni acciones del dominio.

Fuera de alcance. Este documento codifica el lado UX/UI: los patrones, el contrato de descriptor que la UX consume, y la previsualización y confirmación de cambios. El motor que sostiene la capacidad (registro de descriptores como servicio, validación del lado del sistema, salidas estructuradas / tool calling de la IA, mecánica de plan-and-apply) es arquitectura técnica: vive en la categoría 05 y se ancla en el qué funcional de 02. Acá se lo referencia como cross-ref, no se lo implementa ni se describe su construcción.

Marco de referencia: el del documento base, más el principio de divulgación progresiva, la ley de Hick (acotar opciones simultáneas) y el patrón de human-in-the-loop para acciones que cambian estado.

---

## 1. Principio: el parámetro como dato descrito por esquema

Cada parámetro configurable se describe con un descriptor único, que es la fuente de verdad de ese parámetro. El valor por defecto, los límites, la leyenda explicativa y los ejemplos viven en el descriptor, nunca hardcodeados en la pantalla.

Consecuencia directa: la pantalla no inventa textos, defaults ni rangos por su cuenta. Los lee del descriptor y los presenta. Si el default de un parámetro cambia, cambia en el descriptor y la pantalla lo refleja sin edición. Esto es la aplicación, a la capa de configuración, del principio de consistencia (ley de Jakob) y de fuente única de tokens del documento base: acá la fuente única no es solo el color, es también el contrato de cada parámetro.

---

## 2. Contrato del descriptor de parámetro

El descriptor declara, como mínimo, los campos que la UX consume para renderizar, explicar y validar el parámetro. La UX no necesita más que esto; el resto (persistencia, identidad técnica del parámetro) es responsabilidad del motor (05).

| Campo del descriptor | Qué hace en la UI |
| --- | --- |
| `etiqueta` | Rótulo visible del campo (label), en sentence case. |
| `leyenda` | Explicación de una a dos líneas: qué hace el parámetro, en palabras del usuario. Alimenta la ayuda contextual. |
| `tipo` | Determina el control a renderizar (numérico, texto, selección, booleano, rango). |
| `unidad` | Sufijo o adorno de unidad del control y de la explicación (por ejemplo, segundos, cantidad, porcentaje). |
| `default` | Valor precargado del control al abrir la superficie. No se hardcodea en la pantalla. |
| `min` / `max` / `enum` | Límites o conjunto de valores admitidos. Gobiernan la validación inline y los topes del control. |
| `ejemplos` | Lista de pares valor → consecuencia ("con este valor, pasa esto"). Alimentan la ayuda contextual. |
| `visibleSi` | Condición de visibilidad: el campo aparece solo si se cumple (divulgación condicional dependiente de otro parámetro). |

Reglas de uso del contrato:
- Todo campo configurable de una superficie tiene su descriptor; no hay campos sin descriptor.
- La `leyenda` y los `ejemplos` se escriben una vez, en el descriptor, no por pantalla.
- El control que se renderiza se deriva de `tipo` + `unidad` + límites; no se elige a mano contradiciendo el descriptor.

---

## 3. Los cuatro consumidores del descriptor

Un mismo descriptor alimenta cuatro consumidores. Un cambio en el descriptor se refleja en los cuatro a la vez; esa es la ventaja de la fuente única.

| Consumidor | Qué toma del descriptor |
| --- | --- |
| Render del campo | `etiqueta`, `tipo`, `unidad`, `default`, límites, `visibleSi` para dibujar el control con su valor inicial. |
| Ayuda contextual | `leyenda` + `ejemplos` para armar la tarjeta de ayuda del campo. |
| Validación | `min` / `max` / `enum` para validar la entrada inline y antes de aplicar. |
| Contrato para IA (forward-compat) | El descriptor completo es el contrato que una IA usa para entender qué parámetro puede tocar, dentro de qué límites, y devolver una propuesta válida. |

El cuarto consumidor no se construye hoy (ver §7), pero el contrato se diseña desde el inicio para que sea legible por una IA, de modo que enchufarla no obligue a redefinir descriptores.

---

## 4. Patrones de componente

Heredan los tokens, la tipografía y el espaciado del documento base. Cada patrón declara anatomía, estados y comportamiento, y los wireframes lo referencian por nombre.

### 4.1 Campo configurable dirigido por descriptor
Anatomía: label visible (`type.caption`) tomado de `etiqueta`; control debajo (según `tipo` y `unidad`); hint de apoyo bajo el control con el default y los límites ("por defecto N; entre min y max"); ícono de info al lado del label que abre la ayuda contextual. Estados: normal, foco (anillo de marca de 2px), en error (borde `color.border.danger` + mensaje), deshabilitado por `visibleSi` no cumplido (oculto, no atenuado). Comportamiento: el valor inicial es `default`; la validación inline usa los límites; nada del contenido del campo se escribe a mano por pantalla.

### 4.2 Tarjeta de ayuda contextual
Estado `info`. Anatomía: título "¿qué es <parámetro>?" (de `etiqueta`), leyenda de una a dos líneas (de `leyenda`), y lista de ejemplos valor → consecuencia (de `ejemplos`). Disclosure: se abre con el ícono de info del campo (§4.1) y se cierra con la misma tecla o foco fuera. Color: usa el estado semántico `info` del base (texto + tint de fondo). Comportamiento: el contenido se deriva del descriptor; nunca se escribe ayuda a mano por campo. Accesible por teclado (ver §8).

### 4.3 Divulgación progresiva
Los parámetros comunes quedan visibles en la vista; los avanzados se ocultan en un expander ("Opciones avanzadas") colapsado por defecto. La pertenencia a común o avanzado es una propiedad del descriptor o de la agrupación, no una decisión visual por pantalla. Respeta la ley de Hick: menos opciones simultáneas, decisión más rápida. El expander declara su estado (colapsado/expandido) y es operable por teclado.

### 4.4 Presets / recetas
Un preset es una configuración completa lista para aplicar: un conjunto de valores coherentes para los descriptores de la superficie. Anatomía: lista o grupo de presets con nombre y una línea de propósito. Comportamiento: el usuario elige un preset, lo ajusta si quiere y lo aterriza en simulación (no se aplica directo). Un preset es una forma rápida de llenar una propuesta (§6); pasa por la misma previsualización y confirmación que cualquier cambio manual.

### 4.5 Explicación en lenguaje natural ("en palabras")
Bloque que describe en prosa la configuración actual o propuesta, armado por plantilla a partir de los descriptores y los valores ("en palabras: cuando el parámetro X supera N, el sistema hace Y"). Anatomía: bloque con barra de acento lateral, texto corrido legible. Comportamiento: se regenera al cambiar un valor; no se escribe a mano. Es la operación inversa del prompt futuro: hoy traduce valores → palabras para que el usuario entienda una configuración sin IA; mañana la IA hace palabras → valores. Sirve también como texto de previsualización en la confirmación (§6).

### 4.6 Indicador de modo simulación
Chip de estado `warning` (Atención) en la cabecera de la superficie, con texto explícito ("Modo simulación"). Señala que los cambios se están probando y todavía no se aplicaron. Comportamiento: visible mientras la superficie está en simulación; desaparece o cambia a confirmación al aplicar. El color nunca es el único canal: lleva siempre la etiqueta.

### 4.7 Ranura del asistente
Hueco de UI forward-compatible reservado para el futuro asistente de IA. Anatomía: contenedor con borde discontinuo, título del asistente y badge "próximamente"; estado deshabilitado. Comportamiento: no realiza ninguna acción hoy; existe para que, cuando se enchufe la IA, no haya que rediseñar el layout. La ranura deshabilitada se anuncia a tecnologías asistivas (ver §8). No ocupa un lugar central que compita con la configuración manual; es un afluente, no el cauce.

---

## 5. Estados, feedback y validación

Además de los estados del documento base, las superficies de configuración declaran:

| Estado | Condición | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Campo válido | Valor dentro de límites | Control normal | Hint de default/límites |
| Campo en error | Valor fuera de `min`/`max` o de `enum` | Borde `color.border.danger`, mensaje inline | Qué límite se violó y el rango admitido |
| Ayuda desplegada / colapsada | El usuario abrió o cerró la ayuda contextual | Tarjeta `info` visible u oculta | Leyenda + ejemplos del descriptor |
| Preset aplicado → simulación | El usuario eligió un preset | Valores precargados + chip "Modo simulación" | Qué preset se cargó; aún no aplicado |
| Propuesta en previsualización | Hay cambios sin aplicar | Explicación "en palabras" + alcance afectado | Resumen de qué cambia y a qué afecta |
| Ranura del asistente deshabilitada | La IA no está conectada | Contenedor con borde discontinuo + badge "próximamente" | Estado anunciado a lectores de pantalla |

La validación inline se deriva siempre del descriptor (`min`/`max`/`enum`); no se duplican reglas de validación en la pantalla. La validación de UI es de conveniencia: la validación que decide si una propuesta se aplica vive en el motor (05) contra los mismos descriptores.

---

## 6. La frontera `PropuestaDeConfiguracion` (lado UX)

Una `PropuestaDeConfiguracion` representa un conjunto de cambios sobre los parámetros, y se valida contra los mismos descriptores antes de aplicarse. El formulario es solo una de las formas de llenar una propuesta: un preset, una explicación editada o, en el futuro, una sugerencia de IA, llenan la misma frontera.

Reglas de UX de la frontera:
- Toda propuesta se previsualiza antes de aplicar: explicación "en palabras" (§4.5) más el alcance afectado (qué se ve impactado por el cambio).
- Toda propuesta se confirma explícitamente: plan-and-apply con human-in-the-loop. La UI propone, el humano confirma, el sistema valida.
- La UI nunca aplica directo. El modo simulación (§4.6) es la red de seguridad: el usuario prueba el efecto antes de comprometerlo.

Cross-ref. El motor que valida la propuesta, el registro de descriptores como servicio y la mecánica técnica de aplicación (incluidas las salidas estructuradas / tool calling cuando se sume la IA) son arquitectura técnica de la categoría 05. El qué funcional de cada parámetro y de la operación de configurar vive en la categoría 02. Este documento define el contrato de UX de la frontera; no su implementación.

---

## 7. Enganche de IA futura (forward-compat)

Para no rehacer la superficie cuando se sume la IA, hoy se dejan listas cuatro piezas. Tres son contrato y mecánica que ya rinden sin IA; la cuarta es un hueco de UI.

1. Registro de descriptores como fuente única. Cada parámetro tiene su descriptor; ese registro es lo que una IA leería para saber qué puede tocar.
2. Frontera `PropuestaDeConfiguracion` validable. Toda forma de cambiar la configuración (formulario, preset, IA) produce una propuesta que se valida igual.
3. Modo simulación. La red de seguridad ya existe y aplica a cualquier propuesta, venga de quien venga.
4. Ranura de UI del asistente. El hueco forward-compatible (§4.7) reserva el lugar visual del asistente.

Con esas cuatro piezas, el motor de IA se enchufa después contra la frontera: genera una `PropuestaDeConfiguracion`, que pasa por la misma previsualización, simulación y confirmación que una propuesta manual. La IA propone; no ejecuta. No se tocan reglas, eventos ni acciones del dominio para conectarla.

---

## 8. Accesibilidad de los patrones nuevos (WCAG 2.2 AA, piso)

- Disclosure por teclado: el ícono de info (§4.1/§4.2) y el expander de divulgación progresiva (§4.3) son operables con teclado (Enter/Espacio) y declaran `aria-expanded` segun estén abiertos o cerrados.
- Ayuda asociada al campo: la tarjeta de ayuda contextual se asocia a su control por `aria-describedby`, de modo que el lector de pantalla la anuncia con el campo.
- Validación anunciada: el mensaje de error inline se asocia al campo (`aria-describedby`) y se anuncia; indica el rango admitido, no solo "valor inválido".
- Ranura deshabilitada anunciada: el contenedor del asistente deshabilitado expone su estado (deshabilitado + "próximamente") a tecnologías asistivas, no solo de forma visual.
- Estados por más de un canal: el modo simulación y los estados de campo combinan color con texto e ícono (heredado del base, §2.1).
- Foco visible en todos los controles nuevos; respeto de `prefers-reduced-motion` en la apertura de ayuda y expanders.

---

## 9. Criterios de aceptación del diseño

Una superficie de configuración cumple esta extensión cuando: cada parámetro configurable tiene su descriptor y la pantalla no hardcodea defaults, límites, leyendas ni ejemplos; la ayuda contextual y la explicación "en palabras" se derivan del descriptor y no se escriben a mano por campo; los parámetros avanzados están en divulgación progresiva; los presets, cuando existen, aterrizan en simulación y no se aplican directo; toda propuesta se previsualiza y se confirma antes de aplicar (la UI nunca aplica directo); el modo simulación está declarado; la ranura del asistente queda reservada y deshabilitada; y los patrones nuevos cumplen accesibilidad AA (disclosure por teclado, `aria-expanded`, `aria-describedby`, estado de la ranura anunciado).

---

## 10. Anti-patrones

| Anti-patrón | Problema | Corrección |
| --- | --- | --- |
| Default hardcodeado en la pantalla | El valor por defecto se desincroniza del descriptor; dos fuentes de verdad | El default vive en el descriptor; la pantalla lo lee |
| Ayuda escrita a mano por campo | La explicación se desactualiza respecto del parámetro y duplica esfuerzo | Derivar la ayuda de `leyenda` + `ejemplos` del descriptor |
| Explicación "en palabras" escrita a mano | Se desfasa de los valores reales; no es la inversa del prompt | Generarla por plantilla a partir de descriptores + valores |
| Aplicar cambios sin previsualización ni simulación | El usuario no ve el efecto antes de comprometerlo; sin red de seguridad | Previsualizar (en palabras + alcance) y simular antes de confirmar |
| Dar a la IA capacidad de ejecutar en vez de proponer | Saca al humano del lazo; cambios sin control | La IA llena una `PropuestaDeConfiguracion`; el humano confirma, el sistema valida |

---

## 11. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Especialidad dueña | AG-03 UX/UI |
| Hereda de | `design-rules-web-generico_v1.0.md` |
| Mapeado por | especializaciones por stack (por ejemplo `design-rules-blazor-mudblazor_v1.0.md`) |
| Regla que lo invoca | `devs/rules/03_rules_ux_ui_dx.md` (cuando el proyecto tiene superficies de configuración) |
| Cross-ref técnico | categoría 05 (motor, registro de descriptores, validación, salidas estructuradas / tool calling, plan-and-apply) |
| Cross-ref funcional | categoría 02 (qué funcional de los parámetros y de la operación de configurar) |
| Marco teórico | `guides/marco-teorico-sdd2.2_v1.0.md`, cap. UX/UI/DX |
| Artefactos operativos que lo aplican | `experiencia-de-uso`, `wireframes-<superficie>` de las superficies de configuración del proyecto |

---

## 12. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial. Extensión por capacidad: configuración dirigida por esquema. Contrato del descriptor de parámetro y sus cuatro consumidores, patrones de componente (campo dirigido por descriptor, ayuda contextual, divulgación progresiva, presets, explicación en palabras, indicador de simulación, ranura del asistente), estados y validación, frontera `PropuestaDeConfiguracion` (lado UX), enganche de IA forward-compat, accesibilidad AA y anti-patrones. Agnóstico de framework, sin literales de dominio. | AG-03 UX/UI |
