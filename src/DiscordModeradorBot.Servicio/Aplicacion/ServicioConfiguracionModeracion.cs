using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>Resultado de  validar un valor entero contra su descriptor (CU-11, RN-10).</summary> 
public sealed record ResultadoValidacionDescriptor(bool Valido, int ValorEfectivo, string? Codigo = null, string? Mensaje = null);

/// <summary>Resultado de validar un valor decimal contra su descriptor (CU-11, RN-10).</summary>
public sealed record ResultadoValidacionDescriptorDecimal(bool Valido, double ValorEfectivo, string? Codigo = null, string? Mensaje = null);

/// <summary>Resultado de una operación de configuración (CU-11).</summary>
public sealed record ResultadoConfiguracion(bool Exito, int? Id = null, string? Codigo = null, string? Mensaje = null)
{
    public static ResultadoConfiguracion Ok(int? id = null) => new(true, id);

    public static ResultadoConfiguracion Falla(string codigo, string mensaje) => new(false, null, codigo, mensaje);
}

/// <summary>
/// Servicio de aplicación de la configuración de moderación dirigida por descriptores (R7,
/// CU-11, ADR-12, RN-10). Toda la validación de parámetros se hace contra el
/// <see cref="DescriptorParametro{T}"/> correspondiente: los límites NO se hardcodean, se toman
/// del descriptor (fuente única de verdad). La composición de grupos se valida con el dominio
/// (<see cref="GrupoDeReglas"/>, RN-15, RC-04): un grupo sin reglas o un N inválido se rechazan
/// con su código de CU. La persistencia se delega al repositorio del modelo normalizado. La UI
/// propone, este servicio valida, el humano confirma; la ranura del asistente de IA queda
/// reservada (no se construye).
/// </summary>
public sealed class ServicioConfiguracionModeracion
{
    /// <summary>Código de valor fuera de límites del descriptor (CU-11 CA-02, RN-10).</summary>
    public const string CodigoValorFueraDeLimite = "CONFIG_VALOR_FUERA_DE_LIMITE";

    /// <summary>Código de referencia requerida al eliminar un grupo referenciado (CU-11 CA-04, RC-03).</summary>
    public const string CodigoReferenciaRequerida = "CONFIG_REFERENCIA_REQUERIDA";

    private readonly IRepositorioConfiguracion _repositorio;

    public ServicioConfiguracionModeracion(IRepositorioConfiguracion repositorio) => _repositorio = repositorio;

    /// <summary>
    /// Valida un valor entero contra su descriptor (RN-10). Si la política es RECHAZAR fuera de
    /// límites (por defecto en CU-11), un valor fuera de rango devuelve no válido con el código del
    /// CU; con <paramref name="normalizar"/> en true, en cambio, se acota/sustituye según el
    /// descriptor (semántica de tope, p. ej. ventana de borrado). El valor válido se acepta tal cual.
    /// </summary>
    public static ResultadoValidacionDescriptor ValidarEntero(
        DescriptorParametro<int> descriptor, int valor, bool normalizar = false)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (descriptor.EsValido(valor))
        {
            return new ResultadoValidacionDescriptor(true, valor);
        }

        if (normalizar)
        {
            // Política de normalización (tope): el valor se acota a los límites del descriptor.
            return new ResultadoValidacionDescriptor(true, descriptor.NormalizarConTope(valor));
        }

