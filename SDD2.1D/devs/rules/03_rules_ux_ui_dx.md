# Reglas constructivas — 03 UX / UI / DX

**Carpeta target (por proyecto):** `/SDD2.1D/docs/proyectos/<nombre-proyecto-kebab>/03_ux_ui_dx/`
**Subagente target del orquestador:** Especialista UX/UI o Especialista DX (AG-03), según variante.
**Versión de las reglas:** 1.3

---

## 0. Posición en la cadena SDD 2.1

La categoría 03 recibe insumos de 00 (visión, alcance, persona objetivo) y de 02 (CU con interacción humana significativa, RN que afectan la presentación). Produce los artefactos que sirven como ancla para 05 (arquitectura de la capa de presentación o del portal de developers), 06 (US con criterios de aceptación visuales o de ergonomía de API), 08 (tests de UI, snapshot y accesibilidad) y 11 (ejemplos de uso ilustrados). Su salida define cómo se siente el producto, sin invadir el qué de 02 ni el cómo de 05.

La categoría 03 tiene dos variantes principales:

- Variante UX/UI, para tipos D8 con interfaz visible al usuario final.
- Variante DX, para tipos D8 cuya superficie pública es código, contratos, mensajes de error y documentación.

El orquestador elige la variante según el `project_type` del proyecto en curso (leído del manifiesto) y, en algunos casos, las combina (ver §1.3). La variante se aplica una vez por cada proyecto de la solución.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Especialista en experiencia, equivalente AG-03 del catálogo SDD. Su perfil profesional combina investigación de usuarios, diseño de interacción, accesibilidad, redacción técnica y diseño de APIs. Se alinea con ISO 9241-210 para el proceso centrado en personas, con WCAG 2.2 nivel AA como piso de accesibilidad, con las heurísticas de Nielsen y las reglas de Shneiderman para la inspección, y con el marco Diátaxis para la estructura de documentación técnica orientada al developer.

La variante UX/UI se enfoca en personas no técnicas que recorren pantallas. La variante DX se enfoca en developers que integran una librería, consumen una API, operan un CLI o supervisan un servicio en segundo plano. Las dos variantes comparten el mismo cuerpo conceptual y los mismos principios; cambian los artefactos producidos y las métricas de éxito.

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Variante | Especialidad específica | Justificación |
| --- | --- | --- | --- |
| library | DX | DX Lead | El producto se consume por código. El foco está en la superficie pública, la documentación de cada función o tipo, y los ejemplos ejecutables. |
| web-monolith | UX/UI | UX/UI Designer + Frontend Lead | El usuario final recorre pantallas en navegador. Se diseña experiencia, layout, estados y accesibilidad de cada vista. |
| web-microservices (con frontend) | UX/UI | UX/UI Designer + Frontend Architect | El frontend integra varios servicios. Se diseña experiencia consistente entre módulos y un design system compartido. |
| web-microservices (sin frontend) | DX | DX Lead para portal de developers | La superficie pública es la API. Se diseña el portal de documentación, los ejemplos de integración y los mensajes de error. |
| desktop-app | UX/UI | UX/UI Designer + Cross-Platform Specialist | Se diseña la experiencia para sistema operativo con convenciones de plataforma (atajos de teclado, ventanas, instaladores). |
| mobile-app-maui | UX/UI | Mobile UX Designer + Accessibility Specialist | El uso es táctil, en contextos variables, con consideraciones de permisos del sistema operativo y de accesibilidad reforzada. |
| rest-api | DX (con UX en portal) | API DX Designer + Developer Advocate | El consumidor es un developer integrador. Se aplica Diátaxis al portal de docs, OpenAPI como contrato y ejemplos de cada endpoint. |
| cli-tool | DX | CLI UX Designer + DX Engineer | La interfaz es la línea de comandos. Se diseña ayuda contextual, mensajes de error accionables, autocompletado y exit codes. |
| worker-service | DX para operadores | DX Engineer (Operability) | El consumidor es quien opera el servicio. Se diseñan logs estructurados, dashboards, alertas y runbooks. |

### 1.3 Multi-especialidad

La categoría 03 se combina con otras especialidades cuando el alcance lo requiere:

