# SOLUTION-INTAKE-template

Plantilla metodológica SDD 2.2 para producir el documento `SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md`: el único documento de intake de una solución. Reúne el negocio (lo que el cliente quiere), la composición (qué proyectos componen la solución y cómo dependen entre sí) y la técnica de construcción (decisiones por proyecto). El orquestador lo lee, valida su completitud en la fase de validación de intake, deriva de él el `SOLUTION-MANIFEST` canónico (con tu confirmación) y recién entonces despacha la generación.

Reemplaza a las dos plantillas anteriores (`PROJECT-BRIEF-template.md` y `PROJECT-README-template.md`), que quedan deprecadas. El usuario completa un solo documento.

Estructura en tres partes:

- Parte A — Negocio de la solución (§1 a §12). El qué y el porqué, en lenguaje del cliente. Es de nivel solución: el negocio es uno.
- Parte B — Composición de la solución (§13 a §16). La jerarquía de proyectos tipados, el estilo de solución, la descomposición y la estructura de repositorio. El §13 es la fuente de la que el orquestador deriva el manifiesto.
- Parte C — Técnica por proyecto (§17 a §18). Las decisiones de construcción, en un bloque repetible por cada proyecto declarado en §13.

## Guía de uso de esta plantilla

1. Copiar este archivo como `SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md` en `/SDD2.2D/devs/intake/`.
2. Completar la cabecera y recorrer §1 a §19 en orden. Las preguntas guía marcadas con `(*)` son bloqueantes: el orquestador no avanza sin ellas.
3. Replicar el bloque técnico de §17 una vez por cada proyecto declarado en §13.
4. Borrar los bloques `Ejemplo` y `Lo que NO va en esta sección` una vez completado cada apartado.
5. Validar el §19 (checklist) antes de pasar el intake al orquestador.
6. Versionar siempre como `_v1.0` en la primera emisión.

Para una solución de un solo proyecto (caso degenerado), §13 tiene una sola fila y §17 se replica una vez: el orquestador aplana el layout y reproduce el comportamiento del template de tipo único.

---

## Cabecera del documento

Bloque obligatorio al inicio. Reproducir y completar:

| Campo | Valor |
|---|---|
| Nombre de la solución | [Nombre legible de la solución] |
| Cliente / Stakeholder principal | [Persona, área u organización que la impulsa] |
| Repositorio | [Repositorio URL] |
| Lead técnico | [Nombre y rol] |
| Documento | `SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md` |
| Versión | 1.0 |
| Fecha | [YYYY-MM-DD] |
| Stack principal | [Lenguaje + framework principal de la solución] |
| Estado | Borrador / En revisión / Aprobado |

> Este documento captura qué quiere el cliente, cómo se compone la solución y cómo se construye cada proyecto.
> El orquestador deriva de §13 el `SOLUTION-MANIFEST` canónico; no completes el manifiesto a mano.

---

# Parte A — Negocio de la solución

## §1 Idea y problema

Instrucción: Describir en lenguaje del cliente qué dolor concreto motiva la solución y por qué ahora. Dos a cuatro párrafos: qué pasa hoy, a quién le duele, y la consecuencia de no resolverlo.

Preguntas guía:
- (*) ¿Qué problema concreto resuelve la solución y a quién le pasa hoy?
- (*) ¿Qué pasa si NO se construye en los próximos meses?
- ¿Por qué ahora? ¿Hay un disparador externo (regulación, competencia, crecimiento)?

Ejemplo (sistema de turnos médicos): Hoy los pacientes solo sacan turno por teléfono en horario de atención, lo que satura la línea y genera esperas. Si no se resuelve, se pierden pacientes contra clínicas con reserva online.

Lo que NO va en esta sección:
- Stack, frameworks, decisiones de arquitectura (van a la Parte C).
- Estimaciones de esfuerzo o sprint plan.

---

## §2 Audiencia y stakeholders

