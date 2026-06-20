# Reglas constructivas — 00 Contexto del producto

**Carpeta target:** `/SDD2.1D/docs/00_contexto/`
**Subagente target del orquestador:** Product Manager Senior (AG-00) en conjunción con Analista de Negocio Senior (AG-01) si el proyecto tiene stakeholders múltiples.
**Versión de las reglas:** 1.3

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Product Manager Senior, equivalente a AG-00 del catálogo de especialidades. Define el porqué del proyecto, la visión a mediano plazo, el alcance, el roadmap de fases y las plataformas target. Trabaja desde el SOLUTION-INTAKE como insumo de negocio (Parte A) y de tipo de proyecto (§13). Su responsabilidad principal es completar lo que el cliente todavía no dijo: forzar la priorización MoSCoW, declarar exclusiones explícitas, traducir aspiraciones en objetivos SMART y dejar el contexto listo para que las categorías 01 (NB), 02 (CU), 03 (UX), 05 (arquitectura), 07 (sprint plan) y 11 (examples) puedan trabajar sin volver a preguntarle al cliente.

### 1.2 Variantes según tipo de proyecto

| Tipo (D8) | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Product Manager + Curador de Librería | El foco está en la API surface, los casos de uso del consumidor y la estabilidad de contratos. El roadmap se piensa por versiones de API, no por features de usuario final. |
| web-monolith | Product Manager Senior | Es el caso canónico. Stakeholders mixtos negocio/técnicos, roadmap por épicas, visión orientada a producto SaaS o aplicación interna. |
| web-microservices | Product Manager + Domain Modeler (DDD) | Requiere recortar el dominio en bounded contexts desde la visión para que cada microservicio tenga propietario funcional claro. |
| desktop-app | Product Manager + UX Lead | Las decisiones de visión se cruzan fuerte con la experiencia: instalación, actualización, modo offline, integraciones con el SO. |
| mobile-app-maui | Product Manager + Mobile UX Lead | La compatibilidad de plataformas y el ciclo de release en stores condicionan el roadmap. El UX móvil define qué es viable. |
| rest-api | Product Manager + API Product Owner | Los consumidores son desarrolladores. La visión se expresa por capabilities de la API, contratos versionados y métricas de adopción. |
| cli-tool | Product Manager + Developer Advocate | La audiencia es técnica. La visión enfatiza ergonomía de uso, scripting y documentación ejecutable; el roadmap suele ser ligero. |
| worker-service | Product Manager + Operations Lead | La visión se mide por SLA operativos, throughput y resiliencia. El alcance se define por colas y eventos procesados, no por pantallas. |

Esta categoría se genera a nivel solución, una vez por solución, a partir del SOLUTION-INTAKE único. El orquestador selecciona la variante según el `project_type` del proyecto principal declarado en el manifiesto; la compatibilidad de plataformas agrega las plataformas de todos los proyectos de la solución.

### 1.3 Multi-especialidad

Se combina con AG-01 Analista de Negocio Senior cuando se cumple al menos una de estas condiciones:

- Hay más de tres categorías de stakeholders con intereses divergentes.
- El dominio está regulado (salud, financiero, público) y la visión debe declarar restricciones normativas.
- El proyecto es B2B con múltiples organizaciones cliente que aportan requisitos.
- El SOLUTION-INTAKE §11 declara riesgos de negocio que requieren mitigación con dueño funcional.

En proyectos pequeños (1 a 2 stakeholders, sin regulación), AG-00 trabaja solo. En proyectos de 1 dev sin cliente externo, AG-00 actúa también como AG-01 y el `acuerdo-equipo` queda omitido.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra de documentos

| Archivo | Obligatorio para | Recomendado para | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `vision-producto_v1.0.md` | Todos los tipos D8 | — | — | Por qué existe el sistema, audiencia, propuesta de valor, objetivos SMART, métricas de éxito. |
| `alcance-proyecto_v1.0.md` | Todos los tipos D8 | — | — | Qué entra y qué no entra en el sistema, supuestos, restricciones, criterios de aceptación del proyecto. |
| `roadmap-producto_v1.0.md` | web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, worker-service | cli-tool, library | — | Hitos, fases y criterios de transición entre fases. |
| `compatibilidad-plataformas_v1.0.md` | desktop-app, mobile-app-maui, cli-tool | web-monolith, rest-api, worker-service | library de puro lenguaje sin runtime particular | Versiones de SO, runtimes, navegadores y dispositivos soportados. |
| `acuerdo-equipo_v1.0.md` | Todos los tipos con equipo de más de 2 personas | Equipos de 2 personas que coordinan con stakeholders externos | Proyectos de 1 desarrollador solo, sin equipo | Convenciones de trabajo, ceremonias, herramientas, branching strategy. |

