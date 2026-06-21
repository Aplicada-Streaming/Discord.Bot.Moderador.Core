using DiscordModeradorBot.Servicio.Dominio.Moderacion;

namespace DiscordModeradorBot.Servicio.Dominio.Gateway;

/// <summary>
/// Naturaleza de la falla de una acción contra la plataforma, ABSTRAÍDA del SDK (Discord.Net)
/// para poder clasificarla y probarla sin red ni tipos del SDK (RN-01, ADR-08). El adaptador real
/// traduce su excepción (p. ej. <c>Discord.Net.HttpException</c>) a uno de estos valores y delega
/// la clasificación final en <see cref="ClasificadorResultadoAccion"/>; los tests ejercitan la
/// clasificación con estos valores directamente, sin <c>DiscordSocketClient</c>.
/// </summary>
public enum TipoFallaAccion
{
    /// <summary>No hubo falla: la acción se aplicó.</summary>
    Ninguna = 0,

    /// <summary>El usuario objetivo tiene rol jerárquicamente superior o igual al del bot (RN-01).</summary>
    JerarquiaSuperior = 1,

    /// <summary>El bot carece del permiso requerido para la acción (RN-01).</summary>
    PermisosFaltantes = 2,

    /// <summary>Otra falla de la plataforma (no estructural): se trata como fallida (ADR-08).</summary>
    FallaPlataforma = 3,
}

/// <summary>
/// Clasifica la naturaleza de una falla de acción a un <see cref="ResultadoAccion"/> registrable
/// (RN-01, ADR-08). Lógica PURA del dominio, sin dependencia del SDK ni de
/// <c>DiscordSocketClient</c>, para que el mapeo sea testeable: una jerarquía superior →
/// <see cref="ResultadoAccion.NoAccionablePorJerarquia"/>; permisos faltantes →
/// <see cref="ResultadoAccion.NoAccionablePorPermisos"/>; cualquier otra falla →
/// <see cref="ResultadoAccion.Fallida"/>. La excepción NUNCA se propaga al pipeline (ADR-08); el
/// adaptador la traduce a <see cref="TipoFallaAccion"/> antes de clasificar.
/// </summary>
public static class ClasificadorResultadoAccion
{
    /// <summary>Mapea la naturaleza de la falla al resultado registrable de la acción (RN-01, ADR-08).</summary>
    public static ResultadoAccion Clasificar(TipoFallaAccion falla) => falla switch
    {
        TipoFallaAccion.Ninguna => ResultadoAccion.Ejecutada,
        TipoFallaAccion.JerarquiaSuperior => ResultadoAccion.NoAccionablePorJerarquia,
        TipoFallaAccion.PermisosFaltantes => ResultadoAccion.NoAccionablePorPermisos,
        _ => ResultadoAccion.Fallida,
    };
}
