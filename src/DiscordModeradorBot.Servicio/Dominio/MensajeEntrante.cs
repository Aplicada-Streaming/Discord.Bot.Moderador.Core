namespace DiscordModeradorBot.Servicio.Dominio;

/// <summary>
/// Mensaje normalizado que ingresa al pipeline de moderación desde el canal de eventos
/// de la plataforma (flujo-ejecucion §1). Snowflakes como texto (RN-08).
/// </summary>
public sealed record MensajeEntrante(
    Snowflake ServidorId,
    Snowflake CanalId,
    Snowflake UsuarioId,
    Snowflake MensajeId,
    DateTimeOffset Instante,
    string Contenido);
