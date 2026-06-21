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

    public async Task<ResultadoAccion> ReportarAsync(
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
            return ResultadoAccion.Fallida;
        }

        await canal.SendMessageAsync(ComponerTextoReporte(reporte));
        return ResultadoAccion.Ejecutada;
    }

    /// <summary>
    /// Compone el texto del reporte de moderación a publicar en el canal (CU-05). Si el
    /// incidente quedó no accionable por jerarquía o permisos (RN-01), el reporte incluye la
    /// advertencia y deja constancia de que la acción no se ejecutó (CU-02 §7, TC-60).
    /// </summary>
    private static string ComponerTextoReporte(ReporteIncidente reporte)
    {
        var sb = new StringBuilder();
        sb.Append(reporte.EsSimulacion
            ? "[SIMULACIÓN] "
            : reporte.EsNoAccionable ? "[MODERACIÓN — NO ACCIONABLE] " : "[MODERACIÓN] ");
        sb.AppendLine($"Política '{reporte.NombrePolitica}' — acción {reporte.Accion}.");
        if (reporte.EsNoAccionable)
        {
            sb.AppendLine(
                "Advertencia: la acción NO se ejecutó porque el usuario tiene un rol " +
                "jerárquicamente superior o el bot carece de permisos (RN-01).");
        }

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

    public async Task<ResultadoAccion> BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default)
    {
        var guild = _cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (guild is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return ResultadoAccion.Fallida;
        }

        // La API de Discord acota el borrado a 7 días; la ventana ya llega acotada (RC-11, RN-02).
        var dias = Math.Clamp((int)Math.Round(ventanaBorrado.TotalDays), 0, 7);
        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, "baneo con borrado retroactivo",
            () => guild.AddBanAsync(ulong.Parse(usuarioId.Valor), pruneDays: dias, reason: "Ráfaga distribuida"));
    }

    public async Task<ResultadoAccion> DesbanearAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default)
    {
        var guild = _cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (guild is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return ResultadoAccion.Fallida;
        }

        // Remueve el baneo del usuario (CU-07). El desbaneo NO restaura los mensajes
        // borrados al banear (RN-11): solo levanta la prohibición de ingreso.
        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, "desbaneo",
            () => guild.RemoveBanAsync(ulong.Parse(usuarioId.Valor)));
    }

    public async Task<ResultadoAccion> AplicarTimeoutAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan duracion, CancellationToken ct = default)
    {
        // El timeout (silenciamiento) se aplica sobre el miembro del servidor; si no es un
        // miembro presente, la acción no es posible (RN-01/ADR-08).
        var miembro = ObtenerMiembro(servidorId, usuarioId);
        if (miembro is null)
        {
            return ResultadoAccion.Fallida;
        }

        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, $"timeout de {duracion.TotalMinutes} minuto(s)",
            () => miembro.SetTimeOutAsync(duracion));
    }

    public async Task<ResultadoAccion> ExpulsarAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default)
    {
        var miembro = ObtenerMiembro(servidorId, usuarioId);
        if (miembro is null)
        {
            return ResultadoAccion.Fallida;
        }

        // Expulsión (kick): no impide el reingreso, a diferencia del baneo.
        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, "expulsión", () => miembro.KickAsync("Ráfaga distribuida"));
    }

    public async Task<ResultadoAccion> AsignarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default)
    {
        var (miembro, rolGuild) = ObtenerMiembroYRol(servidorId, usuarioId, rol);
        if (miembro is null || rolGuild is null)
        {
            return ResultadoAccion.Fallida;
        }

        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, $"asignar rol {rol.Valor}", () => miembro.AddRoleAsync(rolGuild));
    }

    public async Task<ResultadoAccion> QuitarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default)
    {
        var (miembro, rolGuild) = ObtenerMiembroYRol(servidorId, usuarioId, rol);
        if (miembro is null || rolGuild is null)
        {
            return ResultadoAccion.Fallida;
        }

        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, $"quitar rol {rol.Valor}", () => miembro.RemoveRoleAsync(rolGuild));
    }

    private SocketGuildUser? ObtenerMiembro(Snowflake servidorId, Snowflake usuarioId)
    {
        var guild = _cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (guild is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return null;
        }

        var miembro = guild.GetUser(ulong.Parse(usuarioId.Valor));
        if (miembro is null)
        {
            _logger.LogWarning(
                "Usuario {Usuario} no es un miembro presente del servidor {Servidor}.",
                usuarioId.Valor, servidorId.Valor);
        }

        return miembro;
    }

    private (SocketGuildUser? Miembro, IRole? Rol) ObtenerMiembroYRol(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol)
    {
        var guild = _cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (guild is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return (null, null);
        }

        var miembro = guild.GetUser(ulong.Parse(usuarioId.Valor));
        var rolGuild = guild.GetRole(ulong.Parse(rol.Valor));
        return (miembro, rolGuild);
    }

    /// <summary>
    /// Ejecuta una acción de contención contra la plataforma y mapea las fallas esperables a un
    /// <see cref="ResultadoAccion"/> sin propagar excepción (RN-01, ADR-08). La plataforma rechaza
    /// con error de permisos (403/Forbidden) cuando el bot no tiene el permiso o el usuario tiene
    /// rol superior; ese caso se clasifica como no accionable y no abota el pipeline.
    /// </summary>
    private async Task<ResultadoAccion> EjecutarContraPlataformaAsync(
        Snowflake servidorId, Snowflake usuarioId, string descripcion, Func<Task> accion)
    {
        try
        {
            await accion();
            return ResultadoAccion.Ejecutada;
        }
        catch (Discord.Net.HttpException ex) when (ex.HttpCode == System.Net.HttpStatusCode.Forbidden)
        {
            // Jerarquía superior o permisos faltantes: la plataforma responde 403 (RN-01, CU-02 §7).
            _logger.LogWarning(
                ex,
                "{Descripcion} sobre usuario {Usuario} en servidor {Servidor} NO accionable por " +
                "jerarquía o permisos (HTTP 403); no se aborta el pipeline (RN-01, ADR-08).",
                descripcion, usuarioId.Valor, servidorId.Valor);
            return ResultadoAccion.NoAccionablePorPermisos;
        }
        catch (Exception ex)
        {
            // Otro fallo de plataforma: se clasifica como fallida y se reporta (ADR-08).
            _logger.LogWarning(
                ex,
                "{Descripcion} sobre usuario {Usuario} en servidor {Servidor} falló en la plataforma; " +
                "se registra como acción fallida y continúa el pipeline (ADR-08).",
                descripcion, usuarioId.Valor, servidorId.Valor);
            return ResultadoAccion.Fallida;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cliente.MessageReceived -= OnMessageReceivedAsync;
        await _cliente.DisposeAsync();
    }
}
