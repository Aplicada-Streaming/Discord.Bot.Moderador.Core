# Reglas constructivas — 10 Developer guide

**Carpeta target (por proyecto):** `/SDD2.1D/docs/proyectos/<nombre-proyecto-kebab>/10_developer_guide/`
**Subagente target del orquestador:** Technical Writer / Developer Advocate Senior (AG-10)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.1

La categoría 10 es la ventana que el proyecto le abre al desarrollador que lo va a consumir. Recibe upstream de 02 (casos de uso y modelo de datos lógico que explican qué resuelve el sistema), de 05 (arquitectura, contratos públicos, NFR y puntos de extensión que el consumidor debe respetar) y de 08 (estrategia de testing y convenciones de pruebas que la guía cita pero no redefine). Alimenta a 11 (examples ejecutables que ilustran los flujos descriptos por la guía) y, de manera transversal, a cualquier release y a su changelog público.

Esta categoría es **obligatoria para library, rest-api y cli-tool**, donde la audiencia es explícitamente un desarrollador externo o integrador. Es **recomendada para web-microservices** cuando expone APIs públicas o un BFF documentado. Es **opcional para web-monolith, desktop-app, mobile-app-maui y worker-service**, casos en los que la documentación de consumo se limita al equipo interno y suele resolverse con el README del repositorio.

La auditoría de Fase 0 (`_bootstrap/audit-sdd1.md`) detectó dos déficits del fuente SDD 1.0 que SDD 2.1 corrige aquí. Primero, los artefactos de `10_developer_guide/` del fuente carecen del sufijo `_v<X.Y>.md` (aparecen como `conceptos-fundamentales.md`, `formato-dsl-templates.md`, `guia-integracion-maui.md`, etc.), rompiendo la convención de versionado del resto del repositorio. Segundo, los nombres atan la guía a un sistema objetivo concreto (`guia-integracion-maui.md`), lo cual contamina la plantilla. SDD 2.1 corrige ambos puntos: impone `_v<X.Y>.md` uniforme y parametriza el nombre de la guía de integración como `guia-integracion-<sistema-objetivo>_v<X.Y>.md`.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Technical Writer / Developer Advocate Senior, equivalente al AG-10 del catálogo SDD. Perfil profesional de documentación técnica orientada al consumidor de la solución, no a su constructor. La distinción es deliberada: mientras 05 documenta cómo está construido el sistema, 10 documenta cómo usarlo. Se apoya en el framework Diátaxis (Procida, 2017+) para separar tutoriales, how-to, referencia y explicación; en la Google Developer Documentation Style Guide para criterios de redacción; y en el principio del "test del desarrollador nuevo": un developer que nunca vio el proyecto debería poder hacerlo funcionar siguiendo solamente esta carpeta, sin pedirle ayuda al equipo que lo construyó.

Combina dos facetas profesionales. La cara Technical Writer redacta conceptos, referencias y troubleshooting con precisión, sin jerga interna del equipo y con vocabulario consistente. La cara Developer Advocate prioriza el tiempo a primer éxito (TTFS, Time To First Success), publica snippets copiables que compilan al pegarlos, mantiene un onboarding corto y mide la curva de adopción. Cuando el proyecto expone una API pública o un CLI, el rol incluye además la generación o curación de referencia autogenerada desde código (XML docs, OpenAPI, help embebido), garantizando paridad estricta entre la API publicada y la documentada.

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Foco de audiencia |
| --- | --- | --- |
| library | Technical Writer + SDK Documentation Lead | Developer integrador que consume la librería desde su propia aplicación. Énfasis en conceptos, modelo mental, referencia de tipos y métodos públicos, ejemplos copy-paste. |
| rest-api | Technical Writer + API Documentation Lead | Developer consumidor de la API HTTP. Énfasis en referencia generada desde OpenAPI, autenticación, paginación, errores RFC 9457, sandboxes y colecciones reutilizables. |
| cli-tool | Technical Writer + CLI Documentation Lead | Operador o developer que usa la herramienta desde terminal. Énfasis en referencia de comandos, subcomandos, flags, exit codes, ejemplos por subcomando y variables de entorno. |
| web-microservices | Technical Writer + Platform Documentation Lead | Equipos consumidores internos o externos de los servicios públicos. Énfasis en guías por servicio, contratos publicados, observabilidad esperada. Categoría recomendada, no obligatoria. |
| web-monolith | Technical Writer (opcional) | Equipo interno y operación. La guía suele colapsar en README del repositorio. Categoría opcional. |
| desktop-app | Technical Writer (opcional) | Equipo interno y, si corresponde, integradores de plugins. Categoría opcional, sólo si hay puntos de extensión publicados. |
| mobile-app-maui | Technical Writer (opcional) | Equipo interno y operadores. Categoría opcional, sólo si la app expone un SDK o un canal de extensión. |
| worker-service | Technical Writer (opcional) | Equipo interno de operación. Categoría opcional; cuando aplica, se reduce a runbooks y a la guía de los mensajes consumidos y emitidos. |

