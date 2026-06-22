using System.Collections.Concurrent;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Gateway;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Adaptador REAL del gateway con Discord.Net (ADR-13, intake §17 P.1/P.3). Cumple dos roles:
/// <list type="number">
///   <item>
///     <b>Fábrica/registro de conexiones por servidor</b> (<see cref="IFabricaClienteGateway"/>):
///     crea un <see cref="ClienteGatewayServidorDiscord"/> por contexto (una conexión por servidor,
///     ADR-13) y lo conserva para poder ejecutar acciones REST sobre el contexto correcto.
///   </item>
///   <item>
///     <b>Operaciones de moderación</b> (<see cref="IAdaptadorGateway"/>): mapea cada acción del
///     catálogo a la operación REST de Discord.Net y a un <see cref="ResultadoAccion"/>, sin
///     propagar excepción al pipeline (ADR-08). La clasificación de la falla (jerarquía/permisos vs
///     falla de plataforma) se hace con lógica pura testeable (<see cref="ClasificadorResultadoAccion"/>),
///     traduciendo la excepción del SDK con <see cref="TraductorFallaDiscord"/>.
///   </item>
/// </list>
/// La recepción de mensajes se enruta a través del gestor de conexiones
/// (<see cref="DiscordModeradorBot.Servicio.Aplicacion.GestorConexionesGateway"/>), no por el evento
/// <see cref="MensajeRecibido"/> de este adaptador (presente por compatibilidad con el puerto). El
/// token nunca se loguea (RN-14).
/// </summary>
public sealed class AdaptadorGatewayDiscord : IAdaptadorGateway, IFabricaClienteGateway, IAsyncDisposable
{
    private readonly ILogger<AdaptadorGatewayDiscord> _logger;
    private readonly ConcurrentDictionary<string, ClienteGatewayServidorDiscord> _clientes = new();

    public AdaptadorGatewayDiscord(ILogger<AdaptadorGatewayDiscord> logger) => _logger = logger;

    /// <summary>
    /// Reenvía los logs internos de Discord.Net a nuestro logger, mapeando la severidad. Permite ver
    /// el motivo exacto del gateway —p. ej. el close code 4014 "Disallowed intent(s)" cuando faltan
    /// los intents privilegiados— sin tener que adivinar (diagnóstico de CU-12/CU-13).
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

    /// <summary>
    /// Compatibilidad con el puerto: el adaptador real enruta los mensajes vía el gestor de
    /// conexiones (<see cref="ClienteGatewayServidorDiscord"/> → <c>GestorConexionesGateway</c>), así
    /// que este evento del puerto no se usa en el camino real (queda sin suscriptores).
    /// </summary>
#pragma warning disable CS0067 // Evento requerido por el puerto; el ruteo real va por el gestor.
    public event Func<MensajeEntrante, Task>? MensajeRecibido;
#pragma warning restore CS0067

    public IClienteGatewayServidor Crear(Snowflake servidorId)
    {
        var cliente = new ClienteGatewayServidorDiscord(servidorId, _logger);
        _clientes[servidorId.Valor] = cliente;
        return cliente;
    }

