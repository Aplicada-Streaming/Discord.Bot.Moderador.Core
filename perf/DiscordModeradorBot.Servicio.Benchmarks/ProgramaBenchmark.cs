using System.Diagnostics;
using System.Globalization;
using BenchmarkDotNet.Running;
using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Dominio;

namespace DiscordModeradorBot.Servicio.Benchmarks;

/// <summary>
/// Punto de entrada del harness de benchmark de los NFR del motor (intake §17 P.10).
///
/// Uso:
///   dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks
///       -> corre los microbenchmarks de BenchmarkDotNet (latencia con p95 + throughput + memoria)
///          con la configuración ACOTADA (120 iteraciones) para producir números en tiempo
///          razonable.
///
///   dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- throughput
///       -> corre SOLO un loop cronometrado que procesa N mensajes y reporta mensajes/s directos,
///          como verificación complementaria del NFR de throughput (>= 50 msg/s).
///
///   dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- full
///       -> corre BenchmarkDotNet con la configuración COMPLETA (más iteraciones, p95 más preciso,
///          a costa de tiempo).
/// </summary>
public static class ProgramaBenchmark
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 &&
            string.Equals(args[0], "throughput", StringComparison.OrdinalIgnoreCase))
        {
            CorrerLoopThroughput();
            return;
        }

        // Por defecto: configuración ACOTADA (corrida corta) con columnas de p95/mediana para
        // números rápidos. Con "full" se usa la configuración COMPLETA (más iteraciones, p95 más
        // preciso, a costa de tiempo).
        var usarCompleta = args.Length > 0 &&
            string.Equals(args[0], "full", StringComparison.OrdinalIgnoreCase);

        var config = usarCompleta
            ? ConfiguracionBenchmark.Completa()
            : ConfiguracionBenchmark.Acotada();

        BenchmarkRunner.Run(
            new[] { typeof(MotorDeModeracionBenchmarks), typeof(ThroughputBenchmarks) },
            config);
    }

    /// <summary>
    /// Loop cronometrado que mide el throughput sostenido del motor procesando un volumen grande de
    /// mensajes benignos y reportando mensajes/s. Es la verificación directa del NFR de throughput
    /// (>= 50 msg/s) sin depender de la conversión del microbenchmark. Tras un calentamiento, mide
    /// varias rondas y reporta el mejor y el promedio.
    /// </summary>
    private static void CorrerLoopThroughput()
    {
        const int mensajesPorRonda = 50_000;
        const int rondasCalentamiento = 1;
        const int rondasMedidas = 5;

        var fabrica = new ThroughputBenchmarks();
        var lote = ThroughputBenchmarks.ConstruirLote(mensajesPorRonda);

        Console.WriteLine("== Loop cronometrado de throughput del MotorDeModeracion ==");
        Console.WriteLine(
            "Motor en memoria (sin DB, sin red, sin logging). Mensajes por ronda: "
            + mensajesPorRonda.ToString("N0", CultureInfo.InvariantCulture));
        Console.WriteLine("NFR objetivo: >= 50 mensajes/s en la Raspberry Pi 4 (este equipo es baseline x64).");
        Console.WriteLine();

        // Calentamiento: JIT y warm caches, no se mide.
        for (var w = 0; w < rondasCalentamiento; w++)
        {
            EjecutarRonda(fabrica, lote).GetAwaiter().GetResult();
        }

        double mejor = 0;
        double sumaThroughput = 0;
        for (var r = 0; r < rondasMedidas; r++)
        {
            var (msPorRonda, throughput) = EjecutarRonda(fabrica, lote).GetAwaiter().GetResult();
            mejor = Math.Max(mejor, throughput);
            sumaThroughput += throughput;
            Console.WriteLine(
                $"Ronda {r + 1}: {msPorRonda.ToString("N1", CultureInfo.InvariantCulture)} ms"
                + $" -> {throughput.ToString("N0", CultureInfo.InvariantCulture)} mensajes/s");
        }

        var promedio = sumaThroughput / rondasMedidas;
        Console.WriteLine();
        Console.WriteLine(
            $"Throughput promedio: {promedio.ToString("N0", CultureInfo.InvariantCulture)} mensajes/s"
            + $" | mejor: {mejor.ToString("N0", CultureInfo.InvariantCulture)} mensajes/s");
        Console.WriteLine(
            promedio >= 50
                ? "Resultado: CUMPLE el NFR de throughput (>= 50 msg/s) en x64 baseline."
                : "Resultado: NO cumple el NFR de throughput (>= 50 msg/s) en este equipo.");
    }

    private static async Task<(double MsPorRonda, double Throughput)> EjecutarRonda(
        ThroughputBenchmarks fabrica, MensajeEntrante[] lote)
    {
        // Motor con estado fresco por ronda: no acumula ventanas entre rondas.
        MotorDeModeracion motor = fabrica.CrearMotor();

        var cronometro = Stopwatch.StartNew();
        for (var i = 0; i < lote.Length; i++)
        {
            await motor.ProcesarAsync(lote[i]);
        }

        cronometro.Stop();

        var ms = cronometro.Elapsed.TotalMilliseconds;
        var throughput = lote.Length / cronometro.Elapsed.TotalSeconds;
        return (ms, throughput);
    }
}
