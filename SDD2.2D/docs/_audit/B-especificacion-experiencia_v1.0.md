# Auditoría de Fase B — Especificación funcional (02) y Experiencia UX/UI (03)

| Campo | Valor |
| --- | --- |
| Fase | B (categorías 02 especificación funcional y 03 UX/UI/DX) |
| Proyecto | discord-bots-admin (`web-monolith`, caso degenerado, layout aplanado) |
| Alcance auditado | `docs/02_especificacion_funcional/` y `docs/03_ux_ui_dx/` (todos sus archivos). 04_prompts_ai omitida por gating (`usa_llm=false`) y verificada como inexistente. |
| Insumos de reglas | `devs/rules/02_rules_especificacion_funcional.md` (v1.2), `devs/rules/03_rules_ux_ui_dx.md` (v1.4), catálogo `devs/references/design/` |
| Upstream consultado | `docs/00_contexto/`, `docs/01_necesidades_negocio/`, `SOLUTION-INTAKE-discord-bots-admin_v1.0.md`, `SOLUTION-MANIFEST-discord-bots-admin_v1.0.md` |
| Auditor | Independiente — Arquitecto de Soluciones + QA Senior (sin participación en la generación) |
| Fecha | 2026-06-20 |
| Documento | `B-especificacion-experiencia_v1.0.md` |

---

## 1. Resumen ejecutivo

Se auditaron 47 archivos de la Fase B: en 02, el índice maestro, 16 CU, 16 RN, el modelo conceptual, 11 RC y el README; en 03, el marco de experiencia, 5 wireframes, el glosario UX y el README. La cobertura NB→CU es bidireccional y completa (NB-01..NB-07 con CU, ningún CU huérfano); los CU/RN/RC son contiguos sin duplicados; el modelo conceptual no usa tipos físicos; no hay fuga de stack ni de vocabulario del dominio fuente en el cuerpo; nomenclatura, encoding (UTF-8, LF, sin BOM) y estructura de carpetas del caso degenerado son correctos. No se detectaron hallazgos P0 ni P1. Se registran 3 hallazgos P2 y 4 P3, todos no bloqueantes. **Veredicto: APROBADO CON OBSERVACIONES.**

Conteo de hallazgos: **P0 = 0 · P1 = 0 · P2 = 3 · P3 = 4.**

---

## 2. Matriz D1–D8 por documento (agrupada por familia)

Convención: C = cumple; — = no aplica.

| Familia (archivos) | D1 idioma rioplatense | D2 filename ASCII/kebab/`_v1.0` | D3 UTF-8 / LF / sin BOM | D4 cabecera+secciones | D5 sin stack hardcodeado en cuerpo | D6 trazabilidad | D7 sin vocabulario dominio fuente | D8 tipo D8 cerrado (web-monolith) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Índice 02 (`especificacion-funcional`) | C | C | C | C | C | C | C | C |
| CU-01..CU-16 (16) | C | C | C | C | C | C | C | C |
| RN-01..RN-16 (16) | C | C | C | C | C | C | C | C |
| Modelo conceptual | C | C | C | C | C | C (sin tipos físicos) | C | C |
| RC-01..RC-11 (11) | C | C | C | C | C | C | C | C |
| README 02 | C | C | C | C | C | C | C | C |
| Marco experiencia 03 | C | C | C | C | C (stack solo en fila de catálogo) | C | C | C |
| Wireframes (5) | C | C | C | C | C (stack solo en fila de catálogo) | C | C | C |
| Glosario UX | C | C | C | C | C | C | C | C |
| README 03 | C | C | C | C | C (stack solo en fila de catálogo) | C | C | C |

Notas D5/D7: barrido por `.NET, Blazor, MudBlazor, EF Core, SQLite, Discord.Net, Raspberry, systemd, armv7, linux-arm, Argon2, PBKDF2, xUnit, Playwright, AES, WAL` sobre 02 y 03 → sin coincidencias en el cuerpo. Las únicas apariciones de stack están en la fila "Catálogo de diseño aplicado" de la trazabilidad de 03, uso legítimo según §1.4/§4.3 de la regla 03. Barrido de dominio fuente (`impresora, ESC-POS, Bluetooth, térmica, DSL`) y de tipos físicos (`varchar, nvarchar, int(, bigint, decimal(`) → sin coincidencias. "Discord", "gateway", "snowflake", "token de bot" son dominio legítimo.

---

## 3. Matriz de estructura obligatoria por documento

