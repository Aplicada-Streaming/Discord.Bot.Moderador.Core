# Reglas constructivas — 02 Especificación funcional

**Carpeta target (por proyecto):** `/SDD2.2D/docs/proyectos/<nombre-proyecto-kebab>/02_especificacion_funcional/`
**Subagente target del orquestador:** Analista Funcional / Ingeniero de Requisitos (AG-02)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.2

La categoría 02 es el nodo central de la cadena de trazabilidad D6. Recibe insumos de 00 (visión, alcance) y 01 (necesidades de negocio NB-XX) y produce los artefactos que sirven como ancla para 03 (UX/UI), 04 (prompts si aplica), 05 (arquitectura y ADR), 06 (US/BT), 07 (sprints), 08 (tests) y 11 (ejemplos). Su salida define el qué del sistema sin invadir el cómo. Aplica de manera obligatoria a los ocho tipos D8.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Analista Funcional senior, equivalente AG-02 del catálogo SDD. Su perfil profesional combina elicitación, formalización y modelado de requisitos. Traduce cada NB-XX en uno o más casos de uso (CU) verificables, identifica las reglas de negocio (RN) que restringen el dominio y, cuando aplica, levanta el modelo conceptual de datos junto con sus reglas conceptuales (RC). Se alinea con IREB CPRE para elicitación y gestión, con formato de casos de uso al estilo Cockburn y con criterios de aceptación en BDD (Given/When/Then).

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Analista Funcional + API Designer | El foco está en la superficie pública. Cada CU describe un contrato de uso. |
| web-monolith | Analista Funcional senior | Flujos UI/UX-driven, con CU que cruzan presentación, dominio y persistencia. |
| web-microservices | Analista Funcional + Domain Modeler (DDD) | Bounded contexts, aggregates, eventos de dominio. CU acotado por servicio. |
| desktop-app | Analista Funcional + UX Researcher | Flujos offline-first, sincronización diferida, criterios de continuidad. |
| mobile-app-maui | Analista Funcional + Mobile UX Analyst | Flujos táctiles, permisos del sistema operativo y ciclo de vida de la app. |
| rest-api | Analista Funcional + API Designer (OpenAPI/AsyncAPI) | Cada endpoint público es un CU con contrato declarativo y códigos de error. |
| cli-tool | Analista Funcional + CLI UX | Comandos, subcomandos y flags como CU, con exit codes definidos. |
| worker-service | Analista Funcional + Event Modeler | Mensajes y triggers como entrada de cada CU; idempotencia explícita. |

### 1.3 Multi-especialidad

La categoría 02 se combina con otras especialidades cuando el dominio lo requiere:

- AG-05 Arquitecto, en proyectos con DDD o con bounded contexts múltiples, para alinear el modelo conceptual con el modelo lógico que se produce en 05.
- AG-04 Ingeniero de Prompts, cuando algún CU delega parte del flujo en un LLM (clasificación, extracción, generación), para fijar contratos de prompt y criterios de evaluación.
- AG-03 DX/UX, cuando el CU describe interacción humana significativa, para que las decisiones de experiencia no contaminen el flujo funcional.
- AG-08 QA, para revisar que cada criterio Given/When/Then sea automatizable y trazable a un test en 08.

El AG-02 mantiene siempre la titularidad del artefacto; las demás especialidades aportan revisiones acotadas.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra

