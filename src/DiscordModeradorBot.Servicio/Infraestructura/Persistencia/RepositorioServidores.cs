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

    private static ServidorRegistradoEntidad AMappear(ServidorRegistrado s) => new()
    {
        SnowflakeServidor = s.SnowflakeServidor.Valor,
        TokenCifrado = s.TokenCifrado,
        EstadoConexion = s.EstadoConexion.ToString().ToLowerInvariant(),
        EstadoActivacion = s.EstadoActivacion.ToString().ToLowerInvariant(),
        NombreDescriptivo = s.NombreDescriptivo,
        CreadoEn = s.CreadoEn,
    };

    private static ServidorRegistrado ADominio(ServidorRegistradoEntidad e) => new(
        new Snowflake(e.SnowflakeServidor),
        e.TokenCifrado,
        Enum.Parse<EstadoConexion>(e.EstadoConexion, ignoreCase: true),
        Enum.Parse<EstadoActivacion>(e.EstadoActivacion, ignoreCase: true),
        e.NombreDescriptivo,
        e.CreadoEn);
}