- AG-02 Analista Funcional, para que cada flujo UX se ancle en un CU con interacción humana y cada artefacto DX se ancle en el contrato funcional de la API o de la CLI.
- AG-04 Ingeniero de Prompts, cuando una pantalla o un comando dispara un flujo asistido por LLM, para definir los affordances y los estados de espera, error y revisión humana.
- AG-05 Arquitecto, para alinear el wireframe con la arquitectura de la capa de presentación, o el portal de developers con el contrato OpenAPI.
- AG-08 QA, para que los estados visuales y los mensajes DX sean automatizables como snapshot tests o tests de contrato.
- AG-10 Technical Writer, para alinear el tono de los mensajes y la documentación con la guía de estilo del proyecto.

Hay dos casos de combinación explícita de variantes:

- rest-api con portal de developers visible. La superficie técnica es DX, pero el portal en sí es una aplicación web con experiencia propia. El artefacto principal es `dx-portal-developers_v1.0.md`, complementado con wireframes de las pantallas clave del portal.
- web-microservices con frontend más SDK público. Se produce experiencia para el usuario final (UX/UI) y experiencia para el integrador externo (DX), en documentos separados.

El AG-03 mantiene siempre la titularidad del artefacto; las demás especialidades aportan revisiones acotadas.

### 1.4 Insumos normativos de diseño por stack

Antes de redactar `experiencia-de-uso` y `wireframes`, el AG-03 carga el catálogo de diseño de `devs/references/design/` a través de su índice `_index_design-rules.md`.

Aplica siempre el documento base `design-rules-web-generico_v1.0.md` y, si existe, la especialización del stack declarado en la Parte C del intake (por ejemplo `design-rules-blazor-mudblazor_v1.0.md` para Blazor Interactive Server + MudBlazor). Si no hay especialización para el stack, rige únicamente el documento base.

Los tokens, patrones, estados y la iconografía SVG del catálogo son normativos: el subagente referencia sus patrones por nombre y hereda sus tokens; tiene prohibido definir tokens visuales ad hoc por proyecto. Un token nuevo solo se admite si es transversal y se promueve al catálogo.

Trazabilidad: cada artefacto 03 con UI declara, en su tabla de trazabilidad, el o los documentos del catálogo de diseño aplicados.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra de artefactos

| Archivo | Variante | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- | --- |
| `experiencia-de-uso_v1.0.md` | UX/UI | web-monolith, web-microservices (con frontend), desktop-app, mobile-app-maui | — | library, cli-tool, worker-service, rest-api sin portal | Marco de experiencia: audiencia, principios de diseño, flujos clave, accesibilidad, internacionalización, performance percibida. |
| `wireframes-<superficie>_v1.0.md` | UX/UI | Tipos con UI final, uno por pantalla o flujo principal | — | Tipos sin UI final | Esquema textual o ASCII de cada pantalla con componentes, estados, interacciones, versión móvil cuando aplica. |
| `representacion-<concepto>_v1.0.md` | UX/UI o DX | Cuando hay una representación visual o estructural reutilizada (un componente, una notificación, un documento exportado) | — | Si no aplica | Documento focalizado sobre una representación específica del sistema. |
| `glosario-ux_v1.0.md` | UX/UI | Recomendado para todos los tipos con UI final | Tipos DX si hay vocabulario propio | — | Terminología canónica de la sección (pantalla, vista, modal, toast, estado vacío, etcétera). |
| `dx-developer-experience_v1.0.md` | DX | library, cli-tool, worker-service, rest-api, web-microservices sin frontend | — | Tipos con UI final únicamente | Marco DX: audiencia developer, onboarding por tramos, quick-start, Diátaxis, mensajes de error, métricas DX. |
| `guia-onboarding-developer_v1.0.md` | DX | library, rest-api, web-microservices sin frontend | cli-tool, worker-service | — | Recorrido de primera hora del developer integrador. |
| `dx-error-messages_v1.0.md` | DX | cli-tool, library | rest-api, worker-service | — | Catálogo de mensajes de error y su diagnóstico accionable. |
| `dx-portal-developers_v1.0.md` | DX | rest-api con portal visible, web-microservices con SDK público | library con portal hospedado | Tipos sin portal | Especificación del portal de documentación de developers. |
| `dx-operability_v1.0.md` | DX | worker-service | rest-api con SLO estricto | Tipos con UI final | Experiencia del operador: logs estructurados, dashboards, alertas, runbooks. |
| `README.md` de la sección | Ambas | Recomendado para todos | — | — | Índice navegable de la sección con estado actual de cada artefacto. |

### 2.2 Reglas de inclusión y exclusión por tipo