| Archivo | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `especificacion-funcional_v1.0.md` | Todos los tipos D8 | — | — | Índice maestro de CU, RN y referencia al modelo. Incluye matriz NB→CU→RN→US. |
| `definicion-<concepto-central>_v1.0.md` | Proyectos con un concepto técnico central (por ejemplo lenguaje declarativo, modelo de pagos, taxonomía de eventos) | library con superficie estrecha | Tipos sin concepto central | Define vocabulario, semántica y elementos del concepto en un único documento. |
| `casos-de-uso/CU-XX-<kebab>_v1.0.md` | Todos los tipos D8, con un mínimo declarado en §2.2 | — | — | Un caso de uso por archivo. |
| `reglas-de-negocio/RN-XX-<kebab>_v1.0.md` | Proyectos con reglas regulatorias o dominio fuerte | Resto de los proyectos | Proyectos triviales sin estado ni invariantes | Una regla de negocio por archivo. Invariante atemporal. |
| `modelo-datos/modelo-conceptual_v1.0.md` | Proyectos con persistencia (web-monolith, web-microservices, rest-api, worker-service, mobile-app-maui) | desktop-app si guarda local | library puro sin estado, cli-tool sin estado | Modelo conceptual estilo ER o de clases de dominio. |
| `modelo-datos/reglas-conceptuales-de-modelo/RC-XX-<kebab>_v1.0.md` | Proyectos con modelo rico (más de diez entidades o invariantes de integridad explícitas) | — | Resto | Una regla conceptual por archivo, focalizada en integridad de dominio. |
| `README.md` de la sección | Recomendado para todos | — | — | Índice navegable de CU, RN, modelo y RC con su estado actual. |

### 2.2 Reglas de inclusión y exclusión por tipo

| Tipo D8 | Mínimo de CU | RN obligatorias | Modelo conceptual | RC obligatorias |
| --- | --- | --- | --- | --- |
| library | 5 | No (recomendadas si hay reglas de dominio en la API) | No | No |
| web-monolith | 8 | Sí | Sí | Solo si modelo > 10 entidades |
| web-microservices | 6 por bounded context | Sí | Sí, uno por contexto | Solo si modelo > 10 entidades por contexto |
| desktop-app | 6 | Sí | Recomendado si guarda local | No |
| mobile-app-maui | 6 | Sí | Sí, si hay almacenamiento offline | No |
| rest-api | 1 por recurso público + 5 transversales | Sí | Sí | Sí, si modelo > 10 entidades |
| cli-tool | 1 por comando + 3 transversales | Recomendadas | No (salvo persistencia local) | No |
| worker-service | 1 por tipo de mensaje + 3 transversales | Sí | Sí, si guarda estado | Sí, si modelo > 10 entidades |

El mínimo es piso, no techo. La cota superior queda definida por la cobertura completa de las NB-XX declaradas en 01.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `CU-XX-<kebab-lowercase>_v<X.Y>.md`, con dos dígitos en `XX`, kebab estricto en el slug y guion bajo antes de la versión.
- `RN-XX-<kebab-lowercase>_v<X.Y>.md`, mismas reglas.
- `RC-XX-<kebab-lowercase>_v<X.Y>.md` para reglas conceptuales del modelo.
- `modelo-conceptual_v<X.Y>.md` para el modelo conceptual de datos.
- `definicion-<concepto>_v<X.Y>.md` para el documento opcional de concepto central.
- `especificacion-funcional_v<X.Y>.md` para el índice maestro.

Queda prohibido el patrón heredado `NB-01-Desacople.v1.0.md` u homólogos. La versión siempre va con `_v`, jamás con `.v`. El slug siempre va en minúsculas; queda prohibido `CU-01-Cargar-Plantilla_v1.0.md` o variantes con mayúsculas o camelCase.

### 3.2 Convenciones de prefijos y sufijos

- `CU-`: caso de uso. Acción funcional con flujo, actores y criterios de aceptación.
- `RN-`: regla de negocio. Invariante atemporal del dominio. No describe acción; describe restricción.
- `RC-`: regla conceptual del modelo. Restricción de integridad expresada sobre entidades y relaciones.
- Sufijo `_v<X.Y>.md` uniforme. La versión menor avanza por aclaración o corrección; la mayor avanza por cambio de alcance del CU/RN/RC.

### 3.3 Vinculación cross-doc

