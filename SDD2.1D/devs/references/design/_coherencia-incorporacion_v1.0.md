# Nota de coherencia — Incorporación del catálogo de reglas de diseño

**Proyecto:** Template SDD 2.1
**Documento:** _coherencia-incorporacion_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-19
**Autor:** AG-ROOT (Arquitecto de Soluciones)

---

## 1. Alcance

Pase de verificación en lectura sobre la incorporación del catálogo de reglas de diseño (`devs/references/design/`) y su cableado en la metodología SDD 2.1. Cubre los archivos creados y editados en las fases 1 a 3, contra las invariantes D1–D8 del template y la trazabilidad esperada del catálogo.

## 2. Inventario de archivos

### 2.1 Creados

| Archivo | Rol |
| --- | --- |
| `devs/references/design/_index_design-rules.md` | Índice del catálogo: propósito, tabla del catálogo, roadmap de especializaciones, criterio de selección, control de cambios. |
| `devs/references/design/_coherencia-incorporacion_v1.0.md` | Esta nota de coherencia. |

### 2.2 Provistos, intactos (no modificados)

| Archivo | Estado |
| --- | --- |
| `devs/references/design/design-rules-web-generico_v1.0.md` | Documento base, sin alteración. |
| `devs/references/design/design-rules-blazor-mudblazor_v1.0.md` | Especialización Blazor + MudBlazor, sin alteración. |

### 2.3 Editados

| Archivo | Cambios | Versión |
| --- | --- | --- |
| `devs/rules/03_rules_ux_ui_dx.md` | Nueva §1.4 (insumos normativos de diseño por stack), fila de anti-patrón en §4.4, fila "Catálogo de diseño aplicado" en la tabla de trazabilidad de §4.3, control de cambios. | 1.2 → 1.3 |
| `devs/orchestrator/master-prompt.md` | Nota operativa en §6 sobre el insumo del catálogo para AG-03 en proyectos con UI, fila en la tabla de versionado §16. | 3.0 → 3.1 |
| `devs/guides/marco-teorico-sdd2.1_v1.0.md` | `references/` sumado al mapa de carpetas del plano `devs/` (§1.5), nueva subsección §8.7 sobre el catálogo. | sin marca de versión interna; solo edición |
| `guides/guia-usuario-sdd2.1_v1.0.md` | Subárbol `references/design/` sumado al mapa de carpetas del plano `devs/` (§10.2). | sin marca de versión interna; solo edición |

No se modificó `docs/`. No se sobrescribió ninguna plantilla.

## 3. Verificación de invariantes (D1–D8)

| Invariante | Resultado | Evidencia |
| --- | --- | --- |
| Idioma español rioplatense neutro técnico | Cumple | Todo el contenido nuevo en español; sin emojis ni negritas decorativas. |
| Encoding UTF-8 | Cumple | Archivos nuevos en UTF-8 sin BOM. |
| kebab-case en nombres de archivo | Cumple | `_index_design-rules.md`, `_coherencia-incorporacion_v1.0.md` y los `design-rules-*` siguen kebab-case; los prefijos `_` se alinean con los meta-archivos existentes (`_root_rules.md`, `_intake_rules.md`). |
| Sufijo `_v1.0` con guion bajo antes de `v` | Cumple | `_coherencia-incorporacion_v1.0.md` y los dos `design-rules-*_v1.0.md` usan guion bajo; no aparece el patrón prohibido `.v1.0`. El índice no lleva versión en el nombre por ser meta-archivo de entrada, igual que `_root_rules.md` y `_intake_rules.md`. |
| Una sola versión vigente por nombre lógico | Cumple | Un único archivo por nombre lógico en `references/design/`; sin versiones paralelas. |
| Trazabilidad explícita en cabeceras | Cumple | El índice y esta nota declaran Proyecto/Documento/Versión/Estado/Fecha/Autor; la cabecera de la regla 03 sube a 1.3 y la del master-prompt a 3.1, con su control de cambios respectivo. |
| Vocabulario neutro sin atar al dominio de ninguna solución (D7) | Cumple | El contenido nuevo no introduce literales de dominio de solución. Los nombres de stack (Blazor, MudBlazor, MAUI, HTML) y los tokens de diseño pertenecen a la capa de especialización por stack del catálogo, no al dominio funcional de una solución. |
| Conjunto cerrado D8 de `project_type` | Cumple | El índice y los cableados referencian valores D8 existentes (web-monolith, web-microservices, mobile-app-maui, desktop-app); no se inventa ningún `project_type` nuevo. |

## 4. Verificación de trazabilidad

| Eslabón | Resultado | Evidencia |
| --- | --- | --- |
| El índice referencia los dos documentos | Cumple | Tabla del catálogo en `_index_design-rules.md` §2 lista `design-rules-web-generico_v1.0.md` y `design-rules-blazor-mudblazor_v1.0.md` con su relación base→especialización. |
| La regla 03 referencia el índice | Cumple | §1.4 de `03_rules_ux_ui_dx.md` indica cargar el catálogo a través de `_index_design-rules.md`. |
| El marco teórico referencia el subárbol | Cumple | §1.5 (mapa de carpetas) y §8.7 de `marco-teorico-sdd2.1_v1.0.md` mencionan `devs/references/design/` y el modelo base→especialización. |
| La guía de usuario referencia el subárbol | Cumple | §10.2 de `guia-usuario-sdd2.1_v1.0.md` incluye `references/design/` en el árbol del plano `devs/` con su línea de propósito. |
| El master-prompt incluye el insumo | Cumple | Nota operativa de §6 de `master-prompt.md` suma el catálogo como insumo de AG-03 para proyectos con UI. |

## 5. Observaciones

1. Fin de línea CRLF (informativa, preexistente). La tabla de invariantes de solución del master-prompt menciona LF como EOL, pero todos los archivos del repositorio (incluidos los dos documentos provistos del catálogo, la regla 03 y el master-prompt) usan CRLF, y no hay `.gitattributes` que fuerce LF. Los archivos nuevos se crearon en CRLF para mantener consistencia con el repositorio y con los archivos provistos. Es una discrepancia preexistente a nivel template, fuera del alcance de esta incorporación; normalizar el EOL implicaría reescribir todos los archivos del plano `devs/`.

## 6. Veredicto

APROBADO CON OBSERVACIONES.

Las invariantes verificadas (kebab-case, sufijo `_v1.0` con guion bajo, encoding UTF-8, idioma, ausencia de literales de dominio, una sola versión vigente por nombre) y la trazabilidad del catálogo cumplen. La única observación es la del EOL CRLF, preexistente y de alcance template, sin impacto sobre la coherencia de la incorporación.

## 7. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-19 | Nota de coherencia inicial de la incorporación del catálogo de reglas de diseño y su cableado en la regla 03, el marco teórico, la guía de usuario y el master-prompt. | AG-ROOT |
