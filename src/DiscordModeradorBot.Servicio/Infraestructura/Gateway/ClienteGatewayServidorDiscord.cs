using Discord;
using Discord.WebSocket;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Gateway;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Conexión de gateway de UN servidor con Discord.Net (ADR-13: una conexión por contexto). Wrappea
/// un <see cref="DiscordSocketClient"/> autenticado con el token del servidor (descifrado en
/// memoria, RN-14), suscribe la recepción de mensajes y los mapea a <see cref="MensajeEntrante"/>
/// (delegando el mapeo y el descarte en <see cref="MapeadorMensajeGremio"/>, testeable y sin SDK),
/// e informa los cambios de estado de conexión (CU-13). La reconexión automática la maneja el SDK;
/// este wrapper solo observa el estado y lo reporta. El token nunca se loguea (RN-14).
///
/// Expone su <see cref="DiscordSocketClient"/> subyacente al adaptador de acciones
/// (<see cref="AdaptadorGatewayDiscord"/>) para ejecutar las operaciones REST sobre el contexto
/// correcto (banear, timeout, etc.).
/// </summary>
public sealed class ClienteGatewayServidorDiscord : IClienteGatewayServidor
{
    private readonly DiscordSocketClient _cliente;
    private readonly ILogger _logger;

    public ClienteGatewayServidorDiscord(Snowflake servidorId, ILogger logger)
    {
        ServidorId = servidorId;
        _logger = logger;

        _cliente = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = IntentsGateway.Requeridos,
            // La reconexión automática del SDK queda habilitada por defecto (CU-13).
            MessageCacheSize = 0,
            AlwaysDownloadUsers = false,
        });

        _cliente.MessageReceived += OnMessageReceivedAsync;
        _cliente.Connected += OnConnectedAsync;
        _cliente.Disconnected += OnDisconnectedAsync;
        _cliente.Log += RegistrarLogDiscord;
    }

    /// <summary>
    /// Reenvía los logs internos de Discord.Net a nuestro logger, mapeando la severidad, para ver el
    /// motivo exacto del gateway (p. ej. close code 4014 "Disallowed intent(s)") al diagnosticar (CU-13).
    /// </summary>
    private Task RegistrarLogDiscord(LogMessage mensaje)
    {
        var nivel = mensaje.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            _ => LogLevel.Trace,
        };

        _logger.Log(nivel, mensaje.Exception, "[Discord.Net:{Fuente}] {Mensaje}", mensaje.Source, mensaje.Message);
        return Task.CompletedTask;
    }

    public Snowflake ServidorId { get; }

    public event Func<MensajeEntrante, Task>? MensajeRecibido;

    public event Action<EstadoConexion, MotivoEstadoConexion>? EstadoConexionCambiado;

    /// <summary>Cliente del SDK subyacente, usado por el adaptador de acciones sobre este contexto.</summary>
    internal DiscordSocketClient Cliente => _cliente;

    public async Task ConectarAsync(string token, CancellationToken ct = default)
    {
        // El token vive solo en memoria para autenticar; jamás se loguea (RN-14, RC-07).
        await _cliente.LoginAsync(TokenType.Bot, token);
        await _cliente.StartAsync();
    }

    public async Task DetenerAsync(CancellationToken ct = default)
    {
        await _cliente.StopAsync();
        await _cliente.LogoutAsync();
    }

    private Task OnConnectedAsync()
    {
        EstadoConexionCambiado?.Invoke(EstadoConexion.Conectado, MotivoEstadoConexion.Conectado);
        return Task.CompletedTask;
    }

    private Task OnDisconnectedAsync(Exception? ex)
    {
        // Una credencial revocada/vencida/incorrecta deja el servidor desconectado por token
        // inválido y requiere re-validar (CU-13 CA-03, CONEXION_TOKEN_INVALIDO); cualquier otra
        // caída es transitoria y el SDK reintenta (CU-13 CA-01).
        var tokenInvalido =
            ex is Discord.Net.HttpException { HttpCode: System.Net.HttpStatusCode.Unauthorized }
            || ClasificadorFallaGateway.EsSenalDeTokenInvalido(ex?.Message);

        var motivo = tokenInvalido
            ? MotivoEstadoConexion.DesconectadoTokenInvalido
            : MotivoEstadoConexion.DesconectadoTransitorio;

        EstadoConexionCambiado?.Invoke(EstadoConexion.Desconectado, motivo);

        // Un token inválido NO se arregla reintentando: el SDK reintentaría en bucle (401 en
        // /gateway/bot), saturando el log y desestabilizando el host. Se detiene la conexión para
        // cortar el bucle; el servidor queda desconectado hasta re-validar el token (CU-13 CA-03).
        // Se hace fire-and-forget para no reentrar al pipeline de eventos del SDK.
        if (tokenInvalido)
        {
            _ = DetenerPorTokenInvalidoAsync();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Detiene la conexión tras un token inválido para frenar el bucle de reconexión del SDK. Se
    /// invoca desde el manejador de desconexión (fire-and-forget); cualquier error al detener se
    /// ignora porque la conexión ya está caída.
    /// </summary>
    private async Task DetenerPorTokenInvalidoAsync()
    {
        try
        {
            await _cliente.StopAsync();
            _logger.LogWarning(
                "[GATEWAY] Servidor {Servidor}: token inválido; se detuvo la reconexión. " +
                "Requiere re-validar el token (CU-13 CA-03).",
                ServidorId.Valor);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[GATEWAY] Error al detener la conexión tras token inválido (se ignora).");
        }
    }

    private async Task OnMessageReceivedAsync(SocketMessage socketMessage)
    {
        var datos = ExtraerDatos(socketMessage);

        // El mapeo y el descarte (bots, DM, sistema, snowflakes inválidos) viven en una función
        // pura testeable (RN-07, RN-08); si el mensaje se descarta, no se entrega al motor.
        var entrante = MapeadorMensajeGremio.Mapear(datos);
        if (entrante is null)
        {
            return;
        }

        var handler = MensajeRecibido;
        if (handler is not null)
        {
            await handler(entrante);
        }
    }

    /// <summary>
    /// Extrae los datos del mensaje del tipo del SDK a un DTO ABSTRACTO (sin SDK) para que el mapeo
    /// y el descarte sean testeables. El rol universal @everyone (Id == Guild.Id) se omite por ser
    /// universal (RN-08).
    /// </summary>
    private static DatosMensajeGremio ExtraerDatos(SocketMessage socketMessage)
    {
        var esDeGremio = socketMessage.Channel is SocketGuildChannel;
        var esSistema = socketMessage is not SocketUserMessage;
        var canalGremio = socketMessage.Channel as SocketGuildChannel;
        var servidorId = canalGremio?.Guild.Id.ToString() ?? string.Empty;

        var roles = socketMessage.Author is SocketGuildUser miembro && canalGremio is not null
            ? miembro.Roles
                .Where(r => r.Id != canalGremio.Guild.Id)
                .Select(r => r.Id.ToString())
                .ToArray()
            : [];

        return new DatosMensajeGremio(
            ServidorId: servidorId,
            CanalId: socketMessage.Channel.Id.ToString(),
            UsuarioId: socketMessage.Author.Id.ToString(),
            MensajeId: socketMessage.Id.ToString(),
            Instante: socketMessage.Timestamp,
            Contenido: socketMessage.Content ?? string.Empty,
            EsDeGremio: esDeGremio,
            AutorEsBot: socketMessage.Author.IsBot,
            EsMensajeDeSistema: esSistema,
            RolesDelAutor: roles)
        {
            // Nombre legible del canal (CU-06); el canal del SDK siempre lo expone. Para un mensaje
            // que no es de gremio queda vacío y la UI cae al snowflake.
            NombreCanal = canalGremio?.Name ?? socketMessage.Channel.Name ?? string.Empty,
        };
    }

    public async ValueTask DisposeAsync()
    {
        _cliente.MessageReceived -= OnMessageReceivedAsync;
        _cliente.Connected -= OnConnectedAsync;
        _cliente.Disconnected -= OnDisconnectedAsync;
        await _cliente.DisposeAsync();
    }
}
