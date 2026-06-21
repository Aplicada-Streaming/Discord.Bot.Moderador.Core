using DiscordModeradorBot.Servicio.Dominio.Gateway;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas de la clasificación PURA del resultado de una acción (RN-01, ADR-08), aislada del SDK:
/// una jerarquía superior → no accionable por jerarquía; permisos faltantes → no accionable por
/// permisos; cualquier otra falla → fallida. No toca Discord.Net ni red.
/// </summary>
public sealed class ClasificadorResultadoAccionTests
{
    [Theory]
    [InlineData(TipoFallaAccion.Ninguna, ResultadoAccion.Ejecutada)]
    [InlineData(TipoFallaAccion.JerarquiaSuperior, ResultadoAccion.NoAccionablePorJerarquia)]
    [InlineData(TipoFallaAccion.PermisosFaltantes, ResultadoAccion.NoAccionablePorPermisos)]
    [InlineData(TipoFallaAccion.FallaPlataforma, ResultadoAccion.Fallida)]
    public void Clasifica_cada_naturaleza_de_falla_al_resultado_esperado(
        TipoFallaAccion falla, ResultadoAccion esperado)
    {
        ClasificadorResultadoAccion.Clasificar(falla).Should().Be(esperado);
    }

    [Fact]
    public void Jerarquia_y_permisos_son_no_accionables_y_fallida_no()
    {
        ClasificadorResultadoAccion.Clasificar(TipoFallaAccion.JerarquiaSuperior)
            .EsNoAccionable().Should().BeTrue();
        ClasificadorResultadoAccion.Clasificar(TipoFallaAccion.PermisosFaltantes)
            .EsNoAccionable().Should().BeTrue();
        ClasificadorResultadoAccion.Clasificar(TipoFallaAccion.FallaPlataforma)
            .EsNoAccionable().Should().BeFalse();
    }
}
