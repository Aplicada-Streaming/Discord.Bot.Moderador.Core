# Auditoría Fase C — Arquitectura técnica (categoría 05)

**Fase auditada:** C — Diseño técnico (categoría 05 Arquitectura técnica)
**Proyecto:** discord-bots-admin (Administrador de Bots Moderador para Discord)
**Alcance:** `SDD2.2D/docs/05_arquitectura_tecnica/` completo (documento maestro, índice de ADR, 13 ADR individuales, modelo de datos lógico, flujo de ejecución, extensibilidad, README) más verificación de ausencia de `_solucion/` y de `contratos-<area>`.
**Tipo de proyecto (D8):** web-monolith. Caso degenerado (un único proyecto, layout aplanado).
**Auditor:** Arquitecto de Soluciones + QA Senior, independiente (no participó de la generación de la Fase C).
**Fecha:** 2026-06-20
**Reglas aplicadas:** `devs/rules/05_rules_arquitectura_tecnica.md` v1.2 (§2.2, §3.3, §3.6, §4, §6).

---

## 1. Resumen ejecutivo

La categoría 05 está completa y conforme. Los 19 artefactos (maestro, índice, 13 ADR individuales, modelo lógico, flujo, extensibilidad, README) existen, cumplen las cabeceras y secciones obligatorias, respetan la convención crítica de ADR individuales con estado declarado e inmutabilidad, y no introducen marcas comerciales del stack ni vocabulario del dominio fuente en el cuerpo (el stack se ancla por referencia a `SOLUTION-INTAKE §17`). El modelo lógico nace del conceptual de 02 entidad por entidad (13↔13), los 16 CU quedan cubiertos, las 16 RN referenciadas, y los tres gating esperados (compliance Ley 25.326, omisión de 04, colapso de 10) están presentes como ADR. No existe `_solucion/` ni `contratos-<area>`; ambas ausencias están declaradas como decisión. Conteo de hallazgos: **P0: 0 — P1: 0 — P2: 0 — P3: 4**. Veredicto: **APROBADO**.

---

## 2. Matriz D1–D8 por documento

Familias agrupadas: M=documento maestro; I=índice ADR; ML=modelo lógico; FE=flujo de ejecución; EX=extensibilidad; RM=README; ADR=los 13 ADR (verificados uno por uno para estado y estructura, ver §3 y §4).

| Documento | D1 idioma rioplatense c/ tildes | D2 sin marcas/ jerga fuente (D7 stack) | D3 filename ASCII + slug kebab + `_v1.0` | D4 UTF-8 / sin BOM | D5 EOL LF | D6 trazabilidad | D8 sin emojis / sin negrita decorativa |
| --- | --- | --- | --- | --- | --- | --- | --- |
| M `arquitectura-solucion_v1.0.md` | OK | OK | OK | OK (UTF-8, sin BOM) | CRLF (P3-01) | OK | OK |
| I `decisiones-arquitectura_v1.0.md` | OK | OK | OK | OK | CRLF (P3-01) | OK | OK |
| ML `modelo-datos-logico_v1.0.md` | OK | OK | OK | OK | CRLF (P3-01) | OK | OK |
| FE `flujo-ejecucion_v1.0.md` | OK | OK | OK | OK | CRLF (P3-01) | OK | OK |
| EX `extensibilidad_v1.0.md` | OK | OK | OK | OK | CRLF (P3-01) | OK | OK |
| RM `README.md` | OK | OK | OK (sin sufijo de versión por ser README) | OK | CRLF (P3-01) | OK | OK |
| ADR-01..ADR-13 (13 archivos) | OK | OK | OK (regex `^ADR-\d{2}-[a-z0-9-]+_v\d+\.\d+\.md$` verificado en los 13) | OK | CRLF (P3-01) | OK | OK |

