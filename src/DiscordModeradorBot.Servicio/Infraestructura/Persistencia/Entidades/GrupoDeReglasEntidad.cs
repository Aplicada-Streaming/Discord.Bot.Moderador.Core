namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de un grupo de reglas (R7, modelo-datos-logico: GrupoDeReglas;
/// RN-15, RC-04). Normaliza el grupo con su modo de coincidencia y, para AlMenosN, su N mínimo.
/// El grupo pertenece a un servidor por snowflake (RN-08). La composición mínima (≥1 regla,
/// RC-04) la garantiza el dominio al construir <c>GrupoDeReglas</c> y la relación M:M
/// <see cref="GrupoReglaEntidad"/>.
/// </summary>
public sealed class GrupoDeReglasEntidad
{
    public int Id { get; set; }

    /// <summary>Snowflake del servidor dueño del grupo (FK lógica, RN-08, RC-01).</summary>
    public string SnowflakeServidor { get; set; } = string.Empty;

    /// <summary>Etiqueta del grupo para el panel (CU-11).</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Modo de coincidencia del conjunto cerrado: "todas" | "alguna" | "almenosn".</summary>
    public string ModoCoincidencia { get; set; } = string.Empty;

    /// <summary>N mínimo de coincidencias; solo se completa en modo "almenosn" (RN-15).</summary>
    public int? MinimoCoincidencias { get; set; }

    /// <summary>Reglas del grupo (relación grupo-regla, RC-03).</summary>
    public List<GrupoReglaEntidad> Reglas { get; set; } = new();
}
