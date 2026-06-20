# Auditoría de Fase 2 — 13 reglas constructivas

**Fase:** 2 (Reglas constructivas)
**Auditor:** Auditor independiente cross-doc
**Alcance:** 13 archivos de `/SDD2.1D/devs/rules/`
**Fecha:** 2026-05-17
**Versión del informe:** 1.0

---

## 1. Resumen ejecutivo

Las 13 reglas constructivas de la Fase 2 del bootstrap SDD 2.1 están redactadas con consistencia metodológica, respetan la cadena de trazabilidad D6 declarada y aplican explícitamente las lecciones del audit-sdd1.md (Fase 0). La conformidad con D1–D8 es alta y la coherencia cross-doc entre las 13 reglas es satisfactoria. Se detectan pocos defectos, todos de severidad baja o media: una mención de `.NET MAUI` que cae dentro de la excepción admitida (mobile-app-maui), referencias formales (no canónicas) al fuente Motor DSL como justificación de correcciones heredadas y dos archivos (`_root_rules`, `00_rules_contexto`) con cabecera ligeramente incompleta (sin §0 explícito). Total: 0 P0, 5 P1, 6 P2.

Veredicto: APROBADO CON OBSERVACIONES.

---

## 2. Conformidad D1–D8 (matriz)

| Archivo | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| _root_rules | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 00_rules_contexto | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 01_rules_necesidades_negocio | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 02_rules_especificacion_funcional | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 03_rules_ux_ui_dx | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 04_rules_prompts_ai | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 05_rules_arquitectura_tecnica | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 06_rules_backlog_tecnico | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 07_rules_plan_sprint | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 08_rules_calidad_y_pruebas | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | parcial | ✓ |
| 09_rules_devops | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | parcial | ✓ |
| 10_rules_developer_guide | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| 11_rules_examples | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |

Observaciones:
- 08 menciona `.NET MAUI` una vez en la fila `mobile-app-maui` de §1.2 como stack-hint del propio tipo. La excepción aceptable de C4 lo admite, pero se marca como `parcial` para señalar que es la única instancia de mención literal del SDK fuera de la etiqueta `mobile-app-maui`.
- 09 menciona `paquete-nuget` varias veces dentro de enumeraciones de >3 package managers (npm, pypi, cargo, maven, gem, composer, github-packages, nuget). La excepción C4 sobre NuGet lo admite.

---

## 3. Estructura obligatoria (matriz)

