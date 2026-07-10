using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
using Microsoft.Extensions.Logging;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary> 
/// Carga las políticas de un servidor desde la configuración persistida del panel (CU-11): toma los
/// eventos, sus grupos, las reglas de cada grupo (de contenido por id, o de conducta por clave) y
/// las acciones, y los materializa al modelo de dominio que evalúa el <see cref="MotorDeModeracion"/>
/// (<see cref="Politica"/> + <see cref="ComposicionPolitica"/> + <see cref="GrupoDeReglas"/>). Es el
/// puente que hace que lo configurado en el panel DIRIJA la moderación, en lugar de una lista fija.
///
/// Tolerante a configuración incompleta (RN-15, RC-04, ADR-08): un grupo sin reglas válidas se omite,
/// un evento que se queda sin grupos válidos se omite, y cualquier error al materializar un evento se
/// registra y se saltea sin tumbar el resto. Así un dato a medio configurar nunca tira el pipeline.
/// </summary>
public sealed class CargadorPoliticasDesdeConfiguracion : ICargadorPoliticas
{
    private readonly IRepositorioConfiguracion _configuracion;
    private readonly IRepositorioReglasContenido _reglasContenido;
    private readonly EvaluadorReglaContenido _evaluadorContenido;
    private readonly EvaluadorRafagaDistribuida _evaluadorRafaga;
    private readonly ILogger<CargadorPoliticasDesdeConfiguracion> _logger;

    public CargadorPoliticasDesdeConfiguracion(
        IRepositorioConfiguracion configuracion,
        IRepositorioReglasContenido reglasContenido,
        EvaluadorReglaContenido evaluadorContenido,
        EvaluadorRafagaDistribuida evaluadorRafaga,
        ILogger<CargadorPoliticasDesdeConfiguracion> logger)
    {
        _configuracion = configuracion;
        _reglasContenido = reglasContenido;
        _evaluadorContenido = evaluadorContenido;
        _evaluadorRafaga = evaluadorRafaga;
        _logger = logger;
    }

    public async Task<ParametrosModeracion?> ObtenerParametrosAsync(
        Snowflake servidorId, CancellationToken ct = default)
        => await _configuracion.ObtenerParametrosAsync(servidorId, ct);

    public async Task<IReadOnlyList<Politica>> CargarAsync(Snowflake servidorId, CancellationToken ct = default)
    {
        var eventos = await _configuracion.ListarEventosAsync(servidorId, ct);
        if (eventos.Count == 0)
        {
            return Array.Empty<Politica>();
        }

        var grupos = (await _configuracion.ListarGruposAsync(servidorId, ct))
            .ToDictionary(g => g.Id);

        // Las reglas de contenido se compilan con su tope de tiempo (ADR-08) y se indexan por id
        // para resolver las referencias de los grupos (ReglaDeGrupo.ReglaContenidoId).
        var reglasContenido = (await _reglasContenido.ListarPorServidorAsync(
                servidorId, ServicioRegistroReglaContenido.TopeTiempoEvaluacion, ct))
            .ToDictionary(r => r.Id);

        // El umbral y la ventana de detección por servidor (CU-01, RN-10) se embeben en cada regla
        // de conducta: así una ventana configurada en el panel realmente cambia la detección.
        var parametros = await _configuracion.ObtenerParametrosAsync(servidorId, ct);

        var politicas = new List<Politica>(eventos.Count);
        foreach (var evento in eventos)
        {
            try
            {
                var politica = MaterializarEvento(evento, grupos, reglasContenido, parametros, servidorId);
                if (politica is not null)
                {
                    politicas.Add(politica);
                }
            }
            catch (Exception ex)
            {
                // Configuración inconsistente de un evento puntual: se registra y se omite, sin
                // afectar al resto de las políticas del servidor (ADR-08).
                _logger.LogWarning(
                    ex,
                    "No se pudo materializar el evento '{Evento}' (id {EventoId}) del servidor {Servidor}; " +
                    "se omite y se continúa con los demás (CU-11, ADR-08).",
                    evento.Nombre, evento.Id, servidorId.Valor);
            }
        }

        return politicas;
    }

