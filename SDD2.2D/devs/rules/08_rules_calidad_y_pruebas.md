# Reglas constructivas — 08 Calidad y pruebas

**Carpeta target (por proyecto):** `/SDD2.2D/docs/proyectos/<nombre-proyecto-kebab>/08_calidad_y_pruebas/`
**Subagente target del orquestador:** Ingeniero QA / SDET Senior (AG-08)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.2

La categoría 08 ancla la disciplina de validación del sistema. Recibe upstream de 02 (casos de uso con criterios de aceptación Given-When-Then y reglas de negocio), de 05 (arquitectura, componentes, contratos y NFR con métricas numéricas), de 06 (Definition of Ready de las US y backlog técnico) y de 07 (sprint goals y compromisos por iteración). Alimenta a 09 (quality gates ejecutados en el pipeline CI/CD), a 10 (developer guide de testing y convenciones de tests del repositorio) y a 11 (examples ejecutables que se validan mediante los mismos casos de prueba).

Esta categoría es **obligatoria para los ocho tipos D8**. Ningún proyecto está exento de declarar su estrategia de calidad, su pirámide de testing, su matriz de cobertura y su Definition of Done. El nivel de exhaustividad y la mezcla de artefactos cambia por tipo, pero la categoría siempre existe.

La auditoría de Fase 0 (`_bootstrap/audit-sdd1.md`) detectó dos déficits del fuente SDD 1.0 que SDD 2.2 corrige aquí. Primero, los artefactos del fuente usan un sufijo de dominio (por ejemplo `estrategia-testing-<dominio>_v1.0.md`, `criterios-validacion-<dominio>_v1.0.md`, `estrategia-calidad-<dominio>_v1.0.md`) que ata el nombre al producto particular. SDD 2.2 unifica todos los nombres con el sufijo `_v<X.Y>.md` sin marcador de dominio. Segundo, el material teórico de calidad estaba ausente en `/references/` y debía deducirse de los `docs/`; estas reglas formalizan el marco.

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Ingeniero QA / SDET Senior, equivalente al AG-08 del catálogo SDD. Perfil profesional que diseña, implementa y mantiene la estrategia de calidad del software, define quality gates en CI/CD y articula el equilibrio entre niveles de testing. No se limita a la ejecución manual: trata cada caso de prueba como un artefacto de software con trazabilidad explícita a CU, RN y NFR, con setup determinista y con expected output verificable. Se alinea con ISTQB para vocabulario de testing, con IEEE 829 para la especificación de planes y casos de prueba, con ISO/IEC 25010 para atributos de calidad, con la pirámide de testing de Martin Fowler para distribución de esfuerzo y con la Definition of Done de la Scrum Guide 2020.

Combina dos facetas que históricamente estaban separadas. La cara QA define qué probar, el enfoque por niveles, los quality gates y los criterios de aceptación. La cara SDET implementa frameworks de automatización, fixtures reutilizables, snapshot baselines, integración en CI y reportes de cobertura. Cuando el sistema tiene SLAs estrictos o exposición pública, el rol se extiende a testing de performance (load, stress, soak) y de seguridad (OWASP ZAP, threat modeling, fuzz), siempre con métricas numéricas verificables.

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Justificación |
| --- | --- | --- |
| library | QA + SDET Library | Mucho test unitario sobre la superficie pública, contract tests por interfaz, property-based testing para invariantes y mutation testing para validar la calidad de los tests. La librería no tiene UI ni ambiente, sólo entradas y salidas. |
| web-monolith | QA + SDET (pirámide clásica) | Pirámide 70 unit, 20 integración, 10 e2e. UI testing con un framework headless, integración contra base de datos efímera. |
| web-microservices | QA + Contract Testing Specialist | Pact o equivalente para contract testing consumer-driven entre servicios. E2E reducidos a journeys críticos. Tests de integración por servicio con dependencias mockeadas en el contrato. |
| desktop-app | QA + UI Testing Specialist | Automatización con framework adecuado al runtime de escritorio: Appium para multi-plataforma, FlaUI o equivalente para Windows. Snapshot testing de vistas relevantes. |
| mobile-app-maui | QA + Mobile Testing Specialist | UI testing con el framework móvil correspondiente, incluyendo .NET MAUI UI Testing cuando el tipo lo amerita. Snapshot de pantallas críticas, pruebas de orientación y de ciclo de vida. |
| rest-api | QA + API Testing Specialist | REST Assured o cliente HTTP equivalente, validación de OpenAPI contra implementación, Schemathesis o fuzz testing de endpoints, contract tests con consumidores. |
| cli-tool | QA + CLI Testing | bats o framework equivalente, snapshot del output de stdout y stderr, validación de exit codes, tests por subcomando y por combinación de flags. |
| worker-service | QA + Event Testing | Consumer tests por tipo de mensaje, validación de idempotencia, simulación de replay y de mensajes envenenados, tests de dead-letter y de orden cuando aplica. |

