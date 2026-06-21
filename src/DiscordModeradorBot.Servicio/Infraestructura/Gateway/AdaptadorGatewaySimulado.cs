using System.Collections.Concurrent;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Adaptador del gateway simulado (intake §18 sample a/b, ADR-04). Es el adaptador activo
/// fuera de la integración real: permite inyectar mensajes simulados para el walking
/// skeleton y los tests, y REGISTRA (loguea) las acciones ejecutadas — reporte, baneo con
/// borrado retroactivo, timeout, expulsión y gestión de roles (R6) — en vez de llamar a la
/// plataforma real. No usa red ni token real. Conserva las acciones ejecutadas en memoria
/// para verificación y demo.
///
/// R6 también permite SIMULAR el caso "no accionable" (RN-01, CU-02 §7): un usuario marcado
/// como de rol superior (<see cref="MarcarUsuarioDeRolSuperior"/>) hace que las acciones de
/// contención devuelvan <see cref="ResultadoAccion.NoAccionablePorJerarquia"/> en vez de
/// ejecutarse, para demostrar que el motor registra el incidente como NoAccionable, igual
/// reporta y no se cae (ADR-08).
/// </summary>
public sealed class AdaptadorGatewaySimulado : IAdaptadorGateway
{
    private readonly ILogger<AdaptadorGatewaySimulado> _logger;
    private readonly List<AccionEjecutada> _accionesEjecutadas = new();
    private readonly object _candado = new();

    // Usuarios marcados como de rol jerárquicamente superior al bot: las acciones de contención
    // sobre ellos no son accionables (RN-01, CU-02 §7). Particionado por (servidor, usuario).
    private readonly ConcurrentDictionary<(string Servidor, string Usuario), byte> _rolSuperior = new();

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

    /// <summary>Timeout (silenciamiento) ejecutado por una duración (R6).</summary>
    public sealed record TimeoutEjecutado(
        Snowflake ServidorId, Snowflake UsuarioId, TimeSpan Duracion) : AccionEjecutada;

    /// <summary>Expulsión (kick) ejecutada (R6).</summary>
    public sealed record ExpulsionEjecutada(Snowflake ServidorId, Snowflake UsuarioId) : AccionEjecutada;

    /// <summary>Asignación de rol ejecutada (R6).</summary>
    public sealed record RolAsignado(Snowflake ServidorId, Snowflake UsuarioId, Snowflake Rol) : AccionEjecutada;

    /// <summary>Quita de rol ejecutada (R6).</summary>
    public sealed record RolQuitado(Snowflake ServidorId, Snowflake UsuarioId, Snowflake Rol) : AccionEjecutada;

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
    /// Marca a un usuario como de rol jerárquicamente superior al bot en un servidor (R6, RN-01):
    /// las acciones de contención sobre él devolverán no accionable por jerarquía (CU-02 §7).
    /// </summary>
    public void MarcarUsuarioDeRolSuperior(Snowflake servidorId, Snowflake usuarioId) =>
        _rolSuperior[(servidorId.Valor, usuarioId.Valor)] = 1;

    /// <summary>
    /// Inyecta un mensaje simulado, como si lo hubiera entregado el canal de eventos. El mensaje
    /// puede traer los roles del autor (R5) para los escenarios de exención por rol (CU-15).
    /// </summary>
    public async Task InyectarMensajeAsync(MensajeEntrante mensaje)
    {
        var handler = MensajeRecibido;
        if (handler is not null)
        {
            await handler(mensaje);
        }
    }

    public Task<ResultadoAccion> ReportarAsync(
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
            "política '{Politica}': emisor {Usuario}, acción {Accion}, resultado {Resultado}, canales " +
            "afectados [{Canales}], mensajes accionados [{Mensajes}].",
            reporte.EsSimulacion ? "(SIMULACIÓN)" : "(EJECUCIÓN)",
            canalSalida.SnowflakeCanal.Valor, canalSalida.PropositoLogico, reporte.NombrePolitica,
            reporte.UsuarioId.Valor, reporte.Accion, reporte.Resultado, canales, mensajes);

