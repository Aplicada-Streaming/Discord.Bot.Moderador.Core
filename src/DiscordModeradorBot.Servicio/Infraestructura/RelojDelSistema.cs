using DiscordModeradorBot.Servicio.Aplicacion.Puertos;

namespace DiscordModeradorBot.Servicio.Infraestructura;

/// <summary>Reloj real del sistema (implementación de <see cref="IReloj"/>).</summary>
public sealed class RelojDelSistema : IReloj
{
    public DateTimeOffset Ahora => DateTimeOffset.UtcNow;
}
