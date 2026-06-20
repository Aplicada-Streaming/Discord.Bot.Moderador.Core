# Reglas constructivas — 11 Examples

**Carpeta target (por proyecto):** `/SDD2.1D/docs/proyectos/<nombre-proyecto-kebab>/11_examples/`
**Subagente target del orquestador:** Developer Advocate / Sample Engineer Senior (AG-11)
**Versión de las reglas:** 1.2

---

## 0. Posición en la cadena SDD 2.1

La categoría 11 materializa el sistema en código ejecutable concreto. Es el punto final de la cadena de trazabilidad SDD 2.1 y la primera puerta de entrada práctica para todo desarrollador que consuma el producto. Recibe upstream de 02 (casos de uso que cada sample ilustra), de 05 (arquitectura y contratos públicos que cada sample respeta) y de 10 (developer guide cuyos conceptos cada sample demuestra paso a paso). No tiene downstream metodológico dentro de SDD; sus consumidores son externos al ciclo de especificación: equipos integradores, devs nuevos del proyecto, evaluadores técnicos del cliente.

A diferencia de la categoría 10, que documenta conceptos y guías intercaladas con snippets, la categoría 11 produce proyectos completos y funcionales que se clonan, se ejecutan en un entorno limpio y se modifican como punto de partida. La diferencia es operativa: 10 explica, 11 ejecuta.

Esta categoría es opcional según el tipo D8. Es obligatoria para `library`, `cli-tool`, `mobile-app-maui`, `rest-api`, `desktop-app` y `web-microservices` porque el consumidor del artefacto necesita ejemplos reproducibles para arrancar. Es recomendada para `web-monolith` y `worker-service` cuando el proyecto se expone como referencia para otros equipos. Es omisible cuando el proyecto es estrictamente interno y no hay nuevos consumidores previsibles.

La auditoría de Fase 0 (`_bootstrap/audit-sdd1.md`) detectó dos déficits del fuente SDD 1.0 que SDD 2.1 corrige aquí. Primero, los archivos del fuente nombran los ejemplos por dominio (`ejemplo-02-multa.md`, `ejemplo-03-multaapp-nuget.md`), atando el sample al producto particular y descartando la idea de progresión didáctica. SDD 2.1 obliga a nombrar por nivel de complejidad o por capacidad demostrada (`ejemplo-01-basico`, `ejemplo-02-intermedio`, `ejemplo-03-avanzado`, o variantes por capacidad como `ejemplo-01-cliente-http-basico`, `ejemplo-02-postman-collection`). Segundo, los markdown del fuente no llevan sufijo de versión; SDD 2.1 obliga al sufijo uniforme `_v<X.Y>.md` para todos los archivos versionables de esta categoría, incluyendo `README.md` cuando se considere artefacto versionable (en este caso se mantiene `README.md` sin sufijo por convención de índice).

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Developer Advocate / Sample Engineer Senior, equivalente al AG-11 del catálogo SDD. Perfil profesional que construye aplicaciones de referencia ejecutables que demuestran cómo usar el producto en escenarios reales. A diferencia del Technical Writer (AG-10) que documenta conceptos y guías, el Developer Advocate produce código que se compila, se corre y se observa. Cada sample es un proyecto autocontenido con su propia documentación, sus prerequisitos, sus comandos de arranque y su resultado esperado verificable.

El rol combina Sample Engineering (construcción de proyectos de referencia progresivos) con curaduría editorial: decide qué capacidades exhibe cada sample, cómo escalan en complejidad, qué CU ilustran y qué punto de extensión del sistema cubren. Cuando el proyecto se expone públicamente, el rol también incluye Developer Relations (DevRel) y Developer Education, pero esas facetas se activan en proyectos con comunidad de consumidores externos, no en todo proyecto interno.

### 1.2 Variantes según tipo de proyecto (8 valores D8)

