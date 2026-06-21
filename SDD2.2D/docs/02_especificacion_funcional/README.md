# 02 Especificación funcional — discord-bots-admin

**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-20

Punto de entrada navegable de la especificación funcional del proyecto discord-bots-admin (tipo web-monolith). Define el qué del sistema: casos de uso (CU), reglas de negocio (RN), modelo conceptual de datos y reglas conceptuales (RC). El cómo de la presentación vive en 03 y el de la implementación en 05.

## Índice maestro

| Documento | Propósito | Estado |
| --- | --- | --- |
| [especificacion-funcional_v1.0.md](especificacion-funcional_v1.0.md) | Índice maestro con la matriz NB→CU→RN→US y la cobertura bidireccional | Propuesto |

## Casos de uso

| CU | Propósito en una línea | Estado |
| --- | --- | --- |
| [CU-01](casos-de-uso/CU-01-detectar-rafaga-distribuida_v1.0.md) | Reconocer el patrón de ráfaga distribuida por canales distintos en ventana corta | Propuesto |
| [CU-02](casos-de-uso/CU-02-banear-emisor-rafaga_v1.0.md) | Banear automáticamente al emisor cuando se dispara la política de ráfaga | Propuesto |
| [CU-03](casos-de-uso/CU-03-banear-con-borrado-retroactivo_v1.0.md) | Banear y limpiar retroactivamente los mensajes del emisor dentro de la ventana | Propuesto |
| [CU-04](casos-de-uso/CU-04-detectar-contenido-no-deseado_v1.0.md) | Detectar contenido no deseado en un mensaje y contener al emisor | Propuesto |
| [CU-05](casos-de-uso/CU-05-reportar-incidente-canal-privado_v1.0.md) | Reportar a un canal privado los mensajes accionados y los canales afectados | Propuesto |
| [CU-06](casos-de-uso/CU-06-revisar-incidentes-panel_v1.0.md) | Revisar incidentes y su evidencia desde el panel | Propuesto |
| [CU-07](casos-de-uso/CU-07-revertir-contencion-desbaneo_v1.0.md) | Revertir un baneo (desbaneo) desde el panel ante un falso positivo | Propuesto |
| [CU-08](casos-de-uso/CU-08-alta-credenciales-administrador-primer-ingreso_v1.0.md) | Crear la cuenta única de administrador en el primer ingreso | Propuesto |
| [CU-09](casos-de-uso/CU-09-autenticar-administrador_v1.0.md) | Autenticar al administrador y abrir una sesión autorizada | Propuesto |
| [CU-10](casos-de-uso/CU-10-registrar-servidor-con-token_v1.0.md) | Registrar un servidor con su credencial cifrada en reposo | Propuesto |
| [CU-11](casos-de-uso/CU-11-administrar-reglas-grupos-eventos-acciones_v1.0.md) | Administrar reglas, grupos, eventos, acciones y parámetros con ayuda contextual | Propuesto |
| [CU-12](casos-de-uso/CU-12-probar-configuracion-servidor_v1.0.md) | Probar la configuración de un servidor antes de activarlo | Propuesto |
| [CU-13](casos-de-uso/CU-13-reconectar-y-mostrar-estado-conexion_v1.0.md) | Reconectar automáticamente y mostrar el estado de conexión por servidor | Propuesto |
| [CU-14](casos-de-uso/CU-14-ejecutar-politica-modo-simulacion_v1.0.md) | Ejecutar una política en modo simulación registrando lo que haría | Propuesto |
| [CU-15](casos-de-uso/CU-15-definir-exenciones_v1.0.md) | Definir exenciones por rol, usuario o canal de confianza | Propuesto |
| [CU-16](casos-de-uso/CU-16-antirrebote-por-usuario_v1.0.md) | Evitar acciones repetidas sobre el mismo usuario durante una ráfaga | Propuesto |

## Reglas de negocio