| Tipo D8 | Variante principal | Mínimo de wireframes | DX docs obligatorios |
| --- | --- | --- | --- |
| library | DX | 0 | `dx-developer-experience`, `guia-onboarding-developer`, `dx-error-messages` |
| web-monolith | UX/UI | 1 por superficie clave (mínimo 4: login, home, flujo principal, error) | — |
| web-microservices (con frontend) | UX/UI | 1 por módulo expuesto (mínimo 5) | — |
| web-microservices (sin frontend) | DX | 0 | `dx-developer-experience`, `dx-portal-developers` |
| desktop-app | UX/UI | 1 por ventana principal (mínimo 4) | — |
| mobile-app-maui | UX/UI | 1 por pantalla principal en portrait, más una nota responsive (mínimo 5) | — |
| rest-api | DX (con UX en portal opcional) | 0 (1 a 3 si hay portal visible) | `dx-developer-experience`, `guia-onboarding-developer`, `dx-portal-developers` |
| cli-tool | DX | 0 | `dx-developer-experience`, `dx-error-messages` |
| worker-service | DX | 0 | `dx-developer-experience`, `dx-operability` |

El mínimo es piso, no techo. La cota superior queda definida por la cobertura de los CU con interacción humana relevante o de la superficie pública del producto.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `experiencia-de-uso_v<X.Y>.md` para el marco UX.
- `wireframes-<superficie-kebab>_v<X.Y>.md` para cada superficie. Ejemplo: `wireframes-pantalla-login_v1.0.md`.
- `representacion-<concepto-kebab>_v<X.Y>.md`. Ejemplo: `representacion-notificacion-toast_v1.0.md`.
- `glosario-ux_v<X.Y>.md` para el vocabulario de la sección.
- `dx-developer-experience_v<X.Y>.md` para el marco DX.
- `guia-onboarding-developer_v<X.Y>.md` para el recorrido de primera hora.
- `dx-error-messages_v<X.Y>.md` para el catálogo de errores accionables.
- `dx-portal-developers_v<X.Y>.md` para el portal de documentación.
- `dx-operability_v<X.Y>.md` para la experiencia del operador.

Queda prohibido el patrón heredado `.v<X.Y>.md`. La versión siempre va con guion bajo antes de `v`, jamás con punto. El slug siempre va en minúsculas; queda prohibido `wireframes-Pantalla-Login_v1.0.md` o variantes con mayúsculas. El sufijo `_v<X.Y>.md` es uniforme para todos los artefactos de la sección.

### 3.2 Convenciones de prefijos

- `wireframes-`: una superficie por archivo. Una superficie es una pantalla, un modal con flujo propio, una ventana o una pestaña con estados independientes.
- `representacion-`: una representación reutilizable por archivo. Sirve para conceptos que aparecen en varios wireframes y conviene centralizar.
- `dx-`: artefactos DX. Cada uno cubre un aspecto distinto: marco general, onboarding, mensajes de error, portal, operabilidad.
- `glosario-ux`: nombre fijo. Sin variante de dominio.

### 3.3 Vinculación cross-doc

- Upstream: cada `experiencia-de-uso` declara la persona objetivo definida en 00 y los CU de 02 que tienen interacción humana. Cada wireframe declara el CU que lo origina. Cada DX doc declara la superficie pública (módulos de la librería, endpoints de la API, comandos del CLI, mensajes del worker) a la que sirve.
- Downstream: la sección 03 alimenta 05 con requisitos no funcionales de la capa de presentación o del portal, alimenta 06 con criterios de aceptación visuales o de ergonomía, y alimenta 08 con escenarios de snapshot test y de test de accesibilidad.
- Glosario: todo término que aparezca en más de un artefacto de 03 debe estar en `glosario-ux`. Si un término ya está en el glosario de 02 con la misma semántica, se referencia y no se duplica.

### 3.4 README de la sección

Recomendado para todos los tipos. Debe listar los artefactos vigentes con propósito en una línea, la variante aplicada y el estado actual. Sirve como punto de entrada navegable para revisores externos (AG-05, AG-06, AG-08).

### 3.5 Política de versionado

Una sola versión vigente por nombre lógico. Cuando un artefacto pasa de `v1.0` a `v1.1` o `v2.0`:

1. Se crea el archivo nuevo en la carpeta principal con la versión actualizada.
2. La versión anterior se mueve a `_legacy/` con estado `Superado` y una nota al inicio que apunte a la versión vigente.
3. El README de la sección referencia únicamente la versión vigente.
4. Los wireframes y `dx-` docs que dependan del artefacto modificado se actualizan en la misma operación o se marca explícitamente la deuda.

