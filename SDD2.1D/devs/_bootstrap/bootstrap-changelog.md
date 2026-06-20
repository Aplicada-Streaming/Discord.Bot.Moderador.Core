# Changelog del bootstrap SDD 2.1

**Versión del bootstrap:** 1.0
**Estado:** Cerrado con observaciones (según veredicto del audit final §8)

---

## Resumen ejecutivo

El bootstrap del template SDD 2.1 produjo, en 6 fases (F0 a F5) y a través de aproximadamente 20 despachos de subagentes coordinados por un master-prompt de bootstrap, un template metodológico stack-agnóstico orientado a la construcción de proyectos por especificación viva, basado en la auditoría del fuente SDD 1.0 (caso de estudio histórico "Motor DSL" de la cátedra de Aplicada 2026).

Se generaron 23 archivos markdown distribuidos en seis carpetas funcionales: `/SDD2.1D/devs/_bootstrap/`, `/SDD2.1D/devs/intake/`, `/SDD2.1D/devs/rules/`, `/SDD2.1D/devs/orchestrator/`, `/SDD2.1D/devs/guides/`, `/SDD2.1D/guides/`. La carpeta `/SDD2.1D/docs/` queda reservada vacía con `.gitkeep` como destino del orquestador del proyecto.

El bootstrap incorporó auditoría sistemática entre fases (5 audits intermedios + 1 audit final) y sostuvo las invariantes D1 a D8 a lo largo de todo el ciclo. Los defectos P0 detectados durante el camino fueron resueltos antes del cierre de su fase. Los P1 y P2 abiertos al cierre se documentan para v1.1.

## Métricas globales

| Métrica | Valor |
|---|---|
| Archivos generados en `/SDD2.1D/` | 23 markdown + 1 `.gitkeep` |
| Líneas markdown totales | 11.217 líneas |
| Palabras aproximadas | ~90.000 palabras |
| Subagentes despachados | ~20 (F1=2, F2=13, F3=1, F4=2, F5=2 auditoría, F0 y audits intermedios completan el conteo) |
| Audits ejecutados | 5 (F0/audit-sdd1, F1, F2, F3, F4) + 1 audit final |
| Fases ejecutadas | 6 (F0..F5) |
| Tiempo aproximado | varias horas equivalentes (sin timestamp por D8.5 idempotencia) |

## Inventario completo

### /SDD2.1D/devs/_bootstrap/

| Archivo | Líneas | Propósito |
|---|---|---|
| `audit-sdd1.md` | 166 | Auditoría Fase 0 del material fuente SDD 1.0: inventario de 161 archivos, 14 patrones repetibles, 14 inconsistencias, mapa de cobertura por capítulo y recomendaciones de generalización. |
| `audit-fase-1.md` | 263 | Auditoría de las plantillas intake (BRIEF + README). Detectó el P0-001 de MAUI y aplicó la corrección antes del cierre. |
| `audit-fase-2.md` | 308 | Auditoría de las 13 reglas constructivas. Verificó D1–D8, completitud §1.2, §8, cobertura de tipos D8, presencia de prompt-snippets ejecutables y cadena D6 sin huérfanos. |
| `audit-fase-3.md` | 221 | Auditoría del master-prompt. Verificó autocontención, plan-then-confirm, delegación, manejo de ambigüedad, handoff. |
| `audit-fase-4.md` | 234 | Auditoría del marco teórico y la guía de usuario. Verificó ≥14 capítulos, 6 pasos del flujo, mini-casos, bibliografía. |
| `audit-final.md` | (esta fase) | Auditoría final cross-doc del bootstrap. Consolida el veredicto del template. |
| `bootstrap-changelog.md` | (esta fase) | Este documento. Trazabilidad histórica del bootstrap. |

### /SDD2.1D/devs/intake/

| Archivo | Líneas | Propósito |
|---|---|---|
| `PROJECT-BRIEF-template.md` | 373 | Plantilla del documento de necesidades de negocio. Captura visión, problema, objetivos, alcance, métricas, restricciones. Input bloqueante de la cadena D6. |
| `PROJECT-README-template.md` | 552 | Plantilla del documento técnico de construcción. Captura tipo de proyecto (D8), stack, arquitectura, testing, CI/CD, samples. Gobierna las variantes de especialidad de las 12 categorías SDD. |

### /SDD2.1D/devs/rules/

