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
/// Prueba de extremo a extremo de la rebanada "la configuración DIRIGE el motor" (CU-11): el motor
/// carga las políticas del servidor desde la configuración persistida (cargador real) y modera con
/// ellas, en lugar de una política hardcodeada. Reproduce el escenario que el operador reportó —
/// dar de alta una regla por palabras clave ("baneame") y que un mensaje con esa palabra de otro
/// usuario realmente dispare la acción configurada y registre el incidente.
/// </summary>
public sealed class MotorConfigDirigidaTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);
    private const string CanalSalida = "500000000000000001";

    private readonly IAdaptadorGateway _adaptador = Substitute.For<IAdaptadorGateway>();
    private readonly IRepositorioIncidentes _repositorio = Substitute.For<IRepositorioIncidentes>();
    private readonly IRepositorioServidores _repositorioServidores = Substitute.For<IRepositorioServidores>();
    private readonly IRepositorioExenciones _repositorioExenciones = Substitute.For<IRepositorioExenciones>();
    private readonly IRepositorioConfiguracion _configuracion = Substitute.For<IRepositorioConfiguracion>();
    private readonly IRepositorioReglasContenido _reglasContenido = Substitute.For<IRepositorioReglasContenido>();

    public MotorConfigDirigidaTests()
    {
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(
            new Snowflake(MensajesDePrueba.ServidorPorDefecto), "token-cifrado", canalDeSalida: canal);
        _repositorioServidores.ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(servidor);
        _repositorioExenciones.ListarPorServidorAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Exencion>());
    }

    private void ConfigurarReglaDePalabra(string modo)
    {
        var regla = new ReglaContenidoPersistida(
            100, "Palabrota",
            ReglaContenido.PorPalabrasClave("Palabrota", "baneame", TimeSpan.FromMilliseconds(100)));
        var grupo = new GrupoPersistido(
            10, "Palabras prohibidas", "Alguna", null,
            new[] { new ReglaDeGrupo("contenido", 100, null) });
        var evento = new EventoPersistido(
            1, "Palabras prohibidas", 0, false, modo, "alguno", new[] { 10 },
            new[]
            {
                new AccionPersistida("ReportarACanalPrivado", 0, null, null, null),
                new AccionPersistida("BaneoConBorradoRetroactivo", 1, 1, null, null),
            });

        _configuracion.ListarEventosAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new[] { evento });
        _configuracion.ListarGruposAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new[] { grupo });
        _reglasContenido.ListarPorServidorAsync(
                Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(new[] { regla });
    }

    private MotorDeModeracion CrearMotor()
    {
        var cargador = new CargadorPoliticasDesdeConfiguracion(
            _configuracion, _reglasContenido, new EvaluadorReglaContenido(), new EvaluadorRafagaDistribuida(),
            NullLogger<CargadorPoliticasDesdeConfiguracion>.Instance);

        return new MotorDeModeracion(
            new EstadoConductaEnMemoria(),
            new EstadoAntirreboteEnMemoria(),
            new EvaluadorRafagaDistribuida(),
            new EvaluadorReglaContenido(),
            new EvaluadorExenciones(),
            cargador,
            _adaptador,
            _repositorio,
            _repositorioServidores,
            _repositorioExenciones,
            new RelojFijo(Base),
            NullLogger<MotorDeModeracion>.Instance);
    }

    [Fact]
    public async Task La_palabra_configurada_dispara_la_accion_y_registra_el_incidente()
    {
        // Given una regla por palabras clave "baneame" dada de alta en el panel, en EJECUCIÓN.
        ConfigurarReglaDePalabra("ejecucion");
        var motor = CrearMotor();

        // When otro usuario escribe la palabra.
        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "decí baneame y listo"));

        // Then la política configurada disparó: se baneó y se registró el incidente (CU-11, CU-04).
        incidente.Should().NotBeNull();
        incidente!.NombrePolitica.Should().Be("Palabras prohibidas");
        incidente.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _repositorio.Received(1).AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Un_mensaje_sin_la_palabra_no_dispara_nada()
    {
        ConfigurarReglaDePalabra("ejecucion");
        var motor = CrearMotor();

        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "un mensaje totalmente sano"));

        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Sin_configuracion_el_motor_no_modera()
    {
        // Given un servidor sin eventos configurados (el hardcode ya no existe): nada que evaluar.
        _configuracion.ListarEventosAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<EventoPersistido>());
        var motor = CrearMotor();

        var incidente = await motor.ProcesarAsync(MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "decí baneame y listo"));

        // Then no se dispara ninguna acción (la moderación depende de la configuración del panel).
        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }
}