El orquestador lee esta tabla y, según el `project_type` del proyecto en curso (leído del manifiesto), decide tres cosas: si la categoría 10 se genera, qué variante de especialidad se activa y qué subconjunto de artefactos se materializa. Cuando la categoría es opcional y el equipo decide omitirla, la decisión se documenta en una ADR con la justificación.

### 1.3 Multi-especialidad

La categoría 10 colabora con varias especialidades durante la redacción y la revisión:

- AG-02 Analista Funcional, para alinear el vocabulario de conceptos fundamentales con los CU y el modelo de datos lógico, evitando que la guía y la especificación se contradigan.
- AG-05 Arquitecto, para que la referencia y los conceptos respeten los contratos públicos, los puntos de extensión y las NFR declaradas.
- AG-08 QA / SDET, para que la guía cite la estrategia de testing del consumidor (cómo correr los tests del proyecto, cómo agregar tests a una integración) sin redefinirla.
- AG-11 Developer Advocate, dueño de la carpeta 11, que produce los examples ejecutables que la guía referencia. La frontera es nítida: 10 explica y referencia, 11 demuestra con código completo.
- AG-09 DevOps, para que la guía indique qué versión del artefacto se está documentando y cómo instalarlo desde el repositorio público correspondiente, sin atar el documento a un gestor de paquetes específico.

El AG-10 mantiene titularidad de los artefactos de 10. Las demás especialidades aportan revisión sectorial y validan paridad con su carpeta upstream.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra

| Archivo | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `conceptos-fundamentales_v<X.Y>.md` | library, rest-api, cli-tool | web-microservices con APIs públicas | Resto | Modelo mental, vocabulario, decisiones de diseño relevantes para el consumidor y qué NO hace el sistema. |
| `guia-onboarding-developer_v<X.Y>.md` | library, rest-api, cli-tool | web-microservices con APIs públicas | Resto | Tutorial Diátaxis. De cero a "primer éxito" en cinco pasos, con Hello world en menos de cinco minutos. |
| `guia-integracion-<sistema-objetivo>_v<X.Y>.md` | library con stacks de integración nombrados | rest-api integrada a un stack específico, cli-tool ejecutado dentro de un stack específico | Resto | How-to Diátaxis. El nombre del archivo se parametriza con `<sistema-objetivo>` (por ejemplo `guia-integracion-spa-react_v1.0.md`, `guia-integracion-aplicacion-cli_v1.0.md`, `guia-integracion-servicio-web_v1.0.md`). Se puede haber más de una guía si hay más de un stack objetivo. |
| `referencia-api_v<X.Y>.md` | library, rest-api | web-microservices con APIs públicas | Resto | Reference Diátaxis. Tipos públicos, métodos, eventos, excepciones, esquemas. Para REST, generada o curada desde OpenAPI. |
| `referencia-cli_v<X.Y>.md` | cli-tool | rest-api con CLI auxiliar oficial | Resto | Reference Diátaxis. Comandos, subcomandos, flags, exit codes, variables de entorno. |
| `troubleshooting_v<X.Y>.md` | library, rest-api, cli-tool | web-microservices con APIs públicas | Resto | Errores comunes con síntoma, causa, diagnóstico paso-a-paso y solución. Tabla de logs útiles. Cómo reportar un bug. |
| `glosario-tecnico_v<X.Y>.md` | library, rest-api, cli-tool | web-microservices con APIs públicas | Resto | Vocabulario canónico del consumidor con definición y referencia cross-doc. Reemplaza glosarios duplicados en cada documento. |
| `README.md` de la sección | Recomendado para todos los tipos donde la categoría aplica | — | — | Índice navegable, orden de lectura, prerequisitos y quick-start de cinco líneas. |

