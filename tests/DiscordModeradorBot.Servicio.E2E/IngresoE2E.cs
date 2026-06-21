using DiscordModeradorBot.Servicio.E2E.Soporte;
using Microsoft.Playwright;

namespace DiscordModeradorBot.Servicio.E2E;

/// <summary>
/// E2E de la AUTENTICACIÓN del administrador (CU-09): credenciales válidas entran al panel; inválidas
/// muestran un mensaje neutro (sin enumeración); y el LOCKOUT: tras 5 intentos fallidos, el 6º (aun
/// con la clave correcta) se rechaza con AUTH_DEMASIADOS_INTENTOS (CU-09 §6).
/// </summary>
public sealed class IngresoE2E : PruebaE2EBase
{
    private const string Usuario = "admin";
    private const string Clave = "clave-segura-2026";

    [HechoSaltable]
    public async Task Credenciales_validas_entran_al_panel()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);

            // Given: cuenta existente. When: ingreso con credenciales válidas (CU-09 CA-01).
            await IniciarSesionAsync(pagina, Usuario, Clave);

            // Then: entra al panel de incidentes.
            await Assertions.Expect(pagina).ToHaveURLAsync(
                new System.Text.RegularExpressions.Regex("/incidentes"));
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading,
                new() { Name = "Incidentes de moderación" })).ToBeVisibleAsync();
        });
    }

    [HechoSaltable]
    public async Task Credenciales_invalidas_muestran_mensaje_neutro()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);

            // Given: cuenta existente. When: ingreso con contraseña incorrecta (CU-09 CA-02).
            await IniciarSesionAsync(pagina, Usuario, "clave-incorrecta-xyz");

            // Then: mensaje neutro, sin distinguir qué campo falló (sin enumeración de cuentas).
            await Assertions.Expect(pagina.GetByText("Credenciales inválidas")).ToBeVisibleAsync();
        });
    }

    [HechoSaltable]
    public async Task Lockout_tras_5_fallos_el_6to_con_clave_correcta_muestra_demasiados_intentos()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);

            // When: intentos fallidos consecutivos (misma cuenta + misma IP → misma clave de control).
            // Con el máximo en 5 (ControlIntentosAutenticacion), los primeros 4 fallos devuelven
            // "Credenciales inválidas" y el 5º fallo ya alcanza el límite y devuelve
            // "Demasiados intentos" (CU-09 §6). Se espera el texto resultante tras cada POST para
            // garantizar que el fallo se contabilizó antes del siguiente intento.
            for (var intento = 0; intento < 4; intento++)
            {
                await IntentarFalloAsync(pagina, $"fallo-{intento}");
                await Assertions.Expect(pagina.GetByText("Credenciales inválidas"))
                    .ToBeVisibleAsync(new() { Timeout = TimeoutMs });
            }

            // 5º fallo: alcanza el límite; ya muestra el bloqueo por demasiados intentos.
            await IntentarFalloAsync(pagina, "fallo-5");
            await Assertions.Expect(pagina.GetByText("Demasiados intentos fallidos"))
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });

            // Then: el 6º intento, AUN con la clave CORRECTA, sigue bloqueado (CU-09 §6): durante el
            // enfriamiento ni una credencial válida abre sesión.
            await IntentarFalloAsync(pagina, Clave);
            await Assertions.Expect(pagina.GetByText("Demasiados intentos fallidos"))
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });
            await Assertions.Expect(pagina).ToHaveURLAsync(
                new System.Text.RegularExpressions.Regex("error=AUTH_DEMASIADOS_INTENTOS"));
        });
    }

    /// <summary>
    /// Carga /ingresar y postea un intento de login con la contraseña indicada, esperando la
    /// navegación que dispara el submit. El form es SSR (data-enhance="false") y postea como
    /// navegación HTML real; envolver el click en RunAndWaitForNavigation evita la carrera entre el
    /// POST y la aserción posterior, garantizando que cada intento se contabilizó.
    /// </summary>
    private static async Task IntentarFalloAsync(IPage pagina, string contrasena)
    {
        await pagina.GotoAsync("/ingresar");
        await pagina.FillAsync("#usuario", Usuario);
        await pagina.FillAsync("#contrasena", contrasena);
        await pagina.ClickAsync("button[type=submit]");
        // Espera la navegación del POST (a /ingresar?error=...) antes de que el llamador asevere.
        await pagina.WaitForURLAsync(
            url => url.Contains("/ingresar?"),
            new PageWaitForURLOptions { Timeout = TimeoutMs });
    }
}
