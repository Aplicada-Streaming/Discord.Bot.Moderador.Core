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

    private ServicioAdministrador CrearServicio() => new(_repositorio, _hash, _reloj);

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

        public Task<int> ContarAsync() => Task.FromResult(_administrador is null ? 0 : 1);
    }
}
