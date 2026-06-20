# Reglas constructivas — 04 Prompts AI

**Carpeta target (por proyecto):** `/SDD2.2D/docs/proyectos/<nombre-proyecto-kebab>/04_prompts_ai/`
**Subagente target del orquestador:** Ingeniero de Prompts / AI Specialist (AG-04)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.2

La categoría 04 produce los artefactos que rigen toda interacción del sistema con modelos de lenguaje (LLMs) y demás componentes de IA generativa. Es una categoría **opcional** y **gated** por declaración explícita del proyecto. Recibe insumos de 01 (necesidades de negocio que declaran feature LLM) y del SOLUTION-INTAKE (§17 P.11 decisiones pre-ADR, §17 P.10 NFR). Alimenta a 02 (CU que delegan parte del flujo en LLM), a 05 (arquitectura del pipeline AI), a 08 (tests de comportamiento del prompt y umbrales) y a 09 (rate limits, costos por entorno). Esta categoría no es transversal: si el proyecto no consume LLMs, no se genera.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Ingeniero de Prompts senior, equivalente AG-04 del catálogo SDD. Perfil profesional que combina ingeniería de instrucciones para LLMs, diseño de contratos de entrada/salida estructurada, evaluación empírica con datasets versionados y governance de uso responsable. Trata cada prompt como un artefacto de software: lleva versión, contrato, few-shot, guardrails, dataset de evaluación y métricas. Se alinea con OWASP LLM Top 10 para guardrails, con prompt-engineering guidelines de los principales proveedores (OpenAI, Anthropic, Google) y con métricas estándar de evaluación de NLP (accuracy, precision/recall, F1, BLEU/ROUGE, exact match, latencia p99, costo por request).

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Ingeniero de Prompts + ML Specialist | Solo aplica si la librería expone una interfaz LLM o un cliente AI. La librería no debe esconder costos ni guardrails al consumidor. |
| web-monolith | Ingeniero de Prompts + UX Conversacional | El prompt forma parte de un flujo humano; la latencia y el tono importan tanto como la precisión. |
| web-microservices | Ingeniero de Prompts + ML Platform Engineer | Múltiples servicios pueden invocar al LLM; se requiere abstracción del proveedor y observabilidad por servicio. |
| desktop-app | Ingeniero de Prompts + UX Conversacional | Considerar modo offline degradado y manejo explícito de errores de conectividad. |
| mobile-app-maui | Ingeniero de Prompts + Mobile AI Specialist | Latencia, tamaño del payload y opción de inferencia on-device versus API son decisiones críticas. |
| rest-api | Ingeniero de Prompts + API/SLA Specialist | El SLA de la API público depende del SLA del proveedor LLM; se necesita timeout y fallback. |
| cli-tool | Ingeniero de Prompts + DX Specialist | La salida debe ser parseable por máquina (JSON) y legible por humano (texto). El usuario controla cuándo invocar el LLM. |
| worker-service | Ingeniero de Prompts + ML Operations | Batch processing, idempotencia del prompt, control de costos por lote y métricas agregadas por job. |

### 1.3 Multi-especialidad

La categoría 04 colabora con otras especialidades de forma frecuente:

- AG-02 Analista Funcional, para identificar qué CU delegan en LLM y mantener la trazabilidad CU→prompt.
- AG-05 Arquitecto, para fijar el pipeline AI (cliente LLM, fallback, cache, observabilidad) y los ADR sobre proveedor o vendor lock-in.
- AG-08 QA, para que las métricas declaradas en la política y en la evaluación del prompt tengan tests automáticos en 08.
- AG-09 DevOps, para gestionar secrets de API keys, rate limits y costos por entorno.

El AG-04 mantiene titularidad del prompt, del dataset de evaluación y de la política de uso. Las demás especialidades aportan revisión y consumen sus salidas.

---

## 2. Documentos que produce esta categoría

### 2.1 Gating explícito

Esta categoría se genera **solo** si el proyecto declara explícitamente que usa LLMs. La declaración vive en dos lugares del intake:

