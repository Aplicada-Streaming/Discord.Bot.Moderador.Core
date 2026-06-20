# Reglas constructivas — 01 Necesidades de negocio

**Carpeta target:** `/SDD2.1D/docs/01_necesidades_negocio/`
**Subagente target del orquestador:** Analista de Negocio Senior (AG-01)
**Versión de las reglas:** 1.2

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Analista de Negocio Senior, equivalente al AG-01 del catálogo de especialidades. Su rol es traducir los dolores y oportunidades capturados en SOLUTION-INTAKE y en la visión del producto (categoría 00) en necesidades de negocio (NB) estructuradas, medibles y trazables hacia los casos de uso (CU) que las implementarán. El AG-01 trabaja a partir del intake y consolida cada NB en un archivo independiente bajo `/SDD2.1D/docs/01_necesidades_negocio/necesidades-de-negocio/`, además de mantener un índice maestro en la raíz de la categoría. Su responsabilidad no es definir flujos funcionales (eso es del AG-02) ni decisiones técnicas (AG-05), sino articular qué problema concreto del negocio se resuelve, para quién, con qué métrica de éxito y con qué prioridad relativa.

### 1.2 Variantes según tipo de proyecto

| Tipo de proyecto (D8) | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Analista de Negocio + Curador de API | El consumidor de una librería es un desarrollador; las NB deben articular dolores de integración, ergonomía de la API y compatibilidad. |
| web-monolith | Analista de Negocio Senior | Estructura clásica de levantamiento de necesidades organizacionales sobre un sistema único. |
| web-microservices | Analista de Negocio + Domain Modeler (DDD) | Cada NB debe poder asociarse a un bounded context candidato para evitar mezclar dominios. |
| desktop-app | Analista de Negocio + UX Researcher | Las NB de aplicaciones de escritorio dependen fuertemente del trabajo del usuario final con la interfaz. |
| mobile-app-maui | Analista de Negocio + Mobile Product Analyst | Las NB incorporan supuestos de movilidad, conectividad intermitente y permisos del dispositivo. |
| rest-api | Analista de Negocio + API Product Analyst | El consumidor primario es un sistema cliente; las NB deben hablar en términos de capacidades expuestas y contratos. |
| cli-tool | Analista de Negocio + Developer Experience Analyst | Las NB se redactan desde la mirada del desarrollador que invoca la herramienta en su pipeline. |
| worker-service | Analista de Negocio + Operations Analyst | Las NB se centran en throughput, latencia funcional y resiliencia operativa del proceso de fondo. |

El orquestador lee esta tabla y selecciona la variante según el `project_type` del proyecto principal de la solución (leído del manifiesto), porque esta categoría se genera una vez a nivel solución a partir del SOLUTION-INTAKE único.

### 1.3 Multi-especialidad

El AG-01 se combina sistemáticamente con dos roles aledaños. Con AG-00 (Product Manager) revisa que las NB estén alineadas con la visión y el alcance del producto, en especial cuando aparecen NB que no estaban anticipadas en el roadmap. Con AG-02 (Analista Funcional) coordina la trazabilidad downstream: cada NB declara las CU previstas y el AG-02 las desarrolla en la categoría 02. Cuando el proyecto incorpora capacidades de IA o LLMs, también se sincroniza con AG-04 (Ingeniero de Prompts) para que las NB que motivan prompts queden explícitas. La propiedad del documento permanece en AG-01; los otros roles aportan revisión y validación, no autoría compartida.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra de documentos

| Archivo | Obligatorio para | Recomendado para | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `necesidades-negocio_v1.0.md` | Todos los tipos D8 | — | — | Índice maestro de NB del proyecto, con tabla resumen, mapa de dependencias y trazabilidad agregada. |
| `necesidades-de-negocio/NB-XX-<kebab>_v1.0.md` | Todos los tipos D8 (mínimo 3 NB, máximo razonable 15) | — | — | Una necesidad de negocio por archivo, en subcarpeta dedicada. |
| `README.md` de la sección | Proyectos con más de 5 NB | Proyectos con 4 o 5 NB | Proyectos con 3 NB | Índice navegable de las NB, incluye RACI y orden de lectura sugerido. |