Instrucción: Identificar a quién usa la solución día a día, quién paga y quién decide su rumbo. Completar la tabla con al menos un representante por categoría (propietario, implementador, beneficiario). Sin genéricos como "los usuarios".

Preguntas guía:
- (*) ¿Quién es el propietario del problema y aprueba el intake?
- (*) ¿Quiénes son los usuarios finales y qué rol cumple cada uno?
- ¿Quién financia y a quién hay que rendir resultados? ¿Hay actores indirectos (auditoría, legal, soporte)?

Ejemplo:

| Rol | Nombre o cargo | Categoría | Responsabilidad principal |
|---|---|---|---|
| Dueño del problema | [Nombre] | Propietario | Aprueba el intake |
| Equipo de desarrollo | [Estudio/equipo] | Implementador | Construye y mantiene |
| Usuarios finales | [Rol] | Beneficiario | Operan la solución |

Lo que NO va en esta sección:
- Roles técnicos internos del equipo (eso es Parte C).
- Permisos o perfiles de seguridad (eso es especificación funcional / §17 P.5).

---

## §3 Propuesta de valor y diferenciación

Instrucción: Explicar qué hace la solución mejor o distinto que la alternativa actual (sistema viejo, planilla manual, competencia o no hacer nada). Una o dos frases por diferenciador, máximo cinco.

Preguntas guía:
- (*) ¿Qué hace hoy el cliente y por qué no le alcanza?
- (*) ¿Cuál es la promesa central que justifica el proyecto?
- ¿Cuál es el diferenciador defendible si la competencia copia la funcionalidad?

Ejemplo (CLI de migración): Hoy el equipo migra con scripts SQL ad-hoc. La promesa es ejecutar migraciones declarativas con un comando, con reporte de filas migradas y fallidas y reintento de lo fallido.

Lo que NO va en esta sección:
- Listas exhaustivas de funcionalidades (eso va a §4).
- Lenguaje de marketing vacío.

---

## §4 Alcance funcional pretendido (MoSCoW)

