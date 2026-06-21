namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Modo de una política/evento. El modo seguro por defecto es Simulacion (RC-10, RN-09).
/// </summary>
public enum Modo
{
    /// <summary>Registra lo que haría sin ejecutar acción real (RN-09).</summary>
    Simulacion = 0,

    /// <summary>Ejecuta la acción real sobre el usuario y sus mensajes.</summary>
    Ejecucion = 1,
}
