# Reglas de diseño — Web genérico

**Proyecto:** {{nombre-solucion}}
**Documento:** design-rules-web-generico_v1.0.md
**Versión:** 1.1
**Estado:** Vigente
**Fecha:** 2026-06-19
**Autor:** {{equipo-o-rol}} (AG-03 UX/UI)
**Ámbito:** Web genérico (agnóstico de framework)
**Posición:** Insumo normativo de la categoría 03. Base del catálogo `references/design/`. No es un artefacto operativo de `docs/`.

---

## 0. Propósito y alcance

Este documento codifica el lenguaje de diseño que el subagente AG-03 aplica al producir los artefactos operativos de la categoría 03 (`experiencia-de-uso`, `wireframes-<superficie>`, `representacion-<concepto>`) de cualquier proyecto con interfaz web. Define tokens, patrones de componente, estados, iconografía vectorial, accesibilidad y criterios de calidad, de forma agnóstica del framework de presentación.

Es la pieza base del catálogo de diseño. Los documentos por stack (`design-rules-blazor-mudblazor`, y los futuros de HTML puro, MAUI y Blazor en MAUI) heredan estas reglas y solo mapean cada token y cada patrón a su tecnología concreta. Si una regla de este documento entra en conflicto con la especialización, manda la regla base salvo limitación técnica explícita y justificada en el documento hijo.

Alcance: layout, color, tipografía, espaciado, iconografía SVG, patrones de componente, estados, feedback, accesibilidad, responsive y movimiento. Fuera de alcance: la arquitectura de la capa de presentación (vive en 05) y el qué funcional (vive en 02).

Marco de referencia: heurísticas de Nielsen, reglas de Shneiderman, leyes UX (Hick, Fitts, Miller, Jakob), WCAG 2.2 nivel AA como piso, ISO 9241-210 para el proceso centrado en personas.

---

## 1. Principios rectores

1. Claridad antes que densidad. Cada elemento hace exactamente un trabajo: una etiqueta etiqueta, un ejemplo demuestra, un control dice qué hace al usarse. Se prefiere el espacio en blanco a la compresión.
2. Jerarquía visual explícita. El usuario debe distinguir en menos de un segundo qué es título, qué es contenido, qué es acción primaria y qué es secundaria, por tamaño, peso, color y posición, no por adorno.
3. Consistencia interna (ley de Jakob). Un mismo concepto se ve y se comporta igual en toda la solución. Los tokens y los patrones de este catálogo son la fuente única; está prohibido definir tokens visuales ad hoc por pantalla.
4. Estado siempre visible. Toda superficie declara y muestra sus estados (vacío, cargando, con datos, error, éxito, sin permiso). El flujo feliz no alcanza.
5. Accesibilidad como piso, no como extra. WCAG 2.2 AA es requisito de aceptación, no una mejora opcional.
6. Restricción del color y del movimiento. El color de marca señala acción y jerarquía, no decora. La animación sirve a la comprensión (transición de estado, orientación espacial), nunca al lucimiento.
7. Vector primero. Iconos e ilustraciones son SVG por defecto (ver §6). El raster es la excepción justificada.
8. Configuración dirigida por esquema. En superficies de configuración, cada parámetro se describe con un descriptor único, que es su fuente de verdad: el default, los límites, la leyenda y los ejemplos viven en el descriptor, no hardcodeados en la pantalla. El detalle de patrones, estados y la frontera de propuesta vive en la extensión por capacidad `design-rules-config-esquema`, que se carga solo cuando el proyecto tiene superficies de configuración.

---

## 2. Sistema de tokens

Los valores visuales se expresan como tokens semánticos, nunca como literales sueltos en la superficie. El catálogo define los nombres lógicos; cada documento por stack los materializa en su mecanismo (variables CSS, paleta del theme, recursos de la plataforma).

### 2.1 Color

Se trabaja con tres familias: superficies, texto y bordes, más una rampa de marca y un set de estados semánticos.

