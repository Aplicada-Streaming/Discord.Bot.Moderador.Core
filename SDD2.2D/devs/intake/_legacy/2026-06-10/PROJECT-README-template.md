# PROJECT-README-template

Plantilla metodológica SDD 2.2 para producir el documento `PROJECT-README-<nombre-solucion-kebab>_v1.0.md` durante la fase de intake. Captura las decisiones técnicas de construcción de la solución descripta en `PROJECT-BRIEF`. Una solución agrupa una jerarquía de proyectos; cada proyecto lleva exactamente uno de los 8 valores D8 y se documenta en su propio bloque técnico dentro de este documento.

Este artefacto es de nivel solución: hay uno por solución, no uno por proyecto. La enumeración canónica de proyectos, su tipado, sus dependencias y sus nombres de código viven en `SOLUTION-MANIFEST-<nombre-solucion-kebab>_v1.0.md`. El §1 de este documento refleja esa enumeración, no la reabre; si difieren, manda el manifiesto. Este README alimenta downstream a `05_arquitectura_tecnica/`, `09_devops/` y `11_examples/`, y el `project_type` de cada proyecto gobierna las variantes de especialidad de las 12 categorías SDD para ese proyecto.

## Guía de uso de esta plantilla

1. Copiar este archivo como `PROJECT-README-<nombre-solucion-kebab>_v1.0.md` en `/SDD2.2D/devs/intake/`.
2. Completar el bloque de cabecera con los valores reales de la solución.
3. Recorrer §1 a §4 (nivel solución) y luego replicar el bloque técnico de §5 una vez por cada proyecto declarado en §1, contestando las preguntas guía marcadas con `(*)` (bloqueantes).
4. Borrar los bloques `Ejemplo` y `Lo que NO va en esta sección` una vez completado cada apartado.
5. Validar el §7 (checklist) antes de cambiar el estado a Aprobado.
6. Versionar el documento siempre como `_v1.0` en su primera emisión.

Para una solución de un solo proyecto, el §1 tiene una sola fila y el §5 se replica una sola vez: el documento equivale al PROJECT-README de tipo único del template anterior.

---

## Cabecera del documento

Bloque obligatorio al inicio del documento generado. Reproducir y completar:

| Campo | Valor |
|---|---|
| Nombre de solución | [Nombre legible de la solución] |
| Repositorio | [Repositorio URL] |
| Lead técnico | [Nombre y rol] |
| Manifiesto | `SOLUTION-MANIFEST-<nombre-solucion-kebab>_v1.0.md` |
| Documento | `PROJECT-README-<nombre-solucion-kebab>_v1.0.md` |
| Versión | 1.0 |
| Fecha | [YYYY-MM-DD] |
| Stack principal | [Lenguaje + framework principal de la solución] |
| Estado | Borrador / En revisión / Aprobado |

> Este documento captura cómo se va a construir la solución descrita en PROJECT-BRIEF.
> Contiene decisiones técnicas de stack, arquitectura, testing, CI/CD y samples, por proyecto.
> NO repite las necesidades del cliente: las asume conocidas vía PROJECT-BRIEF.

---

## §1 Proyectos de la solución

Instrucción: Enumerar los proyectos de la solución, reflejando la tabla del manifiesto. Cada proyecto declara exactamente uno de los 8 valores D8. No se elige un único tipo para la solución: la solución es compuesta y cada proyecto lleva su tipo. Una solución con un solo proyecto también es compuesta (caso degenerado): tiene una sola fila acá y se comporta como el template de tipo único. La enumeración canónica vive en el manifiesto; esta tabla la refleja y agrega la justificación técnica del tipo y del rol.

Preguntas guía:
- (*) ¿Qué proyectos componen la solución y qué valor D8 lleva cada uno?
- (*) ¿Cuál es el proyecto principal (cabeza de la solución)?
- (*) ¿Qué dependencias hay entre proyectos? ¿El grafo es acíclico?
- ¿Algún proyecto se publica como paquete redistribuible independiente de la solución?

