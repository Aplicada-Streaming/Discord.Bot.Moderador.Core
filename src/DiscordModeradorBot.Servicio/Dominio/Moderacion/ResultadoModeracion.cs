namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Conjunto cerrado de resultados de moderación (arquitectura §7, modelo-datos-logico
/// ck_incidente_resultado, RN-01). En R1 se materializan Simulada y Ejecutada;
/// NoAccionable y Fallida quedan disponibles para R2 (jerarquía/permisos).
/// </summary>
public enum ResultadoModeracion
{
    /// <summary>El evento se evaluó pero no hubo coincidencia accionable.</summary>
    NoAccionable = 0,

    /// <summary>La acción se registró sin ejecutarse (modo simulación, RN-09).</summary>
    Simulada = 1,

    /// <summary>La acción real se ejecutó contra la plataforma.</summary>
    Ejecutada = 2,

    /// <summary>La acción no pudo ejecutarse (jerarquía/permisos); reservado para R2.</summary>
    Fallida = 3,
}