    /// <summary>
    /// Prueba la configuración de un servidor contra la plataforma (CU-12, RN-16): valida el token
    /// (login efímero, RN-14), los intents requeridos, los permisos del bot (banear/expulsar/
    /// moderar/gestionar roles/escribir en el canal de salida), la existencia del canal de salida y
    /// la jerarquía del rol del bot. Devuelve los chequeos con su severidad (bloqueante/advertencia)
    /// que espera <see cref="ServicioPruebaConfiguracion"/>. El cliente efímero se cierra siempre.
    /// </summary>
    public async Task<ResultadoPruebaConfiguracion> ProbarConfiguracionAsync(
        SolicitudPruebaConfiguracion solicitud, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(solicitud);

        var chequeos = new List<ChequeoConfiguracion>();
        var cliente = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = IntentsGateway.Requeridos,
        });
        cliente.Log += RegistrarLogDiscord;

        try
        {
            // 1) Token: login efímero. Un token inválido es bloqueante (CU-12 CA-03, RN-14).
            try
            {
                await cliente.LoginAsync(TokenType.Bot, solicitud.TokenEnClaro);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Prueba de configuración del servidor {Servidor}: el token no validó " +
                    "(PRUEBA_TOKEN_INVALIDO, bloqueante, CU-12, RN-16).",
                    solicitud.ServidorId.Valor);
                chequeos.Add(ChequeoConfiguracion.Bloqueante(
                    ResultadoPruebaConfiguracion.CodigoTokenInvalido,
                    "Credencial válida",
                    "El token no fue aceptado por la plataforma."));
                return new ResultadoPruebaConfiguracion(chequeos);
            }

            chequeos.Add(ChequeoConfiguracion.Superado(
                ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida"));

            // Se arranca el gateway para que el SDK sincronice el guild y permita leer permisos y
            // jerarquía. La sincronización es asíncrona: se espera la presencia del guild con tope.
            await cliente.StartAsync();
            var guild = await EsperarGuildAsync(cliente, solicitud.ServidorId, ct);

            // 2) Intents privilegiados habilitados (MessageContent, GuildMembers): sin ellos no se
            // recibe contenido ni roles (CU-12). Si la conexión llegó a sincronizar, los intents
            // solicitados fueron aceptados; si el portal los tiene deshabilitados, la conexión no
            // sincroniza y este chequeo queda bloqueante.
            chequeos.Add(guild is not null
                ? ChequeoConfiguracion.Superado(
                    ResultadoPruebaConfiguracion.CodigoIntentsFaltantes, "Intents habilitados")
                : ChequeoConfiguracion.Bloqueante(
                    ResultadoPruebaConfiguracion.CodigoIntentsFaltantes,
                    "Intents habilitados",
                    "El gateway no sincronizó el servidor; verificá los intents privilegiados " +
                    "(MessageContent, GuildMembers) en el portal de desarrolladores."));

            // 3) Recepción de eventos: el bot debe estar presente en el guild.
            if (guild is null)
            {
                chequeos.Add(ChequeoConfiguracion.Bloqueante(
                    ResultadoPruebaConfiguracion.CodigoRecepcionEventos,
                    "Recepción de eventos",
                    "El bot no está presente en el servidor o el gateway no terminó de sincronizar."));
                return new ResultadoPruebaConfiguracion(chequeos);
            }

            chequeos.Add(ChequeoConfiguracion.Superado(
                ResultadoPruebaConfiguracion.CodigoRecepcionEventos, "Recepción de eventos"));

            // 4) Permisos requeridos del bot (banear, expulsar, moderar/timeout, gestionar roles).
            var permisos = guild.CurrentUser.GuildPermissions;
            var faltantes = new List<string>();
            if (!permisos.BanMembers)
            {
                faltantes.Add("Banear miembros");
            }

            if (!permisos.KickMembers)
            {
                faltantes.Add("Expulsar miembros");
            }

            if (!permisos.ModerateMembers)
            {
                faltantes.Add("Moderar miembros (timeout)");
            }

            if (!permisos.ManageRoles)
            {
                faltantes.Add("Gestionar roles");
            }

            chequeos.Add(faltantes.Count == 0
                ? ChequeoConfiguracion.Superado(
                    ResultadoPruebaConfiguracion.CodigoPermisosFaltantes, "Permisos requeridos presentes")
                : ChequeoConfiguracion.Bloqueante(
                    ResultadoPruebaConfiguracion.CodigoPermisosFaltantes,
                    "Permisos requeridos presentes",
                    $"Faltan permisos: {string.Join(", ", faltantes)}."));

            // 5) Canal de salida designado, si lo hay: debe existir, ser de mensajes y el bot poder
            // escribir en él (CU-05, CU-12 PRUEBA_CANAL_SALIDA_AUSENTE).
            if (solicitud.CanalDeSalida is { } canalSalida)
            {
                var canal = guild.GetChannel(ulong.Parse(canalSalida.SnowflakeCanal.Valor));
                if (canal is not IMessageChannel)
                {
                    chequeos.Add(ChequeoConfiguracion.Bloqueante(
                        ResultadoPruebaConfiguracion.CodigoCanalSalidaAusente,
                        "Canal de salida disponible",
                        "El canal de salida designado no existe o el bot no puede verlo."));
                }
                else if (!guild.CurrentUser.GetPermissions(canal).SendMessages)
                {
                    chequeos.Add(ChequeoConfiguracion.Bloqueante(
                        ResultadoPruebaConfiguracion.CodigoCanalSalidaAusente,
                        "Canal de salida disponible",
                        "El bot no tiene permiso para escribir en el canal de salida designado."));
                }
                else
                {
                    chequeos.Add(ChequeoConfiguracion.Superado(
                        ResultadoPruebaConfiguracion.CodigoCanalSalidaAusente, "Canal de salida disponible"));
                }
            }

            // 6) Jerarquía de roles: ADVERTENCIA (no bloquea) si hay roles por encima del rol más
            // alto del bot (RN-01, CU-12 CA-04): no podrá accionar sobre quienes los porten.
            var posicionBot = guild.CurrentUser.Hierarchy;
            var rolesPorEncima = guild.Roles.Count(
                r => r.Position >= posicionBot && r.Id != guild.EveryoneRole.Id);
            chequeos.Add(rolesPorEncima > 0
                ? ChequeoConfiguracion.Advertencia(
                    ResultadoPruebaConfiguracion.CodigoJerarquiaInsuficiente,
                    "Jerarquía de roles suficiente",
                    $"Hay {rolesPorEncima} rol(es) por encima del bot; no podrá accionar sobre sus " +
                    "portadores.")
                : ChequeoConfiguracion.Superado(
                    ResultadoPruebaConfiguracion.CodigoJerarquiaInsuficiente, "Jerarquía de roles suficiente"));

            return new ResultadoPruebaConfiguracion(chequeos);
        }
        finally
        {
            // El cliente efímero de la prueba se cierra siempre (el token solo vivió en memoria, RN-14).
            await cliente.StopAsync();
            await cliente.DisposeAsync();
        }
    }

    /// <summary>
    /// Espera hasta que el SDK sincronice el guild del servidor o se agote un tope corto. El gateway
    /// sincroniza de forma asíncrona; sin esta espera el guild podría no estar disponible al leer.
    /// </summary>
    private static async Task<SocketGuild?> EsperarGuildAsync(
        DiscordSocketClient cliente, Snowflake servidorId, CancellationToken ct)
    {
        if (!ulong.TryParse(servidorId.Valor, out var idGuild))
        {
            return null;
        }

        var limite = DateTimeOffset.UtcNow.AddSeconds(15);
        while (DateTimeOffset.UtcNow < limite && !ct.IsCancellationRequested)
        {
            var guild = cliente.GetGuild(idGuild);
            if (guild is not null && guild.IsConnected && guild.CurrentUser is not null)
            {
                return guild;
            }

            await Task.Delay(250, ct);
        }

        return cliente.GetGuild(idGuild);
    }

    public async Task<ResultadoAccion> EnviarMensajePruebaAsync(
        SolicitudPruebaConfiguracion solicitud, string mensaje, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(solicitud);

        if (solicitud.CanalDeSalida is not { } canalSalida)
        {
            return ResultadoAccion.Fallida;
        }

        // Conexión EFÍMERA dedicada (como la prueba de configuración): no requiere que el servidor
        // esté activado. El token solo vive en memoria y el cliente se cierra siempre (RN-14).
        var cliente = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = IntentsGateway.Requeridos,
        });
        cliente.Log += RegistrarLogDiscord;

        try
        {
            try
            {
                await cliente.LoginAsync(TokenType.Bot, solicitud.TokenEnClaro);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, "Mensaje de prueba: el token del servidor {Servidor} no validó.",
                    solicitud.ServidorId.Valor);
                return ResultadoAccion.Fallida;
            }

            await cliente.StartAsync();
            var guild = await EsperarGuildAsync(cliente, solicitud.ServidorId, ct);
            if (guild is null)
            {
                return ResultadoAccion.Fallida;
            }

            var canalSocket = guild.GetChannel(ulong.Parse(canalSalida.SnowflakeCanal.Valor));
            if (canalSocket is not IMessageChannel canalMensaje)
            {
                return ResultadoAccion.Fallida;
            }

            if (!guild.CurrentUser.GetPermissions(canalSocket).SendMessages)
            {
                return ResultadoAccion.NoAccionablePorPermisos;
            }

            try
            {
                await canalMensaje.SendMessageAsync(mensaje);
                _logger.LogInformation(
                    "Mensaje de prueba publicado en el canal {Canal} del servidor {Servidor} (CU-05).",
                    canalSalida.SnowflakeCanal.Valor, solicitud.ServidorId.Valor);
                return ResultadoAccion.Ejecutada;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, "Mensaje de prueba: fallo al publicar en el canal {Canal} del servidor {Servidor}.",
                    canalSalida.SnowflakeCanal.Valor, solicitud.ServidorId.Valor);
                return ResultadoAccion.Fallida;
            }
        }
        finally
        {
            await cliente.StopAsync();
            await cliente.DisposeAsync();
        }
    }

    public async Task<ResultadoAccion> ReportarAsync(
        CanalDeSalida canalSalida, ReporteIncidente reporte, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(canalSalida);
        ArgumentNullException.ThrowIfNull(reporte);

        if (!TryObtenerGuild(reporte.ServidorId, out var guild) ||
            guild.GetChannel(ulong.Parse(canalSalida.SnowflakeCanal.Valor)) is not IMessageChannel canal)
        {
            _logger.LogWarning(
                "Canal de salida {Canal} no disponible en el servidor {Servidor}; el reporte no se " +
                "publicó (REPORTE_CANAL_NO_DESIGNADO).",
                canalSalida.SnowflakeCanal.Valor, reporte.ServidorId.Valor);
            return ResultadoAccion.Fallida;
        }

        return await EjecutarContraPlataformaAsync(
            reporte.ServidorId, reporte.UsuarioId, "reporte al canal de salida",
            () => canal.SendMessageAsync(ComponerTextoReporte(reporte)));
    }

    /// <summary>
    /// Compone el texto del reporte a publicar en el canal (CU-05). Si el incidente quedó no
    /// accionable por jerarquía o permisos (RN-01), el reporte incluye la advertencia (CU-02 §7,
    /// TC-60). Nunca incluye el token ni datos sensibles.
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
        if (!TryObtenerGuild(servidorId, out var guild))
        {
            return ResultadoAccion.Fallida;
        }

        // La API de Discord acota el borrado a 7 días; la ventana ya llega acotada (RC-11, RN-02).
        var dias = Math.Clamp((int)Math.Round(ventanaBorrado.TotalDays), 0, 7);
        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, "baneo con borrado retroactivo",
            () => guild.AddBanAsync(
                ulong.Parse(usuarioId.Valor), pruneDays: dias, reason: "Ráfaga distribuida"));
    }

    public async Task<ResultadoAccion> DesbanearAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default)
    {
        if (!TryObtenerGuild(servidorId, out var guild))
        {
            return ResultadoAccion.Fallida;
        }

        // El desbaneo NO restaura los mensajes borrados al banear (RN-11): solo levanta el baneo.
        return await EjecutarContraPlataformaAsync(
            servidorId, usuarioId, "desbaneo",
            () => guild.RemoveBanAsync(ulong.Parse(usuarioId.Valor)));
    }

    public async Task<ResultadoAccion> AplicarTimeoutAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan duracion, CancellationToken ct = default)
    {
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

    private bool TryObtenerGuild(Snowflake servidorId, out SocketGuild guild)
    {
        guild = null!;
        if (!_clientes.TryGetValue(servidorId.Valor, out var cliente))
        {
            _logger.LogWarning(
                "Servidor {Servidor} sin conexión de gateway activa; no se puede accionar.",
                servidorId.Valor);
            return false;
        }

        var resuelto = cliente.Cliente.GetGuild(ulong.Parse(servidorId.Valor));
        if (resuelto is null)
        {
            _logger.LogWarning("Servidor {Servidor} no disponible en el gateway.", servidorId.Valor);
            return false;
        }

        guild = resuelto;
        return true;
    }

    private SocketGuildUser? ObtenerMiembro(Snowflake servidorId, Snowflake usuarioId)
    {
        if (!TryObtenerGuild(servidorId, out var guild))
        {
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
        if (!TryObtenerGuild(servidorId, out var guild))
        {
            return (null, null);
        }

        var miembro = guild.GetUser(ulong.Parse(usuarioId.Valor));
        var rolGuild = guild.GetRole(ulong.Parse(rol.Valor));
        return (miembro, rolGuild);
    }

    /// <summary>
    /// Ejecuta una operación contra la plataforma y mapea la falla a un <see cref="ResultadoAccion"/>
    /// sin propagar excepción (RN-01, ADR-08). La naturaleza de la falla se traduce del SDK
    /// (<see cref="TraductorFallaDiscord"/>) y se clasifica con lógica pura testeable
    /// (<see cref="ClasificadorResultadoAccion"/>): jerarquía/permisos → no accionable; otras →
    /// fallida.
    /// </summary>
    private async Task<ResultadoAccion> EjecutarContraPlataformaAsync(
        Snowflake servidorId, Snowflake usuarioId, string descripcion, Func<Task> accion)
    {
        try
        {
            await accion();
            return ResultadoAccion.Ejecutada;
        }
        catch (Exception ex)
        {
            var falla = TraductorFallaDiscord.Traducir(ex);
            var resultado = ClasificadorResultadoAccion.Clasificar(falla);

            if (resultado.EsNoAccionable())
            {
                _logger.LogWarning(
                    ex,
                    "{Descripcion} sobre usuario {Usuario} en servidor {Servidor} NO accionable " +
                    "({Resultado}); no se aborta el pipeline (RN-01, ADR-08).",
                    descripcion, usuarioId.Valor, servidorId.Valor, resultado);
            }
            else
            {
                _logger.LogWarning(
                    ex,
                    "{Descripcion} sobre usuario {Usuario} en servidor {Servidor} falló en la " +
                    "plataforma; se registra como fallida y continúa el pipeline (ADR-08).",
                    descripcion, usuarioId.Valor, servidorId.Valor);
            }

            return resultado;
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var cliente in _clientes.Values)
        {
            await cliente.DisposeAsync();
        }

        _clientes.Clear();
    }
}
