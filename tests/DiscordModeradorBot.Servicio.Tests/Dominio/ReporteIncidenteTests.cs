using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas de la composición del reporte de incidente (CU-05, R6 / TC-60): el reporte derivado
/// de un incidente conserva su resultado y, cuando el incidente quedó no accionable por
/// jerarquía o permisos (RN-01), el reporte lo señala (advertencia de no accionable) sin perder
/// la evidencia (RN-11).
/// </summary>
public sealed class ReporteIncidenteTests
{
    private static readonly DateTimeOffset Instante = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private static Incidente CrearIncidente(ResultadoModeracion resultado) => new(
        new Snowflake("100000000000000001"),
        new Snowflake("200000000000000002"),
        "Ráfaga distribuida",
        Modo.Ejecucion,
        TipoAccion.BaneoConBorradoRetroactivo,
        resultado,
        new[]
        {
            new MensajeAccionado(
                new Snowflake("400000000000000001"), new Snowflake("300000000000000001"), "spam"),
        },
        new[] { new Snowflake("300000000000000001") },
        Instante);

    [Fact]
    public void Un_incidente_no_accionable_compone_un_reporte_con_la_advertencia()
    {
        // Given un incidente no accionable por jerarquía superior del emisor (TC-60, RN-01).
        var incidente = CrearIncidente(ResultadoModeracion.NoAccionable);

        // When se compone el reporte.
        var reporte = ReporteIncidente.DesdeIncidente(incidente);

        // Then el reporte señala que es no accionable y conserva la evidencia (RN-11).
        reporte.Resultado.Should().Be(ResultadoModeracion.NoAccionable);
        reporte.EsNoAccionable.Should().BeTrue();
        reporte.MensajesAccionados.Should().ContainSingle();
        reporte.CanalesAfectados.Should().ContainSingle();
    }

    [Fact]
    public void Un_incidente_ejecutado_no_marca_no_accionable()
    {
        // Given un incidente ejecutado normalmente.
        var incidente = CrearIncidente(ResultadoModeracion.Ejecutada);

        // When se compone el reporte.
        var reporte = ReporteIncidente.DesdeIncidente(incidente);

        // Then el reporte no lleva la advertencia de no accionable.
        reporte.EsNoAccionable.Should().BeFalse();
        reporte.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
    }
}
