namespace DiscordModeradorBot.Servicio.Dominio.Exenciones;

/// <summary>
/// Filtro de exentos del pipeline (flujo-ejecucion etapa 1, CU-15, RN-07). Dado un mensaje
/// entrante y el conjunto de exenciones de su servidor, decide si el sujeto queda DESCARTADO
/// antes de evaluar cualquier regla. Predicado puro, sin estado: un sujeto está exento si
/// coincide con alguna exención por usuario emisor, por alguno de los roles del emisor, o por
/// el canal del mensaje. La exención por rol cubre dinámicamente a quien porte el rol, sin
/// enumerar usuarios (CU-15 §5.C). Las comparaciones de snowflake son por texto exacto (RN-08).
/// </summary>
public sealed class EvaluadorExenciones
{
    /// <summary>
    /// Indica si el mensaje debe descartarse por una exención del servidor (RN-07). Devuelve
    /// true ante el primer criterio que coincide (usuario, rol o canal); las exenciones de
    /// otros servidores no aplican porque la colección que se pasa es la del contexto del mensaje.
    /// </summary>
    public bool EstaExento(MensajeEntrante mensaje, IReadOnlyCollection<Exencion> exenciones)
    {
        ArgumentNullException.ThrowIfNull(mensaje);
        ArgumentNullException.ThrowIfNull(exenciones);

        if (exenciones.Count == 0)
        {
            return false;
        }

        foreach (var exencion in exenciones)
        {
            var coincide = exencion.Tipo switch
            {
                // Usuario emisor de confianza (CU-15 CA-01/CA-02).
                TipoSujetoExento.Usuario => exencion.Sujeto.Valor == mensaje.UsuarioId.Valor,

                // Canal de confianza: excluye la actividad de ese canal (CU-15 §5.B / CA-04).
                TipoSujetoExento.Canal => exencion.Sujeto.Valor == mensaje.CanalId.Valor,

                // Rol exento: cubre a quien porte el rol (CU-15 §5.C). Requiere los roles del
                // autor poblados por el adaptador en el mensaje (default vacío).
                TipoSujetoExento.Rol => mensaje.RolesDelAutor.Any(r => r.Valor == exencion.Sujeto.Valor),

                _ => false,
            };

            if (coincide)
            {
                return true;
            }
        }

        return false;
    }
}
