# Propuesta de modelo: solución más jerarquía de proyectos (ST-02)

| Campo | Valor |
|---|---|
| Archivo | `propuesta-modelo-solucion-jerarquia_v1.0.md` |
| Versión | 1.1 |
| Subtarea | ST-02 de la reformulación SDD 2.2D |
| Fecha | 2026-06-08 |
| Autor | AG-05 Arquitecto de Software Senior, con revisión de coherencia de AG-ROOT |
| Insumos | §3 y §3.1 del prompt de reformulación; `matriz-coherencia-template_v1.0.md` (ST-01) |
| Estado | APROBADA CON CAMBIOS (Opción B de intake) — habilita ST-03 |
| Invariante | El conjunto cerrado D8 sigue teniendo exactamente 8 valores. No se agregan ni se quitan tipos |

---

## §1 Propósito y estado

Esta es la propuesta del modelo objetivo que reemplaza el supuesto de un único `project_type` por repositorio por el de una solución que agrupa una jerarquía de proyectos tipados. No materializa ningún cambio sobre el template: define el modelo, el manifiesto, la topología, la convención de nombres, el caso degenerado y el mapa de impacto, para que el humano lo apruebe antes de que ST-03 y siguientes lo implementen.

Contenía tres decisiones que requerían confirmación explícita (consolidadas en §11), ya resueltas por el humano el 2026-06-08. La principal, la topología del intake (§6), se resolvió en la Opción B: un PROJECT-BRIEF y un PROJECT-README únicos a nivel solución, con el tipo por proyecto en el README §1 y un bloque técnico por proyecto. El resto del modelo deriva de §3 y §3.1 del prompt.

---

## §2 Modelo conceptual

### §2.1 Entidades

Solución. Contenedor raíz del entregable. Agrupa una jerarquía de proyectos. No tiene un valor D8 propio: su tipo es compuesto y se describe por el conjunto de proyectos que agrupa. Posee su propio README de solución y su propia vista de arquitectura de alto nivel (detallada en ST-05).

Proyecto. Nodo de la jerarquía con exactamente un valor D8. Es la unidad sobre la que el orquestador aplica la variante de especialidad de §1.2 de cada regla y filtra documentos por §2.1 y §2.2. Cada proyecto tiene un nombre de documentación (kebab) y un nombre de código (`/src`).

Jerarquía. Conjunto de proyectos y sus relaciones de composición y dependencia, representado como grafo dirigido acíclico (DAG). Define el orden de generación (topológico) y el orden de construcción (ST-07).

Proyecto principal. El nodo cabeza de la solución, equivalente al «tipo dominante» del intake actual de tipo único. Sigue existiendo como tal, pero los demás proyectos dejan de ser texto en §5 y pasan a ser nodos de primera clase con su propio tipado y su propia generación.

### §2.2 Invariantes del modelo

1. Cardinalidad: una solución tiene N proyectos, con N >= 1.
2. Tipado: cada proyecto lleva exactamente uno de los 8 valores D8. La solución no lleva D8.
3. Unicidad del principal: exactamente un proyecto es el principal.
4. Aciclicidad: el grafo de dependencias entre proyectos es un DAG. Un ciclo detiene la cadena.
5. Unicidad de nombres: dos planos de nombres sin colisión (ver §5). Dos proyectos no pueden compartir nombre de código ni nombre kebab.
6. Cierre D8: el conjunto de tipos sigue siendo cerrado y de 8 valores. La cardinalidad y la topología cambian; el conjunto no.
7. Degeneración: con N == 1, el modelo reproduce exactamente el comportamiento actual del template (ver §8).

---

## §3 Topología de la jerarquía

### §3.1 Tipos de relación

Entre proyectos se admiten dos relaciones, ambas dirigidas:

- Dependencia: el proyecto A depende de B si A consume artefactos de B en tiempo de compilación o de ejecución. Determina el orden de generación y de construcción: primero B, después A.
- Composición: relación de pertenencia conceptual (por ejemplo, un conjunto de microservicios que componen una plataforma). Para efectos de orden, la composición se modela como dependencia del compuesto hacia sus partes cuando corresponde, o como agrupación sin orden cuando las partes son independientes entre sí.

A los fines operativos, el orquestador solo necesita el grafo de dependencias para derivar el orden. La composición se documenta en la vista de solución (ST-05) pero no agrega aristas de orden salvo que implique dependencia real.

### §3.2 Orden topológico