### 2.2 Reglas de inclusión y exclusión por tipo

| Tipo D8 | Categoría 10 | Conceptos | Onboarding | Integración | Referencia API | Referencia CLI | Troubleshooting | Glosario |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| library | Obligatoria | Sí | Sí | Sí (uno por stack objetivo) | Sí | No | Sí | Sí |
| rest-api | Obligatoria | Sí | Sí | Recomendado (cliente HTTP de referencia) | Sí (curada desde OpenAPI) | Sólo si hay CLI oficial | Sí | Sí |
| cli-tool | Obligatoria | Sí | Sí | Recomendado (integración en pipelines o stacks específicos) | Sólo si la herramienta expone API embebible | Sí | Sí | Sí |
| web-microservices | Recomendada si hay APIs públicas | Sí | Sí | Recomendado | Sí (por servicio público) | No | Sí | Sí |
| web-monolith | Opcional | Sólo README del repositorio | No | No | No | No | Resumido en el README | No |
| desktop-app | Opcional | Sólo si hay plugins | Si hay SDK de plugins | Si hay SDK de plugins | Si hay SDK de plugins | No | Sólo si hay plugins | Sólo si hay plugins |
| mobile-app-maui | Opcional | Sólo si hay SDK público | Si hay SDK público | Si hay SDK público | Si hay SDK público | No | Sólo si hay SDK público | Sólo si hay SDK público |
| worker-service | Opcional | No | No | No | No | No | Runbook breve | No |

El gating de la categoría es de granularidad doble: primero se decide si la categoría existe para el tipo D8; luego, dentro de la categoría, qué artefactos se materializan. El intake registra ambas decisiones en `decisiones-proyecto_v1.0.md`.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `conceptos-fundamentales_v<X.Y>.md` para el modelo mental del consumidor.
- `guia-onboarding-developer_v<X.Y>.md` para el tutorial de primer éxito.
- `guia-integracion-<sistema-objetivo>_v<X.Y>.md` para cada how-to de integración. El segmento `<sistema-objetivo>` es kebab-case y describe el stack o sistema receptor de la integración (por ejemplo `cli`, `servicio-web`, `aplicacion-de-escritorio`, `pipeline-ci`). No se admite hardcodear un nombre comercial.
- `referencia-api_v<X.Y>.md` para la referencia de la API pública.
- `referencia-cli_v<X.Y>.md` para la referencia de la CLI pública.
- `troubleshooting_v<X.Y>.md` para errores y diagnóstico.
- `glosario-tecnico_v<X.Y>.md` para el vocabulario canónico.
- `README.md` para el índice de la sección.

El sufijo `_v<X.Y>.md` es uniforme y obligatorio para todos los artefactos versionables, incluyendo los de esta categoría. Queda explícitamente corregido el antecedente del fuente SDD 1.0 (`conceptos-fundamentales.md`, `formato-dsl-templates.md`, `guia-integracion-maui.md`, etc.) que omitía el sufijo. Queda prohibido el patrón heredado `.v<X.Y>`. Queda prohibido el sufijo de dominio hardcodeado en el nombre (por ejemplo `guia-integracion-maui_v1.0.md` está vedado; debe parametrizarse como `guia-integracion-<sistema-objetivo>_v<X.Y>.md` con un slug genérico).

### 3.2 Convenciones de identificadores internos

- Cada concepto fundamental se nombra con un identificador kebab dentro del documento (por ejemplo `concepto-pipeline`, `concepto-binding`) para enlazar desde la referencia y desde 11.
- Cada paso del onboarding se numera (Paso 1, Paso 2) y declara el tiempo objetivo en minutos.
- Cada entrada del troubleshooting se identifica con un código `ISSUE-XX` para que se la pueda referenciar desde el código de error real, desde logs o desde tickets.

### 3.3 Vinculación cross-doc