    private Politica? MaterializarEvento(
        EventoPersistido evento,
        IReadOnlyDictionary<int, GrupoPersistido> grupos,
        IReadOnlyDictionary<int, ReglaContenidoPersistida> reglasContenido,
        ParametrosModeracion parametros,
        Snowflake servidorId)
    {
        var gruposDominio = new List<GrupoDeReglas>(evento.GruposIds.Count);
        foreach (var grupoId in evento.GruposIds)
        {
            if (!grupos.TryGetValue(grupoId, out var grupo))
            {
                _logger.LogWarning(
                    "Evento '{Evento}' del servidor {Servidor} referencia un grupo inexistente " +
                    "(id {GrupoId}); se omite el grupo (CU-11).",
                    evento.Nombre, servidorId.Valor, grupoId);
                continue;
            }

            var grupoDominio = MaterializarGrupo(grupo, reglasContenido, parametros, servidorId);
            if (grupoDominio is not null)
            {
                gruposDominio.Add(grupoDominio);
            }
        }

        if (gruposDominio.Count == 0)
        {
            // Un evento sin grupos materializables no tiene condición de disparo: se omite (RC-04).
            _logger.LogWarning(
                "Evento '{Evento}' del servidor {Servidor} no tiene grupos con reglas válidas; " +
                "se omite la política (CU-11, RC-04).",
                evento.Nombre, servidorId.Valor);
            return null;
        }

        var modoCombinacion = Enum.TryParse<ModoCombinacionGrupos>(
            evento.ModoCombinacionGrupos, ignoreCase: true, out var mc)
            ? mc
            : ModoCombinacionGrupos.Todos;

        var composicion = new ComposicionPolitica(gruposDominio, modoCombinacion);

        var modo = string.Equals(evento.Modo, "ejecucion", StringComparison.OrdinalIgnoreCase)
            ? Modo.Ejecucion
            : Modo.Simulacion;

        var acciones = evento.Acciones
            .Select(MaterializarAccion)
            .OrderBy(a => a.OrdenEjecucion)
            .ToList();

        return new Politica(
            nombre: evento.Nombre,
            prioridad: evento.Prioridad,
            modo: modo,
            continuar: evento.Continuar,
            acciones: acciones.Count > 0 ? acciones : null,
            composicion: composicion);
    }

    private GrupoDeReglas? MaterializarGrupo(
        GrupoPersistido grupo,
        IReadOnlyDictionary<int, ReglaContenidoPersistida> reglasContenido,
        ParametrosModeracion parametros,
        Snowflake servidorId)
    {
        var evaluables = new List<IReglaEvaluable>(grupo.Reglas.Count);
        foreach (var regla in grupo.Reglas)
        {
            if (string.Equals(regla.ClaseRegla, "contenido", StringComparison.OrdinalIgnoreCase))
            {
                if (regla.ReglaContenidoId is { } reglaId
                    && reglasContenido.TryGetValue(reglaId, out var rc))
                {
                    evaluables.Add(new ReglaEvaluableContenido(rc.Regla, _evaluadorContenido));
                }
                else
                {
                    _logger.LogWarning(
                        "Grupo '{Grupo}' del servidor {Servidor} referencia una regla de contenido " +
                        "inexistente (id {ReglaId}); se omite la regla (CU-11).",
                        grupo.Nombre, servidorId.Valor, regla.ReglaContenidoId);
                }
            }
            else if (string.Equals(regla.ClaseRegla, "conducta", StringComparison.OrdinalIgnoreCase))
            {
                // La ráfaga usa el umbral y la ventana CONFIGURADOS del servidor (CU-01, RN-10): así
                // ensanchar la ventana en el panel cambia de verdad cuándo se dispara la detección.
                evaluables.Add(new ReglaEvaluableConducta(
                    _evaluadorRafaga,
                    nombre: regla.ClaveReglaConducta ?? "Ráfaga distribuida",
                    umbralConfigurado: parametros.UmbralCanalesDistintos,
                    ventanaSegundosConfigurada: parametros.VentanaDeteccionSegundos));
            }
        }

        if (evaluables.Count == 0)
        {
            // Grupo sin reglas materializables: GrupoDeReglas lo rechazaría (RC-04). Se omite acá
            // para no propagar la excepción y poder evaluar los demás grupos del evento.
            _logger.LogWarning(
                "Grupo '{Grupo}' del servidor {Servidor} no tiene reglas válidas; se omite (CU-11, RC-04).",
                grupo.Nombre, servidorId.Valor);
            return null;
        }

        var modoCoincidencia = Enum.TryParse<ModoCoincidencia>(
            grupo.ModoCoincidencia, ignoreCase: true, out var modo)
            ? modo
            : ModoCoincidencia.Alguna;

        return new GrupoDeReglas(grupo.Nombre, modoCoincidencia, evaluables, grupo.MinimoCoincidencias);
    }

    private static Accion MaterializarAccion(AccionPersistida accion)
    {
        var tipo = Enum.TryParse<TipoAccion>(accion.Tipo, ignoreCase: true, out var t)
            ? t
            : TipoAccion.ReportarACanalPrivado;

        var duracion = accion.DuracionTimeoutMinutos is { } minutos
            ? TimeSpan.FromMinutes(minutos)
            : (TimeSpan?)null;

        Snowflake? rol = Snowflake.TryParse(accion.RolObjetivo, out var s) ? s : null;

        return new Accion(
            tipo,
            OrdenEjecucion: accion.OrdenEjecucion,
            VentanaBorradoDias: accion.VentanaBorradoDias ?? 1,
            DuracionTimeout: duracion,
            RolObjetivo: rol);
    }
}