### 2.2 Reglas de inclusión/exclusión por tipo de proyecto

La categoría 01 es obligatoria para los 8 tipos D8: ningún proyecto avanza al diseño funcional sin tener al menos 3 NB documentadas. Lo que varía es el foco temático:

- `library`: las NB se redactan desde la mirada del desarrollador consumidor (consumibilidad, estabilidad de API, ergonomía).
- `web-monolith`: NB centradas en el flujo organizacional y la operación cotidiana del usuario final.
- `web-microservices`: cada NB se asocia a un bounded context candidato; evitar NB transversales sin dueño.
- `desktop-app`: NB con fuerte componente de experiencia de uso y compatibilidad con sistemas operativos del usuario.
- `mobile-app-maui`: NB que incluyen supuestos de conectividad y de uso en movilidad.
- `rest-api`: NB redactadas desde el sistema cliente que consume la API.
- `cli-tool`: NB que articulan el dolor en el pipeline de desarrollo o en la operación por línea de comandos.
- `worker-service`: NB que articulan la necesidad operativa de procesar volumen sin intervención humana.

Una NB se fusiona con otra cuando ambas tienen el mismo dolor central y se distinguen solo por el ejemplo. Una NB se divide en dos cuando se descubre que sus criterios de éxito apuntan a métricas distintas con públicos distintos.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

El índice se llama `necesidades-negocio_v1.0.md` (sin sufijo de proyecto). Cada necesidad va en un archivo `NB-XX-<kebab>_v<X.Y>.md` dentro de la subcarpeta `necesidades-de-negocio/`. Reglas estrictas:

- `XX` son dos dígitos, con cero a la izquierda si corresponde (`NB-01`, no `NB-1`).
- `<kebab>` es lowercase, palabras separadas por guion medio, sin acentos ni eñes en el filename (sí pueden aparecer en el cuerpo del documento).
- El separador entre el slug y la versión es guion bajo (`_v1.0`), nunca punto.
- La versión inicial es `_v1.0.md` y sigue D5.

Ejemplo válido: `NB-03-trazabilidad-pedidos_v1.0.md`.
Ejemplo inválido a evitar: `NB-03-Trazabilidad-Pedidos.v1.0.md` (mayúsculas en kebab y punto antes de la versión; éste es el patrón heredado del material fuente SDD 1.0 explícitamente registrado en la auditoría de Fase 0, §4, y se corrige por convención en SDD 2.1).

### 3.2 Convenciones de prefijos / sufijos

- Prefijo obligatorio: `NB-XX-`.
- Sufijo obligatorio: `_v<X.Y>.md` con dos componentes numéricos `mayor.menor`.
- Subcarpeta obligatoria para los archivos por necesidad: `necesidades-de-negocio/`.
- El índice maestro vive en la raíz de `01_necesidades_negocio/`, no dentro de la subcarpeta.
- Filename sin acentos, sin emojis, sin espacios, sin mayúsculas, sin caracteres especiales (excepto guion medio y guion bajo).

### 3.3 Vinculación cross-doc (trazabilidad upstream/downstream)

Upstream:

- SOLUTION-INTAKE: §1 (Idea y problema), §3 (Propuesta de valor), §4 (Alcance funcional con MoSCoW provisorio), §8 (Métricas de éxito), §13 (tipo D8) y §17 P.1 (stack) para condicionar variantes.
- 00_contexto: `vision-producto_v1.0.md` y `alcance-proyecto_v1.0.md` consolidan el marco previo.

Downstream:

- 02_especificacion_funcional: cada NB declara las CU previstas que la implementarán.
- 04_prompts_ai: si el proyecto declara IA o LLMs en SOLUTION-INTAKE, las NB que motivan prompts se enlazan a sus artefactos en esa categoría.
- 06_backlog-tecnico, 07_plan-sprint: la priorización MoSCoW de cada NB ordena el backlog y los sprints.
- 08_calidad_y_pruebas: cada criterio de éxito de §5 de la NB es input directo para los criterios de aceptación de la categoría 08.