### 2.2 Reglas de inclusión/exclusión por tipo de proyecto

| Tipo (D8) | Genera | Omite |
| --- | --- | --- |
| library | vision-producto, alcance-proyecto, acuerdo-equipo | roadmap-producto (opcional), compatibilidad-plataformas (salvo binding nativo) |
| web-monolith | vision-producto, alcance-proyecto, roadmap-producto, acuerdo-equipo | compatibilidad-plataformas (salvo soporte a navegadores legacy) |
| web-microservices | vision-producto, alcance-proyecto, roadmap-producto, acuerdo-equipo | compatibilidad-plataformas |
| desktop-app | Los 5 documentos | — |
| mobile-app-maui | Los 5 documentos | — |
| rest-api | vision-producto, alcance-proyecto, roadmap-producto, acuerdo-equipo | compatibilidad-plataformas (salvo SDKs cliente) |
| cli-tool | vision-producto, alcance-proyecto, compatibilidad-plataformas | roadmap-producto (opcional si el alcance es bajo), acuerdo-equipo (omitir si es 1 dev) |
| worker-service | vision-producto, alcance-proyecto, roadmap-producto, acuerdo-equipo | compatibilidad-plataformas |

Regla general: si el documento se omite, dejar nota explícita en el README de la sección 00 indicando el motivo y la categoría D8 que justifica la omisión.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

Patrón único: `<nombre-documento>_v<X.Y>.md`. El sufijo `_v<X.Y>` usa guion bajo antes de la letra `v`. Prohibido el patrón `.v<X.Y>.md` (punto) observado en SDD 1.0. Ejemplos válidos: `vision-producto_v1.0.md`, `alcance-proyecto_v1.0.md`, `roadmap-producto_v1.0.md`. Ejemplo inválido: `vision-producto.v1.0.md`.

### 3.2 Convenciones de prefijos / sufijos

No se aplica en esta categoría. Los documentos de contexto no usan prefijos del tipo `NB-XX`, `CU-XX`, `RN-XX`. Solo nombre kebab-case más sufijo de versión.

### 3.3 Vinculación cross-doc (trazabilidad upstream/downstream)

- Upstream: SOLUTION-INTAKE (§1 idea y problema, §3 propuesta de valor, §5 historias de usuario, §9 exclusiones, §10 restricciones del cliente, §11 riesgos, §13 tipo de proyecto, §17 P.9 plataformas target).
- Downstream: alimenta 01 (necesidades de negocio), 02 (casos de uso y reglas), 03 (UX/UI/DX), 05 (arquitectura y ADR), 07 (sprint plan), 11 (examples). La categoría 00 es upstream de toda la cadena de trazabilidad D6.
- Cada documento de esta categoría declara en su cabecera la trazabilidad upstream (qué secciones del SOLUTION-INTAKE originaron su contenido) y downstream (qué categorías consumen sus decisiones).

### 3.4 README de la sección

Recomendado. La carpeta `/SDD2.1D/docs/00_contexto/` lleva un `README.md` (sin versión) que enumera los 5 documentos con su propósito, su estado y el orden de lectura sugerido. Si algún documento fue omitido por aplicación de §2.2, el README declara el motivo. El README también lista los stakeholders del proyecto con nombre o rol.

---

## 4. Estructura de redacción de cada documento

### 4.1 Cabecera obligatoria

Todos los documentos de la categoría 00 arrancan con un bloque markdown estándar:

```markdown
# <Título del documento>

**Proyecto:** <nombre-del-proyecto>
**Documento:** <nombre-archivo>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Propuesto | Aprobado | Vigente | Superado
**Fecha:** <YYYY-MM-DD>
**Autor:** <rol o nombre>
**Trazabilidad upstream:** SOLUTION-INTAKE §<n>
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 05_arquitectura_tecnica, ...
```

### 4.2 Secciones obligatorias

#### vision-producto_v1.0.md

- §1 Problema de negocio.
- §2 Audiencia y stakeholders.
- §3 Propuesta de valor.
- §4 Visión a 3 años.
- §5 Objetivos SMART.
- §6 Métricas de éxito.
- §7 Restricciones.
- §8 Riesgos.
- §9 Glosario del dominio.
- §10 Trazabilidad.