| Tipo | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Sample Engineer | Apps consumidoras progresivas (consola, mini-app) que invocan la librería vía package manager. Cada sample muestra un nivel distinto de la superficie pública. |
| web-monolith | Sample Engineer | Datos seed para arrancar la app de demostración, scripts de bootstrap, screenshots de UI y opcionalmente un tema custom mínimo. |
| web-microservices | Sample Engineer + Demo Orchestrator | docker-compose end-to-end con todos los servicios más un cliente de prueba (curl, Postman, Bruno). Demo Orchestrator garantiza que el entorno levante con un comando. |
| desktop-app | Sample Engineer | Plugin de demostración y tema custom que prueban los puntos de extensión declarados en 05. |
| mobile-app-maui | Sample Engineer | Proyecto móvil multiplataforma demo con storyboard de pantallas, datos mock y un escenario offline-first. |
| rest-api | Sample Engineer + API Demo | Cliente HTTP de referencia (curl + lenguaje del consumidor típico), colección Postman o Bruno, SDK generado a partir del contrato OpenAPI. |
| cli-tool | Sample Engineer | Recetas multi-OS (Windows, Linux, macOS) con scripts batch/bash equivalentes y casos de uso reales por subcomando. |
| worker-service | Sample Engineer | docker-compose con broker (RabbitMQ, Kafka o equivalente) y productor de prueba que dispara mensajes representativos al worker. |

El orquestador lee esta tabla y, según el `project_type` del proyecto en curso (leído del manifiesto de solución), selecciona la variante correspondiente y la combina con la especialidad base. La variante se aplica una vez por cada proyecto de la solución.

### 1.3 Multi-especialidad

La categoría 11 colabora con varias especialidades durante la redacción y la revisión:

- AG-02 Analista Funcional, para identificar qué CU debe ilustrar cada sample y validar la trazabilidad declarada.
- AG-05 Arquitecto, para garantizar que los samples respeten los contratos públicos y los puntos de extensión definidos.
- AG-08 QA, para que cada sample tenga al menos un test ejecutable que sirva de verificación automatizada del resultado esperado.
- AG-09 DevOps, para que los samples se construyan en un job del pipeline CI y se verifique periódicamente que siguen siendo ejecutables.
- AG-10 Technical Writer, para mantener consistencia entre las guías conceptuales y los samples que las materializan.

El AG-11 mantiene titularidad de los artefactos. Las demás especialidades aportan revisión sectorial y consumen los samples como ejemplo verificable de las decisiones tomadas en sus categorías.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra

| Archivo | Obligatorio para | Recomendado | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `README.md` | library, cli-tool, mobile-app-maui, rest-api, desktop-app, web-microservices | web-monolith, worker-service | Proyectos estrictamente internos sin consumidores externos | Índice de samples con tabla maestra (nivel, tiempo de setup, CU ilustrados, ubicación en `/samples`). |
| `ejemplo-01-basico_v<X.Y>.md` o `ejemplo-01-<kebab-progresion>_v<X.Y>.md` | library, cli-tool, mobile-app-maui, rest-api, desktop-app, web-microservices | web-monolith, worker-service | — | Markdown explicativo del sample de nivel básico. |
| `ejemplo-02-intermedio_v<X.Y>.md` o `ejemplo-02-<kebab-progresion>_v<X.Y>.md` | library, cli-tool, mobile-app-maui, rest-api | desktop-app, web-microservices, web-monolith, worker-service | — | Markdown explicativo del sample de nivel intermedio. |
| `ejemplo-03-avanzado_v<X.Y>.md` o `ejemplo-03-<kebab-progresion>_v<X.Y>.md` | library, cli-tool, mobile-app-maui, rest-api | desktop-app, web-microservices | — | Markdown explicativo del sample de nivel avanzado o de integración. |
| `imagenes/` (carpeta) | Cuando los markdown referencian screenshots o assets visuales | — | Samples sin UI | Carpeta de assets versionados (PNG, SVG). Sin assets binarios pesados; preferir vectoriales. |

Cada markdown explicativo se acompaña de un proyecto ejecutable en `/samples/<carpeta-correspondiente>/` del repositorio. La categoría 11 documenta el sample; la materialización en código vive en `/samples` y se gobierna desde §16.1 del SOLUTION-INTAKE (materialización de `/samples`).

### 2.2 Cantidad mínima de samples por tipo

