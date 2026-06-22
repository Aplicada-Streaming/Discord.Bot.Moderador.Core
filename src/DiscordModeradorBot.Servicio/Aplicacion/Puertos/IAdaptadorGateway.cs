using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto del adaptador del canal de eventos y de las operaciones de moderación de la
/// plataforma (arquitectura §3, ADR-04, ADR-13). El dominio/aplicación dependen de esta
/// abstracción; la infraestructura la implementa (adaptador simulado, Discord.Net
/// estructural). Expone el flujo de mensajes entrantes y las acciones de ejecución real:
/// el reporte a un canal de salida privado (CU-05), el baneo con borrado retroactivo
/// (CU-02/CU-03), el desbaneo (CU-07) y, en R6, las acciones adicionales sobre el usuario
/// (intake §4 Should Have): timeout, expulsión y gestión de roles.
///
/// R6 también deja de tratar las acciones de contención como fire-and-forget: cada acción
/// sobre un usuario devuelve un <see cref="ResultadoAccion"/> para que el motor mapee el
/// incidente y, si el bot no puede accionar por jerarquía superior o permisos faltantes,
/// NO se caiga: registra el incidente como no accionable y continúa (RN-01, ADR-08). El
/// reporte conserva firma de resultado por uniformidad. Todas las acciones se invocan solo
/// en modo ejecución; en simulación no se llaman (RN-09).
/// </summary>
public interface IAdaptadorGateway
{
    /// <summary>
    /// Evento que emite cada <see cref="MensajeEntrante"/> normalizado recibido del canal
    /// de eventos (flujo-ejecucion §1).
    /// </summary>
    event Func<MensajeEntrante, Task>? MensajeRecibido;

    /// <summary>
    /// Indica si el adaptador activo es el SIMULADO (sin red): las acciones se registran en el log
    /// en vez de ejecutarse contra Discord. Permite que la UI sea honesta sobre lo que ocurrió
    /// (p. ej. el mensaje de prueba NO llega al servidor real en modo simulado).
    /// </summary>
    bool EsSimulado { get; }

    /// <summary>
    /// Prueba la configuración de un servidor contra la plataforma ANTES de activarlo (CU-12,
    /// RN-16, RC-08): valida el token, los intents, los permisos requeridos (banear, borrar
    /// mensajes, gestionar roles), la recepción de eventos, el canal de salida y la jerarquía de
    /// roles del bot. Devuelve la lista de chequeos con su severidad (bloqueante vs advertencia):
    /// token inválido, intents o permisos faltantes son bloqueantes; una jerarquía del bot por
    /// debajo de algún rol es advertencia. El servidor solo se activa si no hay chequeos
    /// bloqueantes (RN-16). El token se descifra en memoria para la prueba y nunca se expone
    /// (RN-14). El simulado resuelve el resultado de forma configurable (tests/escenarios); el de
    /// Discord.Net lo resuelve de forma estructural contra la plataforma.
    /// </summary>
    Task<ResultadoPruebaConfiguracion> ProbarConfiguracionAsync(
        SolicitudPruebaConfiguracion solicitud, CancellationToken ct = default);

    /// <summary>
    /// Envía un MENSAJE DE PRUEBA al canal de salida designado para verificar que el bot puede
    /// publicar reportes (CU-05), sin necesidad de tener el servidor activado. Usa el token
    /// (descifrado en memoria, RN-14) y el canal de la solicitud. Devuelve <c>Ejecutada</c> si se
    /// publicó, <c>NoAccionablePorPermisos</c> si el bot no puede escribir en el canal, o
    /// <c>Fallida</c> si el token no validó, el canal no existe/no es de texto o la plataforma
    /// rechazó el envío. El adaptador real abre una conexión efímera; el simulado lo registra.
    /// </summary>
    Task<ResultadoAccion> EnviarMensajePruebaAsync(
        SolicitudPruebaConfiguracion solicitud, string mensaje, CancellationToken ct = default);

    /// <summary>
    /// Publica el reporte del incidente en el canal de salida privado designado del servidor
    /// (CU-05). Incluye el emisor, los mensajes que dispararon la acción y los canales
    /// afectados. Se invoca solo en modo ejecución (RN-09).
    /// </summary>
    Task<ResultadoAccion> ReportarAsync(
        CanalDeSalida canalSalida, ReporteIncidente reporte, CancellationToken ct = default);

    /// <summary>
    /// Ejecuta el baneo del usuario con borrado retroactivo de sus mensajes dentro de la
    /// ventana indicada (CU-02/CU-03). La ventana llega ya acotada al tope de 7 días (RN-02).
    /// Devuelve no accionable si el usuario tiene rol superior o faltan permisos (RN-01). Se
    /// invoca solo en modo ejecución; en simulación no se llama (RN-09).
    /// </summary>
    Task<ResultadoAccion> BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default);

    /// <summary>
    /// Revierte el baneo del usuario en el servidor (desbaneo, CU-07). Solo revierte el baneo;
    /// NO restaura los mensajes borrados (RN-11). Se invoca al revertir un incidente desde el
    /// panel sobre un baneo ejecutado (CU-07). El adaptador simulado lo registra/loguea; el de
    /// Discord remueve el baneo del usuario.
    /// </summary>
    Task<ResultadoAccion> DesbanearAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default);

    /// <summary>
    /// Silencia (timeout) al usuario durante la duración indicada (CU-11/intake §4 Should
    /// Have, R6). Devuelve no accionable si el usuario tiene rol superior o faltan permisos
    /// (RN-01). Se invoca solo en modo ejecución (RN-09).
    /// </summary>
    Task<ResultadoAccion> AplicarTimeoutAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan duracion, CancellationToken ct = default);

    /// <summary>
    /// Expulsa (kick) al usuario del servidor (intake §4 Should Have, R6). A diferencia del
    /// baneo, no impide su reingreso. Devuelve no accionable si el usuario tiene rol superior
    /// o faltan permisos (RN-01). Se invoca solo en modo ejecución (RN-09).
    /// </summary>
    Task<ResultadoAccion> ExpulsarAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default);

    /// <summary>
    /// Asigna el rol indicado al usuario (intake §4 Should Have, R6). Devuelve no accionable
    /// si el rol es jerárquicamente superior al del bot o faltan permisos (RN-01). Se invoca
    /// solo en modo ejecución (RN-09).
    /// </summary>
    Task<ResultadoAccion> AsignarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default);

    /// <summary>
    /// Quita el rol indicado al usuario (intake §4 Should Have, R6). Devuelve no accionable
    /// si el rol es jerárquicamente superior al del bot o faltan permisos (RN-01). Se invoca
    /// solo en modo ejecución (RN-09).
    /// </summary>
    Task<ResultadoAccion> QuitarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default);
}
