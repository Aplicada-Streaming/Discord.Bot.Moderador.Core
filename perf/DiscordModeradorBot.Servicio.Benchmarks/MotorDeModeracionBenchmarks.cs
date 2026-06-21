using BenchmarkDotNet.Attributes;
using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Benchmarks.Dobles;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscordModeradorBot.Servicio.Benchmarks;

/// <summary>
/// Benchmark de LATENCIA por mensaje del hot path del <see cref="MotorDeModeracion"/> (NFR de
/// intake §17 P.10; arquitectura-solucion §8; matriz-cobertura §3 TC-55). Mide los dos caminos
/// que importan para el percentil:
///  - <see cref="NoCoincide"/>: el camino COMÚN (la inmensa mayoría del tráfico no dispara nada).
///  - <see cref="RafagaQueDispara"/>: el camino caro (una ráfaga distribuida dispara reportar+banear).
///
/// QUÉ SE MIDE: el motor de evaluación con dependencias EN MEMORIA / dobles —repositorio de
/// incidentes no-op, adaptador no-op (sin red ni logging), estado de conducta y antirrebote en
/// memoria, reloj fijo—. SIN base de datos en disco, SIN red, SIN logging real (NullLogger).
/// QUÉ NO SE MIDE: la E/S de Discord.Net, la persistencia SQLite, el panel Blazor ni la
/// instrumentación del adaptador simulado del producto. El objetivo es aislar el costo del
/// pipeline de decisión (etapas 1-9 del flujo-ejecucion), que es lo que fija el NFR de latencia.
///
/// El estado en memoria se reinicia en cada iteración (<see cref="IterationSetup"/>) para que no
/// crezca de forma acumulativa entre invocaciones y la medición sea estable.
/// </summary>
[MemoryDiagnoser] // proxy del NFR de memoria: asignaciones por operación (intake §17 P.10).
public class MotorDeModeracionBenchmarks
{
    private const string ServidorId = "100000000000000001";
    private const string UsuarioId = "200000000000000002";
    private const string CanalSalida = "500000000000000001";

    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly IAdaptadorGateway _adaptador = new AdaptadorGatewayNoOp();
    private readonly RepositorioIncidentesNoOp _repositorioIncidentes = new();
    private readonly IRepositorioServidores _repositorioServidores;
    private readonly IRepositorioExenciones _repositorioExenciones = new RepositorioExencionesEnMemoria();
    private readonly IReloj _reloj = new RelojFijo(Base);
    private readonly EvaluadorRafagaDistribuida _evaluador = new();
    private readonly EvaluadorReglaContenido _evaluadorContenido = new();
    private readonly EvaluadorExenciones _evaluadorExenciones = new();
    private readonly IReadOnlyList<Politica> _politicas;

    // Mensajes preconstruidos fuera del cuerpo medido: el benchmark mide el PROCESAMIENTO del
    // motor, no la construcción del mensaje. El no-coincidente publica siempre en un mismo canal
    // (1 canal distinto: por debajo del umbral de 3); la ráfaga publica en 3 canales distintos.
    private readonly MensajeEntrante _mensajeNoCoincide;
    private readonly MensajeEntrante[] _rafaga;

    // Estado en memoria: se reinstancia por iteración para no acumular actividad ni marcas de
    // antirrebote entre invocaciones (mediciones estables y comparables).
    private MotorDeModeracion _motor = null!;

    public MotorDeModeracionBenchmarks()
    {
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(new Snowflake(ServidorId), "token-cifrado", canalDeSalida: canal);
        _repositorioServidores = new RepositorioServidoresEnMemoria(servidor);

        // Política de CONTENIDO (CU-04) por expresión regular que NO coincide con el texto común,
        // y la política de CONDUCTA de ráfaga distribuida (CU-01) en modo ejecución. El camino
        // no-coincidente evalúa ambos ejes sin disparar; el de ráfaga dispara el eje de conducta.
        var reglaContenido = ReglaContenido.PorExpresionRegular(
            nombre: "Dominio de estafa",
            patron: @"https?://(?:[a-z0-9-]+\.)*estafa-conocida\.example/\S+",
            topeTiempoEvaluacion: EvaluadorReglaContenido.TopeTiempoPorDefecto);

        _politicas = new[]
        {
            new Politica(
                nombre: "Contenido prohibido",
                prioridad: 0,
                modo: Modo.Ejecucion,
                reglaContenido: reglaContenido,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
            new Politica(
                nombre: "Ráfaga distribuida",
                prioridad: 1,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
        };

        _mensajeNoCoincide = CrearMensaje(
            canalId: "300000000000000000",
            instante: Base,
            contenido: "hola equipo, ¿alguien revisó el ticket de ayer?");

        // Tres mensajes en tres canales distintos dentro de la ventana de 2 s: dispara la ráfaga
        // (umbral por defecto 3 canales distintos, CU-01).
        _rafaga = new[]
        {
            CrearMensaje("300000000000000001", Base, "primer mensaje del fan-out"),
            CrearMensaje("300000000000000002", Base.AddMilliseconds(200), "segundo mensaje del fan-out"),
            CrearMensaje("300000000000000003", Base.AddMilliseconds(400), "tercer mensaje del fan-out"),
        };
    }

    private static MensajeEntrante CrearMensaje(string canalId, DateTimeOffset instante, string contenido) =>
        new(
            new Snowflake(ServidorId),
            new Snowflake(canalId),
            new Snowflake(UsuarioId),
            new Snowflake("400000000000000001"),
            instante,
            contenido);

    private MotorDeModeracion CrearMotor() =>
        new(
            new EstadoConductaEnMemoria(),
            new EstadoAntirreboteEnMemoria(),
            _evaluador,
            _evaluadorContenido,
            _evaluadorExenciones,
            _politicas,
            _adaptador,
            _repositorioIncidentes,
            _repositorioServidores,
            _repositorioExenciones,
            _reloj,
            NullLogger<MotorDeModeracion>.Instance);

    [IterationSetup]
    public void IterationSetup() => _motor = CrearMotor();

    /// <summary>
    /// Camino COMÚN (no coincide): un mensaje benigno que no cumple la regla de contenido y no
    /// alcanza el umbral de ráfaga. Es el caso que domina el tráfico real y, por lo tanto, el
    /// percentil p95 del NFR de latencia.
    /// </summary>
    [Benchmark(Baseline = true, Description = "No coincide (camino común)")]
    public async Task<Incidente?> NoCoincide() => await _motor.ProcesarAsync(_mensajeNoCoincide);

    /// <summary>
    /// Camino CARO (ráfaga que dispara): tres mensajes en tres canales distintos disparan la
    /// política de conducta, que reporta y banea contra el adaptador no-op. Mide el costo del
    /// pipeline cuando hay acción simulada en memoria (sin red ni persistencia).
    /// </summary>
    [Benchmark(Description = "Ráfaga que dispara (reportar + banear)")]
    public async Task<Incidente?> RafagaQueDispara()
    {
        await _motor.ProcesarAsync(_rafaga[0]);
        await _motor.ProcesarAsync(_rafaga[1]);
        return await _motor.ProcesarAsync(_rafaga[2]);
    }
}
