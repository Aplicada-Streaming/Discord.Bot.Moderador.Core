# Reglas constructivas — README raíz del proyecto

**Carpeta target:** `/SDD2.1D/docs/`
**Archivo target:** `/SDD2.1D/docs/README.md`
**Subagente target del orquestador:** Arquitecto de Soluciones Senior (AG-ROOT)
**Versión de las reglas:** 1.3

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Arquitecto de Soluciones Senior, equivalente al AG-ROOT del catálogo de especialidades. Su rol es garantizar la coherencia integral de la solución desde una perspectiva sistémica, asegurando que el README raíz funcione como punto de entrada efectivo, narrativa técnica ejecutiva, presentación de la jerarquía de proyectos y mapa navegable hacia la documentación de cada proyecto y de la solución. Su responsabilidad no es producir contenido de detalle (eso corresponde a los AG-00 a AG-11), sino integrar, vincular y validar la coherencia transversal entre los proyectos de la solución. Su alcance abarca: definición de la estructura documental, redacción del README maestro, presentación de la tabla de proyectos con su tipo D8 y dependencias, validación de enlaces internos, diseño del flujo de lectura por audiencia y alineación entre la visión de la solución y la organización de las carpetas.

### 1.2 Variantes según tipo de proyecto

| Tipo de proyecto (D8) | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Arquitecto de Soluciones + Curador de Librería | El README debe priorizar instalación, API pública, ejemplos de consumo y compatibilidad semántica. |
| web-monolith | Arquitecto de Soluciones Senior | Estructura clásica; el énfasis está en flujos funcionales y onboarding general. |
| web-microservices | Arquitecto de Soluciones + Arquitecto Distribuido | Requiere mapa de servicios, diagrama de despliegue y matriz de contratos entre componentes. |
| desktop-app | Arquitecto de Soluciones + Especialista Cross-Platform | Debe declarar compatibilidad por sistema operativo, empaquetado y dependencias nativas. |
| mobile-app-maui | Arquitecto de Soluciones + Mobile Lead | Foco en plataformas objetivo, ciclo de publicación en tiendas y permisos del dispositivo. |
| rest-api | Arquitecto de Soluciones + API Designer | Énfasis en quick-start de consumo, autenticación, versionado de endpoints y referencia OpenAPI. |
| cli-tool | Arquitecto de Soluciones + CLI Designer | El README es la primera ayuda visible; debe contener instalación, comandos y ejemplos de uso. |
| worker-service | Arquitecto de Soluciones + Streaming/Event Engineer | Requiere descripción del modelo de eventos, fuentes, sinks y reintentos. |

El orquestador lee esta tabla y selecciona la variante según el `project_type` del proyecto principal de la solución (leído del manifiesto), porque el README raíz se genera una vez a nivel solución.

### 1.3 Multi-especialidad

Cuando el README raíz necesita atender audiencias mixtas, se admite combinar AG-ROOT con el Technical Writer (AG-10) para refinar la narrativa orientada a desarrollador externo, y con el Analista de Negocio (AG-01) para validar que la propuesta de valor expuesta en la sección de identidad coincide con la visión declarada en `/SDD2.1D/docs/00_contexto/`. En proyectos `library` y `cli-tool` se recomienda incorporar a AG-11 (Developer Advocate) para curar el bloque de quick-start. La regla es: AG-ROOT mantiene la propiedad del documento; las multi-especialidades aportan revisión y enmienda, no autoría compartida.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra de documentos

| Archivo | Obligatorio para | Recomendado para | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `/SDD2.1D/docs/README.md` | Todos los tipos D8 | — | — | Punto de entrada de la documentación del proyecto y ancla del árbol SDD. |
| `/SDD2.1D/docs/CHANGELOG.md` | library, rest-api, cli-tool | web-monolith, web-microservices, worker-service, desktop-app, mobile-app-maui | — | Bitácora de cambios con relevancia para consumidores externos. |
| `/SDD2.1D/docs/CONTRIBUTING.md` | library, cli-tool | rest-api, worker-service | web-monolith | Guía de contribución cuando el proyecto admite aportes externos. |
| `/SDD2.1D/docs/LICENSE.md` | library, cli-tool | rest-api | — | Texto de licencia visible desde el árbol de documentación. |

