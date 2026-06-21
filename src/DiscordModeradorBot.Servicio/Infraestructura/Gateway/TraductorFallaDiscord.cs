using System.Net;
using Discord;
using DiscordModeradorBot.Servicio.Dominio.Gateway;

namespace DiscordModeradorBot.Servicio.Infraestructura.Gateway;

/// <summary>
/// Traduce una excepción del SDK (Discord.Net) a la naturaleza ABSTRACTA de la falla
/// (<see cref="TipoFallaAccion"/>), para que la CLASIFICACIÓN final del resultado siga siendo pura
/// y testeable (<see cref="ClasificadorResultadoAccion"/>, RN-01, ADR-08). Aquí vive lo único que
/// depende del SDK: el reconocimiento de los códigos HTTP/Discord de permisos y jerarquía.
///
/// La plataforma responde HTTP 403 (Forbidden) tanto cuando el usuario tiene rol superior como
/// cuando al bot le falta el permiso; el código de Discord los distingue: <c>MissingPermissions</c>
/// (50001) corresponde a "no se puede accionar sobre este usuario" (jerarquía superior, RN-01),
/// mientras que <c>InsufficientPermissions</c> (50013) corresponde a que el bot carece del permiso.
/// Cualquier otra falla se trata como falla de plataforma (ADR-08).
/// </summary>
public static class TraductorFallaDiscord
{
    /// <summary>Traduce una excepción a la naturaleza de la falla (RN-01, ADR-08).</summary>
    public static TipoFallaAccion Traducir(Exception ex)
    {
        if (ex is not Discord.Net.HttpException http)
        {
            // No es una falla HTTP de la plataforma (timeout, red, etc.): falla de plataforma.
            return TipoFallaAccion.FallaPlataforma;
        }

        // Código de Discord específico, cuando está presente: distingue jerarquía de permisos.
        if (http.DiscordCode is { } codigo)
        {
            return codigo switch
            {
                DiscordErrorCode.MissingPermissions => TipoFallaAccion.JerarquiaSuperior,
                DiscordErrorCode.InsufficientPermissions => TipoFallaAccion.PermisosFaltantes,
                _ => http.HttpCode == HttpStatusCode.Forbidden
                    ? TipoFallaAccion.PermisosFaltantes
                    : TipoFallaAccion.FallaPlataforma,
            };
        }

        // Sin código específico: un 403 se atribuye a permisos faltantes (no accionable, RN-01);
        // el resto es falla de plataforma (ADR-08).
        return http.HttpCode == HttpStatusCode.Forbidden
            ? TipoFallaAccion.PermisosFaltantes
            : TipoFallaAccion.FallaPlataforma;
    }
}
