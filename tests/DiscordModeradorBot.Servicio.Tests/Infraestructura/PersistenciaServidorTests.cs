using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la EDICIÓN de un servidor (CU-10): RepositorioServidores.ActualizarDatosAsync
/// persiste nombre y canal de salida, reemplaza el token solo si se provee uno nuevo (en blanco se
/// conserva, RN-14) y no toca el identificador. Corren sobre SQLite en archivo temporal migrado.
/// </summary>
public sealed class PersistenciaServidorTests : IDisposable
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 0, 0, 0, TimeSpan.Zero);

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaServidorTests()
    {
        _rutaBase = Path.Combine(Path.GetTempPath(), $"dmb-servidor-{Guid.NewGuid():N}.db");
        _cadenaConexion = new SqliteConnectionStringBuilder { DataSource = _rutaBase }.ToString();
    }

    private ContextoPersistencia CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<ContextoPersistencia>()
            .UseSqlite(_cadenaConexion)
            .Options;
        return new ContextoPersistencia(opciones);
    }

    private async Task SembrarServidorAsync(Snowflake servidorId, string tokenCifrado, CanalDeSalida? canal)
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioServidores(contexto);
            await repo.AgregarAsync(new ServidorRegistrado(
                servidorId, tokenCifrado, EstadoConexion.Desconectado, EstadoActivacion.Inactivo,
                "Nombre viejo", Base, canal));
        }
    }

    [Fact]
    public async Task Actualizar_datos_persiste_nombre_y_canal_y_conserva_el_token_si_no_se_provee()
    {
        var servidorId = new Snowflake("100000000000000001");
        await SembrarServidorAsync(servidorId, "token-cifrado-original", canal: null);

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioServidores(contexto);
            var ok = await repo.ActualizarDatosAsync(
                servidorId, "Nombre nuevo", tokenCifrado: null,
                new CanalDeSalida(new Snowflake("400000000000000001"), CanalDeSalida.PropositoReporteIncidentes));
            ok.Should().BeTrue();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioServidores(contexto);
            var s = await repo.ObtenerAsync(servidorId);
            s!.NombreDescriptivo.Should().Be("Nombre nuevo");
            s.CanalDeSalida!.SnowflakeCanal.Valor.Should().Be("400000000000000001");
            // El token en blanco conserva el actual (RN-14).
            s.TokenCifrado.Should().Be("token-cifrado-original");
        }
    }

    [Fact]
    public async Task Actualizar_datos_reemplaza_el_token_y_puede_quitar_el_canal()
    {
        var servidorId = new Snowflake("100000000000000001");
        await SembrarServidorAsync(
            servidorId, "token-original",
            new CanalDeSalida(new Snowflake("500000000000000001"), CanalDeSalida.PropositoReporteIncidentes));

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioServidores(contexto);
            var ok = await repo.ActualizarDatosAsync(
                servidorId, "Nombre nuevo", tokenCifrado: "token-cifrado-nuevo", canalDeSalida: null);
            ok.Should().BeTrue();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioServidores(contexto);
            var s = await repo.ObtenerAsync(servidorId);
            s!.TokenCifrado.Should().Be("token-cifrado-nuevo");
            s.CanalDeSalida.Should().BeNull(); // vaciar el canal lo quita
        }
    }

    [Fact]
    public async Task Actualizar_un_servidor_inexistente_devuelve_false()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioServidores(contexto);
            (await repo.ActualizarDatosAsync(new Snowflake("999"), "x", null, null)).Should().BeFalse();
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