El orquestador ordena los proyectos por dependencias: genera primero los proyectos sin dependencias, luego los que dependen solo de ya generados, y así sucesivamente. Los proyectos sin relación de dependencia entre sí pertenecen al mismo nivel topológico y pueden generarse en paralelo si el agente lo soporta.

### §3.3 Validaciones bloqueantes de la topología

- Ciclo detectado: el orquestador se detiene y reporta el ciclo con la lista de proyectos involucrados.
- Dependencia colgante: una dependencia que apunta a un proyecto que no existe en el manifiesto detiene la cadena.
- Más de un proyecto principal, o ninguno: detiene la cadena.
- `project_type` fuera del conjunto D8 en cualquier proyecto: detiene la cadena (igual que hoy en `master-prompt.md` §3).

---

## §4 Manifiesto de solución

### §4.1 Identidad

- Nombre: `SOLUTION-MANIFEST-<nombre-solucion-kebab>_v1.0.md`.
- Ubicación: `/SDD2.2D/devs/intake/`.
- Rol: declara la jerarquía. Es el primer insumo que el orquestador lee, antes de cualquier intake de proyecto. Es la fuente única de verdad de la enumeración de proyectos, su tipado, sus dependencias y sus nombres de código.
- Formato: markdown con tablas, coherente con el estilo de `PROJECT-README-template.md`. Un bloque de metadatos a nivel solución y una tabla de proyectos. Se elige markdown (no YAML puro) por consistencia con el resto del intake y por legibilidad humana; el orquestador parsea las tablas.

### §4.2 Esquema a nivel solución

| Campo | Descripción |
|---|---|
| Nombre de solución | Nombre legible de la solución |
| `nombre-solucion-kebab` | Slug derivado por el algoritmo de §5.3 (docs y archivos) |
| `NombreSolucionCodigo` | Forma PascalCase del nombre de solución (raíz de los nombres de código) |
| Proyecto principal | `nombre-proyecto-kebab` del nodo cabeza |
| Perfil de convención de nombres | Forma PascalCase, separador de segmentos (por defecto `.`) y prefijo de redistribuibles (por defecto `Aplicada`) |
| Intake de negocio | Referencia al `PROJECT-BRIEF-<nombre-solucion-kebab>_v1.0.md` (uno por solución) |
| Intake técnico | Referencia al `PROJECT-README-<nombre-solucion-kebab>_v1.0.md` (uno por solución, con bloque técnico por proyecto) |

### §4.3 Esquema por proyecto (una fila por proyecto)

| Campo | Descripción | Validación |
|---|---|---|
| `nombre-proyecto-kebab` | Nombre del proyecto para docs y archivos | kebab-case D3, único en la solución |
| `nombre-proyecto-codigo` | Nombre del proyecto en `/src` | `<NombreSolucionCodigo>.<Sufijo>` o `Aplicada.<X>` si redistribuible; único |
| `project_type` | Valor D8 del proyecto | pertenece al conjunto cerrado D8 |
| Rol en la solución | Una frase: qué aporta el proyecto a la solución | no vacío |
| `redistribuible` | `true` o `false` | booleano |
| Dependencias | Lista de `nombre-proyecto-kebab` de los que depende | referencian proyectos del manifiesto; no forman ciclo |
| Path en `/src` | Ruta de la carpeta del proyecto | coherente con `nombre-proyecto-codigo` |

El BRIEF y el README de solución se referencian una sola vez en el bloque de §4.2, no por fila. Cada proyecto tiene su entrada técnica en el bloque de proyecto del README de solución (decisión Opción B de §6), no un intake propio.

### §4.4 Ejemplo de manifiesto (solución multi-proyecto)

Bloque a nivel solución:

| Campo | Valor |
|---|---|
| Nombre de solución | Gestión de Turnos |
| `nombre-solucion-kebab` | `gestion-de-turnos` |
| `NombreSolucionCodigo` | `GestionDeTurnos` |
| Proyecto principal | `gestion-de-turnos-api` |
| Perfil de convención | PascalCase; separador `.`; prefijo de redistribuibles `Aplicada` |

Tabla de proyectos:

| `nombre-proyecto-kebab` | `nombre-proyecto-codigo` | `project_type` | Rol | `redistribuible` | Dependencias | Path `/src` |
|---|---|---|---|---|---|---|
| `gestion-de-turnos-api` | `GestionDeTurnos.WebApi` | `rest-api` | API pública de turnos (principal) | false | `gestion-de-turnos-domain`, `aplicada-validaciones` | `src/GestionDeTurnos.WebApi/` |
| `gestion-de-turnos-domain` | `GestionDeTurnos.Domain` | `library` | Dominio y reglas de negocio compartidas | false | `aplicada-validaciones` | `src/GestionDeTurnos.Domain/` |
| `gestion-de-turnos-notificaciones` | `GestionDeTurnos.Worker` | `worker-service` | Envío asincrónico de recordatorios | false | `gestion-de-turnos-domain` | `src/GestionDeTurnos.Worker/` |
| `aplicada-validaciones` | `Aplicada.Validaciones` | `library` | Paquete reusable de validaciones | true | — | `src/Aplicada.Validaciones/` |

Grafo de dependencias derivado (DAG):

```text
aplicada-validaciones  ->  gestion-de-turnos-domain  ->  gestion-de-turnos-api
                       \                              \-> gestion-de-turnos-notificaciones
                        \-> gestion-de-turnos-api
```

Orden topológico de generación y construcción:

```text
nivel 0: aplicada-validaciones
nivel 1: gestion-de-turnos-domain
nivel 2: gestion-de-turnos-api, gestion-de-turnos-notificaciones   (paralelizables)
```

### §4.5 Validaciones del manifiesto

El orquestador, al leer el manifiesto, verifica en orden: tipos D8 válidos; un único proyecto principal; nombres kebab y de código únicos; dependencias resueltas; grafo acíclico. Cualquier falla detiene la cadena y se reporta con el patrón de ambigüedad de §9 del `master-prompt.md`.

---

## §5 Convención de nombres (formalización de §3.1)

### §5.1 Dos planos que no se mezclan

Plano de documentación (`/docs`, `/devs`). Sin cambios respecto de hoy: D3 intacto. kebab-case lowercase, sin acentos ni eñes en el nombre de archivo, sufijo `_v<X.Y>.md`. De aquí salen `nombre-solucion-kebab` y, por proyecto, `nombre-proyecto-kebab`. Los identificadores de artefactos (`NB-XX`, `CU-XX`, `ADR-XX`, etc.) no cambian.

Plano de código (`/src`). Sigue la convención idiomática del ecosistema, anclada en el nombre de la solución. Se formaliza como regla parametrizable, sin nombrar ningún framework ni gestor de paquetes (conformidad D7).

### §5.2 Regla de nombres de código

1. El nombre de cada proyecto de código se forma como `<NombreSolucionCodigo>.<Sufijo>`, donde `<NombreSolucionCodigo>` es la forma PascalCase del nombre de la solución y `<Sufijo>` identifica el rol del proyecto.
2. Excepción para paquetes redistribuibles. Si `redistribuible: true`, el nombre arranca con el prefijo de organización `Aplicada` en lugar del nombre de la solución (por ejemplo `Aplicada.Validaciones`). El motivo: un paquete reusable necesita un espacio de nombres estable e independiente de la solución que lo consume.
3. El `<Sufijo>` orienta su valor según el `project_type` D8 y el rol. Mapa orientativo, no cerrado:

| `project_type` | Sufijo orientativo |
|---|---|
| `rest-api` | `.WebApi` o `.Api` |
| `web-monolith` | `.Web` |
| `worker-service` | `.Worker` |
| `cli-tool` | `.Cli` |
| `desktop-app` | `.Ui` o `.Desktop` |
| `mobile-app-maui` | `.Mobile` |
| `library` | `.Core`, `.Abstractions`, `.Domain`, `.Infrastructure` u otro rol |
| `web-microservices` | un proyecto por servicio bajo `<NombreSolucionCodigo>.Services.<Servicio>` más `.Gateway` y `.BuildingBlocks` |

### §5.3 Algoritmo de derivación (a cargo del orquestador)

`nombre-solucion-kebab` y `nombre-proyecto-kebab` se derivan con el algoritmo de normalización del `master-prompt.md` §3: lowercase, espacios a guion medio, acentos a su equivalente sin acento, eliminación de caracteres no ascii/dígito/guion, colapso de guiones, recorte de guiones extremos.

`NombreSolucionCodigo` se obtiene en PascalCase del mismo nombre legible: separar por espacios, capitalizar inicial de cada palabra, concatenar sin separadores.

`nombre-proyecto-codigo` se compone por la regla §5.2 a partir de `NombreSolucionCodigo`, el sufijo y la bandera redistribuible.

### §5.4 Detección de colisión

Si dos proyectos derivan el mismo `nombre-proyecto-codigo` o el mismo `nombre-proyecto-kebab`, el orquestador se detiene y lo reporta como ambigüedad, pidiendo desambiguar el sufijo o el nombre.

