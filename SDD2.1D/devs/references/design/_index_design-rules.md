# Índice — Catálogo de reglas de diseño

**Proyecto:** Template SDD 2.1
**Documento:** _index_design-rules.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-19
**Autor:** AG-ROOT (Arquitecto de Soluciones)

---

## 1. Propósito

`devs/references/design/` es un subárbol metodológico del plano `devs/`, no un artefacto operativo del plano `docs/`. Reúne el catálogo de reglas de diseño que la categoría 03 (UX/UI/DX) consume como insumo normativo: tokens, patrones de componente, estados, feedback, iconografía vectorial, accesibilidad, responsive y movimiento.

El catálogo sigue un modelo base→especialización. Hay un documento base de diseño web genérico, agnóstico de framework, y especializaciones por stack que heredan del base y mapean cada token y cada patrón a su tecnología concreta. La especialización no redefine principios ni inventa patrones: solo materializa el catálogo base en su stack y, cuando una limitación técnica lo impone, documenta la desviación con su justificación.

Este subárbol vive en `devs/` porque es material metodológico estable, no salida generada por proyecto. No se ubica en `docs/`, que es exclusivamente la salida del orquestador.

Este índice es el punto de entrada del catálogo: el subagente AG-03 lo carga primero y desde acá resuelve qué documento o documentos aplicar.

## 2. Catálogo de reglas de diseño

| Documento | Ámbito | Hereda de | `project_type` / stack al que aplica | Estado |
| --- | --- | --- | --- | --- |
| `design-rules-web-generico_v1.0.md` | Web genérico | — (base) | web-monolith, web-microservices (con frontend) | Vigente |
| `design-rules-blazor-mudblazor_v1.0.md` | Web Blazor Interactive Server + MudBlazor | web-genérico | proyectos web con stack Blazor + MudBlazor | Vigente |

## 3. Documentos previstos (roadmap)

Huecos esperados del catálogo. Cada especialización futura hereda del documento base web genérico y aplica a su `project_type` o stack. Aún no existen como archivo; se listan para fijar la convención de nombre y el lugar que ocuparán.

| Documento previsto | Ámbito | Hereda de | `project_type` / stack al que aplicará |
| --- | --- | --- | --- |
| `design-rules-html_v1.0.md` | Web con HTML/CSS puro | web-genérico | web-monolith / web-microservices con frontend sin framework de componentes |
| `design-rules-mobile-maui_v1.0.md` | Mobile nativo | web-genérico | mobile-app-maui |
| `design-rules-blazor-maui_v1.0.md` | Blazor embebido en MAUI | web-genérico | mobile-app-maui / desktop-app con UI Blazor Hybrid |

## 4. Cómo se selecciona el documento

El subagente AG-03 elige la especialización según el stack declarado en la Parte C del intake (bloque técnico del proyecto) y el `project_type` del proyecto en curso:

1. Aplica siempre el documento base `design-rules-web-generico_v1.0.md`.
2. Si existe una especialización para el stack declarado, la suma por encima del base (por ejemplo `design-rules-blazor-mudblazor_v1.0.md` cuando el stack es Blazor Interactive Server + MudBlazor).
3. Si no hay especialización para el stack declarado, rige únicamente el documento base.

Ante conflicto entre el base y una especialización, manda la regla base salvo limitación técnica explícita y justificada en el documento hijo.

## 5. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-19 | Índice inicial del catálogo de reglas de diseño. Registra el documento base web genérico y la especialización Blazor + MudBlazor, el roadmap de especializaciones previstas (HTML, MAUI, Blazor en MAUI) y el criterio de selección por stack y `project_type`. | AG-ROOT |
