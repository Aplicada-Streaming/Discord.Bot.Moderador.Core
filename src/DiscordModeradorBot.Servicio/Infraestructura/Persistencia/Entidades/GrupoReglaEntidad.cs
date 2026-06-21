namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de la relación M:M grupo-regla (R7, modelo-datos-logico: GrupoRegla; RC-03). Asocia
/// una regla (de contenido o de conducta) a un grupo. La regla de contenido se referencia por su
/// id en la tabla Regla; las reglas de conducta del catálogo se referencian por una clave lógica
/// (p. ej. "rafaga-distribuida"), ya que en v1 la conducta es una sola regla parametrizada por
/// descriptores (CU-01) y no se persiste como fila de la tabla Regla.
/// </summary>
public sealed class GrupoReglaEntidad
{
    public int Id { get; set; }

    /// <summary>FK al grupo (RC-03).</summary>
    public int GrupoDeReglasId { get; set; }

    /// <summary>Clase de la regla asociada: "contenido" | "conducta".</summary>
    public string ClaseRegla { get; set; } = string.Empty;

    /// <summary>
    /// FK a la regla de contenido (tabla Regla) cuando la clase es "contenido"; null si es de
    /// conducta.
    /// </summary>
    public int? ReglaContenidoId { get; set; }

    /// <summary>
    /// Clave lógica de la regla de conducta del catálogo cuando la clase es "conducta" (p. ej.
    /// "rafaga-distribuida"); null si la regla es de contenido.
    /// </summary>
    public string? ClaveReglaConducta { get; set; }
}
