namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Contenido del reporte que se publica en el canal de salida privado al dispararse una
/// política (CU-05). Compone el emisor, los mensajes que dispararon la acción (copia tomada
/// antes de cualquier borrado, RN-11), los canales afectados, la acción y el modo. Se marca
/// con <see cref="EsSimulacion"/> cuando proviene de una política en modo simulación
/// (CU-05 §5.A), para etiquetar el reporte como "lo que se habría ejecutado" (RN-09).
///
/// R6 agrega <see cref="Resultado"/>: cuando una acción no se pudo aplicar por jerarquía
/// superior del emisor o permisos faltantes (RN-01), el reporte se publica igualmente con la
/// advertencia de no accionable (CU-02 §7, TC-60), dejando constancia de que la acción no se
/// ejecutó (ADR-08, RN-11).
/// </summary>
public sealed record ReporteIncidente(
    Snowflake ServidorId,
    Snowflake UsuarioId,
    string NombrePolitica,
    TipoAccion Accion,
    bool EsSimulacion,
    IReadOnlyList<MensajeAccionado> MensajesAccionados,
    IReadOnlyList<Snowflake> CanalesAfectados,
    ResultadoModeracion Resultado = ResultadoModeracion.Ejecutada)
{
    /// <summary>
    /// El reporte corresponde a un incidente no accionable por jerarquía o permisos (RN-01):
    /// el reporte se publica igual con la advertencia y deja constancia de que la acción no se
    /// ejecutó (CU-02 §7, TC-60).
    /// </summary>
    public bool EsNoAccionable => Resultado == ResultadoModeracion.NoAccionable;

    /// <summary>
    /// Compone el reporte a partir de un incidente recién construido (CU-05 §4 paso 1-2). La
    /// copia de mensajes y los canales afectados se reutilizan tal cual del incidente para
    /// preservar la integridad de la evidencia (RN-11). El resultado del incidente viaja al
    /// reporte para surfacear la advertencia de no accionable (RN-01, R6).
    /// </summary>
    public static ReporteIncidente DesdeIncidente(Incidente incidente)
    {
        ArgumentNullException.ThrowIfNull(incidente);

        return new ReporteIncidente(
            incidente.ServidorId,
            incidente.UsuarioId,
            incidente.NombrePolitica,
            incidente.Accion,
            incidente.Modo == Modo.Simulacion,
            incidente.MensajesAccionados,
            incidente.CanalesAfectados,
            incidente.Resultado);
    }
}
