# Auditoría de Fase G — Examples (11)

**Proyecto:** discord-bots-admin (solución: Administrador de Bots Moderador para Discord)
**Fase auditada:** G — Categoría 11 (examples / samples)
**Alcance:** `SDD2.2D/docs/11_examples/` (README.md; ejemplo-01-basico-conexion-gateway_v1.0.md; ejemplo-02-intermedio-configuracion-por-descriptores_v1.0.md; ejemplo-03-avanzado-deteccion-rafaga_v1.0.md).
**Auditor:** Independiente (Arquitecto de Soluciones + QA Senior). No participó de la generación de la Fase G.
**Fecha:** 2026-06-20
**Reglas aplicadas:** `SDD2.2D/devs/rules/11_rules_examples.md` v1.2 (§2.2 mínimos, §3 nomenclatura, §4 estructura, §6 criterios).
**Etapa:** Documentación, no codificación. `/src` y `/samples` con código ejecutable aún no existen; su materialización es posterior al handoff (`SOLUTION-INTAKE §16.1`). Se evalúa la completitud y calidad de la documentación de los samples, no la existencia de binarios ni de un CI de samples en ejecución.

---

## 1. Resumen ejecutivo

La Fase G está completa y bien construida. Existen el README de la sección y los tres markdown explicativos; los tres son samples de capacidad (conexión al gateway, configuración por descriptores, detección de ráfaga), superan el mínimo de dos de `web-monolith` (§2.2) y ninguno se nombra por una entidad del dominio fuente. Los nueve apartados §4.2 están presentes en cada markdown; la progresión 01→03 va de menor a mayor complejidad con el sample 03 (detección de ráfaga) como punto central; toda la trazabilidad a CU/ADR/NFR/componentes de 02 y 05 resuelve contra artefactos reales y vigentes. El README declara explícitamente la sustitución del default genérico §2.3 (`datos-seed`/`tema-custom`) por los tres samples de capacidad (`SOLUTION-INTAKE §16.1, §18`) y la materialización diferida de `/samples` (etapa de documentación). No se detectó vocabulario del dominio fuente del bootstrap, ni emojis, ni negritas decorativas, ni filename inválido, ni sufijo `.v`. Conteo de hallazgos: **P0 = 0; P1 = 0; P2 = 1; P3 = 2.** **Veredicto: APROBADO CON OBSERVACIONES.** La cadena puede promoverse; no hay condiciones bloqueantes.

---

## 2. Matriz de conformidad D1–D8 por documento

Documentos: RM = README.md; E1 = ejemplo-01; E2 = ejemplo-02; E3 = ejemplo-03.

| Criterio | RM | E1 | E2 | E3 |
| --- | --- | --- | --- | --- |
| D1 idioma rioplatense técnico, con tildes y eñes correctas | OK | OK | OK | OK |
| D2 filename ASCII, slug kebab lowercase, sufijo `_v1.0.md` (no `.v`); README sin sufijo | OK | OK | OK | OK |
| D3 sin emojis ni negritas decorativas (solo bold de metadatos del template §4.1) | OK | OK | OK | OK |
| D4 estructura conforme a la regla (README §4.3; markdown §4.2) | OK | OK | OK | OK |
| D5 codificación UTF-8 sin BOM | OK | OK | OK | OK |
| D6 trazabilidad upstream presente y enlazada | n/a (índice) | OK | OK | OK |
| D7 estándares namables; marcas del stack ancladas a §17, no hardcodeadas; sin vocabulario del dominio fuente | OK | OK | obs | OK |
| D8 categoría aplicable y obligatoriedad correcta (web-monolith: recomendada; intake la materializa) | OK | OK | OK | OK |

Notas D1–D8:

