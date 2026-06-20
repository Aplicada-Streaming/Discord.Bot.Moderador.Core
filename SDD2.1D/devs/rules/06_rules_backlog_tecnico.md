# Reglas constructivas — 06 Backlog técnico

**Carpeta target (por proyecto):** `/SDD2.1D/docs/proyectos/<nombre-proyecto-kebab>/06_backlog-tecnico/`
**Subagente target del orquestador:** Scrum Master / Agile Coach senior (AG-06)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.1

La categoría 06 es la bisagra entre el diseño y la ejecución dentro de la cadena de trazabilidad D6. Recibe upstream de 01 (necesidades de negocio NB-XX), 02 (CU, RN, modelo conceptual) y 05 (arquitectura de solución, ADRs y modelo lógico). Produce los artefactos que alimentan 07 (sprint plan), 08 (acceptance tests y QA) y 10 (developer guide cuando se trata de onboarding al backlog). Su salida define qué se construye y en qué orden, sin entrar en el detalle de planificación temporal por sprint, que vive en 07. Aplica de manera obligatoria a los ocho tipos D8: ningún tipo está exento de producir un product backlog priorizado, un backlog técnico y una Definition of Ready acotada.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Scrum Master con perfil de Agile Coach orientado al backlog, equivalente al AG-06 del catálogo SDD. Su perfil profesional combina la facilitación ágil con la curaduría del backlog: asegura que las historias de usuario cumplan INVEST, que la priorización MoSCoW refleje el valor real de negocio, que cada historia trace a uno o más CU y que la Definition of Ready filtre trabajo no refinado antes del Sprint Planning. Conecta la capa funcional (CU y RN de 02) y la capa técnica (componentes y ADRs de 05) con el trabajo planificable en sprints. Se alinea con la Scrum Guide 2020 para los artefactos de producto, con el método MoSCoW (DSDM Atern) para priorización y con la práctica de Backlog Refinement como mecanismo continuo de curaduría.

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Scrum Master + Backlog Curator | El foco está en la API pública y la consumibilidad; las épicas se arman por superficie de Abstractions y por capacidades del motor interno. |
| web-monolith | Scrum Master | Épicas por capacidad funcional del producto; flujo estándar de US con criterios BDD y trazabilidad CU. |
| web-microservices | Scrum Master + Tech Lead Distribuido | Épicas por bounded context, con BT que reflejan acuerdos de contrato entre servicios y dependencias inter-equipo. |
| desktop-app | Scrum Master + Product Owner | Épicas por flujo de operación local; el PO arbitra alcance entre funcionalidad y experiencia desktop. |
| mobile-app-maui | Scrum Master + Mobile Lead | Épicas por capacidad móvil (offline, sincronización, permisos, ciclo de vida); BT específicas de plataforma cuando aplica. |
| rest-api | Scrum Master + API Product Owner | Épicas por recurso o por dominio del contrato OpenAPI; el API PO arbitra retro-compatibilidad y deprecaciones. |
| cli-tool | Scrum Master | Épicas por comando o pipeline; BT por contrato de stdout/stderr y exit codes. |
| worker-service | Scrum Master + Operations Lead | Épicas por flujo de mensajes y por capacidades operativas (idempotencia, dead-letter, backpressure); el Ops Lead arbitra prioridades operativas. |

El orquestador lee esta tabla y, según el `project_type` del proyecto en curso (leído del manifiesto de solución), selecciona la variante correspondiente y la combina con la especialidad base. La variante se aplica una vez por cada proyecto de la solución.

### 1.3 Multi-especialidad

La categoría 06 admite revisiones acotadas por otras especialidades:

- AG-02 Analista Funcional, para validar que cada US traza a un CU y que la columna `CU relacionados` está poblada sin huérfanas.
- AG-05 Arquitecto, para validar que cada BT se justifica en un componente, una ADR o un contrato declarado en 05, sin BT inventadas.
- AG-08 QA, para validar que los criterios de aceptación de cada US son verificables y aptos para alimentar los acceptance tests de 08.
- AG-07 PM ágil, para alinear capacidad y secuencia con el plan de sprints de 07 sin invadir su titularidad.

El AG-06 mantiene siempre la titularidad del artefacto; las demás especialidades aportan revisiones acotadas.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra

