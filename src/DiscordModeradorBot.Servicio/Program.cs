using System.Security.Claims;
using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Components;
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

// Render interactivo del lado servidor + MudBlazor (intake §17 P.1).
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Persistencia SQLite (archivo local; modo WAL configurado en la conexión, ADR-02) y
// composición del Dominio/Aplicación/Infraestructura (ADR-01, ADR-04).
var rutaBase = Path.Combine(AppContext.BaseDirectory, "discordmoderador.db");
var cadenaConexion = new SqliteConnectionStringBuilder
{
    DataSource = rutaBase,
}.ToString();

// Modo de gateway por configuración (Moderacion:Gateway = Simulado | Discord). Default Simulado:
// dev/tests corren sin red ni token (ADR-04, ADR-13). En Discord se abren las conexiones reales.
var modoGateway = builder.Configuration.GetValue("Moderacion:Gateway", ModoGateway.Simulado);

builder.Services.AgregarServiciosModeracion(cadenaConexion, modoGateway);

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
        // (mitiga CSRF) y Secure=Always (solo viaja por HTTPS). El panel se sirve por HTTPS;
        // Secure=Always es coherente con el despliegue como servicio del sistema (ADR-05).
        opciones.Cookie.HttpOnly = true;
        opciones.Cookie.SameSite = SameSiteMode.Strict;
        opciones.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
    opciones.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

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
