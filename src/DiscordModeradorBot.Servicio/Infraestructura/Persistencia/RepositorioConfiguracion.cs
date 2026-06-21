using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Repositorio EF Core del modelo de configuración normalizado de R7 (CU-11,
/// modelo-datos-logico §2.7-§2.10). Persiste grupos de reglas con su relación grupo-regla
/// (RC-03), eventos/políticas con su relación evento-grupo y sus acciones (RN-05). Eliminar un
/// grupo referenciado por un evento se BLOQUEA (RC-03, CU-11 CA-04). Mantiene el dominio
/// independiente del ORM (ADR-04).
/// </summary>
public sealed class RepositorioConfiguracion : IRepositorioConfiguracion
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioConfiguracion(ContextoPersistencia contexto) => _contexto = contexto;

    public async Task<int> AgregarGrupoAsync(
        Snowflake servidorId,
        string nombre,
        string modoCoincidencia,
        int? minimoCoincidencias,
        IReadOnlyList<ReglaDeGrupo> reglas,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reglas);

        var entidad = new GrupoDeReglasEntidad
        {
            SnowflakeServidor = servidorId.Valor,
            Nombre = nombre,
            ModoCoincidencia = modoCoincidencia,
            MinimoCoincidencias = minimoCoincidencias,
            Reglas = reglas.Select(r => new GrupoReglaEntidad
            {
                ClaseRegla = r.ClaseRegla,
                ReglaContenidoId = r.ReglaContenidoId,
                ClaveReglaConducta = r.ClaveReglaConducta,
            }).ToList(),
        };

        _contexto.GruposDeReglas.Add(entidad);
        await _contexto.SaveChangesAsync(ct);
        return entidad.Id;
    }

    public async Task<IReadOnlyList<GrupoPersistido>> ListarGruposAsync(
        Snowflake servidorId, CancellationToken ct = default)
    {
        var entidades = await _contexto.GruposDeReglas
            .AsNoTracking()
            .Include(g => g.Reglas)
            .Where(g => g.SnowflakeServidor == servidorId.Valor)
            .ToListAsync(ct);

        return entidades.Select(AGrupoPersistido).ToList();
    }

    public async Task<bool> EliminarGrupoAsync(int grupoId, CancellationToken ct = default)
    {
        // RC-03 / CU-11 CA-04: un grupo referenciado por algún evento no se puede eliminar.
        var referenciado = await _contexto.EventosGrupo
            .AnyAsync(eg => eg.GrupoDeReglasId == grupoId, ct);
        if (referenciado)
        {
            return false;
        }

        var entidad = await _contexto.GruposDeReglas.FirstOrDefaultAsync(g => g.Id == grupoId, ct);
        if (entidad is null)
        {
            return false;
        }

        _contexto.GruposDeReglas.Remove(entidad);
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> AgregarEventoAsync(
        Snowflake servidorId,
        string nombre,
        int prioridad,
        bool continuar,
        string modo,
        string modoCombinacionGrupos,
        IReadOnlyList<int> gruposIds,
        IReadOnlyList<AccionPersistida> acciones,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(gruposIds);
        ArgumentNullException.ThrowIfNull(acciones);

        var entidad = new EventoEntidad
        {
            SnowflakeServidor = servidorId.Valor,
            Nombre = nombre,
            Prioridad = prioridad,
            Continuar = continuar,
            Modo = modo,
            ModoCombinacionGrupos = modoCombinacionGrupos,
            Grupos = gruposIds
                .Select(id => new EventoGrupoEntidad { GrupoDeReglasId = id })
                .ToList(),
            Acciones = acciones.Select(a => new AccionEntidad
            {
                Tipo = a.Tipo,
                OrdenEjecucion = a.OrdenEjecucion,
                VentanaBorradoDias = a.VentanaBorradoDias,
                DuracionTimeoutMinutos = a.DuracionTimeoutMinutos,
                RolObjetivo = a.RolObjetivo,
            }).ToList(),
        };

        _contexto.Eventos.Add(entidad);
        await _contexto.SaveChangesAsync(ct);
        return entidad.Id;
    }

    public async Task<IReadOnlyList<EventoPersistido>> ListarEventosAsync(
        Snowflake servidorId, CancellationToken ct = default)
    {
        var entidades = await _contexto.Eventos
            .AsNoTracking()
            .Include(e => e.Grupos)
            .Include(e => e.Acciones)
            .Where(e => e.SnowflakeServidor == servidorId.Valor)
            .OrderBy(e => e.Prioridad)
            .ToListAsync(ct);

        return entidades.Select(AEventoPersistido).ToList();
    }

    public async Task<IReadOnlyList<ReglaContenidoResumen>> ListarReglasContenidoAsync(
        Snowflake servidorId, CancellationToken ct = default)
    {
        return await _contexto.ReglasContenido
            .AsNoTracking()
            .Where(r => r.SnowflakeServidor == servidorId.Valor)
            .Select(r => new ReglaContenidoResumen(r.Id, r.Nombre))
            .ToListAsync(ct);
    }

    private static GrupoPersistido AGrupoPersistido(GrupoDeReglasEntidad e) => new(
        e.Id,
        e.Nombre,
        e.ModoCoincidencia,
        e.MinimoCoincidencias,
        e.Reglas
            .Select(r => new ReglaDeGrupo(r.ClaseRegla, r.ReglaContenidoId, r.ClaveReglaConducta))
            .ToList());

    private static EventoPersistido AEventoPersistido(EventoEntidad e) => new(
        e.Id,
        e.Nombre,
        e.Prioridad,
        e.Continuar,
        e.Modo,
        e.ModoCombinacionGrupos,
        e.Grupos.Select(g => g.GrupoDeReglasId).ToList(),
        e.Acciones
            .OrderBy(a => a.OrdenEjecucion)
            .Select(a => new AccionPersistida(
                a.Tipo, a.OrdenEjecucion, a.VentanaBorradoDias, a.DuracionTimeoutMinutos, a.RolObjetivo))
            .ToList());
}
