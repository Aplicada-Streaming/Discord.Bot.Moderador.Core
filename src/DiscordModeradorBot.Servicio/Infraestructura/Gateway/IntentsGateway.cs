using Discord;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Intents del gateway requeridos por el bot (intake §17 P.1/P.3, CU-12). Centraliza los intents
/// para que la conexión y la prueba de configuración usen el mismo conjunto:
/// <list type="bullet">
///   <item><see cref="GatewayIntents.Guilds"/>: presencia y estructura de servidores/canales.</item>
///   <item><see cref="GatewayIntents.GuildMessages"/>: eventos de mensajes de servidores.</item>
///   <item><see cref="GatewayIntents.MessageContent"/>: contenido de los mensajes (PRIVILEGIADO).</item>
///   <item><see cref="GatewayIntents.GuildMembers"/>: roles del autor para exenciones (PRIVILEGIADO).</item>
///   <item><see cref="GatewayIntents.GuildBans"/>: eventos de baneos/moderación.</item>
/// </list>
/// Los intents privilegiados (MessageContent, GuildMembers) deben habilitarse en el portal de
/// desarrolladores; sin ellos la conexión falla o llega contenido vacío (ver SMOKE-TEST-DISCORD.md).
/// </summary>
public static class IntentsGateway
{
    /// <summary>Conjunto de intents requeridos por el bot.</summary>
    public const GatewayIntents Requeridos =
        GatewayIntents.Guilds
        | GatewayIntents.GuildMessages
        | GatewayIntents.MessageContent
        | GatewayIntents.GuildMembers
        | GatewayIntents.GuildBans;
}
