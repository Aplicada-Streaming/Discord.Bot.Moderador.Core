using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la persistencia normalizada del incidente (R2, RN-11): un
/// incidente ejecutado se persiste con sus filas hijas de mensajes accionados y canales
/// afectados, y se recupera con la evidencia completa tras la remoción (TC-50, modelo-datos-
/// logico §2.12, §2.13). Ejercita las migraciones MIG-0001 y MIG-0002 sobre una base SQLite
/// en archivo temporal, que confirma que el esquema es reconstruible desde las migraciones.
/// </summary>
public sealed class PersistenciaIncidenteTests : IDisposable
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaIncidenteTests()
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
    public async Task Incidente_ejecutado_persiste_con_mensajes_y_canales_normalizados()
    {
        // Given una base recién migrada (MIG-0001 + MIG-0002).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var incidente = new Incidente(
            new Snowflake("100000000000000001"),
            new Snowflake("200000000000000002"),
            "Ráfaga distribuida",
            Modo.Ejecucion,
            TipoAccion.BaneoConBorradoRetroactivo,
            ResultadoModeracion.Ejecutada,
            new[]
            {
                new MensajeAccionado(
                    new Snowflake("400000000000000001"), new Snowflake("300000000000000001"), "spam 1"),
                new MensajeAccionado(
                    new Snowflake("400000000000000002"), new Snowflake("300000000000000002"), "spam 2"),
            },
            new[] { new Snowflake("300000000000000001"), new Snowflake("300000000000000002") },
            Base);

        // When se persiste el incidente como unidad confirmada (RN-11).
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioIncidentes(contexto);
            await repositorio.AgregarAsync(incidente);
        }

        // Then se recupera con la copia de mensajes y los canales afectados normalizados.
        await using (var contexto = CrearContexto())
        {
            // Las tablas hijas tienen filas propias (no JSON embebido).
            (await contexto.MensajesAccionados.CountAsync()).Should().Be(2);
            (await contexto.CanalesAfectados.CountAsync()).Should().Be(2);

            var repositorio = new RepositorioIncidentes(contexto);
            var incidentes = await repositorio.ListarAsync();

            incidentes.Should().ContainSingle();
            var recuperado = incidentes[0];
            recuperado.Resultado.Should().Be(ResultadoModeracion.Ejecutada);
            recuperado.MensajesAccionados.Should().HaveCount(2);
            recuperado.MensajesAccionados.Select(m => m.ContenidoCopiado)
                .Should().BeEquivalentTo("spam 1", "spam 2");
            recuperado.CanalesAfectados.Select(c => c.Valor)
                .Should().BeEquivalentTo("300000000000000001", "300000000000000002");
        }
    }

    [Fact]
    public async Task Incidente_persiste_y_recupera_el_nombre_del_canal_del_mensaje()
    {
        // Given una base migrada (incluida MIG-0008 que agrega NombreCanal a MensajeAccionado).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var incidente = new Incidente(
            new Snowflake("100000000000000001"),
            new Snowflake("200000000000000002"),
            "Palabras prohibidas",
            Modo.Ejecucion,
            TipoAccion.BaneoConBorradoRetroactivo,
            ResultadoModeracion.Ejecutada,
            new[]
            {
                new MensajeAccionado(
                    new Snowflake("400000000000000001"), new Snowflake("300000000000000001"),
                    "dijo baneame", "general"),
            },
            new[] { new Snowflake("300000000000000001") },
            Base);

        await using (var contexto = CrearContexto())
        {
            await new RepositorioIncidentes(contexto).AgregarAsync(incidente);
        }

        // Then el nombre del canal vuelve junto con el texto (CU-06), no solo el snowflake.
        await using (var contexto = CrearContexto())
        {
            var recuperado = (await new RepositorioIncidentes(contexto).ListarAsync()).Single();
            var mensaje = recuperado.MensajesAccionados.Single();
            mensaje.ContenidoCopiado.Should().Be("dijo baneame");
            mensaje.NombreCanal.Should().Be("general");
            mensaje.CanalId.Valor.Should().Be("300000000000000001");
        }
    }

    [Fact]
    public async Task Servidor_persiste_y_recupera_su_canal_de_salida()
    {
        // Given una base migrada y un servidor con canal de salida designado (CU-05).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var canal = new CanalDeSalida(
            new Snowflake("500000000000000001"), CanalDeSalida.PropositoReporteIncidentes);
        var servidor = new ServidorRegistrado(
            new Snowflake("100000000000000001"),
            "token-cifrado",
            canalDeSalida: canal);

        // When se persiste.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioServidores(contexto);
            await repositorio.AgregarAsync(servidor);
        }

        // Then se recupera con su canal de salida (snowflake + propósito lógico).
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioServidores(contexto);
            var recuperado = await repositorio.ObtenerAsync(new Snowflake("100000000000000001"));

            recuperado.Should().NotBeNull();
            recuperado!.CanalDeSalida.Should().NotBeNull();
            recuperado.CanalDeSalida!.SnowflakeCanal.Valor.Should().Be("500000000000000001");
            recuperado.CanalDeSalida.PropositoLogico.Should().Be(CanalDeSalida.PropositoReporteIncidentes);
        }
    }

    [Fact]
    public async Task Eliminar_todos_borra_los_incidentes_y_su_evidencia_en_cascada()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var incidente = new Incidente(
            new Snowflake("100000000000000001"),
            new Snowflake("200000000000000002"),
            "Ráfaga distribuida",
            Modo.Ejecucion,
            TipoAccion.BaneoConBorradoRetroactivo,
            ResultadoModeracion.Ejecutada,
            new[]
            {
                new MensajeAccionado(
                    new Snowflake("400000000000000001"), new Snowflake("300000000000000001"), "spam 1"),
                new MensajeAccionado(
                    new Snowflake("400000000000000002"), new Snowflake("300000000000000002"), "spam 2"),
            },
            new[] { new Snowflake("300000000000000001"), new Snowflake("300000000000000002") },
            Base);

        await using (var contexto = CrearContexto())
        {
            await new RepositorioIncidentes(contexto).AgregarAsync(incidente);
        }

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioIncidentes(contexto);
            var eliminados = await repositorio.EliminarTodosAsync();
            eliminados.Should().Be(1);
        }

        await using (var contexto = CrearContexto())
        {
            // El incidente y su evidencia (mensajes y canales) desaparecen en cascada (RN-11).
            (await contexto.Incidentes.CountAsync()).Should().Be(0);
            (await contexto.MensajesAccionados.CountAsync()).Should().Be(0);
            (await contexto.CanalesAfectados.CountAsync()).Should().Be(0);
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
