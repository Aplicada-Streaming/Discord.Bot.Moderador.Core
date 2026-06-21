using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del pipeline del motor de moderación (CU-14, RN-09): en simulación registra
/// sin invocar la acción real; en ejecución invoca la acción una vez.
/// </summary>
public sealed class MotorDeModeracionTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly IAdaptadorGateway _adaptador = Substitute.For<IAdaptadorGateway>();
    private readonly IRepositorioIncidentes _repositorio = Substitute.For<IRepositorioIncidentes>();
    private readonly IRepositorioServidores _repositorioServidores = Substitute.For<IRepositorioServidores>();
    private readonly IReloj _reloj = new RelojFijo(Base);
    private readonly EstadoConductaEnMemoria _estado = new();
    private readonly EvaluadorRafagaDistribuida _evaluador = new();
    private readonly EvaluadorReglaContenido _evaluadorContenido = new();

    private MotorDeModeracion CrearMotor(Modo modo)
    {
        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida",
                prioridad: 0,
                modo: modo,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo, VentanaBorradoDias: 1) }),
        };

        return new MotorDeModeracion(
            _estado, _evaluador, _evaluadorContenido, politicas, _adaptador, _repositorio,
            _repositorioServidores, _reloj, NullLogger<MotorDeModeracion>.Instance);
    }

    private async Task<Incidente?> InyectarRafagaAsync(MotorDeModeracion motor)
    {
        // 3 canales distintos dentro de la ventana: dispara la ráfaga (CU-01).
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000001", Base));
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000002", Base.AddMilliseconds(300)));
        return await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000003", Base.AddMilliseconds(600)));
    }

    [Fact]
    public async Task En_simulacion_persiste_incidente_simulado_y_no_invoca_baneo()
    {
        // Given una política en modo simulación con acción de baneo (TC-07 / CA-01).
        var motor = CrearMotor(Modo.Simulacion);

        // When la ráfaga se marca para el emisor.
        var incidente = await InyectarRafagaAsync(motor);

        // Then NO se invoca BanearConBorradoAsync y se registra un incidente simulado (RN-09).
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());

        incidente.Should().NotBeNull();
        incidente!.Modo.Should().Be(Modo.Simulacion);
        incidente.Resultado.Should().Be(ResultadoModeracion.Simulada);
        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Incidente>(i => i.Resultado == ResultadoModeracion.Simulada), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task En_ejecucion_invoca_baneo_una_vez_y_registra_ejecutada()
    {
        // Given una política en modo ejecución con acción de baneo.
        var motor = CrearMotor(Modo.Ejecucion);

        // When la ráfaga se marca para el emisor.
        var incidente = await InyectarRafagaAsync(motor);

        // Then se invoca BanearConBorradoAsync exactamente una vez y el incidente es ejecutado.
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Is<Snowflake>(s => s.Valor == MensajesDePrueba.ServidorPorDefecto),
            Arg.Is<Snowflake>(u => u.Valor == MensajesDePrueba.UsuarioPorDefecto),
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>());

        incidente.Should().NotBeNull();
        incidente!.Modo.Should().Be(Modo.Ejecucion);
        incidente.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
    }

    [Fact]
    public async Task Mensajes_en_un_solo_canal_no_disparan_ninguna_accion()
    {
        // Given una política en ejecución y el emisor publicando solo en UN canal.
        var motor = CrearMotor(Modo.Ejecucion);

        Incidente? incidente = null;
        for (var i = 0; i < 5; i++)
        {
            incidente = await motor.ProcesarAsync(
                MensajesDePrueba.Crear("300000000000000001", Base.AddMilliseconds(i * 100)));
        }

        // Then no hay coincidencia ni baneo ni incidente.
        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _repositorio.DidNotReceive().AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }
}