El orquestador lee esta tabla y, según el `project_type` del proyecto en curso (leído del manifiesto de solución), selecciona la variante correspondiente y la combina con la especialidad base. La variante se aplica una vez por cada proyecto de la solución.

### 1.3 Multi-especialidad

La categoría 08 colabora con varias especialidades durante la redacción y la revisión:

- AG-02 Analista Funcional, para validar que cada CU tiene al menos un caso de prueba que cubre cada criterio Given-When-Then declarado.
- AG-05 Arquitecto, para validar que cada NFR con objetivo numérico tiene un test que lo verifica (latencia, throughput, disponibilidad).
- AG-06 Scrum Master, para que la Definition of Done quede referenciada desde el backlog y desde el DoR, sin redefinirse en cada sprint.
- AG-09 DevOps, para que los quality gates declarados en 08 se materialicen como stages del pipeline en 09.
- AG-10 Technical Writer, para que la guía de testing del repositorio en 10 cite y respete la estrategia definida acá.

El AG-08 mantiene titularidad de los artefactos. Las demás especialidades aportan revisión sectorial y consumen los criterios.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra

| Archivo | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `estrategia-calidad_v<X.Y>.md` | Todos los tipos D8 | — | — | Definición de calidad para el proyecto, atributos priorizados (ISO 25010), quality gates, roles y cadencia. |
| `estrategia-testing_v<X.Y>.md` | Todos los tipos D8 | — | — | Pirámide de testing, cobertura mínima por capa, tooling, datos de prueba, ambiente. |
| `plan-pruebas_v<X.Y>.md` | Todos los tipos D8 | — | — | Alcance del plan, criterios de entrada y salida, riesgos de calidad, plan por sprint, recursos. |
| `matriz-cobertura-pruebas_v<X.Y>.md` | Todos los tipos D8 | — | — | Trazabilidad CU↔tests, NFR↔tests, RN↔tests. |
| `casos-prueba-referenciales_v<X.Y>.md` | Todos los tipos D8 | — | — | Catálogo de TC-XX con setup, pasos, expected, actual y status. |
| `criterios-validacion_v<X.Y>.md` | Todos los tipos D8 | — | — | Criterios numéricos que permiten declarar al sistema validado para release. |
| `definition-of-done_v<X.Y>.md` | Todos los tipos D8 | — | — | DoD por capa (US, BT, sprint, release) con criterios verificables y excepciones admitidas. |
| `guia-testing-extensibilidad_v<X.Y>.md` | library con plugins, web-microservices con plugins | cli-tool con plugins, rest-api con handlers externos | Tipos sin puntos de extensión | Cómo testear plugins, extensiones y handlers externos sin modificar el núcleo. |
| `README.md` de la sección | Recomendado para todos | — | — | Índice navegable de los artefactos de calidad. |

### 2.2 Reglas de inclusión y exclusión por tipo

| Tipo D8 | Pirámide objetivo (unit/integ/e2e+snapshot) | Cobertura mínima por capa | Guía de extensibilidad |
| --- | --- | --- | --- |
| library | 80 / 15 / 5 | 85 % dominio, 70 % infraestructura, mutation score >= 60 % en dominio | Sí, si expone plugins o extensiones |
| web-monolith | 70 / 20 / 10 | 80 % aplicación, 70 % infraestructura, 60 % presentación | No (salvo motor de extensión interno) |
| web-microservices | 65 / 25 / 10 (contract tests cuentan como integración) | 80 % por servicio en dominio, 70 % en infraestructura | Sí, si la plataforma admite plugins por servicio |
| desktop-app | 70 / 15 / 15 (UI tests en e2e) | 75 % lógica, 60 % presentación | No (salvo plugins) |
| mobile-app-maui | 70 / 15 / 15 (UI tests en e2e) | 75 % lógica, 60 % presentación | No (salvo plugins) |
| rest-api | 70 / 20 / 10 | 80 % aplicación, 70 % infraestructura, 100 % de endpoints cubiertos por contract test | Sí, si admite handlers o middlewares externos |
| cli-tool | 75 / 15 / 10 (snapshot CLI en e2e) | 80 % comandos, 100 % de subcomandos con snapshot de stdout/stderr y exit code | Sí, si admite plugins |
| worker-service | 70 / 25 / 5 | 80 % handlers, 100 % de tipos de mensaje cubiertos, idempotencia testeada por handler | Sí, si la plataforma admite handlers externos |

