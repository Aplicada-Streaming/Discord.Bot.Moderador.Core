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
/// Pruebas del descarte previo de exentos en el pipeline (R5, CU-15, RN-07): un sujeto exento
/// —por usuario, por rol o por canal— se descarta en la ETAPA 1, ANTES de evaluar cualquier
/// regla, de modo que no dispara política ni registra incidente ni acción. El descarte es
/// PREVIO: el estado de conducta no se actualiza con el mensaje exento, por lo que un mensaje
/// exento no contribuye al conteo de una ráfaga posterior de otro usuario. Un usuario sin
/// exención con la misma ráfaga SÍ dispara (regresión). Verifica con NSubstitute.
/// </summary>
public sealed class MotorDeModeracionExencionesTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string CanalSalida = "500000000000000001";
    private const string RolStaff = "700000000000000001";
    private const string UsuarioStaff = "200000000000000077";

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

    public MotorDeModeracionExencionesTests()
    {
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(
            new Snowflake(MensajesDePrueba.ServidorPorDefecto), "token-cifrado", canalDeSalida: canal);
        _repositorioServidores
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(servidor);
    }

    private void ConExenciones(params Exencion[] exenciones)
    {
        _repositorioExenciones
            .ListarPorServidorAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(exenciones);
    }

    private MotorDeModeracion CrearMotorRafaga()
    {
        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
        };

        return new MotorDeModeracion(
            _estado, _antirrebote, _evaluador, _evaluadorContenido, _evaluadorExenciones, politicas,
            _adaptador, _repositorio, _repositorioServidores, _repositorioExenciones, _reloj,
            NullLogger<MotorDeModeracion>.Instance);
    }

    private static MensajeEntrante Rafaga(
        string canal, DateTimeOffset instante, string usuario, params string[] roles)
    {
        return MensajesDePrueba.Crear(canal, instante, usuarioId: usuario) with
        {
            RolesDelAutor = roles.Select(r => new Snowflake(r)).ToArray(),
        };
    }

    private async Task<Incidente?> InyectarRafagaAsync(
        MotorDeModeracion motor, string usuario, params string[] roles)
    {
        await motor.ProcesarAsync(Rafaga("300000000000000001", Base, usuario, roles));
        await motor.ProcesarAsync(Rafaga("300000000000000002", Base.AddMilliseconds(300), usuario, roles));
        return await motor.ProcesarAsync(
            Rafaga("300000000000000003", Base.AddMilliseconds(600), usuario, roles));
    }

    private async Task NoSeAcciono(Incidente? incidente)
    {
        incidente.Should().BeNull();
        await _adaptador.DidNotReceive().ReportarAsync(
            Arg.Any<CanalDeSalida>(), Arg.Any<ReporteIncidente>(), Arg.Any<CancellationToken>());
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _repositorio.DidNotReceive().AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Rafaga_de_usuario_exento_no_dispara_ni_registra_incidente()
    {
        // Given una exención por el usuario emisor (TC-16 / CU-15, RN-07).
        ConExenciones(Exencion.PorUsuario(new Snowflake(MensajesDePrueba.UsuarioPorDefecto)));
        var motor = CrearMotorRafaga();

        // When ese usuario postea una ráfaga que normalmente dispararía.
        var incidente = await InyectarRafagaAsync(motor, MensajesDePrueba.UsuarioPorDefecto);

        // Then queda descartado antes de evaluar: ni política, ni incidente, ni acción (RN-07).
        await NoSeAcciono(incidente);
    }

    [Fact]
    public async Task Rafaga_de_usuario_con_rol_exento_no_dispara()
    {
        // Given una exención por el rol staff y un usuario que lo porta (TC-04 / CU-15 CA-02).
        ConExenciones(Exencion.PorRol(new Snowflake(RolStaff)));
        var motor = CrearMotorRafaga();

        // When el usuario de staff postea la ráfaga en varios canales.
        var incidente = await InyectarRafagaAsync(motor, UsuarioStaff, RolStaff);

        // Then se descarta por rol antes de evaluar; no hay contención (RN-07).
        await NoSeAcciono(incidente);
    }

    [Fact]
    public async Task Actividad_en_canal_exento_no_dispara()
    {
        // Given los tres canales de la ráfaga declarados de confianza (TC / CU-15 CA-04).
        ConExenciones(
            Exencion.PorCanal(new Snowflake("300000000000000001")),
            Exencion.PorCanal(new Snowflake("300000000000000002")),
            Exencion.PorCanal(new Snowflake("300000000000000003")));
        var motor = CrearMotorRafaga();

        // When un usuario no exento postea en esos canales de confianza.
        var incidente = await InyectarRafagaAsync(motor, MensajesDePrueba.UsuarioPorDefecto);

        // Then la actividad de esos canales se excluye de la evaluación: no dispara (RN-07).
        await NoSeAcciono(incidente);
    }

    [Fact]
    public async Task Usuario_no_exento_con_la_misma_rafaga_si_dispara()
    {
        // Given ninguna exención aplicable (regresión: los no exentos siguen sujetos a la moderación).
        ConExenciones();
        var motor = CrearMotorRafaga();

        // When el usuario no exento postea la misma ráfaga.
        var incidente = await InyectarRafagaAsync(motor, MensajesDePrueba.UsuarioPorDefecto);

        // Then SÍ dispara, ejecuta y registra el incidente.
        incidente.Should().NotBeNull();
        incidente!.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _repositorio.Received(1).AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task El_mensaje_exento_no_contribuye_al_estado_de_conducta_de_una_rafaga_posterior()
    {
        // Given una exención por el usuario de staff: sus mensajes deben descartarse en la etapa 1
        // SIN actualizar el estado de conducta (RN-07, descarte PREVIO a la evaluación).
        ConExenciones(Exencion.PorUsuario(new Snowflake(UsuarioStaff)));
        var motor = CrearMotorRafaga();

        // When el usuario de staff EXENTO publica en 3 canales distintos (no debe registrar estado),
        // y luego un usuario NO exento publica en SOLO 2 canales distintos.
        await motor.ProcesarAsync(Rafaga("300000000000000001", Base, UsuarioStaff));
        await motor.ProcesarAsync(Rafaga("300000000000000002", Base.AddMilliseconds(100), UsuarioStaff));
        await motor.ProcesarAsync(Rafaga("300000000000000003", Base.AddMilliseconds(200), UsuarioStaff));

        var incidenteNoExento1 = await motor.ProcesarAsync(
            Rafaga("300000000000000004", Base.AddMilliseconds(300), MensajesDePrueba.UsuarioPorDefecto));
        var incidenteNoExento2 = await motor.ProcesarAsync(
            Rafaga("300000000000000005", Base.AddMilliseconds(400), MensajesDePrueba.UsuarioPorDefecto));

        // Then el usuario exento no produjo nada y, además, su actividad NO contó: el estado de
        // conducta del usuario exento quedó intacto (su conteo es 0, no 3) y el no exento, con solo
        // 2 canales, no alcanza el umbral de 3. Nada disparó (descarte previo verificado por efecto).
        incidenteNoExento1.Should().BeNull();
        incidenteNoExento2.Should().BeNull();
        _estado.CanalesDistintosEnVentana(
                new Snowflake(MensajesDePrueba.ServidorPorDefecto),
                new Snowflake(UsuarioStaff),
                Base.AddMilliseconds(400),
                TimeSpan.FromSeconds(2))
            .Should().Be(0, "el mensaje exento se descarta antes de actualizar el estado de conducta (RN-07)");

        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _repositorio.DidNotReceive().AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }
}
