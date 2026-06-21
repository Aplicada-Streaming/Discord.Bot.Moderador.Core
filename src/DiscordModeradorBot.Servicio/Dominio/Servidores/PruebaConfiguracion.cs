namespace DiscordModeradorBot.Servicio.Dominio.Servidores;

/// <summary>
/// Severidad de un chequeo de la prueba de configuración (CU-12, RN-16, RC-08). Un chequeo
/// BLOQUEANTE impide activar el servidor (token inválido, intents o permisos faltantes); una
/// ADVERTENCIA no impide activar pero queda visible (p. ej. jerarquía de roles del bot por
/// debajo de algún rol). Un chequeo SUPERADO no aporta restricción.
/// </summary>
public enum SeveridadChequeo
{
    /// <summary>El chequeo pasó: no impone restricción.</summary>
    Superado = 0,

    /// <summary>El chequeo falló pero no impide activar; queda visible (CU-12 CA-04).</summary>
    Advertencia = 1,

    /// <summary>El chequeo falló y BLOQUEA la activación del servidor (RN-16).</summary>
    Bloqueante = 2,
}

/// <summary>
/// Resultado de un chequeo individual de la prueba de configuración (CU-12). Lleva un código
/// estable del CU (p. ej. PRUEBA_TOKEN_INVALIDO), una etiqueta legible y su severidad.
/// </summary>
public sealed record ChequeoConfiguracion(string Codigo, string Etiqueta, SeveridadChequeo Severidad, string? Detalle = null)
{
    /// <summary>Crea un chequeo superado con su etiqueta.</summary>
    public static ChequeoConfiguracion Superado(string codigo, string etiqueta) =>
        new(codigo, etiqueta, SeveridadChequeo.Superado);

    /// <summary>Crea un chequeo bloqueante (RN-16): impide activar el servidor.</summary>
    public static ChequeoConfiguracion Bloqueante(string codigo, string etiqueta, string? detalle = null) =>
        new(codigo, etiqueta, SeveridadChequeo.Bloqueante, detalle);

    /// <summary>Crea una advertencia no bloqueante (CU-12 CA-04): visible pero permite activar.</summary>
    public static ChequeoConfiguracion Advertencia(string codigo, string etiqueta, string? detalle = null) =>
        new(codigo, etiqueta, SeveridadChequeo.Advertencia, detalle);
}

/// <summary>
/// Resultado de la prueba de configuración de un servidor (CU-12, RN-16, RC-08): la lista de
/// chequeos con su severidad. El servidor solo puede activarse si NO hay chequeos bloqueantes
/// (<see cref="PuedeActivar"/>); las advertencias se muestran pero no bloquean. El puerto del
/// adaptador (<c>IAdaptadorGateway.ProbarConfiguracionAsync</c>) devuelve este resultado: el
/// simulado lo resuelve de forma configurable para tests/escenarios y el de Discord.Net lo
/// resuelve de forma estructural contra la plataforma (token, intents, permisos, canales,
/// jerarquía).
/// </summary>
public sealed class ResultadoPruebaConfiguracion
{
    public ResultadoPruebaConfiguracion(IReadOnlyList<ChequeoConfiguracion> chequeos)
    {
        ArgumentNullException.ThrowIfNull(chequeos);
        Chequeos = chequeos;
    }

    public IReadOnlyList<ChequeoConfiguracion> Chequeos { get; }

    /// <summary>Chequeos bloqueantes detectados (RN-16): si hay alguno, no se puede activar.</summary>
    public IReadOnlyList<ChequeoConfiguracion> Bloqueantes =>
        Chequeos.Where(c => c.Severidad == SeveridadChequeo.Bloqueante).ToList();

    /// <summary>Advertencias no bloqueantes (CU-12 CA-04): visibles, no impiden activar.</summary>
    public IReadOnlyList<ChequeoConfiguracion> Advertencias =>
        Chequeos.Where(c => c.Severidad == SeveridadChequeo.Advertencia).ToList();

    /// <summary>
    /// El servidor puede activarse solo si NO hay chequeos bloqueantes (RN-16, RC-08). Las
    /// advertencias no bloquean.
    /// </summary>
    public bool PuedeActivar => Bloqueantes.Count == 0;

    /// <summary>
    /// Indica si la prueba detectó que el token es inválido/revocado (PRUEBA_TOKEN_INVALIDO),
    /// caso en el que el servidor además queda marcado como desconectado (CU-12 CA-03, CU-13).
    /// </summary>
    public bool TokenInvalido =>
        Chequeos.Any(c => c.Codigo == CodigoTokenInvalido && c.Severidad == SeveridadChequeo.Bloqueante);

    // Códigos estables de los chequeos (CU-12 §6).
    public const string CodigoTokenInvalido = "PRUEBA_TOKEN_INVALIDO";
    public const string CodigoPermisosFaltantes = "PRUEBA_PERMISOS_FALTANTES";
    public const string CodigoIntentsFaltantes = "PRUEBA_INTENTS_FALTANTES";
    public const string CodigoCanalSalidaAusente = "PRUEBA_CANAL_SALIDA_AUSENTE";
    public const string CodigoJerarquiaInsuficiente = "PRUEBA_JERARQUIA_INSUFICIENTE";
    public const string CodigoRecepcionEventos = "PRUEBA_RECEPCION_EVENTOS";
}