### 2.2 Reglas de inclusión/exclusión por tipo de proyecto

El README raíz es siempre obligatorio. Lo que varía es la presencia de bloques internos según el tipo:

- `library`: incluye sección de instalación, API pública y compatibilidad de versiones.
- `web-monolith`: incluye flujos de negocio principales y enlace a la guía de despliegue.
- `web-microservices`: incluye listado de servicios y diagrama de interacción.
- `desktop-app`: incluye matriz de sistemas operativos soportados y empaquetadores.
- `mobile-app-maui`: incluye plataformas objetivo, versiones mínimas y ciclo de publicación.
- `rest-api`: incluye quick-start con `curl`, autenticación y referencia al contrato.
- `cli-tool`: incluye instalación, comandos principales y ejemplos.
- `worker-service`: incluye fuentes y sinks de eventos, política de reintentos y observabilidad.

Los archivos `CHANGELOG.md`, `CONTRIBUTING.md` y `LICENSE.md` se incluyen en `/SDD2.1D/docs/` solo cuando el proyecto requiere comunicación con consumidores externos al equipo.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

El archivo es `README.md` literal, sin versión en el nombre. El versionado vive en la cabecera del documento mediante el campo `Versión` y se actualiza siguiendo la regla D5 (inicio en v1.0). Los archivos satélite mencionados en §2.1 también van en mayúsculas convencionales: `CHANGELOG.md`, `CONTRIBUTING.md`, `LICENSE.md`.

### 3.2 Convenciones de prefijos / sufijos

El propio README raíz no usa prefijos. Para los archivos linkeados desde el README, se respetan los patrones canónicos de las 12 categorías:

- `NB-XX-<kebab>_v<X.Y>.md` (necesidades de negocio).
- `CU-XX-<kebab>_v<X.Y>.md` (casos de uso).
- `RN-XX-<kebab>_v<X.Y>.md` (reglas de negocio).
- `ADR-XX-<kebab>_v<X.Y>.md` (decisiones de arquitectura).
- `US-XX-<kebab>_v<X.Y>.md` (historias de usuario).
- `BT-XX-<kebab>_v<X.Y>.md` (backlog técnico).
- `sprint-XX-<kebab>_v<X.Y>.md` (planes de sprint).
- `ejemplo-XX-<kebab>_v<X.Y>.md` (ejemplos progresivos).

Todos los nombres respetan kebab-case estricto en minúsculas (D3) y sufijo de versión con guion bajo (D4).

### 3.3 Vinculación cross-doc (trazabilidad upstream/downstream)

- Upstream: el README raíz consume `SOLUTION-MANIFEST` y `SOLUTION-INTAKE` producidos en la fase de intake. De allí extrae el nombre de la solución, la propuesta de valor, la enumeración de proyectos con su tipo D8 y dependencias, y los stacks declarados.
- Downstream: el README raíz enlaza a las categorías de nivel solución (`00_contexto`, `01_necesidades_negocio`), a la vista y el pipeline de solución en `_solucion/`, y a la documentación de cada proyecto bajo `proyectos/<nombre-proyecto-kebab>/`. No enlaza directamente a artefactos internos; eso lo hace el README de cada sección o de cada proyecto.

### 3.4 README de la sección

No aplica. Este archivo es el README de la raíz de `/SDD2.1D/docs/`. Los README de sección (uno por cada carpeta numerada) son responsabilidad de los respectivos AG-00 a AG-11 y se rigen por su propio archivo de reglas.

---

## 4. Estructura de redacción del documento

### 4.1 Cabecera obligatoria

La cabecera del `README.md` generado debe seguir este bloque, completando los valores entre llaves dobles a partir de SOLUTION-INTAKE:

