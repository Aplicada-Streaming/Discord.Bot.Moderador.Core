using DiscordModeradorBot.Servicio.E2E.Soporte;
using Microsoft.Playwright;

namespace DiscordModeradorBot.Servicio.E2E;

/// <summary>
/// E2E del PRIMER INGRESO (CU-08, TC-67 first-run; estrategia-testing §1 nivel e2e). El host arranca
/// SIN admin (seed deshabilitado en E2E); navegar a una página protegida redirige a
/// /configuracion-inicial, se crea la cuenta única respetando el token antiforgery del formulario y
/// luego se redirige a /ingresar para iniciar sesión (CU-08 paso 7).
/// </summary>
public sealed class PrimerIngresoE2E : PruebaE2EBase
{
    [HechoSaltable]
    public async Task Sin_admin_pagina_protegida_redirige_a_configuracion_inicial_y_crea_la_cuenta()
    {
        await EjecutarEscenarioAsync(async (pagina, _) =>
        {
            // Given: sistema en first-run (sin admin). When: navego a una página protegida.
            await pagina.GotoAsync("/incidentes", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

            // Then: el gating de autenticación redirige al alta de primer ingreso (CU-08).
            await pagina.WaitForURLAsync(
                url => url.Contains("/configuracion-inicial"), new PageWaitForURLOptions { Timeout = TimeoutMs });
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading,
                new() { Name = "Crear la cuenta de administrador" })).ToBeVisibleAsync();

            // When: completo el formulario (el <AntiforgeryToken /> viaja en el POST automáticamente).
            await pagina.FillAsync("#usuario", "admin");
            await pagina.FillAsync("#contrasena", "clave-segura-2026");
            await pagina.FillAsync("#confirmacion", "clave-segura-2026");
            await pagina.ClickAsync("button[type=submit]");

            // Then: tras el alta se redirige a /ingresar (CU-08 paso 7) con el aviso de cuenta creada.
            await pagina.WaitForURLAsync(
                url => url.Contains("/ingresar"), new PageWaitForURLOptions { Timeout = TimeoutMs });
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading,
                new() { Name = "Iniciar sesión" })).ToBeVisibleAsync();
            await Assertions.Expect(pagina.GetByText("Cuenta creada")).ToBeVisibleAsync();
        });
    }

    [HechoSaltable]
    public async Task Confirmacion_que_no_coincide_no_crea_la_cuenta_y_sigue_en_first_run()
    {
        await EjecutarEscenarioAsync(async (pagina, _) =>
        {
            // Given: first-run. When: la confirmación de contraseña no coincide (CU-08 CA-04).
            await pagina.GotoAsync("/configuracion-inicial");
            await pagina.FillAsync("#usuario", "admin");
            await pagina.FillAsync("#contrasena", "clave-segura-2026");
            await pagina.FillAsync("#confirmacion", "otra-clave-distinta-2026");
            await pagina.ClickAsync("button[type=submit]");

            // Then: se rechaza el alta con el aviso de diferencia; el sistema permanece en first-run.
            await pagina.WaitForURLAsync(
                url => url.Contains("/configuracion-inicial"), new PageWaitForURLOptions { Timeout = TimeoutMs });
            await Assertions.Expect(pagina.GetByText("Las contraseñas no coinciden")).ToBeVisibleAsync();
        });
    }
}
