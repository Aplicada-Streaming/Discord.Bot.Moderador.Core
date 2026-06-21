using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Exenciones;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Repositorio EF Core de exenciones (CU-15, RN-07, ADR-02, R5). El tipo del sujeto se serializa
/// en minúsculas como texto del conjunto cerrado {rol, usuario, canal} (ck_exencion_tipo),
/// coherente con la persistencia de enums por texto del resto del modelo. La unicidad por
/// (servidor, tipo, snowflake) la garantiza el índice único; aquí se comprueba antes de insertar
/// para devolver el duplicado de forma controlada (CU-15 EXENCION_DUPLICADA).
/// </summary>
public sealed class RepositorioExenciones : IRepositorioExenciones
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioExenciones(ContextoPersistencia contexto) => _contexto = contexto;

    public async Task<bool> AgregarAsync(
        Snowflake servidorId, Exencion exencion, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(exencion);

        var tipo = ATexto(exencion.Tipo);
        var snowflake = exencion.Sujeto.Valor;

        var yaExiste = await _contexto.Exenciones.AnyAsync(
            e => e.SnowflakeServidor == servidorId.Valor
                && e.TipoSujeto == tipo
                && e.SnowflakeSujeto == snowflake,
            ct);

        if (yaExiste)
        {
            // CU-15 EXENCION_DUPLICADA: no se crea un duplicado.
            return false;
        }

        _contexto.Exenciones.Add(new ExencionEntidad
        {
            SnowflakeServidor = servidorId.Valor,
            TipoSujeto = tipo,
            SnowflakeSujeto = snowflake,
        });
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<Exencion>> ListarPorServidorAsync(
        Snowflake servidorId, CancellationToken ct = default)
    {
        var entidades = await _contexto.Exenciones
            .AsNoTracking()
            .Where(e => e.SnowflakeServidor == servidorId.Valor)
            .ToListAsync(ct);

        return entidades.Select(ADominio).ToList();
    }

    public async Task<bool> QuitarAsync(
        Snowflake servidorId, Exencion exencion, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(exencion);

        var tipo = ATexto(exencion.Tipo);
        var snowflake = exencion.Sujeto.Valor;

        var entidad = await _contexto.Exenciones.FirstOrDefaultAsync(
            e => e.SnowflakeServidor == servidorId.Valor
                && e.TipoSujeto == tipo
                && e.SnowflakeSujeto == snowflake,
            ct);

        if (entidad is null)
        {
            return false;
        }

        _contexto.Exenciones.Remove(entidad);
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    private static string ATexto(TipoSujetoExento tipo) => tipo.ToString().ToLowerInvariant();

    private static Exencion ADominio(ExencionEntidad e) => new(
        Enum.Parse<TipoSujetoExento>(e.TipoSujeto, ignoreCase: true),
        new Snowflake(e.SnowflakeSujeto));
}
