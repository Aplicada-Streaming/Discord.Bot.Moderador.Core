using DiscordModeradorBot.Servicio.Dominio.Administracion;
using FluentAssertions;

namespace DiscordModeradorBot.Servicio.Tests.Dominio;

/// <summary>
/// Pruebas del control de intentos fallidos de autenticación (CU-09 AUTH_DEMASIADOS_INTENTOS,
/// ADR-09). Deterministas: el tiempo avanza con un reloj inyectado (instantes explícitos), sin
/// pausas reales. Verifican el bloqueo tras N fallos en la ventana, el desbloqueo pasado el
/// enfriamiento, el reinicio del contador por éxito y el reinicio por ventana vencida.
/// </summary>
public sealed class ControlIntentosAutenticacionTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 20, 12, 0, 0, TimeSpan.Zero);

    private const string Clave = "admin|127.0.0.1";

    // Parámetros chicos y explícitos para que el escenario sea legible y determinista.
    private static ControlIntentosAutenticacion CrearControl() =>
        new(maximoIntentos: 5, ventana: TimeSpan.FromMinutes(15), enfriamiento: TimeSpan.FromMinutes(15));

    [Fact]
    public void Sin_registro_no_esta_bloqueado()
    {
        var control = CrearControl();

        control.EstaBloqueado(Clave, Base).Should().BeFalse();
    }

    [Fact]
    public void Bloquea_al_alcanzar_el_maximo_de_intentos_en_la_ventana()
    {
        var control = CrearControl();

        // 4 fallos: todavía no bloquea (por debajo del máximo).
        for (var i = 0; i < 4; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(i)).Should().BeFalse();
        }

        control.EstaBloqueado(Clave, Base.AddSeconds(4)).Should().BeFalse();

        // El 5.º fallo cierra el límite: queda bloqueado (AUTH_DEMASIADOS_INTENTOS).
        control.RegistrarFallo(Clave, Base.AddSeconds(5)).Should().BeTrue();
        control.EstaBloqueado(Clave, Base.AddSeconds(5)).Should().BeTrue();
    }

    [Fact]
    public void Sigue_bloqueado_durante_el_enfriamiento_y_se_libera_al_expirar()
    {
        var control = CrearControl();

        for (var i = 0; i < 5; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(i));
        }

        var ultimoFallo = Base.AddSeconds(4);

        // Dentro del enfriamiento (15 min desde el último fallo): sigue bloqueado.
        control.EstaBloqueado(Clave, ultimoFallo + TimeSpan.FromMinutes(14)).Should().BeTrue();

        // Pasado el enfriamiento: se libera y vuelve a permitirse el ingreso.
        control.EstaBloqueado(Clave, ultimoFallo + TimeSpan.FromMinutes(15)).Should().BeFalse();
        control.EstaBloqueado(Clave, ultimoFallo + TimeSpan.FromMinutes(20)).Should().BeFalse();
    }

    [Fact]
    public void Tras_el_enfriamiento_un_nuevo_fallo_reinicia_el_conteo()
    {
        var control = CrearControl();

        for (var i = 0; i < 5; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(i));
        }

        var trasEnfriamiento = Base.AddSeconds(4) + TimeSpan.FromMinutes(16);

        // El primer fallo después del enfriamiento arranca un conteo nuevo: NO bloquea.
        control.RegistrarFallo(Clave, trasEnfriamiento).Should().BeFalse();
        control.EstaBloqueado(Clave, trasEnfriamiento).Should().BeFalse();
    }

    [Fact]
    public void Un_exito_resetea_el_contador()
    {
        var control = CrearControl();

        // 4 fallos (sin alcanzar el límite) y luego un éxito: el contador se reinicia.
        for (var i = 0; i < 4; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(i));
        }

        control.RegistrarExito(Clave);

        // Tras el reset, hacen falta de nuevo 5 fallos para bloquear: 4 más no alcanzan.
        for (var i = 0; i < 4; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(10 + i)).Should().BeFalse();
        }

        control.EstaBloqueado(Clave, Base.AddSeconds(14)).Should().BeFalse();
    }

    [Fact]
    public void Fallos_fuera_de_la_ventana_no_se_acumulan()
    {
        var control = CrearControl();

        // 4 fallos dentro de la ventana inicial.
        for (var i = 0; i < 4; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(i));
        }

        // Un fallo MUCHO después (fuera de la ventana de 15 min): reinicia el conteo a 1.
        var tarde = Base.AddMinutes(30);
        control.RegistrarFallo(Clave, tarde).Should().BeFalse();
        control.EstaBloqueado(Clave, tarde).Should().BeFalse();
    }

    [Fact]
    public void El_bloqueo_es_por_clave_independiente()
    {
        var control = CrearControl();
        const string otraClave = "otro|10.0.0.1";

        // 5 fallos en una clave: queda bloqueada esa, no la otra.
        for (var i = 0; i < 5; i++)
        {
            control.RegistrarFallo(Clave, Base.AddSeconds(i));
        }

        control.EstaBloqueado(Clave, Base.AddSeconds(5)).Should().BeTrue();
        control.EstaBloqueado(otraClave, Base.AddSeconds(5)).Should().BeFalse();
    }
}
