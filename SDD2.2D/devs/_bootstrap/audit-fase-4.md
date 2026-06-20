# Auditoría de Fase 4 — Marco teórico + Guía de usuario

**Fase:** 4
**Auditor:** Auditor independiente cross-doc
**Alcance:** `SDD2.2D/devs/guides/marco-teorico-sdd_v1.0.md` + `SDD2.2D/guides/guia-usuario-sdd2.0_v1.0.md`
**Versión del informe:** 1.0
**Fecha:** 2026-05-17

---

## 1. Resumen ejecutivo

Los dos entregables de Fase 4 cumplen sustancialmente con los invariantes D1–D8 y con la estructura solicitada. El marco teórico tiene los 14 capítulos requeridos, glosario con 53 términos (umbral ≥30) y bibliografía con 33 entradas APA 7 (umbral ≥15) que incluye casi todas las fuentes canónicas listadas. La guía de usuario tiene los 10 capítulos, 15 entradas de FAQ (umbral ≥10), 3 mini-casos con sus 4 sub-bloques cada uno y cobertura D8 ilustrada en al menos 3 tipos. La separación marco-teoría vs guía-operativa se respeta. D7 se cumple con observaciones: las menciones a Motor DSL, NuGet y MAUI aparecen exclusivamente como referencia formal histórica o como literal del identificador D8 `mobile-app-maui`, sin uso canónico.

Conteo de defectos: **P0=0**, **P1=3**, **P2=4**.

Veredicto preliminar: **APROBADO CON OBSERVACIONES**.

---

## 2. Conformidad D1–D8

### 2.1 Marco teórico (`marco-teorico-sdd_v1.0.md`)

| Invariante | Cumple | Evidencia |
|---|---|---|
| D1 — Rioplatense técnico, tildes, sin emojis, sin negritas decorativas | Sí | Idioma declarado en cabecera (líneas 9, 52); 0 emojis (grep Unicode emoji ranges = 0); las negritas se usan como etiquetas semánticas (`**Patrón típico.**`, `**Decisiones canónicas.**`), no decorativas; tildes sostenidas (estudió, técnico, sección, decisión, automático todas con tilde en cuerpo). |
| D2 — UTF-8, LF, `.md`, `#`, `-` | Sí | `file(1)` reporta "Unicode text, UTF-8" sin "CRLF"; extensión `.md`; encabezados ATX con `#`; listas con `-`. |
| D3 — Kebab-case en filename | Sí | `marco-teorico-sdd_v1.0.md` cumple kebab-case. |
| D4 — Nomenclatura uniforme `_v<X.Y>.md` | Sí | Sufijo `_v1.0.md` correcto. |
| D5 — v1.0 | Sí | Cabecera declara "Versión: 1.0" y tabla de control de cambios fila única. |
| D6 — Declara cadena Visión→NB→CU→RN→ADR→US→BT→Sprint→Test→Pipeline | Parcial | Cadena AG-XX en §4.4 cubre todos los nodos vía nombres de especialidad; el glosario (línea 1612) abrevia la cadena como "visión → necesidad → caso de uso → arquitectura → US → BT → test", sin incluir explícitamente RN, Sprint ni Pipeline en una sola sentencia canónica. Se sostiene por composición pero no aparece literal. **(P1)** |
| D7 — 0 menciones canónicas Motor DSL/ESC-POS/MAUI/Bluetooth/NuGet/.NET 10/impresora térmica | Sí | Ver §5. Las pocas apariciones son referencia histórica explícita o literal D8. |
| D8 — Enumera los 8 tipos y cubre los 8 estilos en §7 | Sí | Enumeración en líneas 124, 267, 767–776; §7 cubre los 8 tipos D8 en sub-secciones 7.2.1 a 7.2.8 con estilo arquitectónico declarado por tipo (ver §9 de este informe). |

### 2.2 Guía de usuario (`guia-usuario-sdd2.0_v1.0.md`)

