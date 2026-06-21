namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de hashing de la contraseña del administrador (ADR-03, RN-13). Produce un
/// resguardo en formato PHC (auto-descriptivo: algoritmo, costo, salt y hash en una sola
/// cadena) y lo verifica sin comparar la contraseña en claro (CU-09 paso 4). La contraseña
/// solo se maneja para hashear o verificar; nunca se persiste ni se loguea (RN-13). La
/// implementación vive en Infraestructura para que el dominio no dependa de las primitivas
/// criptográficas del framework.
/// </summary>
public interface IServicioHashContrasena
{
    /// <summary>
    /// Deriva el resguardo PHC de una contraseña con salt aleatorio. Dos resguardos de la
    /// misma contraseña difieren por el salt (ADR-03).
    /// </summary>
    string Hashear(string contrasena);

    /// <summary>
    /// Verifica una contraseña candidata contra un resguardo PHC, sin compararla en claro
    /// (RN-13). Devuelve <c>true</c> solo si coinciden.
    /// </summary>
    bool Verificar(string contrasena, string resguardoPhc);
}
