namespace DiscordModeradorBot.Servicio.Dominio;

/// <summary>
/// Identificador de 64 bits de la plataforma (servidor, canal, usuario o mensaje).
/// Se trata y persiste como texto para preservar el valor exacto sin desborde del
/// entero con signo (RN-08, RC-02).
/// </summary>
public readonly record struct Snowflake
{
    public string Valor { get; }

    public Snowflake(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("Un snowflake no puede estar vacío.", nameof(valor));
        }

        valor = valor.Trim();

        if (!EsFormatoValido(valor))
        {
            throw new ArgumentException(
                $"'{valor}' no tiene el formato de snowflake esperado (solo dígitos).", nameof(valor));
        }

        Valor = valor;
    }

    /// <summary>
    /// Un snowflake de la plataforma es una secuencia de dígitos decimales.
    /// La validación es de formato; no se interpreta como número (RN-08).
    /// </summary>
    public static bool EsFormatoValido(string? candidato)
    {
        if (string.IsNullOrWhiteSpace(candidato))
        {
            return false;
        }

        foreach (var c in candidato.Trim())
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    public static bool TryParse(string? candidato, out Snowflake snowflake)
    {
        if (EsFormatoValido(candidato))
        {
            snowflake = new Snowflake(candidato!);
            return true;
        }

        snowflake = default;
        return false;
    }

    public override string ToString() => Valor;

    public static implicit operator string(Snowflake snowflake) => snowflake.Valor;
}