| Token semántico | Rol | Valor de referencia (claro) |
| --- | --- | --- |
| `color.background.primary` | Superficie de tarjetas y contenido elevado | `#FFFFFF` |
| `color.background.secondary` | Superficie de controles inertes / chips neutros | `#F1F1EF` |
| `color.background.tertiary` | Fondo de página / lienzo | `#F7F6F4` |
| `color.text.primary` | Texto principal / títulos | `#1A1A18` |
| `color.text.secondary` | Texto de apoyo / labels | `#5C5C57` |
| `color.text.tertiary` | Texto terciario / placeholders / metadatos | `#8A8A82` |
| `color.text.danger` | Texto de error / acción destructiva | `#A03030` |
| `color.border.secondary` | Borde de controles interactivos | `#D9D9D4` |
| `color.border.tertiary` | Borde sutil de tarjetas y separadores (0.5px) | `#E6E6E1` |
| `color.border.danger` | Borde de control en error o acción destructiva | `#E0B4B4` |

Rampa de marca y acentos por módulo. La marca tiene un primario y un oscuro de chrome; cada módulo funcional puede llevar un acento propio para codificar pertenencia (no estética). Valores de referencia tomados del sistema de ejemplo:

| Token | Rol | Valor |
| --- | --- | --- |
| `color.brand.primary` | Acción primaria, foco, selección | `#0F6E56` |
| `color.brand.primary.dark` | Chrome / barra lateral | `#04342C` |
| `color.brand.primary.tint` | Fondo suave de íconos y badges de marca | `#E1F5EE` |
| `color.accent.module-a` | Acento módulo (p. ej. mensajería) | `#0F6E56` |
| `color.accent.module-b` | Acento módulo (p. ej. tareas) | `#534AB7` |
| `color.accent.module-c` | Acento módulo (p. ej. equipo) | `#854F0B` |
| `color.accent.module-d` | Acento módulo (p. ej. integraciones) | `#185FA5` |

Estados semánticos (texto + tint de fondo, siempre en par):

| Estado | Texto | Fondo suave |
| --- | --- | --- |
| Éxito / activo | `#0F6E56` | `#E1F5EE` |
| Atención | `#854F0B` | `#FAEEDA` |
| Error / inactivo destructivo | `#993556` / `#A03030` | `#FBEAF0` |
| Informativo / ayuda contextual | `#185FA5` | `#E7F0F9` |
| Neutro / inactivo | `color.text.secondary` | `color.background.secondary` |

El estado informativo se expresa con los tokens `color.text.info` (`#185FA5`, reusando el hue de `color.accent.module-d`), `color.background.info` (`#E7F0F9`) y `color.border.info` (`#BBD4EA`). Es un token transversal: lo consumen la tarjeta de ayuda contextual de la extensión `design-rules-config-esquema` y cualquier otro uso informativo en cualquier superficie. El estado `warning` corresponde al estado "Atención" ya existente; si una especialización agrega aliases `color.*.warning`, deben apuntar a los valores de Atención.

Regla de contraste: todo texto cumple 4.5:1 sobre su fondo (3:1 para texto grande ≥ 24px o ≥ 18.66px bold). El color nunca es el único portador de significado: un estado se acompaña siempre de etiqueta textual o ícono.

### 2.2 Tipografía

Una familia sans humanista para todo el producto (`font.sans`), con una escala acotada y pesos intencionales. Se evita el uso de más de tres pesos.

| Token | Tamaño | Peso | Uso |
| --- | --- | --- | --- |
| `type.title` | 17px | 500 | Título de sección o de pantalla |
| `type.body-strong` | 14px | 500 | Título de tarjeta, etiqueta de fila destacada |
| `type.body` | 13px | 400 | Texto de interfaz, ítems de navegación, celdas |
| `type.caption` | 12px | 400 | Labels de formulario, descripciones |
| `type.meta` | 11px | 500 | Metadatos, badges, encabezados de tabla |

Interlineado 1.4–1.5 en bloques de texto; títulos en 1.2. Sentence case en toda la interfaz (no Title Case ni mayúsculas sostenidas). Números tabulares para columnas numéricas.

### 2.3 Espaciado, radios y bordes

Escala de espaciado base 4 (4, 8, 12, 14, 16, 18, 20, 22, 28). El padding interno de tarjeta de referencia es 16px; el de contenedor de página, 18–20px.

| Token | Valor | Uso |
| --- | --- | --- |
| `radius.md` | 8–10px | Botones, inputs, controles |
| `radius.lg` | 12–14px | Tarjetas, contenedores, paneles |
| `radius.pill` | 20px | Badges, chips, toggles, botón "volver" |
| `radius.icon` | 11–14px | Contenedor cuadrado de ícono |