#### alcance-proyecto_v1.0.md

- §1 Propósito.
- §2 Descripción general.
- §3 Objetivos del proyecto.
- §4 Alcance incluido (capacidades, entregables, ambientes).
- §5 Alcance excluido (con justificación por exclusión).
- §6 Supuestos.
- §7 Restricciones.
- §8 Criterios de aceptación del proyecto.
- §9 Gestión de cambios de alcance.
- §10 Trazabilidad.

#### roadmap-producto_v1.0.md

- §1 Propósito.
- §2 Fases del producto (objetivo, épicas asociadas, entregable, release target).
- §3 Matriz fase → épica → sprint → release.
- §4 Dependencias entre fases.
- §5 Criterios de transición entre fases.
- §6 Trazabilidad downstream a 06 backlog y 07 sprint plan.

#### compatibilidad-plataformas_v1.0.md

- §1 Resumen ejecutivo.
- §2 Matriz de compatibilidad (componente x plataforma).
- §3 Restricciones de plataforma justificadas.
- §4 Alternativas para plataformas no soportadas.
- §5 Estado de implementación por plataforma.
- §6 Trazabilidad downstream a 09 DevOps.

#### acuerdo-equipo_v1.0.md

- §1 Propósito.
- §2 Equipo y roles (Scrum o el modelo de gestión adoptado).
- §3 Cadencia de ceremonias.
- §4 Acuerdos de trabajo (branching, code review, comunicación, horario core, documentación, convenciones de commits).
- §5 Definition of Done (referencia a 08).
- §6 Definition of Ready (referencia a 06).
- §7 Herramientas.

### 4.3 Secciones opcionales según tipo de proyecto

- Sección "NFR de compatibilidad" se agrega en `alcance-proyecto` solo si el tipo es `mobile-app-maui`, `desktop-app` o `cli-tool`.
- Sección "Modelo de licenciamiento" se agrega en `vision-producto` solo si el tipo es `library` o si el proyecto es comercial.
- Sección "Modelo operativo y SLA" se agrega en `vision-producto` solo si el tipo es `worker-service` o `rest-api` con consumidores externos.
- Sección "Estrategia de internacionalización" se agrega en `alcance-proyecto` solo si el BRIEF §2 declara audiencia en más de una región lingüística.

### 4.4 Tablas tipo y formatos recurrentes

| Tabla | Documento | Columnas |
| --- | --- | --- |
| Stakeholders | vision-producto §2 | Rol, nombre o cargo, categoría (propietario/implementador/beneficiario), nivel de involucramiento, responsabilidad principal |
| Objetivos SMART | vision-producto §5 | Objetivo, métrica, target numérico, plazo, responsable |
| Métricas de éxito | vision-producto §6 | Criterio, métrica, target, plazo, fuente del dato |
| Riesgos | vision-producto §8 | ID, riesgo, probabilidad, impacto, mitigación, responsable |
| Hitos del roadmap | roadmap-producto §2 | Fase, objetivo, épicas, sprints estimados, entregable, release target |
| Criterios de transición | roadmap-producto §5 | Fase origen, fase destino, criterios verificables `- [ ]` |
| Matriz de compatibilidad | compatibilidad-plataformas §2 | Componente, plataforma 1, plataforma 2, ..., notas |
| Exclusiones | alcance-proyecto §5 | Funcionalidad excluida, justificación, versión futura tentativa |
| Glosario | vision-producto §9 | Término, definición, sinónimos o notas |
| Ceremonias | acuerdo-equipo §3 | Ceremonia, cuándo, duración, participantes, notas |

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Visión técnica en lugar de visión de negocio | La visión menciona stack, frameworks o patrones; queda inútil para alinear stakeholders no técnicos | Reescribir en lenguaje del cliente, mover el stack al SOLUTION-INTAKE (§17 P.1) |
| Alcance sin exclusiones explícitas | Sin "fuera de alcance" declarado, el scope creep es inevitable | Forzar mínimo 3 exclusiones con justificación |
| Roadmap sin criterios para reordenar | Fases listadas sin criterios de salida; nunca se sabe si una fase terminó | Cada fase tiene §5 con checklist verificable de transición |
| Objetivos sin métrica numérica | "Que sea rápido", "que tenga buena UX" no se pueden validar | Cada objetivo en formato SMART con target numérico y plazo |
| Stakeholders genéricos | "Los usuarios", "el equipo", "la empresa" no tienen dueño | Forzar nombre o rol concreto por stakeholder |
| Compatibilidad enumerada sin justificación | "Soportamos Android e iOS" sin explicar versiones mínimas ni motivos | Tabla con componente, plataforma, versión mínima y motivo |
| Acuerdo de equipo aspiracional | "Vamos a comunicarnos bien" no es verificable | Cada acuerdo se redacta como regla operativa (canal, horario, SLA) |
| Glosario universal | Definir "API" o "framework" en el glosario no aporta | Solo términos del dominio del cliente |