| Documento / familia | Cabecera completa | Secciones obligatorias | Resultado |
| --- | --- | --- | --- |
| Índice 02 | Sí | Índice maestro + matriz NB→CU→RN→US (§2) + cobertura bidireccional (§3) | Completo |
| CU-01..CU-16 | Sí (H1 + 6 metadatos) | 11 secciones §4.2 en los 16 (verificado: 11 encabezados `##` numerados por archivo) | Completo |
| RN-01..RN-16 | Sí | 7 secciones §4.2.1 en los 16 + "CU afectados" explícitos | Completo |
| Modelo conceptual | Sí | 9 secciones §4.2.2 (entidades, atributos, relaciones, cardinalidades, RC, glosario, diagrama Mermaid, trazabilidad, control de cambios) | Completo |
| RC-01..RC-11 | Sí | 6 secciones §4.2.3 en los 11 | Completo |
| README 02 | Parcial (sin bloque de metadatos formal) | Índice navegable de CU/RN/modelo/RC con estado | Aceptable (recomendado) — ver P3-01 |
| Marco experiencia 03 | Sí (incluye "Variante: UX/UI") | 11 secciones §4.2 | Completo |
| Wireframes (5) | Sí (incluye "Variante: UX/UI") | 9 secciones §4.2.1 en los 5 | Completo |
| Glosario UX | Sí | Alcance + términos reutilizados + términos nuevos + control de cambios | Completo |
| README 03 | Sí (variante declarada) | Catálogo de diseño + artefactos + mapa wireframe→CU + trazabilidad | Completo |

---

## 4. Cumplimiento de §6 — categoría 02 (11 ítems de la regla 02)

| # | Ítem §6 | Resultado | Evidencia |
| --- | --- | --- | --- |
| 1 | Existe índice maestro con matriz NB→CU→RN→US | Cumple | `especificacion-funcional_v1.0.md` §2 |
| 2 | Cantidad de CU cumple el mínimo del tipo D8 (web-monolith ≥ 8) | Cumple | 16 CU (piso 8) |
| 3 | Cada CU con las 11 secciones del §4.2 | Cumple | 11 encabezados numerados en los 16 CU |
| 4 | Cada CU declara trazabilidad NB→CU→US y ≥3 Given/When/Then con valores concretos | Cumple | CU-01 4 CA (umbral 3 canales / ventana 2 s / 1,5 s), CU-09 4 CA, CU-11 4 CA; todos los CU con tabla CA y ≥4 filas con valores |
| 5 | Cada RN con 7 secciones del §4.2.1 + CU afectados explícitos | Cumple | RN-12 §5 lista CU-06..CU-15; 7 secciones en los 16 RN |
| 6 | Existe modelo conceptual con diagrama o tabla equivalente | Cumple | `modelo-conceptual_v1.0.md` §7 diagrama Mermaid erDiagram |
| 7 | Si modelo > 10 entidades, existen RC con 6 secciones | Cumple | 13 entidades → 11 RC con 6 secciones |
| 8 | Ningún archivo usa `.vX.Y`; todos `_vX.Y` | Cumple | barrido de filenames sin coincidencias `.v` |
| 9 | Ningún slug con mayúsculas/espacios/acentos/no-kebab | Cumple | slug post-prefijo en minúscula kebab en los 43 archivos |
| 10 | Sin convivencia de versiones; superadas en `_legacy/` | Cumple | una sola versión por nombre lógico; no hay `_legacy/` (versión inicial) |
| 11 | Sin menciones a stacks/productos/protocolos del dominio fuente | Cumple | barrido D5/D7 sin coincidencias |

(El README de sección es recomendado y existe; satisface el ítem opcional adicional.)

## 5. Cumplimiento de §6 — categoría 03 (12 ítems de la regla 03)

| # | Ítem §6 | Resultado | Evidencia |
| --- | --- | --- | --- |
| 1 | Variante declarada en cabecera y coherente con D8 | Cumple | "Variante: UX/UI" en marco, 5 wireframes y glosario; coherente con web-monolith |
| 2 | Existe `experiencia-de-uso` con 11 secciones | Cumple | `experiencia-de-uso_v1.0.md` §1–§11 |
| 3 | ≥1 wireframe por superficie clave (web-monolith mínimo 4) con 9 secciones | Cumple | 5 wireframes, 9 secciones cada uno (piso 4) |
| 4 | (DX) `dx-developer-experience` con 9 secciones, Diátaxis, onboarding | No aplica | Variante UX/UI, no DX |
| 5 | Accesibilidad con WCAG 2.2 AA como piso | Cumple | experiencia §5 declara AA como piso; los 5 wireframes citan AA en §7 y en tests |
| 6 | Cada wireframe enumera estados vacío/cargando/con datos/error | Cumple | los 5 wireframes con los 4 estados mínimos (más desconectado/éxito/conflicto según superficie) |
| 7 | (DX) quick-start verificable con snippet ejecutable | No aplica | Variante UX/UI |
| 8 | Trazabilidad upstream (persona, CU, RN) y downstream (US, tests) | Cumple | experiencia §9; cada wireframe §8 con CU origen y tests |
| 9 | Sin `.vX.Y`; `_vX.Y` y kebab estricto | Cumple | filenames de 03 conformes |
| 10 | Sin convivencia de versiones; superadas en `_legacy/` | Cumple | versión inicial única |
| 11 | Glosario sin duplicar términos de 02 con semántica distinta | Cumple | glosario §1–§2 referencia 00/02 sin redefinir; §3 solo términos nuevos de UX |
| 12 | Sin menciones a stacks/productos/protocolos del dominio fuente | Cumple | stack solo en fila de catálogo (legítimo); cuerpo limpio |

