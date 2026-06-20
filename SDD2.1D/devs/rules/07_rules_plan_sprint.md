# Reglas constructivas — 07 Plan de sprint

**Carpeta target (por proyecto):** `/SDD2.1D/docs/proyectos/<nombre-proyecto-kebab>/07_plan-sprint/`
**Subagente target del orquestador:** Scrum Master / Gestión Ágil de Proyectos senior (AG-07)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.1

La categoría 07 es la primera categoría de ejecución iterativa de la cadena de trazabilidad D6. Recibe upstream de 06 (backlog técnico priorizado, US y BT con estimación), 02 (CU y RN cuya trazabilidad sobrevive sprint a sprint) y 05 (arquitectura y ADRs que condicionan el alcance técnico). Alimenta 08 (acceptance tests del sprint vigente y plan de pruebas por iteración), 09 (DevOps si el sprint introduce cambios de pipeline o despliegue) y 10 (developer guide cuando una decisión del sprint impacta convenciones de código). La categoría 07 no decide qué se construye en el largo plazo, eso lo hace 06; decide qué se construye en la ventana timeboxed del sprint vigente, con compromiso del equipo y métrica de cumplimiento. Es obligatoria para todos los proyectos con equipo mayor a un desarrollador; para proyectos de un solo dev se reduce a un mini-plan documentado, pero no se omite.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Scrum Master / Gestión Ágil de Proyectos senior, equivalente al AG-07 del catálogo SDD. El perfil combina facilitación de ceremonias (planning, review, retrospectiva, daily), tracking de métricas ágiles (velocity, burndown, ratio de completitud) y gestión de impedimentos. Se alinea con la Scrum Guide 2020 para artefactos y eventos, con Evidence-Based Management para métricas de valor y con prácticas de Kanban para casos de demanda continua o equipos de mantenimiento. Traduce el backlog priorizado de 06 en planes de iteración ejecutables con commitment realista, mantiene la consistencia inter-sprint y asegura que cada ceremonia produzca artefactos accionables, no documentos teóricos.

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Scrum Master + Maintainer Lead | El trabajo es release-driven más que feature-driven; los sprints se organizan alrededor de versiones publicables y compatibilidad hacia atrás. |
| web-monolith | Scrum Master | Iteraciones de dos semanas con review demostrable sobre el entorno de prueba; ceremonias estándar Scrum. |
| web-microservices | Scrum Master + Coordinador de Releases distribuidos | Necesidad de sincronizar sprints entre bounded contexts; revisión cross-team de dependencias y ventanas de despliegue coordinadas. |
| desktop-app | Scrum Master + Cross-Platform PM | Coordinación entre plataformas objetivo (Windows, Linux, macOS) y cadencias de empaquetado por sistema operativo. |
| mobile-app-maui | Scrum Master + Mobile Release Manager | Ciclos atados a tiendas (revisión y publicación) que condicionan el cierre del sprint y la planificación del siguiente. |
| rest-api | Scrum Master + API PM | Coordinación de cambios de contrato, deprecaciones y ventanas de breaking changes en cada sprint. |
| cli-tool | Scrum Master simplificado | Equipos pequeños y ceremonias condensadas; planning y retro fusionados o reducidos. |
| worker-service | Scrum Master + Operations Lead | Sprints que combinan features con tareas de mantenimiento operativo (queues, reintentos, dead-letter); seguimiento de incidentes recurrentes. |

El orquestador lee esta tabla y, según el `project_type` del proyecto en curso (leído del manifiesto de solución), selecciona la variante correspondiente y la combina con la especialidad base. La variante se aplica una vez por cada proyecto de la solución.

### 1.3 Multi-especialidad

La categoría 07 admite revisiones acotadas por otras especialidades:

- AG-06 Product Owner / Backlog, para validar que las US y BT comprometidas son las correctas según prioridad y para confirmar el sprint goal con el dueño del producto.
- AG-08 QA, para acordar qué casos de prueba acompañan a cada US comprometida y cómo se verifica la DoD aplicada.
- AG-09 DevOps, cuando el sprint introduce cambios en el pipeline, en el entorno de prueba o en la estrategia de release.
- AG-05 Arquitecto, cuando una US del sprint requiere una ADR nueva o impacta una existente.

