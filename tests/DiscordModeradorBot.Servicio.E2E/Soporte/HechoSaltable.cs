using System.ComponentModel;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DiscordModeradorBot.Servicio.E2E.Soporte;

/// <summary>
/// Excepción que, lanzada dentro de un test marcado con <see cref="HechoSaltableAttribute"/>, hace
/// que el test se reporte como OMITIDO (Skipped) en vez de fallido. La usamos para que los e2e se
/// omitan cuando el entorno local no tiene navegador de Playwright (estrategia-testing §7); en CI los
/// navegadores se instalan y los tests corren de verdad.
/// </summary>
public sealed class SaltarPruebaException : Exception
{
    public SaltarPruebaException(string razon)
        : base(razon)
    {
    }
}

/// <summary>Helper para omitir un test condicionalmente lanzando <see cref="SaltarPruebaException"/>.</summary>
public static class Saltar
{
    public static void Si(bool condicion, string razon)
    {
        if (condicion)
        {
            throw new SaltarPruebaException(razon);
        }
    }
}

/// <summary>
/// Variante de [Fact] que soporta OMISIÓN en tiempo de ejecución: si el cuerpo lanza
/// <see cref="SaltarPruebaException"/>, el test se reporta como omitido. xUnit v2 no trae omisión
/// dinámica nativa, así que se implementa con un discoverer propio (patrón estándar de extensibilidad
/// xUnit). Evita depender de paquetes externos solo para esto.
/// </summary>
[XunitTestCaseDiscoverer(
    "DiscordModeradorBot.Servicio.E2E.Soporte.DescubridorHechoSaltable",
    "DiscordModeradorBot.Servicio.E2E")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class HechoSaltableAttribute : FactAttribute
{
}

/// <summary>Descubre los <see cref="HechoSaltableAttribute"/> y los envuelve en un caso saltable.</summary>
public sealed class DescubridorHechoSaltable : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _diagnosticMessageSink;

    public DescubridorHechoSaltable(IMessageSink diagnosticMessageSink) =>
        _diagnosticMessageSink = diagnosticMessageSink;

    public IEnumerable<IXunitTestCase> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        yield return new CasoHechoSaltable(
            _diagnosticMessageSink,
            discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(),
            testMethod);
    }
}

/// <summary>
/// Caso de prueba que traduce una <see cref="SaltarPruebaException"/> lanzada por el test en un
/// resultado OMITIDO, en lugar de fallido.
/// </summary>
public sealed class CasoHechoSaltable : XunitTestCase
{
    [Obsolete("Solo para el serializador de xUnit.", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CasoHechoSaltable()
    {
    }

    public CasoHechoSaltable(
        IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        object[]? testMethodArguments = null)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
    {
    }

    public override async Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        // Intercepta los mensajes para convertir una falla por SaltarPruebaException en una omisión.
        var busInterceptor = new BusFiltraSalto(messageBus);
        var resumen = await base.RunAsync(
            diagnosticMessageSink, busInterceptor, constructorArguments, aggregator, cancellationTokenSource);

        if (busInterceptor.RazonSalto is { } razon)
        {
            resumen.Failed--;
            resumen.Skipped++;
        }

        return resumen;
    }

    /// <summary>
    /// Bus que detecta un <see cref="ITestFailed"/> cuya excepción es <see cref="SaltarPruebaException"/>
    /// y lo reemplaza por un <see cref="ITestSkipped"/> con la razón del salto.
    /// </summary>
    private sealed class BusFiltraSalto : IMessageBus
    {
        private readonly IMessageBus _interno;

        public BusFiltraSalto(IMessageBus interno) => _interno = interno;

        public string? RazonSalto { get; private set; }

        public bool QueueMessage(IMessageSinkMessage mensaje)
        {
            if (mensaje is ITestFailed fallo &&
                fallo.ExceptionTypes.FirstOrDefault() == typeof(SaltarPruebaException).FullName)
            {
                RazonSalto = fallo.Messages.FirstOrDefault() ?? "Test omitido.";
                return _interno.QueueMessage(new TestSkipped(fallo.Test, RazonSalto));
            }

            return _interno.QueueMessage(mensaje);
        }

        public void Dispose() => _interno.Dispose();
    }
}
