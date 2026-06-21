using DiscordModeradorBot.Servicio.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordModeradorBot.Servicio.Infraestructura.Persistencia;

/// <summary>
/// Contexto de persistencia EF Core sobre SQLite en modo WAL (ADR-02). En R1 materializaba
/// ServidorRegistrado e Incidente con la evidencia en JSON; R2 normaliza la evidencia del
/// incidente a las tablas hijas MensajeAccionado y CanalAfectado (modelo-datos-logico §2.12,
/// §2.13) y agrega el canal de salida al servidor (§2.3). R3 agrega la tabla Regla para las
/// reglas de contenido (modelo-datos-logico §2.5, CU-04). El esquema completo de 13 tablas se
/// incorpora en rebanadas posteriores. Los snowflakes se almacenan como TEXTO (RN-08, RC-02).
/// </summary>
public sealed class ContextoPersistencia : DbContext
{
    public ContextoPersistencia(DbContextOptions<ContextoPersistencia> options)
        : base(options)
    {
    }

    public DbSet<ServidorRegistradoEntidad> Servidores => Set<ServidorRegistradoEntidad>();

    public DbSet<IncidenteEntidad> Incidentes => Set<IncidenteEntidad>();

    public DbSet<MensajeAccionadoEntidad> MensajesAccionados => Set<MensajeAccionadoEntidad>();

    public DbSet<CanalAfectadoEntidad> CanalesAfectados => Set<CanalAfectadoEntidad>();

    public DbSet<ReglaContenidoEntidad> ReglasContenido => Set<ReglaContenidoEntidad>();

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
            // Canal de salida designado para reportes (modelo-datos-logico §2.3, CU-05).
            e.Property(x => x.SnowflakeCanalSalida);
            e.Property(x => x.PropositoCanalSalida);
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

            // Evidencia normalizada a tablas hijas, borrado en cascada con el incidente (RN-11).
            e.HasMany(x => x.MensajesAccionados)
                .WithOne()
                .HasForeignKey(m => m.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.CanalesAfectados)
                .WithOne()
                .HasForeignKey(c => c.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MensajeAccionadoEntidad>(e =>
        {
            e.ToTable("MensajeAccionado");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeMensaje).IsRequired();
            e.Property(x => x.SnowflakeCanal).IsRequired();
            e.Property(x => x.ContenidoCopiado).IsRequired();
            // Recuperar evidencia del incidente (CU-06, índice ix_mensaje_accionado_incidente).
            e.HasIndex(x => x.IncidenteId);
        });

        modelBuilder.Entity<CanalAfectadoEntidad>(e =>
        {
            e.ToTable("CanalAfectado");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeCanal).IsRequired();
            // Listar canales afectados (CU-06, índice ix_canal_afectado_incidente).
            e.HasIndex(x => x.IncidenteId);
        });

        modelBuilder.Entity<ReglaContenidoEntidad>(e =>
        {
            e.ToTable("Regla");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeServidor).IsRequired();
            e.Property(x => x.TipoCriterio).IsRequired();
            e.Property(x => x.Criterio).IsRequired();
            e.Property(x => x.Nombre).IsRequired();
            e.Property(x => x.NombrePolitica).IsRequired();
            e.Property(x => x.SensibleAMayusculas).IsRequired();
            // Recuperar las reglas de contenido de un servidor (CU-04, índice ix_regla_servidor).
            e.HasIndex(x => x.SnowflakeServidor);
        });
    }
}
