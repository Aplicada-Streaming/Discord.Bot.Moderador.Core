# Reglas de validación del intake unificado

**Archivo target:** `/SDD2.1D/devs/intake/SOLUTION-INTAKE-<nombre-solucion-kebab>_v1.0.md`
**Consumidor:** la Fase de validación de intake del `master-prompt.md` (previa a la Fase A).
**Versión de las reglas:** 1.0

---

## §1 Propósito y posición

Esta es una regla meta, no una de las doce categorías numeradas: por eso lleva el prefijo `_`, como `_root_rules.md`. Codifica cómo el orquestador valida la completitud del intake unificado antes de despachar cualquier subagente, y cómo deriva de él el `SOLUTION-MANIFEST` canónico.

La validación que define este archivo se distingue de los dos mecanismos preexistentes del orquestador y no los pisa:

- `master-prompt.md` §2 (detención por placeholders): scan sintáctico superficial de marcadores literales sin completar. Sigue vigente.
- `master-prompt.md` §9 (ambigüedad por subagente): detección reactiva, en runtime, cuando un subagente ya generando detecta un dato faltante. Sigue vigente.
- Estas reglas: validación de completitud semántica, proactiva y previa al despacho, que emite una batería consolidada de preguntas para evitar que la generación derive por subespecificación.

El orden es: el orquestador lee el intake, corre §2 (placeholders), corre esta validación (completitud semántica + derivación del manifiesto), se detiene hasta que el humano resuelve, y recién entonces entra a la Fase A. La ambigüedad de §9 queda para lo que aparezca en runtime pese a la validación previa.

---

## §2 Campos bloqueantes

Son bloqueantes todos los campos del intake marcados con `(*)` en sus preguntas guía, más los siguientes, sin los cuales el orquestador no puede operar:

- Cabecera: nombre de la solución, estado.
- §13 Proyectos: la tabla con al menos una fila; por cada proyecto su `nombre-proyecto-kebab`, su `project_type` D8, su rol y sus dependencias; el proyecto principal señalado; el perfil de convención de nombres.
- §17 por proyecto: para cada proyecto de §13, el bloque técnico con su identidad y, como mínimo, P.6 (cobertura numérica), P.7 (SemVer/Conventional Commits), P.8 (quality gates), P.9 (plataformas) y P.10 (NFR numéricos).

Un campo bloqueante vacío, con placeholder o con valor `desconocido` detiene la cadena y genera una entrada en la batería de preguntas de §6.

---

## §3 Patrones de placeholder que disparan pregunta

Disparan pregunta, además de los de §2 del master-prompt:

- Marcadores literales sin completar: `[…]`, `Pendiente`, `TBD`, `[Reemplazar]`, `[Nombre]`, `[YYYY-MM-DD]`, `[kebab]`, `[uno de los 8 D8]`, y cualquier corchete de la plantilla original.
- Tablas con filas de ejemplo no sustituidas (por ejemplo la fila `| [kebab] (principal) | … |` de §13 sin reemplazar).
- NFR o cobertura expresados de forma no numérica ("rápido", "alta", "razonable") donde la regla exige número.
- `project_type` fuera del conjunto cerrado D8.

---

## §4 Reglas de derivación del `SOLUTION-MANIFEST`

A partir de §13 del intake (y del perfil de convención declarado), el orquestador construye el `SOLUTION-MANIFEST-<nombre-solucion-kebab>_v1.0.md` en el formato de referencia de `SOLUTION-MANIFEST-template.md`. Pasos:

1. Derivar `nombre-solucion-kebab` y `NombreSolucionCodigo` (PascalCase) del nombre de la solución, con el algoritmo de normalización de `master-prompt.md` §3.2.
2. Por cada fila de §13, derivar `nombre-proyecto-codigo` como `<NombreSolucionCodigo>.<Sufijo>`, salvo `redistribuible: true`, que arranca con el prefijo de organización del perfil (`Aplicada` por defecto). Derivar el path `src/<nombre-proyecto-codigo>/`.
3. Componer el bloque de solución (nombre, kebab, NombreSolucionCodigo, proyecto principal, perfil de convención, referencia al `SOLUTION-INTAKE` como origen) y la tabla de proyectos.

Mapeo de campos de §13 del intake al manifiesto:

| Campo en `SOLUTION-INTAKE` §13 | Campo en el manifiesto |
|---|---|
| `nombre-proyecto-kebab` | `nombre-proyecto-kebab` (directo) |
| `project_type` (D8) | `project_type` (directo) |
| Rol en la solución | Rol (directo) |
| Dependencias | Dependencias (directo, validadas) |
| `redistribuible` | `redistribuible` (directo) |
| (derivado) | `nombre-proyecto-codigo` = `<NombreSolucionCodigo>.<Sufijo>` o `Aplicada.<X>` |
| (derivado) | Path `src/<nombre-proyecto-codigo>/` |
| Perfil de convención (cabecera/§13) | Perfil de convención del bloque de solución |
| Nombre de la solución (cabecera) | `nombre-solucion-kebab`, `NombreSolucionCodigo`, proyecto principal |

