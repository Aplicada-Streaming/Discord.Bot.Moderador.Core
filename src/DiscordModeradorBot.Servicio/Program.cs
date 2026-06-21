using DiscordModeradorBot.Servicio.Components;
using DiscordModeradorBot.Servicio.Infraestructura;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
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

// Servicio en segundo plano del walking skeleton (R1).
builder.Services.AddHostedService<WalkingSkeletonHostedService>();

var app = builder.Build();

// Aplicar la migración inicial y habilitar el modo WAL al arranque (ADR-02, MIG-0001).
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
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Expone la clase Program para los tests de integración (WebApplicationFactory).
public partial class Program;