| Tipo D8 | Samples mínimos | Niveles cubiertos |
| --- | --- | --- |
| library | 3 | básico + intermedio + avanzado |
| web-monolith | 2 | datos seed + tema custom (si hay punto de extensión visual) |
| web-microservices | 2 | compose mínimo + compose end-to-end |
| desktop-app | 2 | plugin demo + tema custom |
| mobile-app-maui | 3 | app básica + sync offline + multiplataforma |
| rest-api | 3 | cliente HTTP básico + colección Postman/Bruno + SDK tipado |
| cli-tool | 3 | recetas Windows + recetas Linux + recetas macOS |
| worker-service | 2 | compose con broker + productor de prueba |

Estos son pisos. Un proyecto puede agregar samples adicionales para cubrir capacidades extra (autenticación, multi-tenancy, observabilidad), siempre que cada uno mantenga la nomenclatura por progresión o por capacidad y declare su nivel.

### 2.3 Matriz tipo D8 → carpetas en `/samples`

La carpeta `/samples` del repositorio refleja directamente los archivos documentados en `docs/11_examples/`. Hay correspondencia 1:1 entre cada `ejemplo-XX-<kebab>_v<X.Y>.md` y una carpeta ejecutable en `/samples/`.

| Tipo D8 | Estructura mínima de `/samples` |
| --- | --- |
| library | `/samples/01-basico-consola/`, `/samples/02-intermedio-con-extensiones/`, `/samples/03-avanzado-integracion-real/` |
| web-monolith | `/samples/01-datos-seed/`, `/samples/02-tema-custom/` (este último sólo si hay punto de extensión visual) |
| web-microservices | `/samples/01-compose-minimo/`, `/samples/02-compose-end-to-end/` |
| desktop-app | `/samples/01-plugin-demo/`, `/samples/02-tema-custom/` |
| mobile-app-maui | `/samples/01-app-basica/`, `/samples/02-sync-offline/`, `/samples/03-multiplatform-demo/` |
| rest-api | `/samples/01-cliente-http-basico/`, `/samples/02-postman-collection/`, `/samples/03-sdk-tipado-generado/` |
| cli-tool | `/samples/01-recetas-windows/`, `/samples/02-recetas-linux/`, `/samples/03-recetas-mac/` |
| worker-service | `/samples/01-compose-broker/`, `/samples/02-productor-de-prueba/` |

Esta tabla es vinculante: el contenido de `/samples` no se inventa; se deriva del tipo D8 declarado y se ajusta sólo agregando carpetas extra, nunca renombrando las base por nombres atados al dominio.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

- `ejemplo-XX-<kebab-progresion>_v<X.Y>.md` para el markdown explicativo de cada sample.
- `XX` es el número correlativo en dos dígitos, empezando en `01` y respetando la progresión.
- `<kebab-progresion>` describe nivel de complejidad o capacidad demostrada, en kebab-case estricto y lowercase. Valores admitidos por nivel: `basico`, `intermedio`, `avanzado`. Valores admitidos por capacidad: `cliente-http-basico`, `postman-collection`, `sdk-tipado-generado`, `plugin-demo`, `tema-custom`, `compose-minimo`, `compose-end-to-end`, `recetas-windows`, `recetas-linux`, `recetas-mac`, `compose-broker`, `productor-de-prueba`, `app-basica`, `sync-offline`, `multiplatform-demo`, `datos-seed`, `con-extensiones`, `integracion-real`.
- Sufijo `_v<X.Y>.md` obligatorio y uniforme. Queda prohibido el patrón heredado sin versión (por ejemplo `ejemplo-01-simple.md` del fuente). Queda prohibido el sufijo de dominio (por ejemplo `ejemplo-02-multa`, `ejemplo-03-multaapp-nuget`, `ejemplo-04-factura`, `ejemplo-05-recibo`, o cualquier otro nombre atado al producto particular).
- `README.md` de la sección sin sufijo de versión. Es el índice navegable.

### 3.2 Reglas de progresión

