using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace DiscordModeradorBot.Servicio.Benchmarks;

/// <summary>
/// Configuraciones de BenchmarkDotNet del harness de NFR. El NFR de latencia se expresa como un
/// PERCENTIL (p95 &lt; 200 ms), de modo que el harness debe reportar el percentil además de la
/// media: se agregan las columnas de mediana y p95 (<see cref="StatisticColumn.Median"/>,
/// <see cref="StatisticColumn.P95"/>).
///
/// Como los benchmarks de latencia usan <c>[IterationSetup]</c> para reiniciar el estado en
/// memoria por iteración, BenchmarkDotNet fuerza <c>InvocationCount=1</c>: cada iteración aporta
/// UNA medición, así que el percentil sale de la distribución de iteraciones. Por eso el Job
/// "acotado" usa muchas iteraciones (para que el p95 sea representativo) pero sigue terminando en
/// tiempo razonable; el Job "completo" agrega aún más iteraciones para mayor precisión.
/// </summary>
public static class ConfiguracionBenchmark
{
    /// <summary>
    /// Configuración ACOTADA (corrida corta): suficientes iteraciones para un p95 representativo en
    /// tiempo razonable. Para los benchmarks con [IterationSetup] (InvocationCount=1) cada iteración
    /// es una medición; 120 iteraciones dan una muestra suficiente para el percentil.
    /// </summary>
    public static IConfig Acotada()
    {
        var job = Job.Default
            .WithWarmupCount(10)
            .WithIterationCount(120)
            .WithLaunchCount(1)
            .WithId("Acotada");

        return Base().AddJob(job);
    }

    /// <summary>
    /// Configuración COMPLETA: más iteraciones para reducir el margen de error del p95. Más lenta;
    /// se documenta que da números de aceptación más precisos.
    /// </summary>
    public static IConfig Completa()
    {
        var job = Job.Default
            .WithWarmupCount(15)
            .WithIterationCount(300)
            .WithLaunchCount(2)
            .WithId("Completa");

        return Base().AddJob(job);
    }

    private static ManualConfig Base() =>
        ManualConfig.CreateMinimumViable()
            .AddColumn(StatisticColumn.Mean)
            .AddColumn(StatisticColumn.Median)
            .AddColumn(StatisticColumn.P95)
            .AddColumn(StatisticColumn.StdDev)
            .WithSummaryStyle(
                SummaryStyle.Default.WithTimeUnit(TimeUnit.Microsecond));
}
