namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia del servidor registrado (modelo-datos-logico §2.2). Mantiene
/// el dominio independiente del ORM (ADR-04). Snowflake como TEXTO (RN-08); token
/// cifrado en reposo (RN-14).
/// </summary>
public sealed class ServidorRegistradoEntidad
{
    public int Id { get; set; }

    public string SnowflakeServidor { get; set; } = string.Empty;

    public string TokenCifrado { get; set; } = string.Empty;

    public string EstadoConexion { get; set; } = "desconectado";

    public string EstadoActivacion { get; set; } = "inactivo";

    public string? NombreDescriptivo { get; set; }

    public DateTimeOffset CreadoEn { get; set; }
}
