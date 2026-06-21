namespace DiscordModeradorBot.Servicio.Dominio.Exenciones;

/// <summary>
/// Tipo de sujeto exento de la moderación (modelo-conceptual §1.4, modelo-datos-logico §2.4).
/// Conjunto cerrado coherente con la restricción ck_exencion_tipo (rol, usuario, canal).
/// </summary>
public enum TipoSujetoExento
{
    /// <summary>Exención por rol: cubre dinámicamente a todo usuario que porte el rol (CU-15 §5.C).</summary>
    Rol = 0,

    /// <summary>Exención por usuario emisor de confianza (CU-15 CA-01/CA-02).</summary>
    Usuario = 1,

    /// <summary>Exención por canal de confianza: excluye su actividad de la evaluación (CU-15 §5.B).</summary>
    Canal = 2,
}

/// <summary>
/// Sujeto de confianza (rol, usuario o canal) excluido de la moderación de un servidor
/// (CU-15, RN-07, modelo-datos-logico §2.4). El identificador del sujeto es un snowflake
/// tratado como texto (RN-08, RC-02). Inmutable: una exención es un par (tipo, snowflake)
/// declarado por el administrador y descartado ANTES de evaluar cualquier regla (RN-07).
/// </summary>
public sealed record Exencion(TipoSujetoExento Tipo, Snowflake Sujeto)
{
    /// <summary>Crea una exención por rol que cubre a quienes porten ese rol (CU-15 §5.C).</summary>
    public static Exencion PorRol(Snowflake rol) => new(TipoSujetoExento.Rol, rol);

    /// <summary>Crea una exención por usuario emisor de confianza (CU-15 CA-01).</summary>
    public static Exencion PorUsuario(Snowflake usuario) => new(TipoSujetoExento.Usuario, usuario);

    /// <summary>Crea una exención por canal de confianza (CU-15 §5.B).</summary>
    public static Exencion PorCanal(Snowflake canal) => new(TipoSujetoExento.Canal, canal);
}
