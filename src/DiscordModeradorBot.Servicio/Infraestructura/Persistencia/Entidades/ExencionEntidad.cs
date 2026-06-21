namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de una exención (modelo-datos-logico §2.4, CU-15, R5). Mantiene el
/// dominio independiente del ORM (ADR-04). Una exención asocia un servidor con un sujeto de
/// confianza (rol, usuario o canal) excluido de la moderación (RN-07). El tipo se guarda como
/// texto del conjunto cerrado {rol, usuario, canal} (restricción ck_exencion_tipo) y el
/// identificador del sujeto como snowflake TEXTO (RN-08, RC-02). La unicidad por
/// (servidor, tipo, snowflake) evita duplicados (índice ux_exencion_sujeto, CU-15 EXENCION_DUPLICADA).
/// </summary>
public sealed class ExencionEntidad
{
    public int Id { get; set; }

    /// <summary>Snowflake del servidor dueño de la exención (FK lógica, RN-08, RC-01).</summary>
    public string SnowflakeServidor { get; set; } = string.Empty;

    /// <summary>Tipo del sujeto del conjunto cerrado {rol, usuario, canal} (ck_exencion_tipo).</summary>
    public string TipoSujeto { get; set; } = string.Empty;

    /// <summary>Snowflake del rol, usuario o canal exento, como TEXTO (RN-08, RC-02).</summary>
    public string SnowflakeSujeto { get; set; } = string.Empty;
}
