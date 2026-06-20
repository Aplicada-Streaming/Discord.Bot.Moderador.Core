# Auditoría de Fase 3 — Master-prompt

**Fase:** 3 (Master-prompt del orquestador)
**Auditor:** Auditor independiente del master-prompt
**Alcance:** /SDD2.2D/devs/orchestrator/master-prompt.md
**Versión:** 1.0
**Fecha:** 2026-05-17

---

## 1. Resumen ejecutivo

El master-prompt está completo, coherente y operativamente ejecutable. Cubre las 17 secciones obligatorias (§0 a §16), declara el principio de delegación con sus cuatro razones de fondo, implementa pattern de detención/pregunta/reanudación, gating de la categoría 04, handoff con detención explícita y tabla de adaptabilidad para los 8 tipos D8. No se encontraron menciones D7 prohibidas (MAUI aparece solo como literal `mobile-app-maui` permitido por D8). Veredicto preliminar: APROBADO CON OBSERVACIONES. Conteo: 0 P0, 2 P1, 3 P2.

---

## 2. Referencias a archivos existentes

| Path referenciado | Existe | Línea(s) | Veredicto |
| --- | --- | --- | --- |
| `/SDD2.2D/devs/intake/PROJECT-BRIEF-<nombre-kebab>_v1.0.md` | Plantilla `PROJECT-BRIEF-template.md` presente; el master-prompt usa nombre derivado en runtime | 7, 18, 73, 301, 371, 485 | OK (plantilla de origen presente; instanciación es runtime) |
| `/SDD2.2D/devs/intake/PROJECT-README-<nombre-kebab>_v1.0.md` | Plantilla `PROJECT-README-template.md` presente | 7, 19, 74, 302, 372, 485 | OK |
| `/SDD2.2D/devs/rules/_root_rules.md` | Sí | 214, 498, 506, 508 | OK |
| `/SDD2.2D/devs/rules/00_rules_contexto.md` | Sí | 58, 202 | OK |
| `/SDD2.2D/devs/rules/01_rules_necesidades_negocio.md` | Sí | 203 | OK |
| `/SDD2.2D/devs/rules/02_rules_especificacion_funcional.md` | Sí | 204, 359, 373 | OK |
| `/SDD2.2D/devs/rules/03_rules_ux_ui_dx.md` | Sí | 205 | OK |
| `/SDD2.2D/devs/rules/04_rules_prompts_ai.md` | Sí | 206 | OK |
| `/SDD2.2D/devs/rules/05_rules_arquitectura_tecnica.md` | Sí | 207 | OK |
| `/SDD2.2D/devs/rules/06_rules_backlog_tecnico.md` | Sí | 208 | OK |
| `/SDD2.2D/devs/rules/07_rules_plan_sprint.md` | Sí | 137, 209 | OK |
| `/SDD2.2D/devs/rules/08_rules_calidad_y_pruebas.md` | Sí | 210 | OK |
| `/SDD2.2D/devs/rules/09_rules_devops.md` | Sí | 211 | OK |
| `/SDD2.2D/devs/rules/10_rules_developer_guide.md` | Sí | 212 | OK |
| `/SDD2.2D/devs/rules/11_rules_examples.md` | Sí | 213 | OK |
| `/SDD2.2D/devs/_bootstrap/audit-sdd1.md` | Sí | 330 | OBSERVADO (acoplamiento bootstrap, ver §9) |
| `/SDD2.2D/devs/_bootstrap/audit-fase-2.md` | Sí | 330, 460 | OBSERVADO (acoplamiento bootstrap, ver §9) |

Todos los archivos de reglas (13) y las plantillas de intake existen. No hay path roto. Aparecen dos referencias a archivos del bootstrap que el master-prompt cita textualmente como fuente de vocabulario prohibido y como referencia estructural de audit; se discute en §9.

---

## 3. Conformidad D1–D8

