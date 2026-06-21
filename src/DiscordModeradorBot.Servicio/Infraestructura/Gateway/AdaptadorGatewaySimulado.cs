using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Adaptador del gateway simulado (intake §18 sample a/b, ADR-04). Es el adaptador activo
/// en R1: permite inyectar mensajes simulados para el walking skeleton y los tests, y
/// registra (loguea) la acción de baneo en vez de llamar a la plataforma real. No usa red
/// ni token real.
/// </summary>
public sealed class AdaptadorGatewaySimulado : IAdaptadorGateway
{
    private readonly ILogger<AdaptadorGatewaySimulado> _logger;

    public AdaptadorGatewaySimulado(ILogger<AdaptadorGatewaySimulado> logger) => _logger = logger;

    public event Func<MensajeEntrante, Task>? MensajeRecibido;

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

    public Task BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default)
    {
        // En el adaptador simulado la acción se loguea, no se ejecuta contra la plataforma.
        _logger.LogInformation(
            "[GATEWAY SIMULADO] Baneo con borrado retroactivo de {Dias} día(s) sobre usuario {Usuario} " +
            "en servidor {Servidor} (acción registrada, no se llamó a la plataforma).",
            ventanaBorrado.TotalDays, usuarioId.Valor, servidorId.Valor);

        return Task.CompletedTask;
    }
}
