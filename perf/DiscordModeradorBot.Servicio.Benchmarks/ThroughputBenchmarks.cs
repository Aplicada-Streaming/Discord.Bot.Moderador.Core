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
/// Benchmark de THROUGHPUT sostenido del motor (NFR de intake §17 P.10; arquitectura-solucion §8;
/// matriz-cobertura §3 TC-56). Procesa un LOTE de <see cref="MensajesPorLote"/> mensajes
/// benignos por invocación; con <c>OperationsPerInvoke</c> BenchmarkDotNet reporta el tiempo POR
/// MENSAJE, del que se deriva el throughput (mensajes/s = 1e9 / ns-por-mensaje). El doble
/// <see cref="ProgramaBenchmark"/> también imprime el throughput directamente con un loop
/// cronometrado, para tener el número de mensajes/s sin conversión manual.
///
/// QUÉ SE MIDE / NO SE MIDE: igual que <see cref="MotorDeModeracionBenchmarks"/> — motor en
/// memoria, sin DB ni red ni logging. El lote usa muchos usuarios distintos (uno por mensaje) para
/// no acumular ventanas deslizantes gigantes de un solo usuario; representa el régimen de tráfico
/// normal (mensajes que no disparan), que es donde se sostiene el throughput.
/// </summary>
public class ThroughputBenchmarks
{
    /// <summary>Cantidad de mensajes procesados por invocación del benchmark (lote sostenido).</summary>
    public const int MensajesPorLote = 1_000;

    private const string ServidorId = "100000000000000001";
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

    private MensajeEntrante[] _lote = null!;
    private MotorDeModeracion _motor = null!;

    public ThroughputBenchmarks()
    {
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(new Snowflake(ServidorId), "token-cifrado", canalDeSalida: canal);
        _repositorioServidores = new RepositorioServidoresEnMemoria(servidor);

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
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) }),
            new Politica(
                nombre: "Ráfaga distribuida",
                prioridad: 1,
                modo: Modo.Ejecucion,
                acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) }),
        };
    }

    [GlobalSetup]
    public void GlobalSetup() => _lote = ConstruirLote(MensajesPorLote);

    [IterationSetup]
    public void IterationSetup() => _motor = CrearMotor();

    /// <summary>
    /// Procesa el lote completo de mensajes benignos. Con <c>OperationsPerInvoke</c>,
    /// BenchmarkDotNet divide el tiempo entre la cantidad de mensajes y reporta el tiempo por
    /// mensaje; el throughput (msg/s) es 1e9 / (ns por mensaje).
    /// </summary>
    [Benchmark(OperationsPerInvoke = MensajesPorLote, Description = "Throughput (lote de mensajes benignos)")]
    public async Task ProcesarLote()
    {
        for (var i = 0; i < _lote.Length; i++)
        {
            await _motor.ProcesarAsync(_lote[i]);
        }
    }

    /// <summary>
    /// Construye un lote de mensajes benignos, uno por usuario distinto, dentro de una ventana
    /// temporal acotada. Cada mensaje no coincide con la regla de contenido ni alcanza el umbral
    /// de ráfaga (1 canal por usuario): representa el tráfico normal sostenido.
    /// </summary>
    public static MensajeEntrante[] ConstruirLote(int cantidad)
    {
        var lote = new MensajeEntrante[cantidad];
        for (var i = 0; i < cantidad; i++)
        {
            // Snowflakes deterministas como texto (RN-08): usuario y mensaje únicos por índice.
            var usuario = (200000000000000000UL + (ulong)i).ToString(System.Globalization.CultureInfo.InvariantCulture);
            var mensajeId = (400000000000000000UL + (ulong)i).ToString(System.Globalization.CultureInfo.InvariantCulture);
            lote[i] = new MensajeEntrante(
                new Snowflake(ServidorId),
                new Snowflake("300000000000000000"),
                new Snowflake(usuario),
                new Snowflake(mensajeId),
                Base.AddMilliseconds(i),
                "mensaje benigno de tráfico normal numero " + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        return lote;
    }

    /// <summary>Crea un motor con estado en memoria fresco (no participa de la medición).</summary>
    public MotorDeModeracion CrearMotor() =>
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
}
