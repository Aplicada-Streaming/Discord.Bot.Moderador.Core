using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas del adaptador simulado (R2): registra las acciones ejecutadas (reporte con su
/// contenido y baneo con su ventana) en orden, para verificación y demo del walking skeleton
/// (RN-05, CU-05).
/// </summary>
public sealed class AdaptadorGatewaySimuladoTests
{
    [Fact]
    public async Task Registra_reporte_y_baneo_ejecutados_en_orden()
    {
        // Given el adaptador simulado.
        var adaptador = new AdaptadorGatewaySimulado(NullLogger<AdaptadorGatewaySimulado>.Instance);
        var canal = new CanalDeSalida(
            new Snowflake("500000000000000001"), CanalDeSalida.PropositoReporteIncidentes);
        var reporte = new ReporteIncidente(
            new Snowflake("100000000000000001"),
            new Snowflake("200000000000000002"),
            "Ráfaga distribuida",
            TipoAccion.BaneoConBorradoRetroactivo,
            EsSimulacion: false,
            new[]
            {
                new MensajeAccionado(
                    new Snowflake("400000000000000001"), new Snowflake("300000000000000001"), "spam"),
            },
            new[] { new Snowflake("300000000000000001") });

        // When se reporta y luego se banea.
        await adaptador.ReportarAsync(canal, reporte);
        await adaptador.BanearConBorradoAsync(
            new Snowflake("100000000000000001"), new Snowflake("200000000000000002"), TimeSpan.FromDays(1));

        // Then ambas acciones quedan registradas en orden con sus argumentos.
        adaptador.AccionesEjecutadas.Should().HaveCount(2);
        adaptador.AccionesEjecutadas[0].Should().BeOfType<AdaptadorGatewaySimulado.ReporteEjecutado>()
            .Which.Reporte.MensajesAccionados.Should().ContainSingle();

        var baneo = adaptador.AccionesEjecutadas[1].Should()
            .BeOfType<AdaptadorGatewaySimulado.BaneoEjecutado>().Subject;
        baneo.VentanaBorrado.Should().Be(TimeSpan.FromDays(1));
        baneo.UsuarioId.Valor.Should().Be("200000000000000002");
    }

    [Fact]
    public async Task Registra_el_desbaneo_ejecutado()
    {
        // Given el adaptador simulado (CU-07).
        var adaptador = new AdaptadorGatewaySimulado(NullLogger<AdaptadorGatewaySimulado>.Instance);

        // When se desbanea a un usuario.
        await adaptador.DesbanearAsync(
            new Snowflake("100000000000000001"), new Snowflake("200000000000000002"));

        // Then queda registrado el desbaneo con sus argumentos (no toca la plataforma real).
        var desbaneo = adaptador.AccionesEjecutadas.Should().ContainSingle()
            .Which.Should().BeOfType<AdaptadorGatewaySimulado.DesbaneoEjecutado>().Subject;
        desbaneo.ServidorId.Valor.Should().Be("100000000000000001");
        desbaneo.UsuarioId.Valor.Should().Be("200000000000000002");
    }
}