- **D2:** ningún archivo usa el patrón prohibido `.v1.0.md`; los cuatro usan `_v1.0.md` salvo el README, que correctamente va sin sufijo (§3.1, §3.5). Slugs en kebab-case lowercase y ASCII: `basico-conexion-gateway`, `intermedio-configuracion-por-descriptores`, `avanzado-deteccion-rafaga`. Todos combinan nivel (`basico`/`intermedio`/`avanzado`) con descriptor de capacidad, forma admitida por §3.1.
- **D3:** barrido de pictogramas/emojis: 0. Las únicas negritas son las del bloque de metadatos obligatorio de §4.1 (Proyecto, Documento, Versión, Estado, Fecha, Autor, Nivel, Ubicación del código) y los encabezados de columna del README; no hay negritas decorativas en el cuerpo.
- **D5:** los cuatro archivos comienzan con bytes `23 20…` (`# `), sin BOM UTF-8. Conforme.
- **D7:** barrido de vocabulario del dominio fuente del bootstrap (impresora/térmica/ESC-POS/Bluetooth/DSL/multa/factura/recibo/nuget): **0 ocurrencias.** El léxico de dominio legítimo del producto ("Discord", "gateway", "canal de eventos", "snowflake", "ráfaga distribuida", "token", "contexto", "descriptor") se usa correctamente. Sobre marcas del stack, ver §5.7 de este informe y H-01/H-02; la observación recae sobre E2 (`.razor`) y, como nota de consistencia, sobre los comandos `dotnet`/extensión `.cs` en los tres.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 README.md de la sección (§4.3: cinco apartados)

| Apartado §4.3 | Estado | Evidencia |
| --- | --- | --- |
| 1. Propósito de la carpeta (`docs/11_examples/` y `/samples/`) | OK | §1, más §1.1 "Estado de materialización" |
| 2. Tabla maestra de samples vigentes (§4.4: sample, nivel, tiempo de setup, CU ilustrados, ubicación) | OK | §2, cinco columnas completas |
| 3. Convenciones de los samples | OK | §3 (autocontenidos, ≤5 pasos, trazabilidad, nivel, sin credenciales, output) |
| 4. Cómo agregar un sample nuevo (ref §6 reglas + template) | OK | §4, remite a `11_rules_examples.md §4` y §6 |
| 5. Vínculo con la developer guide de 10 y la arquitectura de 05 | OK | §5; declara colapso de 10 en READMEs (ADR-11) y enlaza 05 y 09 |

Adicional correcto: §6 replica la tabla tipo D8 → `/samples` (§4.4) y §7 lleva control de cambios. El README declara el caso degenerado (layout aplanado) en §1.

### 3.2 ejemplo-01-basico-conexion-gateway_v1.0.md (§4.2: nueve secciones)

| Sección §4.2 | Estado | Evidencia |
| --- | --- | --- |
| Cabecera §4.1 (8 campos) | OK | Incluye Nivel: Básico y Ubicación `/samples/01-basico-conexion-gateway/` |
| 1. Objetivo del sample | OK | Ingreso de mensajes al pipeline; normaliza y loguea, sin evaluar reglas |
| 2. Nivel | OK | Básico, justificado como base de 02 y 03 |
| 3. Prerequisites (con versión) | OK | Runtime/SDK anclado a §17 P.1/P.9; SO de desarrollo; sin token |
| 4. Cómo correrlo (≤5 pasos) | OK | 5 pasos copiables |
| 5. Estructura del código | OK | Árbol con descripción por archivo (planificado) |
| 6. Qué esperar | OK | Output exacto de consola (log por mensaje + resumen) |
| 7. Variaciones sugeridas | OK | Tabla de 4 variaciones, incluye puente al sample 03 |
| 8. Trazabilidad | OK | CU-01 (paso previo), componente Adaptador, ADR-13, ADR-04 |
| 9. Control de cambios | OK | Tabla de versiones v1.0 |

### 3.3 ejemplo-02-intermedio-configuracion-por-descriptores_v1.0.md (§4.2: nueve secciones)

| Sección §4.2 | Estado | Evidencia |
| --- | --- | --- |
| Cabecera §4.1 (8 campos) | OK | Nivel: Intermedio; Ubicación `/samples/02-intermedio-configuracion-por-descriptores/` |
| 1. Objetivo del sample | OK | Configuración dirigida por descriptores; descriptor como fuente única |
| 2. Nivel | OK | Intermedio, justificado respecto de 01 y 03 |
| 3. Prerequisites (con versión) | OK | SDK §17 P.1/P.9; navegador evergreen §17 P.9; base embebida §17 P.4 |
| 4. Cómo correrlo (≤5 pasos) | OK | 5 pasos copiables |
| 5. Estructura del código | OK | Árbol con descripción por archivo (planificado) |
| 6. Qué esperar | OK | Tres outputs exactos: render del campo, guardado válido, rechazo fuera de límite |
| 7. Variaciones sugeridas | OK | Tabla de 4 variaciones, incluye puente a CU-15 |
| 8. Trazabilidad | OK | CU-11, CU-15, componente Registro de descriptores, ADR-12, RN-10, artefacto 03 |
| 9. Control de cambios | OK | Tabla de versiones v1.0 |

