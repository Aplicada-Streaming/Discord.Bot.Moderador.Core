using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Contenido;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia de reglas de contenido (CU-04, ADR-02, R3). El criterio que se
/// persiste ya fue validado al configurar la regla (RN-03); al recuperarlo se vuelve a compilar
/// con su tope de tiempo (ADR-08). La asociación a la política se guarda por su nombre lógico
/// (mínimo de R3); la normalización a Evento/GrupoDeReglas es de una rebanada posterior.
/// </summary>
public interface IRepositorioReglasContenido
{
    /// <summary>
    /// Persiste una regla de contenido ya validada para un servidor y política (CU-04). El nombre
    /// de la política asocia la regla al disparo correspondiente en R3.
    /// </summary>
    Task AgregarAsync(
        Snowflake servidorId, string nombrePolitica, ReglaContenido regla, CancellationToken ct = default);

    /// <summary>
    /// Recupera las reglas de contenido de un servidor, recompilando su criterio con el tope de
    /// tiempo indicado (ADR-08). Devuelve la regla de dominio junto con el nombre de la política
    /// a la que dispara.
    /// </summary>
    Task<IReadOnlyList<ReglaContenidoPersistida>> ListarPorServidorAsync(
        Snowflake servidorId, TimeSpan topeTiempoEvaluacion, CancellationToken ct = default);

    /// <summary>
    /// Reemplaza el criterio de una regla de contenido ya validada (CU-04, RN-03): nombre, clase,
    /// criterio y sensibilidad. No cambia el servidor ni la política. Devuelve false si no existe.
    /// </summary>
    Task<bool> ActualizarAsync(int reglaId, ReglaContenido regla, CancellationToken ct = default);
}

/// <summary>
/// Regla de contenido recuperada de la persistencia junto con la política a la que dispara (R3).
/// <paramref name="Id"/> es el identificador persistido de la regla, usado por el cargador de
/// políticas para asociarla a un <c>GrupoDeReglas</c> (CU-11) a partir de <c>ReglaDeGrupo.ReglaContenidoId</c>.
/// </summary>
public sealed record ReglaContenidoPersistida(int Id, string NombrePolitica, ReglaContenido Regla);
