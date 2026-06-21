namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Modo de coincidencia de un grupo de reglas (modelo-conceptual: GrupoDeReglas.modo_coincidencia,
/// RN-15). Define la combinación booleana sobre las reglas del grupo.
/// </summary>
public enum ModoCoincidencia
{
    /// <summary>El grupo coincide solo si TODAS sus reglas coinciden (AND).</summary>
    Todas = 0,

    /// <summary>El grupo coincide si AL MENOS UNA de sus reglas coincide (OR).</summary>
    Alguna = 1,

    /// <summary>El grupo coincide si coinciden AL MENOS N de sus reglas (umbral configurable).</summary>
    AlMenosN = 2,
}

/// <summary>
/// Grupo de reglas (R7, modelo-conceptual: GrupoDeReglas; RN-15, RC-04). Compone un conjunto
/// de reglas evaluables —de contenido y/o de conducta— bajo un <see cref="ModoCoincidencia"/>:
/// <list type="bullet">
/// <item><see cref="ModoCoincidencia.Todas"/>: coincide si todas las reglas coinciden.</item>
/// <item><see cref="ModoCoincidencia.Alguna"/>: coincide si al menos una coincide.</item>
/// <item><see cref="ModoCoincidencia.AlMenosN"/>: coincide si coinciden al menos N reglas.</item>
/// </list>
/// Un grupo NO puede estar vacío (RC-04): se rechaza al construir con
/// <see cref="GrupoDeReglasInvalidoException"/> (código <c>CONFIG_GRUPO_SIN_REGLAS</c>). El grupo
/// es el primero de los DOS niveles de anidamiento booleano admitidos (grupo y combinación de
/// grupos); un grupo NO contiene otros grupos (Won't-Have del intake).
/// </summary>
public sealed class GrupoDeReglas
{
    /// <summary>Código de error de grupo sin reglas (CU-11 CA-03, RN-15, RC-04).</summary>
    public const string CodigoGrupoSinReglas = "CONFIG_GRUPO_SIN_REGLAS";

    /// <summary>Código de error de N inválido para el modo AlMenosN (RN-15).</summary>
    public const string CodigoNInvalido = "CONFIG_GRUPO_N_INVALIDO";

    private readonly IReadOnlyList<IReglaEvaluable> _reglas;

    public GrupoDeReglas(
        string nombre,
        ModoCoincidencia modo,
        IReadOnlyList<IReglaEvaluable> reglas,
        int? minimoCoincidencias = null)
    {
        ArgumentNullException.ThrowIfNull(reglas);

        // RC-04 / RN-15: un grupo siempre tiene al menos una regla; un grupo vacío no decide.
        if (reglas.Count == 0)
        {
            throw new GrupoDeReglasInvalidoException(
                CodigoGrupoSinReglas,
                $"El grupo de reglas '{nombre}' debe tener al menos una regla (RN-15, RC-04).");
        }

        if (modo == ModoCoincidencia.AlMenosN)
        {
            // El umbral N debe estar entre 1 y la cantidad de reglas del grupo: por debajo de 1 no
            // exige nada; por encima de la cantidad de reglas nunca puede satisfacerse (RN-15).
            var n = minimoCoincidencias ?? 0;
            if (n < 1 || n > reglas.Count)
            {
                throw new GrupoDeReglasInvalidoException(
                    CodigoNInvalido,
                    $"El grupo '{nombre}' en modo AlMenosN requiere un N entre 1 y {reglas.Count} " +
                    $"(recibido {minimoCoincidencias?.ToString() ?? "ninguno"}, RN-15).");
            }

            MinimoCoincidencias = n;
        }
        else
        {
            // En Todas/Alguna el N no aplica; se deriva para uniformidad de la explicación en palabras.
            MinimoCoincidencias = modo == ModoCoincidencia.Todas ? reglas.Count : 1;
        }

        Nombre = nombre;
        Modo = modo;
        _reglas = reglas;
    }

    public string Nombre { get; }

    public ModoCoincidencia Modo { get; }

    /// <summary>Cantidad de reglas del grupo.</summary>
    public int CantidadReglas => _reglas.Count;

    /// <summary>
    /// Umbral efectivo de coincidencias para que el grupo coincida: N en AlMenosN, la cantidad de
    /// reglas en Todas, 1 en Alguna. Útil para la explicación en palabras (CU-11).
    /// </summary>
    public int MinimoCoincidencias { get; }

    /// <summary>Reglas del grupo (de contenido y/o de conducta), para inspección del panel.</summary>
    public IReadOnlyList<IReglaEvaluable> Reglas => _reglas;

    /// <summary>
    /// Evalúa el grupo sobre el mensaje en el contexto dado: cuenta cuántas reglas coinciden y
    /// aplica el modo de coincidencia (RN-15). Las reglas de conducta usan el estado y el instante
    /// del contexto; las de contenido solo el texto del mensaje.
    /// </summary>
    public bool Evaluar(ContextoEvaluacionRegla contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);

        var coincidencias = _reglas.Count(regla => regla.Evaluar(contexto));

        return Modo switch
        {
            ModoCoincidencia.Todas => coincidencias == _reglas.Count,
            ModoCoincidencia.Alguna => coincidencias >= 1,
            ModoCoincidencia.AlMenosN => coincidencias >= MinimoCoincidencias,
            _ => false,
        };
    }
}