Notas D2/D7: el cuerpo no nombra ningún producto comercial del stack prohibido (Blazor, MudBlazor, Discord.Net, EF Core, SQLite, .NET, GitHub, Docker, Raspberry/Raspbian, GitVersion, Nerdbank, NuGet) ni jerga del dominio fuente (impresoras térmicas, ESC-POS, Bluetooth, DSL). El stack se describe por capacidad/rol y se ancla a `SOLUTION-INTAKE §17 P.1` (declarado en M §1). Términos legítimos presentes y correctos: Discord, gateway, snowflake, token de bot, WAL, AES, Argon2/PBKDF2/PHC, systemd, ARM/armv7l, self-contained, x64, AAIP, SemVer. Ninguno es marca comercial prohibida.

Nota D3: los filenames son ASCII, slug kebab lowercase, sufijo `_v1.0` con guion bajo. No se halló el patrón prohibido `.v`. El README no lleva sufijo de versión por convención (correcto).

---

## 3. Matriz de estructura obligatoria por documento

| Documento | Cabecera (H1 + metadatos §4.1) | Secciones obligatorias | Resultado |
| --- | --- | --- | --- |
| M arquitectura-solucion | OK | §1 Objetivo, §2 Estilo (c/ alternativas), §3 Vista lógica, §4 Vista de procesos, §5 Vista de despliegue, §6 Vista de datos, §7 Cross-cutting, §8 NFR, §9 Riesgos, §10 Trazabilidad (las 10 de §4.2 + control de cambios) | Completo |
| I decisiones-arquitectura | OK | Propósito, tabla índice (ADR, título, categoría, estado, fecha), notas de estado, control de cambios | Completo (índice, sin cuerpo de decisiones) |
| ML modelo-datos-logico | OK | §1 Tablas (c/ entidad de origen), §2 Atributos c/ tipo físico, §3 Índices, §4 Restricciones, §5 Migración inicial, §6 Estrategia multi-tenant, §7 Trazabilidad (las 7 de §4.4) | Completo |
| FE flujo-ejecucion | OK | Entrada, 9 etapas con transformaciones, diagrama, concurrencia/estado, trazabilidad | Completo (pipeline paso a paso) |
| EX extensibilidad | OK | Superficie de extensión, frontera reservada, no-extensible, ejemplo en 11, trazabilidad | Completo |
| RM README | OK | Índice del maestro, ML, flujo, extensibilidad, ADR, NFR, decisión de contratos, decisión de vista de solución | Completo (recomendado) |
| ADR-01..13 | OK (los 13) | Las 10 de §4.3: Contexto, Decisión, Estado, Alternativas, Consecuencias positivas, Consecuencias negativas/trade-offs, Implementación, Métricas de validación, Referencias, Control de cambios | Completo en los 13 |

Verificación ADR uno por uno (estado y set de 10 secciones):

| ADR | Categoría | Estado declarado | 10 secciones §4.3 | Motivación (NB/CU/RN/NFR) |
| --- | --- | --- | --- | --- |
| ADR-01 estilo monolítico capas+pipeline | Estilo | Aceptado | Sí | NB-01; CU-01/02/04/14/16; RN-04/05; NFR latencia/throughput/memoria |
| ADR-02 persistencia relacional embebida WAL | Persistencia | Aceptado | Sí | CU-05/06/10/11/15; RN-08/11; RC-01..11 |
| ADR-03 autenticación admin único hash robusto | Seguridad | Aceptado | Sí | NB-05; CU-08/09; RN-12/13; RC-06 |
| ADR-04 separación de capas dominio independiente | Estilo | Aceptado | Sí | NB-01; CU-01/02/04/14/16; RN-04/05/07/09; NFR cobertura |
| ADR-05 despliegue self-contained ARM + servicio | Despliegue | Aceptado | Sí | CU-13; NFR disponibilidad; restricciones de plataforma 00 |
| ADR-06 compliance Ley 25.326 | Seguridad | Aceptado | Sí | RN-11/13/14; CU-05/06; alcance 00 |
| ADR-07 cifrado tokens en reposo clave maestra | Seguridad | Aceptado | Sí | NB-05/06; CU-10/12/13; RN-14; RC-07 |
| ADR-08 manejo de errores pipeline / resultados | Estilo | Aceptado | Sí | CU-02/03/04/05/07/10/11/15; RN-01/03/10/11 |
| ADR-09 estado conducta y antirrebote en memoria | Persistencia | Aceptado | Sí | CU-01/16; RN-06/10; NFR latencia/throughput/memoria |
| ADR-10 omisión contratos de prompts AI | Estilo | Aceptado | Sí | Alcance excluido 00/01/02; intake §4 Won't Have, §17 P.11 |
| ADR-11 colapso developer guide en READMEs | Estilo | Aceptado | Sí | intake §17 P.3/P.7 (sin superficie pública ni redistribuibles) |
| ADR-12 configuración dirigida por esquema | Extensibilidad | Aceptado | Sí | NB-05; CU-01/11/16; RN-04/09/10 |
| ADR-13 dominio firewall multi-contexto | Estilo | Aceptado | Sí | NB-01; CU-01/10/13; RN-08/14; RC-01/02 |