La numeración refleja un orden de lectura recomendado de menor a mayor complejidad. Sample 01 introduce el camino feliz mínimo; sample 02 agrega complejidad típica del caso intermedio (configuración, datos reales, integraciones simples); sample 03 demuestra el caso avanzado o el punto de extensión principal. Si el proyecto opta por progresión por capacidad en lugar de por nivel, cada sample debe igual declarar su nivel implícito en §2 del markdown (básico, intermedio o avanzado).

Está prohibida la numeración por dominio del proyecto. La progresión `multa → multaapp-nuget` del fuente es exactamente lo que SDD 2.1 corrige: ese par mezcla nombre de caso de uso con nombre de mecanismo de distribución, sin transmitir progresión didáctica. SDD 2.1 obliga a que el slug responda a "qué demuestra el sample respecto al anterior", no a "qué dominio modela".

### 3.3 Vinculación cross-doc

- Upstream: cada sample declara qué CU de 02 ilustra, qué bloque arquitectónico de 05 ejercita y qué guía de 10 lo acompaña conceptualmente. La trazabilidad vive en §8 del markdown explicativo.
- Downstream: el sample en sí no tiene downstream metodológico; sus consumidores son externos (devs integradores, evaluadores técnicos, equipos de adopción).
- Vinculación con el código: cada markdown apunta a la carpeta concreta de `/samples/XX-<kebab-progresion>/` del repositorio. La estructura del código sigue la misma progresión que la documentación.

### 3.4 Política de versionado

Cada markdown se versiona como `_v<X.Y>.md`. La primera emisión es siempre `_v1.0`. Cuando el sample se reescribe sustancialmente (cambia el escenario, el nivel o el conjunto de capacidades demostradas), se promueve a `_v2.0` y la versión anterior se archiva en `_legacy/` con estado `Superado`. Cambios menores que no alteran la progresión ni el escenario (correcciones de typos, refresh de screenshots, ajustes de prerequisitos) van en `_v1.1`, `_v1.2`, etc. dentro de la misma rama mayor.

El código en `/samples/` se versiona junto con el repositorio principal: no lleva sufijo propio, pero su CI debe garantizar que siempre compila contra la versión actual del producto.

### 3.5 README de la sección

El `README.md` de `/docs/11_examples/` lista los samples vigentes en una tabla maestra con columnas: número, slug, nivel, tiempo de setup estimado, CU ilustrados, ubicación en `/samples`. El README se actualiza cada vez que se agrega, renombra o jubila un sample.

---

## 4. Estructura de redacción

### 4.1 Cabecera obligatoria

Cada markdown explicativo inicia con un H1 y un bloque de metadatos uniforme:

```markdown
# Ejemplo XX — <Nombre descriptivo del sample>

**Proyecto:** {{nombre-proyecto}}
**Documento:** ejemplo-XX-<kebab-progresion>_v<X.Y>.md
**Versión:** <X.Y>
**Estado:** Borrador | Propuesto | Aprobado | Vigente | Superado | Archivado
**Fecha:** YYYY-MM-DD
**Autor:** {{equipo-o-rol}}
**Nivel:** Básico | Intermedio | Avanzado
**Ubicación del código:** `/samples/XX-<kebab-progresion>/`
```

### 4.2 Secciones obligatorias del markdown explicativo

1. **Objetivo del sample.** Una a tres oraciones que describen qué capacidad demuestra el sample y qué aprende el desarrollador después de ejecutarlo. Responde a "¿por qué este sample existe?".
2. **Nivel.** Declaración explícita del nivel (básico, intermedio o avanzado) con justificación breve respecto al sample anterior. Si la progresión es por capacidad, igualmente se declara el nivel implícito.
3. **Prerequisites.** Lista de herramientas, runtimes, SDKs, credenciales y dependencias externas necesarias para ejecutar el sample. Sin ambigüedad; cada ítem tiene versión mínima cuando aplica.
4. **Cómo correrlo.** Comandos paso a paso copiables, máximo cinco pasos para llegar a la primera ejecución exitosa. Cada comando se ejecuta en un entorno limpio reproducible.
5. **Estructura del código.** Árbol resumido de la carpeta `/samples/XX-<kebab-progresion>/` con descripción breve por archivo o subcarpeta relevante.
6. **Qué esperar.** Output exacto que el desarrollador verá después de ejecutar el sample, ya sea texto en consola, payload HTTP, screenshot de UI o archivo generado. Si hay UI, screenshot bajo `imagenes/`.
7. **Variaciones sugeridas.** Tabla con dos a cuatro variaciones que el desarrollador puede probar modificando el sample, con descripción de qué cambiar y qué resultado esperar. Sirve de puente didáctico hacia el sample siguiente.
8. **Trazabilidad.** Tabla con los CU, NB, ADR o NFR de 02/05 que el sample ilustra. Cada fila enlaza al artefacto fuente.
9. **Control de cambios.** Tabla de versiones del propio markdown con fecha, versión y descripción del cambio.

