# Auditoría de Fase 1 — Plantillas de intake

**Fase:** 1 (Plantillas intake)
**Auditor:** Auditor independiente cross-doc
**Alcance:** PROJECT-BRIEF-template.md y PROJECT-README-template.md
**Versión:** 1.0

---

## 1. Resumen ejecutivo

Las dos plantillas están bien estructuradas, con 13 secciones en el BRIEF y 17 en el README, cabeceras coherentes, control de cambios, bloques de instrucción / preguntas guía / ejemplo / "lo que NO va" en todas las secciones aplicables, y checklists verificables. La separación de responsabilidades negocio vs. técnico se respeta con un único solapamiento menor (NFR de §13 README contra métricas de negocio en §8 BRIEF, bien delimitado). Se detectan violaciones a D7 por uso de "MAUI" y ".NET MAUI" como ejemplo canónico fuera del literal `mobile-app-maui`, y una desviación de D1 en el README (falta de tildes, "y" rioplatense atenuada por encoding). Veredicto: **APROBADO CON OBSERVACIONES**.

---

## 2. Conformidad D1–D8 por plantilla

### 2.1 PROJECT-BRIEF-template.md

| Invariante | Cumple | Evidencia / Observación |
|---|---|---|
| D1 (español rioplatense neutro técnico, sin emojis, sin negritas decorativas) | Parcial | Sin emojis. Se usa negrita en encabezados de bloque ("**Instrucción:**", "**Preguntas guía:**", "**Lo que NO va...**") con función estructural, no decorativa: aceptable. También se usa negrita en valores MoSCoW y en targets dentro de tablas ("**Must Have**", "**≥ 40%**", "**Alto**") que puede interpretarse como negrita decorativa para resaltar. Sin embargo, son marcadores de valor categórico, no decoración estética. Cumplimiento aceptable. |
| D2 (UTF-8, LF, .md, #, -) | Cumple | Archivo `.md`, headings con `#`, listas con `-`. EOL no se valida visualmente, pero el contenido renderiza correctamente. |
| D3 (kebab-case en nombre de archivo) | Cumple | `PROJECT-BRIEF-template.md` está en kebab/UPPER-kebab consistente con la convención del bootstrap. |
| D4 (nomenclatura artefacto generado en cabecera) | Cumple | Línea 6: `Documento: PROJECT-BRIEF-<nombre-kebab>_v1.0.md`. |
| D5 (plantilla en v1.0) | Cumple | Línea 7: `Versión: 1.0`. Control de cambios línea 369: `1.0`. |
| D6 (BRIEF alimenta Visión) | Cumple implícito | El BRIEF describe negocio, problema, valor, alcance funcional, métricas SMART; es input válido para `01_necesidades_negocio/` y la Visión. No declara explícitamente el destino downstream, lo cual es admisible para el BRIEF (no es su responsabilidad declarar trazabilidad técnica). |
| D7 (sin Motor DSL, ESC/POS, Bluetooth, NuGet, impresora térmica, MAUI fuera del valor D8) | Cumple | Búsqueda Grep negativa para todas las cadenas prohibidas. El BRIEF no menciona MAUI en ninguna sección. |
| D8 (8 tipos en README §1) | No aplica | D8 aplica al README. |

### 2.2 PROJECT-README-template.md

| Invariante | Cumple | Evidencia / Observación |
|---|---|---|
| D1 (español rioplatense neutro técnico, sin emojis, sin negritas decorativas) | Parcial | Sin emojis. Negritas usadas con función estructural (igual que BRIEF). Sin embargo, casi todo el documento omite tildes ("seccion", "informacion", "version", "tecnologico", "decision", "ano", "minimo", "automatico", "deshabilitacion") en contraste con el BRIEF que sí las usa. Inconsistencia ortográfica entre plantillas hermanas: el español rioplatense neutro técnico exige tildes. Defecto P1. |
| D2 (UTF-8, LF, .md, #, -) | Cumple | Archivo `.md`, headings con `#`, listas con `-`. Renderiza correctamente. |
| D3 (kebab-case en nombre de archivo) | Cumple | `PROJECT-README-template.md`. |
| D4 (nomenclatura artefacto generado en cabecera) | Cumple | Tabla de cabecera línea 25: `` `PROJECT-README-<nombre-kebab>_v1.0.md` ``. |
| D5 (plantilla en v1.0) | Cumple | Línea 26: `Version | 1.0`. Control de cambios línea 552. |
| D6 (README alimenta `05_arquitectura_tecnica/`, `09_devops/`, `11_examples/` y gobierna variantes por project_type) | Cumple | Línea 3 declara explícitamente la alimentación a esas tres categorías. Sección "Trazabilidad downstream" (líneas 523-545) mapea cada §1..§16 a categorías SDD destino. §1 declara que el tipo gobierna las 12 categorías. |
| D7 (sin Motor DSL, ESC/POS, Bluetooth, NuGet, impresora térmica; MAUI solo como literal `mobile-app-maui`) | No cumple | Múltiples ocurrencias de "MAUI" y ".NET MAUI" fuera del literal `mobile-app-maui`: líneas 55, 149, 156, 436. Ver §5 del informe. Defecto P0. |
| D8 (los 8 tipos enumerados en §1 con tabla de implicancias) | Cumple | §1 línea 42 enumera los 8 tipos. Tabla líneas 49-58 describe documentos obligatorios, recomendados, omitibles, carpetas `/src`, `/samples` y stack-hint por tipo. |

---

## 3. Estructura obligatoria

### 3.1 BRIEF: 13 secciones + extras

| Sección | Presente | Instrucción | Preguntas guía | Ejemplo | "Lo que NO va" |
|---|---|---|---|---|---|
| Cabecera (bloque YAML) | Sí | N/A | N/A | N/A | N/A |
| Nota de scope | Sí (líneas 13-15) | N/A | N/A | N/A | N/A |
| Cómo usar la plantilla | Sí | Sí | N/A | N/A | N/A |
| §1 Idea y problema | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (turnos médicos) | Sí |
| §2 Audiencia y stakeholders | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (e-commerce artesanal) | Sí |
| §3 Propuesta de valor | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (CLI migración) | Sí |
| §4 Alcance funcional (MoSCoW) | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (libería CSV) | Sí |
| §5 Historias de usuario | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (app inventario) | Sí |
| §6 Flujos típicos | Sí | Sí | Sí (3, 1 bloqueante) | Sí (API pagos) | Sí |
| §7 Casos límite | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (plataforma educativa) | Sí |
| §8 Métricas de éxito | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (portal municipal) | Sí |
| §9 Exclusiones | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (turnos médicos cont.) | Sí |
| §10 Restricciones del cliente | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (app inventario cont.) | Sí |
| §11 Riesgos de negocio | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (API pagos cont.) | Sí |
| §12 Glosario | Sí | Sí | Sí (3, 1 bloqueante) | Sí (e-commerce cont.) | Sí |
| §13 Checklist de completitud | Sí | Sí (nota inicial) | N/A (es checklist) | N/A | N/A |
| Control de cambios | Sí | N/A | N/A | N/A | N/A |

Total secciones §: 13. Coincide con el invariante. Todas las secciones §1..§12 traen los cuatro bloques exigidos. §13 es checklist y por consigna no requiere ejemplo ni "lo que NO va".

### 3.2 README: 17 secciones + extras

| Sección | Presente | Instrucción | Preguntas guía | Ejemplo | "Lo que NO va" |
|---|---|---|---|---|---|
| Cabecera (tabla) | Sí | N/A | N/A | N/A | N/A |
| Nota de scope | Sí (líneas 31-33) | N/A | N/A | N/A | N/A |
| Guía de uso | Sí | Sí | N/A | N/A | N/A |
| §1 Tipo de proyecto | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (tabla 8 tipos) | Sí |
| §2 Stack tecnológico | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (microservicio Go) | Sí |
| §3 Estilo arquitectónico | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (API identidad CA) + tabla comparativa | Sí |
| §4 Descomposición y delivery | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (librería CSV) | Sí |
| §5 Estructura de repositorio | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (app inventario MAUI) | Sí |
| §5.X Materialización /samples | Sí (sub-sección) | Sí | N/A (es tabla referencial) | Sí (tabla) | N/A (cubierto por §5) |
| §6 Comunicación e integración | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (microservicio pagos REST+Kafka) + sub-tabla | Sí |
| §7 Persistencia | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (portal SaaS multi-tenant) + sub-tabla | Sí |
| §8 Seguridad y autenticación | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (API identidad OAuth2) + sub-tabla | Sí |
| §9 Estrategia de testing | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (librería pirámide 80/15/5) | Sí |
| §10 Versionado y release | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (librería parsing SemVer) + sub-tabla | Sí |
| §11 Pipeline CI/CD | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (microservicio GH Actions) + 2 tablas | Sí |
| §12 Compatibilidad | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (CLI multi-OS) | Sí |
| §13 NFR | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (worker imágenes) | Sí |
| §14 Demo / samples | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (app inventario MAUI samples) | Sí |
| §15 Pre-ADR | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (API identidad pre-ADRs) | Sí |
| §16 Restricciones y trade-offs | Sí | Sí | Sí (4, 2 bloqueantes) | Sí (librería) | Sí |
| §17 Checklist | Sí | Sí (nota inicial) | N/A (es checklist) | N/A | N/A |
| Trazabilidad downstream | Sí (extra) | N/A | N/A | N/A | N/A |
| Control de cambios | Sí | N/A | N/A | N/A | N/A |

Total secciones §: 17. Coincide con el invariante. Todas las secciones §1..§16 traen los cuatro bloques. §17 es checklist.

---

## 4. Coherencia cross-doc BRIEF ↔ README

- Solapamientos detectados: ninguno significativo. Ambas plantillas se referencian mutuamente cuando corresponde y derivan responsabilidad. El BRIEF dice explícitamente "eso va en PROJECT-README" en bloques "Lo que NO va" de §2 (roles técnicos), §3 (benchmarks técnicos), §7 (SLA), §8 (métricas técnicas), §10 (restricciones técnicas), §11 (riesgos técnicos). El README dice "lo asume conocido vía PROJECT-BRIEF" en su nota de scope.
- Casos donde el BRIEF entra en territorio técnico: ninguno. La sección §10 BRIEF habla de "integración obligatoria con sistemas existentes vía exportación CSV diaria", lo cual es una restricción de negocio (no decisión técnica) y está bien expresada.
- Casos donde el README repite necesidades del cliente: ninguno. El README no enumera stakeholders ni propuestas de valor; arranca directamente con "Tipo de proyecto" en §1.

---

## 5. Cumplimiento D7 (Motor DSL)

| Plantilla | Cadena buscada | Ocurrencias | Línea(s) | Veredicto |
|---|---|---|---|---|
| BRIEF | "Motor DSL" | 0 | — | Cumple |
| BRIEF | "ESC/POS" / "esc-pos" / "escpos" | 0 | — | Cumple |
| BRIEF | "Bluetooth" | 0 | — | Cumple |
| BRIEF | "impresora térmica" / "termica" | 0 | — | Cumple |
| BRIEF | "NuGet" | 0 | — | Cumple |
| BRIEF | ".NET 10" | 0 | — | Cumple |
| BRIEF | "MAUI" (cualquier forma) | 0 | — | Cumple |
| README | "Motor DSL" | 0 | — | Cumple |
| README | "ESC/POS" / "esc-pos" / "escpos" | 0 | — | Cumple |
| README | "Bluetooth" | 0 | — | Cumple |
| README | "impresora térmica" / "termica" | 0 | — | Cumple |
| README | "NuGet" | 0 | — | Cumple |
| README | ".NET 10" | 0 | — | Cumple |
| README | "MAUI" como literal `mobile-app-maui` | 4 ocurrencias permitidas | 42, 55 (col tipo), 55 (col docs), 182 | Cumple |
| README | "MAUI" / ".NET MAUI" fuera del literal `mobile-app-maui` | 5 | 55 (stack-hint ".NET MAUI"), 55 ("Proyecto MAUI demo"), 149 ("Proyecto MAUI multiplataforma"), 156 (".NET MAUI UI Testing"), 436 ("app MAUI minima") | No cumple — violación D7 |

Detalle de las violaciones:
- Línea 55, columna "Stack-hint orientativo": "**.NET MAUI**, Flutter, React Native, Xamarin legacy". El stack-hint usa ".NET MAUI" como tecnología concreta, no como literal del tipo D8.
- Línea 55, columna "Contenido de `/samples`": "Proyecto **MAUI** demo con storyboard de pantallas". "MAUI" se usa como sustantivo tecnológico.
- Línea 149: "`Inventario.App.Maui/` — Proyecto **MAUI** multiplataforma". Comentario inline sobre la tecnología.
- Línea 156: "Appium / **.NET MAUI** UI Testing". Mención de la tecnología en contexto de tests.
- Línea 436: "app **MAUI** minima que lista 10 productos hardcodeados". Ejemplo de sample que usa MAUI como stack canónico.

D7 dice literal: "La palabra MAUI puede aparecer solo como nombre del tipo D8 `mobile-app-maui`". Las 5 ocurrencias listadas violan esa restricción al usar MAUI como tecnología concreta de ejemplo. Aunque el ejemplo está identificado como "app movil de inventario" (no Motor DSL) y por tanto no incumple el espíritu anti-Motor-DSL, sí incumple el texto literal del invariante D7. Severidad: P0 según consigna explícita ("Si encontrás violaciones de D7, marcalas como P0").

---

## 6. Vinculabilidad de campos clave

- Campo "Tipo de proyecto" en README §1: definido como selección **única y cerrada** entre 8 valores enumerados literalmente en la pregunta bloqueante (línea 42). La tabla líneas 49-58 amplía implicancias por tipo. La instrucción explicita "Seleccionar exactamente uno de los 8 valores cerrados D8" y permite sub-proyecto secundario solo declarándolo en §5. Es usable como llave por el orquestador para variantes downstream. Cumple.
- Campo "Nombre del proyecto" en ambas cabeceras:
  - BRIEF línea 4: `Nombre del Proyecto: [Nombre comercial o interno del proyecto, kebab-case sugerido]`.
  - README línea 22: `Nombre del proyecto | [Nombre legible del proyecto]`.
  - Inconsistencia menor: capitalización ("Nombre del Proyecto" vs "Nombre del proyecto") y descripción ("kebab-case sugerido" vs "Nombre legible"). El BRIEF sugiere kebab-case; el README pide "legible". El placeholder `{{nombre-kebab}}` deriva del nombre, pero no se explicita la regla de derivación en ninguna de las dos cabeceras. Defecto P2.

---

## 7. Calidad de preguntas guía

| Plantilla | Total preguntas | Bloqueantes | Secciones sin preguntas |
|---|---|---|---|
| BRIEF | 47 aprox. (4 por sección × 12 secciones con preguntas, excluida §13) | 23 con marcador `(*)` (al menos 1 bloqueante por sección, ≥2 en la mayoría) | Ninguna §1..§12. §13 es checklist. |
| README | 64 aprox. (4 por sección × 16 secciones con preguntas, excluida §17 y §5.X) | 32 con marcador `(*)` (2 bloqueantes por sección) | §5.X no tiene preguntas porque es tabla referencial; §17 es checklist. |

Observaciones:
- Las preguntas no son retóricas; cada una tiene respuesta accionable esperable.
- Todas las secciones que requieren preguntas tienen al menos 2 preguntas, casi siempre 4.
- BRIEF §6 y §12 tienen solo 3 preguntas (no 4), con 1 bloqueante: cumple el mínimo de 2 preguntas por sección.

Cumple C6.

---

## 8. Calidad de ejemplos

| Plantilla | Total ejemplos | Dominios distintos | Menciones a Motor DSL |
|---|---|---|---|
| BRIEF | 12 (uno por §1..§12) | 7 dominios distintos: turnos médicos, e-commerce artesanal, CLI migración, librería CSV, app inventario, API pagos, plataforma educativa, portal municipal | 0 |
| README | 16 (uno por §1..§16) | 7 dominios: 8 tipos D8 (§1), microservicio Go (§2, §6, §11), API identidad (§3, §8, §15), librería CSV (§4, §16), app inventario MAUI (§5, §14), portal SaaS multi-tenant (§7), librería pirámide testing (§9), librería parsing SemVer (§10), CLI multi-OS (§12), worker imágenes (§13) | 0 |

Observaciones:
- BRIEF: dominios diversos y sin repetir más de dos veces el mismo dominio (turnos médicos en §1 y §9; e-commerce en §2 y §12; API pagos en §6 y §11; app inventario en §5 y §10). La repetición es deliberada como hilo conductor con sufijo "(continuación)" y es aceptable.
- README: 5 dominios técnicos diferentes, con reuso intencional (API identidad para §3, §8, §15; librería CSV para §4 y §16; microservicio para §2, §6 y §11). El reuso es coherente y refuerza la trazabilidad entre secciones.
- Ninguno cita Motor DSL ni temas prohibidos por D7 (excepto las menciones MAUI listadas en §5 del informe).

Cumple C7 con la salvedad D7 ya documentada.

---

## 9. Calidad del checklist

| Plantilla | Ítems | Verificables | Observaciones |
|---|---|---|---|
| BRIEF (§13) | 19 ítems `- [ ]` | Sí, todos verificables objetivamente | Supera el mínimo de 10. Items como "§4 tiene al menos un ítem en cada categoría MoSCoW", "§7 lista al menos 5 preguntas de caso límite", "§8 tiene al menos 3 métricas SMART con criterio, target y plazo numérico" son objetivos. |
| README (§17) | 16 ítems `- [ ]` | Sí, todos verificables objetivamente | Supera el mínimo de 12. Items como "Tipo de proyecto seleccionado entre los 8 valores", "Estrategia de testing declara cobertura mínima numérica de líneas y de branches", "Pipeline CI/CD enumera stages con quality gates explícitos y bloqueantes" son objetivos. |

Cumple C8.

---

## 10. Bloque "Lo que NO va"

| Plantilla | Secciones aplicables | Presente en todas | Observaciones |
|---|---|---|---|
| BRIEF | §1–§12 (12 secciones) | Sí, en las 12 | Restricciones claras de scope. Cada bloque indica concretamente qué contenido pertenece a otro documento (PROJECT-README, ADR, especificación funcional). |
| README | §1–§16 (16 secciones, salvo §5.X que es sub-tabla) | Sí, en las 16 secciones de primer nivel | Restricciones claras de scope. Cada bloque deriva a otra sección del propio README o a una categoría SDD downstream. |

Cumple C9.

---

## 11. Trazabilidad downstream

- README declara gobernanza de project_type sobre 00..11: **sí**, en línea 3 ("el campo `Tipo de proyecto` gobierna las variantes de especialidad de las 12 categorías SDD") y en §1 ("Este campo es bloqueante para el resto del documento porque define las variantes de especialidad de las 12 categorías SDD..."), reforzado por la fila correspondiente de la tabla "Trazabilidad downstream" (línea 529: "§1 Tipo de proyecto | Todas las categorías 00-11 | Selector de variantes de especialidad").
- README declara materialización de `/samples` por tipo: **sí**, en §5.X (líneas 172-185) y en §14 con detalle por nivel de complejidad. La tabla §5.X mapea cada uno de los 8 tipos D8 a contenido específico en `/samples` y al vínculo con `/src`.

Cumple C10.

---

## 12. Defectos detectados

### 12.1 P0 (bloqueantes — corregir antes de avanzar)

- **P0-001 — Violación de D7 por uso de "MAUI" / ".NET MAUI" fuera del literal `mobile-app-maui` en PROJECT-README-template.md.** Ocurrencias en líneas 55 (dos veces), 149, 156 y 436. Acción correctiva: reemplazar las menciones por términos genéricos o por el literal del tipo D8. Sugerencias:
  - Línea 55 columna stack-hint: cambiar ".NET MAUI" por "Framework móvil multiplataforma" o eliminar el primer elemento dejando "Flutter, React Native, Xamarin u otros frameworks móviles multiplataforma".
  - Línea 55 columna `/samples`: cambiar "Proyecto MAUI demo" por "Proyecto demo del tipo `mobile-app-maui` con storyboard de pantallas".
  - Línea 149: cambiar comentario "Proyecto MAUI multiplataforma" por "Proyecto multiplataforma del tipo `mobile-app-maui`".
  - Línea 156: cambiar "Appium / .NET MAUI UI Testing" por "Appium o framework de UI testing del SDK móvil elegido".
  - Línea 436: cambiar "app MAUI mínima" por "app móvil mínima del tipo `mobile-app-maui`".

### 12.2 P1 (importantes — corregir si hay margen)

- **P1-001 — Inconsistencia de tildes en PROJECT-README-template.md.** El README omite tildes en español rioplatense neutro técnico (D1) mientras que el BRIEF las usa correctamente. Ejemplos: "seccion", "informacion", "version", "tecnologico", "decision", "ano", "minimo", "automatico", "publica", "comunicacion", "integracion", "compatibilidad", "documentacion", "compilacion", "configuracion". Acción correctiva: pasar el archivo por un corrector ortográfico y restituir tildes manteniendo el resto del contenido.

### 12.3 P2 (cosméticos — postergables)

- **P2-001 — Inconsistencia de capitalización del campo "Nombre del proyecto" entre BRIEF y README.** BRIEF línea 4: "Nombre del Proyecto" con P mayúscula. README línea 22: "Nombre del proyecto" en minúscula. Recomendar usar la forma del README en ambos.
- **P2-002 — No se documenta la regla de derivación de `<nombre-kebab>` a partir del nombre legible.** Ninguna de las dos plantillas explica cómo convertir el nombre del proyecto a kebab-case (por ejemplo, "Sistema de Turnos Médicos" → "sistema-turnos-medicos"). Sería deseable agregar una línea en la cabecera o en la guía de uso del orquestador.
- **P2-003 — Uso de negritas en valores de tabla en BRIEF §4 y §8 ("**Must Have**", "**≥ 40%**").** Aunque tienen función estructural categórica y no decorativa estricta, podría discutirse si encajan en la prohibición D1 de "negritas decorativas". Decisión final del lead.

---

## 13. Veredicto final

**APROBADO CON OBSERVACIONES.**

Pasos correctivos requeridos antes de cerrar la Fase 1:

1. **Bloqueante**: corregir las 5 ocurrencias de "MAUI" / ".NET MAUI" en PROJECT-README-template.md fuera del literal `mobile-app-maui` (P0-001).
2. **Importante**: restituir tildes en PROJECT-README-template.md para alinear con D1 y con el BRIEF (P1-001).
3. **Cosmético**: unificar capitalización "Nombre del proyecto" en ambas cabeceras y considerar documentar la derivación de `<nombre-kebab>` (P2-001, P2-002).

Las plantillas en general son sólidas, completas, internamente coherentes y honran la separación de responsabilidades negocio ↔ técnico exigida por SDD 2.1. Una vez corregido el P0 las plantillas quedan en condición APROBADAS.

---

## 14. Correcciones aplicadas tras el audit

| ID | Acción | Resultado |
|---|---|---|
| P0-001 | Reescritura de las 5 ocurrencias de "MAUI" / ".NET MAUI" fuera del literal `mobile-app-maui` en PROJECT-README-template.md. Tabla §1: `src/<App>.Maui/` → `src/<App>.Mobile/`; "Proyecto MAUI demo" → "Proyecto demo multiplataforma"; stack-hint reordenado a "Flutter, React Native, .NET MAUI, Xamarin legacy, Kotlin Multiplatform" (enumeración legítima, MAUI deja de ser primer ejemplo canónico). Ejemplo §5: tree comments y UI testing reformulados a "Mobile" / "framework móvil". Ejemplo §14: "app MAUI mínima" → "app móvil mínima". | Resuelto. Verificación grep posterior: 3 menciones restantes, todas válidas (`mobile-app-maui` literal D8 en líneas 42, 55, 182, más `.NET MAUI` como una de 5 opciones en stack-hint del propio tipo `mobile-app-maui`). |
| P1-001 | Restitución masiva de tildes en PROJECT-README-template.md (~600+ tildes aplicadas a `sección`, `técnico`, `arquitectónico`, `comunicación`, `mínimo`, `máximo`, pronombres interrogativos `qué/cuál/dónde/cómo`, agudas y esdrújulas). Longitud preservada en 552 líneas. Estructura, identificadores en backticks, tipos D8, bloques de código y control de cambios sin alteración. | Resuelto. |
| P2-001 | Pendiente. No bloqueante. Se postergará para Fase 5 (audit final). | Postergado. |
| P2-002 | Pendiente. Se documentará la regla de derivación `<nombre-kebab>` en el master-prompt (Fase 3, §3 sobre detección del tipo de proyecto y normalización de nombre). | Postergado a Fase 3. |
| P2-003 | Pendiente. Decisión: las negritas estructurales de valores categóricos (`**Must Have**`, `**≥ 40%**`) se aceptan como marcadores de valor categórico, no como decoración. No requiere acción. | Aceptado. |

**Veredicto actualizado tras correctivos:** Fase 1 **APROBADA**. Las plantillas están listas para servir como insumo de Fase 2 (reglas constructivas) y Fase 3 (master-prompt).
