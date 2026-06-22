using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
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
/// R5 implementa la ETAPA 1 "descarte de exentos" que hasta ahora era un punto de extensión
/// vacío: si el emisor, alguno de sus roles o el canal del mensaje coinciden con una exención del
/// servidor, el mensaje se descarta de inmediato — ANTES de evaluar contenido o conducta, sin
/// tocar el estado de conducta, sin disparar política y sin registrar incidente ni acción (CU-15,
/// RN-07).
///
/// R6 cierra el pipeline de ejecución: (a) materializa las acciones adicionales del catálogo
/// (timeout, expulsión, asignar/quitar rol) reutilizando el camino de R2 y respetando el orden
/// declarado (RN-05, intake §4); (b) agrega la ETAPA 7 "antirrebote por usuario": una coincidencia
/// sobre un usuario ya accionado dentro de la ventana de antirrebote se SUPRIME (0 acciones
/// adicionales), sin reinvocar al adaptador (CU-16, RN-06, ADR-09); (c) deja de tratar las acciones
/// del adaptador como fire-and-forget: mapea su <see cref="ResultadoAccion"/> al
/// <see cref="ResultadoModeracion"/> del incidente y, ante una jerarquía superior o permisos
/// faltantes, NO se cae: registra el incidente como NoAccionable, igual reporta al canal de
/// incidencias y continúa (RN-01, ADR-08).
/// </summary>
public sealed class MotorDeModeracion
{
    private readonly EstadoConductaEnMemoria _estadoConducta;
    private readonly EstadoAntirreboteEnMemoria _estadoAntirrebote;
    private readonly EvaluadorRafagaDistribuida _evaluador;
    private readonly EvaluadorReglaContenido _evaluadorContenido;
    private readonly EvaluadorExenciones _evaluadorExenciones;
    private readonly ICargadorPoliticas _cargador;
    private readonly IAdaptadorGateway _adaptador;
    private readonly IRepositorioIncidentes _repositorioIncidentes;
    private readonly IRepositorioServidores _repositorioServidores;
    private readonly IRepositorioExenciones _repositorioExenciones;
    private readonly IReloj _reloj;
    private readonly TimeSpan _ventanaAntirrebote;
    private readonly ILogger<MotorDeModeracion> _logger;

    /// <summary>
    /// Constructor principal: las políticas a evaluar se cargan en cada mensaje desde el
    /// <paramref name="cargador"/>, según el servidor del mensaje (CU-11). En producción el cargador
    /// lee la configuración persistida del panel, de modo que lo configurado DIRIGE la moderación.
    /// </summary>
    public MotorDeModeracion(
        EstadoConductaEnMemoria estadoConducta,
        EstadoAntirreboteEnMemoria estadoAntirrebote,
        EvaluadorRafagaDistribuida evaluador,
        EvaluadorReglaContenido evaluadorContenido,
        EvaluadorExenciones evaluadorExenciones,
        ICargadorPoliticas cargador,
        IAdaptadorGateway adaptador,
        IRepositorioIncidentes repositorioIncidentes,
        IRepositorioServidores repositorioServidores,
        IRepositorioExenciones repositorioExenciones,
        IReloj reloj,
        ILogger<MotorDeModeracion> logger,
        TimeSpan? ventanaAntirrebote = null)
    {
        _estadoConducta = estadoConducta;
        _estadoAntirrebote = estadoAntirrebote;
        _evaluador = evaluador;
        _evaluadorContenido = evaluadorContenido;
        _evaluadorExenciones = evaluadorExenciones;
        _cargador = cargador;
        _adaptador = adaptador;
        _repositorioIncidentes = repositorioIncidentes;
        _repositorioServidores = repositorioServidores;
        _repositorioExenciones = repositorioExenciones;
        _reloj = reloj;
        // Ventana de antirrebote (CU-16, RN-06): si no se inyecta, se toma el default del
        // descriptor único, normalizando un valor fuera de rango al default (ADR-12, RN-10,
        // ANTIRREBOTE_VENTANA_INVALIDA, CU-16 CA-03).
        _ventanaAntirrebote = TimeSpan.FromSeconds(
            RegistroDescriptores.VentanaAntirreboteSegundos.NormalizarOPorDefecto(
                ventanaAntirrebote?.TotalSeconds));
        _logger = logger;
    }