### 3.4 ejemplo-03-avanzado-deteccion-rafaga_v1.0.md (§4.2: nueve secciones)

| Sección §4.2 | Estado | Evidencia |
| --- | --- | --- |
| Cabecera §4.1 (8 campos) | OK | Nivel: Avanzado; Ubicación `/samples/03-avanzado-deteccion-rafaga/` |
| 1. Objetivo del sample | OK | Punto central: detección de ráfaga, simulación vs ejecución, contención |
| 2. Nivel | OK | Avanzado, sample de integración; reúne 01 y 02 |
| 3. Prerequisites (con versión) | OK | SDK §17 P.1/P.9; SO de desarrollo; adaptador de acciones simulado |
| 4. Cómo correrlo (≤5 pasos) | OK | 5 pasos copiables (simulación y ejecución contra adaptador simulado) |
| 5. Estructura del código | OK | Árbol con descripción por archivo (planificado) |
| 6. Qué esperar | OK | Dos outputs exactos (simulación / ejecución) + discriminador del falso positivo |
| 7. Variaciones sugeridas | OK | Tabla de 4 variaciones (umbral, ventana, jerarquía, orden de acciones) |
| 8. Trazabilidad | OK | CU-01, CU-02, CU-14, Motor, Evaluador/Estado de conducta, Ejecutor, NFR latencia, ADR-09 |
| 9. Control de cambios | OK | Tabla de versiones v1.0 |

Resultado: **9/9 secciones presentes en cada uno de los tres markdown**, con cabecera §4.1 completa en los tres (incluyendo el campo Nivel, que el anti-patrón §4.5 exige).

---

## 4. Cumplimiento de §6 de la regla 11 (13 ítems)

Los ítems de "código ejecutable real" y "pipeline CI de samples corriendo" se evalúan como **PLANIFICADOS** (declarados en el README §1.1 y en el bloque de aviso de cada markdown), conforme a la etapa de documentación.

| # | Criterio §6 | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | README con tabla maestra (nivel, tiempo de setup, CU ilustrados, ubicación) | OK | README §2, cinco columnas |
| 2 | Al menos los samples mínimos del tipo D8 (web-monolith ≥ 2) | OK | Hay 3 (intake §18); supera el mínimo |
| 3 | Cada sample con markdown `ejemplo-XX-<kebab>_v1.0.md` y las nueve secciones | OK | Verificado en §3.2–3.4 de este informe |
| 4 | Ejecutable con comandos copiables en ≤ 5 pasos | OK (planificado) | §4 de cada markdown: 5 pasos; comandos documentados como contrato |
| 5 | Cada sample declara su nivel en §2 | OK | Básico / Intermedio / Avanzado, justificados |
| 6 | Trazabilidad a CU/ADR/NFR en §8 con ≥ 1 fila | OK | E1: 4 filas; E2: 6 filas; E3: 7 filas |
| 7 | Nombres por nivel o capacidad, nunca por dominio del proyecto | OK | Slugs de capacidad; 0 entidades del dominio fuente |
| 8 | Todos los markdown con sufijo `_v<X.Y>.md` | OK | `_v1.0.md` en los tres; README sin sufijo (correcto) |
| 9 | README lista los samples con todas las columnas de §4.4 | OK | README §2 |
| 10 | Cada sample declara tiempo de setup en la tabla maestra | OK | < 5 min / 10-15 min / 15-25 min |
| 11 | Output esperado documentado en §6 con texto exacto o screenshot | OK | Bloques de consola exactos en los tres |
| 12 | Prerequisites con versiones mínimas en §3 | OK | .NET 10 anclado a §17 P.1/P.9; navegadores con versión §17 P.9 |
| 13 | Estructura `/samples` coincide con la matriz §2.3 (ajustada al intake) | OK (planificado) | README §6: 3 carpetas 1:1 con los markdown; sustitución del default declarada |
| (sub) | Existe código ejecutable real en `/samples` | **PLANIFICADO** | Declarado diferido al post-handoff (README §1.1) — no es hallazgo en esta fase |
| (sub) | Existe pipeline CI que compila/ejecuta los samples | **PLANIFICADO** | Declarado diferido al post-handoff (README §1.1) — no es hallazgo en esta fase |