La cadena de trazabilidad obligatoria por D6 es: SOLUTION-INTAKE → 00_contexto → NB → CU → US → BT → Sprint → Test → Pipeline.

### 3.4 README de la sección

El `README.md` de `01_necesidades_negocio/` se recomienda cuando hay más de 5 NB. Contiene como mínimo:

- Tabla `NB-XX / título / impacto / prioridad MoSCoW / estado / enlace al archivo`.
- Mapa de dependencias entre NB en formato lista o tabla.
- Orden de lectura sugerido si las NB tienen dependencias fuertes.
- RACI breve con propietario, implementador y revisor por NB.

Proyectos con 3 o 4 NB pueden prescindir del README y dejar al índice `necesidades-negocio_v1.0.md` como punto de entrada único.

---

## 4. Estructura de redacción de cada NB

### 4.1 Cabecera obligatoria

Cada archivo `NB-XX-<kebab>_v1.0.md` arranca con el siguiente bloque, completando los valores entre llaves dobles:

```markdown
# NB-XX — {{titulo-descriptivo}}

| Campo | Valor |
| --- | --- |
| Proyecto | {{nombre-proyecto}} |
| Documento | NB-XX-<kebab>_v1.0.md |
| Versión | 1.0 |
| Estado | Borrador / Propuesto / Aprobado / Vigente / Superado / Archivado |
| Fecha | YYYY-MM-DD |
| Autor | {{rol-y-nombre}} |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §4; vision-producto_v1.0.md |
| Trazabilidad downstream | CU-XX, CU-YY (previstas en 02_especificacion_funcional) |
```

El bloque equivalente para el índice `necesidades-negocio_v1.0.md` agrega los campos `Cantidad de NB` y `Versión del catálogo de NB`.

### 4.2 Secciones obligatorias del archivo `NB-XX-<kebab>_v1.0.md`

El cuerpo del documento debe contener, en este orden, las siguientes diez secciones numeradas:

1. Descripción de la necesidad: dos o tres párrafos que articulan el dolor concreto, qué pasa hoy y por qué importa. No debe leerse como solución técnica ni como historia de usuario.
2. Ejemplo de uso desde la perspectiva del negocio: un caso narrativo breve donde la necesidad se manifiesta, en lenguaje del cliente, sin tecnicismos.
3. Impacto: qué áreas del sistema o del negocio se ven afectadas si la necesidad se resuelve o si queda sin resolver. Se redacta como lista de impactos cualitativos.
4. Problema específico que resuelve: enumeración de los puntos de dolor concretos que la NB ataca. Cada punto en una línea, accionable.
5. Criterios de éxito: tabla con columnas `criterio / métrica / target / plazo`. Mínimo 4 filas, todas SMART, todas con número y unidad.
6. Stakeholders involucrados: tabla con `rol / nivel de involucramiento / qué pide o aporta`. Mínimo 3 filas, una por categoría (propietario, implementador, beneficiario).
7. Trazabilidad a CU: tabla con `NB-XX / CU-YY prevista / estado de la CU`. Estado posible: `a generar`, `en redacción`, `aprobada`.
8. Dependencias con otras NB: lista de NB de las que depende y de las que es prerequisito. Si la NB no tiene dependencias se declara explícitamente "Sin dependencias".
9. Prioridad MoSCoW: declaración explícita de `Must Have`, `Should Have`, `Could Have` o `Won't Have v1`, con una línea de justificación.
10. Control de cambios: tabla `versión / fecha / cambios`, con al menos la entrada inicial 1.0.

### 4.3 Secciones opcionales según tipo de proyecto

| Sección | Aplica a | Notas |
| --- | --- | --- |
| Implicancias de plataforma | mobile-app-maui, desktop-app, cli-tool | Listar supuestos de sistema operativo, versión mínima y restricciones de instalación. |
| Implicancias de SLA | rest-api, worker-service | Documentar throughput esperado, latencia funcional y ventanas de mantenimiento. |
| Implicancias de consumibilidad | library, cli-tool | Restricciones de compatibilidad de versiones y política de breaking changes vista desde la NB. |
| Implicancias de bounded context | web-microservices | Asociar la NB a un contexto delimitado y registrar el lenguaje ubicuo aplicable. |
| Implicancias de movilidad | mobile-app-maui | Comportamiento esperado con conectividad intermitente y modo offline. |