- Upstream: cada CU declara una o más NB que implementa. No puede existir CU huérfano. La revisión de cobertura es bidireccional: cada NB debe tener al menos un CU y cada CU debe tener al menos una NB.
- Downstream: cada CU enumera las US que se generarán en 06, los componentes esperados en 05 y los tests previstos en 08.
- RN es atemporal: no cambia con la versión del producto. Si cambia, la versión nueva de la RN se publica y la anterior se archiva (ver §3.5).
- RC se vincula al modelo conceptual y a las RN o CU que la justifican.
- El modelo conceptual se vincula a CU y RN, no al revés. Es el AG-02 quien decide qué CU lo consumen.

### 3.4 README de la sección

Recomendado para todos los tipos. Debe listar CU, RN, modelo y RC vigentes con propósito en una línea y estado actual. Sirve como punto de entrada navegable para revisores externos (AG-05, AG-06, AG-08).

### 3.5 Política de versionado

Una sola versión vigente por nombre lógico. Cuando un CU pasa de `v1.0` a `v2.0`:

1. Se crea `CU-XX-<kebab>_v2.0.md` en la carpeta principal.
2. La versión `v1.0` se mueve a `casos-de-uso/_legacy/` con estado `Superado` y una nota al inicio que apunte a la versión vigente.
3. El índice `especificacion-funcional_v<X.Y>.md` referencia únicamente la versión vigente.
4. Las RN, RC y referencias downstream se actualizan en la misma operación.

Queda prohibido mantener `v1.0` y `v2.0` conviviendo en la carpeta principal sin marcado de deprecación. La lección está documentada en `_bootstrap/audit-sdd1.md` Fase 0.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada artefacto inicia con un H1 y un bloque markdown de metadatos:

```markdown
# CU-XX — <Nombre del caso de uso>

**Proyecto:** {{nombre-proyecto}}
**Documento:** CU-XX-<kebab>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Propuesto | Aprobado | Vigente | Superado | Archivado
**Fecha:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
```

Para RN, RC y modelo conceptual aplica la misma cabecera adaptando el prefijo y la descripción del documento.

### 4.2 Secciones obligatorias de un CU

1. Propósito. Una a tres oraciones que respondan qué resuelve el CU y para quién.
2. Actores. Tabla con actor, tipo (primario/secundario/sistema) y rol.
3. Precondiciones. Lista de condiciones previas verificables.
4. Flujo principal. Pasos numerados con actor y acción.
5. Flujos alternativos. Cada uno con su disparador y punto de retorno.
6. Excepciones y errores. Cada error con código, causa y respuesta del sistema.
7. Postcondiciones. Estado del sistema en caso de éxito y de fallo.
8. Criterios de aceptación Given/When/Then. Mínimo tres por CU, con valores concretos.
9. Trazabilidad. Tabla con NB upstream, RN aplicables, US a generar en 06, componentes en 05, tests en 08.
10. Notas y supuestos. Decisiones explícitas de alcance y dependencias externas.
11. Control de cambios. Tabla con versión, fecha y descripción.

#### 4.2.1 Secciones obligatorias de una RN

1. Enunciado de la regla. Formal, declarativo, una sola oración cuando se pueda.
2. Justificación. Origen regulatorio, contractual o de negocio.
3. Ámbito de aplicación. Procesos, CU y momentos en los que la regla se evalúa.
4. Consecuencia si se viola. Rechazo, advertencia, compensación.
5. CU afectados. Lista explícita.
6. Pruebas que la verifican. Referencia a casos de prueba previstos en 08.
7. Control de cambios.

#### 4.2.2 Secciones obligatorias del modelo conceptual

1. Entidades. Una por subsección, con propósito y ejemplo de instancia.
2. Atributos clave. Sin tipos físicos (eso vive en 05). Solo nombre y semántica.
3. Relaciones. Verbalizadas en lenguaje natural.
4. Cardinalidades. Notación uniforme (1, N, 0..1, 1..N).
5. Reglas conceptuales. Enlace a cada RC-XX que el modelo invoca.
6. Glosario. Términos del dominio reutilizados por toda la categoría 02.
7. Diagrama. Mermaid embebido o referencia a archivo aparte.
8. Trazabilidad. Tabla que liga cada entidad a los CU y RN que la consumen.
9. Control de cambios.