| Archivo | Líneas | Categoría |
|---|---|---|
| `_root_rules.md` | 346 | Reglas raíz transversales: invariantes D1–D8, nomenclatura, idioma, checklist genérica, cadena D6 conceptual. |
| `00_rules_contexto.md` | 364 | Contexto del proyecto, glosario, stakeholders, visión. |
| `01_rules_necesidades_negocio.md` | 382 | Necesidades de negocio y reglas de negocio. |
| `02_rules_especificacion_funcional.md` | 427 | Casos de uso y especificación funcional. |
| `03_rules_ux_ui_dx.md` | 502 | UX, UI y DX. |
| `04_rules_prompts_ai.md` | 438 | Prompts de IA y artefactos prompt-engineered. |
| `05_rules_arquitectura_tecnica.md` | 434 | Arquitectura técnica y ADRs (corrección obligatoria: ADRs individuales). |
| `06_rules_backlog_tecnico.md` | 440 | Backlog técnico, US y BT (corrección obligatoria: IDs uniformes de 2 dígitos). |
| `07_rules_plan_sprint.md` | 405 | Plan de iteración y sprints. |
| `08_rules_calidad_y_pruebas.md` | 452 | Calidad y pruebas. |
| `09_rules_devops.md` | 435 | DevOps, CI/CD y publicación de artefactos (stack-agnóstico). |
| `10_rules_developer_guide.md` | 419 | Guía de desarrollador del proyecto. |
| `11_rules_examples.md` | 484 | Samples y ejemplos por nivel/capacidad (corrección obligatoria: prohibido nombrar por dominio). |

### /SDD2.1D/devs/orchestrator/

| Archivo | Líneas | Propósito |
|---|---|---|
| `master-prompt.md` | 621 | Prompt orquestador autocontenido del proyecto. Despacha la generación de `/SDD2.1D/docs/` por fases A–H trazables a la cadena D6, con auditoría entre fases. |

### /SDD2.1D/devs/guides/

| Archivo | Líneas | Propósito |
|---|---|---|
| `marco-teorico-sdd_v1.0.md` | 1.747 | Marco teórico SDD 2.1: 14 capítulos. Spec-First / Spec-Anchored / Spec-as-Source, cadena D6, 8 tipos D8, ADRs, calidad, DevOps, prompting colaborativo, bibliografía. |

### /SDD2.1D/guides/

| Archivo | Líneas | Propósito |
|---|---|---|
| `guia-usuario-sdd2.0_v1.0.md` | 1.206 | Guía de usuario del template. 6 pasos del flujo, 3 mini-casos por tipo D8, glosario, comparación SDD 1.0 vs 2.0. |

### /SDD2.1D/docs/

Carpeta vacía con `.gitkeep`. Destino del orquestador del proyecto cuando un usuario consuma el template para un proyecto nuevo.

---

## Fases ejecutadas

### Fase 0 — Auditoría material fuente

- Subagente despachado: auditor independiente del fuente SDD 1.0.
- Producto: `audit-sdd1.md` (166 líneas).
- Métricas: 161 archivos markdown del fuente inventariados, 14 patrones repetibles identificados, 14 inconsistencias detectadas, mapa de cobertura por capítulo construido.
- Observaciones: identificó como vocabulario prohibido a propagar al template: "Motor DSL", "ESC/POS", "Bluetooth", ".NET 10", "NuGet", "MAUI" (fuera del literal D8), "impresora térmica". Estableció D7 con su excepción de "referencia formal histórica al fuente".

### Fase 1 — Plantillas intake

- Subagentes despachados: 1 generador de BRIEF, 1 generador de README, más auditor independiente.
- Productos: `PROJECT-BRIEF-template.md` (373 líneas), `PROJECT-README-template.md` (552 líneas).
- Métricas: BRIEF con 14 secciones + checklist; README con 17 secciones + checklist §17. README §1 declara los 8 valores D8.
- Defectos detectados: 1 P0 (MAUI fuera de literal en README), 1 P1 (tildes ausentes en README), 3 P2 (capitalización, derivación kebab, negritas categóricas).
- Acción correctiva: P0 resuelto en la fase. P1 también resuelto. P2 documentados.

### Fase 2 — Reglas constructivas (13)

- Subagentes despachados: 13 (uno por categoría) + auditor independiente.
- Productos: `_root_rules.md` + `00_rules_contexto.md` a `11_rules_examples.md`.
- Métricas: ~5.628 líneas en total, 9 secciones por regla, §1.2 + §8 verificados en los 13 archivos.
- Defectos detectados: 0 P0, 5 P1 (cobertura D8 incompleta en §7, neutralidad de stack-hints, otros), 6 P2.
- Lecciones aplicadas del fuente: 9 lecciones de `audit-sdd1.md` aplicadas explícitamente (IDs uniformes en US/BT, ADRs individuales, nombrado de samples por nivel, sufijo `_v<X.Y>.md`, etc.).

### Fase 3 — Master-prompt

- Subagente despachado: ingeniero de prompts.
- Producto: `master-prompt.md` (621 líneas).
- Métricas: §1 a §15 + apéndices. Plan-then-confirm con detención entre fases. Auditoría entre fases declarada (§10). Cadena D6 explicitada (§7 y §14).
- Defectos detectados: 0 P0, 2 P1 (acoplamiento residual al bootstrap, ambigüedad gramatical de `tiene_ui_final`), 3 P2.

