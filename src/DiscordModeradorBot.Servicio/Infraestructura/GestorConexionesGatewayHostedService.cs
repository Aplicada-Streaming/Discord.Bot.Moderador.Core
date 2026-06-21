using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>
/// Servicio en segundo plano del modo de gateway REAL (Moderacion:Gateway = Discord, ADR-13,
/// CU-13). Al iniciar: aplica las migraciones, construye el gestor de conexiones, cablea el ruteo
/// de cada mensaje entrante al <see cref="MotorDeModeracion"/> del contexto (un alcance por mensaje,
/// porque el motor y sus repositorios son scoped) y abre las conexiones de los servidores activos
/// (descifrando sus tokens en memoria, RN-14). NO inyecta mensajes simulados: el tráfico llega del
/// gateway real. Reemplaza al <see cref="WalkingSkeletonHostedService"/> en modo Discord; el modo
/// por defecto sigue siendo Simulado (no se rompe dev/tests).
/// </summary>
public sealed class GestorConexionesGatewayHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GestorConexionesGatewayHostedService> _logger;
    private GestorConexionesGateway? _gestor;

    public GestorConexionesGatewayHostedService(
        IServiceProvider serviceProvider, ILogger<GestorConexionesGatewayHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Migraciones al arranque (ADR-02), igual que en el modo simulado.
        using (var scope = _serviceProvider.CreateScope())
        {
            var contexto = scope.ServiceProvider.GetRequiredService<ContextoPersistencia>();
            await contexto.Database.MigrateAsync(stoppingToken);
        }

        // El gestor es singleton: vive lo que vive el host. El ruteo abre un alcance por mensaje y
        // resuelve el motor del contexto (scoped). El token jamás se loguea (RN-14).
        _gestor = _serviceProvider.GetRequiredService<GestorConexionesGateway>();
        _gestor.EnrutadorMensaje = EnrutarAlMotorAsync;

        _logger.LogInformation(
            "[GATEWAY] Modo Discord activo: abriendo conexiones de los servidores activos (ADR-13, " +
            "CU-13). No se inyectan mensajes simulados.");

        await _gestor.IniciarAsync(stoppingToken);
    }

    /// <summary>
    /// Enruta un mensaje entrante al motor de moderación del contexto. Abre un alcance de DI por
    /// mensaje (el motor y los repositorios son scoped) y delega el pipeline (ADR-01, ADR-13). Una
    /// falla no debe cerrar la conexión: el gestor ya envuelve esta llamada y la registra (ADR-08).
    /// </summary>
    private async Task EnrutarAlMotorAsync(Snowflake servidorId, MensajeEntrante mensaje, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var motor = scope.ServiceProvider.GetRequiredService<MotorDeModeracion>();
        await motor.ProcesarAsync(mensaje, ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_gestor is not null)
        {
            await _gestor.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
