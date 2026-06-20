# Reglas de diseño — Blazor Interactive Server + MudBlazor

**Proyecto:** {{nombre-solucion}}
**Documento:** design-rules-blazor-mudblazor_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-19
**Autor:** {{equipo-o-rol}} (AG-03 UX/UI + Frontend Architect .NET)
**Ámbito:** Web — Blazor Interactive Server (.NET 8/9) con MudBlazor v9
**Hereda de:** `design-rules-web-generico_v1.0.md`
**Posición:** Insumo normativo de la categoría 03 para proyectos `project_type = web-monolith` o `web-microservices (con frontend)` cuyo stack declarado en la Parte C del intake sea Blazor + MudBlazor.

---

## 0. Propósito y relación con el documento base

Este documento especializa el catálogo de diseño web genérico para el stack Blazor Interactive Server con MudBlazor. No redefine principios ni inventa patrones: toma cada token, patrón, estado y criterio del documento base y lo mapea a la API concreta de MudBlazor v9 y a las particularidades del render mode interactivo de servidor.

Regla de herencia: todo lo que no se mapee acá se rige por el documento base. Cuando una limitación de MudBlazor o del modelo de render impida cumplir una regla base, se documenta la desviación en §9 con su justificación.

Versión de referencia del framework: MudBlazor v9 (paleta tipada `PaletteLight`/`PaletteDark`, `MudThemeProvider` con `bind-CurrentPalette`, `MudStepper`, `MudDataGrid`, variables CSS `--mud-palette-*`). Render: `@rendermode InteractiveServer` en .NET 8/9.

---

## 1. Theme: del token al `MudTheme`

Los tokens semánticos del documento base se materializan una sola vez en un `MudTheme` central, consumido por `MudThemeProvider` en el layout raíz. Está prohibido reintroducir colores literales en componentes o en CSS de página: el theme es la fuente única.

```csharp
public static readonly MudTheme AppTheme = new()
{
    PaletteLight = new PaletteLight
    {
        Primary        = "#0F6E56",   // color.brand.primary
        Secondary      = "#534AB7",   // color.accent.module-b
        Tertiary       = "#185FA5",   // color.accent.module-d
        Background      = "#F7F6F4",   // color.background.tertiary (lienzo)
        Surface         = "#FFFFFF",   // color.background.primary (tarjetas)
        AppbarBackground = "#04342C",  // color.brand.primary.dark (chrome)
        DrawerBackground = "#04342C",
        TextPrimary     = "#1A1A18",
        TextSecondary   = "#5C5C57",
        Success         = "#0F6E56",
        Warning         = "#854F0B",
        Error           = "#A03030",
        LinesDefault    = "#E6E6E1",   // color.border.tertiary
        LinesInputs     = "#D9D9D4",   // color.border.secondary
    },
    Typography = new Typography
    {
        Default = new DefaultTypography { FontFamily = ["Inter", "system-ui", "sans-serif"], FontSize = "13px" },
        H6      = new H6Typography { FontSize = "17px", FontWeight = "500" }, // type.title
        Subtitle2 = new Subtitle2Typography { FontSize = "14px", FontWeight = "500" }, // type.body-strong
        Caption = new CaptionTypography { FontSize = "12px" }, // type.caption
    },
    LayoutProperties = new LayoutProperties
    {
        DefaultBorderRadius = "10px",   // radius.md
        DrawerWidthLeft = "168px",
    }
};
```

Tabla de correspondencia token → MudBlazor:

| Token base | MudBlazor |
| --- | --- |
| `color.brand.primary` | `Palette.Primary`, `Color.Primary` en componentes |
| `color.brand.primary.dark` | `Palette.AppbarBackground` / `DrawerBackground` |
| `color.accent.module-*` | `Palette.Secondary` / `Tertiary` + clases utilitarias `mud-theme-*` o `Style` puntual |
| `color.text.*` | `Palette.TextPrimary` / `TextSecondary`; terciario vía `Typography`/`Style` |
| estados (éxito/atención/error) | `Color.Success` / `Color.Warning` / `Color.Error` |
| `radius.md/lg/pill` | `LayoutProperties.DefaultBorderRadius`; pill vía `Style="border-radius: 9999px"` o `Variant` de `MudChip` |
| escala tipográfica | `Typography.*` (H6, Subtitle2, Body2, Caption) |

Acceso desde CSS propio (sin duplicar valores): usar las variables que expone el provider, p. ej. `var(--mud-palette-primary)`, `var(--mud-palette-surface)`, `var(--mud-palette-lines-default)`.

---

## 2. Render mode y estado (Interactive Server)

- Render mode por defecto de las páginas interactivas: `@rendermode InteractiveServer`. Componentes puramente estáticos quedan en SSR estático para no abrir circuito innecesario.
- Prohibido `localStorage`/`sessionStorage` desde el patrón base de artefactos; el estado de UI vive en el componente (`@code`, servicios scoped del circuito) o en almacenamiento de servidor. Persistencia de usuario va por servicio de aplicación, no por almacenamiento de navegador improvisado.
- Latencia de circuito: toda acción que viaja al servidor muestra feedback de "cargando" (ver §5). Botones primarios usan `Disabled="@_procesando"` durante la operación para evitar doble envío.
- Reconexión: la UI de reconexión del circuito se estiliza acorde a la marca (no el default crudo).
- `EventCallback` para comunicación hijo→padre; binding `@bind-Value` para formularios. Evitar lógica pesada en el render; usar `StateHasChanged` con criterio.

---

## 3. Shell de aplicación

El patrón de shell del documento base se arma con el layout de MudBlazor:

```razor
<MudThemeProvider Theme="AppTheme" @bind-IsDarkMode="_isDark" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudDrawer Open="true" Variant="DrawerVariant.Persistent" Width="168px" Class="app-chrome">
        <MudDrawerHeader>...</MudDrawerHeader>
        <MudNavMenu>
            <MudNavLink Href="/" Icon="@TablerIcons.Home" Match="NavLinkMatch.All">Inicio</MudNavLink>
            <MudNavLink Href="/bandeja" Icon="@TablerIcons.Inbox">Bandeja</MudNavLink>
            <MudNavLink Href="/tareas" Icon="@TablerIcons.ListCheck">Tareas</MudNavLink>
        </MudNavMenu>
    </MudDrawer>
    <MudMainContent Class="pa-5" Style="background: var(--mud-palette-background);">
        @Body
    </MudMainContent>
</MudLayout>
```

El ítem activo de `MudNavMenu` ya recibe el color primario; el chrome oscuro se logra con `DrawerBackground` del theme. La barra de acento por módulo (4px) se replica con un `MudPaper` o un `<div>` con `Style="width:4px; background: var(--mud-palette-secondary)"`.

---

## 4. Patrones → componentes MudBlazor

| Patrón base (§4 genérico) | Componentes MudBlazor |
| --- | --- |
| Navegación lateral | `MudDrawer` + `MudNavMenu` + `MudNavLink` (Icon SVG) |
| Tarjeta de acceso | `MudCard` / `MudPaper` con `Elevation="0"` y `Outlined="true"`; ícono en `MudAvatar` cuadrado con `Rounded` |
| ABM grilla de listado | `MudDataGrid<T>` (filtros, orden, paginación, `Loading`) o `MudTable<T>` para casos simples |
| ABM formulario | `MudForm` + `MudTextField` / `MudSelect` / `MudNumericField`, validación con `@bind-Value:after` o `Validation` |
| Wizard / stepper | `MudStepper` con `MudStep` por paso; `MudStepper` ya da indicador, conector y navegación |
| Toggle | `MudSwitch<bool>` con `Color="Color.Primary"` |
| Badge / chip de estado | `MudChip<string>` con `Variant="Variant.Text"` o `Size="Size.Small"` y `Color` semántico |
| Botones | `MudButton` (`Variant.Filled` primario / `Variant.Outlined` secundario), `MudIconButton` para acciones de fila |
| Búsqueda | `MudTextField` con `Adornment="Adornment.Start"` y `AdornmentIcon` de lupa |
| Diálogos / confirmación destructiva | `MudDialog` vía `IDialogService` |
| Toast de éxito/error | `ISnackbar.Add(...)` con `Severity` |