| Invariante | Cumple | Evidencia |
|---|---|---|
| D1 — Rioplatense técnico, tildes, sin emojis, sin negritas decorativas | Sí | Idioma declarado en cabecera YAML (línea 8); 0 emojis; voseo natural usado ("asegurate", "completá", "regenerás"); negritas usadas semánticamente en tablas. |
| D2 — UTF-8, LF, `.md`, `#`, `-` | Sí | `file(1)` reporta UTF-8 sin CRLF; listas y encabezados correctos. |
| D3 — Kebab-case en filename | Sí | `guia-usuario-sdd2.0_v1.0.md` cumple. |
| D4 — Nomenclatura uniforme `_v<X.Y>.md` | Sí | Sufijo `_v1.0.md` correcto. |
| D5 — v1.0 | Sí | Cabecera declara "Versión: 1.0". |
| D6 — Declara cadena Visión→NB→CU→RN→ADR→US→BT→Sprint→Test→Pipeline | Sí | Línea 25: "Visión → Necesidad de Negocio → Caso de Uso → Regla de Negocio → ADR → User Story → Backlog Técnico → Sprint → Test → Pipeline". Línea 1064 reitera en formato corto. |
| D7 — 0 menciones canónicas | Sí | Ver §5. Única aparición de "Motor DSL" es en §9.2 como antecedente histórico explícito. |
| D8 — Enumera los 8 tipos | Sí | Enumeración explícita en líneas 93–100 y 1062. |

---

## 3. Estructura

### 3.1 Marco teórico (14 capítulos)

| Capítulo esperado | Presente | Contenido mínimo |
|---|---|---|
| §1 Introducción y encuadre | Sí (L34) | Propósito, audiencia, ruta de lectura, diferencias con guía, mapa visual, origen. |
| §2 Fundamentos SDD (3 niveles) | Sí (L128) | Spec-First / Spec-Anchored / Spec-as-Source con marco de decisión, comparación TDD/BDD, limitaciones. |
| §3 Metodología del template (6 pasos) | Sí (L236) | Los 6 pasos del flujo de usuario están: chat informal → consolidado → intakes → bootstrap → master-prompt → handoff. Diagrama ASCII presente. |
| §4 Especialidades (13 roles) | Sí (L393) | AG-ROOT + AG-00..AG-11 con responsabilidades, documentos, variantes y trazabilidad. |
| §5 Metodología ágil (Scrum + DoR/DoD) | Sí (L562) | Scrum vs Kanban vs Scrumban, Sprint 0, ceremonias, roles, artefactos, DoR y DoD canónicos. |
| §6 Técnicas de descomposición | Sí (L678) | Vertical Slicing, Walking Skeleton, Story Mapping, Example Mapping, Spike, ATDD, Mob, Thin Slice, Shape Up, Impact Mapping, Dual-Track. |
| §7 Estilos arquitectónicos (cobertura D8) | Sí (L761) | Los 8 tipos D8 con estilo dominante y patrón típico. |
| §8 UX/UI/DX | Sí (L1001) | Nielsen, leyes UX, WCAG 2.2 AA, Diátaxis, DX de APIs. |
| §9 Calidad y pruebas | Sí (L1099) | Pirámide, tipos, ISO 25010, quality gates, DoD como artefacto. |
| §10 DevOps y release | Sí (L1189) | CALMS, DORA, SemVer, Conventional Commits, GitHub Flow / Trunk-Based, supply chain (SBOM/SLSA), observabilidad, GitOps. |
| §11 Prompting con IA (plan-then-confirm) | Sí (L1325) | Plan-then-confirm, orquestador/subagentes/auditor, patrones de prompt, anti-patrones, fundamentación (CoT, ReAct, Self-Refine, Multi-agent, Constitutional). |
| §12 Anti-patrones | Sí (L1473) | Proceso SDD, Scrum, prompting, arquitectónicos, calidad/testing, DevOps, documentación. |
| §13 Glosario (≥30 términos) | Sí (L1559) | 53 términos contados. |
| §14 Bibliografía APA 7 (≥15 referencias) | Sí (L1621) | 33 referencias contadas, organizadas en cinco sub-secciones. |

### 3.2 Guía de usuario (10 capítulos)