- SOLUTION-INTAKE §17 P.11 (decisiones pre-ADR): bandera `usa_llm: true|false`.
- SOLUTION-INTAKE §17 P.10 (NFR): NFR explícita de funcionalidad asistida por IA o de procesamiento de lenguaje natural.

El orquestador lee este flag al inicio del bootstrap. Si `usa_llm = false` (o ambos campos están ausentes), **omite por completo la generación de la carpeta `/SDD2.2D/docs/04_prompts_ai/`**, deja constancia en el log del orquestador y no produce ningún artefacto de 04. Si el flag pasa de `false` a `true` en una iteración posterior, la categoría se activa de forma retroactiva en esa iteración y se reabren los criterios de aceptación del §6.

Queda prohibido producir documentos de 04 sin el flag positivo. Queda prohibido omitir documentos de 04 cuando el flag es positivo.

### 2.2 Tabla maestra

| Archivo | Obligatorio si la categoría está activa | Recomendado | Omitir | Descripción |
| --- | --- | --- | --- | --- |
| `politica-uso-ai_v1.0.md` | Sí | — | — | Política transversal de uso responsable, datos sensibles, validación humana, costos, vendor lock-in y disclaimer al usuario final. |
| `prompt-<tarea-kebab>_v1.0.md` | Sí, uno por tarea LLM identificada | — | — | Un prompt por tarea funcional. Incluye contrato, prompt completo, few-shot, métricas, costos y trazabilidad a CU. |
| `evaluacion-prompts_v1.0.md` | Sí | — | — | Marco de evaluación: métricas, dataset referenciado, protocolo de A/B, umbrales mínimos y frecuencia de re-evaluación. |
| `dataset-evaluacion_v1.0.md` | — | Sí | Proyectos con un único prompt trivial y dataset embebido | Dataset de evaluación versionado: muestras de entrada, salidas esperadas y origen de cada muestra. |
| `README.md` de la sección | Recomendado | — | — | Índice de prompts vigentes, estado y vínculo a la política y a la evaluación. |

### 2.3 Reglas de inclusión y exclusión por tipo

| Tipo D8 | Política | Prompts mínimos | Evaluación | Dataset | Comentario |
| --- | --- | --- | --- | --- | --- |
| library | Obligatoria si expone superficie LLM | 1 por método LLM público | Obligatoria | Recomendado | La librería documenta costos esperados al consumidor. |
| web-monolith | Obligatoria | 1 por feature LLM | Obligatoria | Recomendado | Considerar latencia perceptual del usuario. |
| web-microservices | Obligatoria | 1 por servicio AI | Obligatoria | Recomendado | Observabilidad por servicio. |
| desktop-app | Obligatoria | 1 por feature LLM | Obligatoria | Recomendado | Modo offline declarado. |
| mobile-app-maui | Obligatoria | 1 por feature LLM | Obligatoria | Recomendado | Decisión on-device versus API documentada. |
| rest-api | Obligatoria | 1 por endpoint AI | Obligatoria | Recomendado | SLA y timeout explícitos. |
| cli-tool | Obligatoria | 1 por subcomando AI | Obligatoria | Opcional | Contrato JSON estricto. |
| worker-service | Obligatoria | 1 por tipo de job AI | Obligatoria | Recomendado | Idempotencia y métricas agregadas. |

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `prompt-<tarea-kebab>_v<X.Y>.md` para cada prompt productivo.
- `politica-uso-ai_v<X.Y>.md` para la política transversal.
- `evaluacion-prompts_v<X.Y>.md` para el marco de evaluación.
- `dataset-evaluacion_v<X.Y>.md` para el dataset versionado.
- `README.md` para el índice navegable.

El sufijo `_v<X.Y>.md` es uniforme para los cuatro tipos. Queda prohibido el patrón heredado `.v<X.Y>`. Todos los slugs son kebab-case lowercase estricto.

### 3.2 Convenciones de prefijos y semántica