Tabla de proyectos (mirror del manifiesto):

| `nombre-proyecto-kebab` | `project_type` (D8) | Rol en la solución | Dependencias | `redistribuible` |
|---|---|---|---|---|
| [kebab] | [uno de los 8 D8] | [una frase] | [lista o vacío] | [true / false] |

Valores cerrados D8, exactamente 8:

```text
library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service
```

Tabla de referencia de implicancias por tipo D8 (aplicar por proyecto):

| Tipo | Documentos obligatorios | Documentos recomendados | Omitibles | Carpetas `/src` típicas | Contenido de `/samples` | Stack-hint orientativo |
|---|---|---|---|---|---|---|
| `library` | 00, 01, 02, 05, 06, 08, 09, 10, 11 | 03 (DX), 07 | 04 | `<Sol>.Core/`, `<Sol>.Abstractions/` | Consumidor mínimo, intermedio y avanzado que usa la librería vía package manager | C#/.NET, Java, Rust, Python, TypeScript |
| `web-monolith` | 00, 01, 02, 03, 05, 06, 07, 08, 09, 13 (NFR) | 04 | 11 si es interno | `<Sol>.Web/`, `<Sol>.Domain/`, `<Sol>.Infrastructure/` | App de demostración del frontend con datos seed | ASP.NET Core, Django, Rails, Spring Boot, Laravel |
| `web-microservices` | 00, 01, 02, 03, 05, 06, 07, 08, 09, 13 | 04 | — | `<Sol>.Services.<Servicio>/`, `<Sol>.Gateway/`, `<Sol>.BuildingBlocks/` | Compose con N servicios más un cliente de prueba | .NET + YARP, Go + gRPC, Java + Spring Cloud, Node + NestJS |
| `desktop-app` | 00, 01, 02, 03, 05, 06, 08, 09, 12 | 11 si hay plugins | — | `<Sol>.Ui/`, `<Sol>.Core/` | Proyecto skin con tema custom o plugin de ejemplo | WPF/WinUI, Avalonia, Electron, Tauri, Qt |
| `mobile-app-maui` | 00, 01, 02, 03, 05, 06, 08, 09, 12 | 11 | — | `<Sol>.Mobile/`, `<Sol>.Shared/` | Proyecto demo multiplataforma con storyboard de pantallas | Flutter, React Native, .NET MAUI, Xamarin legacy, Kotlin Multiplatform |
| `rest-api` | 00, 01, 02, 05, 06, 07, 08, 09, 10, 13 | 03 (DX), 04 | 11 si solo es interno | `<Sol>.Api/`, `<Sol>.Application/`, `<Sol>.Domain/`, `<Sol>.Infrastructure/` | Cliente HTTP de referencia, colección de pruebas, SDK generado | ASP.NET Core, FastAPI, Express, Gin, Quarkus |
| `cli-tool` | 00, 01, 02, 05, 06, 08, 09, 10, 11, 12 | 03 | 04, 07 | `<Sol>.Cli/`, `<Sol>.Core/` | Recetas de uso por OS, scripts de demostración | Cobra (Go), Click (Python), System.CommandLine (.NET), oclif (Node) |
| `worker-service` | 00, 01, 02, 05, 06, 08, 09, 13 | 04, 07 | 11 si no hay API | `<Sol>.Worker/`, `<Sol>.Domain/`, `<Sol>.Infrastructure/` | Compose con broker y productor de prueba | .NET Worker Service, Sidekiq, Celery, Temporal, Hangfire |

Lo que NO va en esta sección:
- Stack concreto, versiones, frameworks por proyecto (eso va al bloque técnico de §5).
- Patrones arquitectónicos por proyecto (van al bloque técnico de §5).
- Estructura de carpetas (va a §4).

---

## §2 Estilo arquitectónico de la solución

