using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Factory de diseño para que el tooling de migraciones de EF Core pueda construir el
/// contexto sin arrancar la aplicación (MIG-0001, ADR-02).
/// </summary>
public sealed class ContextoPersistenciaFactory : IDesignTimeDbContextFactory<ContextoPersistencia>
{
    public ContextoPersistencia CreateDbContext(string[] args)
    {
        var opciones = new DbContextOptionsBuilder<ContextoPersistencia>()
            .UseSqlite("Data Source=discordmoderador.db")
            .Options;

        return new ContextoPersistencia(opciones);
    }
}