| Archivo | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `product-backlog_v1.0.md` | Todos los tipos D8 | — | — | Índice maestro priorizado de historias de usuario con MoSCoW, story points, trazabilidad a CU y a épicas. |
| `backlog-tecnico_v1.0.md` | Todos los tipos D8 | — | — | Vista del backlog desde la lente técnica, organizada por épicas técnicas y por BT-XX con dependencias, fuente upstream y criterios de aceptación. |
| `historias-usuario/US-XX-<kebab>_v1.0.md` | Proyectos con más de 20 US | Proyectos con 10 a 20 US | Proyectos con menos de 10 US (US inline en `product-backlog_v1.0.md`) | Un archivo por historia, con criterios BDD, INVEST check, trazabilidad y DoR. |
| `tareas-tecnicas/BT-XX-<kebab>_v1.0.md` | Proyectos con más de 30 BT | Proyectos con 15 a 30 BT | Proyectos con menos de 15 BT (BT inline en `backlog-tecnico_v1.0.md`) | Un archivo por tarea técnica, con justificación, dependencias, tipo y trazabilidad a US. |
| `definition-of-ready_v1.0.md` | Todos los tipos D8 | — | — | Criterios DoR para US y para BT, con aprobador y excepciones admitidas. |
| `README.md` de la sección | Recomendado para todos | — | — | Índice navegable del backlog: épicas, US Must Have, BT prioritarias y DoR vigente. |

### 2.2 Reglas de inclusión y exclusión por tipo

| Tipo D8 | Estimación recomendada | Refinement mínimo | BT mínimas | Énfasis del backlog |
| --- | --- | --- | --- | --- |
| library | Fibonacci o T-shirt | 1 sesión por sprint | 10 (fundaciones, API, tests) | API pública y consumibilidad |
| web-monolith | Fibonacci | 1 sesión por sprint | 8 (capas, persistencia, autenticación) | Capacidad funcional end-to-end |
| web-microservices | Fibonacci por bounded context | 1 sesión por contexto por sprint | 12 por contexto | Contratos inter-servicio y deploy independiente |
| desktop-app | T-shirt o Fibonacci | 1 sesión por sprint | 8 (persistencia local, distribución) | Flujo de operación local y experiencia |
| mobile-app-maui | Fibonacci | 1 sesión por sprint | 10 (offline, sync, permisos) | Capacidad móvil y ciclo de vida |
| rest-api | Fibonacci | 1 sesión por sprint | 10 (recursos, autenticación, errores) | Contrato REST y retro-compatibilidad |
| cli-tool | T-shirt | 1 sesión por sprint | 6 (parser, comandos, contrato stdout/stderr) | Pipeline de comandos y exit codes |
| worker-service | Fibonacci | 1 sesión por sprint | 10 (mensajería, idempotencia, dead-letter) | Flujo de mensajes y resiliencia operativa |

Los mínimos son piso, no techo. La cantidad real surge del refinamiento iterativo a partir de los CU y de las ADRs vigentes.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `product-backlog_v<X.Y>.md` para el índice maestro de historias.
- `backlog-tecnico_v<X.Y>.md` para la vista técnica por épicas.
- `historias-usuario/US-XX-<kebab-lowercase>_v<X.Y>.md` para cada historia individual, con dos dígitos en `XX`, kebab estricto en el slug y guion bajo antes de la versión.
- `tareas-tecnicas/BT-XX-<kebab-lowercase>_v<X.Y>.md` para cada tarea técnica individual, con dos dígitos en `XX`, kebab estricto y guion bajo antes de la versión.
- `definition-of-ready_v<X.Y>.md` para la DoR.

Queda prohibido el patrón heredado `product-backlog.v1.0.md`; la versión siempre va con `_v`, jamás con `.v`. El slug siempre va en minúsculas; queda prohibido `US-05-Paciente-Cancela-Turno_v1.0.md` o variantes con mayúsculas o camelCase.

### 3.2 Convención crítica de identificadores de dos dígitos

La auditoría de Fase 0 del bootstrap (`_bootstrap/audit-sdd1.md`) detectó que el fuente Motor DSL usa identificadores heterogéneos: `US-01` con dos dígitos en el product-backlog y `BT-001` con tres dígitos en el backlog técnico. SDD 2.1 corrige esta práctica de manera obligatoria:

1. Todos los identificadores de US y BT llevan dos dígitos uniformes (`US-01` a `US-99`, `BT-01` a `BT-99`).
2. Si un proyecto supera 99 US o 99 BT, debe descomponerse en épicas separadas o subdividirse en sub-backlogs por bounded context antes de saltar a tres dígitos.
3. Queda prohibido mezclar `US-01` y `BT-001` en el mismo proyecto; los esquemas de numeración deben ser consistentes entre artefactos.
4. Las épicas mantienen su prefijo propio `EP-XX` con dos dígitos.

Esta convención asegura coherencia visual entre product-backlog y backlog-tecnico y compatibilidad con herramientas de búsqueda y matriz de trazabilidad.

### 3.3 Convención de archivos individuales para US y BT

- Para proyectos con más de 20 US, cada historia vive en su archivo individual bajo `historias-usuario/`. Esto permite versionar cada US, asignarla a un autor distinto y revisarla por separado.
- Para proyectos con más de 30 BT, cada tarea técnica vive en su archivo individual bajo `tareas-tecnicas/`. Esto permite trazar dependencias técnicas por archivo y bloquear individualmente sin tocar el backlog completo.
- Para proyectos por debajo de esos umbrales, las US y BT pueden vivir inline en el `product-backlog_v1.0.md` y el `backlog-tecnico_v1.0.md` respectivamente, sin perder la estructura de secciones obligatoria.
- En ambos modos (inline o por archivo), los criterios de aceptación, la trazabilidad y el DoR check deben estar presentes; no se admite "US sin criterios" ni "BT sin justificación" en ningún modo.

### 3.4 Vinculación cross-doc

- Upstream: cada US referencia al menos un CU de 02 y opcionalmente una NB de 01. Cada BT referencia un componente, una ADR o un contrato de 05; o un CU si la BT lo cubre como tarea técnica derivada.
- Downstream: cada US alimenta el sprint plan de 07 (asignación a sprint y velocity) y los acceptance tests de 08 (escenarios Given/When/Then).
- El `product-backlog_v1.0.md` referencia el `backlog-tecnico_v1.0.md` y viceversa.
- La DoR referencia el `product-backlog_v1.0.md` como filtro de entrada y la Definition of Done de 08 como filtro de salida (sin solaparlas).

### 3.5 README de la sección

Recomendado para todos los tipos. Debe listar el `product-backlog_v1.0.md`, el `backlog-tecnico_v1.0.md`, el resumen de épicas vigentes, las US Must Have del MVP, las BT prioritarias y la DoR vigente. Sirve como punto de entrada navegable para revisores externos (AG-02, AG-05, AG-07, AG-08).

### 3.6 Política de versionado

Una sola versión vigente por nombre lógico para todos los artefactos. Cuando el backlog evoluciona de manera significativa (por ejemplo, una nueva épica grande o un cambio de alcance del MVP), se pasa de `v1.0` a `v2.0` y la versión anterior se mueve a `_legacy/` con estado `Superado`. Para `US-XX` y `BT-XX` aplica una regla diferente: una vez que una US o BT entró a un sprint y se cerró con `Done`, no cambia de versión; si la funcionalidad evoluciona, se crea una nueva US o BT con el siguiente identificador disponible.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada artefacto inicia con un H1 y un bloque markdown de metadatos:

```markdown
# US-XX — <Título de la historia>

**Proyecto:** {{nombre-proyecto}}
**Documento:** US-XX-<kebab>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Ready | En curso | Done | Descartada
**Fecha:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
**Épica:** EP-XX
**Prioridad MoSCoW:** Must | Should | Could | Won't (v1.0)
**Estimación:** <story points o T-shirt>
```

Para `product-backlog`, `backlog-tecnico`, `BT-XX` y `definition-of-ready` aplica la misma cabecera adaptando el título y los campos pertinentes (la BT lleva `Tipo` en vez de `Prioridad MoSCoW`).

### 4.2 Secciones obligatorias de `product-backlog_v1.0.md`

1. Objetivos del producto. Una a tres oraciones que enuncien el propósito del backlog y el MVP buscado.
2. Épicas. Tabla con identificador `EP-XX`, nombre, descripción breve y sprints estimados.
3. Historias por épica. Tabla con `US-XX`, título, MoSCoW, story points, estado, CU relacionados y épica.
4. Métricas de avance. Resumen por prioridad (cantidad y SP), porcentaje cerrado, deuda en backlog.
5. Refinamiento. Cadencia de refinement, responsables y formato (Planning Poker, T-shirt, etc.).

### 4.3 Secciones obligatorias de `backlog-tecnico_v1.0.md`