### §5.5 Conformidad D7

La regla se expresa de forma agnóstica de stack a propósito: no nombra ningún framework ni gestor de paquetes, porque están prohibidos por D7 en los artefactos normativos. El perfil de convención del manifiesto es el lugar donde una solución concreta materializa la convención de su ecosistema sin contaminar el texto normativo. El token `Aplicada` se admite por ser un prefijo de organización, no vocabulario de stack.

---

## §6 Topología del intake — DECISIÓN CLAVE

El template actual tiene dos intake por repositorio: PROJECT-BRIEF (negocio) y PROJECT-README (técnico). Al pasar a N proyectos hay que decidir cómo se reparten. El prompt de reformulación, en su §3, menciona «su par de intake (PROJECT-BRIEF y PROJECT-README del proyecto)», lo que admite lectura literal de un par por proyecto; pero el BRIEF es, por naturaleza, un documento de negocio del cliente, y el cliente quiere una solución, no N negocios independientes.

Opciones:

- Opción A (recomendada). Un PROJECT-BRIEF a nivel solución (el problema de negocio es uno) más un PROJECT-README por proyecto (las decisiones técnicas son por proyecto), más el manifiesto que los vincula. El BRIEF de solución alimenta a 00_contexto y 01_necesidades_negocio de toda la solución; cada README gobierna las categorías técnicas de su proyecto.
- Opción B. Un PROJECT-BRIEF y un PROJECT-README a nivel solución, donde el README §1 enumera el tipo por proyecto en una tabla, sin README por proyecto. Más simple, pero mezcla decisiones técnicas heterogéneas (una API, una librería, un worker) en un solo documento y rompe la unidad de especialización por proyecto.
- Opción C (lectura literal del prompt). Un par BRIEF + README por proyecto, más el manifiesto. Maximiza separación pero duplica el negocio: cada proyecto repetiría el problema, los stakeholders y las métricas del cliente, que son de la solución, no del proyecto.

Decisión adoptada (humano, 2026-06-08): Opción B. Un PROJECT-BRIEF y un PROJECT-README únicos a nivel solución, más el manifiesto. El README §1 enumera el tipo por proyecto en una tabla; no hay README por proyecto. Las decisiones técnicas que hoy ocupan §2 a §16 del README pasan a expresarse por proyecto dentro del mismo documento (un bloque técnico por proyecto), porque stack, arquitectura, persistencia, testing y pipeline son por proyecto.

Reconciliación con D-B (fuente única de verdad). Para no abrir dos fuentes de verdad entre el manifiesto y la tabla del README §1, rige esta separación de responsabilidades:

- El manifiesto es canónico para la enumeración de proyectos, su `project_type`, sus dependencias y sus nombres de código. El orquestador lee de ahí la lista de proyectos y sus D8, y de ahí deriva el orden topológico y el despacho por proyecto.
- El README §1 refleja esa enumeración (mirror) y agrega, por proyecto, la justificación técnica del tipo y del rol. No introduce proyectos nuevos ni dependencias que no estén en el manifiesto. Si el README §1 y el manifiesto divergen en la lista de proyectos o en un `project_type`, el orquestador se detiene y reporta la divergencia; el manifiesto manda.

Preservación de la unidad de especialización. Aunque el README sea un único documento, la especialización por proyecto se conserva en la orquestación: el orquestador toma el D8 de cada proyecto desde el manifiesto, aplica la variante §1.2 correspondiente y filtra documentos por §2.1 y §2.2 para ese proyecto, alimentándose del bloque técnico de ese proyecto en el README. El documento es un contenedor de entrada; la unidad de generación sigue siendo el proyecto.

Implicancia sobre el manifiesto: el BRIEF y el README se referencian una sola vez a nivel solución (campos del bloque de solución). Las filas de proyecto no referencian un intake propio; su entrada técnica vive en el bloque del proyecto dentro del README de solución.

Implicancia sobre `PROJECT-README` (ST-03): se reestructura para ser un documento de solución, con un encabezado de solución, un §1 con la tabla de proyectos y su D8 (mirror del manifiesto), y un bloque técnico repetible por proyecto que condensa las secciones §2 a §16 actuales. ST-03 diseña ese layout en detalle.

---

## §7 Aplicación de variantes y matriz de adaptabilidad por proyecto