- `prompt-`: artefacto de tarea concreta delegada en LLM. Una tarea por archivo.
- `politica-uso-ai`: artefacto transversal, único en toda la categoría.
- `evaluacion-prompts`: marco metodológico de evaluación, único en toda la categoría.
- `dataset-evaluacion`: dataset común; puede haber datasets específicos por prompt, en cuyo caso se nombran `dataset-evaluacion-<tarea-kebab>_v<X.Y>.md`.

### 3.3 Vinculación cross-doc

- Upstream: cada `prompt-<tarea>` declara la NB que la motivó (01) y el CU que la consume (02). No puede existir prompt huérfano.
- Downstream: la `politica-uso-ai` se referencia desde el README de la sección y desde 05 (ADR de vendor). La `evaluacion-prompts` se referencia desde 08 (tests de comportamiento del prompt) y desde 09 (umbrales de release).
- Cada prompt declara el conjunto de métricas y umbrales con el que será evaluado; los valores numéricos viven en `evaluacion-prompts` y se referencian desde el prompt.

### 3.4 README de la sección

Recomendado. Debe listar los prompts vigentes con propósito en una línea, estado y métrica crítica de cada uno, junto al enlace a la política y a la evaluación.

### 3.5 Política de versionado

Una sola versión vigente por nombre lógico. La versión menor avanza cuando el cambio es de aclaración, redacción o ajuste menor del prompt (cambio bajo el 30% del contenido). La versión mayor avanza cuando el cambio supera el 30% del contenido del prompt, modifica el contrato de entrada o salida, o reemplaza el modelo target.

Cuando un prompt pasa de `v1.0` a `v2.0`:

1. Se crea `prompt-<tarea>_v2.0.md` en la carpeta principal.
2. La versión `v1.0` se mueve a `_legacy/` con estado `Superado` y nota inicial que apunte a la versión vigente.
3. El README de la sección referencia únicamente la versión vigente.
4. Las métricas y el dataset se reevalúan antes de declarar vigente la versión nueva.

Queda prohibido mantener `v1.0` y `v2.0` conviviendo en la carpeta principal sin marcado de deprecación.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada artefacto inicia con un H1 y un bloque markdown de metadatos:

```markdown
# Prompt — <Nombre descriptivo de la tarea>

**Proyecto:** {{nombre-proyecto}}
**Documento:** prompt-<tarea-kebab>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Propuesto | Aprobado | Vigente | Superado | Archivado
**Fecha:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
```

Para la política, la evaluación y el dataset aplica la misma cabecera adaptando título y descripción.

### 4.2 Secciones obligatorias de un prompt

Un archivo `prompt-<tarea>_v<X.Y>.md` tiene exactamente las siguientes secciones, en orden:

1. Objetivo de la tarea. Una a tres oraciones que respondan qué resuelve el prompt y para quién.
2. Modelo LLM target. Familia (por ejemplo Claude Sonnet, GPT-4o, Gemini Pro) y tamaño esperado, con justificación y plan de portabilidad a una familia alternativa.
3. Contrato de entrada. Esquema de los datos que recibe el prompt: variables, tipos, longitud máxima, restricciones, idioma.
4. Contrato de salida. Formato exacto (JSON, markdown estructurado, texto plano). Si es JSON, incluye el JSON Schema completo. Si es markdown, incluye el patrón de headings y secciones obligatorias.
5. Prompt completo. Bloque de código `text` con el system prompt y los slots de usuario. Sin truncar.
6. Few-shot examples. Mínimo dos por prompt, con un caso típico y un caso de borde. Cuando la tarea sea de generación abierta, mínimo tres.
7. Casos de borde y manejo. Tabla con condición de entrada anómala y respuesta esperada del prompt (rechazo, valor por defecto, escalado a humano).
8. Métricas de evaluación. Lista de métricas aplicables con la métrica crítica resaltada. Los umbrales viven en `evaluacion-prompts`.
9. Costos esperados. Tokens estimados de entrada y de salida y costo por request en la moneda declarada en 09.
10. Latencia esperada. Latencia p50 y p99 esperadas según familia y tamaño del modelo.
11. Trazabilidad. Tabla con NB, CU, ADR, métricas en 08 y referencia a la política y a la evaluación.