Los porcentajes son piso, no techo. El equipo puede subir cobertura cuando el dominio lo exige, pero no bajarla sin un ADR que lo justifique.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `estrategia-calidad_v<X.Y>.md` para la estrategia transversal.
- `estrategia-testing_v<X.Y>.md` para la pirámide y el tooling.
- `plan-pruebas_v<X.Y>.md` para el plan operativo.
- `matriz-cobertura-pruebas_v<X.Y>.md` para la trazabilidad.
- `casos-prueba-referenciales_v<X.Y>.md` para el catálogo de TC-XX.
- `criterios-validacion_v<X.Y>.md` para los criterios de release.
- `definition-of-done_v<X.Y>.md` para la DoD canónica del proyecto.
- `guia-testing-extensibilidad_v<X.Y>.md` cuando aplica.
- `README.md` para el índice de la sección.

El sufijo `_v<X.Y>.md` es uniforme. Queda prohibido el patrón heredado `.v<X.Y>`. Queda prohibido el sufijo de dominio `-motor` u otros marcadores temáticos en el nombre de archivo (por ejemplo `estrategia-testing-motor_v1.0.md`, `estrategia-calidad-motor_v1.0.md` o `criterios-validacion-motor_v1.0.md` están explícitamente vedados). El nombre del artefacto describe el rol del documento, no el dominio del proyecto.

### 3.2 Convenciones de identificadores internos

- `TC-XX`: caso de prueba referencial. Numeración contigua con dos dígitos, kebab opcional para el slug interno cuando se nombra (`TC-01-parsing-feliz`).
- `NFR-XX`: las NFR provienen de 05; en 08 sólo se referencian.
- `CU-XX`, `RN-XX`: provienen de 02; en 08 sólo se referencian.

### 3.3 Vinculación cross-doc

- Upstream: cada caso de prueba referencia el CU o NFR que valida; cada criterio de validación referencia la NFR que mide; la DoD referencia los CU mínimos y los NFR críticos.
- Downstream: los quality gates de 09 ejecutan los tests categorizados en 08; la developer guide de 10 detalla cómo se corren los tests definidos acá; los examples de 11 incluyen al menos un test ejecutable por ejemplo.
- La matriz de cobertura es el documento bisagra: relaciona CU↔TC, NFR↔TC y RN↔TC en tres tablas explícitas.

### 3.4 Política de versionado

Una sola versión vigente por nombre lógico. Cuando se pasa de `v1.0` a `v2.0` la versión anterior se mueve a `_legacy/` con estado `Superado`. La DoD es especialmente sensible: cualquier cambio en sus criterios versionables debe quedar registrado en §9 del propio documento y comunicado al equipo en el sprint review siguiente.

### 3.5 README de la sección