- Upstream: la referencia API/CLI cita los contratos públicos de 05; los conceptos fundamentales citan el modelo de datos lógico de 02 y las decisiones de 05; el onboarding cita el quick-start del README maestro; el troubleshooting cita los códigos de error declarados en 05 y los casos de prueba de 08.
- Downstream: los examples de 11 referencian conceptos por su identificador y siguen la guía de integración del stack que ilustran. El README maestro del proyecto referencia el onboarding como su punto de entrada para developers.
- Cada documento de esta categoría incluye al pie una sección "Referencias cruzadas" con tres a cinco enlaces a documentos de 02, 05, 08 u 11 según corresponda. La regla mínima es al menos un enlace a 05 (contratos o arquitectura) por cada documento de 10.

### 3.4 Política de versionado

Una sola versión vigente por nombre lógico. Cuando se pasa de `v1.0` a `v2.0`, la versión anterior se archiva en `_legacy/` con estado `Superado`. La numeración mayor se reserva para cambios incompatibles en la API documentada (renombres de tipos públicos, eliminación de endpoints, cambios en flags de CLI con efecto breaking). La numeración menor cubre incorporaciones que no rompen el contrato vigente. Las correcciones de redacción sin cambio semántico se hacen sobre la misma versión y se reflejan en §9 control de cambios del documento afectado.

### 3.5 README de la sección

Obligatorio cuando la categoría 10 existe. Lista los artefactos vigentes, su nivel de complejidad (Básico / Medio / Avanzado), el orden de lectura recomendado, los prerequisitos para empezar y un quick-start de cinco a diez líneas que invita a leer el onboarding completo. No duplica contenido de los artefactos individuales: actúa como tabla de contenidos viva.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada artefacto inicia con un H1 y un bloque markdown de metadatos uniforme:

```markdown
# <Título del documento>

**Proyecto:** {{nombre-proyecto}}
**Documento:** <nombre-archivo>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Propuesto | Aprobado | Vigente | Superado | Archivado
**Fecha:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
**Tipo Diátaxis:** Tutorial | How-to | Reference | Explanation
**Audiencia:** {{perfil-del-consumidor}}
**Nivel:** Básico | Medio | Avanzado
**Tiempo estimado de lectura:** <N> min
```

El campo `Tipo Diátaxis` es obligatorio para esta categoría: declara explícitamente el cuadrante al que pertenece el documento. Mezclar cuadrantes es el anti-patrón principal que estas reglas previenen.

### 4.2 Estructura de `conceptos-fundamentales_v1.0.md` (Explanation)

1. Concepto central. Qué es el sistema en una a tres oraciones, con foco en qué transforma, qué recibe y qué entrega. Sin jerga interna.
2. Modelo mental. Diagrama del flujo principal con etapas nombradas, entradas y salidas. Cada etapa se explica en un párrafo corto y una tabla concepto/qué es/ejemplo.
3. Decisiones de diseño relevantes para el consumidor. Por qué se eligió X frente a Y cuando la decisión afecta cómo se usa el sistema (no cuando es un detalle interno). Cada decisión cita la ADR fuente en 05.
4. Vocabulario. Tabla término/definición operativa/ejemplo. Es el insumo del glosario; aquí aparece sólo el subconjunto crítico para entender el sistema.
5. Qué NO hace el sistema. Tabla que delimita la responsabilidad del consumidor frente a la del sistema. Evita expectativas falsas.

### 4.3 Estructura de `guia-onboarding-developer_v1.0.md` (Tutorial)

1. Prerequisites. Versión del runtime, herramientas, dependencias, credenciales si aplica. Cada prerequisito con el comando o link para obtenerlo.
2. Hello world (5 min). Script o snippet mínimo que produce un resultado visible. Copy-paste-ready. Output esperado mostrado textualmente.
3. Primer caso real (30 min). Caso con datos representativos, no triviales, pero acotados. Cada paso numerado, con código y output esperado.
4. Integración con un sistema (1 hora). Paso a paso de la integración con el sistema objetivo más común para el proyecto, sirviendo de puente al how-to. Linkea a `guia-integracion-<sistema-objetivo>_v1.0.md` para el detalle completo.
5. Siguientes pasos. Tres rutas posibles: profundizar en conceptos, consultar referencia, ver examples.

El criterio de éxito del onboarding es duro: un developer nuevo debe llegar al final del paso 4 en menos de una hora. Si el tiempo medido supera el objetivo, se reescribe.

