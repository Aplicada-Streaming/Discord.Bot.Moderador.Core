namespace DiscordModeradorBot.Servicio.Dominio.Servidores;

/// <summary>Estado de conexión del servidor (modelo-datos-logico §2.2).</summary>
public enum EstadoConexion
{
    Desconectado = 0,
    Conectado = 1,
}

/// <summary>
/// Estado de activación del servidor; activo solo tras superar la prueba de
/// configuración (RC-08, RN-16). En R1 el servidor queda inactivo (CU-10).
/// </summary>
public enum EstadoActivacion
{
    Inactivo = 0,
    Activo = 1,
}

/// <summary>
/// Servidor registrado: contexto independiente del firewall multi-contexto (ADR-13,
/// modelo-datos-logico §2.2). Guarda su snowflake (como texto, RN-08), su token cifrado en
/// reposo (RN-14, RC-07; el token nunca se conserva en claro) y, en R2, el canal de salida
/// lógico al que se publican los reportes de moderación (CU-05, modelo-datos-logico §2.3).
/// </summary>
public sealed class ServidorRegistrado
{
    public ServidorRegistrado(
        Snowflake snowflakeServidor,
        string tokenCifrado,
        EstadoConexion estadoConexion = EstadoConexion.Desconectado,
        EstadoActivacion estadoActivacion = EstadoActivacion.Inactivo,
        string? nombreDescriptivo = null,
        DateTimeOffset? creadoEn = null,
        CanalDeSalida? canalDeSalida = null)
    {
        if (string.IsNullOrWhiteSpace(tokenCifrado))
        {
            throw new ArgumentException("El token cifrado es obligatorio.", nameof(tokenCifrado));
        }

        SnowflakeServidor = snowflakeServidor;
        TokenCifrado = tokenCifrado;
        EstadoConexion = estadoConexion;
        EstadoActivacion = estadoActivacion;
        NombreDescriptivo = nombreDescriptivo;
        CreadoEn = creadoEn ?? DateTimeOffset.UtcNow;
        CanalDeSalida = canalDeSalida;
    }

    public Snowflake SnowflakeServidor { get; }
    public string TokenCifrado { get; }
    public EstadoConexion EstadoConexion { get; }
    public EstadoActivacion EstadoActivacion { get; }
    public string? NombreDescriptivo { get; }
    public DateTimeOffset CreadoEn { get; }

    /// <summary>
    /// Canal de salida lógico designado para reportes de moderación (CU-05). Puede ser null
    /// si el servidor todavía no lo designó (CU-05 CA-03, REPORTE_CANAL_NO_DESIGNADO).
    /// </summary>
    public CanalDeSalida? CanalDeSalida { get; }
}