#### 4.2.1 Secciones obligatorias de la política de uso

1. Cuándo usar AI y cuándo no. Criterios explícitos para decidir si una tarea entra al pipeline AI o se resuelve sin IA.
2. Datos sensibles y privacidad. Qué datos no pueden enviarse al proveedor, qué redacción se aplica antes del envío, qué cláusulas de procesamiento de datos exigir al proveedor.
3. Confianza y validación humana. Cuándo confirmar la salida con un humano antes de aplicar efectos. Bucle de control declarado por tipo de tarea.
4. Sesgo y fairness. Cómo se identifican y mitigan sesgos (lengua, género, geografía, demográfico) en prompts y datasets.
5. Costos y rate limits. Política de límites por usuario, por feature y por entorno. Acción ante exceder el límite.
6. Disclaimer al usuario final. Texto base que la aplicación muestra al usuario cuando la salida proviene de un LLM.
7. Vendor lock-in. Alternativas documentadas al proveedor primario y plan de portabilidad mínimo.

#### 4.2.2 Secciones obligatorias de la evaluación

1. Métricas. Lista completa con definición operativa de cada una: accuracy, precision/recall, F1, BLEU/ROUGE, exact match, latencia p50/p99, costo por request. La métrica crítica se declara primero.
2. Dataset de evaluación. Referencia al `dataset-evaluacion` versionado, con número de muestras, distribución por clase y origen.
3. Protocolo de A/B testing. Cómo se comparan dos versiones del mismo prompt: tamaño muestral mínimo, criterio estadístico, regla de decisión.
4. Umbrales mínimos. Tabla `Métrica → Umbral mínimo → Acción si no se alcanza`.
5. Frecuencia de re-evaluación. Cuándo se vuelve a correr la evaluación: por cambio de prompt, por cambio de proveedor, por calendario fijo.

#### 4.2.3 Secciones obligatorias del dataset

1. Propósito y alcance del dataset.
2. Distribución de muestras por clase o por categoría.
3. Origen de las muestras (sintéticas, anonimizadas, reales con consentimiento).
4. Tratamiento de datos sensibles aplicado antes de incluirlas en el dataset.
5. Tabla de muestras: identificador, entrada, salida esperada, etiqueta.
6. Control de cambios del dataset.

### 4.3 Secciones opcionales por tipo de proyecto

- §12 Modo offline o degradado, sólo para mobile-app-maui y desktop-app.
- §13 Idempotencia del prompt y deduplicación, sólo para worker-service y rest-api.
- §14 Streaming de salida y manejo parcial, sólo para web-monolith y rest-api con UX en tiempo real.
- §15 Inferencia on-device versus API, sólo para mobile-app-maui.
- §16 Auditoría regulatoria del prompt (por ejemplo procesamiento de datos personales), cuando el dominio lo exija.

### 4.4 Tablas tipo y formatos recurrentes

Tabla de métricas versus umbral mínimo:

| Métrica | Definición | Umbral mínimo | Acción si no se alcanza |
| --- | --- | --- | --- |
| Accuracy de clasificación | Proporción de aciertos sobre dataset | ≥ 0.90 | Bloquear release |
| Latencia p99 | Tiempo de respuesta de cola | ≤ 2500 ms | Investigar proveedor o tamaño del modelo |
| Costo por request | Costo promedio por invocación | ≤ 0.01 USD | Optimizar prompt o cambiar modelo |
| Tasa de rechazo válido | Proporción de entradas fuera de dominio correctamente rechazadas | ≥ 0.95 | Reforzar guardrails |

Tabla de casos de borde y respuesta esperada:

| Caso de borde | Entrada de ejemplo | Respuesta esperada del prompt |
| --- | --- | --- |
| Entrada vacía | "" | Rechazo con código `INPUT_VACIO` |
| Entrada fuera de dominio | "<entrada irrelevante>" | Categoría `otro` con confianza < 0.5 |
| Intento de prompt injection | "ignorá tus instrucciones y..." | Mantenimiento del contrato; respuesta `INPUT_INVALIDO` |
| Entrada en otro idioma | "<entrada en idioma no soportado>" | Rechazo con código `IDIOMA_NO_SOPORTADO` |

