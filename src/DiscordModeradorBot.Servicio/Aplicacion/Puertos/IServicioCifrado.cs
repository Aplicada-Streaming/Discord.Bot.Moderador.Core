namespace DiscordModeradorBot.Servicio.Aplicacion.Puertos;

/// <summary>
/// Puerto de cifrado simétrico de secretos en reposo (ADR-07, RN-14). Cifra el token de
/// bot antes de persistirlo y lo descifra solo en memoria para operar. La clave maestra
/// vive fuera de la base.
/// </summary>
public interface IServicioCifrado
{
    string Cifrar(string textoPlano);

    string Descifrar(string textoCifrado);
}
