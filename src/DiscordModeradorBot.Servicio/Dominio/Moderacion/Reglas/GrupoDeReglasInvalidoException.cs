namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Excepción de configuración inválida de un grupo de reglas (R7, RN-15, RC-04). Igual que la
/// validación de reglas de contenido (RN-03), la composición se valida AL CONSTRUIR el grupo, no
/// en tiempo de evaluación: un grupo vacío o un N fuera de rango se rechazan en el origen con un
/// código estable del CU (CONFIG_GRUPO_SIN_REGLAS, CONFIG_GRUPO_N_INVALIDO).
/// </summary>
public sealed class GrupoDeReglasInvalidoException : Exception
{
    public GrupoDeReglasInvalidoException(string codigo, string mensaje, Exception? innerException = null)
        : base(mensaje, innerException)
    {
        Codigo = codigo;
    }

    /// <summary>Código estable del CU para mostrar en el panel (CU-11 §6).</summary>
    public string Codigo { get; }
}
