using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Configuracion;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la persistencia de parámetros de moderación por servidor (CU-11,
/// RN-10): umbral y ventana de detección de la ráfaga (CU-01) y ventana de antirrebote (CU-16).
/// Ejercita la migración MIG-0007 (tabla ParametrosServidor) sobre una base SQLite en archivo. Un
/// servidor sin fila trae los valores por defecto del descriptor; al guardar, los valores se
/// normalizan y el upsert mantiene una sola fila por servidor.
/// </summary>
public sealed class PersistenciaParametrosServidorTests : IDisposable
{
    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaParametrosServidorTests()
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
    public async Task Sin_fila_devuelve_los_valores_por_defecto_del_descriptor()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioConfiguracion(contexto);
            var parametros = await repositorio.ObtenerParametrosAsync(new Snowflake("100000000000000001"));

            parametros.UmbralCanalesDistintos.Should().Be(RegistroDescriptores.UmbralCanalesDistintos.ValorPorDefecto);
            parametros.VentanaDeteccionSegundos.Should().Be(RegistroDescriptores.VentanaDeteccionSegundos.ValorPorDefecto);
            parametros.VentanaAntirreboteSegundos.Should().Be(RegistroDescriptores.VentanaAntirreboteSegundos.ValorPorDefecto);
        }
    }

    [Fact]
    public async Task Guarda_y_recupera_los_parametros_y_el_upsert_mantiene_una_sola_fila()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");

        // When se guardan parámetros y luego se actualizan (upsert por servidor).
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioConfiguracion(contexto);
            await repositorio.GuardarParametrosAsync(servidorId, new ParametrosModeracion(2, 6.0, 8.0));
            await repositorio.GuardarParametrosAsync(servidorId, new ParametrosModeracion(4, 10.0, 12.0));
        }

        // Then queda UNA sola fila con los últimos valores.
        await using (var contexto = CrearContexto())
        {
            (await contexto.ParametrosServidor.CountAsync()).Should().Be(1);

            var repositorio = new RepositorioConfiguracion(contexto);
            var parametros = await repositorio.ObtenerParametrosAsync(servidorId);

            parametros.UmbralCanalesDistintos.Should().Be(4);
            parametros.VentanaDeteccionSegundos.Should().Be(10.0);
            parametros.VentanaAntirreboteSegundos.Should().Be(12.0);
        }
    }

    [Fact]
    public async Task Un_valor_fuera_de_limites_se_normaliza_al_persistir()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");

        // When se intenta guardar un umbral por debajo del mínimo del descriptor.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioConfiguracion(contexto);
            await repositorio.GuardarParametrosAsync(servidorId, new ParametrosModeracion(0, 2.0, 2.0));
        }

        // Then el repositorio lo normaliza al valor por defecto (RN-10), no persiste un valor inválido.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioConfiguracion(contexto);
            var parametros = await repositorio.ObtenerParametrosAsync(servidorId);

            parametros.UmbralCanalesDistintos.Should().Be(RegistroDescriptores.UmbralCanalesDistintos.ValorPorDefecto);
        }
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_rutaBase))
        {
            File.Delete(_rutaBase);
        }
    }
}
