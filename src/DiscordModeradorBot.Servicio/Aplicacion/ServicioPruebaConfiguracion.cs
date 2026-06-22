using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>Resultado de la operación de prueba y activación (CU-12, RN-16).</summary>
public sealed record ResultadoProbarYActivar(
    bool ServidorEncontrado,
    ResultadoPruebaConfiguracion? Prueba,
    bool Activado)
{
    /// <summary>El servidor no existe (no se pudo probar).</summary>
    public static ResultadoProbarYActivar NoEncontrado() => new(false, null, false);
}

/// <summary>Resultado del envío de un mensaje de prueba al canal de reportes (CU-05).</summary>
public sealed record ResultadoEnvioPrueba(bool Exito, string Mensaje)
{
    public static ResultadoEnvioPrueba Ok(string mensaje) => new(true, mensaje);

    public static ResultadoEnvioPrueba Falla(string mensaje) => new(false, mensaje);
}

/// <summary>
/// Servicio de aplicación de la prueba de configuración de un servidor (CU-12, RN-16, RC-08).
/// Orquesta: obtiene el servidor registrado, DESCIFRA su token en memoria (RN-14, nunca en claro
/// persistido ni logueado), invoca el puerto del adaptador para probar token, intents, permisos,
/// recepción de eventos, canal de salida y jerarquía de roles contra la plataforma, y, según el
/// resultado, ACTIVA el servidor solo si no hay chequeos bloqueantes (RN-16). Si la prueba
/// detecta un token inválido, además marca el servidor como desconectado (CU-12 CA-03, CU-13).
///
/// La dependencia es unidireccional hacia el Dominio: la prueba estructural vive en el adaptador
/// (infraestructura), detrás del puerto <see cref="IAdaptadorGateway"/>; este servicio solo decide
/// la transición de estado a partir del resultado.
/// </summary>
public sealed class ServicioPruebaConfiguracion
{
    private readonly IRepositorioServidores _repositorio;
    private readonly IAdaptadorGateway _adaptador;
    private readonly IServicioCifrado _cifrado;
    private readonly ILogger<ServicioPruebaConfiguracion> _logger;
    private readonly GestorConexionesGateway? _gestorConexiones;

    public ServicioPruebaConfiguracion(
        IRepositorioServidores repositorio,
        IAdaptadorGateway adaptador,
        IServicioCifrado cifrado,
        ILogger<ServicioPruebaConfiguracion> logger,
        GestorConexionesGateway? gestorConexiones = null)
    {
        _repositorio = repositorio;
        _adaptador = adaptador;
        _cifrado = cifrado;
        _logger = logger;
        // Solo presente en modo gateway Discord (ADR-13): al activar un servidor se abre su
        // conexión real (CU-13). En modo simulado es null y la activación no abre red.
        _gestorConexiones = gestorConexiones;
    }

    /// <summary>
    /// Solo PRUEBA la configuración sin activar (para previsualizar los chequeos en el panel,
    /// CU-12 paso 3-7). No cambia el estado salvo marcar desconectado ante token inválido.
    /// </summary>
    public async Task<ResultadoProbarYActivar> ProbarAsync(Snowflake servidorId, CancellationToken ct = default)
        => await EjecutarAsync(servidorId, activarSiSupera: false, ct);

    /// <summary>
    /// Prueba la configuración y, si NO hay chequeos bloqueantes, activa el servidor (CU-12 paso 8,
    /// RN-16). Si hay bloqueantes, el servidor queda sin activar; un token inválido lo marca
    /// desconectado (CU-12 CA-03).
    /// </summary>
    public async Task<ResultadoProbarYActivar> ProbarYActivarAsync(
        Snowflake servidorId, CancellationToken ct = default)
        => await EjecutarAsync(servidorId, activarSiSupera: true, ct);