### 4.4 Estructura de `guia-integracion-<sistema-objetivo>_v1.0.md` (How-to)

1. Objetivo. Una a tres oraciones que declaran qué problema concreto resuelve esta guía y para qué sistema objetivo.
2. Prerequisites. Estado inicial mínimo del sistema objetivo, dependencias agregadas, configuración previa.
3. Pasos numerados. Cada paso con título imperativo, código copy-paste y output o efecto esperado. Sin desviaciones explicativas: el porqué vive en `conceptos-fundamentales`.
4. Verificación. Cómo confirmar que la integración funciona. Comandos, checks visibles, criterios de éxito.
5. Troubleshooting específico. Subset de problemas conocidos para esta integración en particular, con código de issue del troubleshooting global cuando corresponde.

### 4.5 Estructura de `referencia-api_v1.0.md` (Reference)

1. Tipos públicos. Cada tipo con propósito, propiedades, invariantes documentadas.
2. Métodos. Cada método con firma exacta, parámetros (nombre, tipo, descripción, obligatorio/opcional, default), valor de retorno y excepciones que puede lanzar.
3. Eventos. Cada evento publicado por la API con su payload y semántica de orden y entrega.
4. Excepciones. Cada excepción con tipo, código si aplica, mensaje canónico y cuándo se lanza.
5. Ejemplos breves por método. Snippet de tres a diez líneas que ilustra el método más complejo de cada bloque. La referencia completa sin ejemplos es ilegible.

Cuando la API es HTTP, la referencia se genera o se cura desde OpenAPI 3.x y los ejemplos van en formato `curl` y en al menos un cliente HTTP. Los códigos de error siguen RFC 9457 (`application/problem+json`).

### 4.6 Estructura de `referencia-cli_v1.0.md` (Reference)

1. Comandos. Tabla con comando principal, descripción de una línea y página completa con el detalle.
2. Subcomandos. Por comando, los subcomandos disponibles con su descripción y ejemplo invocacional.
3. Flags. Por subcomando, tabla flag/tipo/default/descripción/obligatorio.
4. Variables de entorno. Tabla nombre/propósito/default/precedencia frente a flags.
5. Códigos de salida. Tabla código/significado. El cero significa éxito; el resto se documenta exhaustivamente.
6. Ejemplos. Al menos un ejemplo por subcomando, con input, comando exacto, output esperado en stdout y stderr.

### 4.7 Estructura de `troubleshooting_v1.0.md`

1. Errores comunes. Tabla síntoma/causa probable/solución, con identificador `ISSUE-XX`.
2. Diagnóstico paso-a-paso. Por cada error frecuente, secuencia ordenada de comandos y checks que permiten al consumidor confirmar la causa antes de aplicar la solución.
3. Logs útiles. Qué logs revisar, en qué nivel y qué patrón buscar. Comando concreto cuando aplica.
4. Cómo reportar un bug. Plantilla de issue, datos mínimos a adjuntar, dónde se reportan, política de severidad y tiempo de respuesta esperado.

### 4.8 Estructura de `glosario-tecnico_v1.0.md`

Tabla con tres columnas: término en kebab-case, definición operativa en una a tres oraciones, referencia cross-doc (al concepto en `conceptos-fundamentales` o al tipo en `referencia-api`). El glosario es la fuente canónica del vocabulario; el resto de los documentos enlaza acá en lugar de redefinir.

### 4.9 Tablas tipo y formatos recurrentes

Audiencia, documento y tipo Diátaxis:

| Audiencia | Documento | Tipo Diátaxis |
| --- | --- | --- |
| Developer que ingresa por primera vez | `guia-onboarding-developer_v1.0.md` | Tutorial |
| Developer con tarea concreta de integración | `guia-integracion-<sistema-objetivo>_v1.0.md` | How-to |
| Developer que necesita el dato exacto de un tipo, método o flag | `referencia-api_v1.0.md`, `referencia-cli_v1.0.md` | Reference |
| Developer que quiere entender por qué el sistema funciona como funciona | `conceptos-fundamentales_v1.0.md` | Explanation |
| Developer atascado con un error | `troubleshooting_v1.0.md` | How-to (orientado a diagnóstico) |
| Cualquier audiencia, lookup terminológico | `glosario-tecnico_v1.0.md` | Reference |