Recomendado para todos los tipos. Lista los artefactos vigentes, el estado de cada uno, los quality gates configurados en CI y el enlace a la DoD canónica del proyecto.

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
```

### 4.2 Estructura de `estrategia-calidad_v1.0.md`

1. Definición de calidad para el proyecto. Qué significa "el sistema tiene calidad" en este contexto, en una a tres oraciones que respondan al alcance funcional y al perfil de riesgo.
2. Atributos de calidad priorizados según ISO/IEC 25010. Funcionalidad, eficiencia de desempeño, compatibilidad, usabilidad, fiabilidad, seguridad, mantenibilidad, portabilidad. Cada atributo tiene prioridad declarada y, cuando corresponde, métrica numérica con su NFR de origen.
3. Quality gates. Conjunto de criterios mecánicos que el pipeline aplica antes de declarar un build, una rama o un release como aceptable. Cada gate especifica condición, herramienta y consecuencia.
4. Roles QA dentro del equipo. Quién diseña tests, quién los ejecuta, quién aprueba el release. RACI explícito si el equipo lo amerita.
5. Cadencia de revisión. Frecuencia con la que se actualiza la estrategia, sus métricas, sus umbrales y su cobertura.

### 4.3 Estructura de `estrategia-testing_v1.0.md`

1. Pirámide de testing deseada. Distribución porcentual entre unit, integration, e2e y snapshot. Justificación contra la pirámide invertida (e2e pesado) y contra la pirámide aplanada (cobertura cuantitativa sin distinguir capas).
2. Cobertura mínima por capa. Tabla con valores numéricos por capa (dominio, aplicación, infraestructura, presentación) y mutation score si aplica.
3. Tooling. Frameworks elegidos por nivel y por tipo de test, sin atar el documento a productos comerciales concretos.
4. BDD si aplica. Cuándo se usan especificaciones Given-When-Then ejecutables y dónde viven los archivos `.feature` o equivalentes.
5. Mocks y fixtures. Política de aislamiento, reuso, versionado y control de duplicación.
6. Datos de prueba. Origen (sintéticos, anonimizados, snapshot de producción), versionado, mecanismo de regeneración.
7. Ambiente de testing. Aislamiento entre tests, base de datos efímera, contenedores, variables de entorno y secretos no productivos.

### 4.4 Estructura de `plan-pruebas_v1.0.md`

1. Alcance del plan. Sprints o release que cubre, módulos incluidos, módulos excluidos.
2. Criterios de entrada. Qué tiene que estar listo para que el plan se ejecute (build verde, datos disponibles, ambiente desplegado).
3. Criterios de salida. Qué tiene que cumplirse para declarar el plan ejecutado con éxito (cobertura mínima alcanzada, defectos blockers cerrados, NFR validados).
4. Riesgos de calidad. Cada riesgo con impacto, probabilidad y mitigación, alineado con riesgos arquitectónicos de 05.
5. Plan por sprint. Tabla con sprint, alcance de testing, recursos y entregables.
6. Recursos. Personas, ambientes, datasets y herramientas necesarios.

### 4.5 Estructura de `matriz-cobertura-pruebas_v1.0.md`

1. Propósito y alcance.
2. Tabla CU↔Tests. Cada CU con sus criterios Given-When-Then, el TC-XX que lo cubre y el estado del test.
3. Tabla NFR↔Tests. Cada NFR con su SLA numérico, el test que lo valida y el tooling de medición.
4. Tabla RN↔Tests. Cada RN con el TC-XX que verifica su cumplimiento.
5. Tabla de cobertura por capa. Líneas, branches y mutation score por capa.
6. Gaps identificados. Áreas sin cobertura suficiente con plan de remediación.

### 4.6 Estructura de `casos-prueba-referenciales_v1.0.md`

Catálogo de TC-XX. Para cada caso:

1. Identificador y nombre kebab.
2. Tipo (unit, integration, e2e, snapshot, contract, property-based).
3. CU, NFR o RN cubierto.
4. Setup. Estado inicial requerido, fixtures cargados.
5. Pasos. Acción concreta, idealmente expresable como Given-When-Then.
6. Expected output. Resultado verificable.
7. Actual output. Resultado observado en la última ejecución.
8. Status. Verde, rojo, pendiente, deshabilitado con motivo.

### 4.7 Estructura de `criterios-validacion_v1.0.md`

1. Propósito. Qué define "sistema validado para release".
2. Criterios funcionales. Cada CU crítico cubierto y verde.
3. Criterios no funcionales. Cada NFR cumple su SLA medido en el ambiente de pruebas equivalente al productivo.
4. Criterios de regresión. Suite de regresión ejecutada y verde; ningún test verde en la versión anterior pasó a rojo sin justificación.
5. Criterios de calidad de código. Cobertura por capa cumplida; mutation score cumplido si aplica; análisis estático sin warnings nuevos.
6. Excepciones documentadas. Cualquier criterio no cumplido se acepta sólo con ADR explícita y plan de remediación.

### 4.8 Estructura de `definition-of-done_v1.0.md`

1. DoD por capa. Cuatro subsecciones: US (historia de usuario), BT (tarea técnica), sprint y release. Cada capa tiene su lista de criterios `- [ ]` verificables mecánicamente. Cada criterio responde a la pregunta "¿cómo se valida?" con una operación concreta (un comando, un check del pipeline, una métrica del reporte).
2. Excepciones admitidas. Cuándo se puede declarar Done sin cumplir un criterio (por ejemplo, deuda técnica documentada en backlog con BT explícito).
3. Vigencia. Este documento es la fuente canónica. Los sprint plans referencian la DoD, no la redefinen.

### 4.9 Tablas tipo y formatos recurrentes

Pirámide de testing con porcentajes objetivo:

| Nivel | Qué cubre | Tooling | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unit | Lógica aislada por unidad cohesiva | <framework unitario> | 70 % |
| Integration | Interacción entre componentes, persistencia, contratos | <framework de integración> | 20 % |
| Snapshot | Output estable (CLI, render, payload) | <framework de snapshot> | 5 % |
| E2E | Journey crítico end-to-end | <framework e2e> | 5 % |

Trazabilidad CU↔criterio↔test:

| CU | Criterio Given-When-Then | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-XX | Given <contexto>, When <acción>, Then <resultado> | TC-XX | Unit / Integration / E2E | Verde / Rojo / Pendiente |

Trazabilidad NFR↔SLA↔test:

| NFR | SLA | Test | Tooling |
| --- | --- | --- | --- |
| NFR-XX | <valor numérico> | TC-XX | <herramienta de medición> |

Cobertura por capa:

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio | <valor> | <valor> | <valor> | 85 / 80 / 60 |
| Aplicación | <valor> | <valor> | <valor> | 80 / 70 / — |
| Infraestructura | <valor> | <valor> | <valor> | 70 / 60 / — |
| Presentación | <valor> | <valor> | <valor> | 60 / 50 / — |

### 4.10 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Cobertura como número global sin distinguir capas | "85 % cobertura" puede esconder 100 % en getters y 30 % en dominio | Reportar cobertura por capa con umbrales diferenciados |
| Tests sin trazabilidad a CU, RN o NFR | No se puede responder qué requisito valida cada test | Cada TC referencia al menos un CU, RN o NFR |
| Snapshot test sin política de regeneración controlada | Los snapshots se "regeneran" para que pasen, perdiendo su valor | Regeneración requiere PR con justificación y revisión |
| E2E pesado sustituyendo unit tests | Suite lenta, frágil y de difícil diagnóstico | Mantener proporción declarada en la pirámide |
| DoD sin métricas verificables | "El rendimiento es aceptable" no se puede chequear en CI | Cada criterio DoD tiene una operación mecánica que lo valida |
| Falta de prueba de regresión cuando se cierra un bug | El bug puede volver y nadie se entera | Todo bug cerrado genera al menos un TC nuevo o extiende uno existente |
| Tests sin assert | Ejecutan código pero no verifican nada | Cada test tiene al menos una verificación explícita |
| Coverage como meta única | 100 % de cobertura con tests triviales no mejora la calidad | Priorizar edge cases, mutation testing y trazabilidad sobre el número |
| DoD redefinida en cada sprint plan | Inconsistencia y dilución del concepto | DoD canónica única referenciada desde los sprint plans |
| Matriz desactualizada | Dice "Pendiente" pero hay 50 tests implementados | Actualizar la matriz al cierre de cada sprint |

---

## 5. Preguntas guía para el subagente

### 5.1 Input upstream

- ¿Cada CU de 02 con criterios Given-When-Then tiene un caso de prueba en `casos-prueba-referenciales` que cubra cada uno de esos criterios?
- ¿Cada NFR declarada en 05 con objetivo numérico tiene un test que lo mide y un tooling identificado en `matriz-cobertura-pruebas`?
- ¿La DoR de 06 exige criterios de aceptación expresados en Given-When-Then, de manera que la US sea testeable desde su definición?
- ¿Los sprint goals de 07 declaran qué módulos entran en alcance de testing en cada iteración?

### 5.2 Scope

- ¿La pirámide objetivo es adecuada al tipo D8 del proyecto y a la variante de especialidad activa?
- ¿Hay capas en la arquitectura que no tienen umbral de cobertura definido?
- ¿La guía de testing de extensibilidad existe si y sólo si el tipo D8 admite plugins o handlers externos?

### 5.3 Trazabilidad

- ¿Cada test referencia un CU, RN o NFR explícito?
- ¿La matriz de cobertura tiene las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests)?
- ¿Cada criterio del DoD tiene un mecanismo de validación nombrado?
- ¿Existen tests huérfanos (sin upstream) o requisitos huérfanos (sin test)?

### 5.4 Calidad de los tests

- ¿Cada caso de prueba es determinista, reproducible y no depende de orden de ejecución?
- ¿Los snapshots tienen política de regeneración con revisión?
- ¿Los mocks y fixtures están centralizados, versionados y se reutilizan entre tests?
- ¿Los tests fallan rápido cuando se viola una invariante (fail-fast) o se enmascaran errores con catch silenciosos?
- ¿Cada bug cerrado generó al menos un test de regresión que lo previene?

---

## 6. Criterios de aceptación

- [ ] Existe `estrategia-calidad_v1.0.md` con atributos ISO 25010 priorizados y quality gates declarados.
- [ ] Existe `estrategia-testing_v1.0.md` con pirámide objetivo numérica, cobertura mínima por capa y tooling declarado.
- [ ] Existe `plan-pruebas_v1.0.md` con criterios de entrada, salida y riesgos por sprint.
- [ ] Existe `matriz-cobertura-pruebas_v1.0.md` con las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más la tabla de cobertura por capa.
- [ ] Existe `casos-prueba-referenciales_v1.0.md` con al menos un TC por CU crítico, con setup, pasos, expected y status.
- [ ] Existe `criterios-validacion_v1.0.md` con criterios numéricos que permiten declarar al sistema validado.
- [ ] Existe `definition-of-done_v1.0.md` con DoD por capa (US, BT, sprint, release), cada criterio verificable mecánicamente y excepciones explícitas.
- [ ] Si el tipo D8 admite plugins, existe `guia-testing-extensibilidad_v1.0.md`.
- [ ] Ningún archivo lleva sufijo de dominio (`-motor` u otros marcadores temáticos); todos siguen el patrón `_v<X.Y>.md`.
- [ ] Cada NFR con objetivo numérico tiene un test asociado en la matriz.
- [ ] Cada caso de prueba referencia explícitamente al menos un CU, RN o NFR.
- [ ] La DoD no se redefine en sprint plans; los sprint plans referencian este documento.
- [ ] La cobertura se reporta por capa, no como número global único.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — `estrategia-testing_v1.0.md` para una librería de parsing CSV

Fragmento ilustrativo, no documento completo:

```markdown
# Estrategia de testing — Librería de parsing CSV

