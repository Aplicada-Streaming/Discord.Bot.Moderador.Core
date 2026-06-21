using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Criterios de búsqueda filtrada de incidentes para la revisión del panel (CU-06 5.A,
/// wireframes-revision-de-incidentes §3 barra de filtros). Cada criterio es OPCIONAL: si es null no
/// acota. Los filtros se traducen a predicados aplicables en la consulta a la base (no se filtra en
/// memoria) y la paginación se resuelve también en la consulta. Es un contrato del puerto, no toca
/// la plataforma ni el dominio del motor.
/// </summary>
public sealed record FiltroIncidentes
{
    /// <summary>Acota por servidor (snowflake). Null = todos los servidores.</summary>
    public Snowflake? Servidor { get; init; }

    /// <summary>Acota por modo (Simulación/Ejecución, RN-09). Null = todos los modos.</summary>
    public Modo? Modo { get; init; }

    /// <summary>Acota por resultado (Ejecutada/Simulada/NoAccionable/Fallida, RN-01). Null = todos.</summary>
    public ResultadoModeracion? Resultado { get; init; }

    /// <summary>
    /// Texto de búsqueda de usuario emisor (snowflake o subcadena). Null/vacío = todos los usuarios.
    /// Se compara como contiene sobre el snowflake del emisor.
    /// </summary>
    public string? UsuarioTexto { get; init; }

    /// <summary>Límite inferior del rango de fechas (inclusivo). Null = sin tope inferior.</summary>
    public DateTimeOffset? Desde { get; init; }

    /// <summary>Límite superior del rango de fechas (inclusivo). Null = sin tope superior.</summary>
    public DateTimeOffset? Hasta { get; init; }

    /// <summary>Página solicitada, base 1 (CU-06 paginación). Se acota a un mínimo de 1.</summary>
    public int Pagina { get; init; } = 1;

    /// <summary>Tamaño de página (cantidad de incidentes por página). Se acota a un mínimo de 1.</summary>
    public int TamanoPagina { get; init; } = 10;
}

/// <summary>
/// Resultado de una consulta filtrada y paginada de incidentes (CU-06): la página de resultados y el
/// total que satisface el filtro (para calcular la cantidad de páginas en el panel). El total se
/// calcula en la base sobre el mismo predicado, antes de paginar.
/// </summary>
/// <param name="Incidentes">La página de incidentes que satisface el filtro, ordenada por fecha descendente.</param>
/// <param name="Total">Cantidad total de incidentes que satisfacen el filtro (sin paginar).</param>
public sealed record PaginaIncidentes(IReadOnlyList<Incidente> Incidentes, int Total);