#### 4.2.3 Secciones obligatorias de una RC

1. Enunciado. Restricción de integridad expresada sobre entidades y relaciones.
2. Entidades involucradas. Lista explícita.
3. Tipo de restricción. Identidad, referencial, cardinalidad, valor permitido, derivación.
4. Mecanismo de verificación conceptual. Cómo se comprueba sin entrar en detalles físicos.
5. RN o CU que la justifican.
6. Control de cambios.

### 4.3 Secciones opcionales por tipo de proyecto

Se permiten secciones adicionales según el tipo D8, sin desplazar las obligatorias:

- §12 Performance esperado del CU, sólo para rest-api, worker-service y mobile-app-maui.
- §13 Interacción multiusuario y concurrencia, sólo para web-monolith y web-microservices.
- §14 Permisos del sistema operativo, sólo para mobile-app-maui y desktop-app.
- §15 Idempotencia y reintento, sólo para worker-service y rest-api.
- §16 Contrato de stdout/stderr y exit codes, sólo para cli-tool.
- §17 Compatibilidad de versión pública, sólo para library.

### 4.4 Tablas tipo y formatos recurrentes

Se estandariza el uso de las siguientes tablas:

Tabla de criterios de aceptación:

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | <contexto> | <acción> | <resultado verificable> |

Tabla de trazabilidad del CU:

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-XX |
| Reglas de negocio aplicables | RN-XX, RN-YY |
| Historias de usuario a generar | US-XX (en 06) |
| Componentes esperados | <referencia tentativa a 05> |
| Tests previstos | <referencia tentativa a 08> |

Tabla del modelo conceptual:

| Entidad | Atributo | Semántica | Restricción conceptual |
| --- | --- | --- | --- |

Diagrama de flujo del CU expresado como pasos numerados o Mermaid sequenceDiagram cuando el flujo lo amerite.

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| CU que mezcla flujo funcional con detalle de interfaz | Invade 03 UX/UI y vuelve frágil el CU | Mover el detalle visual a 03; mantener en 02 sólo qué hace y para quién |
| RN escrita como CU | Confunde acción con invariante; rompe la trazabilidad | Reescribir como enunciado declarativo atemporal |
| Modelo conceptual con tipos físicos (varchar(255), int(11)) | Invade 05 modelo lógico | Mantener nombres y semántica; los tipos viven en 05 |
| Versionado paralelo `CU-06_v1.0` y `CU-06_v2.0` sin marcar deprecación | Ambigüedad sobre cuál vigencia leer | Aplicar §3.5: una vigente, anteriores a `_legacy/` |
| Casing inconsistente (`NB-01-Desacople` vs `NB-02-estandarizacion`) | Inconsistencia que rompe automatizaciones | Forzar lowercase + kebab estricto |
| Patrón `.v1.0` heredado del fuente | Convención prohibida en SDD 2.2 | Forzar `_v1.0` |
| CU sin escenarios de error | Solo flujo feliz; el sistema queda subdefinido | Agregar al menos una excepción por CU |
| Criterios de aceptación narrativos sin valores concretos | No automatizables; no anclan tests | Reescribir Given/When/Then con valores explícitos |
| CU con más de un actor primario | Ambigüedad de responsabilidad | Separar en dos CU o reorganizar el flujo |
| RN ambigua o subjetiva | No verificable | Reescribir hasta que un test pueda decidir cumplimiento |
| Numeración no contigua de CU sin justificación | Huecos confusos en el catálogo | Documentar la causa o renumerar |

---

## 5. Preguntas guía para el subagente

### 5.1 Upstream