| RN | Propósito en una línea | Estado |
| --- | --- | --- |
| [RN-01](reglas-de-negocio/RN-01-jerarquia-de-roles-del-bot_v1.0.md) | El bot no acciona sobre un usuario con rol superior o sin permisos | Propuesto |
| [RN-02](reglas-de-negocio/RN-02-tope-borrado-retroactivo_v1.0.md) | La ventana de borrado nunca excede 7 días | Propuesto |
| [RN-03](reglas-de-negocio/RN-03-validez-patron-de-contenido_v1.0.md) | Una regla de contenido solo se aplica si su criterio es válido | Propuesto |
| [RN-04](reglas-de-negocio/RN-04-evaluacion-politicas-por-prioridad_v1.0.md) | Las políticas se evalúan por prioridad con primera coincidencia y bandera continuar | Propuesto |
| [RN-05](reglas-de-negocio/RN-05-orden-de-ejecucion-de-acciones_v1.0.md) | Las acciones se ejecutan en orden y la copia se toma antes de remover | Propuesto |
| [RN-06](reglas-de-negocio/RN-06-antirrebote-por-usuario_v1.0.md) | Un usuario no recibe la misma acción más de una vez por ventana de antirrebote | Propuesto |
| [RN-07](reglas-de-negocio/RN-07-descarte-previo-de-exentos_v1.0.md) | Los sujetos exentos se descartan antes de evaluar cualquier regla | Propuesto |
| [RN-08](reglas-de-negocio/RN-08-identidad-de-snowflakes_v1.0.md) | Los snowflakes se tratan como texto; el servidor es único por su snowflake | Propuesto |
| [RN-09](reglas-de-negocio/RN-09-modo-simulacion-no-ejecuta_v1.0.md) | El modo simulación registra pero no ejecuta acción real | Propuesto |
| [RN-10](reglas-de-negocio/RN-10-configuracion-dirigida-por-descriptor_v1.0.md) | Cada parámetro se rige por su descriptor de default, límites, leyenda y ejemplos | Propuesto |
| [RN-11](reglas-de-negocio/RN-11-integridad-del-incidente_v1.0.md) | Todo incidente conserva la copia de mensajes y los canales afectados | Propuesto |
| [RN-12](reglas-de-negocio/RN-12-autorizacion-rol-administrador-unico_v1.0.md) | Solo el administrador autenticado opera las funciones administrativas | Propuesto |
| [RN-13](reglas-de-negocio/RN-13-resguardo-de-credenciales-del-administrador_v1.0.md) | Cuenta de administrador única con contraseña resguardada por hash | Propuesto |
| [RN-14](reglas-de-negocio/RN-14-cifrado-del-token-en-reposo_v1.0.md) | El token de bot se cifra en reposo y nunca se persiste en texto claro | Propuesto |
| [RN-15](reglas-de-negocio/RN-15-composicion-de-grupo-de-reglas_v1.0.md) | Un grupo tiene al menos una regla y un modo de coincidencia; anidamiento de dos niveles | Propuesto |
| [RN-16](reglas-de-negocio/RN-16-activacion-condicionada-a-prueba_v1.0.md) | La activación de un servidor se condiciona a la prueba de configuración | Propuesto |

## Modelo de datos

| Documento | Propósito en una línea | Estado |
| --- | --- | --- |
| [modelo-conceptual_v1.0.md](modelo-datos/modelo-conceptual_v1.0.md) | Modelo conceptual de 13 entidades del dominio de moderación, con diagrama Mermaid | Propuesto |

El modelo conceptual tiene 13 entidades, por encima del umbral de 10; por eso se generan las reglas conceptuales (RC) obligatorias.

| RC | Propósito en una línea | Estado |
| --- | --- | --- |
| [RC-01](modelo-datos/reglas-conceptuales-de-modelo/RC-01-integridad-referencial-dependientes_v1.0.md) | Las entidades dependientes referencian un contenedor existente | Propuesto |
| [RC-02](modelo-datos/reglas-conceptuales-de-modelo/RC-02-identidad-de-snowflakes_v1.0.md) | Los snowflakes se conservan como texto sin pérdida de precisión | Propuesto |
| [RC-03](modelo-datos/reglas-conceptuales-de-modelo/RC-03-integridad-asociaciones-grupo-evento_v1.0.md) | Las asociaciones grupo-regla y evento-grupo tienen ambos extremos válidos | Propuesto |
| [RC-04](modelo-datos/reglas-conceptuales-de-modelo/RC-04-composicion-minima-de-grupo_v1.0.md) | Un grupo tiene al menos una regla y un modo de coincidencia | Propuesto |
| [RC-05](modelo-datos/reglas-conceptuales-de-modelo/RC-05-orden-determinista-evento-accion_v1.0.md) | Eventos y acciones tienen orden determinista | Propuesto |
| [RC-06](modelo-datos/reglas-conceptuales-de-modelo/RC-06-unicidad-de-administrador_v1.0.md) | Hay a lo sumo un administrador con contraseña no reversible | Propuesto |
| [RC-07](modelo-datos/reglas-conceptuales-de-modelo/RC-07-confidencialidad-del-token_v1.0.md) | El token del servidor se conserva solo cifrado | Propuesto |
| [RC-08](modelo-datos/reglas-conceptuales-de-modelo/RC-08-activacion-condicionada-de-servidor_v1.0.md) | Un servidor está activo solo con prueba superada | Propuesto |
| [RC-09](modelo-datos/reglas-conceptuales-de-modelo/RC-09-validez-del-criterio-de-regla_v1.0.md) | El criterio de una regla es coherente con su clase | Propuesto |
| [RC-10](modelo-datos/reglas-conceptuales-de-modelo/RC-10-coherencia-de-modo-evento-incidente_v1.0.md) | El modo del incidente es coherente con el del evento | Propuesto |
| [RC-11](modelo-datos/reglas-conceptuales-de-modelo/RC-11-tope-ventana-de-borrado_v1.0.md) | La ventana de borrado de una acción está entre 0 y 7 días | Propuesto |

## Convenciones

- Nomenclatura `CU-XX-<kebab>_v1.0.md`, `RN-XX-<kebab>_v1.0.md`, `RC-XX-<kebab>_v1.0.md`, con `_v` (no `.v`) y slug en minúsculas estricto.
- Una sola versión vigente por nombre lógico; las superadas irían a `_legacy/` con estado Superado (no hay ninguna en esta versión inicial).
- Trazabilidad upstream a NB-01..NB-07 y a 00 (visión, alcance); trazabilidad downstream a US en 06, componentes en 05 y tests en 08.
