using DiscordModeradorBot.Servicio.Dominio;
using DiscordModeradorBot.Servicio.Dominio.Conducta;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas unitarias del estado de antirrebote por usuario en memoria (CU-16, RN-06, ADR-09).
/// Deterministas: el instante "ahora" se inyecta en cada consulta. Verifican que la primera
/// acción se permite y marca, que las repeticiones dentro de la ventana se suprimen, que al
/// expirar la ventana vuelve a permitirse, y que el antirrebote es por par (servidor, usuario).
/// </summary>
public sealed class EstadoAntirreboteEnMemoriaTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan Ventana = TimeSpan.FromSeconds(10);

    private readonly Snowflake _servidor = new("100000000000000001");
    private readonly Snowflake _usuario = new("200000000000000002");
    private readonly EstadoAntirreboteEnMemoria _estado = new();

    [Fact]
    public void La_primera_consulta_no_suprime_la_accion()
    {
        // Given un usuario nunca accionado (CU-16 CA-04).
        // When se consulta si suprimir.
        var suprime = _estado.DebeSuprimir(_servidor, _usuario, Base, Ventana);

        // Then la primera acción se permite.
        suprime.Should().BeFalse();
    }

    [Fact]
    public void Tras_marcar_suprime_una_repeticion_dentro_de_la_ventana()
    {
        // Given una acción ya marcada (CU-16 CA-01, RN-06).
        _estado.RegistrarAccion(_servidor, _usuario, Base);

        // When llega una repetición DENTRO de la ventana.
        var suprime = _estado.DebeSuprimir(_servidor, _usuario, Base.AddSeconds(3), Ventana);

        // Then se suprime (0 acciones adicionales).
        suprime.Should().BeTrue();
    }

    [Fact]
    public void En_el_limite_exacto_de_la_ventana_suprime()
    {
        // Given una acción marcada en Base (la ventana es inclusiva en el límite).
        _estado.RegistrarAccion(_servidor, _usuario, Base);

        // When la repetición cae exactamente en el límite de la ventana.
        var suprime = _estado.DebeSuprimir(_servidor, _usuario, Base + Ventana, Ventana);

        // Then se suprime.
        suprime.Should().BeTrue();
    }

    [Fact]
    public void Al_expirar_la_ventana_vuelve_a_permitir()
    {
        // Given una acción marcada en Base (CU-16 CA-02).
        _estado.RegistrarAccion(_servidor, _usuario, Base);

        // When la repetición llega DESPUÉS de la ventana.
        var suprime = _estado.DebeSuprimir(_servidor, _usuario, Base + Ventana + TimeSpan.FromSeconds(1), Ventana);

        // Then la nueva acción se permite (la marca expiró).
        suprime.Should().BeFalse();
    }

    [Fact]
    public void El_antirrebote_es_por_usuario()
    {
        // Given un usuario marcado.
        _estado.RegistrarAccion(_servidor, _usuario, Base);
        var otroUsuario = new Snowflake("200000000000000003");

        // When se consulta por OTRO usuario dentro de la ventana.
        var suprime = _estado.DebeSuprimir(_servidor, otroUsuario, Base.AddSeconds(1), Ventana);

        // Then no se suprime: cada usuario tiene su propia marca (RN-06).
        suprime.Should().BeFalse();
    }

    [Fact]
    public void El_antirrebote_es_por_servidor()
    {
        // Given un usuario marcado en un servidor.
        _estado.RegistrarAccion(_servidor, _usuario, Base);
        var otroServidor = new Snowflake("100000000000000009");

        // When se consulta el mismo usuario en OTRO servidor (firewall multi-contexto, ADR-13).
        var suprime = _estado.DebeSuprimir(otroServidor, _usuario, Base.AddSeconds(1), Ventana);

        // Then no se suprime: el estado se particiona por contexto.
        suprime.Should().BeFalse();
    }
}