No hay ADR sin estado, ni ADR consolidado, ni ADR huérfano de motivación. ADR-06 y ADR-10/11 añaden un campo de cabecera extra (`Compliance:` / `Bandera:`) coherente con su naturaleza; no es defecto.

---

## 4. Convención crítica §3.3 e inmutabilidad §3.6

| Criterio | Resultado |
| --- | --- |
| Cada ADR vive en un archivo individual bajo `adrs/` (no consolidado) | Cumple. 13 archivos individuales en `adrs/`; `decisiones-arquitectura_v1.0.md` es solo índice (no contiene cuerpo de decisiones). |
| Cada ADR tiene estado declarado | Cumple. Los 13 declaran `Estado: Aceptado` en cabecera y en su §3. |
| Inmutabilidad: ningún ADR aceptado editado para reflejar otra decisión | Cumple. Todos en v1.0, control de cambios con única entrada "Decisión inicial" y la nota de que la única edición permitida es el cambio de estado a `Superado por ADR-YY`. No hay ADR superado ni reescrito. |
| Mínimo por tipo (web-monolith: 5 — estilo, persistencia, autenticación, separación de capas, manejo de errores) | Cumple y supera: estilo (ADR-01), persistencia (ADR-02), autenticación (ADR-03), separación de capas (ADR-04), manejo de errores (ADR-08); total 13 (piso de 5 superado). |

---

## 5. Cumplimiento de §6 (12 ítems)

| # | Criterio §6 | Resultado | Evidencia |
| --- | --- | --- | --- |
| 1 | `arquitectura-solucion_v1.0.md` con 4 vistas mínimas y §1–§10 | Cumple | M §3 lógica, §4 procesos, §5 despliegue, §6 datos; §1–§10 presentes |
| 2 | `decisiones-arquitectura_v1.0.md` indexa ADR con estado y fecha | Cumple | I §2 tabla con ADR/título/categoría/estado/fecha (2026-06-20) |
| 3 | ≥3 ADR individuales con las 10 secciones §4.3 | Cumple | 13 ADR, 10 secciones cada uno (ver §3) |
| 4 | Cada ADR con estado declarado | Cumple | Los 13 en estado Aceptado |
| 5 | Modelo lógico con migración inicial referenciada y trazabilidad al conceptual de 02 | Cumple | ML §5 `MIG-0001-esquema-inicial`; ML §1 y §7 trazan 13 tablas a las 13 entidades de 02 |
| 6 | Contratos externos por área si el tipo los exige | No aplica / cumple | web-monolith sin API externa; ausencia declarada en README "Decisión sobre contratos externos" (intake §17 P.3) |
| 7 | Estilo justificado contra ≥2 alternativas | Cumple | M §2.1: 3 alternativas descartadas (microservicios, procesos separados, capas planas); ADR-01 §4: 3 alternativas |
| 8 | Cada NFR con objetivo numérico y mecanismo de medición | Cumple | M §8: 6 NFR, cada uno con valor numérico (p95<200ms, ≥50 msg/s, 99%, ≤8 MB, ≥90%, ≥98%) y mecanismo de medición |
| 9 | Trazabilidad NFR↔arquitectura↔ADR en una tabla del maestro | Cumple | M §8 columna "ADR relacionada"; M §10 cruza CU/RN/ADR |
| 10 | Ningún archivo con patrón `.v<X.Y>.md` | Cumple | Todos usan `_v1.0`; no se halló `.v` |
| 11 | Ningún ADR consolidado en otro documento | Cumple | 13 archivos individuales |
| 12 | Sin menciones a stacks concretos/productos comerciales/protocolos del dominio fuente | Cumple | Scan negativo (ver §2 D2/D7) |
| (+) | README de sección presente (recomendado) | Cumple | `README.md` presente y completo |