Instrucción: Listar las capacidades funcionales, sin implementación, cada una con etiqueta MoSCoW provisoria (Must / Should / Could / Won't Have v1). Si todo es Must, no hay priorización.

Preguntas guía:
- (*) ¿Cuál es el conjunto mínimo de capacidades sin el cual la solución no resuelve el problema?
- (*) ¿Qué capacidades son importantes pero pueden esperar?
- ¿Hay capacidades para una versión futura, no la inicial?

Ejemplo:

| ID | Capacidad | MoSCoW |
|---|---|---|
| F-01 | [Capacidad mínima] | Must Have |
| F-02 | [Capacidad importante] | Should Have |
| F-03 | [Capacidad deseable] | Could Have |
| F-04 | [Fuera de v1] | Won't Have v1 |

Lo que NO va en esta sección:
- Algoritmos, estructuras de datos, firmas de métodos.
- Detalles de UI concretos.

---

## §5 Historias de usuario / experiencias deseadas

Instrucción: Describir la solución desde quien la usa, en formato `Como [rol], quiero [acción], para [valor]`. Mínimo tres, cubriendo al menos dos roles si hay más de un actor.

Preguntas guía:
- (*) ¿Quiénes operan la solución y qué quiere lograr cada uno?
- (*) ¿Cuáles son las tres acciones más frecuentes de un usuario típico?
- ¿Hay acciones raras pero críticas (cierre de mes, baja de cuenta)?

Ejemplo: Como encargado de depósito, quiero escanear un código de barras con la cámara, para registrar la entrada sin tipear.

Lo que NO va en esta sección:
- Criterios de aceptación detallados (van al refinamiento de cada US).
- Wireframes o restricciones técnicas.

---

## §6 Flujos típicos

Instrucción: Narrar dos o tres recorridos representativos del uso normal, en lenguaje coloquial, cuatro a ocho pasos cada uno. Sin diagramas formales.

Preguntas guía:
- (*) ¿Cuál es el flujo más frecuente, el del 80 % del tiempo?
- ¿Hay un flujo crítico que rara vez pasa pero no puede fallar? ¿Cuál es el onboarding de un usuario nuevo?

Lo que NO va en esta sección:
- Diagramas UML/BPMN, endpoints, payloads.
- Implementación interna (colas, retries).

---

## §7 Casos límite y "qué pasa si"

Instrucción: Listar las preguntas incómodas que el cliente debe responder antes de construir. Mínimo cinco, cada una con espacio para la respuesta del cliente.

Preguntas guía:
- (*) ¿Qué pasa si dos personas hacen la misma operación a la vez?
- (*) ¿Qué pasa si se pierde la conexión en medio de una operación?
- ¿Qué pasa si un dato obligatorio llega vacío o mal formado? ¿Si alguien quiere borrar su cuenta o sus datos?

Lo que NO va en esta sección:
- Soluciones técnicas (retries, locks, idempotencia): acá va el "qué", no el "cómo".

---

## §8 Métricas de éxito desde el negocio

Instrucción: Establecer métricas SMART de negocio (específicas, medibles, alcanzables, relevantes, temporales). Mínimo tres, cada una con criterio, unidad, target y plazo. Son métricas de resultado de negocio, distintas de los NFR técnicos de §17 P.10.

Preguntas guía:
- (*) ¿Cómo sabemos, en seis meses, si fue exitoso?
- (*) ¿Qué número concreto le hace decir al cliente "valió la pena"?
- ¿Qué métricas de hoy vamos a poder comparar antes y después?

Ejemplo:

| Criterio | Métrica | Target | Plazo |
|---|---|---|---|
| Adopción del canal digital | % de operaciones online sobre el total | ≥ 40 % | 6 meses post-lanzamiento |

Lo que NO va en esta sección:
- Métricas técnicas (latencia p99, throughput, uptime): van a §17 P.10.
- Métricas de proceso de desarrollo (velocity, cobertura).

---

## §9 Lo que NO es esta solución (exclusiones)

Instrucción: Declarar explícitamente qué NO va a hacer la solución, con justificación. Mínimo tres exclusiones; cada una explica por qué queda afuera y, si aplica, cuándo podría incorporarse.

Preguntas guía:
- (*) ¿Qué se pidió y se decidió dejar afuera de esta versión?
- (*) ¿Qué supuestos del equipo serían incorrectos?
- ¿Qué integraciones quedan fuera de alcance aunque parezcan obvias?

Lo que NO va en esta sección:
- Capacidades que sí están dentro del alcance (van a §4).
- Exclusiones sin justificación.

---

## §10 Restricciones del cliente

Instrucción: Documentar restricciones externas: presupuesto orientativo, fecha objetivo, restricciones legales o regulatorias, integraciones obligatorias con sistemas existentes. No son negociables a nivel técnico. Distintas de los trade-offs técnicos autoimpuestos de §17 P.12.

Preguntas guía:
- (*) ¿Hay fecha objetivo concreta y qué la motiva?
- (*) ¿Cuál es el presupuesto orientativo o el rango?
- ¿Hay normativas que cumplir? ¿Sistemas con los que integrar sí o sí?

Lo que NO va en esta sección:
- Restricciones técnicas autoimpuestas por el equipo (van a §17 P.12).
- Decisiones de arquitectura derivadas (van a los ADR).

---

## §11 Riesgos detectados desde el negocio

Instrucción: Listar los riesgos del dominio que el cliente conoce o intuye, con probabilidad cualitativa, impacto y mitigación. Mínimo tres. Foco en riesgos de negocio, no en riesgos técnicos generales.

Preguntas guía:
- (*) ¿Qué le quita el sueño al cliente sobre esta solución?
- (*) ¿Hubo un intento previo y por qué falló?
- ¿Qué supuesto crítico, si se rompe, hace inviable el resultado?

Lo que NO va en esta sección:
- Riesgos técnicos puros (deuda técnica, refactor): van a §17 P.12.

---

## §12 Glosario del dominio del cliente

Instrucción: Definir los términos del dominio del cliente que el equipo técnico necesita aprender. Mínimo cinco, en una o dos frases, con sinónimos si los hay.

Preguntas guía:
- (*) ¿Qué palabras usa el cliente que el equipo no entendería de entrada?
- ¿Hay términos que parecen comunes pero significan algo distinto acá? ¿Siglas del rubro?

Lo que NO va en esta sección:
- Términos técnicos universales del software (API, base de datos, framework).

---

# Parte B — Composición de la solución

## §13 Proyectos de la solución

Instrucción: Enumerar los proyectos que componen la solución. Cada proyecto lleva exactamente uno de los 8 valores cerrados D8. De esta tabla el orquestador deriva el `SOLUTION-MANIFEST` canónico; revisala con cuidado. No se elige un único tipo para la solución: la solución es compuesta. Una solución de un proyecto es el caso degenerado (una sola fila).

Preguntas guía:
- (*) ¿Qué proyectos componen la solución y qué valor D8 lleva cada uno?
- (*) ¿Cuál es el proyecto principal (cabeza de la solución)?
- (*) ¿Qué dependencias hay entre proyectos? ¿El grafo es acíclico?
- ¿Algún proyecto se publica como paquete redistribuible independiente de la solución?

Valores cerrados D8, exactamente 8:

```text
library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service
```

Tabla de proyectos (fuente del manifiesto derivado):

| `nombre-proyecto-kebab` | `project_type` (D8) | Rol en la solución | Dependencias | `redistribuible` |
|---|---|---|---|---|
| [kebab] (principal) | [uno de los 8 D8] | [una frase] | [lista de kebab o vacío] | [true / false] |

Perfil de convención de nombres de código (el orquestador deriva los nombres `/src` con esta regla):

| Parámetro | Valor por defecto | Notas |
|---|---|---|
| Forma del nombre de solución en código | PascalCase | `NombreSolucionCodigo` derivado del nombre legible |
| Separador de segmentos | `.` | Separa la raíz de la solución del sufijo de rol |
| Prefijo de paquetes redistribuibles | `Aplicada` | Reemplaza la raíz de la solución cuando `redistribuible: true` |

Regla de nombres de código: cada proyecto se nombra `<NombreSolucionCodigo>.<Sufijo>` (por ejemplo `GestionDeTurnos.WebApi`, `GestionDeTurnos.Domain`); los redistribuibles arrancan con el prefijo de organización (`Aplicada.Validaciones`). La regla es agnóstica de stack a propósito.

Lo que NO va en esta sección:
- Decisiones técnicas internas de cada proyecto (van a §17).
- El árbol de carpetas completo (va a §16).

---

## §14 Estilo arquitectónico de la solución

Instrucción: Describir a alto nivel cómo se componen los proyectos entre sí: quién depende de quién, qué expone cada proyecto a sus dependientes y por qué la jerarquía es la elegida. El detalle interno de cada proyecto va a §17 P.2 y a `05_arquitectura_tecnica/`.

Preguntas guía:
- (*) ¿Cómo se relacionan los proyectos y qué contrato expone cada uno a sus dependientes?
- (*) ¿Por qué esta descomposición y no otra (un monolito, más microservicios)?
- ¿Qué proyecto es el punto de entrada para el consumidor final? ¿Hay proyectos compartidos (dominio, validaciones)?

Lo que NO va en esta sección:
- Detalle de capas internas de cada proyecto (va a §17 P.2 y a 05).
- Diagramas formales de despliegue (van a 05 y a la vista de solución).

---

## §15 Esquema de descomposición y delivery

Instrucción: Declarar la estrategia de descomposición del trabajo en el tiempo (vertical slicing, walking skeleton, thin slice) y justificarla. Criterio bloqueante: el primer sprint entrega valor demostrable end-to-end a través de la jerarquía.

Preguntas guía:
- (*) ¿El primer sprint entrega valor demostrable end-to-end a través de la jerarquía?
- (*) ¿La descomposición es vertical (rebanadas funcionales) u horizontal (capas/proyectos por sprint)?
- ¿En qué orden se construyen los proyectos según sus dependencias (orden topológico)?

Ejemplo (solución multi-proyecto): walking skeleton en Sprint 0 que atraviesa el paquete de validaciones, el dominio y la API para una operación mínima end-to-end; el worker y el resto de las capacidades se agregan en sprints posteriores manteniendo el camino end-to-end. El orden de construcción respeta el orden topológico de las dependencias de §13.

Lo que NO va en esta sección:
- Plan de sprint con US/BT (va a `07_plan-sprint/`).

---

## §16 Estructura de repositorio de la solución

Instrucción: Proponer el árbol `tree` con `/src`, `/tests`, `/samples`, `/docs`, `/devs`. Se deriva de la jerarquía de §13 y de la convención de nombres: cada proyecto es `src/<NombreProyectoCodigo>/`, salvo redistribuibles con prefijo de organización. Incluir la subsección §16.1 sobre `/samples`.

Preguntas guía:
- (*) ¿Cada proyecto de §13 tiene su carpeta en `/src` con su nombre de código?
- (*) ¿La estructura sigue las convenciones del ecosistema del lenguaje?
- ¿Los redistribuibles arrancan con el prefijo de organización?

Ejemplo (solución Gestión de Turnos):
```text
gestion-de-turnos/
├── src/
│   ├── GestionDeTurnos.WebApi/         # rest-api (principal)
│   ├── GestionDeTurnos.Domain/         # library de dominio
│   ├── GestionDeTurnos.Worker/         # worker-service
│   └── Aplicada.Validaciones/          # library redistribuible
├── tests/
├── samples/
├── docs/                               # categorías 00-11 SDD (por proyecto bajo proyectos/<kebab>/)
└── devs/intake/                        # SOLUTION-INTAKE
```

### §16.1 Materialización de `/samples`

Instrucción: Describir cómo se materializan los samples según el tipo D8 de cada proyecto que los produce. Cada sample autocontenido, ejecutable, con su nivel de complejidad.

| Tipo D8 | Qué hay en `/samples` |
|---|---|
| `library` | Consumidores progresivos vía package manager |
| `rest-api` | Cliente HTTP de referencia, colección de pruebas, SDK |
| `cli-tool` | Recetas de uso por OS |
| `worker-service` | Productor de prueba + compose con broker |
| (otros tipos) | Según la tabla de adaptabilidad del orquestador |

Lo que NO va en esta sección:
- Contenido detallado de los samples (va a `11_examples/`).

---

# Parte C — Técnica por proyecto

## §17 Bloque técnico por proyecto (plantilla repetible)

Instrucción: Para cada proyecto declarado en §13, copiar el bloque de identidad más las subsecciones P.1 a P.12 y completarlas. Si la solución tiene N proyectos, este bloque aparece N veces. Cada proyecto es autocontenido.

Identidad del proyecto (repetir por proyecto):

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | [kebab] |
| `nombre-proyecto-codigo` | [`<NombreSolucionCodigo>.<Sufijo>` o `Aplicada.<X>`] |
| `project_type` (D8) | [uno de los 8] |
| Rol | [una frase] |
| `redistribuible` | [true / false] |

### §17.P.1 Stack tecnológico
Instrucción: Lenguaje, versión, runtime, framework y plataformas target del proyecto, con las dependencias core justificadas.
Preguntas guía: (*) ¿Versión mínima del lenguaje y runtime? (*) ¿Dependencias core sin las que no compila?

### §17.P.2 Estilo arquitectónico del proyecto
Instrucción: Estilo interno (capas, hexagonal, pipeline, event-driven) justificado contra dos alternativas. Coherente con su `project_type` y con §14.
Preguntas guía: (*) ¿Qué estilo y por qué? (*) ¿Qué dos alternativas se descartaron?

### §17.P.3 Comunicación e integración
Instrucción: Protocolo, formato de payload, versión de contratos y política de breaking changes. Los contratos hacia otros proyectos deben ser coherentes con las dependencias de §13. Si no aplica, "No aplica" con justificación.
Preguntas guía: (*) ¿Protocolos sincrónicos y asincrónicos? (*) ¿Cómo se versionan los contratos? ¿Qué expone a sus dependientes?

### §17.P.4 Persistencia
Instrucción: Qué guarda, dónde, cómo se versiona el esquema, patrones de acceso, multi-tenant si aplica. Si no hay persistencia, "No aplica".
Preguntas guía: (*) ¿Motor y por qué? (*) ¿Cómo se versiona el esquema? ¿Multi-tenant?

### §17.P.5 Seguridad y autenticación
Instrucción: Autenticación, autorización y manejo de secretos del proyecto, en runtime y en CI/CD.
Preguntas guía: (*) ¿Mecanismo de autenticación y dónde reside el Identity Provider? (*) ¿Dónde viven los secretos? ¿Compliance?

### §17.P.6 Estrategia de testing
Instrucción: Pirámide con porcentajes, cobertura mínima numérica (gate del CI), frameworks por nivel, BDD/ATDD. La cobertura mínima es bloqueante y numérica.
Preguntas guía: (*) ¿Cobertura mínima de líneas y branches? (*) ¿Frameworks por nivel? ¿Tests de contrato hacia otros proyectos?

### §17.P.7 Estrategia de versionado y release
Instrucción: SemVer 2.0.0 y Conventional Commits; herramienta de cálculo de versión, branching, canales, feed.
Preguntas guía: (*) ¿SemVer y Conventional Commits sin excepciones? (*) ¿Qué herramienta calcula la versión? ¿Canales y dónde se publica?

### §17.P.8 Pipeline CI/CD
Instrucción: Stages (build, test, lint, SCA, SBOM, firma, publicación), matriz de SO/runtime, ambientes; cada stage con quality gate explícito. La cobertura mínima coincide con P.6.
Preguntas guía: (*) ¿Plataforma de CI? (*) ¿Quality gates bloqueantes para mergear? ¿Cómo se hace rollback?

### §17.P.9 Compatibilidad y plataformas target
Instrucción: SO, runtimes, navegadores, dispositivos y versiones mínimas, coherentes con el `project_type`. Toda combinación no listada se considera no soportada.
Preguntas guía: (*) ¿Plataformas target? (*) ¿Versión mínima de cada runtime/SO?

### §17.P.10 Requerimientos no funcionales (NFR)
Instrucción: Métricas numéricas de performance, escalabilidad, disponibilidad, observabilidad y compliance del proyecto. Cada métrica medible. NFR vagos no se aceptan. Distintos de las métricas de negocio de §8.
Preguntas guía: (*) ¿Latencia objetivo p99 y throughput mínimo? (*) ¿SLO de disponibilidad? ¿Qué se loguea, mide y traza?

### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)
Instrucción: Decisiones cerradas antes del Sprint 0 con justificación y alternativas. Se convierten en ADRs en `05_arquitectura_tecnica/`.
Preguntas guía: (*) ¿Decisiones cerradas y por qué? (*) ¿Alternativas evaluadas? ¿Qué queda abierto para Sprint 0?

### §17.P.12 Restricciones técnicas y trade-offs aceptados
Instrucción: A qué renuncia el proyecto conscientemente para ganar otra cosa. Distintos de las restricciones del cliente de §10.
Preguntas guía: (*) ¿Qué ganancias se priorizaron y a costa de qué? (*) ¿Restricciones del ecosistema? ¿Qué cargas no soporta?

Lo que NO va en el bloque técnico:
- Necesidades de negocio (van a la Parte A).
- ADRs formales (viven en `05_arquitectura_tecnica/`).

---

## §18 Estrategia de demo / samples

Instrucción: Detallar qué samples se construyen en `/samples`, qué proyecto de la solución ilustra cada uno, su nivel de complejidad y cómo se vincula a `/src`. Cada sample autocontenido y reproducible en cinco pasos o menos.

Preguntas guía:
- (*) ¿Cuántos samples y qué proyecto ilustra cada uno?
- (*) ¿Cómo se vincula cada sample con el código productivo?
- ¿Hay un sample que demuestre el punto de extensión principal?

Lo que NO va en esta sección:
- Tutoriales conceptuales (van a `10_developer_guide/`).

---

## §19 Checklist de completitud del intake

Verificar antes de pasar el intake al orquestador. Todos los ítems deben estar tildados.

Negocio (Parte A):
- [ ] La cabecera tiene nombre de solución, cliente, fecha y estado.
- [ ] §1 describe un problema concreto y qué pasa si no se construye.
- [ ] §2 tiene al menos un stakeholder por categoría con rol explícito.
- [ ] §4 tiene al menos un ítem en cada categoría MoSCoW y el Must Have es el mínimo razonable.
- [ ] §5 tiene al menos 3 historias en formato `Como/quiero/para`, cubriendo 2 roles si hay más de uno.
- [ ] §7 lista al menos 5 casos límite con espacio para respuesta del cliente.
- [ ] §8 tiene al menos 3 métricas SMART de negocio con target y plazo numéricos.
- [ ] §9 lista al menos 3 exclusiones con justificación.
- [ ] §10 declara presupuesto orientativo y fecha objetivo (o "sin fecha" justificado).
- [ ] §11 lista al menos 3 riesgos con probabilidad, impacto y mitigación.
- [ ] §12 define al menos 5 términos del dominio.

Composición (Parte B):
- [ ] §13 enumera todos los proyectos, cada uno con uno de los 8 valores D8, señala el principal, y el grafo de dependencias es acíclico.
- [ ] §13 declara el perfil de convención de nombres; no hay colisión de nombres de proyecto.
- [ ] §14 describe la composición y los contratos entre proyectos.
- [ ] §15 garantiza valor demostrable end-to-end en el primer sprint a través de la jerarquía.
- [ ] §16 publica el árbol `tree` derivado de la jerarquía y de la convención de nombres, con §16.1.

Técnica por proyecto (Parte C):
- [ ] §17 está completo para cada proyecto de §13 (identidad + P.1 a P.12).
- [ ] Cada proyecto: P.6 declara cobertura mínima numérica; P.7 adopta SemVer y Conventional Commits; P.8 enumera quality gates bloqueantes; P.9 declara plataformas y versiones mínimas; P.10 expresa NFR con métricas numéricas.

General:
- [ ] No hay vocabulario del dominio fuente del bootstrap ni stacks hardcodeados en el texto normativo (D7).
- [ ] El control de cambios refleja la versión y fecha del documento.

---

## Trazabilidad downstream

Este documento alimenta las siguientes secciones SDD 2.2. La parte de negocio (A) es de nivel solución; la composición (B) deriva el manifiesto; la técnica (C) se aplica por proyecto.

| Sección del intake | Destino | Documento downstream típico |
|---|---|---|
| §1 a §12 (negocio) | `00_contexto/`, `01_necesidades_negocio/` | visión, alcance, NB-XX |
| §13 (proyectos) | `SOLUTION-MANIFEST` derivado; todas las categorías por proyecto | manifiesto canónico; selector de variantes D8 |
| §14 estilo de solución | `05_arquitectura_tecnica/` (vista de solución) | `arquitectura-solucion_v1.0.md` |
| §16 estructura | `05_arquitectura_tecnica/`, `10_developer_guide/` | árbol, README de carpeta |
| §17 P.x (técnica por proyecto) | `05`, `08`, `09`, `00` (según P) por proyecto | ADRs, estrategia testing, pipeline, NFR |
| §18 samples | `11_examples/` | `ejemplo-XX_v1.0.md` |

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | [YYYY-MM-DD] | Intake unificado inicial de la solución | [Autor] |
