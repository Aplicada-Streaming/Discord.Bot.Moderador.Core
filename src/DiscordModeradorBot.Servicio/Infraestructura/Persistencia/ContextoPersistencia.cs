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

    public DbSet<AdministradorEntidad> Administradores => Set<AdministradorEntidad>();

    public DbSet<ExencionEntidad> Exenciones => Set<ExencionEntidad>();

    // Modelo de configuración normalizado de R7 (CU-11): grupos de reglas, relación grupo-regla,
    // eventos/políticas, relación evento-grupo y acciones (modelo-datos-logico §2.7-§2.10).
    public DbSet<GrupoDeReglasEntidad> GruposDeReglas => Set<GrupoDeReglasEntidad>();

    public DbSet<GrupoReglaEntidad> GruposRegla => Set<GrupoReglaEntidad>();

    public DbSet<EventoEntidad> Eventos => Set<EventoEntidad>();

    public DbSet<EventoGrupoEntidad> EventosGrupo => Set<EventoGrupoEntidad>();

    public DbSet<AccionEntidad> Acciones => Set<AccionEntidad>();

    // Parámetros de moderación por servidor (CU-11, RN-10): umbral/ventana de ráfaga y antirrebote.
    public DbSet<ParametrosServidorEntidad> ParametrosServidor => Set<ParametrosServidorEntidad>();

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
            // Estado de reversión del baneo (CU-07, modelo-datos-logico §2.11): quién y cuándo.
            // Nulables: solo se completan si el incidente se revirtió.
            e.Property(x => x.ReversionAutorId);
            e.Property(x => x.ReversionFecha)
                .HasConversion(new ValueConverter<DateTimeOffset?, long?>(
                    v => v.HasValue ? v.Value.UtcTicks : null,
                    v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null));
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

        modelBuilder.Entity<AdministradorEntidad>(e =>
        {
            e.ToTable("Administrador");
            e.HasKey(x => x.Id);
            e.Property(x => x.IdentificadorCuenta).IsRequired();
            // Resguardo PHC de la contraseña; nunca en claro (RN-13, ADR-03).
            e.Property(x => x.ResguardoPassword).IsRequired();
            e.Property(x => x.CreadoEn).HasConversion(conversorMarcaTiempo);
            // Unicidad del identificador de cuenta (RC-06, índice ux_administrador_cuenta).
            e.HasIndex(x => x.IdentificadorCuenta).IsUnique();
        });

        modelBuilder.Entity<ExencionEntidad>(e =>
        {
            e.ToTable("Exencion");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeServidor).IsRequired();
            e.Property(x => x.TipoSujeto).IsRequired();
            e.Property(x => x.SnowflakeSujeto).IsRequired();
            // Descarte de exentos por servidor (RN-07, índice ix_exencion_servidor).
            e.HasIndex(x => x.SnowflakeServidor);
            // Evitar exenciones duplicadas por sujeto (CU-15, índice ux_exencion_sujeto).
            e.HasIndex(x => new { x.SnowflakeServidor, x.TipoSujeto, x.SnowflakeSujeto }).IsUnique();
        });

        // ---- Modelo de configuración normalizado de R7 (CU-11, modelo-datos-logico §2.7-§2.10) ----

        modelBuilder.Entity<GrupoDeReglasEntidad>(e =>
        {
            e.ToTable("GrupoDeReglas");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeServidor).IsRequired();
            e.Property(x => x.Nombre).IsRequired();
            e.Property(x => x.ModoCoincidencia).IsRequired();
            e.Property(x => x.MinimoCoincidencias);
            // Recuperar los grupos de un servidor (CU-11, índice ix_grupo_servidor).
            e.HasIndex(x => x.SnowflakeServidor);
            // Reglas del grupo (relación grupo-regla, RC-03), borrado en cascada con el grupo.
            e.HasMany(x => x.Reglas)
                .WithOne()
                .HasForeignKey(r => r.GrupoDeReglasId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GrupoReglaEntidad>(e =>
        {
            e.ToTable("GrupoRegla");
            e.HasKey(x => x.Id);
            e.Property(x => x.ClaseRegla).IsRequired();
            e.Property(x => x.ReglaContenidoId);
            e.Property(x => x.ClaveReglaConducta);
            // Recuperar las reglas de un grupo (RC-03, índice ix_gruporegla_grupo).
            e.HasIndex(x => x.GrupoDeReglasId);
        });

        modelBuilder.Entity<EventoEntidad>(e =>
        {
            e.ToTable("Evento");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeServidor).IsRequired();
            e.Property(x => x.Nombre).IsRequired();
            e.Property(x => x.Prioridad).IsRequired();
            e.Property(x => x.Continuar).IsRequired();
            e.Property(x => x.Modo).IsRequired();
            e.Property(x => x.ModoCombinacionGrupos).IsRequired();
            // Evaluación ordenada por prioridad (RN-04, índice ix_evento_servidor_prioridad).
            e.HasIndex(x => new { x.SnowflakeServidor, x.Prioridad });
            // Grupos del evento (relación evento-grupo, RC-03), borrado en cascada.
            e.HasMany(x => x.Grupos)
                .WithOne()
                .HasForeignKey(g => g.EventoId)
                .OnDelete(DeleteBehavior.Cascade);
            // Acciones del evento (RN-05), borrado en cascada con el evento.
            e.HasMany(x => x.Acciones)
                .WithOne()
                .HasForeignKey(a => a.EventoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventoGrupoEntidad>(e =>
        {
            e.ToTable("EventoGrupo");
            e.HasKey(x => x.Id);
            // Recuperar los grupos de un evento (RC-03, índice ix_eventogrupo_evento).
            e.HasIndex(x => x.EventoId);
        });

        modelBuilder.Entity<AccionEntidad>(e =>
        {
            e.ToTable("Accion");
            e.HasKey(x => x.Id);
            e.Property(x => x.Tipo).IsRequired();
            e.Property(x => x.OrdenEjecucion).IsRequired();
            e.Property(x => x.VentanaBorradoDias);
            e.Property(x => x.DuracionTimeoutMinutos);
            e.Property(x => x.RolObjetivo);
            // Ejecución ordenada por orden (RN-05, índice ix_accion_evento_orden).
            e.HasIndex(x => new { x.EventoId, x.OrdenEjecucion });
        });

        modelBuilder.Entity<ParametrosServidorEntidad>(e =>
        {
            e.ToTable("ParametrosServidor");
            e.HasKey(x => x.Id);
            e.Property(x => x.SnowflakeServidor).IsRequired();
            e.Property(x => x.UmbralCanalesDistintos);
            e.Property(x => x.VentanaDeteccionSegundos);
            e.Property(x => x.VentanaAntirreboteSegundos);
            // Una fila por servidor (CU-11, índice ux_parametros_servidor).
            e.HasIndex(x => x.SnowflakeServidor).IsUnique();
        });
    }
}
