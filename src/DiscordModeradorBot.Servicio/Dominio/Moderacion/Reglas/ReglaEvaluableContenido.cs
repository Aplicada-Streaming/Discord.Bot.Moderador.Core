using DiscordModeradorBot.Servicio.Dominio.Contenido;

namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Adaptador de una <see cref="ReglaContenido"/> (CU-04) a la abstracción <see cref="IReglaEvaluable"/>
/// para que pueda formar parte de un <see cref="GrupoDeReglas"/> (R7, RN-15). Reúsa el
/// <see cref="EvaluadorReglaContenido"/> de R3 SIN cambiarlo: el tope de tiempo viaja compilado
/// en la regla (ADR-08) y un exceso de tope se trata como no coincidencia (no propaga al grupo).
/// </summary>
public sealed class ReglaEvaluableContenido : IReglaEvaluable
{
    private readonly ReglaContenido _regla;
    private readonly EvaluadorReglaContenido _evaluador;

    public ReglaEvaluableContenido(ReglaContenido regla, EvaluadorReglaContenido evaluador)
    {
        _regla = regla ?? throw new ArgumentNullException(nameof(regla));
        _evaluador = evaluador ?? throw new ArgumentNullException(nameof(evaluador));
    }

    public string Nombre => _regla.Nombre;

    public ClaseRegla Clase => ClaseRegla.Contenido;

    /// <summary>La regla de contenido subyacente (para la explicación en palabras del panel).</summary>
    public ReglaContenido Regla => _regla;

    public bool Evaluar(ContextoEvaluacionRegla contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);

        // El predicado de contenido es SIN estado: solo mira el texto del mensaje (CU-04). Un
        // exceso del tope de tiempo se resuelve como no coincidencia (ADR-08), sin afectar al grupo.
        return _evaluador.Evaluar(contexto.Mensaje, _regla).Coincide;
    }
}