| Capítulo esperado | Presente | Contenido mínimo |
|---|---|---|
| §1 ¿Qué es el template? | Sí (L17) | Pilares, audiencia, separación con marco teórico. |
| §2 Prerequisitos | Sí (L37) | Cuenta Claude.ai, Claude Code, git, editor, conocimientos previos, verificación rápida. |
| §3 Cuándo usar y cuándo no | Sí (L72) | Perfilado por tamaño, complejidad, tipo D8, plazo; tabla combinada; cuándo no usar. |
| §4 Recorrido paso a paso (6 pasos) | Sí (L135) | §4.1 a §4.6 cubren los 6 pasos con prompts ejemplo. |
| §5 Ejemplos aplicados (2–3 mini-casos) | Sí (L396) | 3 mini-casos: rest-api, library, mobile-app-maui. |
| §6 FAQ (≥10) | Sí (L732) | 15 entradas (F-01..F-15). |
| §7 Cómo extender | Sí (L882) | Agregar categoría, agregar tipo D8, agregar variante. |
| §8 Cómo regenerar parcialmente | Sí (L926) | Identificar afectados, correr orquestador parcial. |
| §9 Hojas de ruta | Sí (L974) | Principiantes, equipos con experiencia SDD, evaluadores académicos. |
| §10 Glosario rápido + mapa visual | Sí (L1046) | 15 términos esenciales + árbol ASCII completo. |

---

## 4. Separación marco vs guía

- Solapamientos significativos: **ninguno**.
- Análisis: el marco teórico declara explícitamente en §1.4 (líneas 74–87) la separación de responsabilidades vía tabla comparativa "Aspecto / Marco / Guía"; la guía declara en §1 (líneas 12–13 y 31) que no enseña teoría y refiere al marco. El paso 5 del flujo aparece en ambos pero con corte distinto: el marco lo describe a nivel conceptual (qué hace el orquestador, por qué paraleliza o serializa); la guía lo describe operativamente (qué prompt pegar, qué confirmar, qué tarda). §11 del marco explica el patrón plan-then-confirm como concepto, §4.5 de la guía lo aplica con comandos concretos. La división se respeta.

---

## 5. Cumplimiento D7

Búsqueda case-insensitive de cadenas prohibidas (`Motor DSL`, `ESC/POS`, `esc-pos`, `escpos`, `Bluetooth`, `.NET 10`, `NuGet`, `impresora térmica`, `MAUI`).

| Archivo | Cadena | Ocurrencias | Línea(s) | Veredicto |
|---|---|---|---|---|
| marco-teorico | Motor DSL | 1 | 122 | (b) — referencia formal histórica en §1.6 ("caso de estudio histórico Motor DSL"), inmediatamente acotada en línea 124: "se conservan exclusivamente como referencia formal del origen, no como contenido canónico". Aceptable. |
| marco-teorico | NuGet | 2 | 769, 1314 | (b) — aparece como ejemplo en tabla "NuGet, npm, PyPI, Maven" mostrando que el template es agnóstico al ecosistema de paquetes. Aceptable (no canónico). |
| marco-teorico | MAUI (mayúscula sola) | 1 | 382 | (b) — celda de tabla comparativa SDD 1.0 vs 2.0: "Específico .NET / MAUI / NuGet | Stack-agnóstico" describe explícitamente al fuente como obsoleto y al nuevo como stack-agnóstico. Aceptable. |
| marco-teorico | mobile-app-maui | 5 | 124, 267, 454, 773, 876, 985, 1318 | (a) — literal del identificador D8. Aceptable. |
| marco-teorico | Bluetooth | 0 | — | OK. |
| marco-teorico | ESC/POS, esc-pos, escpos | 0 | — | OK. |
| marco-teorico | .NET 10 | 0 | — | OK. (Solo aparece ".NET" suelto una vez en la tabla histórica L382). |
| marco-teorico | impresora térmica | 0 | — | OK. |
| guía-usuario | Motor DSL | 1 | 998 | (b) — §9.2 "la del Motor DSL, referenciada al final de §1 de esta guía como antecedente histórico". Aceptable (la nota explícita "antecedente histórico" cumple la excepción). |
| guía-usuario | mobile-app-maui | 7 | 97, 629, 651, 662, 903, 1062, 1202 | (a) — literal del identificador D8 en todas las apariciones. Aceptable. |
| guía-usuario | NuGet, MAUI (suelto), .NET 10, Bluetooth, ESC/POS, impresora térmica | 0 | — | OK. |

