using System.Collections.Concurrent;

namespace DiscordModeradorBot.Servicio.Dominio.Administracion;

/// <summary>
/// Control de intentos fallidos de autenticación en memoria (CU-09 AUTH_DEMASIADOS_INTENTOS,
/// RN-12). Tras <see cref="MaximoIntentos"/> fallos dentro de la <see cref="Ventana"/> se
/// bloquea temporalmente el ingreso por el período de <see cref="Enfriamiento"/>: durante el
/// bloqueo, aun con credenciales correctas, el intento se rechaza (CU-09 §6). Un login exitoso
/// resetea el contador de la clave (CU-09 CA-01). Pasado el enfriamiento, el contador se reinicia
/// y se vuelve a permitir el ingreso.
///
/// El estado es efímero y vive solo en memoria, coherente con ADR-09 (estado de alta frecuencia y
/// vida corta que no se persiste): se pierde ante un reinicio, lo que es aceptable porque el
/// bloqueo es una contención temporal. La clave de seguimiento es opaca para este control (un
/// identificador de cuenta o una IP de origen); el llamador decide cómo componerla sin filtrar si
/// el usuario existe (CU-09 §6, sin enumeración de cuentas). El reloj se inyecta para que los tests
/// sean deterministas (estrategia-testing §7, ADR-09).
/// </summary>
public sealed class ControlIntentosAutenticacion
{
    /// <summary>
    /// Cantidad de intentos fallidos dentro de la ventana que disparan el bloqueo temporal
    /// (CU-09 AUTH_DEMASIADOS_INTENTOS). Valor por defecto documentado: 5.
    /// </summary>
    public const int MaximoIntentosPorDefecto = 5;

    /// <summary>
    /// Ventana, en minutos, dentro de la cual se cuentan los intentos fallidos consecutivos.
    /// Valor por defecto documentado: 15 min.
    /// </summary>
    public const double VentanaMinutosPorDefecto = 15.0;

    /// <summary>
    /// Período de enfriamiento, en minutos, durante el cual el ingreso queda bloqueado tras
    /// alcanzar el máximo de intentos. Valor por defecto documentado: 15 min.
    /// </summary>
    public const double EnfriamientoMinutosPorDefecto = 15.0;

    private readonly ConcurrentDictionary<string, RegistroIntentos> _intentosPorClave = new();
    private readonly object _candado = new();

    public ControlIntentosAutenticacion(
        int maximoIntentos = MaximoIntentosPorDefecto,
        TimeSpan? ventana = null,
        TimeSpan? enfriamiento = null)
    {
        if (maximoIntentos < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximoIntentos), "El máximo de intentos debe ser al menos 1.");
        }

        var ventanaEfectiva = ventana ?? TimeSpan.FromMinutes(VentanaMinutosPorDefecto);
        var enfriamientoEfectivo = enfriamiento ?? TimeSpan.FromMinutes(EnfriamientoMinutosPorDefecto);

        if (ventanaEfectiva <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ventana), "La ventana debe ser positiva.");
        }

        if (enfriamientoEfectivo <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(enfriamiento), "El enfriamiento debe ser positivo.");
        }

        MaximoIntentos = maximoIntentos;
        Ventana = ventanaEfectiva;
        Enfriamiento = enfriamientoEfectivo;
    }

    /// <summary>Cantidad de fallos en la ventana que disparan el bloqueo.</summary>
    public int MaximoIntentos { get; }

    /// <summary>Ventana dentro de la cual se cuentan los fallos.</summary>
    public TimeSpan Ventana { get; }

    /// <summary>Duración del bloqueo temporal una vez alcanzado el máximo.</summary>
    public TimeSpan Enfriamiento { get; }

    /// <summary>
    /// Indica si la clave está bloqueada en el instante dado (CU-09 AUTH_DEMASIADOS_INTENTOS).
    /// Está bloqueada cuando ya se alcanzó el máximo de fallos y el enfriamiento sigue vigente.
    /// Consulta sin efectos secundarios; pasado el enfriamiento devuelve false (se permite de
    /// nuevo). Una clave sin registro nunca está bloqueada.
    /// </summary>
    public bool EstaBloqueado(string clave, DateTimeOffset ahora)
    {
        if (string.IsNullOrEmpty(clave))
        {
            return false;
        }

        lock (_candado)
        {
            if (!_intentosPorClave.TryGetValue(clave, out var registro))
            {
                return false;
            }

            return EstaBloqueadoInterno(registro, ahora);
        }
    }

    /// <summary>
    /// Registra un intento fallido para la clave en el instante dado (CU-09 CA-02). Si el último
    /// fallo cae fuera de la ventana, el contador se reinicia a 1; dentro de la ventana, se
    /// incrementa. Devuelve true si, tras este fallo, la clave queda bloqueada (alcanzó el
    /// máximo), para que el llamador pueda registrar la situación sin filtrar datos sensibles.
    /// </summary>
    public bool RegistrarFallo(string clave, DateTimeOffset ahora)
    {
        if (string.IsNullOrEmpty(clave))
        {
            return false;
        }

        lock (_candado)
        {
            var registro = _intentosPorClave.TryGetValue(clave, out var existente)
                ? existente
                : new RegistroIntentos(0, ahora);

            // Si el bloqueo previo ya expiró o el último fallo quedó fuera de la ventana, se
            // arranca un nuevo conteo desde este fallo (CU-09: el enfriamiento reinicia el conteo).
            var reiniciar = EstaExpiradoElConteo(registro, ahora);
            var conteo = reiniciar ? 1 : registro.Conteo + 1;

            var actualizado = new RegistroIntentos(conteo, ahora);
            _intentosPorClave[clave] = actualizado;

            return EstaBloqueadoInterno(actualizado, ahora);
        }
    }

    /// <summary>
    /// Resetea el contador de la clave tras un ingreso exitoso (CU-09 CA-01): el historial de
    /// fallos previos no penaliza un login válido.
    /// </summary>
    public void RegistrarExito(string clave)
    {
        if (string.IsNullOrEmpty(clave))
        {
            return;
        }

        lock (_candado)
        {
            _intentosPorClave.TryRemove(clave, out _);
        }
    }

    /// <summary>
    /// Está bloqueada cuando alcanzó el máximo de fallos y todavía no transcurrió el enfriamiento
    /// desde el último fallo. El enfriamiento se cuenta desde el fallo que cerró el límite.
    /// </summary>
    private bool EstaBloqueadoInterno(RegistroIntentos registro, DateTimeOffset ahora)
        => registro.Conteo >= MaximoIntentos && ahora - registro.UltimoFallo < Enfriamiento;

    /// <summary>
    /// El conteo está expirado (se reinicia con el próximo fallo) cuando, alcanzado el máximo, ya
    /// pasó el enfriamiento; o cuando, sin alcanzarlo, el último fallo quedó fuera de la ventana.
    /// </summary>
    private bool EstaExpiradoElConteo(RegistroIntentos registro, DateTimeOffset ahora)
    {
        if (registro.Conteo >= MaximoIntentos)
        {
            return ahora - registro.UltimoFallo >= Enfriamiento;
        }

        return ahora - registro.UltimoFallo > Ventana;
    }

    private readonly record struct RegistroIntentos(int Conteo, DateTimeOffset UltimoFallo);
}