Queda prohibido mantener `v1.0` y `v1.1` conviviendo en la carpeta principal sin marcado de deprecación. La lección está documentada en `_bootstrap/audit-sdd1.md` Fase 0, donde se detectaron `experiencia-de-uso-del-motor_v1.0` y `v1.1` coexistiendo con la representación y los wireframes en el mismo estado.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada artefacto inicia con un H1 y un bloque markdown de metadatos:

```markdown
# <Título del artefacto>

**Proyecto:** {{nombre-proyecto}}
**Documento:** <nombre-de-archivo>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Propuesto | Aprobado | Vigente | Superado | Archivado
**Fecha:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
**Variante:** UX/UI | DX
```

### 4.2 Secciones obligatorias de `experiencia-de-uso_v1.0.md`

1. Audiencia y contexto de uso. Persona objetivo, contexto físico y emocional, frecuencia y duración de uso.
2. Principios de diseño. Selección explícita de heurísticas de Nielsen aplicables y leyes UX relevantes (Hick, Fitts, Miller, Jakob, según el caso) con justificación de aplicación al producto.
3. Flujos clave. Narrativa de cada user journey con disparador, pasos, puntos de fricción anticipados y salida.
4. Estados y feedback. Mapa de estados por superficie clave (vacío, cargando, con datos, error, sin conexión, éxito) con el feedback visual y textual esperado.
5. Accesibilidad. Compromiso explícito con WCAG 2.2 nivel AA mínimo. Identificación de criterios prioritarios (contraste, foco visible, navegación por teclado, etiquetas semánticas, alternativas textuales).
6. Internacionalización. Idiomas soportados, expansión de texto esperada, dirección de lectura, formatos de fecha y número.
7. Performance percibida. Tiempos máximos tolerables por acción, uso de skeletons, optimistic UI cuando aplica, criterios de animación.
8. Errores y recuperación. Taxonomía de errores que el usuario verá, tono de los mensajes, vías de recuperación o handoff humano.
9. Trazabilidad. Tabla con CU upstream, wireframes que materializan el marco, US a generar en 06, tests previstos en 08.
10. Notas y supuestos.
11. Control de cambios.

### 4.2.1 Secciones obligatorias de `wireframes-<superficie>_v1.0.md`

1. Pantalla y propósito. Nombre canónico de la superficie y una a tres oraciones sobre la tarea que el usuario completa.
2. Layout. Descripción textual o ASCII art de la disposición espacial. No se incluyen colores, tipografías ni valores de CSS.
3. Componentes principales. Tabla con nombre, propósito, datos que muestra y comportamiento.
4. Interacciones. Tabla con acción, disparador, resultado esperado y precondición.
5. Estados. Tabla con estado, condición que lo produce y representación esperada.
6. Versión móvil o responsive cuando aplica. Notas sobre breakpoints, reflujo y elementos que se ocultan o se reorganizan.
7. Notas de implementación. Recomendaciones de accesibilidad específicas de la pantalla, performance percibida, internacionalización.
8. Trazabilidad. Tabla con CU origen, marco `experiencia-de-uso` aplicado, US a generar, tests previstos.
9. Control de cambios.

### 4.2.2 Secciones obligatorias de `representacion-<concepto>_v1.0.md`

1. Concepto representado y propósito.
2. Apariencia esquemática. Descripción textual o ASCII.
3. Variantes. Tabla con variante, condición de uso y diferencias esperadas.
4. Datos que consume.
5. Restricciones de accesibilidad.
6. Reutilización. Listado de wireframes o `dx-` docs que la invocan.
7. Control de cambios.

### 4.2.3 Secciones obligatorias de `dx-developer-experience_v1.0.md`

1. Audiencia developer. Tipo de developer (integrador, contributor, operador), nivel de experiencia esperado, herramientas que ya conoce.
2. Onboarding por tramos. Qué logra el developer en 5 minutos, en 30 minutos y en 1 hora. Cada tramo con su objetivo verificable.
3. Quick-start. Pasos mínimos para producir el primer resultado exitoso. Debe ser ejecutable y reproducible.
4. Diátaxis. Plan explícito de los cuatro modos de documentación: tutorial (orientado al aprendizaje), how-to (orientado a la tarea), reference (orientado a la información), explanation (orientado a la comprensión). Indicar dónde vive cada modo y cómo se enlazan.
5. Mensajes de error y diagnóstico. Principios de redacción (qué pasó, por qué pasó, qué hacer al respecto). Referencia al catálogo `dx-error-messages` cuando exista.
6. Métricas DX. Time-to-first-success (TTFS), time-to-first-value (TTFV), tasa de error de onboarding, otras métricas pertinentes.
7. Feedback loop. Cómo se recoge el feedback del developer (issues, discusiones, encuestas, telemetría con consentimiento) y cómo se incorpora al ciclo de mejora.
8. Trazabilidad. Tabla con superficie pública que se documenta, CU upstream, US a generar, tests previstos.
9. Control de cambios.

