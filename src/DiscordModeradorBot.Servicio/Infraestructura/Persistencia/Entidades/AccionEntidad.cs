namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de una acción de un evento (R7, modelo-datos-logico §2.10; CU-11,
/// RN-05). Normaliza el tipo del conjunto cerrado, el orden de ejecución (RN-05) y los parámetros
/// propios del tipo: la ventana de borrado para el baneo (RC-11, 0-7 días), la duración del
/// timeout y el rol objetivo para asignar/quitar rol. Pertenece a un evento por FK.
/// </summary>
public sealed class AccionEntidad
{
    public int Id { get; set; }

    /// <summary>FK al evento dueño de la acción.</summary>
    public int EventoId { get; set; }

    /// <summary>Tipo de acción del conjunto cerrado (modelo-datos-logico §2.10).</summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>Orden de ejecución dentro del evento (RN-05, RC-05).</summary>
    public int OrdenEjecucion { get; set; }

    /// <summary>Ventana de borrado retroactivo en días para el baneo (RC-11, 0-7); null si no aplica.</summary>
    public int? VentanaBorradoDias { get; set; }

    /// <summary>Duración del timeout en minutos; null si no aplica (R6).</summary>
    public int? DuracionTimeoutMinutos { get; set; }

    /// <summary>Snowflake del rol objetivo para asignar/quitar rol (RN-08); null si no aplica (R6).</summary>
    public string? RolObjetivo { get; set; }
}
