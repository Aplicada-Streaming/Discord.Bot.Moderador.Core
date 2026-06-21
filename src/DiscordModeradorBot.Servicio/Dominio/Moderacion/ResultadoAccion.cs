namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Resultado de una acción individual ejecutada contra el adaptador del gateway (RN-01,
/// ADR-08). R6 deja de tratar las acciones del adaptador como fire-and-forget: cada
/// invocación devuelve uno de estos resultados para que el motor mapee el incidente a un
/// <see cref="ResultadoModeracion"/> y, ante una jerarquía superior o permisos faltantes,
/// NO se caiga: registra el incidente como no accionable y continúa (RN-01, ADR-08,
/// CU-02 §7 BANEO_JERARQUIA_INSUFICIENTE).
/// </summary>
public enum ResultadoAccion
{
    /// <summary>La acción se aplicó contra la plataforma (o se registró en el simulado).</summary>
    Ejecutada = 0,

    /// <summary>
    /// El usuario objetivo tiene un rol jerárquicamente superior o igual al del bot; la
    /// plataforma no permite accionar sobre él (RN-01, CU-02 §7).
    /// </summary>
    NoAccionablePorJerarquia = 1,

    /// <summary>El bot carece del permiso requerido para esta acción (RN-01).</summary>
    NoAccionablePorPermisos = 2,

    /// <summary>La acción se intentó pero la plataforma la rechazó o no la confirmó (ADR-08).</summary>
    Fallida = 3,
}

/// <summary>
/// Métodos de conveniencia sobre <see cref="ResultadoAccion"/> compartidos por el motor y
/// el reporte (RN-01, ADR-08).
/// </summary>
public static class ResultadoAccionExtensiones
{
    /// <summary>
    /// La acción no se pudo aplicar por una limitación estructural del bot (jerarquía o
    /// permisos), no por un fallo transitorio de la plataforma (RN-01).
    /// </summary>
    public static bool EsNoAccionable(this ResultadoAccion resultado) =>
        resultado is ResultadoAccion.NoAccionablePorJerarquia or ResultadoAccion.NoAccionablePorPermisos;

    /// <summary>
    /// Mapea el resultado de una acción al resultado registrable del incidente (RN-01, ADR-08).
    /// Jerarquía/permisos → no accionable; fallo de plataforma → fallida; en otro caso, ejecutada.
    /// </summary>
    public static ResultadoModeracion AResultadoModeracion(this ResultadoAccion resultado) => resultado switch
    {
        ResultadoAccion.NoAccionablePorJerarquia or ResultadoAccion.NoAccionablePorPermisos
            => ResultadoModeracion.NoAccionable,
        ResultadoAccion.Fallida => ResultadoModeracion.Fallida,
        _ => ResultadoModeracion.Ejecutada,
    };
}
