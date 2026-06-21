using DiscordModeradorBot.Servicio.Dominio.Conducta;

namespace DiscordModeradorBot.Servicio.Dominio.Moderacion.Reglas;

/// <summary>
/// Adaptador de la regla de CONDUCTA de ráfaga distribuida (CU-01) a la abstracción
/// <see cref="IReglaEvaluable"/> para que pueda formar parte de un <see cref="GrupoDeReglas"/>
/// (R7, RN-15). Reúsa el <see cref="EvaluadorRafagaDistribuida"/> de R1 SIN cambiarlo: la
/// ventana y el umbral se normalizan contra sus descriptores (RN-10). Su evaluación SÍ depende
/// del estado de conducta y del instante de evaluación, que vienen en el contexto.
/// </summary>
public sealed class ReglaEvaluableConducta : IReglaEvaluable
{
    private readonly EvaluadorRafagaDistribuida _evaluador;
    private readonly int? _umbralConfigurado;
    private readonly double? _ventanaSegundosConfigurada;

    public ReglaEvaluableConducta(
        EvaluadorRafagaDistribuida evaluador,
        string nombre = "Ráfaga distribuida",
        int? umbralConfigurado = null,
        double? ventanaSegundosConfigurada = null)
    {
        _evaluador = evaluador ?? throw new ArgumentNullException(nameof(evaluador));
        Nombre = string.IsNullOrWhiteSpace(nombre) ? "Ráfaga distribuida" : nombre;
        _umbralConfigurado = umbralConfigurado;
        _ventanaSegundosConfigurada = ventanaSegundosConfigurada;
    }

    public string Nombre { get; }

    public ClaseRegla Clase => ClaseRegla.Conducta;

    public bool Evaluar(ContextoEvaluacionRegla contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto);

        return _evaluador.Evaluar(
                contexto.Mensaje,
                contexto.EstadoConducta,
                contexto.InstanteEvaluacion,
                _umbralConfigurado,
                _ventanaSegundosConfigurada)
            .Coincide;
    }
}