### 4.3 Estructura del `README.md` de la sección

1. Propósito de la carpeta: qué encuentra el lector en `/docs/11_examples/` y en `/samples/`.
2. Tabla maestra de samples vigentes.
3. Convenciones de los samples (autocontenidos, ejecutables en entorno limpio, trazabilidad obligatoria, niveles declarados).
4. Cómo agregar un sample nuevo: referencia al §6 de estas reglas y al template del markdown explicativo.
5. Vínculo con la developer guide de 10 y con la arquitectura de 05.

### 4.4 Tablas tipo

Tabla maestra del `README.md`:

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación |
| --- | --- | --- | --- | --- |
| `ejemplo-01-basico_v1.0.md` | Básico | < 5 min | CU-01, CU-02 | `/samples/01-basico/` |
| `ejemplo-02-intermedio_v1.0.md` | Intermedio | 10-15 min | CU-03, CU-04, CU-05 | `/samples/02-intermedio/` |
| `ejemplo-03-avanzado_v1.0.md` | Avanzado | 20-30 min | CU-06, CU-07, CU-08 | `/samples/03-avanzado/` |

Tabla tipo de proyecto vs estructura de `/samples` (replica resumida de §2.3 al pie del README):

| Tipo D8 | Estructura de `/samples` |
| --- | --- |
| library | `01-basico-consola/`, `02-intermedio-con-extensiones/`, `03-avanzado-integracion-real/` |
| rest-api | `01-cliente-http-basico/`, `02-postman-collection/`, `03-sdk-tipado-generado/` |
| cli-tool | `01-recetas-windows/`, `02-recetas-linux/`, `03-recetas-mac/` |
| (resto) | Ver §2.3 de las reglas constructivas. |

Tabla de trazabilidad por sample (en §8 de cada markdown):

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-XX | Caso de uso | El sample ejecuta el flujo principal del CU end-to-end. |
| ADR-XX | Decisión arquitectónica | El sample materializa la decisión declarada en el ADR. |
| NFR-XX | Requisito no funcional | El sample mide el SLA del NFR mediante un script o test. |

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| Samples nombrados por dominio del proyecto | Atan el ejemplo al producto particular y rompen la progresión didáctica (lección del fuente: `multa`, `multaapp-nuget`) | Nombrar por nivel o por capacidad, nunca por entidad del dominio |
| Samples sin nivel declarado | El lector no sabe qué orden seguir y no hay progresión | Cada sample declara nivel explícito en §2 |
| Samples no ejecutables o desactualizados | El dev clona y no le compila; la documentación pierde credibilidad | CI que compila y ejecuta cada sample en cada push |
| Samples que duplican el `/src` sin agregar valor demostrativo | Inflan el repositorio sin enseñar nada nuevo | Cada sample demuestra una capacidad distinta o un punto de extensión |
| Falta de trazabilidad a CU | El sample existe en el vacío y no se sabe qué requisito ejercita | §8 obligatoria con tabla de upstream |
| Samples sin versión en el nombre | Hace imposible saber a qué versión del producto pertenecen (lección del fuente: `ejemplo-01-simple.md` sin sufijo) | Sufijo `_v<X.Y>.md` obligatorio en todos los markdown explicativos |
| Más de cinco pasos para correr el sample | Fricción de adopción; el dev abandona antes de ver el resultado | Refactorizar a scripts de bootstrap o usar contenedores |
| Dependencias externas no documentadas | El sample falla en máquinas limpias y nadie sabe por qué | Prerequisites exhaustivos con versión mínima |
| Output esperado no documentado | El dev no sabe si su ejecución fue exitosa | §6 con output exacto o screenshot |
| Samples sólo en un OS sin justificar | Excluye a parte de la audiencia objetivo | Cubrir al menos los OS declarados en §17 P.9 del SOLUTION-INTAKE |
| Mezclar progresión por nivel y por dominio en un mismo proyecto | El lector pierde el hilo de lectura | Elegir una progresión (nivel o capacidad) y aplicarla consistentemente |