Instrucción: Describir cómo se componen los proyectos entre sí a alto nivel: quién depende de quién, qué expone cada proyecto a los que lo consumen y por qué la jerarquía es la elegida. El detalle arquitectónico interno de cada proyecto va a su bloque técnico (§5 P.2) y a `05_arquitectura_tecnica/`. Esta sección es la vista de solución por encima de los proyectos.

Preguntas guía:
- (*) ¿Cómo se relacionan los proyectos y qué contrato expone cada uno a sus dependientes?
- (*) ¿Por qué esta descomposición en proyectos y no otra (un monolito, más microservicios)?
- ¿Qué proyecto es el punto de entrada de la solución para el consumidor final?
- ¿Hay proyectos compartidos (dominio, validaciones) que varios consumen?

Ejemplo genérico (solución Gestión de Turnos):
> La solución se compone de una API REST (proyecto principal) que expone los turnos, una librería de dominio compartida que concentra entidades y reglas, un worker de notificaciones que consume el dominio para enviar recordatorios, y un paquete redistribuible de validaciones reusable e independiente de la solución. La API y el worker dependen del dominio; el dominio y la API dependen del paquete de validaciones. El grafo es acíclico. El consumidor final entra por la API.

Lo que NO va en esta sección:
- Detalle de capas internas de cada proyecto (va a §5 P.2 y a 05).
- Esquema de persistencia (va a §5 P.4 del proyecto correspondiente).
- Diagramas formales de despliegue (van a 05 y a la vista de solución de ST-05).

---

## §3 Esquema de descomposición y delivery

Instrucción: Declarar la estrategia de descomposición del trabajo en el tiempo para la solución. Optar por vertical slicing, horizontal slicing, walking skeleton o thin slice, y justificar. El criterio bloqueante es que el primer sprint entregue valor demostrable end-to-end atravesando los proyectos necesarios. Si la estrategia elegida no cumple ese criterio, redefinir.

Preguntas guía:
- (*) ¿El primer sprint entrega valor demostrable end-to-end a través de la jerarquía?
- (*) ¿La descomposición es vertical (rebanadas funcionales) u horizontal (capas o proyectos por sprint)?
- ¿Existe un walking skeleton que atraviese los proyectos clave en Sprint 0/1?
- ¿En qué orden se construyen los proyectos según sus dependencias (orden topológico del manifiesto)?

Ejemplo genérico (solución Gestión de Turnos, vertical slicing):
> Se aplica vertical slicing desde el Sprint 1. El walking skeleton del Sprint 0 atraviesa el paquete de validaciones, el dominio y la API para reservar un turno mínimo end-to-end, sin worker de notificaciones. Cada sprint posterior agrega profundidad manteniendo la capacidad de reservar un turno: Sprint 2 incorpora el worker de recordatorios, Sprint 3 agrega cancelaciones. El orden de construcción respeta el orden topológico del manifiesto: validaciones, dominio, y luego API y worker en paralelo.

Lo que NO va en esta sección:
- Plan de sprint detallado con US/BT (va a `07_plan-sprint/`).
- Backlog de features (va a `06_backlog-tecnico/`).

---

## §4 Estructura de repositorio de la solución

Instrucción: Proponer el árbol `tree` completo con `/src`, `/tests`, `/samples`, `/docs`, `/devs`. La estructura se deriva de la jerarquía declarada en el manifiesto y de la convención de nombres de código: cada proyecto es una carpeta `src/<NombreProyectoCodigo>/`, donde `<NombreProyectoCodigo>` es `<NombreSolucionCodigo>.<Sufijo>` salvo los redistribuibles, que arrancan con el prefijo de organización. Incluir obligatoriamente la subsección §4.1 sobre `/samples`.

