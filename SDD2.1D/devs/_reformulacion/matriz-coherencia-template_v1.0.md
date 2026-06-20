# Matriz de coherencia del template SDD 2.1D — Línea base (ST-01)

| Campo | Valor |
|---|---|
| Archivo | `matriz-coherencia-template_v1.0.md` |
| Versión | 1.0 |
| Subtarea | ST-01 de la reformulación SDD 2.1D (solución más jerarquía de proyectos) |
| Fecha | 2026-06-08 |
| Auditor | Auditor independiente (§10 `master-prompt.md`) coordinado por AG-ROOT |
| Modo | Pase read-only. Ningún artefacto del template fue modificado |
| Alcance | Todos los markdown de `/SDD2.1D/devs/**`, `/SDD2.1D/guides/**` y `/SDD2.1D/docs/**` |

---

## §1 Resumen ejecutivo

Se inventariaron y evaluaron 25 documentos markdown del template contra su objetivo declarado, antes de introducir cualquier cambio. La carpeta `/SDD2.1D/docs/` está vacía (solo `.gitkeep`): es el destino de salida del orquestador, no contiene artefactos a evaluar.

Resultado de la línea base:

| Nivel | Cantidad | Documentos |
|---|---|---|
| P0 (bloqueante) | 0 | — |
| P1 (importante) | 1 | `00_rules_contexto.md` |
| P2 (menor) | 2 | `master-prompt.md`, `guia-usuario-sdd2.0_v1.0.md` |
| P3 / limpio | 22 | resto del corpus |

Veredicto: APROBADO CON OBSERVACIONES. No hay hallazgos P0, por lo que la cadena de la reformulación puede avanzar a ST-02. El único P1 es una desviación de higiene D7 acotada y de corrección trivial. Los P2 son una marca de versión desactualizada en el nombre de un archivo de guía (resto del rename del repositorio) y un acoplamiento residual del orquestador al bootstrap ya documentado durante el propio bootstrap.

Nota de método: la evaluación se delegó en seis subagentes lectores que devolvieron hallazgos crudos; el auditor verificó directamente cada hallazgo de nivel P0 o P1 antes de registrarlo. Tres P0 reportados por los subagentes fueron recalificados tras verificación: uno a P1 y dos descartados como falsos positivos (ver §4.3).

---

## §2 Alcance y método

### §2.1 Corpus evaluado

| Grupo | Documentos | Cantidad |
|---|---|---|
| Orquestador | `devs/orchestrator/master-prompt.md` | 1 |
| Intake | `devs/intake/PROJECT-BRIEF-template.md`, `devs/intake/PROJECT-README-template.md` | 2 |
| Reglas constructivas | `devs/rules/_root_rules.md` + `00`–`11` (`devs/rules/XX_rules_<categoria>.md`) | 13 |
| Guías | `devs/guides/marco-teorico-sdd_v1.0.md`, `guides/guia-usuario-sdd2.0_v1.0.md` | 2 |
| Bootstrap (histórico) | `devs/_bootstrap/audit-fase-1..4.md`, `audit-final.md`, `audit-sdd1.md`, `bootstrap-changelog.md` | 7 |
| Total | | 25 |

### §2.2 Criterios de coherencia (heredados de §5 del prompt de reformulación)

1. Cumplimiento del objetivo declarado: el contenido responde de forma completa al objetivo que el propio documento declara en su cabecera, su §0 de posición en la cadena o su primer párrafo.
2. Scope: el documento no invade el scope de otra categoría; si lo toca, refiere y delega en lugar de duplicar.
3. Conformidad D1–D8: idioma, encoding, kebab-case, versionado con guion bajo, deprecación, trazabilidad, vocabulario neutro y conjunto cerrado D8.
4. Trazabilidad: los insumos upstream y downstream declarados en la cabecera existen y cierran sin huérfanos.
5. Consistencia cross-doc: las afirmaciones sobre los 8 tipos D8, sobre el catálogo de 13 subagentes y sobre las fases del orquestador coinciden entre documentos.

