namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Modo de combinación de los grupos de una política (R7, RN-15). Es el SEGUNDO nivel de
/// anidamiento booleano: combina los resultados de cada <see cref="GrupoDeReglas"/>.
/// </summary>
public enum ModoCombinacionGrupos
{
    /// <summary>La composición dispara si TODOS los grupos coinciden (AND de grupos).</summary>
    Todos = 0,

    /// <summary>La composición dispara si AL MENOS UN grupo coincide (OR de grupos).</summary>
    Alguno = 1,
}

/// <summary>
/// Composición de la condición de disparo de una política/evento (R7, modelo-conceptual:
/// Evento ↔ EventoGrupo ↔ GrupoDeReglas; RN-15). Es el SEGUNDO y último nivel de anidamiento
/// booleano admitido: una política se compone de uno o más <see cref="GrupoDeReglas"/> (nivel 1:
/// reglas dentro de un grupo) combinados con un <see cref="ModoCombinacionGrupos"/> (nivel 2:
/// combinación de grupos). NO se admite un tercer nivel (un grupo no contiene grupos): es un
/// Won't-Have del intake. Una política de R1-R6 con una sola regla equivale a una composición de
/// un grupo en modo <see cref="ModoCoincidencia.Alguna"/> con esa regla.
/// </summary>
public sealed class ComposicionPolitica
{
    /// <summary>Código de error de composición sin grupos (RN-15).</summary>
    public const string CodigoSinGrupos = "CONFIG_COMPOSICION_SIN_GRUPOS";

    private readonly IReadOnlyList<GrupoDeReglas> _grupos;

    public ComposicionPolitica(
        IReadOnlyList<GrupoDeReglas> grupos,
        ModoCombinacionGrupos modo = ModoCombinacionGrupos.Todos)
    {
        ArgumentNullException.ThrowIfNull(grupos);

        if (grupos.Count == 0)
        {
            throw new GrupoDeReglasInvalidoException(
                CodigoSinGrupos,
                "La composición de la política debe tener al menos un grupo de reglas (RN-15).");
        }

        _grupos = grupos;
        Modo = modo;
    }

    public ModoCombinacionGrupos Modo { get; }

    public IReadOnlyList<GrupoDeReglas> Grupos => _grupos;

    /// <summary>
    /// Crea una composición de un solo grupo, que a su vez contiene una sola regla en modo
    /// <see cref="ModoCoincidencia.Alguna"/>. Es la equivalencia de una política de R1-R6 con una
    /// regla única, para integrar el modelo nuevo sin cambiar el comportamiento (regresión).
    /// </summary>
    public static ComposicionPolitica DeReglaUnica(IReglaEvaluable regla, string? nombreGrupo = null)
    {
        ArgumentNullException.ThrowIfNull(regla);

        var grupo = new GrupoDeReglas(
            nombreGrupo ?? regla.Nombre, ModoCoincidencia.Alguna, new[] { regla });
        return new ComposicionPolitica(new[] { grupo }, ModoCombinacionGrupos.Todos);
    }

    /// <summary>
    /// Evalúa la composición sobre el mensaje en el contexto dado, combinando los grupos según el
    /// modo (RN-15). En modo Todos exige que cada grupo coincida; en Alguno alcanza con uno.
    /// </summary>
    public bool Evaluar(ContextoEvaluacionRegla contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);

        return Modo switch
        {
            ModoCombinacionGrupos.Todos => _grupos.All(g => g.Evaluar(contexto)),
            ModoCombinacionGrupos.Alguno => _grupos.Any(g => g.Evaluar(contexto)),
            _ => false,
        };
    }
}
