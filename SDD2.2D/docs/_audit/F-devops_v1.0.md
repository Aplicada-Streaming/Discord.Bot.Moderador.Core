# Auditoría de Fase F — DevOps (09)

**Fase auditada:** F (categoría 09 devops)
**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Layout:** caso degenerado (09 directo bajo `SDD2.2D/docs/`, sin `proyectos/<kebab>/` ni `_solucion/`)
**Alcance:** `SDD2.2D/docs/09_devops/` completa: pipeline-ci-cd, estrategia-versionado, entornos-deploy, guia-publicacion-paquete-self-contained-linux-arm, supply-chain-seguridad y README. La categoría 10 (developer_guide) está omitida/colapsada por gating (web-monolith opcional + `tiene_portal_developers = false`), decisión registrada como ADR-11; su ausencia con contenido es correcta y no constituye hallazgo.
**Auditor:** Arquitecto de Soluciones + QA Senior, independiente (no participó de la generación de la Fase F)
**Fecha:** 2026-06-20
**Reglas de referencia:** `09_rules_devops.md` v1.3 (§6 criterios de aceptación; §2.2 modelo de ambientes y tipo de artefacto por tipo D8; §3.1 nombre parametrizado de la guía de publicación; §4 estructura; §4.8 anti-patrones)
**Insumos upstream:** 05 (`arquitectura-solucion_v1.0.md` §8 tabla de NFR, ADR-05 despliegue self-contained ARM systemd, ADR-06 compliance, ADR-07 cifrado, ADR-11 colapso de 10), 08 (`definition-of-done_v1.0.md` DoD canónica, `estrategia-calidad_v1.0.md` §3 gates G1–G5, cobertura por capa), intake (`SOLUTION-INTAKE-discord-bots-admin_v1.0.md` §10 restricciones de despliegue, §17 P.6/P.7/P.8/P.9/P.10/P.11/P.12)

---

## 1. Resumen ejecutivo

La Fase F del proyecto discord-bots-admin está completa y sólidamente construida: existen los cinco documentos obligatorios más el README; el pipeline declara doce stages contiguos (lint, build, análisis estático, test por nivel, cobertura, SCA, SBOM, firma, package, publish) con matriz SO/runtime, caché, promotion rules, rollback con comandos concretos y notificaciones, ejecutando los gates G1–G5 de 08 sin redefinirlos; el versionado cubre SemVer 2.0.0, Conventional Commits, herramienta de auto-versioning anclada a §17 P.7, branching, canales y deprecation; los entornos declaran el modelo reducido justificado; supply-chain cubre SBOM/firma/SLSA/SCA/SAST-DAST/CVE. Las dos divergencias esperadas (artefacto paquete self-contained `linux-arm` en lugar de `image-docker`; modelo de ambientes reducido en lugar de la escalera cloud) están justificadas explícitamente contra el default §2.2 con referencia a intake §10 y ADR-05, no faltan silenciosamente. ADR-11 cubre la omisión de 10. El nombre de la guía usa el patrón parametrizado familia `paquete-` sin hardcodear gestor. No hay `_solucion/` (correcto en caso degenerado). El único punto de tensión es de estilo de anclaje: los comandos `gh release` de la guía y del rollback nombran la CLI de la plataforma de hosting en el cuerpo, mientras el resto del corpus la ancla por capacidad a §17 P.8. Conteo de hallazgos: P0 = 0; P1 = 0; P2 = 2; P3 = 3. **Veredicto: APROBADO CON OBSERVACIONES.** La cadena puede promoverse; no hay condiciones bloqueantes.

---

## 2. Matriz de conformidad D1–D8 por documento

Leyenda: OK = conforme; obs = observación menor (ver hallazgos). Documentos: PL = pipeline-ci-cd; VR = estrategia-versionado; EN = entornos-deploy; GP = guia-publicacion-paquete-self-contained-linux-arm; SC = supply-chain-seguridad; RD = README.