El criterio 6 del prompt (aptitud para la jerarquía de proyectos) se evalúa en ST-09, no acá. ST-01 fija deliberadamente una línea base de coherencia del estado actual; el gap de proyecto único frente a solución con jerarquía no es un defecto de coherencia, sino el problema que la reformulación resuelve. Lo relevante de ese gap para las subtareas siguientes se registra en §6 como insumo, no como hallazgo.

### §2.3 Niveles de hallazgo

- P0 bloqueante: viola D1–D8 o rompe la estructura obligatoria del documento. Detiene la cadena.
- P1 importante: coherencia parcial o trazabilidad con huecos. Se corrige si hay margen en la subtarea.
- P2 menor: mejora de redacción o de ejemplo. Se posterga a una versión siguiente.
- P3 observación: sugerencia sin impacto operativo.

---

## §3 Matriz de coherencia (una fila por documento)

Convención: «Cumple objetivo» en Sí / Parcial / No; «D1–D8» en Conforme / desvío; «Trazab.» en cierre upstream/downstream. La columna «Hallazgos» referencia los IDs enumerados en §4.

| Documento | Objetivo declarado (resumen) | Cumple | Invade scope | D1–D8 | Trazab. cierra | Hallazgos | Nivel |
|---|---|---|---|---|---|---|---|
| `master-prompt.md` | Orquestar la generación de `/docs` por fases plan-then-confirm | Sí | No | Conforme | Sí | H-01 | P2 |
| `PROJECT-BRIEF-template.md` | Plantilla de intake de negocio sin decisiones técnicas | Sí | No | Conforme | Sí | — | limpio |
| `PROJECT-README-template.md` | Plantilla de intake técnico; §1 fija el tipo de proyecto | Sí | No | Conforme | Sí | — (ver §6) | limpio |
| `_root_rules.md` | Reglas del README raíz (AG-ROOT) | Sí | No | Conforme | Sí | — | limpio |
| `00_rules_contexto.md` | Reglas de la categoría 00 Contexto (AG-00) | Sí | No | Desvío D7 | Sí | H-02 | P1 |
| `01_rules_necesidades_negocio.md` | Reglas de la categoría 01 Necesidades de negocio (AG-01) | Sí | No | Conforme | Sí | H-03 | limpio |
| `02_rules_especificacion_funcional.md` | Reglas de la categoría 02 Especificación funcional (AG-02) | Sí | No | Conforme | Sí | H-04 | limpio |
| `03_rules_ux_ui_dx.md` | Reglas de la categoría 03 UX/UI/DX (AG-03) | Sí | No | Conforme | Sí | — | limpio |
| `04_rules_prompts_ai.md` | Reglas de la categoría 04 Prompts AI, gated por `usa_llm` (AG-04) | Sí | No | Conforme | Sí | H-05 | limpio |
| `05_rules_arquitectura_tecnica.md` | Reglas de la categoría 05 Arquitectura técnica (AG-05) | Sí | No | Conforme | Sí | H-06 | limpio |
| `06_rules_backlog_tecnico.md` | Reglas de la categoría 06 Backlog técnico (AG-06) | Sí | No | Conforme | Sí | — | limpio |
| `07_rules_plan_sprint.md` | Reglas de la categoría 07 Plan de sprint (AG-07) | Sí | No | Conforme | Sí | H-07 | limpio |
| `08_rules_calidad_y_pruebas.md` | Reglas de la categoría 08 Calidad y pruebas (AG-08) | Sí | No | Conforme | Sí | H-08 | limpio |
| `09_rules_devops.md` | Reglas de la categoría 09 DevOps (AG-09) | Sí | No | Conforme | Sí | — | limpio |
| `10_rules_developer_guide.md` | Reglas de la categoría 10 Developer guide (AG-10) | Sí | No | Conforme | Sí | — | limpio |
| `11_rules_examples.md` | Reglas de la categoría 11 Examples (AG-11) | Sí | No | Conforme | Sí | — | limpio |
| `marco-teorico-sdd_v1.0.md` | Explicar el porqué metodológico del template | Sí | No | Conforme | Sí | H-09 | limpio |
| `guia-usuario-sdd2.0_v1.0.md` | Guiar el uso del template paso a paso | Sí | No | Desvío menor | Sí | H-10, H-11 | P2 |
| `audit-fase-1.md` | Registro: audit de los intake (Fase 1) | Sí | n/a | Histórico | n/a | — | limpio |
| `audit-fase-2.md` | Registro: audit de las 13 reglas (Fase 2) | Sí | n/a | Histórico | n/a | — | limpio |
| `audit-fase-3.md` | Registro: audit del orquestador (Fase 3) | Sí | n/a | Histórico | n/a | H-01 (origen) | limpio |
| `audit-fase-4.md` | Registro: audit de guías (Fase 4) | Sí | n/a | Histórico | n/a | H-10 (origen) | limpio |
| `audit-final.md` | Registro: consolidación final del bootstrap | Sí | n/a | Histórico | n/a | — | limpio |
| `audit-sdd1.md` | Registro: audit del fuente SDD 1.0; fuente canónica de D7 | Sí | n/a | Histórico | n/a | — | limpio |
| `bootstrap-changelog.md` | Registro: bitácora del bootstrap | Sí | n/a | Histórico | n/a | — | limpio |

