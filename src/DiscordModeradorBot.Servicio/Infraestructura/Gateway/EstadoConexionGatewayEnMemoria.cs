using System.Collections.Concurrent;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Implementación en memoria del registro de estado de conexión por servidor (CU-13, ADR-13,
/// ADR-09). Thread-safe (el gestor de conexiones actualiza desde callbacks del SDK y el panel lee
/// concurrentemente). No persiste: el estado se reconstruye al reconectar tras un reinicio.
/// </summary>
public sealed class EstadoConexionGatewayEnMemoria : IEstadoConexionGateway
{
    private readonly ConcurrentDictionary<string, EstadoConexionServidor> _estados = new();
    private readonly Func<DateTimeOffset> _ahora;

    public EstadoConexionGatewayEnMemoria(Func<DateTimeOffset>? ahora = null)
        => _ahora = ahora ?? (() => DateTimeOffset.UtcNow);

    public void Actualizar(Snowflake servidorId, EstadoConexion estado, MotivoEstadoConexion motivo) =>
        _estados[servidorId.Valor] = new EstadoConexionServidor(servidorId, estado, motivo, _ahora());

    public EstadoConexionServidor? Obtener(Snowflake servidorId) =>
        _estados.TryGetValue(servidorId.Valor, out var estado) ? estado : null;

    public IReadOnlyList<EstadoConexionServidor> ObtenerTodos() => _estados.Values.ToList();
}
