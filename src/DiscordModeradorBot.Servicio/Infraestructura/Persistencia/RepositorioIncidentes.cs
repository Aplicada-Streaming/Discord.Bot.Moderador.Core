using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Repositorio EF Core de incidentes (RN-11, ADR-02). En R2 la copia de mensajes y los
/// canales afectados se persisten en las tablas hijas MensajeAccionado y CanalAfectado, en
/// la misma unidad confirmada que el incidente (RN-11).
/// </summary>
public sealed class RepositorioIncidentes : IRepositorioIncidentes
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioIncidentes(ContextoPersistencia contexto) => _contexto = contexto;

    public async Task AgregarAsync(Incidente incidente, CancellationToken ct = default)
    {
        var entidad = new IncidenteEntidad
        {
            ServidorId = incidente.ServidorId.Valor,
            UsuarioId = incidente.UsuarioId.Valor,
            NombrePolitica = incidente.NombrePolitica,
            Modo = incidente.Modo.ToString(),
            Accion = incidente.Accion.ToString(),
            Resultado = incidente.Resultado.ToString(),
            Instante = incidente.Instante,
            MensajesAccionados = incidente.MensajesAccionados
                .Select(m => new MensajeAccionadoEntidad
                {
                    SnowflakeMensaje = m.MensajeId.Valor,
                    SnowflakeCanal = m.CanalId.Valor,
                    ContenidoCopiado = m.ContenidoCopiado,
                })
                .ToList(),
            CanalesAfectados = incidente.CanalesAfectados
                .Select(c => new CanalAfectadoEntidad { SnowflakeCanal = c.Valor })
                .ToList(),
        };

        _contexto.Incidentes.Add(entidad);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Incidente>> ListarAsync(CancellationToken ct = default)
    {
        var entidades = await _contexto.Incidentes
            .AsNoTracking()
            .Include(i => i.MensajesAccionados)
            .Include(i => i.CanalesAfectados)
            .OrderByDescending(i => i.Instante)
            .ToListAsync(ct);

        return entidades.Select(ADominio).ToList();
    }

    public async Task<PaginaIncidentes> BuscarAsync(FiltroIncidentes filtro, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        // Predicados aplicados EN LA CONSULTA (no en memoria, CU-06): cada filtro opcional acota la
        // consulta solo si está presente. Modo y Resultado se comparan por su nombre, que es como se
        // persisten; el rango de fechas usa el conversor de Instante (ticks) de forma transparente.
        var consulta = _contexto.Incidentes.AsNoTracking().AsQueryable();

        if (filtro.Servidor is { } servidor)
        {
            consulta = consulta.Where(i => i.ServidorId == servidor.Valor);
        }

        if (filtro.Modo is { } modo)
        {
            var nombreModo = modo.ToString();
            consulta = consulta.Where(i => i.Modo == nombreModo);
        }

        if (filtro.Resultado is { } resultado)
        {
            var nombreResultado = resultado.ToString();
            consulta = consulta.Where(i => i.Resultado == nombreResultado);
        }

        if (!string.IsNullOrWhiteSpace(filtro.UsuarioTexto))
        {
            var texto = filtro.UsuarioTexto.Trim();
            consulta = consulta.Where(i => i.UsuarioId.Contains(texto));
        }

        if (filtro.Desde is { } desde)
        {
            consulta = consulta.Where(i => i.Instante >= desde);
        }

        if (filtro.Hasta is { } hasta)
        {
            consulta = consulta.Where(i => i.Instante <= hasta);
        }

        // Total que satisface el filtro, calculado en la base sobre el mismo predicado (antes de paginar).
        var total = await consulta.CountAsync(ct);

        // Paginación EN LA CONSULTA: solo se materializa la página pedida. Página y tamaño se acotan a
        // un mínimo de 1 para tolerar valores fuera de rango del panel sin romper la consulta.
        var pagina = filtro.Pagina < 1 ? 1 : filtro.Pagina;
        var tamano = filtro.TamanoPagina < 1 ? 1 : filtro.TamanoPagina;

        var entidades = await consulta
            .Include(i => i.MensajesAccionados)
            .Include(i => i.CanalesAfectados)
            .OrderByDescending(i => i.Instante)
            .ThenByDescending(i => i.Id)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(ct);

        return new PaginaIncidentes(entidades.Select(ADominio).ToList(), total);
    }

    public async Task<Incidente?> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var entidad = await _contexto.Incidentes
            .AsNoTracking()
            .Include(i => i.MensajesAccionados)
            .Include(i => i.CanalesAfectados)
            .SingleOrDefaultAsync(i => i.Id == id, ct);

        return entidad is null ? null : ADominio(entidad);
    }

    public async Task<bool> MarcarRevertidoAsync(
        int incidenteId, int administradorId, DateTimeOffset fecha, CancellationToken ct = default)
    {
        var entidad = await _contexto.Incidentes.SingleOrDefaultAsync(i => i.Id == incidenteId, ct);
        if (entidad is null)
        {
            return false;
        }

        // Registra quién revirtió y cuándo (CU-07). No toca la evidencia: el desbaneo NO
        // restaura los mensajes borrados (RN-11).
        entidad.ReversionAutorId = administradorId;
        entidad.ReversionFecha = fecha;
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    private static Incidente ADominio(IncidenteEntidad e)
    {
        var mensajes = e.MensajesAccionados
            .Select(m => new MensajeAccionado(
                new Snowflake(m.SnowflakeMensaje), new Snowflake(m.SnowflakeCanal), m.ContenidoCopiado))
            .ToList();

        var canales = e.CanalesAfectados
            .Select(c => new Snowflake(c.SnowflakeCanal))
            .ToList();

        return new Incidente(
            new Snowflake(e.ServidorId),
            new Snowflake(e.UsuarioId),
            e.NombrePolitica,
            Enum.Parse<Modo>(e.Modo),
            Enum.Parse<TipoAccion>(e.Accion),
            Enum.Parse<ResultadoModeracion>(e.Resultado),
            mensajes,
            canales,
            e.Instante,
            e.Id,
            e.ReversionAutorId,
            e.ReversionFecha);
    }
}
