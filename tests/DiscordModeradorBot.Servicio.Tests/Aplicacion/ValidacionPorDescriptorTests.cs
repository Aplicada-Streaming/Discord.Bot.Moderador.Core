using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas de la validación dirigida por descriptor (R7, CU-11, RN-10, ADR-12). Los límites se
/// toman del descriptor (fuente única de verdad), no se hardcodean: un valor fuera de límites se
/// rechaza con CONFIG_VALOR_FUERA_DE_LIMITE o se normaliza según la política definida; un valor
/// válido se acepta tal cual.
/// </summary>
public sealed class ValidacionPorDescriptorTests
{
    private static DescriptorParametro<int> Umbral => RegistroDescriptores.UmbralCanalesDistintos;

    [Fact]
    public void Valor_dentro_de_limites_se_acepta()
    {
        // El umbral 4 está dentro de [2,10] (CU-11 CA-01, TC-39).
        var resultado = ServicioConfiguracionModeracion.ValidarEntero(Umbral, 4);

        resultado.Valido.Should().BeTrue();
        resultado.ValorEfectivo.Should().Be(4);
        resultado.Codigo.Should().BeNull();
    }

    [Fact]
    public void Valor_fuera_de_limites_se_rechaza_con_codigo_y_limites()
    {
        // El umbral 1 está por debajo del mínimo 2 (CU-11 CA-02, TC-40): se rechaza con el código.
        var resultado = ServicioConfiguracionModeracion.ValidarEntero(Umbral, 1);

        resultado.Valido.Should().BeFalse();
        resultado.Codigo.Should().Be(ServicioConfiguracionModeracion.CodigoValorFueraDeLimite);
        // El mensaje cita los límites del descriptor (no hardcodeados).
        resultado.Mensaje.Should().Contain(Umbral.Minimo.ToString());
        resultado.Mensaje.Should().Contain(Umbral.Maximo.ToString());
    }

    [Fact]
    public void Valor_fuera_de_limites_se_normaliza_cuando_la_politica_es_tope()
    {
        // Con política de normalización (tope), un valor por encima del máximo se acota al máximo
        // (semántica de la ventana de borrado, RN-02), no se rechaza.
        var borrado = RegistroDescriptores.VentanaBorradoRetroactivoDias;
        var resultado = ServicioConfiguracionModeracion.ValidarEntero(borrado, 99, normalizar: true);

        resultado.Valido.Should().BeTrue();
        resultado.ValorEfectivo.Should().Be(borrado.Maximo);
    }

    [Fact]
    public void El_descriptor_es_la_fuente_de_los_limites()
    {
        // RN-10: el límite efectivo aplicado es exactamente el del descriptor (no un literal aparte).
        Umbral.EsValido(Umbral.Maximo).Should().BeTrue();
        Umbral.EsValido(Umbral.Maximo + 1).Should().BeFalse();
        Umbral.EsValido(Umbral.Minimo - 1).Should().BeFalse();
    }

    private static DescriptorParametro<double> Ventana => RegistroDescriptores.VentanaDeteccionSegundos;

    [Fact]
    public void Valor_decimal_dentro_de_limites_se_acepta()
    {
        // La ventana 4.0 s está dentro de [0.5, 60.0] (CU-11 CA-01).
        var resultado = ServicioConfiguracionModeracion.ValidarDecimal(Ventana, 4.0);

        resultado.Valido.Should().BeTrue();
        resultado.ValorEfectivo.Should().Be(4.0);
        resultado.Codigo.Should().BeNull();
    }

    [Fact]
    public void Valor_decimal_fuera_de_limites_se_rechaza_con_codigo_y_limites()
    {
        // 0.1 s está por debajo del mínimo 0.5 (CU-11 CA-02): se rechaza con el código y los límites.
        var resultado = ServicioConfiguracionModeracion.ValidarDecimal(Ventana, 0.1);

        resultado.Valido.Should().BeFalse();
        resultado.Codigo.Should().Be(ServicioConfiguracionModeracion.CodigoValorFueraDeLimite);
        resultado.Mensaje.Should().Contain(Ventana.Minimo.ToString());
        resultado.Mensaje.Should().Contain(Ventana.Maximo.ToString());
    }

    [Fact]
    public void Valor_decimal_fuera_de_limites_se_normaliza_al_tope_del_descriptor()
    {
        // Con normalización (tope), un valor por encima del máximo se acota al máximo del descriptor.
        var resultado = ServicioConfiguracionModeracion.ValidarDecimal(Ventana, 999.0, normalizar: true);

        resultado.Valido.Should().BeTrue();
        resultado.ValorEfectivo.Should().Be(Ventana.Maximo);
    }
}
