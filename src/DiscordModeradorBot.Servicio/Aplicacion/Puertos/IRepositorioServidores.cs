using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia de servidores registrados (CU-10, ADR-02). La unicidad por
/// snowflake la garantiza el esquema (RN-08, RC-02).
/// </summary>
public interface IRepositorioServidores
{
    Task<bool> ExisteAsync(Snowflake snowflakeServidor, CancellationToken ct = default);

    Task AgregarAsync(ServidorRegistrado servidor, CancellationToken ct = default);

    Task<ServidorRegistrado?> ObtenerAsync(Snowflake snowflakeServidor, CancellationToken ct = default);

    Task<IReadOnlyList<ServidorRegistrado>> ListarAsync(CancellationToken ct = default);
}
