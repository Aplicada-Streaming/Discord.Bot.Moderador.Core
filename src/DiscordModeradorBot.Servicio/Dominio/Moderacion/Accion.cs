namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Tipo de acción de moderación (modelo-datos-logico §2.10). R1 ejercita
/// BaneoConBorradoRetroactivo; el resto del catálogo se materializa en rebanadas
/// posteriores (R2, R6).
/// </summary>
public enum TipoAccion
{
    Reportar,
    Banear,
    BaneoConBorradoRetroactivo,
    Desbanear,
    Timeout,
    Expulsar,
    AsignarRol,
    QuitarRol,
}

/// <summary>
/// Acción de moderación de un evento. En R1 la única acción de ejecución relevante es el
/// baneo con borrado retroactivo; su ventana de borrado se acota entre 0 y 7 días
/// (RC-11, RN-02). En ejecución se invoca contra el puerto del adaptador; en simulación
/// no se invoca (RN-09).
/// </summary>
public sealed record Accion(
    TipoAccion Tipo,
    int OrdenEjecucion = 0,
    int VentanaBorradoDias = 1)
{
    /// <summary>Tope de la ventana de borrado retroactivo por la plataforma (RC-11, RN-02).</summary>
    public const int VentanaBorradoMaximaDias = 7;

    public int VentanaBorradoEfectivaDias =>
        Math.Clamp(VentanaBorradoDias, 0, VentanaBorradoMaximaDias);
}