```markdown
# {{nombre-solucion}}

| Campo | Valor |
| --- | --- |
| Solución | {{nombre-solucion}} |
| Versión del documento | 1.0 |
| Estado | Borrador / Propuesto / Aprobado / Vigente / Superado / Archivado |
| Fecha | YYYY-MM-DD |
| Stack principal | {{stack-declarado}} |
| Composición | {{N}} proyectos (ver tabla de proyectos) |
| Proyecto principal | {{nombre-proyecto-principal}} |
| Documento | README raíz de la solución |
```

Nota: el README raíz, por ser el ancla del árbol, no declara un bloque "Trazabilidad upstream/downstream" en su cabecera. Esa trazabilidad se materializa en el cuerpo del documento generado: la tabla de proyectos (sección 2), el mapa de la documentación (sección 4) con las categorías de nivel solución (00, 01), la vista y el pipeline de solución (`_solucion/`), y la documentación de cada proyecto bajo `proyectos/<nombre-proyecto-kebab>/`.

### 4.2 Secciones obligatorias

El README generado debe contener, como mínimo, las siguientes secciones en este orden:

1. Identidad de la solución: propósito en 2 a 3 párrafos, propuesta de valor, audiencia objetivo.
2. Proyectos de la solución: tabla con cada proyecto (`nombre-proyecto-kebab`, tipo D8, rol, dependencias, bandera redistribuible), con el proyecto principal señalado. Refleja el `SOLUTION-MANIFEST`.
3. Stack y composición: tabla con el stack de cada proyecto y las plataformas soportadas.
4. Mapa de la documentación: las categorías de nivel solución (`00_contexto`, `01_necesidades_negocio`), la vista y el pipeline de solución en `_solucion/`, y la documentación de cada proyecto bajo `proyectos/<nombre-proyecto-kebab>/`, cada una con descripción de propósito y enlace.
5. Flujo de lectura recomendado por audiencia: al menos 3 audiencias diferenciadas, con orden de lectura sugerido y justificación.
6. Cómo contribuir y cómo regenerar la documentación: enlace a `CONTRIBUTING.md` si aplica y proceso de regeneración con los subagentes SDD.
7. Estado actual y roadmap: tabla de estado por proyecto y por categoría, y enlace al roadmap detallado en `00_contexto`.
8. Glosario rápido: mínimo 10 términos del dominio de la solución, breves, sin reemplazar el glosario completo de la categoría UX/UI.
9. Contacto y responsables: tabla con rol, responsable y canal de comunicación.
10. Control de cambios: tabla con versión, fecha y descripción del cambio.

### 4.3 Secciones opcionales según tipo de proyecto

| Sección | Aplica a | Notas |
| --- | --- | --- |
| Diagrama de despliegue | web-microservices, worker-service | Incluir vista de servicios, colas, almacenamiento y red. |
| Compatibilidad de plataformas | mobile-app-maui, desktop-app, cli-tool | Tabla con sistema operativo, versión mínima y observaciones. |
| Cómo consumir como dependencia | library | Comando de instalación, importación mínima y ejemplo de 5 líneas. |
| Quick-start | rest-api, cli-tool | Bloque ejecutable con 3 a 5 comandos para validar el camino feliz. |
| Modelo de eventos | worker-service | Listar fuentes, sinks, formato de payload y política de reintentos. |
| Política de versionado y soporte | library, rest-api | Tabla con versiones vigentes, fin de soporte y ruta de migración. |

### 4.4 Tablas tipo y formatos recurrentes

El documento debe usar las siguientes tablas estandarizadas:

Tabla de proyectos de la solución (refleja el `SOLUTION-MANIFEST`).

| Proyecto | Tipo D8 | Rol | Dependencias | Redistribuible |
| --- | --- | --- | --- | --- |
| <nombre-proyecto-kebab> (principal) | rest-api | API pública de la solución | <nombre-proyecto-kebab> | false |
| <nombre-proyecto-kebab> | library | Dominio compartido | — | false |

Tabla A: Mapa de documentación.

