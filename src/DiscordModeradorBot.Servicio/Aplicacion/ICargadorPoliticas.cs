using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>
/// Fuente de las políticas que el <see cref="MotorDeModeracion"/> evalúa para un servidor (CU-11).
/// Desacopla al motor de DÓNDE vienen las políticas: en producción se cargan desde la configuración
/// persistida del panel (<see cref="CargadorPoliticasDesdeConfiguracion"/>); en pruebas y en el
/// walking skeleton se usa una lista fija (<see cref="CargadorPoliticasFijas"/>). Antes de R-config
/// el motor recibía una <c>IReadOnlyList&lt;Politica&gt;</c> hardcodeada en el contenedor; ahora la
/// configuración del panel (eventos/grupos/reglas/acciones) DIRIGE realmente la moderación.
/// </summary>
public interface ICargadorPoliticas
{
    /// <summary>
    /// Devuelve las políticas activas del servidor indicado, sin un orden garantizado (el motor las
    /// ordena por prioridad, RN-04). Una configuración inconsistente o incompleta no debe tumbar el
    /// pipeline: el cargador omite lo que no puede materializar y devuelve lo válido.
    /// </summary>
    Task<IReadOnlyList<Politica>> CargarAsync(Snowflake servidorId, CancellationToken ct = default);
}

/// <summary>
/// Cargador que devuelve SIEMPRE la misma lista de políticas, sin importar el servidor. Es el puente
/// de compatibilidad con el camino previo (lista fija de políticas): lo usan las pruebas del motor y
/// el walking skeleton, que construyen sus políticas en código. En producción se usa el cargador
/// desde configuración.
/// </summary>
public sealed class CargadorPoliticasFijas : ICargadorPoliticas
{
    private readonly IReadOnlyList<Politica> _politicas;

    public CargadorPoliticasFijas(IReadOnlyList<Politica> politicas)
        => _politicas = politicas ?? throw new ArgumentNullException(nameof(politicas));

    public Task<IReadOnlyList<Politica>> CargarAsync(Snowflake servidorId, CancellationToken ct = default)
        => Task.FromResult(_politicas);
}
