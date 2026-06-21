using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la persistencia de exenciones (R5, CU-15, RN-07): una exención se
/// persiste y se recupera; un duplicado por (servidor, tipo, snowflake) no se crea
/// (EXENCION_DUPLICADA); una baja la quita. Ejercita las migraciones MIG-0001..MIG-0005 sobre una
/// base SQLite en archivo temporal, confirmando que el esquema es reconstruible y que MIG-0005
/// aplica.
/// </summary>
public sealed class PersistenciaExencionTests : IDisposable
{
    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaExencionTests()
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
    public async Task Exencion_persiste_y_se_recupera()
    {
        // Given una base recién migrada (MIG-0001..MIG-0005).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        var exencion = Exencion.PorRol(new Snowflake("700000000000000001"));

        // When se persiste la exención.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioExenciones(contexto);
            (await repositorio.AgregarAsync(servidorId, exencion)).Should().BeTrue();
        }

        // Then la tabla Exencion tiene la fila y se recupera con su tipo y snowflake.
        await using (var contexto = CrearContexto())
        {
            (await contexto.Exenciones.CountAsync()).Should().Be(1);

            var repositorio = new RepositorioExenciones(contexto);
            var recuperadas = await repositorio.ListarPorServidorAsync(servidorId);

            recuperadas.Should().ContainSingle();
            recuperadas[0].Tipo.Should().Be(TipoSujetoExento.Rol);
            recuperadas[0].Sujeto.Valor.Should().Be("700000000000000001");
        }
    }

    [Fact]
    public async Task Exencion_duplicada_no_se_crea_y_baja_la_quita()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        var exencion = Exencion.PorUsuario(new Snowflake("200000000000000002"));

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioExenciones(contexto);

            // Primera alta: se crea.
            (await repositorio.AgregarAsync(servidorId, exencion)).Should().BeTrue();
            // Segunda alta del mismo sujeto: NO se crea duplicado (CU-15 EXENCION_DUPLICADA).
            (await repositorio.AgregarAsync(servidorId, exencion)).Should().BeFalse();
        }

        await using (var contexto = CrearContexto())
        {
            (await contexto.Exenciones.CountAsync()).Should().Be(1);

            var repositorio = new RepositorioExenciones(contexto);
            // Baja: la quita y el sujeto vuelve a estar sujeto a la moderación (CU-15 §5.A).
            (await repositorio.QuitarAsync(servidorId, exencion)).Should().BeTrue();
            (await repositorio.ListarPorServidorAsync(servidorId)).Should().BeEmpty();
            // Quitar de nuevo: ya no había nada.
            (await repositorio.QuitarAsync(servidorId, exencion)).Should().BeFalse();
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