| Criterio | PL | VR | EN | GP | SC | RD |
| --- | --- | --- | --- | --- | --- | --- |
| D1 idioma rioplatense con tildes/eñes | OK | OK | OK | OK | OK | OK |
| D2 filename ASCII, kebab lowercase, sufijo `_v1.0` con guion bajo (no `.v`) | OK | OK | OK | OK | OK | OK |
| D3 identificadores STAGE-XX de dos dígitos contiguos | OK | n/a | n/a | OK | OK | n/a |
| D4 codificación UTF-8 | OK | OK | OK | OK | OK | OK |
| D5 sin emojis ni negritas decorativas | OK | OK | OK | OK | OK | OK |
| D6 trazabilidad upstream (DoD/NFR/ADR/intake) | OK | OK | OK | OK | OK | OK |
| D7 estándares abiertos namables; productos comerciales anclados a §17, no hardcodeados; sin vocabulario del dominio fuente | OK | OK | OK | obs | OK | OK |
| D8 coherencia con tipo web-monolith (divergencias gobernadas por ADR) | OK | OK | OK | OK | OK | OK |

Notas de evidencia:

- D2: ningún archivo usa el patrón prohibido `.v1.0.md`; todos usan `_v1.0.md`. Slugs kebab lowercase y ASCII. El nombre de la guía `guia-publicacion-paquete-self-contained-linux-arm_v1.0.md` respeta §3.1 (prefijo de familia `paquete-`, kebab-case, sin gestor hardcodeado).
- D3: numeración STAGE-01..STAGE-12 contigua, de dos dígitos, sin huecos (§3.2). Los gates G1–G5 y los NFR se referencian, no se inventan.
- D5: sin emojis. El único símbolo no-ASCII recurrente es `↔` ("gates ↔ DoD"), terminología de trazabilidad sancionada. Negritas limitadas a las metalíneas de cabecera (estructurales, §4.1); no hay negritas decorativas mid-sentence (verificado con barrido).
- D7 — estándares abiertos (legítimos y namados): SemVer 2.0.0, Conventional Commits 1.0.0, Keep a Changelog 1.1.0, SLSA, CycloneDX/SPDX, NIST SSDF (SP 800-218), OWASP SCVS. Términos legítimos del dominio/plataforma: Discord, gateway, snowflake, token, systemd, ARM/armv7l/linux-arm. **Sin vocabulario del dominio fuente del bootstrap** (barrido por impresora térmica, ESC-POS, Bluetooth, DSL, ticket, printer: 0 coincidencias).
- D7 — productos comerciales del stack: la plataforma de CI y la herramienta de auto-versioning NO se nombran en el cuerpo; se anclan por capacidad a §17 P.8 (CI) y §17 P.7 (auto-versioning), con la elección puntual abierta a Sprint 0 (§17 P.11). El runtime (.NET / `dotnet`) se nombra como stack legítimo anclado a §17 P.1/P.8 fila por fila. `sigstore/cosign` y `Dependabot/Renovate` aparecen como ejemplares "capacidad tipo … u homólogo", forma que las propias reglas §2.2/§4.5/§4.6 sancionan; no son hardcodeo. Única tensión: los comandos `gh release create/download/list` (CLI de la plataforma de hosting) en GP §2/§3/§4 y PL §6 nombran un producto comercial en el cuerpo sin anclaje explícito a §17 P.8 en la línea (ver H-01, P2).
- D7 — `image-docker`/`docker`: las cinco menciones son rechazos explícitos del default §2.2 prohibido por el cliente (intake §10), forma correcta según la regla; no es uso del artefacto prohibido.
- D8: el tipo web-monolith mapea por default §2.2 a `image-docker` + DEV/QA/STAGING/PROD; ambas divergencias están gobernadas por ADR-05 y referenciadas a intake §10, coherente con "no quitar ambientes sin ADR que lo justifique".

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 pipeline-ci-cd_v1.0.md (§4.2: seis secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 (H1 + metadatos) | OK |
| 1. Stages obligatorios (lint, build, test, SCA, SBOM, firma, publish) con comando/tooling/criterio | OK (STAGE-01..STAGE-12) |
| 2. Matriz de SO y runtime con justificación | OK (x64 build + cross-compile linux-arm) |
| 3. Caché y artefactos con retención | OK (caché por llave; artefactos con retención y adjunto a release) |
| 4. Promotion rules con trigger y prerequisitos | OK (dos transiciones sobre el modelo reducido) |
| 5. Rollback por tipo de artefacto con comando concreto | OK (seis pasos con comandos systemctl/gh/instalador) |
| 6. Notificaciones (canales, severidad, escalamiento) | OK |

### 3.2 estrategia-versionado_v1.0.md (§4.3: seis secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. SemVer 2.0.0 con reglas de incremento | OK (MAJOR/MINOR/PATCH adaptados a proyecto sin API pública) |
| 2. Conventional Commits 1.0.0 con prefijos y BREAKING CHANGE | OK (tabla de prefijos + bump + CHANGELOG por Keep a Changelog) |
| 3. Herramienta de versionado | OK (anclada a §17 P.7, elección abierta a Sprint 0) |
| 4. Branching | OK (GitHub Flow §17 P.7) |
| 5. Canales y semántica de prerelease | OK (prerelease/estable como tipo de release, sin feed) |
| 6. Deprecation policy | OK (descriptor + contrato de entorno + CHANGELOG) |

### 3.3 entornos-deploy_v1.0.md (§4.4: cinco secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Lista de ambientes o canales (propósito, destino, aprobador, SLA) | OK (modelo reducido de tres etapas, justificado) |
| 2. Provisión (IaC) | OK (IaC mínima como scripts versionados, justificada) |
| 3. Configuración por ambiente (12-factor) | OK (mapa por ambiente; binario único promovido) |
| 4. Secretos (almacenamiento, rotación, prohibición de commit) | OK (clave maestra, token cifrado, credencial) |
| 5. Promoción (procedimiento, aprobador, registro de auditoría) | OK (tag SemVer + release manager) |

### 3.4 guia-publicacion-paquete-self-contained-linux-arm_v1.0.md (§4.5: cinco secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. Pre-requisitos (credenciales, scopes, config local) | OK |
| 2. Comando o stage de publicación con variables de entorno | OK (comando reproducible + STAGE-11/12) |
| 3. Verificación post-publish (instalación, descarga, firma/checksum) | OK (cinco pasos, incluye medición de NFR) |
| 4. Rollback (retiro/reemplazo, ventana de gracia, comunicación) | OK (seis pasos con comando concreto, preserva entorno y clave) |
| 5. Métricas | OK (seis indicadores observables) |

Nombre del archivo: cumple §3.1. `paquete-self-contained-linux-arm` usa el prefijo de familia `paquete-`, kebab-case, parametrizado por tipo de artefacto, sin hardcodear gestor (corrección del antecedente `guia-publicacion-nuget` del fuente).

### 3.5 supply-chain-seguridad_v1.0.md (§4.6: seis secciones)

| Sección obligatoria | Estado |
| --- | --- |
| Cabecera §4.1 | OK |
| 1. SBOM (formato, generador, salida, publicación, firma del SBOM) | OK (CycloneDX/SPDX JSON, adjunto y firmado) |
| 2. Firma (mecanismo, transparency log, verificación) | OK (firma del paquete y del SBOM) |
| 3. SLSA (nivel objetivo + plan de elevación) | OK (L1 piso, plan a L2, L3 futuro, L4 no previsto) |
| 4. Dependency scanning (tooling, frecuencia, política por severidad) | OK (SCA + actualizador automático) |
| 5. SAST y DAST (herramientas, stages, criterios de bloqueo) | OK (SAST en pipeline; DAST acotado al panel) |
| 6. Política de CVE (SLA por severidad, comunicación) | OK (SLA por severidad + comunicación por CHANGELOG) |

### 3.6 README.md de la sección (recomendado, §3.5)

| Elemento esperado | Estado |
| --- | --- |
| Índice navegable con estado por documento | OK |
| Orden de lectura sugerido | OK |
| Resolución de divergencias §2.2 (artefacto y ambientes) | OK |
| Mapeo gates ↔ stages y NFR ↔ stage/gate | OK |
| Trazabilidad upstream/downstream | OK |

### 3.7 Nivel solución (caso degenerado)

| Elemento | Estado |
| --- | --- |
| `_solucion/pipeline-solucion_v1.0.md` omitido (un único proyecto) | OK (correcto, §2.1/§4.9; no existe carpeta `_solucion/`) |

---

## 4. Cumplimiento de §6 (12 ítems de nivel proyecto de la regla 09)

| # | Criterio §6 | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | Existe pipeline-ci-cd con stages obligatorios, matriz SO/runtime, caché, artefactos, promotion rules, rollback y notificaciones | OK | STAGE-01..12 (lint, build, SCA, SBOM, firma, publish presentes); §3 matriz; §4 caché/artefactos; §5 promotion; §6 rollback; §7 notificaciones |
| 2 | Existe estrategia-versionado con SemVer 2.0.0, Conventional Commits, herramienta declarada, branching, canales, deprecation | OK | §1 SemVer; §2 CC + Keep a Changelog; §3 herramienta anclada §17 P.7; §4 GitHub Flow; §5 canales; §6 deprecation |
| 3 | Existe entornos-deploy con el modelo correcto para el tipo D8 | OK | Modelo reducido de tres etapas, justificado contra el default §2.2 por intake §10 y ADR-05 (divergencia gobernada, no silenciosa) |
| 4 | Existe ≥1 guia-publicacion con pre-requisitos, comando/stage, verificación post-publish, rollback y métricas | OK | GP §1–§5 completas; nombre parametrizado familia `paquete-` |
| 5 | Existe supply-chain-seguridad con SBOM, firma, SLSA, dependency scanning, SAST/DAST, CVE | OK | SC §1–§6 completas |
| 6 | Ningún archivo hardcodea un gestor de paquetes en el nombre genérico; patrón `guia-publicacion-<tipo-artefacto>` parametrizado | OK | `paquete-self-contained-linux-arm` (familia `paquete-`, sin nuget/npm/etc.) |
| 7 | El pipeline ejecuta la DoD de 08 como gates, sin redefinir criterios localmente | OK | PL §2 nota 3 y §8: STAGE-01..07 ejecutan "literalmente G1–G5 de 08"; remite a `definition-of-done` y `estrategia-calidad §3`; excepción solo con ADR + BT |
| 8 | Cada NFR numérico de 05 §8 tiene stage o gate que lo verifica antes de promover | OK | Cobertura → STAGE-07/G3; latencia/throughput/memoria → pre-producción sobre hardware real (PL §5/§8, EN §1, GP §3); disponibilidad → operación (reinicio systemd, ADR-05); limpieza efectiva → suite 08 |
| 9 | Cada ambiente/canal declara aprobador y SLA o ventana cuando corresponde | OK | EN §1: desarrollo (Auto, —), pre-producción (Auto, sin SLO formal, mide NFR), producción (release manager, SLO 99 % mensual) |
| 10 | Rollback documentado por tipo de artefacto con comando concreto | OK | PL §6 y GP §4: `gh release list/download`, `systemctl stop/start/status`, `instalar.sh --conservar-entorno`; preserva entorno y clave (ADR-07) |
| 11 | SBOM y firma se generan automáticamente en el pipeline y se adjuntan al release | OK | STAGE-09 (SBOM, "Sí en release"), STAGE-10 (firma, transparency log), ambos adjuntos (§4 artefactos: permanente, adjunto al release) |
| 12 | No aparecen stacks/protocolos del dominio fuente fuera de la tabla Tipo D8 → artefacto | OK | Barrido D7: 0 vocabulario del dominio fuente; `docker` solo como rechazo del default |

Observación sobre los ítems 7 y 8: el pipeline no solo referencia la DoD sino que mapea cada stage al gate G1–G5 y a la frase DoD/NFR que verifica (PL §2 columna "DoD/NFR que verifica"). Los NFR no medibles en el pipeline (latencia, throughput, memoria) se verifican en pre-producción sobre hardware real antes de promover, y disponibilidad se sostiene por reinicio systemd y se mide en operación — atribución coherente con la naturaleza auto-hospedada del proyecto y con 05 §8.

---

## 5. Coherencia cross-doc y trazabilidad

### 5.1 Cada quality gate ↔ criterio DoD de 08 o NFR de 05 §8

Mapeo verificado contra `estrategia-calidad_v1.0.md` §3 y `definition-of-done_v1.0.md`:

| Gate (08 §3) | Stage del pipeline | Criterio | Referencia DoD/NFR | Coincide |
| --- | --- | --- | --- | --- |
| G1 Compilación | STAGE-02 Build | 0 errores | DoD US/BT/release "compila sin errores" | OK |
| G2 Tests en verde | STAGE-04/05/06 | unit/integración/e2e 100 % | DoD §1.1/§1.3; pirámide 08 | OK |
| G3 Cobertura por capa | STAGE-07 | líneas ≥ 75 %, branches ≥ 65 %, detección ≥ 90 % | DoD §1.3/§1.4; NFR cobertura 05 §8 | OK |
| G4 Formato | STAGE-01 | sin diferencias | DoD "formato sin diferencias" | OK |
| G5 Análisis estático | STAGE-03 | sin warnings nuevos | DoD "análisis estático sin warnings nuevos" | OK |

Los valores de cobertura (≥ 90 % detección; global ≥ 75 % líneas / ≥ 65 % branches) son idénticos en intake §17 P.6, 05 §8, 08 §3 (G3) y 09 (PL §2 STAGE-07, README). El pipeline declara explícitamente que la DoD no se redefine (PL §8). Coherente.

### 5.2 Modelo de ambientes ↔ NFR de disponibilidad/latencia de 05

- Producción declara el SLO de disponibilidad 99 % mensual (EN §1, README, PL §8), idéntico a 05 §8 e intake §17 P.10, sostenido por reinicio automático del servicio systemd (ADR-05).
- Pre-producción es donde se miden latencia (p95 < 200 ms), throughput (≥ 50 mensajes/s) y memoria (≤ 8 MB por conexión) sobre hardware real antes de promover (EN §1, PL §5/§8, GP §3). Valores idénticos a 05 §8. La promoción a producción exige esos NFR dentro de SLA o excepción con ADR (DoD release). Coherente.

### 5.3 Rollback ↔ ADR-07 (preservación de entorno y clave maestra)

PL §6 y GP §4 reinstalan la publicación previa preservando el archivo de entorno y la clave maestra (`instalar.sh --conservar-entorno`), de modo que los tokens cifrados sigan válidos sin re-registro, citando ADR-07 §8 y ADR-05. Coincide con ADR-07 §5.3 ("el rollback preserva el archivo de entorno y la clave; los tokens cifrados siguen válidos") y con la DoD release de 08 §1.4 ("el rollback preserva la clave maestra y los tokens cifrados siguen siendo válidos"). El rollback no toca la base SQLite; los defectos de datos/esquema se tratan por migración. Coherente.

### 5.4 Divergencias de artefacto y de ambientes ↔ ADR-05 / intake §10

- Artefacto: paquete self-contained `linux-arm` instalado como servicio systemd, NO `image-docker`. Justificado en PL §intro, PL §2, GP §intro, README §"resolución de divergencias", con referencia a ADR-05 e intake §10 (cliente prohíbe contenedores). ADR-05 §2/§4 confirma la decisión y descarta contenedores ("Prohibido por el cliente … viola §10"). Divergencia gobernada, no silenciosa.
- Ambientes: modelo reducido auto-hospedado de un solo dispositivo, NO la escalera cloud DEV/QA/STAGING/PROD. Justificado en EN §1 contra el default §2.2, con referencia a intake §10/§17 P.9/P.12 y ADR-05, respetando "no quitar ambientes sin ADR". Coherente.
- Anti-patrón §4.8 "confundir publicación con despliegue": respetado explícitamente en PL §intro, EN §intro y GP §intro (publicación = build/firma/publish del paquete; despliegue = instalación/registro del servicio en el dispositivo).

### 5.5 Omisión de 10 ↔ ADR-11

ADR-11 existe en `SDD2.2D/docs/05_arquitectura_tecnica/adrs/`, estado Aceptado, bandera `tiene_portal_developers = false`, y justifica el colapso de la categoría 10 en READMEs (operador único, sin superficie pública §17 P.3, no redistribuible §17 P.7). La carpeta 10 con contenido NO existe (verificado), lo cual es correcto. Los documentos de 09 referencian consistentemente "categoría 10 colapsada en READMEs (ADR-11)" para los comandos locales equivalentes. Coherente; no es hallazgo.

### 5.6 Anclaje de productos comerciales del stack a §17

La plataforma de CI y la herramienta de auto-versioning están ancladas por capacidad a §17 P.8 y §17 P.7 sin nombrarse en el cuerpo (verificado: 0 menciones de "GitHub Actions", "GitVersion", "Nerdbank" en el cuerpo). El runtime (.NET/`dotnet`) se ancla a §17 P.1/P.8 fila por fila. La única excepción es la CLI `gh` de la plataforma de hosting en los comandos concretos de publicación y rollback (ver H-01).

---

## 6. Hallazgos enumerados

No se detectaron hallazgos P0 ni P1. Hay dos hallazgos P2 (medios) y tres P3 (bajos), todos de estilo/precisión, ninguno bloqueante.

| # | Nivel | Archivo | Sección | Evidencia | Recomendación |
| --- | --- | --- | --- | --- | --- |
| H-01 | P2 | guia-publicacion-paquete-self-contained-linux-arm_v1.0.md; pipeline-ci-cd_v1.0.md | GP §2/§3/§4; PL §6 | Los comandos `gh release create`, `gh release download`, `gh release list` nombran la CLI de la plataforma de hosting (producto comercial) directamente en el cuerpo. El resto del corpus ancla la plataforma de CI/hosting por capacidad a §17 P.8 y evita nombrarla. La plataforma SÍ está declarada en intake §17 P.8 (la regla §4.2/§4.5 además exige "comando exacto reproducible" y "comando concreto" en rollback), por lo que la tensión es de consistencia de anclaje, no de hardcodeo de una plataforma no declarada; por eso es P2 y no P0/P1. | Agregar el anclaje explícito a §17 P.8 junto a los bloques de comandos `gh` (p. ej. "CLI de la plataforma de releases, `§17 P.8`"), o presentar el comando como "comando de release de la plataforma (`§17 P.8`)" con el `gh` como instancia concreta, para mantener la consistencia con la política de anclaje del resto de los documentos. |
| H-02 | P2 | pipeline-ci-cd_v1.0.md | §6 paso 1; trazabilidad | El rollback referencia `SOLUTION-INTAKE §17 P.8` como respaldo de la preservación de la clave maestra/entorno. §17 P.8 (Pipeline CI/CD) menciona efectivamente el rollback que preserva el archivo de entorno y la clave, por lo que la cita es correcta; sin embargo, el sustento conceptual de la preservación de la clave es ADR-07 (cifrado) y la validez del cifrado es §17 P.5/P.12. La cita a §17 P.8 para la validez de los tokens cifrados es secundaria frente a ADR-07. | Priorizar la referencia a ADR-07 (y §17 P.5) como fuente de la preservación de la clave y la validez de los tokens, dejando §17 P.8 como referencia del procedimiento de rollback del pipeline. Ajuste de precisión de cita; sin impacto funcional. |
| H-03 | P3 | README.md | §"Documentos de la sección" / encabezado | El README declara en su intro "Alimenta a 10 (colapsada en READMEs por ADR-11)" y "11 (los samples…)"; la categoría 11 aún no está en alcance de esta fase. La mención forward es informativa y consistente con la cadena, pero podría leerse como dependencia no satisfecha. | Mantener la mención como forward-reference informativa o marcarla explícitamente como "downstream previsto (fuera del alcance de la Fase F)" para evitar ambigüedad. Cosmético. |
| H-04 | P3 | entornos-deploy_v1.0.md | §3 fila "Ruta de la base SQLite"; §4 fila "Token de bot" | Se nombra "SQLite" y "AES" en el cuerpo. Son, respectivamente, el motor de persistencia del stack (anclado a ADR-02/§17 P.4) y un estándar de cifrado abierto (ADR-07/§17 P.5); ambos están anclados por ADR y no constituyen hardcodeo de producto comercial de la cadena DevOps. Se registra solo por consistencia con la política de describir por capacidad cuando es posible. | Opcional: sustituir "SQLite" por "la base relacional embebida (ADR-02)" en la fila de configuración, coherente con cómo 05 lo describe por capacidad. AES es estándar abierto namable y puede mantenerse. Sin acción obligatoria. |
| H-05 | P3 | pipeline-ci-cd_v1.0.md | §1 Triggers (fila "Tag de prerelease"); §3 matriz | La fila de prerelease enumera `-rc.N`, `-alpha.N`, `-beta.N`; el orden de severidad/madurez natural (alpha → beta → rc) aparece invertido respecto de `estrategia-versionado §5` (que los ordena alpha/beta/rc). Es solo orden de enumeración, no semántica; ambos documentos definen la misma precedencia SemVer. | Unificar el orden de enumeración de sufijos de prerelease entre PL §1 y VR §5 para consistencia de lectura. Estilo. |

---

## 7. Veredicto final

**APROBADO CON OBSERVACIONES.**

La Fase F (categoría 09) del proyecto discord-bots-admin cumple los 12 criterios de §6 de la regla 09, presenta los cinco documentos obligatorios más el README con su estructura completa (§4.2–§4.6), ejecuta los gates G1–G5 de 08 como stages sin redefinir la DoD, verifica cada NFR numérico de 05 §8 con un stage/gate o con una medición en pre-producción/operación antes de promover, documenta el rollback por tipo de artefacto con comandos concretos preservando entorno y clave maestra (ADR-07), genera SBOM y firma automáticos adjuntos al release, y respeta la nomenclatura parametrizada de la guía de publicación sin hardcodear gestor. Las dos divergencias esperadas frente al default §2.2 —artefacto paquete self-contained `linux-arm` en lugar de `image-docker`, y modelo de ambientes reducido en lugar de la escalera cloud— están justificadas explícitamente contra el default con referencia a intake §10 y ADR-05, no faltan silenciosamente, y el anti-patrón §4.8 "confundir publicación con despliegue" se respeta. ADR-11 existe y gobierna la omisión de la categoría 10; la ausencia de la carpeta 10 con contenido es correcta. No existe `_solucion/` (correcto en caso degenerado). No se detectó vocabulario del dominio fuente, ni emojis, ni negritas decorativas, ni el patrón de nombre prohibido `.v`.

No hay hallazgos P0 ni P1: la cadena no se detiene y no hay condiciones bloqueantes para la promoción.

Condiciones / recomendaciones para promover (no bloqueantes):

- H-01 (P2): anclar explícitamente a §17 P.8 los comandos `gh release` de publicación y rollback, para consistencia con la política de anclaje del resto del corpus.
- H-02 (P2): priorizar ADR-07 (y §17 P.5) como fuente de la preservación de la clave maestra y la validez de los tokens cifrados en la cita del rollback, dejando §17 P.8 para el procedimiento.
- H-03 a H-05 (P3): mejoras de claridad y consistencia de enumeración/forward-references; pueden absorberse en una revisión menor sin generar versión nueva, o diferirse.

La Fase F puede promoverse. Los hallazgos P2/P3 son de pulido y no condicionan la promoción.

---

## 8. Anexo — Método de verificación

- Lectura completa de los seis entregables de 09 (pipeline-ci-cd, estrategia-versionado, entornos-deploy, guia-publicacion-paquete-self-contained-linux-arm, supply-chain-seguridad, README) y de la regla 09 v1.3 (§2.2, §3.1, §4, §4.8, §6).
- Verificación de upstream: 05 (`arquitectura-solucion_v1.0.md` §8 tabla de NFR; ADR-05, ADR-06, ADR-07, ADR-11 leídas íntegras), 08 (`definition-of-done_v1.0.md` cuatro capas; `estrategia-calidad_v1.0.md` §3 gates G1–G5), intake (`SOLUTION-INTAKE-discord-bots-admin_v1.0.md` §10 restricciones, §17 P.6/P.7/P.8/P.9/P.10/P.11/P.12).
- Reconstrucción del mapeo gate ↔ stage ↔ DoD/NFR (5/5 gates), de los seis NFR de 05 §8 con su stage/gate o medición, y de las dos divergencias (artefacto y ambientes) contra ADR-05 e intake §10.
- Cotejo del rollback (PL §6 y GP §4) contra ADR-07 §5.3 y DoD release 08 §1.4 (preservación de clave/entorno, validez de tokens cifrados).
- Barridos automáticos sobre los seis archivos: patrón `.v` en nombres (0); filenames no-ASCII (0); vocabulario del dominio fuente del bootstrap —impresora térmica, ESC-POS, Bluetooth, DSL, ticket, printer— (0); emojis (0; solo `↔` sancionado); negritas decorativas mid-sentence (0; solo metalíneas de cabecera); `image-docker`/`docker` (5, todas como rechazo explícito del default §2.2); productos comerciales de CI/auto-versioning en el cuerpo (0 de "GitHub Actions"/"GitVersion"/"Nerdbank"; `sigstore/cosign` y `Dependabot/Renovate` como ejemplares "capacidad tipo … u homólogo" sancionados por la regla; `gh` CLI presente — H-01).
- Comprobación de la existencia de ADR-11 y de la ausencia correcta de la carpeta 10 con contenido y de `_solucion/`.