        // El reporte no es una acción de contención sobre el usuario: siempre se publica (RN-11).
        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    public Task<ResultadoAccion> BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default)
    {
        if (NoAccionable(servidorId, usuarioId, "baneo con borrado retroactivo") is { } noAccionable)
        {
            return Task.FromResult(noAccionable);
        }

        lock (_candado)
        {
            _accionesEjecutadas.Add(new BaneoEjecutado(servidorId, usuarioId, ventanaBorrado));
        }

        // En el adaptador simulado la acción se loguea, no se ejecuta contra la plataforma.
        _logger.LogInformation(
            "[GATEWAY SIMULADO] Baneo con borrado retroactivo de {Dias} día(s) sobre usuario {Usuario} " +
            "en servidor {Servidor} (acción registrada, no se llamó a la plataforma).",
            ventanaBorrado.TotalDays, usuarioId.Valor, servidorId.Valor);

        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    public Task<ResultadoAccion> DesbanearAsync(
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

        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    public Task<ResultadoAccion> AplicarTimeoutAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan duracion, CancellationToken ct = default)
    {
        if (NoAccionable(servidorId, usuarioId, "timeout") is { } noAccionable)
        {
            return Task.FromResult(noAccionable);
        }

        lock (_candado)
        {
            _accionesEjecutadas.Add(new TimeoutEjecutado(servidorId, usuarioId, duracion));
        }

        _logger.LogInformation(
            "[GATEWAY SIMULADO] Timeout de {Minutos} minuto(s) sobre usuario {Usuario} en servidor " +
            "{Servidor} (acción registrada, no se llamó a la plataforma).",
            duracion.TotalMinutes, usuarioId.Valor, servidorId.Valor);

        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    public Task<ResultadoAccion> ExpulsarAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default)
    {
        if (NoAccionable(servidorId, usuarioId, "expulsión") is { } noAccionable)
        {
            return Task.FromResult(noAccionable);
        }

        lock (_candado)
        {
            _accionesEjecutadas.Add(new ExpulsionEjecutada(servidorId, usuarioId));
        }

        _logger.LogInformation(
            "[GATEWAY SIMULADO] Expulsión del usuario {Usuario} en servidor {Servidor} (acción " +
            "registrada, no se llamó a la plataforma).",
            usuarioId.Valor, servidorId.Valor);

        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    public Task<ResultadoAccion> AsignarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default)
    {
        if (NoAccionable(servidorId, usuarioId, $"asignar rol {rol.Valor}") is { } noAccionable)
        {
            return Task.FromResult(noAccionable);
        }

        lock (_candado)
        {
            _accionesEjecutadas.Add(new RolAsignado(servidorId, usuarioId, rol));
        }

        _logger.LogInformation(
            "[GATEWAY SIMULADO] Rol {Rol} ASIGNADO al usuario {Usuario} en servidor {Servidor} " +
            "(acción registrada, no se llamó a la plataforma).",
            rol.Valor, usuarioId.Valor, servidorId.Valor);

        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    public Task<ResultadoAccion> QuitarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default)
    {
        if (NoAccionable(servidorId, usuarioId, $"quitar rol {rol.Valor}") is { } noAccionable)
        {
            return Task.FromResult(noAccionable);
        }

        lock (_candado)
        {
            _accionesEjecutadas.Add(new RolQuitado(servidorId, usuarioId, rol));
        }

        _logger.LogInformation(
            "[GATEWAY SIMULADO] Rol {Rol} QUITADO al usuario {Usuario} en servidor {Servidor} " +
            "(acción registrada, no se llamó a la plataforma).",
            rol.Valor, usuarioId.Valor, servidorId.Valor);

        return Task.FromResult(ResultadoAccion.Ejecutada);
    }

    /// <summary>
    /// Devuelve no accionable por jerarquía si el usuario fue marcado como de rol superior
    /// (RN-01, CU-02 §7); en otro caso null para que el llamador ejecute la acción.
    /// </summary>
    private ResultadoAccion? NoAccionable(Snowflake servidorId, Snowflake usuarioId, string descripcion)
    {
        if (!_rolSuperior.ContainsKey((servidorId.Valor, usuarioId.Valor)))
        {
            return null;
        }

        _logger.LogWarning(
            "[GATEWAY SIMULADO] {Descripcion} sobre usuario {Usuario} en servidor {Servidor} NO es " +
            "accionable: el usuario tiene rol jerárquicamente superior al del bot (RN-01, " +
            "BANEO_JERARQUIA_INSUFICIENTE); no se ejecuta.",
            descripcion, usuarioId.Valor, servidorId.Valor);

        return ResultadoAccion.NoAccionablePorJerarquia;
    }
}
