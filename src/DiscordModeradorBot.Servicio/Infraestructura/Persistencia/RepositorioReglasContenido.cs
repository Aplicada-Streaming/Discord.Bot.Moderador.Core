using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Repositorio EF Core de reglas de contenido (CU-04, ADR-02, R3). Persiste el criterio ya
/// validado (RN-03) y, al recuperar, lo recompila con su tope de tiempo (ADR-08). En R3 solo se
/// materializa el criterio por expresión regular; el de palabras clave queda como extensión.
/// </summary>
public sealed class RepositorioReglasContenido : IRepositorioReglasContenido
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioReglasContenido(ContextoPersistencia contexto) => _contexto = contexto;

    public async Task AgregarAsync(
        Snowflake servidorId, string nombrePolitica, ReglaContenido regla, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(regla);

        var entidad = new ReglaContenidoEntidad
        {
            SnowflakeServidor = servidorId.Valor,
            NombrePolitica = nombrePolitica,
            Nombre = regla.Nombre,
            TipoCriterio = regla.TipoCriterio.ToString(),
            Criterio = regla.Criterio,
            SensibleAMayusculas = regla.SensibleAMayusculas,
        };

        _contexto.ReglasContenido.Add(entidad);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task<bool> ActualizarAsync(int reglaId, ReglaContenido regla, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(regla);

        var entidad = await _contexto.ReglasContenido.FirstOrDefaultAsync(r => r.Id == reglaId, ct);
        if (entidad is null)
        {
            return false;
        }

        entidad.Nombre = regla.Nombre;
        entidad.TipoCriterio = regla.TipoCriterio.ToString();
        entidad.Criterio = regla.Criterio;
        entidad.SensibleAMayusculas = regla.SensibleAMayusculas;
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<ReglaContenidoPersistida>> ListarPorServidorAsync(
        Snowflake servidorId, TimeSpan topeTiempoEvaluacion, CancellationToken ct = default)
    {
        var entidades = await _contexto.ReglasContenido
            .AsNoTracking()
            .Where(r => r.SnowflakeServidor == servidorId.Valor)
            .ToListAsync(ct);

        return entidades.Select(e => ADominio(e, topeTiempoEvaluacion)).ToList();
    }

    private static ReglaContenidoPersistida ADominio(ReglaContenidoEntidad e, TimeSpan topeTiempoEvaluacion)
    {
        // El criterio persistido ya fue validado al configurarse (RN-03); se reconstruye según su
        // clase y se recompila con su matchTimeout para evaluar (ADR-08). Las palabras clave se
        // reconstruyen desde la lista almacenada; cualquier otra clase, como expresión regular.
        var tipo = Enum.TryParse<TipoCriterioContenido>(e.TipoCriterio, out var t)
            ? t
            : TipoCriterioContenido.ExpresionRegular;

        var regla = tipo == TipoCriterioContenido.PalabrasClave
            ? ReglaContenido.PorPalabrasClave(e.Nombre, e.Criterio, topeTiempoEvaluacion, e.SensibleAMayusculas)
            : ReglaContenido.PorExpresionRegular(e.Nombre, e.Criterio, topeTiempoEvaluacion, e.SensibleAMayusculas);

        return new ReglaContenidoPersistida(e.NombrePolitica, regla);
    }
}
