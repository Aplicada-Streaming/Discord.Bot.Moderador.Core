# Auditoría de Fase D — Backlog técnico (06) y Plan de sprint (07)

**Fase auditada:** D (categorías 06 backlog-tecnico y 07 plan-sprint)
**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Layout:** caso degenerado (06 y 07 directo bajo `SDD2.2D/docs/`, sin `proyectos/<kebab>/`)
**Alcance:** `SDD2.2D/docs/06_backlog-tecnico/` (product-backlog, backlog-tecnico, definition-of-ready, README) y `SDD2.2D/docs/07_plan-sprint/` (mini-plan, README)
**Auditor:** Arquitecto de Soluciones + QA Senior, independiente (no participó de la generación de la Fase D)
**Fecha:** 2026-06-20
**Reglas de referencia:** `06_rules_backlog_tecnico.md` v1.2; `07_rules_plan_sprint.md` v1.2
**Insumos upstream:** 01 (NB-01..NB-07), 02 (CU-01..CU-16, RN, RC), 05 (ADR-01..ADR-13, componentes, modelo lógico, extensibilidad), intake y manifiesto de `SDD2.2D/devs/intake/`

---

## 1. Resumen ejecutivo

La Fase D del proyecto discord-bots-admin está completa y conforme. Los seis entregables existen, cumplen su estructura obligatoria y mantienen trazabilidad bidireccional intacta: los 16 CU y las 7 NB tienen al menos una US; ninguna US es huérfana de CU; cada BT declara fuente upstream en 05 y al menos una US consumidora o justificación de infraestructura; y cada item del mini-plan referencia un ID real de 06 sin invención. El modo inline (19 US bajo 20; 17 BT entre el piso de 8 y el umbral de 30) y el modo mini-plan de 1 dev (cuatro artefactos de equipo ausentes) están correctamente aplicados. Conteo de hallazgos: P0 = 0; P1 = 0; P2 = 1; P3 = 3. **Veredicto: APROBADO.** La cadena de fase puede promoverse a 08.

---

## 2. Matriz de conformidad D1–D8 por documento

Leyenda: OK = conforme; n/a = no aplica. Documentos: PB = product-backlog_v1.0.md; BT = backlog-tecnico_v1.0.md; DoR = definition-of-ready_v1.0.md; R6 = README 06; MP = mini-plan_v1.0.md; R7 = README 07.

| Criterio | PB | BT | DoR | R6 | MP | R7 |
| --- | --- | --- | --- | --- | --- | --- |
| D1 idioma rioplatense con tildes/eñes | OK | OK | OK | OK | OK | OK |
| D2 filenames ASCII, slug kebab lowercase, sufijo `_v1.0` con guion bajo | OK | OK | OK | OK | OK | OK |
| D3 identificadores US/BT/EP de dos dígitos uniformes (sin tres dígitos) | OK | OK | OK | OK | OK | OK |
| D4 codificación UTF-8 | OK | OK | OK | OK | OK | OK |
| D5 sin emojis ni negritas decorativas | OK | OK | OK | OK | OK | OK |
| D6 trazabilidad upstream (NB/CU/ADR) | OK | OK | OK | OK | OK | OK |
| D7 sin stack hardcodeado ni vocabulario del dominio fuente | OK | OK | OK | OK | OK | OK |
| D8 coherencia con tipo web-monolith | OK | OK | OK | OK | OK | OK |

