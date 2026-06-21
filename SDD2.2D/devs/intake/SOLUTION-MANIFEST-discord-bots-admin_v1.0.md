# SOLUTION-MANIFEST-discord-bots-admin

Manifiesto de la solución: enumera los proyectos, su tipo D8, su rol, sus dependencias y sus nombres de código. Es la fuente única de verdad de la enumeración de proyectos para el resto del orquestador.

Artefacto derivado: el orquestador lo construye a partir de `SOLUTION-INTAKE-discord-bots-admin_v1.0.md` §13 durante la Fase de validación de intake (`master-prompt.md` §3), siguiendo `rules/_intake_rules.md` §4, y lo confirma el humano. No se completa a mano. Toda regeneración posterior sigue el flujo de no-modificación de `master-prompt.md` §13.

Esta solución es el caso degenerado (un único proyecto): el manifiesto tiene una sola fila y el orquestador aplana el layout de `/SDD2.2D/docs/` (categorías 00 a 11 directo, sin el subnivel `proyectos/<kebab>/` ni la carpeta `_solucion/`).

---

## §1 Bloque de solución

| Campo | Valor |
|---|---|
| Nombre de solución | Administrador de Bots Moderador para Discord |
| `nombre-solucion-kebab` | discord-bots-admin |
| `NombreSolucionCodigo` | DiscordModeradorBot |
| Proyecto principal | discord-bots-admin |
| Intake (origen) | `SOLUTION-INTAKE-discord-bots-admin_v1.0.md` (de su §13 se deriva este manifiesto) |
| Documento | `SOLUTION-MANIFEST-discord-bots-admin_v1.0.md` |
| Versión | 1.0 |
| Fecha | 2026-06-20 |
| Estado | En revisión |

Nota de nomenclatura: el `nombre-solucion-kebab` (`discord-bots-admin`) y el `NombreSolucionCodigo` (`DiscordModeradorBot`) se adoptan de los valores declarados en el intake (nombre del archivo de intake, raíz del repositorio en §16, perfil de convención de §13 y nombre de código de §17), no de la aplicación literal del algoritmo de `master-prompt.md` §3.2 sobre el nombre legible completo. El perfil de convención de §1.1 es la fuente canónica del naming de código.

### §1.1 Perfil de convención de nombres

| Parámetro | Valor | Notas |
|---|---|---|
| Forma del nombre de solución en código | PascalCase | `DiscordModeradorBot` |
| Separador de segmentos | `.` | Separa la raíz de la solución del sufijo de rol |
| Prefijo de paquetes redistribuibles | `Aplicada` | No se usa en v1: la solución no expone redistribuibles |

---

## §2 Tabla de proyectos

| `nombre-proyecto-kebab` | `nombre-proyecto-codigo` | `project_type` (D8) | Rol en la solución | `redistribuible` | Dependencias | Path `/src` |
|---|---|---|---|---|---|---|
| discord-bots-admin | DiscordModeradorBot.Servicio | web-monolith | Servicio monolítico: panel de administración Blazor Server + bot de moderación embebido + persistencia SQLite, en un solo proceso | false | — | `src/DiscordModeradorBot.Servicio/` |

### §2.1 Regla de nombres de código aplicada

- `nombre-proyecto-codigo` = `<NombreSolucionCodigo>.<Sufijo>` = `DiscordModeradorBot.Servicio`. El sufijo `.Servicio` es el declarado en el intake §17 (mapa de sufijos orientativo, no cerrado; para `web-monolith` el orientativo es `.Web`, pero el intake materializa `.Servicio` por su rol de servicio monolítico panel + bot + persistencia).
- `redistribuible: false`, por lo que el nombre arranca con la raíz de la solución `DiscordModeradorBot` y no con el prefijo `Aplicada`.

---

## §3 Grafo de dependencias y orden topológico

Un único nodo, acíclico de forma trivial.

```text
discord-bots-admin   (sin dependencias)
```

Orden topológico:

```text
nivel 0: discord-bots-admin
```

---

## §4 Validaciones bloqueantes (resultado)

| Validación | Resultado |
|---|---|
| Cada `project_type` ∈ D8 cerrado | Cumple (`web-monolith`) |
| Exactamente un proyecto principal | Cumple (`discord-bots-admin`) |
| Sin colisión de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo` | Cumple (proyecto único) |
| Cada dependencia referencia un proyecto existente | Cumple (sin dependencias) |
| Grafo de dependencias acíclico (DAG) | Cumple (nodo único) |
| `SOLUTION-INTAKE` §13 recorrible | Cumple (filas reales, perfil presente, campos bloqueantes completos) |

---

## §5 Checklist de validación del manifiesto derivado

- [x] El bloque de solución tiene nombre, `nombre-solucion-kebab`, `NombreSolucionCodigo`, proyecto principal y referencias de intake completos.
- [x] El perfil de convención de nombres está declarado (forma PascalCase, separador, prefijo de redistribuibles).
- [x] La tabla de proyectos tiene al menos una fila y todos los campos obligatorios completos.
- [x] Cada `project_type` pertenece al conjunto cerrado D8 de 8 valores.
- [x] Hay exactamente un proyecto principal.
- [x] No hay colisiones de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo`.
- [x] Cada dependencia referencia un proyecto existente en la tabla.
- [x] El grafo de dependencias es acíclico.
- [x] Cada proyecto marcado `redistribuible: true` arranca su nombre de código con el prefijo de organización del perfil (no aplica: no hay redistribuibles).
- [x] El control de cambios refleja la versión y fecha del documento.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-20 | Manifiesto inicial derivado de `SOLUTION-INTAKE-discord-bots-admin_v1.0.md` §13 durante la Fase de validación de intake. Solución de un único proyecto (caso degenerado): `discord-bots-admin` (`DiscordModeradorBot.Servicio`, `web-monolith`). Confirmado por el humano. | Orquestador SDD 2.2 |
