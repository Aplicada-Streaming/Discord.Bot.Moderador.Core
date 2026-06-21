using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del camino de EJECUCIÓN real del motor (R2): orden de acciones reportar→banear
/// (RN-05, CU-02/CU-03/CU-05), copia tomada antes del borrado (RN-11), tope de 7 días de la
/// ventana de borrado (RN-02), contenido del reporte (CU-05) y regresión del modo simulación
/// (RN-09). Verifica con NSubstitute el orden y los argumentos efectivos.
/// </summary>
public sealed class MotorDeModeracionR2Tests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string CanalSalida = "500000000000000001";

    private readonly IAdaptadorGateway _adaptador = Substitute.For<IAdaptadorGateway>();
    private readonly IRepositorioIncidentes _repositorio = Substitute.For<IRepositorioIncidentes>();
    private readonly IRepositorioServidores _repositorioServidores = Substitute.For<IRepositorioServidores>();
    private readonly IReloj _reloj = new RelojFijo(Base);
    private readonly EstadoConductaEnMemoria _estado = new();
    private readonly EvaluadorRafagaDistribuida _evaluador = new();

    public MotorDeModeracionR2Tests()
    {
        // Servidor con canal de salida designado (CU-05): el motor lo resuelve para reportar.
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(
            new Snowflake(MensajesDePrueba.ServidorPorDefecto),
            "token-cifrado",
            canalDeSalida: canal);
        _repositorioServidores
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(servidor);
    }

    private MotorDeModeracion CrearMotor(Modo modo, IReadOnlyList<Accion> acciones)
    {
        var politicas = new[]
        {
            new Politica("Ráfaga distribuida", prioridad: 0, modo: modo, acciones: acciones),
        };

        return new MotorDeModeracion(
            _estado, _evaluador, politicas, _adaptador, _repositorio, _repositorioServidores, _reloj,
            NullLogger<MotorDeModeracion>.Instance);
    }

    private async Task<Incidente?> InyectarRafagaAsync(MotorDeModeracion motor)
    {
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000001", Base));
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000002", Base.AddMilliseconds(300)));
        return await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000003", Base.AddMilliseconds(600)));
    }

    [Fact]
    public async Task En_ejecucion_reporta_y_luego_banea_en_orden_con_copia_antes_del_borrado()
    {
        // Given una política en ejecución con acciones reportar (orden 0) y banear (orden 1)
        // (TC-19 / CU-02/CU-03 CA-01, RN-05).
        var motor = CrearMotor(Modo.Ejecucion, new[]
        {
            new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
        });

        // When la ráfaga se marca para el emisor.
        var incidente = await InyectarRafagaAsync(motor);

        // Then se reporta ANTES de banear (RN-05) y el reporte ya lleva la copia de mensajes
        // (tomada antes del borrado, RN-11).
        Received.InOrder(() =>
        {
            _adaptador.ReportarAsync(
                Arg.Is<CanalDeSalida>(c => c.SnowflakeCanal.Valor == CanalSalida),
                Arg.Is<ReporteIncidente>(r => r.MensajesAccionados.Count == 1 && !r.EsSimulacion),
                Arg.Any<CancellationToken>());
            _adaptador.BanearConBorradoAsync(
                Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        });

        incidente.Should().NotBeNull();
        incidente!.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
        incidente.MensajesAccionados.Should().HaveCount(1);
    }

    [Fact]
    public async Task En_ejecucion_la_ventana_de_borrado_mayor_a_7_dias_se_acota_a_7()
    {
        // Given una acción de baneo con ventana de borrado de 10 días (TC-10 / CU-03 CA-02, RN-02).
        var motor = CrearMotor(Modo.Ejecucion, new[]
        {
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 0, VentanaBorradoDias: 10),
        });

        // When se ejecuta la acción sobre el emisor.
        await InyectarRafagaAsync(motor);

        // Then la ventana efectiva pasada al adaptador se topa a 7 días (no se rechaza, RN-02).
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(),
            Arg.Any<Snowflake>(),
            Arg.Is<TimeSpan>(v => v == TimeSpan.FromDays(7)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task En_ejecucion_ventana_cero_no_borra_pero_banea()
    {
        // Given una acción con ventana de borrado de 0 días (TC-11 / CU-03 CA-03).
        var motor = CrearMotor(Modo.Ejecucion, new[]
        {
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 0, VentanaBorradoDias: 0),
        });

        // When se ejecuta la acción.
        await InyectarRafagaAsync(motor);

        // Then se banea con ventana 0 (sin remover mensajes previos).
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(),
            Arg.Any<Snowflake>(),
            Arg.Is<TimeSpan>(v => v == TimeSpan.Zero),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task El_reporte_incluye_los_mensajes_que_dispararon_y_los_canales_afectados()
    {
        // Given una política en ejecución que reporta (TC-25 / CU-05 CA-01, RN-11).
        var motor = CrearMotor(Modo.Ejecucion, new[]
        {
            new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1),
        });

        ReporteIncidente? reporteCapturado = null;
        await _adaptador.ReportarAsync(
            Arg.Any<CanalDeSalida>(),
            Arg.Do<ReporteIncidente>(r => reporteCapturado = r),
            Arg.Any<CancellationToken>());

        // When la ráfaga se marca para el emisor en el canal 3.
        await InyectarRafagaAsync(motor);

        // Then el reporte lleva el emisor, los mensajes accionados y los canales afectados.
        reporteCapturado.Should().NotBeNull();
        reporteCapturado!.UsuarioId.Valor.Should().Be(MensajesDePrueba.UsuarioPorDefecto);
        reporteCapturado.MensajesAccionados.Should().ContainSingle();
        reporteCapturado.MensajesAccionados[0].ContenidoCopiado.Should().Be("contenido de prueba");
        reporteCapturado.CanalesAfectados.Should().ContainSingle()
            .Which.Valor.Should().Be("300000000000000003");
        reporteCapturado.EsSimulacion.Should().BeFalse();
    }

    [Fact]
    public async Task En_simulacion_no_reporta_ni_banea_y_persiste_simulada()
    {
        // Given una política en simulación con reportar y banear (regresión R2, RN-09).
        var motor = CrearMotor(Modo.Simulacion, new[]
        {
            new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1),
        });

        // When la ráfaga se marca para el emisor.
        var incidente = await InyectarRafagaAsync(motor);

        // Then NO se invoca ni el reporte ni el baneo, y el incidente queda simulado (RN-09).
        await _adaptador.DidNotReceive().ReportarAsync(
            Arg.Any<CanalDeSalida>(), Arg.Any<ReporteIncidente>(), Arg.Any<CancellationToken>());
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());

        incidente.Should().NotBeNull();
        incidente!.Modo.Should().Be(Modo.Simulacion);
        incidente.Resultado.Should().Be(ResultadoModeracion.Simulada);
        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Incidente>(i => i.Resultado == ResultadoModeracion.Simulada), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Sin_canal_de_salida_designado_no_reporta_pero_banea_y_persiste()
    {
        // Given un servidor SIN canal de salida designado (CU-05 CA-03, REPORTE_CANAL_NO_DESIGNADO).
        _repositorioServidores
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new ServidorRegistrado(
                new Snowflake(MensajesDePrueba.ServidorPorDefecto), "token-cifrado"));

        var motor = CrearMotor(Modo.Ejecucion, new[]
        {
            new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1),
        });

        // When la ráfaga se marca.
        var incidente = await InyectarRafagaAsync(motor);

        // Then no se envía reporte, pero el baneo se ejecuta y el incidente se conserva (RN-11).
        await _adaptador.DidNotReceive().ReportarAsync(
            Arg.Any<CanalDeSalida>(), Arg.Any<ReporteIncidente>(), Arg.Any<CancellationToken>());
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        incidente.Should().NotBeNull();
        incidente!.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
    }
}
