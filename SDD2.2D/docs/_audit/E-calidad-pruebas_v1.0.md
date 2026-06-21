# Auditoría de Fase E — Calidad y pruebas (08)

**Fase auditada:** E (categoría 08 calidad_y_pruebas)
**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Layout:** caso degenerado (08 directo bajo `SDD2.2D/docs/`, sin `proyectos/<kebab>/`)
**Alcance:** `SDD2.2D/docs/08_calidad_y_pruebas/` completa: estrategia-calidad, estrategia-testing, plan-pruebas, matriz-cobertura-pruebas, casos-prueba-referenciales, criterios-validacion, definition-of-done, guia-testing-extensibilidad y README
**Auditor:** Arquitecto de Soluciones + QA Senior, independiente (no participó de la generación de la Fase E)
**Fecha:** 2026-06-20
**Reglas de referencia:** `08_rules_calidad_y_pruebas.md` v1.2 (§6 criterios; §2.2 pirámide y cobertura; §3 nomenclatura; §4 estructura; §4.10 anti-patrones)
**Insumos upstream:** 02 (CU-01..CU-16 con CA-01..CA-04, RN-01..RN-16), 05 (`arquitectura-solucion_v1.0.md` §8 tabla de NFR, extensibilidad), 06 (`definition-of-ready_v1.0.md` §5), 07 (`mini-plan_v1.0.md` §5), intake (`SOLUTION-INTAKE-discord-bots-admin_v1.0.md` §17 P.6/P.8/P.10)

---

## 1. Resumen ejecutivo

La Fase E del proyecto discord-bots-admin está sustancialmente completa y bien construida: los nueve documentos existen (ocho obligatorios + la guia-testing-extensibilidad correctamente presente por `tiene_extensibilidad = true`), la matriz tiene las tres tablas obligatorias más cobertura por capa, la pirámide 70/20/10 está justificada, los gates de cobertura coinciden con el intake §17 P.6 y los quality gates con §17 P.8, la DoD canónica cubre las cuatro capas y satisface las referencias forward de 06 y 07 sin redefinirse, el tooling se describe por capacidad sin nombrar productos comerciales y la cobertura se reporta por capa (no número global único). El defecto de fondo es de trazabilidad fina: la matriz CU↔Tests no representa todos los criterios de aceptación de los 16 CU; 11 CA de 9 CU quedan sin fila ni TC, en contradicción con la propia política BDD declarada en `estrategia-testing_v1.0.md` §4 y con la DoD §1.1. Conteo de hallazgos: P0 = 0; P1 = 2; P2 = 2; P3 = 3. **Veredicto: APROBADO CON OBSERVACIONES.** La cadena puede promoverse a 09 condicionada al cierre de los dos hallazgos P1.

---

## 2. Matriz de conformidad D1–D8 por documento

Leyenda: OK = conforme. Documentos: EC = estrategia-calidad; ET = estrategia-testing; PP = plan-pruebas; MX = matriz-cobertura-pruebas; CP = casos-prueba-referenciales; CV = criterios-validacion; DoD = definition-of-done; GE = guia-testing-extensibilidad; RD = README.

| Criterio | EC | ET | PP | MX | CP | CV | DoD | GE | RD |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| D1 idioma rioplatense con tildes/eñes | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| D2 filename ASCII, kebab lowercase, sufijo `_v1.0` con guion bajo | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| D3 identificadores TC-XX de dos dígitos | n/a | OK | OK | OK | OK | OK | OK | OK | OK |
| D4 codificación UTF-8 | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| D5 sin emojis ni negritas decorativas | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| D6 trazabilidad upstream (CU/RN/NFR/ADR) | OK | OK | OK | OK | OK | OK | OK | OK | OK |
| D7 sin stack/marca de framework ni vocabulario del dominio fuente | OK | obs | OK | OK | OK | OK | OK | OK | OK |
| D8 coherencia con tipo web-monolith (pirámide 70/20/10) | OK | OK | OK | OK | OK | OK | OK | OK | OK |