Los siete documentos de `_bootstrap/` son registros históricos no normativos. Su objetivo es dejar constancia fiel de cada fase del bootstrap; cumplen ese objetivo. No se les aplican los criterios de scope ni de trazabilidad upstream/downstream de las categorías, y no se los penaliza por nombrar el dominio fuente (de hecho `audit-sdd1.md` es la fuente canónica del listado de vocabulario prohibido por D7).

---

## §4 Hallazgos enumerados

### §4.1 Hallazgos vigentes

H-01 — P2 — `master-prompt.md` §8 (línea 330) y §10 (línea 460).
Evidencia: el esqueleto de despacho remite al subagente al «listado de términos prohibidos en la auditoría `_bootstrap/audit-sdd1.md` y `_bootstrap/audit-fase-2.md`», y el informe de audit se referencia contra `audit-fase-2.md`. El §15 declara al master-prompt «autocontenido».
Lectura: acoplamiento residual del prompt operativo a archivos del bootstrap. Ya fue detectado y registrado durante el propio bootstrap (`audit-fase-3.md`, defecto P1 abierto para v1.1).
Recomendación: inlinear el listado D7 en el master-prompt o en las reglas constructivas, de modo que la operación no dependa de leer artefactos de `_bootstrap/`. Puede atenderse en ST-04 (reformulación del orquestador), que de todos modos toca §8.
Estado: RESUELTO el 2026-06-09 en ST-04. El `master-prompt.md` v2.0 delega la guía de vocabulario prohibido por D7 a las reglas y ya no remite a `_bootstrap/audit-sdd1.md` ni `_bootstrap/audit-fase-2.md` en §8 ni §10. Se conserva el hallazgo en esta línea base para la comparación de no regresión de ST-09.

H-02 — P1 — `00_rules_contexto.md` §6 (línea 252).
Evidencia: «Ningún documento contiene emojis, negritas decorativas ni referencias hardcoded a stack, frameworks o ejemplos del dominio Motor DSL.» El criterio nombra literalmente el dominio fuente del bootstrap.
Lectura: desviación de higiene D7 en texto normativo. Es la única regla que nombra «Motor DSL» dentro de un criterio de aceptación (no como lección histórica). Una lectura estricta de «P0 = viola D1–D8» la elevaría a P0; se la fija en P1 porque la cláusula es autorreferencial (prohíbe el término, no contamina la salida generada) y no rompe estructura ni trazabilidad.
Recomendación: reemplazar «ejemplos del dominio Motor DSL» por «ejemplos del dominio fuente del bootstrap». Corrección de una línea, atendible en ST-06 (validación de reglas) o de inmediato.
Estado: RESUELTO el 2026-06-08. `00_rules_contexto.md` subió a v1.1 con la cláusula corregida y entrada en su control de cambios. Se conserva el hallazgo en esta línea base para la comparación de no regresión de ST-09.

