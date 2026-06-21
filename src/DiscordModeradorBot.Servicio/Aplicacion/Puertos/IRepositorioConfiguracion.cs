using DiscordModeradorBot.Servicio.Dominio;

namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Datos de una regla dentro de un grupo, tal como se persisten/recuperan (R7, CU-11). Una
/// regla del grupo es de contenido (referida por id de la tabla Regla) o de conducta (clave
/// lógica del catálogo, p. ej. "rafaga-distribuida").
/// </summary>
public sealed record ReglaDeGrupo(string ClaseRegla, int? ReglaContenidoId, string? ClaveReglaConducta);

/// <summary>
/// Resumen de una regla de contenido persistida, con su id, su clase de criterio y el criterio
/// almacenado (R7, para listarla y armar grupos en el panel). <c>TipoCriterio</c> es el nombre de
/// la clase (p. ej. "ExpresionRegular" o "PalabrasClave"); <c>Criterio</c> es el patrón o la lista
/// de palabras tal como se guardó, para mostrarlo en la tabla de reglas.
/// </summary>
public sealed record ReglaContenidoResumen(
    int Id, string Nombre, string TipoCriterio, string Criterio, bool SensibleAMayusculas);

/// <summary>Grupo de reglas persistido, con su modo de coincidencia y sus reglas (R7, RN-15).</summary>
public sealed record GrupoPersistido(
    int Id,
    string Nombre,
    string ModoCoincidencia,
    int? MinimoCoincidencias,
    IReadOnlyList<ReglaDeGrupo> Reglas);

/// <summary>Acción persistida de un evento (R7, RN-05).</summary>
public sealed record AccionPersistida(
    string Tipo,
    int OrdenEjecucion,
    int? VentanaBorradoDias,
    int? DuracionTimeoutMinutos,
    string? RolObjetivo);

/// <summary>Evento/política persistido con su composición de grupos y sus acciones (R7, CU-11).</summary>
public sealed record EventoPersistido(
    int Id,
    string Nombre,
    int Prioridad,
    bool Continuar,
    string Modo,
    string ModoCombinacionGrupos,
    IReadOnlyList<int> GruposIds,
    IReadOnlyList<AccionPersistida> Acciones);

/// <summary>
/// Puerto de persistencia del modelo de configuración normalizado de R7 (CU-11,
/// modelo-datos-logico §2.7-§2.10): grupos de reglas, relación grupo-regla, eventos/políticas,
/// relación evento-grupo y acciones. La validación de composición (≥1 regla por grupo, RC-04;
/// N válido, RN-15) la garantiza el dominio antes de persistir. Eliminar un grupo referenciado
/// por un evento debe bloquearse (RC-03, CU-11 CA-04, CONFIG_REFERENCIA_REQUERIDA).
/// </summary>
public interface IRepositorioConfiguracion
{
    /// <summary>Persiste un grupo de reglas y devuelve su id (CU-11, RN-15).</summary>
    Task<int> AgregarGrupoAsync(
        Snowflake servidorId,
        string nombre,
        string modoCoincidencia,
        int? minimoCoincidencias,
        IReadOnlyList<ReglaDeGrupo> reglas,
        CancellationToken ct = default);

    /// <summary>
    /// Reemplaza el nombre, el modo de coincidencia, el N mínimo y la composición de reglas de un
    /// grupo (CU-11, RN-15, RC-03). La composición ya fue validada por el dominio. Devuelve false
    /// si el grupo no existe.
    /// </summary>
    Task<bool> ActualizarGrupoAsync(
        int grupoId,
        string nombre,
        string modoCoincidencia,
        int? minimoCoincidencias,
        IReadOnlyList<ReglaDeGrupo> reglas,
        CancellationToken ct = default);

    /// <summary>Lista los grupos de un servidor (CU-11).</summary>
    Task<IReadOnlyList<GrupoPersistido>> ListarGruposAsync(Snowflake servidorId, CancellationToken ct = default);

    /// <summary>
    /// Elimina un grupo. Devuelve false sin borrar si algún evento lo referencia (RC-03, CU-11
    /// CA-04): no se rompe la integridad de la composición.
    /// </summary>
    Task<bool> EliminarGrupoAsync(int grupoId, CancellationToken ct = default);

    /// <summary>
    /// Elimina una regla de contenido. Devuelve false sin borrar si algún grupo la referencia
    /// (RC-03): no se rompe la integridad de la composición; primero hay que sacarla del grupo.
    /// </summary>
    Task<bool> EliminarReglaContenidoAsync(int reglaId, CancellationToken ct = default);

    /// <summary>Persiste un evento/política con su composición de grupos y sus acciones, devuelve su id.</summary>
    Task<int> AgregarEventoAsync(
        Snowflake servidorId,
        string nombre,
        int prioridad,
        bool continuar,
        string modo,
        string modoCombinacionGrupos,
        IReadOnlyList<int> gruposIds,
        IReadOnlyList<AccionPersistida> acciones,
        CancellationToken ct = default);

    /// <summary>
    /// Reemplaza los datos de un evento/política y su composición de grupos y acciones (CU-11,
    /// RN-04, RN-05). No cambia el servidor. Devuelve false si el evento no existe.
    /// </summary>
    Task<bool> ActualizarEventoAsync(
        int eventoId,
        string nombre,
        int prioridad,
        bool continuar,
        string modo,
        string modoCombinacionGrupos,
        IReadOnlyList<int> gruposIds,
        IReadOnlyList<AccionPersistida> acciones,
        CancellationToken ct = default);

    /// <summary>Lista los eventos de un servidor, ordenados por prioridad (RN-04).</summary>
    Task<IReadOnlyList<EventoPersistido>> ListarEventosAsync(Snowflake servidorId, CancellationToken ct = default);

    /// <summary>
    /// Elimina un evento/política y, en cascada, su relación con los grupos y sus acciones
    /// (modelo-datos-logico §2.8-§2.10). Nada referencia a un evento, así que no se bloquea.
    /// Devuelve false si el evento no existe.
    /// </summary>
    Task<bool> EliminarEventoAsync(int eventoId, CancellationToken ct = default);

    /// <summary>
    /// Lista las reglas de contenido del servidor con su id (para armar grupos en el panel,
    /// CU-11). Es una vista de la tabla Regla; las reglas de conducta del catálogo no figuran como
    /// filas (en v1 hay una sola, parametrizada por descriptores).
    /// </summary>
    Task<IReadOnlyList<ReglaContenidoResumen>> ListarReglasContenidoAsync(
        Snowflake servidorId, CancellationToken ct = default);
}
