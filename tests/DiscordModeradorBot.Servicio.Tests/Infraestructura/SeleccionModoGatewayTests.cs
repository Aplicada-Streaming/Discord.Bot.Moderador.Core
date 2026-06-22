using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Infraestructura;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de la SELECCIÓN del modo de gateway por la composición de dependencias (ADR-13), sin
/// conectarse a la plataforma ni inyectar mensajes. Verifican que en modo Simulado se registra el
/// adaptador simulado y el walking skeleton (y NO el gestor real), y que en modo Discord se registra
/// el adaptador real, el gestor de conexiones y su hosted service (y NO el simulado). El default es
/// Simulado para no romper dev/tests.
/// </summary>
public sealed class SeleccionModoGatewayTests
{
    private const string CadenaConexion = "DataSource=:memory:";

    private static IServiceCollection Componer(ModoGateway modo)
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.SetMinimumLevel(LogLevel.None));
        services.AgregarServiciosModeracion(CadenaConexion, modo);
        return services;
    }

    [Fact]
    public void Default_de_AgregarServiciosModeracion_es_simulado()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AgregarServiciosModeracion(CadenaConexion);

        services.Should().Contain(d => d.ServiceType == typeof(AdaptadorGatewaySimulado));
        services.Should().NotContain(d => d.ServiceType == typeof(AdaptadorGatewayDiscord));
    }

    [Fact]
    public void Modo_simulado_registra_el_simulado_y_no_el_gestor_real()
    {
        var services = Componer(ModoGateway.Simulado);

        // El adaptador simulado está registrado; el real y el gestor de conexiones, no.
        services.Should().Contain(d => d.ServiceType == typeof(AdaptadorGatewaySimulado));
        services.Should().NotContain(d => d.ServiceType == typeof(AdaptadorGatewayDiscord));
        services.Should().NotContain(d => d.ServiceType == typeof(GestorConexionesGateway));

        // El hosted service del modo simulado es el walking skeleton; no el gestor real.
        var hosted = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
        hosted.Should().Contain(d => d.ImplementationType == typeof(WalkingSkeletonHostedService));
        hosted.Should().NotContain(d => d.ImplementationType == typeof(GestorConexionesGatewayHostedService));
    }

    [Fact]
    public void Modo_discord_registra_el_adaptador_real_el_gestor_y_no_el_simulado()
    {
        var services = Componer(ModoGateway.Discord);

        // El adaptador real, su fábrica de conexiones y el gestor están registrados; el simulado, no.
        services.Should().Contain(d => d.ServiceType == typeof(AdaptadorGatewayDiscord));
        services.Should().Contain(d => d.ServiceType == typeof(IFabricaClienteGateway));
        services.Should().Contain(d => d.ServiceType == typeof(GestorConexionesGateway));
        services.Should().NotContain(d => d.ServiceType == typeof(AdaptadorGatewaySimulado));

        // El hosted service del modo Discord es el gestor de conexiones; no el walking skeleton.
        var hosted = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
        hosted.Should().Contain(d => d.ImplementationType == typeof(GestorConexionesGatewayHostedService));
        hosted.Should().NotContain(d => d.ImplementationType == typeof(WalkingSkeletonHostedService));
    }

    [Fact]
    public async Task Modo_discord_construye_el_contenedor_validando_scopes()
    {
        // Regresión (DI): el gestor de conexiones es SINGLETON y no debe capturar el repositorio de
        // servidores, que es SCOPED (dependencia cautiva del DbContext). Se construye el contenedor
        // con la MISMA validación que aplica el host en Development (ValidateScopes + ValidateOnBuild):
        // antes del fix, esto lanzaba "Cannot consume scoped service ... from singleton ...".
        var services = Componer(ModoGateway.Discord);

        ServiceProvider? proveedor = null;
        var construir = () => proveedor = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });

        construir.Should().NotThrow();

        if (proveedor is not null)
        {
            await proveedor.DisposeAsync();
        }
    }

    [Fact]
    public async Task En_ambos_modos_se_resuelve_un_unico_IAdaptadorGateway()
    {
        // En modo simulado el puerto resuelve al simulado.
        await using (var spSimulado = Componer(ModoGateway.Simulado).BuildServiceProvider())
        {
            spSimulado.GetRequiredService<IAdaptadorGateway>().Should().BeOfType<AdaptadorGatewaySimulado>();
        }

        // En modo Discord el puerto resuelve al adaptador real (sin conectarse: solo se construye).
        // El adaptador es IAsyncDisposable, por eso el proveedor se libera con await using.
        await using (var spDiscord = Componer(ModoGateway.Discord).BuildServiceProvider())
        {
            spDiscord.GetRequiredService<IAdaptadorGateway>().Should().BeOfType<AdaptadorGatewayDiscord>();
        }
    }
}
