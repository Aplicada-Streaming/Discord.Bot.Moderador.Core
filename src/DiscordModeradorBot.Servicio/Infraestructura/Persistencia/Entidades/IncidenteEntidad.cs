namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia del incidente (modelo-datos-logico §2.11, RN-11). La copia de
/// mensajes y los canales afectados se serializan como texto JSON en R1 (versión mínima);
/// el modelo relacional completo de MensajeAccionado/CanalAfectado como tablas propias se
/// materializa en R2. Snowflakes como TEXTO (RN-08).
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

    /// <summary>Lista de canales afectados, serializada como JSON (RN-11).</summary>
    public string CanalesAfectados { get; set; } = "[]";

    /// <summary>Copia de los mensajes accionados, serializada como JSON (RN-11).</summary>
    public string CopiaMensajes { get; set; } = "[]";

    public DateTimeOffset Instante { get; set; }
}