Las tablas §1.2 de las 13 reglas ya están indexadas por D8 y no se reescriben. Lo que cambia es la cardinalidad de su invocación: el orquestador las invoca una vez por proyecto, con el D8 de ese proyecto.

La matriz de adaptabilidad del `master-prompt.md` §14 no cambia de contenido: cambia su ámbito. Hoy se aplica una vez contra el `project_type` del repositorio. Tras la reformulación se aplica una vez por cada proyecto del manifiesto, contra su D8.

Bucle del orquestador reformulado (ST-04 lo materializa):

1. Leer el manifiesto y validar (tipos, principal único, nombres únicos, dependencias, aciclicidad).
2. Derivar `nombre-solucion-kebab`, `NombreSolucionCodigo` y, por proyecto, `nombre-proyecto-kebab` y `nombre-proyecto-codigo`.
3. Ordenar los proyectos por orden topológico.
4. Para cada proyecto, en orden: seleccionar la variante §1.2 de cada regla según su D8, filtrar documentos por §2.1 y §2.2, despachar subagentes con el contexto del proyecto (su D8 y su nombre de código), auditar por fase.
5. Los proyectos del mismo nivel topológico pueden generarse en paralelo si el agente lo soporta.
6. Cerrar con la vista de solución (§9) y el README de solución, por encima de los proyectos.

---

## §8 Caso degenerado y compatibilidad hacia atrás

Una solución de un solo proyecto es el caso degenerado y la garantía de no ruptura.

- Manifiesto trivial: un único proyecto, que es el principal, sin dependencias. `NombreSolucionCodigo` igual al nombre del proyecto si se desea.
- Intake: con la Opción B adoptada, el par BRIEF + README de solución coincide con el intake actual; el README de solución tiene un solo bloque técnico (el del único proyecto) y un §1 con una sola fila. No hay documentos adicionales más allá del manifiesto trivial.
- Generación: el bucle de §7 recorre un solo proyecto, lo que equivale a la ejecución actual del `master-prompt.md` contra un único `project_type`.
- Resultado: la salida `/SDD2.2D/docs/` es idéntica a la que produce el template hoy para ese tipo. ST-09 verificará esta equivalencia como prueba de no regresión contra la línea base de ST-01.

En consecuencia, los proyectos existentes que hoy usan el template no se rompen: pasan a ser soluciones de un proyecto sin cambiar su salida.

---

## §9 Vista de solución (anticipo de ST-05)

Por encima de la arquitectura de cada proyecto, la solución gana una vista propia, que ST-05 materializa:

- Mapa de proyectos: qué proyectos componen la solución, su D8 y su rol.
- Contratos inter-proyecto: qué expone cada proyecto a los que dependen de él.
- Grafo de dependencias: el DAG del manifiesto, como vista navegable.
- README de solución: punto de entrada que presenta la solución, la jerarquía y la tabla de proyectos (ST-08 lo reformula a nivel README raíz).

Las variantes §1.2 por D8 se siguen aplicando por proyecto dentro de su propia arquitectura; la vista de solución es un nivel adicional, no un reemplazo.

---

## §10 Mapa de impacto a las subtareas siguientes

| Subtarea | Qué materializa de este modelo |
|---|---|
| ST-03 | `SOLUTION-MANIFEST-template.md` (§4); reestructurar `PROJECT-README-template.md` a documento de solución: encabezado de solución, §1 con tabla de proyectos y su D8 (mirror del manifiesto), y bloque técnico repetible por proyecto (condensa §2 a §16 actuales); §5 deriva la estructura de la jerarquía y de la convención de nombres (§5); `PROJECT-BRIEF` se mantiene a nivel solución con ajuste mínimo para nombrar la solución |
| ST-04 | `master-prompt.md`: §3 lee el manifiesto y deriva la lista de proyectos tipados y sus nombres de código; §6 plan por proyecto; §7 bucle topológico; §8 inyecta contexto de proyecto; §14 matriz por proyecto |
| ST-05 | Vista de solución (§9) por encima de `05_rules_arquitectura_tecnica.md`; contratos inter-proyecto y grafo de dependencias |
| ST-06 | Validar que las §1.2 de las 13 reglas se aplican por proyecto sin reescritura; ajuste mínimo del texto que asuma tipo único por repositorio |
| ST-07 | `09_rules_devops.md`: build y publicación multi-proyecto en orden topológico; artefactos publicables por proyecto |
| ST-08 | README raíz: presenta la solución, la jerarquía y la tabla de proyectos con su D8, rol y dependencias |
| ST-09 | Re-evaluar coherencia; verificar D1–D8, trazabilidad a nivel solución y proyecto, y el caso degenerado contra la línea base de ST-01 |

