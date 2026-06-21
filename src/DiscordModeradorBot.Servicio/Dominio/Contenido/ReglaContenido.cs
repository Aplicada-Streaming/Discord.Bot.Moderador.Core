using System.Text.RegularExpressions;

namespace DiscordModeradorBot.Servicio.Dominio.Contenido;

/// <summary>
/// Regla de contenido (modelo-datos-logico §2.5, CU-04): predicado SIN estado que evalúa el
/// texto de un mensaje aislado contra un criterio configurado por el administrador. R3
/// materializa el criterio por expresión regular (<see cref="TipoCriterioContenido.ExpresionRegular"/>);
/// el criterio por palabras clave queda como punto de extensión.
///
/// El criterio se valida AL CONSTRUIR la regla (RN-03): un patrón que no compila se rechaza con
/// <see cref="ReglaContenidoInvalidaException"/> en el origen, nunca en tiempo de evaluación
/// (ADR-08). La compilación se hace UNA sola vez y se reutiliza para todas las evaluaciones; el
/// tope de tiempo de evaluación (matchTimeout) se aplica al evaluar (ADR-08, anti-ReDoS), porque
/// es el costo de evaluar, no de compilar.
///
/// El criterio se trata como NO confiable (lo provee el administrador): la expresión regular se
/// compila siempre con <see cref="RegexOptions.CultureInvariant"/> y un matchTimeout obligatorio;
/// nunca se evalúa una regex sin tope de tiempo.
/// </summary>
public sealed class ReglaContenido
{
    private readonly Regex _expresionCompilada;

    private ReglaContenido(
        string nombre,
        TipoCriterioContenido tipoCriterio,
        string criterio,
        bool sensibleAMayusculas,
        Regex expresionCompilada)
    {
        Nombre = nombre;
        TipoCriterio = tipoCriterio;
        Criterio = criterio;
        SensibleAMayusculas = sensibleAMayusculas;
        _expresionCompilada = expresionCompilada;
    }

    /// <summary>Etiqueta de la regla para el panel (modelo-datos-logico §2.5).</summary>
    public string Nombre { get; }

    /// <summary>Clase del criterio (RC-09); en R3 expresión regular.</summary>
    public TipoCriterioContenido TipoCriterio { get; }

    /// <summary>Criterio serializado: la expresión regular (modelo-datos-logico §2.5).</summary>
    public string Criterio { get; }

    /// <summary>
    /// Si la coincidencia distingue mayúsculas/minúsculas. Por defecto NO distingue (el
    /// contenido indeseado suele camuflarse con variaciones de caja); el administrador puede
    /// activar la sensibilidad por regla (CU-04).
    /// </summary>
    public bool SensibleAMayusculas { get; }