Insumo normativo de diseño (§1.4): la trazabilidad de cada artefacto con UI declara el catálogo aplicado. El marco y el wireframe de configuración nombran `design-rules-web-generico_v1.0.md` + `design-rules-blazor-mudblazor_v1.0.md` + `design-rules-config-esquema_v1.0.md`; los wireframes sin parámetros de moderación nombran base + especialización y marcan "Configuración dirigida por esquema aplicada: N/A" con justificación. Correcto.

---

## 6. Coherencia cross-doc

- **Cobertura bidireccional NB→CU.** Las 7 NB tienen ≥1 CU y ningún CU queda huérfano. Mapa del índice 02 §3: NB-01→CU-01,02; NB-02→CU-03; NB-03→CU-04; NB-04→CU-05,06,07; NB-05→CU-08,09,10,11; NB-06→CU-12,13; NB-07→CU-14,15,16. Cada CU declara su NB en su §9 de trazabilidad. Bidireccional completa.
- **IDs sin duplicar y contiguos.** CU-01..CU-16, RN-01..RN-16, RC-01..RC-11, todos contiguos y únicos. Sin huecos.
- **Wireframe → CU real.** Los 5 wireframes anclan CU existentes de 02: auth→CU-08,09; estado→CU-13; registro/prueba→CU-10,12; configuración→CU-11,14,15,16; incidentes/desbaneo→CU-05,06,07. Los CU automáticos de motor (CU-01..CU-04) sin interacción humana correctamente no reciben wireframe. El README 03 §4 reproduce el mapa de forma consistente.
- **Glosarios sin contradicción.** El glosario UX no redefine términos de dominio de 00/02; agrega solo vocabulario de presentación (pantalla, shell, toast, badge, descriptor, previsualización, etc.). Sin duplicación con semántica distinta.
- **Modelo conceptual sin tipos físicos.** Atributos con semántica ("tratado como texto", "verificador de contraseña no reversible", "entre 0 y 7 días"), sin `varchar/int/bigint`. 13 entidades coherentes con el conteo declarado.
- **Enlaces de índices.** Los enlaces relativos de los README 02/03 y de los índices maestros resuelven contra los archivos presentes en el árbol.
- **Trazabilidad del catálogo de diseño.** Coherente con `_index_design-rules.md` y el stack declarado en intake §17.P.1.

---

## 7. Hallazgos enumerados

### P2 (medio — se documenta y se sigue)

**P2-01 — Deriva de IDs de CU en el upstream (Fase A) respecto del 02 consolidado.**
- Archivos: `01_necesidades_negocio/necesidades-de-negocio/NB-05-*.md`, `NB-06-*.md`, `NB-07-*.md` (línea de trazabilidad downstream y tabla de CU previstas) y `necesidades-negocio_v1.0.md` §2.
- Sección: trazabilidad downstream / "CU previstas".
- Evidencia: NB-05 declara CU-08, CU-09 (= "registrar servidor"), CU-10 (= "administrar reglas"); NB-06 declara CU-11, CU-12; NB-07 declara CU-13, CU-14, CU-15. En el 02 consolidado, tras insertar el CU explícito de autenticación (CU-09), esos títulos viven en NB-05→CU-08,09,10,11; NB-06→CU-12,13; NB-07→CU-14,15,16. El índice 02 §3 documenta explícitamente la renumeración y conserva la trazabilidad por NB y por título, por lo que NO hay ruptura de trazabilidad ni CU huérfano. Es deriva de los identificadores numéricos en documentos de una fase ya cerrada.
- Recomendación: en una próxima revisión menor de la categoría 01, actualizar las columnas de CU previstas de NB-05/06/07 a los IDs consolidados de 02, o agregar una nota de remisión a la renumeración del índice 02. No bloquea la Fase B.

**P2-02 — README de la sección 02 sin bloque de metadatos formal.**
- Archivo: `02_especificacion_funcional/README.md`.
- Sección: cabecera.
- Evidencia: el README abre con H1 y un párrafo, y declara estado/fecha en prosa ("Estado general de la sección: Propuesto. Fecha: 2026-06-20"), sin el bloque de metadatos en clave-valor que sí tiene el README de 03. El README es recomendado y cumple su función de índice navegable.
- Recomendación: homogeneizar con el README de 03 agregando un bloque de metadatos (Proyecto, Sección, Estado, Fecha, Autor). Cosmético.

