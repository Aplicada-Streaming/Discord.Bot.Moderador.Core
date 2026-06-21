using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>Códigos de error del registro de servidor (CU-10 §6).</summary>
public enum ErrorRegistroServidor
{
    ServidorYaRegistrado,
    ServidorIdentificadorInvalido,
    ServidorTokenVacio,
    CanalIdentificadorInvalido,
}

/// <summary>Resultado del registro de un servidor (CU-10).</summary>
public sealed record ResultadoRegistroServidor(bool Exito, ErrorRegistroServidor? Error = null)
{
    public static ResultadoRegistroServidor Ok() => new(true);

    public static ResultadoRegistroServidor Falla(ErrorRegistroServidor error) => new(false, error);
}

/// <summary>
/// Servicio de registro de servidores (CU-10). Registra un servidor con su token; el
/// token se cifra vía <see cref="IServicioCifrado"/> antes de persistir (ADR-07, RN-14)
/// y nunca se conserva en claro. Valida el formato del snowflake (RN-08) y la unicidad.
/// </summary>
public sealed class ServicioRegistroServidor
{
    private readonly IRepositorioServidores _repositorio;
    private readonly IServicioCifrado _cifrado;
    private readonly IReloj _reloj;

    public ServicioRegistroServidor(
        IRepositorioServidores repositorio, IServicioCifrado cifrado, IReloj reloj)
    {
        _repositorio = repositorio;
        _cifrado = cifrado;
        _reloj = reloj;
    }

    public async Task<ResultadoRegistroServidor> RegistrarAsync(
        string identificadorServidor,
        string token,
        string? nombreDescriptivo = null,
        CancellationToken ct = default,
        string? snowflakeCanalSalida = null)
    {
        if (!Snowflake.TryParse(identificadorServidor, out var snowflake))
        {
            return ResultadoRegistroServidor.Falla(ErrorRegistroServidor.ServidorIdentificadorInvalido);
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return ResultadoRegistroServidor.Falla(ErrorRegistroServidor.ServidorTokenVacio);
        }

        // El canal de salida es opcional; cuando se indica, su identificador se valida como
        // snowflake ACÁ (RN-08) y se rechaza con un error de dominio si no compila. Antes la UI
        // construía el Snowflake del canal y un valor no numérico tiraba una excepción que caía
        // el circuito; ahora la validación vive en el servicio y nunca rompe la página (CU-10 §6).
        CanalDeSalida? canalDeSalida = null;
        if (!string.IsNullOrWhiteSpace(snowflakeCanalSalida))
        {
            if (!Snowflake.TryParse(snowflakeCanalSalida, out var snowflakeCanal))
            {
                return ResultadoRegistroServidor.Falla(ErrorRegistroServidor.CanalIdentificadorInvalido);
            }

            canalDeSalida = new CanalDeSalida(snowflakeCanal, CanalDeSalida.PropositoReporteIncidentes);
        }

        if (await _repositorio.ExisteAsync(snowflake, ct))
        {
            return ResultadoRegistroServidor.Falla(ErrorRegistroServidor.ServidorYaRegistrado);
        }

        // El token se cifra ANTES de persistir; nunca se guarda en claro (RN-14, ADR-07).
        var tokenCifrado = _cifrado.Cifrar(token);

        var servidor = new ServidorRegistrado(
            snowflake,
            tokenCifrado,
            EstadoConexion.Desconectado,
            EstadoActivacion.Inactivo, // registrado pero inactivo hasta superar la prueba (CU-10 paso 5).
            nombreDescriptivo,
            _reloj.Ahora,
            canalDeSalida); // canal de salida designado para reportes (CU-05, R2).

        await _repositorio.AgregarAsync(servidor, ct);
        return ResultadoRegistroServidor.Ok();
    }

    /// <summary>
    /// Elimina un servidor y su configuración (reglas, exenciones, grupos y eventos), conservando
    /// los incidentes como historial de auditoría (RN-11). Devuelve true si el servidor existía y
    /// se eliminó; false si el identificador no es un snowflake válido o el servidor no existe.
    /// </summary>
    public async Task<bool> EliminarAsync(string identificadorServidor, CancellationToken ct = default)
    {
        if (!Snowflake.TryParse(identificadorServidor, out var snowflake))
        {
            return false;
        }

        return await _repositorio.EliminarAsync(snowflake, ct);
    }
}