Estas secciones se numeran a partir de §11 y solo se incluyen si el tipo D8 las activa. Su omisión no es un defecto en otros tipos.

### 4.4 Tablas tipo y formatos recurrentes

Tabla A: Criterios de éxito.

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Tiempo de asignación de turno | Minutos promedio desde solicitud a confirmación | ≤ 2 min | 6 meses post-lanzamiento |
| Tasa de errores de doble asignación | Incidentes por mes | 0 | continuo |

Tabla B: Stakeholders involucrados.

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Director del centro de salud | Propietario | Aprueba prioridad y financiación |
| Equipo de desarrollo | Implementador | Construye y mantiene la solución |
| Recepcionista | Beneficiario | Valida que el flujo reduce su carga operativa |

Tabla C: Trazabilidad a CU.

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-01 | CU-01 reservar turno por especialidad | a generar |
| NB-01 | CU-02 confirmar turno por canal de mensajería | a generar |

Tabla D (solo para el índice maestro): Resumen de NB.

| ID | Necesidad | Prioridad MoSCoW | CU previstas | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Reducir tiempo de asignación de turno | Must | CU-01, CU-02 | Aprobado | [NB-01](necesidades-de-negocio/NB-01-reduccion-tiempo-asignacion-turno_v1.0.md) |

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| NB redactada como historia de usuario | El formato `Como rol quiero acción para valor` corresponde a US, no a NB; mezcla niveles de abstracción | Reescribir en términos de dolor del negocio: "La organización necesita reducir X porque hoy Y" |
| NB sin métrica numérica | El criterio de éxito no es verificable y se desliza al subjetivismo | Forzar cada fila de la tabla de criterios a tener número y unidad |
| NB que en realidad es un caso de uso | Describe un flujo paso a paso del sistema, no una necesidad del negocio | Mover el contenido a la categoría 02 y redactar la NB como problema |
| NB con dependencias circulares | NB-A depende de NB-B y viceversa; bloquea la planificación | Detectar el dolor común y fusionar o reordenar; nunca admitir ciclos |
| NB con más de 3 dependencias upstream | Señal de mal recorte: probablemente esa NB debió fusionarse o partirse | Revisar si las dependencias son reales o si la NB es demasiado granular |
| NB con título en mayúsculas o con punto antes de la versión | Rompe D3 y D4; reaparece el patrón heredado del fuente SDD 1.0 | Validar con regex `^NB-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$` antes de aceptar el filename |
| NB con stakeholders genéricos como "el equipo" | Sin accountability concreto, la NB no tiene quién la valide | Nombrar rol específico (Product Owner, Recepcionista, Auditor) en cada fila |
| NB sin prioridad MoSCoW declarada | El backlog no sabe en qué orden atacar las NB | Forzar §9 con valor explícito y justificación de una línea |

---

## 5. Preguntas guía para el subagente

### 5.1 Comprensión del input upstream

- ¿Qué dolor concreto describe SOLUTION-INTAKE §1 y cómo se mapea a una o más NB?
- ¿Qué capacidades de SOLUTION-INTAKE §4 marcadas como Must Have requieren una NB explícita y cuáles son técnicas o derivadas?
- ¿Qué métricas de éxito de SOLUTION-INTAKE §8 se traducen en criterios de éxito de una NB específica?
- ¿La visión de producto de la categoría 00 introduce dolores nuevos no capturados en el intake?
- ¿Hay riesgos de SOLUTION-INTAKE §11 que motivan NB defensivas (por ejemplo, NB de auditabilidad)?

### 5.2 Decisiones de scope

