using DiscordModeradorBot.Servicio.Infraestructura.Seguridad;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas del resguardo de contraseña PBKDF2 en formato PHC (TC-30, ADR-03, RN-13). El hash
/// es auto-descriptivo (algoritmo, iteraciones, salt y hash), verifica sin comparar en claro y
/// dos resguardos de la misma contraseña difieren por el salt aleatorio.
/// </summary>
public sealed class ServicioHashContrasenaTests
{
    // Iteraciones reducidas para tests rápidos; el formato y el comportamiento son los mismos.
    private readonly ServicioHashContrasenaPbkdf2 _hash = new(iteraciones: 1_000);

    [Fact]
    public void Hashear_produce_un_resguardo_en_formato_PHC()
    {
        // Given una contraseña.
        const string contrasena = "una-clave-robusta-123";

        // When se deriva su resguardo.
        var resguardo = _hash.Hashear(contrasena);

        // Then está en formato PHC: $pbkdf2-sha256$i=<iteraciones>$<salt_b64>$<hash_b64> (ADR-03).
        resguardo.Should().StartWith($"${ServicioHashContrasenaPbkdf2.Algoritmo}$i=1000$");
        resguardo.Split('$').Should().HaveCount(5);
        // No contiene la contraseña en claro (RN-13).
        resguardo.Should().NotContain(contrasena);
    }

    [Fact]
    public void Verificar_acepta_la_contrasena_correcta()
    {
        // Given un resguardo de una contraseña.
        const string contrasena = "una-clave-robusta-123";
        var resguardo = _hash.Hashear(contrasena);

        // When se verifica con la misma contraseña.
        var verifica = _hash.Verificar(contrasena, resguardo);

        // Then verifica correctamente (CU-09 CA-01).
        verifica.Should().BeTrue();
    }

    [Fact]
    public void Verificar_rechaza_la_contrasena_incorrecta()
    {
        // Given un resguardo de una contraseña.
        var resguardo = _hash.Hashear("una-clave-robusta-123");

        // When se verifica con otra contraseña.
        var verifica = _hash.Verificar("otra-clave-distinta-456", resguardo);

        // Then no verifica (CU-09 CA-02).
        verifica.Should().BeFalse();
    }

    [Fact]
    public void Dos_hashes_de_la_misma_contrasena_difieren_por_el_salt()
    {
        // Given la misma contraseña hasheada dos veces.
        const string contrasena = "una-clave-robusta-123";

        var primero = _hash.Hashear(contrasena);
        var segundo = _hash.Hashear(contrasena);

        // Then los resguardos difieren (salt aleatorio, ADR-03), pero ambos verifican.
        primero.Should().NotBe(segundo);
        _hash.Verificar(contrasena, primero).Should().BeTrue();
        _hash.Verificar(contrasena, segundo).Should().BeTrue();
    }

    [Fact]
    public void Verificar_rechaza_un_resguardo_con_formato_invalido()
    {
        // Given un resguardo que no está en formato PHC.
        // When se verifica.
        // Then falla de forma segura, sin lanzar (RN-13).
        _hash.Verificar("clave", "no-es-un-resguardo-phc").Should().BeFalse();
        _hash.Verificar("clave", string.Empty).Should().BeFalse();
    }
}
