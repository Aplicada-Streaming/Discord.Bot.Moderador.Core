namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia del administrador único (modelo-datos-logico §2.1, RC-06, RN-13).
/// El identificador de cuenta es único (índice ux_administrador_cuenta) y la contraseña se
/// guarda solo como resguardo PHC (hash no reversible), nunca en claro (RN-13, ADR-03). La
/// unicidad de la cuenta (a lo sumo una fila, RC-06) se resguarda en el alta del servicio.
/// </summary>
public sealed class AdministradorEntidad
{
    public int Id { get; set; }

    public string IdentificadorCuenta { get; set; } = string.Empty;

    /// <summary>Resguardo de la contraseña en formato PHC; nunca en claro (RN-13, ADR-03).</summary>
    public string ResguardoPassword { get; set; } = string.Empty;

    public DateTimeOffset CreadoEn { get; set; }
}