El AG-07 mantiene siempre la titularidad de los artefactos; las demás especialidades aportan revisiones acotadas y validan el alcance comprometido.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra

| Archivo | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `plan-iteracion-sprint-XX_v1.0.md` | Todos los tipos D8 con equipo mayor a 1 dev; mínimo Sprint 0 y Sprint 1 | — | Proyectos de 1 dev (se reemplaza por mini-plan) | Plan de iteración por sprint. Un archivo por sprint, numerado con dos dígitos. |
| `template-sprint-review_v1.0.md` | Todos los tipos D8 con equipo mayor a 1 dev | — | Proyectos de 1 dev | Plantilla reusable para documentar el sprint review al cierre de cada iteración. |
| `template-sprint-retrospectiva_v1.0.md` | Todos los tipos D8 con equipo mayor a 1 dev | — | Proyectos de 1 dev | Plantilla reusable para documentar la retrospectiva al cierre de cada iteración. |
| `velocidad-equipo_v1.0.md` | Todos los tipos D8 con equipo mayor a 1 dev | — | Proyectos de 1 dev | Tracking acumulado de velocity sprint a sprint, con promedio móvil y outliers explicados. |
| `mini-plan_v1.0.md` | Proyectos de 1 dev (sustituye a los cuatro anteriores) | — | Equipos con más de 1 dev | Plan único condensado que combina sprint goal, lista de items y bitácora de avance semanal. |
| `README.md` de la sección | Recomendado para todos | — | — | Índice navegable de planes de sprint vigentes, plantillas reusables y tabla de velocity. |

### 2.2 Reglas de inclusión y exclusión por tamaño de equipo

| Escenario | Sprint plans | Templates review/retro | Velocidad | Mini-plan |
| --- | --- | --- | --- | --- |
| Equipo de 2 o más devs | Uno por sprint, mínimo Sprint 0 y Sprint 1 | Sí | Sí | No |
| Equipo de 1 dev | No (se reemplaza) | No | No | Sí |
| Equipo mixto con freelancers ocasionales | Uno por sprint si hay ceremonias formales | Sí | Sí | No |

Sprint 0 corresponde al sprint de arranque dedicado a setup técnico, validación del backlog y alineación del equipo. Cuando aplica, su plan documenta tareas de bootstrap (entorno de desarrollo, repositorio, CI inicial, definition of done acordada) y no necesariamente entrega valor de negocio.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `plan-iteracion-sprint-XX_v<X.Y>.md` para cada plan de iteración. `XX` con dos dígitos siempre (01, 02, ..., 99). Un único separador previo a la versión: `_v`.
- `template-sprint-review_v<X.Y>.md` para la plantilla de review.
- `template-sprint-retrospectiva_v<X.Y>.md` para la plantilla de retrospectiva.
- `velocidad-equipo_v<X.Y>.md` para el tracking de velocidad.
- `mini-plan_v<X.Y>.md` para proyectos de un solo dev.

La auditoría de Fase 0 del bootstrap detectó que el fuente Motor DSL usa el patrón `plan-iteracion_sprint-01_v1.0.md`, que mezcla dos separadores (`_sprint-01` y luego `_v1.0`) sin coherencia con el resto del repositorio. SDD 2.1 corrige esta práctica: el separador interno del nombre lógico es el guion (`-`), y el guion bajo (`_`) se reserva exclusivamente para anteceder a la versión. Queda prohibido el patrón `plan-iteracion_sprint-XX_v<X.Y>.md` con doble separador; el patrón correcto y único es `plan-iteracion-sprint-XX_v<X.Y>.md`.

### 3.2 Duración estándar del sprint

La duración estándar de un sprint es de dos semanas (10 días hábiles). Justificaciones aceptadas para variar:

- Sprint de una semana cuando el equipo arranca, valida un walking skeleton o necesita ciclos cortos para reducir riesgo en una fase exploratoria. Documentar en §1 del plan que se trata de un sprint corto justificado.
- Sprint de tres semanas cuando el dominio exige ventanas más largas (ciclos de revisión de tienda en mobile-app-maui, releases coordinados en web-microservices, validaciones regulatorias). Documentar la justificación y revertir a dos semanas en cuanto la restricción desaparezca.

Sprints de cuatro semanas o más quedan desaconsejados: rompen la cadencia de feedback y diluyen la métrica de velocity.

### 3.3 Convenciones de prefijos y sufijos

- Prefijo `plan-iteracion-sprint-`: identifica el plan de un sprint específico.
- Prefijo `template-sprint-`: identifica plantillas reusables (review, retrospectiva).
- Sufijo `_v<X.Y>.md` uniforme para todos los artefactos. Sin excepciones.

### 3.4 Vinculación cross-doc

- Upstream: cada US o BT incluida en el sprint plan referencia el identificador exacto del backlog técnico de 06 (`BT-XX`, `US-XX`). Sin invención de identificadores nuevos.
- Trazabilidad a 02: cada sprint declara qué CU y NB avanzan al cierre. Un sprint sin trazabilidad a CU es un sprint sin sentido funcional.
- Downstream a 08: cada US comprometida en el sprint dispara la creación o actualización de su caso de aceptación en 08.
- Downstream a 09: si el sprint introduce un cambio de pipeline o despliegue, el plan referencia explícitamente la actualización prevista en 09.

### 3.5 README de la sección

Recomendado. Debe listar el sprint actual con su plan, el histórico de sprints cerrados, las plantillas reusables y un enlace directo a `velocidad-equipo_v1.0.md`. Sirve como punto de entrada para revisores externos (AG-06, AG-08, AG-09) y para nuevos miembros del equipo.

### 3.6 Política de versionado

Los planes de sprint (`plan-iteracion-sprint-XX_v1.0.md`) no se versionan tras el cierre del sprint: el plan refleja el compromiso original. Si durante el sprint cambia el scope, el cambio se documenta en la sección de control de cambios del propio plan, pero el archivo conserva la versión inicial. Las plantillas reusables y `velocidad-equipo_v1.0.md` siguen la regla general del repositorio: una sola versión vigente; al pasar a `v2.0` la anterior se mueve a `_legacy/` con estado `Superado`.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada artefacto inicia con un H1 y un bloque markdown de metadatos. Queda prohibida la apertura con `--` o cualquier separador previo al H1 (corrección del antecedente Motor DSL detectada en la auditoría):

```markdown
# Plan de Iteración — Sprint XX

**Proyecto:** {{nombre-proyecto}}
**Documento:** plan-iteracion-sprint-XX_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Propuesto | En curso | Cerrado
**Fecha inicio:** YYYY-MM-DD
**Fecha fin:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
```

Las plantillas reusables y el archivo de velocidad adaptan el título y el estado pero mantienen la misma estructura de cabecera.

### 4.2 Secciones obligatorias de `plan-iteracion-sprint-XX_v1.0.md`

1. Información general. Fechas, duración (en semanas), composición del equipo, capacidad disponible (en story points o en horas, declarando la unidad).
2. Objetivo del sprint (sprint goal). Una sola frase orientada a valor. Prohibido bullets, listas o enumeraciones. Debe responder a la pregunta "¿qué se logra al final del sprint?".
3. Historias y tareas comprometidas. Tabla con columnas: ID, tipo (US o BT), descripción corta, prioridad (Alta, Media, Baja), estimación, asignado, estado. Total de puntos comprometidos al pie.
4. Alcance técnico. Qué componentes se construyen o modifican, en qué orden, qué dependencias existen entre BT (por ejemplo BT-04 depende de BT-02). Esta sección no redefine arquitectura; referencia la arquitectura de 05.
5. Definition of Done aplicada. Referencia explícita a la DoD canónica del proyecto (vive en 08). Criterios específicos del sprint, si los hay, listados aparte.
6. Riesgos del sprint y mitigaciones. Tabla con cada riesgo, probabilidad (Alta, Media, Baja), impacto (Alto, Medio, Bajo) y plan de mitigación concreto. Mínimo dos riesgos por sprint.
7. Criterios de hecho del sprint. Cuándo se considera el sprint completo (todas las US comprometidas en estado terminado, demo realizada, retrospectiva facilitada).
8. Trazabilidad. Tabla con qué NB y qué CU avanzan en este sprint, qué ADRs gobiernan las decisiones técnicas implicadas.
9. Control de cambios. Tabla con versión, fecha y descripción. Para sprints cerrados, registra cambios de scope ocurridos durante la ejecución.

