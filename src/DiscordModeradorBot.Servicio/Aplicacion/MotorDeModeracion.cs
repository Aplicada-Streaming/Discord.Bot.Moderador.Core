using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>
/// Motor de moderación: orquesta el pipeline de evaluación de un mensaje entrante hasta
/// el incidente (flujo-ejecucion). En R1 implementaba el camino feliz de detección y el
/// camino de simulación (RN-09). R2 agrega el camino de EJECUCIÓN real: tras tomar la copia
/// de mensajes (RN-11), ejecuta las acciones de la política EN ORDEN (RN-05) contra el
/// adaptador — reportar al canal privado (CU-05) y banear con borrado retroactivo acotado a
/// 7 días (CU-02/CU-03, RN-02) — y persiste el incidente como ejecutado. R3 agrega el segundo
/// eje de defensa: las reglas de CONTENIDO sin estado (CU-04) se evalúan en la etapa 2, ANTES
/// de actualizar el estado de conducta (flujo-ejecucion); una política de contenido que coincide
/// reutiliza el mismo camino de acciones de R2 (reportar + banear). El camino de simulación
/// queda intacto: registra <see cref="ResultadoModeracion.Simulada"/> sin invocar acción (RN-09).
/// </summary>
public sealed class MotorDeModeracion
{
    private readonly EstadoConductaEnMemoria _estadoConducta;
    private readonly EvaluadorRafagaDistribuida _evaluador;
    private readonly EvaluadorReglaContenido _evaluadorContenido;
    private readonly IReadOnlyList<Politica> _politicas;
    private readonly IAdaptadorGateway _adaptador;
    private readonly IRepositorioIncidentes _repositorioIncidentes;
    private readonly IRepositorioServidores _repositorioServidores;
    private readonly IReloj _reloj;
    private readonly ILogger<MotorDeModeracion> _logger;

    public MotorDeModeracion(
        EstadoConductaEnMemoria estadoConducta,
        EvaluadorRafagaDistribuida evaluador,
        EvaluadorReglaContenido evaluadorContenido,
        IReadOnlyList<Politica> politicas,
        IAdaptadorGateway adaptador,
        IRepositorioIncidentes repositorioIncidentes,
        IRepositorioServidores repositorioServidores,
        IReloj reloj,
        ILogger<MotorDeModeracion> logger)
    {
        _estadoConducta = estadoConducta;
        _evaluador = evaluador;
        _evaluadorContenido = evaluadorContenido;
        // Las políticas se evalúan por prioridad ascendente, primera coincidencia (RN-04).
        _politicas = politicas.OrderBy(p => p.Prioridad).ToList();
        _adaptador = adaptador;
        _repositorioIncidentes = repositorioIncidentes;
        _repositorioServidores = repositorioServidores;
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

        // Etapa 2 — Reglas de CONTENIDO (sin estado), antes de tocar el estado de conducta
        // (flujo-ejecucion etapa 2, CU-04). Se evalúa una vez por regla de contenido y el
        // resultado se reutiliza en la etapa 4; así el eje de contenido es un predicado aislado
        // del mensaje, independiente de la actividad acumulada del usuario.
        var coincidenciasContenido = EvaluarReglasContenido(mensaje);

        // Etapa 3 — Actualización del estado de conducta en memoria (ADR-09).
        _estadoConducta.RegistrarActividad(mensaje);

        // La ventana deslizante se evalúa respecto del instante del mensaje recién recibido
        // (que es "ahora" en operación); el reloj inyectado se usa para sellar el incidente.
        var instanteEvaluacion = mensaje.Instante;

        // Etapa 4 — Evaluación de políticas por prioridad, primera coincidencia (RN-04). Una
        // política de contenido (CU-04) usa la coincidencia de su regla de la etapa 2; una de
        // conducta usa el evaluador de ráfaga (CU-01). En ambos ejes el camino de acciones es el
        // mismo de R2 (reportar + banear), parametrizado por la política (CU-04 nota §10.2).
        Incidente? ultimoIncidente = null;
        foreach (var politica in _politicas)
        {
            bool coincide = politica.EsDeContenido
                ? coincidenciasContenido.TryGetValue(politica, out var c) && c
                : _evaluador.Evaluar(mensaje, _estadoConducta, instanteEvaluacion).Coincide;

            if (!coincide)
            {
                continue;
            }

            ultimoIncidente = await AplicarPoliticaAsync(politica, mensaje, _reloj.Ahora, ct);

            // Primera coincidencia detiene la evaluación salvo bandera continuar (RN-04).
            if (!politica.Continuar)
            {
                return ultimoIncidente;
            }
        }

        return ultimoIncidente;
    }