- ¿Cada CU cubre al menos una NB del proyecto? ¿Hay NB sin CU? Esto último debe disparar una alerta y no un silencio.
- ¿Qué partes de la visión y del alcance (00) y de las NB (01) se materializan como CU? ¿Cuáles quedan deliberadamente fuera y por qué?
- ¿Existe un concepto técnico central que merezca un documento `definicion-<concepto>` propio?

### 5.2 Scope

- ¿Cuántos CU son razonables para el tipo D8 del proyecto? Library con menos de diez; microservicios con más de quince repartidos por bounded context.
- ¿Hay CU que en realidad son sub-flujos de otros? Si la respuesta es sí, fusionarlos o anidarlos como flujo alternativo.
- ¿Hay flujos de error que se repiten en varios CU? Si la respuesta es sí, considerar un CU transversal de manejo de errores.

### 5.3 Trazabilidad

- ¿Cada CU declara explícitamente su NB upstream?
- ¿Cada CU enumera RN que lo restringen, sin inventarlas?
- ¿Cada CU lista las US que se realizarán en 06, sin invadir el detalle de implementación?
- ¿El modelo conceptual referencia los CU y RN que lo justifican?

### 5.4 Calidad

- ¿Cada CU tiene mínimo tres criterios Given/When/Then con valores concretos?
- ¿Cada flujo alternativo tiene un escenario disparador y un punto de retorno explícito?
- ¿Cada excepción declara recuperación, handoff o terminación controlada?
- ¿Cada RN es verificable por un test automatizable?

### 5.5 Versionado

- ¿Existe alguna versión anterior en la carpeta principal? Si la respuesta es sí, archivarla en `_legacy/` antes de publicar la nueva.
- ¿El índice maestro apunta solamente a la versión vigente?

---

## 6. Criterios de aceptación

- [ ] Existe `especificacion-funcional_v1.0.md` con índice maestro y matriz NB→CU→RN→US.
- [ ] La cantidad de CU cumple el mínimo declarado para el tipo D8 del proyecto.
- [ ] Cada CU contiene las once secciones obligatorias del §4.2.
- [ ] Cada CU declara trazabilidad NB→CU→US y al menos tres criterios Given/When/Then con valores concretos.
- [ ] Cada RN contiene las siete secciones obligatorias del §4.2.1 y enumera CU afectados explícitos.
- [ ] Si el tipo D8 exige modelo de datos, existe `modelo-datos/modelo-conceptual_v1.0.md` con diagrama o tabla equivalente.
- [ ] Si el modelo supera diez entidades, existen RC-XX en `modelo-datos/reglas-conceptuales-de-modelo/` con las seis secciones obligatorias del §4.2.3.
- [ ] Ningún archivo usa el patrón `.v<X.Y>.md`; todos usan `_v<X.Y>.md`.
- [ ] Ningún slug contiene mayúsculas, espacios, acentos ni caracteres no kebab.
- [ ] No coexisten versiones distintas del mismo nombre lógico en la carpeta principal; las superadas viven en `_legacy/`.
- [ ] No hay menciones a stacks concretos, productos comerciales ni protocolos específicos del dominio fuente.
- [ ] Existe `README.md` de la sección si así lo decide el equipo (recomendado).

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — Fragmento de CU para un web-monolith de salud

Fragmento ilustrativo, no documento completo:

```markdown
# CU-03 — Asignar turno médico

**Proyecto:** {{nombre-proyecto}}
**Documento:** CU-03-asignar-turno-medico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-17
**Autor:** Equipo Funcional

## 1. Propósito
Permitir que un agente administrativo asigne un turno a un paciente sobre la agenda de un profesional, respetando los cupos y las reglas de superposición.

## 2. Actores
| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente administrativo | Primario | Solicita la asignación |
| Sistema de agendas | Sistema | Valida cupos y reserva el horario |
| Paciente | Secundario | Recibe la confirmación |

## 8. Criterios de aceptación
| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una agenda con cupo libre a las 10:00 | El agente solicita asignar a las 10:00 | El sistema reserva el turno y emite confirmación |
| CA-02 | Una agenda sin cupo libre a las 10:00 | El agente solicita asignar a las 10:00 | El sistema rechaza la solicitud con el código TURNO_SIN_CUPO |
| CA-03 | Un paciente con turno superpuesto vigente | El agente solicita asignar | El sistema rechaza con el código PACIENTE_SOLAPADO |

## 9. Trazabilidad
| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-02, RN-04 |
| Historias de usuario | US-05, US-06 |
```

