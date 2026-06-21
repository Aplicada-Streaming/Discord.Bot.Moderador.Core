namespace DiscordModeradorBot.Servicio.Dominio.Administracion;

/// <summary>
/// Administrador único del sistema (modelo-datos-logico §2.1, RC-06, RN-12, RN-13). Es el
/// único rol y la única cuenta: existe a lo sumo una instancia (RC-06). Guarda el
/// identificador de cuenta (único) y el resguardo de la contraseña como hash en formato PHC,
/// nunca en texto claro (RN-13, ADR-03). El dominio no conoce el algoritmo de hash: el
/// resguardo es opaco y se produce/verifica detrás del puerto
/// <see cref="Aplicacion.Puertos.IServicioHashContrasena"/>.
/// </summary>
public sealed class Administrador
{
    public Administrador(
        string identificadorCuenta,
        string resguardoPassword,
        DateTimeOffset creadoEn,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(identificadorCuenta))
        {
            throw new ArgumentException(
                "El identificador de cuenta del administrador es obligatorio.", nameof(identificadorCuenta));
        }

        if (string.IsNullOrWhiteSpace(resguardoPassword))
        {
            throw new ArgumentException(
                "El resguardo de la contraseña es obligatorio (nunca se guarda en claro, RN-13).",
                nameof(resguardoPassword));
        }

        Id = id;
        IdentificadorCuenta = identificadorCuenta.Trim();
        ResguardoPassword = resguardoPassword;
        CreadoEn = creadoEn;
    }

    /// <summary>Identidad de persistencia; 0 hasta que se confirma el alta (CU-08).</summary>
    public int Id { get; }

    /// <summary>Identificador de cuenta, único en el sistema (RC-06, modelo-datos-logico §2.1).</summary>
    public string IdentificadorCuenta { get; }

    /// <summary>
    /// Resguardo de la contraseña en formato PHC (hash no reversible, RN-13, ADR-03). Es
    /// opaco para el dominio; el puerto de hashing lo produce y lo verifica.
    /// </summary>
    public string ResguardoPassword { get; }

    /// <summary>Momento del alta en el primer ingreso (CU-08).</summary>
    public DateTimeOffset CreadoEn { get; }
}