H-03 — P3 — `01_rules_necesidades_negocio.md` §5.4 (línea 242).
Evidencia: la enumeración del vocabulario prohibido por D7 (DSL, ESC-POS, MAUI literal, Bluetooth, NuGet, .NET 10, impresora térmica) vive en una pregunta-guía.
Lectura: no es defecto; es deseable que la regla enumere lo prohibido. Observación: podría además referenciarse desde §4 para que el redactor lo encuentre en la estructura de redacción, no solo en las preguntas-guía.
Recomendación: opcional. Sin impacto operativo.

H-04 — P3 — `02_rules_especificacion_funcional.md` §3.1 (línea 89).
Evidencia: el ejemplo de anti-patrón de nombre `CU-01-Cargar-Plantilla_v1.0.md` ilustra la prohibición de mayúsculas/camelCase.
Lectura: «Plantilla» es una palabra genérica, no vocabulario del dominio fuente; el ejemplo cumple su función (mostrar el anti-patrón de capitalización). No es desvío D7.
Recomendación: opcional, elegir un sustantivo aún más neutro si se busca higiene total. Sin impacto operativo.

H-05 — P3 — `04_rules_prompts_ai.md` §2.1.
Evidencia: el gating `usa_llm` se origina en dos lugares del intake (PROJECT-README §15 y §13 NFR) y la regla no fija precedencia.
Lectura: la precedencia ya está resuelta aguas arriba: `master-prompt.md` §4 calcula `usa_llm` como verdadero si cualquiera de las dos fuentes lo activa (lógica OR). No hay ambigüedad operativa.
Recomendación: opcional, citar esa lógica OR en la regla para autonomía. Sin impacto operativo.

H-06 — P3 — `05_rules_arquitectura_tecnica.md` §3.6.
Evidencia: los ADR son inmutables (no se versionan en el mismo archivo; se superan con un ADR nuevo y ambos coexisten en `adrs/`), mientras el resto de los artefactos sigue la regla general de una versión vigente con archivado en `_legacy/`.
Lectura: es la convención estándar de ADR (Nygard), explícita y coherente en el documento. No es una inconsistencia; un subagente lector la reportó como P0 por error de interpretación y se descarta (ver §4.3).
Recomendación: ninguna.

H-07 — P3 — `07_rules_plan_sprint.md` §2.2 frente a `06_rules_backlog_tecnico.md`.
Evidencia: la regla «si `equipo_n` == 1 se produce únicamente `mini-plan_v1.0.md`» vive en 07; 06 no la replica.
Lectura: no es desvío. El backlog de 06 no varía por tamaño de equipo; la conmutación por `equipo_n` es competencia de 07 y del flag de `master-prompt.md` §4. No corresponde duplicarla en 06.
Recomendación: ninguna.

H-08 — P3 — `08_rules_calidad_y_pruebas.md` §4.8 frente a `06_rules_backlog_tecnico.md` §3.4.
Evidencia: 08 declara la DoD como fuente canónica; 06 §3.4 ya diferencia DoR (filtro de entrada) de DoD de 08 (filtro de salida) «sin solaparlas».
Lectura: la diferenciación existe y es correcta. Una mención recíproca explícita en 08 sería un agregado de claridad, no una corrección.
Recomendación: opcional. Sin impacto operativo.

H-09 — P3 — `marco-teorico-sdd_v1.0.md` §2.3.
Evidencia: el texto sobre «Spec-First» dice que la especificación se «descarta» tras la implementación, y luego propone «Spec-Anchored» como estándar de SDD 2.1.
Lectura: coherente, pero «descartar» puede leerse como «borrar» en vez de «dejar de mantener activamente».
Recomendación: opcional, una nota aclaratoria. Sin impacto operativo.

