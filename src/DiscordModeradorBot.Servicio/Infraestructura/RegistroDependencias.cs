using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio.Administracion;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using DiscordModeradorBot.Servicio.Infraestructura.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>
/// Composición de dependencias del Dominio/Aplicación/Infraestructura (ADR-01, ADR-04, ADR-13).
/// El adaptador del gateway activo se elige por el flag de configuración Moderacion:Gateway
/// (<see cref="ModoGateway"/>): <c>Simulado</c> (default; sin red ni token, para dev/tests) o
/// <c>Discord</c> (adaptador real con Discord.Net + gestor de conexiones por servidor).
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarServiciosModeracion(
        this IServiceCollection services,
        string cadenaConexionSqlite,
        ModoGateway modoGateway = ModoGateway.Simulado)
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
        // Modelo de configuración normalizado de R7: grupos, eventos y acciones (CU-11).
        services.AddScoped<IRepositorioConfiguracion, RepositorioConfiguracion>();

        // Servicios transversales (Infraestructura).
        services.AddSingleton<IReloj, RelojDelSistema>();
        services.AddSingleton<IServicioCifrado>(_ => new ServicioCifradoAes());
        // Hashing PBKDF2 del administrador en formato PHC (ADR-03, RN-13); usa primitivas del
        // framework, sin paquetes nuevos.
        services.AddSingleton<IServicioHashContrasena>(_ => new ServicioHashContrasenaPbkdf2());
        // Control de intentos fallidos de login en memoria (CU-09 AUTH_DEMASIADOS_INTENTOS, ADR-09):
        // tras N fallos en una ventana bloquea el ingreso por un enfriamiento. Estado efímero,
        // singleton para que sea coherente entre requests; no se persiste. Parámetros por defecto
        // documentados en la propia clase (5 intentos / 15 min ventana / 15 min enfriamiento).
        services.AddSingleton<ControlIntentosAutenticacion>(_ => new ControlIntentosAutenticacion());

        // Registro de estado de conexión por servidor en memoria (CU-13, ADR-13): lo comparte el
        // gestor de conexiones (real) y el panel para mostrar el estado vigente.
        services.AddSingleton<IEstadoConexionGateway, EstadoConexionGatewayEnMemoria>();

        // Selección del adaptador del gateway por el flag Moderacion:Gateway (ADR-13).
        if (modoGateway == ModoGateway.Discord)
        {
            // Modo REAL: adaptador con Discord.Net, que además es la fábrica de conexiones por
            // servidor; el gestor de conexiones abre una conexión por contexto y enruta los
            // mensajes al motor. NO se inyectan mensajes simulados (intake §17 P.1/P.3, CU-13).
            services.AddSingleton<AdaptadorGatewayDiscord>();
            services.AddSingleton<IAdaptadorGateway>(sp => sp.GetRequiredService<AdaptadorGatewayDiscord>());
            services.AddSingleton<IFabricaClienteGateway>(sp => sp.GetRequiredService<AdaptadorGatewayDiscord>());
            services.AddSingleton<GestorConexionesGateway>();
            services.AddHostedService<GestorConexionesGatewayHostedService>();
        }
        else
        {
            // Modo SIMULADO (default): adaptador simulado + walking skeleton, sin red ni token
            // (ADR-04, intake §18). Mantiene dev/tests sin tocar la plataforma real.
            services.AddSingleton<AdaptadorGatewaySimulado>();
            services.AddSingleton<IAdaptadorGateway>(sp => sp.GetRequiredService<AdaptadorGatewaySimulado>());
            services.AddHostedService<WalkingSkeletonHostedService>();
        }

        // Núcleo de dominio.
        services.AddSingleton<EstadoConductaEnMemoria>();
        // Estado de antirrebote por usuario en memoria (CU-16, RN-06, ADR-09, R6): no se persiste.
        services.AddSingleton<EstadoAntirreboteEnMemoria>();
        services.AddSingleton<EvaluadorRafagaDistribuida>();
        // Evaluador de reglas de contenido sin estado (CU-04, R3); el tope de tiempo viaja en
        // cada regla (matchTimeout), no en el evaluador (ADR-08).
        services.AddSingleton<EvaluadorReglaContenido>();
        // Evaluador de exenciones del descarte previo (CU-15, RN-07, R5): predicado puro.
        services.AddSingleton<EvaluadorExenciones>();

        // Las políticas que evalúa el motor se CARGAN desde la configuración persistida del panel
        // (CU-11): eventos → grupos → reglas → acciones. Antes había una única política hardcodeada
        // acá; se eliminó para que la configuración del servidor DIRIJA realmente la moderación. El
        // cargador es scoped porque depende de los repositorios scoped de configuración/reglas.
        services.AddScoped<ICargadorPoliticas, CargadorPoliticasDesdeConfiguracion>();

        // Orquestación (Aplicación).
        services.AddScoped<ServicioRegistroServidor>();
        services.AddScoped<ServicioRegistroReglaContenido>();
        // Gestión de exenciones por servidor para el panel y el walking skeleton (CU-15, R5).
        services.AddScoped<ServicioExenciones>();
        services.AddScoped<MotorDeModeracion>();
        // Autenticación del administrador (CU-08/CU-09) y reversión de baneos (CU-07).
        services.AddScoped<ServicioAdministrador>();
        services.AddScoped<ServicioDesbaneo>();
        // Configuración de moderación dirigida por descriptores (CU-11, RN-10, R7).
        services.AddScoped<ServicioConfiguracionModeracion>();
        // Prueba de configuración previa a la activación del servidor (CU-12, RN-16, R7).
        services.AddScoped<ServicioPruebaConfiguracion>();

        return services;
    }
}
