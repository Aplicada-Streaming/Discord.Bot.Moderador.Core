namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia del incidente (modelo-datos-logico §2.11, RN-11). En R2 la copia
/// de mensajes y los canales afectados dejan de serializarse como JSON y se normalizan a las
/// tablas hijas <see cref="MensajeAccionadoEntidad"/> y <see cref="CanalAfectadoEntidad"/>
/// (modelo-datos-logico §2.12, §2.13), persistidas en la misma unidad confirmada (RN-11).
/// Snowflakes como TEXTO (RN-08).
/// </summary>
public sealed class IncidenteEntidad
{
    public int Id { get; set; }

    public string ServidorId { get; set; } = string.Empty;

    public string UsuarioId { get; set; } = string.Empty;

    public string NombrePolitica { get; set; } = string.Empty;

    public string Modo { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;

    public string Resultado { get; set; } = string.Empty;

    public DateTimeOffset Instante { get; set; }

    /// <summary>
    /// Administrador que revirtió el baneo (FK a Administrador), o null si no hubo reversión
    /// (CU-07, modelo-datos-logico §2.11).
    /// </summary>
    public int? ReversionAutorId { get; set; }

    /// <summary>Fecha del desbaneo, o null si no hubo reversión (CU-07).</summary>
    public DateTimeOffset? ReversionFecha { get; set; }

    /// <summary>Copia de los mensajes accionados, normalizada a tabla hija (RN-11).</summary>
    public List<MensajeAccionadoEntidad> MensajesAccionados { get; set; } = new();

    /// <summary>Canales afectados por el incidente, normalizados a tabla hija (RN-11).</summary>
    public List<CanalAfectadoEntidad> CanalesAfectados { get; set; } = new();
}
