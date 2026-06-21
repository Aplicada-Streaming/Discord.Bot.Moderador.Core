using System.Security.Claims;
using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Components;
using DiscordModeradorBot.Servicio.Infraestructura;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
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

builder.Services.AgregarServiciosModeracion(cadenaConexion);

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
        opciones.Cookie.HttpOnly = true;
        opciones.Cookie.SameSite = SameSiteMode.Strict;
        // La sesión vence (CU-09 CA-03); al vencer se exige reautenticar.
        opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
        opciones.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Servicio en segundo plano del walking skeleton (R1).
builder.Services.AddHostedService<WalkingSkeletonHostedService>();

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
    ServicioAdministrador servicioAdministrador) =>
{
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
}).DisableAntiforgery();

app.MapPost("/api/auth/ingresar", async (
    HttpContext ctx,
    ServicioAdministrador servicioAdministrador) =>
{
    var formulario = await ctx.Request.ReadFormAsync();
    var usuario = formulario["usuario"].ToString();
    var contrasena = formulario["contrasena"].ToString();

    // Sin cuenta: se redirige al primer ingreso (CU-09 CA-04, AUTH_SIN_CUENTA).
    if (!await servicioAdministrador.ExisteAdministradorAsync())
    {
        return Results.Redirect("/configuracion-inicial");
    }

    if (!await servicioAdministrador.VerificarCredencialesAsync(usuario, contrasena))
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
}).DisableAntiforgery();

app.MapPost("/api/auth/salir", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/ingresar");
}).DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Expone la clase Program para los tests de integración (WebApplicationFactory).
public partial class Program;