---

## 5. Preguntas guía para el subagente

### 5.1 Qué CU se ilustran

- ¿Qué CU de 02 cubre cada sample y por qué se eligió ese conjunto?
- ¿Hay algún CU crítico que no esté ilustrado por ningún sample?
- ¿Algún sample cubre más de un CU? ¿Está justificada la combinación o se podrían separar?
- ¿La progresión de samples respeta el orden natural de lectura de los CU?

### 5.2 Scope de cada sample

- ¿El sample es autocontenido o requiere servicios externos no triviales?
- ¿La cantidad de pasos para llegar a la primera ejecución exitosa es menor o igual a cinco?
- ¿El nivel declarado se condice con la complejidad real del código?
- ¿El sample demuestra al menos una capacidad que el anterior no demostraba?

### 5.3 Trazabilidad

- ¿Cada sample declara explícitamente los CU, ADR y NFR que ilustra en §8?
- ¿La trazabilidad apunta a artefactos vigentes (no a versiones jubiladas)?
- ¿Los puntos de extensión declarados en 05 tienen al menos un sample que los exhibe?

### 5.4 Calidad del sample

- ¿El código compila en un entorno limpio sin warnings nuevos?
- ¿El output esperado documentado coincide con la ejecución real?
- ¿Hay un test en `/samples/XX-<kebab>/tests/` que verifica el output esperado?
- ¿El sample sigue siendo reproducible bajo los OS declarados en §17 P.9 del SOLUTION-INTAKE?
- ¿La estructura del código respeta las convenciones de §16 del SOLUTION-INTAKE?

### 5.5 Mantenimiento

- ¿Existe un pipeline CI que compila y ejecuta los samples periódicamente?
- ¿Los samples se actualizan junto con cambios mayores del producto?
- ¿Las versiones de prerequisites están alineadas con §17 P.9 del SOLUTION-INTAKE?

---

## 6. Criterios de aceptación

- [ ] Existe `README.md` con tabla maestra de samples, columnas nivel, tiempo de setup, CU ilustrados, ubicación.
- [ ] Existen al menos los samples mínimos declarados en §2.2 para el tipo D8 del proyecto.
- [ ] Cada sample tiene su markdown explicativo `ejemplo-XX-<kebab-progresion>_v1.0.md` con las nueve secciones obligatorias.
- [ ] Cada sample es ejecutable con comandos copiables en entorno limpio en menos o igual a cinco pasos.
- [ ] Cada sample declara su nivel (básico, intermedio o avanzado) explícitamente en §2.
- [ ] Cada sample declara trazabilidad a CU, ADR o NFR en §8 con al menos una fila.
- [ ] Los nombres de archivo usan progresión por nivel o por capacidad, nunca por dominio del proyecto.
- [ ] Todos los markdown explicativos llevan sufijo `_v<X.Y>.md` (corrección obligatoria respecto al fuente, que omitía la versión).
- [ ] El README de la carpeta lista los samples en tabla con todas las columnas declaradas en §4.4.
- [ ] Cada sample declara tiempo de setup estimado en la tabla maestra del README.
- [ ] Cada sample documenta el output esperado en §6 con texto exacto o screenshot.
- [ ] Cada sample documenta prerequisites con versiones mínimas en §3.
- [ ] La estructura de `/samples/` del repositorio coincide con la matriz declarada en §2.3 para el tipo D8 del proyecto.
- [ ] Existe pipeline CI que valida que los samples compilan y ejecutan (recomendado fuerte; obligatorio para `library`, `rest-api` y `cli-tool`).

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo 1 — `ejemplo-02-intermedio-extensibilidad_v1.0.md` para una librería de parsing CSV

