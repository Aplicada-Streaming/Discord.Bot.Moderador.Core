using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del gestor de conexiones de gateway (ADR-13, CU-13) con el puerto y los clientes
/// MOCKEADOS (NSubstitute), sin red ni token real. Verifican que al activar/arrancar un servidor se
/// intenta conectar con su token DESCIFRADO (RN-14) y que un mensaje entrante se enruta al pipeline
/// (al motor). El motor real no se construye: el ruteo se inyecta como manejador para aislar el
/// gestor.
/// </summary>
public sealed class GestorConexionesGatewayTests
{
    private const string ServidorId = "100000000000000001";
    private const string TokenCifrado = "token-cifrado";
    private const string TokenEnClaro = "token-en-claro-de-prueba";

    private readonly IRepositorioServidores _repositorio = Substitute.For<IRepositorioServidores>();
    private readonly IServicioCifrado _cifrado = Substitute.For<IServicioCifrado>();
    private readonly IFabricaClienteGateway _fabrica = Substitute.For<IFabricaClienteGateway>();
    private readonly IEstadoConexionGateway _estado = Substitute.For<IEstadoConexionGateway>();
    private readonly IClienteGatewayServidor _cliente = Substitute.For<IClienteGatewayServidor>();

    public GestorConexionesGatewayTests()
    {
        _cifrado.Descifrar(TokenCifrado).Returns(TokenEnClaro);
        _fabrica.Crear(Arg.Any<Snowflake>()).Returns(_cliente);

        var servidor = new ServidorRegistrado(
            new Snowflake(ServidorId), TokenCifrado, EstadoConexion.Desconectado, EstadoActivacion.Activo);
        _repositorio.ObtenerAsync(Arg.Any<Snowflake>(), Arg.Any<CancellationToken>()).Returns(servidor);
        _repositorio.ListarAsync(Arg.Any<CancellationToken>())
            .Returns(new List<ServidorRegistrado> { servidor });
    }

    private GestorConexionesGateway CrearGestor() =>
        new(_repositorio, _cifrado, _fabrica, _estado, NullLogger<GestorConexionesGateway>.Instance);

    [Fact]
    public async Task Activar_un_servidor_conecta_con_el_token_descifrado()
    {
        // Given el gestor con un servidor registrado y activo.
        var gestor = CrearGestor();

        // When se activa el servidor.
        await gestor.ActivarServidorAsync(new Snowflake(ServidorId));

        // Then se creó la conexión del contexto y se conectó con el token DESCIFRADO (RN-14), nunca
        // el cifrado.
        _fabrica.Received(1).Crear(Arg.Is<Snowflake>(s => s.Valor == ServidorId));
        await _cliente.Received(1).ConectarAsync(TokenEnClaro, Arg.Any<CancellationToken>());
        await _cliente.DidNotReceive().ConectarAsync(TokenCifrado, Arg.Any<CancellationToken>());
        gestor.ServidoresConectados.Should().ContainSingle().Which.Valor.Should().Be(ServidorId);
    }

    [Fact]
    public async Task Iniciar_solo_conecta_los_servidores_activos()
    {
        // Given un servidor activo y otro inactivo.
        var activo = new ServidorRegistrado(
            new Snowflake(ServidorId), TokenCifrado, EstadoConexion.Desconectado, EstadoActivacion.Activo);
        var inactivo = new ServidorRegistrado(
            new Snowflake("100000000000000002"), TokenCifrado, EstadoConexion.Desconectado,
            EstadoActivacion.Inactivo);
        _repositorio.ListarAsync(Arg.Any<CancellationToken>())
            .Returns(new List<ServidorRegistrado> { activo, inactivo });

        var gestor = CrearGestor();

        // When inicia.
        await gestor.IniciarAsync();

        // Then solo se conectó el activo (RN-16).
        gestor.ServidoresConectados.Should().ContainSingle().Which.Valor.Should().Be(ServidorId);
    }

    [Fact]
    public async Task Un_mensaje_entrante_se_enruta_al_motor()
    {
        // Given el gestor con un manejador de ruteo (que representa el pipeline del motor).
        var gestor = CrearGestor();
        Snowflake? servidorEnrutado = null;
        MensajeEntrante? mensajeEnrutado = null;
        gestor.EnrutadorMensaje = (servidor, mensaje, _) =>
        {
            servidorEnrutado = servidor;
            mensajeEnrutado = mensaje;
            return Task.CompletedTask;
        };

        await gestor.ActivarServidorAsync(new Snowflake(ServidorId));

        // When el cliente del contexto emite un mensaje (se levanta el evento suscripto). El gestor
        // ruteó síncronamente al manejador inyectado, que captura el contexto y el mensaje.
        var mensaje = new MensajeEntrante(
            new Snowflake(ServidorId),
            new Snowflake("300000000000000001"),
            new Snowflake("200000000000000002"),
            new Snowflake("400000000000000003"),
            DateTimeOffset.UnixEpoch,
            "hola");
        _cliente.MensajeRecibido += Raise.Event<Func<MensajeEntrante, Task>>(mensaje);

        // Then el mensaje se enrutó al pipeline con el contexto correcto (ADR-13).
        servidorEnrutado!.Value.Valor.Should().Be(ServidorId);
        mensajeEnrutado.Should().BeSameAs(mensaje);
    }

    [Fact]
    public async Task Desactivar_un_servidor_cierra_su_conexion()
    {
        // Given un servidor conectado.
        var gestor = CrearGestor();
        await gestor.ActivarServidorAsync(new Snowflake(ServidorId));

        // When se desactiva.
        await gestor.DesactivarServidorAsync(new Snowflake(ServidorId));

        // Then la conexión se detuvo y ya no figura activa.
        await _cliente.Received(1).DetenerAsync(Arg.Any<CancellationToken>());
        gestor.ServidoresConectados.Should().BeEmpty();
    }

    [Fact]
    public async Task Activar_dos_veces_no_abre_una_segunda_conexion()
    {
        // Given un servidor ya conectado.
        var gestor = CrearGestor();
        await gestor.ActivarServidorAsync(new Snowflake(ServidorId));

        // When se vuelve a activar.
        await gestor.ActivarServidorAsync(new Snowflake(ServidorId));

        // Then no se crea una segunda conexión (idempotente).
        _fabrica.Received(1).Crear(Arg.Any<Snowflake>());
    }
}
