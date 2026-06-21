using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>
/// Servicio en segundo plano que demuestra el walking skeleton end-to-end. R1 demostraba la
/// detección de ráfaga y el reporte en modo simulación (CU-01, CU-14, RN-09). R2 extiende el
/// escenario para demostrar TAMBIÉN una política en modo ejecución: una ráfaga que dispara,
/// en orden, el reporte al canal privado (CU-05) y el baneo con borrado retroactivo
/// (CU-02/CU-03), persistiendo el incidente como ejecutado con sus mensajes y canales
/// normalizados (RN-05, RN-11). Mantiene el escenario de simulación de R1 intacto (RN-09).
/// Todo corre contra el adaptador simulado; no toca la plataforma real.
/// </summary>
public sealed class WalkingSkeletonHostedService : BackgroundService
{
    private const string ServidorSimulacion = "100000000000000001";
    private const string ServidorEjecucion = "100000000000000009";
    private const string ServidorContenido = "100000000000000033";
    private const string ServidorExencion = "100000000000000055";
    // Servidores de los escenarios de R6: acción adicional (timeout), antirrebote y jerarquía.
    private const string ServidorAccionAdicional = "100000000000000066";
    private const string ServidorAntirrebote = "100000000000000077";
    private const string ServidorJerarquia = "100000000000000088";
    // Servidores de los escenarios de R7: grupo de reglas con modo de coincidencia y prueba de
    // configuración (una que bloquea, otra que activa).
    private const string ServidorGrupo = "100000000000000111";
    private const string ServidorPruebaBloquea = "100000000000000122";
    private const string ServidorPruebaActiva = "100000000000000133";
    private const string UsuarioDemo = "200000000000000002";
    // Usuario con rol jerárquicamente superior al bot (R6, RN-01): la acción no es accionable.
    private const string UsuarioRolSuperior = "200000000000000099";
    private const string CanalSalida = "500000000000000001";
    private const string CanalContenido = "300000000000000044";

    // Sujetos del escenario de exenciones (R5, CU-15): un usuario de staff exento POR ROL
    // (el rol staff está exento; el usuario lo porta) que postea una ráfaga que normalmente
    // dispararía y NO se acciona (queda descartado, sin incidente, RN-07).
    private const string RolStaffExento = "700000000000000001";
    private const string UsuarioStaff = "200000000000000077";

    // Patrón de contenido prohibido de demostración: un enlace a un acortador de URL en un
    // mensaje. Es un ejemplo neutro de regla de contenido por expresión regular (CU-04), sin
    // vocabulario de ningún dominio en particular.
    private const string PatronContenidoProhibido = @"https?://(?:bit\.ly|tinyurl\.com)/\S+";

    // Credenciales del administrador de DESARROLLO sembrado al arrancar (R4). SOLO para uso local
    // y SOLO en el entorno Development (RN-13, ADR-03): en cualquier otro entorno NO se siembra y el
    // alta es el first-run real desde /configuracion-inicial (CU-08). NUNCA un admin con clave por
    // defecto en producción. La contraseña cumple la política mínima (longitud + letra + dígito,
    // RN-13) y puede sobrescribirse por configuración/user-secrets (clave Seed:AdminDesarrollo:*).
    private const string AdminDesarrolloUsuario = "admin";

    /// <summary>
    /// Contraseña por defecto del admin de DESARROLLO. Es un default SOLO-Development, claramente
    /// marcado; el operador puede (y debería) sobrescribirla por user-secrets o variable de entorno
    /// (clave <c>Seed:AdminDesarrollo:Contrasena</c>). Nunca se usa fuera de Development (RN-13).
    /// </summary>
    private const string AdminDesarrolloContrasenaPorDefecto = "cambiar-esta-clave-2026";

    /// <summary>Claves de configuración del seed de desarrollo (user-secrets / variables de entorno).</summary>
    private const string ClaveConfigUsuario = "Seed:AdminDesarrollo:Usuario";
    private const string ClaveConfigContrasena = "Seed:AdminDesarrollo:Contrasena";

    /// <summary>
    /// Flag explícito para DESHABILITAR el seed de desarrollo aun en Development
    /// (<c>Seed:AdminDesarrollo:Habilitado=false</c>). Por defecto está habilitado; el seed sigue
    /// siendo Development-only (RN-13, ADR-03). Lo usan las pruebas e2e del first-run, que necesitan
    /// arrancar el host SIN admin sembrado para probar el alta real (CU-08).
    /// </summary>
    private const string ClaveConfigHabilitado = "Seed:AdminDesarrollo:Habilitado";

    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _entorno;
    private readonly IConfiguration _configuracion;
    private readonly ILogger<WalkingSkeletonHostedService> _logger;

    public WalkingSkeletonHostedService(
        IServiceProvider serviceProvider,
        IHostEnvironment entorno,
        IConfiguration configuracion,
        ILogger<WalkingSkeletonHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _entorno = entorno;
        _configuracion = configuracion;
        _logger = logger;
    }