        // Política de rechazo (CU-11 CA-02): se devuelve el código con los límites del descriptor.
        return new ResultadoValidacionDescriptor(
            false,
            descriptor.ValorPorDefecto,
            CodigoValorFueraDeLimite,
            $"El valor {valor} de '{descriptor.Etiqueta}' está fuera de los límites " +
            $"({descriptor.Minimo}–{descriptor.Maximo}).");
    }

    /// <summary>
    /// Valida un valor decimal contra su descriptor (RN-10), con la misma semántica que
    /// <see cref="ValidarEntero"/>: rechaza fuera de límites con el código del CU, o normaliza al tope
    /// del descriptor cuando <paramref name="normalizar"/> es true. Cubre los descriptores de tiempo
    /// (ventana de detección, antirrebote) cuyos límites NO se hardcodean: se toman del descriptor.
    /// </summary>
    public static ResultadoValidacionDescriptorDecimal ValidarDecimal(
        DescriptorParametro<double> descriptor, double valor, bool normalizar = false)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (descriptor.EsValido(valor))
        {
            return new ResultadoValidacionDescriptorDecimal(true, valor);
        }

        if (normalizar)
        {
            return new ResultadoValidacionDescriptorDecimal(true, descriptor.NormalizarConTope(valor));
        }

        return new ResultadoValidacionDescriptorDecimal(
            false,
            descriptor.ValorPorDefecto,
            CodigoValorFueraDeLimite,
            $"El valor {valor} de '{descriptor.Etiqueta}' está fuera de los límites " +
            $"({descriptor.Minimo}–{descriptor.Maximo}).");
    }

    /// <summary>
    /// Persiste un grupo de reglas tras validar su composición con el dominio (RN-15, RC-04). El
    /// dominio valida que tenga al menos una regla y que el N de AlMenosN sea coherente; un grupo
    /// inválido se rechaza con su código de CU sin persistir.
    /// </summary>
    public async Task<ResultadoConfiguracion> GuardarGrupoAsync(
        Snowflake servidorId,
        string nombre,
        ModoCoincidencia modo,
        IReadOnlyList<IReglaEvaluable> reglas,
        IReadOnlyList<ReglaDeGrupo> reglasPersistencia,
        int? minimoCoincidencias = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reglas);
        ArgumentNullException.ThrowIfNull(reglasPersistencia);

        try
        {
            // Validación de composición por el dominio (RN-15, RC-04): puede lanzar con su código.
            _ = new GrupoDeReglas(nombre, modo, reglas, minimoCoincidencias);
        }
        catch (GrupoDeReglasInvalidoException ex)
        {
            return ResultadoConfiguracion.Falla(ex.Codigo, ex.Message);
        }

        var id = await _repositorio.AgregarGrupoAsync(
            servidorId, nombre, modo.ToString().ToLowerInvariant(), minimoCoincidencias, reglasPersistencia, ct);
        return ResultadoConfiguracion.Ok(id);
    }

    /// <summary>
    /// Actualiza un grupo tras validar su nueva composición con el dominio (RN-15, RC-04), igual
    /// que el alta. Un grupo inválido se rechaza con su código de CU sin persistir cambios.
    /// </summary>
    public async Task<ResultadoConfiguracion> ActualizarGrupoAsync(
        int grupoId,
        string nombre,
        ModoCoincidencia modo,
        IReadOnlyList<IReglaEvaluable> reglas,
        IReadOnlyList<ReglaDeGrupo> reglasPersistencia,
        int? minimoCoincidencias = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reglas);
        ArgumentNullException.ThrowIfNull(reglasPersistencia);

        try
        {
            _ = new GrupoDeReglas(nombre, modo, reglas, minimoCoincidencias);
        }
        catch (GrupoDeReglasInvalidoException ex)
        {
            return ResultadoConfiguracion.Falla(ex.Codigo, ex.Message);
        }

        var ok = await _repositorio.ActualizarGrupoAsync(
            grupoId, nombre, modo.ToString().ToLowerInvariant(), minimoCoincidencias, reglasPersistencia, ct);
        return ok
            ? ResultadoConfiguracion.Ok(grupoId)
            : ResultadoConfiguracion.Falla(CodigoReferenciaRequerida, "El grupo no existe o fue eliminado.");
    }

    /// <summary>Lista los grupos persistidos de un servidor (CU-11).</summary>
    public Task<IReadOnlyList<GrupoPersistido>> ListarGruposAsync(Snowflake servidorId, CancellationToken ct = default)
        => _repositorio.ListarGruposAsync(servidorId, ct);

    /// <summary>
    /// Elimina un grupo; si está referenciado por un evento, lo bloquea con
    /// CONFIG_REFERENCIA_REQUERIDA (RC-03, CU-11 CA-04).
    /// </summary>
    public async Task<ResultadoConfiguracion> EliminarGrupoAsync(int grupoId, CancellationToken ct = default)
    {
        var eliminado = await _repositorio.EliminarGrupoAsync(grupoId, ct);
        return eliminado
            ? ResultadoConfiguracion.Ok(grupoId)
            : ResultadoConfiguracion.Falla(
                CodigoReferenciaRequerida,
                "El grupo está referenciado por un evento o no existe; no se puede eliminar (RC-03).");
    }

    /// <summary>
    /// Elimina una regla de contenido; si está referenciada por algún grupo, lo bloquea con
    /// CONFIG_REFERENCIA_REQUERIDA (RC-03): primero hay que sacarla del grupo.
    /// </summary>
    public async Task<ResultadoConfiguracion> EliminarReglaContenidoAsync(int reglaId, CancellationToken ct = default)
    {
        var eliminada = await _repositorio.EliminarReglaContenidoAsync(reglaId, ct);
        return eliminada
            ? ResultadoConfiguracion.Ok(reglaId)
            : ResultadoConfiguracion.Falla(
                CodigoReferenciaRequerida,
                "La regla está referenciada por un grupo o no existe; quitala del grupo antes de eliminarla (RC-03).");
    }

    /// <summary>Persiste un evento/política con su composición de grupos y sus acciones (CU-11, RN-04, RN-05).</summary>
    public async Task<ResultadoConfiguracion> GuardarEventoAsync(
        Snowflake servidorId,
        string nombre,
        int prioridad,
        bool continuar,
        string modo,
        string modoCombinacionGrupos,
        IReadOnlyList<int> gruposIds,
        IReadOnlyList<AccionPersistida> acciones,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(gruposIds);
        ArgumentNullException.ThrowIfNull(acciones);

        if (gruposIds.Count == 0)
        {
            return ResultadoConfiguracion.Falla(
                ComposicionPolitica.CodigoSinGrupos,
                "El evento debe componerse de al menos un grupo de reglas (RN-15).");
        }

        var id = await _repositorio.AgregarEventoAsync(
            servidorId, nombre, prioridad, continuar, modo, modoCombinacionGrupos, gruposIds, acciones, ct);
        return ResultadoConfiguracion.Ok(id);
    }

    /// <summary>
    /// Actualiza un evento/política con su composición de grupos y sus acciones (CU-11, RN-04, RN-05).
    /// Exige al menos un grupo (RN-15), igual que el alta. Devuelve falla si el evento no existe.
    /// </summary>
    public async Task<ResultadoConfiguracion> ActualizarEventoAsync(
        int eventoId,
        string nombre,
        int prioridad,
        bool continuar,
        string modo,
        string modoCombinacionGrupos,
        IReadOnlyList<int> gruposIds,
        IReadOnlyList<AccionPersistida> acciones,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(gruposIds);
        ArgumentNullException.ThrowIfNull(acciones);

        if (gruposIds.Count == 0)
        {
            return ResultadoConfiguracion.Falla(
                ComposicionPolitica.CodigoSinGrupos,
                "El evento debe componerse de al menos un grupo de reglas (RN-15).");
        }

        var ok = await _repositorio.ActualizarEventoAsync(
            eventoId, nombre, prioridad, continuar, modo, modoCombinacionGrupos, gruposIds, acciones, ct);
        return ok
            ? ResultadoConfiguracion.Ok(eventoId)
            : ResultadoConfiguracion.Falla(CodigoReferenciaRequerida, "El evento no existe o fue eliminado.");
    }

    /// <summary>Lista los eventos persistidos de un servidor, ordenados por prioridad (RN-04).</summary>
    public Task<IReadOnlyList<EventoPersistido>> ListarEventosAsync(Snowflake servidorId, CancellationToken ct = default)
        => _repositorio.ListarEventosAsync(servidorId, ct);

    /// <summary>Elimina un evento/política con su composición de grupos y sus acciones (CU-11).</summary>
    public async Task<ResultadoConfiguracion> EliminarEventoAsync(int eventoId, CancellationToken ct = default)
    {
        var eliminado = await _repositorio.EliminarEventoAsync(eventoId, ct);
        return eliminado
            ? ResultadoConfiguracion.Ok(eventoId)
            : ResultadoConfiguracion.Falla(
                CodigoReferenciaRequerida, "El evento no existe o ya fue eliminado.");
    }

    /// <summary>Lista las reglas de contenido del servidor con su id, para armar grupos (CU-11).</summary>
    public Task<IReadOnlyList<ReglaContenidoResumen>> ListarReglasContenidoAsync(
        Snowflake servidorId, CancellationToken ct = default)
        => _repositorio.ListarReglasContenidoAsync(servidorId, ct);

    /// <summary>
    /// Devuelve los parámetros de moderación efectivos del servidor (CU-11, RN-10) para mostrarlos
    /// en el panel (umbral/ventana de ráfaga y antirrebote).
    /// </summary>
    public Task<ParametrosModeracion> ObtenerParametrosAsync(Snowflake servidorId, CancellationToken ct = default)
        => _repositorio.ObtenerParametrosAsync(servidorId, ct);

    /// <summary>
    /// Valida los parámetros contra sus descriptores (RN-10, política de RECHAZO de CU-11) y, si son
    /// válidos, los persiste por servidor para que el motor los aplique (ráfaga CU-01, antirrebote
    /// CU-16). Un valor fuera de límites se rechaza con su código del CU sin guardar nada.
    /// </summary>
    public async Task<ResultadoConfiguracion> GuardarParametrosAsync(
        Snowflake servidorId,
        int umbralCanalesDistintos,
        double ventanaDeteccionSegundos,
        double ventanaAntirreboteSegundos,
        CancellationToken ct = default)
    {
        var umbral = ValidarEntero(RegistroDescriptores.UmbralCanalesDistintos, umbralCanalesDistintos);
        if (!umbral.Valido)
        {
            return ResultadoConfiguracion.Falla(umbral.Codigo!, umbral.Mensaje!);
        }

        var ventana = ValidarDecimal(RegistroDescriptores.VentanaDeteccionSegundos, ventanaDeteccionSegundos);
        if (!ventana.Valido)
        {
            return ResultadoConfiguracion.Falla(ventana.Codigo!, ventana.Mensaje!);
        }

        var antirrebote = ValidarDecimal(RegistroDescriptores.VentanaAntirreboteSegundos, ventanaAntirreboteSegundos);
        if (!antirrebote.Valido)
        {
            return ResultadoConfiguracion.Falla(antirrebote.Codigo!, antirrebote.Mensaje!);
        }

        await _repositorio.GuardarParametrosAsync(
            servidorId,
            new ParametrosModeracion(umbral.ValorEfectivo, ventana.ValorEfectivo, antirrebote.ValorEfectivo),
            ct);

        return ResultadoConfiguracion.Ok();
    }
}