Tiempo a primer éxito (TTFS) por tarea:

| Tarea del consumidor | TTFS objetivo | Documento que lo soporta |
| --- | --- | --- |
| Hello world | < 5 min | `guia-onboarding-developer_v1.0.md` (§2) |
| Primer caso real | < 30 min | `guia-onboarding-developer_v1.0.md` (§3) |
| Integración completa con un stack | < 1 hora | `guia-onboarding-developer_v1.0.md` (§4) + `guia-integracion-<sistema-objetivo>_v1.0.md` |
| Resolver un error frecuente | < 10 min | `troubleshooting_v1.0.md` |
| Encontrar la firma exacta de un método o flag | < 1 min | `referencia-api_v1.0.md` o `referencia-cli_v1.0.md` |

Glosario término / definición / referencia cross-doc:

| Término | Definición operativa | Referencia cross-doc |
| --- | --- | --- |
| <termino-en-kebab> | <definicion-en-una-a-tres-oraciones> | <ruta-al-concepto-o-tipo> |

### 4.10 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Documentos sin sufijo de versión | Pierde trazabilidad entre release y documentación; impide convivir con `_legacy/` | Sufijo `_v<X.Y>.md` obligatorio en todos los artefactos (corrección del antecedente SDD 1.0) |
| Nombre de archivo hardcodeado con un sistema concreto | Atascamiento del template a un stack particular; bloqueo de proyectos con otro objetivo | Parametrizar como `guia-integracion-<sistema-objetivo>_v<X.Y>.md` con slug genérico |
| Mezcla de cuadrantes Diátaxis | Un tutorial con explicaciones largas se vuelve ilegible; una referencia con narrativa pierde rigor | Declarar `Tipo Diátaxis` en la cabecera y respetarlo; cada cuadrante con su tono y estructura |
| Onboarding que supera la hora | El consumidor abandona; el TTFS objetivo no se cumple | Cortar a Hello world en 5 min, primer caso en 30 min, integración completa en 1 hora; medir |
| Referencia sin ejemplos | La firma sola es ambigua; el consumidor inventa interpretaciones | Cada método o flag complejo con un snippet de tres a diez líneas |
| Troubleshooting sin diagnóstico paso-a-paso | "Causa probable: configuración" no le sirve al consumidor para confirmar | Cada `ISSUE-XX` con secuencia de comandos y checks antes de la solución |
| Pseudo-código en lugar de la API real | El snippet no compila; pérdida de credibilidad | Snippets validados en CI contra la versión documentada |
| Jerga interna del equipo | El consumidor no entiende los términos | Aplicar el test del developer nuevo; glosario centralizado |
| Docs desactualizados respecto a la API real | La guía dice X pero la API hace Y | Versionar 10 al ritmo de la API; checks de paridad en CI |
| Glosarios duplicados por documento | El mismo término con definiciones distintas en cada archivo | Un único `glosario-tecnico_v<X.Y>.md` referenciado desde todos los documentos |
| Cobertura cero de troubleshooting | El consumidor se traba ante el primer error y abandona | Mínimo cinco entradas `ISSUE-XX` con base en los errores reales reportados |
| Conceptos fundamentales que documentan la implementación interna | Confunde audiencia y duplica 05 | 10 documenta el modelo mental visible al consumidor; la implementación vive en 05 |

---

## 5. Preguntas guía para el subagente

### 5.1 Input upstream

- ¿La referencia API/CLI lista exactamente los contratos públicos declarados en 05? ¿Hay paridad uno a uno?
- ¿Cada decisión de diseño citada en `conceptos-fundamentales` tiene una ADR de origen en 05?
- ¿El onboarding usa la versión vigente de la API y no una rama experimental?
- ¿Los códigos de error documentados en `troubleshooting` coinciden con los que el código realmente emite?
- ¿La estrategia de testing referenciada cita 08 sin redefinirla?

### 5.2 Scope

- ¿La categoría 10 corresponde al tipo D8 del proyecto (obligatoria, recomendada u opcional)?
- ¿Se materializan sólo los artefactos pertinentes según §2.2, o se duplican documentos por inercia?
- ¿Hay guías de integración para los sistemas objetivo más comunes, sin saturar con stacks marginales?
- ¿El glosario contiene términos del consumidor o se contaminó con jerga interna del equipo?