### Fase 4 — Marco teórico + Guía

- Subagentes despachados: 1 redactor del marco teórico, 1 redactor de la guía + auditor independiente.
- Productos: `marco-teorico-sdd_v1.0.md` (1.747 líneas), `guia-usuario-sdd2.0_v1.0.md` (1.206 líneas).
- Métricas: marco con 14 capítulos, glosario y bibliografía. Guía con 9 secciones, 6 pasos del flujo del usuario, 3 mini-casos (rest-api, library, mobile-app-maui), comparación SDD 1.0 vs 2.0 en §9.2.
- Defectos detectados: 0 P0, 3 P1 (cadena D6 abreviada en glosario, falta Cockburn en bibliografía, otros), 4 P2.

### Fase 5 — Auditoría final + Changelog

- Auditor independiente final + generación del changelog.
- Productos: `audit-final.md`, `bootstrap-changelog.md` (este documento).
- Métricas: 0 P0 acumulados, 7 P1 abiertos, 11 P2 abiertos.
- Veredicto: CERRADO CON OBSERVACIONES.

---

## Decisiones tomadas durante el bootstrap

| ID | Decisión | Justificación |
|---|---|---|
| D1 | Idioma español rioplatense neutro técnico con tildes, sin emojis, sin negritas decorativas | Coherencia con la cátedra y reproducibilidad del template fuera del contexto rioplatense |
| D2 | UTF-8 + LF + `.md` + `#` + `-` | Portabilidad cross-OS y diff-friendliness en Git |
| D3 | Kebab-case en filenames | Convención uniforme del fuente; legible en URLs y CLI |
| D4 | Nomenclatura `_v<X.Y>.md` | Versionado por archivo, corrige el problema del fuente donde varios artefactos omitían versión |
| D5 | Artefactos en v1.0 inicial | Convención: la primera emisión siempre es `_v1.0`; los cambios posteriores siguen la política del repositorio del proyecto |
| D6 | Cadena Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline declarada literalmente | Hilo conductor del template. Sin huérfanos. |
| D7 | Prohibidos Motor DSL / ESC-POS / MAUI (excepto literal `mobile-app-maui`) / Bluetooth / NuGet / .NET 10 / impresora térmica como ejemplos canónicos. Excepción aceptable: referencias formales al fuente como antecedente histórico | Stack-agnóstico. Permite citar al fuente solo cuando se justifica una corrección. |
| D8 | 8 tipos D8: `library`, `web-monolith`, `web-microservices`, `desktop-app`, `mobile-app-maui`, `rest-api`, `cli-tool`, `worker-service` | Cobertura amplia de artefactos de software actuales; cerrada para evitar inflación tipológica |
| D8.5 | Idempotencia: sin timestamps en artefactos del bootstrap | Reproducibilidad: el bootstrap debe poder regenerarse sin diff espurio por reloj |
| F1-P0-001 | Corrección de menciones de `MAUI` y `.NET MAUI` en PROJECT-README-template fuera del literal D8 | Violación de D7 detectada en audit Fase 1 |
| F1-P1-001 | Restitución masiva de tildes en PROJECT-README-template (~600+ tildes en `sección`, `técnico`, `arquitectónico`, etc.) | Violación de D1 detectada en audit Fase 1 |
| F2-D-001 | Convención obligatoria: ADRs individuales bajo `adrs/`, prohibido consolidar | Lección del fuente Motor DSL (audit-sdd1.md), aplicada en 05 |
| F2-D-002 | IDs de US/BT/EP con dos dígitos uniformes | Lección del fuente Motor DSL (mezcla `US-01` y `BT-001`), aplicada en 06 |
| F2-D-003 | Apertura de artefactos con H1 directo, prohibido `--` previo | Lección del fuente, aplicada en 07 |
| F2-D-004 | Naming de samples por nivel/capacidad, prohibido por dominio | Lección del fuente (`multa`, `multaapp-nuget`), aplicada en 11 |
| F2-D-005 | `guia-publicacion-<paquete-X>_v<X.Y>.md` parametrizada por tipo de paquete | Generalización del `guia-publicacion-nuget` del fuente, aplicada en 09 |

---

## Defectos P1 abiertos para v1.1 del template