---

## 5. Preguntas guía para el subagente

### 5.1 Comprensión del input upstream

- ¿Qué dice el SOLUTION-INTAKE §1 sobre el problema y cuál es la consecuencia si no se construye?
- ¿Qué dice el SOLUTION-INTAKE §3 sobre propuesta de valor y diferenciación frente a alternativas existentes?
- ¿Qué dice el SOLUTION-INTAKE §9 sobre exclusiones declaradas por el cliente?
- ¿Qué dice el SOLUTION-INTAKE §10 sobre restricciones del cliente (presupuesto, fecha, normativa, integración obligatoria)?
- ¿Qué dice el SOLUTION-INTAKE §11 sobre riesgos detectados desde el negocio?
- ¿El SOLUTION-INTAKE §13 declara un tipo de proyecto que requiera `compatibilidad-plataformas_v1.0.md`?
- ¿El SOLUTION-INTAKE §17 P.9 enumera plataformas target que deban reflejarse en compatibilidad?

### 5.2 Decisiones de scope

- ¿Qué entra en la visión a 3 años y qué se posterga explícitamente?
- ¿Hay capacidades pedidas por el cliente en §4 del BRIEF que se descartan? ¿Por qué?
- ¿El alcance está balanceado contra el plazo y presupuesto declarados en §10 del BRIEF?
- ¿La cantidad de Must Have del BRIEF §4 cabe en el plazo objetivo? Si no, ¿qué se renegocia con el cliente?

### 5.3 Trazabilidad

- Cada objetivo SMART de la visión, ¿se va a poder medir desde 08 Calidad?
- Cada restricción de plataforma, ¿se refleja en 09 DevOps (matriz SO/runtime/CI)?
- Cada fase del roadmap, ¿se mapea a épicas conocidas o pendientes de definir en 06 backlog?
- Cada exclusión del alcance, ¿queda registrada para que 02 CU no la genere por error?

### 5.4 Calidad

- ¿Las exclusiones del alcance tienen justificación explícita (no solo "fuera de alcance")?
- ¿Los hitos del roadmap tienen fecha objetivo o iteración objetivo? Sin uno de los dos, el hito no se puede cerrar.
- ¿El glosario cubre los 10 términos más usados en NB y CU upstream esperados?
- ¿Los stakeholders están con nombre o rol concreto, sin genéricos?
- ¿El acuerdo de equipo declara herramientas, canales y SLA de respuesta?

---

## 6. Criterios de aceptación del entregable

- [ ] La visión expresa el problema en lenguaje de negocio, sin mencionar stack, frameworks ni patrones de implementación.
- [ ] El alcance enumera al menos 5 capacidades incluidas y 3 exclusiones explícitas con justificación.
- [ ] El roadmap tiene al menos 3 hitos con criterios de avance verificables tipo `- [ ]`.
- [ ] La sección de objetivos SMART tiene al menos 3 objetivos con métrica numérica, target y plazo.
- [ ] La sección de stakeholders tiene mínimo 1 representante por categoría (propietario, implementador, beneficiario) con rol concreto.
- [ ] El glosario del dominio tiene mínimo 10 términos en proyectos con equipo de más de 2 personas, o mínimo 5 términos en proyectos individuales.
- [ ] La compatibilidad-plataformas declara todas las plataformas target listadas en el SOLUTION-INTAKE §17 P.9 cuando aplica por tipo D8.
- [ ] El acuerdo-equipo declara herramientas, ceremonias, branching strategy y SLA de respuesta cuando aplica.
- [ ] Cada documento de la carpeta declara su trazabilidad upstream (SOLUTION-INTAKE con secciones específicas) y downstream (categorías 01, 02, 05, 07, 11 con detalle).
- [ ] El nombre de cada archivo respeta el patrón `<kebab>_v1.0.md` con guion bajo antes de la versión.
- [ ] Ningún documento contiene emojis, negritas decorativas ni referencias hardcoded a stack, frameworks o ejemplos del dominio fuente del bootstrap.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — Sistema de turnos médicos (web-monolith)