Bordes hairline de 0.5px con `color.border.tertiary` para separar superficies de bajo contraste; 1px con `color.border.secondary` para controles interactivos. Sombra mínima o nula: la elevación se comunica con borde y superficie, no con drop-shadows pesadas.

---

## 3. Layout y composición

### 3.1 Estructura de aplicación (shell)

Patrón de shell con barra lateral fija más área de contenido. La barra lateral usa el chrome oscuro de marca, ancho fijo (~168px), con identidad arriba, navegación al medio (ítem activo con fondo `color.brand.primary`, inactivos a opacidad reducida), separador hairline y acción de cierre de sesión diferenciada al pie. El área de contenido usa el lienzo `color.background.tertiary`.

### 3.2 Agrupación por módulo

El contenido de configuración se agrupa en secciones, una por módulo, cada una con una barra de acento vertical (4px, color del módulo), título, subtítulo descriptivo y una grilla de tarjetas responsiva (`repeat(auto-fit, minmax(190px, 1fr))`, gap 14px). Cada tarjeta lleva un contenedor de ícono cuadrado con tint del módulo, título y una descripción de una a dos líneas escrita desde el lado del usuario.

### 3.3 Densidad y ritmo

Separación entre secciones de 28px; entre tarjetas de 14px. El ritmo vertical es regular: el ojo debe poder escanear de arriba a abajo sin saltos arbitrarios. Se respeta la ley de Miller: no más de 5–7 ítems de primer nivel por agrupación antes de subdividir.

---

## 4. Catálogo de patrones de componente

Cada patrón declara su anatomía, sus estados y su comportamiento. Los wireframes de cada proyecto referencian estos patrones por nombre en lugar de redibujarlos.

### 4.1 Navegación lateral
Ítems con ícono + etiqueta, área táctil mínima 40px de alto. Activo: fondo de marca, texto `#FFF`. Inactivo: texto a 72% de opacidad. Sección destructiva (cerrar sesión) separada por hairline y en color de atención.

### 4.2 Tarjeta de acceso (entry card)
Ícono en contenedor con tint, título `type.body-strong`, descripción `type.caption` en `color.text.tertiary`. Toda la tarjeta es el área clicleable; foco visible en el contenedor.

### 4.3 ABM — grilla de listado
Encabezado de columnas en `type.meta`, fondo `color.background.tertiary`, hairline inferior. Filas con avatar de iniciales (círculo con tint y color de rol), nombre `type.body-strong`, columnas de datos en `type.caption`, badge de estado y celda de acciones alineada a la derecha. Fila seleccionada con tint sutil de marca (`#F4F9F7`). Acciones por fila como icon-buttons con `aria-label` (editar en color de marca, eliminar en color de peligro con borde `color.border.danger`).
Encabezado del ABM: título + subtítulo a la izquierda, botón primario "Nuevo …" a la derecha. Barra de filtros debajo: campo de búsqueda con ícono interior y selects de filtro.

### 4.4 ABM — formulario de edición
Grilla de campos responsiva (`minmax(200px, 1fr)`). Cada campo: label `type.caption` arriba, control debajo, ancho completo de su celda. Pie del formulario con toggle de estado a la izquierda y par de botones a la derecha (secundario "Cancelar", primario "Guardar" con ícono). El verbo del botón nombra la acción exacta.

### 4.5 Wizard / stepper
Indicador de pasos horizontal: círculo numerado por paso, conector entre pasos, etiqueta debajo. Estados del paso: pendiente (superficie secundaria), actual (marca, número), completado (marca, ícono de check), con la línea anterior pintada en marca. Panel del paso activo abajo, navegación al pie (Anterior deshabilitado con opacidad 0.4 y sin eventos en el primer paso; Siguiente vira a la acción de confirmación en el último paso) y contador "Paso X de N". Paso final de revisión con resumen en filas clave/valor y toggle de activación.

### 4.6 Controles de formulario
Inputs y selects con borde `color.border.secondary`, radio `radius.md`, alto cómodo, foco con anillo de marca de 2px y desplazamiento. Placeholder en `color.text.tertiary`, nunca como sustituto del label.

