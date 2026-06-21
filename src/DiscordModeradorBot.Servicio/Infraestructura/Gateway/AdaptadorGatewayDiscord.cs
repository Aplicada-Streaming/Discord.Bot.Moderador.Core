using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Adaptador del gateway con Discord.Net (ADR-13, BT-09). Implementación mínima
/// estructural: conecta con un token, mapea los mensajes del gateway a
/// <see cref="MensajeEntrante"/>, publica el reporte del incidente en un canal de salida
/// (CU-05) y ejecuta el baneo con borrado retroactivo contra la API REST (CU-02/CU-03). NO
/// se ejercita en tests (requiere token real) y NO se registra por defecto; el adaptador
/// activo es <see cref="AdaptadorGatewaySimulado"/>. Queda compilando como estructura para
/// conectar la integración real en rebanadas posteriores.
/// </summary>
public sealed class AdaptadorGatewayDiscord : IAdaptadorGateway, IAsyncDisposable
{
    private readonly DiscordSocketClient _cliente;
    private readonly ILogger<AdaptadorGatewayDiscord> _logger;

    public AdaptadorGatewayDiscord(ILogger<AdaptadorGatewayDiscord> logger)
    {
        _logger = logger;

        var config = new DiscordSocketConfig
        {
            // Intents mínimos para recibir el contenido de los mensajes de los servidores.
            GatewayIntents = GatewayIntents.Guilds
                | GatewayIntents.GuildMessages
                | GatewayIntents.MessageContent,
        };

        _cliente = new DiscordSocketClient(config);
        _cliente.MessageReceived += OnMessageReceivedAsync;
    }

    public event Func<MensajeEntrante, Task>? MensajeRecibido;

    /// <summary>Conecta el cliente al gateway con el token (descifrado en memoria, ADR-07).</summary>
    public async Task ConectarAsync(string token)
    {
        await _cliente.LoginAsync(TokenType.Bot, token);
        await _cliente.StartAsync();
    }

    private async Task OnMessageReceivedAsync(SocketMessage socketMessage)
    {
        // Solo mensajes de usuario en un servidor (guild). Se ignoran bots y mensajes directos.
        if (socketMessage is not SocketUserMessage mensaje ||
            mensaje.Channel is not SocketGuildChannel canal ||
            mensaje.Author.IsBot)
        {
            return;
        }

        // Roles del autor para evaluar exenciones por rol (CU-15, RN-07). Cuando el autor es un
        // miembro del servidor (SocketGuildUser) se mapean sus roles a snowflakes como texto
        // (RN-08); si no está disponible, el conjunto queda vacío (default) y el mensaje nunca
        // queda exento por rol. El rol @everyone (Id == Guild.Id) se omite por ser universal.
        var rolesAutor = mensaje.Author is SocketGuildUser miembro
            ? miembro.Roles
                .Where(r => r.Id != canal.Guild.Id)
                .Select(r => new Snowflake(r.Id.ToString()))
                .ToArray()
            : [];

        var entrante = new MensajeEntrante(
            new Snowflake(canal.Guild.Id.ToString()),
            new Snowflake(canal.Id.ToString()),
            new Snowflake(mensaje.Author.Id.ToString()),
            new Snowflake(mensaje.Id.ToString()),
            mensaje.Timestamp,
            mensaje.Content ?? string.Empty)
        {
            RolesDelAutor = rolesAutor,
        };

        var handler = MensajeRecibido;
        if (handler is not null)
        {
            await handler(entrante);
        }
    }

    public async Task ReportarAsync(
        CanalDeSalida canalSalida, ReporteIncidente reporte, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(canalSalida);
        ArgumentNullException.ThrowIfNull(reporte);

        // El canal de salida se referencia por su snowflake (RN-08); el propósito lógico lo
        // resuelve la capa de aplicación (CU-05). Se publica un mensaje de texto con la
        // evidencia copiada antes de cualquier borrado (RN-11).
        if (_cliente.GetChannel(ulong.Parse(canalSalida.SnowflakeCanal.Valor)) is not IMessageChannel canal)
        {
            _logger.LogWarning(
                "Canal de salida {Canal} no disponible en el gateway; el reporte no se publicó.",
                canalSalida.SnowflakeCanal.Valor);
            return;
        }

        await canal.SendMessageAsync(ComponerTextoReporte(reporte));
    }

    /// <summary>Compone el texto del reporte de moderación a publicar en el canal (CU-05).</summary>
    private static string ComponerTextoReporte(ReporteIncidente reporte)
    {
        var sb = new StringBuilder();
        sb.Append(reporte.EsSimulacion ? "[SIMULACIÓN] " : "[MODERACIÓN] ");
        sb.AppendLine($"Política '{reporte.NombrePolitica}' — acción {reporte.Accion}.");
        sb.AppendLine($"Emisor: {reporte.UsuarioId.Valor}");
        sb.AppendLine(
            $"Canales afectados: {string.Join(", ", reporte.CanalesAfectados.Select(c => c.Valor))}");
        sb.AppendLine("Mensajes que dispararon la acción:");
        foreach (var mensaje in reporte.MensajesAccionados)
        {
            sb.AppendLine($"- [{mensaje.CanalId.Valor}] {mensaje.ContenidoCopiado}");
        }

        return sb.ToString();
    }

    public async Task BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default)
    {
        var guild = _cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (guild is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return;
        }

        // La API de Discord acota el borrado a 7 días; la ventana ya llega acotada (RC-11, RN-02).
        var dias = Math.Clamp((int)Math.Round(ventanaBorrado.TotalDays), 0, 7);
        await guild.AddBanAsync(ulong.Parse(usuarioId.Valor), pruneDays: dias, reason: "Ráfaga distribuida");
    }

    public async Task DesbanearAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default)
    {
        var guild = _cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (guild is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return;
        }

        // Remueve el baneo del usuario (CU-07). El desbaneo NO restaura los mensajes
        // borrados al banear (RN-11): solo levanta la prohibición de ingreso.
        await guild.RemoveBanAsync(ulong.Parse(usuarioId.Valor));
    }

    public async ValueTask DisposeAsync()
    {
        _cliente.MessageReceived -= OnMessageReceivedAsync;
        await _cliente.DisposeAsync();
    }
}