---

## 6. Coherencia cross-doc y trazabilidad

| Verificación | Resultado | Evidencia |
| --- | --- | --- |
| Cada ADR referencia NB/CU/RN/NFR que la motiva (sin ADR huérfana) | Cumple | Ver tabla §3; los 13 declaran §9 Referencias con motivación |
| Cada componente de la vista lógica declara los CU que cubre | Cumple | M §3 columna "CU cubiertos" en los 16 componentes |
| Cobertura de los 16 CU (sin CU sin componente) | Cumple | CU-01..CU-16 cubiertos en M §3 y mapeados en M §10; verificado uno a uno |
| Modelo lógico referencia las 13 entidades conceptuales de 02 (entidad por entidad) | Cumple | ML §1 (1.1–1.13) y ML §7 trazan a las 13 entidades del conceptual `modelo-conceptual_v1.0.md` (1.1–1.13); 13↔13 exacto |
| Tipos físicos presentes; sin tipos faltantes | Cumple | ML §2: tipos abstractos (TEXTO/ENTERO/BOOLEANO/MARCA_DE_TIEMPO), nulabilidad, default por atributo; índices §3; restricciones §4 |
| No multi-tenant declarado correctamente | Cumple | ML §6 `multi_tenant = false`, sin columna discriminadora; aislamiento por `servidor_id`; coherente con intake §17 P.4 y ADR-13 |
| Snowflakes como texto / token cifrado | Cumple | ML §2 snowflakes TEXTO (RC-02), token_cifrado TEXTO (RC-07, ADR-07) |
| Flujo de ejecución refleja el pipeline de moderación de 02/intake §6 | Cumple | FE: 9 etapas (exentos, contenido, conducta en memoria, políticas por prioridad, copia de mensajes, modo, antirrebote, ejecución, registro) derivadas de `SOLUTION-INTAKE §6` |
| Extensibilidad documenta la frontera reservada PropuestaDeConfiguracion | Cumple | EX §2: frontera reservada `PropuestaDeConfiguracion` (no construida en v1, ADR-10, usa_llm=false) |
| Índice de ADR coincide con archivos reales en `adrs/` | Cumple | 13 archivos en disco = 13 filas en `decisiones` = 13 filas en README; identificadores y títulos coinciden |
| Gating: ADR de compliance (Ley 25.326) | Presente (correcto) | ADR-06 |
| Gating: ADR de omisión de 04 (usa_llm=false) | Presente (correcto) | ADR-10 |
| Gating: ADR de colapso de 10 (tiene_portal_developers=false) | Presente (correcto) | ADR-11 |
| Ausencia de `_solucion/` (caso degenerado de un único proyecto) | Correcto | No existe la carpeta; declarado en README y en I "Decisión sobre la vista de solución" |
| Ausencia de `contratos-<area>` (integra como cliente, no expone API) | Correcto | No existe; declarado como decisión en README (intake §17 P.3) |