---

## 5. Coherencia cross-doc y trazabilidad

### 5.1 Cada sample ilustra CU reales de 02

- **E1 → CU-01 (paso previo):** declara materializar pasos 1 y 3 del flujo principal de CU-01 (entrega del mensaje por el canal de eventos y su normalización), sin evaluar la regla. Verificado contra `CU-01 §4` (paso 1: "La plataforma de mensajería entrega un mensaje al servicio de moderación a través del canal de eventos"). El encuadre como "upstream de CU-01" en lugar de CU completo es honesto y correcto para un sample que sólo loguea.
- **E2 → CU-11 y CU-15:** CU-11 (administrar reglas/parámetros, paso 3: default, leyenda, ejemplos y validación por descriptor) y CU-15 (exenciones, materializado como variación de §7, ligado a RN-07). Ambos CU existen y vigentes.
- **E3 → CU-01, CU-02, CU-14:** detección (CU-01), baneo con borrado retroactivo y antirrebote (CU-02), modo simulación (CU-14). Los tres existen y son coherentes con el escenario del sample.

Todos los enlaces de §8 resuelven a archivos existentes en `02_especificacion_funcional/casos-de-uso/` y `reglas-de-negocio/`.

### 5.2 Cada sample ejercita componentes/ADR reales de 05

Verificado contra `arquitectura-solucion_v1.0.md §3` (catálogo de componentes):

- E1: "Adaptador del gateway y de la API de la plataforma" (Infraestructura) — existe; ADR-13 y ADR-04 — existen, títulos coinciden.
- E2: "Registro de descriptores de parámetro" (Dominio) y "Servicio de configuración dirigida por descriptores" (Aplicación) — ambos existen; ADR-12 ("Configuración dirigida por esquema") — existe, título coincide.
- E3: "Motor de moderación (pipeline)", "Evaluador de reglas de conducta", "Estado de conducta en memoria", "Ejecutor de acciones" — los cuatro existen con esos nombres; ADR-09 — existe. El NFR de latencia p95 < 200 ms está anclado a `SOLUTION-INTAKE §17 P.10` y coincide con `05 §8` (fila "Latencia de procesamiento por mensaje, p95 < 200 ms").

No se detectaron referencias a artefactos inexistentes ni a versiones jubiladas.

### 5.3 Progresión 01→03 de menor a mayor complejidad; el 03 como punto central

La progresión es consistente y declarada: 01 sólo recibe/normaliza/loguea (sin reglas ni acciones); 02 agrega host web, página interactiva, capa de configuración y persistencia, sin tocar el motor; 03 integra todo (detección con estado, política por prioridad, ejecución de acciones, simulación vs real). El README §2 y el §2 de cada markdown justifican el salto respecto del anterior. El sample 03 se identifica explícitamente como "el punto central de la solución" / "el valor diferencial", coherente con intake §18 que designa el sample (b) como demostración del núcleo. Conforme a §3.2 (reglas de progresión).

### 5.4 Sustitución del default §2.3 declarada

El README §6 (más su nota) declara expresamente que el default genérico de `web-monolith` (`01-datos-seed/`, `02-tema-custom/`) se reemplaza por los tres samples de capacidad porque el intake materializa `/samples` con ellos (`SOLUTION-INTAKE §16.1, §18`), y deja constancia de que los tres superan el mínimo de dos y nombran capacidades, no entidades del dominio. La sustitución es legítima (§2.2 permite agregar samples; §16.1/§18 del intake la fijan) y no falta silenciosamente.

### 5.5 Materialización diferida de `/samples` declarada