| ID | Origen | Descripción | Acción propuesta |
|---|---|---|---|
| F2-P1-01 | Audit Fase 2 | Cobertura D8 incompleta de `desktop-app` y `mobile-app-maui` en los §7 de las 13 reglas (los ejemplos prosa nunca los usan como dominio ilustrativo). | Reescribir al menos 2 §7 para incorporarlos. |
| F2-P1-02 | Audit Fase 2 | Reordenar enumeración de package managers en 09 §2.2 para que `paquete-nuget` no sea primer ejemplo en la fila `library`. | Reordenamiento cosmético. |
| F2-P1-03 | Audit Fase 2 | Política de versionado de sprint plans no del todo explícita en 07. | Documentar política única. |
| F3-P1-01 | Audit Fase 3 | Acoplamiento residual del master-prompt al bootstrap (referencias por nombre a `audit-sdd1.md` en §8 y §10 sin incorporar el listado al cuerpo). | Incorporar el listado de términos prohibidos al cuerpo. |
| F3-P1-02 | Audit Fase 3 | Definición de `tiene_ui_final` para `web-microservices` ambigua sintácticamente. | Reescribir la fila. |
| F4-P1-01 | Audit Fase 4 | Cadena D6 abreviada en glosario marco L1612 (omite RN, Sprint, Pipeline en esa única línea). | Uniformar al patrón canónico completo. |
| F4-P1-02 | Audit Fase 4 | Falta Cockburn en bibliografía del marco. | Agregar entrada Cockburn, A. (2001) en §14.1. |

## Defectos P2 postergables

| ID | Origen | Descripción |
|---|---|---|
| F1-P2-01 | Audit Fase 1 | Inconsistencia de capitalización del campo "Nombre del proyecto" entre BRIEF y README. |
| F1-P2-02 | Audit Fase 1 | No se documenta regla de derivación de `<nombre-kebab>` a partir del nombre legible. |
| F1-P2-03 | Audit Fase 1 | Uso de negritas en valores categóricos en BRIEF §4 y §8. Aceptado en fase. |
| F2-P2-01 | Audit Fase 2 | Reordenamiento de package managers en 09 §2.2 (cosmético). |
| F2-P2-02 | Audit Fase 2 | 00 §6 dice "Motor DSL" donde otras reglas usan "fuente"; sugerencia de uniformar. |
| F3-P2-01 | Audit Fase 3 | Microdesalineamiento de umbral `acuerdo-equipo` entre §6 y §14 del master-prompt. |
| F3-P2-02 | Audit Fase 3 | Reanudación post-ambigüedad sin contrato explícito de estado. |
| F3-P2-03 | Audit Fase 3 | Fase H del master-prompt mezcla README raíz con handoff. |
| F4-P2-01 | Audit Fase 4 | Cross-link mal apuntado en guía §9.2 (debería citar marco §1.6). |
| F4-P2-02 | Audit Fase 4 | Entrada de glosario `RCL` cita `.NET / Blazor`. |
| F4-P2-03 | Audit Fase 4 | Diagrama ASCII del marco §3.8 sin tildes por compatibilidad de render. Aceptable. |
| F4-P2-04 | Audit Fase 4 | Entrada ISO en §14.3 con formato APA no estricto. |

## Lecciones aprendidas del bootstrap

- La auditoría entre fases salvó al menos un P0 (F1-P0-001 MAUI fuera de literal) que de otra forma habría contaminado downstream las 13 reglas constructivas.
- El despacho por subagente especializado (uno por categoría en Fase 2) escala mejor que un único agente generalista: cada subagente produjo un artefacto de ~430 líneas con estructura uniforme verificable por grep.
- La invariante D7 con su excepción de "referencia formal histórica" demostró ser pragmática: permitió documentar correcciones del fuente sin contaminar el template como ejemplo canónico.
- La cadena D6 como hilo conductor declarado literalmente en marco + master-prompt + reglas raíz garantiza cobertura, pero su verificación requiere búsqueda case-insensitive amplia, ya que las versiones abreviadas en glosarios fueron un foco recurrente de P1.
- La fase 0 (auditoría del fuente) es decisiva: 9 de las decisiones constructivas de Fase 2 son aplicaciones explícitas de lecciones del fuente.
- El requisito de idempotencia (D8.5) excluyendo timestamps fue acertado: el bootstrap puede regenerarse total o parcialmente sin diff por reloj del sistema.

## Próximos pasos para el usuario del template

1. Validar el template ejecutándolo en un proyecto piloto real, completando ambas plantillas de intake y disparando el `master-prompt.md` para generar `/SDD2.1D/docs/`.
2. Recolectar feedback estructurado: registro de ambigüedades del master-prompt, completitud de las 13 reglas, ejemplos faltantes por tipo D8.
3. Iterar a v1.1 cerrando los 7 defectos P1 documentados arriba (especialmente F2-P1-01 sobre cobertura agregada de `desktop-app` y `mobile-app-maui`, y F3-P1-01 sobre autocontención del master-prompt).
4. Publicar v1.1 como versión consolidada de uso general.
5. Considerar v2.0 si emergen requisitos para tipos D8 adicionales (`embedded-firmware`, `data-pipeline`, etc.) — implica reabrir D8 según el procedimiento descripto en la guía §9.