1. Épicas técnicas. Cada épica con objetivo, alcance, fuente upstream (ADR, componente o contrato) y BT contenidas.
2. BT por épica. Tabla con `BT-XX`, título, tipo (spike, feature, refactor, devops, docs), prioridad, estimación, fuente (NB, CU, ADR, contrato), dependencias y criterios de aceptación.
3. Trazabilidad BT↔US↔CU. Tabla cruzada que permita, para cada BT, identificar las US que la consumen y los CU upstream.

### 4.4 Secciones obligatorias de `US-XX-<kebab>_v1.0.md`

1. Historia. Bloque `Como [rol], quiero [acción], para [valor]` con valor explícito.
2. Contexto. Por qué existe esta historia, qué NB o CU la motivan, qué problema resuelve.
3. Criterios de aceptación. Lista de escenarios en formato Given/When/Then, mínimo dos escenarios, con al menos un happy path y un edge case.
4. Trazabilidad. Tabla con NB de origen, CU relacionados, BT derivadas y tests previstos en 08.
5. Prioridad y estimación. MoSCoW declarada con justificación, story points (Fibonacci o T-shirt) y técnica usada.
6. DoR check. Lista de los criterios DoR vigentes con su estado (cumplido o pendiente).
7. Notas y supuestos. Aclaraciones que no son criterios pero documentan decisiones de scope o ambigüedades resueltas.

### 4.5 Secciones obligatorias de `BT-XX-<kebab>_v1.0.md`

1. Descripción. Qué se construye, refactoriza o investiga; alcance acotado a una tarea ejecutable en menos de un sprint.
2. Justificación. Qué NB, CU, ADR o contrato exige esta tarea; sin justificación upstream no hay BT.
3. Criterios de aceptación. Lista de verificación técnica (la BT compila, los tests pasan, el contrato se respeta, la deuda queda saldada).
4. Dependencias. BT o US que deben estar terminadas antes; bloqueos externos identificados.
5. Tipo. Feature, spike, refactor, devops o docs. Spike implica caja temporal explícita.
6. Estimación. Story points o T-shirt, con técnica declarada.
7. Trazabilidad a US. US que consumen esta BT (mínimo una; BT sin US consumidora debe justificarse como infraestructura compartida).

### 4.6 Secciones obligatorias de `definition-of-ready_v1.0.md`

1. Criterios DoR para US. Lista enumerada con criterios verificables (mínimo cinco, máximo ocho).
2. Criterios DoR para BT. Lista enumerada con criterios técnicos (mínimo cuatro, máximo seis).
3. Excepciones admitidas. Casos donde la DoR puede flexibilizarse (por ejemplo, spike exploratorio) y quién aprueba la excepción.
4. Aprobador. Rol responsable de validar que un ítem cumple la DoR antes de entrar a Sprint Planning.

### 4.7 Tablas tipo y formatos recurrentes

Tabla de estimación con técnicas disponibles:

| Técnica | Escala | Recomendado para | Limitación |
| --- | --- | --- | --- |
| Fibonacci | 1, 2, 3, 5, 8, 13, 21 | Equipos con velocity estable | Necesita historial de sprints previos |
| T-shirt | XS, S, M, L, XL | Equipos nuevos o backlog inicial | Menos preciso para forecast |
| Horas ideales | 1h, 2h, 4h, 8h | BT técnicas con alcance ya cerrado | Pierde la idea relativa de complejidad |

Tabla de INVEST check por historia:

| Atributo | Verificación |
| --- | --- |
| Independent | Se puede planificar sin esperar a otra US del mismo sprint |
| Negotiable | El alcance admite ajuste en refinamiento |
| Valuable | El valor para el rol está explícito en el `para [...]` |
| Estimable | El equipo tiene información suficiente para estimarla |
| Small | Cabe en un sprint con holgura |
| Testable | Los criterios de aceptación son verificables |

Tabla de priorización MoSCoW con justificación:

| Prioridad | Significado | Criterio práctico | Distribución sugerida |
| --- | --- | --- | --- |
| Must | Imprescindible para el MVP | Sin esto no se cumple el propósito del sistema | 50 a 60 % del backlog inicial |
| Should | Importante pero no bloqueante | El MVP funciona sin esto pero queda incompleto | 20 a 30 % |
| Could | Deseable si hay capacidad | Agrega valor; puede esperar a v2 | 10 a 20 % |
| Won't (v1.0) | Fuera de alcance explícito | Documentado para no perderse | sin tope |

