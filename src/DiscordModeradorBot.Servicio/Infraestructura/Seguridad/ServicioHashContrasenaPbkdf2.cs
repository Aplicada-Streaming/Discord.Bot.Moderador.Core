using System.Security.Cryptography;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;

namespace DiscordModeradorBot.Servicio.Infraestructura.Seguridad;

/// <summary>
/// Implementación del resguardo de contraseña con PBKDF2-HMAC-SHA256 en formato PHC
/// (ADR-03, RN-13). Usa las primitivas del framework
/// (<see cref="Rfc2898DeriveBytes"/>); NO agrega paquetes externos. El resguardo es
/// auto-descriptivo y permite migrar el costo sin romper verificaciones previas: cada
/// resguardo lleva el algoritmo, las iteraciones, el salt y el hash en una sola cadena.
///
/// Formato PHC: <c>$pbkdf2-sha256$i=ITERACIONES$SALT_B64$HASH_B64</c>, donde SALT y HASH van
/// en Base64. Las iteraciones por defecto se documentan abajo y son configurables por
/// constructor (calibrables al hardware de referencia, ADR-03).
///
/// La contraseña solo se maneja para derivar o verificar y no se loguea (RN-13).
/// </summary>
public sealed class ServicioHashContrasenaPbkdf2 : IServicioHashContrasena
{
    /// <summary>Algoritmo declarado en el prefijo PHC.</summary>
    public const string Algoritmo = "pbkdf2-sha256";

    /// <summary>
    /// Iteraciones por defecto de PBKDF2-HMAC-SHA256 (ADR-03). Valor calibrado para un costo
    /// de verificación aceptable en hardware de referencia con baja frecuencia de login; es
    /// configurable por constructor para recalibrar sin tocar el formato.
    /// </summary>
    public const int IteracionesPorDefecto = 210_000;

    private const int TamanioSaltBytes = 16; // 128 bits.
    private const int TamanioHashBytes = 32; // 256 bits (coincide con el tamaño de SHA-256).

    private static readonly HashAlgorithmName AlgoritmoHmac = HashAlgorithmName.SHA256;

    private readonly int _iteraciones;

    public ServicioHashContrasenaPbkdf2(int iteraciones = IteracionesPorDefecto)
    {
        if (iteraciones < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(iteraciones), "Las iteraciones de PBKDF2 deben ser al menos 1.");
        }

        _iteraciones = iteraciones;
    }

    public string Hashear(string contrasena)
    {
        ArgumentNullException.ThrowIfNull(contrasena);

        var salt = RandomNumberGenerator.GetBytes(TamanioSaltBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            contrasena, salt, _iteraciones, AlgoritmoHmac, TamanioHashBytes);

        // $pbkdf2-sha256$i=<iteraciones>$<salt_b64>$<hash_b64>
        return $"${Algoritmo}$i={_iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string contrasena, string resguardoPhc)
    {
        ArgumentNullException.ThrowIfNull(contrasena);

        if (!TryParse(resguardoPhc, out var iteraciones, out var salt, out var hashEsperado))
        {
            return false;
        }

        var hashCandidato = Rfc2898DeriveBytes.Pbkdf2(
            contrasena, salt, iteraciones, AlgoritmoHmac, hashEsperado.Length);

        // Comparación de tiempo constante para no filtrar información por el tiempo (RN-13).
        return CryptographicOperations.FixedTimeEquals(hashCandidato, hashEsperado);
    }

    /// <summary>
    /// Descompone un resguardo PHC en sus parámetros. Devuelve <c>false</c> ante un formato
    /// desconocido o un algoritmo distinto, en vez de lanzar, para que la verificación falle
    /// de forma segura.
    /// </summary>
    private static bool TryParse(
        string? resguardoPhc, out int iteraciones, out byte[] salt, out byte[] hash)
    {
        iteraciones = 0;
        salt = [];
        hash = [];

        if (string.IsNullOrEmpty(resguardoPhc))
        {
            return false;
        }

        // Partes esperadas: ["", "pbkdf2-sha256", "i=<n>", "<salt_b64>", "<hash_b64>"].
        var partes = resguardoPhc.Split('$');
        if (partes.Length != 5 || partes[0].Length != 0 || partes[1] != Algoritmo)
        {
            return false;
        }

        var parametros = partes[2];
        if (!parametros.StartsWith("i=", StringComparison.Ordinal) ||
            !int.TryParse(parametros.AsSpan(2), out iteraciones) ||
            iteraciones < 1)
        {
            return false;
        }

        try
        {
            salt = Convert.FromBase64String(partes[3]);
            hash = Convert.FromBase64String(partes[4]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length > 0 && hash.Length > 0;
    }
}
