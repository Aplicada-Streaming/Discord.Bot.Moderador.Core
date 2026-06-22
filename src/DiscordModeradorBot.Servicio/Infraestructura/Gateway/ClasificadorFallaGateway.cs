namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Clasificación pura (sin red) de las señales de log/excepción de Discord.Net durante la prueba de
/// configuración (CU-12). Con un token inválido, <c>LoginAsync</c> no siempre lanza: la plataforma
/// rechaza la credencial al conectar el gateway (REST 401 Unauthorized o close code 4004). Sin
/// distinguir ese caso, la prueba daba un falso "token válido" y culpaba a los intents. Esta lógica
/// reconoce el rechazo de credencial y, deliberadamente, NO confunde el 4014 ("Disallowed intent(s)",
/// que sí es problema de intents) con un token inválido.
/// </summary>
public static class ClasificadorFallaGateway
{
    /// <summary>
    /// True si el mensaje indica que la PLATAFORMA rechazó la credencial (token inválido): 401
    /// Unauthorized, close code 4004 / "Authentication failed", o "token ... invalid". NO matchea el
    /// 4014 de intents (se evita a propósito el prefijo numérico "401" suelto, que está dentro de "4014").
    /// </summary>
    public static bool EsSenalDeTokenInvalido(string? mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje))
        {
            return false;
        }

        return mensaje.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase)
            || mensaje.Contains("4004", StringComparison.OrdinalIgnoreCase)
            || mensaje.Contains("Authentication failed", StringComparison.OrdinalIgnoreCase)
            || (mensaje.Contains("token", StringComparison.OrdinalIgnoreCase)
                && mensaje.Contains("invalid", StringComparison.OrdinalIgnoreCase));
    }
}