Preguntas guía:
- (*) ¿Cada proyecto del manifiesto tiene su carpeta en `/src` con su nombre de código?
- (*) ¿La estructura sigue las convenciones del ecosistema del lenguaje?
- ¿Los redistribuibles arrancan con el prefijo de organización y no con la raíz de la solución?
- ¿Hay separación clara entre código productivo (`/src`), tests (`/tests`) y samples (`/samples`)?

Ejemplo genérico (solución Gestión de Turnos, estructura repo):
```text
gestion-de-turnos/
├── src/
│   ├── GestionDeTurnos.WebApi/         # rest-api (principal)
│   ├── GestionDeTurnos.Domain/         # library de dominio compartida
│   ├── GestionDeTurnos.Worker/         # worker-service de notificaciones
│   └── Aplicada.Validaciones/          # library redistribuible (prefijo de organización)
├── tests/
│   ├── GestionDeTurnos.WebApi.Tests/
│   ├── GestionDeTurnos.Domain.Tests/
│   └── Aplicada.Validaciones.Tests/
├── samples/
│   ├── 01-basico/                      # consumo mínimo
│   ├── 02-intermedio/
│   └── 03-avanzado/
├── docs/
│   ├── 00_contexto/
│   └── ... (categorías 00-11 SDD)
├── devs/
│   ├── intake/                         # SOLUTION-MANIFEST, PROJECT-BRIEF, PROJECT-README
│   └── decisiones-proyecto_v1.0.md
└── README.md                           # Apuntador corto al PROJECT-README
```

### §4.1 Materialización de `/samples` (alimenta `11_examples/`)

Instrucción: Describir cómo se materializan los samples según el tipo D8 de cada proyecto que los produce. Cada sample debe ser autocontenido, ejecutable y declarar su complejidad (básico, intermedio, avanzado).

| Tipo D8 | Qué hay en `/samples` | Vínculo con `/src` |
|---|---|---|
| `library` | 3 proyectos consumidores progresivos | ProjectReference en desarrollo, PackageReference para validar la publicación |
| `web-monolith` | Datos seed, scripts de bootstrap, opcional skin custom | Mismo solution |
| `web-microservices` | Compose con todos los servicios más un cliente de prueba | Submódulos o monorepo |
| `desktop-app` | Plugin de demostración, tema custom | Plugin vía interfaz pública |
| `mobile-app-maui` | Proyecto demo con storyboard de pantallas y datos mock | Mismo solution o consumer externo |
| `rest-api` | SDK generado, cliente HTTP de referencia, colección de pruebas | Cliente externo |
| `cli-tool` | Recetas de uso por OS, scripts de demostración | Consumer externo |
| `worker-service` | Productor de prueba, compose con broker | Compose paralelo |

Lo que NO va en esta sección:
- Contenido detallado de los samples (va a `11_examples/`).
- Convenciones de versionado de paquetes (van al bloque técnico §5 P.7).

---

## §5 Bloque técnico por proyecto (plantilla repetible)

Instrucción: Para cada proyecto declarado en §1, copiar el bloque de identidad más las subsecciones P.1 a P.12 y completarlas. Si la solución tiene N proyectos, este bloque aparece N veces, uno por proyecto. Cada proyecto es autocontenido: sus decisiones de stack, arquitectura, persistencia, testing, versionado, pipeline, compatibilidad y NFR se declaran acá, para su `project_type`. Encabezar cada réplica con el bloque de identidad del proyecto.

Identidad del proyecto (repetir por proyecto):

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | [kebab] |
| `nombre-proyecto-codigo` | [`<NombreSolucionCodigo>.<Sufijo>` o `Aplicada.<X>`] |
| `project_type` (D8) | [uno de los 8] |
| Rol | [una frase] |
| `redistribuible` | [true / false] |

### §5.P.1 Stack tecnológico

Instrucción: Declarar lenguaje, versión, runtime, framework y plataformas target del proyecto. Incluir las dependencias core con justificación. Excluir dependencias de desarrollo o testing (van a P.6 o P.8).

