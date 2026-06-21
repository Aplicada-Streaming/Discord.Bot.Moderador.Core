using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using FluentAssertions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del servicio de configuración de reglas de contenido (CU-04, RN-03, R3): el patrón se
/// valida AL GUARDAR. Un patrón inválido se rechaza con el código del CU y NO se persiste; un
/// patrón válido se acepta y se persiste (TC-14).
/// </summary>
public sealed class ServicioRegistroReglaContenidoTests
{
    private readonly IRepositorioReglasContenido _repositorio = Substitute.For<IRepositorioReglasContenido>();
    private static readonly Snowflake Servidor = new("100000000000000001");

    private ServicioRegistroReglaContenido CrearServicio() => new(_repositorio);

    [Fact]
    public async Task Patron_invalido_se_rechaza_al_guardar_y_no_persiste()
    {
        // Given un patrón regex que no compila (paréntesis sin cerrar): RN-03 / CU-04 CA-03.
        var servicio = CrearServicio();

        // When se intenta registrar la regla.
        var resultado = await servicio.RegistrarPorExpresionRegularAsync(
            Servidor, "Contenido prohibido", "Rota", "(abc");

        // Then falla con el código CONTENIDO_PATRON_INVALIDO y NO se persiste nada (RN-03).
        resultado.Exito.Should().BeFalse();
        resultado.Codigo.Should().Be(ReglaContenidoInvalidaException.CodigoPatronInvalido);
        resultado.Mensaje.Should().NotBeNullOrWhiteSpace();
        await _repositorio.DidNotReceive().AgregarAsync(
            Arg.Any<Snowflake>(), Arg.Any<string>(), Arg.Any<ReglaContenido>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Patron_valido_se_acepta_y_persiste()
    {
        // Given un patrón válido. RN-03.
        var servicio = CrearServicio();

        // When se registra.
        var resultado = await servicio.RegistrarPorExpresionRegularAsync(
            Servidor, "Contenido prohibido", "Enlace de acortador",
            @"https?://(?:bit\.ly|tinyurl\.com)/\S+");

        // Then se acepta, devuelve la regla validada y la persiste con su política y servidor.
        resultado.Exito.Should().BeTrue();
        resultado.Regla.Should().NotBeNull();
        resultado.Regla!.TipoCriterio.Should().Be(TipoCriterioContenido.ExpresionRegular);
        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Snowflake>(s => s.Valor == Servidor.Valor),
            "Contenido prohibido",
            Arg.Is<ReglaContenido>(r => r.Nombre == "Enlace de acortador"),
            Arg.Any<CancellationToken>());
    }
}
