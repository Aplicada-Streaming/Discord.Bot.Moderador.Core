# Marco Teórico SDD 2.1

**Documento:** marco-teorico-sdd2.1_v1.0.md
**Versión:** 1.3
**Estado:** Aprobado
**Fecha:** 2026-06-10
**Autor:** Equipo Template SDD 2.1 — UTN
**Audiencia:** estudiantes universitarios (UTN), docentes, profesionales que adopten el template
**Idioma:** español rioplatense neutro técnico
**Norma bibliográfica:** APA 7
**Encoding:** UTF-8 / EOL LF

---

## Índice

- [§1 Introducción y encuadre](#1-introducción-y-encuadre)
- [§2 Fundamentos del enfoque SDD](#2-fundamentos-del-enfoque-sdd)
- [§3 Metodología del template SDD 2.1](#3-metodología-del-template-sdd-21)
  - [§3.10 El modelo de solución más jerarquía de proyectos](#310-el-modelo-de-solución-más-jerarquía-de-proyectos)
- [§4 Especialidades del template](#4-especialidades-del-template)
- [§5 Metodología ágil aplicada](#5-metodología-ágil-aplicada)
- [§6 Técnicas de descomposición y planificación](#6-técnicas-de-descomposición-y-planificación)
- [§7 Estilos arquitectónicos soportados](#7-estilos-arquitectónicos-soportados)
- [§8 UX/UI/DX como continuo](#8-uxuidx-como-continuo)
- [§9 Calidad y pruebas en SDD](#9-calidad-y-pruebas-en-sdd)
- [§10 DevOps y release engineering](#10-devops-y-release-engineering)
- [§11 Prompting colaborativo con IA](#11-prompting-colaborativo-con-ia)
- [§12 Anti-patrones y errores frecuentes](#12-anti-patrones-y-errores-frecuentes)
- [§13 Glosario](#13-glosario)
- [§14 Bibliografía y referencias](#14-bibliografía-y-referencias)

---

# §1 Introducción y encuadre

## 1.1 Propósito de este marco

Este documento es el cuerpo doctrinal del template SDD 2.1. Su función es explicar el porqué de cada decisión metodológica que el template propone, ofrecer la fundamentación académica e industrial que sostiene esas decisiones y servir de material de estudio profundo para estudiantes universitarios y profesionales que adopten el template como base operativa.

El marco teórico no enseña a operar el template paso a paso (esa es función de la guía de usuario, documento operativo separado). Acá se discute por qué la especificación es el artefacto central, por qué se separan 13 especialidades, por qué se prefiere Scrum como punto de partida, por qué se versionan los artefactos con SemVer, por qué se prohíben emojis en el cuerpo normativo y por qué la separación entre orquestador y subagentes es deliberada.

Quien lea este documento de principio a fin termina con un mapa conceptual completo del enfoque Specification-Driven Development aplicado al template SDD 2.1, con capacidad para auditar, criticar, extender o adaptar el método a contextos nuevos.

## 1.2 Audiencia primaria

El documento se redacta para tres audiencias concurrentes con distinto nivel de profundidad esperado:

- **Estudiantes universitarios (UTN, especialmente cátedras de Aplicada de la Tecnicatura Universitaria en Programación).** Se asume familiaridad con programación, control de versiones, ciclo de vida del software, lectura de UML básico y conocimiento introductorio de Scrum. No se asume experiencia industrial.
- **Docentes universitarios** que adopten el template como artefacto pedagógico de cátedra y necesiten una fundamentación bibliográfica trazable para acreditarlo académicamente.
- **Profesionales** de la industria que evalúen incorporar el template a su práctica profesional y necesiten entender la fundamentación antes de comprometerse.

El idioma es español rioplatense neutro técnico: se acepta el voseo cuando aporta naturalidad, pero el vocabulario técnico se conserva en su forma canónica (commit, branch, pull request, prompt, scaffolding, etc.). Las traducciones forzadas degradan precisión.

## 1.3 Cómo leer este documento

El documento sigue una progresión que va de lo conceptual a lo operativo:

- **§2** establece qué es SDD y por qué importa.
- **§3** describe la metodología concreta del template (qué hace el usuario, en qué orden).
- **§4** cataloga las 13 especialidades que produce artefactos.
- **§5–§11** desarrollan los marcos disciplinares: ágil, descomposición, arquitectura, UX/UI/DX, calidad, DevOps, prompting con IA.
- **§12** consolida anti-patrones para evitar errores típicos.
- **§13** glosa los términos canónicos.
- **§14** lista la bibliografía APA 7.

Tres rutas sugeridas según el perfil del lector:

| Perfil | Ruta sugerida | Objetivo |
|---|---|---|
| Estudiante que recién arranca | §1 → §2 → §3 → §5 → §13 → §6 → §9 → §11 | Entender qué es SDD y cómo se aplica en un proyecto académico. |
| Docente que evalúa adopción | §1 → §2 → §4 → §11 → §12 → §14 | Verificar coherencia metodológica y trazabilidad bibliográfica. |
| Profesional industrial | §2 → §3 → §7 → §9 → §10 → §11 → §12 | Validar que el template se integra a su práctica industrial existente. |

## 1.4 Diferencias con la guía de usuario

Es importante no confundir este marco teórico con la guía de usuario del template:

| Aspecto | Marco teórico (este documento) | Guía de usuario |
|---|---|---|
| Naturaleza | Doctrina, fundamentos, justificación | Operativa, paso a paso |
| Pregunta que responde | ¿Por qué? ¿En qué se basa? | ¿Cómo? ¿Qué hago ahora? |
| Audiencia | Estudia, audita, adapta | Ejecuta, aplica |
| Longitud | Extenso, denso, completo | Concisa, accionable |
| Frecuencia de consulta | Una vez en profundidad, luego como referencia | Cada vez que se inicia un proyecto |
| Cita fuentes | Sí, con APA 7 | No, asume que el marco las cubre |

Las dos piezas son complementarias y se referencian mutuamente, pero no se solapan. Si un lector necesita saber cómo completar el documento `SOLUTION-INTAKE`, debe acudir a la guía de usuario. Si necesita saber por qué el intake pregunta lo que pregunta, debe acudir a este marco teórico.

## 1.5 Mapa visual del template SDD 2.1

El template se organiza en dos planos paralelos:

```text
e:/repos/...../SDD2.1D/
├── devs/                    Plano metodológico (marco teórico, plantillas, reglas)
│   ├── guides/              Marco teórico (este documento)
│   ├── intake/              Plantillas de carga inicial
│   ├── orchestrator/        Prompts orquestadores y subagentes
│   ├── rules/               Reglas de nomenclatura, decisiones D1..D8
│   ├── references/          Catálogo de reglas de diseño por stack (insumo de AG-03)
│   └── _bootstrap/          Auditoría del fuente SDD 1.0, ADR de origen
│
└── docs/                    Plano operativo de la solución generada
    ├── 00_contexto/         AG-00  Product Manager        (nivel solución)
    ├── 01_necesidades_negocio/        AG-01  Analista de Negocio  (nivel solución)
    ├── _solucion/           Consolidación de solución: vista de solución
    │                        (mapa de proyectos, contratos inter-proyecto,
    │                        grafo de dependencias) y pipeline de solución
    │                        (orden de build topológico, artefactos publicables)
    ├── proyectos/           Una carpeta por proyecto (kebab-case)
    │   └── <nombre-proyecto>/
    │       ├── 02_especificacion_funcional/   AG-02  Analista Funcional
    │       ├── 03_ux-ui/            AG-03  Especialista DX/UX
    │       ├── 04_prompts_ai/       AG-04  Ingeniero de Prompts
    │       ├── 05_arquitectura_tecnica/       AG-05  Arquitecto de Software
    │       ├── 06_backlog-tecnico/  AG-06  Scrum Master (backlog)
    │       ├── 07_plan-sprint/      AG-07  Scrum Master (planificación)
    │       ├── 08_calidad_y_pruebas/          AG-08  QA / SDET
    │       ├── 09_devops/           AG-09  DevOps Engineer
    │       ├── 10_developer_guide/  AG-10  Technical Writer
    │       └── 11_examples/         AG-11  Developer Advocate
    └── README.md            AG-ROOT  Arquitecto de Soluciones
```

Las categorías 00 y 01 viven a nivel solución (el negocio es uno por solución); las categorías 02 a 11 viven por proyecto (la técnica se especializa según el tipo D8 de cada proyecto); `_solucion/` consolida la mirada de conjunto. En el caso degenerado de una solución con un único proyecto, el layout se aplana y `docs/` reproduce exactamente la estructura plana clásica (00 a 11 directamente bajo `docs/`, sin nivel `proyectos/`), garantizando la no ruptura con el modelo de tipo único. La fundamentación de este modelo solución más jerarquía de proyectos se desarrolla en §3.10.

La separación `/devs/` versus `/docs/` proviene del aprendizaje del fuente SDD 1.0: mezclar material metodológico con documentación operativa del producto contamina ambos. El marco teórico es estable y cambia rara vez; la documentación de la solución cambia cada sprint. Mantenerlos en árboles separados permite versionarlos de manera independiente.

## 1.6 Origen del template

Este template se construyó a partir de la auditoría del fuente SDD 1.0 aplicado al caso de estudio histórico Motor DSL en la cátedra de Aplicada del cuatrimestre 2026. El audit Fase 0 (ver `SDD2.1D/devs/_bootstrap/audit-sdd1.md`) inventarió 161 archivos markdown, identificó 14 patrones repetibles, 14 inconsistencias y un mapa de cobertura por capítulo. Sobre ese análisis se generalizó el material domain-specific para producir el template stack-agnóstico que SDD 2.1 propone.

Las menciones al caso original se conservan exclusivamente como referencia formal del origen, no como contenido canónico del template. El template SDD 2.1 cubre ocho tipos de proyecto distintos (decisión D8): library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service. Estos ocho valores son un conjunto cerrado que no cambia; lo que el modelo reformulado introdujo (ver §3.10) es que el tipo D8 se asigna por proyecto y una solución agrupa una jerarquía de N proyectos tipados, en lugar de asumir un único tipo por repositorio.

---

# §2 Fundamentos del enfoque SDD

## 2.1 Qué es Specification-Driven Development

**Specification-Driven Development (SDD)** es un enfoque de desarrollo de software en el que se escribe primero una especificación formal del problema y de la solución antes de generar o escribir código. La especificación actúa como el contrato que guía la implementación y como la fuente de verdad consultable a lo largo del ciclo de vida del producto.

La idea no es novedosa en sí misma: la ingeniería de software lleva décadas reclamando especificación formal antes de implementar (Royce, 1970; Boehm, 1988; IEEE 830-1998; ISO/IEC/IEEE 29148:2018). Lo que renueva SDD en su versión 2024-2026 es el contexto de desarrollo asistido por modelos de lenguaje grandes (LLM). En ese contexto, la especificación ya no es solo un documento humano: es el insumo principal que el modelo lee para generar implementaciones coherentes, verificables y trazables.

En palabras del material de referencia que sustenta este capítulo:

> "La spec define el qué y el por qué; el código define el cómo. Una spec que no se actualiza junto con el código se convierte en documentación desactualizada y pierde su valor como fuente de verdad" (Panaversity Agent Factory, 2024).

## 2.2 Por qué la especificación es el artefacto central

Cuando el código se genera asistido por IA, surge un fenómeno que la literatura denomina **inconsistencia determinística**: dos generaciones del mismo modelo, con la misma entrada, no producen código sintácticamente idéntico (Panaversity, 2024). Si el código no es el artefacto estable, ¿qué lo es?

La respuesta de SDD es: la especificación. La especificación es lo que el equipo discute, refina, versiona y aprueba. El código es la materialización (potencialmente cambiante) de esa especificación. Esto invierte la jerarquía tradicional donde el código era el activo de primera clase y la documentación un subproducto secundario.

Las consecuencias prácticas son tres:

1. **La especificación se versiona con la misma seriedad que el código.** Cambios en la spec preceden a cambios en el código, no al revés.
2. **La trazabilidad es obligatoria.** Cada elemento del código debe poder rastrearse a la spec que lo justifica, y cada spec debe poder rastrearse al valor de negocio que pretende capturar.
3. **El debate de diseño ocurre en la spec, no en el pull request.** Los pull requests verifican que el código respeta la spec; no son el lugar para repensar la arquitectura.

## 2.3 Los tres niveles de SDD

Panaversity (2024) distingue tres niveles de adopción de SDD según el grado de centralidad que la especificación tenga en el ciclo de vida del proyecto.

### Nivel 1 — Spec-First

> "La spec guía la implementación. Una vez completada, se descarta" (Panaversity, 2024).

El flujo es lineal: el equipo escribe la spec, la IA o el desarrollador la implementan, el documento se archiva o se elimina. La spec cumplió su función como contrato puntual.

**Cuándo aplica.** Tareas de una sola sesión, proyectos personales sin coordinación de equipo, prototipos, correcciones puntuales de bugs.

**Ventaja.** Sin carga de mantenimiento documental.

**Desventaja.** En seis meses, si se necesita modificar la implementación, no hay documentación viva que oriente al desarrollador que reabre el código.

### Nivel 2 — Spec-Anchored

Tanto la especificación como el código se mantienen como artefactos vivos. Los cambios a la spec preceden a los cambios de código. Este es el nivel estándar que SDD 2.1 propone para proyectos con equipo o con horizonte de mantenimiento mayor a seis meses.

**Cuándo aplica.** Proyectos con múltiples contribuyentes, sistemas que requieren documentación de compliance, productos con horizonte de mantenimiento de seis meses o más, features que sufrirán múltiples iteraciones.

**Requisito crítico.** Disciplina para actualizar la spec antes de tocar el código. Sin esa disciplina, spec y código divergen y el artefacto pierde su valor como contrato.

### Nivel 3 — Spec-as-Source

La especificación es el artefacto principal y el código se regenera bajo demanda a partir de ella. Es el nivel experimental.

**Desafío fundamental.** La inconsistencia determinística mencionada antes: la misma spec genera código sintácticamente distinto entre regeneraciones, lo que complica el debugging y el control de versiones.

**Cuándo tiene sentido.** Código altamente repetitivo (operaciones CRUD), código desechable (scripts de migración, scaffolding), entornos con cobertura de tests robusta que validen el comportamiento independientemente de la forma del código.

### Marco de decisión

| Situación | Nivel recomendado |
|---|---|
| Trabajo individual, sesión única | Spec-First (1) |
| Equipo de dos o más personas | Spec-Anchored (2) |
| Sistema con mantenimiento mayor a seis meses | Spec-Anchored (2) |
| Código repetitivo o desechable con buena cobertura de tests | Spec-as-Source (3) |
| Experimento personal con buena cobertura | Spec-as-Source (3) |

**Recomendación general.** Empezar con Spec-First y migrar a Spec-Anchored cuando el proyecto involucre equipo o tenga horizonte de mantenimiento largo. Reservar Spec-as-Source para contextos controlados con tests que garanticen el comportamiento esperado. El template SDD 2.1 está diseñado por defecto para operar en nivel Spec-Anchored.

## 2.4 Comparación con TDD y BDD

SDD no reemplaza ni excluye a Test-Driven Development (TDD) ni a Behavior-Driven Development (BDD). Es una capa superior que define el qué; TDD y BDD operan sobre el cómo.

| Enfoque | Artefacto guía | Audiencia | Pregunta que responde |
|---|---|---|---|
| **SDD** | Especificación (qué hace el sistema y por qué) | Equipo de producto + desarrollo | ¿Qué tiene que hacer el sistema y para qué? |
| **BDD** | Escenarios Given/When/Then (cómo se verifica el comportamiento) | Producto + QA + desarrollo | ¿Cómo se observa que el sistema cumple la spec? |
| **TDD** | Test unitario (cómo se valida una unidad de código) | Desarrollo | ¿Cómo se construye correctamente la implementación? |

En un flujo combinado típico bajo SDD 2.1:

1. SDD produce la especificación funcional (`docs/02_especificacion_funcional/`).
2. BDD traduce los casos de uso de la spec a escenarios Given/When/Then ejecutables.
3. TDD se aplica al construir cada unidad de código, asegurando que la implementación cumple los criterios de aceptación.

Las tres prácticas son compatibles y se refuerzan. Ninguna sustituye a la otra.

## 2.5 Por qué SDD encaja con el desarrollo asistido por IA

El contexto industrial de adopción de SDD 2.1 incluye el uso de asistentes de IA basados en LLM (Claude, GitHub Copilot, Cursor, entre otros). En ese contexto, la especificación cumple cuatro funciones adicionales a las clásicas:

1. **Contrato para el modelo.** El LLM lee la spec y produce código que la respeta. Sin spec, el modelo improvisa.
2. **Mecanismo de verificación.** El equipo puede pedirle al modelo que verifique si el código existente cumple la spec, detectando deriva.
3. **Unidad de regeneración.** Si se necesita reescribir un módulo, la spec es lo que se le pasa al modelo. El código viejo se descarta.
4. **Anclaje de contexto.** Cuando la conversación con el modelo se alarga y el contexto se compacta, la spec funciona como ancla para que el modelo no pierda el hilo.

## 2.6 Limitaciones del enfoque

SDD no es bala de plata. Tiene costos y riesgos que conviene reconocer:

- **Costo de escritura.** Una buena spec requiere tiempo. En proyectos de muy corto plazo (menos de una semana) el costo puede no justificarse.
- **Riesgo de spec desactualizada.** Si la disciplina de actualizar la spec antes que el código falla, la spec se vuelve engañosa (peor que no tenerla).
- **Riesgo de over-engineering documental.** Hay equipos que producen specs barrocas que nadie lee. La spec debe ser tan extensa como sea necesario y tan breve como sea posible.
- **Curva de aprendizaje.** Equipos acostumbrados a "shipear primero, documentar después" tardan en internalizar la inversión.

El template SDD 2.1 mitiga estos riesgos con plantillas mínimas, validación automatizada de nomenclatura, y un proceso explícito de actualización spec-code que se describe en §3 y §11.

---

# §3 Metodología del template SDD 2.1

## 3.1 Visión general del flujo

El template SDD 2.1 propone un flujo de seis pasos que va desde la idea informal hasta la documentación operativa lista para codificar. El flujo se diseñó con dos premisas:

1. **El humano decide; la IA ejecuta.** El equipo humano aporta visión, restricciones y criterio profesional. La IA materializa esas decisiones en artefactos coherentes con la estructura del template.
2. **El plan precede a la acción.** En cada paso se pide a la IA que explique qué va a hacer antes de hacerlo, para que el humano confirme antes de comprometer cambios.

El flujo está calibrado para proyectos académicos e industriales pequeños a medianos (uno a diez integrantes). Equipos más grandes pueden ramificar el paso 5 en múltiples ciclos paralelos.

## 3.2 Paso 1 — Chat informal con la web de Claude para armar contexto

El proceso arranca fuera del repositorio, en una conversación informal en `claude.ai` o equivalente. El humano describe la idea del proyecto, las restricciones que conoce, el público objetivo, el stack probable y cualquier dato relevante (capturas, documentos existentes, links a sistemas legacy).

El objetivo es producir dos insumos:

- **BRIEF**: un texto corto (una a dos páginas) que captura la esencia del proyecto: problema, usuarios, alcance, restricciones, criterios de éxito.
- **README inicial**: una versión preliminar del archivo `README.md` del repositorio, escrita en el tono del proyecto.

Estos dos artefactos no son la especificación final. Son el material crudo que alimenta los pasos siguientes. Conviene iterarlos hasta que el humano sienta que reflejan fielmente la idea.

**Por qué este paso ocurre fuera del repo.** La conversación informal explora múltiples caminos, descarta opciones, vuelve atrás. Hacer eso dentro del repo contamina el historial git con churn de decisiones que después no aportan. Mantener el chat informal fuera permite usar el repo solo para artefactos consolidados.

## 3.3 Paso 2 — Consolidación en un documento único

Cuando el BRIEF y el README inicial están maduros, se consolidan en un documento único que servirá de entrada al paso 3. Este documento debe contener, como mínimo:

- Nombre tentativo del proyecto en kebab-case.
- Problema que el sistema resuelve, en una frase.
- Usuarios objetivo (primario, secundario, terciario si aplica).
- Composición de la solución: lista de proyectos que la integran y, por cada proyecto, su tipo según la decisión D8 (library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service). En una solución de un único proyecto, esta lista tiene un solo elemento y equivale a declarar un único tipo D8.
- Restricciones técnicas conocidas (stack, integraciones, plataformas) por proyecto.
- Criterios de éxito tempranos (qué tiene que pasar para que el proyecto se considere exitoso en tres meses).
- Lista de cosas que están explícitamente fuera de alcance (es tan importante saber qué no se hace como saber qué se hace).

El consolidado se guarda como `intake-source.md` o se mantiene en el chat como mensaje fijado. Su rol es funcionar como fuente de verdad para los pasos siguientes.

## 3.4 Paso 3 — Volcado al documento único de intake

El template provee un único documento de intake que el humano completa manualmente a partir del consolidado: `SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md`. Este documento reemplaza y deja deprecados a los dos artefactos previos (`PROJECT-BRIEF`, que capturaba el negocio, y `PROJECT-README`, que capturaba la técnica): ambos quedan subsumidos como partes de un solo documento de entrada.

El intake se organiza en tres partes:

- Parte A — Negocio (§1 a §12): visión del producto, problema, usuarios, alcance, stakeholders, criterios de éxito y exclusiones. Es el insumo de negocio de AG-00 (Product Manager) y AG-01 (Analista de Negocio). Se carga una vez, porque el negocio es uno por solución (intake a nivel solución, ver §3.10.5).
- Parte B — Composición (§13 a §16): la tabla de proyectos tipados (§13), el estilo de solución (§14), la descomposición (§15) y la estructura (§16). La tabla de proyectos tipados del §13 declara, para cada proyecto, su tipo D8, su rol y sus dependencias; es la fuente desde la que se deriva el SOLUTION-MANIFEST (ver §3.10.3). En una solución de un único proyecto, la tabla tiene una sola fila y la composición se comporta igual que en el modelo de tipo único.
- Parte C — Técnica por proyecto (§17): un bloque repetible P.1 a P.12 por cada proyecto, con el stack tentativo, las decisiones D1–D8 que ese proyecto adopta y las secciones del SDD que aplican y las que no.

El documento se cierra con §18 (samples) y §19 (checklist de completitud).

Por qué un único documento de intake y no dos o tres artefactos separados: mantener visión, composición y técnica en un solo archivo da una entrada única y autocontenida que el humano completa de corrido y que el orquestador lee de una sola fuente, sin tener que conciliar versiones de varios documentos. La asimetría negocio/técnica no se pierde: se preserva como secciones del documento (Parte A única por solución, Parte C repetible por proyecto), no como archivos distintos. La composición declarada en la Parte B es la que alimenta el SOLUTION-MANIFEST como fuente única de verdad de la jerarquía (ver §3.10.3); el manifiesto ya no se completa a mano sino que se deriva del §13 (ver §3.6).

## 3.5 Paso 4 — Bootstrap local en Claude Code

El humano clona o crea el repositorio del proyecto, copia el árbol `/SDD2.1D/devs/` del template adentro, y abre el repositorio en Claude Code (o el agente CLI equivalente). En este momento ya están en el repo:

- El documento único de intake completado (`SOLUTION-INTAKE`).
- Las reglas de nomenclatura (`devs/rules/`), incluida la regla meta `rules/_intake_rules.md` que dirige la validación de intake.
- Las plantillas de artefactos.
- Los prompts del orquestador y de los subagentes (`devs/orchestrator/`).
- Este marco teórico (`devs/guides/marco-teorico-sdd2.1_v1.0.md`).

El humano verifica que el intake está bien cargado (no hay placeholders sin completar) y que las decisiones D1–D8 están explícitas. El SOLUTION-MANIFEST no se completa en este paso: es un artefacto derivado que el orquestador construye en la Fase de validación de intake (ver §3.6) a partir del §13 del intake.

## 3.6 Paso 5 — Ejecución del master-prompt

El humano ejecuta en Claude Code el prompt orquestador maestro, en su versión v3.0. Antes de generar artefacto alguno, este prompt corre una Fase de validación de intake (previa a la Fase A de generación) y, recién con el intake validado y el manifiesto confirmado, despacha la generación.

### 3.6.1 Fase de validación de intake (previa a la Fase A)

La Fase de validación de intake está dirigida por la regla meta `rules/_intake_rules.md`, que describe cómo leer el intake, qué verificar y cómo derivar el manifiesto. La fase hace tres cosas distintas:

1. Valida la completitud semántica del intake. No se limita al scan sintáctico de placeholders que el humano ya hizo en el Paso 4: revisa que cada parte del intake esté no solo presente sino también suficientemente especificada para generar sin ambigüedad. Es una validación proactiva, previa al despacho de subagentes.
2. Emite una batería consolidada de preguntas. Cuando detecta subespecificación, el orquestador agrupa todas las preguntas pendientes en una sola batería y la presenta al humano de una vez, en lugar de descubrir los huecos de a uno durante la generación. Responder esa batería antes de generar evita que la IA derive por subespecificación, es decir, que rellene huecos inventando supuestos que después contaminan toda la cadena de artefactos.
3. Deriva y confirma el manifiesto. El orquestador construye el SOLUTION-MANIFEST desde la tabla de proyectos tipados del §13 del intake y lo presenta para confirmación humana explícita. El manifiesto deja de ser un documento que el usuario completa a mano y pasa a ser un artefacto derivado.

Por qué validar la completitud antes de generar. La generación asistida por IA es vulnerable a la subespecificación: ante un hueco en la entrada, el modelo no se detiene, sino que improvisa un supuesto plausible y sigue. Ese supuesto se propaga por la cadena de trazabilidad y resulta caro de revertir una vez materializado en decenas de artefactos. Concentrar la verificación de completitud al inicio, en una sola batería de preguntas, convierte un problema distribuido y reactivo en un punto de control único y barato: el humano resuelve las ambigüedades antes de que cuesten.

Por qué derivar el manifiesto en lugar de pedirlo. El modelo reformulado tiene dos exigencias que en apariencia tiran en sentidos opuestos: que el humano cargue un único documento de entrada (Paso 3) y que exista una fuente única de verdad de la jerarquía de proyectos (el SOLUTION-MANIFEST, ver §3.10.3). Derivar el manifiesto del §13 del intake concilia ambas: la entrada sigue siendo única (el humano no mantiene dos artefactos en sincronía) y la fuente única de verdad de la jerarquía se preserva, porque el manifiesto se computa de manera determinística desde la tabla declarada y se confirma antes de usarse. Si el humano cargara el manifiesto por separado, podría divergir del §13; al derivarlo, esa divergencia es imposible por construcción.

Esta fase es complementaria, no sustituta, de los mecanismos previos: no reemplaza el scan de placeholders del Paso 4 ni la resolución reactiva de ambigüedad por subagente en runtime; se suma como un control semántico anticipado que esos dos mecanismos no cubrían.

### 3.6.2 Fase A — Generación

Confirmado el manifiesto, el orquestador ejecuta la generación. Esta etapa:

1. Lee el intake y el SOLUTION-MANIFEST derivado y confirmado en la Fase de validación de intake, junto con las reglas.
2. Genera las categorías de nivel solución (00 y 01) una vez, y determina el orden topológico de los proyectos a partir del grafo de dependencias del manifiesto.
3. Para cada proyecto, en orden topológico (primero las dependencias, después los dependientes), identifica las secciones 02 a 11 que aplican según su tipo D8 y aplica la variante §1.2 de cada regla correspondiente a ese D8.
4. Despacha subagentes especializados (uno por sección) en paralelo o en serie según la dependencia, y cada subagente genera la documentación de su sección respetando las plantillas y la nomenclatura.
5. El orquestador consolida los resultados en `_solucion/` (vista de solución y pipeline de solución), valida coherencia cruzada intra e inter-proyecto y reporta inconsistencias.

El producto del paso 5 es el árbol `/SDD2.1D/docs/` poblado con las categorías de solución, las carpetas `proyectos/<kebab>/` de cada proyecto y la consolidación `_solucion/`, listo para revisión humana. En el caso degenerado de un único proyecto, el árbol se aplana y el resultado es indistinguible del modelo de tipo único.

**Paralelización vs serialización.** Las secciones que dependen del resultado de otras se ejecutan en serie. Dentro de cada proyecto, el orden canónico de especialidades es: AG-02 → AG-03 → AG-04 → AG-05 → AG-06 → AG-07 → AG-08 → AG-09 → AG-10 → AG-11, precedido por AG-00 y AG-01 a nivel solución; las secciones que no dependen entre sí (por ejemplo AG-08 y AG-10) pueden ejecutarse en paralelo si el agente lo soporta. Entre proyectos, el orden lo dicta el grafo de dependencias: un proyecto se genera después de aquellos de los que depende, de modo que sus contratos ya estén disponibles.

## 3.7 Paso 6 — Confirmación y handoff a codificación

El humano revisa la documentación generada en `/docs/`. Si encuentra inconsistencias, las reporta al orquestador, que despacha un subagente correctivo. Cuando la documentación está aprobada:

1. Se hace commit del estado completo en git.
2. Se etiqueta como `docs-v1.0` (estado base de la documentación).
3. Se inicia el primer sprint con el backlog producido por AG-06 y AG-07.
4. La codificación comienza con la documentación como referencia obligatoria.

A partir de este punto, el equipo opera en nivel Spec-Anchored: cualquier cambio significativo del comportamiento del sistema requiere primero actualizar la spec correspondiente, después modificar el código.

## 3.8 Diagrama del flujo completo

```text
+--------------------------------------------------------------------+
|                                                                    |
|    PASO 1: Chat informal en claude.ai                              |
|    +----------------+                                              |
|    | Humano <-> IA  |  Produce: BRIEF + README inicial             |
|    +--------+-------+                                              |
|             |                                                      |
|             v                                                      |
|    PASO 2: Consolidacion en documento unico (intake-source.md)     |
|             |                                                      |
|             v                                                      |
|    PASO 3: Volcado al documento unico de intake                    |
|    +-----------------------------------------+                     |
|    | SOLUTION-INTAKE  (A negocio / B comp. /  |                     |
|    | C tecnica)  reemplaza BRIEF + README     |                     |
|    +-----------------------------------------+                     |
|             |                                                      |
|             v                                                      |
|    PASO 4: Bootstrap en repo local + Claude Code                   |
|    Copia /SDD2.1D/devs/ al repo, abre Claude Code                  |
|             |                                                      |
|             v                                                      |
|    PASO 5: Ejecucion del master-prompt (v3.0)                      |
|    +------------------+                                            |
|    |   Orquestador    |                                            |
|    +--------+---------+                                            |
|             |                                                      |
|             v                                                      |
|    FASE VALIDACION DE INTAKE (previa a Fase A)                     |
|    dirigida por rules/_intake_rules.md                            |
|    - valida completitud semantica del intake                      |
|    - emite bateria consolidada de preguntas                       |
|    - deriva SOLUTION-MANIFEST desde §13 y lo confirma             |
|             |                                                      |
|             v                                                      |
|    FASE A: Generacion                                              |
|             | despacha                                             |
|             v                                                      |
|    +----+----+----+----+----+----+----+----+----+----+----+----+   |
|    |AG0 |AG1 |AG2 |AG3 |AG4 |AG5 |AG6 |AG7 |AG8 |AG9 |AG10|AG11|   |
|    +----+----+----+----+----+----+----+----+----+----+----+----+   |
|             |                                                      |
|             v                                                      |
|    Consolidacion + validacion cruzada                              |
|             |                                                      |
|             v                                                      |
|    /SDD2.1D/docs/ poblado                                          |
|             |                                                      |
|             v                                                      |
|    PASO 6: Confirmacion humana                                     |
|    +--------+----------+                                           |
|    | Revisa, corrige   |                                           |
|    | Aprueba           |                                           |
|    +--------+----------+                                           |
|             |                                                      |
|             v                                                      |
|    Handoff a codificacion: comienza Sprint 1                       |
|                                                                    |
+--------------------------------------------------------------------+
```

El diagrama representa el flujo lineal pero los pasos 5 y 6 pueden iterar. La Fase de validación de intake, al inicio del Paso 5, es el primer punto donde el flujo puede volver atrás: si la validación de completitud semántica detecta huecos, su batería consolidada de preguntas devuelve el control al humano antes de generar nada. Si el humano detecta gaps en la documentación ya generada, vuelve al paso 5 con un prompt correctivo. Si detecta gaps en el intake, vuelve al paso 3.

## 3.9 Diferencias con SDD 1.0

SDD 2.1 hereda el patrón general del fuente SDD 1.0 (la cadena AG-00 → AG-11, la numeración `00..11`, las plantillas de CU/NB/RN/US/BT) pero introduce mejoras tomadas de la auditoría Fase 0:

| Aspecto | SDD 1.0 | SDD 2.1 |
|---|---|---|
| Stack | Específico .NET / MAUI / NuGet | Stack-agnóstico con perfil por tipo D8 |
| Separación devs/docs | Implícita | Explícita: `/devs/` versus `/docs/` |
| Tipos de proyecto | Uno (caso de estudio histórico) | Ocho (decisión D8) |
| Versionado de artefactos | Mixto (`.v1.0` y `_v1.0`) | Unificado (`_v1.0`) |
| Casing en nombres | Mixto | Estricto kebab-case lowercase |
| ADR | Consolidado en un archivo | Un archivo por decisión (`ADR-XX-...`) |
| Auditoría inicial | Inexistente | Obligatoria (Fase 0) |
| Marco teórico | Disperso en `/references/` | Consolidado en `/devs/guides/` |

Aclaración sobre el alcance de D3 (casing kebab-lowercase). La regla de casing estricto kebab-lowercase gobierna los nombres de los artefactos generados: documentos versionados y carpetas de categoría y de proyecto bajo `/docs/` y `/src/`. No alcanza a los identificadores de variante de la metodología, que son tokens de marca, no artefactos generados: las carpetas raíz `SDD1.0`, `SDD2.1D`, `SDD2.1M` y `SDD2.1R` usan ese esquema de identificación a propósito, y su mayúscula no es una violación de D3. Lo mismo aplica a los prefijos de organización como `Aplicada` en los nombres de código. La validación automatizada de nomenclatura se aplica al plano de artefactos, no a estos identificadores de marca.

## 3.10 El modelo de solución más jerarquía de proyectos

El modelo original del template asumía un proyecto único por repositorio: un solo tipo D8, un árbol de documentación plano, una sola cadena de generación. Ese modelo es correcto pero limitado: la mayoría de los sistemas reales no son una pieza monolítica de un solo tipo, sino un conjunto coordinado de piezas de tipos distintos (por ejemplo, una librería compartida, un servicio REST que la consume y una herramienta de línea de comandos que opera contra ese servicio). El modelo reformulado de SDD 2.1 generaliza el template a este caso sin romper el caso simple.

### 3.10.1 Qué es una solución y qué es un proyecto

Una solución agrupa una jerarquía de N proyectos, con N mayor o igual a uno. Cada proyecto lleva exactamente uno de los ocho valores cerrados de la decisión D8 (library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service). La solución no tiene un D8 propio: su tipo es compuesto, derivado de la combinación de los tipos de sus proyectos.

La distinción es deliberada y responde a una asimetría real:

- El negocio es uno por solución. Hay una visión de producto, un conjunto de necesidades de negocio, un público objetivo. Por eso las categorías 00 (contexto) y 01 (necesidades de negocio) se especifican a nivel solución.
- La técnica es por proyecto. Cada proyecto resuelve una parte del sistema con un estilo, un contrato y un ciclo de vida propios. Por eso las categorías 02 a 11 (especificación funcional, UX/UI, prompts, arquitectura, backlog, sprints, calidad, devops, guía de desarrollador, ejemplos) se especifican por proyecto.

### 3.10.2 El proyecto como unidad de especialización

En el modelo reformulado, el proyecto es la unidad de especialización del template. Las reglas del template tienen una variante por tipo D8 (la variante que cada capítulo de reglas documenta en su §1.2). El orquestador aplica esa variante por proyecto, según el D8 que el proyecto declara. Así, en una misma solución, el proyecto de tipo library recibe el tratamiento de librería (API pública mínima, SemVer estricto, distribución por feed de paquetes) y el proyecto de tipo rest-api recibe el suyo (OpenAPI primero, versionado de API, despliegue a runtime), sin que uno contamine al otro.

Esto preserva intacto el catálogo de las 13 especialidades (ver §4) y la cadena de trazabilidad (ver §4.4): lo único que cambia es el punto de aplicación. Las variantes D8 dejan de aplicarse al repositorio entero y pasan a aplicarse a cada proyecto.

### 3.10.3 El manifiesto como fuente única de verdad

La composición de la solución se consolida en un único artefacto: el SOLUTION-MANIFEST. El manifiesto enumera los proyectos de la solución y, para cada uno, su tipo D8, su rol dentro de la solución, sus dependencias hacia otros proyectos y sus nombres de código. El manifiesto es la fuente única de verdad sobre qué proyectos existen y cómo se relacionan.

El manifiesto no lo completa el humano: es un artefacto derivado. El orquestador lo construye desde la tabla de proyectos tipados del §13 del intake durante la Fase de validación de intake (ver §3.6) y lo presenta para confirmación humana antes de usarlo. De este modo, la composición se declara una sola vez (en el §13 del intake) y el manifiesto que el resto de la generación consume se computa de manera determinística desde esa declaración, sin riesgo de divergencia entre ambos.

Centralizar esta información en un manifiesto, en lugar de dispersarla por los árboles de cada proyecto, tiene tres consecuencias prácticas:

1. La composición se puede leer, auditar y versionar en un solo lugar. Agregar un proyecto, cambiar su tipo o reescribir una dependencia es un cambio localizado y trazable.
2. El orquestador deriva del manifiesto el plan de generación completo sin tener que inferir relaciones leyendo cada carpeta.
3. La coherencia inter-proyecto (contratos, dependencias) se valida contra una declaración explícita, no contra suposiciones.

### 3.10.4 El grafo de dependencias y el orden topológico

Las dependencias declaradas entre proyectos forman un grafo. Ese grafo debe ser acíclico (un DAG): un proyecto no puede depender, directa o transitivamente, de sí mismo. La aciclicidad no es un capricho formal; es la condición que hace posible un orden de generación y de build bien definido.

A partir del DAG se obtiene un orden topológico: primero se generan y se construyen las dependencias, después los dependientes. Si el proyecto rest-api depende de la library, la library se especifica y se construye antes. Este orden garantiza que cuando un proyecto se genera, los contratos de los proyectos de los que depende ya están disponibles, y que el pipeline de solución produce los artefactos en una secuencia en la que cada paso encuentra resueltas sus entradas.

### 3.10.5 Intake a nivel solución

El intake del modelo reformulado opera a nivel solución, coherente con la asimetría negocio/técnica, y se consolida en un único documento: `SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md`. Este documento reemplaza y deja deprecados a los dos artefactos previos (`PROJECT-BRIEF` de negocio y `PROJECT-README` técnico), que dejan de usarse como archivos separados y pasan a ser partes del documento único. La asimetría negocio/técnica se preserva como secciones:

- Una Parte A de negocio por solución, que captura la visión y el problema una sola vez (el negocio es uno).
- Una Parte C técnica con un bloque repetible por proyecto (la técnica es por proyecto).
- Una Parte B de composición, cuya tabla de proyectos tipados (§13) declara la jerarquía y es la fuente desde la que se deriva el SOLUTION-MANIFEST descrito arriba.

De este modo, lo que es único (el negocio) se carga una vez y lo que se especializa (la técnica de cada proyecto) se carga por proyecto, evitando duplicación del negocio y mezcla de tecnologías heterogéneas en un único bloque.

### 3.10.6 Niveles de la generación y consolidación de solución

La generación trabaja en dos niveles coherentes con la estructura de `docs/` descrita en §1.5:

- Nivel solución: categorías 00 y 01 se generan una vez por solución.
- Nivel proyecto: categorías 02 a 11 se generan por proyecto, en el orden topológico que dicta el grafo de dependencias.
- Consolidación de solución: la carpeta `_solucion/` reúne una vista de solución (el mapa de proyectos, los contratos inter-proyecto y el grafo de dependencias) y un pipeline de solución (el orden de build topológico y los artefactos publicables por proyecto).

### 3.10.7 El caso degenerado como garantía de no ruptura

Una solución de un solo proyecto reproduce exactamente el comportamiento del template de tipo único: el layout se aplana (las categorías 00 a 11 quedan directamente bajo `docs/`, sin nivel `proyectos/`), no hay grafo no trivial que ordenar y no hay contratos inter-proyecto que consolidar. Este caso degenerado es la garantía de no ruptura: quien venía usando el template con un proyecto único no percibe cambio alguno, y quien necesita componer varios proyectos extiende el mismo modelo sin reaprenderlo.

Conviene subrayar qué no cambió. El conjunto D8 sigue teniendo exactamente ocho valores; no se agregó ni se quitó ningún tipo. Lo que cambió es la cardinalidad (de un tipo por repositorio a N proyectos tipados por solución) y la topología (la composición y la dependencia entre proyectos), no el conjunto de tipos.

### 3.10.8 La composición como decisión arquitectónica de nivel solución

Decidir qué proyectos integran la solución y qué dependencias hay entre ellos es, en sí misma, una decisión arquitectónica, pero de un nivel superior al estilo interno de cada proyecto. El estilo de cada proyecto (ver §7) resuelve cómo se organiza ese proyecto por dentro; la composición de la solución resuelve qué piezas hay y cómo se conectan. Ambas decisiones se documentan como ADR, pero pertenecen a planos distintos: la composición es un ADR de nivel solución, el estilo interno es un ADR de nivel proyecto.

---

# §4 Especialidades del template

## 4.1 Por qué 13 especialidades y no menos

El template asigna una especialidad por carpeta de `/docs/` siguiendo una premisa: cada artefacto documental tiene un responsable claro con un perfil profesional específico. Esto cumple dos funciones:

1. **Criterio de calidad.** Cada especialidad define qué cuenta como un buen artefacto desde su disciplina. El AG-08 (QA) sabe qué hace que una matriz de cobertura sea útil; el AG-05 (Arquitecto) sabe qué hace que un ADR sea completo. Mezclar perfiles diluye los criterios.
2. **Trazabilidad RACI.** Para cada documento existe un responsable (R) y un aprobador (A) explícitos. Esto evita el clásico "alguien debería actualizar esto" que termina sin actualizar.

Trece especialidades no implican trece personas. En equipos pequeños una persona cubre varios roles. Lo que el template fija es el perfil profesional necesario para cumplir el rol, no la dotación de personal.

Las 13 especialidades siguen siendo válidas sin cambios en el modelo de solución más jerarquía de proyectos (§3.10). Lo que se precisa es el alcance de cada una: AG-00 (Product Manager) y AG-01 (Analista de Negocio) operan a nivel solución, porque el negocio es uno por solución; las especialidades AG-02 a AG-11 operan por proyecto, porque la técnica se especializa según el tipo D8 de cada proyecto. Las variantes por tipo D8 que cada especialidad documenta más abajo se aplican, por lo tanto, por proyecto: en una solución con varios proyectos de tipos distintos, una misma especialidad puede producir artefactos con foco distinto en cada proyecto.

## 4.2 Catálogo de las 13 especialidades

### AG-ROOT — Arquitecto de Soluciones Senior

**Alias.** Solutions Architect, Tech Lead documental.
**Responsabilidad principal.** Coherencia integral del repositorio. Onboarding de personas nuevas en cinco minutos de lectura. Validación cruzada entre secciones.
**Documentos que produce.** `docs/README.md` (índice maestro), referencias cruzadas, mapa de trazabilidad.
**Variantes por tipo de proyecto.** Estable en todos los D8.
**Interacciones cross-rol.** Consume artefactos de todos los AG-XX para componer la narrativa global. Valida que la documentación de cada AG-XX es consistente con el resto.

### AG-00 — Product Manager

**Alias.** PM, Product Owner senior en contextos donde el rol no existe formalmente.
**Responsabilidad principal.** Estrategia del producto, alineación con stakeholders, métricas de éxito.
**Documentos que produce.** Visión del producto, alcance, roadmap, acuerdo de equipo, compatibilidad de plataformas.
**Variantes por tipo de proyecto.** En proyectos D8 = library el roadmap es más técnico y menos comercial; en proyectos D8 = web-monolith o D8 = rest-api el roadmap suele integrar fases de adopción por terceros.
**Interacciones cross-rol.** Alimenta a AG-01 (necesidades) y AG-06 (priorización del backlog).

### AG-01 — Analista de Negocio Senior

**Alias.** BA, CBAP certificado en contextos formales.
**Responsabilidad principal.** Elicitación de requisitos de negocio, traducción a necesidades concretas, identificación de stakeholders.
**Documentos que produce.** Necesidades de negocio (NB-XX), criterios de éxito por NB, métricas de valor.
**Variantes por tipo de proyecto.** En library/cli-tool/worker-service los stakeholders son típicamente otros desarrolladores (DX); en web/mobile los stakeholders son usuarios finales (UX).
**Interacciones cross-rol.** Consume visión de AG-00, alimenta a AG-02 (casos de uso) y a AG-06 (épicas).

### AG-02 — Analista Funcional

**Alias.** Ingeniero de Requisitos, IREB certificado en contextos formales.
**Responsabilidad principal.** Especificación funcional formal, casos de uso, reglas de negocio, modelo conceptual de datos.
**Documentos que produce.** Casos de uso (CU-XX), reglas de negocio (RN-XX), reglas conceptuales (RC-XX), modelo conceptual, criterios de aceptación Given/When/Then.
**Variantes por tipo de proyecto.** En library/cli-tool puede no haber actores humanos directos; los actores son consumidores programáticos.
**Interacciones cross-rol.** Consume necesidades de AG-01, alimenta a AG-03 (DX), AG-05 (arquitectura), AG-06 (US), AG-08 (test cases).

### AG-03 — Especialista en Developer Experience / UX

**Alias.** DX Specialist, UX Designer cuando el proyecto tiene usuarios finales.
**Responsabilidad principal.** Diseño de la experiencia de uso, sea de usuarios finales (UX) o de desarrolladores integradores (DX).
**Documentos que produce.** Flujos de uso, wireframes, representaciones de salida, ergonomía del API, journey maps.
**Variantes por tipo de proyecto.**

| D8 | Foco principal |
|---|---|
| library | DX puro: API ergonomy, error messages, snippets |
| cli-tool | DX: argumentos, output formats, exit codes |
| rest-api | DX: OpenAPI spec, naming de endpoints, status codes |
| worker-service | DX limitado: configuración, logging, métricas |
| web-monolith | UX completo de usuario final + DX de admin |
| web-microservices | UX por bounded context + DX de cada API |
| desktop-app | UX completo, ergonomía de teclado/mouse, accesibilidad |
| mobile-app-maui | UX completo, gestos, touch targets, modos offline |

**Interacciones cross-rol.** Consume casos de uso de AG-02, alimenta a AG-05 (contratos del API), AG-10 (developer guide), AG-11 (ejemplos).

### AG-04 — Ingeniero de Prompts

**Alias.** AI Specialist, ML Engineer cuando el proyecto incorpora modelos.
**Responsabilidad principal.** Diseño de prompts estructurados, guardrails, esquemas JSON de salida, evaluación de outputs.
**Documentos que produce.** Prompts versionados, esquemas de salida, few-shot examples, métricas de evaluación.
**Variantes por tipo de proyecto.** Esta sección puede marcarse "no aplica" si el proyecto no incorpora IA en su superficie funcional. La distinción es importante: usar IA como asistente de desarrollo (Copilot) no requiere AG-04; integrarle IA al producto (clasificador, RAG, chatbot) sí.
**Interacciones cross-rol.** Consume especificación de AG-02, alimenta a AG-05 (integración del modelo en la arquitectura) y AG-08 (evaluación de outputs).

### AG-05 — Arquitecto de Software Senior

**Alias.** Software Architect, Solution Architect técnico.
**Responsabilidad principal.** Diseño técnico del sistema, decisiones de arquitectura (ADR), contratos, modelo lógico de datos.
**Documentos que produce.** Arquitectura de la solución, ADRs individuales (`ADR-XX-<kebab>_v1.0.md`), contratos, modelo lógico, extensibilidad, diagramas (C4 o equivalente).
**Variantes por tipo de proyecto.** Ver §7 para la decisión arquitectónica por tipo D8.
**Interacciones cross-rol.** Consume requisitos de AG-02 y AG-03, alimenta a AG-06 (BT técnico), AG-08 (testabilidad), AG-09 (deploy).

### AG-06 — Scrum Master / Agile Coach (Backlog)

**Alias.** Backlog Owner, Agile BA.
**Responsabilidad principal.** Construcción y mantenimiento del Product Backlog, definición de DoR, priorización MoSCoW.
**Documentos que produce.** Product Backlog (US-XX), Backlog técnico (BT-XX), Definition of Ready, mapa US → CU → BT.
**Variantes por tipo de proyecto.** Estable en todos los D8.
**Interacciones cross-rol.** Consume casos de uso de AG-02, alimenta a AG-07 (sprint planning).

### AG-07 — Scrum Master / Gestión Ágil

**Alias.** Scrum Master operativo, Project Manager ágil.
**Responsabilidad principal.** Planificación de sprints, métricas (velocity, burndown), templates de Review y Retro.
**Documentos que produce.** Plan de sprint por iteración, templates de Review, templates de Retro, tabla de velocidad.
**Variantes por tipo de proyecto.** Estable en todos los D8.
**Interacciones cross-rol.** Consume backlog priorizado de AG-06, coordina ejecución con AG-08 (quality gates) y AG-09 (deploy).

### AG-08 — Ingeniero QA / SDET Senior

**Alias.** QA Engineer, SDET, Test Engineer.
**Responsabilidad principal.** Estrategia de testing, Definition of Done, matriz de cobertura, quality gates.
**Documentos que produce.** Estrategia de testing, DoD canónico, casos de prueba referenciales, matriz de cobertura, criterios de validación.
**Variantes por tipo de proyecto.** La pirámide concreta varía: library puede tolerar 80% unit / 15% integración / 5% E2E; web-microservices necesita contract testing explícito.
**Interacciones cross-rol.** Consume criterios de aceptación de AG-02, alimenta a AG-09 (gates del CI/CD).

### AG-09 — Ingeniero DevOps Senior

**Alias.** DevOps Engineer, Release Engineer, Platform Engineer.
**Responsabilidad principal.** Pipeline CI/CD, versionado SemVer, entornos de deploy, supply chain.
**Documentos que produce.** Pipeline CI/CD, estrategia de versionado, entornos de deploy, guía de publicación, SBOM.
**Variantes por tipo de proyecto.** Ver §10 para detalle. library publica en un feed de paquetes; rest-api se despliega a runtime; cli-tool produce binarios firmados.
**Interacciones cross-rol.** Consume DoD de AG-08 y versionado de AG-00, opera transversalmente.

### AG-10 — Technical Writer

**Alias.** Documentation Engineer.
**Responsabilidad principal.** Documentación orientada al consumidor (desarrollador integrador o usuario final), tutorial, referencia, troubleshooting.
**Documentos que produce.** Quick start, conceptos fundamentales, referencia de API, guías de integración, troubleshooting.
**Variantes por tipo de proyecto.** En library el target es desarrollador integrador; en web/mobile incluye también usuario final.
**Interacciones cross-rol.** Consume todo (necesita ver el panorama completo), alimenta a AG-11.

### AG-11 — Developer Advocate

**Alias.** DevRel, Application Engineer, Solutions Engineer.
**Responsabilidad principal.** Aplicaciones de referencia, ejemplos ejecutables, progresión didáctica.
**Documentos que produce.** Ejemplos de complejidad creciente (básico, intermedio, avanzado), README de ejemplos, assets de soporte.
**Variantes por tipo de proyecto.** Estable en todos los D8 pero el material concreto cambia: library produce snippets compilables; cli-tool produce scripts; rest-api produce colecciones Postman / Insomnia.
**Interacciones cross-rol.** Consume developer guide de AG-10 y arquitectura de AG-05.

## 4.3 Tabla resumen del catálogo

| ID | Especialidad | Sección de docs | Artefacto principal | Aplica si |
|---|---|---|---|---|
| AG-ROOT | Arquitecto de Soluciones | `README.md` | Índice maestro | Siempre |
| AG-00 | Product Manager | `00_contexto/` | Visión, alcance | Siempre |
| AG-01 | Analista de Negocio | `01_necesidades_negocio/` | NB-XX | Siempre |
| AG-02 | Analista Funcional | `02_especificacion_funcional/` | CU-XX, RN-XX | Siempre |
| AG-03 | DX/UX Specialist | `03_ux-ui/` | Flujos, wireframes | Siempre (varía foco) |
| AG-04 | Ingeniero de Prompts | `04_prompts_ai/` | Prompts versionados | Solo si producto integra IA |
| AG-05 | Arquitecto de Software | `05_arquitectura_tecnica/` | ADR, contratos | Siempre |
| AG-06 | SM Backlog | `06_backlog-tecnico/` | US, BT, DoR | Siempre |
| AG-07 | SM Sprints | `07_plan-sprint/` | Sprint plans | Siempre |
| AG-08 | QA / SDET | `08_calidad_y_pruebas/` | Estrategia testing, DoD | Siempre |
| AG-09 | DevOps | `09_devops/` | Pipeline, versionado | Siempre |
| AG-10 | Technical Writer | `10_developer_guide/` | Guías de consumo | Siempre |
| AG-11 | Developer Advocate | `11_examples/` | Ejemplos ejecutables | Siempre |

## 4.4 Flujo de trazabilidad entre especialidades

```text
AG-00 (Vision)
  -> AG-01 (Necesidades de negocio)
     -> AG-02 (Casos de uso + reglas)
        -> AG-03 (DX/UX)
        -> AG-04 (Prompts si aplica)
        -> AG-05 (Arquitectura + ADR)
           -> AG-06 (Backlog priorizado)
              -> AG-07 (Sprint planning)
                 -> AG-08 (Calidad y pruebas)
                 -> AG-09 (DevOps)
                    -> AG-10 (Developer guide)
                       -> AG-11 (Ejemplos)
                          -> AG-ROOT (Integracion y coherencia global)
```

Esta cadena es la materialización de la trazabilidad vertical que define el enfoque SDD. Cada artefacto downstream traza explícitamente a su upstream mediante referencias por ID (CU traza a NB, US traza a CU, BT traza a US, Test traza a CU, etc.). Sin esa trazabilidad la documentación se vuelve archipiélago de islas inconexas.

En el modelo de solución más jerarquía de proyectos (§3.10), la cadena conserva su forma pero cierra en dos niveles complementarios. A nivel solución, la visión y las necesidades de negocio (AG-00 y AG-01) son únicas y trazan al valor de negocio de la solución completa. A nivel proyecto, la cadena de especificación funcional a ejemplos (AG-02 a AG-11) se materializa por proyecto, y los casos de uso de cada proyecto trazan hacia arriba a las necesidades de negocio compartidas a nivel solución. La consolidación en `_solucion/` cierra además la trazabilidad horizontal entre proyectos: los contratos inter-proyecto y el grafo de dependencias documentan cómo lo que un proyecto produce satisface lo que otro consume. En el caso degenerado de un único proyecto, ambos niveles colapsan en uno y la cadena se lee tal como en el modelo de tipo único.

---

# §5 Metodología ágil aplicada

## 5.1 Por qué Scrum por defecto

El template SDD 2.1 propone Scrum como metodología ágil por defecto. La elección no es dogmática: se justifica porque Scrum aporta tres cosas que SDD necesita:

1. **Cadencia fija para sincronización spec-código.** El sprint funciona como ventana en la que la spec y el código se alinean. Al cierre de cada sprint, ambos artefactos están coherentes.
2. **Roles explícitos.** Product Owner (que valida la spec contra el negocio) y Scrum Master (que protege el ritmo) tienen contrapartes naturales en AG-00 y AG-07.
3. **Eventos como puntos de control.** Sprint Review valida que el incremento cumple la spec; Retro identifica problemas en el flujo spec-código.

Cuando Scrum no encaja, el template admite Kanban o Scrumban. La elección depende del contexto del proyecto, no de una preferencia ideológica.

## 5.2 Cuándo Scrum, cuándo Kanban, cuándo Scrumban

| Factor | Scrum | Kanban | Scrumban |
|---|---|---|---|
| Predictibilidad de la demanda | Alta o media | Baja (interrupciones frecuentes) | Mixta |
| Tamaño típico del equipo | 3 a 9 personas | 1 a 15 personas | 3 a 9 personas |
| Cadencia de release | Cada 1 a 4 semanas | On-demand | Mixta |
| Tolerancia al cambio durante el sprint | Baja (sprint goal protegido) | Alta (no hay sprint) | Media |
| Ceremonias | Daily, Planning, Review, Retro, Refinement | Daily opcional, Replenishment ad hoc | Subset de Scrum + WIP limits |
| Métrica principal | Velocity | Lead time, cycle time | Velocity + WIP |
| Cuándo elegir | Producto nuevo, equipo cohesionado, roadmap claro | Soporte, mantenimiento, equipos multi-proyecto | Transición desde Scrum cuando la demanda se vuelve menos predecible |

**Recomendación operativa para SDD 2.1.** Empezar con Scrum en proyectos nuevos. Migrar a Scrumban si la demanda externa rompe el sprint goal con frecuencia (más de 30% de los sprints comprometidos se renegocian). Kanban puro queda reservado para equipos de soporte o mantenimiento de productos ya estabilizados.

## 5.3 Sprint 0 y artefactos previos

El Sprint 0 es el sprint conceptual previo al primer sprint productivo. No es un sprint canónico de Scrum (el framework no lo nombra), pero es práctica industrial extendida (Cohn, 2009). Su propósito es producir los artefactos sin los cuales el primer sprint no puede arrancar:

| Artefacto | Producido por | Estado al cierre del Sprint 0 |
|---|---|---|
| Visión del producto | AG-00 | Aprobada |
| Alcance + exclusiones | AG-00 | Aprobado |
| Necesidades de negocio (NB-01..NB-N) | AG-01 | Versionadas |
| Casos de uso prioritarios (Must Have) | AG-02 | Versionados, con criterios Given/When/Then |
| Arquitectura de la solución (alto nivel) | AG-05 | Diagrama C4 nivel 1 (contexto) y nivel 2 (contenedores) |
| ADRs iniciales (D1..D8 + decisiones técnicas fundacionales) | AG-05 | Aprobados |
| Product Backlog inicial | AG-06 | US Must Have estimadas en story points |
| DoR | AG-06 | Acordada por el equipo |
| DoD | AG-08 | Acordada por el equipo |
| Pipeline CI mínimo (build + test) | AG-09 | Funcional en verde |
| Acuerdo de equipo | AG-00 | Firmado |

Solo cuando estos artefactos están en su lugar, el equipo arranca el Sprint 1.

## 5.4 Ceremonias de Scrum

| Ceremonia | Duración (sprint 2 semanas) | Propósito | Output |
|---|---|---|---|
| **Sprint Planning** | 2 a 4 horas | Definir Sprint Goal y comprometer Sprint Backlog | Sprint Backlog, Sprint Goal |
| **Daily Scrum** | 15 minutos | Sincronizar avance, identificar impedimentos | Plan ajustado del día |
| **Sprint Review** | 1 a 2 horas | Inspeccionar el incremento con stakeholders | Feedback, Backlog ajustado |
| **Sprint Retrospective** | 1 a 1,5 horas | Inspeccionar el proceso, decidir mejoras | Acciones concretas para el próximo sprint |
| **Backlog Refinement** | 1 a 2 horas (continuo) | Preparar items para futuros sprints (DoR) | Backlog refinado |

**Anti-patrones de ceremonias.**

- **Daily de status report.** El Daily no es un reporte al manager. Es sincronización entre pares. Si se vuelve reporte, pierde valor.
- **Planning sin DoR.** Comprometerse a historias sin DoR es comprometerse a riesgo no medido.
- **Retro decorativa.** Si las acciones de la Retro no se ejecutan en el siguiente sprint, la ceremonia se vacía de sentido.
- **Refinement saltado.** Sin refinement continuo, el Planning se vuelve un mini-refinement apurado.

## 5.5 Roles de Scrum

| Rol Scrum | Mapeo en SDD 2.1 | Responsabilidad nuclear |
|---|---|---|
| **Product Owner** | AG-00 (Product Manager) | Maximizar el valor del producto; dueño del Product Backlog |
| **Scrum Master** | AG-06 + AG-07 | Facilitar el proceso; remover impedimentos; coachar al equipo |
| **Development Team** | Implementadores (devs, QA embebido) | Construir el incremento que cumple el Sprint Goal y el DoD |

En equipos pequeños es común que una persona cubra varios roles, pero Scrum prohíbe explícitamente que el PO sea el mismo que el SM (conflicto de intereses) y que el SM sea miembro de tiempo completo del Development Team (porque entonces no protege el proceso).

## 5.6 Artefactos de Scrum

| Artefacto | Dueño | Propósito |
|---|---|---|
| **Product Backlog** | Product Owner | Lista priorizada de todo lo que el producto necesita |
| **Sprint Backlog** | Development Team | Subset del Product Backlog comprometido para el sprint actual |
| **Incremento** | Development Team | Conjunto de items del backlog terminados (cumplen DoD) al cierre del sprint |

Los tres artefactos son siempre transparentes y siempre actualizados. Un Product Backlog desactualizado es peor que no tener Product Backlog.

## 5.7 Definition of Ready (DoR) canónica

Una historia entra a Sprint Planning solo si cumple el DoR. El template SDD 2.1 propone como DoR mínima:

- La historia está escrita en formato "Como [rol], quiero [acción], para [beneficio]".
- Tiene al menos tres escenarios de criterios de aceptación (happy path + dos edge cases) en formato Given/When/Then.
- Está estimada en story points por el Development Team mediante Planning Poker o similar.
- Las dependencias técnicas y de diseño están identificadas.
- Los wireframes o contratos de API necesarios están disponibles.
- El PO la aprobó como prioridad del sprint.
- Traza a un caso de uso (CU-XX) y a una necesidad de negocio (NB-XX).

## 5.8 Definition of Done (DoD) canónica

Una historia se considera terminada solo si cumple el DoD. El template SDD 2.1 propone como DoD mínima:

- El código fue revisado y aprobado en pull request por al menos una persona distinta del autor.
- Tiene tests automatizados (unit + integración cuando aplica).
- La cobertura de tests no degrada el umbral del proyecto (típicamente 70% o el valor pactado).
- Pasa todos los tests en CI (build verde).
- Fue probada manualmente contra sus criterios de aceptación.
- La documentación correspondiente (CU, ADR, README) fue actualizada si la historia la afectaba.
- El PO la aceptó en la Sprint Review.

**Diferencia clave entre DoR y DoD.**

- **DoR** = condiciones para empezar a construir.
- **DoD** = condiciones para terminar de construir.

Sin DoR explícita el equipo arranca trabajo inmaduro. Sin DoD explícita el equipo acumula deuda silenciosa.

---

# §6 Técnicas de descomposición y planificación

## 6.1 Por qué la descomposición importa

Una de las falencias más frecuentes en equipos ágiles novatos es comprometer trabajo en sprints sin haber descompuesto suficientemente los items. Una historia de "construir el módulo de pagos" no es accionable: es un proyecto chico, no una historia. Las técnicas que se describen acá ayudan a llegar a items lo suficientemente pequeños como para terminarlos dentro de un sprint pero lo suficientemente significativos como para entregar valor.

## 6.2 Vertical Slicing

Una técnica para cortar features verticalmente, atravesando todas las capas (UI, lógica, persistencia) en lugar de cortar horizontalmente (primero todas las UIs, después toda la lógica). Cada slice vertical es entregable independiente.

**Ejemplo.** En lugar de "Sprint 1: pantallas, Sprint 2: backend, Sprint 3: base de datos", el equipo entrega "Sprint 1: registro de usuario funcional end-to-end (UI + backend + DB)" y "Sprint 2: login funcional end-to-end".

**Cuándo usarla.** Siempre que sea posible. Es la técnica fundacional de la entrega de valor incremental.

## 6.3 Walking Skeleton

Un esqueleto mínimo del sistema que atraviesa todas las capas y se despliega a producción (o un entorno equivalente) lo antes posible. Después se va engordando con funcionalidad real.

**Ejemplo.** En un proyecto rest-api, el walking skeleton podría ser un endpoint `/health` que responde 200, desplegado en el ambiente de staging, con pipeline CI/CD funcional. Cero valor de negocio, pero el ciclo end-to-end queda probado.

**Cuándo usarla.** Al inicio de cualquier proyecto que necesite validar integración de stack tempranamente.

## 6.4 Story Mapping (Jeff Patton)

Técnica de visualización del backlog en dos ejes: horizontal por flujo del usuario (qué hace primero, qué después), vertical por prioridad (qué es Must Have, qué Should Have).

**Ejemplo.** En un sistema de turnos médicos: eje horizontal = "Buscar profesional → Elegir horario → Confirmar reserva → Recibir confirmación". Eje vertical = "MVP: las cuatro tareas funcionando; v2: agregar pago online; v3: agregar reprogramación".

**Cuándo usarla.** Al inicio del proyecto, cuando se construye el Product Backlog y se busca consenso sobre el MVP.

## 6.5 Example Mapping

Técnica de Matt Wynne para refinar una historia explorando ejemplos concretos antes de escribir código. Se trabaja con tarjetas de colores: amarillo (historia), azul (regla), verde (ejemplo), rojo (pregunta abierta).

**Ejemplo.** Historia "Cancelar turno". Regla 1: "Se puede cancelar hasta 2hs antes". Ejemplo: "Turno a las 10:00, cancelo a las 9:30 → permitido". Pregunta: "¿Qué pasa si el médico ya marcó como confirmado?".

**Cuándo usarla.** En refinement, especialmente para historias con muchas reglas de negocio.

## 6.6 Spike

Time-box de investigación técnica con resultado documentado. No produce funcionalidad de producto pero reduce incertidumbre.

**Ejemplo.** "Spike de 1 día: investigar si la librería X soporta multi-tenancy nativo o requiere implementación custom. Output: ADR con la conclusión".

**Cuándo usarla.** Cuando una decisión técnica bloquea estimación de historias dependientes.

## 6.7 ATDD (Acceptance Test-Driven Development)

Los criterios de aceptación se escriben primero como tests ejecutables (Given/When/Then automatizables) y la implementación se considera terminada cuando esos tests pasan.

**Cuándo usarla.** Siempre que sea posible. Es la materialización ejecutable de la spec.

## 6.8 Mob Programming

Todo el equipo trabaja simultáneamente en una sola estación, alternando quién tiene el teclado cada pocos minutos. Es la versión extrema del pair programming.

**Cuándo usarla.** En la onboarding de personas nuevas, al construir el walking skeleton, al resolver bugs críticos con conocimiento disperso.

## 6.9 Otras técnicas

| Técnica | Idea nuclear | Cuándo usarla |
|---|---|---|
| **Thin Slice / Tracer Bullet** | Implementación mínima end-to-end de una feature, para validar arquitectura | Inicio de feature técnica compleja |
| **Shape Up (Basecamp)** | Ciclos de 6 semanas con appetite fijo en lugar de scope fijo | Equipos pequeños con autonomía total |
| **Impact Mapping (Adzic)** | Mapa Goal → Actor → Impact → Deliverable | Alineación de stakeholders sobre el "por qué" antes del "qué" |
| **Dual-Track Agile** | Discovery (UX, prototipos) y Delivery (sprints) corriendo en paralelo | Producto con incertidumbre alta de mercado |

## 6.10 Tabla resumen — Cuándo usar cada técnica

| Si el problema es... | Técnica recomendada |
|---|---|
| El backlog es una lista plana, no se entiende el flujo | Story Mapping |
| Una historia es demasiado grande para un sprint | Vertical Slicing |
| Hay reglas de negocio confusas | Example Mapping |
| Hay incertidumbre técnica que bloquea estimación | Spike |
| El equipo discute sobre la implementación sin ver el output | Walking Skeleton + Thin Slice |
| Los criterios de aceptación no se verifican consistentemente | ATDD |
| Conocimiento concentrado en pocas personas | Mob Programming |
| Stakeholders desalineados sobre el porqué | Impact Mapping |
| Producto nuevo con incertidumbre de mercado | Dual-Track Agile |

---

# §7 Estilos arquitectónicos soportados

## 7.1 La decisión D8 — Ocho tipos de proyecto

El template SDD 2.1 soporta ocho tipos de proyecto. La decisión D8 (definida en el intake) selecciona uno por proyecto, y el resto del template se calibra en consecuencia para ese proyecto. En el modelo de solución más jerarquía de proyectos (§3.10), una solución agrupa N proyectos y cada uno declara su propio valor D8; la solución no tiene un D8 propio, su tipo es compuesto. El conjunto de tipos sigue siendo cerrado y de exactamente ocho valores: lo que cambió es que la decisión se toma por proyecto, no por repositorio.

| ID D8 | Tipo de proyecto | Descripción corta |
|---|---|---|
| **library** | Librería reutilizable | Distribuida como paquete (NuGet, npm, PyPI, Maven) |
| **web-monolith** | Aplicación web monolítica | Un único deploy, todas las capas en un proceso |
| **web-microservices** | Plataforma web de microservicios | N servicios autónomos detrás de un API gateway |
| **desktop-app** | Aplicación de escritorio | Instalable en Windows/Mac/Linux |
| **mobile-app-maui** | Aplicación móvil multiplataforma | iOS + Android + opcionalmente desktop |
| **rest-api** | API REST standalone | Servicio HTTP sin frontend acoplado |
| **cli-tool** | Herramienta de línea de comandos | Binario invocable desde shell |
| **worker-service** | Servicio en background | Procesa colas, eventos, timers; sin UI |

## 7.2 Estilos arquitectónicos por tipo D8

Cada tipo D8 tiende a un conjunto de estilos arquitectónicos por defecto, pero la elección final depende del contexto.

### 7.2.1 library

**Estilo dominante.** Arquitectura por capas, separación strict de API pública versus internals.

**Patrón típico.**

```text
ProyectoLib/
  ├── PublicAPI/      (interfaces, modelos públicos, factories)
  ├── Internal/       (implementación, no exportada)
  ├── Extensions/     (puntos de extensibilidad)
  └── Tests/
```

**Decisiones canónicas.**

- API mínima superficial; lo que no se exporta no se puede romper accidentalmente.
- SemVer estricto: cualquier breaking change en API pública bumpea MAJOR.
- Sin estado global mutable; preferencia por funciones puras y objetos inmutables.
- Cero dependencias transitivas innecesarias.

### 7.2.2 web-monolith

**Estilo dominante.** Monolito modular con Clean Architecture; opcionalmente CQRS ligero.

**Patrón típico.**

```text
src/
  ├── Domain/
  ├── Application/
  ├── Infrastructure/
  ├── Web/            (controllers, vistas, assets)
  └── Tests/
```

**Decisiones canónicas.**

- Una sola base de datos relacional, accedida vía repositorio o EF Core.
- Módulos separados por bounded context; reglas claras de qué módulo puede importar qué.
- Posibilidad de extraer módulos a microservicios en el futuro sin reescritura masiva (patrón Monolito Modular → Microservicios).

### 7.2.3 web-microservices

**Estilo dominante.** Microservicios + API Gateway + Identity Provider centralizado + comunicación asíncrona via event bus.

**Patrón típico.**

```text
src/
  ├── BuildingBlocks/
  │   ├── SharedKernel/
  │   ├── EventBus/
  │   └── AuthShared/
  ├── Gateway/
  ├── Identity/
  ├── Services/
  │   ├── ServicioA/   (Domain, Application, Infrastructure, API)
  │   ├── ServicioB/
  │   └── ServicioC/
  └── Web/             (frontends consumidores)
```

**Decisiones canónicas.**

- Base de datos por servicio. Ningún servicio accede a la DB de otro.
- Comunicación síncrona via HTTP/gRPC para request-response; asíncrona via eventos para notificaciones.
- Multi-tenancy resuelta via claim en JWT.
- Cada servicio expone health check, métricas y trazas.

**Cuándo NO usarlo.** Equipos menores a cinco personas, dominios sin fronteras claras, prototipos. La complejidad operativa de microservicios solo se justifica con escala humana suficiente.

### 7.2.4 desktop-app

**Estilo dominante.** MVVM (Model-View-ViewModel) sobre framework GUI nativo (WPF, Avalonia, Electron, Qt).

**Patrón típico.**

```text
src/
  ├── Domain/
  ├── Application/    (use cases, ViewModels)
  ├── Infrastructure/ (persistencia local, llamadas API si aplica)
  ├── UI/             (vistas, recursos visuales)
  └── Tests/
```

**Decisiones canónicas.**

- Separación strict View / ViewModel.
- Persistencia local típicamente SQLite o archivos del usuario.
- Actualización del binario gestionada por mecanismo de auto-update o store.
- Atención específica a accesibilidad (lectores de pantalla, contraste, navegación por teclado).

### 7.2.5 mobile-app-maui

**Estilo dominante.** MVVM compartido entre plataformas; lógica de negocio aislada de la UI específica.

**Patrón típico.**

```text
src/
  ├── App.Shared/       (lógica, ViewModels, modelos)
  ├── App.UI/           (XAML compartido)
  ├── Platforms/
  │   ├── Android/
  │   ├── iOS/
  │   └── Windows/
  └── Tests/
```

**Decisiones canónicas.**

- Modo offline first; sincronización cuando hay conectividad.
- Permisos del sistema gestionados explícitamente (cámara, ubicación, notificaciones).
- Distribución vía stores (Google Play, App Store) o mecanismo enterprise.
- Pruebas E2E en dispositivos reales o emuladores oficiales.

### 7.2.6 rest-api

**Estilo dominante.** Clean Architecture + CQRS opcional; especificación OpenAPI primero.

**Patrón típico.**

```text
src/
  ├── Domain/
  ├── Application/    (commands, queries, validators)
  ├── Infrastructure/ (persistencia, integraciones)
  ├── API/            (controllers, middleware, OpenAPI)
  └── Tests/
```

**Decisiones canónicas.**

- OpenAPI 3.x como contrato versionado.
- Versionado del API via URL (`/api/v1/`) o header.
- Problem Details (RFC 7807) para respuestas de error.
- Paginación, filtrado y ordenamiento estandarizados.
- Health checks, métricas Prometheus, trazas OpenTelemetry.

### 7.2.7 cli-tool

**Estilo dominante.** Command pattern; cada subcomando es una unidad invocable.

**Patrón típico.**

```text
src/
  ├── Commands/       (uno por subcomando)
  ├── Core/           (lógica compartida)
  ├── Output/         (formateadores: text, json, table)
  └── Tests/
```

**Decisiones canónicas.**

- Argumentos parseados con librería convencional del stack (System.CommandLine, Click, Cobra, Commander).
- Exit codes consistentes: 0 = éxito, !=0 = error.
- Output dual: human-readable por defecto, JSON cuando `--json`.
- Sin dependencias de red salvo que el comando lo requiera explícitamente.

### 7.2.8 worker-service

**Estilo dominante.** Pipeline / Event-driven; el servicio escucha eventos o timers y procesa.

**Patrón típico.**

```text
src/
  ├── Domain/
  ├── Application/    (handlers de mensajes/eventos)
  ├── Infrastructure/ (broker, persistencia)
  ├── Worker/         (host, registro de handlers)
  └── Tests/
```

**Decisiones canónicas.**

- Idempotencia obligatoria en los handlers (el mismo mensaje puede llegar más de una vez).
- Dead letter queue para mensajes que fallan persistentemente.
- Métricas de cola (depth, throughput, lag) expuestas para monitoreo.
- Apagado limpio (drain de mensajes en proceso antes de matar el container).

## 7.3 Tabla de decisión — Estilo arquitectónico

| Factor | Library | Monolito | Monolito modular | Microservicios | Pipeline / Worker |
|---|---|---|---|---|---|
| Tamaño del equipo | 1-3 | 1-5 | 3-15 | 10+ | 1-5 |
| Dominios de negocio | N/A (transversal) | 1-2 | 2-5 | 5+ | 1 (procesar eventos) |
| Deploy independiente | N/A | No | Deseable | Requerido | Sí (por worker) |
| Complejidad operativa | Baja | Baja | Media | Alta | Media |
| Time to market | Rápido | Rápido | Medio | Lento | Medio |
| Mejor para | Funcionalidad reutilizable | MVP, prototipos | Producto con módulos claros | Plataforma multi-equipo | Procesamiento async, ETL |

## 7.4 Tabla de cobertura D8 → estilo dominante

La tabla mantiene los ocho tipos D8 del conjunto cerrado. En el modelo de solución más jerarquía de proyectos se lee por proyecto: cada proyecto de la solución toma la fila correspondiente a su propio tipo D8.

| D8 | Estilo dominante por defecto | Patrón alternativo |
|---|---|---|
| library | Capas + API pública mínima | Plugin-based si soporta extensiones |
| web-monolith | Monolito modular + Clean Architecture | MVC clásico para CRUDs simples |
| web-microservices | Microservicios + API Gateway + Event Bus | Modular monolith si el equipo es chico |
| desktop-app | MVVM + Clean Architecture | MVP / MVI en stacks específicos |
| mobile-app-maui | MVVM + lógica compartida | MVI (Redux-like) si la UI es muy reactiva |
| rest-api | Clean Architecture + CQRS | Hexagonal / Ports & Adapters |
| cli-tool | Command pattern | Pipe & filters para tools de procesamiento |
| worker-service | Event-driven + idempotent handlers | Pipeline con stages explícitos |

## 7.5 Patrones transversales

Independientemente del tipo D8, hay patrones que aplican universalmente:

- **Clean Architecture** (Martin, 2017): separación de capas con dependencias apuntando al núcleo. Aplicable en library, monolitos, microservicios, desktop, mobile y rest-api.
- **CQRS** (Young, 2010): separación de lecturas y escrituras. Aplicable en rest-api, microservicios y monolitos modulares con dominio rico.
- **Event-driven**: comunicación por eventos en lugar de llamadas directas. Aplicable en microservicios y worker-service.
- **Repository pattern**: abstracción del acceso a datos. Aplicable en cualquier proyecto con persistencia.

## 7.6 La composición de la solución como decisión arquitectónica

Las tablas y los estilos de este capítulo describen el estilo interno de cada proyecto: cómo se organiza por dentro según su tipo D8. En el modelo de solución más jerarquía de proyectos (§3.10) hay una decisión arquitectónica adicional, de un nivel superior: qué proyectos integran la solución y qué dependencias hay entre ellos. Esa composición (qué piezas y cómo se conectan) es una decisión de nivel solución, por encima del estilo interno de cada proyecto, y se documenta como ADR de nivel solución. El grafo de dependencias resultante debe ser acíclico, lo que habilita un orden topológico de generación y de build (primero las dependencias, después los dependientes). Decidir, por ejemplo, extraer una library compartida que un rest-api y un cli-tool consumen, o introducir un worker-service para descargar procesamiento asíncrono, son decisiones de composición que anteceden y enmarcan al estilo interno de cada proyecto.

---

# §8 UX/UI/DX como continuo

## 8.1 Las tres caras de una misma disciplina

UX (User Experience), UI (User Interface) y DX (Developer Experience) son variantes de una misma disciplina: el diseño de interacción humano-sistema. Lo que cambia es el humano y la interfaz.

| Disciplina | Humano | Interfaz | Output típico |
|---|---|---|---|
| **UX** | Usuario final | Pantalla, gesto, voz | Journey, prototipo, métricas SUS/NPS |
| **UI** | Usuario final | Píxeles concretos | Mockup hi-fi, design system |
| **DX** | Desarrollador integrador | API, SDK, documentación, mensaje de error | OpenAPI, snippets, troubleshooting |

Las heurísticas de usabilidad, las leyes psicológicas y los estándares aplican a las tres con instanciaciones específicas. Un mensaje de error confuso es mala UX en una app de banco y mala DX en una librería; el principio que se viola es el mismo (heurística 9 de Nielsen: ayudar a reconocer, diagnosticar y recuperarse de errores).

## 8.2 Heurísticas de Nielsen (versión condensada)

Jakob Nielsen (1994) propuso diez heurísticas que siguen siendo la herramienta de inspección más usada de la industria:

1. **Visibilidad del estado del sistema.** El usuario siempre sabe qué está pasando.
2. **Correspondencia con el mundo real.** Lenguaje y conceptos familiares al usuario.
3. **Control y libertad.** Salidas de emergencia, deshacer.
4. **Consistencia y estándares.** Mismos términos para mismas cosas.
5. **Prevención de errores.** Mejor evitar el error que mostrar un mensaje.
6. **Reconocer antes que recordar.** Minimizar carga de memoria.
7. **Flexibilidad y eficiencia.** Aceleradores para usuarios expertos.
8. **Diseño estético y minimalista.** Cada elemento extra compite por atención.
9. **Reconocer, diagnosticar, recuperar errores.** Mensajes en lenguaje claro con sugerencia de solución.
10. **Ayuda y documentación.** Buscable, contextual, orientada a la tarea.

## 8.3 Leyes UX más usadas

| Ley | Enunciado | Aplicación |
|---|---|---|
| **Ley de Fitts** | El tiempo para alcanzar un target depende de su tamaño y distancia | Hacer grandes los botones primarios; ubicar acciones frecuentes accesibles |
| **Ley de Hick** | El tiempo de decisión crece con el número de opciones | Limitar opciones simultáneas, agrupar en jerarquías |
| **Ley de Miller** | La memoria de trabajo maneja 7±2 chunks | No exigir recordar más de 7 ítems simultáneamente |
| **Ley de Jakob** | Los usuarios esperan que tu sitio funcione como los otros que ya usan | Respetar convenciones de la plataforma |
| **Ley de Tesler** | Toda app tiene complejidad inherente; alguien debe absorberla | Decidir si la absorbe el usuario o el sistema |

## 8.4 Accesibilidad WCAG 2.2 nivel AA

WCAG 2.2 (W3C, 2023) define tres niveles de conformidad: A (mínimo), AA (estándar industrial), AAA (estricto). El template SDD 2.1 recomienda AA como objetivo por defecto.

**Cuatro principios POUR.**

- **P**erceivable: contenido disponible para los sentidos.
- **O**perable: interfaz operable por cualquier modo de entrada.
- **U**nderstandable: contenido comprensible.
- **R**obust: contenido robusto para tecnologías asistivas.

**Criterios típicos del nivel AA.**

- Contraste mínimo 4.5:1 entre texto y fondo.
- Navegación por teclado completa, focus visible.
- Texto alternativo en imágenes informativas.
- Labels asociados a controles de formulario.
- Sin información transmitida solo por color.
- Encabezados jerárquicos correctos (h1 → h2 → h3 sin saltos).

## 8.5 Diátaxis para documentación

Diátaxis (Procida, 2022) es un framework que organiza la documentación técnica en cuatro modos según dos ejes: orientación (a la práctica o al estudio) y propósito (adquirir habilidad o entender).

| Modo | Orientación | Propósito | Cuándo se usa |
|---|---|---|---|
| **Tutorial** | Práctica | Adquirir habilidad | "Quiero empezar de cero" |
| **How-to** | Práctica | Resolver problema concreto | "Necesito hacer X" |
| **Reference** | Estudio | Encontrar información precisa | "¿Cuál era el parámetro Y?" |
| **Explanation** | Estudio | Entender el porqué | "¿Por qué funciona así?" |

El template SDD 2.1 organiza los artefactos por modo Diátaxis:

| Artefacto | Modo Diátaxis |
|---|---|
| `docs/10_developer_guide/quick-start.md` | Tutorial |
| `docs/10_developer_guide/troubleshooting.md` | How-to |
| `docs/05_arquitectura_tecnica/contratos-*.md` | Reference |
| `docs/05_arquitectura_tecnica/ADR-XX-*.md` | Explanation |

## 8.6 DX específico de APIs

Cuando el "usuario" es un desarrollador integrador, las heurísticas se instancian con vocabulario propio:

| Heurística general | Versión DX |
|---|---|
| Visibilidad del estado | HTTP status codes precisos + Problem Details |
| Correspondencia con el mundo real | Naming de endpoints en términos del dominio, no de la implementación |
| Control y libertad | Endpoints idempotentes; operaciones reversibles cuando aplica |
| Consistencia y estándares | REST/OpenAPI; convenciones de paginación, filtrado, ordenamiento |
| Prevención de errores | Validación temprana; respuestas 400 con detalles |
| Reconocer antes que recordar | OpenAPI navegable, SDKs auto-generados |
| Flexibilidad y eficiencia | Bulk endpoints, expansión condicional (sparse fieldsets) |
| Diseño minimalista | No exponer endpoints "por las dudas" |
| Recuperar errores | Mensajes accionables: qué falló, qué hacer al respecto |
| Ayuda y documentación | Quick start, ejemplos copy-paste, troubleshooting |

## 8.7 Catálogo de reglas de diseño

El plano `devs/` incluye un catálogo de reglas de diseño en `devs/references/design/`, insumo normativo de la categoría 03. Sigue un modelo base→especialización: un documento base de diseño web genérico (`design-rules-web-generico_v1.0.md`), agnóstico de framework, y especializaciones por stack que heredan del base y mapean cada token y patrón a su tecnología concreta (la primera es `design-rules-blazor-mudblazor_v1.0.md`; están previstas las de HTML puro, MAUI y Blazor en MAUI). El AG-03 carga el catálogo a través de su índice `_index_design-rules.md`, aplica siempre el documento base y, si existe, la especialización del stack declarado en el intake. Los tokens, patrones, estados e iconografía del catálogo son normativos: el subagente los hereda en lugar de definir decisiones visuales ad hoc por proyecto. Este subárbol es metodológico, vive en `devs/` y no en `docs/`.

---

# §9 Calidad y pruebas en SDD

## 9.1 Por qué la pirámide de testing

La pirámide de testing (Cohn, 2009) es la heurística de distribución de tests más extendida en la industria: muchos tests rápidos en la base (unit), menos tests medios (integración), pocos tests lentos en la cima (E2E).

```text
        /\
       /  \         E2E (pocos, lentos, caros)
      /----\
     /      \       Integration (medios)
    /--------\
   /          \     Unit (muchos, rápidos, baratos)
  /____________\
```

**Por qué esta forma.** Tests unitarios son baratos de escribir, rápidos de ejecutar, fáciles de mantener. Tests E2E son caros, lentos y frágiles. Optimizar el costo total exige más unitarios y menos E2E.

**Cifras típicas (orientativas, no normativas).**

| Capa | Porcentaje del total |
|---|---|
| Unit | 60–80% |
| Integration | 15–25% |
| Snapshot / Contract | 5–10% |
| E2E | 5–10% |

## 9.2 Tipos de prueba relevantes

| Tipo | Qué prueba | Cuándo aplica |
|---|---|---|
| **Unit testing** | Una unidad aislada (función, clase) | Siempre |
| **Integration testing** | Interacción entre módulos | Cuando hay dependencias internas no triviales |
| **Contract testing** | Que dos servicios respeten su contrato | Microservicios, integraciones |
| **Snapshot testing** | Que la salida no cambie inadvertidamente | UI, generadores de output |
| **End-to-end (E2E)** | Flujo completo desde la perspectiva del usuario | Críticos de negocio |
| **Property-based testing** | Propiedades invariantes sobre entradas aleatorias | Lógica compleja, parsers, algoritmos |
| **Mutation testing** | Que los tests detectan cambios en la implementación | Validación de la calidad de la suite de tests |
| **Performance testing** | Latencia, throughput, uso de recursos | Componentes con SLA explícito |
| **Security testing** | Vulnerabilidades conocidas | Antes de cada release a producción |

## 9.3 ISO/IEC 25010 — Atributos de calidad

ISO/IEC 25010:2011 (actualizada en 2023) define ocho atributos de calidad de producto. SDD 2.1 los usa como checklist en la definición de la estrategia de calidad por proyecto.

| Atributo | Definición operativa | Cómo se verifica |
|---|---|---|
| **Functional suitability** | El sistema hace lo que la spec dice | Casos de prueba que validan los criterios de aceptación |
| **Performance efficiency** | El sistema responde dentro de límites aceptables | Tests de carga, SLOs |
| **Compatibility** | Convive con otros sistemas | Tests de integración, contract tests |
| **Usability** | Es usable por la audiencia objetivo | Pruebas de usabilidad, SUS, métricas UX |
| **Reliability** | Funciona en condiciones nominales sin fallar | Tests de larga duración, chaos engineering |
| **Security** | Protege información y funciones | Pentesting, SAST, DAST, SCA |
| **Maintainability** | Es modificable por el equipo | Métricas de complejidad, cobertura, time-to-merge |
| **Portability** | Se puede mover a otros entornos | Tests en distintos ambientes, IaC |

## 9.4 Quality gates en CI/CD

Los **quality gates** son checks automáticos que un cambio debe pasar para ser mergeado o desplegado. Si fallan, el cambio no avanza.

| Gate | Qué verifica | Falla si |
|---|---|---|
| **Build** | El código compila | Errores de compilación |
| **Lint** | Estilo y reglas estáticas | Violaciones de reglas |
| **Tests unitarios** | Comportamiento por unidad | Cualquier test rojo |
| **Cobertura mínima** | Porcentaje de líneas cubiertas | Cobertura < umbral pactado |
| **Tests de integración** | Interacción entre módulos | Cualquier test rojo |
| **SAST** | Análisis estático de seguridad | CVEs por encima de severidad acordada |
| **SCA** | Dependencias vulnerables | CVEs en dependencias |
| **Análisis de licencias** | Compatibilidad de licencias | Licencia incompatible (ej: GPL en proyecto MIT) |

## 9.5 Definition of Done como artefacto de calidad

El DoD (ver §5.8) es el artefacto que materializa los quality gates en una checklist legible por humanos. CI/CD enforza la parte automatizable; el equipo valida la parte manual.

## 9.6 Tabla — Atributo de calidad vs técnica de validación

| Atributo (ISO 25010) | Técnica primaria | Técnica secundaria |
|---|---|---|
| Functional suitability | Tests unitarios + integración + ATDD | E2E |
| Performance efficiency | Load testing | Profiling |
| Compatibility | Contract testing | Tests en matriz de entornos |
| Usability | Pruebas de usabilidad con usuarios | Heurísticas (Nielsen) |
| Reliability | Tests de larga duración | Chaos engineering |
| Security | SAST + DAST + SCA | Pentesting periódico |
| Maintainability | Métricas (complejidad, cobertura) | Code review, refactoring continuo |
| Portability | Tests en ambientes distintos | Containerización + IaC |

---

# §10 DevOps y release engineering

## 10.1 Qué es DevOps en una línea

DevOps es el conjunto de prácticas culturales, técnicas y de medición que reducen el tiempo entre la decisión de cambiar el software y el momento en que ese cambio está en manos del usuario, manteniendo o mejorando la confiabilidad (Forsgren, Humble & Kim, 2018).

No es un rol. No es un equipo. No es una herramienta. Es una disciplina que se evalúa por outcomes (DORA metrics) no por outputs (cantidad de pipelines, líneas de YAML).

## 10.2 CALMS — Los cinco pilares

| Pilar | Definición |
|---|---|
| **Culture** | Responsabilidad compartida dev/ops; postmortems blameless |
| **Automation** | Eliminación sistemática del trabajo manual repetible |
| **Lean** | Optimización del flujo end-to-end; reducir batch size y WIP |
| **Measurement** | Decisión por datos; DORA metrics, SLI/SLO |
| **Sharing** | Conocimiento como bien común; ADRs, documentación viva |

## 10.3 Métricas DORA

| Métrica | Definición | Elite |
|---|---|---|
| **Lead Time for Changes** | Tiempo entre commit y disponibilidad en prod | < 1 hora |
| **Deployment Frequency** | Cadencia de deploys exitosos | On-demand |
| **Change Failure Rate** | % de deploys que requieren remediación | 0–5% |
| **Failed Deployment Recovery Time** | Tiempo para restaurar tras deploy fallido | < 1 hora |

## 10.4 SemVer 2.0.0

**Semantic Versioning** (semver.org) es el estándar de versionado para librerías y APIs. La versión tiene formato `MAJOR.MINOR.PATCH`.

- **MAJOR**: cambios incompatibles en la API pública.
- **MINOR**: funcionalidad nueva retro-compatible.
- **PATCH**: bug fixes retro-compatibles.

Suffixes opcionales: `-alpha.1`, `-beta.2`, `-rc.1` para pre-releases; `+build.123` para metadata de build.

**Regla nuclear.** Un breaking change en API pública exige bump MAJOR. Si MAJOR está en 0, la API se considera inestable y cualquier cambio puede romper consumidores.

## 10.5 Conventional Commits

**Conventional Commits 1.0.0** (conventionalcommits.org) es la convención que estructura los mensajes de commit para que herramientas automáticas puedan inferir el bump SemVer.

Formato:

```text
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

Tipos canónicos: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `perf`, `build`, `ci`, `revert`.

| Tipo del commit | Bump SemVer inferido |
|---|---|
| `feat:` | MINOR |
| `fix:` | PATCH |
| `feat!:` o `fix!:` con `BREAKING CHANGE:` | MAJOR |
| Resto (`docs:`, `chore:`, `test:`, etc.) | Ningún bump |

## 10.6 GitHub Flow y Trunk-Based Development

**GitHub Flow.** Una rama main protegida. Cualquier cambio se hace en feature branches que mergean a main vía PR. Releases se taggean desde main.

**Trunk-Based Development.** Variante más estricta: feature branches duran horas o pocos días. Releases continuos desde trunk. Feature flags para ocultar trabajo en progreso.

**Cuándo cuál.**

- GitHub Flow: equipos pequeños a medianos, releases semanales o quincenales.
- Trunk-Based: equipos maduros con automatización fuerte, releases diarios o varias veces al día.

## 10.7 Herramientas de versionado automático

| Herramienta | Stack típico | Cómo opera |
|---|---|---|
| **MinVer** | .NET | Infiere versión desde tags git |
| **GitVersion** | .NET, multipropósito | Infiere versión desde branch + tags |
| **semantic-release** | Node.js, multistack | Infiere versión desde Conventional Commits |
| **release-please** | Multipropósito | Genera PRs de release automáticamente |

## 10.8 Supply chain — SBOM, firma, SLSA

A partir de 2022 (Log4Shell, SolarWinds) la cadena de suministro de software se volvió primer ciudadano de la seguridad.

| Práctica | Qué es | Objetivo |
|---|---|---|
| **SBOM** (Software Bill of Materials) | Lista de todos los componentes que conforman el artefacto | Trazabilidad: ante un CVE, saber qué releases son vulnerables |
| **Firma de artefactos** | Firma criptográfica de paquetes (Sigstore, GPG) | Verificar autoría e integridad |
| **SLSA** (Supply-chain Levels for Software Artifacts) | Marco de niveles de confianza (L1..L4) | Estandarizar madurez de supply chain |
| **NIST SSDF SP 800-218** | Marco de prácticas seguras de desarrollo | Compliance regulatorio |
| **CycloneDX 1.6** | Formato estándar para SBOM | Interoperabilidad |

**Recomendación SDD 2.1.** Como mínimo SBOM en cada release. Firma cuando el feed lo soporte. Apuntar a SLSA L2 como objetivo para librerías públicas.

## 10.9 Feeds preview vs stable

| Feed | Audiencia | Cadencia | Versión típica |
|---|---|---|---|
| **Preview** | Early adopters, testers | A cada merge a main | `1.2.3-preview.42` |
| **Stable** | Producción | Manual / planificado | `1.2.3` |

El feed preview permite que consumidores integren cambios temprano y reporten problemas antes del release estable.

## 10.10 Observabilidad — Logs, métricas, trazas

| Pilar | Para qué sirve | Herramientas típicas |
|---|---|---|
| **Logs** | Reconstruir qué pasó en un evento puntual | ELK, Loki, Splunk, Seq |
| **Métricas** | Tendencias, agregaciones, SLOs | Prometheus + Grafana, Datadog |
| **Trazas** | Latencia y errores cruzando servicios | OpenTelemetry, Jaeger, Zipkin |

**OpenTelemetry** (CNCF Graduated, 2024) es el estándar de facto para instrumentación neutra. SDD 2.1 lo recomienda como punto de partida.

## 10.11 GitOps

Patrón donde el estado deseado de infraestructura y aplicaciones está declarado en un repo git, y un agente (ArgoCD, Flux) reconcilia el estado real con el declarado.

**Cuándo aplica.** Despliegues a Kubernetes con múltiples ambientes (dev/staging/prod) y múltiples servicios. **Cuándo no aplica.** Librerías, CLIs, desktop apps (no hay "ambiente" a reconciliar).

## 10.12 Tabla de decisión — Versionado y release

| Tipo D8 | Versionado | Distribución | Estrategia de release |
|---|---|---|---|
| library | SemVer estricto | Feed de paquetes (NuGet, npm, PyPI, Maven) | Stable + preview |
| web-monolith | SemVer relajado o CalVer | Despliegue continuo o por tag | Blue-green o canary |
| web-microservices | SemVer por servicio | Container registry | GitOps + canary |
| desktop-app | SemVer + auto-update | Installer firmado, store | Channel: stable, beta |
| mobile-app-maui | SemVer | Stores (Google Play, App Store) | Internal → closed → open testing |
| rest-api | SemVer + URL versioning | Container registry | Canary + feature flags |
| cli-tool | SemVer | Binarios firmados, package managers (homebrew, scoop) | Stable + preview |
| worker-service | SemVer relajado | Container registry | Rolling update |

---

# §11 Prompting colaborativo con IA

## 11.1 El patrón nuclear — Plan-then-confirm

El patrón de trabajo con IA que SDD 2.1 adopta se llama **plan-then-confirm con subagentes especializados y audit independiente**. Sus tres elementos:

1. **Plan-then-confirm.** El agente IA primero declara qué va a hacer (lectura previa, archivos a crear, restricciones a respetar, criterios de éxito). El humano confirma antes de que el agente actúe. Esto evita acciones destructivas no intencionadas y obliga al humano a internalizar el plan.
2. **Subagentes especializados.** El trabajo se divide entre agentes con perfil profesional acotado (uno por especialidad AG-XX). Cada subagente conoce solo su scope y sus criterios. Esto evita que un solo agente cubra demasiado terreno y diluya criterios.
3. **Audit independiente.** Un agente revisor distinto de los productores verifica el resultado contra los criterios. Esto evita que el productor sea juez de su propia obra.

## 11.2 Separación orquestador / subagentes

| Rol | Función | Acceso |
|---|---|---|
| **Orquestador** | Coordina, despacha, consolida | Lee todo, escribe el plan |
| **Subagente productor** | Genera artefactos de su scope | Lee referencias, escribe en su carpeta |
| **Subagente auditor** | Revisa lo producido | Lee todo, no escribe (solo reporta) |
| **Humano** | Decide, aprueba, corrige | Acceso total |

El orquestador no escribe artefactos finales (eso es trabajo del subagente productor). El subagente productor no audita su propia obra (eso es trabajo del auditor). El humano no escribe los artefactos a mano (eso es trabajo del subagente). Cada rol tiene fronteras claras y se respetan.

## 11.3 Cuándo paralelizar y cuándo serializar

| Caso | Paralelizar | Serializar |
|---|---|---|
| Secciones independientes (AG-04 y AG-10) | Sí | — |
| Secciones con dependencia upstream (AG-01 → AG-02) | — | Sí |
| Refinamiento masivo de coherencia | Sí (12 auditores paralelos) | — |
| Generación inicial de toda la doc | — | Sí (la cadena 00 → 11) |
| Bug fixes correlacionados | — | Sí |
| Test cases que dependen de spec aprobada | — | Sí (después de AG-02) |

**Regla práctica.** Paralelizar lo que no depende; serializar lo que depende. La trazabilidad SDD ya marca las dependencias (ver §4.4); seguir esa cadena evita errores.

## 11.4 Patrones de prompt recurrentes

### 11.4.1 Lectura antes de acción

Todo prompt destinado a generar artefactos comienza con una lista explícita de archivos a leer en orden. Esto evita que el agente invente contexto.

```text
Antes de escribir nada, leé en orden:
1. devs/intake/SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md
2. devs/rules/decisiones-D1-D8.md
3. devs/guides/marco-teorico-sdd2.1_v1.0.md (sección §4 si el rol que asumis es AG-XX)

Respondeme solo con:
- Los puntos clave que entendiste del contexto
- Las restricciones que vas a respetar
- Tu plan de archivos a crear

No escribas archivos todavía. Esperá mi confirmación.
```

### 11.4.2 Uno a la vez con confirmación

Para generación de múltiples artefactos, se exige creación secuencial con confirmación humana entre cada uno:

```text
Crea estos archivos en orden, uno a la vez, esperando mi confirmación entre cada uno:

ARCHIVO 1 — docs/00_contexto/vision-producto_v1.0.md
ARCHIVO 2 — docs/00_contexto/alcance-proyecto_v1.0.md
ARCHIVO 3 — docs/00_contexto/roadmap-producto_v1.0.md

Al terminar cada archivo, mostrá su contenido para verificación.
```

### 11.4.3 Diagnóstico antes de plan

Antes de planificar un sprint o una nueva funcionalidad, se ejecuta un diagnóstico del estado actual:

```text
Antes de planificar el Sprint X, leé estos documentos:
[lista]

Respondeme solo con:
- Qué items del backlog están completados
- Qué items quedan pendientes ordenados por prioridad
- Tu recomendación de qué entra en el sprint y por qué

No planifiques todavía. Solo el diagnóstico.
```

### 11.4.4 Decisión de arquitectura embebida

Cuando hay decisiones de diseño que el agente debe respetar, se incluyen explícitamente en el prompt:

```text
DECISION 1 — La librería NO es responsable de obtener configuración desde APIs externas.
Eso es responsabilidad del cliente integrador.

DECISION 2 — Sin estado global mutable.
Toda función pura, todo objeto inmutable salvo justificación explícita en ADR.
```

### 11.4.5 Verificación final

Todo prompt termina con comandos de verificación:

```text
Al terminar, ejecutá:
- Linter del proyecto
- Tests automatizados
- Verificación de que no se modificaron archivos fuera del scope
```

## 11.5 Anti-patrones de prompting

| Anti-patrón | Problema | Mitigación |
|---|---|---|
| **Mega-prompt inicial** | El modelo prioriza arbitrariamente lo que más le interesa | Prompts en capas: contexto → scaffolding → uno a uno |
| **Aprobación sin revisión** | El humano dice "sí, seguí" sin verificar | Pedir confirmación con `Get-Content` o equivalente |
| **Agente sin lectura previa** | El agente inventa contexto | Toda instrucción empieza con "leé primero estos archivos" |
| **Sesión demasiado larga** | El agente compacta la conversación y pierde contexto | Máximo 2-3 unidades de trabajo por sesión |
| **Sin verificación final** | No se confirma que el resultado compila o pasa tests | Todo prompt termina con comandos de verificación |
| **Productor que se autoaudita** | Sesgo de confirmación | Auditor distinto del productor |
| **Mezcla de orquestador y productor** | El mismo agente decide y ejecuta | Separación de roles |

## 11.6 Fundamentación en literatura

El patrón plan-then-confirm tiene fundamentos en la literatura de prompt engineering 2023–2025:

- **Chain-of-Thought prompting** (Wei et al., 2022): obligar al modelo a explicitar su razonamiento mejora la calidad de la respuesta.
- **ReAct** (Yao et al., 2022): alternar razonamiento (Reason) y acción (Act) reduce alucinaciones.
- **Self-Refine** (Madaan et al., 2023): el modelo mejora cuando audita su propia obra.
- **Multi-agent collaboration** (Wu et al., 2023; Anthropic, 2024): agentes especializados colaborando producen mejores resultados que un solo agente generalista.
- **Constitutional AI** (Bai et al., 2022): definir restricciones y criterios de aceptación explícitamente mejora alineamiento.

La metodología SDD 2.1 toma elementos de estos enfoques y los compone en un patrón aplicable a la generación y mantenimiento de documentación técnica.

## 11.7 Cadena de trazabilidad spec-prompt-código

El círculo virtuoso de SDD asistido por IA cierra cuando la cadena spec-prompt-código se mantiene en ambas direcciones:

```text
Spec actualizada
    -> prompt que respeta la spec
        -> codigo que respeta el prompt
            -> tests que verifican el codigo contra la spec
                -> feedback al equipo si hay desviacion
                    -> Spec actualizada (vuelta al inicio)
```

Cada paso del ciclo tiene un dueño (especialidad AG-XX) y un artefacto verificable. Cuando un eslabón se rompe (la spec dice X pero el código hace Y), el problema es trazable a un punto concreto del ciclo, no a "el sistema en general".

---

# §12 Anti-patrones y errores frecuentes

Este capítulo consolida los errores típicos al adoptar SDD 2.1, agrupados por categoría. Cada anti-patrón se acompaña del problema concreto que genera y de una mitigación accionable.

## 12.1 Anti-patrones de proceso SDD

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Spec desactualizada** | Spec y código divergen, la spec deja de ser fuente de verdad | Política "spec primero, código después"; quality gate que detecte código sin spec correspondiente |
| **Especificación barroca** | Docs extensos que nadie lee | Plantillas mínimas; eliminar secciones no aplicables; revisión periódica de utilidad |
| **Falta de trazabilidad** | Imposible saber qué código justifica qué necesidad | Tablas obligatorias de trazabilidad CU→NB, US→CU, BT→US, Test→CU |
| **ADR ausentes** | Decisiones críticas pierden su justificación con el tiempo | ADR como artefacto de primera clase, un archivo por decisión |
| **Versionado inconsistente de docs** | `_v1.0` vs `.v1.0` vs sin sufijo | Convención única (`_v1.0`) enforzada por linter |
| **Coexistencia de versiones múltiples sin marcar deprecación** | v1.0 y v2.0 en paralelo confunde al lector | Solo la versión vigente queda en el árbol; el resto archivado en `_legacy/` |

## 12.2 Anti-patrones de Scrum aplicado a SDD

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Todo es Must Have** | No hay priorización real | Forzar que Must Have sea el mínimo absoluto del MVP |
| **Daily como status report** | Pierde valor de sincronización | Reformular el Daily como herramienta del equipo, no del manager |
| **Planning sin DoR** | Se compromete trabajo inmaduro | DoR explícito y enforzado |
| **Retro decorativa** | Acciones identificadas no se ejecutan | Acciones de la Retro se cargan como items del próximo sprint |
| **Refinement saltado** | Planning se convierte en mini-refinement apurado | Refinement continuo a lo largo del sprint, no concentrado al final |
| **Sprint Goal ausente** | El sprint no tiene foco | Sprint Goal de una frase, orientado a valor, acordado por el equipo |
| **Burndown decorativo** | Métrica que no informa decisiones | Burndown comparado con la línea ideal; si hay desvío, decidir |

## 12.3 Anti-patrones de prompting con IA

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Mega-prompt inicial** | Modelo prioriza arbitrariamente | Prompts en capas |
| **Sin lectura previa** | Modelo inventa contexto | "Leé primero estos archivos" obligatorio |
| **Aprobación sin revisión** | Trabajo destructivo no intencionado | Confirmación explícita con verificación |
| **Sesión demasiado larga** | Pérdida de contexto por compactación | Máximo 2-3 unidades de trabajo por sesión |
| **Sin verificación final** | No se valida que compila o pasa tests | Cada prompt termina con comandos de verificación |
| **Productor que se autoaudita** | Sesgo de confirmación | Auditor independiente |
| **Mezcla orquestador / productor** | Decisión y ejecución sin separación | Roles distintos, separados por prompt |

## 12.4 Anti-patrones arquitectónicos

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Microservicios prematuros** | Complejidad operativa sin escala humana que la justifique | Empezar con monolito modular; extraer servicios cuando duela |
| **Base de datos compartida en microservicios** | Acoplamiento implícito vía DB | Base de datos por servicio; comunicación vía API o eventos |
| **God class / God service** | Servicio que sabe demasiado | Refactor por bounded context |
| **Acoplamiento por compilación cruzado** | Cambios obligan a recompilar todo | Contratos explícitos, abstracciones, plugins |
| **Falta de health checks** | Detección tardía de servicios caídos | Health endpoint obligatorio por servicio |
| **Stateful workers** | Worker que mantiene estado en memoria entre mensajes | Idempotencia + estado externalizado |

## 12.5 Anti-patrones de calidad y testing

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Pirámide invertida** | Muchos E2E, pocos unit | Reescribir tests pesados en niveles más bajos |
| **Cobertura como métrica única** | 80% de cobertura sin validar comportamiento | Mutation testing valida la calidad de la suite |
| **Tests acoplados a la implementación** | Cualquier refactor rompe tests | Testear comportamiento, no estructura interna |
| **Tests sin determinismo** | Flaky tests que pasan o fallan al azar | Aislar dependencias, mocks deterministas, semillas fijas |
| **Skip persistente** | Tests "temporariamente" deshabilitados que se vuelven permanentes | Política: skip caduca en X sprints; si no, se borra el test |
| **DoD subjetivo** | "Está terminado cuando se siente terminado" | DoD explícito, checklist verificable |

## 12.6 Anti-patrones de DevOps

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Despliegue manual** | Inconsistencia, errores humanos | Pipeline CI/CD automatizado end-to-end |
| **Secretos en repo** | Credenciales expuestas | Secret manager (Vault, Key Vault, GitHub Secrets) |
| **Versionado a mano** | Errores en el bump SemVer | Conventional Commits + herramienta automática |
| **Sin rollback** | Deploy fallido bloquea producción | Estrategia de rollback documentada y probada |
| **Sin observabilidad** | Problemas en producción invisibles | Logs estructurados + métricas + trazas |
| **Pipelines sin gates** | Código roto llega a producción | Quality gates obligatorios pre-merge |
| **Dependencias sin escaneo** | CVEs no detectados | SCA en el pipeline; bloquear release ante CVE crítico |

## 12.7 Anti-patrones de documentación

| Anti-patrón | Problema | Solución |
|---|---|---|
| **Documentación monolítica** | Un solo archivo gigante que nadie lee entero | Diátaxis: tutorial, how-to, reference, explanation |
| **Snippets que no compilan** | Lector copia, falla, pierde confianza | Snippets extraídos de tests reales, validados en CI |
| **Sin troubleshooting** | Errores comunes sin solución documentada | Sección "errores frecuentes" en developer guide |
| **Cross-links rotos** | Lector hace clic, llega a 404 | Linter de markdown que valide links |
| **Mezcla de fidelidades** | Spec funcional con detalles de implementación específicos | Separación clara: qué (funcional) versus cómo (técnico) |
| **Sin glosario** | Términos canónicos usados sin definir | Glosario obligatorio en cada README de sección |

---

# §13 Glosario

Términos canónicos del template SDD 2.1. Cada uno con definición operativa en una a dos líneas.

| Término | Definición |
|---|---|
| **ADR (Architecture Decision Record)** | Documento corto que registra una decisión arquitectónica con su contexto, alternativas evaluadas y consecuencias. Un archivo por decisión. |
| **AG-XX** | Identificador de especialidad del template (AG-ROOT, AG-00 a AG-11). Cada AG-XX es responsable de una sección de `/docs/`. |
| **ATDD** | Acceptance Test-Driven Development. Los criterios de aceptación se escriben primero como tests ejecutables. |
| **BDD** | Behavior-Driven Development. Escenarios Given/When/Then ejecutables que validan el comportamiento del sistema. |
| **BFF (Backend for Frontend)** | Capa backend intermedia diseñada para las necesidades específicas de un tipo de frontend. |
| **BRIEF** | Documento corto de 1 a 2 páginas que captura la esencia del proyecto antes de cargar las plantillas de intake. |
| **BT-XX** | Identificador de Backlog Técnico. Tarea técnica que descompone una US o que cubre trabajo de infraestructura. |
| **Clean Architecture** | Estilo arquitectónico que organiza el sistema en capas concéntricas donde las dependencias apuntan hacia el dominio. |
| **CQRS** | Command Query Responsibility Segregation. Separación de operaciones de lectura (queries) y escritura (commands). |
| **CU-XX** | Identificador de Caso de Uso. Especifica una intención del usuario o del sistema con flujo principal, alternativos y excepciones. |
| **DoD (Definition of Done)** | Conjunto de criterios verificables que una historia debe cumplir para considerarse terminada. |
| **DoR (Definition of Ready)** | Conjunto de criterios verificables que una historia debe cumplir para entrar a un sprint. |
| **DORA metrics** | Cuatro métricas de rendimiento DevOps: lead time, deployment frequency, change failure rate, failed deployment recovery time. |
| **DX (Developer Experience)** | Experiencia de uso por parte de un desarrollador integrador: API, SDK, mensajes de error, documentación. |
| **Épica** | Capacidad funcional completa del sistema demasiado grande para un sprint, descompuesta en múltiples User Stories. |
| **Event-driven** | Estilo arquitectónico donde la comunicación entre componentes ocurre principalmente vía eventos asíncronos. |
| **Feature flag** | Mecanismo para activar o desactivar funcionalidad en runtime sin redespliegue. |
| **GitHub Flow** | Modelo de branching: una rama main protegida, feature branches que mergean vía PR. |
| **GitOps** | Patrón donde el estado deseado de infraestructura y aplicaciones se declara en git y un agente reconcilia el estado real. |
| **Given/When/Then** | Formato BDD para criterios de aceptación: contexto inicial, evento, resultado esperado. |
| **Intake** | Documento inicial único que el humano completa para alimentar al template: `SOLUTION-INTAKE`, con tres partes (A negocio, B composición, C técnica por proyecto). Reemplaza a `PROJECT-BRIEF` y `PROJECT-README`, ahora deprecados. |
| **SOLUTION-INTAKE** | Documento único de entrada del template (`SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md`). Parte A negocio (§1-§12), Parte B composición (§13 tabla de proyectos tipados, §14 estilo, §15 descomposición, §16 estructura), Parte C técnica por proyecto (§17, bloque P.1-P.12), más §18 samples y §19 checklist. Su §13 es la fuente desde la que se deriva el SOLUTION-MANIFEST. |
| **ISO 25010** | Norma ISO que define ocho atributos de calidad de producto de software. |
| **ISO 29148** | Norma ISO de ingeniería de requisitos de software y sistemas. |
| **Manifest-driven** | Patrón donde cada componente expone un manifest declarativo que un sistema consumidor lee para configurar UI o comportamiento. |
| **MoSCoW** | Técnica de priorización: Must Have, Should Have, Could Have, Won't Have v1. |
| **Orden topológico** | Ordenamiento de los proyectos de una solución derivado del grafo acíclico de dependencias: primero las dependencias, después los dependientes. Define la secuencia de generación y de build. |
| **MVP** | Minimum Viable Product. Versión mínima del producto con la que se valida una hipótesis de valor. |
| **NB-XX** | Identificador de Necesidad de Negocio. Problema concreto que el producto resuelve, con criterios de éxito medibles. |
| **OpenAPI** | Estándar de especificación de APIs REST en formato YAML o JSON, parseable por herramientas. |
| **PKCE** | Proof Key for Code Exchange. Extensión de OAuth2 que protege el flujo Authorization Code contra interceptación. |
| **Proyecto** | Unidad de especialización del template. Lleva exactamente uno de los ocho valores D8. Las variantes por tipo D8 y las categorías 02 a 11 se aplican por proyecto. |
| **Pull Request (PR)** | Solicitud de merge de una rama a otra; mecanismo de revisión de código y aplicación de quality gates. |
| **RACI** | Matriz Responsible / Accountable / Consulted / Informed. Define quién hace qué para cada actividad. |
| **RCL (Razor Class Library)** | Proyecto .NET que empaqueta componentes Blazor para reutilización entre aplicaciones. |
| **Refactor** | Modificación de la estructura interna del código sin cambiar su comportamiento observable. |
| **RN-XX** | Identificador de Regla de Negocio. Restricción del dominio que el sistema debe respetar. |
| **SBOM** | Software Bill of Materials. Lista trazable de todos los componentes que conforman un artefacto de software. |
| **SDD (Specification-Driven Development)** | Enfoque que pone a la especificación como artefacto central del ciclo de desarrollo. |
| **SemVer** | Semantic Versioning 2.0.0. Estándar de versionado MAJOR.MINOR.PATCH para librerías y APIs. |
| **SOLUTION-MANIFEST** | Fuente única de verdad de una solución. Enumera sus proyectos y, por cada uno, su tipo D8, rol, dependencias y nombres de código. Su grafo de dependencias es acíclico (DAG). Es un artefacto derivado: el orquestador lo construye desde el §13 del intake en la Fase de validación de intake y lo confirma con el humano; no se completa a mano. |
| **Solución** | Agrupación de una jerarquía de N proyectos (N≥1). No tiene un D8 propio: su tipo es compuesto. El negocio (categorías 00 y 01) se especifica a nivel solución. |
| **Sprint** | Iteración de tiempo fijo (típicamente 1 a 4 semanas) donde el equipo entrega un incremento del producto. |
| **Sprint 0** | Iteración previa al primer sprint productivo, dedicada a producir los artefactos sin los cuales el Sprint 1 no puede arrancar. |
| **SLSA** | Supply-chain Levels for Software Artifacts. Marco de niveles L1..L4 de madurez de cadena de suministro. |
| **SLI / SLO / SLA** | Service Level Indicator, Objective, Agreement. Métricas y compromisos de nivel de servicio. |
| **Spec-Anchored** | Nivel 2 de SDD: spec y código se mantienen vivos en paralelo; los cambios a la spec preceden a los del código. |
| **Spec-as-Source** | Nivel 3 de SDD: la spec es el artefacto principal y el código se regenera bajo demanda. |
| **Spec-First** | Nivel 1 de SDD: la spec guía la implementación inicial y luego se descarta. |
| **Story point** | Unidad relativa de estimación de esfuerzo. No representa horas; representa complejidad / esfuerzo relativo. |
| **Subagente** | Agente IA especializado en un scope acotado, despachado por un agente orquestador. |
| **TDD** | Test-Driven Development. Escribir el test antes que el código que lo hace pasar. |
| **Trazabilidad vertical** | Cadena que conecta visión → necesidad → caso de uso → arquitectura → US → BT → test. |
| **US-XX** | Identificador de User Story. Funcionalidad descrita desde la perspectiva del usuario en formato Como/Quiero/Para. |
| **UX** | User Experience. Percepción y respuesta del usuario al uso del sistema. |
| **Vertical slicing** | Técnica de descomposición que corta features verticalmente atravesando todas las capas. |
| **Walking skeleton** | Esqueleto mínimo del sistema que atraviesa todas las capas y se despliega lo antes posible. |
| **WCAG 2.2** | Web Content Accessibility Guidelines 2.2. Estándar W3C de accesibilidad web. |

---

# §14 Bibliografía y referencias

Las referencias siguen la norma APA 7ª edición. Se priorizan fuentes primarias (libros canónicos, normas, papers); las URLs aparecen solo cuando la fuente es nativamente web (especificaciones, guías oficiales online).

## 14.1 Libros canónicos sobre desarrollo ágil y prácticas de software

Beck, K. (2002). *Test-driven development: By example*. Addison-Wesley.

Cohn, M. (2004). *User stories applied: For agile software development*. Addison-Wesley.

Cohn, M. (2009). *Succeeding with agile: Software development using Scrum*. Addison-Wesley.

Evans, E. (2003). *Domain-driven design: Tackling complexity in the heart of software*. Addison-Wesley.

Forsgren, N., Humble, J., & Kim, G. (2018). *Accelerate: The science of lean software and DevOps*. IT Revolution Press.

Humble, J., & Farley, D. (2010). *Continuous delivery: Reliable software releases through build, test, and deployment automation*. Addison-Wesley.

Kim, G., Humble, J., Debois, P., Willis, J., & Forsgren, N. (2021). *The DevOps handbook* (2ª ed.). IT Revolution Press.

Martin, R. C. (2017). *Clean architecture: A craftsman's guide to software structure and design*. Prentice Hall.

Newman, S. (2021). *Building microservices: Designing fine-grained systems* (2ª ed.). O'Reilly Media.

Patton, J. (2014). *User story mapping: Discover the whole story, build the right product*. O'Reilly Media.

Schwaber, K., & Sutherland, J. (2020). *The Scrum Guide*. Scrum.org. https://scrumguides.org/

## 14.2 Libros canónicos sobre UX/UI/DX

Cooper, A., Reimann, R., Cronin, D., & Noessel, C. (2014). *About face: The essentials of interaction design* (4ª ed.). Wiley.

Garrett, J. J. (2011). *The elements of user experience: User-centered design for the web and beyond* (2ª ed.). New Riders.

Gothelf, J., & Seiden, J. (2021). *Lean UX: Designing great products with agile teams* (3ª ed.). O'Reilly Media.

Krug, S. (2014). *Don't make me think, revisited: A common sense approach to web usability* (3ª ed.). New Riders.

Nielsen, J. (1993). *Usability engineering*. Academic Press.

Norman, D. A. (2013). *The design of everyday things* (Edición revisada y expandida). Basic Books.

Rosenfeld, L., Morville, P., & Arango, J. (2015). *Information architecture: For the web and beyond* (4ª ed.). O'Reilly Media.

Shneiderman, B., Plaisant, C., Cohen, M., Jacobs, S., Elmqvist, N., & Diakopoulos, N. (2017). *Designing the user interface: Strategies for effective human-computer interaction* (6ª ed.). Pearson.

## 14.3 Normas y especificaciones

International Organization for Standardization. (2011, actualizada 2023). *ISO/IEC 25010:2011 — Systems and software engineering — Systems and software quality requirements and evaluation (SQuaRE) — System and software quality models*. ISO.

International Organization for Standardization. (2018). *ISO 9241-11:2018 — Ergonomics of human-system interaction — Part 11: Usability: Definitions and concepts*. ISO.

International Organization for Standardization. (2019). *ISO 9241-210:2019 — Ergonomics of human-system interaction — Part 210: Human-centred design for interactive systems*. ISO.

International Organization for Standardization. (2018). *ISO/IEC/IEEE 29148:2018 — Systems and software engineering — Life cycle processes — Requirements engineering*. ISO.

Preston-Werner, T. (2013). *Semantic Versioning 2.0.0*. https://semver.org/

Conventional Commits. (2021). *Conventional Commits 1.0.0*. https://www.conventionalcommits.org/

W3C. (2023). *Web Content Accessibility Guidelines (WCAG) 2.2*. World Wide Web Consortium. https://www.w3.org/TR/WCAG22/

National Institute of Standards and Technology. (2022). *NIST SP 800-218: Secure Software Development Framework (SSDF) Version 1.1*. NIST. https://csrc.nist.gov/publications/detail/sp/800-218/final

Open Source Security Foundation. (2023). *SLSA — Supply-chain Levels for Software Artifacts (v1.0)*. https://slsa.dev/

CycloneDX. (2024). *CycloneDX 1.6 Specification*. OWASP. https://cyclonedx.org/

Internet Engineering Task Force. (2020). *RFC 7807 — Problem Details for HTTP APIs*. https://datatracker.ietf.org/doc/html/rfc7807

## 14.4 Papers y artículos académicos

Bai, Y., Kadavath, S., Kundu, S., Askell, A., Kernion, J., Jones, A., Chen, A., Goldie, A., Mirhoseini, A., McKinnon, C., Chen, C., Olsson, C., Olah, C., Hernandez, D., Drain, D., Ganguli, D., Li, D., Tran-Johnson, E., Perez, E., … Kaplan, J. (2022). *Constitutional AI: Harmlessness from AI feedback*. arXiv. https://arxiv.org/abs/2212.08073

Card, S. K., Moran, T. P., & Newell, A. (1983). *The psychology of human-computer interaction*. Lawrence Erlbaum Associates.

Fagerholm, F., & Münch, J. (2012). Developer experience: Concept and definition. En *2012 International Conference on Software and System Process (ICSSP)* (pp. 73–77). IEEE.

Hewett, T. T., Baecker, R., Card, S., Carey, T., Gasen, J., Mantei, M., Perlman, G., Strong, G., & Verplank, W. (1992). *ACM SIGCHI curricula for human-computer interaction*. Association for Computing Machinery.

Madaan, A., Tandon, N., Gupta, P., Hallinan, S., Gao, L., Wiegreffe, S., Alon, U., Dziri, N., Prabhumoye, S., Yang, Y., Welleck, S., Majumder, B. P., Gupta, S., Yazdanbakhsh, A., & Clark, P. (2023). *Self-Refine: Iterative refinement with self-feedback*. arXiv. https://arxiv.org/abs/2303.17651

Nielsen, J. (1994). Heuristic evaluation. En J. Nielsen & R. L. Mack (Eds.), *Usability inspection methods* (pp. 25–62). Wiley.

Royce, W. W. (1970). Managing the development of large software systems. En *Proceedings of IEEE WESCON* (pp. 1–9).

Wei, J., Wang, X., Schuurmans, D., Bosma, M., Ichter, B., Xia, F., Chi, E. H., Le, Q. V., & Zhou, D. (2022). *Chain-of-thought prompting elicits reasoning in large language models*. arXiv. https://arxiv.org/abs/2201.11903

Westrum, R. (2004). A typology of organisational cultures. *Quality and Safety in Health Care, 13*(Suppl 2), ii22–ii27.

Wu, Q., Bansal, G., Zhang, J., Wu, Y., Li, B., Zhu, E., Jiang, L., Zhang, X., Zhang, S., Liu, J., Awadallah, A. H., White, R. W., Burger, D., & Wang, C. (2023). *AutoGen: Enabling next-gen LLM applications via multi-agent conversation framework*. arXiv. https://arxiv.org/abs/2308.08155

Yao, S., Zhao, J., Yu, D., Du, N., Shafran, I., Narasimhan, K., & Cao, Y. (2022). *ReAct: Synergizing reasoning and acting in language models*. arXiv. https://arxiv.org/abs/2210.03629

## 14.5 Recursos online y guías de la industria

Agile Alliance. (2001). *Manifesto for agile software development*. https://agilemanifesto.org/

Anthropic. (2024). *Claude — Building with the Anthropic API*. https://docs.anthropic.com/

Design Council. (2019). *Framework for innovation: Design Council's evolved Double Diamond*. https://www.designcouncil.org.uk/

DORA. (2023). *State of DevOps Report 2023*. Google Cloud. https://dora.dev/

OpenTelemetry Authors. (2024). *OpenTelemetry — Cloud Native Computing Foundation*. https://opentelemetry.io/

Panaversity Agent Factory. (2024). *Three levels of Spec-Driven Development*. https://agentfactory.panaversity.org/docs/General-Agents-Foundations/spec-driven-development/three-levels-of-sdd

Procida, D. (2022). *Diátaxis: A systematic approach to technical documentation authoring*. https://diataxis.fr/

Stanford d.school. (2010). *An introduction to design thinking process guide*. Stanford University Hasso Plattner Institute of Design.

Stickdorn, M., Hormess, M. E., Lawrence, A., & Schneider, J. (2018). *This is service design doing: Applying service design thinking in the real world*. O'Reilly Media.

W3C. (2024). *ARIA — Accessible Rich Internet Applications*. https://www.w3.org/TR/wai-aria/

---

## Control de Cambios

| Versión | Fecha | Descripción |
|---|---|---|
| 1.0 | 2026-05-17 | Versión inicial — 14 capítulos, 50+ términos en glosario, 40+ referencias APA 7, 8 tipos D8 cubiertos. |
| 1.1 | 2026-06-10 | Actualización al modelo de solución más jerarquía de proyectos: fundamentación del modelo (solución, proyecto como unidad de especialización, manifiesto como fuente única de verdad, orden topológico, caso degenerado como garantía de no ruptura), estructura del template con niveles solución/proyecto y _solucion/, y aclaración de que las variantes D8 y la trazabilidad se aplican por proyecto. Conjunto D8 sin cambios (8 valores). |
| 1.2 | 2026-06-10 | Actualización al intake unificado: un único documento `SOLUTION-INTAKE` reemplaza a `PROJECT-BRIEF` y `PROJECT-README`; el `SOLUTION-MANIFEST` pasa a artefacto derivado del §13 del intake; se incorpora la fundamentación de la Fase de validación de intake (validación de completitud semántica, batería de preguntas, derivación y confirmación del manifiesto). Conjunto D8 sin cambios. |
| 1.3 | 2026-06-10 | Higiene (resolución de P3): el archivo se renombra a `marco-teorico-sdd2.1_v1.0.md` para alinear el marcador de variante con `guia-usuario-sdd2.1` (se actualizan metadato y auto-referencias). Se agrega en §3.9 la aclaración del alcance de D3: el casing kebab-lowercase gobierna los artefactos generados; los identificadores de variante de la metodología (`SDD1.0`, `SDD2.1D`, `SDD2.1M`, `SDD2.1R`) y los prefijos de organización quedan fuera de su alcance. | Reformulación SDD 2.1D |

---

**Fin del documento**
