using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>
/// Servicio en segundo plano que demuestra el walking skeleton end-to-end. R1 demostraba la
/// detección de ráfaga y el reporte en modo simulación (CU-01, CU-14, RN-09). R2 extiende el
/// escenario para demostrar TAMBIÉN una política en modo ejecución: una ráfaga que dispara,
/// en orden, el reporte al canal privado (CU-05) y el baneo con borrado retroactivo
/// (CU-02/CU-03), persistiendo el incidente como ejecutado con sus mensajes y canales
/// normalizados (RN-05, RN-11). Mantiene el escenario de simulación de R1 intacto (RN-09).
/// Todo corre contra el adaptador simulado; no toca la plataforma real.
/// </summary>
public sealed class WalkingSkeletonHostedService : BackgroundService
{
    private const string ServidorSimulacion = "100000000000000001";
    private const string ServidorEjecucion = "100000000000000009";
    private const string UsuarioDemo = "200000000000000002";
    private const string CanalSalida = "500000000000000001";

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WalkingSkeletonHostedService> _logger;

    public WalkingSkeletonHostedService(
        IServiceProvider serviceProvider, ILogger<WalkingSkeletonHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        // Aplicar las migraciones al arranque (MIG-0001, MIG-0002, ADR-02).
        var contexto = sp.GetRequiredService<ContextoPersistencia>();
        await contexto.Database.MigrateAsync(stoppingToken);

        var registro = sp.GetRequiredService<ServicioRegistroServidor>();
        var adaptador = sp.GetRequiredService<IAdaptadorGateway>();

        if (adaptador is not AdaptadorGatewaySimulado simulado)
        {
            return;
        }

        // Escenario 1 — Simulación (regresión de R1): registra el servidor de demostración y
        // procesa una ráfaga en modo simulación; no se invoca ninguna acción real (RN-09).
        await RegistrarServidorAsync(
            registro, ServidorSimulacion, "Servidor de demostración (simulación)", null, stoppingToken);
        await CorrerRafagaAsync(
            sp, simulado, ServidorSimulacion, Modo.Simulacion, canalSalida: null, stoppingToken);

        // Escenario 2 — Ejecución (R2): registra un servidor con canal de salida designado y
        // procesa una ráfaga en modo ejecución; el motor reporta y luego banea, en orden
        // (RN-05), y persiste el incidente ejecutado con su evidencia normalizada (RN-11).
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        await RegistrarServidorAsync(
            registro, ServidorEjecucion, "Servidor de demostración (ejecución)", canal, stoppingToken);
        await CorrerRafagaAsync(
            sp, simulado, ServidorEjecucion, Modo.Ejecucion, canal, stoppingToken);

        _logger.LogInformation(
            "[WALKING SKELETON] Escenarios completados. Acciones ejecutadas registradas por el " +
            "adaptador simulado: {Cantidad}. Revise los incidentes en /incidentes.",
            simulado.AccionesEjecutadas.Count);
    }

    private async Task RegistrarServidorAsync(
        ServicioRegistroServidor registro,
        string servidorId,
        string nombre,
        CanalDeSalida? canal,
        CancellationToken ct)
    {
        var resultado = await registro.RegistrarAsync(
            servidorId, "token-de-ejemplo-walking-skeleton", nombre, ct, canal);

        _logger.LogInformation(
            resultado.Exito
                ? "[WALKING SKELETON] Servidor {Servidor} registrado con token cifrado{Canal}."
                : "[WALKING SKELETON] Servidor {Servidor} ya estaba registrado{Canal}.",
            servidorId,
            canal is null ? string.Empty : $" y canal de salida {canal.SnowflakeCanal.Valor}");
    }

    /// <summary>
    /// Construye un motor con una política en el modo indicado e inyecta una ráfaga
    /// distribuida (mismo usuario en 3 canales distintos dentro de la ventana de detección,
    /// CU-01). En ejecución la política reporta y luego banea, en orden (RN-05).
    /// </summary>
    private async Task CorrerRafagaAsync(
        IServiceProvider sp,
        AdaptadorGatewaySimulado simulado,
        string servidorId,
        Modo modo,
        CanalDeSalida? canalSalida,
        CancellationToken ct)
    {
        var acciones = modo == Modo.Ejecucion
            ? new[]
            {
                new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
            }
            : new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) };

        var politicas = new[]
        {
            new Politica("Ráfaga distribuida", prioridad: 0, modo: modo, acciones: acciones),
        };

        var motor = new MotorDeModeracion(
            new EstadoConductaEnMemoria(),
            sp.GetRequiredService<EvaluadorRafagaDistribuida>(),
            politicas,
            simulado,
            sp.GetRequiredService<IRepositorioIncidentes>(),
            sp.GetRequiredService<IRepositorioServidores>(),
            sp.GetRequiredService<IReloj>(),
            sp.GetRequiredService<ILogger<MotorDeModeracion>>());

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando ráfaga distribuida en modo {Modo} (servidor {Servidor}).",
            modo, servidorId);

        // Suscribe el motor al adaptador y emite la ráfaga por el canal de eventos simulado;
        // se desuscribe al terminar para no acumular handlers entre escenarios (el adaptador
        // simulado es compartido).
        Func<MensajeEntrante, Task> handler = mensaje => motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };
            var baseMensaje = modo == Modo.Ejecucion ? 4000_0000_0000_0000L : 4900_0000_0000_0000L;

            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioDemo),
                    new Snowflake((baseMensaje + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje de ráfaga {i + 1}");

                await simulado.InyectarMensajeAsync(mensaje);
            }
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }
    }
}
