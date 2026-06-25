namespace DiscordModeradorBot.Servicio.Dominio;

/// <summary>
/// Mensaje normalizado que ingresa al pipeline de moderación desde el canal de eventos
/// de la plataforma (flujo-ejecucion §1). Snowflakes como texto (RN-08).
/// </summary>
/// <remarks>
/// R5 agrega <see cref="RolesDelAutor"/> para evaluar exenciones por rol (CU-15, RN-07):
/// el adaptador puebla los roles del emisor; el evaluador de exenciones descarta el mensaje
/// si alguno de esos roles está exento. Es una propiedad init con default VACÍO para no
/// romper las construcciones posicionales previas (R1-R4), que siguen compilando sin cambios.
/// </remarks>
public sealed record MensajeEntrante(
    Snowflake ServidorId,
    Snowflake CanalId,
    Snowflake UsuarioId,
    Snowflake MensajeId,
    DateTimeOffset Instante,
    string Contenido)
{
    /// <summary>
    /// Roles del autor del mensaje (snowflakes como texto, RN-08), poblados por el adaptador.
    /// Default VACÍO: un mensaje sin roles conocidos nunca queda exento por rol (RN-07). Las
    /// construcciones de R1-R4 que no lo indican conservan el conjunto vacío.
    /// </summary>
    public IReadOnlyCollection<Snowflake> RolesDelAutor { get; init; } = [];

    /// <summary>
    /// Nombre legible del canal del mensaje (p. ej. "general"), poblado por el adaptador para que el
    /// incidente conserve un dato entendible además del snowflake (CU-06). Default VACÍO: las
    /// construcciones previas que no lo indican conservan cadena vacía y la UI cae al id.
    /// </summary>
    public string NombreCanal { get; init; } = string.Empty;

    /// <summary>
    /// Nombre legible del autor del mensaje (display name de Discord, p. ej. "Juan"), poblado por el
    /// adaptador para identificar al emisor del incidente más allá del snowflake (CU-06). Default
    /// VACÍO: las construcciones previas conservan cadena vacía y la UI cae al id.
    /// </summary>
    public string NombreUsuario { get; init; } = string.Empty;
}
