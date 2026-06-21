using DiscordModeradorBot.Servicio.Dominio;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto del adaptador del canal de eventos y de las operaciones de moderación de la
/// plataforma (arquitectura §3, ADR-04, ADR-13). El dominio/aplicación dependen de esta
/// abstracción; la infraestructura la implementa (adaptador simulado en R1, Discord.Net
/// estructural). Expone el flujo de mensajes entrantes y la operación de baneo con
/// borrado retroactivo.
/// </summary>
public interface IAdaptadorGateway
{
    /// <summary>
    /// Evento que emite cada <see cref="MensajeEntrante"/> normalizado recibido del canal
    /// de eventos (flujo-ejecucion §1).
    /// </summary>
    event Func<MensajeEntrante, Task>? MensajeRecibido;

    /// <summary>
    /// Ejecuta el baneo del usuario con borrado retroactivo de sus mensajes dentro de la
    /// ventana indicada (CU-02/CU-03). Se invoca solo en modo ejecución; en simulación no
    /// se llama (RN-09).
    /// </summary>
    Task BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default);
}
