namespace DiscordModeradorBot.Servicio.Dominio.Configuracion;

/// <summary>
/// Registro de descriptores de parámetro: fuente única de verdad de los parámetros
/// configurables del dominio (ADR-12, RN-10). En R1 cubre los dos parámetros del
/// evaluador de ráfaga distribuida; agregar parámetros es el punto de extensión
/// declarado (extensibilidad). Los defaults y límites no se hardcodean en la lógica;
/// se toman de estos descriptores.
/// </summary>
public static class RegistroDescriptores
{
    /// <summary>
    /// Umbral de canales distintos que dispara la ráfaga distribuida (CU-01, RN-10).
    /// Default 3, mínimo 2. El valor exacto del default queda abierto a calibración
    /// (intake §17 P.11); este es el punto único donde se define.
    /// </summary>
    public static DescriptorParametro<int> UmbralCanalesDistintos { get; } =
        new(
            clave: "umbral-canales-distintos",
            etiqueta: "Umbral de canales distintos",
            leyenda: "Cantidad de canales distintos en los que un mismo usuario debe publicar, " +
                     "dentro de la ventana de detección, para que se marque una ráfaga distribuida. " +
                     "El discriminador es la cantidad de canales distintos, no el volumen de mensajes.",
            valorPorDefecto: 3,
            minimo: 2,
            maximo: 10,
            ejemplos: new[] { 2, 3, 4 });

    /// <summary>
    /// Ventana de detección de la ráfaga distribuida (CU-01, RN-10). Default 2 s.
    /// </summary>
    public static DescriptorParametro<double> VentanaDeteccionSegundos { get; } =
        new(
            clave: "ventana-deteccion",
            etiqueta: "Ventana de detección (segundos)",
            leyenda: "Tiempo, en segundos, dentro del cual se cuentan los canales distintos. " +
                     "Una ventana mayor captura fan-outs más espaciados.",
            valorPorDefecto: 2.0,
            minimo: 0.5,
            maximo: 60.0,
            ejemplos: new[] { 2.0, 4.0, 6.0 });

    /// <summary>Ventana de detección por defecto como <see cref="TimeSpan"/>, derivada del descriptor.</summary>
    public static TimeSpan VentanaDeteccionPorDefecto =>
        TimeSpan.FromSeconds(VentanaDeteccionSegundos.ValorPorDefecto);
}
