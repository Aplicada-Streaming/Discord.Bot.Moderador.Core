using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia de exenciones por servidor (CU-15, RN-07, ADR-02, R5). Las exenciones
/// se administran por contexto de servidor (ADR-13): se agregan, listan y quitan. El filtro de
/// descarte del pipeline (etapa 1) consulta las exenciones del servidor del mensaje antes de
/// evaluar cualquier regla (RN-07).
/// </summary>
public interface IRepositorioExenciones
{
    /// <summary>
    /// Persiste una exención para un servidor. Idempotente respecto del duplicado: si ya existe
    /// una exención con el mismo (servidor, tipo, snowflake) devuelve false sin crear un duplicado
    /// (CU-15 EXENCION_DUPLICADA); si la crea devuelve true.
    /// </summary>
    Task<bool> AgregarAsync(Snowflake servidorId, Exencion exencion, CancellationToken ct = default);

    /// <summary>Recupera las exenciones declaradas para un servidor (descarte por servidor, RN-07).</summary>
    Task<IReadOnlyList<Exencion>> ListarPorServidorAsync(
        Snowflake servidorId, CancellationToken ct = default);

    /// <summary>
    /// Quita una exención de un servidor; el sujeto vuelve a estar sujeto a la moderación
    /// (CU-15 §5.A). Devuelve true si existía y se quitó, false si no había nada que quitar.
    /// </summary>
    Task<bool> QuitarAsync(Snowflake servidorId, Exencion exencion, CancellationToken ct = default);
}
