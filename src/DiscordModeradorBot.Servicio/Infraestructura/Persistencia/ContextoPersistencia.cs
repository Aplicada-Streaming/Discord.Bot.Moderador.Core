using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Contexto de persistencia EF Core sobre SQLite en modo WAL (ADR-02). En R1 materializa
/// las entidades del walking skeleton: ServidorRegistrado e Incidente. El esquema
/// completo de 13 tablas (modelo-datos-logico) se incorpora en rebanadas posteriores.
/// Los snowflakes se almacenan como TEXTO (RN-08, RC-02).
/// </summary>
public sealed class ContextoPersistencia : DbContext
{
    public ContextoPersistencia(DbContextOptions<ContextoPersistencia> options)
        : base(options)
    {
    }

    public DbSet<ServidorRegistradoEntidad> Servidores => Set<ServidorRegistradoEntidad>();

    public DbSet<IncidenteEntidad> Incidentes => Set<IncidenteEntidad>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SQLite no traduce ORDER BY ni comparaciones sobre DateTimeOffset; se persiste como
        // ticks UTC (long), que ordena correctamente y round-trip sin pérdida (ADR-02).
        var conversorMarcaTiempo = new ValueConverter<DateTimeOffset, long>(
            v => v.UtcTicks,
            v => new DateTimeOffset(v, TimeSpan.Zero));

        modelBuilder.Entity<ServidorRegistradoEntidad>(e =>
        {
            e.ToTable("Servidor");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeServidor).IsRequired();
            e.Property(x => x.TokenCifrado).IsRequired();
            e.Property(x => x.EstadoConexion).IsRequired();
            e.Property(x => x.EstadoActivacion).IsRequired();
            e.Property(x => x.CreadoEn).HasConversion(conversorMarcaTiempo);
            // Unicidad del servidor por snowflake (RN-08, RC-02, índice ux_servidor_snowflake).
            e.HasIndex(x => x.SnowflakeServidor).IsUnique();
        });

        modelBuilder.Entity<IncidenteEntidad>(e =>
        {
            e.ToTable("Incidente");
            e.HasKey(x => x.Id);
            e.Property(x => x.ServidorId).IsRequired();
            e.Property(x => x.UsuarioId).IsRequired();
            e.Property(x => x.NombrePolitica).IsRequired();
            e.Property(x => x.Modo).IsRequired();
            e.Property(x => x.Accion).IsRequired();
            e.Property(x => x.Resultado).IsRequired();
            e.Property(x => x.Instante).HasConversion(conversorMarcaTiempo);
            // Revisión de incidentes por fecha (CU-06, índice ix_incidente_servidor_fecha).
            e.HasIndex(x => new { x.ServidorId, x.Instante });
        });
    }
}
