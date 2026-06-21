using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio.Administracion;

namespace DiscordModeradorBot.Servicio.Aplicacion;

/// <summary>Códigos de error del alta de credenciales del administrador (CU-08 §6).</summary>
public enum ErrorAltaAdministrador
{
    /// <summary>Ya existe la cuenta; el first-run está completado (CU-08 CA-03, SETUP_YA_COMPLETADO).</summary>
    SetupYaCompletado,

    /// <summary>La contraseña no cumple la política mínima (CU-08 CA-02, SETUP_CONTRASENA_DEBIL).</summary>
    SetupContrasenaDebil,

    /// <summary>El identificador de cuenta es inválido (vacío).</summary>
    SetupIdentificadorInvalido,
}

/// <summary>Resultado del alta del administrador inicial (CU-08).</summary>
public sealed record ResultadoAltaAdministrador(bool Exito, ErrorAltaAdministrador? Error = null)
{
    public static ResultadoAltaAdministrador Ok() => new(true);

    public static ResultadoAltaAdministrador Falla(ErrorAltaAdministrador error) => new(false, error);
}

/// <summary>Resultado de la autenticación del administrador (CU-09 §6).</summary>
public enum ResultadoAutenticacion
{
    /// <summary>Credenciales válidas: se abre la sesión (CU-09 CA-01).</summary>
    Ok,

    /// <summary>El identificador o la contraseña no coinciden (CU-09 CA-02, AUTH_CREDENCIALES_INVALIDAS).</summary>
    CredencialesInvalidas,

    /// <summary>
    /// Se superó el límite de intentos fallidos: el ingreso está bloqueado temporalmente
    /// (CU-09 §6, AUTH_DEMASIADOS_INTENTOS). No distingue si el usuario existe (sin enumeración).
    /// </summary>
    DemasiadosIntentos,
}

/// <summary>
/// Servicio del administrador único (CU-08 alta, CU-09 autenticación; RN-12, RN-13, RC-06).
/// Crea la cuenta única en el primer ingreso, rechaza un segundo alta (unicidad) y verifica
/// credenciales contra el resguardo PHC sin comparar la contraseña en claro. Aplica además el
/// control de intentos fallidos para bloquear temporalmente el ingreso ante fuerza bruta
/// (CU-09 AUTH_DEMASIADOS_INTENTOS). La contraseña solo se maneja para hashear o verificar y
/// nunca se loguea (RN-13).
/// </summary>
public sealed class ServicioAdministrador
{
    private readonly IRepositorioAdministrador _repositorio;
    private readonly IServicioHashContrasena _hash;
    private readonly IReloj _reloj;
    private readonly ControlIntentosAutenticacion _controlIntentos;

    public ServicioAdministrador(
        IRepositorioAdministrador repositorio,
        IServicioHashContrasena hash,
        IReloj reloj,
        ControlIntentosAutenticacion controlIntentos)
    {
        _repositorio = repositorio;
        _hash = hash;
        _reloj = reloj;
        _controlIntentos = controlIntentos;
    }

    /// <summary>Indica si ya hay una cuenta de administrador (decide el first-run, CU-08/CU-09).</summary>
    public Task<bool> ExisteAdministradorAsync(CancellationToken ct = default) =>
        _repositorio.ExisteAsync(ct);

    /// <summary>
    /// Crea el administrador inicial en el primer ingreso (CU-08). Falla con
    /// SETUP_YA_COMPLETADO si ya existe (unicidad, RC-06/RN-12), con SETUP_CONTRASENA_DEBIL si
    /// la contraseña no cumple la política (RN-13) y con SETUP_IDENTIFICADOR_INVALIDO si el
    /// identificador es vacío. La contraseña se resguarda como hash PHC, nunca en claro (RN-13).
    /// </summary>
    public async Task<ResultadoAltaAdministrador> CrearAdministradorInicialAsync(
        string usuario, string contrasena, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(usuario))
        {
            return ResultadoAltaAdministrador.Falla(ErrorAltaAdministrador.SetupIdentificadorInvalido);
        }

