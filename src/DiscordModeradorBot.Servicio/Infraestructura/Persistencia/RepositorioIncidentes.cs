using System.Text.Json;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>Repositorio EF Core de incidentes (RN-11, ADR-02).</summary>
public sealed class RepositorioIncidentes : IRepositorioIncidentes
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioIncidentes(ContextoPersistencia contexto) => _contexto = contexto;

    private sealed record MensajeAccionadoDto(string MensajeId, string CanalId, string Contenido);

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
            CanalesAfectados = JsonSerializer.Serialize(
                incidente.CanalesAfectados.Select(c => c.Valor).ToList()),
            CopiaMensajes = JsonSerializer.Serialize(
                incidente.MensajesAccionados
                    .Select(m => new MensajeAccionadoDto(m.MensajeId.Valor, m.CanalId.Valor, m.ContenidoCopiado))
                    .ToList()),
            Instante = incidente.Instante,
        };

        _contexto.Incidentes.Add(entidad);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Incidente>> ListarAsync(CancellationToken ct = default)
    {
        var entidades = await _contexto.Incidentes
            .AsNoTracking()
            .OrderByDescending(i => i.Instante)
            .ToListAsync(ct);

        return entidades.Select(ADominio).ToList();
    }

    private static Incidente ADominio(IncidenteEntidad e)
    {
        var canales = (JsonSerializer.Deserialize<List<string>>(e.CanalesAfectados) ?? new List<string>())
            .Select(c => new Snowflake(c))
            .ToList();

        var mensajes = (JsonSerializer.Deserialize<List<MensajeAccionadoDto>>(e.CopiaMensajes)
                ?? new List<MensajeAccionadoDto>())
            .Select(m => new MensajeAccionado(new Snowflake(m.MensajeId), new Snowflake(m.CanalId), m.Contenido))
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
