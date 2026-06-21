using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>Códigos de error de la reversión de una contención (desbaneo, CU-07 §6).</summary>
public enum ErrorDesbaneo
{
    /// <summary>El incidente no existe o fue purgado (CU-06 INCIDENTE_NO_ENCONTRADO).</summary>
    IncidenteNoEncontrado,

    /// <summary>
    /// El incidente no es un baneo ejecutado: una simulación no se revierte (CU-07 CA-02,
    /// RN-09). El desbaneo solo aplica a un baneo real.
    /// </summary>
    NoEsBaneoReversible,

    /// <summary>El baneo ya fue revertido antes (CU-07; no se revierte dos veces).</summary>
    YaRevertido,
}

/// <summary>Resultado de la reversión de un baneo (CU-07).</summary>
public sealed record ResultadoDesbaneo(bool Exito, ErrorDesbaneo? Error = null)
{
    public static ResultadoDesbaneo Ok() => new(true);

    public static ResultadoDesbaneo Falla(ErrorDesbaneo error) => new(false, error);
}

/// <summary>
/// Servicio de reversión de una contención (desbaneo desde el panel, CU-07; RN-11, RN-12).
/// Sobre un incidente cuyo resultado fue un baneo ejecutado y que aún no fue revertido:
/// invoca el desbaneo en la plataforma a través del adaptador, marca el incidente como
/// revertido (con quién y cuándo) y NO restaura los mensajes borrados (RN-11). Rechaza el
/// desbaneo sobre incidentes que no son baneos reales o que ya fueron revertidos. La
/// autorización (solo el administrador, RN-12) la impone la capa de presentación con
/// [Authorize]; este servicio recibe la identidad del administrador autenticado.
/// </summary>
public sealed class ServicioDesbaneo
{
    private readonly IRepositorioIncidentes _repositorioIncidentes;
    private readonly IAdaptadorGateway _adaptador;
    private readonly IReloj _reloj;
    private readonly ILogger<ServicioDesbaneo> _logger;

    public ServicioDesbaneo(
        IRepositorioIncidentes repositorioIncidentes,
        IAdaptadorGateway adaptador,
        IReloj reloj,
        ILogger<ServicioDesbaneo> logger)
    {
        _repositorioIncidentes = repositorioIncidentes;
        _adaptador = adaptador;
        _reloj = reloj;
        _logger = logger;
    }

    /// <summary>
    /// Revierte el baneo del incidente indicado por orden del administrador autenticado
    /// (CU-07). Valida que el incidente exista, sea un baneo ejecutado y no esté ya revertido;
    /// invoca <see cref="IAdaptadorGateway.DesbanearAsync"/>, registra la reversión y advierte
    /// (vía contrato/UX) que los mensajes borrados no se restauran (RN-11).
    /// </summary>
    public async Task<ResultadoDesbaneo> RevertirAsync(
        int incidenteId, int administradorId, CancellationToken ct = default)
    {
        var incidente = await _repositorioIncidentes.ObtenerAsync(incidenteId, ct);
        if (incidente is null)
        {
            return ResultadoDesbaneo.Falla(ErrorDesbaneo.IncidenteNoEncontrado);
        }

        // Solo se revierte un baneo ejecutado (CU-07 CA-02, RN-09): una simulación no.
        if (!incidente.EsBaneoEjecutado)
        {
            return ResultadoDesbaneo.Falla(ErrorDesbaneo.NoEsBaneoReversible);
        }

        // No se revierte dos veces (CU-07).
        if (incidente.FueRevertido)
        {
            return ResultadoDesbaneo.Falla(ErrorDesbaneo.YaRevertido);
        }

        // Revierte el baneo en la plataforma; el adaptador NO restaura los mensajes (RN-11).
        await _adaptador.DesbanearAsync(incidente.ServidorId, incidente.UsuarioId, ct);

        // Marca el incidente como revertido: quién (el administrador) y cuándo (CU-07).
        await _repositorioIncidentes.MarcarRevertidoAsync(
            incidente.Id, administradorId, _reloj.Ahora, ct);

        _logger.LogInformation(
            "Incidente {Incidente} revertido por el administrador {Administrador}: desbaneo del " +
            "usuario {Usuario} en servidor {Servidor}. Los mensajes borrados NO se restauran (RN-11).",
            incidente.Id, administradorId, incidente.UsuarioId.Valor, incidente.ServidorId.Valor);

        return ResultadoDesbaneo.Ok();
    }
}
