using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Gateway;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas de la prueba de configuración previa a la activación (R7, CU-12, RN-16, RC-08).
/// Verifican con el adaptador simulado CONFIGURABLE: token inválido → bloqueante → no activa;
/// permisos/intents faltantes → bloqueante; jerarquía baja → advertencia (no bloquea); todo OK →
/// activa. El servidor solo se activa si no hay chequeos bloqueantes (RN-16).
/// </summary>
public sealed class ServicioPruebaConfiguracionTests
{
    private const string ServidorId = "100000000000000001";

    private readonly AdaptadorGatewaySimulado _adaptador =
        new(NullLogger<AdaptadorGatewaySimulado>.Instance);
    private readonly IRepositorioServidores _repositorio = Substitute.For<IRepositorioServidores>();
    private readonly IServicioCifrado _cifrado = Substitute.For<IServicioCifrado>();

    public ServicioPruebaConfiguracionTests()
    {
        var servidor = new ServidorRegistrado(
            new Snowflake(ServidorId), "token-cifrado", EstadoConexion.Desconectado, EstadoActivacion.Inactivo);
        _repositorio.ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>()).Returns(servidor);
        _repositorio
            .ActualizarEstadoAsync(
                Arg.Any<Snowflake>(), Arg.Any<EstadoActivacion>(), Arg.Any<EstadoConexion>(),
                Arg.Any<CancellationToken>())
            .Returns(true);
        // El cifrado se mockea: descifra el token solo en memoria para la prueba (RN-14).
        _cifrado.Descifrar(Arg.Any<string>()).Returns("token-en-claro");
    }

    private ServicioPruebaConfiguracion CrearServicio() =>
        new(_repositorio, _adaptador, _cifrado, NullLogger<ServicioPruebaConfiguracion>.Instance);

    [Fact]
    public async Task Todo_OK_activa_el_servidor()
    {
        // Given una prueba sin chequeos bloqueantes (resultado por defecto del simulado).
        var servicio = CrearServicio();

        // When se prueba y activa.
        var resultado = await servicio.ProbarYActivarAsync(new Snowflake(ServidorId));

        // Then el servidor se activa (RN-16) y se persiste el estado activo.
        resultado.Activado.Should().BeTrue();
        resultado.Prueba!.PuedeActivar.Should().BeTrue();
        await _repositorio.Received(1).ActualizarEstadoAsync(
            Arg.Any<Snowflake>(), EstadoActivacion.Activo, EstadoConexion.Conectado, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Token_invalido_es_bloqueante_no_activa_y_marca_desconectado()
    {
        // Given una prueba con token inválido (bloqueante, CU-12 CA-03).
        _adaptador.ConfigurarPruebaConfiguracion(new Snowflake(ServidorId), new ResultadoPruebaConfiguracion(new[]
        {
            ChequeoConfiguracion.Bloqueante(
                ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida", "Token revocado"),
        }));
        var servicio = CrearServicio();

        // When se prueba y activa.
        var resultado = await servicio.ProbarYActivarAsync(new Snowflake(ServidorId));

        // Then no se activa (RN-16) y el servidor queda desconectado (CU-13).
        resultado.Activado.Should().BeFalse();
        resultado.Prueba!.PuedeActivar.Should().BeFalse();
        resultado.Prueba.TokenInvalido.Should().BeTrue();
        await _repositorio.Received(1).ActualizarEstadoAsync(
            Arg.Any<Snowflake>(), EstadoActivacion.Inactivo, EstadoConexion.Desconectado, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Permisos_faltantes_es_bloqueante_no_activa()
    {
        // Given una prueba con permisos faltantes (bloqueante, CU-12 CA-02, RN-01).
        _adaptador.ConfigurarPruebaConfiguracion(new Snowflake(ServidorId), new ResultadoPruebaConfiguracion(new[]
        {
            ChequeoConfiguracion.Superado(ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida"),
            ChequeoConfiguracion.Bloqueante(
                ResultadoPruebaConfiguracion.CodigoPermisosFaltantes, "Permisos requeridos presentes",
                "Falta Banear miembros"),
        }));
        var servicio = CrearServicio();

        var resultado = await servicio.ProbarYActivarAsync(new Snowflake(ServidorId));

        resultado.Activado.Should().BeFalse();
        resultado.Prueba!.Bloqueantes.Should().ContainSingle()
            .Which.Codigo.Should().Be(ResultadoPruebaConfiguracion.CodigoPermisosFaltantes);
    }

    [Fact]
    public async Task Intents_faltantes_es_bloqueante_no_activa()
    {
        // Given una prueba con intents faltantes (bloqueante, CU-12).
        _adaptador.ConfigurarPruebaConfiguracion(new Snowflake(ServidorId), new ResultadoPruebaConfiguracion(new[]
        {
            ChequeoConfiguracion.Superado(ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida"),
            ChequeoConfiguracion.Bloqueante(
                ResultadoPruebaConfiguracion.CodigoIntentsFaltantes, "Intents habilitados", "Falta MessageContent"),
        }));
        var servicio = CrearServicio();

        var resultado = await servicio.ProbarYActivarAsync(new Snowflake(ServidorId));

        resultado.Activado.Should().BeFalse();
    }

    [Fact]
    public async Task Jerarquia_baja_es_advertencia_no_bloquea_y_activa()
    {
        // Given una prueba con todos OK salvo una ADVERTENCIA de jerarquía (CU-12 CA-04, RN-01).
        _adaptador.ConfigurarPruebaConfiguracion(new Snowflake(ServidorId), new ResultadoPruebaConfiguracion(new[]
        {
            ChequeoConfiguracion.Superado(ResultadoPruebaConfiguracion.CodigoTokenInvalido, "Credencial válida"),
            ChequeoConfiguracion.Superado(
                ResultadoPruebaConfiguracion.CodigoPermisosFaltantes, "Permisos requeridos presentes"),
            ChequeoConfiguracion.Advertencia(
                ResultadoPruebaConfiguracion.CodigoJerarquiaInsuficiente, "Jerarquía de roles suficiente",
                "1 rol por encima del bot"),
        }));
        var servicio = CrearServicio();

        var resultado = await servicio.ProbarYActivarAsync(new Snowflake(ServidorId));

        // Then la advertencia NO bloquea: el servidor se activa pero la advertencia queda visible.
        resultado.Activado.Should().BeTrue();
        resultado.Prueba!.Advertencias.Should().ContainSingle()
            .Which.Codigo.Should().Be(ResultadoPruebaConfiguracion.CodigoJerarquiaInsuficiente);
        resultado.Prueba.Bloqueantes.Should().BeEmpty();
    }

    [Fact]
    public async Task Probar_sin_activar_no_activa_aunque_supere()
    {
        // Given una prueba que superaría todos los chequeos.
        var servicio = CrearServicio();

        // When solo se PRUEBA (sin activar): el panel previsualiza los chequeos antes de activar.
        var resultado = await servicio.ProbarAsync(new Snowflake(ServidorId));

        // Then la prueba indica que podría activar, pero no se activó todavía.
        resultado.Prueba!.PuedeActivar.Should().BeTrue();
        resultado.Activado.Should().BeFalse();
        await _repositorio.DidNotReceive().ActualizarEstadoAsync(
            Arg.Any<Snowflake>(), EstadoActivacion.Activo, Arg.Any<EstadoConexion>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Servidor_inexistente_devuelve_no_encontrado()
    {
        _repositorio
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns((ServidorRegistrado?)null);
        var servicio = CrearServicio();

        var resultado = await servicio.ProbarYActivarAsync(new Snowflake(ServidorId));

        resultado.ServidorEncontrado.Should().BeFalse();
        resultado.Activado.Should().BeFalse();
    }

    [Fact]
    public async Task Enviar_mensaje_de_prueba_con_canal_designado_confirma_el_envio()
    {
        // Given un servidor con canal de reportes designado.
        var conCanal = new ServidorRegistrado(
            new Snowflake(ServidorId), "token-cifrado", EstadoConexion.Desconectado, EstadoActivacion.Inactivo,
            nombreDescriptivo: null, creadoEn: null,
            canalDeSalida: new CanalDeSalida(
                new Snowflake("400000000000000001"), CanalDeSalida.PropositoReporteIncidentes));
        _repositorio.ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>()).Returns(conCanal);
        var servicio = CrearServicio();

        // When se envía el mensaje de prueba con el adaptador SIMULADO (no hay red).
        var resultado = await servicio.EnviarMensajePruebaAsync(new Snowflake(ServidorId));

        // Then no falla, pero se marca como SIMULADO: el mensaje NO llegó a Discord, solo se
        // registró. La UI debe ser honesta al respecto (CU-05).
        resultado.Exito.Should().BeTrue();
        resultado.Simulado.Should().BeTrue();
        resultado.Mensaje.Should().Contain("400000000000000001");
    }

    [Fact]
    public async Task Enviar_mensaje_de_prueba_sin_canal_designado_falla_con_mensaje_claro()
    {
        // El servidor por defecto del fixture no tiene canal designado (CU-05 CA-03).
        var servicio = CrearServicio();

        var resultado = await servicio.EnviarMensajePruebaAsync(new Snowflake(ServidorId));

        resultado.Exito.Should().BeFalse();
        resultado.Mensaje.Should().Contain("canal");
    }

    [Fact]
    public async Task Enviar_mensaje_de_prueba_a_servidor_inexistente_falla()
    {
        _repositorio
            .ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>())
            .Returns((ServidorRegistrado?)null);
        var servicio = CrearServicio();

        var resultado = await servicio.EnviarMensajePruebaAsync(new Snowflake(ServidorId));

        resultado.Exito.Should().BeFalse();
    }
}
