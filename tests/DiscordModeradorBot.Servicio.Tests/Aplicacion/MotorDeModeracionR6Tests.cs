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
/// Pruebas de R6: acciones adicionales del catálogo ejecutadas en orden (RN-05, intake §4),
/// antirrebote por usuario (CU-16, RN-06) y jerarquía no accionable (RN-01, ADR-08, CU-02 §7).
/// Deterministas con reloj inyectado. Verifican con NSubstitute la invocación y los parámetros
/// de cada acción (duración del timeout, rol asignado/quitado) y la supresión de repeticiones.
/// </summary>
public sealed class MotorDeModeracionR6Tests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string CanalSalida = "500000000000000001";
    private const string Rol = "700000000000000123";

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

    // Ventana de antirrebote acotada y determinista para el test (5 s).
    private static readonly TimeSpan Ventana = TimeSpan.FromSeconds(5);

    public MotorDeModeracionR6Tests()
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
        // Por defecto el adaptador ejecuta todas las acciones de contención (NSubstitute devuelve
        // el valor por defecto del enum, Ejecutada = 0).
    }

    private MotorDeModeracion CrearMotor(IReadOnlyList<Accion> acciones)
    {
        var politicas = new[]
        {
            new Politica("Ráfaga distribuida", prioridad: 0, modo: Modo.Ejecucion, acciones: acciones),
        };

        return new MotorDeModeracion(
            _estado, _antirrebote, _evaluador, _evaluadorContenido, _evaluadorExenciones, politicas,
            _adaptador, _repositorio, _repositorioServidores, _repositorioExenciones, _reloj,
            NullLogger<MotorDeModeracion>.Instance, Ventana);
    }

    /// <summary>
    /// Inyecta una ráfaga distribuida (mismo usuario en 3 canales distintos dentro de la ventana
    /// de detección, CU-01). Cada llamada usa snowflakes de mensaje frescos.
    /// </summary>
    private async Task<Incidente?> RafagaAsync(MotorDeModeracion motor, DateTimeOffset desde)
    {
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000001", desde));
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000002", desde.AddMilliseconds(200)));
        return await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000003", desde.AddMilliseconds(400)));
    }

    // ---- Acciones adicionales (intake §4 Should Have, RN-05) ----

    [Fact]
    public async Task Timeout_se_invoca_con_su_duracion_en_el_orden_declarado()
    {
        // Given una política que reporta (orden 0) y luego aplica un TIMEOUT de 15 min (orden 1).
        var motor = CrearMotor(new[]
        {
            new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
            new Accion(TipoAccion.Timeout, OrdenEjecucion: 1, DuracionTimeout: TimeSpan.FromMinutes(15)),
        });

        // When la ráfaga dispara.
        var incidente = await RafagaAsync(motor, Base);

        // Then se reporta antes del timeout (RN-05) y el timeout lleva la duración configurada.
        Received.InOrder(() =>
        {
            _adaptador.ReportarAsync(
                Arg.Any<CanalDeSalida>(), Arg.Any<ReporteIncidente>(), Arg.Any<CancellationToken>());
            _adaptador.AplicarTimeoutAsync(
                Arg.Any<Snowflake>(), Arg.Any<Snowflake>(),
                Arg.Is<TimeSpan>(d => d == TimeSpan.FromMinutes(15)), Arg.Any<CancellationToken>());
        });
        await _adaptador.Received(1).AplicarTimeoutAsync(
            Arg.Is<Snowflake>(s => s.Valor == MensajesDePrueba.ServidorPorDefecto),
            Arg.Is<Snowflake>(u => u.Valor == MensajesDePrueba.UsuarioPorDefecto),
            Arg.Is<TimeSpan>(d => d == TimeSpan.FromMinutes(15)), Arg.Any<CancellationToken>());
        incidente!.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
    }

    [Fact]
    public async Task Expulsion_se_invoca_una_vez()
    {
        // Given una política con una acción de EXPULSIÓN (kick).
        var motor = CrearMotor(new[] { new Accion(TipoAccion.Expulsar, OrdenEjecucion: 0) });

        // When la ráfaga dispara.
        await RafagaAsync(motor, Base);

        // Then se expulsa al emisor exactamente una vez.
        await _adaptador.Received(1).ExpulsarAsync(
            Arg.Is<Snowflake>(s => s.Valor == MensajesDePrueba.ServidorPorDefecto),
            Arg.Is<Snowflake>(u => u.Valor == MensajesDePrueba.UsuarioPorDefecto),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Asignar_y_quitar_rol_se_invocan_con_el_rol_en_el_orden_declarado()
    {
        // Given una política que asigna un rol (orden 0) y luego quita otro (orden 1).
        var motor = CrearMotor(new[]
        {
            new Accion(TipoAccion.AsignarRol, OrdenEjecucion: 0, RolObjetivo: new Snowflake(Rol)),
            new Accion(TipoAccion.QuitarRol, OrdenEjecucion: 1, RolObjetivo: new Snowflake(Rol)),
        });

        // When la ráfaga dispara.
        await RafagaAsync(motor, Base);

        // Then se asigna antes de quitar (RN-05) y ambas llevan el rol objetivo.
        Received.InOrder(() =>
        {
            _adaptador.AsignarRolAsync(
                Arg.Any<Snowflake>(), Arg.Any<Snowflake>(),
                Arg.Is<Snowflake>(r => r.Valor == Rol), Arg.Any<CancellationToken>());
            _adaptador.QuitarRolAsync(
                Arg.Any<Snowflake>(), Arg.Any<Snowflake>(),
                Arg.Is<Snowflake>(r => r.Valor == Rol), Arg.Any<CancellationToken>());
        });
    }

    // ---- Antirrebote (CU-16, RN-06) ----

    [Fact]
    public async Task Antirrebote_suprime_la_accion_repetida_dentro_de_la_ventana()
    {
        // Given una política en ejecución que banea (TC-08 / CU-02 CA-04, RN-06).
        var motor = CrearMotor(new[]
        {
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 0, VentanaBorradoDias: 1),
        });

        // When la ráfaga dispara y, dentro de la ventana de antirrebote, el MISMO usuario vuelve a
        // disparar (más mensajes en canales distintos: la condición sigue marcada).
        await RafagaAsync(motor, Base);
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000004", Base.AddMilliseconds(600)));
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000005", Base.AddMilliseconds(800)));

        // Then el adaptador banea UNA sola vez: las repeticiones se suprimieron (0 adicionales).
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        // Y solo se registró un incidente (no se duplicaron, RN-06).
        await _repositorio.Received(1).AgregarAsync(Arg.Any<Incidente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Antirrebote_no_suprime_a_usuarios_distintos()
    {
        // Given una política en ejecución que banea.
        var motor = CrearMotor(new[]
        {
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 0),
        });

        // When dos usuarios distintos disparan sendas ráfagas dentro de la misma ventana.
        await RafagaUsuarioAsync(motor, "200000000000000002", Base);
        await RafagaUsuarioAsync(motor, "200000000000000003", Base);

        // Then cada usuario se acciona: el antirrebote es POR usuario (RN-06).
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Is<Snowflake>(u => u.Valor == "200000000000000002"),
            Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        await _adaptador.Received(1).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Is<Snowflake>(u => u.Valor == "200000000000000003"),
            Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Tras_vencer_la_ventana_una_nueva_coincidencia_vuelve_a_accionar()
    {
        // Given una política en ejecución que banea (TC-20 / CU-16 CA-02, RN-06).
        var motor = CrearMotor(new[]
        {
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 0),
        });

        // When la ráfaga dispara y se marca al usuario (reloj fijo en Base).
        await RafagaAsync(motor, Base);

        // El reloj avanza MÁS ALLÁ de la ventana de antirrebote: la marca expira.
        _reloj.Ahora = Base + Ventana + TimeSpan.FromSeconds(1);

        // Una nueva ráfaga del mismo usuario, con instantes dentro de la ventana de DETECCIÓN del
        // nuevo "ahora", vuelve a disparar.
        var despues = _reloj.Ahora;
        await RafagaAsync(motor, despues);

        // Then el adaptador banea DOS veces: la marca de antirrebote había expirado (CU-16 CA-02).
        await _adaptador.Received(2).BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Antirrebote_no_aplica_en_simulacion_no_invoca_acciones()
    {
        // Given una política en SIMULACIÓN: nunca se invoca acción real (RN-09) ni se marca el
        // antirrebote, que actúa solo en el camino de ejecución (etapa 7, flujo-ejecucion).
        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida", prioridad: 0, modo: Modo.Simulacion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) }),
        };
        var motor = new MotorDeModeracion(
            _estado, _antirrebote, _evaluador, _evaluadorContenido, _evaluadorExenciones, politicas,
            _adaptador, _repositorio, _repositorioServidores, _repositorioExenciones, _reloj,
            NullLogger<MotorDeModeracion>.Instance, Ventana);

        // When la ráfaga dispara y se repite dentro de la ventana.
        await RafagaAsync(motor, Base);
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000004", Base.AddMilliseconds(600)));

        // Then no se invoca el adaptador (RN-09) y cada coincidencia registra su incidente simulado.
        await _adaptador.DidNotReceive().BanearConBorradoAsync(
            Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    // ---- Jerarquía no accionable (RN-01, ADR-08, CU-02 §7) ----

    [Fact]
    public async Task Jerarquia_superior_registra_no_accionable_reporta_y_no_lanza()
    {
        // Given un usuario sobre el que el baneo NO es accionable por jerarquía (TC-06 / CU-02 CA-02).
        _adaptador
            .BanearConBorradoAsync(
                Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(ResultadoAccion.NoAccionablePorJerarquia);

        var motor = CrearMotor(new[]
        {
            new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
            new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1),
        });

        // When la ráfaga dispara: el pipeline NO debe lanzar (ADR-08).
        var incidente = await RafagaAsync(motor, Base);

        // Then el incidente queda NoAccionable, se reportó igualmente y se registró (RN-01, RN-11).
        incidente.Should().NotBeNull();
        incidente!.Resultado.Should().Be(ResultadoModeracion.NoAccionable);
        await _adaptador.Received(1).ReportarAsync(
            Arg.Any<CanalDeSalida>(), Arg.Any<ReporteIncidente>(), Arg.Any<CancellationToken>());
        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Incidente>(i => i.Resultado == ResultadoModeracion.NoAccionable),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Permisos_faltantes_registra_no_accionable()
    {
        // Given una acción que el adaptador rechaza por permisos faltantes (RN-01).
        _adaptador
            .ExpulsarAsync(Arg.Any<Snowflake>(), Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(ResultadoAccion.NoAccionablePorPermisos);

        var motor = CrearMotor(new[] { new Accion(TipoAccion.Expulsar, OrdenEjecucion: 0) });

        // When la ráfaga dispara.
        var incidente = await RafagaAsync(motor, Base);

        // Then el incidente queda NoAccionable y el pipeline continúa (ADR-08).
        incidente!.Resultado.Should().Be(ResultadoModeracion.NoAccionable);
    }

    private async Task RafagaUsuarioAsync(MotorDeModeracion motor, string usuario, DateTimeOffset desde)
    {
        await motor.ProcesarAsync(MensajesDePrueba.Crear("300000000000000001", desde, usuarioId: usuario));
        await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000002", desde.AddMilliseconds(200), usuarioId: usuario));
        await motor.ProcesarAsync(
            MensajesDePrueba.Crear("300000000000000003", desde.AddMilliseconds(400), usuarioId: usuario));
    }
}
