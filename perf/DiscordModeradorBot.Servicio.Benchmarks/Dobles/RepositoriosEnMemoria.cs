using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Benchmarks.Dobles;

/// <summary>
/// Repositorio de incidentes NO-OP para el benchmark: descarta el incidente sin tocar disco ni
/// base de datos. Así la etapa 9 del pipeline (registro del incidente, RN-11) no introduce E/S y
/// el benchmark mide el motor de evaluación, no la persistencia. El conteo de agregados queda
/// disponible para validar que el camino "ráfaga que dispara" efectivamente persistió.
/// </summary>
internal sealed class RepositorioIncidentesNoOp : IRepositorioIncidentes
{
    public long Agregados { get; private set; }

    public Task AgregarAsync(Incidente incidente, CancellationToken ct = default)
    {
        Agregados++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Incidente>> ListarAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Incidente>>(Array.Empty<Incidente>());

    public Task<PaginaIncidentes> BuscarAsync(FiltroIncidentes filtro, CancellationToken ct = default) =>
        Task.FromResult(new PaginaIncidentes(Array.Empty<Incidente>(), 0));

    public Task<Incidente?> ObtenerAsync(int id, CancellationToken ct = default) =>
        Task.FromResult<Incidente?>(null);

    public Task<bool> MarcarRevertidoAsync(
        int incidenteId, int administradorId, DateTimeOffset fecha, CancellationToken ct = default) =>
        Task.FromResult(false);
}

/// <summary>
/// Repositorio de servidores en memoria para el benchmark: devuelve un único servidor fijo (con su
/// canal de salida designado) para que el camino de reporte resuelva el canal sin tocar la base.
/// </summary>
internal sealed class RepositorioServidoresEnMemoria : IRepositorioServidores
{
    private readonly ServidorRegistrado? _servidor;

    public RepositorioServidoresEnMemoria(ServidorRegistrado? servidor) => _servidor = servidor;

    public Task<bool> ExisteAsync(Snowflake snowflakeServidor, CancellationToken ct = default) =>
        Task.FromResult(_servidor is not null);

    public Task AgregarAsync(ServidorRegistrado servidor, CancellationToken ct = default) =>
        Task.CompletedTask;

    public Task<ServidorRegistrado?> ObtenerAsync(Snowflake snowflakeServidor, CancellationToken ct = default) =>
        Task.FromResult(_servidor);

    public Task<IReadOnlyList<ServidorRegistrado>> ListarAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ServidorRegistrado>>(
            _servidor is null ? Array.Empty<ServidorRegistrado>() : new[] { _servidor });

    public Task<bool> ActualizarEstadoAsync(
        Snowflake snowflakeServidor,
        EstadoActivacion estadoActivacion,
        EstadoConexion estadoConexion,
        CancellationToken ct = default) =>
        Task.FromResult(false);
}

/// <summary>
/// Repositorio de exenciones en memoria para el benchmark: siempre devuelve un conjunto fijo de
/// exenciones del servidor. Por defecto vacío (la etapa 1 no descarta y se mide el pipeline
/// completo), pero la colección es configurable para escenarios futuros.
/// </summary>
internal sealed class RepositorioExencionesEnMemoria : IRepositorioExenciones
{
    private readonly IReadOnlyList<Exencion> _exenciones;

    public RepositorioExencionesEnMemoria(IReadOnlyList<Exencion>? exenciones = null) =>
        _exenciones = exenciones ?? Array.Empty<Exencion>();

    public Task<bool> AgregarAsync(Snowflake servidorId, Exencion exencion, CancellationToken ct = default) =>
        Task.FromResult(true);

    public Task<IReadOnlyList<Exencion>> ListarPorServidorAsync(
        Snowflake servidorId, CancellationToken ct = default) =>
        Task.FromResult(_exenciones);

    public Task<bool> QuitarAsync(Snowflake servidorId, Exencion exencion, CancellationToken ct = default) =>
        Task.FromResult(false);
}

/// <summary>Reloj de tiempo fijo para una medición determinista (estrategia-testing §7, ADR-09).</summary>
internal sealed class RelojFijo : IReloj
{
    public RelojFijo(DateTimeOffset ahora) => Ahora = ahora;

    public DateTimeOffset Ahora { get; set; }
}
