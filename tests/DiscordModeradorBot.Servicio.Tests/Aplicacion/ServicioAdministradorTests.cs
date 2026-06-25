using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio.Administracion;
using DiscordModeradorBot.Servicio.Infraestructura.Seguridad;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del servicio del administrador único (CU-08 alta, CU-09 autenticación; RC-06, RN-13).
/// El first-run crea la cuenta única; un segundo alta se rechaza (unicidad, TC-32); las
/// credenciales se verifican contra el resguardo PHC sin comparar en claro (TC-33/TC-34); una
/// contraseña débil se rechaza (TC-31).
/// </summary>
public sealed class ServicioAdministradorTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string Usuario = "admin";
    private const string ContrasenaRobusta = "una-clave-robusta-123";

    private readonly RepositorioAdministradorEnMemoria _repositorio = new();
    // Hash real (con iteraciones reducidas) para ejercitar el resguardo PHC de punta a punta.
    private readonly ServicioHashContrasenaPbkdf2 _hash = new(iteraciones: 1_000);
    private readonly RelojFijo _reloj = new(Base);
    private readonly ControlIntentosAutenticacion _control = new();

    private ServicioAdministrador CrearServicio() => new(_repositorio, _hash, _reloj, _control);

    [Fact]
    public async Task First_run_crea_el_administrador_con_resguardo_PHC()
    {
        // Given un sistema sin cuenta (first-run, CU-08 CA-01).
        var servicio = CrearServicio();
        (await servicio.ExisteAdministradorAsync()).Should().BeFalse();

        // When se crea el administrador inicial con una contraseña robusta.
        var resultado = await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // Then la cuenta única se crea y la contraseña se resguarda como hash PHC, nunca en claro.
        resultado.Exito.Should().BeTrue();
        (await servicio.ExisteAdministradorAsync()).Should().BeTrue();
        var administrador = await _repositorio.ObtenerAsync();
        administrador.Should().NotBeNull();
        administrador!.IdentificadorCuenta.Should().Be(Usuario);
        administrador.ResguardoPassword.Should().StartWith($"${ServicioHashContrasenaPbkdf2.Algoritmo}$");
        administrador.ResguardoPassword.Should().NotContain(ContrasenaRobusta);
    }

    [Fact]
    public async Task Un_segundo_alta_se_rechaza_por_unicidad()
    {
        // Given un sistema con la cuenta ya creada (TC-32, CU-08 CA-03, RC-06).
        var servicio = CrearServicio();
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // When se intenta crear una segunda cuenta.
        var resultado = await servicio.CrearAdministradorInicialAsync("otro", "otra-clave-robusta-456");

        // Then se bloquea (SETUP_YA_COMPLETADO) y sigue habiendo a lo sumo una cuenta.
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorAltaAdministrador.SetupYaCompletado);
        (await _repositorio.ContarAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Una_contrasena_debil_se_rechaza()
    {
        // Given un sistema en first-run (TC-31, CU-08 CA-02, RN-13).
        var servicio = CrearServicio();

        // When se intenta crear la cuenta con una contraseña que no cumple la política.
        var resultado = await servicio.CrearAdministradorInicialAsync(Usuario, "corta");

        // Then se rechaza con SETUP_CONTRASENA_DEBIL y no se crea cuenta.
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorAltaAdministrador.SetupContrasenaDebil);
        (await servicio.ExisteAdministradorAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Verificar_credenciales_correctas_devuelve_true()
    {
        // Given un administrador dado de alta (TC-33, CU-09 CA-01).
        var servicio = CrearServicio();
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // When se verifican credenciales correctas.
        var ok = await servicio.VerificarCredencialesAsync(Usuario, ContrasenaRobusta);

        // Then verifica (contra el resguardo PHC, sin comparar en claro).
        ok.Should().BeTrue();
    }

    [Fact]
    public async Task Verificar_credenciales_incorrectas_devuelve_false()
    {
        // Given un administrador dado de alta (TC-34, CU-09 CA-02).
        var servicio = CrearServicio();
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // When se verifica con contraseña incorrecta o usuario incorrecto.
        var contrasenaMal = await servicio.VerificarCredencialesAsync(Usuario, "clave-equivocada-999");
        var usuarioMal = await servicio.VerificarCredencialesAsync("otro", ContrasenaRobusta);

        // Then ambos casos fallan, sin distinguir cuál campo falló (sin enumeración).
        contrasenaMal.Should().BeFalse();
        usuarioMal.Should().BeFalse();
    }

    [Fact]
    public async Task Verificar_sin_cuenta_devuelve_false()
    {
        // Given un sistema sin cuenta (CU-09 CA-04).
        var servicio = CrearServicio();

        // When se intenta verificar.
        var ok = await servicio.VerificarCredencialesAsync(Usuario, ContrasenaRobusta);

        // Then no verifica (no hay cuenta contra la cual verificar).
        ok.Should().BeFalse();
    }

    [Fact]
    public async Task Autenticar_con_credenciales_correctas_devuelve_Ok()
    {
        // Given un administrador dado de alta (CU-09 CA-01).
        var servicio = CrearServicioConControl(out _);
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // When se autentica con credenciales correctas.
        var resultado = await servicio.AutenticarAsync(Usuario, ContrasenaRobusta, ClaveSeguimiento);

        // Then abre sesión (Ok).
        resultado.Should().Be(ResultadoAutenticacion.Ok);
    }

    [Fact]
    public async Task Tras_N_fallos_el_siguiente_intento_se_bloquea_aun_con_credenciales_correctas()
    {
        // Given un administrador dado de alta y un control de 3 intentos / 15 min de enfriamiento.
        var servicio = CrearServicioConControl(
            out _, maximoIntentos: 3, enfriamiento: TimeSpan.FromMinutes(15));
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // When se agotan los intentos con credenciales incorrectas (CU-09 CA-02).
        for (var i = 0; i < 3; i++)
        {
            var fallo = await servicio.AutenticarAsync(Usuario, "clave-incorrecta-000", ClaveSeguimiento);
            // El tercer fallo cierra el límite y ya devuelve DemasiadosIntentos.
            (fallo == ResultadoAutenticacion.CredencialesInvalidas
                || fallo == ResultadoAutenticacion.DemasiadosIntentos).Should().BeTrue();
        }

        // Then aun con la contraseña CORRECTA el ingreso queda bloqueado (AUTH_DEMASIADOS_INTENTOS).
        var bloqueado = await servicio.AutenticarAsync(Usuario, ContrasenaRobusta, ClaveSeguimiento);
        bloqueado.Should().Be(ResultadoAutenticacion.DemasiadosIntentos);
    }

    [Fact]
    public async Task Pasado_el_enfriamiento_se_permite_de_nuevo()
    {
        // Given una cuenta y el control agotado (bloqueado).
        var servicio = CrearServicioConControl(
            out _, maximoIntentos: 3, enfriamiento: TimeSpan.FromMinutes(15));
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        for (var i = 0; i < 3; i++)
        {
            await servicio.AutenticarAsync(Usuario, "clave-incorrecta-000", ClaveSeguimiento);
        }

        (await servicio.AutenticarAsync(Usuario, ContrasenaRobusta, ClaveSeguimiento))
            .Should().Be(ResultadoAutenticacion.DemasiadosIntentos);

        // When avanza el reloj más allá del enfriamiento (reloj inyectado, sin pausas reales).
        _reloj.Ahora = Base.AddMinutes(16);

        // Then con credenciales correctas vuelve a abrir sesión (Ok).
        (await servicio.AutenticarAsync(Usuario, ContrasenaRobusta, ClaveSeguimiento))
            .Should().Be(ResultadoAutenticacion.Ok);
    }

    [Fact]
    public async Task Un_login_exitoso_antes_del_limite_resetea_el_contador()
    {
        // Given una cuenta y el control de 3 intentos.
        var servicio = CrearServicioConControl(out _, maximoIntentos: 3);
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);

        // When hay 2 fallos (sin alcanzar el límite) y luego un login exitoso (resetea, CU-09 CA-01).
        await servicio.AutenticarAsync(Usuario, "clave-incorrecta-000", ClaveSeguimiento);
        await servicio.AutenticarAsync(Usuario, "clave-incorrecta-001", ClaveSeguimiento);
        (await servicio.AutenticarAsync(Usuario, ContrasenaRobusta, ClaveSeguimiento))
            .Should().Be(ResultadoAutenticacion.Ok);

        // Then tras el reset hacen falta 3 fallos nuevos para bloquear: 2 más siguen sin bloquear.
        (await servicio.AutenticarAsync(Usuario, "clave-incorrecta-002", ClaveSeguimiento))
            .Should().Be(ResultadoAutenticacion.CredencialesInvalidas);
        (await servicio.AutenticarAsync(Usuario, "clave-incorrecta-003", ClaveSeguimiento))
            .Should().Be(ResultadoAutenticacion.CredencialesInvalidas);
    }

    [Fact]
    public async Task Cambiar_contrasena_con_actual_correcta_y_nueva_robusta_actualiza_el_resguardo()
    {
        // Given un administrador dado de alta (RN-13).
        var servicio = CrearServicio();
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);
        var resguardoPrevio = (await _repositorio.ObtenerAsync())!.ResguardoPassword;

        // When cambia la contraseña con la actual correcta y una nueva robusta.
        const string nueva = "otra-clave-robusta-456";
        var resultado = await servicio.CambiarContrasenaAsync(ContrasenaRobusta, nueva);

        // Then se actualiza el resguardo (nuevo hash) y la nueva contraseña verifica; la vieja no.
        resultado.Exito.Should().BeTrue();
        var resguardoNuevo = (await _repositorio.ObtenerAsync())!.ResguardoPassword;
        resguardoNuevo.Should().NotBe(resguardoPrevio);
        resguardoNuevo.Should().NotContain(nueva);
        (await servicio.VerificarCredencialesAsync(Usuario, nueva)).Should().BeTrue();
        (await servicio.VerificarCredencialesAsync(Usuario, ContrasenaRobusta)).Should().BeFalse();
    }

    [Fact]
    public async Task Cambiar_contrasena_con_actual_incorrecta_se_rechaza_y_no_cambia_nada()
    {
        // Given un administrador dado de alta.
        var servicio = CrearServicio();
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);
        var resguardoPrevio = (await _repositorio.ObtenerAsync())!.ResguardoPassword;

        // When intenta cambiarla con una contraseña actual equivocada.
        var resultado = await servicio.CambiarContrasenaAsync("clave-equivocada-999", "otra-clave-robusta-456");

        // Then se rechaza (ContrasenaActualInvalida) y el resguardo no cambia.
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorCambioContrasena.ContrasenaActualInvalida);
        (await _repositorio.ObtenerAsync())!.ResguardoPassword.Should().Be(resguardoPrevio);
    }

    [Fact]
    public async Task Cambiar_contrasena_con_nueva_debil_se_rechaza_y_no_cambia_nada()
    {
        // Given un administrador dado de alta.
        var servicio = CrearServicio();
        await servicio.CrearAdministradorInicialAsync(Usuario, ContrasenaRobusta);
        var resguardoPrevio = (await _repositorio.ObtenerAsync())!.ResguardoPassword;

        // When intenta cambiarla con la actual correcta pero una nueva que no cumple la política.
        var resultado = await servicio.CambiarContrasenaAsync(ContrasenaRobusta, "corta");

        // Then se rechaza (ContrasenaNuevaDebil) y el resguardo no cambia (RN-13).
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorCambioContrasena.ContrasenaNuevaDebil);
        (await _repositorio.ObtenerAsync())!.ResguardoPassword.Should().Be(resguardoPrevio);
    }

    private const string ClaveSeguimiento = "admin|127.0.0.1";

    private ServicioAdministrador CrearServicioConControl(
        out ControlIntentosAutenticacion control,
        int maximoIntentos = 5,
        TimeSpan? enfriamiento = null)
    {
        control = new ControlIntentosAutenticacion(
            maximoIntentos: maximoIntentos,
            ventana: TimeSpan.FromMinutes(15),
            enfriamiento: enfriamiento ?? TimeSpan.FromMinutes(15));
        return new ServicioAdministrador(_repositorio, _hash, _reloj, control);
    }

    /// <summary>
    /// Doble en memoria del repositorio del administrador que modela la unicidad de la cuenta
    /// (a lo sumo una fila, RC-06) y asigna identidad incremental.
    /// </summary>
    private sealed class RepositorioAdministradorEnMemoria : IRepositorioAdministrador
    {
        private Administrador? _administrador;
        private int _proximoId = 1;

        public Task<bool> ExisteAsync(CancellationToken ct = default) =>
            Task.FromResult(_administrador is not null);

        public Task<Administrador?> ObtenerAsync(CancellationToken ct = default) =>
            Task.FromResult(_administrador);

        public Task<Administrador> AgregarAsync(Administrador administrador, CancellationToken ct = default)
        {
            if (_administrador is not null)
            {
                throw new InvalidOperationException("Ya existe un administrador (RC-06).");
            }

            _administrador = new Administrador(
                administrador.IdentificadorCuenta,
                administrador.ResguardoPassword,
                administrador.CreadoEn,
                _proximoId++);
            return Task.FromResult(_administrador);
        }

        public Task ActualizarAsync(Administrador administrador, CancellationToken ct = default)
        {
            if (_administrador is null || _administrador.Id != administrador.Id)
            {
                throw new InvalidOperationException("No existe el administrador a actualizar (RC-06).");
            }

            _administrador = new Administrador(
                administrador.IdentificadorCuenta,
                administrador.ResguardoPassword,
                administrador.CreadoEn,
                administrador.Id);
            return Task.CompletedTask;
        }

        public Task<int> ContarAsync() => Task.FromResult(_administrador is null ? 0 : 1);
    }
}
