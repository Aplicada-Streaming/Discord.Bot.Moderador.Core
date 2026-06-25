using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio.Administracion;
using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Repositorio EF Core del administrador único (RC-06, RN-13, ADR-02). Persiste el resguardo
/// PHC de la contraseña (nunca en claro) y resguarda la unicidad de la cuenta: el alta solo
/// procede si no existe ya un administrador (a lo sumo una fila, RC-06).
/// </summary>
public sealed class RepositorioAdministrador : IRepositorioAdministrador
{
    private readonly ContextoPersistencia _contexto;

    public RepositorioAdministrador(ContextoPersistencia contexto) => _contexto = contexto;

    public Task<bool> ExisteAsync(CancellationToken ct = default) =>
        _contexto.Administradores.AnyAsync(ct);

    public async Task<Administrador?> ObtenerAsync(CancellationToken ct = default)
    {
        var entidad = await _contexto.Administradores
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .FirstOrDefaultAsync(ct);

        return entidad is null ? null : ADominio(entidad);
    }

    public async Task<Administrador> AgregarAsync(
        Administrador administrador, CancellationToken ct = default)
    {
        // Unicidad de la cuenta (RC-06, RN-13): no se crea un segundo administrador.
        if (await _contexto.Administradores.AnyAsync(ct))
        {
            throw new InvalidOperationException(
                "Ya existe una cuenta de administrador; el sistema admite a lo sumo una (RC-06).");
        }

        var entidad = new AdministradorEntidad
        {
            IdentificadorCuenta = administrador.IdentificadorCuenta,
            ResguardoPassword = administrador.ResguardoPassword,
            CreadoEn = administrador.CreadoEn,
        };

        _contexto.Administradores.Add(entidad);
        await _contexto.SaveChangesAsync(ct);

        return ADominio(entidad);
    }

    public async Task ActualizarAsync(Administrador administrador, CancellationToken ct = default)
    {
        var entidad = await _contexto.Administradores
            .FirstOrDefaultAsync(a => a.Id == administrador.Id, ct);

        if (entidad is null)
        {
            throw new InvalidOperationException(
                "No existe la cuenta de administrador a actualizar (RC-06).");
        }

        // Solo cambia el resguardo de contraseña (RN-13); el identificador y la fecha de alta se
        // conservan. Nunca se guarda la contraseña en claro.
        entidad.ResguardoPassword = administrador.ResguardoPassword;
        await _contexto.SaveChangesAsync(ct);
    }

    private static Administrador ADominio(AdministradorEntidad e) =>
        new(e.IdentificadorCuenta, e.ResguardoPassword, e.CreadoEn, e.Id);
}
