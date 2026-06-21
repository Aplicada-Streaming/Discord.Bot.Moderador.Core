using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas de R7: políticas que disparan por una COMPOSICIÓN de grupos de reglas con modos de
/// coincidencia (RN-15), integradas en el motor manteniendo prioridad/primera coincidencia
/// (RN-04) y el camino de acciones de R2. Incluye regresión de los disparos de ráfaga (R1) y de
/// contenido (R3) bajo el nuevo modelo: una política sin composición debe seguir comportándose
/// igual que antes.
/// </summary>
public sealed class MotorDeModeracionR7Tests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);
    private const string CanalSalida = "500000000000000001";

    private readonly IAdaptadorGateway _adaptador = Substitute.For<IAdaptadorGateway>();
    private readonly IRepositorioIncidentes _repositorio = Substitute.For<IRepositorioIncidentes>();
    private readonly IRepositorioServidores _repositorioServidores = Substitute.For<IRepositorioServidores>();
    private readonly IRepositorioExenciones _repositorioExenciones = Substitute.For<IRepositorioExenciones>();
    private readonly RelojFijo _reloj = new(Base);
    private readonly EstadoConductaEnMemoria _estado = new();
    private readonly EstadoAntirreboteEnMemoria _antirrebote = new();
    private readonly EvaluadorRafagaDistribuida _evaluador = new();
    private readonly EvaluadorReglaContenido _evaluadorContenido = new();
    private readonly EvaluadorExenciones _evaluadorExenciones = new();

    public MotorDeModeracionR7Tests()
    {
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(
            new Snowflake(MensajesDePrueba.ServidorPorDefecto), "token-cifrado", canalDeSalida: canal);
        _repositorioServidores
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(servidor);
        _repositorioExenciones
            .ListarPorServidorAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Exencion>());
    }

    private MotorDeModeracion CrearMotor(IReadOnlyList<Politica> politicas) =>
        new(
            _estado, _antirrebote, _evaluador, _evaluadorContenido, _evaluadorExenciones, politicas,
            _adaptador, _repositorio, _repositorioServidores, _repositorioExenciones, _reloj,
            NullLogger<MotorDeModeracion>.Instance);

    private ReglaContenido ReglaUrl() => ReglaContenido.PorExpresionRegular(
        "Enlace de acortador", @"https?://(?:bit\.ly|tinyurl\.com)/\S+", EvaluadorReglaContenido.TopeTiempoPorDefecto);

    private async Task<Incidente?> RafagaAsync(MotorDeModeracion motor, DateTimeOffset desde, string contenido)
    {
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000001", desde, contenido: contenido));
        await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000002", desde.AddMilliseconds(200), contenido: contenido));
        return await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000003", desde.AddMilliseconds(400), contenido: contenido));
    }

    [Fact]
    public async Task Composicion_AlMenosN_dispara_segun_la_cantidad_de_reglas_que_coinciden()
    {
        // Given una política con UN grupo en modo AlMenosN (N=2) sobre dos reglas: contenido (URL)
        // y conducta (ráfaga). Solo dispara si AL MENOS 2 coinciden (RN-15).
        var grupo = new GrupoDeReglas("Spam distribuido", ModoCoincidencia.AlMenosN, new IReglaEvaluable[]
        {
            new ReglaEvaluableContenido(ReglaUrl(), _evaluadorContenido),
            new ReglaEvaluableConducta(_evaluador),
        }, minimoCoincidencias: 2);
        var composicion = new ComposicionPolitica(new[] { grupo });

        var motor = CrearMotor(new[]
        {
            new Politica(
                "Spam distribuido", prioridad: 0, modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) },
                composicion: composicion),
        });

        // When llega una ráfaga CON contenido de URL: coinciden las dos reglas (≥2) → dispara.
        var incidente = await RafagaAsync(motor, Base, "oferta https://bit.ly/abc");

        // Then se baneó (el grupo coincidió por AlMenosN=2).
        incidente.Should().NotBeNull();
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Composicion_AlMenosN_no_dispara_si_solo_coincide_una_regla()
    {
        // Given el mismo grupo AlMenosN(2), pero llega UN solo mensaje (no hay ráfaga) con URL:
        // coincide solo la regla de contenido (1 < 2) → no dispara (RN-15).
        var grupo = new GrupoDeReglas("Spam distribuido", ModoCoincidencia.AlMenosN, new IReglaEvaluable[]
        {
            new ReglaEvaluableContenido(ReglaUrl(), _evaluadorContenido),
            new ReglaEvaluableConducta(_evaluador),
        }, minimoCoincidencias: 2);

        var motor = CrearMotor(new[]
        {
            new Politica(
                "Spam distribuido", prioridad: 0, modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) },
                composicion: new ComposicionPolitica(new[] { grupo })),
        });

        // When llega un único mensaje con URL (sin ráfaga).
        var incidente = await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000001", Base, contenido: "oferta https://bit.ly/abc"));

        // Then no se acciona: solo coincidió una de las dos reglas requeridas.
        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Composicion_Alguna_dispara_con_una_regla_que_coincide()
    {
        // Given un grupo en modo Alguna (OR) con contenido y conducta: con que UNA coincida dispara.
        var grupo = new GrupoDeReglas("Cualquiera", ModoCoincidencia.Alguna, new IReglaEvaluable[]
        {
            new ReglaEvaluableContenido(ReglaUrl(), _evaluadorContenido),
            new ReglaEvaluableConducta(_evaluador),
        });

        var motor = CrearMotor(new[]
        {
            new Politica(
                "Cualquiera", prioridad: 0, modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) },
                composicion: new ComposicionPolitica(new[] { grupo })),
        });

        // When llega un único mensaje con URL (solo coincide la de contenido) → Alguna basta.
        var incidente = await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000001", Base, contenido: "oferta https://bit.ly/abc"));

        incidente.Should().NotBeNull();
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Composicion_respeta_prioridad_y_primera_coincidencia()
    {
        // Given dos políticas con composición que ambas coincidirían; la de mayor prioridad
        // (prioridad menor) sin continuar debe detener la evaluación (RN-04).
        IReglaEvaluable ReglaConducta() => new ReglaEvaluableConducta(_evaluador);

        var alta = new Politica(
            "Alta", prioridad: 0, modo: Modo.Ejecucion,
            acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) },
            composicion: ComposicionPolitica.DeReglaUnica(ReglaConducta()));
        var baja = new Politica(
            "Baja", prioridad: 1, modo: Modo.Ejecucion,
            acciones: new[] { new Accion(TipoAccion.Expulsar) },
            composicion: ComposicionPolitica.DeReglaUnica(ReglaConducta()));

        var motor = CrearMotor(new[] { baja, alta }); // desordenadas a propósito: el motor ordena.

        // When la ráfaga dispara: solo la de prioridad 0 acciona (primera coincidencia, RN-04).
        var incidente = await RafagaAsync(motor, Base, "ráfaga");

        incidente!.NombrePolitica.Should().Be("Alta");
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _adaptador.DidNotReceive().ExpulsarAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<CancellationToken>());
    }

    // ---- Regresión: el modelo previo (sin composición) sigue funcionando bajo R7 ----

    [Fact]
    public async Task Regresion_rafaga_R1_sigue_disparando_sin_composicion()
    {
        // Given una política de CONDUCTA clásica (sin composición ni regla de contenido): el eje de
        // ráfaga de R1 debe seguir intacto.
        var motor = CrearMotor(new[]
        {
            new Politica(
                "Ráfaga distribuida", prioridad: 0, modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) }),
        });

        var incidente = await RafagaAsync(motor, Base, "ráfaga");

        incidente.Should().NotBeNull();
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Regresion_contenido_R3_sigue_disparando_sin_composicion()
    {
        // Given una política de CONTENIDO clásica (reglaContenido, sin composición): el eje de
        // contenido de R3 debe seguir intacto.
        var motor = CrearMotor(new[]
        {
            new Politica(
                "Contenido prohibido", prioridad: 0, modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) },
                reglaContenido: ReglaUrl()),
        });

        // When llega un único mensaje con URL prohibida.
        var incidente = await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000001", Base, contenido: "oferta https://bit.ly/abc"));

        incidente.Should().NotBeNull();
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }
}