- ¿Cuántas NB tiene este proyecto y por qué? Si superan 15, justificar o agrupar.
- ¿Hay dos NB que comparten dolor y se pueden fusionar?
- ¿Hay una NB que mezcla dos dolores con públicos distintos y conviene partir?
- ¿Las NB están al nivel correcto de abstracción (problema de negocio) y no descendiendo a flujo o a tarea técnica?

### 5.3 Trazabilidad

- ¿Cada NB declara qué CU la implementarán, aunque la CU todavía no esté redactada?
- ¿Cada criterio de éxito de §5 es medible desde la categoría 08 con un caso de prueba?
- ¿Las dependencias entre NB están explícitas y son acíclicas?
- ¿El índice maestro `necesidades-negocio_v1.0.md` referencia todas las NB con paths correctos a la subcarpeta?

### 5.4 Calidad

- ¿Cada NB tiene mínimo 4 criterios de éxito SMART con número, unidad y plazo?
- ¿Cada NB tiene prioridad MoSCoW explícita y justificada?
- ¿Ninguna NB depende de más de 3 otras NB?
- ¿Los stakeholders son nominales (rol específico), no genéricos?
- ¿Los filenames respetan el patrón `NB-XX-<kebab>_v1.0.md` sin punto antes de la versión y sin mayúsculas en el kebab?
- ¿Se respetó D1 (rioplatense técnico, sin emojis, sin negritas decorativas)?
- ¿Se evitó vocabulario del dominio prohibido por D7 (DSL, ESC-POS, MAUI literal fuera del valor del tipo D8, Bluetooth, NuGet, .NET 10, impresora térmica)?

---

## 6. Criterios de aceptación del entregable

- [ ] Existe el índice maestro `necesidades-negocio_v1.0.md` en la raíz de `/SDD2.1D/docs/01_necesidades_negocio/` con tabla resumen de todas las NB.
- [ ] Existen al menos 3 archivos `NB-XX-<kebab>_v1.0.md` dentro de la subcarpeta `necesidades-de-negocio/`.
- [ ] Cada NB contiene las 10 secciones obligatorias §1 a §10 en el orden definido en §4.2 de estas reglas.
- [ ] Cada NB tiene al menos 4 criterios de éxito SMART en §5, todos con métrica numérica, target y plazo.
- [ ] Cada NB declara su prioridad MoSCoW en §9 con justificación de una línea.
- [ ] Cada NB tiene trazabilidad upstream explícita a SOLUTION-INTAKE y/o a documentos de 00_contexto.
- [ ] Cada NB declara en §7 las CU previstas que la implementarán, con estado `a generar`, `en redacción` o `aprobada`.
- [ ] Cada NB tiene al menos 3 stakeholders nominales en §6, cubriendo las categorías propietario, implementador y beneficiario.
- [ ] Ningún filename usa `.v` antes de la versión ni mayúsculas en el kebab; todos respetan el regex `^NB-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$`.
- [ ] El índice maestro referencia las NB con paths relativos correctos hacia la subcarpeta y todos los enlaces resuelven.
- [ ] Ninguna NB depende de más de 3 otras NB en §8 ni existen ciclos de dependencias.
- [ ] Si el proyecto tiene más de 5 NB, existe el `README.md` de la sección con la tabla de §3.4 completa.
- [ ] El estado declarado en la cabecera de cada NB pertenece al enum cerrado: Borrador, Propuesto, Aprobado, Vigente, Superado o Archivado.
- [ ] No aparecen emojis, negritas decorativas, ni términos del dominio prohibido por D7.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo A: NB-01 de un sistema de turnos médicos (web-monolith)