Notas de evidencia:
- D2: ningún archivo usa el patrón prohibido `.v1.0.md`; todos usan `_v1.0.md`. Sin doble separador en 07 (`mini-plan_v1.0.md` correcto).
- D3: barrido regex sobre 06 y 07 no detecta ningún identificador de tres dígitos (`US-001`, `BT-001`, etc.). 19 US, 17 BT y 8 EP, todos de dos dígitos.
- D5: el único símbolo no-ASCII recurrente es `↔` en "matriz BT↔US↔CU", terminología sancionada por la propia regla 06 (§4.3 y §6); no es emoji decorativo. Las negritas se limitan a los bloques de cabecera de §4.1 y a las metalíneas inline de cada US/BT (estructurales, no decorativas).
- D7: barrido por `.NET`, Blazor, MudBlazor, EF Core, SQLite, Discord.Net, Raspberry, armv7, ESC-POS, impresora térmica, Bluetooth y DSL sobre 06 y 07: cero coincidencias. Los términos "Discord", "gateway", "snowflake", "token de bot/de acceso", "canal de eventos", "intents" se usan como dominio legítimo. (El stack concreto aparece solo en el manifiesto e intake, que son insumos y no entregables de la Fase D.)

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 product-backlog_v1.0.md (§4.2: cinco secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 (H1 + metadatos) | OK |
| 1. Objetivos del producto y MVP | OK |
| 2. Épicas (tabla EP-XX) | OK (EP-01..EP-08) |
| 3. Historias por épica (tabla US-XX) | OK (19 US, tabla + inline) |
| 4. Métricas de avance | OK |
| 5. Refinamiento | OK |

### 3.2 backlog-tecnico_v1.0.md (§4.3: tres secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Épicas técnicas (objetivo, alcance, fuente upstream, BT) | OK (EPT-01..EPT-07) |
| 2. BT por épica (tabla con tipo, prioridad, estimación, fuente, dependencias, criterios) | OK (17 BT) |
| 3. Trazabilidad BT↔US↔CU | OK (matriz completa, 17 filas) |

### 3.3 definition-of-ready_v1.0.md (§4.6: cuatro secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Criterios DoR para US (5–8) | OK (7 criterios) |
| 2. Criterios DoR para BT (4–6) | OK (5 criterios) |
| 3. Excepciones admitidas + quién aprueba | OK (3 excepciones) |
| 4. Aprobador | OK (Scrum Master AG-06) |

### 3.4 mini-plan_v1.0.md (07 modo 1 dev)

| Elemento esperado | Estado |
| --- | --- |
| Cabecera §4.1 (H1 directo, sin `--`) | OK |
| Información general (equipo, unidad de estimación, capacidad) | OK (§1) |
| Sprint goal — una sola frase orientada a valor | OK (§2, sin bullets) |
| Lista de items del primer sprint | OK (§4) |
| Riesgos con mitigación (≥2) | OK (§6, cuatro riesgos) |
| Trazabilidad a CU y NB | OK (§3 y §7) |
| DoD referenciada (no redefinida) | OK (§5, referencia a 08) |
| Bitácora de avance | OK (§8) |

### 3.5 READMEs de sección

| README | Estado |
| --- | --- |
| 06/README.md (índice, épicas, US Must, BT prioritarias, DoR vigente) | OK |
| 07/README.md (modo mini-plan, omisión justificada de los 4 artefactos, índice) | OK |

---

## 4. Cumplimiento de §6 por categoría

### 4.1 Regla 06 — los 11 ítems de §6

| # | Criterio §6 (regla 06) | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | product-backlog con 5 secciones y épicas EP-XX | OK | EP-01..EP-08; secciones 1–5 presentes |
| 2 | backlog-tecnico con 3 secciones y matriz BT↔US↔CU completa | OK | EPT-01..EPT-07; matriz §3 de 17 filas |
| 3 | DoR con US (5–8) y BT (4–6), excepciones y aprobador | OK | 7 US / 5 BT; 3 excepciones; aprobador AG-06 |
| 4 | dos dígitos uniformes, sin rastros de BT-001 | OK | barrido regex: 0 identificadores de tres dígitos |
| 5 | ninguna US huérfana de CU | OK | las 19 US declaran ≥1 CU |
| 6 | cada BT con fuente upstream y ≥1 US consumidora o justificación de infra | OK | 15 BT con US consumidora; BT-01 y BT-17 justificadas como infraestructura compartida con ADR |
| 7 | MoSCoW no 100% Must | OK | 10 Must / 8 Should / 1 Could (53% Must por cantidad, 59% por SP) |
| 8 | Given/When/Then ≥2 escenarios en US Must/Should | OK | las 18 US Must/Should tienen ≥2 escenarios; US-01 y US-12 tienen 3 |
| 9 | umbrales de archivos individuales aplicados | OK | 19<20 → US inline; 17<30 → BT inline; sin carpetas `historias-usuario/` ni `tareas-tecnicas/` |
| 10 | DoR no solapa la DoD | OK | §5 de la DoR delimita explícitamente con la DoD de 08 |
| 11 | sin stacks concretos ni vocabulario del dominio fuente | OK | barrido D7 sin coincidencias |

### 4.2 Regla 07 — modo 1 dev de §6

| Criterio §6 (regla 07) aplicable a 1 dev | Estado | Evidencia |
| --- | --- | --- |
| Existe `mini-plan_v1.0.md` y NO existen los 4 artefactos de equipo | OK | solo `mini-plan_v1.0.md` y `README.md` en 07; ausencia correcta de plan-iteracion, review, retro y velocidad |
| Sprint goal como UNA sola frase orientada a valor, sin bullets | OK | §2 del mini-plan: frase única declarativa |
| Trazabilidad a CU y NB | OK | §3 y §7 mapean CU y NB por rebanada |
| ≥2 riesgos con mitigación concreta | OK | §6: cuatro riesgos con probabilidad, impacto y mitigación |
| Nomenclatura sin doble separador | OK | `mini-plan_v1.0.md` |
| Sin apertura con `--` antes del H1 | OK | primera línea es `# Mini-plan de sprints — discord-bots-admin` |
| DoD referenciada, no redefinida | OK | §5 referencia la DoD canónica de 08 como pendiente |

---

## 5. Coherencia cross-doc

### 5.1 Cobertura NB → US (bidireccional)

| NB | US que la realizan |
| --- | --- |
| NB-01 | US-01, US-02, US-03, US-19 |
| NB-02 | US-04 |
| NB-03 | US-05 |
| NB-04 | US-06, US-07, US-08, US-19 |
| NB-05 | US-09, US-10, US-11, US-12, US-13 |
| NB-06 | US-14, US-15 |
| NB-07 | US-16, US-17, US-18, US-19 |

Las siete NB tienen ≥1 US. Sin NB huérfana.

### 5.2 Cobertura CU → US (bidireccional)

| CU | US | CU | US |
| --- | --- | --- | --- |
| CU-01 | US-01, US-02 | CU-09 | US-10 |
| CU-02 | US-03 | CU-10 | US-11 |
| CU-03 | US-04 | CU-11 | US-12, US-13, US-19 |
| CU-04 | US-05 | CU-12 | US-14 |
| CU-05 | US-06, US-19 | CU-13 | US-15 |
| CU-06 | US-07 | CU-14 | US-16 |
| CU-07 | US-08 | CU-15 | US-17 |
| CU-08 | US-09 | CU-16 | US-18 |

Los 16 CU tienen ≥1 US. Sin CU huérfano. Las 19 US declaran al menos un CU (sin US huérfana de CU).

### 5.3 IDs no duplicados

US-01..US-19 (19, sin huecos ni repetidos), BT-01..BT-17 (17, sin huecos ni repetidos), EP-01..EP-08, EPT-01..EPT-07. Numeración densa y consistente entre product-backlog y backlog-tecnico.

### 5.4 Items del mini-plan ↔ 06 (sin invención de IDs)

Las US referenciadas por el mini-plan (US-01..US-19) y las BT (BT-01..BT-17) existen todas en 06 con el identificador exacto. Cero IDs inventados. Los 16 CU, las 7 NB y los ADR-01..ADR-13 (salvo ADR-10, omisión justificada en 05) citados por el plan corresponden a artefactos reales de 02 y 05.

### 5.5 Rebanadas verticales ↔ intake §15

La secuencia R1..R7 del mini-plan reproduce fielmente el delivery del intake §15: R1 walking skeleton (registrar servidor con token cifrado, recibir mensaje, evaluar ráfaga, reportar en simulación); R2 baneo con borrado retroactivo y reporte real; R3 contenido por expresión regular; R4 revisión de incidentes y desbaneo; R5 exenciones; R6 acciones adicionales (timeout/expulsión/rol); R7 configuración dirigida por descriptores. Coherente con §15 e §4 MoSCoW del intake. El walking skeleton de R1 invoca la excepción de DoR §3 de forma documentada.

### 5.6 Enlaces internos

Los enlaces relativos de ambos READMEs y de los documentos de 06 (cruce product-backlog↔backlog-tecnico↔DoR) y de 07 hacia 06, 02, 01 y 05 apuntan a archivos existentes. Resolución verificada contra el árbol de `SDD2.2D/docs/`.

---

## 6. Hallazgos enumerados

No se detectaron hallazgos P0 ni P1. Los hallazgos abiertos son de severidad media y baja, no bloqueantes.

| # | Nivel | Archivo | Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| H-01 | P2 | product-backlog_v1.0.md | §1 Objetivos del producto | §4.2 de la regla 06 pide "una a tres oraciones" para los objetivos; el párrafo de objetivos se extiende a dos oraciones muy largas que enumeran casi todo el MVP, rozando el límite de concisión. | Acortar a un enunciado de propósito + MVP en 1–3 oraciones; mover el detalle del listado Must a la tabla de la sección 3. No bloqueante. |
| H-02 | P3 | mini-plan_v1.0.md | §4 lista de items / §3 | La descripción corta de BT-13 en el plan ("Cifrado de la credencial del servidor en reposo") difiere del título canónico de 06 ("Cifrado de tokens en reposo con clave maestra"); el ID es correcto y el sentido es equivalente, pero el texto no es literal. | Alinear la descripción corta con el título de 06 para lectura uniforme. Cosmético. |
| H-03 | P3 | product-backlog_v1.0.md | §4 Métricas de avance | La fila "Won't (v1.0)" figura en 0; los Won't del intake §9 (multi-servidor, motor de IA, etc.) viven como exclusiones en el intake, no en el backlog. Es admisible, pero la tabla MoSCoW de la regla menciona "Won't documentado para no perderse". | Opcional: agregar una nota o una mini-sección que enlace los Won't del intake §9, para cerrar el cuadro MoSCoW. Sin impacto en trazabilidad. |
| H-04 | P3 | mini-plan_v1.0.md | §3 nota / §4 | R1 declara 68 SP nominales y aclara que el esfuerzo real es una fracción por tomar BT en versión mínima; el criterio rector es valor demostrable, no techo de velocity. Es coherente con la regla (1 dev, sin historial), pero un lector externo podría confundir el 68 con un compromiso. | Mantener la aclaración ya presente; opcionalmente resaltar que 68 SP es referencia de tamaño, no commitment. Estilo. |

---

## 7. Veredicto final

**APROBADO.**

La Fase D (categorías 06 y 07) del proyecto discord-bots-admin cumple los criterios de aceptación de ambas reglas constructivas, conserva la trazabilidad D6 íntegra en ambos sentidos (NB→US, CU→US, BT→fuente upstream, mini-plan→06) y respeta el caso degenerado de layout aplanado, el modo inline de 06 y el modo mini-plan de 07 para equipo de 1 dev. No hay hallazgos P0 ni P1.

Condiciones para promover a la fase 08:
- La promoción no está condicionada a ningún bloqueo. Los hallazgos H-01 a H-04 (1 P2 + 3 P3) son mejoras de concisión y consistencia textual que pueden absorberse en una revisión menor sin generar versión nueva, o diferirse a la primera evolución del backlog.
- Al generar 08, materializar la DoD canónica que el mini-plan §5 y la DoR §5 referencian como pendiente, y crear los acceptance tests Given/When/Then de cada US Must y Should a partir de los criterios ya redactados en el product-backlog.

---

## 8. Anexo — Método de verificación

- Lectura completa de los seis entregables de 06 y 07 y de ambas reglas (06 v1.2, 07 v1.2).
- Verificación de existencia de upstream: 7 NB, 16 CU, 13 ADR, componentes y modelo lógico de 05; intake §4 (MoSCoW) y §15 (rebanadas) y manifiesto (project_type web-monolith, proyecto único).
- Barridos automáticos sobre 06 y 07: identificadores de tres dígitos (0), patrón `.v` en nombres (0), stack/vocabulario fuente (0), emojis decorativos (0; solo `↔` sancionado por la regla).
- Reconstrucción de las matrices de cobertura NB→US y CU→US y conteo MoSCoW/SP a partir del texto de los documentos.
- Comprobación de que cada US/BT citada por el mini-plan existe en 06 con el identificador exacto.
- Verificación de la ausencia de los cuatro artefactos de equipo en 07 y de la apertura con H1 directo (sin `--`) en todos los archivos.