Notas de fidelidad:
- Tarjetas: `Elevation="0"` + `Outlined` para respetar la elevación por borde hairline (no drop-shadow). El borde toma `--mud-palette-lines-default`.
- ABM: en `MudDataGrid`/`MudTable`, la fila seleccionada se resalta con `RowClassFunc` aplicando un tint sutil de marca; las acciones por fila son `MudIconButton` con `aria-label` (`UserAttributes`).
- Stepper: usar el estado completado/actual/pendiente nativo de `MudStepper`; el paso final de revisión es un `MudStep` con resumen en `MudSimpleTable` o filas clave/valor y un `MudSwitch` de activación.
- Formularios: el label va como `Label` del campo (no placeholder); `Variant="Variant.Outlined"` para el borde de control.

---

## 5. Estados y feedback en MudBlazor

| Estado base | Realización MudBlazor |
| --- | --- |
| Cargando (lista/tabla) | `MudDataGrid Loading="true"` o `MudSkeleton` por fila; `MudProgressLinear Indeterminate` en cabecera |
| Cargando (acción puntual) | `MudButton` con `Disabled` + `MudProgressCircular Size="Size.Small"` en el contenido, u `<MudOverlay>` para bloqueo modal |
| Vacío | Bloque centrado con ilustración SVG + texto + `MudButton` de acción siguiente (no tabla vacía a secas) |
| Error | `MudAlert Severity="Severity.Error"` inline, o `ISnackbar` para errores transitorios; mensaje con causa y acción |
| Éxito | `ISnackbar.Add("Publicado", Severity.Success)`; el verbo del toast coincide con el del botón |
| Sin permiso | Control con `Disabled` o no renderizado según rol; `MudTooltip` con el motivo si es visible |

Performance percibida: skeletons por encima de ~400ms; dado que cada interacción cruza el circuito, preferir indicadores tempranos. `MudProgressLinear` fino en la parte superior del contenido durante cargas de página.

---

## 6. Iconografía SVG en MudBlazor (requisito heredado)

MudBlazor renderiza íconos como SVG inline a partir de un string de path/markup, lo que cumple el requisito "vector primero" del documento base. Reglas:

- Los íconos se pasan como string SVG a `MudIcon`, `MudIconButton`, `Icon` de `MudButton`/`MudNavLink`, etc. El SVG hereda el color del componente vía `Color` o `currentColor`.
- Set único por solución. MudBlazor trae Material Symbols (`Icons.Material.*`); si la solución usa Tabler u otro set, se registran como íconos custom (constantes string con el markup SVG) en una clase estática, p. ej.:

```csharp
public static class TablerIcons
{
    public const string Home =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" " +
        "stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M5 12H3l9-9 9 9h-2M5 12v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7\"/></svg>";
    public const string ListCheck = "<svg viewBox=\"0 0 24 24\" ...>...</svg>";
    // ...
}
```