| Archivo | Cab | §1 | §2 | §3 | §4 | §5 | §6 | §7 | §8 | §9 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| _root_rules | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (10 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 00_rules_contexto | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (11 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 01_rules_necesidades_negocio | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (14 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 02_rules_especificacion_funcional | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (12 ítems) | ✓ (3 ej.) | ✓ | ✓ |
| 03_rules_ux_ui_dx | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (12 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 04_rules_prompts_ai | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (12 ítems) | ✓ (3 ej.) | ✓ | ✓ |
| 05_rules_arquitectura_tecnica | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (13 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 06_rules_backlog_tecnico | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (12 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 07_rules_plan_sprint | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (12 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 08_rules_calidad_y_pruebas | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (14 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 09_rules_devops | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (12 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 10_rules_developer_guide | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (13 ítems) | ✓ (2 ej.) | ✓ | ✓ |
| 11_rules_examples | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ (14 ítems) | ✓ (2 ej.) | ✓ | ✓ |

Observaciones:
- Todas las reglas tienen cabecera, las 9 secciones obligatorias, criterios verificables y prompt-snippet ejecutable.
- 11 reglas (de 00 a 11) incluyen `§0 Posición en la cadena SDD 2.1` adicional al esquema mínimo, lo cual refuerza la trazabilidad pero no es obligatorio.
- _root y 00 no presentan `§0`; correcto porque `_root` es el ancla y 00 es upstream de toda la cadena.

---

## 4. Coherencia cross-doc

### 4.1 Cadena de trazabilidad cerrada

| Origen | Destinos declarados | Esperados | OK/Falta |
| --- | --- | --- | --- |
| _root | 12 categorías (00–11) | 12 | OK |
| 00 | 01, 02, 03, 05, 07, 11 | 01, 02, 03, 05, 07, 11 | OK |
| 01 | 02, 04 (si aplica), 06, 07, 08 | 02, 04, 06, 07, 08 | OK |
| 02 | 04, 05, 06, 08, 11 | 04, 05, 06, 08, 11 | OK |
| 03 | 05, 06, 08 | 05, 06, 08 | OK |
| 04 | 02, 05, 08, 09 | 02, 08; recibe de 01 y README §15 | OK (declara además 05 y 09, lo cual es coherente) |
| 05 | 06, 07, 08, 09, 10, 11 | 06, 07, 08, 09, 10, 11 | OK |
| 06 | 07, 08, 10 | 07, 08 | OK (incluye 10 como onboarding al backlog, válido) |
| 07 | 08, 09, 10 | 08, 09 | OK (10 mencionado como bonus, no rompe) |
| 08 | 09, 10, 11 | 09, 10, 11 | OK |
| 09 | 10, 11 | 10, 11 | OK |
| 10 | 11 | 11 | OK |
| 11 | (punto final) | (punto final) | OK |

No se detectan huérfanos. La cadena `Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline` está declarada explícita o conceptualmente en cada §3.3 relevante.

### 4.2 Variantes D8 consistentes

| Archivo | Tipos cubiertos en §1.2 | Faltantes |
| --- | --- | --- |
| _root | library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service | Ninguno |
| 00 | 8 tipos D8 | Ninguno |
| 01 | 8 tipos D8 | Ninguno |
| 02 | 8 tipos D8 | Ninguno |
| 03 | 8 tipos D8 (con split web-microservices con/sin frontend) | Ninguno |
| 04 | 8 tipos D8 | Ninguno |
| 05 | 8 tipos D8 | Ninguno |
| 06 | 8 tipos D8 | Ninguno |
| 07 | 8 tipos D8 | Ninguno |
| 08 | 8 tipos D8 | Ninguno |
| 09 | 8 tipos D8 | Ninguno |
| 10 | 8 tipos D8 | Ninguno |
| 11 | 8 tipos D8 | Ninguno |

Sin "TBD" en ninguna tabla §1.2. D8 OK.

### 4.3 Nomenclatura consistente

| Archivo | Patrón declarado | Cumple `_v<X.Y>.md` | Cumple IDs 2 dígitos |
| --- | --- | --- | --- |
| _root | README.md sin versión; satélites `<kebab>_v<X.Y>.md` | ✓ | n/a (no produce IDs) |
| 00 | `<nombre>_v<X.Y>.md` | ✓ | n/a |
| 01 | `NB-XX-<kebab>_v<X.Y>.md` | ✓ | ✓ (XX 2 dígitos, prohibe `.v`) |
| 02 | `CU/RN/RC-XX-<kebab>_v<X.Y>.md` | ✓ | ✓ |
| 03 | `<nombre>_v<X.Y>.md` | ✓ | n/a |
| 04 | `prompt-<tarea>_v<X.Y>.md` | ✓ | n/a |
| 05 | `ADR-XX-<kebab>_v<X.Y>.md` | ✓ | ✓ |
| 06 | `US-XX/BT-XX/EP-XX-<kebab>_v<X.Y>.md` | ✓ | ✓ (corrige BT-001 del fuente) |
| 07 | `plan-iteracion-sprint-XX_v<X.Y>.md` | ✓ | ✓ |
| 08 | `<nombre>_v<X.Y>.md`, `TC-XX` | ✓ | ✓ |
| 09 | `<nombre>_v<X.Y>.md`, `STAGE-XX` | ✓ | ✓ |
| 10 | `<nombre>_v<X.Y>.md`, `ISSUE-XX` | ✓ | ✓ |
| 11 | `ejemplo-XX-<kebab>_v<X.Y>.md` | ✓ | ✓ |

Todos los archivos prohiben explícitamente el patrón `.v<X.Y>.md` y usan `_v<X.Y>.md`. IDs de 2 dígitos uniformes en todas las reglas que producen prefijos.

### 4.4 Especialidades distintas

| Regla | Especialidad base |
| --- | --- |
| _root | AG-ROOT Arquitecto de Soluciones Senior |
| 00 | AG-00 Product Manager Senior |
| 01 | AG-01 Analista de Negocio Senior |
| 02 | AG-02 Analista Funcional / Ingeniero de Requisitos |
| 03 | AG-03 Especialista UX/UI o DX |
| 04 | AG-04 Ingeniero de Prompts |
| 05 | AG-05 Arquitecto de Software Senior |
| 06 | AG-06 Scrum Master / Agile Coach (Backlog) |
| 07 | AG-07 Scrum Master / Gestión Ágil de Proyectos |
| 08 | AG-08 Ingeniero QA / SDET Senior |
| 09 | AG-09 Ingeniero DevOps Senior |
| 10 | AG-10 Technical Writer / Developer Advocate |
| 11 | AG-11 Developer Advocate / Sample Engineer |

Solapamientos admitidos (ROOT con varias; PM con AG-00 y AG-01; QA cruzado): los solapamientos son los permitidos por C3.4 y se declaran en §1.3 multi-especialidad de cada regla.

---

## 5. Cumplimiento D7 estricto

Resultados de `grep -niE` sobre los 13 archivos. Se indica entre paréntesis si la mención cae bajo excepción aceptable (referencia formal al fuente Motor DSL, literal `mobile-app-maui`, `.NET MAUI` como stack-hint del propio tipo, NuGet en enumeración de >3 package managers).

| Archivo | Motor DSL | ESC/POS | Bluetooth | .NET 10 | NuGet | MAUI fuera de literal |
| --- | --- | --- | --- | --- | --- | --- |
| _root | 0 | 0 | 0 | 0 | 0 | 0 (solo `mobile-app-maui`) |
| 00 | 1 (línea 252, contexto: "ejemplos del dominio Motor DSL" en criterio de aceptación negativa — referencia formal, aceptable) | 0 | 0 | 0 | 0 | 0 |
| 01 | 0 | 0 (solo mención negativa en pregunta §5.4) | 0 (solo mención negativa) | 0 (solo mención negativa) | 0 (solo mención negativa) | 0 (la mención de "MAUI literal" en §5.4 es prohibición textual, no ejemplo canónico) |
| 02 | 0 | 0 | 0 | 0 | 0 | 0 |
| 03 | 0 | 0 | 0 | 0 | 0 | 0 |
| 04 | 0 | 0 | 0 | 0 | 0 | 0 |
| 05 | 3 (líneas 103, 241, 418, 434, contexto: "fuente Motor DSL" justificando corrección heredada — referencia formal, aceptable) | 0 | 0 | 0 | 0 | 0 |
| 06 | 3 (líneas 93, 237, 423, 440, contexto: "antecedente Motor DSL" justificando 2 dígitos — referencia formal, aceptable) | 0 | 0 | 0 | 0 | 0 |
| 07 | 2 (líneas 84, 122, 223, contexto: "antecedente Motor DSL" justificando doble separador y `--` previo — referencia formal, aceptable) | 0 | 0 | 0 | 0 | 0 |
| 08 | 0 (mención al "fuente SDD 1.0" no nombra Motor DSL) | 0 | 0 | 0 | 0 | 1 (.NET MAUI en línea 35 como stack-hint del propio tipo `mobile-app-maui`; cae en excepción C4 aceptable) |
| 09 | 0 | 0 | 0 | 0 | varias (siempre dentro de enumeración npm/pypi/cargo/maven/gem/composer/nuget/github-packages, >3 elementos; aceptable) | 0 |
| 10 | 0 (menciona "el guia-integracion-maui del fuente" en línea 296 como referencia formal a la nomenclatura prohibida — aceptable) | 0 | 0 | 0 | 0 | 0 |
| 11 | 0 (menciona "multaapp-nuget" en líneas 113, 120, 210, 466 como nomenclatura prohibida del fuente — aceptable) | 0 | 0 | 0 | 0 (la mención `multaapp-nuget` es referencia textual al filename prohibido) | 0 |

Veredicto D7: cumplimiento estricto. Todas las menciones aparentes caen bajo excepciones admitidas por la consigna (referencia formal a la fuente, stack-hint en el propio tipo, enumeraciones de package managers).

---

## 6. Calidad §6 (criterios aceptación)

| Archivo | Ítems `- [ ]` | Verificables | Observaciones |
| --- | --- | --- | --- |
| _root_rules | 10 | Sí | Todos objetivos (12 categorías, tipo D8 reflejado, 3 audiencias, 10 términos glosario, líneas 200-400, enum estados). |
| 00_rules_contexto | 11 | Sí | Cada ítem nombra umbrales numéricos o existencia verificable. |
| 01_rules_necesidades_negocio | 14 | Sí | Incluye regex de filename validable mecánicamente. |
| 02_rules_especificacion_funcional | 12 | Sí | Mínimo de CU, secciones exactas, prohibido `.v`. |
| 03_rules_ux_ui_dx | 12 | Sí | WCAG 2.2 AA piso, Diátaxis explícito, snippets ejecutables. |
| 04_rules_prompts_ai | 12 | Sí | Gating positivo verificable, few-shot mínimo, JSON Schema. |
| 05_rules_arquitectura_tecnica | 13 | Sí | Mínimo 3 ADRs individuales, NFR con número y mecanismo. |
| 06_rules_backlog_tecnico | 12 | Sí | 2 dígitos uniformes, distribución MoSCoW, INVEST. |
| 07_rules_plan_sprint | 12 | Sí | Mínimo 2 sprints, sprint goal en una frase, mini-plan para 1 dev. |
| 08_rules_calidad_y_pruebas | 14 | Sí | DoD por capa, matriz CU↔Tests/NFR↔Tests/RN↔Tests, sin `-motor`. |
| 09_rules_devops | 12 | Sí | SBOM, firma, modelo ambientes por tipo D8, patrón parametrizado. |
| 10_rules_developer_guide | 13 | Sí | Sufijo `_v<X.Y>.md` obligatorio, slug genérico, gating por tipo. |
| 11_rules_examples | 14 | Sí | Materialización /samples, progresión por nivel/capacidad, 5 pasos máx. |

Todos los archivos superan el mínimo de 8 ítems y los criterios son objetivos (sin opiniones).

---

## 7. Calidad §8 (prompt-snippet)

| Archivo | Bloque text | Placeholders | Insumos | Salida | Ejecutable |
| --- | --- | --- | --- | --- | --- |
| _root | ✓ | ✓ | PROJECT-BRIEF/README | /SDD2.1D/docs/README.md | Sí |
| 00 | ✓ | ✓ | BRIEF/README | /SDD2.1D/docs/00_contexto/ | Sí |
| 01 | ✓ | ✓ | BRIEF/README + 00 | /SDD2.1D/docs/01_necesidades_negocio/ | Sí |
| 02 | ✓ | ✓ | BRIEF/README + 00/01 | /SDD2.1D/docs/02_especificacion_funcional/ | Sí |
| 03 | ✓ | ✓ | BRIEF/README + 00/02 | /SDD2.1D/docs/03_ux_ui_dx/ | Sí |
| 04 | ✓ | ✓ | BRIEF/README + 01/02 (con gating) | /SDD2.1D/docs/04_prompts_ai/ | Sí |
| 05 | ✓ | ✓ | BRIEF/README + 00/01/02/04 | /SDD2.1D/docs/05_arquitectura_tecnica/ | Sí |
| 06 | ✓ | ✓ | BRIEF/README + 01/02/05 | /SDD2.1D/docs/06_backlog-tecnico/ | Sí |
| 07 | ✓ | ✓ | BRIEF/README + 06/02/05 | /SDD2.1D/docs/07_plan-sprint/ | Sí |
| 08 | ✓ | ✓ | BRIEF/README + 02/05/06/07 | /SDD2.1D/docs/08_calidad_y_pruebas/ | Sí |
| 09 | ✓ | ✓ | BRIEF/README + 05/08 | /SDD2.1D/docs/09_devops/ | Sí |
| 10 | ✓ | ✓ | BRIEF/README + 02/05/08 | /SDD2.1D/docs/10_developer_guide/ | Sí |
| 11 | ✓ | ✓ | BRIEF/README + 02/05/10 | /SDD2.1D/docs/11_examples/ + /samples/ | Sí |

Todos los snippets declaran rol, insumos, documentos a generar, reglas de redacción, trazabilidad, criterios de calidad y path de salida. Ejecutables por subagente sin información adicional.

---

## 8. Calidad §7 (ejemplos)

| Archivo | # ejemplos | Dominios | Tipos D8 cubiertos |
| --- | --- | --- | --- |
| _root | 2 | turnos médicos, librería CSV | web-monolith, library |
| 00 | 2 | turnos médicos, librería CSV | web-monolith, library |
| 01 | 2 | turnos médicos, librería CSV | web-monolith, library |
| 02 | 3 | turnos médicos (salud), API pagos, worker mensajes | web-monolith, rest-api, worker-service |
| 03 | 2 | turnos médicos, librería CSV | web-monolith (UX/UI), library (DX) |
| 04 | 3 | mesa de ayuda, procesamiento documental, política transversal | web-monolith, rest-api |
| 05 | 2 | turnos médicos, microservicio de pagos | web-monolith, web-microservices |
| 06 | 2 | turnos médicos, API pagos | web-monolith, rest-api |
| 07 | 2 | turnos médicos, librería CSV | web-monolith, library |
| 08 | 2 | librería CSV, API pagos | library, rest-api |
| 09 | 2 | librería CSV, API pagos | library, rest-api |
| 10 | 2 | librería CSV, CLI migración datos | library, cli-tool |
| 11 | 2 | librería CSV, API pagos | library, rest-api |

Dominios usados cubren turnos médicos, salud, pagos, parsing CSV, mesa de ayuda, procesamiento documental, worker de mensajes, CLI de migración. Todos distintos al Motor DSL. Tipos D8 cubiertos por los 13 archivos en conjunto: library, web-monolith, web-microservices, rest-api, cli-tool, worker-service. Faltan ejemplos explícitos de desktop-app y mobile-app-maui en los §7, aunque las tablas §1.2 y §2 sí los cubren — observación P2.

---

## 9. Gating de categorías opcionales

- 04 (prompts AI): gating explícito declarado en §2.1 "Gating explícito" con bandera `usa_llm: true|false` en PROJECT-README §15 o NFR en §13. El prompt-snippet incluye precondición de gating con la consigna de no generar la carpeta si el flag es negativo y dejar constancia en el log. Cumple.
- 10 (developer guide): gating por tipo D8 declarado en §0 ("obligatoria para library, rest-api y cli-tool", "recomendada para web-microservices", "opcional para web-monolith, desktop-app, mobile-app-maui y worker-service"). §2.2 fija matriz tipo D8 → categoría 10 con granularidad doble. Cumple.
- 11 (examples): materialización de `/samples` documentada en §2.3 con matriz tipo D8 → estructura de carpetas; cada `ejemplo-XX-<kebab>_v<X.Y>.md` tiene correspondencia 1:1 con `/samples/XX-<kebab>/`. Cumple.

---

## 10. Lecciones audit Fase 0 aplicadas

| Regla | Lección audit-sdd1.md | Aplicada |
| --- | --- | --- |
| 01 | Nomenclatura `NB-XX-<kebab-lowercase>_v1.0.md` (no `.v1.0` ni mayúsculas) | Sí, §3.1 y anti-patrones §4.5 con regex de validación |
| 02 | Política versionado: una sola versión vigente | Sí, §3.5 con `_legacy/` para superadas |
| 05 | ADRs como archivos individuales (no consolidado) | Sí, §3.3 "Convención crítica de ADR individuales" y anti-patrón §4.7 |
| 06 | IDs de 2 dígitos (no `BT-001`) | Sí, §3.2 "Convención crítica de identificadores de dos dígitos" |
| 07 | Doble separador eliminado en nombres de archivo | Sí, §3.1 prohibición explícita y §4.7 anti-patrón |
| 08 | Eliminado sufijo `-motor` domain-specific | Sí, §3.1 prohibición explícita y §6 criterio de aceptación |
| 09 | `<tipo-artefacto>` parametrizado en lugar de `nuget` hardcodeado | Sí, §3.1 con lista admitida y §4.8 anti-patrón |
| 10 | TODOS los archivos llevan sufijo `_v1.0` | Sí, §3.1 y §0 corrige antecedente sin sufijo |
| 11 | Nomenclatura por progresión (`basico/intermedio/avanzado`) no por dominio (`multa`) | Sí, §3.2 "Reglas de progresión" con prohibición de slug por dominio |

Todas las correcciones del audit-sdd1.md están aplicadas y verificadas con anti-patrón explícito o criterio de aceptación.

---

## 11. Defectos detectados

### 11.1 P0 (bloqueantes)

Ninguno.

### 11.2 P1 (importantes)

1. **08 §1.2 `mobile-app-maui` menciona `.NET MAUI` literal.** En la fila `mobile-app-maui` de §1.2 (línea 35) se menciona ".NET MAUI UI Testing" como stack-hint. La excepción C4 lo admite (única mención dentro de stack-hint enumerado del propio tipo), pero conviene generalizar a "framework de UI testing del runtime objetivo" para mantener neutralidad estricta.
2. **08 §0 contiene "marco teórico" mezclado con explicación del audit Fase 0.** El bloque de §0 explica los déficits del fuente SDD 1.0 (sufijo `-motor`, referencias ausentes) intercalado con la posición en la cadena. La explicación queda válida, pero rompería el patrón si se aplica al pie de la letra el principio de "una sola versión vigente, sin contaminación con historia". Cosmético; no afecta semántica.
3. **09 §2.2 incluye `paquete-nuget` como ejemplo principal en la fila `library`.** Aunque la enumeración tiene >3 elementos (npm, pypi, cargo, maven, gem, composer, github-packages, nuget) y cumple la excepción C4, conviene reordenar para que `nuget` no aparezca antes que opciones más universales (npm, pypi). Cosmético.
4. **Falta cobertura de tipos D8 `desktop-app` y `mobile-app-maui` en los §7 de las 13 reglas.** Las tablas §1.2, §2.1 y §2.2 cubren los 8 tipos, pero los ejemplos genéricos del §7 nunca seleccionan `desktop-app` ni `mobile-app-maui` como dominio ilustrativo. P1 menor: la consigna pide "mínimo 2 dominios distintos al Motor DSL" — se cumple en cada regla — pero la cobertura agregada a través de los 13 archivos queda incompleta para esos 2 tipos.
5. **07 §3.6 política de versionado deja ambigüedad sobre carry-over.** "Los planes de sprint no se versionan tras el cierre del sprint" entra en tensión potencial con "Si durante el sprint cambia el scope, el cambio se documenta en la sección de control de cambios del propio plan, pero el archivo conserva la versión inicial". Aclaración recomendable.

### 11.3 P2 (cosméticos)

1. **00 §6 último ítem dice "Motor DSL"** (línea 252): "ni referencias hardcoded a stack, frameworks o ejemplos del dominio Motor DSL". Es una referencia formal a la prohibición, aceptable bajo C4, pero podría generalizarse a "del dominio fuente" para coherencia con 02/05/06/07/08/09/10/11 que usan formulación neutra.
2. **01 §5.4 menciona "MAUI literal fuera del valor del tipo D8"** (línea 242). Mención negativa correcta, pero formulación verbosa.
3. **02 §0 introduce numeración `§0` que no aparece en _root ni 00.** Inconsistencia menor de plantilla.
4. **03, 04, 06, 07, 08, 09, 10, 11 incluyen `§0`; 01 incluye un párrafo introductorio sin numerar.** Inconsistencia menor entre 01 y el resto de la familia.
5. **_root §4.4 Tabla A muestra solo 2 filas de ejemplo (00_contexto, 01_necesidades_negocio).** Convendría completar las 12 entradas para mostrar el patrón completo. La consigna no obliga, pero el README real sí lo exigirá.
6. **11 §2.1 admite "imagenes/" sin sufijo de versión.** El criterio es razonable (carpeta de assets) pero rompe el patrón uniforme de §3.1 que dice "obligatorio para todos los artefactos versionables". La excepción está implícita; convendría hacerla explícita.

---

## 12. Veredicto final

APROBADO CON OBSERVACIONES.

Las 13 reglas constructivas de la Fase 2 cumplen los invariantes D1–D8, declaran la cadena de trazabilidad D6 sin huérfanos, aplican explícitamente las 9 lecciones del audit-sdd1.md y exponen prompt-snippets ejecutables por subagente. No se detectan defectos P0 bloqueantes. Los 5 defectos P1 son señalamientos de mejora (neutralidad de stack-hints, completitud de ejemplos por tipo D8, aclaración de versionado de sprint plans) y los 6 P2 son inconsistencias cosméticas de plantilla.

Pasos correctivos sugeridos (no bloqueantes para la Fase 3 del bootstrap):

1. Generalizar la mención de `.NET MAUI` en 08 §1.2 a "framework de UI testing del runtime objetivo".
2. Reordenar la enumeración de package managers en 09 §2.2 para que `paquete-nuget` no sea el primer ejemplo en la fila `library`.
3. Agregar al menos un ejemplo §7 con dominio `desktop-app` y uno con `mobile-app-maui` en al menos 2 de las 13 reglas (sugerencia: 03 UX/UI y 09 DevOps).
4. Aclarar en 07 §3.6 que el control de cambios del plan registra modificaciones de scope intra-sprint y no incrementa la versión del archivo.
5. Unificar la presencia o ausencia de `§0` entre _root, 00 y 01 con el resto de las reglas, o documentar la excepción.
6. Completar la Tabla A del _root §4.4 con las 12 entradas para que el README maestro pueda copiarla directamente.

Con estas correcciones no bloqueantes aplicadas o aceptadas como deuda menor, las 13 reglas quedan listas para gobernar la Fase 3 (bootstrap intake del proyecto piloto SDD 2.1).
