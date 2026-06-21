namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Modo de gateway seleccionable por configuración (Moderacion:Gateway). Decide qué adaptador y
/// servicio en segundo plano se registran:
/// <list type="bullet">
///   <item>
///     <see cref="Simulado"/> (DEFAULT): adaptador simulado + walking skeleton, sin red ni token,
///     para desarrollo y tests (ADR-04, intake §18). NO toca la plataforma real.
///   </item>
///   <item>
///     <see cref="Discord"/>: adaptador real con Discord.Net + gestor de conexiones que abre una
///     conexión por servidor activo con su token descifrado (ADR-13, CU-13). NO inyecta mensajes
///     simulados.
///   </item>
/// </list>
/// El default es <see cref="Simulado"/> para no romper dev/tests ni requerir token.
/// </summary>
public enum ModoGateway
{
    /// <summary>Adaptador simulado y walking skeleton (sin red ni token). Default.</summary>
    Simulado = 0,

    /// <summary>Adaptador real con Discord.Net y gestor de conexiones por servidor.</summary>
    Discord = 1,
}
