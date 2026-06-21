using DiscordModeradorBot.Servicio.Dominio.Gateway;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas del mapeo PURO de un mensaje de gremio (abstraído del SDK) a
/// <see cref="DiscordModeradorBot.Servicio.Dominio.MensajeEntrante"/> y del descarte de los
/// mensajes que no se moderan (bots, DM, mensajes de sistema), sin tocar Discord.Net ni red
/// (RN-07, RN-08, flujo-ejecucion §1). El adaptador real construye el DTO desde el tipo del SDK;
/// estos tests lo construyen con dobles.
/// </summary>
public sealed class MapeadorMensajeGremioTests
{
    private static DatosMensajeGremio CrearDatos(
        bool esDeGremio = true,
        bool autorEsBot = false,
        bool esSistema = false,
        string servidorId = "100000000000000001",
        string canalId = "300000000000000001",
        string usuarioId = "200000000000000002",
        string mensajeId = "400000000000000003",
        string contenido = "hola",
        IReadOnlyCollection<string>? roles = null) =>
        new(
            ServidorId: servidorId,
            CanalId: canalId,
            UsuarioId: usuarioId,
            MensajeId: mensajeId,
            Instante: DateTimeOffset.UnixEpoch,
            Contenido: contenido,
            EsDeGremio: esDeGremio,
            AutorEsBot: autorEsBot,
            EsMensajeDeSistema: esSistema,
            RolesDelAutor: roles ?? []);

    [Fact]
    public void Mapea_un_mensaje_de_gremio_con_sus_identificadores_y_roles()
    {
        // Given los datos de un mensaje de usuario en un servidor, con dos roles del autor (RN-08).
        var datos = CrearDatos(roles: new[] { "700000000000000001", "700000000000000002" });

        // When se mapea.
        var entrante = MapeadorMensajeGremio.Mapear(datos);

        // Then se obtiene un MensajeEntrante con los snowflakes y los roles del autor (RN-07).
        entrante.Should().NotBeNull();
        entrante!.ServidorId.Valor.Should().Be("100000000000000001");
        entrante.CanalId.Valor.Should().Be("300000000000000001");
        entrante.UsuarioId.Valor.Should().Be("200000000000000002");
        entrante.MensajeId.Valor.Should().Be("400000000000000003");
        entrante.Contenido.Should().Be("hola");
        entrante.RolesDelAutor.Select(r => r.Valor)
            .Should().BeEquivalentTo("700000000000000001", "700000000000000002");
    }

    [Fact]
    public void Descarta_los_mensajes_de_bots()
    {
        var entrante = MapeadorMensajeGremio.Mapear(CrearDatos(autorEsBot: true));
        entrante.Should().BeNull();
    }

    [Fact]
    public void Descarta_los_mensajes_directos_no_de_gremio()
    {
        // Un DM (no de gremio) no se modera (flujo §1).
        var entrante = MapeadorMensajeGremio.Mapear(CrearDatos(esDeGremio: false));
        entrante.Should().BeNull();
    }

    [Fact]
    public void Descarta_los_mensajes_de_sistema()
    {
        var entrante = MapeadorMensajeGremio.Mapear(CrearDatos(esSistema: true));
        entrante.Should().BeNull();
    }

    [Fact]
    public void Descarta_un_mensaje_con_snowflake_invalido_sin_romper()
    {
        // Un identificador con formato inválido descarta el mensaje en lugar de lanzar (RN-08).
        var entrante = MapeadorMensajeGremio.Mapear(CrearDatos(canalId: "no-es-snowflake"));
        entrante.Should().BeNull();
    }

    [Fact]
    public void Ignora_los_roles_con_formato_invalido_y_mapea_los_validos()
    {
        // Un rol con formato inválido se descarta; los válidos se conservan (RN-08).
        var datos = CrearDatos(roles: new[] { "700000000000000001", "rol-invalido" });

        var entrante = MapeadorMensajeGremio.Mapear(datos);

        entrante.Should().NotBeNull();
        entrante!.RolesDelAutor.Select(r => r.Valor).Should().ContainSingle()
            .Which.Should().Be("700000000000000001");
    }

    [Fact]
    public void Un_mensaje_sin_contenido_mapea_a_contenido_vacio()
    {
        // Sin el intent de contenido, el texto puede llegar vacío: no rompe el mapeo.
        var entrante = MapeadorMensajeGremio.Mapear(CrearDatos(contenido: ""));
        entrante.Should().NotBeNull();
        entrante!.Contenido.Should().BeEmpty();
    }
}