---

## §11 Decisiones — resueltas (humano, 2026-06-08)

D-A (principal). Topología del intake: RESUELTA en Opción B. Un PROJECT-BRIEF y un PROJECT-README únicos a nivel solución, más el manifiesto. El README §1 enumera el tipo por proyecto; las secciones técnicas §2 a §16 se vuelven un bloque por proyecto dentro del mismo README. Detalle y reconciliación con D-B en §6.

D-B. Fuente única de verdad del tipado: CONFIRMADA. El manifiesto es canónico para la enumeración de proyectos, sus tipos, dependencias y nombres de código. El README §1 refleja esa enumeración y agrega justificación técnica, sin introducir proyectos nuevos; ante divergencia, manda el manifiesto.

D-C. Ubicación del workspace: CONFIRMADA. Los entregables de la reformulación viven en `/SDD2.2D/devs/_reformulacion/` (espejo de `_bootstrap/`). El `SOLUTION-MANIFEST-template.md` que produce ST-03 va a `/SDD2.2D/devs/intake/` como indica el prompt.

Con las tres decisiones resueltas, la propuesta queda aprobada con cambios (Opción B en lugar de la Opción A recomendada) y habilita la planificación de ST-03.

---

## §12 Criterios de aceptación de la propuesta

- [ ] El conjunto cerrado D8 sigue teniendo exactamente 8 valores.
- [ ] El modelo define solución, proyecto, jerarquía y proyecto principal con invariantes verificables (§2).
- [ ] La topología es un DAG con orden topológico y validaciones bloqueantes de ciclo y colisión (§3).
- [ ] El manifiesto declara, por proyecto, nombre de código, sufijo implícito en el código, bandera redistribuible y dependencias; y a nivel solución, el perfil de convención (§4).
- [ ] La convención de nombres separa el plano de documentación (D3 intacto) del plano de código (`<NombreSolucionCodigo>.<Sufijo>` con excepción `Aplicada`), de forma agnóstica de stack (§5).
- [ ] El caso degenerado de un proyecto reproduce el comportamiento actual (§8).
- [ ] Las decisiones que requieren confirmación humana están explicitadas (§11).
- [ ] No aparecen emojis, negritas decorativas ni vocabulario del dominio fuente del bootstrap.

---

## §13 Glosario delta (términos nuevos respecto del template actual)

| Término | Definición operativa |
|---|---|
| Solución | Contenedor raíz que agrupa una jerarquía de proyectos. No tiene D8 propio |
| Proyecto | Nodo de la jerarquía con exactamente un valor D8. Unidad de especialización |
| Manifiesto de solución | Artefacto de intake que enumera proyectos, su D8, rol, dependencias y nombres de código. Primer insumo del orquestador y fuente única de verdad de la enumeración |
| Nombre de solución | Nombre legible del que se derivan `nombre-solucion-kebab` (docs) y `NombreSolucionCodigo` (código) |
| Nombre de proyecto de código | Identificador en `/src`, `<NombreSolucionCodigo>.<Sufijo>`, salvo redistribuibles |
| Paquete redistribuible | Proyecto `redistribuible: true`, cuyo nombre de código arranca con `Aplicada` |
| Perfil de convención de nombres | Configuración del manifiesto que fija PascalCase, separador y prefijo de redistribuibles |
| Orden topológico | Secuencia de generación y construcción que respeta dependencias: primero las dependencias, después los dependientes |
| Caso degenerado | Solución de un único proyecto. Reproduce el comportamiento actual del template |

---

## §14 Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-08 | Propuesta inicial del modelo solución más jerarquía: entidades e invariantes, topología DAG y orden topológico, esquema del manifiesto con ejemplo multi-proyecto, formalización de la convención de nombres en dos planos, decisión clave de topología del intake con recomendación, caso degenerado y compatibilidad, vista de solución y mapa de impacto a ST-03 en adelante. | AG-05 con revisión de AG-ROOT |
| 1.1 | 2026-06-08 | Resolución de las decisiones humanas: intake en Opción B (un BRIEF y un README únicos a nivel solución, tipo por proyecto en §1, bloque técnico por proyecto), con reconciliación de la fuente única de verdad (manifiesto canónico, README §1 mirror). Confirmadas D-B y D-C. Ajustados §4.2, §4.3, §6, §8, §10 y §11. | AG-05 con revisión de AG-ROOT |