Preguntas guía:
- (*) ¿Cuál es la versión mínima del lenguaje y del runtime?
- (*) ¿Cuáles son las dependencias core sin las que el proyecto no compila?
- ¿El stack permite cumplir los NFR de P.10?

### §5.P.2 Estilo arquitectónico del proyecto

Instrucción: Elegir el estilo arquitectónico interno del proyecto y justificarlo contra al menos dos alternativas descartadas. Debe ser coherente con su `project_type` y con la composición de solución de §2. La decisión se materializa como ADR en `05_arquitectura_tecnica/`.

Preguntas guía:
- (*) ¿Qué estilo arquitectónico interno se adopta y por qué?
- (*) ¿Qué dos alternativas se descartaron y por qué?
- ¿La complejidad operativa es proporcional al tamaño del problema del proyecto?

### §5.P.3 Comunicación e integración

Instrucción: Aplica si el proyecto integra con otros sistemas o con otros proyectos de la solución. Declarar protocolo, formato de payload, versión de los contratos y política de breaking changes. Los contratos hacia otros proyectos de la solución deben ser coherentes con las dependencias del manifiesto. Si no aplica, declarar "No aplica" con justificación.

Preguntas guía:
- (*) ¿Qué protocolos se usan en flujos sincrónicos y asincrónicos?
- (*) ¿Cómo se versionan los contratos?
- ¿Qué expone este proyecto a los proyectos de la solución que dependen de él?

### §5.P.4 Persistencia

Instrucción: Declarar qué datos guarda el proyecto, dónde, cómo se versiona el esquema y qué patrones de acceso usa. Aclarar si es multi-tenant y cómo aísla cada tenant. Si no hay persistencia, declarar "No aplica".

Preguntas guía:
- (*) ¿Qué motor de persistencia se usa y por qué?
- (*) ¿Cómo se versiona el esquema?
- ¿El proyecto es multi-tenant? ¿Aislamiento por base, esquema o columna?

### §5.P.5 Seguridad y autenticación

Instrucción: Declarar mecanismos de autenticación, autorización y manejo de secretos del proyecto. Especificar flujos si aplica, formato de tokens y política de roles. Aclarar cómo se manejan los secretos en runtime y en CI/CD.

Preguntas guía:
- (*) ¿Qué mecanismo de autenticación se usa y dónde reside el Identity Provider?
- (*) ¿Dónde se almacenan los secretos en runtime y en CI/CD?
- ¿Hay requisitos de compliance que afecten la seguridad?

### §5.P.6 Estrategia de testing

Instrucción: Declarar la pirámide de testing con porcentajes orientativos, cobertura mínima numérica (gate del CI), frameworks por nivel y si se aplica BDD/ATDD. La cobertura mínima es bloqueante: debe ser numérica.

Preguntas guía:
- (*) ¿Cuál es la cobertura mínima de líneas y de branches?
- (*) ¿Qué frameworks se usan en cada nivel de la pirámide?
- ¿Cómo se manejan los tests de contrato hacia otros proyectos de la solución?

### §5.P.7 Estrategia de versionado y release

Instrucción: Adoptar SemVer 2.0.0 y Conventional Commits. Declarar herramienta de cálculo de versión, estrategia de branching, canales preview/stable y feed de paquetes. Esta sección alimenta P.8.

Preguntas guía:
- (*) ¿Se adopta SemVer 2.0.0 y Conventional Commits sin excepciones?
- (*) ¿Qué herramienta calcula la versión y a partir de qué?
- ¿Qué canales de release existen y dónde se publican los artefactos?

### §5.P.8 Pipeline CI/CD

Instrucción: Declarar stages del pipeline (build, test, lint, SCA, SBOM, firma, publicación), matriz de SO y runtime, y ambientes. Cada stage debe tener un quality gate explícito. La cobertura mínima debe coincidir con P.6.

Preguntas guía:
- (*) ¿Qué plataforma de CI se usa?
- (*) ¿Cuáles son los quality gates bloqueantes para mergear?
- ¿Cómo se hace rollback de una publicación problemática?

