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
///
/// R4 agrega el estado de REVERSIÓN del baneo (CU-07): un incidente cuyo resultado fue un
/// baneo ejecutado puede revertirse desde el panel; al revertirse se registra quién (el
/// administrador) y cuándo. El desbaneo revierte el baneo, NO restaura los mensajes borrados
/// (RN-11). El identificador de persistencia se surfacea para identificar el incidente en el
/// detalle (CU-06) y la reversión (CU-07).
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
        DateTimeOffset instante,
        int id = 0,
        int? reversionAutorId = null,
        DateTimeOffset? reversionFecha = null)
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
        Id = id;
        ReversionAutorId = reversionAutorId;
        ReversionFecha = reversionFecha;
    }

    /// <summary>Identidad de persistencia; 0 hasta que el incidente se registra (CU-06/CU-07).</summary>
    public int Id { get; }

    public Snowflake ServidorId { get; }
    public Snowflake UsuarioId { get; }
    public string NombrePolitica { get; }
    public Modo Modo { get; }
    public TipoAccion Accion { get; }

    /// <summary>
    /// Resultado registrable del incidente (conjunto cerrado, RN-01, ADR-08). Se construye con
    /// el resultado esperado por el modo (simulada o ejecutada) y el motor lo reclasifica tras
    /// ejecutar las acciones: si una acción de contención no fue accionable por jerarquía o
    /// permisos, queda <see cref="ResultadoModeracion.NoAccionable"/> (RN-01); si la plataforma
    /// la rechazó, <see cref="ResultadoModeracion.Fallida"/> (ADR-08). El incidente se persiste
    /// igualmente y se reporta (RN-11). Settable solo dentro del dominio (init/private set).
    /// </summary>
    public ResultadoModeracion Resultado { get; private set; }

    public IReadOnlyList<MensajeAccionado> MensajesAccionados { get; }
    public IReadOnlyList<Snowflake> CanalesAfectados { get; }
    public DateTimeOffset Instante { get; }

    /// <summary>Administrador que revirtió el baneo, o null si no hubo reversión (CU-07).</summary>
    public int? ReversionAutorId { get; }

    /// <summary>Fecha del desbaneo, o null si no hubo reversión (CU-07).</summary>
    public DateTimeOffset? ReversionFecha { get; }

    /// <summary>
    /// El incidente registró un baneo ejecutado (real) y, por lo tanto, es candidato a
    /// reversión (CU-07 CA-01). Una simulación no se revierte (CU-06 CA-03/CU-07 CA-02, RN-09).
    /// </summary>
    public bool EsBaneoEjecutado =>
        Resultado == ResultadoModeracion.Ejecutada &&
        Accion is TipoAccion.Banear or TipoAccion.BaneoConBorradoRetroactivo;

    /// <summary>El baneo ya fue revertido (desbaneo registrado, CU-07 CA-03).</summary>
    public bool FueRevertido => ReversionFecha is not null;

    /// <summary>
    /// El incidente admite desbaneo desde el panel: es un baneo ejecutado y aún no fue
    /// revertido (CU-07). Una simulación o un baneo ya revertido no se ofrecen para revertir.
    /// </summary>
    public bool PuedeRevertirse => EsBaneoEjecutado && !FueRevertido;

    /// <summary>
    /// Reclasifica el resultado del incidente tras ejecutar sus acciones (RN-01, ADR-08, R6).
    /// Solo aplica en modo ejecución: una simulación conserva <see cref="ResultadoModeracion.Simulada"/>
    /// (RN-09). Permite marcar el incidente como no accionable (jerarquía/permisos) o fallido sin
    /// perder el registro ni el reporte (RN-11).
    /// </summary>
    public void ReclasificarResultado(ResultadoModeracion resultado)
    {
        if (Modo == Modo.Ejecucion)
        {
            Resultado = resultado;
        }
    }
}
