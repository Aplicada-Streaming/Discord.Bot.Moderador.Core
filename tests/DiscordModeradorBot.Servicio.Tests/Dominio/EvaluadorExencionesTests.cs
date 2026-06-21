using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas del evaluador de exenciones (R5, CU-15, RN-07): predicado puro que decide si un
/// mensaje queda exento por su usuario emisor, por alguno de sus roles o por su canal. Las
/// comparaciones de snowflake son por texto exacto (RN-08). Sin exenciones, nada queda exento.
/// </summary>
public sealed class EvaluadorExencionesTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string Canal = "300000000000000001";
    private const string CanalConfianza = "300000000000000099";
    private const string RolStaff = "700000000000000001";

    private readonly EvaluadorExenciones _evaluador = new();

    private static MensajeEntrante CrearMensaje(
        string? usuarioId = null, string? canalId = null, params string[] roles)
    {
        return MensajesDePrueba.Crear(canalId ?? Canal, Base, usuarioId: usuarioId) with
        {
            RolesDelAutor = roles.Select(r => new Snowflake(r)).ToArray(),
        };
    }

    [Fact]
    public void Sin_exenciones_no_esta_exento()
    {
        var mensaje = CrearMensaje();

        _evaluador.EstaExento(mensaje, Array.Empty<Exencion>()).Should().BeFalse();
    }

    [Fact]
    public void Usuario_emisor_exento_queda_exento()
    {
        var mensaje = CrearMensaje(usuarioId: MensajesDePrueba.UsuarioPorDefecto);
        var exenciones = new[] { Exencion.PorUsuario(new Snowflake(MensajesDePrueba.UsuarioPorDefecto)) };

        _evaluador.EstaExento(mensaje, exenciones).Should().BeTrue();
    }

    [Fact]
    public void Rol_del_emisor_exento_queda_exento()
    {
        // El emisor porta el rol staff, que está exento (CU-15 §5.C).
        var mensaje = CrearMensaje(roles: RolStaff);
        var exenciones = new[] { Exencion.PorRol(new Snowflake(RolStaff)) };

        _evaluador.EstaExento(mensaje, exenciones).Should().BeTrue();
    }

    [Fact]
    public void Rol_exento_pero_emisor_sin_ese_rol_no_queda_exento()
    {
        // La exención es por rol, pero el emisor no lo porta: no queda exento (RN-07).
        var mensaje = CrearMensaje(roles: "700000000000000002");
        var exenciones = new[] { Exencion.PorRol(new Snowflake(RolStaff)) };

        _evaluador.EstaExento(mensaje, exenciones).Should().BeFalse();
    }

    [Fact]
    public void Canal_de_confianza_queda_exento()
    {
        var mensaje = CrearMensaje(canalId: CanalConfianza);
        var exenciones = new[] { Exencion.PorCanal(new Snowflake(CanalConfianza)) };

        _evaluador.EstaExento(mensaje, exenciones).Should().BeTrue();
    }

    [Fact]
    public void Exencion_de_otro_sujeto_no_aplica()
    {
        // Exención por usuario distinto del emisor, rol que no porta y canal distinto: no exento.
        var mensaje = CrearMensaje(
            usuarioId: MensajesDePrueba.UsuarioPorDefecto, canalId: Canal, roles: "700000000000000002");
        var exenciones = new[]
        {
            Exencion.PorUsuario(new Snowflake("200000000000000999")),
            Exencion.PorRol(new Snowflake(RolStaff)),
            Exencion.PorCanal(new Snowflake(CanalConfianza)),
        };

        _evaluador.EstaExento(mensaje, exenciones).Should().BeFalse();
    }
}
