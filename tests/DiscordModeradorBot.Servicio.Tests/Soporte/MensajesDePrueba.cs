using DiscordModeradorBot.Servicio.Dominio;

namespace DiscordModeradorBot.Servicio.Tests.Soporte;

/// <summary>
/// Fábrica de mensajes de prueba con snowflakes deterministas como texto (RN-08,
/// estrategia-testing §6).
/// </summary>
public static class MensajesDePrueba
{
    public const string ServidorPorDefecto = "100000000000000001";
    public const string UsuarioPorDefecto = "200000000000000002";

    private static int _contadorMensajes;

    public static MensajeEntrante Crear(
        string canalId,
        DateTimeOffset instante,
        string? usuarioId = null,
        string? servidorId = null,
        string contenido = "contenido de prueba")
    {
        var idMensaje = $"4000000000000000{System.Threading.Interlocked.Increment(ref _contadorMensajes):000}";

        return new MensajeEntrante(
            new Snowflake(servidorId ?? ServidorPorDefecto),
            new Snowflake(canalId),
            new Snowflake(usuarioId ?? UsuarioPorDefecto),
            new Snowflake(idMensaje),
            instante,
            contenido);
    }
}
