namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de un mensaje accionado: copia de un mensaje involucrado en un
/// incidente, tomada antes de cualquier remoción (modelo-datos-logico §2.12, RN-11). En R2
/// se normaliza a tabla hija del incidente (antes era JSON embebido en R1). Snowflakes como
/// TEXTO (RN-08).
/// </summary>
public sealed class MensajeAccionadoEntidad
{
    public int Id { get; set; }

    /// <summary>FK a Incidente (RC-01, RN-11).</summary>
    public int IncidenteId { get; set; }

    public string SnowflakeMensaje { get; set; } = string.Empty;

    public string SnowflakeCanal { get; set; } = string.Empty;

    /// <summary>Copia tomada antes de la remoción (RN-11, RN-05).</summary>
    public string ContenidoCopiado { get; set; } = string.Empty;

    /// <summary>Nombre legible del canal al momento del incidente (CU-06); "" para incidentes previos.</summary>
    public string NombreCanal { get; set; } = string.Empty;

    /// <summary>Nombre legible del autor al momento del incidente (CU-06); "" para incidentes previos.</summary>
    public string NombreUsuario { get; set; } = string.Empty;
}