### §5.P.9 Compatibilidad y plataformas target

Instrucción: Declarar SO soportados, runtimes, navegadores, dispositivos y versiones mínimas, coherentes con el `project_type`. Toda combinación no listada se considera no soportada.

Preguntas guía:
- (*) ¿Cuáles son las plataformas target soportadas?
- (*) ¿Cuál es la versión mínima de cada runtime/SO/navegador?
- ¿Hay arquitecturas que NO se soportan?

### §5.P.10 Requerimientos no funcionales (NFR)

Instrucción: Declarar métricas numéricas concretas de performance, escalabilidad, disponibilidad, observabilidad y compliance del proyecto. Cada métrica debe ser medible. NFR vagos no son aceptables.

Preguntas guía:
- (*) ¿Cuál es la latencia objetivo p99 y el throughput mínimo?
- (*) ¿Cuál es el SLO de disponibilidad?
- ¿Qué se loguea, qué se mide y qué se traza?

### §5.P.11 Decisiones técnicas pre-tomadas (pre-ADR)

Instrucción: Listar decisiones técnicas cerradas del proyecto antes del Sprint 0 con justificación y alternativas evaluadas. Se convierten en ADRs aceptados en `05_arquitectura_tecnica/`.

Preguntas guía:
- (*) ¿Cuáles son las decisiones cerradas antes del Sprint 0 y por qué?
- (*) ¿Qué alternativas se evaluaron en cada decisión?
- ¿Qué decisiones quedan deliberadamente abiertas para Sprint 0?

### §5.P.12 Restricciones técnicas y trade-offs aceptados

Instrucción: Documentar a qué renuncia el proyecto conscientemente para ganar otra cosa. Evita reabrir discusiones cerradas y documenta el costo aceptado.

Preguntas guía:
- (*) ¿Qué ganancias se priorizaron y a costa de qué renuncias?
- (*) ¿Hay restricciones impuestas por el cliente o el ecosistema?
- ¿Qué tipo de cargas de trabajo el proyecto no soporta?

Lo que NO va en el bloque técnico:
- Necesidades de negocio (van a PROJECT-BRIEF).
- ADRs formales (viven en `05_arquitectura_tecnica/`).
- Detalle de los samples (va a `11_examples/`).

---

## §6 Estrategia de demo / samples (alimenta `11_examples`)

Instrucción: Detallar qué aplicaciones de ejemplo se construyen en `/samples`, qué proyecto de la solución demuestra cada una, su nivel de complejidad y cómo se vinculan a `/src`. Cada sample debe ser autocontenido y reproducible en un entorno limpio en cinco pasos o menos.

Preguntas guía:
- (*) ¿Cuántos samples se construyen y qué proyecto de la solución ilustra cada uno?
- (*) ¿Cómo se vincula cada sample con el código productivo?
- ¿Hay un sample que demuestre el punto de extensión principal de la solución?

Lo que NO va en esta sección:
- Tutoriales conceptuales (van a `docs/10_developer_guide/`).
- Casos de uso de negocio (van a `docs/02_especificacion_funcional/`).

---

## §7 Checklist de completitud técnica

Verificar antes de marcar el documento como Aprobado. Todos los ítems deben estar tildados.

Nivel solución:
- [ ] §1 enumera todos los proyectos de la solución y refleja la tabla del manifiesto sin divergencias.
- [ ] Cada proyecto de §1 lleva exactamente uno de los 8 valores D8.
- [ ] Hay exactamente un proyecto principal declarado.
- [ ] §2 describe la composición de la solución y los contratos entre proyectos.
- [ ] §3 garantiza valor demostrable end-to-end en el primer sprint a través de la jerarquía.
- [ ] §4 publica el árbol `tree` completo, derivado de la jerarquía y de la convención de nombres.
- [ ] §4.1 describe la materialización de `/samples` por tipo D8.

