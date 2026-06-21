using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Contenido;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas del evaluador de reglas de contenido (CU-04, R3): predicado SIN estado por expresión
/// regular. Cubre coincidencia/no coincidencia, sensibilidad a mayúsculas, validación del patrón
/// al guardar (RN-03) y el tope de tiempo de evaluación ante retroceso catastrófico (ADR-08).
/// </summary>
public sealed class EvaluadorReglaContenidoTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan Tope = TimeSpan.FromMilliseconds(100);

    private readonly EvaluadorReglaContenido _evaluador = new();

    [Fact]
    public void Contenido_que_coincide_con_la_regex_coincide()
    {
        // Given una regla de contenido por regex que detecta enlaces de un acortador (TC-13 / CA-01)
        // y un mensaje con un enlace de ese tipo.
        var regla = ReglaContenido.PorExpresionRegular(
            "Enlace de acortador", @"https?://(?:bit\.ly|tinyurl\.com)/\S+", Tope);
        var mensaje = MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "miralo aca https://bit.ly/abc123 ya");

        // When se evalúa el contenido del mensaje aislado.
        var resultado = _evaluador.Evaluar(mensaje, regla);

        // Then coincide y no se excede el tope.
        resultado.Coincide.Should().BeTrue();
        resultado.ExcedioTope.Should().BeFalse();
    }

    [Fact]
    public void Contenido_que_no_coincide_no_coincide()
    {
        // Given la misma regla y un mensaje sin enlace que coincida.
        var regla = ReglaContenido.PorExpresionRegular(
            "Enlace de acortador", @"https?://(?:bit\.ly|tinyurl\.com)/\S+", Tope);
        var mensaje = MensajesDePrueba.Crear(
            "300000000000000001", Base, contenido: "un mensaje cualquiera sin enlaces");

        // When se evalúa.
        var resultado = _evaluador.Evaluar(mensaje, regla);

        // Then no coincide.
        resultado.Coincide.Should().BeFalse();
        resultado.ExcedioTope.Should().BeFalse();
    }

    [Fact]
    public void Insensible_a_mayusculas_por_defecto_coincide_con_otra_caja()
    {
        // Given una regla por defecto (insensible a mayúsculas) y un texto en caja distinta.
        var regla = ReglaContenido.PorExpresionRegular("Palabra vedada", "prohibido", Tope);
        var resultado = _evaluador.Evaluar("Esto es PROHIBIDO aqui", regla);

        // Then coincide pese a la diferencia de caja.
        resultado.Coincide.Should().BeTrue();
    }

    [Fact]
    public void Sensible_a_mayusculas_no_coincide_con_otra_caja()
    {
        // Given una regla sensible a mayúsculas y un texto en caja distinta.
        var regla = ReglaContenido.PorExpresionRegular(
            "Palabra vedada", "prohibido", Tope, sensibleAMayusculas: true);
        var resultado = _evaluador.Evaluar("Esto es PROHIBIDO aqui", regla);

        // Then no coincide porque la caja no calza.
        resultado.Coincide.Should().BeFalse();
    }

    [Fact]
    public void Mensaje_sin_texto_no_coincide()
    {
        // Given una regla cualquiera y un contenido vacío.
        var regla = ReglaContenido.PorExpresionRegular("Cualquiera", "algo", Tope);

        // When se evalúa un contenido vacío.
        var resultado = _evaluador.Evaluar(string.Empty, regla);

        // Then no coincide.
        resultado.Coincide.Should().BeFalse();
    }

    [Fact]
    public void Patron_regex_invalido_se_rechaza_al_guardar_con_codigo_y_no_en_evaluacion()
    {
        // Given un patrón que no compila (paréntesis sin cerrar): RN-03 / CU-04 CA-03 (TC-14).
        // When se intenta CONFIGURAR la regla.
        var registrar = () => ReglaContenido.PorExpresionRegular("Rota", "(abc", Tope);

        // Then se rechaza al guardar con el código del CU, no en tiempo de evaluación (ADR-08).
        registrar.Should()
            .Throw<ReglaContenidoInvalidaException>()
            .Which.Codigo.Should().Be(ReglaContenidoInvalidaException.CodigoPatronInvalido);
    }

    [Fact]
    public void Patron_regex_valido_se_acepta()
    {
        // Given un patrón válido. When se configura la regla. Then no lanza y queda construida (RN-03).
        var regla = ReglaContenido.PorExpresionRegular("Valida", @"\d{3}-\d{4}", Tope);

        regla.TipoCriterio.Should().Be(TipoCriterioContenido.ExpresionRegular);
        regla.Criterio.Should().Be(@"\d{3}-\d{4}");
    }

    [Fact]
    public void Patron_vacio_se_rechaza_al_guardar()
    {
        // Given un criterio vacío: RN-03 (un criterio vacío no decide de forma confiable).
        var registrar = () => ReglaContenido.PorExpresionRegular("Vacia", string.Empty, Tope);

        registrar.Should().Throw<ReglaContenidoInvalidaException>();
    }

    [Fact]
    public void Tope_de_tiempo_corta_el_retroceso_catastrofico_sin_lanzar_y_no_coincide()
    {
        // Given un patrón con retroceso catastrófico clásico y una entrada adversa, con un tope
        // muy bajo (TC-15 / ADR-08). El patrón es válido (compila) pero costoso de evaluar.
        var regla = ReglaContenido.PorExpresionRegular(
            "Costosa", "(a+)+$", TimeSpan.FromMilliseconds(1));
        var entradaAdversa = new string('a', 60) + "!"; // no termina en $, fuerza el backtracking.

        // When se evalúa con un tope de 1 ms.
        var resultado = _evaluador.Evaluar(entradaAdversa, regla);

        // Then NO se lanza excepción al pipeline; se resuelve como no coincidencia señalando el
        // exceso de tope, de forma determinista (ADR-08).
        resultado.Coincide.Should().BeFalse();
        resultado.ExcedioTope.Should().BeTrue();
    }
}