    /// <summary>
    /// Constructor de compatibilidad con una lista FIJA de políticas (las mismas para todo servidor).
    /// Lo usan las pruebas del motor y el walking skeleton, que construyen sus políticas en código;
    /// internamente envuelve la lista en un <see cref="CargadorPoliticasFijas"/>.
    /// </summary>
    public MotorDeModeracion(
        EstadoConductaEnMemoria estadoConducta,
        EstadoAntirreboteEnMemoria estadoAntirrebote,
        EvaluadorRafagaDistribuida evaluador,
        EvaluadorReglaContenido evaluadorContenido,
        EvaluadorExenciones evaluadorExenciones,
        IReadOnlyList<Politica> politicas,
        IAdaptadorGateway adaptador,
        IRepositorioIncidentes repositorioIncidentes,
        IRepositorioServidores repositorioServidores,
        IRepositorioExenciones repositorioExenciones,
        IReloj reloj,
        ILogger<MotorDeModeracion> logger,
        TimeSpan? ventanaAntirrebote = null)
        : this(
            estadoConducta,
            estadoAntirrebote,
            evaluador,
            evaluadorContenido,
            evaluadorExenciones,
            new CargadorPoliticasFijas(politicas),
            adaptador,
            repositorioIncidentes,
            repositorioServidores,
            repositorioExenciones,
            reloj,
            logger,
            ventanaAntirrebote)
    {
    }