| Sección | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto (solución) | Visión, alcance, roadmap del negocio | AG-00 | [00_contexto](00_contexto/) |
| 01_necesidades_negocio (solución) | Necesidades de negocio | AG-01 | [01_necesidades_negocio](01_necesidades_negocio/) |
| _solucion (solución) | Vista de solución y pipeline de solución | AG-05, AG-09 | [_solucion](_solucion/) |
| proyectos/<nombre-proyecto-kebab> (por proyecto) | Documentación 02 a 11 del proyecto | AG-02 a AG-11 | [proyectos/<nombre-proyecto-kebab>](proyectos/<nombre-proyecto-kebab>/) |

Tabla B: Flujo de lectura por audiencia.

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Product Manager | 00 → 01 → 06 → 07 | Necesita entender visión, necesidades y backlog. |
| Desarrollador | 00 → 02 → 05 → 10 → 11 | Necesita contexto, especificación, arquitectura y ejemplos. |
| QA | 00 → 02 → 08 | Necesita ver requisitos y estrategia de pruebas. |
| DevOps | 00 → 05 → 09 | Necesita arquitectura y pipeline. |

Tabla C: Estado actual.

| Categoría | Estado | Versión vigente |
| --- | --- | --- |
| 00_contexto | Vigente | 1.0 |
| 01_necesidades_negocio | Borrador | 0.9 |

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| README sin tabla de proyectos ni enlaces a la documentación de cada proyecto | Rompe la navegación SDD y oculta la jerarquía de la solución | Incluir la tabla de proyectos y la Tabla A con las categorías de solución y un enlace a la carpeta de cada proyecto. |
| Stack mencionado sin versión | Imposible reproducir entornos y validar compatibilidad | Declarar siempre `tecnología @ versión` en la cabecera y en §2. |
| Flujo de lectura único sin variantes por rol | Cada audiencia se pierde en información no relevante | Producir mínimo 3 flujos por rol en Tabla B. |
| README como wiki extensa | Duplica contenido de las categorías y se desactualiza primero | Mantener el README en 200 a 400 líneas y delegar el detalle a cada categoría. |
| Roadmap inline en el README | Genera dos fuentes de verdad sobre el roadmap | Enlazar a `00_contexto/roadmap-producto_v1.0.md` y no replicar contenido. |
| Glosario que reemplaza al de UX/UI | El glosario rápido se convierte en glosario completo y diverge | Limitar a 10 a 20 términos esenciales y enlazar al glosario de la categoría UX/UI. |
| Estado libre fuera del enum | Estados ambiguos como "casi listo" o "WIP" | Usar exclusivamente: Borrador, Propuesto, Aprobado, Vigente, Superado, Archivado. |

---

## 5. Preguntas guía para el subagente

### 5.1 Comprensión del input upstream

- ¿Cuál es el nombre canónico de la solución en kebab-case y cuáles son los proyectos que la componen según el `SOLUTION-MANIFEST`?
- ¿Cuál es el proyecto principal y qué variante de §1.2 corresponde a su tipo D8? ¿Qué tipo D8 lleva cada proyecto?
- ¿Cuáles son las dependencias entre proyectos y los stacks con versiones y plataformas objetivo de cada uno?
- ¿Cuál es la propuesta de valor de la solución en una sola línea y en un párrafo?
- ¿Qué audiencias se han identificado en el intake como prioritarias?

### 5.2 Decisiones de scope

- ¿Qué contenido es propio del README raíz de la solución y qué debe quedar en la documentación de cada proyecto o categoría?
- ¿Se incluyen secciones opcionales de §4.3 según los tipos D8 presentes en la solución?
- ¿Qué archivos satélite de §2.1 acompañan al README en esta solución?
- ¿El README aporta valor en cada bloque o algún bloque está duplicando la documentación de proyectos o categorías?

### 5.3 Trazabilidad

- ¿La tabla de proyectos y las categorías de nivel solución están enlazadas con un párrafo breve de propósito, y cada proyecto enlaza a su carpeta `proyectos/<nombre-proyecto-kebab>/`?
- ¿Los enlaces apuntan a rutas existentes en `/SDD2.1D/docs/`?
- ¿La cadena Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline está visible al menos como referencia conceptual en §3?
- ¿El roadmap del README es un enlace y no una copia?

### 5.4 Calidad

