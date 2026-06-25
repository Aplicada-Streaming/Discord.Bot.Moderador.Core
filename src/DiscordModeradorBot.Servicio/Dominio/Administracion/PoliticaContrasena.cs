namespace DiscordModeradorBot.Servicio.Dominio.Administracion;

/// <summary>
/// Política mínima de robustez de la contraseña del administrador (CU-08 paso 4,
/// SETUP_CONTRASENA_DEBIL, RN-13). El descriptor del requisito vive en el dominio, no
/// hardcodeado en la presentación, para que el medidor de robustez de la UX
/// (wireframes-primer-ingreso) y la validación de alta usen la MISMA fuente de verdad.
/// </summary>
public static class PoliticaContrasena
{
    /// <summary>Longitud mínima exigida a la contraseña del administrador (RN-13).</summary>
    public const int LongitudMinima = 8;

    /// <summary>
    /// Verifica si la contraseña cumple la política mínima de robustez (RN-13). Exige una
    /// longitud mínima (<see cref="LongitudMinima"/>) y que sea alfanumérica: al menos una letra
    /// y un dígito. Una contraseña que no cumple se rechaza en el alta o el cambio con
    /// SETUP_CONTRASENA_DEBIL (CU-08 CA-02).
    /// </summary>
    public static bool EsRobusta(string? contrasena)
    {
        if (string.IsNullOrEmpty(contrasena) || contrasena.Length < LongitudMinima)
        {
            return false;
        }

        var tieneLetra = false;
        var tieneDigito = false;
        foreach (var c in contrasena)
        {
            if (char.IsLetter(c))
            {
                tieneLetra = true;
            }
            else if (char.IsDigit(c))
            {
                tieneDigito = true;
            }
        }

        return tieneLetra && tieneDigito;
    }
}
