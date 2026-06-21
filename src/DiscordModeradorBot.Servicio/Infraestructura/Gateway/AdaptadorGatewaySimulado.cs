using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Adaptador del gateway simulado (intake §18 sample a/b, ADR-04). Es el adaptador activo
/// fuera de la integración real: permite inyectar mensajes simulados para el walking
/// skeleton y los tests, y REGISTRA (loguea) las acciones ejecutadas — reporte y baneo con
/// borrado retroactivo — en vez de llamar a la plataforma real. No usa red ni token real.
/// Conserva las acciones ejecutadas en memoria para verificación y demo.
/// </summary>
public sealed class AdaptadorGatewaySimulado : IAdaptadorGateway
{
    private readonly ILogger<AdaptadorGatewaySimulado> _logger;
    private readonly List<AccionEjecutada> _accionesEjecutadas = new();
    private readonly object _candado = new();

    public AdaptadorGatewaySimulado(ILogger<AdaptadorGatewaySimulado> logger) => _logger = logger;

    public event Func<MensajeEntrante, Task>? MensajeRecibido;

    /// <summary>Acción ejecutada registrada por el adaptador simulado, para verificación y demo.</summary>
    public abstract record AccionEjecutada;

    /// <summary>Reporte publicado en un canal de salida (CU-05).</summary>
    public sealed record ReporteEjecutado(CanalDeSalida CanalSalida, ReporteIncidente Reporte) : AccionEjecutada;

    /// <summary>Baneo con borrado retroactivo ejecutado (CU-02/CU-03).</summary>
    public sealed record BaneoEjecutado(
        Snowflake ServidorId, Snowflake UsuarioId, TimeSpan VentanaBorrado) : AccionEjecutada;

    /// <summary>Desbaneo (reversión de un baneo) ejecutado (CU-07).</summary>
    public sealed record DesbaneoEjecutado(Snowflake ServidorId, Snowflake UsuarioId) : AccionEjecutada;

    /// <summary>Acciones ejecutadas en orden, para verificación del walking skeleton (RN-05).</summary>
    public IReadOnlyList<AccionEjecutada> AccionesEjecutadas
    {
        get
        {
            lock (_candado)
            {
                return _accionesEjecutadas.ToList();
            }
        }
    }

    /// <summary>
    /// Inyecta un mensaje simulado, como si lo hubiera entregado el canal de eventos.
    /// </summary>
    public async Task InyectarMensajeAsync(MensajeEntrante mensaje)
    {
        var handler = MensajeRecibido;
        if (handler is not null)
        {
            await handler(mensaje);
        }
    }

    public Task ReportarAsync(
        CanalDeSalida canalSalida, ReporteIncidente reporte, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(canalSalida);
        ArgumentNullException.ThrowIfNull(reporte);

        lock (_candado)
        {
            _accionesEjecutadas.Add(new ReporteEjecutado(canalSalida, reporte));
        }

        var canales = string.Join(", ", reporte.CanalesAfectados.Select(c => c.Valor));
        var mensajes = string.Join(
            " | ", reporte.MensajesAccionados.Select(m => $"{m.MensajeId.Valor}:'{m.ContenidoCopiado}'"));

        _logger.LogInformation(
            "[GATEWAY SIMULADO] Reporte {Etiqueta} publicado en canal {Canal} ({Proposito}) por la " +
            "política '{Politica}': emisor {Usuario}, acción {Accion}, canales afectados [{Canales}], " +
            "mensajes accionados [{Mensajes}].",
            reporte.EsSimulacion ? "(SIMULACIÓN)" : "(EJECUCIÓN)",
            canalSalida.SnowflakeCanal.Valor, canalSalida.PropositoLogico, reporte.NombrePolitica,
            reporte.UsuarioId.Valor, reporte.Accion, canales, mensajes);

        return Task.CompletedTask;
    }

    public Task BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default)
    {
        lock (_candado)
        {
            _accionesEjecutadas.Add(new BaneoEjecutado(servidorId, usuarioId, ventanaBorrado));
        }

        // En el adaptador simulado la acción se loguea, no se ejecuta contra la plataforma.
        _logger.LogInformation(
            "[GATEWAY SIMULADO] Baneo con borrado retroactivo de {Dias} día(s) sobre usuario {Usuario} " +
            "en servidor {Servidor} (acción registrada, no se llamó a la plataforma).",
            ventanaBorrado.TotalDays, usuarioId.Valor, servidorId.Valor);

        return Task.CompletedTask;
    }

    public Task DesbanearAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default)
    {
        lock (_candado)
        {
            _accionesEjecutadas.Add(new DesbaneoEjecutado(servidorId, usuarioId));
        }

        // En el adaptador simulado el desbaneo se loguea, no se ejecuta contra la plataforma.
        // El desbaneo NO restaura mensajes borrados (RN-11).
        _logger.LogInformation(
            "[GATEWAY SIMULADO] Desbaneo del usuario {Usuario} en servidor {Servidor} (acción " +
            "registrada, no se llamó a la plataforma; los mensajes borrados no se restauran, RN-11).",
            usuarioId.Valor, servidorId.Valor);

        return Task.CompletedTask;
    }
}
