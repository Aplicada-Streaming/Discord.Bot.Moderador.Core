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

    [Fact]
    public async Task Recorta_espacios_y_saltos_del_token_al_registrar()
    {
        // Given un token pegado con espacios/saltos alrededor (error común de copiar y pegar): un
        // token con espacios lo rechaza la plataforma al loguear (PRUEBA_TOKEN_INVALIDO).
        var resultado = await _servicio.RegistrarAsync("100000000000000001", $"  {TokenEjemplo}\n");

        resultado.Exito.Should().BeTrue();
        var servidor = await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"));
        // El token guardado descifra al valor SIN espacios (se recortó antes de cifrar).
        _cifrado.Descifrar(servidor!.TokenCifrado).Should().Be(TokenEjemplo);
    }

    [Fact]
    public async Task Rechaza_canal_de_salida_no_numerico_sin_excepcion()
    {
        // Given un canal de salida que no es un snowflake (RN-08). Antes la UI construía el
        // Snowflake del canal y un valor como "log" tiraba ArgumentException y caía el circuito;
        // ahora el servicio lo valida y devuelve un error de dominio, sin excepción.
        var resultado = await _servicio.RegistrarAsync(
            "100000000000000001", TokenEjemplo, snowflakeCanalSalida: "log");

        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorRegistroServidor.CanalIdentificadorInvalido);
        (await _repositorio.ExisteAsync(new Snowflake("100000000000000001"))).Should().BeFalse();
    }

    [Fact]
    public async Task Registra_con_canal_de_salida_valido()
    {
        var resultado = await _servicio.RegistrarAsync(
            "100000000000000001", TokenEjemplo, snowflakeCanalSalida: "400000000000000001");

        resultado.Exito.Should().BeTrue();
        var servidor = await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"));
        servidor!.CanalDeSalida.Should().NotBeNull();
        servidor.CanalDeSalida!.SnowflakeCanal.Valor.Should().Be("400000000000000001");
        servidor.CanalDeSalida.PropositoLogico.Should().Be(CanalDeSalida.PropositoReporteIncidentes);
    }

    [Fact]
    public async Task Elimina_servidor_existente()
    {
        await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);

        var eliminado = await _servicio.EliminarAsync("100000000000000001");

        eliminado.Should().BeTrue();
        (await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"))).Should().BeNull();
    }

    [Fact]
    public async Task Eliminar_servidor_inexistente_devuelve_false()
    {
        var eliminado = await _servicio.EliminarAsync("100000000000000099");

        eliminado.Should().BeFalse();
    }

    [Fact]
    public async Task Eliminar_con_identificador_invalido_devuelve_false_sin_excepcion()
    {
        var eliminado = await _servicio.EliminarAsync("no-es-un-snowflake");

        eliminado.Should().BeFalse();
    }

    [Fact]
    public async Task Actualiza_nombre_y_canal_conservando_el_token_si_va_en_blanco()
    {
        await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);
        var antes = await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"));

        var resultado = await _servicio.ActualizarAsync(
            "100000000000000001", "TUP Programación", nuevoToken: "   ", snowflakeCanalSalida: "400000000000000001");

        resultado.Exito.Should().BeTrue();
        var despues = await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"));
        despues!.NombreDescriptivo.Should().Be("TUP Programación");
        despues.CanalDeSalida!.SnowflakeCanal.Valor.Should().Be("400000000000000001");
        // El token en blanco conserva el actual (RN-14): no cambia.
        despues.TokenCifrado.Should().Be(antes!.TokenCifrado);
    }

    [Fact]
    public async Task Actualiza_el_token_recifrandolo_cuando_se_provee_uno_nuevo()
    {
        await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);

        var resultado = await _servicio.ActualizarAsync(
            "100000000000000001", null, nuevoToken: "nuevo-token-secreto", snowflakeCanalSalida: null);

        resultado.Exito.Should().BeTrue();
        var despues = await _repositorio.ObtenerAsync(new Snowflake("100000000000000001"));
        despues!.TokenCifrado.Should().NotBe("nuevo-token-secreto"); // se guarda cifrado, no en claro
        _cifrado.Descifrar(despues.TokenCifrado).Should().Be("nuevo-token-secreto");
    }

    [Fact]
    public async Task Actualizar_con_canal_invalido_devuelve_error_sin_excepcion()
    {
        await _servicio.RegistrarAsync("100000000000000001", TokenEjemplo);

        var resultado = await _servicio.ActualizarAsync(
            "100000000000000001", "x", nuevoToken: null, snowflakeCanalSalida: "log");

        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorRegistroServidor.CanalIdentificadorInvalido);
    }

    [Fact]
    public async Task Actualizar_un_servidor_inexistente_devuelve_no_encontrado()
    {
        var resultado = await _servicio.ActualizarAsync(
            "100000000000000099", "x", nuevoToken: null, snowflakeCanalSalida: null);

        resultado.Exito.Should().BeFalse();
        resultado.Error.Should().Be(ErrorRegistroServidor.ServidorNoEncontrado);
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

        public Task<bool> EliminarAsync(Snowflake snowflakeServidor, CancellationToken ct = default)
            => Task.FromResult(_porSnowflake.Remove(snowflakeServidor.Valor));

        public Task<bool> ActualizarDatosAsync(
            Snowflake snowflakeServidor,
            string? nombreDescriptivo,
            string? tokenCifrado,
            CanalDeSalida? canalDeSalida,
            CancellationToken ct = default)
        {
            if (!_porSnowflake.TryGetValue(snowflakeServidor.Valor, out var servidor))
            {
                return Task.FromResult(false);
            }

            _porSnowflake[snowflakeServidor.Valor] = new ServidorRegistrado(
                servidor.SnowflakeServidor,
                string.IsNullOrWhiteSpace(tokenCifrado) ? servidor.TokenCifrado : tokenCifrado,
                servidor.EstadoConexion, servidor.EstadoActivacion,
                nombreDescriptivo, servidor.CreadoEn, canalDeSalida);
            return Task.FromResult(true);
        }
    }
}