    /// <summary>
    /// Evalúa las reglas de contenido de las políticas de contenido sobre el texto del mensaje
    /// (etapa 2, CU-04). Predicado SIN estado: no observa la actividad acumulada. La evaluación
    /// corre con tope de tiempo (ADR-08): si una regla excede el tope, NO propaga excepción; se
    /// trata como no coincidencia y se registra la regla problemática
    /// (CU-04 CONTENIDO_EVALUACION_EXCEDE_TIEMPO). El patrón inválido no llega aquí: se rechaza al
    /// configurar (RN-03), de modo que toda regla presente ya compiló.
    /// </summary>
    private Dictionary<Politica, bool> EvaluarReglasContenido(MensajeEntrante mensaje)
    {
        var coincidencias = new Dictionary<Politica, bool>();

        foreach (var politica in _politicas)
        {
            if (politica.ReglaContenido is not { } regla)
            {
                continue;
            }

            var resultado = _evaluadorContenido.Evaluar(mensaje, regla);

            if (resultado.ExcedioTope)
            {
                // ADR-08: la evaluación se abortó por el tope de tiempo; no se cuelga el pipeline,
                // se registra la condición y se trata como no coincidencia.
                _logger.LogWarning(
                    "Política de contenido '{Politica}': la evaluación de la regla '{Regla}' excedió " +
                    "el tope de tiempo (CONTENIDO_EVALUACION_EXCEDE_TIEMPO); se omite la regla y " +
                    "continúa el procesamiento (ADR-08).",
                    politica.Nombre, regla.Nombre);
            }

            coincidencias[politica] = resultado.Coincide;
        }

        return coincidencias;
    }

    private async Task<Incidente> AplicarPoliticaAsync(
        Politica politica, MensajeEntrante mensaje, DateTimeOffset ahora, CancellationToken ct)
    {
        // Etapa 5 — Copia de mensajes y canales afectados, ANTES de cualquier remoción
        // (RN-11, RN-05). Esta copia es la única evidencia que sobrevive al borrado.
        var copia = new[]
        {
            new MensajeAccionado(mensaje.MensajeId, mensaje.CanalId, mensaje.Contenido),
        };
        var canalesAfectados = new[] { mensaje.CanalId };

        // Acciones de la política en su orden de ejecución declarado (RN-05).
        var accionesOrdenadas = politica.Acciones.OrderBy(a => a.OrdenEjecucion).ToList();

        // Acción representativa del incidente: la de contención (baneo) si existe; si no, la
        // primera acción declarada. El modelo de Incidente registra una acción resultante
        // (modelo-datos-logico §2.11); el detalle por acción vive en la configuración.
        var accionRepresentativa = accionesOrdenadas
            .FirstOrDefault(a => a.Tipo == TipoAccion.BaneoConBorradoRetroactivo || a.Tipo == TipoAccion.Banear)
            ?? accionesOrdenadas[0];

        // Etapa 6 — Decisión de modo (RN-09). El resultado del incidente se determina por el
        // modo de la política y se sella en la copia construida en la etapa 5.
        var incidente = new Incidente(
            mensaje.ServidorId,
            mensaje.UsuarioId,
            politica.Nombre,
            politica.Modo,
            accionRepresentativa.Tipo,
            politica.Modo == Modo.Ejecucion ? ResultadoModeracion.Ejecutada : ResultadoModeracion.Simulada,
            copia,
            canalesAfectados,
            ahora);

        if (politica.Modo == Modo.Ejecucion)
        {
            // Etapa 8 — Ejecución de las acciones en orden contra el adaptador (RN-05).
            await EjecutarAccionesAsync(politica, incidente, accionesOrdenadas, ct);
        }
        else
        {
            // Modo simulación: NO se invoca ninguna acción real (RN-09). Solo se registra.
            var ventanaBorrado = accionRepresentativa.VentanaBorradoEfectivaDias;
            _logger.LogInformation(
                "Política '{Politica}' SIMULADA: se habrían ejecutado {Cantidad} acción(es) " +
                "({Acciones}) sobre usuario {Usuario} en servidor {Servidor}; el baneo habría " +
                "purgado {Dias} día(s). Ninguna acción real ejecutada.",
                politica.Nombre, accionesOrdenadas.Count,
                string.Join(", ", accionesOrdenadas.Select(a => a.Tipo)),
                mensaje.UsuarioId.Valor, mensaje.ServidorId.Valor, ventanaBorrado);
        }

        // Etapa 9 — Registro del incidente (RN-11).
        await _repositorioIncidentes.AgregarAsync(incidente, ct);

        return incidente;
    }

