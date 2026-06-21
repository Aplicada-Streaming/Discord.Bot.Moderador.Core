using System.Text;

namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Genera la "explicación en palabras" de una política a partir de su composición de grupos y de
/// sus acciones (R7, CU-11, design-rules-config-esquema §4.5). La explicación se compone por
/// PLANTILLA a partir del modelo (modos de coincidencia, nombres de reglas, modo simulación/
/// ejecución), NO se redacta a mano por campo: así nunca se desincroniza de la configuración real
/// (anti-patrón §10). Es el inverso del futuro asistente de IA (valores → palabras); la ranura del
/// asistente queda reservada y deshabilitada (§4.7), aquí no se construye.
/// </summary>
public static class ExplicadorEnPalabras
{
    /// <summary>Explica en lenguaje natural cuándo dispara una política y qué hace (CU-11).</summary>
    public static string Explicar(Politica politica)
    {
        ArgumentNullException.ThrowIfNull(politica);

        var sb = new StringBuilder();
        sb.Append("Cuando ");
        sb.Append(ExplicarCondicion(politica));
        sb.Append(", la política '").Append(politica.Nombre).Append("' ");
        sb.Append(ExplicarAcciones(politica.Acciones));
        sb.Append(politica.Modo == Modo.Simulacion
            ? ". (Modo simulación: no se ejecuta ninguna acción real.)"
            : ". (Modo ejecución real.)");
        return sb.ToString();
    }

    private static string ExplicarCondicion(Politica politica)
    {
        if (politica.Composicion is { } composicion)
        {
            var grupos = composicion.Grupos.Select(ExplicarGrupo).ToList();
            var conector = composicion.Modo == ModoCombinacionGrupos.Todos ? " Y además " : " O bien ";
            return grupos.Count == 1 ? grupos[0] : string.Join(conector, grupos);
        }

        if (politica.ReglaContenido is { } regla)
        {
            return $"el contenido del mensaje cumple la regla '{regla.Nombre}'";
        }

        return "se detecta una ráfaga distribuida del mismo usuario en varios canales";
    }

    private static string ExplicarGrupo(GrupoDeReglas grupo)
    {
        var reglas = grupo.Reglas.Select(r => $"'{r.Nombre}'").ToList();
        var listado = string.Join(", ", reglas);

        return grupo.Modo switch
        {
            ModoCoincidencia.Todas =>
                $"se cumplen TODAS las reglas del grupo '{grupo.Nombre}' ({listado})",
            ModoCoincidencia.Alguna =>
                $"se cumple ALGUNA de las reglas del grupo '{grupo.Nombre}' ({listado})",
            ModoCoincidencia.AlMenosN =>
                $"se cumplen AL MENOS {grupo.MinimoCoincidencias} de las reglas del grupo " +
                $"'{grupo.Nombre}' ({listado})",
            _ => $"se cumple el grupo '{grupo.Nombre}'",
        };
    }

    private static string ExplicarAcciones(IReadOnlyList<Accion> acciones)
    {
        if (acciones.Count == 0)
        {
            return "no ejecuta ninguna acción";
        }

        var ordenadas = acciones.OrderBy(a => a.OrdenEjecucion).Select(DescribirAccion);
        return "ejecuta, en orden: " + string.Join("; luego ", ordenadas);
    }

    private static string DescribirAccion(Accion accion) => accion.Tipo switch
    {
        TipoAccion.ReportarACanalPrivado or TipoAccion.Reportar => "reporta al canal de incidencias",
        TipoAccion.BaneoConBorradoRetroactivo or TipoAccion.Banear =>
            $"banea y borra {accion.VentanaBorradoEfectivaDias} día(s) de mensajes",
        TipoAccion.Timeout => $"silencia por {accion.DuracionTimeoutEfectiva.TotalMinutes} minuto(s)",
        TipoAccion.Expulsar => "expulsa al usuario",
        TipoAccion.AsignarRol => $"asigna el rol {accion.RolObjetivo?.Valor}",
        TipoAccion.QuitarRol => $"quita el rol {accion.RolObjetivo?.Valor}",
        TipoAccion.Desbanear => "desbanea al usuario",
        _ => accion.Tipo.ToString(),
    };
}
