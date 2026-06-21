using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace DiscordModeradorBot.Servicio.E2E.Soporte;

/// <summary>
/// Levanta el HOST REAL del servicio (Kestrel) en un proceso aparte, en un puerto efímero de
/// loopback, para las pruebas e2e (estrategia-testing §1/§7). El host arranca con:
/// <list type="bullet">
///   <item>Base SQLite en un ARCHIVO TEMPORAL por corrida (clave Persistencia:RutaBase): se borra
///   al disponer el host, así cada corrida parte de un estado limpio.</item>
///   <item>Gateway en modo <c>Simulado</c> (sin red ni token): nunca toca la plataforma real.</item>
///   <item>Entorno <c>E2E</c> (no Development): el seed de admin de desarrollo NO corre, y además se
///   apaga explícitamente por <c>Seed:AdminDesarrollo:Habilitado=false</c>, de modo que el panel
///   arranca SIN admin (para poder probar el first-run, CU-08).</item>
///   <item>Cookie de sesión con <c>SecurePolicy=SameAsRequest</c> SOLO en E2E, para que funcione por
///   HTTP de loopback sin certificado de desarrollo (no se relaja fuera de E2E).</item>
/// </list>
/// Espera a que el host responda antes de devolver. Si el SDK/host no arranca, lanza para que el
/// test que lo use quede en error (no se enmascara un host caído).
/// </summary>
public sealed class HostServicioE2E : IAsyncDisposable
{
    private readonly Process _proceso;
    private readonly string _rutaBaseDatos;

    public string UrlBase { get; }

    private HostServicioE2E(Process proceso, string urlBase, string rutaBaseDatos)
    {
        _proceso = proceso;
        UrlBase = urlBase;
        _rutaBaseDatos = rutaBaseDatos;
    }

    /// <summary>
    /// Arranca el host real y espera a que esté listo. <paramref name="sembrarAdmin"/> controla el
    /// seed del administrador de desarrollo: false (por defecto) para los flujos de first-run.
    /// </summary>
    public static async Task<HostServicioE2E> IniciarAsync(CancellationToken ct = default)
    {
        var puerto = ObtenerPuertoLibre();
        var urlBase = $"http://127.0.0.1:{puerto}";

        var rutaBaseDatos = Path.Combine(
            Path.GetTempPath(), $"discordmoderador-e2e-{Guid.NewGuid():N}.db");

        var dllServicio = ResolverDllServicio();

        var inicio = new ProcessStartInfo
        {
            FileName = "dotnet",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetDirectoryName(dllServicio)!,
        };
        inicio.ArgumentList.Add(dllServicio);

        // Configuración por variables de entorno (ASP.NET Core las mapea con doble guion bajo).
        inicio.Environment["ASPNETCORE_ENVIRONMENT"] = "E2E";
        inicio.Environment["ASPNETCORE_URLS"] = urlBase;
        inicio.Environment["Moderacion__Gateway"] = "Simulado";
        inicio.Environment["Persistencia__RutaBase"] = rutaBaseDatos;
        // Apaga explícitamente el seed: el panel arranca sin admin (first-run real, CU-08).
        inicio.Environment["Seed__AdminDesarrollo__Habilitado"] = "false";

        var proceso = Process.Start(inicio)
            ?? throw new InvalidOperationException("No se pudo iniciar el proceso del host e2e.");

        // Drena stdout/stderr para no bloquear el proceso hijo si llena el buffer. Si está definida
        // la variable E2E_HOST_LOG, además tee la salida del host a ese archivo (diagnóstico CI/local).
        var rutaLog = Environment.GetEnvironmentVariable("E2E_HOST_LOG");
        proceso.OutputDataReceived += (_, e) => EscribirLog(rutaLog, e.Data);
        proceso.ErrorDataReceived += (_, e) => EscribirLog(rutaLog, e.Data);
        proceso.BeginOutputReadLine();
        proceso.BeginErrorReadLine();

        var host = new HostServicioE2E(proceso, urlBase, rutaBaseDatos);
        await host.EsperarListoAsync(ct);
        return host;
    }

    /// <summary>Sondea el host hasta que responde (o se agota el tiempo / muere el proceso).</summary>
    private async Task EsperarListoAsync(CancellationToken ct)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var limite = DateTime.UtcNow.AddSeconds(60);

