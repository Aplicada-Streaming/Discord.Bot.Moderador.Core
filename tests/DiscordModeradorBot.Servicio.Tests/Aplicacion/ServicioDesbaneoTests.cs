using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas de la reversión de una contención (desbaneo, CU-07; RN-11, RN-12). Sobre un baneo
/// ejecutado no revertido, el servicio invoca DesbanearAsync del adaptador y marca el incidente
/// revertido con autor y fecha (TC-52); rechaza el desbaneo sobre una simulación (TC-54/CA-02) o
/// sobre un incidente ya revertido. Verifica con NSubstitute.
/// </summary>
public sealed class ServicioDesbaneoTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const int IncidenteId = 7;
    private const int AdministradorId = 1;
    private const string Servidor = "100000000000000001";
    private const string Usuario = "200000000000000002";

    private readonly IRepositorioIncidentes _repositorio = Substitute.For<IRepositorioIncidentes>();
    private readonly IAdaptadorGateway _adaptador = Substitute.For<IAdaptadorGateway>();
    private readonly RelojFijo _reloj = new(Base);

    private ServicioDesbaneo CrearServicio() =>
        new(_repositorio, _adaptador, _reloj, NullLogger<ServicioDesbaneo>.Instance);

    private static Incidente CrearIncidente(
        ResultadoModeracion resultado,
        TipoAccion accion,
        DateTimeOffset? reversionFecha = null,
        int? reversionAutorId = null) =>
        new(
            new Snowflake(Servidor),
            new Snowflake(Usuario),
            "Ráfaga distribuida",
            Modo.Ejecucion,
            accion,
            resultado,
            Array.Empty<MensajeAccionado>(),
            new[] { new Snowflake("300000000000000001") },
            Base,
            id: IncidenteId,
            reversionAutorId: reversionAutorId,
            reversionFecha: reversionFecha);

    [Fact]
    public async Task Desbanea_un_baneo_ejecutado_e_invoca_el_adaptador_y_marca_revertido()
    {
        // Given un incidente de baneo real, no revertido (TC-52, CU-07 CA-01).
        var incidente = CrearIncidente(
            ResultadoModeracion.Ejecutada, TipoAccion.BaneoConBorradoRetroactivo);
        _repositorio.ObtenerAsync(IncidenteId, Arg.Any<CancellationToken>()).Returns(incidente);
        _repositorio
            .MarcarRevertidoAsync(IncidenteId, AdministradorId, Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var servicio = CrearServicio();

        // When el administrador revierte el baneo.
        var resultado = await servicio.RevertirAsync(IncidenteId, AdministradorId);

        // Then se invoca DesbanearAsync del adaptador y se marca el incidente revertido con
        // autor y fecha (RN-12). El desbaneo no restaura mensajes (RN-11): no se invoca borrado.
        resultado.Exito.Should().BeTrue();
        await _adaptador.Received(1).DesbanearAsync(
            Arg.Is<Snowflake>(s => s.Valor == Servidor),
            Arg.Is<Snowflake>(u => u.Valor == Usuario),
            Arg.Any<CancellationToken>());
        await _repositorio.Received(1).MarcarRevertidoAsync(
            IncidenteId, AdministradorId, Base, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Rechaza_el_desbaneo_sobre_un_incidente_inexistente()
    {
        // Given un incidente que no existe.
        _repositorio.ObtenerAsync(IncidenteId, Arg.Any<CancellationToken>()).Returns((Incidente?)null);
        var servicio = CrearServicio();

        // When se intenta revertir.
        var resultado = await servicio.RevertirAsync(IncidenteId, AdministradorId);

        // Then se rechaza y no se invoca el adaptador (INCIDENTE_NO_ENCONTRADO).
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorDesbaneo.IncidenteNoEncontrado);
        await _adaptador.DidNotReceive().DesbanearAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Rechaza_el_desbaneo_sobre_una_simulacion()
    {
        // Given un incidente en simulación (TC-54, CU-07 CA-02, RN-09).
        var incidente = CrearIncidente(
            ResultadoModeracion.Simulada, TipoAccion.BaneoConBorradoRetroactivo);
        _repositorio.ObtenerAsync(IncidenteId, Arg.Any<CancellationToken>()).Returns(incidente);
        var servicio = CrearServicio();

        // When se intenta revertir.
        var resultado = await servicio.RevertirAsync(IncidenteId, AdministradorId);

        // Then se rechaza: una simulación no es un baneo reversible; no se invoca el adaptador.
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorDesbaneo.NoEsBaneoReversible);
        await _adaptador.DidNotReceive().DesbanearAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<CancellationToken>());
        await _repositorio.DidNotReceive().MarcarRevertidoAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Rechaza_el_desbaneo_sobre_un_incidente_ya_revertido()
    {
        // Given un baneo real que ya fue revertido antes (CU-07).
        var incidente = CrearIncidente(
            ResultadoModeracion.Ejecutada,
            TipoAccion.BaneoConBorradoRetroactivo,
            reversionFecha: Base.AddMinutes(-5),
            reversionAutorId: AdministradorId);
        _repositorio.ObtenerAsync(IncidenteId, Arg.Any<CancellationToken>()).Returns(incidente);
        var servicio = CrearServicio();

        // When se intenta revertir de nuevo.
        var resultado = await servicio.RevertirAsync(IncidenteId, AdministradorId);

        // Then se rechaza (YaRevertido) y no se invoca el adaptador ni se vuelve a marcar.
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorDesbaneo.YaRevertido);
        await _adaptador.DidNotReceive().DesbanearAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<CancellationToken>());
        await _repositorio.DidNotReceive().MarcarRevertidoAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }
}