**Proyecto:** {{nombre-proyecto}}
**Documento:** estrategia-testing_v1.0.md
**Versión:** 1.0
**Estado:** Aprobado
**Fecha:** 2026-05-17

## 1. Pirámide de testing deseada

| Nivel | Cobertura objetivo | Justificación |
| --- | --- | --- |
| Unit | 80 % | La librería es pura transformación entrada-salida; la mayoría del valor se valida con tests unitarios sobre la superficie pública. |
| Integration | 15 % | Validan combinaciones de opciones (delimitador, encoding, quoting) y manejo de errores en streams reales. |
| Snapshot | 5 % | Fijan el output de parsing para datasets representativos y detectan regresiones estructurales. |

E2E no aplica: la librería no tiene UI ni ambiente desplegado.

## 2. Cobertura mínima por capa

| Capa | Líneas | Branches | Mutation score |
| --- | --- | --- | --- |
| Dominio (parser, tokenizer) | 90 % | 85 % | 70 % |
| Infraestructura (IO, encoding) | 75 % | 65 % | — |
| API pública (Abstractions) | 100 % | 90 % | 75 % |

## 3. Tooling

| Nivel | Framework |
| --- | --- |
| Unit | Framework de tests unitarios del runtime objetivo |
| Property-based | Framework de property-based testing equivalente |
| Snapshot | Framework de snapshot testing equivalente |
| Mutation | Framework de mutation testing equivalente |