Fragmento ilustrativo, no documento completo:

```markdown
# Ejemplo 02 — Parser custom con extensión de tipo

**Proyecto:** {{nombre-proyecto}}
**Documento:** ejemplo-02-intermedio-extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-05-17
**Nivel:** Intermedio
**Ubicación del código:** `/samples/02-intermedio-con-extensiones/`

## 1. Objetivo del sample

Demostrar cómo registrar un parser custom en la librería para soportar un tipo de columna no incluido por defecto. Al finalizar, el desarrollador sabe implementar la interfaz `IColumnParser`, registrarla en la configuración y validar la salida con un dataset de prueba.

## 2. Nivel

Intermedio. Asume que el lector ya completó el ejemplo 01 (lectura básica de CSV). Agrega el punto de extensión principal de la librería.

## 3. Prerequisites

- Runtime objetivo, versión mínima declarada en §17 P.9 del SOLUTION-INTAKE.
- Gestor de paquetes del ecosistema.
- Editor con soporte para el lenguaje del proyecto.

## 4. Cómo correrlo

1. Clonar el repositorio.
2. Entrar a la carpeta del sample: `cd samples/02-intermedio-con-extensiones`.
3. Restaurar dependencias con el gestor de paquetes del ecosistema.
4. Ejecutar el comando de arranque del sample.
5. Inspeccionar la salida en consola.

## 5. Estructura del código

```
02-intermedio-con-extensiones/
├── README.md
├── src/
│   ├── Program.<ext>            # Punto de entrada
│   ├── CustomColumnParser.<ext> # Implementación del parser custom
│   └── data/sample.csv          # Dataset de entrada
└── tests/
    └── parser_custom_test.<ext> # Verifica el output esperado
```

## 6. Qué esperar

Salida esperada en consola:

```
Filas procesadas: 100
Tipo custom detectado en columna `monto_complejo`: 100 ocurrencias
Errores de parsing: 0
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Registrar dos parsers custom | Agregar segundo `IColumnParser` | El pipeline detecta y aplica el correcto por columna |
| Forzar error de parsing | Modificar una fila del CSV | El sample reporta error con línea y columna afectada |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-03 | Caso de uso | Implementa el flujo "registrar parser custom" |
| ADR-04 | Decisión arquitectónica | Materializa el patrón de extensión por interfaz |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial. |
```

### 7.2 Ejemplo 2 — `ejemplo-01-basico-cliente-http_v1.0.md` para una REST API de pagos

Fragmento ilustrativo, no documento completo:

```markdown
# Ejemplo 01 — Cliente HTTP básico con curl

**Proyecto:** {{nombre-proyecto}}
**Documento:** ejemplo-01-basico-cliente-http_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-05-17
**Nivel:** Básico
**Ubicación del código:** `/samples/01-cliente-http-basico/`

## 1. Objetivo del sample

Demostrar el camino feliz mínimo de la API: confirmar un pago con un POST autenticado y consultar su estado con un GET. Cubre el flujo end-to-end más simple que un integrador necesita para validar credenciales y conectividad.

## 2. Nivel

Básico. Punto de entrada absoluto. No requiere SDK, sólo curl y una clave de API válida.

## 3. Prerequisites

- curl (cualquier versión moderna).
- Una clave de API obtenida desde el portal del proyecto.
- Endpoint base del ambiente sandbox.

## 4. Cómo correrlo

1. Exportar la clave de API en una variable de entorno: `export API_KEY=<valor>`.
2. Ejecutar el script `./confirmar-pago.sh` desde la carpeta del sample.
3. Observar el `payment_id` en la respuesta JSON.
4. Ejecutar `./consultar-pago.sh <payment_id>` para ver el estado.
5. Comparar la respuesta con el output esperado de §6.

## 5. Estructura del código

```
01-cliente-http-basico/
├── README.md
├── confirmar-pago.sh         # POST /v1/payments
├── consultar-pago.sh         # GET /v1/payments/<id>
└── ejemplo-payload.json      # Payload de ejemplo del POST
```