**Observación adicional P2:** la guía de usuario en §9.2 (L998) dice "la del Motor DSL, referenciada al final de §1 de esta guía como antecedente histórico", pero §1 de la guía **no** referencia explícitamente al Motor DSL como antecedente: solo el marco teórico lo hace en §1.6. La referencia cruzada está mal apuntada (debería citar el marco). **(P2)**

---

## 6. Bibliografía APA 7 (marco teórico §14)

- Número de referencias: **33**.
- Sub-secciones: 14.1 Libros canónicos ágiles, 14.2 UX/UI/DX, 14.3 Normas y especificaciones, 14.4 Papers académicos, 14.5 Recursos online.
- Formato APA correcto: **sí, parcial con observación menor**. Las entradas siguen "Autor, A. (Año). *Título*. Editorial / URL". El uso de cursivas `*...*` para títulos de libros y normas es correcto. Las URLs aparecen en entradas web nativas. Una entrada ISO usa "actualizada 2023" en paréntesis, formato no estándar APA estricto pero comprensible. **(P2)**
- Fuentes canónicas requeridas:

| Fuente requerida | Presente | Cita |
|---|---|---|
| Sutherland (Scrum) | Sí | Schwaber, K., & Sutherland, J. (2020). *The Scrum Guide*. (L1647). |
| Evans (DDD) | Sí | Evans, E. (2003). *Domain-driven design*. (L1633). |
| Cohn (US/Estimation) | Sí | Cohn, M. (2004), Cohn, M. (2009). (L1629, L1631). |
| Beck (XP/TDD) | Sí | Beck, K. (2002). *Test-driven development: By example*. (L1627). |
| Cockburn (CU) | **No** | Cockburn no aparece en la bibliografía. El criterio C6 lo lista como fuente canónica para casos de uso ("Writing Effective Use Cases", 2001). **(P1)** |
| Nielsen (heurísticas) | Sí | Nielsen, J. (1993) y Nielsen, J. (1994). (L1659, L1703). |
| W3C (WCAG) | Sí | W3C (2023). *WCAG 2.2*. (L1681). W3C (2024). *ARIA*. (L1735). |
| SemVer.org | Sí | Preston-Werner, T. (2013). *Semantic Versioning 2.0.0*. (L1677). |
| Conventional Commits | Sí | Conventional Commits. (2021). (L1679). |
| Diátaxis (Procida) | Sí | Procida, D. (2022). (L1729). |
| Anthropic (Claude) | Sí | Anthropic. (2024). (L1719). |

---

## 7. Mini-casos guía (§5)

| Caso | Tipo D8 | Sub-bloques presentes | Cumple |
|---|---|---|---|
| §5.1 API REST de turnos médicos | rest-api | (1) Chat resumido (L404–412), (2) Fragmentos clave del intake (L416–438), (3) Output del orquestador (L440–461), (4) Muestras de documentos generados (L463–527, CU-03 + ADR-002). | Sí, 4/4. |
| §5.2 Librería utilitaria parsing CSV | library | (1) Chat resumido (L533–541), (2) Fragmentos del intake (L545–557), (3) Output del orquestador (L559–578), (4) Muestras (L580–627, visión + README de ejemplos). | Sí, 4/4. |
| §5.3 App móvil inventario de almacén | mobile-app-maui | (1) Chat resumido (L633–641), (2) Fragmentos del intake (L645–657), (3) Output del orquestador (L659–678), (4) Muestras (L680–728, wireframe + entornos-deploy). | Sí, 4/4. |

Cobertura D8: 3 tipos distintos representados (rest-api, library, mobile-app-maui). Cumple el umbral solicitado (≥3).

---

## 8. Glosario marco (§13)