### 4.2.4 Secciones obligatorias de `guia-onboarding-developer_v1.0.md`

1. Audiencia y prerrequisitos.
2. Instalación o acceso. Pasos mínimos verificables.
3. Primer ejemplo ejecutable. Snippet que produce un resultado visible.
4. Diagnóstico de problemas frecuentes en la primera hora.
5. Próximos pasos. Enlaces explícitos al modo tutorial, how-to y reference (Diátaxis).
6. Control de cambios.

### 4.2.5 Secciones obligatorias de `dx-error-messages_v1.0.md`

1. Principios de redacción de errores. Lenguaje plano, acción sugerida, sin culpar al usuario.
2. Taxonomía. Categorías de error (entrada inválida, recurso ausente, conflicto de estado, error transitorio, error interno).
3. Catálogo. Tabla con código, categoría, mensaje, causa probable, acción sugerida.
4. Tono y voz. Coherencia con la guía de estilo del proyecto.
5. Localización. Política de traducción de los mensajes técnicos.
6. Control de cambios.

### 4.2.6 Secciones obligatorias de `dx-portal-developers_v1.0.md`

1. Audiencia y objetivos del portal.
2. Estructura de información según Diátaxis.
3. Navegación principal y búsqueda.
4. Páginas obligatorias (landing, quick-start, reference, changelog, status).
5. Ejemplos ejecutables y sandbox.
6. Accesibilidad del portal (WCAG 2.2 AA).
7. Métricas de uso del portal.
8. Control de cambios.

### 4.2.7 Secciones obligatorias de `dx-operability_v1.0.md`

1. Audiencia operador y contexto de operación.
2. Logs estructurados. Campos mínimos, niveles, correlation id.
3. Métricas y dashboards. Indicadores propuestos y umbrales.
4. Alertas y runbooks. Cada alerta con condición, severidad y acción esperada.
5. Procedimientos de recuperación.
6. Control de cambios.

### 4.3 Tablas tipo y formatos recurrentes

Se estandariza el uso de las siguientes tablas:

Tabla de estados y feedback (UX):

| Estado | Condición que lo produce | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | No hay datos disponibles aún | Ilustración o ícono neutro | Texto orientativo con acción siguiente |
| Cargando | Operación asíncrona en curso | Skeleton o spinner según duración esperada | Indicación de qué se está cargando |
| Error | Falla recuperable | Banner o inline | Causa y acción de recuperación |
| Éxito | Acción completada | Confirmación visual sutil | Confirmación con próxima acción posible |

Tabla de heurísticas aplicadas (UX):

| Heurística de Nielsen | Aplicación en el producto | Verificación |
| --- | --- | --- |
| Visibilidad del estado del sistema | Stepper en flujos de varios pasos | Inspección heurística por dos revisores |

Tabla de métricas DX:

| Métrica | Definición | Objetivo | Cómo se mide |
| --- | --- | --- | --- |
| TTFS | Tiempo desde la instalación hasta el primer resultado exitoso | <= 5 minutos | Telemetría opt-in o pruebas con cinco developers |
| TTFV | Tiempo hasta el primer valor de negocio | <= 1 hora | Encuesta a developers de adopción reciente |
| Tasa de error en onboarding | % de developers que abandonan antes del primer éxito | <= 20% | Telemetría opt-in |

Tabla de trazabilidad de un artefacto 03:

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | <referencia a 00> |
| CU origen | <CU-XX en 02> |
| Reglas de negocio relevantes | <RN-XX si aplica> |
| Wireframes asociados | <archivo o N/A> |
| US a generar | <US-XX en 06> |
| Tests previstos | <referencia tentativa a 08> |
| Catálogo de diseño aplicado | <design-rules-web-generico y especialización por stack, o N/A para variante DX> |