H-10 — P2 — `guia-usuario-sdd2.0_v1.0.md` (nombre de archivo y cabecera).
Evidencia: el archivo se llama `guia-usuario-sdd2.0_v1.0.md` y su metadato `Documento` repite «sdd2.0», mientras el cuerpo, el repositorio (`SDD2.1D`) y el commit de rename declaran «SDD 2.1». El `bootstrap-changelog.md` y `audit-fase-4.md` citan el nombre tal cual, reflejando el desfase.
Lectura: resto del rename de `sdd2.0` a `SDD2.1D` que no alcanzó el nombre de este archivo. No bloquea operación; es consistencia de marca.
Recomendación: renombrar a `guia-usuario-sdd2.1_v1.0.md` (con archivado del anterior según D5) y actualizar el metadato y las referencias internas. Atendible en ST-08 o como tarea de higiene. Nota: `marco-teorico-sdd_v1.0.md` no lleva marca de versión en el nombre, así que no requiere rename.
Estado: RESUELTO el 2026-06-10 (tarea de higiene posterior al cierre de ST-09). El archivo se renombró a `guia-usuario-sdd2.1_v1.0.md` con `git mv`; se actualizaron el metadato `Documento`, la mención «novedades de la 2.0» a «2.1» y las referencias internas del árbol. Nota: los registros históricos de `_bootstrap/` (changelog, audit-fase-4) siguen citando el nombre anterior por ser registros congelados del bootstrap.

H-11 — P2 — `guia-usuario-sdd2.0_v1.0.md` §3.3 (líneas 93 a 100).
Evidencia: enumera los 8 tipos de proyecto como «D1 `library`», «D2 `web-monolith`», … «D8 `worker-service`». Esa notación colisiona con los invariantes D1–D8 del template (por ejemplo «D7» significa «prohibición de vocabulario del dominio fuente» como invariante, y acá se usa «D7 `cli-tool`»). El glosario de la propia guía (línea 1062) define «D8» correctamente como el conjunto cerrado de 8 tipos.
Lectura: sobrecarga de la notación D1–D8 que puede confundir al lector sobre qué significa «D8». No afecta al orquestador (que lee la cadena literal del tipo), pero sí la claridad de la guía.
Recomendación: renumerar la lista sin prefijo «Dn» (por ejemplo «1. `library`») o etiquetarla como «Tipo D8-XX». Atendible en ST-08.
Estado: RESUELTO el 2026-06-10. La lista de §3.3 de la guía se renumeró de «D1 … D8» a «1. … 8.», eliminando la colisión con la notación de invariantes D1-D8.

### §4.2 Observaciones transversales

OT-01 — P3 — Referencias a «Motor DSL» como antecedente documentado en `05`, `06` y `07`.
Las reglas 05 (§3.3, §4.5), 06 (§3.2, §4.8) y 07 (§4.1, §3.1) nombran «el antecedente Motor DSL» al justificar convenciones correctivas (ADR individuales, IDs de dos dígitos, separador único de nombre). Son referencias de trazabilidad legítimas a la lección registrada en `audit-sdd1.md`, no contaminación de la salida, y por eso no se cuentan como hallazgos. Un endurecimiento futuro de higiene D7 podría sustituir «Motor DSL» por «el dominio fuente» o «el antecedente documentado en `_bootstrap/audit-sdd1.md`», dejando el nombre del dominio confinado a `_bootstrap/`. Sin impacto operativo.

### §4.3 Hallazgos descartados tras verificación

D-01 — Reportado como P0, descartado. `02_rules_especificacion_funcional.md` §3.1 línea 89, ejemplo `CU-01-Cargar-Plantilla`. Verificado: el ejemplo ilustra el anti-patrón de mayúsculas/camelCase; «Plantilla» es genérico. No es desvío D7. Reclasificado a observación P3 (H-04).

D-02 — Reportado como P0, descartado. `05_rules_arquitectura_tecnica.md` §3.6, supuesta «inconsistencia crítica» entre el versionado de ADRs y el del resto de artefactos. Verificado: es la convención Nygard, explícita y coherente. No hay contradicción. Reclasificado a observación P3 (H-06).

D-03 — Reportado como P0/P1 reiterado por un subagente. La exigencia de que cada regla reliste el vocabulario D7 dentro de su §4. Descartado como defecto: por diseño, el listado canónico vive en `_bootstrap/audit-sdd1.md`, y el `master-prompt.md` §8 dirige a los subagentes a esa fuente. No relistarlo en cada regla no rompe coherencia. Se conserva solo la observación opcional H-03.

---

## §5 Conformidad cross-doc (criterio 5)

