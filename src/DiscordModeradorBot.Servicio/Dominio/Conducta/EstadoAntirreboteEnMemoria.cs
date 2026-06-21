using System.Collections.Concurrent;

namespace DiscordModeradorBot.Servicio.Dominio.Conducta;

/// <summary>
/// Estado de antirrebote por usuario en memoria (CU-16, RN-06, ADR-09, flujo-ejecucion
/// etapa 7). Recuerda, por par (servidor, usuario), el instante de la última acción de
/// moderación aplicada, para suprimir acciones repetidas sobre el mismo usuario dentro de la
/// ventana de antirrebote vigente. La primera acción del ataque se ejecuta y se marca; las
/// coincidencias adicionales dentro de la ventana se suprimen (0 acciones adicionales,
/// RN-06). Al expirar la ventana, una nueva coincidencia vuelve a ser accionable (CU-16
/// CA-02). El estado NO se persiste: se pierde ante un reinicio y se reconstruye con el
/// tráfico (ADR-09); tras un reinicio el usuario puede volver a accionarse
/// (ANTIRREBOTE_ESTADO_NO_DISPONIBLE, CU-16 §6) sin afectar la corrección de la contención.
/// Particionado por contexto (servidor), coherente con el firewall multi-contexto (ADR-13).
/// </summary>
public sealed class EstadoAntirreboteEnMemoria
{
    private readonly ConcurrentDictionary<ClaveUsuario, DateTimeOffset> _ultimaAccionPorUsuario = new();
    private readonly object _candado = new();

    private readonly record struct ClaveUsuario(string ServidorId, string UsuarioId);

    /// <summary>
    /// Indica si una acción sobre el usuario debe suprimirse por antirrebote (RN-06): es así
    /// cuando ya se accionó sobre ese usuario y el instante actual cae dentro de la ventana de
    /// antirrebote contada desde la última acción. La primera vez (sin marca previa) devuelve
    /// false: la acción se permite. Consulta sin efectos secundarios; el registro de la marca
    /// se hace con <see cref="RegistrarAccion"/> recién cuando la acción se ejecuta.
    /// </summary>
    public bool DebeSuprimir(
        Snowflake servidorId, Snowflake usuarioId, DateTimeOffset ahora, TimeSpan ventana)
    {
        var clave = new ClaveUsuario(servidorId.Valor, usuarioId.Valor);

        lock (_candado)
        {
            if (!_ultimaAccionPorUsuario.TryGetValue(clave, out var ultima))
            {
                // Sin marca previa: nunca accionado en la ráfaga vigente (CU-16 CA-04). Tras un
                // reinicio el estado se pierde y se cae a esta rama (ANTIRREBOTE_ESTADO_NO_DISPONIBLE),
                // tratando al usuario como no accionado aún (ADR-09, CU-16 §6).
                return false;
            }

            // Dentro de la ventana [ultima, ultima + ventana]: se suprime (RN-06). Vencida la
            // ventana, la entrada queda obsoleta; se poda y se permite la nueva acción (CU-16 CA-02).
            if (ahora - ultima <= ventana)
            {
                return true;
            }

            _ultimaAccionPorUsuario.TryRemove(clave, out _);
            return false;
        }
    }

    /// <summary>
    /// Marca al usuario como accionado en el instante indicado (CU-16 §4 paso 3, CA-04). Se
    /// invoca tras ejecutar la primera acción real de la ráfaga, para que las repeticiones
    /// dentro de la ventana se supriman (RN-06).
    /// </summary>
    public void RegistrarAccion(Snowflake servidorId, Snowflake usuarioId, DateTimeOffset instante)
    {
        var clave = new ClaveUsuario(servidorId.Valor, usuarioId.Valor);

        lock (_candado)
        {
            _ultimaAccionPorUsuario[clave] = instante;
        }
    }
}