### 7.2 Ejemplo 2 — Fragmento de RN para un rest-api de pagos

```markdown
# RN-02 — Validez del identificador de pago

**Proyecto:** {{nombre-proyecto}}
**Documento:** RN-02-validez-identificador-pago_v1.0.md
**Versión:** 1.0
**Estado:** Aprobada
**Fecha:** 2026-05-17

## 1. Enunciado
Todo identificador de pago aceptado por el sistema debe ser único, opaco al cliente y emitido por el subsistema autorizado.

## 2. Justificación
Auditoría regulatoria del proceso de cobro. Evita colisiones de cobro y permite reconciliación trazable.

## 3. Ámbito de aplicación
- CU-01 Registrar intento de pago.
- CU-04 Confirmar pago.
- CU-07 Reembolsar pago.

## 4. Consecuencia si se viola
El sistema rechaza la operación con el código IDENTIFICADOR_INVALIDO y no registra el intento.

## 5. CU afectados
CU-01, CU-04, CU-07.
```

### 7.3 Ejemplo 3 — Fragmento del modelo conceptual para un worker-service

```markdown
# Modelo conceptual

## 3.1 Mensaje recibido
Representa cada mensaje entrante procesado por el worker.

**Atributos clave**
- IdentificadorMensaje
- TipoEvento
- FechaRecepcion
- EstadoProcesamiento

## 4.1 MensajeRecibido — ResultadoProcesamiento
Un mensaje recibido produce uno y solo un resultado de procesamiento.
**Cardinalidad:** MensajeRecibido (1) —— (1) ResultadoProcesamiento

## 5. Reglas conceptuales
- RC-01: IdentificadorMensaje es único en toda la historia del worker (idempotencia).
- RC-02: Todo MensajeRecibido referencia un TipoEvento del catálogo vigente.
```

Los tres fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar la especificación funcional del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path}}
- Upstream: 00 (visión, alcance), 01 (NB-XX).

A generar (según tipo {{TIPO}} de D8):
- especificacion-funcional_v1.0.md (índice maestro con matriz NB→CU→RN→US).
- casos-de-uso/CU-XX-<kebab>_v1.0.md (mínimo según §2.2).
- reglas-de-negocio/RN-XX-<kebab>_v1.0.md (si aplica).
- modelo-datos/modelo-conceptual_v1.0.md (si aplica).
- modelo-datos/reglas-conceptuales-de-modelo/RC-XX-<kebab>_v1.0.md (si el modelo supera 10 entidades).
- README.md de la sección (recomendado).

Reglas de redacción: §4 de 02_rules_especificacion_funcional.md.
Nomenclatura: `CU-XX-<kebab-lowercase>_v1.0.md` con `_v` (no `.v`); slug en minúsculas estricto.
Trazabilidad: cada CU debe enlazar a una NB y enumerar US a generar en 06.
Criterios de calidad: §6 de 02_rules_especificacion_funcional.md.
Política de versionado: §3.5; una sola versión vigente; anteriores a `_legacy/` con estado Superado.

Restricciones: no introducir stacks concretos, productos comerciales ni protocolos del dominio fuente. Idioma rioplatense técnico, tildes correctas, sin emojis.

Salida: /SDD2.2D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/02_especificacion_funcional/<estructura>.
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.2. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/02_especificacion_funcional/`; la frase de cierre de §1.2 y la ruta de salida del prompt-snippet referencian el `project_type` del proyecto en curso (manifiesto). Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