---

## 7. Hallazgos enumerados

No se registran hallazgos P0, P1 ni P2. Se registran cuatro observaciones P3 (mejora, no bloqueantes).

| ID | Nivel | Archivo | Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| P3-01 | P3 | Todos los archivos de 05 | EOL | Terminadores de línea CRLF en los 19 archivos. D5 pide EOL LF. | Normalizar a LF. No bloquea: es convención de todo el repositorio (también 00, 02, el archivo de reglas y los audits A y B usan CRLF en este entorno Windows); no es un defecto introducido por la Fase C. Considerar un `.gitattributes` con `* text eol=lf` a nivel solución. |
| P3-02 | P3 | `arquitectura-solucion_v1.0.md` | §8 NFR, fila "Memoria por conexión" | El gobierno de la fila se atribuye a `ADR-08`, pero el mecanismo descrito (una conexión por servidor del firewall multi-contexto) corresponde a ADR-13 (y ADR-09 para el presupuesto por contexto). | Reapuntar la columna ADR de esa fila a ADR-13 (y, si se desea, ADR-09) para que la trazabilidad NFR↔ADR sea precisa. |
| P3-03 | P3 | `arquitectura-solucion_v1.0.md` | §8 NFR, fila "Limpieza efectiva de la ráfaga" | La fila atribuye el gobierno a `ADR-02` (persistencia). La métrica de negocio de limpieza depende más del pipeline de acciones (ADR-08) y del estado en memoria (ADR-09) que de la persistencia. | Revisar la atribución de ADR de esa fila para reflejar el componente que realmente la gobierna. Es una imprecisión de referencia, no afecta la cobertura. |
| P3-04 | P3 | `arquitectura-solucion_v1.0.md` | §3 vista lógica vs §10 | "Servicio de cifrado de tokens" se ubica en Infraestructura en §3 pero declara cubrir CU-12 que en §10 se gobierna principalmente por "Prueba de configuración"; la atribución de CU al servicio de cifrado es laxa (cifra/descifra, no "cubre" CU-12 por sí solo). | Afinar la columna "CU cubiertos" del componente de cifrado para distinguir CU que materializa de CU en los que solo participa. Cosmético; la cobertura global de los 16 CU se mantiene. |

---

## 8. Veredicto final y condiciones de promoción

**Veredicto: APROBADO.**

La categoría 05 del proyecto discord-bots-admin cumple las reglas constructivas v1.2 sin hallazgos bloqueantes (P0) ni altos (P1). Documento maestro con las cuatro vistas y las diez secciones; índice de ADR consistente con los 13 archivos individuales reales; los 13 ADR con las diez secciones, estado declarado e inmutabilidad respetada; modelo lógico que nace del conceptual de 02 entidad por entidad (13↔13) con migración inicial referenciada y declaración no multi-tenant; flujo de ejecución que refleja el pipeline de moderación; extensibilidad con la frontera reservada `PropuestaDeConfiguracion`; NFR con objetivos numéricos y mecanismos de medición; stack anclado por referencia a `SOLUTION-INTAKE §17` sin marcas comerciales ni jerga del dominio fuente en el cuerpo. Los tres gating esperados (compliance Ley 25.326, omisión de 04, colapso de 10) están presentes. No existe `_solucion/` ni `contratos-<area>`, y ambas ausencias están declaradas como decisión.

**Condiciones para promover a la fase siguiente:** ninguna condición bloqueante. Se recomienda atender las cuatro observaciones P3 como mejora de prolijidad antes del cierre de v1 (preferentemente la normalización de EOL a LF a nivel de todo el repositorio vía `.gitattributes`, P3-01, por ser transversal a todas las categorías). La cadena de trazabilidad D6 puede continuar hacia 06.

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Informe inicial de auditoría independiente de la Fase C (categoría 05) del proyecto discord-bots-admin. Veredicto APROBADO; 0 P0, 0 P1, 0 P2, 4 P3. |
