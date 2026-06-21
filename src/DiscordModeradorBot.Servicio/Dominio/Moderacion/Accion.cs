using DiscordModeradorBot.Servicio.Dominio.Configuracion;

namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Tipo de acción de moderación (modelo-datos-logico §2.10). R1 ejercitó
/// BaneoConBorradoRetroactivo en simulación; R2 agrega el camino de ejecución real del
/// baneo con borrado y el reporte a un canal privado (CU-02, CU-03, CU-05). El resto del
/// catálogo (timeout, expulsar, roles) se materializa en R6.
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

    /// <summary>Reporte del incidente a un canal de salida privado (CU-05, R2).</summary>
    ReportarACanalPrivado,
}

/// <summary>
/// Acción de moderación de un evento (modelo-datos-logico §2.10). Tiene un orden de
/// ejecución dentro de la política (RN-05) y, para el baneo con borrado retroactivo, una
/// ventana de borrado acotada entre 0 y 7 días (RC-11, RN-02). En ejecución se invoca
/// contra el puerto del adaptador; en simulación no se invoca (RN-09). La ventana de
/// borrado es distinta de la ventana de detección de R1: aquélla limpia mensajes hacia
/// atrás al banear; ésta cuenta canales distintos para detectar la ráfaga.
/// </summary>
public sealed record Accion(
    TipoAccion Tipo,
    int OrdenEjecucion = 0,
    int VentanaBorradoDias = 1)
{
    /// <summary>Tope de la ventana de borrado retroactivo por la plataforma (RC-11, RN-02).</summary>
    public const int VentanaBorradoMaximaDias = RegistroDescriptores.VentanaBorradoMaximaDias;

    /// <summary>
    /// Ventana de borrado efectiva en días: el valor configurado normalizado y acotado al
    /// tope de plataforma de 7 días (RN-02). Un valor superior NO se rechaza, se topa
    /// (CU-03 CA-02, código BORRADO_VENTANA_FUERA_DE_RANGO). Se deriva del descriptor único
    /// para que el tope no quede hardcodeado en la lógica (ADR-12, RN-10).
    /// </summary>
    public int VentanaBorradoEfectivaDias =>
        RegistroDescriptores.VentanaBorradoRetroactivoDias.NormalizarConTope(VentanaBorradoDias);
}
