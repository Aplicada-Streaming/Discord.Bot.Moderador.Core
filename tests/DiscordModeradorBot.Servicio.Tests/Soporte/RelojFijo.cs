using DiscordModeradorBot.Servicio.Aplicacion.Puertos;

namespace DiscordModeradorBot.Servicio.Tests.Soporte;

/// <summary>
/// Reloj inyectable de tiempo fijo para tests deterministas (estrategia-testing §7).
/// </summary>
public sealed class RelojFijo : IReloj
{
    public RelojFijo(DateTimeOffset ahora) => Ahora = ahora;

    public DateTimeOffset Ahora { get; set; }
}
