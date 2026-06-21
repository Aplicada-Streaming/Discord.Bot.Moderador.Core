namespace DiscordModeradorBot.Servicio.Dominio.Contenido;

/// <summary>
/// Error de validación de una regla de contenido: el criterio no es válido según su clase
/// (RN-03, CU-04 excepción CONTENIDO_PATRON_INVALIDO). Se lanza al CONFIGURAR/registrar la regla,
/// nunca en tiempo de evaluación de un mensaje (ADR-08): un patrón inválido se rechaza en el
/// origen para que nunca llegue a la base ni al pipeline. Lleva el código del CU para que el
/// panel y los logs lo presenten al administrador (CU-04 §6).
/// </summary>
public sealed class ReglaContenidoInvalidaException : Exception
{
    /// <summary>Código de error del CU para una expresión regular que no compila (CU-04 §6).</summary>
    public const string CodigoPatronInvalido = "CONTENIDO_PATRON_INVALIDO";

    public ReglaContenidoInvalidaException(string codigo, string mensaje, Exception? innerException = null)
        : base(mensaje, innerException)
    {
        Codigo = codigo;
    }

    /// <summary>Código estable del error para presentación y trazabilidad (CU-04 §6).</summary>
    public string Codigo { get; }
}