### 4.4 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Wireframe con detalle de CSS, paleta de colores o tipografía exacta | Invade UI fina; el wireframe se vuelve frágil ante cambios visuales | Mantener layout, jerarquía y comportamiento; la capa visual vive en 05 o en el design system |
| Documento de experiencia que solo describe el flujo feliz | El producto queda subdefinido ante fallos | Listar estados de error y recuperación por superficie |
| DX doc sin quick-start verificable o con snippet que no corre | El developer no logra el primer éxito y abandona | Validar manualmente el quick-start antes de publicar |
| Accesibilidad ausente o reducida a una mención genérica | Riesgo legal y de exclusión; impide cumplir 08 | Declarar WCAG 2.2 AA como piso y enumerar criterios prioritarios |
| Versionado paralelo `experiencia-de-uso_v1.0` y `v1.1` sin marcado de deprecación | Ambigüedad sobre cuál vigencia leer | Aplicar §3.5: una vigente, anteriores a `_legacy/` |
| Mensajes de error genéricos del tipo "Error 500" o "Ocurrió un problema" | El developer o el usuario no sabe qué hacer | Reescribir con causa probable y acción sugerida |
| Onboarding sin tramos verificables (5/30/60 minutos) | No se puede medir el éxito del onboarding | Agregar hitos verificables por tramo |
| Glosario duplicado con vocabulario distinto entre 02 y 03 | Confusión cross-doc | Reusar términos de 02 y solo agregar los nuevos de 03 |
| Wireframe sin estados | El developer de 05 no sabe qué dibujar cuando algo falla | Enumerar estados mínimos: vacío, cargando, con datos, error |
| DX docs que no aplican Diátaxis | Documentación mezclada entre tutorial y reference, ilegible | Separar los cuatro modos y enlazarlos explícitamente |
| Patrón `.v1.0` heredado | Convención prohibida en SDD 2.1 | Forzar `_v1.0` |
| Definir paleta, tipografía, espaciado o íconos ad hoc por proyecto en vez de heredar del catálogo de diseño | Rompe consistencia cross-proyecto y duplica decisiones ya tomadas; los íconos raster fallan accesibilidad | Heredar tokens y patrones de `references/design/`; iconografía SVG con `currentColor`; agregar token nuevo solo si es transversal y se promueve al catálogo |

---

## 5. Preguntas guía para el subagente

### 5.1 Upstream

- ¿Qué persona objetivo de 00 consume cada artefacto? ¿Es un usuario final no técnico o un developer integrador?
- ¿Qué CU de 02 tienen interacción humana significativa o exponen una superficie pública? Cada uno debe mapearse a un artefacto de 03.
- ¿Hay RN que condicionan la presentación o el mensaje de error? Si la respuesta es sí, deben aparecer enumeradas en el artefacto correspondiente.

### 5.2 Scope

- ¿La variante a aplicar es UX/UI, DX, o una combinación? El tipo D8 marca el default; la presencia de portal de developers en rest-api o microservicios puede agregar la variante alterna.
- ¿Cuántos wireframes son razonables para el alcance? Web-monolith con login, home, flujo principal y error es el piso; sumar uno por flujo crítico.
- ¿Cuáles son las superficies públicas a documentar en DX? Cada módulo o endpoint debe quedar cubierto por al menos un how-to o un reference.

### 5.3 Trazabilidad

- ¿Cada wireframe declara su CU origen?
- ¿Cada `dx-` doc declara la superficie pública que documenta?
- ¿El marco `experiencia-de-uso` enlaza con todos los wireframes producidos?
- ¿El glosario absorbe los términos nuevos sin duplicar los del glosario de 02?

### 5.4 Calidad

- ¿El compromiso de accesibilidad WCAG 2.2 nivel AA está declarado y enumera criterios prioritarios?
- ¿Cada wireframe enumera al menos los estados vacío, cargando, con datos y error?
- ¿Cada `dx-` doc tiene un quick-start verificable, con snippet ejecutable?
- ¿La documentación DX aplica Diátaxis y separa tutorial, how-to, reference y explanation?
- ¿Cada mensaje de error catalogado dice qué pasó, por qué pasó y qué hacer al respecto?

### 5.5 Versionado

- ¿Existe alguna versión anterior en la carpeta principal? Si la respuesta es sí, archivarla en `_legacy/` antes de publicar la nueva.
- ¿El README de la sección apunta solamente a las versiones vigentes?

---

## 6. Criterios de aceptación

