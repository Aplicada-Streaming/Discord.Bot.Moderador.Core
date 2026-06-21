using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la persistencia de reglas de contenido (R3, CU-04): una regla
/// validada (RN-03) se persiste y se recupera recompilando su criterio con su tope de tiempo
/// (ADR-08). Ejercita las migraciones MIG-0001, MIG-0002 y MIG-0003 sobre una base SQLite en
/// archivo temporal, confirmando que el esquema es reconstruible desde las migraciones.
/// </summary>
public sealed class PersistenciaReglaContenidoTests : IDisposable
{
    private static readonly TimeSpan Tope = TimeSpan.FromMilliseconds(100);

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaReglaContenidoTests()
    {
        _rutaBase = Path.Combine(Path.GetTempPath(), $"dmb-test-{Guid.NewGuid():N}.db");
        _cadenaConexion = new SqliteConnectionStringBuilder { DataSource = _rutaBase }.ToString();
    }

    private ContextoPersistencia CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<ContextoPersistencia>()
            .UseSqlite(_cadenaConexion)
            .Options;
        return new ContextoPersistencia(opciones);
    }

    [Fact]
    public async Task Regla_de_contenido_persiste_y_se_recupera_recompilada()
    {
        // Given una base recién migrada (MIG-0001 + MIG-0002 + MIG-0003).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        var regla = ReglaContenido.PorExpresionRegular(
            "Enlace de acortador", @"https?://(?:bit\.ly|tinyurl\.com)/\S+", Tope);

        // When se persiste la regla asociada a una política.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioReglasContenido(contexto);
            await repositorio.AgregarAsync(servidorId, "Contenido prohibido", regla);
        }

        // Then la tabla Regla tiene la fila y se recupera con su criterio y clase, ya recompilada.
        await using (var contexto = CrearContexto())
        {
            (await contexto.ReglasContenido.CountAsync()).Should().Be(1);

            var repositorio = new RepositorioReglasContenido(contexto);
            var recuperadas = await repositorio.ListarPorServidorAsync(servidorId, Tope);

            recuperadas.Should().ContainSingle();
            var persistida = recuperadas[0];
            persistida.NombrePolitica.Should().Be("Contenido prohibido");
            persistida.Regla.Nombre.Should().Be("Enlace de acortador");
            persistida.Regla.TipoCriterio.Should().Be(TipoCriterioContenido.ExpresionRegular);
            persistida.Regla.Criterio.Should().Be(@"https?://(?:bit\.ly|tinyurl\.com)/\S+");

            // La regla recuperada es funcional: coincide contra un mensaje con contenido prohibido.
            var evaluador = new EvaluadorReglaContenido();
            evaluador.Evaluar("oferta https://bit.ly/abc ya", persistida.Regla)
                .Coincide.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Regla_por_palabras_clave_persiste_y_se_recupera_con_su_clase_y_funcional()
    {
        // Given una base migrada y una regla por PALABRAS CLAVE (CU-04, PalabrasClave).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        var regla = ReglaContenido.PorPalabrasClave("Insultos", "idiota, tarado", Tope);

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioReglasContenido(contexto);
            await repositorio.AgregarAsync(servidorId, "Contenido prohibido", regla);
        }

        // Then se recupera reconstruida según su clase (no como expresión regular) y es funcional.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioReglasContenido(contexto);
            var recuperadas = await repositorio.ListarPorServidorAsync(servidorId, Tope);

            recuperadas.Should().ContainSingle();
            var persistida = recuperadas[0];
            persistida.Regla.TipoCriterio.Should().Be(TipoCriterioContenido.PalabrasClave);
            persistida.Regla.Criterio.Should().Be("idiota\ntarado");

            var evaluador = new EvaluadorReglaContenido();
            evaluador.Evaluar("sos un tarado", persistida.Regla).Coincide.Should().BeTrue();
            evaluador.Evaluar("mensaje cordial", persistida.Regla).Coincide.Should().BeFalse();
        }
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
