using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>
/// Resultado de agregar una exención (CU-15). En caso de fallo lleva el código del CU
/// (EXENCION_IDENTIFICADOR_INVALIDO, EXENCION_DUPLICADA) y un mensaje claro para el
/// administrador (CU-15 §6).
/// </summary>
public sealed record ResultadoAltaExencion(
    bool Exito,
    Exencion? Exencion = null,
    string? Codigo = null,
    string? Mensaje = null)
{
    public static ResultadoAltaExencion Ok(Exencion exencion) => new(true, exencion);

    public static ResultadoAltaExencion Falla(string codigo, string mensaje) =>
        new(false, Codigo: codigo, Mensaje: mensaje);
}

/// <summary>
/// Servicio de aplicación de exenciones (CU-15, RN-07, RN-08, R5). Administra las exenciones de
/// un servidor —alta, listado y baja— validando el identificador como snowflake AL GUARDAR: un
/// identificador inválido se rechaza en el origen con EXENCION_IDENTIFICADOR_INVALIDO y nunca se
/// persiste (RN-08). Un duplicado para el mismo (servidor, tipo, sujeto) se rechaza con
/// EXENCION_DUPLICADA sin crear copia. La aplicación de las exenciones en el pipeline (descarte
/// previo) la hace el motor consultando este repositorio (RN-07); este servicio cubre la gestión.
/// </summary>
public sealed class ServicioExenciones
{
    /// <summary>Código de CU para un identificador que no es un snowflake válido (CU-15 §6, RN-08).</summary>
    public const string CodigoIdentificadorInvalido = "EXENCION_IDENTIFICADOR_INVALIDO";

    /// <summary>Código de CU para una exención que ya existe para el mismo sujeto (CU-15 §6).</summary>
    public const string CodigoDuplicada = "EXENCION_DUPLICADA";

    private readonly IRepositorioExenciones _repositorio;

    public ServicioExenciones(IRepositorioExenciones repositorio) => _repositorio = repositorio;

    /// <summary>
    /// Agrega una exención de un tipo para un servidor, validando el identificador como snowflake
    /// (RN-08). Devuelve falla con EXENCION_IDENTIFICADOR_INVALIDO si no compila, o EXENCION_DUPLICADA
    /// si ya existía; si la crea, devuelve la exención persistida.
    /// </summary>
    public async Task<ResultadoAltaExencion> AgregarAsync(
        Snowflake servidorId,
        TipoSujetoExento tipo,
        string identificadorSujeto,
        CancellationToken ct = default)
    {
        // RN-08: el identificador del sujeto se valida como snowflake al configurar (CU-15 CA-03).
        if (!Snowflake.TryParse(identificadorSujeto, out var sujeto))
        {
            return ResultadoAltaExencion.Falla(
                CodigoIdentificadorInvalido,
                $"El identificador '{identificadorSujeto}' no tiene formato de snowflake (solo dígitos).");
        }

        var exencion = new Exencion(tipo, sujeto);
        var creada = await _repositorio.AgregarAsync(servidorId, exencion, ct);

        return creada
            ? ResultadoAltaExencion.Ok(exencion)
            : ResultadoAltaExencion.Falla(
                CodigoDuplicada,
                $"Ya existe una exención por {tipo.ToString().ToLowerInvariant()} para ese sujeto en el servidor.");
    }

    /// <summary>Lista las exenciones declaradas para un servidor (CU-15 §4 paso 1).</summary>
    public Task<IReadOnlyList<Exencion>> ListarAsync(Snowflake servidorId, CancellationToken ct = default)
        => _repositorio.ListarPorServidorAsync(servidorId, ct);

    /// <summary>
    /// Quita una exención de un servidor; el sujeto vuelve a quedar sujeto a la moderación
    /// (CU-15 §5.A). Devuelve true si existía y se quitó.
    /// </summary>
    public Task<bool> QuitarAsync(Snowflake servidorId, Exencion exencion, CancellationToken ct = default)
        => _repositorio.QuitarAsync(servidorId, exencion, ct);
}