### 5.3 Trazabilidad

- ¿Cada documento incluye su sección "Referencias cruzadas" con al menos un enlace a 05?
- ¿Cada concepto fundamental aparece en el glosario y se referencia desde la referencia?
- ¿Cada `ISSUE-XX` de troubleshooting está enlazado desde la guía de integración correspondiente cuando aplica?
- ¿Cada example de 11 referencia el documento de 10 que ilustra?

### 5.4 Calidad

- ¿Todos los snippets compilan o se ejecutan al copiarse y pegarse?
- ¿El TTFS objetivo se cumple en una corrida real con un developer que nunca vio el proyecto?
- ¿Cada documento declara explícitamente su tipo Diátaxis y respeta la estructura del cuadrante?
- ¿La guía pasa el "test del developer nuevo" sin recurrir a contacto humano del equipo?
- ¿Los anti-patrones de §4.10 están ausentes?

---

## 6. Criterios de aceptación

- [ ] Existe `conceptos-fundamentales_v1.0.md` con concepto central, modelo mental, decisiones de diseño relevantes, vocabulario y delimitación de responsabilidades.
- [ ] Existe `guia-onboarding-developer_v1.0.md` con prerequisites, Hello world < 5 min, primer caso real < 30 min, integración < 1 hora y siguientes pasos.
- [ ] Existe al menos una `guia-integracion-<sistema-objetivo>_v1.0.md` con nombre parametrizado (no hardcodeado a un sistema comercial concreto).
- [ ] Existe `referencia-api_v1.0.md` (library/rest-api) o `referencia-cli_v1.0.md` (cli-tool), con la estructura completa de §4.5 o §4.6.
- [ ] Existe `troubleshooting_v1.0.md` con al menos cinco entradas `ISSUE-XX`, diagnóstico paso-a-paso y plantilla de reporte de bug.
- [ ] Existe `glosario-tecnico_v1.0.md` con definición operativa y referencia cross-doc por término.
- [ ] Existe `README.md` con índice, orden de lectura, prerequisitos y quick-start de cinco a diez líneas.
- [ ] Todos los archivos llevan sufijo `_v<X.Y>.md` (corrección obligatoria del antecedente SDD 1.0 que omitía el sufijo en esta carpeta).
- [ ] Ningún nombre de archivo está hardcodeado a un sistema concreto; el slug `<sistema-objetivo>` es genérico (corrección obligatoria del `guia-integracion-maui` del fuente).
- [ ] Cada documento declara en cabecera su `Tipo Diátaxis`, su audiencia, su nivel y su tiempo estimado de lectura.
- [ ] Cada documento incluye al pie referencias cruzadas con al menos un enlace a 05.
- [ ] Los snippets se validan en CI contra la versión documentada (compilan o se ejecutan al copy-paste).
- [ ] La categoría 10 sólo se genera cuando el tipo D8 lo amerita (obligatoria, recomendada u opcional) y la decisión queda registrada en `decisiones-proyecto_v1.0.md`.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — `guia-onboarding-developer_v1.0.md` para una librería de parsing CSV

Fragmento ilustrativo:

```markdown
# Guía de onboarding — Librería de parsing CSV

**Proyecto:** {{nombre-proyecto}}
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Tipo Diátaxis:** Tutorial
**Audiencia:** Developer que integra la librería en su aplicación
**Nivel:** Básico
**Tiempo estimado de lectura:** 12 min

## 1. Prerequisites
- Runtime objetivo en versión soportada según el README.
- Gestor de paquetes configurado para resolver dependencias públicas.

## 2. Hello world (5 min)
Pegá el snippet en un script vacío. Output esperado: lista de objetos con las columnas `name` y `age` parseadas desde un CSV de dos filas.

## 3. Primer caso real (30 min)
Cargá un archivo CSV con encoding UTF-8, separador `;` y comillas dobles. Validá columnas, transformá tipos y reportá errores línea por línea.

## 4. Integración con un sistema (1 hora)
Caso de integración con un servicio web que recibe el CSV por upload y devuelve JSON normalizado. Linkea a `guia-integracion-servicio-web_v1.0.md`.

## 5. Siguientes pasos
- Profundizar: `conceptos-fundamentales_v1.0.md`.
- Referencia: `referencia-api_v1.0.md`.
- Ejemplos: `11_examples/`.
```

