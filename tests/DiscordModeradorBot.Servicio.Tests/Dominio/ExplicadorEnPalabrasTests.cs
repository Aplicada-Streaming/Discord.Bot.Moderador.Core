using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Moderacion;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas de la explicación en palabras generada por plantilla a partir de descriptores y
/// valores (R7, CU-11, design-rules-config-esquema §4.5). La explicación se deriva del modelo
/// (modo de coincidencia, nombres, modo simulación), no se redacta a mano: cambia con la
/// configuración. Es el inverso del futuro asistente de IA (valores → palabras).
/// </summary>
public sealed class ExplicadorEnPalabrasTests
{
    [Fact]
    public void Explica_un_grupo_AlMenosN_y_sus_acciones_en_orden()
    {
        var grupo = new GrupoDeReglas("Spam", ModoCoincidencia.AlMenosN, new IReglaEvaluable[]
        {
            new ReglaEvaluableConducta(new EvaluadorRafagaDistribuida(), "Ráfaga distribuida"),
            new ReglaEvaluableConducta(new EvaluadorRafagaDistribuida(), "Otra regla"),
        }, minimoCoincidencias: 2);

        var politica = new Politica(
            "Corte de spam", prioridad: 0, modo: Modo.Ejecucion,
            acciones: new[]
            {
                new Accion(TipoAccion.ReportarACanalPrivado, OrdenEjecucion: 0),
                new Accion(TipoAccion.BaneoConBorradoRetroactivo, OrdenEjecucion: 1, VentanaBorradoDias: 1),
            },
            composicion: new ComposicionPolitica(new[] { grupo }));

        var texto = ExplicadorEnPalabras.Explicar(politica);

        texto.Should().Contain("AL MENOS 2");
        texto.Should().Contain("Corte de spam");
        texto.Should().Contain("reporta al canal de incidencias");
        texto.Should().Contain("banea");
        texto.Should().Contain("ejecución real");
    }

    [Fact]
    public void Marca_el_modo_simulacion()
    {
        var politica = new Politica(
            "Política simulada", prioridad: 0, modo: Modo.Simulacion,
            acciones: new[] { new Accion(TipoAccion.BaneoConBorradoRetroactivo) });

        ExplicadorEnPalabras.Explicar(politica).Should().Contain("simulación");
    }

    [Fact]
    public void Explica_la_rafaga_distribuida_a_partir_de_valores_y_descriptores()
    {
        // §4.5: la explicación de los parámetros de detección se compone de los valores elegidos y
        // refleja el modo; cambia con la configuración (no se redacta a mano).
        var texto = ExplicadorEnPalabras.ExplicarRafagaDistribuida(
            umbralCanales: 4, ventanaSegundos: 6.0, antirreboteSegundos: 30.0, simulacion: false);

        texto.Should().Contain("4 canales distintos");
        texto.Should().Contain("6");
        texto.Should().Contain("ráfaga distribuida");
        texto.Should().Contain("30");
        texto.Should().Contain("ejecución real");
    }

    [Fact]
    public void La_rafaga_distribuida_marca_el_modo_simulacion()
    {
        var texto = ExplicadorEnPalabras.ExplicarRafagaDistribuida(
            umbralCanales: 3, ventanaSegundos: 2.0, antirreboteSegundos: null, simulacion: true);

        texto.Should().Contain("simulación");
    }
}
