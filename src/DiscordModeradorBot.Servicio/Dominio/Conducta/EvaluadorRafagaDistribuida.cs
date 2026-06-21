using DiscordModeradorBot.Servicio.Dominio.Configuracion;

namespace DiscordModeradorBot.Servicio.Dominio.Conducta;

/// <summary>
/// Resultado de evaluar la regla de ráfaga distribuida para un mensaje.
/// </summary>
/// <param name="Coincide">Si la cantidad de canales distintos alcanzó el umbral.</param>
/// <param name="CanalesDistintos">Cantidad de canales distintos contados en la ventana.</param>
/// <param name="Umbral">Umbral efectivo aplicado (tomado del descriptor).</param>
public readonly record struct ResultadoRafaga(bool Coincide, int CanalesDistintos, int Umbral);

/// <summary>
/// Regla de conducta de ráfaga distribuida (CU-01), módulo crítico de detección.
/// Dado un mensaje y el estado de conducta, determina si la cantidad de canales
/// DISTINTOS en los que el usuario publicó dentro de la ventana alcanza el umbral.
/// El discriminador es la cantidad de canales distintos, NO el volumen de mensajes:
/// muchos mensajes en un solo canal no disparan (CU-01 flujo 5.A, RN para falsos
/// positivos). Umbral y ventana provienen de los descriptores (RN-10), nunca
/// hardcodeados aquí.
/// </summary>
public sealed class EvaluadorRafagaDistribuida
{
    private readonly DescriptorParametro<int> _descriptorUmbral;
    private readonly DescriptorParametro<double> _descriptorVentana;

    public EvaluadorRafagaDistribuida()
        : this(RegistroDescriptores.UmbralCanalesDistintos, RegistroDescriptores.VentanaDeteccionSegundos)
    {
    }

    public EvaluadorRafagaDistribuida(
        DescriptorParametro<int> descriptorUmbral,
        DescriptorParametro<double> descriptorVentana)
    {
        _descriptorUmbral = descriptorUmbral ?? throw new ArgumentNullException(nameof(descriptorUmbral));
        _descriptorVentana = descriptorVentana ?? throw new ArgumentNullException(nameof(descriptorVentana));
    }

    /// <summary>
    /// Evalúa la condición de ráfaga distribuida usando los valores por defecto del
    /// descriptor para umbral y ventana.
    /// </summary>
    public ResultadoRafaga Evaluar(
        MensajeEntrante mensaje, EstadoConductaEnMemoria estado, DateTimeOffset ahora)
        => Evaluar(mensaje, estado, ahora, umbralConfigurado: null, ventanaSegundosConfigurada: null);

    /// <summary>
    /// Evalúa la condición de ráfaga distribuida. El umbral y la ventana configurados
    /// se normalizan contra sus descriptores: un valor fuera de límites se sustituye por
    /// el valor por defecto (RN-10; CU-01 excepción DETECCION_PARAMETRO_INVALIDO).
    /// </summary>
    public ResultadoRafaga Evaluar(
        MensajeEntrante mensaje,
        EstadoConductaEnMemoria estado,
        DateTimeOffset ahora,
        int? umbralConfigurado,
        double? ventanaSegundosConfigurada)
    {
        ArgumentNullException.ThrowIfNull(mensaje);
        ArgumentNullException.ThrowIfNull(estado);

        var umbral = _descriptorUmbral.NormalizarOPorDefecto(umbralConfigurado);
        var ventanaSegundos = _descriptorVentana.NormalizarOPorDefecto(ventanaSegundosConfigurada);
        var ventana = TimeSpan.FromSeconds(ventanaSegundos);

        var canalesDistintos = estado.CanalesDistintosEnVentana(
            mensaje.ServidorId, mensaje.UsuarioId, ahora, ventana);

        var coincide = canalesDistintos >= umbral;
        return new ResultadoRafaga(coincide, canalesDistintos, umbral);
    }
}
