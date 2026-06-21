using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>
/// Servicio en segundo plano que demuestra el walking skeleton end-to-end (R1, intake §18
/// sample b): al iniciar aplica la migración, registra un servidor de demostración con
/// token de ejemplo cifrado (CU-10), suscribe el motor de moderación al adaptador
/// simulado, e inyecta una ráfaga (un mismo usuario posteando en N canales distintos
/// dentro de la ventana) para que el motor la procese en modo simulación (CU-01, CU-14),
/// persistiendo el incidente simulado y logueando el reporte de "lo que se ejecutaría"
/// (RN-09).
/// </summary>
public sealed class WalkingSkeletonHostedService : BackgroundService
{
    private const string ServidorDemo = "100000000000000001";
    private const string UsuarioDemo = "200000000000000002";

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

        // Aplicar la migración inicial al arranque (MIG-0001, ADR-02).
        var contexto = sp.GetRequiredService<ContextoPersistencia>();
        await contexto.Database.MigrateAsync(stoppingToken);

        // Registrar el servidor de demostración con su token de ejemplo cifrado (CU-10).
        var registro = sp.GetRequiredService<ServicioRegistroServidor>();
        var resultado = await registro.RegistrarAsync(
            ServidorDemo, "token-de-ejemplo-walking-skeleton", "Servidor de demostración", stoppingToken);

        if (resultado.Exito)
        {
            _logger.LogInformation(
                "[WALKING SKELETON] Servidor de demostración {Servidor} registrado con token cifrado.",
                ServidorDemo);
        }
        else
        {
            _logger.LogInformation(
                "[WALKING SKELETON] Servidor de demostración {Servidor} ya estaba registrado ({Error}).",
                ServidorDemo, resultado.Error);
        }

        // Cablear el motor de moderación al adaptador simulado.
        var motor = sp.GetRequiredService<MotorDeModeracion>();
        var adaptador = sp.GetRequiredService<IAdaptadorGateway>();
        adaptador.MensajeRecibido += mensaje => motor.ProcesarAsync(mensaje, stoppingToken);

        // Inyectar una ráfaga distribuida: el mismo usuario postea en 3 canales distintos
        // dentro de la ventana de detección (CU-01). En modo simulación (RN-09).
        if (adaptador is AdaptadorGatewaySimulado simulado)
        {
            _logger.LogInformation(
                "[WALKING SKELETON] Inyectando ráfaga distribuida simulada del usuario {Usuario}.",
                UsuarioDemo);

            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };

            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(ServidorDemo),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioDemo),
                    new Snowflake($"4000000000000000{i:00}"),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje de ráfaga {i + 1}");

                await simulado.InyectarMensajeAsync(mensaje);
            }

            _logger.LogInformation(
                "[WALKING SKELETON] Ráfaga procesada. Revise el incidente simulado en /incidentes.");
        }
    }
}
