namespace DiscordModeradorBot.Servicio.Dominio.Servidores;

/// <summary>
/// Datos para probar la configuración de un servidor contra la plataforma (CU-12). Lleva el
/// snowflake del servidor, el TOKEN EN CLARO descifrado en memoria para la prueba (RN-14: el
/// token nunca se persiste ni se expone en claro, solo vive en memoria durante la prueba) y el
/// canal de salida designado (opcional) para verificar su existencia/acceso. La aplicación
/// arma esta solicitud descifrando el token del servidor antes de invocar el puerto del
/// adaptador; el adaptador no accede al almacenamiento.
/// </summary>
public sealed record SolicitudPruebaConfiguracion(
    Snowflake ServidorId,
    string TokenEnClaro,
    CanalDeSalida? CanalDeSalida = null);