Fragmento representativo de `vision-producto_v1.0.md` para un sistema de turnos médicos:

```markdown
# Visión de Producto

**Proyecto:** turnos-medicos-clinica
**Documento:** vision-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-04-10
**Autor:** Product Manager
**Trazabilidad upstream:** SOLUTION-INTAKE §1, §3, §10
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 05_arquitectura_tecnica

## 1. Problema de negocio

Los pacientes de la clínica solo pueden reservar turnos llamando por teléfono
en horario de atención, lo que satura la centralita y produce esperas de 15
a 30 minutos. Si no se resuelve, la clínica pierde pacientes contra
competidores que ya ofrecen reserva online.

## 5. Objetivos SMART

| Objetivo | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Reservas digitales | % de turnos iniciados online sobre el total | ≥ 40% | 6 meses post-lanzamiento |
| Reducción de llamadas | Llamadas/día a centralita | ≤ 80 (vs 200 hoy) | 9 meses post-lanzamiento |
| Satisfacción del paciente | Promedio encuesta post-turno (1-5) | ≥ 4.0 | Continuo, revisión trimestral |
```

### 7.2 Ejemplo 2 — Librería de parsing CSV (library)

Fragmento representativo de `alcance-proyecto_v1.0.md` para una librería utilitaria:

```markdown
# Alcance del Proyecto

**Proyecto:** csv-parser-lib
**Documento:** alcance-proyecto_v1.0.md
**Versión:** 1.0
**Estado:** Aprobado
**Fecha:** 2026-04-12
**Autor:** Product Manager + Curador de Librería
**Trazabilidad upstream:** SOLUTION-INTAKE §4, §9
**Trazabilidad downstream:** 02_especificacion_funcional, 11_examples

## 4. Alcance incluido

### 4.1 API surface

- Lectura de archivos CSV con delimitador configurable.
- Mapeo a tipos del consumidor mediante reflexión y atributos.
- Reporte de filas con error sin abortar la lectura.
- Escritura inversa desde colección a CSV.

### 4.2 Casos de uso del consumidor

- Importación masiva en backoffice.
- Procesamiento batch en pipelines de datos.
- Validación previa a carga en base relacional.

## 5. Alcance excluido

| Funcionalidad excluida | Justificación | Versión futura |
| --- | --- | --- |
| Lectura desde URL remota | Acopla la librería a HTTP; se delega al consumidor | Backlog v1.5 |
| Detección automática de encoding | Costo alto vs valor; la mayoría de consumidores conoce su encoding | v2.0 |
| Streaming reactivo (IObservable) | Audiencia objetivo no lo pidió; agrega dependencias | No planificado |
```

---

## 8. Prompt-snippet sugerido para el subagente

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar los documentos de contexto del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path-al-intake}}
- Documentos upstream: ninguno (sos el inicio de la cadena de trazabilidad D6).

Documentos a generar (según tipo de proyecto {{TIPO}} y reglas de §2.2):
- vision-producto_v1.0.md (obligatorio para todos los tipos)
- alcance-proyecto_v1.0.md (obligatorio para todos los tipos)
- roadmap-producto_v1.0.md (según matriz §2.2)
- compatibilidad-plataformas_v1.0.md (según matriz §2.2)
- acuerdo-equipo_v1.0.md (si equipo > 2 personas)
- README.md de la sección 00 (recomendado, sin versión)

Reglas de redacción: §4 de 00_rules_contexto.md.
Trazabilidad esperada: declarar upstream a SOLUTION-INTAKE y downstream a 01, 02, 03, 05, 07, 11.
Criterios de calidad: §6 de 00_rules_contexto.md (11 ítems verificables).
Restricciones: idioma rioplatense técnico, UTF-8 LF, sin emojis, sin negritas decorativas, sin referencias a stack en visión y alcance.

Salida: /SDD2.1D/docs/00_contexto/<archivos>_v1.0.md.
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.1 |
| 1.1 | 2026-06-08 | Higiene D7 (hallazgo H-02 de la matriz de coherencia ST-01): el criterio de aceptación de §6 deja de nombrar el dominio fuente ("Motor DSL") y pasa a referirse a "ejemplos del dominio fuente del bootstrap". |
| 1.2 | 2026-06-09 | Validación ST-06: aclaración de que la categoría 00 se genera a nivel solución desde el PROJECT-BRIEF único y usa la variante §1.2 del proyecto principal del manifiesto. |
| 1.3 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