### 4.3 Secciones obligatorias de `template-sprint-review_v1.0.md`

1. Objetivo del sprint y resultado. Cita literal del sprint goal y veredicto: cumplido, parcialmente cumplido, no cumplido, con explicación corta.
2. Demos realizadas. Tabla con cada US o BT demostrada, breve descripción de la demo y feedback del Product Owner.
3. Feedback recibido. Comentarios cualitativos de stakeholders, sugerencias y nuevas necesidades detectadas.
4. Métricas del sprint. Tabla con puntos comprometidos, puntos completados, velocity efectiva, ratio de completitud, defectos detectados durante el sprint.
5. Items completados vs comprometidos. Tabla con cada US o BT comprometida y su estado final (Aceptada, Rechazada, Parcial).
6. Carry-over al siguiente sprint. Lista de items no completados que se mueven al siguiente sprint, con motivo del traslado.
7. Decisiones tomadas durante el review. Lista de decisiones explícitas (ajustes al backlog, repriorización, items removidos, items agregados).

### 4.4 Secciones obligatorias de `template-sprint-retrospectiva_v1.0.md`

1. Qué salió bien. Lista de aspectos positivos del proceso del sprint que el equipo quiere mantener.
2. Qué no salió bien. Lista de problemas, fricciones o impedimentos sufridos durante el sprint.
3. Qué probar. Experimentos concretos a intentar en el próximo sprint (cambios de proceso, nuevas prácticas, ajustes de herramienta).
4. Acciones concretas. Tabla con cada acción de mejora, responsable nombrado, fecha de compromiso y estado. Cada retrospectiva produce mínimo una acción con responsable y fecha; queda prohibida la retrospectiva sin acciones.
5. Seguimiento de acciones del sprint anterior. Tabla con las acciones comprometidas en la retro previa y su estado actual (Completada, En progreso, Cancelada).

### 4.5 Secciones obligatorias de `velocidad-equipo_v1.0.md`

1. Por sprint. Tabla con columnas: sprint, puntos comprometidos, puntos completados, velocity efectiva, promedio móvil de 3 sprints.
2. Tendencia. Lectura cualitativa de la evolución (estable, ascendente, descendente, errática) con comentario.
3. Capacidad ajustada. Cálculo de la capacidad sugerida para el próximo sprint a partir del promedio móvil de 3 sprints. Regla por defecto: no comprometer más del 110 % del promedio móvil.
4. Outliers explicados. Cada sprint cuyo valor se desvía más de un 30 % del promedio móvil debe estar explicado en una fila aparte (causa: vacaciones, incidente operativo, sprint inaugural, cambio de equipo).

### 4.6 Tablas tipo y formatos recurrentes

Tabla de capacidad del equipo, en §1 del plan:

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | <n> | <horas> | <0..1> | <horas × factor> |
| Dev frontend | <n> | <horas> | <0..1> | <horas × factor> |
| QA | <n> | <horas> | <0..1> | <horas × factor> |

El factor de focus refleja el porcentaje del tiempo realmente disponible para trabajo del sprint (descontando soporte, ceremonias, interrupciones). Valores típicos entre 0,6 y 0,8.