- Color por token: usar `Color="Color.Primary"`, `Color.Error`, etc., o dejar `currentColor` para heredar del contenedor; no fijar `fill` literal en el path.
- Tamaño por rol con `Size="Size.Small|Medium|Large"` o `Style="font-size:..."`; respetar los tamaños del documento base (24/20/16/15px).
- Accesibilidad: ícono decorativo dentro de un control con texto no necesita rótulo; ícono que es la única etiqueta de un `MudIconButton` lleva `aria-label` vía `UserAttributes` (p. ej. `UserAttributes="@(new() { ["aria-label"] = "Eliminar" })"`). No poner el rótulo en el `<svg>`.
- Ilustraciones (estados vacíos, onboarding): SVG con `viewBox`, sin tamaños fijos, optimizado con SVGO, monocromo o rampa de marca. Avatares por iniciales con `MudAvatar` (no foto obligatoria). Logo en SVG con variante para chrome oscuro.
- Evitar `MudImage`/`<img>` con `.svg` cuando se necesita heredar color o animar; reservarlo para fotografía raster justificada.

---

## 7. Accesibilidad en MudBlazor

- MudBlazor v9 mejoró accesibilidad de varios componentes (p. ej. `mud-sr-only` en `MudCheckBox`, ARIA correcto en `MudStepper`). Aun así, el cumplimiento WCAG 2.2 AA es responsabilidad del artefacto, no del componente.
- Foco visible: no suprimir el anillo de foco de MudBlazor; verificar contraste 3:1 del indicador.
- Labels: usar siempre `Label` en campos; `aria-label` en icon-buttons; `For`/`InputId` cuando se separa label y control.
- Color + texto en estados (los `MudChip` de estado llevan texto explícito, no solo `Color`).
- `prefers-reduced-motion`: respetar; reducir transiciones de ripple/animación cuando aplique.
- Navegación por teclado completa en `MudDataGrid`, `MudStepper`, `MudMenu`, diálogos; objetivos de toque ≥ 24px.

---

## 8. Estructura de proyecto y convenciones de código

- Theme en `Theme/AppTheme.cs`; íconos custom en `Theme/TablerIcons.cs`. Una sola definición, importada globalmente (`_Imports.razor`).
- Componentes reutilizables del catálogo (tarjeta de acceso, badge de estado, fila de ABM) como componentes Razor propios que envuelven MudBlazor, para que el patrón viva en un solo lugar.
- CSS propio mínimo y siempre apoyado en `--mud-palette-*`; nada de hex sueltos.
- Nomenclatura de archivos y artefactos según D1–D8 del template (kebab-case en docs, PascalCase en código .NET según el perfil de convención de nombres del manifiesto).

---

## 9. Desviaciones respecto del documento base

| Regla base | Desviación en este stack | Justificación |
| --- | --- | --- |
| (ninguna conocida al cierre v1.0) | — | MudBlazor v9 cubre los patrones del catálogo. Registrar acá cualquier limitación que aparezca en uso real. |

---

## 10. Criterios de aceptación específicos

Además de los criterios del documento base, una superficie Blazor + MudBlazor cumple cuando: todos los colores salen del `MudTheme` (cero hex en componentes/CSS); las páginas interactivas declaran `@rendermode InteractiveServer` y no usan almacenamiento de navegador improvisado; cada acción de servidor tiene feedback de carga y previene doble envío; los íconos son string SVG con `currentColor`/`Color` y ARIA correcto; y los patrones se realizan con los componentes mapeados en §4, no con HTML ad hoc cuando existe componente equivalente.

---

## 11. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Documento base | `design-rules-web-generico_v1.0.md` |
| Especialidad dueña | AG-03 UX/UI + Frontend Architect .NET |
| Regla que lo invoca | `devs/rules/03_rules_ux_ui_dx.md` (selección por stack) |
| Aplica a `project_type` | web-monolith, web-microservices (con frontend) con stack Blazor + MudBlazor |
| Artefactos operativos que lo aplican | `experiencia-de-uso`, `wireframes-<superficie>` del proyecto Blazor |

---

## 12. Control de cambios

| Versión | Fecha | Cambios | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-19 | Versión inicial. Mapeo del catálogo genérico a MudBlazor v9: theme tipado, render Interactive Server, patrones → componentes, estados, iconografía SVG con íconos custom, accesibilidad. | AG-03 UX/UI + Frontend Architect |
