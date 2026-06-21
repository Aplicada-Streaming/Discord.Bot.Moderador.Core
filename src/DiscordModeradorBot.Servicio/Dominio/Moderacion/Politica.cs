using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Política de moderación (Evento del modelo lógico §2.8). Tiene un modo, una prioridad
/// de evaluación (RN-04), una bandera continuar y un conjunto de acciones a ejecutar en
/// orden (RN-05). En R1 una política agrupa una sola regla de conducta (ráfaga
/// distribuida); R3 agrega el segundo eje: una política puede disparar por una regla de
/// CONTENIDO sin estado (CU-04).
///
/// R7 generaliza la condición de disparo: una política puede componerse de uno o más
/// <see cref="GrupoDeReglas"/> con modos de coincidencia (RN-15) a través de una
/// <see cref="Composicion"/>. Las dos formas previas (regla de conducta única o regla de
/// contenido única) se mantienen como atajos retrocompatibles: si la política trae una
/// <see cref="ReglaContenido"/> equivale a un grupo con esa regla, y si no trae composición ni
/// regla de contenido equivale al eje de conducta de R1 (regresión). El motor prioriza la
/// composición cuando está presente. El modo por defecto es Simulacion (RC-10, RN-09).
/// </summary>
public sealed class Politica
{
    public Politica(
        string nombre,
        int prioridad,
        Modo modo = Modo.Simulacion,
        bool continuar = false,
        IReadOnlyList<Accion>? acciones = null,
        ReglaContenido? reglaContenido = null,
        ComposicionPolitica? composicion = null)
    {
        Nombre = nombre;
        Prioridad = prioridad;
        Modo = modo;
        Continuar = continuar;
        Acciones = acciones ?? new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) };
        ReglaContenido = reglaContenido;
        Composicion = composicion;
    }

    public string Nombre { get; }

    /// <summary>Orden de evaluación; menor valor = mayor prioridad (RN-04, RC-05).</summary>
    public int Prioridad { get; }

    public Modo Modo { get; }

    /// <summary>Permite seguir evaluando políticas de menor prioridad tras coincidir (RN-04).</summary>
    public bool Continuar { get; }

    /// <summary>Acciones del evento, en orden de ejecución (RN-05).</summary>
    public IReadOnlyList<Accion> Acciones { get; }

    /// <summary>
    /// Regla de contenido sin estado asociada (CU-04, R3). Si está presente y NO hay composición,
    /// la política dispara cuando el TEXTO del mensaje cumple el criterio de esta regla (eje de
    /// contenido). Si es null y no hay composición, la política dispara por el eje de conducta
    /// (ráfaga distribuida de R1).
    /// </summary>
    public ReglaContenido? ReglaContenido { get; }

    /// <summary>
    /// Composición de grupos de reglas con modos de coincidencia (R7, RN-15). Si está presente,
    /// es la condición de disparo de la política y tiene prioridad sobre <see cref="ReglaContenido"/>
    /// y sobre el eje de conducta directo. Si es null, la política conserva el comportamiento de
    /// R1-R6 (regla de contenido única o ráfaga de conducta).
    /// </summary>
    public ComposicionPolitica? Composicion { get; }

    /// <summary>Si la política dispara por una composición de grupos de reglas (R7, RN-15).</summary>
    public bool TieneComposicion => Composicion is not null;

    /// <summary>
    /// Si la política se evalúa por el eje de contenido directo (CU-04, R3) en vez del de conducta
    /// (CU-01). Solo aplica cuando NO hay composición (R7): con composición, los ejes los deciden
    /// las reglas de cada grupo.
    /// </summary>
    public bool EsDeContenido => Composicion is null && ReglaContenido is not null;
}
