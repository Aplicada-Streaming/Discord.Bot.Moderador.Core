namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de una regla de contenido (modelo-datos-logico §2.5, CU-04, R3).
/// Mantiene el dominio independiente del ORM (ADR-04). El criterio se guarda como TEXTO: la
/// expresión regular (clase ExpresionRegular) o, a futuro, las palabras clave serializadas;
/// la clase del criterio se guarda como texto del conjunto cerrado (RC-09). La regla se asocia
/// a un servidor por su snowflake (RN-08) y, en R3, a la política por su nombre lógico. El
/// criterio almacenado ya fue validado al configurar la regla (RN-03); nunca se persiste inválido.
/// </summary>
public sealed class ReglaContenidoEntidad
{
    public int Id { get; set; }

    /// <summary>Snowflake del servidor dueño de la regla (FK lógica, RN-08, RC-01).</summary>
    public string SnowflakeServidor { get; set; } = string.Empty;

    /// <summary>Etiqueta de la regla para el panel (modelo-datos-logico §2.5).</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Clase del criterio del conjunto cerrado (RC-09); en R3 "ExpresionRegular".</summary>
    public string TipoCriterio { get; set; } = string.Empty;

    /// <summary>Criterio serializado: la expresión regular ya validada (RN-03, §2.5).</summary>
    public string Criterio { get; set; } = string.Empty;

    /// <summary>Si la coincidencia distingue mayúsculas/minúsculas (CU-04).</summary>
    public bool SensibleAMayusculas { get; set; }

    /// <summary>
    /// Nombre lógico de la política a la que dispara la regla (R3, mínimo del walking skeleton).
    /// La asociación a Evento/GrupoDeReglas del modelo completo se normaliza en una rebanada
    /// posterior (modelo-datos-logico §2.6, §2.7).
    /// </summary>
    public string NombrePolitica { get; set; } = string.Empty;
}
