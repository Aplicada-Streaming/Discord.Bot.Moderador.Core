namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Abstracción de tiempo inyectable, para que la evaluación de ventanas sea determinista
/// y los tests no dependan de pausas reales (estrategia-testing §7, ADR-09).
/// </summary>
public interface IReloj
{
    DateTimeOffset Ahora { get; }
}
