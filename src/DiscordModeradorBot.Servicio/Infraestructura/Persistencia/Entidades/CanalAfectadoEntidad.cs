namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de un canal afectado por un incidente (modelo-datos-logico
/// §2.13, RN-11). En R2 se normaliza a tabla hija del incidente (antes era JSON embebido en
/// R1). Snowflake como TEXTO (RN-08).
/// </summary>
public sealed class CanalAfectadoEntidad
{
    public int Id { get; set; }

    /// <summary>FK a Incidente (RC-01, RN-11).</summary>
    public int IncidenteId { get; set; }

    public string SnowflakeCanal { get; set; } = string.Empty;
}
