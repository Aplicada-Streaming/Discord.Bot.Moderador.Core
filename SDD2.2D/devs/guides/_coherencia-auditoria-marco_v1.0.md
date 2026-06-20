# Nota de coherencia — Auditoría del marco teórico contra configuración dirigida por esquema

**Proyecto:** Template SDD 2.2
**Documento:** _coherencia-auditoria-marco_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-20
**Autor:** AG-ROOT (Arquitecto de Soluciones, pase de QA) con verificación de AG-08

---

## 1. Alcance

Pase de verificación en lectura sobre la auditoría de `marco-teorico-sdd2.2_v1.0.md` contra las mejoras de configuración dirigida por esquema (schema-driven UI) y su AI-readiness. La auditoría diagnosticó sección por sección, convirtió cada brecha en una tarea con especialidad asignada y ejecutó las ediciones de forma quirúrgica y serializada sobre el documento. Esta nota verifica el resultado contra las invariantes D1–D8 (con énfasis en D7) y la cadena de trazabilidad con el catálogo de diseño, la regla 03 y el master-prompt.

## 2. Inventario de archivos

### 2.1 Creados

| Archivo | Rol |
| --- | --- |
| `devs/guides/_coherencia-auditoria-marco_v1.0.md` | Esta nota de coherencia. |

### 2.2 Editado

`devs/guides/marco-teorico-sdd2.2_v1.0.md` (1.3 → 1.4). Secciones tocadas, una por tarea de la auditoría:

| Tarea | Sección | Cambio | Lente |
| --- | --- | --- | --- |
| T1 | §1.5 Mapa visual | `references/` pasa a "por stack y por capacidad" | AG-ROOT |
| T2 | §2.5 SDD encaja con IA | Párrafo: descriptor como contrato + frontera validable + simulación + propone/confirma/valida | AG-05 (apoyo AG-04) |
| T3 | §4.2 AG-03 | Línea: carga la extensión y produce superficies de configuración por descriptor | AG-03 |
| T4 | §4.2 AG-05 | Línea: motor de la frontera `PropuestaDeConfiguracion` como responsabilidad de arquitectura | AG-05 |
| T5 | §4.2 AG-04 | Línea: asistente futuro (descriptores como tool definitions, structured outputs contra la frontera) | AG-04 |
| T6 | §7.5 Patrones transversales | Bullet: plan-and-apply con frontera validable como patrón transversal agnóstico de D8 | AG-05 |
| T7 | §8.3 Leyes UX | Párrafo: divulgación progresiva a Hick/Miller; ayuda contextual a la heurística 10 | AG-03 |
| T8 | §11.1 Plan-then-confirm | Párrafo: propone/confirma/valida como instancia runtime con human-in-the-loop | AG-04 |
| T9 | §12.3 Anti-patrones | Fila "IA que ejecuta en vez de proponer" + nota de anti-patrones de UX (default/ayuda hardcodeados) | AG-08 (apoyo AG-03) |
| T10 | §13 Glosario | Nueve términos nuevos | AG-10 |
| T11 | Cabecera + Control de Cambios | Versión 1.3 → 1.4, fecha y fila de changelog | AG-ROOT |

Secciones sin cambio: §8.7 ya reflejaba el núcleo de la mejora (Cumple en el diagnóstico). §14 Bibliografía queda igual: la mejora no introduce una fuente formal nueva realmente citada, y no se agregan referencias de relleno. No se tocó `docs/` ni se sobrescribió ninguna plantilla.

## 3. Verificación de invariantes (D1–D8)