### 4.7 Toggle (interruptor)
Riel `radius.pill`, 36×20px, perilla circular blanca de 16px. Encendido: riel en marca, perilla a la derecha. Acompañado siempre de etiqueta textual clicleable.

### 4.8 Badge / chip de estado
`radius.pill`, `type.meta`, par texto+tint del estado correspondiente (§2.1). Texto explícito ("Activo", "Inactivo"), nunca solo color.

### 4.9 Botones
Primario: fondo de marca, texto blanco, ícono opcional a la izquierda. Secundario: superficie primaria, borde `color.border.secondary`, texto `color.text.secondary`. Destructivo: borde/texto de peligro. Pill auxiliar (p. ej. "Volver a inicio"): `radius.pill`, superficie primaria, ícono de flecha.

### 4.10 Búsqueda y filtros
Campo con ícono de lupa interior (posición absoluta, padding-left para no solapar el texto), más selects de filtro de ancho acotado. La búsqueda filtra; el resultado vacío muestra estado vacío con acción siguiente.

---

## 5. Estados, feedback y contenido

Toda superficie clave declara su mapa de estados. Tabla canónica:

| Estado | Condición | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | No hay datos aún | Ilustración SVG o ícono neutro centrado | Texto orientativo + acción siguiente |
| Cargando | Operación asíncrona en curso | Skeleton (listas/tarjetas) o spinner (acción puntual) | Qué se está cargando |
| Con datos | Hay contenido | Render normal | — |
| Error | Falla recuperable | Banner o inline con color de peligro | Qué pasó, por qué y qué hacer |
| Éxito | Acción completada | Confirmación sutil (toast/inline) | Confirmación + próxima acción |
| Sin permiso | El rol no habilita la acción | Control deshabilitado u oculto | Motivo si el control es visible |

Reglas de redacción (frontend writing): voz activa; el botón "Publicar" produce un toast "Publicado"; los errores no se disculpan ni son vagos; la pantalla vacía es una invitación a actuar, no un adorno. Nombrar las cosas por lo que la persona controla, no por cómo está construido el sistema.

Performance percibida: skeletons por encima de ~400ms de espera; optimistic UI cuando la operación es reversible; transición de estado en 150–200ms con `ease`.

---

## 6. Iconografía e imágenes (SVG primero)

Requisito normativo: iconos e ilustraciones son SVG por defecto en toda superficie web. El raster (`png`/`jpg`/`webp`) solo se admite para fotografía real, con justificación en el artefacto.

### 6.1 Iconos
- Set vectorial único por solución (p. ej. Tabler Icons o Material Symbols), trazo coherente (1.5–2px), grilla de 24px. No mezclar familias de íconos en una misma superficie.
- Color por `currentColor`: el SVG hereda el color del texto contenedor, de modo que el token de color manda y se evita duplicar paletas.
- Tamaños por rol: 24px en identidad y navegación, 20px en íconos de tarjeta, 16–17px inline y en botones, 15px en icon-buttons de fila.
- Accesibilidad: ícono puramente decorativo lleva `aria-hidden="true"`; ícono que es la única etiqueta de un control lleva `aria-label` en el control (no en el `<svg>`). Un `<svg>` con significado propio lleva `role="img"` y `<title>`.
- Entrega: preferir SVG inline (permite `currentColor` y animación) o sprite `<symbol>`/`<use>` para reutilización. Evitar `<img src=*.svg>` cuando se necesita heredar color.

### 6.2 Ilustraciones
- Estados vacíos y onboarding usan ilustración SVG ligera y monocroma o con la rampa de marca, no stock raster.
- `viewBox` siempre presente; sin `width`/`height` fijos en el markup para que escale por contenedor. Optimizar (SVGO): sin metadatos de editor, sin estilos inline redundantes.
- Contraste y significado: la ilustración no porta información crítica que no esté también en texto.

### 6.3 Avatares y logos
- Avatar por defecto: iniciales sobre círculo con tint (no foto obligatoria). El SVG/markup hereda el color de rol.
- Logo de marca en SVG, con variante para fondo oscuro (chrome) y claro.

---

## 7. Accesibilidad (WCAG 2.2 AA, piso obligatorio)