- ¿La longitud final está entre 200 y 400 líneas?
- ¿Se respetó D1 (español rioplatense neutro, sin emojis, sin negritas decorativas)?
- ¿Las tablas tienen filas completas sin valores "TBD" ni placeholders sin cerrar?
- ¿El glosario tiene al menos 10 términos del dominio?
- ¿Cada audiencia de §4.4 Tabla B tiene un orden de lectura justificado?
- ¿Se evitaron los anti-patrones de §4.5?
- ¿Se respetó el enum cerrado de estados?
- ¿Se respetó D7 evitando ejemplos prohibidos en el contenido?

---

## 6. Criterios de aceptación del entregable

- [ ] La tabla de proyectos de la solución está presente con, por cada proyecto, su tipo D8, rol y dependencias, señala el proyecto principal y refleja el `SOLUTION-MANIFEST` sin divergencias.
- [ ] El mapa de la documentación (Tabla A) enlaza las categorías de nivel solución (00, 01), la vista y el pipeline de solución (`_solucion/`) y la carpeta de cada proyecto (`proyectos/<nombre-proyecto-kebab>/`), con su path correcto.
- [ ] La composición de la solución (número de proyectos y proyecto principal) está reflejada en la cabecera.
- [ ] El flujo de lectura está diferenciado para al menos 3 audiencias en la Tabla B, con justificación por rol.
- [ ] El glosario rápido tiene mínimo 10 términos del dominio del proyecto, definidos en una línea cada uno.
- [ ] Todos los enlaces internos del README apuntan a rutas que existen en `/SDD2.1D/docs/`; no hay enlaces rotos.
- [ ] La cabecera respeta el bloque obligatorio de §4.1 con todos los campos completos.
- [ ] El documento tiene entre 200 y 400 líneas en su versión final.
- [ ] No aparecen emojis, negritas decorativas, ni términos del dominio prohibido por D7.
- [ ] El control de cambios al pie del documento tiene al menos una entrada inicial v1.0.
- [ ] El estado declarado en la cabecera pertenece al enum cerrado: Borrador, Propuesto, Aprobado, Vigente, Superado o Archivado.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo A: Solución multi-proyecto de gestión de turnos

```markdown
# gestion-de-turnos

| Campo | Valor |
| --- | --- |
| Solución | gestion-de-turnos |
| Versión del documento | 1.0 |
| Estado | Vigente |
| Fecha | 2026-03-10 |
| Stack principal | C#/.NET, PostgreSQL 16 |
| Composición | 4 proyectos (ver tabla de proyectos) |
| Proyecto principal | gestion-de-turnos-api |
| Documento | README raíz de la solución |

## 1. Identidad de la solución

Solución para la gestión de turnos médicos en centros de salud de mediana escala.
Expone una API de turnos, comparte un dominio común, envía recordatorios de forma
asincrónica y reutiliza un paquete de validaciones independiente de la solución.

## 2. Proyectos de la solución

| Proyecto | Tipo D8 | Rol | Dependencias | Redistribuible |
| --- | --- | --- | --- | --- |
| gestion-de-turnos-api (principal) | rest-api | API pública de turnos | gestion-de-turnos-domain, aplicada-validaciones | false |
| gestion-de-turnos-domain | library | Dominio y reglas compartidas | aplicada-validaciones | false |
| gestion-de-turnos-notificaciones | worker-service | Recordatorios asincrónicos | gestion-de-turnos-domain | false |
| aplicada-validaciones | library | Validaciones reusables | — | true |

## 4. Mapa de la documentación

| Sección | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto | Visión, alcance, roadmap | AG-00 | [00_contexto](00_contexto/) |
| _solucion | Vista y pipeline de solución | AG-05, AG-09 | [_solucion](_solucion/) |
| proyectos/gestion-de-turnos-api | Documentación de la API | AG-02 a AG-11 | [api](proyectos/gestion-de-turnos-api/) |
```

### 7.2 Ejemplo B: Solución de un proyecto (caso degenerado), librería de parsing CSV

