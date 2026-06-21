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

    [Fact]
    public async Task Registra_las_acciones_adicionales_de_R6_con_sus_parametros()
    {
        // Given el adaptador simulado (R6: timeout, expulsión, asignar/quitar rol).
        var adaptador = new AdaptadorGatewaySimulado(NullLogger<AdaptadorGatewaySimulado>.Instance);
        var servidor = new Snowflake("100000000000000001");
        var usuario = new Snowflake("200000000000000002");
        var rol = new Snowflake("700000000000000003");

        // When se ejecutan las acciones adicionales.
        var rTimeout = await adaptador.AplicarTimeoutAsync(servidor, usuario, TimeSpan.FromMinutes(15));
        var rExpulsion = await adaptador.ExpulsarAsync(servidor, usuario);
        var rAsignar = await adaptador.AsignarRolAsync(servidor, usuario, rol);
        var rQuitar = await adaptador.QuitarRolAsync(servidor, usuario, rol);

        // Then todas devuelven Ejecutada y quedan registradas con sus parámetros (RN-05).
        rTimeout.Should().Be(ResultadoAccion.Ejecutada);
        rExpulsion.Should().Be(ResultadoAccion.Ejecutada);
        rAsignar.Should().Be(ResultadoAccion.Ejecutada);
        rQuitar.Should().Be(ResultadoAccion.Ejecutada);

        adaptador.AccionesEjecutadas.Should().HaveCount(4);
        adaptador.AccionesEjecutadas[0].Should().BeOfType<AdaptadorGatewaySimulado.TimeoutEjecutado>()
            .Which.Duracion.Should().Be(TimeSpan.FromMinutes(15));
        adaptador.AccionesEjecutadas[1].Should().BeOfType<AdaptadorGatewaySimulado.ExpulsionEjecutada>();
        adaptador.AccionesEjecutadas[2].Should().BeOfType<AdaptadorGatewaySimulado.RolAsignado>()
            .Which.Rol.Valor.Should().Be(rol.Valor);
        adaptador.AccionesEjecutadas[3].Should().BeOfType<AdaptadorGatewaySimulado.RolQuitado>()
            .Which.Rol.Valor.Should().Be(rol.Valor);
    }

    [Fact]
    public async Task Un_usuario_de_rol_superior_devuelve_no_accionable_y_no_registra_la_accion()
    {
        // Given un usuario marcado como de rol jerárquicamente superior (R6, RN-01, CU-02 §7).
        var adaptador = new AdaptadorGatewaySimulado(NullLogger<AdaptadorGatewaySimulado>.Instance);
        var servidor = new Snowflake("100000000000000001");
        var usuario = new Snowflake("200000000000000002");
        adaptador.MarcarUsuarioDeRolSuperior(servidor, usuario);

        // When se intenta banear y aplicar un timeout sobre él.
        var rBaneo = await adaptador.BanearConBorradoAsync(servidor, usuario, TimeSpan.FromDays(1));
        var rTimeout = await adaptador.AplicarTimeoutAsync(servidor, usuario, TimeSpan.FromMinutes(5));

        // Then las acciones de contención devuelven no accionable por jerarquía y NO se registran.
        rBaneo.Should().Be(ResultadoAccion.NoAccionablePorJerarquia);
        rTimeout.Should().Be(ResultadoAccion.NoAccionablePorJerarquia);
        adaptador.AccionesEjecutadas.Should().BeEmpty();
    }
}
