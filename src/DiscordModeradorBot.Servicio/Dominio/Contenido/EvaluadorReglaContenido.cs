using System.Text.RegularExpressions;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;

namespace DiscordModeradorBot.Servicio.Dominio.Contenido;

/// <summary>
/// Resultado de evaluar una regla de contenido sobre un mensaje (CU-04).
/// </summary>
/// <param name="Coincide">Si el contenido cumple el criterio de la regla.</param>
/// <param name="ExcedioTope">
/// Si la evaluación se abortó por superar el tope de tiempo (ADR-08, CU-04
/// CONTENIDO_EVALUACION_EXCEDE_TIEMPO). En ese caso <see cref="Coincide"/> es false: una
/// evaluación que no terminó NO marca la condición; se trata como no coincidencia y se registra.
/// </param>
public readonly record struct ResultadoEvaluacionContenido(bool Coincide, bool ExcedioTope)
{
    /// <summary>Resultado de no coincidencia sin excederse del tope.</summary>
    public static ResultadoEvaluacionContenido NoCoincide { get; } = new(false, false);

    /// <summary>Resultado de coincidencia.</summary>
    public static ResultadoEvaluacionContenido Coincidio { get; } = new(true, false);

    /// <summary>Resultado de evaluación abortada por tope de tiempo (no coincidencia, ADR-08).</summary>
    public static ResultadoEvaluacionContenido TopeExcedido { get; } = new(false, true);
}

/// <summary>
/// Evaluador de reglas de contenido (CU-04): predicado SIN estado, segundo eje de defensa
/// complementario al eje de conducta (ráfaga) de R1. Dado un <see cref="MensajeEntrante"/> y una
/// <see cref="ReglaContenido"/>, determina si el TEXTO del mensaje aislado cumple el criterio,
/// sin observar la actividad acumulada del usuario (flujo-ejecucion etapa 2).
///
/// La expresión regular ya está validada y compilada (RN-03), y se evalúa con un tope de tiempo
/// (matchTimeout incorporado en la regla, ADR-08): ante retroceso catastrófico/ReDoS, la
/// evaluación NO propaga excepción al pipeline; se captura <see cref="RegexMatchTimeoutException"/>,
/// se trata como no coincidencia y se señala con <see cref="ResultadoEvaluacionContenido.ExcedioTope"/>
/// para que el motor lo registre (CU-04 CONTENIDO_EVALUACION_EXCEDE_TIEMPO). El tope de tiempo es
/// del Dominio/Aplicación, no de infraestructura.
/// </summary>
public sealed class EvaluadorReglaContenido
{
    /// <summary>
    /// Evalúa el contenido del mensaje contra la regla. No lanza por ReDoS ni por tope de tiempo;
    /// un exceso de tope se resuelve como no coincidencia señalada (ADR-08).
    /// </summary>
    public ResultadoEvaluacionContenido Evaluar(MensajeEntrante mensaje, ReglaContenido regla)
    {
        ArgumentNullException.ThrowIfNull(mensaje);
        ArgumentNullException.ThrowIfNull(regla);

        return Evaluar(mensaje.Contenido, regla);
    }

    /// <summary>
    /// Evalúa un texto contra la regla. Sobrecarga útil para pruebas y para reúso por otros
    /// componentes; el contrato de tope de tiempo y no propagación es el mismo (ADR-08).
    /// </summary>
    public ResultadoEvaluacionContenido Evaluar(string? contenido, ReglaContenido regla)
    {
        ArgumentNullException.ThrowIfNull(regla);

        // Un mensaje sin texto no puede coincidir con un criterio de contenido (CU-04).
        if (string.IsNullOrEmpty(contenido))
        {
            return ResultadoEvaluacionContenido.NoCoincide;
        }

        try
        {
            return regla.ExpresionCompilada.IsMatch(contenido)
                ? ResultadoEvaluacionContenido.Coincidio
                : ResultadoEvaluacionContenido.NoCoincide;
        }
        catch (RegexMatchTimeoutException)
        {
            // Retroceso catastrófico / ReDoS: el matchTimeout cortó la evaluación. No se propaga
            // al pipeline (ADR-08): se trata como no coincidencia y se señala el exceso de tope
            // para que el motor lo registre (CU-04 CONTENIDO_EVALUACION_EXCEDE_TIEMPO).
            return ResultadoEvaluacionContenido.TopeExcedido;
        }
    }

    /// <summary>
    /// Tope de tiempo por defecto del descriptor, expuesto para componer reglas con su matchTimeout
    /// (ADR-08, ADR-12). Derivado del descriptor único; no se hardcodea en la lógica (RN-10).
    /// </summary>
    public static TimeSpan TopeTiempoPorDefecto =>
        TimeSpan.FromMilliseconds(RegistroDescriptores.TopeTiempoEvaluacionContenidoMs.ValorPorDefecto);
}