Criterios prioritarios y verificables:
- Contraste de texto 4.5:1 (3:1 texto grande); de componentes y estados de foco, 3:1.
- Foco visible en todo elemento interactivo, con anillo de ≥2px que no dependa solo del color.
- Navegación completa por teclado en el orden lógico de lectura; sin trampas de foco; objetivos de toque ≥ 24×24px (2.5.8).
- Semántica: encabezado de pantalla con `<h1>`/`sr-only` por vista; landmarks (`nav`, `main`); labels asociados a cada control; `aria-label` en icon-buttons.
- El color nunca es el único canal de información (estado = color + texto/ícono).
- `prefers-reduced-motion` respetado: las animaciones no esenciales se desactivan.
- Mensajes de error asociados al campo (`aria-describedby`) y anunciados.

---

## 8. Responsive y movimiento

- Grillas fluidas con `auto-fit`/`minmax`; punto de quiebre principal alrededor de 768px. La barra lateral colapsa a navegación superior o drawer en viewport angosto.
- Contenido legible sin scroll horizontal a 320px de ancho; reflow conforme WCAG 1.4.10.
- Movimiento: transiciones de 150–250ms, `ease`/`ease-out`; entrada de paneles y cambios de estado, no movimiento ambiental permanente. Un momento orquestado (carga de pantalla, revelado de stepper) rinde más que efectos dispersos.

---

## 9. Criterios de aceptación del diseño

Una superficie cumple el catálogo cuando: usa solo tokens del sistema (cero literales sueltos); declara y resuelve sus estados (al menos vacío, cargando, con datos, error); todos sus íconos son SVG con `currentColor` y tratamiento ARIA correcto; pasa contraste AA y navegación por teclado; sus textos están en voz activa y nombran acciones de forma consistente extremo a extremo; y referencia los patrones de §4 por nombre en lugar de inventar variantes.

---

## 10. Anti-patrones

| Anti-patrón | Problema | Corrección |
| --- | --- | --- |
| Tokens visuales ad hoc por pantalla | Rompe consistencia y mantenibilidad | Heredar del catálogo; agregar token nuevo solo si es transversal |
| Ícono raster o emoji como ícono de sistema | No escala, no hereda color, accesibilidad pobre | SVG con `currentColor` |
| Estado solo por color | Falla daltónicos y AA | Color + texto/ícono |
| Wireframe sin estados | El stack de 05 no sabe qué dibujar al fallar | Enumerar vacío/cargando/datos/error |
| Placeholder como label | Se pierde el rótulo al escribir | Label visible + placeholder de ejemplo |
| Botón "Enviar/Aceptar" genérico | No comunica la acción | Nombrar el verbo real ("Guardar", "Publicar") |
| Animación ambiental permanente | Distrae y delata diseño automático | Movimiento al servicio del estado, con `reduced-motion` |
| Default hardcodeado en la pantalla o ayuda escrita a mano por campo | Se desincroniza del parámetro; dos fuentes de verdad | Derivar default, límites y ayuda del descriptor (ver `design-rules-config-esquema`) |

---

## 11. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Especialidad dueña | AG-03 UX/UI |
| Regla que lo invoca | `devs/rules/03_rules_ux_ui_dx.md` |
| Documentos hijos | `design-rules-blazor-mudblazor_v1.0.md` y futuros por stack |
| Extensión por capacidad | `design-rules-config-esquema_v1.0.md` (configuración dirigida por esquema) |
| Artefactos operativos que lo aplican | `experiencia-de-uso`, `wireframes-<superficie>`, `representacion-<concepto>` por proyecto |
| Marco teórico | `guides/marco-teorico-sdd2.2_v1.0.md`, cap. UX/UI/DX |

---

## 12. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-19 | Versión inicial. Tokens, layout, catálogo de patrones, estados, iconografía SVG, accesibilidad AA, responsive y anti-patrones. Base del catálogo `references/design/`. | AG-03 UX/UI |
| 1.1 | 2026-06-20 | Configuración dirigida por esquema: principio rector 8, estado semántico `info` (`color.text/background/border.info`) en §2.1 con confirmación de que `warning` corresponde a Atención, anti-patrón de default/ayuda hardcodeados en §10 y registro de la extensión por capacidad `design-rules-config-esquema` en §11. | AG-03 UX/UI |