Tabla de trazabilidad de la US:

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-XX |
| CU cubiertos | CU-XX, CU-YY |
| BT derivadas | BT-XX, BT-YY |
| Tests previstos | <referencia tentativa a 08> |

### 4.8 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| US sin valor explícito | La cláusula `para [...]` está vacía o es un sinónimo de la acción | Forzar la verbalización del beneficio para el rol antes de aceptar la US |
| BT que en realidad es una US | "Implementar parser" disfrazado de tarea técnica oculta el valor de negocio | Reformular como US si tiene valor para un rol; mantener como BT solo si soporta una US declarada |
| DoR sin criterios verificables | "La historia debe estar clara" no es verificable | Cada criterio DoR debe poder responderse con sí o no de manera objetiva |
| Estimación sin técnica declarada | Mezcla de SP y horas sin convención | Declarar la técnica en el `product-backlog_v1.0.md` y mantenerla en todas las US |
| Backlog sin refinement con cadencia | El backlog envejece sin curaduría; entra trabajo crudo al sprint | Política explícita de refinement (mínimo una sesión por sprint) documentada en §5 del backlog |
| IDs heterogéneos | `US-01` y `BT-001` rompen búsquedas y matrices de trazabilidad (lección documentada del fuente Motor DSL) | Forzar dos dígitos uniformes en US, BT y EP |
| Todo Must Have | No hay priorización real; el equipo no puede recortar alcance | Forzar distribución 60/20/20 entre Must, Should y Could como ejercicio mínimo |
| US huérfanas de CU | No se puede validar completitud del sistema contra la especificación funcional | Columna `CU relacionados` obligatoria; AG-02 firma trazabilidad |
| BT sin US consumidora | Tarea técnica que nadie usa; deuda inventada | Cada BT debe declarar al menos una US que la consume o justificarse como infraestructura compartida con ADR explícita |
| Criterios de aceptación vagos | "Funciona correctamente" no se puede testear | Formato Given/When/Then obligatorio en US Must y Should |
| DoR de 15 criterios | Nada nunca está ready; el filtro se vuelve burocrático | Máximo 8 criterios DoR para US y 6 para BT |

---

## 5. Preguntas guía para el subagente

### 5.1 Upstream

- ¿Cada US referencia al menos un CU de 02? ¿Hay US que se infieren de los CU pero no están en el backlog?
- ¿Cada BT se justifica en un componente, una ADR o un contrato de 05? Si no, ¿por qué existe?
- ¿Las NB de 01 están todas representadas por al menos una épica o una US?
- ¿Hay ADRs en 05 que disparan BT de configuración o de adopción y todavía no se reflejaron en el backlog?

### 5.2 Scope

- ¿La distribución MoSCoW es realista o todo es Must?
- ¿Hay US que en realidad son épicas (más de un sprint)? ¿Hay épicas que son una sola US?
- ¿El MVP definido por las US Must cubre el propósito del sistema declarado en 00?
- ¿El umbral de archivos individuales (20 US y 30 BT) está aplicado correctamente?

### 5.3 Trazabilidad

- ¿Cada US tiene su columna `CU relacionados` poblada sin huérfanas?
- ¿Cada BT declara al menos una US consumidora o se justifica como infraestructura compartida con ADR?
- ¿La matriz BT↔US↔CU del `backlog-tecnico_v1.0.md` está completa y al día?
- ¿Los criterios de aceptación están alineados con los escenarios previstos en 08?

### 5.4 Calidad

- ¿Las US cumplen INVEST? ¿Hay alguna no estimable, no testeable o demasiado grande?
- ¿Los criterios de aceptación están en Given/When/Then con al menos dos escenarios por US Must?
- ¿La DoR tiene entre 5 y 8 criterios verificables, sin solapamiento con la Definition of Done de 08?
- ¿Hay una política de refinement con cadencia explícita?

### 5.5 Convenciones

- ¿Todos los identificadores usan dos dígitos uniformes? ¿Hay rastros de `BT-001` heredados del fuente?
- ¿Los slugs están en kebab-lowercase estricto?
- ¿Los nombres de archivo respetan `_v<X.Y>.md` y no `.v<X.Y>.md`?
- ¿Las US y BT individuales viven en sus carpetas (`historias-usuario/` y `tareas-tecnicas/`) cuando aplica el umbral?

