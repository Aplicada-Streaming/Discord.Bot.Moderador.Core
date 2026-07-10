using DiscordModeradorBot.Servicio.E2E.Soporte;
using Microsoft.Playwright;

namespace DiscordModeradorBot.Servicio.E2E;

/// <summary>
/// E2E (de menor prioridad, bajo costo) del REGISTRO Y PRUEBA DE SERVIDOR (CU-10/CU-12) y de la
/// CONFIGURACIÓN POR DESCRIPTORES (CU-11). Autenticado: registra un servidor, corre la prueba de
/// configuración mostrando los chequeos, y da de alta una regla de contenido por descriptores.
/// </summary>
public sealed class ServidoresYConfiguracionE2E : PruebaE2EBase
{
    private const string Usuario = "admin";
    private const string Clave = "clave-segura-2026";
    private const string ServidorId = "100000000000000200";

    [HechoSaltable]
    public async Task Registrar_un_servidor_y_probar_la_configuracion_muestra_los_chequeos()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);
            await IngresarAsync(pagina);

            await pagina.GotoAsync("/servidores", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading, new() { Name = "Servidores" }))
                .ToBeVisibleAsync();

            // When: registro un servidor (snowflake + token + canal de salida). El registro y la
            // prueba viven en una página MudBlazor InteractiveServer; se completa y reenvía con
            // reintento hasta que el servidor aparece, para absorber el arranque del circuito.
            await RegistrarServidorAsync(pagina, ServidorId, "500000000000000200");

            // When: corro la prueba de configuración. Then: se muestran los chequeos (gateway Simulado).
            // Acoto a la fila del servidor recién registrado por su texto.
            var filaServidor = pagina.GetByRole(AriaRole.Row)
                .Filter(new() { HasText = ServidorId });
            var botonProbar = filaServidor.GetByRole(AriaRole.Button, new() { Name = "Probar", Exact = true });
            await ClickConReintentoHastaAsync(botonProbar, pagina.GetByText("Verificaciones"));
            await Assertions.Expect(pagina.GetByText("Verificaciones"))
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });
        });
    }

    [HechoSaltable]
    public async Task Configuracion_alta_de_regla_de_contenido_por_descriptores()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);
            await IngresarAsync(pagina);

            // Registro un servidor para tener contexto de configuración (reintento por el circuito).
            await pagina.GotoAsync("/servidores", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await RegistrarServidorAsync(pagina, ServidorId, canalSalida: null);

            // En /configuracion, la pestaña Reglas permite dar de alta una regla de contenido por
            // expresión regular validada al guardar (CU-11, RN-03, ayuda dirigida por descriptores).
            await pagina.GotoAsync("/configuracion", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading,
                new() { Name = "Configuración de moderación" })).ToBeVisibleAsync();

            var confirmacion = pagina.GetByText("Regla guardada");
            await GuardarReglaConReintentoAsync(
                pagina, "Enlace de acortador (e2e)", @"https?://(?:bit\.ly|tinyurl\.com)/\S+", confirmacion);

            // Then: confirmación de regla guardada (validada al guardar, RN-03).
            await Assertions.Expect(confirmacion).ToBeVisibleAsync(new() { Timeout = TimeoutMs });
        });
    }

    [HechoSaltable]
    public async Task Configuracion_rinde_un_campo_desde_el_descriptor_con_ayuda_y_explicacion_en_palabras()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);
            await IngresarAsync(pagina);

            // Registro un servidor para que la página de configuración muestre las pestañas.
            await pagina.GotoAsync("/servidores", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await RegistrarServidorAsync(pagina, ServidorId, canalSalida: null);

            await pagina.GotoAsync("/configuracion", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading,
                new() { Name = "Configuración de moderación" })).ToBeVisibleAsync();

            // When: abro la pestaña "Parámetros" (reactiva, InteractiveServer; reintento por el circuito).
            var pestanaParametros = pagina.GetByRole(AriaRole.Tab, new() { Name = "Parámetros" });
            // El campo numérico se rinde DESDE el descriptor: su etiqueta es la del descriptor
            // (RegistroDescriptores.UmbralCanalesDistintos.Etiqueta).
            var campoUmbral = pagina.GetByLabel("Umbral de canales distintos");
            await ClickConReintentoHastaAsync(pestanaParametros, campoUmbral);

            // Then: el campo dirigido por descriptor está visible, con su hint de default/límites.
            await Assertions.Expect(campoUmbral).ToBeVisibleAsync(new() { Timeout = TimeoutMs });
            await Assertions.Expect(pagina.GetByText("Por defecto 3; entre 2 y 10.").First)
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });

            // Then: la explicación EN PALABRAS (generada por ExplicadorEnPalabras desde descriptores +
            // valores) está visible y refleja los valores por defecto.
            await Assertions.Expect(pagina.GetByText("Explicación en palabras (previsualización)"))
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });
            await Assertions.Expect(pagina.GetByText("ráfaga distribuida").First)
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });

            // Then: la ranura del asistente de IA queda reservada y deshabilitada (forward-compat).
            await Assertions.Expect(pagina.GetByText("Asistente de configuración (IA)"))
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });
        });
    }

    /// <summary>
    /// Completa el formulario de registro de servidor (MudBlazor InteractiveServer) y lo reenvía con
    /// reintento hasta que el servidor aparece en la tabla, absorbiendo el arranque del circuito.
    /// </summary>
    private static async Task RegistrarServidorAsync(IPage pagina, string servidorId, string? canalSalida)
    {
        var fila = pagina.GetByText(servidorId).First;
        for (var intento = 0; intento < 10; intento++)
        {
            await pagina.GetByLabel("ID del servidor (Discord, numérico)").FillAsync(servidorId);
            await pagina.GetByLabel("Token del bot").FillAsync("token-de-ejemplo-e2e");
            if (canalSalida is not null)
            {
                await pagina.GetByLabel("ID del canal de reportes (opcional)").FillAsync(canalSalida);
            }

            await pagina.GetByRole(AriaRole.Button, new() { Name = "Registrar" }).ClickAsync();
            try
            {
                await fila.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 2_000,
                });
                return;
            }
            catch (TimeoutException)
            {
                // Circuito no listo o el formulario se limpió sin postear; reintentar.
            }
        }

        await Assertions.Expect(fila).ToBeVisibleAsync(new() { Timeout = TimeoutMs });
    }

    /// <summary>
    /// Completa y guarda una regla de contenido en /configuracion con reintento hasta ver la
    /// confirmación, para tolerar el arranque del circuito de Blazor.
    /// El formulario es un wizard de 3 pasos: paso 1 selecciona la categoría (ContenidoRegex
    /// queda seleccionada por defecto) y se avanza con "Siguiente"; paso 2 completa nombre y patrón.
    /// </summary>
    private static async Task GuardarReglaConReintentoAsync(
        IPage pagina, string nombre, string patron, ILocator confirmacion)
    {
        for (var intento = 0; intento < 10; intento++)
        {
            // Paso 1: si "Siguiente" está visible, estamos en el selector de categoría.
            // ContenidoRegex es la primera opción y queda seleccionada por defecto.
            var botonSiguiente = pagina.GetByRole(AriaRole.Button, new() { Name = "Siguiente", Exact = true });
            if (await botonSiguiente.IsVisibleAsync())
            {
                await botonSiguiente.ClickAsync();
                await Task.Delay(300); // absorbe la re-render del circuito al cambiar de paso
            }

            // Paso 2: completar nombre y patrón.
            await pagina.GetByLabel("Nombre").First.FillAsync(nombre);
            await pagina.GetByLabel("Patrón (expresión regular)").FillAsync(patron);
            await pagina.GetByRole(AriaRole.Button, new() { Name = "Guardar", Exact = true }).ClickAsync();
            try
            {
                await confirmacion.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 2_000,
                });
                return;
            }
            catch (TimeoutException)
            {
                // Circuito no listo o guardado fallido; reintentar desde el paso 1.
            }
        }
    }

    private static async Task IngresarAsync(IPage pagina)
    {
        await IniciarSesionAsync(pagina, Usuario, Clave);
        await Assertions.Expect(pagina).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex("/incidentes"));
    }
}
