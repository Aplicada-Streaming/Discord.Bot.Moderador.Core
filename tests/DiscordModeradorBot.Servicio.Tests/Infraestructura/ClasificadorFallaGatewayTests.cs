using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas del clasificador de señales del gateway (CU-12): distingue "credencial rechazada"
/// (token inválido: 401 / 4004 / "token invalid") de otros cierres, en particular del 4014
/// "Disallowed intent(s)" que es problema de INTENTS y NO debe reportarse como token inválido.
/// </summary>
public sealed class ClasificadorFallaGatewayTests
{
    [Theory]
    [InlineData("The server responded with error 401: 401: Unauthorized")]
    [InlineData("A supplied token was invalid. The Bot token was invalid.")]
    [InlineData("WebSocket closed: 4004 Authentication failed")]
    [InlineData("Authentication failed.")]
    public void Reconoce_el_rechazo_de_credencial_como_token_invalido(string mensaje)
    {
        ClasificadorFallaGateway.EsSenalDeTokenInvalido(mensaje).Should().BeTrue();
    }

    [Theory]
    [InlineData("WebSocket closed: 4014 Disallowed intent(s)")]
    [InlineData("Disallowed intent(s).")]
    [InlineData("Ready")]
    [InlineData("Connecting")]
    [InlineData("")]
    [InlineData(null)]
    public void No_confunde_intents_ni_mensajes_normales_con_token_invalido(string? mensaje)
    {
        // El 4014 (intents) contiene "401" como subcadena: el clasificador NO debe matchearlo.
        ClasificadorFallaGateway.EsSenalDeTokenInvalido(mensaje).Should().BeFalse();
    }
}