- Número de términos: **53** (filas que comienzan con `| **`).
- Cobertura: incluye términos doctrinales SDD (Spec-First, Spec-Anchored, Spec-as-Source, SDD, Trazabilidad vertical), ágiles (Sprint, Sprint 0, DoR, DoD, MoSCoW, Story point, US-XX, BT-XX), arquitectónicos (Clean Architecture, CQRS, Event-driven, BFF), DevOps (DORA metrics, GitHub Flow, GitOps, SemVer, SBOM, SLSA, SLI/SLO/SLA, Feature flag), UX (UX, DX, WCAG 2.2), normativos (ISO 25010, ISO 29148) e IA (Subagente). Definiciones operativas (una a dos líneas). Umbral ≥30 holgadamente superado.
- Observación menor: la entrada **RCL (Razor Class Library)** menciona `.NET` y `Blazor`. No es D7-prohibido pero está fuera de la promesa stack-agnóstica del marco. **(P2)**

---

## 9. Cobertura D8 en §7 marco

| Tipo D8 | Cubierto | Estilo arquitectónico declarado |
|---|---|---|
| library | Sí (§7.2.1, L782) | Arquitectura por capas + API pública mínima. |
| web-monolith | Sí (§7.2.2, L803) | Monolito modular con Clean Architecture, CQRS ligero opcional. |
| web-microservices | Sí (§7.2.3, L824) | Microservicios + API Gateway + Identity centralizado + event bus. |
| desktop-app | Sí (§7.2.4, L854) | MVVM sobre framework GUI nativo. |
| mobile-app-maui | Sí (§7.2.5, L876) | MVVM compartido entre plataformas. |
| rest-api | Sí (§7.2.6, L900) | Clean Architecture + CQRS opcional + OpenAPI first. |
| cli-tool | Sí (§7.2.7, L923) | Command pattern, una unidad por subcomando. |
| worker-service | Sí (§7.2.8, L944) | Pipeline / Event-driven con idempotencia. |

Tabla resumen 7.4 (L979–988) consolida D8 → estilo dominante + patrón alternativo. Cobertura completa.

---

## 10. Tildes (sondeo D1)

Sondeo con patrón de palabras frecuentes sin tilde: `\b(seccion|tecnico|politica|practica|metrico|automatico|decision|interaccion|funcion|version)\b` case-insensitive.

| Archivo | Hits brutos | Contexto de los hits | Veredicto |
|---|---|---|---|
| marco-teorico | 6 hits relevantes | `06_backlog-tecnico` (kebab-case L109, L533), `DECISION` mayúscula como etiqueta en bloque prompt L1414, L1417, "Decision Record" en término inglés L1565, "Version 1.1" en cita inglesa NIST L1683. | Sin hallazgos de tildes faltantes en cuerpo prosa. Todos los hits son intencionales (anglicismos, kebab-case, mayúsculas de prompt). |
| guía-usuario | 6 hits relevantes | `--version` flag CLI L42, L63, L64, `06_backlog-tecnico` kebab-case L354, L961, L1146. | Sin hallazgos de tildes faltantes en cuerpo prosa. |

Veredicto del sondeo: **D1 conforme**. El cuerpo en prosa sostiene las tildes (verificado manualmente sobre muestras: "especificación", "metodología", "arquitectónicos", "decisión", "técnico", "automático" todas con tilde).

Observación P2: en el bloque ASCII del diagrama del marco §3.8 (L322–372) se usa "Ejecucion", "Confirmacion", "Consolidacion", "Integracion" sin tilde, deliberadamente para evitar problemas de render del bloque `text`. Es práctica corriente; el cuerpo prosa fuera del bloque usa tildes. Aceptable. **(P2 menor cosmético).**

---

## 11. Defectos detectados

### 11.1 P0

Ninguno.

### 11.2 P1