    /// <summary>
    /// Construye y VALIDA una regla de contenido por expresión regular (RN-03). El patrón se
    /// compila aquí, una sola vez, con un matchTimeout para la evaluación posterior; si no
    /// compila se lanza <see cref="ReglaContenidoInvalidaException"/> con el código
    /// CONTENIDO_PATRON_INVALIDO (CU-04 §6). El tope de tiempo NO se rechaza al construir: solo
    /// se usa para acotar cada evaluación.
    /// </summary>
    /// <param name="nombre">Etiqueta de la regla.</param>
    /// <param name="patron">Expresión regular a validar y compilar.</param>
    /// <param name="topeTiempoEvaluacion">Tope de tiempo por evaluación (anti-ReDoS, ADR-08).</param>
    /// <param name="sensibleAMayusculas">Si la coincidencia distingue caja (por defecto no).</param>
    public static ReglaContenido PorExpresionRegular(
        string nombre,
        string patron,
        TimeSpan topeTiempoEvaluacion,
        bool sensibleAMayusculas = false)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ReglaContenidoInvalidaException(
                ReglaContenidoInvalidaException.CodigoPatronInvalido,
                "El nombre de la regla de contenido es obligatorio.");
        }

        if (string.IsNullOrEmpty(patron))
        {
            // Un criterio vacío no decide de forma confiable (RN-03): se rechaza al configurar.
            throw new ReglaContenidoInvalidaException(
                ReglaContenidoInvalidaException.CodigoPatronInvalido,
                "El patrón de la regla de contenido no puede estar vacío (RN-03).");
        }

        if (topeTiempoEvaluacion <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topeTiempoEvaluacion),
                "El tope de tiempo de evaluación debe ser positivo (ADR-08).");
        }

        var opciones = RegexOptions.CultureInvariant
            | (sensibleAMayusculas ? RegexOptions.None : RegexOptions.IgnoreCase);

        Regex compilada;
        try
        {
            // Compila con matchTimeout: el tope se aplica a CADA evaluación, no a la compilación.
            // No se usa RegexOptions.Compiled para una regex no confiable de longitud arbitraria.
            compilada = new Regex(patron, opciones, topeTiempoEvaluacion);
        }
        catch (ArgumentException ex)
        {
            // El patrón no compila como expresión regular válida (RN-03, CU-04 CA-03): se rechaza
            // AL CONFIGURAR con el código del CU, nunca en tiempo de evaluación (ADR-08).
            throw new ReglaContenidoInvalidaException(
                ReglaContenidoInvalidaException.CodigoPatronInvalido,
                $"El patrón '{patron}' no es una expresión regular válida (RN-03): {ex.Message}",
                ex);
        }

        return new ReglaContenido(nombre, TipoCriterioContenido.ExpresionRegular, patron, sensibleAMayusculas, compilada);
    }

    /// <summary>
    /// Construye y VALIDA una regla de contenido por PALABRAS o FRASES CLAVE (CU-04, RN-03). Las
    /// palabras se materializan en una expresión regular interna —cada término se escapa (es texto
    /// literal) y se une por alternancia—, de modo que el evaluador sin estado las trata igual que
    /// cualquier criterio (ADR-12: agregar una clase de criterio no reescribe el evaluador). La
    /// coincidencia es "el texto contiene alguna" de las palabras/frases; por defecto NO distingue
    /// mayúsculas. Se rechaza al configurar (RN-03) si no hay al menos una palabra. El criterio
    /// persistido es la lista normalizada (una por línea), para mostrarla y reconstruir la regla.
    /// </summary>
    /// <param name="nombre">Etiqueta de la regla.</param>
    /// <param name="palabrasClave">Palabras o frases clave separadas por líneas o comas.</param>
    /// <param name="topeTiempoEvaluacion">Tope de tiempo por evaluación (anti-ReDoS, ADR-08).</param>
    /// <param name="sensibleAMayusculas">Si la coincidencia distingue caja (por defecto no).</param>
    public static ReglaContenido PorPalabrasClave(
        string nombre,
        string palabrasClave,
        TimeSpan topeTiempoEvaluacion,
        bool sensibleAMayusculas = false)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ReglaContenidoInvalidaException(
                ReglaContenidoInvalidaException.CodigoPatronInvalido,
                "El nombre de la regla de contenido es obligatorio.");
        }

        var palabras = SepararPalabrasClave(palabrasClave);
        if (palabras.Count == 0)
        {
            // Un criterio vacío no decide de forma confiable (RN-03): se rechaza al configurar.
            throw new ReglaContenidoInvalidaException(
                ReglaContenidoInvalidaException.CodigoPatronInvalido,
                "La regla por palabras clave necesita al menos una palabra o frase (RN-03).");
        }

        if (topeTiempoEvaluacion <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topeTiempoEvaluacion),
                "El tope de tiempo de evaluación debe ser positivo (ADR-08).");
        }

        var opciones = RegexOptions.CultureInvariant
            | (sensibleAMayusculas ? RegexOptions.None : RegexOptions.IgnoreCase);

        // Cada término se escapa (es texto literal, no una regex) y se une por alternancia: coincide
        // si el mensaje CONTIENE alguna palabra o frase. Como todo está escapado, siempre compila.
        var patron = string.Join("|", palabras.Select(Regex.Escape));
        var compilada = new Regex(patron, opciones, topeTiempoEvaluacion);

        // Criterio normalizado (una palabra/frase por línea) para mostrar y reconstruir la regla.
        var criterio = string.Join("\n", palabras);
        return new ReglaContenido(nombre, TipoCriterioContenido.PalabrasClave, criterio, sensibleAMayusculas, compilada);
    }

    /// <summary>
    /// Separa la lista de palabras/frases clave: admite separación por líneas o comas, recorta
    /// espacios, y descarta vacíos y duplicados (sin distinguir caja), preservando el orden.
    /// </summary>
    private static IReadOnlyList<string> SepararPalabrasClave(string? palabrasClave)
    {
        if (string.IsNullOrWhiteSpace(palabrasClave))
        {
            return Array.Empty<string>();
        }

        var vistas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resultado = new List<string>();
        foreach (var bruto in palabrasClave.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var termino = bruto.Trim();
            if (termino.Length > 0 && vistas.Add(termino))
            {
                resultado.Add(termino);
            }
        }

        return resultado;
    }

    /// <summary>
    /// Expresión regular ya compilada y validada, con su matchTimeout incorporado. La usa el
    /// evaluador (sin estado) para coincidir contra el texto del mensaje (CU-04).
    /// </summary>
    internal Regex ExpresionCompilada => _expresionCompilada;
}
