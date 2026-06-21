using DiscordModeradorBot.Servicio.E2E.Soporte;
using Microsoft.Playwright;

namespace DiscordModeradorBot.Servicio.E2E;

/// <summary>
/// E2E de la REVISIÓN DE INCIDENTES (CU-06) y el DESBANEO (CU-07). Autenticado, /incidentes lista los
/// incidentes; el detalle muestra la copia de mensajes y los canales afectados; sobre un incidente de
/// baneo real la acción "Desbanear" (con confirmación) marca el incidente como revertido (RN-11).
/// </summary>
public sealed class IncidentesYDesbaneoE2E : PruebaE2EBase
{
    private const string Usuario = "admin";
    private const string Clave = "clave-segura-2026";

    [HechoSaltable]
    public async Task Autenticado_lista_incidentes_y_el_detalle_muestra_mensajes_y_canales()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);
            var incidenteId = await host.SembrarIncidenteBaneoAsync();

            await IngresarAsync(pagina);

            // Then: /incidentes lista el incidente sembrado.
            await pagina.GotoAsync("/incidentes", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await Assertions.Expect(pagina.GetByRole(AriaRole.Heading,
                new() { Name = "Incidentes de moderación" })).ToBeVisibleAsync();
            await Assertions.Expect(pagina.GetByText("Ráfaga distribuida (e2e)").First).ToBeVisibleAsync();

            // When: abro el detalle. Then: muestra la copia de mensajes y los canales afectados.
            await pagina.GotoAsync($"/incidentes/{incidenteId}", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
            });
            await Assertions.Expect(pagina.GetByText("Canales afectados")).ToBeVisibleAsync();
            await Assertions.Expect(pagina.GetByText("Mensajes accionados")).ToBeVisibleAsync();
            await Assertions.Expect(pagina.GetByText("mensaje de ráfaga 1 (e2e)")).ToBeVisibleAsync();
            await Assertions.Expect(pagina.GetByText("300000000000000001").First).ToBeVisibleAsync();
        });
    }

    [HechoSaltable]
    public async Task Sobre_un_baneo_real_desbanear_con_confirmacion_marca_el_incidente_revertido()
    {
        await EjecutarEscenarioAsync(async (pagina, host) =>
        {
            await host.SembrarAdministradorAsync(Usuario, Clave);
            var incidenteId = await host.SembrarIncidenteBaneoAsync();

            await IngresarAsync(pagina);

            // Given: detalle de un baneo ejecutado, candidato a reversión (botón "Desbanear" visible).
            await pagina.GotoAsync($"/incidentes/{incidenteId}", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
            });
            var botonDesbanear = pagina.GetByRole(AriaRole.Button, new() { Name = "Desbanear" });
            await Assertions.Expect(botonDesbanear).ToBeVisibleAsync();

            // When: confirmo el desbaneo en el diálogo (acción destructiva con confirmación, CU-07).
            // La página es InteractiveServer: el click solo dispara el diálogo una vez conectado el
            // circuito de Blazor. Se reintenta el click hasta que el diálogo aparece, para no perder
            // el primer click si el circuito todavía no estaba listo.
            var botonRevertir = pagina.GetByRole(AriaRole.Button, new() { Name = "Revertir" });
            await ClickConReintentoHastaAsync(botonDesbanear, botonRevertir);
            await botonRevertir.ClickAsync();

            // Then: el incidente queda marcado como revertido (el detalle muestra el aviso de éxito).
            await Assertions.Expect(
                pagina.GetByText("El baneo de este incidente ya fue revertido."))
                .ToBeVisibleAsync(new() { Timeout = TimeoutMs });
        });
    }

    /// <summary>Inicia sesión por la UI y espera el panel; deja la cookie de sesión en el contexto.</summary>
    private static async Task IngresarAsync(IPage pagina)
    {
        await IniciarSesionAsync(pagina, Usuario, Clave);
        await Assertions.Expect(pagina).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex("/incidentes"));
    }
}