- [ ] La variante aplicada (UX/UI, DX o combinada) está declarada explícitamente en la cabecera de cada artefacto y es coherente con el tipo D8.
- [ ] Existe `experiencia-de-uso_v1.0.md` para todo tipo con UI final, con las once secciones obligatorias del §4.2.
- [ ] Para tipos con UI final existe al menos un `wireframes-<superficie>_v1.0.md` por cada superficie clave, con las nueve secciones obligatorias del §4.2.1.
- [ ] Para tipos sin UI final existe `dx-developer-experience_v1.0.md` con las nueve secciones obligatorias del §4.2.3, incluyendo Diátaxis y onboarding por tramos verificables (5/30/60 minutos).
- [ ] Toda accesibilidad declarada toma WCAG 2.2 nivel AA como piso mínimo; las menciones a versiones anteriores aparecen solo en notas de evolución histórica.
- [ ] Cada wireframe enumera al menos los estados vacío, cargando, con datos y error.
- [ ] Cada `dx-` doc presenta un quick-start verificable con snippet ejecutable y reproducible.
- [ ] Cada artefacto declara trazabilidad upstream (persona objetivo, CU origen, RN si aplica) y downstream (US, tests).
- [ ] Ningún archivo usa el patrón `.v<X.Y>.md`; todos usan `_v<X.Y>.md` y kebab-case estricto en el slug.
- [ ] No coexisten versiones distintas del mismo nombre lógico en la carpeta principal; las superadas viven en `_legacy/`.
- [ ] El glosario de la sección no duplica términos del glosario de 02 con semántica distinta.
- [ ] No hay menciones a stacks concretos, productos comerciales ni protocolos específicos del dominio fuente.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — Fragmento de `experiencia-de-uso_v1.0.md` para un sistema de turnos médicos (web-monolith)

Fragmento ilustrativo, no documento completo:

```markdown
# Experiencia de uso — Sistema de turnos médicos

**Proyecto:** {{nombre-proyecto}}
**Documento:** experiencia-de-uso_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-17
**Autor:** Equipo UX
**Variante:** UX/UI

## 1. Audiencia y contexto de uso
- Persona primaria: agente administrativo de centro de salud, entre 30 y 55 años, uso intensivo durante turnos de 6 horas, en escritorio con monitor de 22 pulgadas.
- Persona secundaria: paciente que recibe la confirmación por correo electrónico, lectura en mobile, contexto disperso.

## 2. Principios de diseño
| Heurística de Nielsen | Aplicación |
| --- | --- |
| Visibilidad del estado del sistema | Stepper de tres pasos en la asignación de turno con microcopy del cupo restante. |
| Prevención de errores | Validación inmediata de superposición; el botón "Asignar" queda inhabilitado hasta resolver. |
| Recuperación de errores | Mensaje "El cupo se agotó mientras completabas el formulario. Probá con otro horario disponible." con acción sugerida. |

## 4. Estados y feedback
| Estado | Condición | Feedback |
| --- | --- | --- |
| Vacío | No hay turnos disponibles para la fecha seleccionada | Ilustración neutra y CTA "Probar con otra fecha". |
| Cargando | Consulta a la agenda en curso | Skeleton de tres líneas; spinner solo si supera 800 ms. |
| Error de red | Fallo transitorio | Banner con acción "Reintentar" y enlace a soporte. |

## 5. Accesibilidad
Compromiso WCAG 2.2 nivel AA. Criterios prioritarios: contraste 4.5:1 en texto, foco visible en todos los controles, navegación completa por teclado, etiquetas semánticas en formularios, anuncios de cambios dinámicos por aria-live para la confirmación del turno.

## 9. Trazabilidad
| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente administrativo (00) |
| CU origen | CU-03 Asignar turno médico |
| Reglas de negocio relevantes | RN-02, RN-04 |
| Wireframes asociados | wireframes-pantalla-asignacion-turno_v1.0.md |
| US a generar | US-05, US-06 |
```

### 7.2 Ejemplo 2 — Fragmento de `dx-developer-experience_v1.0.md` para una librería de parsing CSV (library)