```markdown
# NB-01 — Reducción del tiempo de asignación de turno

| Campo | Valor |
| --- | --- |
| Proyecto | turnos-medicos |
| Documento | NB-01-reduccion-tiempo-asignacion-turno_v1.0.md |
| Versión | 1.0 |
| Estado | Aprobado |
| Fecha | 2026-04-10 |
| Autor | Analista de Negocio, Centro de Salud Norte |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §4; vision-producto_v1.0.md |
| Trazabilidad downstream | CU-01, CU-02 (previstas) |

## 1. Descripción de la necesidad

La organización necesita reducir el tiempo que un paciente espera para obtener
un turno con un profesional, hoy condicionado por un canal telefónico saturado
y por la disponibilidad limitada del personal de recepción. La consecuencia
actual son turnos perdidos, sobrecarga del personal y reclamos crecientes.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un paciente que necesita un turno con cardiología llama al consultorio,
espera entre quince y treinta minutos para ser atendido y luego es derivado
a una segunda llamada porque la recepcionista no tiene la agenda del médico.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Tiempo de asignación | Minutos promedio | ≤ 2 min | 6 meses |
| Tasa de doble asignación | Incidentes por mes | 0 | continuo |
| Adopción del canal online | Turnos online sobre total | ≥ 40 % | 6 meses |
| Satisfacción del paciente | Escala 1 a 5 en encuesta | ≥ 4.0 | trimestral |

## 9. Prioridad MoSCoW

Must Have. Sin esta NB, el sistema no resuelve el problema central declarado
en SOLUTION-INTAKE §1 y no hay MVP defendible.
```

### 7.2 Ejemplo B: NB-01 de una librería de parsing CSV (library)

```markdown
# NB-01 — Extensibilidad del esquema de parsers

| Campo | Valor |
| --- | --- |
| Proyecto | csv-parser-lib |
| Documento | NB-01-extensibilidad-parsers_v1.0.md |
| Versión | 1.0 |
| Estado | Aprobado |
| Fecha | 2026-04-22 |
| Autor | Curador de API, equipo de plataforma |
| Trazabilidad upstream | SOLUTION-INTAKE §3, §4 |
| Trazabilidad downstream | CU-04, CU-05 (previstas) |

## 1. Descripción de la necesidad

Los equipos consumidores necesitan extender el parser con tipos de columna
personalizados sin modificar el código fuente de la librería ni recompilarla,
porque cada pipeline de ingesta tiene reglas de tipado propias.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Tiempo de integrar un tipo nuevo | Minutos por desarrollador | ≤ 30 min | 3 meses |
| Cobertura de tipos primitivos | Tipos soportados de fábrica | ≥ 10 | release 1.0 |
| Estabilidad de la API pública | Breaking changes por minor | 0 | continuo |
| Adopción del mecanismo | Proyectos internos usándolo | ≥ 3 | 6 meses |

## 9. Prioridad MoSCoW

Must Have. La promesa de valor de la librería es justamente extensibilidad
sin recompilación; sin esta NB la librería pierde su diferenciador.
```

Ambos ejemplos respetan filename en lowercase, separador de versión con guion bajo, criterios SMART con cifras y prioridad MoSCoW explícita. La estructura es la misma para los demás tipos D8; cambia el dominio del problema y la audiencia, no el esqueleto.

---

## 8. Prompt-snippet sugerido para el subagente

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar las necesidades de negocio del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path-al-intake}}
- Documentos upstream ya generados: vision-producto_v1.0.md, alcance-proyecto_v1.0.md (categoría 00).

Documentos a generar:
- necesidades-negocio_v1.0.md (índice maestro en la raíz de la categoría).
- NB-XX-<kebab>_v1.0.md (uno por necesidad, mínimo 3, en subcarpeta necesidades-de-negocio/).
- README.md de la sección si el proyecto tiene más de 5 NB.

Reglas de redacción: §4 de 01_rules_necesidades_negocio.md.
Nomenclatura: respetar el patrón NB-XX-<kebab-lowercase>_v1.0.md.
Trazabilidad esperada: cada NB declara las CU previstas en 02_especificacion_funcional con estado a generar / en redacción / aprobada.
Criterios de calidad: §6 de 01_rules_necesidades_negocio.md (14 ítems verificables).
Restricciones: respetar D1 a D8; no incluir emojis, negritas decorativas, ni términos del dominio prohibido por D7.

Salida: /SDD2.1D/docs/01_necesidades_negocio/<archivos>.
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.1 |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría 01 se genera a nivel solución; la frase de cierre de §1.2 usa la variante del proyecto principal del manifiesto. Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
