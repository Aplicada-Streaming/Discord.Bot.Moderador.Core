using DiscordModeradorBot.Servicio.Dominio.Moderacion;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia de incidentes (RN-11, flujo-ejecucion etapa 9). Confirma el
/// registro del incidente con su evidencia como una unidad. R4 agrega la consulta por
/// identidad para el detalle del panel (CU-06) y la marca de reversión del baneo (CU-07).
/// </summary>
public interface IRepositorioIncidentes
{
    Task AgregarAsync(Incidente incidente, CancellationToken ct = default);

    Task<IReadOnlyList<Incidente>> ListarAsync(CancellationToken ct = default);

    /// <summary>
    /// Consulta filtrada y paginada de incidentes para la revisión del panel (CU-06 5.A). Aplica los
    /// predicados del <paramref name="filtro"/> (servidor, modo, resultado, usuario, rango de fechas)
    /// y la paginación EN LA CONSULTA a la base, no en memoria: solo trae la página pedida más el
    /// total que satisface el filtro. Ordena por fecha descendente (más reciente primero).
    /// </summary>
    Task<PaginaIncidentes> BuscarAsync(FiltroIncidentes filtro, CancellationToken ct = default);

    /// <summary>Recupera un incidente por su identidad, o null si no existe (CU-06 detalle).</summary>
    Task<Incidente?> ObtenerAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Marca el incidente como revertido, registrando el administrador y la fecha del desbaneo
    /// (CU-07). Devuelve false si el incidente no existe. No restaura mensajes (RN-11).
    /// </summary>
    Task<bool> MarcarRevertidoAsync(
        int incidenteId, int administradorId, DateTimeOffset fecha, CancellationToken ct = default);
}
