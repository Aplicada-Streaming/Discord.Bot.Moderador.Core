namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;

/// <summary>
/// Entidad de persistencia de los parámetros de moderación dirigidos por descriptor (CU-11, RN-10)
/// que aplican POR SERVIDOR: umbral de canales distintos y ventana de detección de la ráfaga
/// distribuida (CU-01) y ventana de antirrebote (CU-16, RN-06). Una fila por servidor. Los valores
/// son nulables: una columna nula significa "usar el valor por defecto del descriptor", de modo que
/// un servidor sin fila o sin un parámetro configurado se comporta con los defaults vigentes.
/// </summary>
public sealed class ParametrosServidorEntidad
{
    public int Id { get; set; }

    /// <summary>Snowflake del servidor dueño de los parámetros (FK lógica, RN-08, RC-01).</summary>
    public string SnowflakeServidor { get; set; } = string.Empty;

    /// <summary>Umbral de canales distintos para la ráfaga distribuida (CU-01, RN-10).</summary>
    public int? UmbralCanalesDistintos { get; set; }

    /// <summary>Ventana de detección de la ráfaga, en segundos (CU-01, RN-10).</summary>
    public double? VentanaDeteccionSegundos { get; set; }

    /// <summary>Ventana de antirrebote por usuario, en segundos (CU-16, RN-06, RN-10).</summary>
    public double? VentanaAntirreboteSegundos { get; set; }
}
