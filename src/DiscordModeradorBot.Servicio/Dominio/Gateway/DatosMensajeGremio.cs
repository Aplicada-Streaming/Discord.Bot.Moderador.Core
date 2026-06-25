namespace DiscordModeradorBot.Servicio.Dominio.Gateway;

/// <summary>
/// Datos de un mensaje recibido del canal de eventos de la plataforma, ABSTRAÍDOS del SDK
/// (Discord.Net) para poder mapearlos y probarlos sin red ni tipos del SDK (flujo-ejecucion §1,
/// ADR-04, ADR-13). El adaptador real (infraestructura) construye este DTO desde el tipo de
/// Discord.Net y delega el mapeo y el descarte en <see cref="MapeadorMensajeGremio"/>; los tests
/// construyen el DTO con dobles. Todos los identificadores llegan como texto (RN-08): un valor de
/// snowflake inválido se descarta en el mapeo, no rompe el pipeline.
/// </summary>
/// <param name="ServidorId">Snowflake del gremio (servidor) origen del mensaje.</param>
/// <param name="CanalId">Snowflake del canal del mensaje.</param>
/// <param name="UsuarioId">Snowflake del autor del mensaje.</param>
/// <param name="MensajeId">Snowflake del mensaje.</param>
/// <param name="Instante">Instante de creación del mensaje en la plataforma.</param>
/// <param name="Contenido">Texto del mensaje (puede llegar vacío si falta el intent de contenido).</param>
/// <param name="EsDeGremio">
/// True si el mensaje pertenece a un gremio (servidor). Un mensaje directo (DM) tiene
/// <c>false</c> y se descarta (RN-07/flujo §1: solo se modera el tráfico de servidores).
/// </param>
/// <param name="AutorEsBot">True si el autor es un bot; esos mensajes se ignoran.</param>
/// <param name="EsMensajeDeSistema">True si es un mensaje de sistema (join, pin, etc.); se ignora.</param>
/// <param name="RolesDelAutor">
/// Roles del autor como snowflakes de texto (RN-08), sin el rol universal @everyone. Default
/// VACÍO: un autor sin roles conocidos nunca queda exento por rol (RN-07).
/// </param>
public sealed record DatosMensajeGremio(
    string ServidorId,
    string CanalId,
    string UsuarioId,
    string MensajeId,
    DateTimeOffset Instante,
    string Contenido,
    bool EsDeGremio,
    bool AutorEsBot,
    bool EsMensajeDeSistema,
    IReadOnlyCollection<string> RolesDelAutor)
{
    public IReadOnlyCollection<string> RolesDelAutor { get; init; } = RolesDelAutor ?? [];

    /// <summary>
    /// Nombre legible del canal (p. ej. "general"), tomado del SDK al recibir el mensaje. Default
    /// VACÍO: los dobles de prueba que no lo indican conservan cadena vacía (RN-08, CU-06).
    /// </summary>
    public string NombreCanal { get; init; } = string.Empty;
}
