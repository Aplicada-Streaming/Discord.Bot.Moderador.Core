using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>Repositorio EF Core de servidores registrados (CU-10, ADR-02).</summary>
public sealed class RepositorioServidores : IRepositorioServidores
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioServidores(ContextoPersistencia contexto) => _contexto = contexto;

    public Task<bool> ExisteAsync(Snowflake snowflakeServidor, CancellationToken ct = default)
        => _contexto.Servidores.AnyAsync(s => s.SnowflakeServidor == snowflakeServidor.Valor, ct);

    public async Task AgregarAsync(ServidorRegistrado servidor, CancellationToken ct = default)
    {
        _contexto.Servidores.Add(AMappear(servidor));
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task<ServidorRegistrado?> ObtenerAsync(
        Snowflake snowflakeServidor, CancellationToken ct = default)
    {
        var entidad = await _contexto.Servidores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SnowflakeServidor == snowflakeServidor.Valor, ct);

        return entidad is null ? null : ADominio(entidad);
    }

    public async Task<IReadOnlyList<ServidorRegistrado>> ListarAsync(CancellationToken ct = default)
    {
        var entidades = await _contexto.Servidores.AsNoTracking().ToListAsync(ct);
        return entidades.Select(ADominio).ToList();
    }

    public async Task<bool> ActualizarEstadoAsync(
        Snowflake snowflakeServidor,
        EstadoActivacion estadoActivacion,
        EstadoConexion estadoConexion,
        CancellationToken ct = default)
    {
        var entidad = await _contexto.Servidores
            .FirstOrDefaultAsync(s => s.SnowflakeServidor == snowflakeServidor.Valor, ct);

        if (entidad is null)
        {
            return false;
        }

        entidad.EstadoActivacion = estadoActivacion.ToString().ToLowerInvariant();
        entidad.EstadoConexion = estadoConexion.ToString().ToLowerInvariant();
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> EliminarAsync(Snowflake snowflakeServidor, CancellationToken ct = default)
    {
        var id = snowflakeServidor.Valor;

        var servidor = await _contexto.Servidores
            .FirstOrDefaultAsync(s => s.SnowflakeServidor == id, ct);

        if (servidor is null)
        {
            return false;
        }

        // Se carga la configuración del servidor con sus hijos para que EF los borre en cascada
        // (GrupoDeReglas→GrupoRegla, Evento→EventoGrupo/Accion). Los incidentes NO se cargan ni se
        // borran: se conservan como historial de auditoría (RN-11; se elimina la configuración y se
        // preserva el historial).
        var grupos = await _contexto.GruposDeReglas
            .Include(g => g.Reglas)
            .Where(g => g.SnowflakeServidor == id)
            .ToListAsync(ct);

        var eventos = await _contexto.Eventos
            .Include(e => e.Grupos)
            .Include(e => e.Acciones)
            .Where(e => e.SnowflakeServidor == id)
            .ToListAsync(ct);

        var reglas = await _contexto.ReglasContenido
            .Where(r => r.SnowflakeServidor == id)
            .ToListAsync(ct);

        var exenciones = await _contexto.Exenciones
            .Where(x => x.SnowflakeServidor == id)
            .ToListAsync(ct);

        _contexto.GruposDeReglas.RemoveRange(grupos);
        _contexto.Eventos.RemoveRange(eventos);
        _contexto.ReglasContenido.RemoveRange(reglas);
        _contexto.Exenciones.RemoveRange(exenciones);
        _contexto.Servidores.Remove(servidor);

        // Una sola SaveChanges: la baja del servidor y de su configuración es atómica.
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    private static ServidorRegistradoEntidad AMappear(ServidorRegistrado s) => new()
    {
        SnowflakeServidor = s.SnowflakeServidor.Valor,
        TokenCifrado = s.TokenCifrado,
        EstadoConexion = s.EstadoConexion.ToString().ToLowerInvariant(),
        EstadoActivacion = s.EstadoActivacion.ToString().ToLowerInvariant(),
        NombreDescriptivo = s.NombreDescriptivo,
        CreadoEn = s.CreadoEn,
        SnowflakeCanalSalida = s.CanalDeSalida?.SnowflakeCanal.Valor,
        PropositoCanalSalida = s.CanalDeSalida?.PropositoLogico,
    };

    private static ServidorRegistrado ADominio(ServidorRegistradoEntidad e) => new(
        new Snowflake(e.SnowflakeServidor),
        e.TokenCifrado,
        Enum.Parse<EstadoConexion>(e.EstadoConexion, ignoreCase: true),
        Enum.Parse<EstadoActivacion>(e.EstadoActivacion, ignoreCase: true),
        e.NombreDescriptivo,
        e.CreadoEn,
        ACanalDeSalida(e));

    private static CanalDeSalida? ACanalDeSalida(ServidorRegistradoEntidad e) =>
        string.IsNullOrWhiteSpace(e.SnowflakeCanalSalida)
            ? null
            : new CanalDeSalida(
                new Snowflake(e.SnowflakeCanalSalida),
                e.PropositoCanalSalida ?? CanalDeSalida.PropositoReporteIncidentes);
}
