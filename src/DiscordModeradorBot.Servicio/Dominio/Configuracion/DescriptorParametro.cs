namespace DiscordModeradorBot.Servicio.Dominio.Configuracion;

/// <summary>
/// Descriptor único de un parámetro configurable: fuente de verdad de su valor por
/// defecto, sus límites, su leyenda y sus ejemplos (ADR-12, RN-10). La validación, la
/// ayuda contextual y la aplicación de defaults se derivan de aquí; la lógica del motor
/// nunca hardcodea estos valores, los toma del descriptor.
/// </summary>
/// <typeparam name="T">Tipo del valor del parámetro.</typeparam>
public sealed class DescriptorParametro<T>
    where T : struct, IComparable<T>
{
    public DescriptorParametro(
        string clave,
        string etiqueta,
        string leyenda,
        T valorPorDefecto,
        T minimo,
        T maximo,
        IReadOnlyList<T> ejemplos)
    {
        if (string.IsNullOrWhiteSpace(clave))
        {
            throw new ArgumentException("La clave del descriptor es obligatoria.", nameof(clave));
        }

        if (minimo.CompareTo(maximo) > 0)
        {
            throw new ArgumentException("El mínimo no puede ser mayor que el máximo.", nameof(minimo));
        }

        if (!EstaEnLimites(valorPorDefecto, minimo, maximo))
        {
            throw new ArgumentException(
                "El valor por defecto debe estar dentro de los límites.", nameof(valorPorDefecto));
        }

        Clave = clave;
        Etiqueta = etiqueta;
        Leyenda = leyenda;
        ValorPorDefecto = valorPorDefecto;
        Minimo = minimo;
        Maximo = maximo;
        Ejemplos = ejemplos;
    }

    public string Clave { get; }
    public string Etiqueta { get; }
    public string Leyenda { get; }
    public T ValorPorDefecto { get; }
    public T Minimo { get; }
    public T Maximo { get; }
    public IReadOnlyList<T> Ejemplos { get; }

    /// <summary>Indica si un valor cae dentro de los límites declarados (RN-10).</summary>
    public bool EsValido(T valor) => EstaEnLimites(valor, Minimo, Maximo);

    /// <summary>
    /// Devuelve el valor si es válido; si no, aplica el valor por defecto del descriptor,
    /// que es el modo seguro definido por la regla (RN-10, CU-01 excepción
    /// DETECCION_PARAMETRO_INVALIDO).
    /// </summary>
    public T NormalizarOPorDefecto(T? valor)
    {
        if (valor is { } v && EsValido(v))
        {
            return v;
        }

        return ValorPorDefecto;
    }

    private static bool EstaEnLimites(T valor, T minimo, T maximo)
        => valor.CompareTo(minimo) >= 0 && valor.CompareTo(maximo) <= 0;
}