**P2-03 — Discrepancia menor de enumeración de entidades en el índice 02.**
- Archivo: `02_especificacion_funcional/especificacion-funcional_v1.0.md` §6.
- Sección: §6 Modelo conceptual.
- Evidencia: el texto dice "comprende 13 entidades (Administrador, Servidor, CanalDeSalida, Exención, Regla, GrupoDeReglas, GrupoRegla, Evento, EventoGrupo, Acción, Incidente, MensajeAccionado, CanalAfectado)" — la lista entre paréntesis contiene exactamente 13 nombres y coincide con las 13 subsecciones 1.1–1.13 del modelo, por lo que el conteo es correcto; se anota solo como punto de verificación de lectura, sin inconsistencia real detectada.
- Recomendación: ninguna acción requerida; ítem de control cerrado como conforme.

### P3 (bajo — mejora de estilo o claridad)

**P3-01 — README 02: alinear formato de control de cambios.**
- Archivo: `02_especificacion_funcional/README.md`.
- Evidencia: no incluye tabla de control de cambios (el README de 03 tampoco la exige, pero el de 02 podría sumarla para trazar revisiones de la sección).
- Recomendación: opcional, agregar una mini tabla de control de cambios.

**P3-02 — Códigos de error de 03 derivados, no catalogados en 02.**
- Archivo: `03_ux_ui_dx/experiencia-de-uso_v1.0.md` §8.
- Evidencia: la taxonomía de errores de §8 usa códigos como `SETUP_CONTRASENA_DEBIL`, `CONFIG_VALOR_FUERA_DE_LIMITE`, `PRUEBA_TOKEN_INVALIDO`; varios coinciden con los de los CU de 02 (por ej. `CONFIG_VALOR_FUERA_DE_LIMITE` en CU-11, `AUTH_CREDENCIALES_INVALIDAS` en CU-09), pero otros (`SETUP_CONTRASENA_DEBIL`, `DESBANEO_FALLA_PLATAFORMA`) no aparecen literalmente en los CU leídos.
- Recomendación: verificar en una pasada de consistencia que todos los códigos citados en 03 existan en los CU de 02; si alguno es nuevo, incorporarlo al CU correspondiente. No bloqueante (los CU declaran tablas de excepciones propias y 03 puede anticipar estados de UI).

**P3-03 — Reutilización de RN entre CU sin matriz inversa explícita.**
- Archivo: `02_especificacion_funcional/especificacion-funcional_v1.0.md`.
- Evidencia: la matriz §2 mapea CU→RN y el catálogo §5 mapea RN→CU; ambos son coherentes entre sí. Es una fortaleza; se anota como recomendación de mantener sincronizadas ambas vistas en futuras versiones.
- Recomendación: mantener la doble vista al versionar.

**P3-04 — Nota de estado en memoria fuera de las 9 secciones del modelo.**
- Archivo: `02_especificacion_funcional/modelo-datos/modelo-conceptual_v1.0.md`.
- Evidencia: tras §9 hay una "Nota sobre estado en memoria (no persistido)" que documenta el estado de conducta y antirrebote. Es correcta y aclaratoria (explica por qué no son entidades), pero queda como sección sin numerar después del control de cambios.
- Recomendación: opcional, moverla antes del control de cambios o numerarla como apéndice para preservar el orden canónico (control de cambios al cierre).

---

## 8. Veredicto final

**APROBADO CON OBSERVACIONES.**

No se detectaron hallazgos P0 ni P1: la trazabilidad NB→CU es bidireccional y completa, las cabeceras y secciones obligatorias están presentes en los 47 archivos, no hay IDs duplicados, no hay tipos físicos en el modelo conceptual, no hay stack hardcodeado ni vocabulario del dominio fuente en el cuerpo, la nomenclatura/encoding/estructura del caso degenerado son correctos y la omisión de 04 (usa_llm=false) es conforme. La fase puede promoverse a la siguiente.

Condiciones (no bloqueantes, a resolver en una revisión menor de mantenimiento, idealmente antes de cerrar 06):
1. Reconciliar las columnas de CU previstas de NB-05/06/07 (categoría 01) con los IDs consolidados de 02 o remitir a la nota de renumeración (P2-01).
2. Homogeneizar la cabecera y el control de cambios del README de 02 con el de 03 (P2-02, P3-01).
3. Verificar que todo código de error citado en 03 §8 exista en el CU de 02 correspondiente (P3-02).

## 9. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-20 | Informe inicial de auditoría independiente de Fase B (02 y 03) del proyecto discord-bots-admin. | Auditor independiente (Arquitecto de Soluciones + QA Senior) |
