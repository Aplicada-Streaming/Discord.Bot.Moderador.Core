using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la CONSULTA FILTRADA Y PAGINADA de incidentes (CU-06 5.A): el
/// repositorio aplica los predicados (servidor, modo, resultado, usuario, rango de fechas) y la
/// paginación EN LA CONSULTA a la base, no en memoria, y devuelve la página más el total del filtro.
/// Corren sobre una base SQLite en archivo temporal migrada (MIG-0001..MIG-0004), como las pruebas de
/// persistencia existentes (estrategia-testing §7).
/// </summary>
public sealed class BusquedaIncidentesTests : IDisposable
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string ServidorA = "100000000000000001";
    private const string ServidorB = "100000000000000002";
    private const string UsuarioX = "200000000000000010";
    private const string UsuarioY = "200000000000000020";

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public BusquedaIncidentesTests()
    {
        _rutaBase = Path.Combine(Path.GetTempPath(), $"dmb-busqueda-{Guid.NewGuid():N}.db");
        _cadenaConexion = new SqliteConnectionStringBuilder { DataSource = _rutaBase }.ToString();
    }

    private ContextoPersistencia CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<ContextoPersistencia>()
            .UseSqlite(_cadenaConexion)
            .Options;
        return new ContextoPersistencia(opciones);
    }

    private async Task SembrarAsync()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioIncidentes(contexto);

            // Servidor A: una ejecución real de baneo de UsuarioX y una simulación de reporte de UsuarioY.
            await repositorio.AgregarAsync(CrearIncidente(
                ServidorA, UsuarioX, Modo.Ejecucion, TipoAccion.BaneoConBorradoRetroactivo,
                ResultadoModeracion.Ejecutada, Base.AddDays(-1)));
            await repositorio.AgregarAsync(CrearIncidente(
                ServidorA, UsuarioY, Modo.Simulacion, TipoAccion.ReportarACanalPrivado,
                ResultadoModeracion.Simulada, Base.AddDays(-3)));

            // Servidor B: una simulación de UsuarioX (más reciente) y una no accionable de UsuarioY.
            await repositorio.AgregarAsync(CrearIncidente(
                ServidorB, UsuarioX, Modo.Simulacion, TipoAccion.BaneoConBorradoRetroactivo,
                ResultadoModeracion.Simulada, Base));
            await repositorio.AgregarAsync(CrearIncidente(
                ServidorB, UsuarioY, Modo.Ejecucion, TipoAccion.Expulsar,
                ResultadoModeracion.NoAccionable, Base.AddDays(-5)));
        }
    }

    private static Incidente CrearIncidente(
        string servidor, string usuario, Modo modo, TipoAccion accion,
        ResultadoModeracion resultado, DateTimeOffset instante) =>
        new(
            new Snowflake(servidor),
            new Snowflake(usuario),
            "Ráfaga distribuida",
            modo,
            accion,
            resultado,
            Array.Empty<MensajeAccionado>(),
            Array.Empty<Snowflake>(),
            instante);

    [Fact]
    public async Task Sin_filtros_devuelve_todos_ordenados_por_fecha_descendente()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(new FiltroIncidentes());

        pagina.Total.Should().Be(4);
        pagina.Incidentes.Should().HaveCount(4);
        // Orden por fecha descendente: el de Base (ServidorB/UsuarioX) primero.
        pagina.Incidentes[0].Instante.Should().Be(Base);
        pagina.Incidentes.Select(i => i.Instante).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Filtra_por_servidor()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(new FiltroIncidentes { Servidor = new Snowflake(ServidorA) });

        pagina.Total.Should().Be(2);
        pagina.Incidentes.Should().OnlyContain(i => i.ServidorId.Valor == ServidorA);
    }

    [Fact]
    public async Task Filtra_por_modo()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(new FiltroIncidentes { Modo = Modo.Ejecucion });

        pagina.Total.Should().Be(2);
        pagina.Incidentes.Should().OnlyContain(i => i.Modo == Modo.Ejecucion);
    }

    [Fact]
    public async Task Filtra_por_resultado()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(
            new FiltroIncidentes { Resultado = ResultadoModeracion.NoAccionable });

        pagina.Total.Should().Be(1);
        pagina.Incidentes[0].Resultado.Should().Be(ResultadoModeracion.NoAccionable);
        pagina.Incidentes[0].UsuarioId.Valor.Should().Be(UsuarioY);
    }

    [Fact]
    public async Task Filtra_por_usuario_como_contiene()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(new FiltroIncidentes { UsuarioTexto = UsuarioX });

        pagina.Total.Should().Be(2);
        pagina.Incidentes.Should().OnlyContain(i => i.UsuarioId.Valor == UsuarioX);
    }

    [Fact]
    public async Task Filtra_por_rango_de_fechas_inclusivo()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        // Rango que cubre [Base-3d, Base-1d]: deja fuera el de Base y el de Base-5d.
        var pagina = await repositorio.BuscarAsync(new FiltroIncidentes
        {
            Desde = Base.AddDays(-3),
            Hasta = Base.AddDays(-1),
        });

        pagina.Total.Should().Be(2);
        pagina.Incidentes.Should().OnlyContain(i =>
            i.Instante >= Base.AddDays(-3) && i.Instante <= Base.AddDays(-1));
    }

    [Fact]
    public async Task Combina_varios_filtros()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(new FiltroIncidentes
        {
            Servidor = new Snowflake(ServidorA),
            Modo = Modo.Ejecucion,
            UsuarioTexto = UsuarioX,
        });

        pagina.Total.Should().Be(1);
        pagina.Incidentes[0].ServidorId.Valor.Should().Be(ServidorA);
        pagina.Incidentes[0].UsuarioId.Valor.Should().Be(UsuarioX);
        pagina.Incidentes[0].Modo.Should().Be(Modo.Ejecucion);
    }

    [Fact]
    public async Task Pagina_en_la_consulta_devuelve_solo_la_pagina_pedida_y_el_total()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina1 = await repositorio.BuscarAsync(new FiltroIncidentes { Pagina = 1, TamanoPagina = 2 });
        var pagina2 = await repositorio.BuscarAsync(new FiltroIncidentes { Pagina = 2, TamanoPagina = 2 });

        // El total es el del filtro completo (4), aunque cada página traiga solo 2.
        pagina1.Total.Should().Be(4);
        pagina2.Total.Should().Be(4);
        pagina1.Incidentes.Should().HaveCount(2);
        pagina2.Incidentes.Should().HaveCount(2);

        // Las dos páginas no se solapan (orden estable por fecha desc + id desc).
        var ids1 = pagina1.Incidentes.Select(i => i.Id).ToList();
        var ids2 = pagina2.Incidentes.Select(i => i.Id).ToList();
        ids1.Should().NotIntersectWith(ids2);
    }

    [Fact]
    public async Task Sin_resultados_devuelve_total_cero_y_pagina_vacia()
    {
        await SembrarAsync();

        await using var contexto = CrearContexto();
        var repositorio = new RepositorioIncidentes(contexto);

        var pagina = await repositorio.BuscarAsync(
            new FiltroIncidentes { UsuarioTexto = "999999999999999999" });

        pagina.Total.Should().Be(0);
        pagina.Incidentes.Should().BeEmpty();
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        TryDelete(_rutaBase);
        TryDelete(_rutaBase + "-wal");
        TryDelete(_rutaBase + "-shm");
    }

    private static void TryDelete(string ruta)
    {
        try
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
        catch (IOException)
        {
            // Limpieza best-effort de los temporales de prueba.
        }
    }
}