    /// <summary>
    /// Decide si corresponde sembrar el administrador de DESARROLLO (RN-13, ADR-03): SOLO en el
    /// entorno Development. En cualquier otro entorno (Staging, Production, etc.) devuelve false y
    /// el alta es el first-run real (CU-08). Es estática y pura para poder testearla sin levantar el
    /// host: nunca se siembra un admin con clave por defecto fuera de Development.
    /// </summary>
    public static bool DebeSembrarAdministradorDesarrollo(IHostEnvironment entorno)
    {
        ArgumentNullException.ThrowIfNull(entorno);
        return entorno.IsDevelopment();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        // Aplicar las migraciones al arranque (MIG-0001..MIG-0004, ADR-02).
        var contexto = sp.GetRequiredService<ContextoPersistencia>();
        await contexto.Database.MigrateAsync(stoppingToken);

        // Seed de un administrador de DESARROLLO SOLO en el entorno Development y solo si no existe
        // (R4, RN-13, ADR-03). Evita que el panel quede bloqueado por el first-run real al arrancar
        // localmente; fuera de Development NO se siembra y el alta es el first-run real desde
        // /configuracion-inicial (CU-08), de modo que nunca queda un admin con clave por defecto en
        // producción. No corre en los tests (no levantan el host).
        await SembrarAdministradorDesarrolloAsync(sp, stoppingToken);

        var registro = sp.GetRequiredService<ServicioRegistroServidor>();
        var adaptador = sp.GetRequiredService<IAdaptadorGateway>();

        if (adaptador is not AdaptadorGatewaySimulado simulado)
        {
            return;
        }

        // Escenario 1 — Simulación (regresión de R1): registra el servidor de demostración y
        // procesa una ráfaga en modo simulación; no se invoca ninguna acción real (RN-09).
        await RegistrarServidorAsync(
            registro, ServidorSimulacion, "Servidor de demostración (simulación)", null, stoppingToken);
        await CorrerRafagaAsync(
            sp, simulado, ServidorSimulacion, Modo.Simulacion, canalSalida: null, stoppingToken);

        // Escenario 2 — Ejecución (R2): registra un servidor con canal de salida designado y
        // procesa una ráfaga en modo ejecución; el motor reporta y luego banea, en orden
        // (RN-05), y persiste el incidente ejecutado con su evidencia normalizada (RN-11).
        var canal = new CanalDeSalida(new Snowflake(CanalSalida), CanalDeSalida.PropositoReporteIncidentes);
        await RegistrarServidorAsync(
            registro, ServidorEjecucion, "Servidor de demostración (ejecución)", canal, stoppingToken);
        await CorrerRafagaAsync(
            sp, simulado, ServidorEjecucion, Modo.Ejecucion, canal, stoppingToken);

        // Escenario 3 — Contenido prohibido (R3, CU-04): registra un servidor con canal de salida,
        // configura una regla de contenido por expresión regular (validada al guardar, RN-03) y
        // procesa UN ÚNICO mensaje con contenido prohibido. El mensaje aislado coincide con la
        // regex (eje de contenido, sin estado) y dispara la política, que reutiliza el camino de
        // acciones de R2: reportar y banear, en orden, persistiendo el incidente (RN-05, RN-11).
        await RegistrarServidorAsync(
            registro, ServidorContenido, "Servidor de demostración (contenido)", canal, stoppingToken);
        await CorrerContenidoProhibidoAsync(sp, simulado, ServidorContenido, canal, stoppingToken);

        // Escenario 4 — Exención por rol (R5, CU-15, RN-07): registra un servidor con canal de
        // salida, declara una exención por el rol staff y procesa una ráfaga distribuida de un
        // usuario que porta ese rol. El descarte de exentos (etapa 1) ocurre ANTES de evaluar:
        // el mensaje se descarta, no se actualiza el estado de conducta, NO se dispara política y
        // NO se registra incidente ni acción. Se contrasta con un usuario SIN el rol (regresión),
        // cuya misma ráfaga SÍ dispara la política.
        await RegistrarServidorAsync(
            registro, ServidorExencion, "Servidor de demostración (exenciones)", canal, stoppingToken);
        await CorrerExencionPorRolAsync(sp, simulado, ServidorExencion, canal, stoppingToken);

        // Escenario 5 — Acción adicional (R6, intake §4 Should Have): una política en ejecución
        // cuyas acciones son reportar y luego TIMEOUT (silenciar por una duración). Demuestra que
        // el catálogo extendido se ejecuta por el motor en el orden declarado (RN-05), reutilizando
        // el camino de R2.
        await RegistrarServidorAsync(
            registro, ServidorAccionAdicional, "Servidor de demostración (acción adicional)", canal,
            stoppingToken);
        await CorrerAccionAdicionalAsync(sp, simulado, ServidorAccionAdicional, stoppingToken);

        // Escenario 6 — Antirrebote (R6, CU-16, RN-06): una ráfaga que dispara varias veces sobre
        // el MISMO usuario dentro de la ventana de antirrebote; solo se acciona UNA vez y las
        // repeticiones se suprimen (0 acciones adicionales).
        await RegistrarServidorAsync(
            registro, ServidorAntirrebote, "Servidor de demostración (antirrebote)", canal,
            stoppingToken);
        await CorrerAntirreboteAsync(sp, simulado, ServidorAntirrebote, stoppingToken);

        // Escenario 7 — Jerarquía no accionable (R6, RN-01, CU-02 §7): el usuario tiene rol
        // superior al bot; la acción no se puede aplicar, el incidente queda NoAccionable pero
        // igual se reporta y el pipeline NO se cae (ADR-08).
        await RegistrarServidorAsync(
            registro, ServidorJerarquia, "Servidor de demostración (jerarquía)", canal, stoppingToken);
        await CorrerJerarquiaNoAccionableAsync(sp, simulado, ServidorJerarquia, stoppingToken);

        // Escenario 8 — Grupo de reglas con modo de coincidencia (R7, RN-15): una política cuya
        // condición de disparo es un GRUPO en modo AlMenosN(2) que combina una regla de contenido
        // (URL) y una de conducta (ráfaga). Solo dispara si al menos 2 de sus reglas coinciden.
        await RegistrarServidorAsync(
            registro, ServidorGrupo, "Servidor de demostración (grupo de reglas)", canal, stoppingToken);
        await CorrerGrupoDeReglasAsync(sp, simulado, ServidorGrupo, stoppingToken);

        // Escenario 9 — Prueba de configuración (R7, CU-12, RN-16): registra dos servidores y prueba
        // su configuración. El primero FALLA por un chequeo bloqueante (permisos faltantes) y NO se
        // activa; el segundo PASA (solo una advertencia de jerarquía) y SÍ se activa.
        await RegistrarServidorAsync(
            registro, ServidorPruebaBloquea, "Servidor de demostración (prueba bloquea)", canal, stoppingToken);
        await RegistrarServidorAsync(
            registro, ServidorPruebaActiva, "Servidor de demostración (prueba activa)", canal, stoppingToken);
        await CorrerPruebaConfiguracionAsync(sp, simulado, stoppingToken);

        _logger.LogInformation(
            "[WALKING SKELETON] Escenarios completados (ráfaga simulación, ráfaga ejecución, " +
            "contenido prohibido, exención por rol, acción adicional timeout, antirrebote, " +
            "jerarquía no accionable, grupo de reglas con modo de coincidencia, prueba de " +
            "configuración que bloquea y que activa). Acciones ejecutadas registradas por el " +
            "adaptador simulado: {Cantidad}. Revise los incidentes en /incidentes, la configuración " +
            "en /configuracion y los servidores en /servidores.",
            simulado.AccionesEjecutadas.Count);
    }