El README §1.1 ("Estado de materialización") y el bloque de aviso al inicio de cada markdown declaran que la etapa es de documentación: los markdown son la especificación vigente; la estructura `/samples/XX/`, el código, los tests y el job de CI se materializan post-handoff (`SOLUTION-INTAKE §16.1`). Por tanto, la ausencia de código en `/samples` y de un CI de samples corriendo no constituye hallazgo en esta fase.

### 5.6 Colapso de 10 (ADR-11)

El README §5 declara que la developer guide (categoría 10) está colapsada en READMEs por ADR-11 y que los samples referencian 05 y 09 en lugar de una guía independiente. Verificado: ADR-11 existe. Que los samples apunten a 05/09 es correcto en este proyecto.

### 5.7 Anclaje de marcas del stack a §17

Barrido del cuerpo de los cuatro archivos: **no aparecen** "Blazor", "MudBlazor", "SQLite", "Discord.Net", "EF Core" ni "Entity Framework". El corpus describe el stack por capacidad ("canal de eventos", "host web", "página interactiva del lado servidor", "base embebida", "adaptador de acciones simulado"), lo cual es el comportamiento esperado. Las únicas marcas presentes:

- `.NET 10` (los tres markdown, §3 Prerequisites): aparece **anclado** en la misma oración a `SOLUTION-INTAKE §17 P.1` y `§17 P.9`. Runtime/lenguaje anclado a §17 = stack legítimo, no hardcodeo (criterio del audit y precedente de la Fase F, que graduó `.NET`/`dotnet` anclados como conformes).
- `dotnet run` (E1, E2, E3, §4) y extensión `.cs` (árboles de §5): el comando del runtime y la extensión del lenguaje. La regla §4.2 exige comandos copiables y árbol de archivos concretos; el runtime está declarado en §17 P.1. Es stack legítimo trazable; única tensión: el anclaje a §17 no se repite junto al comando/árbol (nota de consistencia, H-02, P3).
- `.razor` (sólo E2, §5, archivo `Paginas/Configuracion.razor`): extensión específica del framework de UI. El cuerpo de E2 evita en todo momento nombrar el framework ("página interactiva del lado servidor"), por lo que la extensión `.razor` filtra la marca sin anclaje, en contraste con el cuidado del resto del documento. Es una inconsistencia menor de anclaje, no un hardcodeo de producto no declarado (el framework está en §17 P.1). H-01, P2.

Ninguna de estas alcanza el umbral P0 (marca de un producto comercial no declarado, hardcodeada sin anclaje), calibrado contra el precedente de la Fase F donde incluso `SQLite`/`AES` en el cuerpo, anclados por ADR, se graduaron P3.

---

## 6. Hallazgos enumerados

| ID | Nivel | Archivo | Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| H-01 | P2 | ejemplo-02-intermedio-configuracion-por-descriptores_v1.0.md | §5 (estructura del código) | El árbol nombra `Paginas/Configuracion.razor`. La extensión `.razor` es una marca del framework de UI del stack; el resto del cuerpo de E2 describe la UI por capacidad ("página interactiva del lado servidor") sin nombrar el framework, de modo que la extensión filtra la marca sin anclaje a §17. El framework SÍ está declarado en `SOLUTION-INTAKE §17 P.1`, por lo que es inconsistencia de anclaje, no hardcodeo de producto no declarado; por eso P2 y no P0/P1. | Anclar la extensión a §17 P.1 junto al árbol (p. ej. nota "extensiones del framework de UI declarado en §17 P.1") o usar un descriptor neutro de archivo de página coherente con la convención `<ext>` del template §7 de la regla, manteniendo el estilo del resto de E2. |
| H-02 | P3 | ejemplo-01, ejemplo-02, ejemplo-03 | §4 (cómo correrlo) y §5 (árboles) | `dotnet run` y la extensión `.cs` nombran el comando del runtime y el lenguaje en el cuerpo. Son stack legítimo trazable a §17 P.1 (la regla §4.2 exige comandos copiables y árbol concreto), pero el anclaje a §17 no se repite junto al comando/árbol como sí se hace en §3 Prerequisites. Consistencia, sin acción obligatoria. | Opcional: agregar una nota de anclaje "runtime declarado en §17 P.1" junto a los bloques de comandos, para alinear con la política de anclaje del corpus. Sin impacto en trazabilidad. |
| H-03 | P3 | README.md | §2 (tabla maestra) / E1 §8 | El sample 01 ilustra "Ingreso de mensajes al pipeline (upstream de CU-01)" en lugar de un CU completo; es honesto y correcto (el sample sólo loguea), pero la columna "CU ilustrados" de la tabla maestra queda con una etiqueta descriptiva en vez de un identificador CU para E1, a diferencia de E2/E3. | Opcional: mantener la etiqueta actual (es exacta) o añadir entre paréntesis el identificador formal "(precondición de CU-01)" para uniformar el formato de la columna. Sin impacto en conformidad. |

