using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;
using DiscordModeradorBot.Servicio.Dominio.Contenido;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>
/// Resultado de registrar una regla de contenido (CU-04, RN-03). En caso de fallo, lleva el
/// código del CU (CONTENIDO_PATRON_INVALIDO) y un mensaje claro para el administrador (CU-04 §6).
/// </summary>
public sealed record ResultadoRegistroReglaContenido(
    bool Exito,
    ReglaContenido? Regla = null,
    string? Codigo = null,
    string? Mensaje = null)
{
    public static ResultadoRegistroReglaContenido Ok(ReglaContenido regla) => new(true, regla);

    public static ResultadoRegistroReglaContenido Falla(string codigo, string mensaje) =>
        new(false, Codigo: codigo, Mensaje: mensaje);
}

/// <summary>
/// Servicio de configuración de reglas de contenido (CU-04, RN-03, ADR-08, R3). Valida el patrón
/// AL GUARDAR: un criterio inválido se rechaza en el origen con un error claro y NUNCA se
/// persiste, de modo que en tiempo de evaluación toda regla ya compiló (RN-03). El tope de tiempo
/// de evaluación se toma del descriptor único (ADR-12, RN-10) y viaja con la regla como su
/// matchTimeout (ADR-08); no se hardcodea aquí.
/// </summary>
public sealed class ServicioRegistroReglaContenido
{
    private readonly IRepositorioReglasContenido _repositorio;

    public ServicioRegistroReglaContenido(IRepositorioReglasContenido repositorio) =>
        _repositorio = repositorio;

    /// <summary>Tope de tiempo de evaluación efectivo, derivado del descriptor (ADR-12, ADR-08).</summary>
    public static TimeSpan TopeTiempoEvaluacion =>
        RegistroDescriptores.TopeTiempoEvaluacionContenidoPorDefecto;

    /// <summary>
    /// Registra una regla de contenido por expresión regular para un servidor y política. Valida
    /// el patrón al guardar (RN-03): si no compila, devuelve falla con CONTENIDO_PATRON_INVALIDO
    /// y no persiste nada. Si compila, persiste la regla validada y la devuelve.
    /// </summary>
    public async Task<ResultadoRegistroReglaContenido> RegistrarPorExpresionRegularAsync(
        Snowflake servidorId,
        string nombrePolitica,
        string nombreRegla,
        string patron,
        bool sensibleAMayusculas = false,
        CancellationToken ct = default)
    {
        ReglaContenido regla;
        try
        {
            // RN-03: la validez del patrón se decide al configurar, no en evaluación (ADR-08).
            regla = ReglaContenido.PorExpresionRegular(
                nombreRegla, patron, TopeTiempoEvaluacion, sensibleAMayusculas);
        }
        catch (ReglaContenidoInvalidaException ex)
        {
            return ResultadoRegistroReglaContenido.Falla(ex.Codigo, ex.Message);
        }

        await _repositorio.AgregarAsync(servidorId, nombrePolitica, regla, ct);
        return ResultadoRegistroReglaContenido.Ok(regla);
    }

    /// <summary>
    /// Registra una regla de contenido por PALABRAS o FRASES CLAVE para un servidor y política.
    /// Valida al guardar (RN-03): si no hay al menos una palabra, devuelve falla con
    /// CONTENIDO_PATRON_INVALIDO y no persiste nada. Si es válida, persiste la regla y la devuelve.
    /// </summary>
    public async Task<ResultadoRegistroReglaContenido> RegistrarPorPalabrasClaveAsync(
        Snowflake servidorId,
        string nombrePolitica,
        string nombreRegla,
        string palabrasClave,
        bool sensibleAMayusculas = false,
        CancellationToken ct = default)
    {
        ReglaContenido regla;
        try
        {
            regla = ReglaContenido.PorPalabrasClave(
                nombreRegla, palabrasClave, TopeTiempoEvaluacion, sensibleAMayusculas);
        }
        catch (ReglaContenidoInvalidaException ex)
        {
            return ResultadoRegistroReglaContenido.Falla(ex.Codigo, ex.Message);
        }

        await _repositorio.AgregarAsync(servidorId, nombrePolitica, regla, ct);
        return ResultadoRegistroReglaContenido.Ok(regla);
    }
}
