using DiscordModeradorBot.Servicio.Dominio.Administracion;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia del administrador único (RC-06, modelo-datos-logico §2.1). La
/// unicidad de la cuenta (a lo sumo una fila) se resguarda en el alta (RN-12, RN-13).
/// </summary>
public interface IRepositorioAdministrador
{
    /// <summary>Indica si ya existe la cuenta de administrador (first-run, CU-08/CU-09).</summary>
    Task<bool> ExisteAsync(CancellationToken ct = default);

    /// <summary>Recupera el administrador, o null si todavía no se dio de alta (CU-09).</summary>
    Task<Administrador?> ObtenerAsync(CancellationToken ct = default);

    /// <summary>
    /// Persiste el administrador inicial. Falla si ya existe una cuenta, preservando la
    /// unicidad (RC-06, RN-13). Devuelve el administrador con su identidad asignada.
    /// </summary>
    Task<Administrador> AgregarAsync(Administrador administrador, CancellationToken ct = default);
}