### 7.2 Ejemplo 2 — `referencia-cli_v1.0.md` para un CLI de migración de datos

Fragmento ilustrativo:

```markdown
# Referencia de CLI — Herramienta de migración de datos

**Proyecto:** {{nombre-proyecto}}
**Documento:** referencia-cli_v1.0.md
**Versión:** 1.0
**Tipo Diátaxis:** Reference
**Audiencia:** Operador o developer que ejecuta migraciones desde terminal

## 1. Comandos
| Comando | Descripción |
| --- | --- |
| `data-migrate plan` | Calcula el plan sin ejecutarlo. |
| `data-migrate apply` | Ejecuta el plan contra el destino. |
| `data-migrate verify` | Verifica consistencia post-migración. |

## 2. Subcomandos de `apply`
`--from <origen>`, `--dry-run`, `--resume`.

## 3. Flags
| Flag | Tipo | Default | Obligatorio |
| --- | --- | --- | --- |
| `--from` | string | — | Sí |
| `--to` | string | — | Sí |
| `--batch-size` | int | 1000 | No |
| `--dry-run` | bool | false | No |

## 4. Variables de entorno
`DATA_MIGRATE_LOG_LEVEL` (default `info`), `DATA_MIGRATE_CHECKPOINT_DIR` (default `.data-migrate/`). Precedencia: flag explícito sobre variable de entorno.

## 5. Códigos de salida
0 éxito, 1 validación, 2 conexión, 3 conflicto, 4 verificación fallida.

## 6. Ejemplos
Un ejemplo por subcomando: dry-run, apply con batch size custom, reanudación tras interrupción.
```

Los fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE-10}} responsable de redactar los artefactos de developer guide del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path}}
- Upstream: 02 (CU, modelo de datos lógico), 05 (arquitectura, contratos, NFR, puntos de extensión), 08 (estrategia de testing, casos de prueba referenciales).

A generar (según el tipo D8 declarado en el intake):
- conceptos-fundamentales_v1.0.md
- guia-onboarding-developer_v1.0.md
- guia-integracion-<sistema-objetivo>_v1.0.md (parametrizar el slug; una guía por stack objetivo relevante)
- referencia-api_v1.0.md (library, rest-api)
- referencia-cli_v1.0.md (cli-tool)
- troubleshooting_v1.0.md
- glosario-tecnico_v1.0.md
- README.md de la sección

Reglas de redacción: §4 de 10_rules_developer_guide.md.
Nomenclatura: sufijo uniforme `_v<X.Y>.md` obligatorio en todos los artefactos (corrección obligatoria del antecedente del fuente SDD 1.0 que omitía el sufijo en esta carpeta). Prohibido hardcodear el nombre del sistema objetivo en el archivo: usar el slug genérico `<sistema-objetivo>`.
Diátaxis: cada documento declara su tipo (Tutorial, How-to, Reference, Explanation) en la cabecera y respeta la estructura del cuadrante.
TTFS: Hello world < 5 min, primer caso real < 30 min, integración < 1 hora. Si no se cumple, reescribir.
Trazabilidad: cada documento incluye referencias cruzadas con al menos un enlace a 05; cada concepto aparece en el glosario; cada referencia es consistente con los contratos publicados en 05.
Criterios de calidad: §6 de 10_rules_developer_guide.md. Aplicar el test del developer nuevo antes de declarar Done.
```

---

## 9. Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial. Define la categoría 10 developer guide, sus artefactos obligatorios y opcionales por tipo D8, su estructura por cuadrante Diátaxis, el TTFS objetivo y los anti-patrones. Corrige los dos déficits del fuente SDD 1.0: impone sufijo `_v<X.Y>.md` uniforme en todos los artefactos de esta carpeta y parametriza el nombre de la guía de integración como `guia-integracion-<sistema-objetivo>_v<X.Y>.md` para evitar atar la plantilla a un stack comercial concreto. | AG-10 Technical Writer / Developer Advocate Senior |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/10_developer_guide/`; la frase de selección de variante y la carpeta target referencian el `project_type` del proyecto en curso (manifiesto). Tablas §1.2 sin reescritura. | Reformulación SDD 2.1D |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). | Migración SDD 2.1D |
