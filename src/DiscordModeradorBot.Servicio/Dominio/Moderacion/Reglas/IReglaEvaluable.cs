namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Abstracción de una regla evaluable sobre un mensaje (R7, modelo-conceptual: Regla). Es la
/// pieza atómica que un <see cref="GrupoDeReglas"/> combina según su modo de coincidencia
/// (RN-15). La implementan tanto las reglas de CONTENIDO (predicado sin estado, CU-04) como
/// las de CONDUCTA (ráfaga distribuida con estado, CU-01); ambas se reducen a un predicado
/// booleano sobre el <see cref="ContextoEvaluacionRegla"/>, lo que permite componer ejes
/// distintos en un mismo grupo. La clase de la regla (contenido/conducta) es del modelo, pero
/// la composición no la distingue: solo le importa si coincide.
/// </summary>
public interface IReglaEvaluable
{
    /// <summary>Etiqueta de la regla para el panel y la explicación en palabras (CU-11).</summary>
    string Nombre { get; }

    /// <summary>Clase de la regla (contenido o conducta), para mostrarla en el panel (CU-11).</summary>
    ClaseRegla Clase { get; }

    /// <summary>Indica si la regla coincide con el mensaje en el contexto dado (RN-15).</summary>
    bool Evaluar(ContextoEvaluacionRegla contexto);
}

/// <summary>Clase de una regla (modelo-conceptual: Regla.clase).</summary>
public enum ClaseRegla
{
    /// <summary>Regla sin estado sobre el texto del mensaje (CU-04).</summary>
    Contenido = 0,

    /// <summary>Regla con estado sobre la actividad acumulada del usuario (CU-01).</summary>
    Conducta = 1,
}