---

## 6. Criterios de aceptación

- [ ] Existe `product-backlog_v1.0.md` con las cinco secciones del §4.2 y una tabla de épicas con identificadores `EP-XX` de dos dígitos.
- [ ] Existe `backlog-tecnico_v1.0.md` con las tres secciones del §4.3 y la tabla cruzada BT↔US↔CU completa.
- [ ] Existe `definition-of-ready_v1.0.md` con criterios DoR para US (entre 5 y 8) y para BT (entre 4 y 6), excepciones y aprobador declarado.
- [ ] Todos los identificadores `US-XX`, `BT-XX` y `EP-XX` usan dos dígitos uniformes; no quedan rastros de `BT-001` heredados del fuente.
- [ ] Cada US tiene al menos un CU relacionado declarado en la columna `CU relacionados`; no hay US huérfanas.
- [ ] Cada BT tiene fuente upstream declarada (NB, CU, ADR o contrato) y al menos una US consumidora o justificación de infraestructura compartida.
- [ ] La distribución MoSCoW no es 100 % Must; hay reparto razonable entre Must, Should y Could.
- [ ] Cada US Must y Should tiene criterios de aceptación en Given/When/Then con al menos dos escenarios.
- [ ] Si el proyecto supera 20 US, existen archivos individuales bajo `historias-usuario/`; si supera 30 BT, existen archivos individuales bajo `tareas-tecnicas/`.
- [ ] Ningún archivo usa el patrón `.v<X.Y>.md`; todos usan `_v<X.Y>.md`.
- [ ] La DoR no se solapa con la Definition of Done de 08; la DoR habla de cuándo empezar, la DoD de cuándo terminar.
- [ ] No hay menciones a stacks concretos, productos comerciales ni protocolos específicos del dominio fuente.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — US individual para un web-monolith de turnos médicos

Fragmento ilustrativo, no documento completo:

```markdown
# US-05 — El paciente cancela un turno

**Proyecto:** {{nombre-proyecto}}
**Documento:** US-05-paciente-cancela-turno_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-05-17
**Autor:** Equipo Producto
**Épica:** EP-02 Reserva de turnos
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia
Como paciente, quiero cancelar un turno previamente reservado, para liberar el horario si no puedo asistir y permitir que otra persona lo tome.

## 2. Contexto
NB-02 exige que la disponibilidad de turnos refleje la realidad en tiempo real. CU-04 describe el flujo de cancelación y RN-05 fija la ventana mínima de aviso. Sin esta US, los turnos no asistidos quedan ocupando agenda y bloquean reservas legítimas.

## 3. Criterios de aceptación
- Given un turno reservado con más de 24 h de anticipación, When el paciente confirma la cancelación, Then el sistema libera el horario y muestra confirmación con la hora liberada.
- Given un turno reservado con menos de 2 h de anticipación, When el paciente intenta cancelar, Then el sistema advierte la penalidad pero permite continuar y registra la cancelación tardía.
- Given un turno que ya pasó, When el paciente intenta cancelarlo, Then el sistema informa que no es posible cancelar un turno vencido.

## 4. Trazabilidad
| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-04 |
| BT derivadas | BT-12, BT-15 |
| Tests previstos | acceptance/AT-04-cancelacion-turno |

## 5. Prioridad y estimación
Must porque sin cancelación la agenda queda contaminada por turnos no asistidos. 5 SP estimado por Planning Poker con la técnica Fibonacci adoptada por el equipo.

## 6. DoR check
- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP
- [x] CU relacionado identificado
- [x] Sin dependencias bloqueantes
- [x] Datos de prueba disponibles (fixtures de turnos)

## 7. Notas y supuestos
La política de penalidad por cancelación tardía queda fuera del alcance de esta US; se cubre en US-09. Esta US asume autenticación previa del paciente (US-01).
```

### 7.2 Ejemplo 2 — BT individual para un rest-api de pagos

Fragmento ilustrativo, no documento completo:

