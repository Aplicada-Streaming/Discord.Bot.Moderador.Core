using System.Security.Cryptography;
using System.Text;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;

namespace DiscordModeradorBot.Servicio.Infraestructura.Seguridad;

/// <summary>
/// Servicio de cifrado AES del token en reposo (ADR-07, RN-14). La clave maestra se lee
/// de la variable de entorno <c>DISCORDMODERADOR_CLAVE_MAESTRA</c> y vive fuera de la
/// base. Si no está definida, se deriva una clave de desarrollo determinista SOLO para
/// entorno local; en producción la clave SIEMPRE debe venir del entorno (ver nota más
/// abajo). Usa AES-GCM (cifrado autenticado): cada texto cifrado lleva su nonce y su tag,
/// por lo que dos cifrados del mismo valor difieren.
/// </summary>
public sealed class ServicioCifradoAes : IServicioCifrado
{
    public const string NombreVariableClaveMaestra = "DISCORDMODERADOR_CLAVE_MAESTRA";

    private const int TamanioNonceBytes = 12; // 96 bits, recomendado para AES-GCM.
    private const int TamanioTagBytes = 16;   // 128 bits.

    private readonly byte[] _clave; // 32 bytes (AES-256).

    public ServicioCifradoAes(string? claveMaestra = null)
    {
        claveMaestra ??= Environment.GetEnvironmentVariable(NombreVariableClaveMaestra);
        _clave = DerivarClave(claveMaestra);
    }

    public string Cifrar(string textoPlano)
    {
        ArgumentNullException.ThrowIfNull(textoPlano);

        var datos = Encoding.UTF8.GetBytes(textoPlano);
        var nonce = RandomNumberGenerator.GetBytes(TamanioNonceBytes);
        var cifrado = new byte[datos.Length];
        var tag = new byte[TamanioTagBytes];

        using var aes = new AesGcm(_clave, TamanioTagBytes);
        aes.Encrypt(nonce, datos, cifrado, tag);

        // Formato en reposo: nonce || tag || cifrado, en Base64.
        var salida = new byte[nonce.Length + tag.Length + cifrado.Length];
        Buffer.BlockCopy(nonce, 0, salida, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, salida, nonce.Length, tag.Length);
        Buffer.BlockCopy(cifrado, 0, salida, nonce.Length + tag.Length, cifrado.Length);

        return Convert.ToBase64String(salida);
    }

    public string Descifrar(string textoCifrado)
    {
        ArgumentNullException.ThrowIfNull(textoCifrado);

        var entrada = Convert.FromBase64String(textoCifrado);
        if (entrada.Length < TamanioNonceBytes + TamanioTagBytes)
        {
            throw new CryptographicException("El texto cifrado tiene un formato inválido.");
        }

        var nonce = new byte[TamanioNonceBytes];
        var tag = new byte[TamanioTagBytes];
        var cifrado = new byte[entrada.Length - TamanioNonceBytes - TamanioTagBytes];

        Buffer.BlockCopy(entrada, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(entrada, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(entrada, nonce.Length + tag.Length, cifrado, 0, cifrado.Length);

        var datos = new byte[cifrado.Length];
        using var aes = new AesGcm(_clave, TamanioTagBytes);
        aes.Decrypt(nonce, cifrado, tag, datos);

        return Encoding.UTF8.GetString(datos);
    }

    /// <summary>
    /// Deriva una clave AES-256 de 32 bytes a partir de la clave maestra. En producción la
    /// clave maestra DEBE provenir de la variable de entorno (ADR-07); la rama de
    /// desarrollo solo evita que el arranque local falle por falta de la variable y NO debe
    /// usarse en producción.
    /// </summary>
    private static byte[] DerivarClave(string? claveMaestra)
    {
        if (string.IsNullOrWhiteSpace(claveMaestra))
        {
            // Clave de desarrollo determinista. PRODUCCIÓN: definir DISCORDMODERADOR_CLAVE_MAESTRA.
            claveMaestra = "clave-de-desarrollo-no-usar-en-produccion";
        }

        return SHA256.HashData(Encoding.UTF8.GetBytes(claveMaestra));
    }
}
