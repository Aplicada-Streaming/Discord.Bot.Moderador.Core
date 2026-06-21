using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using DiscordModeradorBot.Servicio.Infraestructura.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>
/// Composición de dependencias del Dominio/Aplicación/Infraestructura (ADR-01, ADR-04).
/// El adaptador del gateway activo en R1 es el simulado; el de Discord queda compilando
/// pero no registrado por defecto.
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarServiciosModeracion(
        this IServiceCollection services, string cadenaConexionSqlite)
    {
        // Persistencia EF Core SQLite en modo WAL (ADR-02). El PRAGMA journal_mode=WAL se
        // aplica en Program.cs al crear/migrar la base.
        services.AddDbContext<ContextoPersistencia>(opciones =>
            opciones.UseSqlite(cadenaConexionSqlite));

        services.AddScoped<IRepositorioServidores, RepositorioServidores>();
        services.AddScoped<IRepositorioIncidentes, RepositorioIncidentes>();
        services.AddScoped<IRepositorioReglasContenido, RepositorioReglasContenido>();
        services.AddScoped<IRepositorioAdministrador, RepositorioAdministrador>();
        // Exenciones por servidor: descarte previo de exentos (CU-15, RN-07, R5).
        services.AddScoped<IRepositorioExenciones, RepositorioExenciones>();

        // Servicios transversales (Infraestructura).
        services.AddSingleton<IReloj, RelojDelSistema>();
        services.AddSingleton<IServicioCifrado>(_ => new ServicioCifradoAes());
        // Hashing PBKDF2 del administrador en formato PHC (ADR-03, RN-13); usa primitivas del
        // framework, sin paquetes nuevos.
        services.AddSingleton<IServicioHashContrasena>(_ => new ServicioHashContrasenaPbkdf2());

        // Adaptador del gateway activo en R1: el simulado (ADR-04, intake §18).
        services.AddSingleton<AdaptadorGatewaySimulado>();
        services.AddSingleton<IAdaptadorGateway>(sp => sp.GetRequiredService<AdaptadorGatewaySimulado>());

        // Núcleo de dominio.
        services.AddSingleton<EstadoConductaEnMemoria>();
        services.AddSingleton<EvaluadorRafagaDistribuida>();
        // Evaluador de reglas de contenido sin estado (CU-04, R3); el tope de tiempo viaja en
        // cada regla (matchTimeout), no en el evaluador (ADR-08).
        services.AddSingleton<EvaluadorReglaContenido>();
        // Evaluador de exenciones del descarte previo (CU-15, RN-07, R5): predicado puro.
        services.AddSingleton<EvaluadorExenciones>();

        // Política de ráfaga distribuida en modo EJECUCIÓN para R2: reporta al canal privado y
        // luego banea con borrado retroactivo, en ese orden (RN-05, intake §6). El modo seguro
        // por defecto sigue siendo simulación a nivel del dominio (RC-10, RN-09); aquí se
        // configura ejecución para demostrar el camino real end-to-end en el walking skeleton.
        services.AddSingleton<IReadOnlyList<Politica>>(_ => new[]
        {
            new Politica(
                nombre: "Ráfaga distribuida",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
        });

        // Orquestación (Aplicación).
        services.AddScoped<ServicioRegistroServidor>();
        services.AddScoped<ServicioRegistroReglaContenido>();
        // Gestión de exenciones por servidor para el panel y el walking skeleton (CU-15, R5).
        services.AddScoped<ServicioExenciones>();
        services.AddScoped<MotorDeModeracion>();
        // Autenticación del administrador (CU-08/CU-09) y reversión de baneos (CU-07).
        services.AddScoped<ServicioAdministrador>();
        services.AddScoped<ServicioDesbaneo>();

        return services;
    }
}
