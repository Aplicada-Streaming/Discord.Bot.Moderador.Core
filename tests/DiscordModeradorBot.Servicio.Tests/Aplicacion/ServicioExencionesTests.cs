using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using FluentAssertions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del servicio de gestión de exenciones (R5, CU-15): el identificador se valida como
/// snowflake AL GUARDAR (RN-08); un identificador inválido se rechaza con
/// EXENCION_IDENTIFICADOR_INVALIDO y NO se persiste (CU-15 CA-03). Un duplicado se rechaza con
/// EXENCION_DUPLICADA. Un alta válida se persiste y se devuelve.
/// </summary>
public sealed class ServicioExencionesTests
{
    private static readonly Snowflake Servidor = new("100000000000000001");

    private readonly IRepositorioExenciones _repositorio = Substitute.For<IRepositorioExenciones>();

    private ServicioExenciones CrearServicio() => new(_repositorio);

    [Fact]
    public async Task Identificador_invalido_se_rechaza_y_no_persiste()
    {
        // Given un identificador que no es un snowflake válido (CU-15 CA-03, RN-08).
        var servicio = CrearServicio();

        // When se intenta agregar la exención.
        var resultado = await servicio.AgregarAsync(Servidor, TipoSujetoExento.Rol, "no-es-snowflake");

        // Then falla con EXENCION_IDENTIFICADOR_INVALIDO y NO se persiste nada.
        resultado.Exito.Should().BeFalse();
        resultado.Codigo.Should().Be(ServicioExenciones.CodigoIdentificadorInvalido);
        resultado.Mensaje.Should().NotBeNullOrWhiteSpace();
        await _repositorio.DidNotReceive().AgregarAsync(
            Arg.Any<Snowflake>(), Arg.Any<Exencion>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Identificador_valido_se_persiste_y_devuelve()
    {
        // Given un repositorio que confirma la creación (no duplicado).
        _repositorio
            .AgregarAsync(Arg.Any<Snowflake>(), Arg.Any<Exencion>(), Arg.Any<CancellationToken>())
            .Returns(true);
        var servicio = CrearServicio();

        // When se agrega una exención por rol con un snowflake válido (CU-15 CA-01).
        var resultado = await servicio.AgregarAsync(Servidor, TipoSujetoExento.Rol, "700000000000000001");

        // Then se acepta, devuelve la exención y la persiste.
        resultado.Exito.Should().BeTrue();
        resultado.Exencion.Should().NotBeNull();
        resultado.Exencion!.Tipo.Should().Be(TipoSujetoExento.Rol);
        resultado.Exencion.Sujeto.Valor.Should().Be("700000000000000001");
        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Snowflake>(s => s.Valor == Servidor.Valor),
            Arg.Is<Exencion>(e => e.Tipo == TipoSujetoExento.Rol && e.Sujeto.Valor == "700000000000000001"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Exencion_duplicada_se_rechaza_con_su_codigo()
    {
        // Given un repositorio que indica que ya existía (no se crea duplicado).
        _repositorio
            .AgregarAsync(Arg.Any<Snowflake>(), Arg.Any<Exencion>(), Arg.Any<CancellationToken>())
            .Returns(false);
        var servicio = CrearServicio();

        // When se intenta agregar la misma exención.
        var resultado = await servicio.AgregarAsync(Servidor, TipoSujetoExento.Usuario, "200000000000000002");

        // Then falla con EXENCION_DUPLICADA (CU-15 §6).
        resultado.Exito.Should().BeFalse();
        resultado.Codigo.Should().Be(ServicioExenciones.CodigoDuplicada);
    }
}