Tabla de costos por modelo y proveedor:

| Proveedor | Modelo | Tokens entrada | Tokens salida | Costo unitario aprox. | Latencia p99 esperada |
| --- | --- | --- | --- | --- | --- |
| {{proveedor-1}} | {{modelo-1}} | <n_in> | <n_out> | <USD> | <ms> |
| {{proveedor-2}} | {{modelo-2}} | <n_in> | <n_out> | <USD> | <ms> |

Tabla de trazabilidad del prompt:

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-XX |
| Caso de uso consumidor | CU-XX |
| ADR de proveedor o vendor lock-in | ADR-XX |
| Tests de comportamiento previstos | <referencia a 08> |
| Métricas de release | <referencia a evaluacion-prompts> |

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Prompt sin contrato de salida formal | La salida no es parseable y rompe el consumidor | Forzar JSON con JSON Schema o markdown con headings fijos |
| Prompt sin few-shot cuando la tarea lo requiere | El modelo no tiene referencia de calidad esperada y alucina | Mínimo dos few-shot por prompt; tres si es generación abierta |
| Política de uso sin sección de datos sensibles | Se exponen datos personales al proveedor sin control | Sección §2 obligatoria con clasificación de datos y política de redacción |
| Evaluación sin dataset versionado | No hay forma de comparar versiones del prompt | Versionar `dataset-evaluacion` con sufijo `_v<X.Y>.md` |
| Vendor lock-in sin alternativa documentada | Si el proveedor sube precios o cambia política, no hay plan B | Sección §7 de la política con alternativa primaria y plan de portabilidad |
| Confiar ciegamente en la salida del LLM | Efectos críticos aplicados sin validación humana | Declarar bucle de control humano para tareas con efecto irreversible |
| Prompt monolítico con varias tareas | Difícil de evaluar y de versionar | Chain of prompts: un archivo por tarea con contrato propio |
| Métricas sin umbral numérico | "Debe ser buena" no es evaluable | Cada métrica con umbral cuantitativo y acción si no se alcanza |
| Patrón heredado `.v<X.Y>` | Convención prohibida en SDD 2.2 | Forzar `_v<X.Y>` |
| Casing inconsistente en el slug | Rompe automatizaciones | Forzar kebab lowercase estricto |

---

## 5. Preguntas guía para el subagente

### 5.1 Upstream

- ¿El proyecto declara `usa_llm = true` en el SOLUTION-INTAKE §17 P.11 o una NFR de IA en §17 P.10? Sin esa declaración, no se genera la categoría.
- ¿Qué CU del proyecto delegan en LLM? ¿Hay CU que mencionan IA sin declarar prompt asociado?
- ¿Qué NB upstream motivan cada prompt?

### 5.2 Diseño del prompt

- ¿Con qué modelo y proveedor se evalúa primero? ¿Cuál es la alternativa documentada?
- ¿La tarea es clasificación, extracción, resumen, generación abierta o conversacional? La métrica crítica cambia según la familia de tarea.
- ¿La salida es consumida por una máquina (JSON) o por un humano (markdown)?
- ¿La tarea admite few-shot o requiere fine-tuning, RAG o herramientas externas? La categoría 04 cubre solo prompts; las otras opciones disparan ADR en 05.

### 5.3 Calidad

- ¿Cada prompt tiene un contrato de entrada y un contrato de salida declarados?
- ¿Cada prompt tiene mínimo dos few-shot? ¿Tres si es generación abierta?
- ¿Los casos de borde están enumerados con respuesta esperada?
- ¿Los umbrales numéricos están en `evaluacion-prompts` y se referencian desde el prompt?

### 5.4 Riesgos