| Invariante | Cumple | Evidencia |
| --- | --- | --- |
| D1 — Español rioplatense, tildes, sin emojis, sin negritas decorativas | Sí | L30, L185, L367 declaran y aplican el registro. Lectura global confirma uso correcto de tildes y ausencia de emojis y de negritas decorativas (las negritas existentes son de cabecera estructural, no decorativas) |
| D2 — UTF-8, LF, `.md`, `#`, `-` | Sí | Archivo `.md`, cabeceras con `#`, listas con `-`. L181-182 redeclaran encoding/LF como invariantes |
| D3 — kebab-case en nombre `master-prompt.md` | Sí | El archivo se llama `master-prompt.md` (L3) |
| D4 — N/A (este archivo no produce artefactos por sí) | N/A | Aplicable: el archivo es ejecutivo, no entregable taxonómico; sin embargo declara D4 para sus subagentes (L189) |
| D5 — Versionado v1.0 | Sí | L4 `Versión: 1.0`, §16 tabla de control de cambios con entrada `1.0 / 2026-05-17` |
| D6 — Cadena de trazabilidad Visión→NB→CU→RN→ADR→US→BT→Sprint→Test→Pipeline | Sí | L40 menciona literal la cadena D6. §7 ejecuta por fases A-H en ese orden. §12 retoma la cadena en la tabla del resumen ejecutivo (L521) |
| D7 — 0 menciones Motor DSL/ESC-POS/MAUI(salvo literal)/Bluetooth/NuGet/.NET 10/impresora térmica | Sí | Búsqueda case-insensitive: 0 matches. MAUI solo aparece en literal `mobile-app-maui` permitido por D8 (L92, L134, L567) |
| D8 — Conjunto cerrado de 8 tipos en §3 y §14 | Sí | §3 L92 enumera los 8 valores. §14 tabla en L562-570 contiene una fila por cada uno de los 8 tipos |

---

## 4. Estructura obligatoria (§0..§16)

| Sección | Presente | Contenido mínimo | Líneas |
| --- | --- | --- | --- |
| Cabecera (archivo, versión, idioma, modo, prerequisitos, salida) | Sí | 6 campos completos | 1-8 |
| §0 Cómo usar | Sí | Prerrequisitos numerados, mecánica plan-then-confirm, idioma | 12-30 |
| §1 Rol del orquestador con principio de delegación | Sí | Verbos, hace/no hace, principio rector con 4 razones | 34-62 |
| §2 Lectura de intake | Sí | Procedimiento + patrón de detención por placeholders ("Pendiente", "TBD", "[Reemplazar]", "[Nombre]", "[YYYY-MM-DD]") | 66-81 |
| §3 Detección del tipo de proyecto | Sí | Conjunto D8 cerrado + reglas determinísticas de derivación de `<nombre-kebab>` | 85-123 |
| §4 Detección de capacidades especiales (gating) | Sí | 10 flags con origen/regla/impacto + bloque de salida obligatorio | 127-167 |
| §5 Recolección de invariantes del proyecto | Sí | 13 invariantes con valor por defecto y notas; separación explícita bootstrap vs proyecto | 170-192 |
| §6 Plan de generación por categoría | Sí | Tabla con 12 categorías + AG-ROOT, columna `Documentos / Subagente / Insumos upstream / Reglas / Path / Audit` | 196-232 |
| §7 Ejecución por fases (orden A..H) | Sí | 8 fases A-H con audits intermedios y detención obligatoria | 236-279 |
| §8 Mecánica de despacho de subagentes | Sí | Esqueleto completo + reglas de construcción + ejemplo aplicado | 282-403 |
| §9 Manejo de ambigüedad | Sí | Pattern detención/pregunta/reanudación con bloque formal + heurísticas | 407-436 |
| §10 Auditoría entre fases | Sí | Perfil auditor, matriz de criterios, niveles P0-P3, estructura de informe, esqueleto de despacho | 439-490 |
| §11 Generación del README raíz | Sí | Despacho AG-ROOT + insumos + salida única + audit final | 494-508 |
| §12 Check-out y handoff a codificación | Sí | Detención explícita + tabla de 6 bloques de resumen + texto literal obligatorio | 512-531 |
| §13 Reglas de no-modificación de intake | Sí | 6 reglas + formato de control de cambios + versionado + archivado | 535-553 |
| §14 Reglas de adaptabilidad por tipo | Sí | Tabla con 8 filas D8 cubriendo 8 columnas (00, 02, 03, 05, 07, 09, 10, 11) + notas | 557-578 |
| §15 Glosario operativo | Sí | 14 términos canónicos (≥10 requeridos) | 582-601 |
| §16 Versionado del prompt | Sí | Control de cambios + reglas de bump de versión | 605-617 |

---

