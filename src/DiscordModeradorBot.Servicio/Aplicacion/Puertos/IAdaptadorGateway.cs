using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto del adaptador del canal de eventos y de las operaciones de moderación de la
/// plataforma (arquitectura §3, ADR-04, ADR-13). El dominio/aplicación dependen de esta
/// abstracción; la infraestructura la implementa (adaptador simulado, Discord.Net
/// estructural). Expone el flujo de mensajes entrantes y las acciones de ejecución real:
/// el reporte a un canal de salida privado (CU-05) y el baneo con borrado retroactivo
/// (CU-02/CU-03). Todas las acciones se invocan solo en modo ejecución; en simulación no
/// se llaman (RN-09).
/// </summary>
public interface IAdaptadorGateway
{
    /// <summary>
    /// Evento que emite cada <see cref="MensajeEntrante"/> normalizado recibido del canal
    /// de eventos (flujo-ejecucion §1).
    /// </summary>
    event Func<MensajeEntrante, Task>? MensajeRecibido;

    /// <summary>
    /// Publica el reporte del incidente en el canal de salida privado designado del servidor
    /// (CU-05). Incluye el emisor, los mensajes que dispararon la acción y los canales
    /// afectados. Se invoca solo en modo ejecución (RN-09).
    /// </summary>
    Task ReportarAsync(
        CanalDeSalida canalSalida, ReporteIncidente reporte, CancellationToken ct = default);

    /// <summary>
    /// Ejecuta el baneo del usuario con borrado retroactivo de sus mensajes dentro de la
    /// ventana indicada (CU-02/CU-03). La ventana llega ya acotada al tope de 7 días (RN-02).
    /// Se invoca solo en modo ejecución; en simulación no se llama (RN-09).
    /// </summary>
    Task BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default);
}
