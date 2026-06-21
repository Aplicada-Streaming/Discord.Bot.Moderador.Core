using DiscordModeradorBot.Servicio.Dominio.Moderacion;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia de incidentes (RN-11, flujo-ejecucion etapa 9). Confirma el
/// registro del incidente con su evidencia como una unidad.
/// </summary>
public interface IRepositorioIncidentes
{
    Task AgregarAsync(Incidente incidente, CancellationToken ct = default);

    Task<IReadOnlyList<Incidente>> ListarAsync(CancellationToken ct = default);
}
