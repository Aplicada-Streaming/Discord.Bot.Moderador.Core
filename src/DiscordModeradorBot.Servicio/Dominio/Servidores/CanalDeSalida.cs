namespace DiscordModeradorBot.Servicio.Dominio.Servidores;

/// <summary>
/// Canal de salida lógico designado de un servidor (modelo-datos-logico §2.3, CU-05). El
/// sistema lo referencia por su propósito lógico (p. ej. "mod-log"), no por su nombre
/// visible, y publica allí los reportes de moderación. Guarda el snowflake del canal (como
/// texto, RN-08) y su propósito lógico. En R2 cada servidor declara a lo sumo un canal de
/// reporte; los múltiples canales de salida con propósito distinto (CU-05 §5.C) se amplían
/// en rebanadas posteriores.
/// </summary>
public sealed record CanalDeSalida(
    Snowflake SnowflakeCanal,
    string PropositoLogico)
{
    /// <summary>Propósito lógico del canal de reporte de incidentes de moderación (CU-05).</summary>
    public const string PropositoReporteIncidentes = "reporte-incidentes";
}