Por cada proyecto (replicar la verificación):
- [ ] El bloque técnico de §5 está completo para el proyecto, con su identidad y `project_type`.
- [ ] P.1 declara lenguaje, versión, runtime y dependencias core con justificación.
- [ ] P.2 elige y justifica el estilo arquitectónico contra dos alternativas.
- [ ] P.6 declara cobertura mínima numérica de líneas y de branches.
- [ ] P.7 adopta SemVer 2.0.0 y Conventional Commits con herramienta automatizada.
- [ ] P.8 enumera stages con quality gates explícitos y bloqueantes.
- [ ] P.9 declara SO, runtimes y versiones mínimas sin ambigüedad.
- [ ] P.10 expresa NFR con métricas numéricas.
- [ ] P.11 documenta decisiones pre-ADR con alternativas evaluadas.
- [ ] P.12 documenta trade-offs aceptados con su contrapartida.

General:
- [ ] El control de cambios refleja la versión y fecha del documento.

---

## Trazabilidad downstream

Este documento alimenta las siguientes secciones SDD 2.2. La trazabilidad se aplica por proyecto según su `project_type`.

| Sección del README | Categoría SDD destino | Documento downstream típico |
|---|---|---|
| §1 Proyectos de la solución | Todas las categorías 00-11 (por proyecto) | Selector de variantes de especialidad por proyecto |
| §2 Estilo de la solución | `05_arquitectura_tecnica/` (vista de solución) | `arquitectura-solucion_v1.0.md` |
| §4 Estructura de repositorio | `05_arquitectura_tecnica/`, `10_developer_guide/` | `arquitectura-solucion_v1.0.md`, README de carpeta |
| §5 P.2 Estilo del proyecto | `05_arquitectura_tecnica/` | `ADR-XX-estilo-arquitectonico_v1.0.md` |
| §5 P.3 Comunicación | `05_arquitectura_tecnica/` | `contratos_v1.0.md` |
| §5 P.4 Persistencia | `02_especificacion_funcional/modelo-datos/`, `05_arquitectura_tecnica/` | `modelo-datos-logico_v1.0.md` |
| §5 P.5 Seguridad | `09_devops/`, `05_arquitectura_tecnica/` | `politica-seguridad_v1.0.md` |
| §5 P.6 Testing | `08_calidad_y_pruebas/` | `estrategia-testing_v1.0.md`, `definition-of-done_v1.0.md` |
| §5 P.7 Versionado | `09_devops/` | `estrategia-versionado_v1.0.md` |
| §5 P.8 Pipeline | `09_devops/` | `pipeline-ci-cd_v1.0.md`, `entornos-deploy_v1.0.md` |
| §5 P.9 Compatibilidad | `00_contexto/` | `compatibilidad-plataformas_v1.0.md` |
| §5 P.10 NFR | `00_contexto/`, `08_calidad_y_pruebas/` | `requerimientos-no-funcionales_v1.0.md` |
| §5 P.11 Pre-ADR | `05_arquitectura_tecnica/` | `ADR-XX-<kebab>_v1.0.md` por decisión |
| §6 Samples | `11_examples/` | `README.md` y `ejemplo-01..03_v1.0.md` |

---

## Control de cambios

| Version | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | [YYYY-MM-DD] | Plantilla inicial generada durante bootstrap SDD 2.2 | Bootstrap SDD 2.2 |
| 1.1 | 2026-06-09 | Reformulación a documento de solución (ST-03): §1 enumera el tipo por proyecto reflejando el manifiesto en lugar de elegir un único tipo; nueva §2 estilo de solución; §4 deriva la estructura de la jerarquía y de la convención de nombres; §5 se convierte en bloque técnico repetible por proyecto (condensa las antiguas §2, §3, §6 a §13, §15 y §16); checklist adaptado a multi-proyecto. | Reformulación SDD 2.2D |