    /// <summary>
    /// Envía un mensaje de prueba al canal de reportes del servidor (CU-05) para verificar que el
    /// bot puede publicar allí. Requiere un canal designado; descifra el token en memoria (RN-14) y
    /// delega en el adaptador. En modo Discord publica de verdad; en Simulado lo registra en el log.
    /// </summary>
    public async Task<ResultadoEnvioPrueba> EnviarMensajePruebaAsync(
        Snowflake servidorId, CancellationToken ct = default)
    {
        var servidor = await _repositorio.ObtenerAsync(servidorId, ct);
        if (servidor is null)
        {
            return ResultadoEnvioPrueba.Falla("El servidor no está registrado.");
        }

        if (servidor.CanalDeSalida is null)
        {
            return ResultadoEnvioPrueba.Falla(
                "El servidor no tiene canal de reportes designado; editalo y agregá el ID del canal.");
        }

        var tokenEnClaro = _cifrado.Descifrar(servidor.TokenCifrado);
        var solicitud = new SolicitudPruebaConfiguracion(servidorId, tokenEnClaro, servidor.CanalDeSalida);
        const string texto =
            "✅ Mensaje de prueba de DiscordModeradorBot: el canal de reportes está configurado y el bot puede publicar acá.";

        var resultado = await _adaptador.EnviarMensajePruebaAsync(solicitud, texto, ct);
        var canal = servidor.CanalDeSalida.SnowflakeCanal.Valor;

        return resultado switch
        {
            ResultadoAccion.Ejecutada => ResultadoEnvioPrueba.Ok(
                $"Mensaje de prueba enviado al canal {canal} (en modo Simulado queda registrado en el log)."),
            ResultadoAccion.NoAccionablePorPermisos => ResultadoEnvioPrueba.Falla(
                "El bot no tiene permiso para escribir en el canal de reportes."),
            _ => ResultadoEnvioPrueba.Falla(
                "No se pudo enviar el mensaje: revisá el token y que el canal exista y sea de texto."),
        };
    }

    private async Task<ResultadoProbarYActivar> EjecutarAsync(
        Snowflake servidorId, bool activarSiSupera, CancellationToken ct)
    {
        var servidor = await _repositorio.ObtenerAsync(servidorId, ct);
        if (servidor is null)
        {
            _logger.LogWarning(
                "Prueba de configuración: el servidor {Servidor} no está registrado (CU-12).",
                servidorId.Valor);
            return ResultadoProbarYActivar.NoEncontrado();
        }

        // El token se descifra SOLO en memoria para la prueba; nunca se loguea ni se expone (RN-14).
        var tokenEnClaro = _cifrado.Descifrar(servidor.TokenCifrado);
        var solicitud = new SolicitudPruebaConfiguracion(servidorId, tokenEnClaro, servidor.CanalDeSalida);

        var prueba = await _adaptador.ProbarConfiguracionAsync(solicitud, ct);

        // Estado de conexión derivado: un token inválido deja el servidor desconectado (CU-12 CA-03,
        // CU-13); en otro caso, una prueba que llega a contactar la plataforma lo deja conectado.
        var estadoConexion = prueba.TokenInvalido ? EstadoConexion.Desconectado : EstadoConexion.Conectado;

        // RN-16/RC-08: el servidor solo se activa si no hay chequeos bloqueantes.
        var activar = activarSiSupera && prueba.PuedeActivar;
        var estadoActivacion = activar
            ? EstadoActivacion.Activo
            : servidor.EstadoActivacion;

        await _repositorio.ActualizarEstadoAsync(servidorId, estadoActivacion, estadoConexion, ct);

        // En modo gateway Discord, activar el servidor abre su conexión real al canal de eventos
        // (CU-13, ADR-13). En modo simulado el gestor es null y no se abre red.
        if (activar && _gestorConexiones is not null)
        {
            await _gestorConexiones.ActivarServidorAsync(servidorId, ct);
        }

        if (activarSiSupera)
        {
            _logger.LogInformation(
                "Prueba de configuración del servidor {Servidor}: {Bloqueantes} chequeo(s) " +
                "bloqueante(s), {Advertencias} advertencia(s). {Decision} (RN-16).",
                servidorId.Valor, prueba.Bloqueantes.Count, prueba.Advertencias.Count,
                activar ? "Servidor ACTIVADO" : "Activación BLOQUEADA");
        }

        return new ResultadoProbarYActivar(true, prueba, activar);
    }
}
