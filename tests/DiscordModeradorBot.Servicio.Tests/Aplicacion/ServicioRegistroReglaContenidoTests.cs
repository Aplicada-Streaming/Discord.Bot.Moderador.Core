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

    [Fact]
    public async Task Palabras_clave_validas_se_aceptan_y_persisten_con_su_clase()
    {
        // Given una lista de palabras clave. When se registra (CU-04, TipoCriterioContenido.PalabrasClave).
        var servicio = CrearServicio();

        var resultado = await servicio.RegistrarPorPalabrasClaveAsync(
            Servidor, "Contenido prohibido", "Insultos", "idiota, tarado");

        // Then se acepta como regla de palabras clave y se persiste con su política y servidor.
        resultado.Exito.Should().BeTrue();
        resultado.Regla!.TipoCriterio.Should().Be(TipoCriterioContenido.PalabrasClave);
        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Snowflake>(s => s.Valor == Servidor.Valor),
            "Contenido prohibido",
            Arg.Is<ReglaContenido>(r => r.Nombre == "Insultos"
                && r.TipoCriterio == TipoCriterioContenido.PalabrasClave),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Palabras_clave_vacias_se_rechazan_al_guardar_y_no_persisten()
    {
        // Given una lista sin términos útiles (solo separadores): RN-03.
        var servicio = CrearServicio();

        var resultado = await servicio.RegistrarPorPalabrasClaveAsync(
            Servidor, "Contenido prohibido", "Vacia", "  , \n ");

        resultado.Exito.Should().BeFalse();
        resultado.Codigo.Should().Be(ReglaContenidoInvalidaException.CodigoPatronInvalido);
        await _repositorio.DidNotReceive().AgregarAsync(
            Arg.Any<Snowflake>(), Arg.Any<string>(), Arg.Any<ReglaContenido>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_por_regex_valida_persiste_la_regla_reconstruida()
    {
        var servicio = CrearServicio();
        _repositorio.ActualizarAsync(Arg.Any<int>(), Arg.Any<ReglaContenido>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var resultado = await servicio.ActualizarPorExpresionRegularAsync(7, "Enlaces", @"\d{3}", sensibleAMayusculas: true);

        resultado.Exito.Should().BeTrue();
        await _repositorio.Received(1).ActualizarAsync(
            7, Arg.Is<ReglaContenido>(r => r.Nombre == "Enlaces" && r.SensibleAMayusculas), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_por_regex_invalida_no_persiste()
    {
        var servicio = CrearServicio();

        var resultado = await servicio.ActualizarPorExpresionRegularAsync(7, "Rota", "(abc");

        resultado.Exito.Should().BeFalse();
        resultado.Codigo.Should().Be(ReglaContenidoInvalidaException.CodigoPatronInvalido);
        await _repositorio.DidNotReceive().ActualizarAsync(
            Arg.Any<int>(), Arg.Any<ReglaContenido>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_por_palabras_clave_valida_persiste_con_su_clase()
    {
        var servicio = CrearServicio();
        _repositorio.ActualizarAsync(Arg.Any<int>(), Arg.Any<ReglaContenido>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var resultado = await servicio.ActualizarPorPalabrasClaveAsync(7, "Insultos", "idiota, tarado");

        resultado.Exito.Should().BeTrue();
        await _repositorio.Received(1).ActualizarAsync(
            7, Arg.Is<ReglaContenido>(r => r.TipoCriterio == TipoCriterioContenido.PalabrasClave),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_una_regla_inexistente_devuelve_falla()
    {
        var servicio = CrearServicio();
        _repositorio.ActualizarAsync(Arg.Any<int>(), Arg.Any<ReglaContenido>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var resultado = await servicio.ActualizarPorExpresionRegularAsync(999, "X", @"\d");

        resultado.Exito.Should().BeFalse();
        resultado.Codigo.Should().Be("CONFIG_REGLA_NO_ENCONTRADA");
    }
}