## 6. Qué esperar

Respuesta del POST:

```json
{
  "payment_id": "pay_abc123",
  "estado": "confirmado",
  "monto": 1500.00,
  "moneda": "ARS"
}
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Reintentar con clave de idempotencia | Reenviar el mismo POST con header `Idempotency-Key` | No se duplica el pago |
| Forzar error de validación | Enviar monto negativo en el payload | Respuesta `problem+json` con código `400` |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-01 | Caso de uso | Confirma un pago válido end-to-end |
| CU-03 | Caso de uso | Consulta el estado de un pago confirmado |
| NFR-01 | Latencia p95 | El sample mide latencia con `curl -w` |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial. |
```

Los dos fragmentos son ilustrativos. Cada proyecto adapta el dominio respetando la estructura, la nomenclatura por progresión y el sufijo de versión obligatorio.

---

## 8. Prompt-snippet sugerido

```text
Sos un {{ESPECIALIDAD-VARIANTE-11}} responsable de redactar los markdown explicativos de los samples del proyecto {{NOMBRE_PROYECTO}} y de coordinar la materialización en código en /samples.

Insumos:
- SOLUTION-INTAKE: {{path}} (sección §13 tipo D8, §16 estructura de repo, §18 estrategia de samples)
- Upstream: 02 (CU que cada sample ilustra), 05 (arquitectura y puntos de extensión), 10 (developer guide).

A generar (según el `project_type` del proyecto, leído del manifiesto):
- README.md con tabla maestra de samples.
- ejemplo-XX-<kebab-progresion>_v1.0.md por cada sample (mínimo según §2.2 de 11_rules_examples.md).
- Carpeta /samples/XX-<kebab-progresion>/ con código ejecutable, README propio, tests de verificación.

Reglas de redacción: §4 de 11_rules_examples.md (nueve secciones obligatorias por markdown).
Nomenclatura: sufijo uniforme `_v<X.Y>.md` (corrección obligatoria del fuente, que omitía la versión). Progresión por nivel (basico/intermedio/avanzado) o por capacidad. Prohibido nombrar por dominio del proyecto (corrección obligatoria respecto al fuente: nada de `multa`, `multaapp-nuget`, `factura`, `recibo`).
Trazabilidad: cada sample referencia al menos un CU, ADR o NFR en §8.
Ejecutabilidad: cada sample arranca en menos o igual a cinco pasos en entorno limpio.
Output esperado: documentado en §6 con texto exacto o screenshot.
Estructura de /samples: respetar la matriz §2.3 de 11_rules_examples.md según tipo D8.
Criterios de calidad: §6 de 11_rules_examples.md.

Restricciones: no introducir productos comerciales ni protocolos del dominio fuente. Idioma rioplatense técnico, tildes correctas, sin emojis ni negritas decorativas.

Salida: /SDD2.1D/docs/proyectos/{{NOMBRE_PROYECTO_KEBAB}}/11_examples/<estructura> + /samples/<estructura> en el repo de la solución.
```

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Versión inicial de las reglas constructivas de la categoría 11. Define el README de la sección y los markdown explicativos por sample con sufijo uniforme `_v<X.Y>.md`, fija la matriz tipo D8 vs estructura de `/samples`, establece cantidades mínimas de samples por tipo, formaliza las nueve secciones obligatorias del markdown explicativo y corrige dos antecedentes del fuente SDD 1.0: la nomenclatura por dominio (`multa`, `multaapp-nuget`) se reemplaza por progresión de complejidad o capacidad, y la ausencia de sufijo de versión se reemplaza por `_v<X.Y>.md` obligatorio en todos los archivos versionables. |
| 1.1 | 2026-06-09 | Validación ST-06: la categoría se genera por proyecto bajo `proyectos/<nombre-proyecto-kebab>/11_examples/`; la frase de cierre de §1.2 y la ruta de salida del prompt-snippet referencian el `project_type` del proyecto en curso (manifiesto), y la referencia a la materialización de `/samples` apunta a §4.1 del PROJECT-README. Tablas §1.2 sin reescritura. |
| 1.2 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
