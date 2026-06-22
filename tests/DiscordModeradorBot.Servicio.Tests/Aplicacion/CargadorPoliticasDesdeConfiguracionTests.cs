using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del cargador que traduce la configuración persistida del panel (CU-11) al modelo de
/// dominio que evalúa el motor: eventos → políticas con composición de grupos, grupos → reglas de
/// contenido (por id) o de conducta (por clave), y acciones con su tipo, parámetros y orden (RN-05).
/// Este puente es el que hace que lo configurado DIRIJA la moderación; antes el motor usaba una
/// política hardcodeada. Verifica también la tolerancia a configuración incompleta (ADR-08, RC-04).
/// </summary>
public sealed class CargadorPoliticasDesdeConfiguracionTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);
    private static readonly Snowflake Servidor = new(MensajesDePrueba.ServidorPorDefecto);

    private readonly IRepositorioConfiguracion _configuracion = Substitute.For<IRepositorioConfiguracion>();
    private readonly IRepositorioReglasContenido _reglasContenido = Substitute.For<IRepositorioReglasContenido>();
    private readonly EvaluadorReglaContenido _evaluadorContenido = new();
    private readonly EvaluadorRafagaDistribuida _evaluadorRafaga = new();

    public CargadorPoliticasDesdeConfiguracionTests()
    {
        // Por defecto no hay nada configurado; cada prueba siembra lo que necesita.
        _configuracion.ListarEventosAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<EventoPersistido>());
        _configuracion.ListarGruposAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<GrupoPersistido>());
        _reglasContenido.ListarPorServidorAsync(
                Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ReglaContenidoPersistida>());
    }

    private CargadorPoliticasDesdeConfiguracion CrearCargador() => new(
        _configuracion, _reglasContenido, _evaluadorContenido, _evaluadorRafaga,
        NullLogger<CargadorPoliticasDesdeConfiguracion>.Instance);

    private void Sembrar(
        EventoPersistido evento,
        GrupoPersistido grupo,
        params ReglaContenidoPersistida[] reglas)
    {
        _configuracion.ListarEventosAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new[] { evento });
        _configuracion.ListarGruposAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns(new[] { grupo });
        _reglasContenido.ListarPorServidorAsync(
                Arg.Any<Snowflake>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(reglas);
    }

    [Fact]
    public async Task Sin_eventos_devuelve_lista_vacia()
    {
        var politicas = await CrearCargador().CargarAsync(Servidor);

        politicas.Should().BeEmpty();
    }

    [Fact]
    public async Task Materializa_un_evento_con_composicion_grupos_reglas_y_acciones()
    {
        // Given un evento en ejecución con combinación "alguno", un grupo "alguna" con una regla de
        // contenido por palabras clave (id 100) y dos acciones (reportar, luego banear) (CU-11, RN-05).
        var regla = new ReglaContenidoPersistida(
            100, "Palabrota",
            ReglaContenido.PorPalabrasClave("Palabrota", "baneame", TimeSpan.FromMilliseconds(100)));
        var grupo = new GrupoPersistido(
            10, "Grupo palabras", "Alguna", null,
            new[] { new ReglaDeGrupo("contenido", 100, null) });
        var evento = new EventoPersistido(
            1, "Palabras prohibidas", 0, false, "ejecucion", "alguno", new[] { 10 },
            new[]
            {
                new AccionPersistida("ReportarACanalPrivado", 0, null, null, null),
                new AccionPersistida("BaneoConBorradoRetroactivo", 1, 1, null, null),
            });
        Sembrar(evento, grupo, regla);

        // When se cargan las políticas del servidor.
        var politicas = await CrearCargador().CargarAsync(Servidor);

        // Then hay una política con la composición y las acciones declaradas (mapeo completo).
        politicas.Should().ContainSingle();
        var politica = politicas[0];
        politica.Nombre.Should().Be("Palabras prohibidas");
        politica.Prioridad.Should().Be(0);
        politica.Modo.Should().Be(Modo.Ejecucion);
        politica.Continuar.Should().BeFalse();
        politica.TieneComposicion.Should().BeTrue();
        politica.Acciones.Should().HaveCount(2);
        politica.Acciones[0].Tipo.Should().Be(TipoAccion.ReportarACanalPrivado);
        politica.Acciones[1].Tipo.Should().Be(TipoAccion.BaneoConBorradoRetroactivo);
        politica.Acciones[1].VentanaBorradoEfectivaDias.Should().Be(1);
    }

    [Fact]
    public async Task La_regla_de_contenido_se_conecta_por_id_y_la_composicion_evalua_el_texto()
    {
        // Given la misma configuración por palabras clave ("baneame").
        var regla = new ReglaContenidoPersistida(
            100, "Palabrota",
            ReglaContenido.PorPalabrasClave("Palabrota", "baneame", TimeSpan.FromMilliseconds(100)));
        var grupo = new GrupoPersistido(
            10, "Grupo palabras", "Alguna", null,
            new[] { new ReglaDeGrupo("contenido", 100, null) });
        var evento = new EventoPersistido(
            1, "Palabras prohibidas", 0, false, "ejecucion", "alguno", new[] { 10 },
            new[] { new AccionPersistida("BaneoConBorradoRetroactivo", 0, 1, null, null) });
        Sembrar(evento, grupo, regla);

        var politica = (await CrearCargador().CargarAsync(Servidor)).Single();

        // Then la composición dispara con un mensaje que contiene la palabra y NO con uno que no.
        politica.Composicion!.Evaluar(ContextoCon("decí baneame ahora")).Should().BeTrue();
        politica.Composicion!.Evaluar(ContextoCon("un mensaje sano")).Should().BeFalse();
    }

    [Fact]
    public async Task Mapea_los_parametros_de_timeout_y_de_rol_de_cada_accion()
    {
        var regla = new ReglaContenidoPersistida(
            100, "X", ReglaContenido.PorPalabrasClave("X", "spam", TimeSpan.FromMilliseconds(100)));
        var grupo = new GrupoPersistido(
            10, "G", "Alguna", null, new[] { new ReglaDeGrupo("contenido", 100, null) });
        var evento = new EventoPersistido(
            1, "Con parámetros", 0, false, "ejecucion", "todos", new[] { 10 },
            new[]
            {
                new AccionPersistida("Timeout", 0, null, 15, null),
                new AccionPersistida("AsignarRol", 1, null, null, "700000000000000007"),
            });
        Sembrar(evento, grupo, regla);

        var politica = (await CrearCargador().CargarAsync(Servidor)).Single();

        var timeout = politica.Acciones.Single(a => a.Tipo == TipoAccion.Timeout);
        timeout.DuracionTimeoutEfectiva.Should().Be(TimeSpan.FromMinutes(15));
        var rol = politica.Acciones.Single(a => a.Tipo == TipoAccion.AsignarRol);
        rol.RolObjetivo.Should().Be(new Snowflake("700000000000000007"));
    }

    [Fact]
    public async Task Un_evento_sin_grupos_con_reglas_validas_se_omite()
    {
        // Given un grupo que referencia una regla de contenido inexistente (id 999): el grupo se
        // queda sin reglas materializables y el evento sin grupos válidos (RC-04, ADR-08).
        var grupo = new GrupoPersistido(
            10, "Grupo roto", "Alguna", null,
            new[] { new ReglaDeGrupo("contenido", 999, null) });
        var evento = new EventoPersistido(
            1, "Evento roto", 0, false, "ejecucion", "todos", new[] { 10 },
            new[] { new AccionPersistida("BaneoConBorradoRetroactivo", 0, 1, null, null) });
        Sembrar(evento, grupo);

        var politicas = await CrearCargador().CargarAsync(Servidor);

        // Then no se materializa ninguna política, pero no se lanza: el pipeline sigue sano.
        politicas.Should().BeEmpty();
    }

    [Fact]
    public async Task Una_regla_de_conducta_se_materializa_como_politica_con_composicion()
    {
        // Given un evento cuyo grupo trae solo la regla de conducta del catálogo (ráfaga distribuida).
        var grupo = new GrupoPersistido(
            10, "Grupo conducta", "Alguna", null,
            new[] { new ReglaDeGrupo("conducta", null, "rafaga-distribuida") });
        var evento = new EventoPersistido(
            1, "Antiráfaga", 0, false, "ejecucion", "todos", new[] { 10 },
            new[] { new AccionPersistida("BaneoConBorradoRetroactivo", 0, 1, null, null) });
        Sembrar(evento, grupo);

        var politicas = await CrearCargador().CargarAsync(Servidor);

        // Then la política se crea con composición (no se descarta por no tener reglas de contenido).
        politicas.Should().ContainSingle();
        politicas[0].TieneComposicion.Should().BeTrue();
    }

    private static ContextoEvaluacionRegla ContextoCon(string contenido) =>
        new(MensajesDePrueba.Crear("300000000000000001", Base, contenido: contenido),
            new EstadoConductaEnMemoria(), Base);
}
