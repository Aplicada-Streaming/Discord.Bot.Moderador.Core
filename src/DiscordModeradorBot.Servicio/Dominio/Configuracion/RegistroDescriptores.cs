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

    /// <summary>Tope de plataforma de la ventana de borrado retroactivo, en días (RC-11, RN-02).</summary>
    public const int VentanaBorradoMaximaDias = 7;

    /// <summary>
    /// Ventana de borrado retroactivo de mensajes al banear (CU-03, RN-02). Es la cantidad
    /// de días hacia atrás dentro de la cual se purgan los mensajes del emisor al ejecutar
    /// el baneo. Default 1 día; rango de 0 (no remueve nada, CU-03 CA-03) a 7 días, tope que
    /// impone la plataforma. Un valor mayor se acota al tope al ejecutar (RN-02), no se
    /// rechaza. Es DISTINTA de la ventana de detección de la ráfaga (R1).
    /// </summary>
    public static DescriptorParametro<int> VentanaBorradoRetroactivoDias { get; } =
        new(
            clave: "ventana-borrado-retroactivo",
            etiqueta: "Ventana de borrado retroactivo (días)",
            leyenda: "Cantidad de días hacia atrás dentro de los cuales se borran los mensajes " +
                     "del emisor al ejecutar el baneo. La plataforma limita este borrado a 7 días; " +
                     "un valor mayor se acota a ese tope. Con 0 días el baneo no remueve mensajes previos.",
            valorPorDefecto: 1,
            minimo: 0,
            maximo: VentanaBorradoMaximaDias,
            ejemplos: new[] { 0, 1, 7 });

    /// <summary>
    /// Tope de tiempo de evaluación de una regla de contenido, en milisegundos (CU-04, ADR-08, R3).
    /// Es el presupuesto por evaluación de la expresión regular sobre un mensaje: protege de
    /// regex con retroceso catastrófico/ReDoS. Si una evaluación lo excede, se aborta esa regla
    /// sin colgar el pipeline y se trata como no coincidencia (ADR-08). Default 100 ms, holgado
    /// para patrones razonables y suficientemente bajo para no bloquear el flujo de mensajes.
    /// </summary>
    public static DescriptorParametro<int> TopeTiempoEvaluacionContenidoMs { get; } =
        new(
            clave: "tope-tiempo-evaluacion-contenido",
            etiqueta: "Tope de tiempo de evaluación de contenido (ms)",
            leyenda: "Presupuesto máximo, en milisegundos, para evaluar una regla de contenido " +
                     "(expresión regular) sobre un mensaje. Si la evaluación lo excede se aborta esa " +
                     "regla sin interrumpir el procesamiento, como protección ante expresiones " +
                     "regulares costosas o con retroceso catastrófico.",
            valorPorDefecto: 100,
            minimo: 1,
            maximo: 5000,
            ejemplos: new[] { 50, 100, 250 });

    /// <summary>Tope de tiempo de evaluación de contenido por defecto como <see cref="TimeSpan"/>.</summary>
    public static TimeSpan TopeTiempoEvaluacionContenidoPorDefecto =>
        TimeSpan.FromMilliseconds(TopeTiempoEvaluacionContenidoMs.ValorPorDefecto);
}
