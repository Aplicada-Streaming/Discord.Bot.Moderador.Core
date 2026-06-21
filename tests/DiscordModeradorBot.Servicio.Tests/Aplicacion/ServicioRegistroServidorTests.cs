using DiscordModeradorBot.Servicio.Aplicacion;
using DiscordModeradorBot.Servicio.Aplicacion.Puertos;
using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Servidores;
using DiscordModeradorBot.Servicio.Infraestructura.Seguridad;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Aplicacion;

/// <summary>
/// Pruebas del registro de servidor (CU-10): token cifrado en reposo, unicidad por
/// snowflake (RN-08), validaciones de formato y token vacío.
/// </summary>
public sealed class ServicioRegistroServidorTests
{
    private const string TokenEjemplo = "token-de-bot-de-prueba";

    private readonly RepositorioServidoresEnMemoria _repositorio = new();
    private readonly ServicioCifradoAes _cifrado = new("clave-de-prueba");
    private readonly ServicioRegistroServidor _servicio;

    public ServicioRegistroServidorTests()
    {
        _servicio = new ServicioRegistroServidor(
            _repositorio, _cifrado, new RelojFijo(new DateTimeOffset(2026, 6, 20, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public async Task Registra_servidor_con_token_cifrado_en_reposo()
    {
        // Given un servidor no registrado (TC-35 / CA-01).
        var resultado = await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);

        // Then se persiste y el token en reposo está cifrado (nunca en claro, RN-14).
        resultado.Exito.Should().BeTrue();
        var servidor = await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"));
        servidor.Should().NotBeNull();
        servidor!.TokenCifrado.Should().NotBe(TokenEjemplo);
        servidor.EstadoActivacion.Should().Be(EstadoActivacion.Inactivo);
        _cifrado.Descifrar(servidor.TokenCifrado).Should().Be(TokenEjemplo);
    }

    [Fact]
    public async Task Rechaza_servidor_duplicado()
    {
        // Given un servidor ya registrado (TC-36 / CA-02).
        await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);

        // When se intenta registrar el mismo snowflake.
        var resultado = await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);

        // Then se rechaza con ServidorYaRegistrado.
        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorRegistroServidor.ServidorYaRegistrado);
    }

    [Fact]
    public async Task Rechaza_identificador_invalido()
    {
        // Given un identificador que no es un snowflake (RN-08).
        var resultado = await _servicio.RegistrarAsync("no-es-un-snowflake", TokenEjemplo);

        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorRegistroServidor.ServidorIdentificadorInvalido);
    }

    [Fact]
    public async Task Rechaza_token_vacio()
    {
        // Given un token vacío (TC-37 / CA-03).
        var resultado = await _servicio.RegistrarAsync("100000000000000001", "   ");

        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorRegistroServidor.ServidorTokenVacio);
    }

    /// <summary>Repositorio en memoria para aislar el servicio del ORM.</summary>
    private sealed class RepositorioServidoresEnMemoria : IRepositorioServidores
    {
        private readonly Dictionary<string, ServidorRegistrado> _porSnowflake = new();

        public Task<bool> ExisteAsync(Snowflake snowflakeServidor, CancellationToken ct = default)
            => Task.FromResult(_porSnowflake.ContainsKey(snowflakeServidor.Valor));

        public Task AgregarAsync(ServidorRegistrado servidor, CancellationToken ct = default)
        {
            _porSnowflake[servidor.SnowflakeServidor.Valor] = servidor;
            return Task.CompletedTask;
        }

        public Task<ServidorRegistrado?> ObtenerAsync(Snowflake snowflakeServidor, CancellationToken ct = default)
            => Task.FromResult(_porSnowflake.GetValueOrDefault(snowflakeServidor.Valor));

        public Task<IReadOnlyList<ServidorRegistrado>> ListarAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ServidorRegistrado>>(_porSnowflake.Values.ToList());

        public Task<bool> ActualizarEstadoAsync(
            Snowflake snowflakeServidor,
            DiscordModeradorBot.Servicio.Dominio.Servidores.EstadoActivacion estadoActivacion,
            DiscordModeradorBot.Servicio.Dominio.Servidores.EstadoConexion estadoConexion,
            CancellationToken ct = default)
        {
            if (!_porSnowflake.TryGetValue(snowflakeServidor.Valor, out var servidor))
            {
                return Task.FromResult(false);
            }

            _porSnowflake[snowflakeServidor.Valor] = new ServidorRegistrado(
                servidor.SnowflakeServidor, servidor.TokenCifrado, estadoConexion, estadoActivacion,
                servidor.NombreDescriptivo, servidor.CreadoEn, servidor.CanalDeSalida);
            return Task.FromResult(true);
        }
    }
}
