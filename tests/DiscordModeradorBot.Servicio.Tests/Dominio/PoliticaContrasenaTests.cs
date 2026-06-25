using DiscordModeradorBot.Servicio.Dominio.Administracion;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas de la política mínima de contraseña del administrador (RN-13). La política exige una
/// longitud mínima de 8 caracteres y que sea alfanumérica (al menos una letra y un dígito). Estas
/// pruebas fijan el borde de la longitud y los requisitos de composición, que comparten el alta
/// (CU-08), el cambio de contraseña y el medidor de robustez de la UX (misma fuente de verdad).
/// </summary>
public sealed class PoliticaContrasenaTests
{
    [Fact]
    public void La_longitud_minima_es_8()
    {
        PoliticaContrasena.LongitudMinima.Should().Be(8);
    }

    [Theory]
    [InlineData("abcd1234")]   // 8 exactos, con letra y dígito
    [InlineData("clave123456")] // más larga, alfanumérica
    public void Acepta_una_contrasena_alfanumerica_de_al_menos_8(string contrasena)
    {
        PoliticaContrasena.EsRobusta(contrasena).Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]          // null
    [InlineData("")]            // vacía
    [InlineData("abc123")]      // 6: por debajo del mínimo
    [InlineData("abcd123")]     // 7: justo por debajo del mínimo
    [InlineData("abcdefgh")]    // 8 pero SIN dígito
    [InlineData("12345678")]    // 8 pero SIN letra
    public void Rechaza_lo_que_no_cumple_longitud_o_composicion(string? contrasena)
    {
        PoliticaContrasena.EsRobusta(contrasena).Should().BeFalse();
    }
}