        // Unicidad de la cuenta: a lo sumo un administrador (RC-06, RN-13).
        if (await _repositorio.ExisteAsync(ct))
        {
            return ResultadoAltaAdministrador.Falla(ErrorAltaAdministrador.SetupYaCompletado);
        }

        // Política mínima de robustez (CU-08 CA-02, RN-13).
        if (!PoliticaContrasena.EsRobusta(contrasena))
        {
            return ResultadoAltaAdministrador.Falla(ErrorAltaAdministrador.SetupContrasenaDebil);
        }

        // La contraseña se resguarda como hash PHC; nunca se guarda en claro (RN-13, ADR-03).
        var resguardo = _hash.Hashear(contrasena);
        var administrador = new Administrador(usuario.Trim(), resguardo, _reloj.Ahora);

        await _repositorio.AgregarAsync(administrador, ct);
        return ResultadoAltaAdministrador.Ok();
    }

    /// <summary>
    /// Verifica las credenciales del administrador contra el resguardo PHC sin comparar la
    /// contraseña en claro (CU-09 paso 4, RN-13). Devuelve true solo si la cuenta existe, el
    /// identificador coincide y la contraseña verifica. No distingue cuál campo falló para no
    /// habilitar enumeración de cuentas (CU-09 AUTH_CREDENCIALES_INVALIDAS).
    /// </summary>
    public async Task<bool> VerificarCredencialesAsync(
        string usuario, string contrasena, CancellationToken ct = default)
    {
        var administrador = await _repositorio.ObtenerAsync(ct);
        if (administrador is null || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
        {
            return false;
        }

        if (!string.Equals(administrador.IdentificadorCuenta, usuario.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        return _hash.Verificar(contrasena, administrador.ResguardoPassword);
    }

    /// <summary>
    /// Autentica al administrador aplicando el control de intentos fallidos (CU-09). Si la clave
    /// está bloqueada por demasiados intentos, rechaza el ingreso con
    /// <see cref="ResultadoAutenticacion.DemasiadosIntentos"/> AUN con credenciales correctas
    /// (CU-09 §6, AUTH_DEMASIADOS_INTENTOS). Un fallo de credenciales suma al contador
    /// (CU-09 CA-02); un ingreso válido lo resetea (CU-09 CA-01). La clave es opaca: el llamador
    /// la compone (por identificador, por IP, o ambos) sin filtrar si el usuario existe (sin
    /// enumeración de cuentas). La contraseña nunca se loguea (RN-13).
    /// </summary>
    /// <param name="usuario">Identificador de cuenta presentado.</param>
    /// <param name="contrasena">Contraseña presentada (solo se usa para verificar).</param>
    /// <param name="claveSeguimiento">
    /// Clave de seguimiento del control de intentos (p. ej. identificador normalizado o IP). Si es
    /// vacía, el control no aplica y solo se verifica la credencial.
    /// </param>
    public async Task<ResultadoAutenticacion> AutenticarAsync(
        string usuario, string contrasena, string claveSeguimiento, CancellationToken ct = default)
    {
        var ahora = _reloj.Ahora;

        // El bloqueo se comprueba ANTES de verificar la credencial: durante el enfriamiento ni
        // siquiera una credencial correcta abre sesión (CU-09 §6, AUTH_DEMASIADOS_INTENTOS).
        if (_controlIntentos.EstaBloqueado(claveSeguimiento, ahora))
        {
            return ResultadoAutenticacion.DemasiadosIntentos;
        }

        if (await VerificarCredencialesAsync(usuario, contrasena, ct))
        {
            // Un ingreso válido resetea el contador de la clave (CU-09 CA-01).
            _controlIntentos.RegistrarExito(claveSeguimiento);
            return ResultadoAutenticacion.Ok;
        }

        // Fallo de credenciales: suma al contador. Si este fallo cierra el límite, el siguiente
        // intento queda bloqueado durante el enfriamiento (CU-09 CA-02 → AUTH_DEMASIADOS_INTENTOS).
        var quedaBloqueado = _controlIntentos.RegistrarFallo(claveSeguimiento, ahora);
        return quedaBloqueado
            ? ResultadoAutenticacion.DemasiadosIntentos
            : ResultadoAutenticacion.CredencialesInvalidas;
    }
}
