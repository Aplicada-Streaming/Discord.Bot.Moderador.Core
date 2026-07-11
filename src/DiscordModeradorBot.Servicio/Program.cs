using System.Security.Claims;
using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Components;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Infraestructura;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Static Web Assets (assets de _content/* como el JS/CSS de MudBlazor y el framework de Blazor) se
// cargan automáticamente en Development y desde la salida publicada. En el entorno de pruebas e2e
// ("E2E") se corre contra la salida de build (no publicada) y en un entorno no-Development, así que
// hay que habilitarlos explícitamente para que MudBlazor (diálogos, snackbar) y la interactividad de
// Blazor funcionen bajo Playwright. No afecta dev ni producción.
if (builder.Environment.IsEnvironment("E2E"))
{
    builder.WebHost.UseStaticWebAssets();
}

// Render interactivo del lado servidor + MudBlazor (intake §17 P.1).
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Persistencia SQLite (archivo local; modo WAL configurado en la conexión, ADR-02) y
// composición del Dominio/Aplicación/Infraestructura (ADR-01, ADR-04). La ruta del archivo es
// configurable por la clave Persistencia:RutaBase (por defecto, junto al ejecutable): permite que
// las pruebas e2e apunten la base a un archivo temporal por corrida, sin tocar la base local de
// desarrollo. En producción no se configura y conserva el comportamiento por defecto.
var rutaBase = builder.Configuration["Persistencia:RutaBase"];
if (string.IsNullOrWhiteSpace(rutaBase))
{
    rutaBase = Path.Combine(AppContext.BaseDirectory, "discordmoderador.db");
}

var cadenaConexion = new SqliteConnectionStringBuilder
{
    DataSource = rutaBase,
}.ToString();

// Modo de gateway por configuración (Moderacion:Gateway = Simulado | Discord). Default Simulado:
// dev/tests corren sin red ni token (ADR-04, ADR-13). En Discord se abren las conexiones reales.
var modoGateway = builder.Configuration.GetValue("Moderacion:Gateway", ModoGateway.Simulado);

builder.Services.AgregarServiciosModeracion(cadenaConexion, modoGateway);

// Política de cookie Secure: SameAsRequest. El panel se sirve por HTTP (auto-hospedado en el
// dispositivo sin certificado, ADR-05; e igual en desarrollo/local), por lo que forzar Secure=Always
// rompe la cookie de sesión y el token antiforgery al no ser una request SSL. Con SameAsRequest la
// cookie viaja como Secure automáticamente cuando la request es HTTPS (por ejemplo detrás de un
// proxy TLS) y funciona sobre HTTP cuando no hay TLS; no se baja la postura cuando sí hay HTTPS.
var politicaCookieSecure = CookieSecurePolicy.SameAsRequest;

// Autenticación del administrador único por cookie (CU-09, ADR-03, RN-12). Blazor interactivo
// Server no puede setear cookies desde un componente, así que el sign-in/sign-out lo emiten
// endpoints minimal API (más abajo); aquí solo se configura el esquema de cookie y el gating.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/ingresar";
        opciones.AccessDeniedPath = "/ingresar";
        opciones.Cookie.Name = "DiscordModerador.Auth";
        // Flags seguros de la cookie de sesión (ADR-03, endurecimiento de seguridad):
        // HttpOnly (no accesible desde JS, mitiga XSS sobre la sesión), SameSite=Strict
        // (mitiga CSRF) y SecurePolicy=SameAsRequest (Secure automático bajo HTTPS; funcional
        // sobre HTTP, que es como se sirve el panel auto-hospedado, ADR-05).
        opciones.Cookie.HttpOnly = true;
        opciones.Cookie.SameSite = SameSiteMode.Strict;
        opciones.Cookie.SecurePolicy = politicaCookieSecure;
        // La sesión vence (CU-09 CA-03); al vencer se exige reautenticar.
        opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
        opciones.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Antiforgery con flags seguros para la cookie del token (endurecimiento de seguridad). Los POST
// de login/first-run/logout validan el token antiforgery (más abajo), de modo que solo formularios
// emitidos por el propio panel pueden postear (protección CSRF).
builder.Services.AddAntiforgery(opciones =>
{
    opciones.Cookie.HttpOnly = true;
    opciones.Cookie.SameSite = SameSiteMode.Strict;
    opciones.Cookie.SecurePolicy = politicaCookieSecure;
});

// El servicio en segundo plano (walking skeleton simulado o gestor de conexiones real) se registra
// según el modo de gateway dentro de AgregarServiciosModeracion (ADR-13).

var app = builder.Build();

// Aplicar la migración inicial y habilitar el modo WAL al arranque (ADR-02, MIG-0001..MIG-0004).
using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<ContextoPersistencia>();
    contexto.Database.Migrate();

    // PRAGMA journal_mode=WAL para tolerar escrituras concurrentes bot/panel (ADR-02).
    var conexion = (SqliteConnection)contexto.Database.GetDbConnection();
    conexion.Open();
    using var comando = conexion.CreateCommand();
    comando.CommandText = "PRAGMA journal_mode=WAL;";
    comando.ExecuteNonQuery();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// Sirve los archivos físicos de wwwroot (incluye wwwroot/_framework/blazor.web.js). Complementa a
// MapStaticAssets: en el publish del contenedor el manifest de endpoints no siempre incluye el
// script del framework, y sin él la interactividad InteractiveServer no arranca (blazor.web.js 404).
// UseStaticFiles sirve el archivo físico directamente, sin depender del manifest (ver
// Infra.Documentacion/Consideracones/blazor-web-js-404-en-contenedor.md).
app.UseStaticFiles();

// El gating de autenticación/autorización va antes del antiforgery y del ruteo de componentes.
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Endpoints de autenticación (minimal API). Las páginas Blazor /configuracion-inicial e
// /ingresar postean a estos endpoints, que validan contra ServicioAdministrador y emiten/limpian
// la cookie (CU-08, CU-09); un componente interactivo no puede setear la cookie por sí mismo.
app.MapPost("/api/auth/configuracion-inicial", async (
    HttpContext ctx,
    IAntiforgery antiforgery,
    ServicioAdministrador servicioAdministrador) =>
{
    // Validación antiforgery del POST de first-run (protección CSRF, endurecimiento de seguridad).
    if (!await EsRequestValidaAsync(antiforgery, ctx))
    {
        return Results.Redirect("/configuracion-inicial?error=ANTIFORGERY");
    }

    var formulario = await ctx.Request.ReadFormAsync();
    var usuario = formulario["usuario"].ToString();
    var contrasena = formulario["contrasena"].ToString();
    var confirmacion = formulario["confirmacion"].ToString();

    // Confirmación no coincidente: se rechaza sin crear cuenta (CU-08 CA-04).
    if (!string.Equals(contrasena, confirmacion, StringComparison.Ordinal))
    {
        return Results.Redirect("/configuracion-inicial?error=confirmacion");
    }

    var resultado = await servicioAdministrador.CrearAdministradorInicialAsync(usuario, contrasena);
    if (!resultado.Exito)
    {
        var codigo = resultado.Error switch
        {
            ErrorAltaAdministrador.SetupYaCompletado => "SETUP_YA_COMPLETADO",
            ErrorAltaAdministrador.SetupContrasenaDebil => "SETUP_CONTRASENA_DEBIL",
            _ => "SETUP_IDENTIFICADOR_INVALIDO",
        };
        // Si ya estaba completado, el alta se bloquea y se redirige a la autenticación (CU-08 CA-03).
        return resultado.Error == ErrorAltaAdministrador.SetupYaCompletado
            ? Results.Redirect("/ingresar")
            : Results.Redirect($"/configuracion-inicial?error={codigo}");
    }

    // Tras el alta se dirige a la autenticación para iniciar sesión (CU-08 paso 7).
    return Results.Redirect("/ingresar?creado=1");
});

app.MapPost("/api/auth/ingresar", async (
    HttpContext ctx,
    IAntiforgery antiforgery,
    ServicioAdministrador servicioAdministrador) =>
{
    // Validación antiforgery del POST de login (protección CSRF, endurecimiento de seguridad).
    if (!await EsRequestValidaAsync(antiforgery, ctx))
    {
        return Results.Redirect("/ingresar?error=ANTIFORGERY");
    }

    var formulario = await ctx.Request.ReadFormAsync();
    var usuario = formulario["usuario"].ToString();
    var contrasena = formulario["contrasena"].ToString();

    // Sin cuenta: se redirige al primer ingreso (CU-09 CA-04, AUTH_SIN_CUENTA).
    if (!await servicioAdministrador.ExisteAdministradorAsync())
    {
        return Results.Redirect("/configuracion-inicial");
    }

    // Clave de seguimiento del control de intentos (CU-09 AUTH_DEMASIADOS_INTENTOS): combina el
    // identificador normalizado y la IP de origen para limitar tanto por cuenta como por origen,
    // sin filtrar si el usuario existe (sin enumeración de cuentas).
    var clave = ClaveIntentos(usuario, ctx);
    var resultado = await servicioAdministrador.AutenticarAsync(usuario, contrasena, clave);

    if (resultado == ResultadoAutenticacion.DemasiadosIntentos)
    {
        // Bloqueo temporal por demasiados intentos fallidos (CU-09 §6, AUTH_DEMASIADOS_INTENTOS).
        return Results.Redirect("/ingresar?error=AUTH_DEMASIADOS_INTENTOS");
    }

    if (resultado == ResultadoAutenticacion.CredencialesInvalidas)
    {
        // Mensaje neutro, sin distinguir qué campo falló (CU-09 CA-02, sin enumeración de cuentas).
        return Results.Redirect("/ingresar?error=AUTH_CREDENCIALES_INVALIDAS");
    }

    // Credenciales válidas: se abre la sesión autorizada del rol administrador (RN-12).
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, usuario.Trim()),
        new Claim(ClaimTypes.Role, "administrador"),
    };
    var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

    return Results.Redirect("/incidentes");
});

app.MapPost("/api/auth/salir", async (HttpContext ctx, IAntiforgery antiforgery) =>
{
    // Validación antiforgery del POST de logout (protección CSRF, endurecimiento de seguridad).
    if (!await EsRequestValidaAsync(antiforgery, ctx))
    {
        return Results.Redirect("/ingresar");
    }

    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/ingresar");
});

// Cambio de contraseña del administrador autenticado (RN-13). RequireAuthorization: solo una sesión
// válida puede invocarlo. Exige la contraseña actual y verifica la nueva contra la política; nunca
// maneja contraseñas en claro fuera del hash/verify y no las loguea.
app.MapPost("/api/auth/cambiar-contrasena", async (
    HttpContext ctx,
    IAntiforgery antiforgery,
    ServicioAdministrador servicioAdministrador) =>
{
    if (!await EsRequestValidaAsync(antiforgery, ctx))
    {
        return Results.Redirect("/cambiar-contrasena?error=ANTIFORGERY");
    }

    var formulario = await ctx.Request.ReadFormAsync();
    var actual = formulario["actual"].ToString();
    var nueva = formulario["nueva"].ToString();
    var confirmacion = formulario["confirmacion"].ToString();

    // La confirmación debe coincidir con la nueva contraseña (evita un cambio por tipeo).
    if (!string.Equals(nueva, confirmacion, StringComparison.Ordinal))
    {
        return Results.Redirect("/cambiar-contrasena?error=confirmacion");
    }

    var resultado = await servicioAdministrador.CambiarContrasenaAsync(actual, nueva);
    if (!resultado.Exito)
    {
        var codigo = resultado.Error switch
        {
            ErrorCambioContrasena.ContrasenaActualInvalida => "ACTUAL_INVALIDA",
            ErrorCambioContrasena.ContrasenaNuevaDebil => "NUEVA_DEBIL",
            _ => "SIN_CUENTA",
        };
        return Results.Redirect($"/cambiar-contrasena?error={codigo}");
    }

    return Results.Redirect("/cambiar-contrasena?ok=1");
}).RequireAuthorization();

// Endpoints de SEMBRADO SOLO para el entorno de pruebas e2e ("E2E"). Permiten que la suite e2e
// prepare un estado determinista (un administrador y/o un incidente conocido) usando los servicios y
// repositorios reales, sin depender del temporizado del walking skeleton ni del navegador. NO se
// montan fuera de E2E: en dev/producción estos endpoints no existen (no son superficie de ataque).
if (app.Environment.IsEnvironment("E2E"))
{
    MapearEndpointsSembradoE2E(app);
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Monta los endpoints de sembrado e2e (SOLO entorno E2E). Crean un administrador conocido y/o un
// incidente determinista mediante los servicios y repositorios reales, para que los tests del panel
// (incidentes, detalle, desbaneo) tengan datos estables sin pasar por el walking skeleton.
static void MapearEndpointsSembradoE2E(WebApplication app)
{
    // Crea (idempotente) un administrador con credenciales conocidas para los flujos de login.
    app.MapPost("/e2e/seed/administrador", async (
        HttpContext ctx, ServicioAdministrador servicioAdministrador) =>
    {
        var formulario = await ctx.Request.ReadFormAsync();
        var usuario = formulario["usuario"].ToString();
        var contrasena = formulario["contrasena"].ToString();
        var resultado = await servicioAdministrador.CrearAdministradorInicialAsync(usuario, contrasena);

        // SetupYaCompletado es aceptable (idempotencia entre tests de la misma corrida).
        return resultado.Exito || resultado.Error == ErrorAltaAdministrador.SetupYaCompletado
            ? Results.Ok()
            : Results.BadRequest(resultado.Error?.ToString());
    });

    // Crea un incidente de baneo EJECUTADO (real) con copia de mensajes y canales, candidato a
    // desbaneo (CU-06/CU-07, RN-11). Devuelve el id del incidente persistido.
    app.MapPost("/e2e/seed/incidente-baneo", async (IRepositorioIncidentes repositorio) =>
    {
        var incidente = new Incidente(
            servidorId: new Snowflake("100000000000000009"),
            usuarioId: new Snowflake("200000000000000002"),
            nombrePolitica: "Ráfaga distribuida (e2e)",
            modo: Modo.Ejecucion,
            accion: TipoAccion.BaneoConBorradoRetroactivo,
            resultado: ResultadoModeracion.Ejecutada,
            mensajesAccionados: new[]
            {
                new MensajeAccionado(
                    new Snowflake("4000000000000000001"), new Snowflake("300000000000000001"),
                    "mensaje de ráfaga 1 (e2e)"),
                new MensajeAccionado(
                    new Snowflake("4000000000000000002"), new Snowflake("300000000000000002"),
                    "mensaje de ráfaga 2 (e2e)"),
            },
            canalesAfectados: new[]
            {
                new Snowflake("300000000000000001"),
                new Snowflake("300000000000000002"),
            },
            instante: DateTimeOffset.UtcNow);

        // Snapshot de IDs antes de insertar para resistir la race condition con
        // WalkingSkeletonHostedService (que crea incidentes demo en paralelo).
        var idsAntes = (await repositorio.ListarAsync()).Select(i => i.Id).ToHashSet();
        await repositorio.AgregarAsync(incidente);
        var todos = await repositorio.ListarAsync();
        var creado = todos.First(i => !idsAntes.Contains(i.Id));
        return Results.Ok(creado.Id);
    });
}

// Valida el token antiforgery de un POST de formulario; devuelve false si falta o es inválido
// (protección CSRF, endurecimiento de seguridad). No lanza al llamador: se traduce a un redirect
// neutro en cada endpoint.
static async Task<bool> EsRequestValidaAsync(IAntiforgery antiforgery, HttpContext ctx)
{
    try
    {
        await antiforgery.ValidateRequestAsync(ctx);
        return true;
    }
    catch (AntiforgeryValidationException)
    {
        return false;
    }
}

// Compone la clave de seguimiento del control de intentos: identificador normalizado + IP de
// origen (CU-09 AUTH_DEMASIADOS_INTENTOS). Es opaca para el control; no se loguea ni se expone, y
// no revela si el usuario existe (sin enumeración de cuentas).
static string ClaveIntentos(string usuario, HttpContext ctx)
{
    var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
    return $"{usuario.Trim().ToLowerInvariant()}|{ip}";
}

// Expone la clase Program para los tests de integración (WebApplicationFactory).
public partial class Program;