    /// <summary>
    /// Procesa un mensaje entrante por el pipeline. Devuelve el incidente generado, o
    /// null si ninguna política coincidió.
    /// </summary>
    public async Task<Incidente?> ProcesarAsync(MensajeEntrante mensaje, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(mensaje);

        // Etapa 1 — Descarte de exentos, ANTES de todo lo demás (flujo-ejecucion etapa 1, RN-07,
        // CU-15). Si el emisor, alguno de sus roles o el canal del mensaje están exentos, el
        // pipeline termina YA: no se evalúan reglas de contenido ni de conducta, NO se actualiza
        // el estado de conducta con este mensaje (no contribuye a una ráfaga posterior), no se
        // dispara política y no se registra incidente ni acción.
        if (await EsExentoAsync(mensaje, ct))
        {
            _logger.LogInformation(
                "Etapa 1 (descarte de exentos): mensaje {Mensaje} del usuario {Usuario} en el canal " +
                "{Canal} del servidor {Servidor} se descarta por una exención vigente; no se evalúa " +
                "ninguna regla (RN-07, CU-15).",
                mensaje.MensajeId.Valor, mensaje.UsuarioId.Valor, mensaje.CanalId.Valor,
                mensaje.ServidorId.Valor);
            return null;
        }

        // Las políticas del servidor del mensaje se cargan en este punto (CU-11) y se evalúan por
        // prioridad ascendente, primera coincidencia (RN-04). El cargador es la fuente de verdad:
        // en producción son las configuradas en el panel; en pruebas/skeleton, una lista fija. Sin
        // políticas para el servidor, el pipeline termina sin incidente (no hay nada que evaluar).
        var politicas = (await _cargador.CargarAsync(mensaje.ServidorId, ct))
            .OrderBy(p => p.Prioridad)
            .ToList();
        if (politicas.Count == 0)
        {
            return null;
        }

        // Ventana de antirrebote EFECTIVA del servidor (CU-16, RN-06, RN-10): si la fuente aporta
        // parámetros por servidor (configuración del panel) se usa el configurado; si no (lista fija
        // de pruebas/skeleton), se conserva el valor del motor (constructor/ default).
        var parametros = await _cargador.ObtenerParametrosAsync(mensaje.ServidorId, ct);
        var ventanaAntirrebote = parametros is { } p
            ? TimeSpan.FromSeconds(
                RegistroDescriptores.VentanaAntirreboteSegundos.NormalizarOPorDefecto(p.VentanaAntirreboteSegundos))
            : _ventanaAntirrebote;

        // Etapa 2 — Reglas de CONTENIDO (sin estado), antes de tocar el estado de conducta
        // (flujo-ejecucion etapa 2, CU-04). Se evalúa una vez por regla de contenido y el
        // resultado se reutiliza en la etapa 4; así el eje de contenido es un predicado aislado
        // del mensaje, independiente de la actividad acumulada del usuario.
        var coincidenciasContenido = EvaluarReglasContenido(mensaje, politicas);

        // Etapa 3 — Actualización del estado de conducta en memoria (ADR-09).
        _estadoConducta.RegistrarActividad(mensaje);

        // La ventana deslizante se evalúa respecto del instante del mensaje recién recibido
        // (que es "ahora" en operación); el reloj inyectado se usa para sellar el incidente.
        var instanteEvaluacion = mensaje.Instante;

        // Etapa 4 — Evaluación de políticas por prioridad, primera coincidencia (RN-04). La
        // condición de disparo de cada política se resuelve según su forma (R7):
        //  - con COMPOSICIÓN de grupos (RN-15): se evalúa la combinación booleana de grupos, donde
        //    cada grupo combina sus reglas (de contenido y/o conducta) según su modo de coincidencia;
        //  - de CONTENIDO directo (CU-04): usa la coincidencia de su regla de la etapa 2;
        //  - de CONDUCTA directo (CU-01): usa el evaluador de ráfaga.
        // En todos los casos el camino de acciones es el mismo de R2 (reportar + banear),
        // parametrizado por la política (CU-04 nota §10.2).
        var contextoReglas = new ContextoEvaluacionRegla(mensaje, _estadoConducta, instanteEvaluacion);
        Incidente? ultimoIncidente = null;
        foreach (var politica in politicas)
        {
            bool coincide;
            if (politica.TieneComposicion)
            {
                coincide = politica.Composicion!.Evaluar(contextoReglas);
            }
            else if (politica.EsDeContenido)
            {
                coincide = coincidenciasContenido.TryGetValue(politica, out var c) && c;
            }
            else
            {
                coincide = _evaluador.Evaluar(mensaje, _estadoConducta, instanteEvaluacion).Coincide;
            }

            if (!coincide)
            {
                continue;
            }

            ultimoIncidente = await AplicarPoliticaAsync(politica, mensaje, _reloj.Ahora, ventanaAntirrebote, ct);

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
    private Dictionary<Politica, bool> EvaluarReglasContenido(
        MensajeEntrante mensaje, IReadOnlyList<Politica> politicas)
    {
        var coincidencias = new Dictionary<Politica, bool>();

        foreach (var politica in politicas)
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

    private async Task<Incidente?> AplicarPoliticaAsync(
        Politica politica, MensajeEntrante mensaje, DateTimeOffset ahora,
        TimeSpan ventanaAntirrebote, CancellationToken ct)
    {
        // Acciones de la política en su orden de ejecución declarado (RN-05).
        var accionesOrdenadas = politica.Acciones.OrderBy(a => a.OrdenEjecucion).ToList();

        // Acción representativa del incidente: la primera de CONTENCIÓN sobre el usuario (baneo,
        // timeout, expulsión, rol) si existe; si no, la primera acción declarada. El modelo de
        // Incidente registra una acción resultante (modelo-datos-logico §2.11); el detalle por
        // acción vive en la configuración.
        var accionRepresentativa = accionesOrdenadas.FirstOrDefault(EsContencionSobreUsuario)
            ?? accionesOrdenadas[0];

        // Etapa 6 — Decisión de modo (RN-09). En simulación NO se invoca ninguna acción real ni
        // se toca el antirrebote (es la primera barrera, antes de la etapa 7): solo se registra
        // lo que se habría hecho.
        if (politica.Modo == Modo.Simulacion)
        {
            var ventanaBorrado = accionRepresentativa.VentanaBorradoEfectivaDias;
            _logger.LogInformation(
                "Política '{Politica}' SIMULADA: se habrían ejecutado {Cantidad} acción(es) " +
                "({Acciones}) sobre usuario {Usuario} en servidor {Servidor}; el baneo habría " +
                "purgado {Dias} día(s). Ninguna acción real ejecutada.",
                politica.Nombre, accionesOrdenadas.Count,
                string.Join(", ", accionesOrdenadas.Select(a => a.Tipo)),
                mensaje.UsuarioId.Valor, mensaje.ServidorId.Valor, ventanaBorrado);

            return await RegistrarIncidenteAsync(
                politica, mensaje, accionRepresentativa, ResultadoModeracion.Simulada, ahora, ct);
        }

        // Etapa 7 — Antirrebote por usuario (CU-16, RN-06, ADR-09). Si ya se accionó sobre el
        // mismo usuario dentro de la ventana vigente, la acción repetida se SUPRIME: no se vuelve
        // a invocar al adaptador (0 acciones adicionales) y no se genera un incidente nuevo. Solo
        // se observa la supresión (ADR-08). La primera acción del ataque sí pasa (no hay marca).
        if (_estadoAntirrebote.DebeSuprimir(
                mensaje.ServidorId, mensaje.UsuarioId, ahora, ventanaAntirrebote))
        {
            _logger.LogInformation(
                "Política '{Politica}': acción sobre el usuario {Usuario} en servidor {Servidor} " +
                "SUPRIMIDA por antirrebote (ya accionado dentro de la ventana de {Ventana}s, RN-06, " +
                "CU-16); 0 acciones adicionales, sin nuevo incidente.",
                politica.Nombre, mensaje.UsuarioId.Valor, mensaje.ServidorId.Valor,
                ventanaAntirrebote.TotalSeconds);
            return null;
        }

        // Etapa 5 — Copia de mensajes y canales afectados, ANTES de cualquier remoción
        // (RN-11, RN-05). Esta copia es la única evidencia que sobrevive al borrado.
        var copia = new[]
        {
            new MensajeAccionado(mensaje.MensajeId, mensaje.CanalId, mensaje.Contenido),
        };
        var canalesAfectados = new[] { mensaje.CanalId };

        var incidente = new Incidente(
            mensaje.ServidorId,
            mensaje.UsuarioId,
            politica.Nombre,
            politica.Modo,
            accionRepresentativa.Tipo,
            ResultadoModeracion.Ejecutada,
            copia,
            canalesAfectados,
            ahora);

        // Etapa 8 — Ejecución de las acciones en orden contra el adaptador (RN-05). El resultado
        // del incidente se reclasifica según el resultado de las acciones de contención (RN-01).
        await EjecutarAccionesAsync(politica, incidente, accionesOrdenadas, ct);

        // El usuario quedó accionado en la ráfaga vigente: se marca para suprimir repeticiones
        // dentro de la ventana (CU-16 §4 paso 3, RN-06). Se marca incluso si la contención no fue
        // accionable por jerarquía/permisos: ya se reportó y no tiene sentido reintentar el mismo
        // caso imposible en cada mensaje de la ráfaga (ADR-08, evita ruido).
        _estadoAntirrebote.RegistrarAccion(mensaje.ServidorId, mensaje.UsuarioId, ahora);

        // Etapa 9 — Registro del incidente (RN-11).
        await _repositorioIncidentes.AgregarAsync(incidente, ct);

        return incidente;
    }

    /// <summary>Registra un incidente simulado (RN-09) sin invocar acciones reales.</summary>
    private async Task<Incidente> RegistrarIncidenteAsync(
        Politica politica,
        MensajeEntrante mensaje,
        Accion accionRepresentativa,
        ResultadoModeracion resultado,
        DateTimeOffset ahora,
        CancellationToken ct)
    {
        var copia = new[]
        {
            new MensajeAccionado(mensaje.MensajeId, mensaje.CanalId, mensaje.Contenido),
        };

        var incidente = new Incidente(
            mensaje.ServidorId,
            mensaje.UsuarioId,
            politica.Nombre,
            politica.Modo,
            accionRepresentativa.Tipo,
            resultado,
            copia,
            new[] { mensaje.CanalId },
            ahora);

        await _repositorioIncidentes.AgregarAsync(incidente, ct);
        return incidente;
    }

    /// <summary>
    /// Una acción de contención se aplica sobre el usuario (baneo, timeout, expulsión, gestión de
    /// roles) y por lo tanto puede quedar no accionable por jerarquía o permisos (RN-01). El
    /// reporte a un canal privado no es contención: nunca queda no accionable por jerarquía.
    /// </summary>
    private static bool EsContencionSobreUsuario(Accion accion) => accion.Tipo is
        TipoAccion.Banear or TipoAccion.BaneoConBorradoRetroactivo or TipoAccion.Timeout or
        TipoAccion.Expulsar or TipoAccion.AsignarRol or TipoAccion.QuitarRol;

    /// <summary>
    /// Ejecuta las acciones de la política en el orden configurado (RN-05). La copia de los
    /// mensajes ya fue tomada antes (RN-11), por lo que el reporte y el incidente conservan la
    /// evidencia aunque el baneo borre los mensajes a continuación. El reporte se publica primero
    /// cuando va primero (orden típico reportar→banear, RN-05) y la contención sobre el usuario a
    /// continuación; cada acción de contención devuelve un <see cref="ResultadoAccion"/> que se
    /// mapea al resultado del incidente. Si una acción no fue accionable por jerarquía/permisos,
    /// el incidente se RECLASIFICA a NoAccionable y el pipeline NO se cae: igual se reportó y se
    /// registra (RN-01, ADR-08). El reporte incluye la advertencia de no accionable cuando el
    /// incidente ya quedó clasificado (CU-02 §7, TC-60) — la advertencia se materializa en el
    /// texto del reporte (ReporteIncidente) compuesto desde el incidente.
    /// </summary>
    private async Task EjecutarAccionesAsync(
        Politica politica, Incidente incidente, IReadOnlyList<Accion> accionesOrdenadas, CancellationToken ct)
    {
        // Resultado agregado de las acciones de contención: el "peor" caso manda (no accionable o
        // fallida priman sobre ejecutada) para clasificar el incidente (RN-01, ADR-08).
        var resultadoAgregado = ResultadoModeracion.Ejecutada;

        foreach (var accion in accionesOrdenadas)
        {
            if (accion.Tipo is TipoAccion.ReportarACanalPrivado or TipoAccion.Reportar)
            {
                await ReportarAsync(politica, incidente, ct);
                continue;
            }

            var resultadoAccion = await EjecutarAccionContencionAsync(politica, incidente, accion, ct);
            resultadoAgregado = PeorResultado(resultadoAgregado, resultadoAccion.AResultadoModeracion());

            // Reclasifica en cuanto se conoce el resultado de la contención (RN-01), de modo que
            // un reporte declarado DESPUÉS de la contención ya incluya la advertencia (TC-60).
            incidente.ReclasificarResultado(resultadoAgregado);
        }
    }

    /// <summary>
    /// Publica el reporte del incidente en el canal de salida privado (CU-05). Si el incidente
    /// quedó no accionable, el reporte se publica igualmente con la advertencia (RN-01, CU-02 §7,
    /// TC-60). Sin canal designado, no se envía y el incidente igual se conserva (REPORTE_CANAL_NO_DESIGNADO).
    /// </summary>
    private async Task ReportarAsync(Politica politica, Incidente incidente, CancellationToken ct)
    {
        var canalSalida = await ResolverCanalSalidaAsync(incidente.ServidorId, ct);
        if (canalSalida is null)
        {
            _logger.LogWarning(
                "Política '{Politica}': no hay canal de salida designado en el servidor {Servidor}; " +
                "el reporte no se envió (REPORTE_CANAL_NO_DESIGNADO), el incidente se conserva.",
                politica.Nombre, incidente.ServidorId.Valor);
            return;
        }

        var reporte = ReporteIncidente.DesdeIncidente(incidente);
        await _adaptador.ReportarAsync(canalSalida, reporte, ct);

        _logger.LogInformation(
            "Política '{Politica}' EJECUTADA: reporte publicado en canal {Canal} ({Proposito}) con " +
            "{Mensajes} mensaje(s) y {Canales} canal(es) afectado(s); resultado {Resultado}.",
            politica.Nombre, canalSalida.SnowflakeCanal.Valor, canalSalida.PropositoLogico,
            incidente.MensajesAccionados.Count, incidente.CanalesAfectados.Count, incidente.Resultado);
    }

    /// <summary>
    /// Ejecuta una acción de contención sobre el usuario contra el adaptador y devuelve su
    /// resultado (RN-01, ADR-08). Una jerarquía superior o permisos faltantes NO lanzan
    /// excepción: el adaptador devuelve un resultado no accionable que el motor mapea.
    /// </summary>
    private async Task<ResultadoAccion> EjecutarAccionContencionAsync(
        Politica politica, Incidente incidente, Accion accion, CancellationToken ct)
    {
        var servidor = incidente.ServidorId;
        var usuario = incidente.UsuarioId;

        switch (accion.Tipo)
        {
            case TipoAccion.BaneoConBorradoRetroactivo or TipoAccion.Banear:
                var ventanaBorrado = TimeSpan.FromDays(accion.VentanaBorradoEfectivaDias);
                var resBaneo = await _adaptador.BanearConBorradoAsync(servidor, usuario, ventanaBorrado, ct);
                RegistrarResultadoContencion(
                    politica, incidente, accion.Tipo, resBaneo,
                    $"baneo con borrado retroactivo de {accion.VentanaBorradoEfectivaDias} día(s)");
                return resBaneo;

            case TipoAccion.Timeout:
                var duracion = accion.DuracionTimeoutEfectiva;
                var resTimeout = await _adaptador.AplicarTimeoutAsync(servidor, usuario, duracion, ct);
                RegistrarResultadoContencion(
                    politica, incidente, accion.Tipo, resTimeout, $"timeout de {duracion.TotalMinutes} minuto(s)");
                return resTimeout;

            case TipoAccion.Expulsar:
                var resExpulsion = await _adaptador.ExpulsarAsync(servidor, usuario, ct);
                RegistrarResultadoContencion(politica, incidente, accion.Tipo, resExpulsion, "expulsión");
                return resExpulsion;

            case TipoAccion.AsignarRol or TipoAccion.QuitarRol:
                if (accion.RolObjetivo is not { } rol)
                {
                    _logger.LogWarning(
                        "Política '{Politica}': la acción {Accion} no declara un rol objetivo; se " +
                        "omite (configuración incompleta).",
                        politica.Nombre, accion.Tipo);
                    return ResultadoAccion.Fallida;
                }

                var resRol = accion.Tipo == TipoAccion.AsignarRol
                    ? await _adaptador.AsignarRolAsync(servidor, usuario, rol, ct)
                    : await _adaptador.QuitarRolAsync(servidor, usuario, rol, ct);
                RegistrarResultadoContencion(
                    politica, incidente, accion.Tipo, resRol,
                    $"{(accion.Tipo == TipoAccion.AsignarRol ? "asignar" : "quitar")} rol {rol.Valor}");
                return resRol;

            default:
                // Tipos no de contención que llegasen aquí (desbaneo se ejecuta desde el panel,
                // no en el pipeline). No alteran el resultado del incidente.
                _logger.LogInformation(
                    "Política '{Politica}': acción {Accion} no se ejecuta en el pipeline; omitida.",
                    politica.Nombre, accion.Tipo);
                return ResultadoAccion.Ejecutada;
        }
    }

    /// <summary>Loguea el resultado de una acción de contención de forma observable (RN-01, ADR-08).</summary>
    private void RegistrarResultadoContencion(
        Politica politica, Incidente incidente, TipoAccion tipo, ResultadoAccion resultado, string descripcion)
    {
        if (resultado == ResultadoAccion.Ejecutada)
        {
            _logger.LogInformation(
                "Política '{Politica}' EJECUTADA: {Descripcion} sobre usuario {Usuario} en servidor " +
                "{Servidor} ({Accion}).",
                politica.Nombre, descripcion, incidente.UsuarioId.Valor, incidente.ServidorId.Valor, tipo);
            return;
        }

        // Jerarquía superior / permisos faltantes / fallo de plataforma: NO se cae el pipeline; se
        // registra y el incidente quedará clasificado y reportado (RN-01, ADR-08, CU-02 §7).
        _logger.LogWarning(
            "Política '{Politica}': {Descripcion} sobre usuario {Usuario} en servidor {Servidor} " +
            "NO se ejecutó ({Accion}, resultado {Resultado}); el incidente se registra y reporta " +
            "como no accionable, el pipeline continúa (RN-01, ADR-08).",
            politica.Nombre, descripcion, incidente.UsuarioId.Valor, incidente.ServidorId.Valor,
            tipo, resultado);
    }

    /// <summary>
    /// Combina dos resultados de incidente quedándose con el más severo: no accionable y fallida
    /// priman sobre ejecutada, para que el incidente refleje la limitación (RN-01, ADR-08).
    /// </summary>
    private static ResultadoModeracion PeorResultado(ResultadoModeracion a, ResultadoModeracion b)
    {
        static int Severidad(ResultadoModeracion r) => r switch
        {
            ResultadoModeracion.NoAccionable => 3,
            ResultadoModeracion.Fallida => 2,
            ResultadoModeracion.Ejecutada => 1,
            _ => 0,
        };

        return Severidad(b) > Severidad(a) ? b : a;
    }

    private async Task<CanalDeSalida?> ResolverCanalSalidaAsync(Snowflake servidorId, CancellationToken ct)
    {
        var servidor = await _repositorioServidores.ObtenerAsync(servidorId, ct);
        return servidor?.CanalDeSalida;
    }

    /// <summary>
    /// Descarte de exentos del pipeline (etapa 1, RN-07, CU-15). Consulta las exenciones del
    /// servidor del mensaje y delega la decisión en el evaluador de exenciones: el sujeto queda
    /// exento por usuario emisor, por alguno de sus roles o por el canal del mensaje. Sin
    /// exenciones para el servidor, nada queda exento y el pipeline continúa normal (regresión).
    /// </summary>
    private async Task<bool> EsExentoAsync(MensajeEntrante mensaje, CancellationToken ct)
    {
        var exenciones = await _repositorioExenciones.ListarPorServidorAsync(mensaje.ServidorId, ct);
        return _evaluadorExenciones.EstaExento(mensaje, exenciones);
    }
}