```markdown
# BT-12 — Spike de comparación de mecanismos de persistencia

**Proyecto:** {{nombre-proyecto}}
**Documento:** BT-12-spike-comparacion-orm_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-05-17
**Autor:** Equipo Plataforma
**Épica:** EP-T3 Persistencia del bounded context de pagos
**Tipo:** Spike
**Estimación:** 3 SP (Fibonacci) con caja temporal de 2 días

## 1. Descripción
Investigar y comparar dos mecanismos de persistencia candidatos para el bounded context de pagos, en función de los criterios de transaccionalidad, soporte de migraciones versionadas y curva de adopción del equipo.

## 2. Justificación
ADR-03 declara la decisión de adoptar una base relacional pero deja abierta la elección del mecanismo de mapeo. CU-06 (registrar pago) y CU-08 (idempotencia de confirmación) exigen transacciones ACID con clave de idempotencia, lo que condiciona la selección. NB-04 fija un objetivo de latencia p95 que también condiciona la elección.

## 3. Criterios de aceptación
- Existe un informe comparativo con dos alternativas evaluadas en transaccionalidad, migraciones y rendimiento bajo carga sintética.
- El informe propone una recomendación con argumento técnico y trade-offs explícitos.
- La recomendación se eleva como ADR candidata; el spike no decide por sí solo, alimenta una ADR posterior.

## 4. Dependencias
- ADR-03 aceptada (persistencia relacional).
- BT-10 (modelo lógico inicial) completada para tener entidades reales contra las que medir.

## 5. Tipo
Spike con caja temporal de 2 días. Si al final del plazo no hay recomendación clara, se documenta el bloqueo y se eleva al arquitecto.

## 6. Estimación
3 SP con caja temporal explícita. La técnica Fibonacci se mantiene aunque la BT sea spike para no romper la cadencia de velocity.

## 7. Trazabilidad a US
| US | Por qué consume esta BT |
| --- | --- |
| US-08 (registrar pago) | Necesita la decisión de persistencia tomada antes de implementarse |
| US-09 (consultar estado de pago) | Lectura sobre el mismo mecanismo |
```

Los dos fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar el backlog técnico del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path}}
- Upstream: 01 (NB-XX), 02 (CU, RN, modelo conceptual), 05 (arquitectura de solución, ADRs, modelo lógico, contratos).

A generar (según tipo {{TIPO}} de D8):
- product-backlog_v1.0.md con las cinco secciones obligatorias y la tabla de épicas EP-XX.
- backlog-tecnico_v1.0.md con épicas técnicas, BT por épica y matriz BT↔US↔CU.
- historias-usuario/US-XX-<kebab>_v1.0.md si el proyecto supera 20 US; en caso contrario, US inline en el product-backlog.
- tareas-tecnicas/BT-XX-<kebab>_v1.0.md si el proyecto supera 30 BT; en caso contrario, BT inline en el backlog-tecnico.
- definition-of-ready_v1.0.md con criterios DoR para US (5-8) y BT (4-6), excepciones y aprobador.
- README.md de la sección (recomendado).

Reglas de redacción: §4 de 06_rules_backlog_tecnico.md.
Nomenclatura: dos dígitos uniformes para US-XX, BT-XX y EP-XX; slug kebab-lowercase; sufijo `_v1.0.md` con guion bajo.
Convención crítica: corregir la herencia del antecedente Motor DSL que mezclaba `US-01` con `BT-001`; forzar dos dígitos en todos los identificadores.
Trazabilidad: cada US referencia al menos un CU; cada BT referencia NB, CU, ADR o contrato; matriz BT↔US↔CU completa.
INVEST + MoSCoW: cada US Must y Should cumple INVEST y tiene criterios Given/When/Then con al menos dos escenarios.
Estimación: declarar técnica (Fibonacci, T-shirt u horas ideales) y mantenerla consistente en todo el backlog.
Criterios de calidad: §6 de 06_rules_backlog_tecnico.md.

Restricciones: no introducir stacks concretos, productos comerciales ni protocolos del dominio fuente. Idioma rioplatense técnico, tildes correctas, sin emojis.

Salida: /SDD2.1D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/06_backlog-tecnico/<estructura>.
```

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial de las reglas constructivas de la categoría 06. Establece la convención obligatoria de dos dígitos uniformes en US, BT y EP como corrección del antecedente Motor DSL (mezcla `US-01` y `BT-001`), define las cinco secciones del product-backlog, las tres del backlog-tecnico, las siete de cada US individual y las siete de cada BT individual, las variantes por tipo D8, los umbrales de archivos individuales (20 US y 30 BT) y los criterios de aceptación. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/06_backlog-tecnico/`; la frase de cierre de §1.2 y la ruta de salida del prompt-snippet referencian el `project_type` del proyecto en curso (manifiesto). Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
