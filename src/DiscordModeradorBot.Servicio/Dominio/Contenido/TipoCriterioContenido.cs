namespace DiscordModeradorBot.Servicio.Dominio.Contenido;

/// <summary>
/// Clase de criterio de una regla de contenido (modelo-datos-logico §2.5, CU-04). El criterio
/// de una regla de contenido evalúa el texto del mensaje aislado, sin estado. R3 materializa el
/// criterio por <see cref="ExpresionRegular"/>; el criterio por palabras o frases clave queda
/// declarado como punto de extensión (CU-04, RN-03) y se materializa en una rebanada posterior.
/// El tipo de criterio es extensible: agregar uno nuevo es el mecanismo de extensión declarado
/// (ADR-12), sin reescribir el evaluador.
/// </summary>
public enum TipoCriterioContenido
{
    /// <summary>El criterio es una expresión regular que coincide contra el texto del mensaje (R3).</summary>
    ExpresionRegular = 0,

    /// <summary>
    /// El criterio es un conjunto no vacío de palabras o frases clave (CU-04, RN-03). Declarado
    /// como punto de extensión; su evaluación se materializa en una rebanada posterior.
    /// </summary>
    PalabrasClave = 1,
}
