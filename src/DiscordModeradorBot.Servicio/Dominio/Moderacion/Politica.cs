namespace DiscordModeradorBot.Servicio.Dominio.Moderacion;

/// <summary>
/// Política de moderación (Evento del modelo lógico §2.8). Tiene un modo, una prioridad
/// de evaluación (RN-04), una bandera continuar y un conjunto de acciones a ejecutar en
/// orden (RN-05). En R1 una política agrupa una sola regla de conducta (ráfaga
/// distribuida); el grupo de reglas con modos de coincidencia se completa en R7.
/// El modo por defecto es Simulacion (RC-10, RN-09).
/// </summary>
public sealed class Politica
{
    public Politica(
        string nombre,
        int prioridad,
        Modo modo = Modo.Simulacion,
        bool continuar = false,
        IReadOnlyList<Accion>? acciones = null)
    {
        Nombre = nombre;
        Prioridad = prioridad;
        Modo = modo;
        Continuar = continuar;
        Acciones = acciones ?? new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) };
    }

    public string Nombre { get; }

    /// <summary>Orden de evaluación; menor valor = mayor prioridad (RN-04, RC-05).</summary>
    public int Prioridad { get; }

    public Modo Modo { get; }

    /// <summary>Permite seguir evaluando políticas de menor prioridad tras coincidir (RN-04).</summary>
    public bool Continuar { get; }

    /// <summary>Acciones del evento, en orden de ejecución (RN-05).</summary>
    public IReadOnlyList<Accion> Acciones { get; }
}