    /// <summary>
    /// Ejecuta las acciones de la política en el orden configurado (RN-05). La copia de los
    /// mensajes ya fue tomada antes (RN-11), por lo que el reporte y el incidente conservan
    /// la evidencia aunque el baneo borre los mensajes a continuación.
    /// </summary>
    private async Task EjecutarAccionesAsync(
        Politica politica, Incidente incidente, IReadOnlyList<Accion> accionesOrdenadas, CancellationToken ct)
    {
        CanalDeSalida? canalSalida = null;

        foreach (var accion in accionesOrdenadas)
        {
            switch (accion.Tipo)
            {
                case TipoAccion.ReportarACanalPrivado or TipoAccion.Reportar:
                    canalSalida ??= await ResolverCanalSalidaAsync(incidente.ServidorId, ct);
                    if (canalSalida is null)
                    {
                        // CU-05 CA-03 / REPORTE_CANAL_NO_DESIGNADO: sin canal designado no se
                        // envía el reporte; el incidente igual se conserva (RN-11).
                        _logger.LogWarning(
                            "Política '{Politica}': no hay canal de salida designado en el servidor " +
                            "{Servidor}; el reporte no se envió (REPORTE_CANAL_NO_DESIGNADO), el " +
                            "incidente se conserva.",
                            politica.Nombre, incidente.ServidorId.Valor);
                        break;
                    }

                    var reporte = ReporteIncidente.DesdeIncidente(incidente);
                    await _adaptador.ReportarAsync(canalSalida, reporte, ct);

                    _logger.LogInformation(
                        "Política '{Politica}' EJECUTADA: reporte publicado en canal {Canal} " +
                        "({Proposito}) con {Mensajes} mensaje(s) y {Canales} canal(es) afectado(s).",
                        politica.Nombre, canalSalida.SnowflakeCanal.Valor, canalSalida.PropositoLogico,
                        incidente.MensajesAccionados.Count, incidente.CanalesAfectados.Count);
                    break;

                case TipoAccion.BaneoConBorradoRetroactivo or TipoAccion.Banear:
                    var ventanaBorrado = TimeSpan.FromDays(accion.VentanaBorradoEfectivaDias);
                    await _adaptador.BanearConBorradoAsync(
                        incidente.ServidorId, incidente.UsuarioId, ventanaBorrado, ct);

                    _logger.LogInformation(
                        "Política '{Politica}' EJECUTADA: baneo con borrado retroactivo de {Dias} " +
                        "día(s) sobre usuario {Usuario} en servidor {Servidor}.",
                        politica.Nombre, accion.VentanaBorradoEfectivaDias,
                        incidente.UsuarioId.Valor, incidente.ServidorId.Valor);
                    break;

                default:
                    // Resto del catálogo (timeout, expulsar, roles): R6.
                    _logger.LogInformation(
                        "Política '{Politica}': acción {Accion} aún no soportada en R2; omitida.",
                        politica.Nombre, accion.Tipo);
                    break;
            }
        }
    }

    private async Task<CanalDeSalida?> ResolverCanalSalidaAsync(Snowflake servidorId, CancellationToken ct)
    {
        var servidor = await _repositorioServidores.ObtenerAsync(servidorId, ct);
        return servidor?.CanalDeSalida;
    }

    /// <summary>
    /// Punto de extensión del descarte de exentos (RN-07). R1 no tiene exenciones; se
    /// completa en R5 (CU-15).
    /// </summary>
    private static bool EsExento(MensajeEntrante mensaje) => false;
}
