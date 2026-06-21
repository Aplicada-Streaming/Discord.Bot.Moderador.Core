using Microsoft.Playwright;

namespace DiscordModeradorBot.Servicio.E2E.Soporte;

/// <summary>
/// Base de las pruebas e2e del panel. Provee el patrón de ejecución de un escenario:
/// <list type="number">
///   <item>lanza el navegador headless (o OMITE el test si no hay navegador local);</item>
///   <item>levanta el host real del servicio (Kestrel, puerto efímero, SQLite temporal, gateway
///   Simulado, entorno E2E sin seed de admin);</item>
///   <item>abre una página apuntando a ese host y ejecuta el cuerpo del escenario;</item>
///   <item>dispone navegador y host (borra la base temporal) al finalizar.</item>
/// </list>
/// Cada test usa su propio host y su propia base, así no dependen del orden ni del estado de otros
/// (estrategia-testing §7).
/// </summary>
public abstract class PruebaE2EBase
{
    /// <summary>Tiempo máximo razonable para esperas de navegación/elementos en e2e.</summary>
    protected const float TimeoutMs = 15_000;

    /// <summary>
    /// Ejecuta un escenario e2e con host y página listos. Si el entorno no tiene navegador de
    /// Playwright, OMITE el test (Skip) en lugar de fallar.
    /// </summary>
    protected static async Task EjecutarEscenarioAsync(Func<IPage, HostServicioE2E, Task> escenario)
    {
        IPlaywright? playwright = null;
        IBrowser? navegador = null;

        try
        {
            (playwright, navegador) = await EntornoPlaywright.LanzarChromiumAsync();
        }
        catch (NavegadorNoDisponibleException ex)
        {
            // Sin navegador local: se OMITE. En CI los navegadores están instalados y sí corre.
            throw new SaltarPruebaException($"Navegador de Playwright no disponible: {ex.Message}");
        }

        await using var host = await HostServicioE2E.IniciarAsync();
        try
        {
            var contexto = await navegador.NewContextAsync(new BrowserNewContextOptions
            {
                BaseURL = host.UrlBase,
                IgnoreHTTPSErrors = true,
            });
            var pagina = await contexto.NewPageAsync();
            pagina.SetDefaultTimeout(TimeoutMs);

            // Diagnóstico: vuelca errores de consola y de página a la salida del test (ayuda a
            // depurar fallas de circuito/JS en CI sin re-ejecutar localmente).
            pagina.Console += (_, msg) =>
            {
                if (msg.Type is "error" or "warning")
                {
                    Console.WriteLine($"[navegador:{msg.Type}] {msg.Text}");
                }
            };
            pagina.PageError += (_, err) => Console.WriteLine($"[navegador:pageerror] {err}");

            await escenario(pagina, host);
        }
        finally
        {
            if (navegador is not null)
            {
                await navegador.DisposeAsync();
            }

            playwright?.Dispose();
        }
    }

    /// <summary>
    /// Inicia sesión por la UI con las credenciales dadas, esperando la navegación del POST. Los
    /// formularios de autenticación son SSR (data-enhance="false") y postean como navegación HTML
    /// real; envolver el submit en RunAndWaitForNavigation evita la carrera entre el POST y la
    /// aserción/navegación posterior. Deja la cookie de sesión en el contexto del navegador.
    /// </summary>
    protected static async Task IniciarSesionAsync(IPage pagina, string usuario, string contrasena)
    {
        await pagina.GotoAsync("/ingresar");
        await pagina.FillAsync("#usuario", usuario);
        await pagina.FillAsync("#contrasena", contrasena);
        await pagina.ClickAsync("button[type=submit]");
        // Espera la navegación del POST: éxito (/incidentes) o rechazo (/ingresar?error=...). El
        // predicado de URL se cumple en ambos casos, garantizando que el POST se procesó antes de
        // que el llamador haga sus aserciones.
        await pagina.WaitForURLAsync(
            url => url.Contains("/incidentes") || url.Contains("/ingresar?"),
            new PageWaitForURLOptions { Timeout = TimeoutMs });
    }

    /// <summary>
    /// Reintenta un click en un componente de Blazor InteractiveServer hasta que aparece el resultado
    /// esperado. El primer click puede perderse si el circuito de Blazor todavía no está conectado;
    /// el reintento lo absorbe. Útil para formularios MudBlazor (registro, guardado) y para abrir
    /// diálogos. No falla por sí mismo: si tras los reintentos no aparece, deja que la aserción del
    /// test reporte el fallo con su contexto.
    /// </summary>
    protected static async Task ClickConReintentoHastaAsync(
        ILocator botonAccion, ILocator resultadoEsperado, int intentos = 10)
    {
        for (var i = 0; i < intentos; i++)
        {
            await botonAccion.ClickAsync();
            try
            {
                await resultadoEsperado.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 2_000,
                });
                return;
            }
            catch (TimeoutException)
            {
                // El circuito aún no estaba listo o la acción no surtió efecto; reintentar.
            }
        }
    }
}
