using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Tests.Infraestructura;

/// <summary>
/// Pruebas de integración de la BAJA de un servidor (CU-10). RepositorioServidores.EliminarAsync
/// borra el servidor y TODA su configuración —reglas de contenido, exenciones, grupos con sus
/// reglas (cascada GrupoDeReglas→GrupoRegla) y eventos con sus grupos y acciones (cascada
/// Evento→EventoGrupo/Accion)— en una sola transacción, y CONSERVA los incidentes como historial
/// de auditoría (RN-11). La configuración de otros servidores no se toca. Corren sobre SQLite en
/// archivo temporal migrado, como las demás pruebas de persistencia (estrategia-testing §7).
/// </summary>
public sealed class PersistenciaBajaServidorTests : IDisposable
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 0, 0, 0, TimeSpan.Zero);

    private const string ServidorA = "100000000000000001";
    private const string ServidorB = "100000000000000002";

    private readonly string _rutaBase;
    private readonly string _cadenaConexion;

    public PersistenciaBajaServidorTests()
    {
        _rutaBase = Path.Combine(Path.GetTempPath(), $"dmb-baja-servidor-{Guid.NewGuid():N}.db");
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
            SembrarServidorConConfiguracionEIncidente(contexto, ServidorA);
            SembrarServidorConConfiguracionEIncidente(contexto, ServidorB);
            await contexto.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Da de alta un servidor con una pieza de cada parte de su configuración (regla, exención,
    /// grupo con una regla, evento con un grupo y una acción) y un incidente del servidor.
    /// </summary>
    private static void SembrarServidorConConfiguracionEIncidente(ContextoPersistencia contexto, string servidor)
    {
        contexto.Servidores.Add(new ServidorRegistradoEntidad
        {
            SnowflakeServidor = servidor,
            TokenCifrado = "cifrado",
            EstadoConexion = "desconectado",
            EstadoActivacion = "inactivo",
            NombreDescriptivo = "Servidor de prueba",
            CreadoEn = Base,
        });

        contexto.ReglasContenido.Add(new ReglaContenidoEntidad
        {
            SnowflakeServidor = servidor,
            Nombre = "Sin insultos",
            TipoCriterio = "ExpresionRegular",
            Criterio = "patron",
            SensibleAMayusculas = false,
            NombrePolitica = "Política",
        });

        contexto.Exenciones.Add(new ExencionEntidad
        {
            SnowflakeServidor = servidor,
            TipoSujeto = "usuario",
            SnowflakeSujeto = "300000000000000001",
        });

        contexto.GruposDeReglas.Add(new GrupoDeReglasEntidad
        {
            SnowflakeServidor = servidor,
            Nombre = "Grupo",
            ModoCoincidencia = "alguna",
            Reglas =
            {
                new GrupoReglaEntidad { ClaseRegla = "conducta", ClaveReglaConducta = "rafaga-distribuida" },
            },
        });

        contexto.Eventos.Add(new EventoEntidad
        {
            SnowflakeServidor = servidor,
            Nombre = "Evento",
            Prioridad = 1,
            Continuar = false,
            Modo = "simulacion",
            ModoCombinacionGrupos = "todos",
            Grupos = { new EventoGrupoEntidad { GrupoDeReglasId = 0 } },
            Acciones = { new AccionEntidad { Tipo = "reportar", OrdenEjecucion = 1 } },
        });

        contexto.Incidentes.Add(new IncidenteEntidad
        {
            ServidorId = servidor,
            UsuarioId = "300000000000000099",
            NombrePolitica = "Política",
            Modo = "ejecucion",
            Accion = "BaneoConBorradoRetroactivo",
            Resultado = "Ejecutada",
            Instante = Base.AddDays(-1),
        });
    }

    [Fact]
    public async Task Eliminar_borra_servidor_y_su_configuracion_y_conserva_incidentes()
    {
        await SembrarAsync();

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioServidores(contexto);
            var eliminado = await repositorio.EliminarAsync(new Snowflake(ServidorA));
            eliminado.Should().BeTrue();
        }

        await using (var contexto = CrearContexto())
        {
            // El servidor A y toda su configuración desaparecieron...
            (await contexto.Servidores.AnyAsync(s => s.SnowflakeServidor == ServidorA)).Should().BeFalse();
            (await contexto.ReglasContenido.AnyAsync(r => r.SnowflakeServidor == ServidorA)).Should().BeFalse();
            (await contexto.Exenciones.AnyAsync(x => x.SnowflakeServidor == ServidorA)).Should().BeFalse();
            (await contexto.GruposDeReglas.AnyAsync(g => g.SnowflakeServidor == ServidorA)).Should().BeFalse();
            (await contexto.Eventos.AnyAsync(e => e.SnowflakeServidor == ServidorA)).Should().BeFalse();

            // ...incluidos los hijos en cascada (solo quedan los del servidor B).
            (await contexto.GruposRegla.CountAsync()).Should().Be(1);
            (await contexto.EventosGrupo.CountAsync()).Should().Be(1);
            (await contexto.Acciones.CountAsync()).Should().Be(1);

            // Los incidentes del servidor A se CONSERVAN como historial (RN-11).
            (await contexto.Incidentes.CountAsync(i => i.ServidorId == ServidorA)).Should().Be(1);

            // El servidor B y su configuración quedan intactos.
            (await contexto.Servidores.AnyAsync(s => s.SnowflakeServidor == ServidorB)).Should().BeTrue();
            (await contexto.ReglasContenido.CountAsync(r => r.SnowflakeServidor == ServidorB)).Should().Be(1);
            (await contexto.Exenciones.CountAsync(x => x.SnowflakeServidor == ServidorB)).Should().Be(1);
            (await contexto.GruposDeReglas.CountAsync(g => g.SnowflakeServidor == ServidorB)).Should().Be(1);
            (await contexto.Eventos.CountAsync(e => e.SnowflakeServidor == ServidorB)).Should().Be(1);
            (await contexto.Incidentes.CountAsync(i => i.ServidorId == ServidorB)).Should().Be(1);
        }
    }

    [Fact]
    public async Task Eliminar_servidor_inexistente_devuelve_false()
    {
        await using (var contexto = CrearContexto())
        {
            await contexto.Database.MigrateAsync();
        }

        await using (var contexto = CrearContexto())
        {
            var repositorio = new RepositorioServidores(contexto);
            var eliminado = await repositorio.EliminarAsync(new Snowflake("100000000000000099"));
            eliminado.Should().BeFalse();
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