    /// <summary>
    /// Siembra un administrador de DESARROLLO SOLO en el entorno Development y solo si todavía no
    /// existe (R4, CU-08, RN-13, ADR-03). En cualquier otro entorno NO se siembra: el alta es el
    /// first-run real desde /configuracion-inicial, de modo que NUNCA queda un admin con clave por
    /// defecto en producción. La contraseña se lee de configuración/user-secrets con un default
    /// SOLO-Development claramente marcado. Reutiliza <see cref="ServicioAdministrador"/>, así la
    /// unicidad (RC-06) y el resguardo PHC (RN-13) se respetan igual. Nunca se loguea la contraseña
    /// (RN-13): solo se loguea una advertencia visible de que es un seed de desarrollo.
    /// </summary>
    private async Task SembrarAdministradorDesarrolloAsync(IServiceProvider sp, CancellationToken ct)
    {
        // Acotado a Development (RN-13, ADR-03): fuera de Development no se siembra nada.
        if (!DebeSembrarAdministradorDesarrollo(_entorno))
        {
            return;
        }

        // Apagado explícito por configuración (Seed:AdminDesarrollo:Habilitado=false): aun en
        // Development se puede deshabilitar el seed. Lo usan las pruebas e2e del first-run, que
        // necesitan arrancar SIN admin para probar el alta real (CU-08). Por defecto, habilitado.
        if (!_configuracion.GetValue(ClaveConfigHabilitado, defaultValue: true))
        {
            _logger.LogInformation(
                "[WALKING SKELETON] Seed de administrador de desarrollo DESHABILITADO por configuración " +
                "({ClaveConfig}=false); el panel arranca sin admin (first-run real, CU-08).",
                ClaveConfigHabilitado);
            return;
        }

        var servicioAdministrador = sp.GetRequiredService<ServicioAdministrador>();
        if (await servicioAdministrador.ExisteAdministradorAsync(ct))
        {
            return;
        }

        // Usuario y contraseña configurables (user-secrets / variables de entorno); si no se
        // configuran, se usa el default SOLO-Development claramente marcado. La contraseña jamás se
        // loguea (RN-13).
        var usuario = _configuracion[ClaveConfigUsuario];
        if (string.IsNullOrWhiteSpace(usuario))
        {
            usuario = AdminDesarrolloUsuario;
        }

        var contrasena = _configuracion[ClaveConfigContrasena];
        if (string.IsNullOrWhiteSpace(contrasena))
        {
            contrasena = AdminDesarrolloContrasenaPorDefecto;
        }

        var resultado = await servicioAdministrador.CrearAdministradorInicialAsync(usuario, contrasena, ct);

        if (resultado.Exito)
        {
            // Advertencia VISIBLE de que es un seed de DESARROLLO; sin la contraseña (RN-13).
            _logger.LogWarning(
                "[WALKING SKELETON] Sembrado un administrador de DESARROLLO (usuario '{Usuario}') porque " +
                "el entorno es Development. SOLO local: este seed NUNCA corre fuera de Development; en " +
                "producción el alta es el first-run real (CU-08, /configuracion-inicial). Cambiá la " +
                "contraseña por user-secrets ({ClaveConfig}).",
                usuario, ClaveConfigContrasena);
        }
        else
        {
            _logger.LogWarning(
                "[WALKING SKELETON] No se sembró el administrador de desarrollo ({Codigo}).",
                resultado.Error?.ToString());
        }
    }

