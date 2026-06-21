namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de la relación M:M evento-grupo (R7, modelo-datos-logico: EventoGrupo; RC-03). Asocia
/// un grupo de reglas a un evento; es el segundo nivel del anidamiento booleano admitido (RN-15).
/// </summary>
public sealed class EventoGrupoEntidad
{
    public int Id { get; set; }

    /// <summary>FK al evento (RC-03).</summary>
    public int EventoId { get; set; }

    /// <summary>FK al grupo de reglas (RC-03).</summary>
    public int GrupoDeReglasId { get; set; }
}
