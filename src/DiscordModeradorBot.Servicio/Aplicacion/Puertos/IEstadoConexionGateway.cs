using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Motivo de la última transición de estado de conexión de un servidor (CU-13). Permite que el
/// panel distinguya una caída transitoria (que se reintenta) de una credencial invalidada (que
/// requiere re-validar, CU-12).
/// </summary>
public enum MotivoEstadoConexion
{
    /// <summary>Estado inicial: aún no se intentó conectar.</summary>
    SinIntento = 0,

    /// <summary>Intentando establecer la conexión al canal de eventos.</summary>
    Conectando = 1,

    /// <summary>Conexión establecida y recibiendo eventos.</summary>
    Conectado = 2,

    /// <summary>Caída transitoria: el cliente reintenta automáticamente (CU-13 CA-01).</summary>
    DesconectadoTransitorio = 3,

    /// <summary>La credencial se revocó o venció: requiere re-validar (CU-13 CA-03, CONEXION_TOKEN_INVALIDO).</summary>
    DesconectadoTokenInvalido = 4,

    /// <summary>El servidor se desactivó/desregistró: la conexión se cerró a propósito.</summary>
    Detenido = 5,
}

/// <summary>Instantánea del estado de conexión de un servidor (CU-13).</summary>
/// <param name="ServidorId">Snowflake del servidor.</param>
/// <param name="Estado">Estado de conexión persistible (conectado/desconectado).</param>
/// <param name="Motivo">Motivo de la última transición (transitorio, token inválido, etc.).</param>
/// <param name="Desde">Instante de la última transición observada.</param>
public sealed record EstadoConexionServidor(
    Snowflake ServidorId,
    EstadoConexion Estado,
    MotivoEstadoConexion Motivo,
    DateTimeOffset Desde);

/// <summary>
/// Registro EN MEMORIA del estado de conexión de cada servidor (CU-13, ADR-13: estado por
/// contexto, no persistido de alta frecuencia, ADR-09). El gestor de conexiones lo actualiza al
/// conectar, caer, reconectar o detener; el panel lo consulta para mostrar el estado vigente. La
/// persistencia del estado de conexión en la base se hace puntualmente desde la prueba de
/// configuración y al activar/desactivar (no en cada parpadeo de la conexión).
/// </summary>
public interface IEstadoConexionGateway
{
    /// <summary>Registra el estado de conexión de un servidor (CU-13).</summary>
    void Actualizar(Snowflake servidorId, EstadoConexion estado, MotivoEstadoConexion motivo);

    /// <summary>Devuelve el estado de conexión vigente de un servidor, o null si nunca se registró.</summary>
    EstadoConexionServidor? Obtener(Snowflake servidorId);

    /// <summary>Devuelve el estado de conexión de todos los servidores observados (CU-13, panel).</summary>
    IReadOnlyList<EstadoConexionServidor> ObtenerTodos();
}
