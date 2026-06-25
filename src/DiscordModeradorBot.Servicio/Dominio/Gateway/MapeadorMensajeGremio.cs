namespace DiscordModeradorBot.Servicio.Dominio.Gateway;

/// <summary>
/// Mapeo puro (sin SDK ni red) de un <see cref="DatosMensajeGremio"/> a
/// <see cref="MensajeEntrante"/>, con el descarte de los mensajes que NO se moderan
/// (flujo-ejecucion §1, ADR-13). Aislar este mapeo del adaptador real (Discord.Net) lo hace
/// testeable con dobles: se prueba el mapeo de los roles del autor (RN-08) y el descarte de bots,
/// mensajes de sistema y mensajes directos (DM) sin tocar la plataforma.
/// </summary>
public static class MapeadorMensajeGremio
{
    /// <summary>
    /// Convierte los datos del mensaje a un <see cref="MensajeEntrante"/> normalizado, o devuelve
    /// null si el mensaje DEBE descartarse: mensaje directo (no de gremio), autor bot, mensaje de
    /// sistema, o algún snowflake con formato inválido (RN-08). El descarte no propaga excepción:
    /// el adaptador simplemente no entrega el mensaje al motor.
    /// </summary>
    public static MensajeEntrante? Mapear(DatosMensajeGremio datos)
    {
        ArgumentNullException.ThrowIfNull(datos);

        // Solo se modera el tráfico de servidores: se ignoran DM, bots y mensajes de sistema.
        if (!datos.EsDeGremio || datos.AutorEsBot || datos.EsMensajeDeSistema)
        {
            return null;
        }

        // Los identificadores deben tener formato de snowflake (RN-08); si no, se descarta el
        // mensaje en lugar de romper el pipeline con una excepción.
        if (!Snowflake.TryParse(datos.ServidorId, out var servidor) ||
            !Snowflake.TryParse(datos.CanalId, out var canal) ||
            !Snowflake.TryParse(datos.UsuarioId, out var usuario) ||
            !Snowflake.TryParse(datos.MensajeId, out var mensajeId))
        {
            return null;
        }

        // Roles del autor para evaluar exenciones por rol (CU-15, RN-07). Se descartan los valores
        // con formato inválido; los válidos se mapean a snowflakes de texto (RN-08). El rol
        // universal @everyone ya viene excluido por el adaptador.
        var roles = datos.RolesDelAutor
            .Where(Snowflake.EsFormatoValido)
            .Select(r => new Snowflake(r))
            .ToArray();

        return new MensajeEntrante(
            servidor,
            canal,
            usuario,
            mensajeId,
            datos.Instante,
            datos.Contenido ?? string.Empty)
        {
            RolesDelAutor = roles,
            NombreCanal = datos.NombreCanal ?? string.Empty,
        };
    }
}