## 5. Mocks y fixtures

- Fixtures de CSV en `tests/fixtures/csv/` versionados con el código.
- Casos generados sintéticamente con property-based para invariantes: "todo parse seguido de stringify devuelve el input original salvo whitespace insignificante".
- Sin mocks externos: la librería no depende de servicios externos.
```

### 7.2 Ejemplo 2 — `matriz-cobertura-pruebas_v1.0.md` para una REST API de pagos

Fragmento ilustrativo, no documento completo:

```markdown
# Matriz de cobertura de pruebas — REST API de pagos

**Proyecto:** {{nombre-proyecto}}
**Documento:** matriz-cobertura-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-05-17

## 2. Trazabilidad CU↔Tests

| CU | Criterio Given-When-Then | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Given pago válido, When se confirma, Then el estado pasa a confirmado | TC-01 | Integration | Verde |
| CU-01 | Given pago duplicado por clave de idempotencia, When se reintenta, Then no se duplica | TC-02 | Integration | Verde |
| CU-02 | Given pago inválido, When se confirma, Then se devuelve problem+json con código de error | TC-03 | Unit | Verde |
| CU-03 | Given pago confirmado, When se consulta su estado, Then devuelve el agregado completo | TC-04 | Contract | Verde |

## 3. Trazabilidad NFR↔Tests