- ¿Hay datos sensibles en la entrada? ¿Qué redacción se aplica antes del envío?
- ¿Qué pasa si el proveedor está caído? ¿El sistema degrada graciosamente?
- ¿Qué pasa si el costo unitario sube? ¿Hay budget alert y kill-switch declarados?
- ¿La salida puede generar daño si es incorrecta? Si sí, ¿hay validación humana antes del efecto?

### 5.5 Métricas críticas

- Para clasificación binaria o multi-clase: accuracy, precision/recall, F1.
- Para resumen: BLEU, ROUGE-L, validación humana muestral.
- Para extracción de entidades: exact match por campo, F1 a nivel token.
- Para generación abierta: tasa de cumplimiento del contrato de salida, validación humana muestral, métricas de adherencia al estilo.
- Transversales: latencia p99, costo por request, tasa de rechazo válido.

### 5.6 Versionado

- ¿El cambio que se está aplicando supera el 30% del contenido del prompt o modifica el contrato? Si sí, versión mayor.
- ¿Existe una versión anterior en la carpeta principal? Si sí, archivarla en `_legacy/` antes de publicar la nueva.

---

## 6. Criterios de aceptación

- [ ] La categoría se activa solamente si SOLUTION-INTAKE declara explícitamente que el sistema usa LLMs; caso contrario la carpeta no existe.
- [ ] Existe `politica-uso-ai_v1.0.md` antes de cualquier `prompt-<tarea>` productivo.
- [ ] Cada `prompt-<tarea>` declara contrato de entrada y contrato de salida (con JSON Schema si la salida es JSON).
- [ ] Cada `prompt-<tarea>` tiene mínimo dos few-shot; tres si la tarea es generación abierta.
- [ ] Cada `prompt-<tarea>` enumera casos de borde con respuesta esperada.
- [ ] Cada `prompt-<tarea>` declara costo esperado por request y latencia esperada p50 y p99.
- [ ] Existe `evaluacion-prompts_v1.0.md` con métricas, dataset referenciado, umbrales numéricos y frecuencia de re-evaluación.
- [ ] Existe `dataset-evaluacion_v1.0.md` versionado, salvo justificación documentada en `evaluacion-prompts`.
- [ ] Cada prompt declara trazabilidad NB→CU→ADR→tests en 08.
- [ ] Ningún archivo usa el patrón `.v<X.Y>.md`; todos usan `_v<X.Y>.md`.
- [ ] Ningún slug contiene mayúsculas, espacios, acentos ni caracteres no kebab.
- [ ] No coexisten versiones distintas del mismo nombre lógico en la carpeta principal; las superadas viven en `_legacy/`.

---

## 7. Ejemplos genéricos

Los ejemplos son ilustrativos y de dominios distintos al material fuente. No deben copiarse literalmente a ningún proyecto.

### 7.1 Ejemplo 1 — Fragmento de prompt para un web-monolith de mesa de ayuda

```markdown
# Prompt — Clasificación de tickets de soporte

**Proyecto:** {{nombre-proyecto}}
**Documento:** prompt-clasificacion-tickets-soporte_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-17
**Autor:** Equipo IA

## 1. Objetivo de la tarea
Clasificar cada ticket entrante en una categoría operativa para enrutarlo al equipo correcto. La métrica crítica es la accuracy de clasificación.

## 2. Modelo LLM target
Familia primaria: Claude Sonnet (Anthropic). Alternativa documentada: GPT-4o (OpenAI). Justificación: balance costo-latencia para tareas de clasificación corta.

## 4. Contrato de salida
JSON estricto:

`json
{
  "categoria": "facturacion | tecnico | cuenta | comercial | otro",
  "confianza": 0.0,
  "razonamiento_corto": ""
}
`

## 7. Casos de borde y manejo
| Caso | Respuesta esperada |
| --- | --- |
| Ticket vacío | `INPUT_VACIO` |
| Ticket en idioma no soportado | `IDIOMA_NO_SOPORTADO` |
| Ticket ambiguo | categoría `otro` con confianza < 0.5 |
```

### 7.2 Ejemplo 2 — Fragmento de prompt para un rest-api de procesamiento documental