- Conjunto cerrado D8: los 25 documentos que lo mencionan enumeran exactamente los mismos 8 valores (`library`, `web-monolith`, `web-microservices`, `desktop-app`, `mobile-app-maui`, `rest-api`, `cli-tool`, `worker-service`). Sin agregados ni faltantes. La única fricción de notación es H-11 (la guía numera los tipos como D1–D8), que no altera el conjunto.
- Catálogo de subagentes: las 13 reglas declaran su titular AG-XX de forma consistente con el catálogo (AG-00 a AG-11 más AG-ROOT). El `master-prompt.md` §6 y §11 los referencian sin divergencia.
- Fases del orquestador: la guía de usuario (§4.5) y el `master-prompt.md` (§7) coinciden en A (00,01), B (02,03,04), C (05), D (06,07), E (08), F (09,10), G (11), H (README raíz).
- Cadena de trazabilidad D6: Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline aparece coherente en el orquestador, las reglas y el marco teórico.

---

## §6 Nota de línea base para ST-02 y ST-09 (no es hallazgo de coherencia)

ST-01 no evalúa la aptitud para la jerarquía de proyectos. A título de insumo para el diseño del modelo (ST-02) y para la verificación de no regresión (ST-09), se registra dónde el template asume hoy un único `project_type` por repositorio. Estas no son fallas de coherencia del estado actual: el template es coherente con su propio supuesto de tipo único. Son los puntos que la reformulación deberá tocar.

| Punto de supuesto «tipo único» | Ubicación | Qué deberá cambiar |
|---|---|---|
| Derivación de `project_type` único | `master-prompt.md` §3 | Leer el manifiesto de solución y derivar N proyectos tipados (ST-04) |
| Pregunta bloqueante de tipo único y regla «no mezclar tipos» | `PROJECT-README-template.md` §1 | Declarar el tipo por proyecto; centralizar la enumeración en el manifiesto (ST-03) |
| Sub-proyectos como comentario inline | `PROJECT-README-template.md` §5 | Derivar la estructura de la jerarquía declarada (ST-03) |
| Matriz de adaptabilidad por repositorio | `master-prompt.md` §14 | Aplicarla una vez por proyecto, en orden topológico (ST-04, ST-07) |
| Vista de arquitectura por proyecto | `05_rules_arquitectura_tecnica.md` | Añadir una vista de solución por encima (ST-05) |
| Convención de nombres de código no formalizada | `PROJECT-README-template.md` §5 (solo por ejemplo) | Formalizar `<NombreSolucionCodigo>.<Sufijo>` y excepción `Aplicada` (ST-03) |

Las tablas §1.2 de las reglas ya están indexadas por D8; el modelo objetivo las invoca una vez por proyecto sin reescribirlas. El caso degenerado (solución de un proyecto) debe reproducir el comportamiento actual; ST-09 verificará que así sea.

---

## §7 Veredicto

APROBADO CON OBSERVACIONES.

- 0 hallazgos P0: la cadena de la reformulación puede avanzar a ST-02.
- 1 hallazgo P1 (H-02): desviación de higiene D7 en `00_rules_contexto.md`, corrección de una línea; se recomienda atenderla en ST-06 o de inmediato, no bloquea ST-02.
- 2 hallazgos P2 (H-01, H-10/H-11): acoplamiento residual del orquestador al bootstrap y marca de versión en la guía de usuario; se atienden en las subtareas que tocan esos artefactos (ST-04, ST-08).
- El resto del corpus está limpio o con observaciones P3 sin impacto operativo.

Esta matriz es la línea base. ST-09 repetirá la evaluación sobre el estado reformulado y comparará contra esta referencia para demostrar que no se introdujeron regresiones.

---

## §8 Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-08 | Línea base de coherencia del template (ST-01). Inventario de 25 markdown, evaluación contra los 5 criterios de coherencia, niveles P0–P3, calibración de hallazgos de subagentes con descarte de 2 falsos positivos y recalificación de 1 P0 a P1. Veredicto APROBADO CON OBSERVACIONES (0 P0, 1 P1, 2 P2). | Auditor independiente + AG-ROOT (reformulación SDD 2.1D) |
