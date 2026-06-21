using DiscordModeradorBot.Servicio.Infraestructura.Seguridad;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas del cifrado del token en reposo (TC-38, RN-14, ADR-07). No usan secretos
/// reales; la clave maestra de prueba se inyecta directamente.
/// </summary>
public sealed class ServicioCifradoTests
{
    private readonly ServicioCifradoAes _cifrado = new("clave-maestra-de-prueba");

    [Fact]
    public void Cifrar_y_descifrar_devuelve_el_token_original()
    {
        // Given un token de bot de ejemplo.
        const string token = "MTAxMjM0NTY3ODkwLabc.def.ghijklmnopqrstuvwxyz";

        // When se cifra y luego se descifra.
        var cifrado = _cifrado.Cifrar(token);
        var descifrado = _cifrado.Descifrar(cifrado);

        // Then el resultado coincide con el token original.
        descifrado.Should().Be(token);
    }

    [Fact]
    public void El_texto_cifrado_difiere_del_texto_plano()
    {
        // Given un token.
        const string token = "token-en-claro-de-prueba";

        // When se cifra.
        var cifrado = _cifrado.Cifrar(token);

        // Then el valor en reposo no es legible como el token original (RN-14).
        cifrado.Should().NotBe(token);
        cifrado.Should().NotContain(token);
    }

    [Fact]
    public void Dos_cifrados_del_mismo_token_difieren_por_el_nonce()
    {
        // Given un token cifrado dos veces.
        const string token = "token-repetido";

        var primero = _cifrado.Cifrar(token);
        var segundo = _cifrado.Cifrar(token);

        // Then los textos cifrados difieren (cifrado autenticado con nonce aleatorio),
        // pero ambos descifran al mismo valor.
        primero.Should().NotBe(segundo);
        _cifrado.Descifrar(primero).Should().Be(token);
        _cifrado.Descifrar(segundo).Should().Be(token);
    }
}
