using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del eje de CONTENIDO en el pipeline (R3, CU-04): un mensaje único cuyo texto cumple la
/// regla de contenido por regex dispara la política y, en ejecución, reutiliza el camino de
/// acciones de R2 (reportar + banear, RN-05); en simulación persiste Simulada sin ejecutar
/// (RN-09). Verifica con NSubstitute. La detección es por el contenido del mensaje aislado, sin
/// estado: no depende de una ráfaga (no se publican varios canales).
/// </summary>
public sealed class MotorDeModeracionContenidoTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan Tope = TimeSpan.FromMilliseconds(100);

    private const string CanalSalida = "500000000000000001";
    private const string Patron = @"https?://(?:bit\.ly|tinyurl\.com)/\S+";

    private readonly IAdaptadorGateway _adaptador = Substitute.For<IAdaptadorGateway>();
    private readonly IRepositorioIncidentes _repositorio = Substitute.For<IRepositorioIncidentes>();
    private readonly IRepositorioServidores _repositorioServidores = Substitute.For<IRepositorioServidores>();
    private readonly IRepositorioExenciones _repositorioExenciones = Substitute.For<IRepositorioExenciones>();
    private readonly IReloj _reloj = new RelojFijo(Base);
    private readonly EstadoConductaEnMemoria _estado = new();
    private readonly EstadoAntirreboteEnMemoria _antirrebote = new();
    private readonly EvaluadorRafagaDistribuida _evaluador = new();
    private readonly EvaluadorReglaContenido _evaluadorContenido = new();
    private readonly EvaluadorExenciones _evaluadorExenciones = new();

    public MotorDeModeracionContenidoTests()
    {
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(
            new Snowflake(MensajesDePrueba.ServidorPorDefecto), "token-cifrado", canalDeSalida: canal);
        _repositorioServidores
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(servidor);
        // Sin exenciones por defecto: el descarte previo (etapa 1) no aplica (regresión R3).
        _repositorioExenciones
            .ListarPorServidorAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<DiscordModeradorBot.Servicio.Dominio.Exenciones.Exencion>());
    }

    private MotorDeModeracion CrearMotorContenido(Modo modo)
    {
        var regla = ReglaContenido.PorExpresionRegular("Enlace de acortador", Patron, Tope);
        var politicas = new[]
        {
            new Politica(
                "Contenido prohibido",
                prioridad: 0,
                modo: modo,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                },
                reglaContenido: regla),
        };

        return new MotorDeModeracion(
            _estado, _antirrebote, _evaluador, _evaluadorContenido, _evaluadorExenciones, politicas,
            _adaptador, _repositorio, _repositorioServidores, _repositorioExenciones, _reloj,
            NullLogger<MotorDeModeracion>.Instance);
    }

    [Fact]
    public async Task Contenido_prohibido_en_ejecucion_reporta_y_banea_en_orden_y_persiste_ejecutada()
    {
        // Given una política de contenido en ejecución (TC-13 / CU-04 CA-01).
        var motor = CrearMotorContenido(Modo.Ejecucion);

        // When llega UN único mensaje con contenido prohibido (sin ráfaga).
        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "oferta https://bit.ly/abc ya"));

        // Then se reporta ANTES de banear (RN-05), reutilizando el camino de acciones de R2.
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
        incidente!.NombrePolitica.Should().Be("Contenido prohibido");
        incidente.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
        incidente.MensajesAccionados.Should().ContainSingle()
            .Which.ContenidoCopiado.Should().Be("oferta https://bit.ly/abc ya");
    }

    [Fact]
    public async Task Contenido_prohibido_en_simulacion_no_ejecuta_y_persiste_simulada()
    {
        // Given una política de contenido en simulación (TC-59 / CU-04 CA-04, RN-09).
        var motor = CrearMotorContenido(Modo.Simulacion);

        // When llega un mensaje con contenido prohibido.
        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "oferta https://tinyurl.com/xyz ya"));

        // Then NO se reporta ni se banea, y el incidente queda simulado (RN-09).
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
    public async Task Contenido_que_no_cumple_el_criterio_no_dispara_ninguna_accion()
    {
        // Given una política de contenido en ejecución (CU-04 §5.A).
        var motor = CrearMotorContenido(Modo.Ejecucion);

        // When llega un mensaje cuyo texto NO cumple el criterio.
        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "un mensaje sano sin enlaces"));

        // Then no hay coincidencia, ni acciones, ni incidente.
        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().ReportarAsync(
            Arg.Any<CanalDeSalida>(), Arg.Any<ReporteIncidente>(), Arg.Any<CancellationToken>());
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _repositorio.DidNotReceive().AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Una_regla_costosa_con_tope_no_cuelga_el_pipeline_y_no_dispara()
    {
        // Given una política de contenido con un patrón de retroceso catastrófico y tope muy bajo,
        // y una entrada adversa (TC-15 / ADR-08). El patrón es válido pero costoso.
        var regla = ReglaContenido.PorExpresionRegular("Costosa", "(a+)+$", TimeSpan.FromMilliseconds(1));
        var politicas = new[]
        {
            new Politica(
                "Contenido costoso",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) },
                reglaContenido: regla),
        };
        var motor = new MotorDeModeracion(
            _estado, _antirrebote, _evaluador, _evaluadorContenido, _evaluadorExenciones, politicas,
            _adaptador, _repositorio, _repositorioServidores, _repositorioExenciones, _reloj,
            NullLogger<MotorDeModeracion>.Instance);

        var entradaAdversa = new string('a', 60) + "!";

        // When se procesa el mensaje adverso.
        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: entradaAdversa));

        // Then el pipeline no se cuelga ni lanza, y la regla excedida se trata como no coincidencia
        // (ADR-08): no hay incidente ni baneo.
        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }
}
