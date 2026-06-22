using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;
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
        // Por defecto el servidor usa los parámetros por defecto (umbral 3, ventana 2 s, etc.).
        _configuracion.ObtenerParametrosAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new ParametrosModeracion(
                RegistroDescriptores.UmbralCanalesDistintos.ValorPorDefecto,
                RegistroDescriptores.VentanaDeteccionSegundos.ValorPorDefecto,
                RegistroDescriptores.VentanaAntirreboteSegundos.ValorPorDefecto));
    }

    private void ConfigurarParametros(int umbral, double ventanaSegundos, double antirreboteSegundos)
        => _configuracion.ObtenerParametrosAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new ParametrosModeracion(umbral, ventanaSegundos, antirreboteSegundos));

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

    [Fact]
    public async Task Una_rafaga_en_tres_canales_dentro_de_la_ventana_dispara_el_baneo()
    {
        // Given el grupo mixto "alguna" (contenido + conducta) del escenario reportado, ventana 2 s.
        ConfigurarGrupoMixto();
        var motor = CrearMotor();

        // When el mismo usuario publica en 3 canales distintos dentro de la ventana, sin la palabra.
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000001", Base, contenido: "hola"));
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000002", Base.AddMilliseconds(300), contenido: "hola"));
        var incidente = await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000003", Base.AddMilliseconds(600), contenido: "hola"));

        // Then la ráfaga (regla de conducta) dispara el evento y se banea (CU-01).
        incidente.Should().NotBeNull();
        incidente!.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Una_rafaga_lenta_no_dispara_con_ventana_corta_pero_si_con_la_ventana_configurada()
    {
        // Given el grupo mixto, pero los 3 mensajes se reparten en ~4 s (posteo MANUAL realista):
        // con la ventana por defecto de 2 s nunca hay 3 canales simultáneos en la ventana.
        ConfigurarGrupoMixto();
        var lentos = new[]
        {
            MensajesDePrueba.Crear("300000000000000001", Base, contenido: "hola"),
            MensajesDePrueba.Crear("300000000000000002", Base.AddSeconds(2), contenido: "hola"),
            MensajesDePrueba.Crear("300000000000000003", Base.AddSeconds(4), contenido: "hola"),
        };

        // When se procesan con la ventana por defecto (2 s) → NO alcanza el umbral.
        var motorCorto = CrearMotor();
        Incidente? conVentanaCorta = null;
        foreach (var m in lentos)
        {
            conVentanaCorta = await motorCorto.ProcesarAsync(m);
        }

        conVentanaCorta.Should().BeNull();
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());

        // And cuando el servidor configura una ventana de 6 s, la MISMA ráfaga lenta sí dispara.
        ConfigurarParametros(
            umbral: RegistroDescriptores.UmbralCanalesDistintos.ValorPorDefecto,
            ventanaSegundos: 6.0,
            antirreboteSegundos: RegistroDescriptores.VentanaAntirreboteSegundos.ValorPorDefecto);
        var motorAncho = CrearMotor();
        Incidente? conVentanaAncha = null;
        foreach (var m in lentos)
        {
            conVentanaAncha = await motorAncho.ProcesarAsync(m);
        }

        // Then la ventana CONFIGURADA por servidor cambió de verdad la detección (CU-11, RN-10, CU-01).
        conVentanaAncha.Should().NotBeNull();
        conVentanaAncha!.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Reproduce la configuración real reportada: UN grupo con modo "alguna" que mezcla la regla de
    /// contenido ("baneame") y la regla de conducta (ráfaga distribuida), bajo un evento en ejecución.
    /// Con "alguna" alcanza con que dispare CUALQUIERA de las dos reglas.
    /// </summary>
    private void ConfigurarGrupoMixto()
    {
        var regla = new ReglaContenidoPersistida(
            100, "Palabrota",
            ReglaContenido.PorPalabrasClave("Palabrota", "baneame", TimeSpan.FromMilliseconds(100)));
        var grupo = new GrupoPersistido(
            10, "Baneame", "alguna", null,
            new[]
            {
                new ReglaDeGrupo("contenido", 100, null),
                new ReglaDeGrupo("conducta", null, "rafaga-distribuida"),
            });
        var evento = new EventoPersistido(
            1, "Baneo", 0, false, "ejecucion", "todos", new[] { 10 },
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
