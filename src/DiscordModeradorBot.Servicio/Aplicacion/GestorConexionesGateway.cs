using System.Collections.Concurrent;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>
/// Gestor de conexiones de gateway por servidor (ADR-13, CU-13). Coordina el ciclo de vida de las
/// conexiones reales: al activar un servidor abre su conexión autenticada con su token DESCIFRADO
/// en memoria (RN-14), enruta cada mensaje entrante al motor de moderación del contexto y refleja
/// el estado de conexión por servidor (CU-13). Al desactivar/desregistrar un servidor cierra su
/// conexión. La reconexión automática la maneja el SDK; este gestor solo arranca, detiene y observa
/// el estado.
///
/// Es lógica de APLICACIÓN, independiente del SDK: la conexión por servidor está detrás del puerto
/// <see cref="IFabricaClienteGateway"/>/<see cref="IClienteGatewayServidor"/> y el ruteo del mensaje
/// se delega en un manejador inyectado (<see cref="EnrutadorMensaje"/>), de modo que el gestor es
/// testeable con dobles (NSubstitute) sin red ni token. El token nunca se loguea (RN-14).
/// </summary>
public sealed class GestorConexionesGateway : IAsyncDisposable
{
    // El gestor es SINGLETON (mantiene conexiones de larga vida); el repositorio de servidores es
    // SCOPED (depende del DbContext). Para no capturar un scoped en un singleton (dependencia
    // cautiva), se resuelve el repositorio dentro de un scope efímero en cada uso.
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServicioCifrado _cifrado;
    private readonly IFabricaClienteGateway _fabrica;
    private readonly IEstadoConexionGateway _estadoConexion;
    private readonly ILogger<GestorConexionesGateway> _logger;
    private readonly ConcurrentDictionary<string, IClienteGatewayServidor> _conexiones = new();

    /// <summary>
    /// Manejador que enruta un mensaje entrante de un servidor a su pipeline de moderación. Se
    /// inyecta para que el gestor no dependa de la construcción del <c>MotorDeModeracion</c>
    /// (que es scoped y necesita un alcance por contexto). El host lo cablea al motor; los tests lo
    /// reemplazan por un doble para verificar el ruteo.
    /// </summary>
    public Func<Snowflake, MensajeEntrante, CancellationToken, Task>? EnrutadorMensaje { get; set; }

    public GestorConexionesGateway(
        IServiceScopeFactory scopeFactory,
        IServicioCifrado cifrado,
        IFabricaClienteGateway fabrica,
        IEstadoConexionGateway estadoConexion,
        ILogger<GestorConexionesGateway> logger)
    {
        _scopeFactory = scopeFactory;
        _cifrado = cifrado;
        _fabrica = fabrica;
        _estadoConexion = estadoConexion;
        _logger = logger;
    }

    /// <summary>
    /// Carga los servidores ACTIVOS al iniciar y abre la conexión de cada uno (CU-13 precondición:
    /// un servidor activo mantiene su conexión). Los inactivos no se conectan (RN-16).
    /// </summary>
    public async Task IniciarAsync(CancellationToken ct = default)
    {
        IReadOnlyList<ServidorRegistrado> servidores;
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IRepositorioServidores>();
            servidores = await repositorio.ListarAsync(ct);
        }

        foreach (var servidor in servidores.Where(s => s.EstadoActivacion == EstadoActivacion.Activo))
        {
            await ConectarServidorAsync(servidor, ct);
        }

