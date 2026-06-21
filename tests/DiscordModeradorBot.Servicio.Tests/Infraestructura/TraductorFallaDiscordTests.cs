using System.Net;
using Discord;
using DiscordModeradorBot.Servicio.Dominio.Gateway;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas del traductor de la excepción del SDK (Discord.Net) a la naturaleza ABSTRACTA de la
/// falla (RN-01, ADR-08). Verifica que un 403 con código MissingPermissions se traduce a jerarquía
/// superior, InsufficientPermissions a permisos faltantes, y otras fallas a falla de plataforma.
/// No abre conexión: solo construye la excepción del SDK con sus códigos.
/// </summary>
public sealed class TraductorFallaDiscordTests
{
    private static Discord.Net.HttpException Http(HttpStatusCode codigo, DiscordErrorCode? discord = null) =>
        new(codigo, request: null!, discordCode: discord, reason: null);

    [Fact]
    public void MissingPermissions_se_traduce_a_jerarquia_superior()
    {
        var falla = TraductorFallaDiscord.Traducir(
            Http(HttpStatusCode.Forbidden, DiscordErrorCode.MissingPermissions));
        falla.Should().Be(TipoFallaAccion.JerarquiaSuperior);
    }

    [Fact]
    public void InsufficientPermissions_se_traduce_a_permisos_faltantes()
    {
        var falla = TraductorFallaDiscord.Traducir(
            Http(HttpStatusCode.Forbidden, DiscordErrorCode.InsufficientPermissions));
        falla.Should().Be(TipoFallaAccion.PermisosFaltantes);
    }

    [Fact]
    public void Un_403_sin_codigo_se_atribuye_a_permisos_faltantes()
    {
        var falla = TraductorFallaDiscord.Traducir(Http(HttpStatusCode.Forbidden));
        falla.Should().Be(TipoFallaAccion.PermisosFaltantes);
    }

    [Fact]
    public void Otra_falla_http_es_falla_de_plataforma()
    {
        var falla = TraductorFallaDiscord.Traducir(Http(HttpStatusCode.InternalServerError));
        falla.Should().Be(TipoFallaAccion.FallaPlataforma);
    }

    [Fact]
    public void Una_excepcion_no_http_es_falla_de_plataforma()
    {
        var falla = TraductorFallaDiscord.Traducir(new TimeoutException());
        falla.Should().Be(TipoFallaAccion.FallaPlataforma);
    }
}