| NFR | SLA | Test | Tooling |
| --- | --- | --- | --- |
| NFR-01 latencia p95 confirmación | < 300 ms | TC-10 | Cliente de load testing equivalente |
| NFR-02 idempotencia | 100 % | TC-02 | Test de integración con replays controlados |
| NFR-03 disponibilidad mensual | >= 99,9 % | Monitoreo SLO en 09 | Métrica observada, no test unitario |

## 4. Trazabilidad RN↔Tests

| RN | TC | Tipo |
| --- | --- | --- |
| RN-01 monto positivo | TC-05 | Unit |
| RN-02 estado válido en transición | TC-06 | Unit |
| RN-03 cliente activo | TC-07 | Integration |

## 5. Cobertura por capa

| Capa | Líneas | Branches | Mutation score |
| --- | --- | --- | --- |
| Dominio | 88 % | 82 % | 65 % |
| Aplicación | 81 % | 74 % | — |
| Infraestructura | 72 % | 64 % | — |
| Presentación (API) | 100 % de endpoints con contract test | — | — |

## 6. Gaps identificados

- TC pendiente para el flujo de compensación tras timeout del bus de eventos: se planifica en sprint S-04.
- Mutation testing aún no corre en CI: planificado en BT-12.
```

Los dos fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE-08}} responsable de redactar los artefactos de calidad y pruebas del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- SOLUTION-INTAKE: {{path}}
- Upstream: 02 (CU con criterios Given-When-Then, RN), 05 (arquitectura y NFR con métricas numéricas), 06 (DoR, US), 07 (sprint goals).

A generar (obligatorio para todos los tipos D8):
- estrategia-calidad_v1.0.md
- estrategia-testing_v1.0.md
- plan-pruebas_v1.0.md
- matriz-cobertura-pruebas_v1.0.md
- casos-prueba-referenciales_v1.0.md
- criterios-validacion_v1.0.md
- definition-of-done_v1.0.md
- guia-testing-extensibilidad_v1.0.md (sólo si el tipo D8 admite plugins o handlers externos)
- README.md de la sección (recomendado)

Reglas de redacción: §4 de 08_rules_calidad_y_pruebas.md.
Nomenclatura: sufijo uniforme `_v<X.Y>.md`. Prohibido el sufijo `-motor` o cualquier marcador de dominio en el nombre del archivo (corrección obligatoria del antecedente del fuente).
Trazabilidad: cada TC referencia al menos un CU, RN o NFR. La matriz tiene las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más la tabla de cobertura por capa.
Pirámide objetivo: la declarada en §2.2 para el tipo D8 del proyecto, con justificación cuando se aparta de ella.
DoD canónica: definition-of-done_v1.0.md es la única fuente. Los sprint plans referencian, no redefinen.
Criterios de calidad: §6 de 08_rules_calidad_y_pruebas.md.

Restricciones: no introducir productos comerciales ni protocolos del dominio fuente. Idioma rioplatense técnico, tildes correctas, sin emojis ni negritas decorativas.

Salida: /SDD2.2D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/08_calidad_y_pruebas/<estructura>.
```

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial de las reglas constructivas de la categoría 08. Define los siete artefactos obligatorios para todo tipo D8 y el opcional `guia-testing-extensibilidad`, fija la pirámide objetivo por tipo D8, establece la matriz de cobertura con tres tablas obligatorias, formaliza la DoD como documento canónico no redefinible por sprint y corrige el antecedente del fuente SDD 1.0 eliminando el sufijo de dominio en los nombres de archivo en favor del patrón uniforme `_v<X.Y>.md`. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/08_calidad_y_pruebas/`; la frase de cierre de §1.2 y la ruta de salida del prompt-snippet referencian el `project_type` del proyecto en curso (manifiesto). Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
