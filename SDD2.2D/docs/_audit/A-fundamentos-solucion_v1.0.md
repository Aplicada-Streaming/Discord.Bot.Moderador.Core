# Auditoría de Fase A — Fundamentos de la solución

| Campo | Valor |
| --- | --- |
| Fase auditada | A (nivel solución) |
| Nivel | Solución (caso degenerado, layout aplanado bajo `SDD2.2D/docs/`) |
| Alcance | 00_contexto y 01_necesidades_negocio |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación) |
| Fecha | 2026-06-20 |
| Documento | A-fundamentos-solucion_v1.0.md |
| Versión | 1.0 |
| Insumos de referencia | `devs/rules/00_rules_contexto.md` (v1.3), `devs/rules/01_rules_necesidades_negocio.md` (v1.2), `devs/intake/SOLUTION-MANIFEST-discord-bots-admin_v1.0.md`, `devs/intake/SOLUTION-INTAKE-discord-bots-admin_v1.0.md` |

---

## 1. Resumen ejecutivo

Se auditaron 13 entregables de la Fase A: 4 documentos de 00_contexto (visión, alcance, roadmap, README) y 9 de 01_necesidades_negocio (índice maestro, README y 7 archivos NB). Los entregables cumplen D1 a D8, respetan la estructura obligatoria de §4 de ambas reglas, satisfacen los 11 criterios de §6 de 00 y los 14 de §6 de 01, y mantienen coherencia cross-doc (IDs NB-01..NB-07 y CU-01..CU-15 sin duplicar, dependencias acíclicas con grado máximo 2, glosarios sin contradicción, 14/14 enlaces del índice y del README resuelven). Las omisiones de `compatibilidad-plataformas` y `acuerdo-equipo` están declaradas con motivo y categoría D8 en el README de 00, como exige la regla. No se detectó vocabulario prohibido del dominio fuente del bootstrap ni stack hardcodeado en visión/alcance, ni emojis ni negritas decorativas.

Hallazgos por nivel: P0 = 0; P1 = 0; P2 = 2; P3 = 3.

Veredicto: APROBADO CON OBSERVACIONES (sin P0 ni P1; solo P2 y P3 menores, no bloqueantes).

---

## 2. Matriz D1-D8 por documento

Convención: OK = conforme; n/a = no aplica. D1 idioma rioplatense con tildes/eñes en cuerpo; D2 encoding UTF-8 sin BOM / EOL LF; D3 filename ASCII kebab lowercase; D4 sufijo `_v1.0` con guion bajo; D5 versionado; D6 trazabilidad en cabecera; D7 sin vocabulario prohibido / sin stack hardcodeado; D8 tipo de proyecto correcto (web-monolith).

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 00_contexto/vision-producto_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 00_contexto/alcance-proyecto_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 00_contexto/roadmap-producto_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 00_contexto/README.md | OK | OK | OK | n/a (sin versión) | n/a | OK (insumos upstream) | OK | OK |
| 01/necesidades-negocio_v1.0.md (índice) | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/README.md | OK | OK | OK | n/a (sin versión) | n/a | OK | OK | OK |
| 01/NB-01-corte-automatico-rafaga-distribuida_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/NB-02-limpieza-retroactiva-mensajes_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/NB-03-contencion-contenido-no-deseado_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/NB-04-trazabilidad-incidentes-falsos-positivos_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/NB-05-configuracion-autonoma-moderacion_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/NB-06-operacion-confiable-validacion-previa_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |
| 01/NB-07-mitigacion-moderacion-erronea_v1.0.md | OK | OK | OK | OK | OK | OK | OK | OK |

Notas de verificación D2/D3/D4/D7:

- D2: verificado a nivel byte. 0 bytes CR (0x0d) en los 13 archivos (EOL LF puro); ningún archivo arranca con BOM `ef bb bf`; codificación UTF-8 (tildes y eñes representadas en 2 bytes correctos).
- D3/D4: los 7 filenames NB validan contra `^NB-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$`; los 3 documentos de 00 usan `<kebab>_v1.0.md` con guion bajo. Ningún `.v` antes de la versión, ninguna mayúscula en el kebab, sin acentos en filename.
- D7: barrido de stack hardcodeado (.NET, C#, Blazor, MudBlazor, Discord.Net, SQLite, EF Core, Raspberry, Raspbian, armv7, linux-arm, systemd, Docker, Argon2, PBKDF2, AES, xUnit, Playwright, GitHub Actions, SemVer, GitVersion, Nerdbank, NuGet, MAUI) sin coincidencias en 00/01. Barrido del vocabulario fuente del bootstrap (impresoras térmicas, ESC-POS, Bluetooth, DSL) sin coincidencias. "Discord" aparece como dominio legítimo del cliente (integración obligatoria del intake), no como stack del bootstrap. Visión y alcance describen la plataforma en lenguaje de negocio ("hardware propio de bajo consumo", "filtro nativo de la plataforma", "interfaz de programación y canal de eventos en tiempo real"), difiriendo el detalle técnico a las categorías 05 y 09.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 Categoría 00 (cabecera §4.1 + secciones §4.2)

vision-producto (esperadas §1 a §10):

| Cabecera | §1 | §2 | §3 | §4 | §5 | §6 | §7 | §8 | §9 | §10 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| OK (8 campos) | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |

Tablas tipo §4.4 presentes: Stakeholders con columna "nivel de involucramiento" (OK), Objetivos SMART con "responsable" (OK), Métricas con "fuente del dato" (OK), Riesgos con ID/probabilidad/impacto/mitigación/responsable (OK), Glosario (OK).

alcance-proyecto (esperadas §1 a §10):

| Cabecera | §1 | §2 | §3 | §4 | §5 | §6 | §7 | §8 | §9 | §10 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| OK | OK | OK | OK | OK (4.1/4.2/4.3) | OK | OK | OK | OK | OK | OK |

Tabla de exclusiones §5 con columnas funcionalidad / justificación / versión futura (OK).

roadmap-producto (esperadas §1 a §6):

| Cabecera | §1 | §2 | §3 | §4 | §5 | §6 |
| --- | --- | --- | --- | --- | --- | --- |
| OK | OK | OK | OK | OK | OK | OK |

Tabla de hitos §2 y tabla de criterios de transición §5 con checklist `- [ ]` (OK).

README de 00: enumera los documentos con propósito/estado/orden, declara documentos omitidos con motivo y categoría D8, lista stakeholders e insumos upstream (OK, recomendado por §3.4).

### 3.2 Categoría 01 (cabecera §4.1 + 10 secciones §4.2)

Las 7 NB presentan la cabecera en formato tabla (Proyecto, Documento, Versión, Estado, Fecha, Autor, Trazabilidad upstream, Trazabilidad downstream) y exactamente las 10 secciones numeradas §1 a §10 en el orden de §4.2.

| NB | Cabecera | §1 | §2 | §3 | §4 | §5 | §6 | §7 | §8 | §9 | §10 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| NB-01 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| NB-02 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| NB-03 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| NB-04 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| NB-05 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| NB-06 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| NB-07 | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK | OK |

Índice maestro `necesidades-negocio_v1.0.md`: cabecera con campos adicionales "Cantidad de NB" (7) y "Versión del catálogo de NB" (1.0) según §4.1; tabla resumen (Tabla D), mapa de dependencias, trazabilidad agregada (OK).

README de 01: presente y justificado (7 NB > 5, §3.4); incluye tabla NB-XX/título/impacto/prioridad/estado/enlace, mapa de dependencias, orden de lectura y RACI (OK).

---

## 4. Cumplimiento de §6 — Categoría 00 (11 ítems)

| # | Criterio de aceptación (§6 de 00_rules_contexto) | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | Visión expresa el problema en lenguaje de negocio, sin stack/frameworks/patrones | CUMPLE | vision §1-§3 describen spam, ráfaga distribuida, filtro nativo; sin stack |
| 2 | Alcance: ≥5 capacidades incluidas y ≥3 exclusiones con justificación | CUMPLE | alcance §4.1 lista 12 capacidades; §5 lista 6 exclusiones con justificación y versión futura |
| 3 | Roadmap: ≥3 hitos con criterios de avance verificables `- [ ]` | CUMPLE | roadmap §2 tiene 7 fases; §5 tiene checklists `- [ ]` por transición |
| 4 | Objetivos SMART: ≥3 con métrica numérica, target y plazo | CUMPLE | vision §5 tiene 3 objetivos (≥95%, ≤2%, ≥98%) con plazo y responsable |
| 5 | Stakeholders: ≥1 por categoría (propietario, implementador, beneficiario) con rol concreto | CUMPLE | vision §2: Propietario, Implementador (Fernando), Beneficiario/operador, Beneficiario indirecto |
| 6 | Glosario: ≥10 términos (proyecto >2 personas) o ≥5 (individual) | CUMPLE | vision §9 tiene 10 términos del dominio; supera el mínimo de 5 del proyecto individual |
| 7 | compatibilidad-plataformas declara plataformas target de §17 P.9 cuando aplica por D8 | CUMPLE (por omisión declarada) | web-monolith con navegadores evergreen; omisión declarada en README de 00 con motivo |
| 8 | acuerdo-equipo declara herramientas/ceremonias/branching/SLA cuando aplica | CUMPLE (por omisión declarada) | equipo_n=1; omisión declarada en README de 00 con motivo |
| 9 | Cada documento declara trazabilidad upstream (secciones del intake) y downstream (categorías) | CUMPLE | las 3 cabeceras de 00 citan secciones específicas del intake y categorías downstream |
| 10 | Filename `<kebab>_v1.0.md` con guion bajo antes de versión | CUMPLE | vision/alcance/roadmap con `_v1.0.md` |
| 11 | Sin emojis, sin negritas decorativas, sin stack/frameworks/ejemplos del dominio fuente del bootstrap | CUMPLE | barrido sin emojis; `**` solo en cabecera obligatoria §4.1; sin vocabulario prohibido |

Resultado 00: 11/11 cumplen.

---

## 5. Cumplimiento de §6 — Categoría 01 (14 ítems)

| # | Criterio de aceptación (§6 de 01_rules_necesidades_negocio) | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | Existe el índice maestro `necesidades-negocio_v1.0.md` en la raíz con tabla resumen | CUMPLE | índice presente con tabla §2 de las 7 NB |
| 2 | ≥3 archivos `NB-XX-<kebab>_v1.0.md` en `necesidades-de-negocio/` | CUMPLE | 7 NB en la subcarpeta |
| 3 | Cada NB con las 10 secciones §1 a §10 en el orden de §4.2 | CUMPLE | verificado: §1..§10 en orden en las 7 NB |
| 4 | Cada NB con ≥4 criterios de éxito SMART en §5 (métrica/target/plazo) | CUMPLE | cada §5 tiene 4 filas con número, unidad y plazo |
| 5 | Cada NB declara MoSCoW en §9 con justificación de 1 línea | CUMPLE | §9: NB-01..05 Must, NB-06/07 Should, con justificación |
| 6 | Cada NB con trazabilidad upstream a intake y/o 00_contexto | CUMPLE | cabeceras citan secciones del intake + vision/alcance |
| 7 | Cada NB declara en §7 las CU previstas con estado a generar/en redacción/aprobada | CUMPLE | §7 de cada NB lista CU con estado "a generar" |
| 8 | Cada NB con ≥3 stakeholders nominales en §6 cubriendo las 3 categorías | CUMPLE | cada §6 tiene 4 filas: Propietario, Implementador, Beneficiario, Beneficiario indirecto |
| 9 | Ningún filename con `.v` ni mayúsculas; todos validan el regex | CUMPLE | 7/7 validan `^NB-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$` |
| 10 | El índice referencia las NB con paths relativos correctos y todos los enlaces resuelven | CUMPLE | 7/7 enlaces del índice resuelven a la subcarpeta |
| 11 | Ninguna NB depende de >3 NB en §8 ni hay ciclos | CUMPLE | grado máximo 2 (NB-04, NB-07); grafo acíclico |
| 12 | Si >5 NB, existe README de la sección con la tabla de §3.4 completa | CUMPLE | 7 NB; README con tabla, mapa de dependencias, orden de lectura y RACI |
| 13 | El estado de cabecera pertenece al enum cerrado | CUMPLE | todas en "Propuesto" (miembro del enum) |
| 14 | Sin emojis, sin negritas decorativas, sin términos del dominio prohibido por D7 | CUMPLE | barrido sin emojis; NB sin `**`; sin vocabulario prohibido |

Resultado 01: 14/14 cumplen.

---

## 6. Coherencia cross-doc

| Aspecto | Estado | Evidencia |
| --- | --- | --- |
| NB coherentes con visión y alcance | OK | NB-01..NB-05 cubren Must Have del intake/alcance; NB-06/NB-07 cubren Should Have; el dolor central (ráfaga distribuida) y el discriminador (canales distintos) son consistentes en visión §3, alcance §2 y NB-01 |
| IDs NB no duplicados | OK | NB-01 a NB-07, secuencia contigua sin repetición |
| IDs CU no duplicados | OK | CU-01 a CU-15, contiguos; cada CU asignada a una sola NB (sin colisión entre NB) |
| Glosario sin contradicción | OK | glosario de visión §9 (10 términos) es subconjunto coherente del glosario del intake §12; mismas definiciones de ráfaga distribuida, evento/política, regla de contenido/conducta, exención, modo simulación, borrado retroactivo, desbaneo, incidente, canal de salida |
| Dependencias acíclicas y consistentes índice/README/NB §8 | OK | índice §3, README y §8 de cada NB declaran las mismas aristas: NB-02→NB-01, NB-03→NB-01, NB-04→{NB-01,NB-05}, NB-06→NB-05, NB-07→{NB-01,NB-05}; NB-01 y NB-05 raíces; grado máximo 2 |
| Enlaces del índice y README resuelven | OK | 7 enlaces del índice + 7 del README = 14/14 apuntan a archivos existentes en la subcarpeta |
| Cobertura MoSCoW del intake | OK | índice §5 mapea Must/Should a NB; Could/Won't no generan NB (frontera de IA reservada, fuera de v1), coherente con intake §4 y visión §4 |
| Trazabilidad a categoría 04 (IA) | OK | índice §4 nota: el proyecto no declara LLM en v1, por lo que ninguna NB enlaza a 04; consistente con Won't Have del intake |

---

## 7. Trazabilidad upstream/downstream (§3.3 de cada regla)

| Documento | Upstream declarado | Downstream declarado | Conformidad §3.3 |
| --- | --- | --- | --- |
| vision-producto | SOLUTION-INTAKE §1,§2,§3,§8,§9,§10,§11,§12 | 01, 02, 03, 05, 07, 11 | OK (secciones específicas del intake; downstream a categorías de §3.3 de 00) |
| alcance-proyecto | SOLUTION-INTAKE §1,§4,§5,§6,§9,§10,§17 P.9 | 01, 02, 03, 05, 07, 11 | OK |
| roadmap-producto | SOLUTION-INTAKE §4,§6,§15 | 06_backlog, 07_plan-sprint | OK en downstream (coincide con §4.2 del roadmap); ver P3-01 sobre §15 |
| índice 01 | SOLUTION-INTAKE §1,§3,§4,§8,§11; vision; alcance | CU en 02; MoSCoW a 06/07; criterios a 08 | OK (cumple §3.3 de 01: intake + 00_contexto) |
| NB-01..NB-07 | SOLUTION-INTAKE (secciones específicas) + vision + alcance | CU-XX previstas en 02 | OK en las 7 NB |

---

## 8. Hallazgos enumerados

No se registran hallazgos P0 ni P1.

### P2-01 — Métrica de NB con target poco verificable como número-unidad estricto

- Nivel: P2 (medio).
- Archivo: `01_necesidades_negocio/necesidades-de-negocio/NB-03-contencion-contenido-no-deseado_v1.0.md`, §5 Criterios de éxito.
- Evidencia: la fila "Control del contenido por el administrador" usa target "≥ 1 por servidor" sobre la métrica "cantidad de criterios de contenido que el administrador puede definir y activar"; mide una capacidad de configuración más que un resultado de negocio medible en operación. El criterio §6.4 pide métricas SMART; el valor es numérico y verificable, pero es de baja densidad informativa frente a las otras tres filas de la misma tabla.
- Recomendación: reformular hacia un resultado observable (por ejemplo, porcentaje de incidentes de contenido individual contenidos en producción) o consolidar con la fila "Cobertura del mensaje individual" para evitar redundancia.

### P2-02 — Criterio de éxito de NB-07 expresado como conteo fijo en lugar de tasa

- Nivel: P2 (medio).
- Archivo: `01_necesidades_negocio/necesidades-de-negocio/NB-07-mitigacion-moderacion-erronea_v1.0.md`, §5.
- Evidencia: la fila "Acciones no repetidas por ataque" fija target "1" para la métrica "cantidad de acciones repetidas sobre el mismo usuario durante una misma ráfaga". El enunciado de la métrica ("acciones repetidas") con target 1 es ambiguo: si se interpreta literalmente, 1 acción repetida contradice el objetivo de antirrebote (que sería 0 repeticiones). Probablemente se quiso decir "una sola acción por usuario por ráfaga".
- Recomendación: precisar la métrica como "acciones por usuario por ráfaga = 1" o "acciones repetidas adicionales = 0" para eliminar la ambigüedad de lectura.

### P3-01 — Roadmap cita §15 del intake como upstream, no listado en §3.3 de 00

- Nivel: P3 (bajo).
- Archivo: `00_contexto/roadmap-producto_v1.0.md`, cabecera y §6.
- Evidencia: la trazabilidad upstream cita SOLUTION-INTAKE §4, §6 y §15. La §3.3 de 00_rules enumera como upstream §1,§3,§5,§9,§10,§11,§13,§17 P.9. §15 (esquema de descomposición y delivery) es la fuente correcta y natural de un roadmap por rebanadas, pero no figura en la lista canónica de §3.3.
- Recomendación: aceptable tal cual (el origen es real y pertinente); opcionalmente, ampliar §3.3 de la regla 00 para reconocer §15 como upstream legítimo del roadmap, cerrando la brecha formal.

### P3-02 — Roadmap declara downstream solo a 06 y 07, omitiendo la cadena ampliada

- Nivel: P3 (bajo).
- Archivo: `00_contexto/roadmap-producto_v1.0.md`, cabecera.
- Evidencia: la cabecera declara downstream "06_backlog, 07_plan-sprint", consistente con §4.2 §6 del propio roadmap. El criterio general §6.9 de 00 pide downstream "con detalle" hacia 01/02/05/07/11; para el roadmap el destino real son 06/07, por lo que la divergencia es esperable y no es defecto, pero queda como nota de granularidad.
- Recomendación: ninguna acción obligatoria; el alcance downstream del roadmap es correcto para su naturaleza.

### P3-03 — Estado homogéneo "Propuesto" en todos los entregables

- Nivel: P3 (bajo).
- Archivo: todos los documentos de 00 y 01.
- Evidencia: los 13 documentos están en estado "Propuesto". Es un estado válido del enum y coherente con una fase recién generada pendiente de promoción; se señala solo para visibilidad de que ningún documento está aún "Aprobado/Vigente".
- Recomendación: promover a "Aprobado" tras la aceptación de esta auditoría, como parte del cierre de fase.

---

## 9. Veredicto final

APROBADO CON OBSERVACIONES.

Fundamento: no se detectó ningún hallazgo P0 (no hay ruptura de trazabilidad, ni violación de D1-D8, ni documento obligatorio omitido sin declaración, ni vocabulario prohibido, ni cabecera o checklist faltante) ni ningún hallazgo P1. Las omisiones de `compatibilidad-plataformas` y `acuerdo-equipo` están correctamente declaradas con motivo y categoría D8 en el README de 00 (no faltan silenciosamente). Los 11 criterios de §6 de 00 y los 14 de §6 de 01 se cumplen contra el contenido real. Las observaciones registradas son 2 P2 (redacción de métricas en NB-03 y NB-07) y 3 P3 (notas de trazabilidad y de estado), todas no bloqueantes.

Condiciones para promover la fase (recomendadas, no bloqueantes):

1. Ajustar la redacción de las métricas señaladas en P2-01 (NB-03) y P2-02 (NB-07) para que el target sea inequívoco; puede hacerse en una corrección menor sin cambio de versión mayor.
2. Al cerrar la fase, considerar la promoción del estado de los documentos de "Propuesto" a "Aprobado" (P3-03).
3. Opcional: reflejar §15 del intake como upstream legítimo del roadmap en la regla 00 §3.3 (P3-01), a criterio del mantenedor de las reglas.

La cadena de trazabilidad D6 (SOLUTION-INTAKE → 00_contexto → NB → CU previstas) está habilitada y consistente; la Fase A puede promover a la generación de la categoría 02 (especificación funcional) sobre la base de estos fundamentos.
