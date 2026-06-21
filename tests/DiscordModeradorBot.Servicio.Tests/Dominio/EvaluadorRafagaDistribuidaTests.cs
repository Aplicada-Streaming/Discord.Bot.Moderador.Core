using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas del módulo crítico de detección (CU-01, ≥ 90 % cobertura). El discriminador es
/// la cantidad de canales DISTINTOS dentro de la ventana, no el volumen de mensajes.
/// </summary>
public sealed class EvaluadorRafagaDistribuidaTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly EstadoConductaEnMemoria _estado = new();
    private readonly EvaluadorRafagaDistribuida _evaluador = new();

    [Fact]
    public void Tres_canales_distintos_con_umbral_3_coincide()
    {
        // Given una regla de ráfaga con umbral 3 y ventana 2 s (defaults del descriptor),
        // y un usuario que publica en 3 canales distintos en 1,5 s (TC-01 / CA-01).
        var c1 = MensajesDePrueba.Crear("300000000000000001", Base);
        var c2 = MensajesDePrueba.Crear("300000000000000002", Base.AddMilliseconds(700));
        var c3 = MensajesDePrueba.Crear("300000000000000003", Base.AddMilliseconds(1500));

        _estado.RegistrarActividad(c1);
        _estado.RegistrarActividad(c2);
        _estado.RegistrarActividad(c3);

        // When se evalúa el último mensaje.
        var resultado = _evaluador.Evaluar(c3, _estado, ahora: c3.Instante);

        // Then la condición de ráfaga se marca como cumplida y la cuenta es 3.
        resultado.Coincide.Should().BeTrue();
        resultado.CanalesDistintos.Should().Be(3);
        resultado.Umbral.Should().Be(3);
    }

    [Fact]
    public void Dos_canales_distintos_con_umbral_3_no_coincide()
    {
        // Given la misma regla (umbral 3) y un usuario que publica en solo 2 canales.
        var c1 = MensajesDePrueba.Crear("300000000000000001", Base);
        var c2 = MensajesDePrueba.Crear("300000000000000002", Base.AddMilliseconds(500));

        _estado.RegistrarActividad(c1);
        _estado.RegistrarActividad(c2);

        // When se evalúa.
        var resultado = _evaluador.Evaluar(c2, _estado, ahora: c2.Instante);

        // Then no coincide; la cuenta de canales distintos es 2.
        resultado.Coincide.Should().BeFalse();
        resultado.CanalesDistintos.Should().Be(2);
    }

    [Fact]
    public void Muchos_mensajes_en_un_solo_canal_no_coincide()
    {
        // Given el usuario publica 10 mensajes pero todos en el MISMO canal (TC-02 / CA-02).
        DateTimeOffset ultimo = Base;
        for (var i = 0; i < 10; i++)
        {
            ultimo = Base.AddMilliseconds(i * 100);
            _estado.RegistrarActividad(MensajesDePrueba.Crear("300000000000000001", ultimo));
        }

        var ultimoMensaje = MensajesDePrueba.Crear("300000000000000001", ultimo);

        // When se evalúa.
        var resultado = _evaluador.Evaluar(ultimoMensaje, _estado, ahora: ultimo);

        // Then NO coincide: el discriminador es canales distintos, no volumen. Cuenta = 1.
        resultado.Coincide.Should().BeFalse();
        resultado.CanalesDistintos.Should().Be(1);
    }

    [Fact]
    public void Actividad_fuera_de_la_ventana_no_cuenta()
    {
        // Given un usuario publica en 3 canales, pero el primero está FUERA de la ventana
        // de 2 s respecto del instante de evaluación (expiración por reloj inyectado).
        _estado.RegistrarActividad(MensajesDePrueba.Crear("300000000000000001", Base)); // t=0, expira.
        _estado.RegistrarActividad(MensajesDePrueba.Crear("300000000000000002", Base.AddSeconds(2.5)));
        var c3 = MensajesDePrueba.Crear("300000000000000003", Base.AddSeconds(3.0));
        _estado.RegistrarActividad(c3);

        // When se evalúa en t=3 s con ventana de 2 s: solo entran los canales 2 y 3.
        var resultado = _evaluador.Evaluar(c3, _estado, ahora: Base.AddSeconds(3.0));

        // Then la cuenta es 2 (el canal 1 expiró) y no coincide con umbral 3.
        resultado.CanalesDistintos.Should().Be(2);
        resultado.Coincide.Should().BeFalse();
    }

    [Fact]
    public void Ventana_ampliada_captura_fan_out_espaciado()
    {
        // Given umbral 3 y ventana ampliada a 6 s; 3 canales distintos a lo largo de 5 s
        // (TC-03 / CA-03). El valor configurado se toma del parámetro, no hardcodeado.
        _estado.RegistrarActividad(MensajesDePrueba.Crear("300000000000000001", Base));
        _estado.RegistrarActividad(MensajesDePrueba.Crear("300000000000000002", Base.AddSeconds(2.5)));
        var c3 = MensajesDePrueba.Crear("300000000000000003", Base.AddSeconds(5.0));
        _estado.RegistrarActividad(c3);

        // When se evalúa con ventana de 6 s.
        var resultado = _evaluador.Evaluar(
            c3, _estado, ahora: Base.AddSeconds(5.0),
            umbralConfigurado: null, ventanaSegundosConfigurada: 6.0);

        // Then la ventana mayor captura los 3 canales y coincide.
        resultado.CanalesDistintos.Should().Be(3);
        resultado.Coincide.Should().BeTrue();
    }

    [Fact]
    public void Umbral_fuera_de_limites_usa_el_default_del_descriptor()
    {
        // Given un umbral configurado inválido (1, por debajo del mínimo 2): RN-10 aplica
        // el default del descriptor (3). 3 canales distintos.
        var c1 = MensajesDePrueba.Crear("300000000000000001", Base);
        var c2 = MensajesDePrueba.Crear("300000000000000002", Base.AddMilliseconds(300));
        var c3 = MensajesDePrueba.Crear("300000000000000003", Base.AddMilliseconds(600));
        _estado.RegistrarActividad(c1);
        _estado.RegistrarActividad(c2);
        _estado.RegistrarActividad(c3);

        // When se evalúa con umbral inválido.
        var resultado = _evaluador.Evaluar(
            c3, _estado, ahora: c3.Instante, umbralConfigurado: 1, ventanaSegundosConfigurada: null);

        // Then se aplica el default (3) y coincide con 3 canales.
        resultado.Umbral.Should().Be(3);
        resultado.Coincide.Should().BeTrue();
    }
}
