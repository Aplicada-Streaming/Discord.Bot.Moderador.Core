using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la persistencia del modelo de configuración de R7 (CU-11,
/// modelo-datos-logico §2.7-§2.10): grupos de reglas con su relación grupo-regla (RC-03),
/// eventos/políticas con su relación evento-grupo y sus acciones (RN-05). Ejercitan la nueva
/// migración MIG-0006 sobre una base SQLite en archivo temporal, confirmando que el esquema es
/// reconstruible desde las migraciones, y que eliminar un grupo referenciado se bloquea (CU-11
/// CA-04, RC-03).
/// </summary>
public sealed class PersistenciaConfiguracionTests : IDisposable
{
    private static readonly TimeSpan Tope = TimeSpan.FromMilliseconds(100);

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaConfiguracionTests()
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
    public async Task La_migracion_MIG0006_aplica_y_crea_las_tablas_de_configuracion()
    {
        await using var contexto = CrearContexto();
        await contexto.Database.MigrateAsync();

        // El esquema de configuración existe (no lanza al consultar).
        (await contexto.GruposDeReglas.CountAsync()).Should().Be(0);
        (await contexto.Eventos.CountAsync()).Should().Be(0);
        (await contexto.Acciones.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Grupo_evento_y_acciones_persisten_y_se_recuperan()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        int grupoId;

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);

            // Grupo con dos reglas: una de contenido y una de conducta (RC-03).
            grupoId = await repo.AgregarGrupoAsync(
                servidorId, "Spam distribuido", "almenosn", minimoCoincidencias: 2,
                new[]
                {
                    new ReglaDeGrupo("contenido", ReglaContenidoId: 10, ClaveReglaConducta: null),
                    new ReglaDeGrupo("conducta", ReglaContenidoId: null, ClaveReglaConducta: "rafaga-distribuida"),
                });

            // Evento que referencia el grupo y tiene dos acciones en orden (RN-05).
            await repo.AgregarEventoAsync(
                servidorId, "Corte de spam", prioridad: 1, continuar: false, modo: "ejecucion",
                modoCombinacionGrupos: "todos", gruposIds: new[] { grupoId },
                acciones: new[]
                {
                    new AccionPersistida("ReportarACanalPrivado", 0, null, null, null),
                    new AccionPersistida("BaneoConBorradoRetroactivo", 1, VentanaBorradoDias: 1, null, null),
                });
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);

            var grupos = await repo.ListarGruposAsync(servidorId);
            grupos.Should().ContainSingle();
            grupos[0].ModoCoincidencia.Should().Be("almenosn");
            grupos[0].MinimoCoincidencias.Should().Be(2);
            grupos[0].Reglas.Should().HaveCount(2);

            var eventos = await repo.ListarEventosAsync(servidorId);
            eventos.Should().ContainSingle();
            eventos[0].Nombre.Should().Be("Corte de spam");
            eventos[0].GruposIds.Should().ContainSingle().Which.Should().Be(grupoId);
            eventos[0].Acciones.Should().HaveCount(2);
            // Las acciones vuelven ordenadas por orden de ejecución (RN-05).
            eventos[0].Acciones[0].Tipo.Should().Be("ReportarACanalPrivado");
            eventos[0].Acciones[1].Tipo.Should().Be("BaneoConBorradoRetroactivo");
            eventos[0].Acciones[1].VentanaBorradoDias.Should().Be(1);
        }
    }

    [Fact]
    public async Task Eliminar_un_grupo_referenciado_por_un_evento_se_bloquea()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");

        await using var ctx = CrearContexto();
        var repo = new RepositorioConfiguracion(ctx);

        var grupoId = await repo.AgregarGrupoAsync(
            servidorId, "Grupo referenciado", "alguna", null,
            new[] { new ReglaDeGrupo("conducta", null, "rafaga-distribuida") });

        await repo.AgregarEventoAsync(
            servidorId, "Evento", prioridad: 0, continuar: false, modo: "simulacion",
            modoCombinacionGrupos: "todos", gruposIds: new[] { grupoId },
            acciones: new[] { new AccionPersistida("ReportarACanalPrivado", 0, null, null, null) });

        // CU-11 CA-04 / RC-03: el grupo referenciado no se puede eliminar.
        var eliminado = await repo.EliminarGrupoAsync(grupoId);
        eliminado.Should().BeFalse();
        (await repo.ListarGruposAsync(servidorId)).Should().ContainSingle();
    }

    [Fact]
    public async Task Eliminar_una_regla_de_contenido_no_referenciada_la_borra()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        int reglaId;

        await using (var contexto = CrearContexto())
        {
            var reglaRepo = new RepositorioReglasContenido(contexto);
            await reglaRepo.AgregarAsync(
                servidorId, "Política", ReglaContenido.PorPalabrasClave("Insultos", "idiota", Tope));
            reglaId = await contexto.ReglasContenido.Select(r => r.Id).FirstAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            var eliminada = await repo.EliminarReglaContenidoAsync(reglaId);

            eliminada.Should().BeTrue();
            (await contexto.ReglasContenido.CountAsync()).Should().Be(0);
        }
    }

    [Fact]
    public async Task Eliminar_una_regla_de_contenido_referenciada_por_un_grupo_se_bloquea()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        int reglaId;

        await using (var contexto = CrearContexto())
        {
            var reglaRepo = new RepositorioReglasContenido(contexto);
            await reglaRepo.AgregarAsync(
                servidorId, "Política", ReglaContenido.PorPalabrasClave("Insultos", "idiota", Tope));
            reglaId = await contexto.ReglasContenido.Select(r => r.Id).FirstAsync();

            var repo = new RepositorioConfiguracion(contexto);
            await repo.AgregarGrupoAsync(
                servidorId, "Grupo que la usa", "alguna", null,
                new[] { new ReglaDeGrupo("contenido", reglaId, null) });
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);

            // RC-03: la regla está referenciada por un grupo; primero hay que sacarla del grupo.
            var eliminada = await repo.EliminarReglaContenidoAsync(reglaId);

            eliminada.Should().BeFalse();
            (await contexto.ReglasContenido.CountAsync()).Should().Be(1);
        }
    }

    [Fact]
    public async Task Actualizar_un_grupo_cambia_nombre_modo_y_reemplaza_su_composicion()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        int grupoId;

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            grupoId = await repo.AgregarGrupoAsync(
                servidorId, "Original", "alguna", null,
                new[] { new ReglaDeGrupo("conducta", null, "rafaga-distribuida") });
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            var ok = await repo.ActualizarGrupoAsync(
                grupoId, "Editado", "almenosn", minimoCoincidencias: 2,
                new[]
                {
                    new ReglaDeGrupo("contenido", 10, null),
                    new ReglaDeGrupo("conducta", null, "rafaga-distribuida"),
                });
            ok.Should().BeTrue();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            var grupos = await repo.ListarGruposAsync(servidorId);

            grupos.Should().ContainSingle();
            grupos[0].Nombre.Should().Be("Editado");
            grupos[0].ModoCoincidencia.Should().Be("almenosn");
            grupos[0].MinimoCoincidencias.Should().Be(2);
            grupos[0].Reglas.Should().HaveCount(2);
            // La composición vieja se reemplaza, no se acumula: solo quedan las 2 nuevas.
            (await contexto.GruposRegla.CountAsync()).Should().Be(2);
        }
    }

    [Fact]
    public async Task Eliminar_un_evento_lo_borra_con_sus_grupos_y_acciones_en_cascada()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        var servidorId = new Snowflake("100000000000000001");
        int eventoId;

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            var grupoId = await repo.AgregarGrupoAsync(
                servidorId, "Grupo", "alguna", null,
                new[] { new ReglaDeGrupo("conducta", null, "rafaga-distribuida") });

            eventoId = await repo.AgregarEventoAsync(
                servidorId, "Evento", prioridad: 0, continuar: false, modo: "simulacion",
                modoCombinacionGrupos: "todos", gruposIds: new[] { grupoId },
                acciones: new[]
                {
                    new AccionPersistida("ReportarACanalPrivado", 0, null, null, null),
                    new AccionPersistida("BaneoConBorradoRetroactivo", 1, 1, null, null),
                });
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            var eliminado = await repo.EliminarEventoAsync(eventoId);
            eliminado.Should().BeTrue();
        }

        await using (var contexto = CrearContexto())
        {
            // El evento y sus hijos (relación evento-grupo y acciones) desaparecen; el grupo queda.
            (await contexto.Eventos.CountAsync()).Should().Be(0);
            (await contexto.EventosGrupo.CountAsync()).Should().Be(0);
            (await contexto.Acciones.CountAsync()).Should().Be(0);
            (await contexto.GruposDeReglas.CountAsync()).Should().Be(1);
        }
    }

    [Fact]
    public async Task Eliminar_un_evento_inexistente_devuelve_false()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repo = new RepositorioConfiguracion(contexto);
            (await repo.EliminarEventoAsync(999)).Should().BeFalse();
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
