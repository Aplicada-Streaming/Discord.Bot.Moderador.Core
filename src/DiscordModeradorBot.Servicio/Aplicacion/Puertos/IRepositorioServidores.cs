using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de persistencia de servidores registrados (CU-10, ADR-02). La unicidad por
/// snowflake la garantiza el esquema (RN-08, RC-02).
/// </summary>
public interface IRepositorioServidores
{
    Task<bool> ExisteAsync(Snowflake snowflakeServidor, CancellationToken ct = default);

    Task AgregarAsync(ServidorRegistrado servidor, CancellationToken ct = default);

    Task<ServidorRegistrado?> ObtenerAsync(Snowflake snowflakeServidor, CancellationToken ct = default);

    Task<IReadOnlyList<ServidorRegistrado>> ListarAsync(CancellationToken ct = default);

    /// <summary>
    /// Actualiza el estado de activación y de conexión de un servidor tras la prueba de
    /// configuración (CU-12, CU-13, RN-16): el servidor pasa a activo solo si superó la prueba sin
    /// chequeos bloqueantes; un token inválido lo marca desconectado. Devuelve false si el servidor
    /// no existe. No toca el token ni el canal de salida.
    /// </summary>
    Task<bool> ActualizarEstadoAsync(
        Snowflake snowflakeServidor,
        EstadoActivacion estadoActivacion,
        EstadoConexion estadoConexion,
        CancellationToken ct = default);

    /// <summary>
    /// Elimina un servidor y su configuración (reglas de contenido, exenciones, grupos de reglas
    /// con sus reglas, y eventos con sus grupos y acciones), en una sola transacción. Conserva los
    /// incidentes del servidor como historial de auditoría (RN-11): no se borran. Devuelve false si
    /// el servidor no existe.
    /// </summary>
    Task<bool> EliminarAsync(Snowflake snowflakeServidor, CancellationToken ct = default);

    /// <summary>
    /// Actualiza los datos editables de un servidor (CU-10): nombre descriptivo y canal de salida,
    /// y el token cifrado SOLO si se provee uno nuevo (<paramref name="tokenCifrado"/> null = se
    /// conserva el actual; RN-14). No toca los estados de conexión/activación ni el snowflake
    /// (identificador). Devuelve false si el servidor no existe.
    /// </summary>
    Task<bool> ActualizarDatosAsync(
        Snowflake snowflakeServidor,
        string? nombreDescriptivo,
        string? tokenCifrado,
        CanalDeSalida? canalDeSalida,
        CancellationToken ct = default);
}
