using Microsoft.Playwright;

namespace DiscordModeradorBot.Servicio.E2E.Soporte;

/// <summary>
/// Excepción interna para señalar que el entorno no tiene navegador de Playwright disponible (no se
/// descargó el binario o no arranca). La usa <see cref="EntornoPlaywright"/> para que los tests se
/// OMITAN en vez de fallar cuando localmente no hay navegador (estrategia-testing §7; el job de CI
/// instala los navegadores y sí corre).
/// </summary>
public sealed class NavegadorNoDisponibleException : Exception
{
    public NavegadorNoDisponibleException(string mensaje, Exception? interna = null)
        : base(mensaje, interna)
    {
    }
}

/// <summary>
/// Helpers para arrancar Playwright tolerando la ausencia de navegador. En CI los navegadores se
/// instalan con `playwright install`; localmente, si la descarga o el arranque fallan, se traduce a
/// <see cref="NavegadorNoDisponibleException"/> para OMITIR el test (no romper la suite).
/// </summary>
public static class EntornoPlaywright
{
    /// <summary>
    /// Crea Playwright y lanza Chromium headless. Si el navegador no está disponible (no instalado o
    /// no arranca), lanza <see cref="NavegadorNoDisponibleException"/>.
    /// </summary>
    public static async Task<(IPlaywright Playwright, IBrowser Navegador)> LanzarChromiumAsync()
    {
        IPlaywright playwright;
        try
        {
            playwright = await Playwright.CreateAsync();
        }
        catch (Exception ex)
        {
            throw new NavegadorNoDisponibleException(
                "No se pudo inicializar Playwright (¿faltan las dependencias del runtime?).", ex);
        }

        try
        {
            var navegador = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
            });
            return (playwright, navegador);
        }
        catch (PlaywrightException ex)
        {
            playwright.Dispose();
            // Mensaje típico cuando falta el binario: "Executable doesn't exist ... run playwright install".
            throw new NavegadorNoDisponibleException(
                "No hay un navegador de Playwright disponible. Instalá con `pwsh bin/.../playwright.ps1 install chromium` " +
                "o ejecutá en CI (que instala los navegadores).", ex);
        }
        catch (Exception ex)
        {
            playwright.Dispose();
            throw new NavegadorNoDisponibleException(
                "No se pudo lanzar el navegador de Playwright.", ex);
        }
    }
}
