using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>
/// Motor de moderación: orquesta el pipeline de evaluación de un mensaje entrante hasta
/// el incidente (flujo-ejecucion). En R1 implementa la versión mínima del camino feliz:
/// descarte de exentos (punto de extensión, sin exenciones todavía, RN-07), reglas de
/// contenido (ninguna en R1), actualización del estado de conducta (etapa 3), evaluación
/// de la política de ráfaga por prioridad con primera coincidencia (RN-04), toma de
/// copia de mensajes y construcción del incidente (RN-11), decisión de modo (RN-09) y
/// persistencia (RN-11).
/// </summary>
public sealed class MotorDeModeracion
{
    private readonly EstadoConductaEnMemoria _estadoConducta;
    private readonly EvaluadorRafagaDistribuida _evaluador;
    private readonly IReadOnlyList<Politica> _politicas;
    private readonly IAdaptadorGateway _adaptador;
    private readonly IRepositorioIncidentes _repositorioIncidentes;
    private readonly IReloj _reloj;
    private readonly ILogger<MotorDeModeracion> _logger;

    public MotorDeModeracion(
        EstadoConductaEnMemoria estadoConducta,
        EvaluadorRafagaDistribuida evaluador,
        IReadOnlyList<Politica> politicas,
        IAdaptadorGateway adaptador,
        IRepositorioIncidentes repositorioIncidentes,
        IReloj reloj,
        ILogger<MotorDeModeracion> logger)
    {
        _estadoConducta = estadoConducta;
        _evaluador = evaluador;
        // Las políticas se evalúan por prioridad ascendente, primera coincidencia (RN-04).
        _politicas = politicas.OrderBy(p => p.Prioridad).ToList();
        _adaptador = adaptador;
        _repositorioIncidentes = repositorioIncidentes;
        _reloj = reloj;
        _logger = logger;
    }

    /// <summary>
    /// Procesa un mensaje entrante por el pipeline. Devuelve el incidente generado, o
    /// null si ninguna política coincidió.
    /// </summary>
    public async Task<Incidente?> ProcesarAsync(MensajeEntrante mensaje, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(mensaje);

        // Etapa 1 — Descarte de exentos. R1: sin exenciones; punto de extensión (RN-07, R5).
        if (EsExento(mensaje))
        {
            return null;
        }

        // Etapa 2 — Reglas de contenido (sin estado). R1: ninguna (R3).

        // Etapa 3 — Actualización del estado de conducta en memoria (ADR-09).
        _estadoConducta.RegistrarActividad(mensaje);

        // La ventana deslizante se evalúa respecto del instante del mensaje recién recibido
        // (que es "ahora" en operación); el reloj inyectado se usa para sellar el incidente.
        var instanteEvaluacion = mensaje.Instante;

        // Etapa 4 — Evaluación de políticas por prioridad, primera coincidencia (RN-04).
        foreach (var politica in _politicas)
        {
            var resultado = _evaluador.Evaluar(mensaje, _estadoConducta, instanteEvaluacion);
            if (!resultado.Coincide)
            {
                continue;
            }

            var incidente = await AplicarPoliticaAsync(politica, mensaje, _reloj.Ahora, ct);

            // Primera coincidencia detiene la evaluación salvo bandera continuar (RN-04).
            // En R1 hay una sola política, pero se respeta el contrato del pipeline.
            if (!politica.Continuar)
            {
                return incidente;
            }
        }

        return null;
    }

    private async Task<Incidente> AplicarPoliticaAsync(
        Politica politica, MensajeEntrante mensaje, DateTimeOffset ahora, CancellationToken ct)
    {
        // Etapa 5 — Copia de mensajes y canales afectados, antes de cualquier remoción (RN-11).
        var copia = new[]
        {
            new MensajeAccionado(mensaje.MensajeId, mensaje.CanalId, mensaje.Contenido),
        };
        var canalesAfectados = new[] { mensaje.CanalId };

        var accion = politica.Acciones[0];

        // Etapa 6 — Decisión de modo (RN-09).
        ResultadoModeracion resultadoModeracion;
        if (politica.Modo == Modo.Ejecucion)
        {
            // Etapa 8 — Ejecución de la acción contra el adaptador (CU-02/CU-03).
            var ventanaBorrado = TimeSpan.FromDays(accion.VentanaBorradoEfectivaDias);
            await _adaptador.BanearConBorradoAsync(
                mensaje.ServidorId, mensaje.UsuarioId, ventanaBorrado, ct);
            resultadoModeracion = ResultadoModeracion.Ejecutada;

            _logger.LogInformation(
                "Política '{Politica}' EJECUTADA: baneo con borrado retroactivo de {Dias} día(s) " +
                "sobre usuario {Usuario} en servidor {Servidor}.",
                politica.Nombre, accion.VentanaBorradoEfectivaDias,
                mensaje.UsuarioId.Valor, mensaje.ServidorId.Valor);
        }
        else
        {
            // Modo simulación: NO se invoca la acción real (RN-09). Se reporta lo que se haría.
            resultadoModeracion = ResultadoModeracion.Simulada;

            _logger.LogInformation(
                "Política '{Politica}' SIMULADA: se habría ejecutado un baneo con borrado " +
                "retroactivo de {Dias} día(s) sobre usuario {Usuario} en servidor {Servidor}. " +
                "Ninguna acción real ejecutada.",
                politica.Nombre, accion.VentanaBorradoEfectivaDias,
                mensaje.UsuarioId.Valor, mensaje.ServidorId.Valor);
        }

        var incidente = new Incidente(
            mensaje.ServidorId,
            mensaje.UsuarioId,
            politica.Nombre,
            politica.Modo,
            accion.Tipo,
            resultadoModeracion,
            copia,
            canalesAfectados,
            ahora);

        // Etapa 9 — Registro del incidente (RN-11).
        await _repositorioIncidentes.AgregarAsync(incidente, ct);

        return incidente;
    }

    /// <summary>
    /// Punto de extensión del descarte de exentos (RN-07). R1 no tiene exenciones; se
    /// completa en R5 (CU-15).
    /// </summary>
    private static bool EsExento(MensajeEntrante mensaje) => false;
}