## 5. Principio de delegación de la especialidad

- Declarado en: §1 líneas 53-62
- Razones expuestas: 4/4
  1. L57 — "La especialidad es propiedad del documento que se va a generar, no del orquestador."
  2. L58 — "Si cambia la especialidad ... se modifica un único archivo de reglas y el orquestador no requiere cambios."
  3. L59 — "Permite que el catálogo de especialidades evolucione sin re-publicar el master-prompt."
  4. L60 — "Mantiene al orquestador delgado y delegativo, lo cual baja el riesgo de inconsistencia entre fases."
- Cita textual rectora (L53): "El master-prompt **no asigna** las especialidades de los subagentes. Las **lee** desde la sección §1 de cada `XX_rules_<categoria>.md`".
- Cita operativa (L62): "toda invocación a un subagente se construye copiando el bloque de §1.2 correspondiente, completando los placeholders y citando el archivo de reglas como fuente".
- Reforzado operativamente en:
  - §6 — procedimiento de lectura de las reglas (L222-231) que enuncia los 7 pasos de copia textual.
  - §8 — esqueleto del despacho (L293) usa "leído literal de §1.2".
  - §15 — entrada de glosario "Principio de delegación de la especialidad" (L593).
  - §11 — AG-ROOT también lee su variante de `_root_rules.md` (L498).

Veredicto: cumplido y reforzado en cuatro secciones.

---

## 6. Manejo de ambigüedad

- Pattern documentado: sí (§9, L411-428).
- Detención/pregunta/reanudación: implementado con seis pasos numerados. Paso 1 detención, paso 2 síntesis con bloque formal de 7 campos (subagente, documento, sección, pregunta, justificación, tipo dato esperado, intake destino), paso 3 entrega al usuario con copy-paste literal, paso 4 respuesta de usuario, paso 5 actualización del intake vía §13, paso 6 reanudación desde el documento bloqueado.
- Control de cambios del intake: documentado en §13 L541-552. Lectura solo por defecto (R1), excepción única para respuestas a ambigüedad (R2), formato de tabla `Versión | Fecha | Cambios | Autor` (R3), reglas de bump minor/major (R4), atomicidad (R5), archivado en `_legacy/<YYYY-MM-DD>/` (R6).
- Heurísticas de discriminación ambigüedad vs improvisación: L430-435 distingue 4 casos (datos numéricos faltantes, múltiples interpretaciones, falta de stakeholder/métrica/fecha bloqueante) frente al no-caso "el subagente cree que sería mejor agregar una sección extra".
- Observación: la reanudación menciona "el documento bloqueado" pero no especifica si el subagente recupera estado interno o reinicia la categoría completa con los datos nuevos. Es una ambigüedad operativa menor (P2 en §10.3).

---

## 7. Handoff a codificación

- Detención explícita: sí. L514: "el orquestador NO inicia automáticamente la generación de código. Se detiene y presenta al usuario un resumen ejecutivo".
- Texto literal obligatorio del cierre: L527-529 con la frase formal: "Documentación `/SDD2.2D/docs/` generada y auditada. Antes de avanzar a la generación de código, necesito confirmación explícita del usuario para arrancar el Sprint 1...".
- Refuerzo bloqueante: L531 "El orquestador no escribe código bajo ninguna circunstancia sin recibir la confirmación literal".
- Resumen ejecutivo enumerado: tabla L519-526 con 6 bloques obligatorios:
  1. Documentos generados por categoría (cantidad/tamaño/estado por carpeta + README raíz).
  2. Cobertura de la cadena de trazabilidad (10 eslabones de D6).
  3. Ítems del Sprint 1 listos para codear (US-XX, BT-XX, CU asociado, criterios BDD, componentes 05).
  4. Audits aprobados (los 8 audits de fase A-H con veredicto y path).
  5. Decisiones pendientes (ambigüedades no resueltas, ADRs sin cerrar, por confirmar).
  6. Flags activos (los flags de §4 con su valor final).

Veredicto: handoff implementado correctamente con detención explícita y resumen enumerado.

---

## 8. Gating

