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
            e.Instante);
    }
}