Notas de evidencia:

- D2: ningún archivo usa el patrón prohibido `.v1.0.md`; todos usan `_v1.0.md`. **Ningún archivo lleva el sufijo de dominio `-motor` ni otro marcador temático** (verificado contra los nueve nombres); el antecedente del fuente SDD 1.0 (`estrategia-testing-<dominio>`, `criterios-validacion-<dominio>`) está corregido. Slugs kebab lowercase y ASCII.
- D3: numeración TC-01..TC-58 contigua, de dos dígitos, sin huecos ni identificadores de tres dígitos. CU-XX, RN-XX y NFR se referencian (no se inventan), conforme a §3.2 de la regla.
- D5: el único símbolo no-ASCII recurrente es `↔` en "CU↔Tests / NFR↔Tests / RN↔Tests", terminología sancionada por la regla 08 (§3.3, §4.5). Sin emojis. Negritas limitadas a metalíneas de cabecera (estructurales).
- D6: cada TC referencia al menos un CU, RN o NFR; cada NFR numérico tiene test o medición; la DoD referencia CU mínimos y NFR. Trazabilidad presente.
- D7: barrido sobre los nueve archivos por `xUnit`, `Playwright`, `NSubstitute`, `FluentAssertions`, `WebApplicationFactory`, `Blazor`, `MudBlazor`, `SQLite`, `EF Core`, `.NET`, `Discord.Net`, `Testcontainers`, `GitHub Actions`, `systemd`, `Raspberry`, `Raspbian`, `armv7`, `Docker`, `Argon2`, `PBKDF2`, `AES`, `ESC-POS`, `Bluetooth`, `impresora térmica`: **una sola coincidencia** ("modo WAL", `estrategia-testing` §7, mecanismo de almacenamiento ligado a la persistencia del stack — ver H-04, no bloqueante). "Discord", "gateway", "snowflake", "ráfaga distribuida", "token de bot", "intents" se usan como dominio legítimo. "hash PHC" / "formato PHC" es vocabulario de estándar (no marca comercial; idéntico a RN-13 y 05 §7) y no constituye hallazgo. El tooling de test se nombra por capacidad ("framework de pruebas unitarias del runtime", "factory de aplicación web", "conducción headless del navegador") anclado a `SOLUTION-INTAKE §17 P.6`.
- D8: pirámide 70/20/10 de web-monolith (regla §2.2) declarada y justificada contra la pirámide invertida y la aplanada.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 estrategia-calidad_v1.0.md (§4.2: cinco secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 (H1 + metadatos) | OK |
| 1. Definición de calidad para el proyecto | OK |
| 2. Atributos ISO/IEC 25010 priorizados con métrica/NFR de origen | OK (8 atributos, prioridad y NFR) |
| 3. Quality gates (condición, herramienta, consecuencia) | OK (G1–G5) |
| 4. Roles QA (RACI) | OK |
| 5. Cadencia de revisión | OK |

### 3.2 estrategia-testing_v1.0.md (§4.3: siete secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Pirámide numérica con justificación anti-pirámides | OK (70/20/10) |
| 2. Cobertura mínima por capa (tabla) | OK (Dominio/Aplicación/Infra/Presentación/Global) |
| 3. Tooling por capacidad | OK |
| 4. BDD a partir de Given/When/Then | OK |
| 5. Mocks y fixtures | OK |
| 6. Datos de prueba | OK |
| 7. Ambiente de testing | OK |

### 3.3 plan-pruebas_v1.0.md (§4.4: seis secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Alcance (módulos incluidos/excluidos) | OK |
| 2. Criterios de entrada | OK |
| 3. Criterios de salida | OK |
| 4. Riesgos de calidad (impacto/probabilidad/mitigación) | OK (RC-Q1..RC-Q9, alineados con 05 §9) |
| 5. Plan por sprint (rebanada) | OK (R1..R7) |
| 6. Recursos | OK |

### 3.4 matriz-cobertura-pruebas_v1.0.md (§4.5: seis secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Propósito y alcance | OK |
| 2. Tabla CU↔Tests | OK presente, **cobertura de CA parcial** (ver §5.1 y H-01) |
| 3. Tabla NFR↔Tests (SLA + tooling) | OK (6 NFR) |
| 4. Tabla RN↔Tests | OK (16 RN) |
| 5. Tabla de cobertura por capa | OK (observadas pendientes por etapa de diseño) |
| 6. Gaps identificados | OK |

### 3.5 casos-prueba-referenciales_v1.0.md (§4.6: ocho campos por TC)

| Campo obligatorio por TC | Estado |
| --- | --- |
| Identificador + nombre kebab | OK |
| Tipo (Unit/Integration/E2E) | OK |
| CU/NFR/RN cubierto | OK |
| Setup | OK |
| Pasos Given/When/Then | OK |
| Expected | OK |
| Actual | OK (pendiente de ejecución, etapa de diseño) |
| Status | OK (Pendiente justificado) |

Verificado sobre TC-01..TC-58: los ocho campos están presentes en todos; ningún TC sin expected ni sin status.

### 3.6 criterios-validacion_v1.0.md (§4.7: seis secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Propósito | OK |
| 2. Criterios funcionales por CU crítico | OK (9 CU críticos) |
| 3. Criterios no funcionales con SLA | OK (6 NFR) |
| 4. Criterios de regresión | OK |
| 5. Criterios de calidad de código (cobertura por capa) | OK |
| 6. Excepciones documentadas (ADR) | OK |

### 3.7 definition-of-done_v1.0.md (§4.8: tres secciones, cuatro capas)

| Elemento obligatorio | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. DoD por capa — US, BT, sprint, release con `- [ ]` mecánicos | OK (las cuatro capas) |
| 2. Excepciones admitidas | OK (walking skeleton, Sprint 0, ADR, deuda con BT) |
| 3. Vigencia (fuente canónica, no redefinible) | OK |

### 3.8 guia-testing-extensibilidad_v1.0.md (obligatoria por extensibilidad)

| Elemento esperado | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| Principio: testear la superficie, no el núcleo | OK |
| Nuevo descriptor / nuevo tipo de regla / nuevo tipo de acción | OK |
| Frontera reservada (PropuestaDeConfiguracion) | OK |
| Trazabilidad (punto de extensión ↔ ADR ↔ CU/RN ↔ TC) | OK |

### 3.9 README.md de la sección

| Elemento esperado | Estado |
| --- | --- |
| Índice navegable con estado por documento | OK |
| Pirámide y cobertura | OK |
| Quality gates configurados | OK (G1–G5) |
| Enlace a la DoD canónica | OK |

---

## 4. Cumplimiento de §6 (13 ítems de la regla 08)

| # | Criterio §6 | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | estrategia-calidad con atributos ISO 25010 priorizados y quality gates | OK | 8 atributos con prioridad y NFR de origen; G1–G5 con condición/herramienta/consecuencia |
| 2 | estrategia-testing con pirámide numérica, cobertura por capa y tooling | OK | 70/20/10; tabla por capa; tooling por capacidad |
| 3 | plan-pruebas con criterios entrada/salida y riesgos por sprint | OK | §2/§3 entrada/salida; RC-Q1..RC-Q9; plan R1..R7 |
| 4 | matriz con las tres tablas obligatorias + cobertura por capa | OK | CU↔Tests (§2), NFR↔Tests (§3), RN↔Tests (§4), cobertura por capa (§5) |
| 5 | casos-prueba con ≥1 TC por CU crítico (setup/pasos/expected/status) | OK | los 9 CU críticos tienen varios TC; 58 TC con los 8 campos |
| 6 | criterios-validacion numéricos | OK | SLA por NFR; gates de cobertura por capa |
| 7 | DoD por capa (US/BT/sprint/release) con criterios mecánicos | OK | §1.1–§1.4; cada `- [ ]` con "¿cómo se valida?" |
| 8 | guia-testing-extensibilidad si el tipo admite extensión | OK | presente; correcta por `tiene_extensibilidad = true` |
| 9 | ningún archivo con sufijo de dominio (`-motor` u otro) | OK | nueve nombres con patrón `_v1.0.md` puro |
| 10 | cada NFR numérico con test asociado en la matriz | OK | latencia→TC-55, throughput→TC-56, memoria→TC-57, limpieza→TC-58, cobertura→G3, disponibilidad→métrica 09 |
| 11 | cada TC referencia ≥1 CU, RN o NFR | OK | barrido TC-01..TC-58: todos con campo "Cubre" |
| 12 | DoD no redefinida en sprint plans | OK | mini-plan 07 §5 referencia, no redefine; DoR 06 §5 delimita |
| 13 | cobertura por capa, no número global único | OK | tabla por capa en ET §2, MX §5, CV §5; ET §1 rechaza explícitamente la pirámide aplanada |

Observación sobre el ítem 5 / política BDD: el §6 exige literalmente "≥1 TC por CU crítico" y ese mínimo se cumple. Sin embargo, la propia `estrategia-testing_v1.0.md` §4 eleva el estándar interno ("cada CA-XX de cada CU debe tener su test asociado en la matriz; un CA sin test es un gap") y la DoD §1.1 lo refuerza. Contra ese estándar autoimpuesto la matriz tiene gaps (ver §5.1, H-01). No rompe el ítem 5 (ningún CU queda sin TC), por eso se clasifica P1 y no P0.

---

## 5. Coherencia cross-doc y trazabilidad

### 5.1 CU↔Tests — cobertura de los 16 CU y de sus criterios de aceptación

Los 16 CU (CU-01..CU-16) aparecen en la tabla CU↔Tests con ≥1 TC; **ningún CU queda huérfano**. Cada CU de 02 define exactamente CA-01..CA-04. Verificación de cobertura de CA contra los archivos reales de 02:

CU con los 4 CA representados (sin gap): CU-01, CU-02, CU-03, CU-11, CU-12, CU-14, CU-15.

CU con CA sin fila ni TC en la matriz:

| CU | CA cubiertos en la matriz | CA real sin representar |
| --- | --- | --- |
| CU-04 | CA-01 (TC-13), CA-02 (TC-16), CA-03 (TC-14) | CA-04 (política en simulación → registra simulado, no contiene). La fila TC-15 del bloque cubre una excepción (tope de tiempo de regex), que **no es un CA** |
| CU-05 | CA-01 (TC-25), CA-02 (TC-22), CA-03 (TC-26) | CA-04 (incidente no accionable por jerarquía → reporte con advertencia de jerarquía/permisos) |
| CU-06 | CA-01 (TC-49), CA-02 (TC-50), CA-04 (TC-51) | CA-03 (incidente en simulación → muestra acción, no ofrece reversión) |
| CU-07 | CA-01 (TC-52), CA-02 (TC-54), CA-04 (TC-53) | CA-03 (baneo real cuyo usuario ya fue desbaneado por otra vía → informa ya no baneado y marca revertido) |
| CU-08 | CA-01 (TC-30), CA-02 (TC-31), CA-03 (TC-32) | CA-04 (first-run con confirmación que no coincide → rechaza sin crear cuenta) |
| CU-09 | CA-01 (TC-33), CA-02 (TC-34) | CA-03 (sesión vencida → invalida y exige reautenticar) y CA-04 (sin cuenta → redirige a primer ingreso, AUTH_SIN_CUENTA) |
| CU-10 | CA-01 (TC-35), CA-02 (TC-36), CA-03 (TC-37) | CA-04 (token revocado + token nuevo → cifra y reemplaza, conserva config) |
| CU-13 | CA-01 (TC-47), CA-03 (TC-48) | CA-02 (panel refleja el estado dentro del tiempo objetivo) y CA-04 (caída prolongada → mantiene estado desconectado visible y sigue reintentando) |
| CU-16 | CA-01 (TC-08), CA-02 (TC-20), CA-03 (TC-21) | CA-04 (usuario nunca accionado dispara por primera vez → ejecuta la acción y lo marca accionado) |

Total: de 64 CA (16 CU × 4) la matriz representa 53; **11 CA quedan sin fila ni TC** en 9 CU. Los textos Given/When/Then de las filas presentes coinciden razonablemente con los CA reales (sin contradicción de escenario), salvo el cruce de CU-04 (TC-15 ocupa la cuarta fila con una excepción, desplazando al CA-04 propio, cuya cobertura se asume vía CU-14). Esto contradice la política BDD de `estrategia-testing` §4 y la afirmación de cierre de la matriz §2 ("el núcleo crítico tiene los cuatro criterios cubiertos") en el caso de CU-05, que es crítico (criterios-validacion §2) y omite su CA-04.

### 5.2 NFR↔Tests — los NFR de 05 §8

Los seis NFR numéricos de `arquitectura-solucion_v1.0.md` §8 aparecen en la tabla NFR↔Tests con SLA y tooling de medición por capacidad, y los valores coinciden con el intake §17 P.10:

| NFR (05 §8) | SLA | Test/medición en matriz | Coincide |
| --- | --- | --- | --- |
| Latencia por mensaje | p95 < 200 ms | TC-55 | OK |
| Throughput sostenido | ≥ 50 msg/s | TC-56 | OK |
| Disponibilidad mensual (SLO) | 99 % mensual | métrica observada en 09 (no test) | OK |
| Memoria por conexión de gateway | ≤ 8 MB | TC-57 | OK |
| Cobertura módulo de detección | ≥ 90 % líneas; global ≥ 75/65 | Gate G3 | OK |
| Limpieza efectiva de la ráfaga | ≥ 98 % en 10 s | TC-58 | OK |

Cada NFR numérico tiene test o medición; ninguno queda sin verificación. Tooling de medición descrito por capacidad ("pipeline instrumentado", "banco de carga", "perfilado de huella"), anclado a §17 P.10.

### 5.3 RN↔Tests — las 16 RN

Las 16 RN (RN-01..RN-16) aparecen en la tabla RN↔Tests, cada una con ≥1 TC; ninguna RN huérfana. Los enunciados abreviados coinciden con el título real de cada RN en 02 (verificado RN por RN). Sin hallazgos negativos en esta tabla.

### 5.4 DoD canónica satisface las referencias forward de 06 y 07

- 06 `definition-of-ready_v1.0.md` §5 declara que la condición de terminado "vive en la categoría 08"; la DoD §1.1–§1.4 la materializa y la DoD §3 confirma "la DoR de 06 referencia esta DoD". Satisfecho.
- 07 `mini-plan_v1.0.md` §5 referencia la DoD canónica "que vive en la categoría 08 y todavía no está generada"; la DoD la provee y declara explícitamente "los planes de sprint de 07 y la DoR de 06 referencian esta DoD; ningún otro documento la redefine". La DoD **no se redefine** fuera de 08 (anti-patrón §4.10 evitado). Satisfecho.

### 5.5 Gates de cobertura coinciden con el intake §17 P.6 y quality gates con §17 P.8

- Cobertura: detección ≥ 90 % líneas; global líneas ≥ 75 %, branches ≥ 65 % — idéntico en intake §17 P.6, 05 §8, EC §3 (G3), ET §2, MX §3/§5, CV §3/§5, DoD §1.3/§1.4. Coherente en toda la cadena.
- Quality gates: G1 build, G2 tests en verde, G3 cobertura, G4 formato, G5 análisis estático sin warnings nuevos — corresponden a los gates bloqueantes del intake §17 P.8 (build, tests, cobertura, formato, análisis estático). Coherente.

---

## 6. Hallazgos enumerados

No se detectaron hallazgos P0. Hay dos hallazgos P1 (altos, no detienen la cadena pero condicionan la promoción), dos P2 y tres P3.

| # | Nivel | Archivo | Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| H-01 | P1 | matriz-cobertura-pruebas_v1.0.md | §2 CU↔Tests | 11 CA de 9 CU sin fila ni TC: CU-04 CA-04, CU-05 CA-04, CU-06 CA-03, CU-07 CA-03, CU-08 CA-04, CU-09 CA-03 y CA-04, CU-10 CA-04, CU-13 CA-02 y CA-04, CU-16 CA-04. Contradice la política BDD de `estrategia-testing` §4 ("un CA sin test es un gap") y la DoD §1.1 ("cada CA-01..CA-04 tiene ≥1 TC"). CU-05 es crítico y omite su CA-04, pese a que la matriz §2 afirma cobertura completa del núcleo. No es P0 porque ningún CU queda sin TC. | Agregar una fila y un TC por cada CA faltante (continuando la numeración a partir de TC-58 o reordenando el bloque), o documentar cada omisión en la sección Gaps (§6 de la matriz) con plan y rebanada de cierre. Reservar la frase "los cuatro criterios cubiertos" sólo para los CU donde es cierto. |
| H-02 | P1 | matriz-cobertura-pruebas_v1.0.md; casos-prueba-referenciales_v1.0.md | MX §2 vs CP catálogo | El bloque CU-04 de la matriz coloca TC-15 (retroceso catastrófico / tope de tiempo) como cuarta fila; ese escenario es una excepción del CU-04 (CONTENIDO_EVALUACION_EXCEDE_TIEMPO), no un CA, y desplaza al CA-04 real (simulación). Además, TC-17, TC-18, TC-19, TC-23 y TC-38 se usan en la tabla RN↔Tests (§4) pero no tienen fila en la tabla CU↔Tests (§2), quedando ligados a RN sin anclaje de CU/CA en la bisagra de trazabilidad. | Mapear cada fila del bloque CU-04 a su CA correspondiente y mover TC-15 a una nota de excepción o a una fila etiquetada como excepción. Incorporar TC-17/18/19/23/38 a la tabla CU↔Tests con su CA, o anotarlos como TC de soporte de RN en la sección Gaps. |
| H-03 | P2 | estrategia-calidad_v1.0.md | §2 atributo Funcionalidad | La columna "Métrica / NFR de origen" mezcla un NFR (limpieza efectiva ≥ 98 %) con una métrica de negocio ("corte automático ≥ 95 %, métrica §8 del intake") que no es un NFR de 05 §8; puede inducir a tratar una métrica de negocio como gate de calidad. | Separar la métrica de negocio (corte automático) de los NFR de arquitectura, o etiquetarla claramente como métrica de negocio no sujeta a gate, para no diluir el conjunto de NFR verificables. |
| H-04 | P2 | estrategia-testing_v1.0.md | §7 Ambiente de testing | "base relacional embebida en archivo, modo WAL" introduce "WAL", mecanismo de almacenamiento atado a la tecnología de persistencia del stack. El resto del documento evita nombrar productos del stack y describe por capacidad. WAL es técnica, no marca comercial, pero roza la política D7 de no atar el cuerpo al stack. | Sustituir "modo WAL" por la descripción por capacidad ya usada en 05 ("registro de escritura anticipada" o "modo de escritura concurrente sin bloqueo de lectores") para mantener consistencia con la política de anclar por capacidad. |
| H-05 | P3 | README.md | §"DoD canónica" / metadatos | El README lista 8 documentos en su tabla pero el README en sí es el noveno artefacto; la guia-testing-extensibilidad figura en la tabla, lo cual es correcto, pero el conteo "ocho documentos obligatorios" no se explicita frente a "+ guía". Menor claridad de inventario. | Aclarar en el README que son ocho obligatorios + la guía de extensibilidad (obligatoria por extensibilidad) + el propio README, para un inventario inequívoco. Cosmético. |
| H-06 | P3 | mini-plan_v1.0.md (referencia desde 08) | autoría | El encabezado del mini-plan de 07 declara autor "Scrum Master (AG-07)"; la categoría 07 corresponde al rol AG-07 pero la regla nombra al Scrum Master como AG-06. La DoD §… y la matriz referencian el documento por path/sección correctos, sin impacto en 08. | Observación heredada de 07; no afecta los entregables de 08. Sin acción en Fase E. |
| H-07 | P3 | casos-prueba-referenciales_v1.0.md | §"Resumen del catálogo" | Se afirma "los seis NFR numéricos tienen su TC de medición (TC-55..TC-58)"; son cuatro TC para cinco NFR medibles por test (el sexto, cobertura, es gate; disponibilidad es métrica observada). La redacción "seis NFR … TC-55..TC-58" puede confundir el conteo. | Precisar que cuatro NFR se miden con TC-55..TC-58, cobertura por el gate G3 y disponibilidad por métrica observada en 09. Estilo. |

---

## 7. Veredicto final

**APROBADO CON OBSERVACIONES.**

La Fase E (categoría 08) del proyecto discord-bots-admin cumple los 13 criterios de §6 de la regla 08, presenta los nueve documentos con su estructura obligatoria, conserva la trazabilidad a nivel CU/RN/NFR (ningún CU, RN ni NFR huérfano), respeta la nomenclatura sin sufijo de dominio, describe el tooling por capacidad sin marcas del stack, reporta cobertura por capa, materializa la DoD canónica que 06 y 07 referencian sin redefinirla, y alinea gates de cobertura y quality gates con el intake §17 P.6/P.8. No hay hallazgos P0; la cadena no se detiene.

El veredicto no es APROBADO pleno por dos hallazgos P1 de trazabilidad fina: 11 criterios de aceptación de 9 CU no están representados en la matriz CU↔Tests pese a la política BDD autoimpuesta y a la DoD que exige un TC por CA (H-01), y un mapeo cruzado en el bloque CU-04 más TC ligados a RN sin anclaje de CU en la bisagra (H-02).

Condiciones para promover a la fase 09:

- Cerrar H-01: completar la matriz y el catálogo con un TC por cada uno de los 11 CA faltantes, o registrar cada omisión en la sección Gaps de la matriz con su plan y la rebanada de cierre, y corregir la afirmación de cobertura completa del núcleo donde no aplica (CU-05).
- Cerrar H-02: alinear el bloque CU-04 a sus CA reales (reubicando TC-15 como excepción) e incorporar o anotar TC-17/18/19/23/38 respecto de su CU/CA.
- Los hallazgos H-03 y H-04 (P2) y H-05 a H-07 (P3) son mejoras de precisión y consistencia que pueden absorberse en una revisión menor sin generar versión nueva, o diferirse; no condicionan la promoción una vez cerrados los P1.

---

## 8. Anexo — Método de verificación

- Lectura completa de los nueve entregables de 08 y de la regla 08 v1.2 (§2.2, §3, §4, §4.10, §6).
- Verificación de upstream: los 16 CU (CA-01..CA-04 leídos archivo por archivo), las 16 RN (título real), 05 §8 (tabla de NFR), intake §17 P.6/P.8/P.10, DoR 06 §5 y mini-plan 07 §5.
- Reconstrucción de la cobertura de CA por CU contra la tabla CU↔Tests (53/64 CA representados; 11 gaps detallados en §5.1).
- Cotejo NFR↔Tests (6/6 con SLA y tooling) y RN↔Tests (16/16 con enunciado fiel).
- Barridos automáticos sobre los nueve archivos: patrón `.v` en nombres (0), sufijo `-motor`/marcador de dominio (0), marcas del stack y framework de test (1: "WAL", H-04), vocabulario del dominio fuente del bootstrap —impresoras térmicas, ESC-POS, Bluetooth, DSL— (0), emojis (0; solo `↔` sancionado), identificadores TC de tres dígitos (0).
- Comprobación de que la DoD cubre las cuatro capas con criterios mecánicos y no se redefine fuera de 08, y de que gates de cobertura/quality gates coinciden con el intake.
