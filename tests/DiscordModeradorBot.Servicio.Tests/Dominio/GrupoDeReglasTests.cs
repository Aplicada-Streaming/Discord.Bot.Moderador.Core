using DiscordModeradorBot.Servicio.Dominio.Conducta;
using DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;
using DiscordModeradorBot.Servicio.Tests.Soporte;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas del grupo de reglas con modos de coincidencia (R7, RN-15, RC-04). Usan reglas
/// evaluables sintéticas con un predicado fijo (true/false), de modo que el modo de coincidencia
/// se prueba aislado de los evaluadores concretos. El contexto trae un mensaje y un estado de
/// conducta de juguete; las reglas sintéticas no los miran.
/// </summary>
public sealed class GrupoDeReglasTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private static ContextoEvaluacionRegla Contexto() =>
        new(MensajesDePrueba.Crear("300000000000000001", Ahora), new EstadoConductaEnMemoria(), Ahora);

    /// <summary>Regla evaluable de prueba con un resultado fijo.</summary>
    private sealed class ReglaFija : IReglaEvaluable
    {
        private readonly bool _coincide;
        public ReglaFija(string nombre, bool coincide) { Nombre = nombre; _coincide = coincide; }
        public string Nombre { get; }
        public ClaseRegla Clase => ClaseRegla.Contenido;
        public bool Evaluar(ContextoEvaluacionRegla contexto) => _coincide;
    }

    [Fact]
    public void Modo_Todas_coincide_solo_si_todas_las_reglas_coinciden()
    {
        var todasCoinciden = new GrupoDeReglas("g", ModoCoincidencia.Todas, new IReglaEvaluable[]
        {
            new ReglaFija("a", true), new ReglaFija("b", true),
        });
        var unaNo = new GrupoDeReglas("g", ModoCoincidencia.Todas, new IReglaEvaluable[]
        {
            new ReglaFija("a", true), new ReglaFija("b", false),
        });

        todasCoinciden.Evaluar(Contexto()).Should().BeTrue();
        unaNo.Evaluar(Contexto()).Should().BeFalse();
    }

    [Fact]
    public void Modo_Alguna_coincide_si_al_menos_una_coincide()
    {
        var unaSi = new GrupoDeReglas("g", ModoCoincidencia.Alguna, new IReglaEvaluable[]
        {
            new ReglaFija("a", false), new ReglaFija("b", true),
        });
        var ningunaSi = new GrupoDeReglas("g", ModoCoincidencia.Alguna, new IReglaEvaluable[]
        {
            new ReglaFija("a", false), new ReglaFija("b", false),
        });

        unaSi.Evaluar(Contexto()).Should().BeTrue();
        ningunaSi.Evaluar(Contexto()).Should().BeFalse();
    }

    [Fact]
    public void Modo_AlMenosN_coincide_si_coinciden_al_menos_N()
    {
        // N = 2 sobre tres reglas: con 2 coincidencias dispara; con 1 no.
        var dos = new GrupoDeReglas("g", ModoCoincidencia.AlMenosN, new IReglaEvaluable[]
        {
            new ReglaFija("a", true), new ReglaFija("b", true), new ReglaFija("c", false),
        }, minimoCoincidencias: 2);
        var una = new GrupoDeReglas("g", ModoCoincidencia.AlMenosN, new IReglaEvaluable[]
        {
            new ReglaFija("a", true), new ReglaFija("b", false), new ReglaFija("c", false),
        }, minimoCoincidencias: 2);

        dos.Evaluar(Contexto()).Should().BeTrue();
        una.Evaluar(Contexto()).Should().BeFalse();
    }

    [Fact]
    public void Grupo_vacio_se_rechaza_con_codigo_CONFIG_GRUPO_SIN_REGLAS()
    {
        // RC-04 / RN-15: un grupo sin reglas no decide; se rechaza al construir.
        var accion = () => new GrupoDeReglas("g", ModoCoincidencia.Todas, Array.Empty<IReglaEvaluable>());

        accion.Should()
            .Throw<GrupoDeReglasInvalidoException>()
            .Where(ex => ex.Codigo == GrupoDeReglas.CodigoGrupoSinReglas);
    }

    [Fact]
    public void AlMenosN_con_N_fuera_de_rango_se_rechaza()
    {
        // N por encima de la cantidad de reglas nunca puede satisfacerse (RN-15).
        var accion = () => new GrupoDeReglas("g", ModoCoincidencia.AlMenosN, new IReglaEvaluable[]
        {
            new ReglaFija("a", true),
        }, minimoCoincidencias: 5);

        accion.Should()
            .Throw<GrupoDeReglasInvalidoException>()
            .Where(ex => ex.Codigo == GrupoDeReglas.CodigoNInvalido);
    }

    [Fact]
    public void Composicion_de_dos_niveles_combina_grupos_con_modo()
    {
        // Nivel 1: cada grupo combina sus reglas; nivel 2: la composición combina los grupos.
        var grupoA = new GrupoDeReglas("A", ModoCoincidencia.Alguna, new IReglaEvaluable[]
        {
            new ReglaFija("a1", true), new ReglaFija("a2", false),
        });
        var grupoB = new GrupoDeReglas("B", ModoCoincidencia.Todas, new IReglaEvaluable[]
        {
            new ReglaFija("b1", true), new ReglaFija("b2", false),
        });

        // En modo Todos exige ambos grupos: B no coincide → la composición no dispara.
        var todos = new ComposicionPolitica(new[] { grupoA, grupoB }, ModoCombinacionGrupos.Todos);
        // En modo Alguno alcanza con A → dispara.
        var alguno = new ComposicionPolitica(new[] { grupoA, grupoB }, ModoCombinacionGrupos.Alguno);

        todos.Evaluar(Contexto()).Should().BeFalse();
        alguno.Evaluar(Contexto()).Should().BeTrue();
    }

    [Fact]
    public void Composicion_sin_grupos_se_rechaza()
    {
        var accion = () => new ComposicionPolitica(Array.Empty<GrupoDeReglas>());

        accion.Should()
            .Throw<GrupoDeReglasInvalidoException>()
            .Where(ex => ex.Codigo == ComposicionPolitica.CodigoSinGrupos);
    }
}