Tabla de items comprometidos, en §3 del plan:

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-XX | Historia | <texto corto> | Alta | <pts> | <nombre o rol> | Pendiente |
| BT-XX | Backlog técnico | <texto corto> | Media | <pts> | <nombre o rol> | Pendiente |

Tabla de burndown, sugerida para tracking diario:

| Día | Puntos restantes |
| --- | --- |
| 0 | <total comprometido> |
| 1 | <restante> |
| ... | ... |
| n | 0 |

Tabla de velocity, en `velocidad-equipo_v1.0.md`:

| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S01 | <pts> | <pts> | <pts> | — | Sprint inaugural |
| S02 | <pts> | <pts> | <pts> | — | — |
| S03 | <pts> | <pts> | <pts> | <pts> | — |

### 4.7 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Sprint goal vago ("avanzar", "mejorar el sistema") | No da dirección ni permite verificar cumplimiento | Reformular como frase orientada a valor con verbo y resultado verificable |
| Sprint goal expresado como lista de tareas | Mata la coherencia del sprint y bloquea decisiones de scope | Una sola frase declarativa; las tareas viven en §3 |
| Sprint sin DoD aplicada | Cada item se cierra con criterio improvisado | Referencia explícita a la DoD canónica de 08 + criterios específicos del sprint si los hay |
| Plan sin trazabilidad a CU o NB | No queda registro de qué necesidad de negocio avanza | Tabla obligatoria en §8 con CU y NB que avanzan |
| Retrospectiva sin acciones concretas | Se habla mucho pero nada cambia | Mínimo una acción con responsable nombrado y fecha de compromiso |
| Carry-over sistemático sin alerta | El equipo arrastra items sprint tras sprint sin replantearse el commitment | Si más del 20 % de los puntos comprometidos se trasladan, disparar análisis en la retro siguiente |
| Estimación basada en horas en lugar de puntos | Falsa precisión; ata la métrica a quien estima | Usar story points con escala consistente (Fibonacci o lineal) y declarar la unidad en §1 |
| DoD redefinida en cada sprint | Inconsistencia inter-sprint y dilución del criterio | Referenciar la DoD canónica; agregar solo criterios específicos del sprint |
| Doble separador en el nombre (`_sprint-XX_v`) | Inconsistencia detectada en el antecedente Motor DSL | Patrón único `plan-iteracion-sprint-XX_v<X.Y>.md` |
| Plan abierto con `--` antes del H1 | Inconsistencia de frontmatter detectada en el antecedente | H1 directo seguido del bloque markdown de metadatos |

---

## 5. Preguntas guía para el subagente

### 5.1 Upstream

- ¿Cada US y cada BT comprometida tiene origen verificable en el backlog técnico de 06? ¿Hay items inventados sin trazabilidad?
- ¿La prioridad declarada coincide con la prioridad del backlog priorizado, o el sprint se está saltando items de mayor prioridad sin justificación?
- ¿La estimación de cada item ya está fijada en 06 o se está re-estimando en el sprint?

### 5.2 Scope

- ¿La suma de estimaciones cabe dentro de la capacidad efectiva calculada en §1?
- ¿El sprint compromete más del 110 % del promedio móvil de velocity sin justificación?
- ¿El sprint goal es una sola frase orientada a valor o degeneró en lista de tareas?
- ¿Hay items que pertenecen a sprints futuros que se colaron por afinidad técnica?

### 5.3 Trazabilidad

- ¿Cada US comprometida declara los CU que cubre?
- ¿Cada BT comprometida declara las ADRs o NFR que la motivan?
- ¿La tabla §8 declara qué NB avanza al cierre del sprint? Un sprint sin NB en avance es un sprint sin valor de negocio.
- ¿El carry-over del sprint anterior está incorporado o explícitamente diferido?

### 5.4 Calidad

- ¿La DoD referenciada es la canónica del proyecto?
- ¿Hay mínimo dos riesgos identificados con mitigación concreta?
- ¿La retrospectiva del sprint anterior produjo acciones, y esas acciones están reflejadas en el sprint vigente?
- ¿La tabla de velocity está actualizada al cierre del sprint anterior?