Validaciones bloqueantes de la derivación (si alguna falla, no se deriva el manifiesto y se reporta en la batería de §6):

- Cada `project_type` pertenece al conjunto cerrado D8 (exactamente 8 valores: `library`, `web-monolith`, `web-microservices`, `desktop-app`, `mobile-app-maui`, `rest-api`, `cli-tool`, `worker-service`).
- Hay exactamente un proyecto principal.
- No hay colisión de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo`.
- Cada dependencia referencia un proyecto existente en §13.
- El grafo de dependencias es acíclico (DAG).

Confirmación humana: el orquestador presenta el manifiesto derivado y espera confirmación explícita antes de tratarlo como artefacto canónico. El manifiesto no se completa a mano; se genera y se confirma. Toda regeneración posterior sigue el flujo de no-modificación de §13 del master-prompt.

---

## §5 Validaciones de completitud semántica

Por parte del intake, el orquestador verifica presencia y coherencia mínima:

- Negocio (Parte A): problema y consecuencia (§1); al menos un stakeholder por categoría (§2); MoSCoW con Must mínimo (§4); 3 historias y 2 roles si aplica (§5); 5 casos límite (§7); 3 métricas SMART de negocio (§8); 3 exclusiones (§9); presupuesto y fecha o "sin fecha" justificado (§10); 3 riesgos (§11); 5 términos de glosario (§12).
- Composición (Parte B): §13 completa y derivable (ver §4); §14 declara los contratos entre proyectos coherentes con las dependencias de §13; §15 garantiza valor end-to-end en el primer sprint; §16 deriva el árbol de la jerarquía y la convención de nombres.
- Técnica (Parte C): §17 completo por cada proyecto de §13, con los P bloqueantes de §2.

Coherencia cross-parte que se chequea: la cantidad de bloques §17 coincide con la cantidad de proyectos de §13; los contratos de §14 corresponden a aristas de dependencia de §13; las métricas de negocio (§8) no se confunden con NFR técnicos (§17 P.10).

---

## §6 Formato de la batería de preguntas consolidada

Cuando hay pendientes, el orquestador emite una única batería consolidada (no una pregunta por vez), agrupada por parte y sección, en este formato:

```text
BATERÍA DE VALIDACIÓN DE INTAKE — <nombre-solucion-kebab>
Total de pendientes: <N> (bloqueantes: <B>)

[<ID>] <Parte> / <Sección> — <bloqueante|recomendado>
- Qué falta: <descripción concreta>
- Por qué bloquea: <consecuencia de no resolverlo antes de generar>
- Qué se necesita: <tipo de dato esperado>
- Dónde completarlo: SOLUTION-INTAKE §<sección> [/ proyecto <kebab> si es P.x]
```

Reglas de la batería:

- Se ordena por parte (A, B, C) y dentro de cada una por sección.
- Los bloqueantes se listan primero y se cuentan aparte.
- El orquestador se detiene hasta que el humano actualiza el intake y confirma; entonces re-valida. No avanza a la Fase A con bloqueantes abiertos.
- La actualización del intake la hace el humano sobre el `SOLUTION-INTAKE`; el orquestador no lo edita por su cuenta (coherente con §13 del master-prompt).

---

## §7 Niveles de bloqueo

- Bloqueante: campo de §2, falla de derivación del manifiesto de §4, o incoherencia cross-parte de §5. Detiene la cadena.
- Recomendado: completitud de §5 no bloqueante (por ejemplo menos del mínimo sugerido de un ítem no marcado `(*)`). Se reporta en la batería como recomendado; el humano decide si lo resuelve antes de continuar.

---

## §8 Articulación con el master-prompt

- La Fase de validación de intake del master-prompt invoca estas reglas una sola vez, antes de la Fase A.
- §2 del master-prompt (placeholders) corre antes y cubre lo sintáctico; estas reglas cubren lo semántico y la derivación del manifiesto.
- §9 del master-prompt (ambigüedad por subagente) sigue cubriendo lo que aparezca en runtime; estas reglas reducen su frecuencia, no la reemplazan.
- La derivación del manifiesto de §4 alimenta la lectura del manifiesto que el resto del master-prompt asume (el manifiesto derivado y confirmado se trata como el insumo canónico de la jerarquía).

---

## §9 Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-10 | Reglas iniciales de validación del intake unificado: campos bloqueantes, patrones de placeholder, derivación del `SOLUTION-MANIFEST` desde §13 del intake con sus validaciones, validaciones de completitud semántica, formato de la batería de preguntas consolidada, niveles de bloqueo y articulación con §2 y §9 del master-prompt. | Reformulación SDD 2.1D (unificación de intake) |