        while (DateTime.UtcNow < limite)
        {
            ct.ThrowIfCancellationRequested();

            if (_proceso.HasExited)
            {
                throw new InvalidOperationException(
                    $"El host e2e terminó inesperadamente con código {_proceso.ExitCode} antes de estar listo.");
            }

            try
            {
                using var resp = await http.GetAsync($"{UrlBase}/ingresar", ct);
                // Cualquier respuesta HTTP (incluida una redirección) indica que el host atiende.
                return;
            }
            catch (HttpRequestException)
            {
                // Aún no levantó; reintentar.
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            {
                // Timeout puntual del sondeo; reintentar.
            }

            await Task.Delay(300, ct);
        }

        throw new TimeoutException("El host e2e no estuvo listo dentro del tiempo esperado (60 s).");
    }

    /// <summary>
    /// Crea (idempotente) un administrador con credenciales conocidas vía el endpoint de sembrado
    /// e2e (solo entorno E2E), para los flujos que requieren una cuenta ya existente (login, panel).
    /// </summary>
    public async Task SembrarAdministradorAsync(string usuario, string contrasena)
    {
        using var http = new HttpClient();
        var contenido = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("usuario", usuario),
            new KeyValuePair<string, string>("contrasena", contrasena),
        });
        var resp = await http.PostAsync($"{UrlBase}/e2e/seed/administrador", contenido);
        resp.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Crea un incidente de baneo EJECUTADO (real) vía el endpoint de sembrado e2e y devuelve su id,
    /// para los flujos de revisión de incidentes y desbaneo (CU-06/CU-07).
    /// </summary>
    public async Task<int> SembrarIncidenteBaneoAsync()
    {
        using var http = new HttpClient();
        var resp = await http.PostAsync($"{UrlBase}/e2e/seed/incidente-baneo", content: null);
        resp.EnsureSuccessStatusCode();
        var cuerpo = await resp.Content.ReadAsStringAsync();
        return int.Parse(cuerpo);
    }

    private static readonly object _candadoLog = new();

    private static void EscribirLog(string? rutaLog, string? linea)
    {
        if (string.IsNullOrEmpty(rutaLog) || linea is null)
        {
            return;
        }

        lock (_candadoLog)
        {
            try
            {
                File.AppendAllText(rutaLog, linea + Environment.NewLine);
            }
            catch
            {
                // Diagnóstico best-effort.
            }
        }
    }

    /// <summary>Reserva un puerto TCP libre de loopback y lo libera para que el host lo tome.</summary>
    private static int ObtenerPuertoLibre()
    {
        var escucha = new TcpListener(IPAddress.Loopback, 0);
        escucha.Start();
        try
        {
            return ((IPEndPoint)escucha.LocalEndpoint).Port;
        }
        finally
        {
            escucha.Stop();
        }
    }

    /// <summary>
    /// Resuelve la ruta del DLL del servicio a partir de la salida de build del e2e. El e2e referencia
    /// al servicio, así que su DLL queda junto a los binarios del test.
    /// </summary>
    private static string ResolverDllServicio()
    {
        var directorioTest = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var dll = Path.Combine(directorioTest, "DiscordModeradorBot.Servicio.dll");
        if (!File.Exists(dll))
        {
            throw new FileNotFoundException(
                "No se encontró DiscordModeradorBot.Servicio.dll junto al test e2e. " +
                "Compilá el proyecto e2e (referencia al servicio) antes de correr.", dll);
        }

        return dll;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (!_proceso.HasExited)
            {
                _proceso.Kill(entireProcessTree: true);
                await _proceso.WaitForExitAsync();
            }
        }
        catch
        {
            // Mejor esfuerzo: el proceso pudo haber terminado solo.
        }
        finally
        {
            _proceso.Dispose();
            BorrarBaseDatos();
        }
    }

    private void BorrarBaseDatos()
    {
        foreach (var sufijo in new[] { string.Empty, "-wal", "-shm" })
        {
            try
            {
                var ruta = _rutaBaseDatos + sufijo;
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }
            }
            catch
            {
                // Mejor esfuerzo: archivo temporal; el SO lo limpia eventualmente.
            }
        }
    }
}