| Categoría | Condición de gating | Documentado | Líneas |
| --- | --- | --- | --- |
| 04 prompts AI | `usa_llm == true` derivado de PROJECT-README §15/§13 | Sí | §4 L133, §6 L206, §7 L248, §16 L611 |
| 10 developer guide | `project_type` ∈ {library, rest-api, cli-tool} o `tiene_portal_developers` | Sí | §4 L138 (`tiene_portal_developers`), §6 L212, §14 columna 10 |
| 11 examples | Diferencias materiales por D8 (samples consumidor / SDK / compose / multi-OS / etc.) | Sí | §6 L213, §14 tabla L562-570 fila por tipo, notas L572-578 |
| 03 variante UX vs DX | `tiene_ui_final` calculado de `project_type` | Sí | §4 L134, §6 L205, §14 columna `03 ux/dx variante principal` |
| 02 modelo conceptual | `tiene_persistencia` | Sí | §4 L140, §6 L204 |
| 02 RC-XX-... | modelo > 10 entidades | Sí | §6 L204 |
| 05 extensibilidad | `tiene_extensibilidad` | Sí | §4 L139, §6 L207 |
| 05 ADR de autenticación | `tiene_auth` | Sí | §4 L136 |
| 07 plan-sprint | `equipo_n > 1` vs `equipo_n == 1` (mini-plan) | Sí | §4 L137, §6 L209 |
| 08 guia-testing-extensibilidad | `tiene_extensibilidad` | Sí | §4 L139, §6 L210 |
| Multi-tenant en 05/07/09 | `multi_tenant` | Sí | §4 L135 |
| Compliance | `requiere_compliance` | Sí | §4 L141 |
| Observabilidad reforzada | `tiene_observabilidad_critica` | Sí | §4 L142 |

Observación P2: §6 fila A-00 hace referencia condicional "acuerdo-equipo si equipo_n > 2" mientras §14 dice "acuerdo-equipo si > 1 dev" para cli-tool y "acuerdo-equipo" sin condición para los demás. Hay un microdesalineamiento del umbral (2 vs 1) entre §6 y §14.

---

## 9. Ejecutabilidad standalone

- Dependencias externas (fuera del intake, fuera de las 13 reglas):
  - `_bootstrap/audit-sdd1.md` (L330): referenciado como fuente del listado de términos del dominio prohibido.
  - `_bootstrap/audit-fase-2.md` (L330 y L460): referenciado como fuente de términos prohibidos y como modelo estructural del informe de audit.
- Autocontención: parcial. El usuario final que ejecute el master-prompt necesita haber recibido (o tener acceso a) los dos archivos de bootstrap citados para que el subagente conozca el listado exacto de vocabulario prohibido. Esto no rompe la ejecución, porque el archivo `audit-sdd1.md` describe vocabulario del dominio fuente del bootstrap, no instrucciones operativas; pero el master-prompt asume que el lector acepta esa lista vinculada por referencia. Marcado P1.
- Separación bootstrap vs proyecto: declarada explícitamente en §5 L171-172: "Las invariantes D1-D8 del bootstrap son globales del template (idioma, encoding, kebab-case, versionado, política, trazabilidad D6, prohibición de ejemplos del dominio fuente, conjunto cerrado D8). Sobre esa base, cada proyecto define otro conjunto de invariantes propias". §15 L590 ratifica en el glosario: "Existen invariantes globales del template (D1-D8 del bootstrap) e invariantes del proyecto (las recolectadas en §5)". §15 L595 declara: "Master-prompt ... Es la única instrucción que el usuario ejecuta para obtener /SDD2.2D/docs/. Es autocontenido."
- Separación bootstrap vs master-prompt: §16 L611 cierra el control de cambios con autor "Bootstrap SDD 2.2", reconociendo que el bootstrap generó este archivo pero ya no es el actor. El master-prompt no se autocomporta como prompt del bootstrap.

Observación: la afirmación L595 ("autocontenido") tiene una tensión literal con las dos referencias de L330 y L460 al bootstrap. Es resoluble bajando esas referencias a "notas de inspiración" sin dependencia operativa.

---

## 10. Defectos detectados

### 10.1 P0

Ninguno.

### 10.2 P1

