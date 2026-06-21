using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Administracion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la persistencia del administrador y de los campos de reversión del
/// incidente (R4, RC-06, RN-13, CU-07). Ejercita las migraciones MIG-0001..MIG-0004 sobre una
/// base SQLite en archivo temporal, confirmando que el esquema (tabla Administrador + columnas de
/// reversión) es reconstruible desde las migraciones (MIG-0004 aplica).
/// </summary>
public sealed class PersistenciaAdministradorTests : IDisposable
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaAdministradorTests()
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
    public async Task Administrador_persiste_con_su_resguardo_y_identificador_unico()
    {
        // Given una base recién migrada (MIG-0001..MIG-0004 aplican).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var administrador = new Administrador("admin", "$pbkdf2-sha256$i=1000$c2FsdA==$aGFzaA==", Base);

        // When se persiste.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioAdministrador(contexto);
            await repositorio.AgregarAsync(administrador);
        }

        // Then se recupera con el identificador y el resguardo PHC (nunca en claro, RN-13).
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioAdministrador(contexto);
            (await repositorio.ExisteAsync()).Should().BeTrue();
            var recuperado = await repositorio.ObtenerAsync();
            recuperado.Should().NotBeNull();
            recuperado!.IdentificadorCuenta.Should().Be("admin");
            recuperado.ResguardoPassword.Should().StartWith("$pbkdf2-sha256$");
            recuperado.Id.Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public async Task El_repositorio_rechaza_un_segundo_administrador()
    {
        // Given una base migrada con un administrador ya persistido (unicidad, RC-06).
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioAdministrador(contexto);
            await repositorio.AgregarAsync(new Administrador("admin", "$pbkdf2-sha256$i=1$c2FsdA==$aGFzaA==", Base));
        }

        // When se intenta persistir un segundo administrador.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioAdministrador(contexto);
            var agregarSegundo = async () =>
                await repositorio.AgregarAsync(new Administrador("otro", "$pbkdf2-sha256$i=1$c2FsdA==$aGFzaA==", Base));

            // Then se rechaza (a lo sumo una cuenta, RC-06).
            await agregarSegundo.Should().ThrowAsync<InvalidOperationException>();
        }
    }

    [Fact]
    public async Task Los_campos_de_reversion_del_incidente_persisten()
    {
        // Given una base migrada y un incidente de baneo ejecutado (CU-07).
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
            Array.Empty<MensajeAccionado>(),
            new[] { new Snowflake("300000000000000001") },
            Base);

        int incidenteId;
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioIncidentes(contexto);
            await repositorio.AgregarAsync(incidente);
            var listados = await repositorio.ListarAsync();
            incidenteId = listados[0].Id;
            // Al inicio no hay reversión registrada.
            listados[0].FueRevertido.Should().BeFalse();
            listados[0].PuedeRevertirse.Should().BeTrue();
        }

        // When se marca el incidente como revertido con autor y fecha (CU-07).
        var fechaReversion = Base.AddHours(1);
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioIncidentes(contexto);
            (await repositorio.MarcarRevertidoAsync(incidenteId, administradorId: 42, fechaReversion))
                .Should().BeTrue();
        }

        // Then la reversión persiste (quién y cuándo); el incidente ya no es reversible.
        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioIncidentes(contexto);
            var recuperado = await repositorio.ObtenerAsync(incidenteId);
            recuperado.Should().NotBeNull();
            recuperado!.FueRevertido.Should().BeTrue();
            recuperado.ReversionAutorId.Should().Be(42);
            recuperado.ReversionFecha.Should().Be(fechaReversion);
            recuperado.PuedeRevertirse.Should().BeFalse();
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
