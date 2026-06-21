using System.Collections.Concurrent;

namespace DiscordModeradorBot.Servicio.Dominio.Conducta;

/// <summary>
/// Estado de conducta en memoria (ADR-09): ventanas deslizantes de actividad reciente
/// por par (servidor, usuario), particionadas por contexto. No se persiste; se pierde
/// ante un reinicio y se reconstruye con el tráfico. Mantiene, por usuario, la lista de
/// (canal, instante) de su actividad reciente, podando lo que cae fuera de la ventana.
/// </summary>
public sealed class EstadoConductaEnMemoria
{
    private readonly ConcurrentDictionary<ClaveUsuario, List<Actividad>> _actividadPorUsuario = new();
    private readonly object _candado = new();

    private readonly record struct ClaveUsuario(string ServidorId, string UsuarioId);

    private readonly record struct Actividad(string CanalId, DateTimeOffset Instante);

    /// <summary>
    /// Registra la actividad de un mensaje en la ventana deslizante de su emisor
    /// (flujo-ejecucion etapa 3). El instante proviene del mensaje, para que la
    /// evaluación sea determinista con un reloj inyectado.
    /// </summary>
    public void RegistrarActividad(MensajeEntrante mensaje)
    {
        ArgumentNullException.ThrowIfNull(mensaje);

        var clave = new ClaveUsuario(mensaje.ServidorId.Valor, mensaje.UsuarioId.Valor);
        var actividad = new Actividad(mensaje.CanalId.Valor, mensaje.Instante);

        lock (_candado)
        {
            var lista = _actividadPorUsuario.GetOrAdd(clave, _ => new List<Actividad>());
            lista.Add(actividad);
        }
    }

    /// <summary>
    /// Cuenta los canales distintos en los que el usuario publicó dentro de la ventana
    /// [ahora - ventana, ahora] (CU-01 pasos 4-5). El discriminador es la cantidad de
    /// canales distintos, no el volumen de mensajes. Al consultar, poda la actividad
    /// vencida para acotar la memoria (ADR-09).
    /// </summary>
    public int CanalesDistintosEnVentana(
        Snowflake servidorId, Snowflake usuarioId, DateTimeOffset ahora, TimeSpan ventana)
    {
        var clave = new ClaveUsuario(servidorId.Valor, usuarioId.Valor);
        var limiteInferior = ahora - ventana;

        lock (_candado)
        {
            if (!_actividadPorUsuario.TryGetValue(clave, out var lista))
            {
                return 0;
            }

            // Poda de la actividad vencida (fuera de la ventana hacia atrás).
            lista.RemoveAll(a => a.Instante < limiteInferior);

            if (lista.Count == 0)
            {
                _actividadPorUsuario.TryRemove(clave, out _);
                return 0;
            }

            var canalesDistintos = new HashSet<string>(StringComparer.Ordinal);
            foreach (var actividad in lista)
            {
                if (actividad.Instante >= limiteInferior && actividad.Instante <= ahora)
                {
                    canalesDistintos.Add(actividad.CanalId);
                }
            }

            return canalesDistintos.Count;
        }
    }
}