```markdown
# csv-parser-lib

| Campo | Valor |
| --- | --- |
| Solución | csv-parser-lib |
| Versión del documento | 1.0 |
| Estado | Vigente |
| Fecha | 2026-04-22 |
| Stack principal | TypeScript 5.5, Node 20 |
| Composición | 1 proyecto (caso degenerado) |
| Proyecto principal | csv-parser-lib |
| Documento | README raíz de la solución |

## 1. Identidad de la solución

Librería liviana para parseo y validación de archivos CSV con soporte de
streaming, inferencia de tipos opcional y reporte estructurado de errores.
Pensada para integrarse en pipelines de ingesta de datos.

## 2. Proyectos de la solución

| Proyecto | Tipo D8 | Rol | Dependencias | Redistribuible |
| --- | --- | --- | --- | --- |
| csv-parser-lib (principal) | library | Librería de parseo (única) | — | false |

## 3. Cómo consumir como dependencia

Instalación mínima desde el registro de paquetes del lenguaje, importación
del módulo y ejemplo de 5 líneas que parsea un archivo y devuelve filas.

## 3. Mapa de la documentación

| Categoría | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 02_especificacion_funcional | Contrato de la API pública | AG-02 | [02_especificacion_funcional](02_especificacion_funcional/) |
| 10_developer_guide | Guía de integración | AG-10 | [10_developer_guide](10_developer_guide/) |
| 11_examples | Ejemplos progresivos | AG-11 | [11_examples](11_examples/) |

## 4. Flujo de lectura recomendado

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Desarrollador consumidor | 11 → 10 → 02 | Empezar por ejemplos y luego ver el contrato |
| Mantenedor de la librería | 00 → 05 → 06 → 09 | Arquitectura, backlog y pipeline |
| QA | 02 → 08 | Validar contrato y matriz de pruebas |
```

---

## 8. Prompt-snippet sugerido para el subagente

```text
Sos un {{ESPECIALIDAD-VARIANTE}} (Arquitecto de Soluciones Senior más la variante D8 del proyecto principal) responsable de redactar el README raíz de la solución {{NOMBRE_SOLUCION}}.

Insumos:
- SOLUTION-MANIFEST: {{path}} (enumeración de proyectos, tipo D8, rol, dependencias, nombres de código).
- SOLUTION-INTAKE: {{path}}
- Documentos upstream ya generados: las categorías de solución (00, 01), la vista y el pipeline de solución (`_solucion/`) y la documentación de cada proyecto (`proyectos/<kebab>/`).

Reglas de redacción: §4 de _root_rules.md.
Trazabilidad esperada: presentar la tabla de proyectos (D8, rol, dependencias) y enlazar las categorías de solución y la carpeta de cada proyecto con descripción de propósito.
Criterios de calidad: §6 de _root_rules.md.
Restricciones: respetar D1 a D8; no incluir emojis, negritas decorativas, ni términos del dominio prohibido por D7.

Salida: /SDD2.1D/docs/README.md (sin versión en el nombre, con versión 1.0 en la cabecera).
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.1 |
| 1.1 | 2026-06-09 | Validación ST-06: el README raíz se genera a nivel solución; §1.2 usa la variante del proyecto principal del manifiesto. El README presenta la solución y la tabla de proyectos (la reformulación de contenido se completa en ST-08). |
| 1.2 | 2026-06-09 | Reformulación ST-08: el README raíz se reformula a documento de solución. La cabecera (§4.1) declara la solución, su composición y el proyecto principal en lugar de un único tipo D8. Se agrega la sección obligatoria "Proyectos de la solución" (§4.2) con la tabla de proyectos (D8, rol, dependencias, redistribuible). El mapa de documentación (§4.4) refleja las categorías de solución (00, 01), `_solucion/` y la carpeta de cada proyecto. Se actualizan §1.1, §3.3, anti-patrones, criterios de aceptación, preguntas guía, ejemplos (uno multi-proyecto y el caso degenerado) y el prompt-snippet. |
| 1.3 | 2026-06-10 | Migración de referencias de intake al documento unificado SOLUTION-INTAKE (unificación de intake). |
