namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de un evento/política (R7, modelo-datos-logico §2.8; CU-11, RN-04,
/// RN-09). Normaliza la prioridad de evaluación (RN-04), la bandera continuar, el modo
/// (simulacion/ejecucion, default simulacion por RN-09) y la combinación de sus grupos. Pertenece
/// a un servidor por snowflake (RN-08). Sus acciones (RN-05) y sus grupos (RC-03) viven en tablas
/// hijas.
/// </summary>
public sealed class EventoEntidad
{
    public int Id { get; set; }

    /// <summary>Snowflake del servidor dueño del evento (FK lógica, RN-08, RC-01).</summary>
    public string SnowflakeServidor { get; set; } = string.Empty;

    /// <summary>Etiqueta del evento para el panel (CU-11).</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Orden de evaluación; menor valor = mayor prioridad (RN-04, RC-05).</summary>
    public int Prioridad { get; set; }

    /// <summary>Permite seguir evaluando eventos de menor prioridad tras coincidir (RN-04).</summary>
    public bool Continuar { get; set; }

    /// <summary>Modo del evento: "simulacion" | "ejecucion" (default simulacion, RN-09).</summary>
    public string Modo { get; set; } = "simulacion";

    /// <summary>Modo de combinación de los grupos del evento: "todos" | "alguno" (RN-15).</summary>
    public string ModoCombinacionGrupos { get; set; } = "todos";

    /// <summary>Grupos del evento (relación evento-grupo, RC-03).</summary>
    public List<EventoGrupoEntidad> Grupos { get; set; } = new();

    /// <summary>Acciones del evento, en orden de ejecución (RN-05).</summary>
    public List<AccionEntidad> Acciones { get; set; } = new();
}
