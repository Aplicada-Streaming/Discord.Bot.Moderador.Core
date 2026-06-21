using MudBlazor;

namespace DiscordModeradorBot.Servicio.Theme;

/// <summary>
/// Tema único de la aplicación (fuente de verdad de color/tipografía/espaciado). Materializa los
/// tokens semánticos del catálogo de diseño (design-rules-web-generico §2 y design-rules-blazor-
/// mudblazor §1) en un <see cref="MudTheme"/>. Está prohibido reintroducir hex literales en
/// componentes o CSS de página: el tema es la fuente única; desde CSS propio se accede a las
/// variables <c>--mud-palette-*</c> que expone el provider.
/// </summary>
public static class AppTheme
{
    /// <summary>Tema de la aplicación, consumido por el <c>MudThemeProvider</c> del shell.</summary>
    public static readonly MudTheme App = new()
    {
        // Paleta clara: tokens del documento base (§2.1) mapeados a la paleta tipada de MudBlazor
        // (design-rules-blazor-mudblazor §1). El chrome (AppBar/Drawer) usa el oscuro de marca.
        PaletteLight = new PaletteLight
        {
            Primary = "#0F6E56",            // color.brand.primary (acción, foco, selección)
            Secondary = "#534AB7",          // color.accent.module-b
            Tertiary = "#185FA5",           // color.accent.module-d
            Info = "#185FA5",               // color.text.info
            Success = "#0F6E56",            // estado éxito/activo
            Warning = "#854F0B",            // estado atención
            Error = "#A03030",              // color.text.danger
            Background = "#F7F6F4",         // color.background.tertiary (lienzo de página)
            Surface = "#FFFFFF",            // color.background.primary (tarjetas)
            AppbarBackground = "#04342C",   // color.brand.primary.dark (chrome)
            AppbarText = "#FFFFFF",
            DrawerBackground = "#04342C",   // chrome de la barra lateral
            DrawerText = "#E1F5EE",         // texto del drawer (inactivos a opacidad reducida vía CSS)
            DrawerIcon = "#E1F5EE",
            TextPrimary = "#1A1A18",        // color.text.primary
            TextSecondary = "#5C5C57",      // color.text.secondary
            TextDisabled = "#8A8A82",       // color.text.tertiary
            ActionDefault = "#5C5C57",
            LinesDefault = "#E6E6E1",       // color.border.tertiary (separadores hairline)
            LinesInputs = "#D9D9D4",        // color.border.secondary (controles)
            TableLines = "#E6E6E1",
            Divider = "#E6E6E1",
            DividerLight = "#E6E6E1",
        },

        // Paleta oscura: derivada de los mismos tokens de marca; superficies oscuras coherentes
        // manteniendo el primario de marca para acción/selección y el contraste AA del texto.
        PaletteDark = new PaletteDark
        {
            Primary = "#3FA587",            // marca aclarada para contraste sobre fondo oscuro
            Secondary = "#8A82E0",
            Tertiary = "#5C97D6",
            Info = "#5C97D6",
            Success = "#3FA587",
            Warning = "#D49A4A",
            Error = "#E07A7A",
            Background = "#0E1512",         // lienzo oscuro derivado del chrome de marca
            Surface = "#152019",           // tarjetas
            AppbarBackground = "#04342C",   // chrome de marca (consistente con el modo claro)
            AppbarText = "#FFFFFF",
            DrawerBackground = "#04342C",
            DrawerText = "#E1F5EE",
            DrawerIcon = "#E1F5EE",
            TextPrimary = "#F1F1EF",
            TextSecondary = "#B8B8B2",
            TextDisabled = "#8A8A82",
            ActionDefault = "#B8B8B2",
            LinesDefault = "#2A352F",
            LinesInputs = "#3A4640",
            TableLines = "#2A352F",
            Divider = "#2A352F",
            DividerLight = "#2A352F",
        },

        // Escala tipográfica acotada (§2.2): una familia sans humanista, tamaños y pesos
        // intencionales. Inter como primaria, con system-ui de respaldo.
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "system-ui", "Segoe UI", "sans-serif"],
                FontSize = "13px",          // type.body
                FontWeight = "400",
                LineHeight = "1.45",
            },
            H4 = new H4Typography { FontSize = "17px", FontWeight = "500", LineHeight = "1.2" }, // type.title (encabezado de pantalla)
            H5 = new H5Typography { FontSize = "17px", FontWeight = "500", LineHeight = "1.2" },
            H6 = new H6Typography { FontSize = "17px", FontWeight = "500", LineHeight = "1.2" }, // type.title
            Subtitle1 = new Subtitle1Typography { FontSize = "14px", FontWeight = "500" },
            Subtitle2 = new Subtitle2Typography { FontSize = "14px", FontWeight = "500" },        // type.body-strong
            Body1 = new Body1Typography { FontSize = "13px", FontWeight = "400", LineHeight = "1.45" }, // type.body
            Body2 = new Body2Typography { FontSize = "13px", FontWeight = "400", LineHeight = "1.45" },
            Caption = new CaptionTypography { FontSize = "12px", FontWeight = "400" },            // type.caption
            Button = new ButtonTypography { FontSize = "13px", FontWeight = "500", TextTransform = "none" },
        },

        // Radios y ancho de drawer (§2.3, §3.1): radio de controles 10px; el drawer toma el ancho
        // del chrome del catálogo. El radio lg de tarjetas se aplica puntualmente donde corresponde.
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",   // radius.md (botones, inputs, controles)
            DrawerWidthLeft = "240px",
        },
    };
}
