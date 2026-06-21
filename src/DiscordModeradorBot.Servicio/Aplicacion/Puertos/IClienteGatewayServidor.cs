using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Conexión de gateway de UN servidor (ADR-13: una conexión por contexto). Abstrae el cliente del
/// SDK (Discord.Net) detrás de un puerto para que el gestor de conexiones sea testeable sin red ni
/// <c>DiscordSocketClient</c> (estrategia-testing). El adaptador real wrappea un
/// <c>DiscordSocketClient</c> autenticado con el token del servidor; los tests usan un doble
/// (NSubstitute). El token se entrega ya descifrado en memoria (RN-14) y nunca se loguea.
/// </summary>
public interface IClienteGatewayServidor : IAsyncDisposable
{
    /// <summary>Snowflake del servidor (contexto) de esta conexión.</summary>
    Snowflake ServidorId { get; }

    /// <summary>
    /// Mensaje normalizado recibido del canal de eventos de este servidor, ya mapeado y filtrado
    /// (sin bots, DM ni mensajes de sistema). El gestor lo enruta al motor de moderación.
    /// </summary>
    event Func<MensajeEntrante, Task>? MensajeRecibido;

    /// <summary>
    /// Cambio de estado de conexión de este servidor (CU-13): conectado, caída transitoria o token
    /// inválido. El gestor lo refleja en el registro de estado y, si corresponde, en la base.
    /// </summary>
    event Action<EstadoConexion, MotivoEstadoConexion>? EstadoConexionCambiado;

    /// <summary>
    /// Inicia sesión con el token (descifrado en memoria, RN-14) y abre la conexión al canal de
    /// eventos. La reconexión automática la maneja el SDK; este método solo arranca la conexión.
    /// </summary>
    Task ConectarAsync(string token, CancellationToken ct = default);

    /// <summary>Cierra la conexión (al desactivar/desregistrar el servidor).</summary>
    Task DetenerAsync(CancellationToken ct = default);
}

/// <summary>
/// Fábrica de conexiones de gateway por servidor (ADR-13). Aísla la construcción del cliente del
/// SDK del gestor de conexiones para que éste sea testeable con dobles (NSubstitute).
/// </summary>
public interface IFabricaClienteGateway
{
    /// <summary>Crea una conexión nueva para el servidor indicado (aún sin conectar).</summary>
    IClienteGatewayServidor Crear(Snowflake servidorId);
}