| Invariante | Resultado | Evidencia |
| --- | --- | --- |
| D1 — Idioma | Cumple | Adiciones en español rioplatense técnico, tono alineado con el marco; sin emojis ni negritas decorativas (salvo las etiquetas de término del glosario, que siguen el estilo preexistente de esa tabla). |
| D2 — Encoding | Cumple | El archivo nuevo se creó en UTF-8 sin BOM; las ediciones no introducen BOM. |
| D3 — Nombres | Cumple | `_coherencia-auditoria-marco_v1.0.md` en kebab-case con prefijo `_` de meta-archivo, igual que `_coherencia-config-esquema`. |
| D4 — Versionado de nombre | Cumple | Sufijo `_v1.0` con guion bajo; no aparece `.v1.0`. |
| D5 — Una sola versión vigente | Cumple | El marco sube de versión in situ (1.3 → 1.4); no hay copias paralelas. |
| D6 — Trazabilidad de cabecera | Cumple | Esta nota declara Proyecto/Documento/Versión/Estado/Fecha/Autor y lleva control de cambios; el marco mantiene su cabecera con la versión actualizada. |
| D7 — Neutralidad de dominio (crítica) | Cumple | Ningún literal del dominio de moderación de la fuente se filtró al marco: la búsqueda de baneo/banear/spam/timeout/fan-out/"palabra prohibida"/moderación no arroja coincidencias en las adiciones. El vocabulario es neutro: parámetro, descriptor, default, límite, ejemplo, preset, propuesta, simulación. |
| D8 — `project_type` | Cumple | No se inventan tipos: la capacidad se describe como transversal "a cualquier tipo D8 con superficies de configuración"; los valores D8 citados son del conjunto cerrado. |

## 4. Verificación de trazabilidad

| Eslabón | Resultado | Evidencia |
| --- | --- | --- |
| Marco → catálogo de diseño | Cumple | §1.5, §2.5, §4.2, §7.5, §8.3, §11.1 y §12.3 referencian `design-rules-config-esquema` y/o la frontera `PropuestaDeConfiguracion`; §8.7 ya la documentaba. |
| Marco → regla 03 | Cumple | §4.2 (AG-03) refleja la carga de la extensión cuando hay superficies de configuración, consistente con `03_rules_ux_ui_dx.md` v1.4 §1.4. |
| Marco → master-prompt | Cumple | La responsabilidad de AG-03 de cargar la extensión (marco §4.2) es coherente con la nota operativa de §6 del master-prompt (insumo de AG-03 para proyectos con superficies de configuración). |
| Reparto de responsabilidades UX/arquitectura/IA | Cumple | §4.2 separa AG-03 (lado UX), AG-05 (motor de la frontera) y AG-04 (asistente como tool definitions), sin solapamiento; coincide con el límite 05/02 del documento de capacidad. |
| Terminología uniforme | Cumple | `PropuestaDeConfiguracion`, descriptor, modo simulación, plan-and-apply, human-in-the-loop y divulgación progresiva se usan con el mismo sentido en todas las secciones editadas y quedan definidos en §13. |
| Auditabilidad (AG-08) | Cumple | Cada mejora reflejada está enunciada de forma verificable, no aspiracional: el default vive en el descriptor; la propuesta se valida antes de aplicar; la IA propone y no ejecuta; la divulgación progresiva oculta lo avanzado en expander. |

## 5. Observaciones

1. Fin de línea CRLF (informativa, preexistente). La cabecera del marco declara "EOL LF", pero todos los archivos del repositorio usan CRLF y no hay `.gitattributes` que fuerce LF; el archivo nuevo se creó en CRLF para mantener consistencia. Discrepancia preexistente a nivel template, fuera del alcance de esta auditoría.
2. El §8.7 y otras adiciones previas de config-esquema se habían incorporado al marco sin subir su versión interna; esta auditoría regulariza esa deuda al subir a 1.4 y registrar la reflexión completa en el Control de Cambios.

## 6. Veredicto

APROBADO CON OBSERVACIONES.

El marco refleja ahora, de forma coherente y completa, las mejoras de configuración dirigida por esquema y su AI-readiness: todas las brechas del diagnóstico quedaron corregidas por la especialidad correspondiente, sin contradicciones cruzadas, con terminología uniforme y trazabilidad cerrada hacia el catálogo, la regla 03 y el master-prompt. Las observaciones son el EOL CRLF preexistente y la regularización de la versión, sin impacto sobre la coherencia.

## 7. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-20 | Nota de coherencia inicial de la auditoría del marco teórico contra la configuración dirigida por esquema: diagnóstico por sección, ediciones quirúrgicas por especialidad, verificación D1–D8 (énfasis D7) y trazabilidad con el catálogo, la regla 03 y el master-prompt. | AG-ROOT (QA) |