### 5.5 Consistencia inter-sprint

- ¿Todos los planes de sprint tienen la misma estructura (nueve secciones del §4.2)?
- ¿La nomenclatura sigue el patrón único `plan-iteracion-sprint-XX_v<X.Y>.md` sin doble separador?
- ¿Algún plan abre con `--` u otro separador previo al H1?

---

## 6. Criterios de aceptación

- [ ] Existe `plan-iteracion-sprint-XX_v1.0.md` para cada sprint planificado, mínimo Sprint 0 y Sprint 1, con las nueve secciones del §4.2.
- [ ] Cada plan declara sprint goal como una sola frase orientada a valor, sin bullets ni listas.
- [ ] Existen `template-sprint-review_v1.0.md` y `template-sprint-retrospectiva_v1.0.md` como plantillas reusables aplicables a cualquier sprint del proyecto.
- [ ] Existe `velocidad-equipo_v1.0.md` con tabla por sprint y promedio móvil de 3 sprints, actualizada al cierre del último sprint cerrado.
- [ ] Cada plan referencia la DoD canónica de 08 y solo agrega criterios específicos del sprint si los hay.
- [ ] Cada plan declara trazabilidad a CU y NB en §8.
- [ ] Cada plan identifica mínimo dos riesgos con mitigación concreta.
- [ ] Toda retrospectiva documentada produce mínimo una acción con responsable nombrado y fecha de compromiso.
- [ ] Ningún archivo usa el patrón `plan-iteracion_sprint-XX_v<X.Y>.md` con doble separador; todos usan `plan-iteracion-sprint-XX_v<X.Y>.md`.
- [ ] Ningún archivo abre con `--` ni con otro separador previo al H1.
- [ ] No hay menciones a stacks concretos, productos comerciales ni protocolos del dominio fuente.
- [ ] Para proyectos de un solo dev, existe `mini-plan_v1.0.md` y no existen los cuatro artefactos completos.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — Fragmento de plan de sprint para un web-monolith de turnos médicos

Fragmento ilustrativo, no documento completo:

```markdown
# Plan de Iteración — Sprint 01

**Proyecto:** Turnos Médicos
**Documento:** plan-iteracion-sprint-01_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-06-01
**Fecha fin:** 2026-06-12

## 1. Información general
- Duración: 2 semanas (10 días hábiles)
- Equipo: 2 devs full-stack, 1 QA part-time
- Capacidad declarada: 22 story points (unidad: puntos Fibonacci)

## 2. Objetivo del sprint
Disponer de un walking skeleton end-to-end que permite a un usuario autenticado consultar la agenda de un profesional y reservar un turno disponible.

## 3. Historias y tareas comprometidas
| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Historia | Autenticación básica de usuario | Alta | 5 | Dev A | Pendiente |
| US-02 | Historia | Listado de agenda por profesional | Alta | 8 | Dev B | Pendiente |
| US-03 | Historia | Reserva de turno con restricción de no superposición | Alta | 8 | Dev A | Pendiente |
| BT-01 | Backlog técnico | Esquema inicial de base de datos con migración versionada | Alta | 3 | Dev B | Pendiente |
| BT-02 | Backlog técnico | Pipeline de CI con build, test y lint | Media | 3 | Dev A | Pendiente |

## 6. Riesgos del sprint
| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| RN-04 (no superposición) requiere restricción única compuesta no probada | Media | Alto | Spike técnico en día 1; fallback a validación en aplicación si la restricción no funciona |
| Curva de aprendizaje del equipo en sprint inaugural | Alta | Medio | Capacidad declarada al 70 % del histórico estimado; ajustar al cierre del sprint |

## 8. Trazabilidad
| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-01, CU-02, CU-03 |
| NB que avanzan | NB-01 (autenticación), NB-02 (agenda visible), NB-03 (reserva sin superposición) |
| ADRs implicadas | ADR-01 (estilo), ADR-03 (persistencia relacional) |
```