**P1-1 — Acoplamiento residual al bootstrap en §8 y §10.**
- Archivo: master-prompt.md L330 y L460.
- Evidencia: L330 enumera como prohibición "No introducir vocabulario ... (ver el listado de términos prohibidos en la auditoría `_bootstrap/audit-sdd1.md` y `_bootstrap/audit-fase-2.md`)". L460 dice "El informe sigue la estructura del audit de Fase 2 que se generó durante el bootstrap (`/SDD2.2D/devs/_bootstrap/audit-fase-2.md`) como referencia."
- Problema: el master-prompt se declara autocontenido en L595 pero referencia dos archivos del bootstrap como fuente activa de información operativa. Un usuario externo que reciba solo el template podría no recibir `_bootstrap/`.
- Recomendación: o bien incorporar el listado de términos prohibidos directamente como sección del master-prompt (o como invariante de §5), o bien aclarar que las referencias son meramente inspirativas y describir la estructura del informe de audit íntegramente en §10.

**P1-2 — Definición de `tiene_ui_final` para web-microservices ambigua sintácticamente.**
- Archivo: master-prompt.md L134.
- Evidencia: "true cuando `project_type` ∈ {web-monolith, web-microservices con frontend, desktop-app, mobile-app-maui}; web-microservices se subdivide según §3 del README explicite presencia de frontend".
- Problema: `web-microservices con frontend` no es un valor del conjunto D8 cerrado declarado en L92. La regla mezcla un valor D8 puro con un literal compuesto. El criterio de subdivisión "según §3 del README explicite presencia de frontend" tiene además gramática rota ("según §3 del README explicite").
- Recomendación: reescribir como "true cuando `project_type` ∈ {web-monolith, desktop-app, mobile-app-maui}, o cuando `project_type == web-microservices` y el PROJECT-README §3 declara explícitamente la presencia de frontend."

### 10.3 P2

**P2-1 — Microdesalineamiento de umbral de `acuerdo-equipo`.**
- Archivo: master-prompt.md §6 L202 vs §14 L569.
- Evidencia: §6 dice "acuerdo-equipo_v1.0.md (si equipo_n > 2)". §14 para cli-tool dice "acuerdo-equipo si > 1 dev"; para library/web-monolith/web-microservices/desktop/mobile/rest-api/worker-service lo lista sin condición.
- Recomendación: fijar el umbral único (probablemente `equipo_n > 1` para consistencia con la lógica de §4 L137).

**P2-2 — Reanudación post-ambigüedad sin contrato de estado.**
- Archivo: master-prompt.md §9 L428.
- Evidencia: "El subagente se reanuda desde el documento bloqueado, con los nuevos datos incorporados".
- Problema: no se especifica si el subagente recupera contexto previo o reinicia la categoría con los datos nuevos. Para subagentes stateless por defecto, conviene declararlo explícitamente.
- Recomendación: agregar una frase del tipo "El orquestador re-despacha al subagente con el mismo prompt original más el bloque `RESPUESTA A AMBIGÜEDAD` adjunto".

**P2-3 — Fase H mezcla README raíz con handoff sin separar audits.**
- Archivo: master-prompt.md §7 L273-276.
- Evidencia: "Fase H — README raíz y handoff. 1. AG-ROOT redacta ... 2. Audit final consolidado ... 3. Se ejecuta §12".
- Problema: §12 no es estrictamente un paso de fase H sino un check-out posterior. Confunde la frontera entre "audit final consolidado" (que cierra docs) y "handoff" (que abre la siguiente etapa). No es bloqueante pero invita a confusión.
- Recomendación: renombrar el paso 3 como "Fase post-H — Check-out y handoff (§12)" o mover §12 fuera de la fase H.

---

## 11. Veredicto final

**APROBADO CON OBSERVACIONES.**

El master-prompt es ejecutable por un usuario final que tenga los dos intake completos y acceso a las 13 reglas. Cumple D1-D8, contiene las 17 secciones obligatorias, declara y refuerza el principio de delegación de la especialidad, implementa pattern de ambigüedad con detención/pregunta/reanudación, separa bootstrap vs proyecto, y materializa detención explícita en el handoff a codificación.

Hallazgos:
- 0 P0
- 2 P1 (acoplamiento bootstrap en L330/L460; ambigüedad gramatical en L134 sobre web-microservices)
- 3 P2 (umbral `acuerdo-equipo`, contrato de reanudación, separación fase H vs §12)

Condición para promover a APROBADO sin observaciones: incorporar el listado de términos prohibidos al cuerpo del master-prompt (P1-1) y reescribir la fila `tiene_ui_final` (P1-2).

---

**Fin del informe de auditoría de Fase 3**