```markdown
# DX — Librería de parsing CSV

**Proyecto:** {{nombre-proyecto}}
**Documento:** dx-developer-experience_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-17
**Autor:** Equipo DX
**Variante:** DX

## 1. Audiencia developer
Developer backend con uno a tres años de experiencia, que conoce manejo de archivos pero no necesariamente parsing avanzado. Llega buscando una librería más rápida que su solución actual.

## 2. Onboarding por tramos
| Tramo | Objetivo verificable |
| --- | --- |
| 5 minutos | Instalar la librería e imprimir las primeras tres filas de un CSV de ejemplo. |
| 30 minutos | Mapear filas a un objeto del dominio con tipado y manejo básico de errores. |
| 1 hora | Procesar un archivo grande con streaming y reportar progreso. |

## 3. Quick-start
Snippet ejecutable que produce el primer resultado visible. El equipo verifica el snippet a mano antes de cada release.

## 4. Diátaxis
| Modo | Ubicación | Propósito |
| --- | --- | --- |
| Tutorial | `docs/tutorial.md` | Aprender los fundamentos desde cero. |
| How-to | `docs/how-to/` | Resolver tareas concretas (procesar archivo grande, validar columnas, exportar a JSON). |
| Reference | `docs/reference.md` | Consultar cada función, tipo y opción de configuración. |
| Explanation | `docs/explanation.md` | Comprender el modelo de streaming y las decisiones de diseño. |

## 5. Mensajes de error y diagnóstico
Cada error indica qué pasó (línea, columna), por qué pasó (regla violada) y qué hacer al respecto (cómo corregir o cómo ignorar la fila). Catálogo completo en `dx-error-messages_v1.0.md`.

## 6. Métricas DX
| Métrica | Objetivo | Cómo se mide |
| --- | --- | --- |
| TTFS | <= 5 minutos | Telemetría opt-in del CLI de demo y pruebas con cinco developers externos. |
| TTFV | <= 1 hora | Encuesta a developers en sus primeras dos semanas. |

## 7. Feedback loop
Issues etiquetados como `dx`, sección de discusiones del repositorio, encuesta breve al cierre del primer mes de uso.

## 8. Trazabilidad
| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Developer backend integrador (00) |
| CU origen | CU-01 Parsear archivo CSV, CU-02 Mapear fila a objeto |
| US a generar | US-01, US-02, US-08 |
```

Los dos fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar la categoría 03 del proyecto {{NOMBRE_PROYECTO}}.

Tipo de proyecto: {{TIPO}} (uno de los ocho valores D8).
Variante por defecto según tipo:
- library, cli-tool, worker-service, rest-api sin portal, web-microservices sin frontend: variante DX.
- web-monolith, web-microservices con frontend, desktop-app, mobile-app-maui: variante UX/UI.
- rest-api con portal visible o web-microservices con SDK público: combinación DX + UX/UI.

Insumos:
- SOLUTION-INTAKE: {{path}}
- Upstream: 00 (visión, persona objetivo), 02 (CU con interacción humana o superficie pública, RN que afectan presentación).

A generar (según variante):
- Variante UX/UI: experiencia-de-uso_v1.0.md, wireframes-<superficie>_v1.0.md (uno por superficie clave), representacion-<concepto>_v1.0.md cuando aplique, glosario-ux_v1.0.md.
- Variante DX: dx-developer-experience_v1.0.md, guia-onboarding-developer_v1.0.md, dx-error-messages_v1.0.md (cli-tool, library), dx-portal-developers_v1.0.md (rest-api con portal), dx-operability_v1.0.md (worker-service).
- README.md de la sección (recomendado).

Reglas de redacción: §4 de 03_rules_ux_ui_dx.md.
Nomenclatura: `<nombre>_v1.0.md` con guion bajo antes de `v`; slug en minúsculas y kebab-case estricto.
Trazabilidad: cada artefacto declara persona objetivo, CU origen y US a generar.
Accesibilidad: WCAG 2.2 nivel AA como piso mínimo en todo artefacto UX.
Diátaxis: aplicación explícita en al menos un artefacto DX.
Política de versionado: §3.5; una sola versión vigente; anteriores a `_legacy/` con estado Superado.

Restricciones: no introducir stacks concretos, productos comerciales ni protocolos del dominio fuente. Idioma rioplatense técnico, tildes correctas, sin emojis.

Salida: /SDD2.1D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/03_ux_ui_dx/<estructura>.
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.1. Variantes UX/UI y DX cubriendo los 8 tipos D8, accesibilidad WCAG 2.2 AA como piso, Diátaxis explícito, política de versionado con `_legacy/`. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/03_ux_ui_dx/`; la selección de variante y la ruta de salida del prompt-snippet referencian el `project_type` del proyecto en curso (manifiesto). Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
| 1.3 | 2026-06-19 | Incorporación del catálogo de reglas de diseño (`devs/references/design/`) como insumo normativo de AG-03: nueva §1.4 (carga del índice, documento base más especialización por stack, tokens y patrones normativos, trazabilidad del catálogo), anti-patrón de tokens ad hoc en §4.4 y fila "Catálogo de diseño aplicado" en la tabla de trazabilidad de §4.3. |