| ID | Defecto | Evidencia | Mitigación sugerida |
|---|---|---|---|
| P1-01 | D6: el marco teórico no declara la cadena canónica completa `Visión→NB→CU→RN→ADR→US→BT→Sprint→Test→Pipeline` en una sola sentencia. La cadena AG-XX de §4.4 cubre los nodos por nombre de especialidad, y la entrada de glosario "Trazabilidad vertical" la abrevia omitiendo RN, Sprint y Pipeline. | Marco L558, L1612; §4.4 L542–555. | Agregar una sentencia explícita al inicio de §4.4 o en §11.7 con la cadena literal exacta, espejando la que la guía declara en L25. |
| P1-02 | Bibliografía: falta Cockburn (canónico para Casos de Uso, ej. *Writing Effective Use Cases*, 2001). El criterio C6 lo lista explícitamente. | Marco §14; grep `Cockburn` = 0. | Agregar la entrada Cockburn, A. (2001). *Writing effective use cases*. Addison-Wesley. en §14.1. |
| P1-03 | El conteo "≥15 referencias" se cumple, pero el bloque indica IEEE como autoría grupal en una entrada implícita ("IEEE 830-1998" mencionado en L134 sin entrada en §14). Falta entrada formal de IEEE 830 (o equivalente actualizada ISO/IEC/IEEE 29148, que sí está). Verificación menor: se cita en cuerpo sin referencia bibliográfica explícita. | Marco L134. | Reemplazar la mención inline por cita a la entrada ISO/IEC/IEEE 29148 ya presente en §14.3, o agregar la IEEE 830 si se quiere conservar literal. |

### 11.3 P2

| ID | Defecto | Evidencia | Mitigación sugerida |
|---|---|---|---|
| P2-01 | Cross-link mal apuntado: la guía §9.2 (L998) dice "Motor DSL referenciada al final de §1 de esta guía como antecedente histórico" pero §1 de la guía no contiene esa referencia. La referencia histórica está en el marco §1.6. | Guía L998. | Cambiar a "referenciada en el marco teórico §1.6 como antecedente histórico". |
| P2-02 | Entrada de glosario `RCL (Razor Class Library)` cita stack `.NET / Blazor` específico, ligeramente fuera de la promesa stack-agnóstica. No es violación D7 pero rompe el tono. | Marco L1596. | Reescribir en términos genéricos ("unidad de empaquetado de componentes UI reutilizables") o eliminar si no se usa en otro lado. |
| P2-03 | Diagrama ASCII del marco §3.8 (L322–372) usa palabras sin tilde ("Ejecucion", "Confirmacion", "Consolidacion", "Integracion") por compatibilidad de render. Aceptable pero cosmético. | Marco L344, L356, L362, L369; §4.4 L555. | Opcional: usar tildes; markdown y monospace renderizan UTF-8 sin problema. |
| P2-04 | Una entrada ISO en §14.3 usa el paréntesis "(2011, actualizada 2023)" no estándar APA estricto. APA 7 sugiere fechar la edición que se cita. | Marco L1669. | Cambiar a "International Organization for Standardization. (2023). *ISO/IEC 25010:2011 — ...*" si se cita la actualización, o conservar 2011 sola. |

---

## 12. Veredicto final

**APROBADO CON OBSERVACIONES.**

Los dos documentos cumplen con D1, D2, D3, D4, D5, D7 y D8 sin reservas. D6 se cumple en la guía de usuario y se cumple parcialmente en el marco teórico (composición correcta pero falta una sentencia canónica literal). La estructura de 14 capítulos del marco y de 10 capítulos de la guía está completa. Los 3 mini-casos de la guía cubren 3 tipos D8 con los 4 sub-bloques requeridos. El glosario del marco supera holgadamente el umbral (53 ≥ 30). La bibliografía APA 7 supera el umbral (33 ≥ 15) y contiene casi todas las fuentes canónicas listadas en C6; falta Cockburn. La separación marco-teoría vs guía-operativa se respeta sin solapamientos significativos. Las referencias al master-prompt en la guía son consistentes con la estructura del prompt orquestador (§§ 1, 5, 6, 7, 8, 9, 10, 12, 13, 14, 16 todas verificadas).

Acciones requeridas para aprobación sin observaciones (P1):

1. Agregar la cadena canónica D6 literal al marco teórico (§4.4 o §11.7).
2. Agregar la entrada de Cockburn a §14.
3. Resolver la mención inline a IEEE 830 (vincular a la entrada ISO existente o agregar la entrada IEEE 830).

Las P2 son ajustes editoriales menores que no bloquean el avance a Fase 5 (codificación / consumo del template).

---

**Fin del informe**