No se identificaron hallazgos P0 ni P1.

---

## 7. Veredicto final

**APROBADO CON OBSERVACIONES.**

La Fase G (categoría 11) del proyecto discord-bots-admin cumple los 13 criterios de §6 de la regla 11 (con los dos ítems de código y CI evaluados como planificados, conforme a la etapa de documentación), presenta el README de la sección con sus cinco apartados §4.3 y los tres markdown explicativos con las nueve secciones §4.2 y la cabecera §4.1 completa. Supera el mínimo de dos samples de `web-monolith` con tres samples de capacidad, ninguno nombrado por una entidad del dominio fuente (prohibición crítica satisfecha). La progresión 01→03 escala correctamente de menor a mayor complejidad con el sample 03 (detección de ráfaga) como punto central. Toda la trazabilidad a CU/ADR/NFR/componentes de 02 y 05 resuelve contra artefactos reales y vigentes. El README declara explícitamente la sustitución del default §2.3 por los tres samples de capacidad (`SOLUTION-INTAKE §16.1, §18`) y la materialización diferida de `/samples` al post-handoff. No se detectó vocabulario del dominio fuente del bootstrap, ni emojis, ni negritas decorativas, ni filename inválido, ni sufijo `.v`, ni codificación con BOM. Las marcas pesadas del stack (Blazor, MudBlazor, SQLite, Discord.Net, EF Core) están ausentes del cuerpo, descritas por capacidad; las únicas marcas presentes (`.NET 10` anclado a §17, `dotnet run`, extensiones `.cs`/`.razor`) son stack legítimo trazable a §17 P.1, con una única tensión menor de anclaje en la extensión `.razor` de E2.

Conteo: **P0 = 0; P1 = 0; P2 = 1; P3 = 2.**

### Condiciones para promover

- No hay condiciones bloqueantes. La cadena puede promoverse tal como está.
- Recomendado (no bloqueante) antes o durante la fase de codificación: resolver H-01 (anclar/neutralizar la extensión `.razor` en E2) para mantener consistencia de anclaje; opcionalmente H-02 y H-03.

---

## 8. Anexo — Método de verificación

- Lectura completa de los cuatro entregables de `SDD2.2D/docs/11_examples/` y de la regla `11_rules_examples.md` v1.2 (§2.2, §3, §4, §6).
- Verificación de existencia de todos los artefactos upstream enlazados: 16 CU en `02/casos-de-uso/`, 16 RN en `02/reglas-de-negocio/`, 13 ADR en `05/adrs/`, componentes en `05/arquitectura-solucion_v1.0.md §3`, NFR de latencia en `05 §8` y `SOLUTION-INTAKE §17 P.10`, artefacto UX referenciado en `03`.
- Confrontación de claims puntuales contra el texto fuente: pasos 1 y 3 del flujo principal de CU-01; títulos de ADR-12 y ADR-13; nombres exactos de componentes; cita de `design-rules-config-esquema` en `03/README §2`; sección §1.1 de `05/extensibilidad`.
- Lectura de `SOLUTION-INTAKE §16.1, §17 (P.1, P.4, P.9, P.10, P.11) y §18` para validar la sustitución del default, el anclaje del stack y la estrategia de samples.
- Barridos automáticos: BOM/encoding (primeros bytes), vocabulario del dominio fuente, marcas del stack, emojis (pictogramas Unicode), negritas decorativas, patrón de filename `.v` vs `_v1.0`.

---

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Auditoría independiente inicial de la Fase G (categoría 11) del proyecto discord-bots-admin. Veredicto: APROBADO CON OBSERVACIONES (P0=0, P1=0, P2=1, P3=2). |
