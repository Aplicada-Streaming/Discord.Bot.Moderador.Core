using DiscordModeradorBot.Servicio.Infraestructura;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de la decisión de sembrar el administrador de DESARROLLO (RN-13, ADR-03). El seed con
/// credenciales por defecto SOLO debe ocurrir en el entorno Development; en cualquier otro entorno
/// (Production, Staging, etc.) la decisión es falsa y el alta es el first-run real (CU-08). Se
/// testea la lógica de decisión pura, sin levantar el host.
/// </summary>
public sealed class SeedAdministradorDesarrolloTests
{
    [Fact]
    public void En_Development_se_siembra()
    {
        var entorno = new EntornoDePrueba(Environments.Development);

        WalkingSkeletonHostedService.DebeSembrarAdministradorDesarrollo(entorno).Should().BeTrue();
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    [InlineData("QA")]
    [InlineData("")]
    public void Fuera_de_Development_no_se_siembra(string nombreEntorno)
    {
        var entorno = new EntornoDePrueba(nombreEntorno);

        WalkingSkeletonHostedService.DebeSembrarAdministradorDesarrollo(entorno).Should().BeFalse();
    }

    /// <summary>Doble mínimo de <see cref="IHostEnvironment"/> que solo fija el nombre del entorno.</summary>
    private sealed class EntornoDePrueba : IHostEnvironment
    {
        public EntornoDePrueba(string environmentName) => EnvironmentName = environmentName;

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}
