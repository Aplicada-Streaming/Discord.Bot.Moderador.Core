using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Benchmarks.Dobles;

/// <summary>
/// Adaptador del gateway NO-OP para el benchmark: toda acción devuelve <see cref="ResultadoAccion.Ejecutada"/>
/// de inmediato (Task ya completada), sin red, sin token y sin logging. A diferencia del
/// <c>AdaptadorGatewaySimulado</c> del producto (que loguea cada acción y guarda historial en
/// memoria), este doble NO introduce overhead de E/S ni de logging: así el benchmark mide el motor
/// de evaluación y no la instrumentación del adaptador.
/// </summary>
internal sealed class AdaptadorGatewayNoOp : IAdaptadorGateway
{
    public event Func<MensajeEntrante, Task>? MensajeRecibido;

    // El evento queda declarado por el contrato del puerto; el benchmark invoca el motor
    // directamente, no por el flujo de eventos. Se referencia para no disparar warning de
    // "evento nunca usado" bajo -warnaserror.
    public void DispararMensajeRecibido(MensajeEntrante mensaje) => MensajeRecibido?.Invoke(mensaje);

    public Task<ResultadoPruebaConfiguracion> ProbarConfiguracionAsync(
        SolicitudPruebaConfiguracion solicitud, CancellationToken ct = default) =>
        Task.FromResult(new ResultadoPruebaConfiguracion(Array.Empty<ChequeoConfiguracion>()));

    public Task<ResultadoAccion> ReportarAsync(
        CanalDeSalida canalSalida, ReporteIncidente reporte, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);

    public Task<ResultadoAccion> BanearConBorradoAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan ventanaBorrado, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);

    public Task<ResultadoAccion> DesbanearAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);

    public Task<ResultadoAccion> AplicarTimeoutAsync(
        Snowflake servidorId, Snowflake usuarioId, TimeSpan duracion, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);

    public Task<ResultadoAccion> ExpulsarAsync(
        Snowflake servidorId, Snowflake usuarioId, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);

    public Task<ResultadoAccion> AsignarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);

    public Task<ResultadoAccion> QuitarRolAsync(
        Snowflake servidorId, Snowflake usuarioId, Snowflake rol, CancellationToken ct = default) =>
        Task.FromResult(ResultadoAccion.Ejecutada);
}