```markdown
# Prompt — Resumen de actas legales

**Proyecto:** {{nombre-proyecto}}
**Documento:** prompt-resumen-documentos-legales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-17
**Autor:** Equipo IA

## 1. Objetivo de la tarea
Producir un resumen estructurado de un acta legal en español neutro, preservando partes, fechas, hechos y resoluciones. La métrica crítica es la cobertura de hechos relevantes verificada por revisión humana muestral, complementada con ROUGE-L sobre referencias.

## 2. Modelo LLM target
Familia primaria: Gemini 1.5 Pro (Google). Alternativa documentada: Claude Sonnet. Justificación: ventana de contexto amplia y costo por token competitivo para documentos largos.

## 4. Contrato de salida
Markdown estructurado con secciones fijas: Partes, Antecedentes, Hechos, Resolución, Plazos.

## 7. Casos de borde y manejo
| Caso | Respuesta esperada |
| --- | --- |
| Documento sin estructura legal reconocible | Devolver `FORMATO_NO_RECONOCIDO` |
| Documento con datos personales sin redactar | Aplicar redacción definida en `politica-uso-ai_v1.0.md` §2 y continuar |
| Documento truncado por límite de tokens | Devolver hasta donde alcanza y declarar `TRUNCADO=true` |
```

### 7.3 Ejemplo 3 — Fragmento de política de uso

```markdown
# Política de uso de AI

## 1. Cuándo usar AI y cuándo no
Se usa AI cuando la tarea es clasificación, extracción o resumen sobre texto no estructurado y la salida puede validarse con métricas objetivas. No se usa AI para decisiones con efecto legal irreversible sin validación humana previa.

## 2. Datos sensibles y privacidad
Antes de enviar al proveedor se redactan: identificadores personales, números de documento, datos financieros y de salud. La redacción se aplica del lado del cliente, no se confía en el proveedor.

## 7. Vendor lock-in
Proveedor primario: {{proveedor-1}}. Alternativa documentada: {{proveedor-2}}. Plan de portabilidad: contratos de entrada/salida estables, abstracción del cliente LLM en 05.
```

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de la categoría 04 (prompts AI) del proyecto {{NOMBRE_PROYECTO}}.

Precondición de gating:
- Antes de generar cualquier artefacto, verificá que SOLUTION-INTAKE declare explícitamente `usa_llm = true` en §17 P.11 o una NFR de IA en §17 P.10. Si no está declarado, NO generes la carpeta /SDD2.2D/docs/04_prompts_ai/ y dejá constancia en el log.

Insumos:
- SOLUTION-INTAKE: {{path}} (especialmente §17 P.10 NFR y §17 P.11 pre-ADR)
- Upstream: 01 (NB que motivan IA), 02 (CU que delegan en LLM si ya existe).

A generar (si la categoría está activa):
- politica-uso-ai_v1.0.md (obligatorio, primero).
- prompt-<tarea-kebab>_v1.0.md (uno por tarea LLM identificada).
- evaluacion-prompts_v1.0.md (obligatorio).
- dataset-evaluacion_v1.0.md (recomendado).
- README.md de la sección (recomendado).

Reglas de redacción: §4 de 04_rules_prompts_ai.md.
Nomenclatura: `prompt-<tarea-kebab>_v1.0.md` con `_v` (no `.v`); slug en minúsculas estricto.
Trazabilidad: cada prompt debe enlazar a una NB y a un CU consumidor.
Criterios de calidad: §6 de 04_rules_prompts_ai.md.
Política de versionado: §3.5; una sola versión vigente; anteriores a `_legacy/` con estado Superado.

Restricciones: no introducir stacks concretos, productos comerciales del dominio fuente ni protocolos específicos de impresión. Idioma rioplatense técnico, tildes correctas, sin emojis.

Salida: /SDD2.2D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/04_prompts_ai/<estructura>.
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.2. Categoría opcional con gating explícito por PROJECT-README. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/04_prompts_ai/` (cuando `usa_llm` del proyecto es true); la ruta de salida del prompt-snippet referencia el proyecto en curso del manifiesto. Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
