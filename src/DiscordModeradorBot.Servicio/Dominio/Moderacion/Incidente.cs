namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Copia de un mensaje involucrado en un incidente, tomada antes de cualquier remoción
/// (RN-11, modelo-datos-logico §2.12).
/// </summary>
public sealed record MensajeAccionado(
    Snowflake MensajeId,
    Snowflake CanalId,
    string ContenidoCopiado);

/// <summary>
/// Registro de un disparo de política (RN-11, modelo-datos-logico §2.11). Conserva la
/// copia de los mensajes involucrados, los canales afectados, el modo, la acción
/// resultante y el instante; es el rastro de auditoría del dominio y la única evidencia
/// disponible tras la remoción (RN-11). Se construye antes de cualquier ejecución de
/// acción (flujo-ejecucion etapa 5).
/// </summary>
public sealed class Incidente
{
    public Incidente(
        Snowflake servidorId,
        Snowflake usuarioId,
        string nombrePolitica,
        Modo modo,
        TipoAccion accion,
        ResultadoModeracion resultado,
        IReadOnlyList<MensajeAccionado> mensajesAccionados,
        IReadOnlyList<Snowflake> canalesAfectados,
        DateTimeOffset instante)
    {
        ServidorId = servidorId;
        UsuarioId = usuarioId;
        NombrePolitica = nombrePolitica;
        Modo = modo;
        Accion = accion;
        Resultado = resultado;
        MensajesAccionados = mensajesAccionados;
        CanalesAfectados = canalesAfectados;
        Instante = instante;
    }

    public Snowflake ServidorId { get; }
    public Snowflake UsuarioId { get; }
    public string NombrePolitica { get; }
    public Modo Modo { get; }
    public TipoAccion Accion { get; }
    public ResultadoModeracion Resultado { get; }
    public IReadOnlyList<MensajeAccionado> MensajesAccionados { get; }
    public IReadOnlyList<Snowflake> CanalesAfectados { get; }
    public DateTimeOffset Instante { get; }
}