        _logger.LogInformation(
            "[GATEWAY] Gestor de conexiones iniciado: {Conexiones} conexión(es) activa(s) de " +
            "{Total} servidor(es) registrado(s) (ADR-13, CU-13).",
            _conexiones.Count, servidores.Count);
    }

    /// <summary>
    /// Activa la conexión de un servidor (al activarlo desde el panel tras superar la prueba, CU-12,
    /// CU-13). Si ya está conectado, no hace nada. Descifra su token en memoria (RN-14), crea la
    /// conexión, cablea el ruteo de mensajes y el estado, y conecta.
    /// </summary>
    public async Task ActivarServidorAsync(Snowflake servidorId, CancellationToken ct = default)
    {
        if (_conexiones.ContainsKey(servidorId.Valor))
        {
            return;
        }

        ServidorRegistrado? servidor;
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IRepositorioServidores>();
            servidor = await repositorio.ObtenerAsync(servidorId, ct);
        }

        if (servidor is null)
        {
            _logger.LogWarning(
                "[GATEWAY] No se puede conectar el servidor {Servidor}: no está registrado (CU-13).",
                servidorId.Valor);
            return;
        }

        await ConectarServidorAsync(servidor, ct);
    }

    /// <summary>
    /// Desactiva la conexión de un servidor (al desactivarlo/desregistrarlo, CU-13): cierra la
    /// conexión y marca el estado como detenido.
    /// </summary>
    public async Task DesactivarServidorAsync(Snowflake servidorId, CancellationToken ct = default)
    {
        if (!_conexiones.TryRemove(servidorId.Valor, out var cliente))
        {
            return;
        }

        await cliente.DetenerAsync(ct);
        await cliente.DisposeAsync();
        _estadoConexion.Actualizar(servidorId, EstadoConexion.Desconectado, MotivoEstadoConexion.Detenido);

        _logger.LogInformation(
            "[GATEWAY] Servidor {Servidor} desconectado a propósito (desactivado, CU-13).",
            servidorId.Valor);
    }

    private async Task ConectarServidorAsync(ServidorRegistrado servidor, CancellationToken ct)
    {
        var servidorId = servidor.SnowflakeServidor;

        // El token se descifra SOLO en memoria para autenticar la conexión; nunca se loguea (RN-14).
        var token = _cifrado.Descifrar(servidor.TokenCifrado);

        var cliente = _fabrica.Crear(servidorId);
        cliente.MensajeRecibido += mensaje => EnrutarAsync(servidorId, mensaje, ct);
        cliente.EstadoConexionCambiado += (estado, motivo) =>
            _estadoConexion.Actualizar(servidorId, estado, motivo);

        if (!_conexiones.TryAdd(servidorId.Valor, cliente))
        {
            await cliente.DisposeAsync();
            return;
        }

        _estadoConexion.Actualizar(servidorId, EstadoConexion.Desconectado, MotivoEstadoConexion.Conectando);

        try
        {
            await cliente.ConectarAsync(token, ct);
            _logger.LogInformation(
                "[GATEWAY] Conexión iniciada para el servidor {Servidor} (ADR-13, CU-13).",
                servidorId.Valor);
        }
        catch (Exception ex)
        {
            // Un fallo al conectar no debe tumbar el gestor (ADR-08); se deja el servidor
            // desconectado y se quita la conexión fallida para permitir un reintento posterior.
            _logger.LogWarning(
                ex,
                "[GATEWAY] No se pudo iniciar la conexión del servidor {Servidor}; queda desconectado " +
                "(CU-13).",
                servidorId.Valor);
            _conexiones.TryRemove(servidorId.Valor, out _);
            await cliente.DisposeAsync();
            _estadoConexion.Actualizar(
                servidorId, EstadoConexion.Desconectado, MotivoEstadoConexion.DesconectadoTransitorio);
        }
    }

    private async Task EnrutarAsync(Snowflake servidorId, MensajeEntrante mensaje, CancellationToken ct)
    {
        var enrutador = EnrutadorMensaje;
        if (enrutador is null)
        {
            return;
        }

        try
        {
            await enrutador(servidorId, mensaje, ct);
        }
        catch (Exception ex)
        {
            // Una falla al procesar un mensaje no debe cerrar la conexión ni tumbar el gestor
            // (ADR-08): se registra y se continúa con el siguiente mensaje.
            _logger.LogError(
                ex,
                "[GATEWAY] Error al enrutar un mensaje del servidor {Servidor} al motor; se descarta " +
                "el mensaje y continúa la conexión (ADR-08).",
                servidorId.Valor);
        }
    }

    /// <summary>Conexiones activas en este momento (para diagnóstico/tests).</summary>
    public IReadOnlyCollection<Snowflake> ServidoresConectados =>
        _conexiones.Keys.Select(k => new Snowflake(k)).ToList();

    public async ValueTask DisposeAsync()
    {
        foreach (var cliente in _conexiones.Values)
        {
            await cliente.DisposeAsync();
        }

        _conexiones.Clear();
    }
}
