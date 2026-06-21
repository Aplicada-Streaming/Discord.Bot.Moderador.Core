using DiscordModeradorBot.Servicio.Dominio.Conducta;

namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Contexto que necesita una regla para evaluarse sobre un mensaje (R7). Empaqueta el mensaje
/// entrante y el estado de conducta más el instante de evaluación, de modo que una regla de
/// CONTENIDO (sin estado, CU-04) y una de CONDUCTA (con estado, CU-01) compartan la misma
/// firma <see cref="IReglaEvaluable.Evaluar"/> y puedan combinarse en un
/// <see cref="GrupoDeReglas"/> con un modo de coincidencia (RN-15). El estado de conducta solo
/// lo usan las reglas de conducta; las de contenido lo ignoran.
/// </summary>
public sealed record ContextoEvaluacionRegla(
    MensajeEntrante Mensaje,
    EstadoConductaEnMemoria EstadoConducta,
    DateTimeOffset InstanteEvaluacion);