    private async Task RegistrarServidorAsync(
        ServicioRegistroServidor registro,
        string servidorId,
        string nombre,
        CanalDeSalida? canal,
        CancellationToken ct)
    {
        var resultado = await registro.RegistrarAsync(
            servidorId, "token-de-ejemplo-walking-skeleton", nombre, ct, canal);

        _logger.LogInformation(
            resultado.Exito
                ? "[WALKING SKELETON] Servidor {Servidor} registrado con token cifrado{Canal}."
                : "[WALKING SKELETON] Servidor {Servidor} ya estaba registrado{Canal}.",
            servidorId,
            canal is null ? string.Empty : $" y canal de salida {canal.SnowflakeCanal.Valor}");
    }

    /// <summary>
    /// Construye un motor con una política en el modo indicado e inyecta una ráfaga
    /// distribuida (mismo usuario en 3 canales distintos dentro de la ventana de detección,
    /// CU-01). En ejecución la política reporta y luego banea, en orden (RN-05).
    /// </summary>
    private async Task CorrerRafagaAsync(
        IServiceProvider sp,
        AdaptadorGatewaySimulado simulado,
        string servidorId,
        Modo modo,
        CanalDeSalida? canalSalida,
        CancellationToken ct)
    {
        var acciones = modo == Modo.Ejecucion
            ? new[]
            {
                new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
            }
            : new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) };

        var politicas = new[]
        {
            new Politica("Ráfaga distribuida", prioridad: 0, modo: modo, acciones: acciones),
        };

        var motor = CrearMotor(sp, simulado, politicas);

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando ráfaga distribuida en modo {Modo} (servidor {Servidor}).",
            modo, servidorId);

        // Suscribe el motor al adaptador y emite la ráfaga por el canal de eventos simulado;
        // se desuscribe al terminar para no acumular handlers entre escenarios (el adaptador
        // simulado es compartido).
        Func<MensajeEntrante, Task> handler = mensaje => motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };
            var baseMensaje = modo == Modo.Ejecucion ? 4000_0000_0000_0000L : 4900_0000_0000_0000L;

            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioDemo),
                    new Snowflake((baseMensaje + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje de ráfaga {i + 1}");

                await simulado.InyectarMensajeAsync(mensaje);
            }
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }
    }

    /// <summary>
    /// Demuestra CU-04 end-to-end: configura una regla de contenido por expresión regular (validada
    /// al guardar, RN-03), la persiste y la asocia a una política en ejecución, y procesa UN ÚNICO
    /// mensaje con contenido prohibido. El predicado sin estado coincide sobre el mensaje aislado y
    /// dispara la política, que reutiliza el camino de acciones de R2 (reportar + banear en orden,
    /// RN-05) y persiste el incidente (RN-11).
    /// </summary>
    private async Task CorrerContenidoProhibidoAsync(
        IServiceProvider sp,
        AdaptadorGatewaySimulado simulado,
        string servidorId,
        CanalDeSalida canalSalida,
        CancellationToken ct)
    {
        const string nombrePolitica = "Contenido prohibido";

        // Configura y PERSISTE la regla de contenido; el registro valida el patrón (RN-03).
        var registroReglas = sp.GetRequiredService<ServicioRegistroReglaContenido>();
        var resultadoRegla = await registroReglas.RegistrarPorExpresionRegularAsync(
            new Snowflake(servidorId), nombrePolitica, "Enlace de acortador",
            PatronContenidoProhibido, sensibleAMayusculas: false, ct);

        if (!resultadoRegla.Exito || resultadoRegla.Regla is null)
        {
            _logger.LogWarning(
                "[WALKING SKELETON] No se pudo registrar la regla de contenido ({Codigo}): {Mensaje}.",
                resultadoRegla.Codigo, resultadoRegla.Mensaje);
            return;
        }

        // Política de CONTENIDO en ejecución: dispara cuando el texto del mensaje cumple la regla
        // (CU-04) y ejecuta las acciones de R2 en orden (RN-05).
        var politicas = new[]
        {
            new Politica(
                nombrePolitica,
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                },
                reglaContenido: resultadoRegla.Regla),
        };

        var motor = CrearMotor(sp, simulado, politicas);

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando UN mensaje con contenido prohibido (servidor {Servidor}); " +
            "la regla de contenido por regex debería disparar la política '{Politica}'.",
            servidorId, nombrePolitica);

        Func<MensajeEntrante, Task> handler = mensaje => motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            var mensaje = new MensajeEntrante(
                new Snowflake(servidorId),
                new Snowflake(CanalContenido),
                new Snowflake(UsuarioDemo),
                new Snowflake("4800000000000000001"),
                ahora,
                "Mirá esta oferta: https://bit.ly/oferta-ahora");

            await simulado.InyectarMensajeAsync(mensaje);
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }
    }

    /// <summary>
    /// Demuestra CU-15/RN-07 end-to-end: declara una exención por el rol staff en el servidor y
    /// procesa la MISMA ráfaga distribuida dos veces. Primero la postea un usuario de staff que
    /// porta el rol exento: el descarte de exentos (etapa 1) lo saca ANTES de evaluar, así que no
    /// se actualiza el estado de conducta, no se dispara política y no se registra incidente ni
    /// acción (queda exento, RN-07). Luego la postea un usuario SIN el rol: la misma ráfaga SÍ
    /// dispara la política (regresión, los no exentos siguen sujetos a la moderación).
    /// </summary>
    private async Task CorrerExencionPorRolAsync(
        IServiceProvider sp,
        AdaptadorGatewaySimulado simulado,
        string servidorId,
        CanalDeSalida canalSalida,
        CancellationToken ct)
    {
        // Declara la exención por rol staff (CU-15): persistida y aplicada por el motor (RN-07).
        var servicioExenciones = sp.GetRequiredService<ServicioExenciones>();
        var alta = await servicioExenciones.AgregarAsync(
            new Snowflake(servidorId), TipoSujetoExento.Rol, RolStaffExento, ct);

        _logger.LogInformation(
            alta.Exito
                ? "[WALKING SKELETON] Exención por rol staff {Rol} declarada en el servidor {Servidor} " +
                  "(CU-15); los usuarios con ese rol quedan fuera de la moderación (RN-07)."
                : "[WALKING SKELETON] No se declaró la exención por rol ({Codigo}); {Mensaje}.",
            alta.Exito ? RolStaffExento : alta.Codigo,
            alta.Exito ? servidorId : alta.Mensaje);

        // Política de ráfaga en ejecución, idéntica a los demás escenarios (RN-05).
        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
        };

        var motor = CrearMotor(sp, simulado, politicas);
        var accionesPrevias = simulado.AccionesEjecutadas.Count;

        Func<MensajeEntrante, Task> handler = mensaje => motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };

            // Ráfaga del usuario de STAFF (porta el rol exento): debe descartarse en la etapa 1.
            _logger.LogInformation(
                "[WALKING SKELETON] Inyectando ráfaga del usuario de STAFF {Usuario} (rol exento {Rol}); " +
                "se espera DESCARTE en la etapa 1 sin incidente (RN-07).",
                UsuarioStaff, RolStaffExento);
            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioStaff),
                    new Snowflake((4700_0000_0000_0000L + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje de staff {i + 1}")
                {
                    RolesDelAutor = new[] { new Snowflake(RolStaffExento) },
                };

                await simulado.InyectarMensajeAsync(mensaje);
            }

            var accionesTrasStaff = simulado.AccionesEjecutadas.Count;
            _logger.LogInformation(
                "[WALKING SKELETON] Tras la ráfaga del staff exento: {Cantidad} acción(es) nuevas " +
                "(se espera 0; el usuario quedó descartado por la exención, CU-15/RN-07).",
                accionesTrasStaff - accionesPrevias);

            // Ráfaga de un usuario SIN el rol exento: la misma actividad SÍ debe disparar (regresión).
            _logger.LogInformation(
                "[WALKING SKELETON] Inyectando la MISMA ráfaga de un usuario NO exento {Usuario}; " +
                "se espera que SÍ dispare la política.",
                UsuarioDemo);
            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioDemo),
                    new Snowflake((4750_0000_0000_0000L + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje no exento {i + 1}");

                await simulado.InyectarMensajeAsync(mensaje);
            }

            _logger.LogInformation(
                "[WALKING SKELETON] Tras la ráfaga del usuario NO exento: {Cantidad} acción(es) nuevas " +
                "respecto del staff (se espera > 0; los no exentos siguen sujetos a la moderación).",
                simulado.AccionesEjecutadas.Count - accionesTrasStaff);
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }
    }

    /// <summary>
    /// Demuestra una ACCIÓN ADICIONAL del catálogo de R6 (intake §4 Should Have): una política
    /// en ejecución cuyas acciones son reportar y luego TIMEOUT (silenciar por una duración). El
    /// motor las ejecuta en el orden declarado (RN-05) reutilizando el camino de R2. Se inyecta
    /// una ráfaga distribuida que dispara la política.
    /// </summary>
    private async Task CorrerAccionAdicionalAsync(
        IServiceProvider sp, AdaptadorGatewaySimulado simulado, string servidorId, CancellationToken ct)
    {
        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida (timeout)",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(
                        TipoAccion.Timeout, OrdenEjecucion: 1, DuracionTimeout: TimeSpan.FromMinutes(15)),
                }),
        };

        var motor = CrearMotor(sp, simulado, politicas);
        var accionesPrevias = simulado.AccionesEjecutadas.Count;

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando ráfaga que dispara una acción adicional TIMEOUT " +
            "(servidor {Servidor}); se espera reporte + timeout en orden (RN-05, R6).",
            servidorId);

        await InyectarRafagaUsuarioAsync(sp, simulado, motor, servidorId, UsuarioDemo, 4600_0000_0000_0000L, ct);

        var nuevas = simulado.AccionesEjecutadas.Skip(accionesPrevias).ToList();
        _logger.LogInformation(
            "[WALKING SKELETON] Tras la acción adicional: {Cantidad} acción(es) nuevas; tipos [{Tipos}] " +
            "(se espera ReporteEjecutado y TimeoutEjecutado).",
            nuevas.Count, string.Join(", ", nuevas.Select(a => a.GetType().Name)));
    }

    /// <summary>
    /// Demuestra el ANTIRREBOTE (R6, CU-16, RN-06): un mismo usuario dispara la política varias
    /// veces dentro de la ventana de antirrebote; solo se acciona la PRIMERA vez y las
    /// repeticiones se suprimen (0 acciones adicionales). Se usa un estado de antirrebote propio
    /// para el escenario y se inyectan dos ráfagas del mismo usuario.
    /// </summary>
    private async Task CorrerAntirreboteAsync(
        IServiceProvider sp, AdaptadorGatewaySimulado simulado, string servidorId, CancellationToken ct)
    {
        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
        };

        // Estado de antirrebote propio del escenario: las dos ráfagas comparten el estado, así la
        // segunda queda suprimida por la marca que dejó la primera (RN-06).
        var antirrebote = new EstadoAntirreboteEnMemoria();
        var motor = CrearMotor(sp, simulado, politicas, antirrebote);

        var accionesPrevias = simulado.AccionesEjecutadas.Count;

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando PRIMERA ráfaga del usuario {Usuario} (servidor {Servidor}); " +
            "se espera que SÍ se accione (primera acción del ataque, RN-06).",
            UsuarioDemo, servidorId);
        await InyectarRafagaUsuarioAsync(sp, simulado, motor, servidorId, UsuarioDemo, 4610_0000_0000_0000L, ct);
        var accionesTrasPrimera = simulado.AccionesEjecutadas.Count;

        _logger.LogInformation(
            "[WALKING SKELETON] Tras la PRIMERA ráfaga: {Cantidad} acción(es) nuevas (se espera > 0).",
            accionesTrasPrimera - accionesPrevias);

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando SEGUNDA ráfaga del MISMO usuario dentro de la ventana de " +
            "antirrebote; se espera que se SUPRIMA (0 acciones adicionales, RN-06/CU-16).");
        await InyectarRafagaUsuarioAsync(sp, simulado, motor, servidorId, UsuarioDemo, 4620_0000_0000_0000L, ct);

        _logger.LogInformation(
            "[WALKING SKELETON] Tras la SEGUNDA ráfaga: {Cantidad} acción(es) adicionales respecto de " +
            "la primera (se espera 0; las repeticiones se suprimieron por antirrebote).",
            simulado.AccionesEjecutadas.Count - accionesTrasPrimera);
    }

    /// <summary>
    /// Demuestra la JERARQUÍA NO ACCIONABLE (R6, RN-01, CU-02 §7): el usuario objetivo tiene rol
    /// jerárquicamente superior al bot; el adaptador devuelve no accionable, el incidente queda
    /// NoAccionable, IGUAL se reporta al canal de incidencias y el pipeline NO se cae (ADR-08).
    /// </summary>
    private async Task CorrerJerarquiaNoAccionableAsync(
        IServiceProvider sp, AdaptadorGatewaySimulado simulado, string servidorId, CancellationToken ct)
    {
        // Marca al usuario como de rol superior: las acciones de contención sobre él no son
        // accionables (RN-01). El reporte SÍ se publica (RN-11).
        simulado.MarcarUsuarioDeRolSuperior(new Snowflake(servidorId), new Snowflake(UsuarioRolSuperior));

        var politicas = new[]
        {
            new Politica(
                "Ráfaga distribuida",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                }),
        };

        var motor = CrearMotor(sp, simulado, politicas);

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando ráfaga de un usuario {Usuario} con ROL SUPERIOR (servidor " +
            "{Servidor}); se espera incidente NoAccionable, reporte publicado y SIN excepción (RN-01, " +
            "ADR-08).",
            UsuarioRolSuperior, servidorId);

        Incidente? ultimo = null;
        Func<MensajeEntrante, Task> handler = async mensaje => ultimo = await motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };
            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioRolSuperior),
                    new Snowflake((4630_0000_0000_0000L + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje de rol superior {i + 1}");
                await simulado.InyectarMensajeAsync(mensaje);
            }
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }

        _logger.LogInformation(
            "[WALKING SKELETON] Resultado del incidente de jerarquía: {Resultado} (se espera " +
            "NoAccionable); el pipeline continuó sin excepción y el incidente se reportó (RN-01).",
            ultimo?.Resultado);
    }

    /// <summary>
    /// Demuestra un GRUPO DE REGLAS con modo de coincidencia (R7, RN-15): una política cuya
    /// condición de disparo es un grupo en modo AlMenosN(2) que combina una regla de CONTENIDO
    /// (URL de acortador) y una de CONDUCTA (ráfaga distribuida). Inyecta una ráfaga cuyos mensajes
    /// además traen la URL prohibida: coinciden las DOS reglas (≥2) y el grupo dispara, por lo que
    /// la política reporta y banea en orden (RN-05).
    /// </summary>
    private async Task CorrerGrupoDeReglasAsync(
        IServiceProvider sp, AdaptadorGatewaySimulado simulado, string servidorId, CancellationToken ct)
    {
        var evaluadorContenido = sp.GetRequiredService<EvaluadorReglaContenido>();
        var evaluadorRafaga = sp.GetRequiredService<EvaluadorRafagaDistribuida>();

        var reglaUrl = ReglaContenido.PorExpresionRegular(
            "Enlace de acortador", PatronContenidoProhibido, EvaluadorReglaContenido.TopeTiempoPorDefecto);

        // Grupo AlMenosN(2): contenido (URL) + conducta (ráfaga). Solo dispara si coinciden ambas.
        var grupo = new GrupoDeReglas(
            "Spam distribuido",
            ModoCoincidencia.AlMenosN,
            new IReglaEvaluable[]
            {
                new ReglaEvaluableContenido(reglaUrl, evaluadorContenido),
                new ReglaEvaluableConducta(evaluadorRafaga),
            },
            minimoCoincidencias: 2);

        var politicas = new[]
        {
            new Politica(
                "Spam distribuido (grupo AlMenosN)",
                prioridad: 0,
                modo: Modo.Ejecucion,
                acciones: new[]
                {
                    new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                    new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
                },
                composicion: new ComposicionPolitica(new[] { grupo })),
        };

        var motor = CrearMotor(sp, simulado, politicas);
        var accionesPrevias = simulado.AccionesEjecutadas.Count;

        _logger.LogInformation(
            "[WALKING SKELETON] Inyectando ráfaga CON URL prohibida (servidor {Servidor}); el grupo " +
            "AlMenosN(2) [contenido + conducta] debería disparar al coincidir AMBAS reglas (RN-15). " +
            "Explicación: {Explicacion}",
            servidorId, ExplicadorEnPalabras.Explicar(politicas[0]));

        Func<MensajeEntrante, Task> handler = mensaje => motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };
            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(UsuarioDemo),
                    new Snowflake((4500_0000_0000_0000L + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"oferta https://bit.ly/oferta-{i}");
                await simulado.InyectarMensajeAsync(mensaje);
            }
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }

        _logger.LogInformation(
            "[WALKING SKELETON] Tras el grupo AlMenosN(2): {Cantidad} acción(es) nuevas (se espera > 0; " +
            "ambas reglas coincidieron).",
            simulado.AccionesEjecutadas.Count - accionesPrevias);
    }

    /// <summary>
    /// Demuestra la PRUEBA DE CONFIGURACIÓN previa a la activación (R7, CU-12, RN-16): configura el
    /// adaptador simulado para que un servidor falle por un chequeo BLOQUEANTE (permisos faltantes)
    /// y otro pase con solo una ADVERTENCIA de jerarquía. El servicio activa únicamente el que no
    /// tiene bloqueantes (RN-16); el otro queda inactivo.
    /// </summary>
    private async Task CorrerPruebaConfiguracionAsync(
        IServiceProvider sp, AdaptadorGatewaySimulado simulado, CancellationToken ct)
    {
        var servicioPrueba = sp.GetRequiredService<ServicioPruebaConfiguracion>();

        // Servidor que BLOQUEA: permisos faltantes (chequeo bloqueante, CU-12 CA-02).
        simulado.ConfigurarPruebaConfiguracion(
            new Snowflake(ServidorPruebaBloquea),
            new ResultadoPruebaConfiguracion(new[]
            {
                ChequeoConfiguracion.Superado(ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida"),
                ChequeoConfiguracion.Bloqueante(
                    ResultadoPruebaConfiguracion.CodigoPermisosFaltantes, "Permisos requeridos presentes",
                    "Falta el permiso de banear miembros."),
            }));

        // Servidor que ACTIVA: todo OK salvo una ADVERTENCIA de jerarquía (no bloquea, CU-12 CA-04).
        simulado.ConfigurarPruebaConfiguracion(
            new Snowflake(ServidorPruebaActiva),
            new ResultadoPruebaConfiguracion(new[]
            {
                ChequeoConfiguracion.Superado(ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida"),
                ChequeoConfiguracion.Superado(
                    ResultadoPruebaConfiguracion.CodigoPermisosFaltantes, "Permisos requeridos presentes"),
                ChequeoConfiguracion.Advertencia(
                    ResultadoPruebaConfiguracion.CodigoJerarquiaInsuficiente, "Jerarquía de roles suficiente",
                    "Hay 1 rol por encima del bot."),
            }));

        var bloquea = await servicioPrueba.ProbarYActivarAsync(new Snowflake(ServidorPruebaBloquea), ct);
        _logger.LogInformation(
            "[WALKING SKELETON] Prueba del servidor {Servidor}: {Bloqueantes} bloqueante(s); activado={Activado} " +
            "(se espera NO activado por chequeo bloqueante, RN-16).",
            ServidorPruebaBloquea, bloquea.Prueba?.Bloqueantes.Count, bloquea.Activado);

        var activa = await servicioPrueba.ProbarYActivarAsync(new Snowflake(ServidorPruebaActiva), ct);
        _logger.LogInformation(
            "[WALKING SKELETON] Prueba del servidor {Servidor}: {Advertencias} advertencia(s), 0 bloqueante(s); " +
            "activado={Activado} (se espera ACTIVADO; la advertencia de jerarquía no bloquea, CU-12 CA-04).",
            ServidorPruebaActiva, activa.Prueba?.Advertencias.Count, activa.Activado);
    }

    /// <summary>
    /// Inyecta una ráfaga distribuida (mismo usuario en 3 canales distintos dentro de la ventana
    /// de detección, CU-01) suscribiendo el motor al adaptador simulado y desuscribiéndolo al
    /// terminar. Reutilizado por los escenarios de R6.
    /// </summary>
    private static async Task InyectarRafagaUsuarioAsync(
        IServiceProvider sp,
        AdaptadorGatewaySimulado simulado,
        MotorDeModeracion motor,
        string servidorId,
        string usuarioId,
        long baseMensaje,
        CancellationToken ct)
    {
        Func<MensajeEntrante, Task> handler = mensaje => motor.ProcesarAsync(mensaje, ct);
        simulado.MensajeRecibido += handler;
        try
        {
            var ahora = sp.GetRequiredService<IReloj>().Ahora;
            string[] canales = { "300000000000000001", "300000000000000002", "300000000000000003" };
            for (var i = 0; i < canales.Length; i++)
            {
                var mensaje = new MensajeEntrante(
                    new Snowflake(servidorId),
                    new Snowflake(canales[i]),
                    new Snowflake(usuarioId),
                    new Snowflake((baseMensaje + i).ToString()),
                    ahora.AddMilliseconds(i * 300),
                    $"mensaje de ráfaga {i + 1}");
                await simulado.InyectarMensajeAsync(mensaje);
            }
        }
        finally
        {
            simulado.MensajeRecibido -= handler;
        }
    }

    /// <summary>
    /// Construye un motor con un estado de conducta y un estado de antirrebote frescos por
    /// escenario y las políticas dadas, resolviendo del contenedor el resto de las dependencias
    /// del pipeline (incluido el evaluador y el repositorio de exenciones de R5). Centraliza la
    /// construcción del motor usada por los escenarios del walking skeleton. Cada escenario usa
    /// estado fresco para que el antirrebote (R6) no arrastre marcas entre escenarios.
    /// </summary>
    private static MotorDeModeracion CrearMotor(
        IServiceProvider sp,
        AdaptadorGatewaySimulado simulado,
        IReadOnlyList<Politica> politicas,
        EstadoAntirreboteEnMemoria? antirrebote = null)
        => new(
            new EstadoConductaEnMemoria(),
            antirrebote ?? new EstadoAntirreboteEnMemoria(),
            sp.GetRequiredService<EvaluadorRafagaDistribuida>(),
            sp.GetRequiredService<EvaluadorReglaContenido>(),
            sp.GetRequiredService<EvaluadorExenciones>(),
            politicas,
            simulado,
            sp.GetRequiredService<IRepositorioIncidentes>(),
            sp.GetRequiredService<IRepositorioServidores>(),
            sp.GetRequiredService<IRepositorioExenciones>(),
            sp.GetRequiredService<IReloj>(),
            sp.GetRequiredService<ILogger<MotorDeModeracion>>());
}