### 7.2 Ejemplo 2 — Fragmento de `velocidad-equipo_v1.0.md` para una librería de procesamiento CSV

Fragmento ilustrativo, con 5 sprints de tracking:

```markdown
# Velocidad del equipo

**Proyecto:** Librería CSV
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 1.0
**Estado:** En curso
**Fecha:** 2026-08-15

## 1. Por sprint
| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S01 | 18 | 13 | 13 | — | Sprint inaugural; curva de aprendizaje |
| S02 | 16 | 16 | 16 | — | Sin imprevistos |
| S03 | 18 | 17 | 17 | 15,3 | Carry-over de 1 punto a S04 |
| S04 | 20 | 14 | 14 | 15,7 | Outlier: vacaciones de un dev (3 días) |
| S05 | 18 | 19 | 19 | 16,7 | Cierre con BT extra terminado |

## 2. Tendencia
Ascendente y estable a partir del S03. El S04 cae por un outlier explicado y se recupera en S05.

## 3. Capacidad ajustada
Promedio móvil 3 sprints (S03–S05): 16,7 puntos. Tope sugerido para S06: 18 puntos (110 % del promedio).

## 4. Outliers explicados
| Sprint | Velocity | Promedio móvil | Desviación | Causa |
| --- | --- | --- | --- | --- |
| S01 | 13 | — | — | Sprint inaugural, factor de focus subestimado |
| S04 | 14 | 15,7 | -11 % | Vacaciones planificadas de un integrante |
```

Los dos fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar el plan de sprint del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path}}
- Upstream: 06 (backlog técnico priorizado con US y BT), 02 (CU y NB), 05 (arquitectura y ADRs).
- Histórico: velocidad-equipo_v1.0.md si existe.

A generar (según tamaño de equipo):
- plan-iteracion-sprint-XX_v1.0.md para el sprint vigente, con las nueve secciones del §4.2.
- template-sprint-review_v1.0.md y template-sprint-retrospectiva_v1.0.md si todavía no existen.
- velocidad-equipo_v1.0.md actualizado al cierre del sprint anterior.
- mini-plan_v1.0.md si el proyecto es de un solo dev (sustituye a los anteriores).
- README.md de la sección (recomendado).

Reglas de redacción: §4 de 07_rules_plan_sprint.md.
Nomenclatura: `plan-iteracion-sprint-XX_v<X.Y>.md` con un único separador antes de la versión (`_v`).
Convención crítica: prohibido el patrón `plan-iteracion_sprint-XX_v<X.Y>.md` con doble separador; prohibido abrir el archivo con `--` previo al H1.
Sprint goal: una sola frase orientada a valor, sin bullets ni listas.
Trazabilidad: cada US y BT con identificador del backlog de 06; cada sprint declara CU y NB que avanzan.
Calidad: DoD por referencia a la canónica de 08; mínimo dos riesgos con mitigación; toda retrospectiva produce mínimo una acción con responsable y fecha.
Estimación: en story points; declarar la unidad en §1.

Restricciones: no introducir stacks concretos, productos comerciales ni protocolos del dominio fuente. Idioma rioplatense técnico, tildes correctas, sin emojis.

Salida: /SDD2.1D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/07_plan-sprint/<estructura>.
```

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial de las reglas constructivas de la categoría 07. Corrige el patrón heredado con doble separador (`plan-iteracion_sprint-XX_v1.0.md`) y fija el patrón único `plan-iteracion-sprint-XX_v<X.Y>.md`. Define nueve secciones obligatorias del plan de sprint, las plantillas reusables de review y retrospectiva, el tracking de velocidad con promedio móvil de 3 sprints, las variantes por tipo D8 y los criterios de aceptación. Habilita el modo simplificado (`mini-plan_v1.0.md`) para proyectos de un solo dev. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/07_plan-sprint/`; la frase de cierre de §1.2 y la ruta de salida del prompt-snippet referencian el `project_type` del proyecto en curso (manifiesto). Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
