# Especificación funcional

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | especificacion-funcional_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista Funcional senior (AG-02) |
| Tipo de proyecto (D8) | web-monolith |
| Cantidad de CU | 16 |
| Cantidad de RN | 16 |
| Cantidad de RC | 11 |
| Entidades del modelo conceptual | 13 |
| Trazabilidad upstream | NB-01..NB-07 (01_necesidades_negocio); vision-producto_v1.0.md; alcance-proyecto_v1.0.md; SOLUTION-INTAKE-discord-bots-admin_v1.0.md |
| Trazabilidad downstream | US a generar en 06_backlog-tecnico; componentes en 05_arquitectura_tecnica; tests en 08_calidad |

## 1. Propósito

Este índice maestro consolida la especificación funcional del proyecto discord-bots-admin: los casos de uso (CU) que materializan las siete necesidades de negocio, las reglas de negocio (RN) que restringen el dominio de moderación y el modelo conceptual de datos con sus reglas conceptuales (RC). Define el qué del sistema sin invadir el cómo (la presentación corresponde a 03 y la implementación a 05). La cobertura NB→CU es bidireccional: ninguna NB queda sin CU y ningún CU queda huérfano.

## 2. Matriz de trazabilidad NB→CU→RN→US

| NB | CU | Título del CU | RN aplicables | US a generar (06) |
| --- | --- | --- | --- | --- |
| NB-01 | CU-01 | Detectar ráfaga distribuida por canales distintos en ventana corta | RN-04, RN-07, RN-08, RN-10 | a generar |
| NB-01 | CU-02 | Banear automáticamente al emisor de la ráfaga | RN-01, RN-04, RN-05, RN-06, RN-07, RN-09 | a generar |
| NB-02 | CU-03 | Banear con borrado retroactivo de los mensajes del emisor | RN-01, RN-02, RN-05 | a generar |
| NB-03 | CU-04 | Detectar contenido no deseado en un mensaje y contener al emisor | RN-03, RN-04, RN-05, RN-07, RN-09 | a generar |
| NB-04 | CU-05 | Reportar a un canal privado los mensajes accionados y los canales afectados | RN-01, RN-09, RN-11 | a generar |
| NB-04 | CU-06 | Revisar incidentes y mensajes accionados desde el panel | RN-09, RN-11, RN-12 | a generar |
| NB-04 | CU-07 | Revertir una contención (desbaneo) desde el panel | RN-01, RN-12 | a generar |
| NB-05 | CU-08 | Dar de alta las credenciales del administrador en el primer ingreso | RN-12, RN-13 | a generar |
| NB-05 | CU-09 | Autenticar al administrador | RN-12, RN-13 | a generar |
| NB-05 | CU-10 | Registrar un servidor con su credencial de acceso | RN-08, RN-12, RN-14 | a generar |
| NB-05 | CU-11 | Administrar reglas, grupos, eventos, acciones y parámetros con ayuda contextual | RN-04, RN-05, RN-09, RN-10, RN-12, RN-15 | a generar |
| NB-06 | CU-12 | Probar la configuración de un servidor antes de activarlo | RN-01, RN-12, RN-14, RN-16 | a generar |
| NB-06 | CU-13 | Reconectar automáticamente y mostrar el estado de conexión de cada servidor | RN-14, RN-16 | a generar |
| NB-07 | CU-14 | Ejecutar una política en modo simulación registrando lo que haría sin ejecutarlo | RN-04, RN-09 | a generar |
| NB-07 | CU-15 | Definir exenciones por rol, usuario o canal de confianza | RN-07, RN-08, RN-12 | a generar |
| NB-07 | CU-16 | Evitar acciones repetidas sobre el mismo usuario durante una ráfaga | RN-06, RN-10 | a generar |

## 3. Cobertura bidireccional NB↔CU

| NB | CU que la cubren | Estado de cobertura |
| --- | --- | --- |
| NB-01 Corte automático de la ráfaga de spam distribuido | CU-01, CU-02 | Cubierta |
| NB-02 Limpieza retroactiva de los mensajes del incidente | CU-03 | Cubierta |
| NB-03 Contención de contenido no deseado por patrón | CU-04 | Cubierta |
| NB-04 Trazabilidad de incidentes y control de falsos positivos | CU-05, CU-06, CU-07 | Cubierta |
| NB-05 Configuración autónoma de la moderación por el administrador | CU-08, CU-09, CU-10, CU-11 | Cubierta |
| NB-06 Operación confiable y validación previa de la moderación | CU-12, CU-13 | Cubierta |
| NB-07 Mitigación del riesgo de moderación errónea | CU-14, CU-15, CU-16 | Cubierta |

Ningún CU queda huérfano: cada uno declara su NB upstream en su sección de trazabilidad. Nota de renumeración: las NB de la categoría 01 previeron CU-01..CU-15; la previsión se consolidó y renumeró agregando el CU explícito de autenticación del administrador (CU-09), porque la solución tiene autenticación, lo que desplazó en uno la numeración a partir del registro de servidor. La trazabilidad por NB y por título de CU se conserva en esta matriz.

## 4. Catálogo de casos de uso

| CU | Título | NB | Estado | Archivo |
| --- | --- | --- | --- | --- |
| CU-01 | Detectar ráfaga distribuida por canales distintos en ventana corta | NB-01 | Propuesto | [CU-01](casos-de-uso/CU-01-detectar-rafaga-distribuida_v1.0.md) |
| CU-02 | Banear automáticamente al emisor de la ráfaga | NB-01 | Propuesto | [CU-02](casos-de-uso/CU-02-banear-emisor-rafaga_v1.0.md) |
| CU-03 | Banear con borrado retroactivo de los mensajes del emisor | NB-02 | Propuesto | [CU-03](casos-de-uso/CU-03-banear-con-borrado-retroactivo_v1.0.md) |
| CU-04 | Detectar contenido no deseado en un mensaje y contener al emisor | NB-03 | Propuesto | [CU-04](casos-de-uso/CU-04-detectar-contenido-no-deseado_v1.0.md) |
| CU-05 | Reportar a un canal privado los mensajes accionados y los canales afectados | NB-04 | Propuesto | [CU-05](casos-de-uso/CU-05-reportar-incidente-canal-privado_v1.0.md) |
| CU-06 | Revisar incidentes y mensajes accionados desde el panel | NB-04 | Propuesto | [CU-06](casos-de-uso/CU-06-revisar-incidentes-panel_v1.0.md) |
| CU-07 | Revertir una contención (desbaneo) desde el panel | NB-04 | Propuesto | [CU-07](casos-de-uso/CU-07-revertir-contencion-desbaneo_v1.0.md) |
| CU-08 | Dar de alta las credenciales del administrador en el primer ingreso | NB-05 | Propuesto | [CU-08](casos-de-uso/CU-08-alta-credenciales-administrador-primer-ingreso_v1.0.md) |
| CU-09 | Autenticar al administrador | NB-05 | Propuesto | [CU-09](casos-de-uso/CU-09-autenticar-administrador_v1.0.md) |
| CU-10 | Registrar un servidor con su credencial de acceso | NB-05 | Propuesto | [CU-10](casos-de-uso/CU-10-registrar-servidor-con-token_v1.0.md) |
| CU-11 | Administrar reglas, grupos, eventos, acciones y parámetros con ayuda contextual | NB-05 | Propuesto | [CU-11](casos-de-uso/CU-11-administrar-reglas-grupos-eventos-acciones_v1.0.md) |
| CU-12 | Probar la configuración de un servidor antes de activarlo | NB-06 | Propuesto | [CU-12](casos-de-uso/CU-12-probar-configuracion-servidor_v1.0.md) |
| CU-13 | Reconectar automáticamente y mostrar el estado de conexión de cada servidor | NB-06 | Propuesto | [CU-13](casos-de-uso/CU-13-reconectar-y-mostrar-estado-conexion_v1.0.md) |
| CU-14 | Ejecutar una política en modo simulación registrando lo que haría sin ejecutarlo | NB-07 | Propuesto | [CU-14](casos-de-uso/CU-14-ejecutar-politica-modo-simulacion_v1.0.md) |
| CU-15 | Definir exenciones por rol, usuario o canal de confianza | NB-07 | Propuesto | [CU-15](casos-de-uso/CU-15-definir-exenciones_v1.0.md) |
| CU-16 | Evitar acciones repetidas sobre el mismo usuario durante una ráfaga | NB-07 | Propuesto | [CU-16](casos-de-uso/CU-16-antirrebote-por-usuario_v1.0.md) |

## 5. Catálogo de reglas de negocio

| RN | Título | CU afectados | Estado | Archivo |
| --- | --- | --- | --- | --- |
| RN-01 | Jerarquía de roles del bot para accionar | CU-02, CU-03, CU-04, CU-07, CU-12 | Propuesto | [RN-01](reglas-de-negocio/RN-01-jerarquia-de-roles-del-bot_v1.0.md) |
| RN-02 | Tope del borrado retroactivo de mensajes | CU-03, CU-11 | Propuesto | [RN-02](reglas-de-negocio/RN-02-tope-borrado-retroactivo_v1.0.md) |
| RN-03 | Validez del patrón de una regla de contenido | CU-04, CU-11 | Propuesto | [RN-03](reglas-de-negocio/RN-03-validez-patron-de-contenido_v1.0.md) |
| RN-04 | Evaluación de políticas por prioridad con primera coincidencia | CU-01, CU-02, CU-04, CU-11, CU-14 | Propuesto | [RN-04](reglas-de-negocio/RN-04-evaluacion-politicas-por-prioridad_v1.0.md) |
| RN-05 | Orden de ejecución de las acciones de una política | CU-02, CU-03, CU-04, CU-11 | Propuesto | [RN-05](reglas-de-negocio/RN-05-orden-de-ejecucion-de-acciones_v1.0.md) |
| RN-06 | Antirrebote por usuario durante una ráfaga | CU-02, CU-16 | Propuesto | [RN-06](reglas-de-negocio/RN-06-antirrebote-por-usuario_v1.0.md) |
| RN-07 | Descarte previo de los sujetos exentos | CU-01, CU-02, CU-04, CU-15 | Propuesto | [RN-07](reglas-de-negocio/RN-07-descarte-previo-de-exentos_v1.0.md) |
| RN-08 | Identidad de los snowflakes de la plataforma | CU-10, CU-15 | Propuesto | [RN-08](reglas-de-negocio/RN-08-identidad-de-snowflakes_v1.0.md) |
| RN-09 | El modo simulación no ejecuta acción real | CU-02, CU-04, CU-05, CU-11, CU-14 | Propuesto | [RN-09](reglas-de-negocio/RN-09-modo-simulacion-no-ejecuta_v1.0.md) |
| RN-10 | Configuración dirigida por descriptor de parámetro | CU-01, CU-11, CU-16 | Propuesto | [RN-10](reglas-de-negocio/RN-10-configuracion-dirigida-por-descriptor_v1.0.md) |
| RN-11 | Integridad de la evidencia del incidente | CU-05, CU-06 | Propuesto | [RN-11](reglas-de-negocio/RN-11-integridad-del-incidente_v1.0.md) |
| RN-12 | Autorización por rol administrador único | CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12, CU-15 | Propuesto | [RN-12](reglas-de-negocio/RN-12-autorizacion-rol-administrador-unico_v1.0.md) |
| RN-13 | Resguardo de las credenciales del administrador y cuenta única | CU-08, CU-09 | Propuesto | [RN-13](reglas-de-negocio/RN-13-resguardo-de-credenciales-del-administrador_v1.0.md) |
| RN-14 | Cifrado del token de bot en reposo | CU-10, CU-12, CU-13 | Propuesto | [RN-14](reglas-de-negocio/RN-14-cifrado-del-token-en-reposo_v1.0.md) |
| RN-15 | Composición de un grupo de reglas | CU-11 | Propuesto | [RN-15](reglas-de-negocio/RN-15-composicion-de-grupo-de-reglas_v1.0.md) |
| RN-16 | Activación de un servidor condicionada a la prueba de configuración | CU-12, CU-13 | Propuesto | [RN-16](reglas-de-negocio/RN-16-activacion-condicionada-a-prueba_v1.0.md) |

## 6. Modelo conceptual y reglas conceptuales

El modelo conceptual de datos vive en [modelo-conceptual_v1.0.md](modelo-datos/modelo-conceptual_v1.0.md) y comprende 13 entidades (Administrador, Servidor, CanalDeSalida, Exención, Regla, GrupoDeReglas, GrupoRegla, Evento, EventoGrupo, Acción, Incidente, MensajeAccionado, CanalAfectado). Por superar las 10 entidades, se generan las reglas conceptuales (RC) obligatorias.

| RC | Título | Archivo |
| --- | --- | --- |
| RC-01 | Integridad referencial de las entidades dependientes | [RC-01](modelo-datos/reglas-conceptuales-de-modelo/RC-01-integridad-referencial-dependientes_v1.0.md) |
| RC-02 | Identidad de los snowflakes almacenados como texto | [RC-02](modelo-datos/reglas-conceptuales-de-modelo/RC-02-identidad-de-snowflakes_v1.0.md) |
| RC-03 | Integridad de las asociaciones GrupoRegla y EventoGrupo | [RC-03](modelo-datos/reglas-conceptuales-de-modelo/RC-03-integridad-asociaciones-grupo-evento_v1.0.md) |
| RC-04 | Composición mínima y modo de coincidencia de un grupo | [RC-04](modelo-datos/reglas-conceptuales-de-modelo/RC-04-composicion-minima-de-grupo_v1.0.md) |
| RC-05 | Orden determinista de eventos y de acciones | [RC-05](modelo-datos/reglas-conceptuales-de-modelo/RC-05-orden-determinista-evento-accion_v1.0.md) |
| RC-06 | Unicidad y resguardo de la cuenta administrador | [RC-06](modelo-datos/reglas-conceptuales-de-modelo/RC-06-unicidad-de-administrador_v1.0.md) |
| RC-07 | Confidencialidad del token del servidor | [RC-07](modelo-datos/reglas-conceptuales-de-modelo/RC-07-confidencialidad-del-token_v1.0.md) |
| RC-08 | Activación de un servidor condicionada a la prueba | [RC-08](modelo-datos/reglas-conceptuales-de-modelo/RC-08-activacion-condicionada-de-servidor_v1.0.md) |
| RC-09 | Validez del criterio de una regla según su clase | [RC-09](modelo-datos/reglas-conceptuales-de-modelo/RC-09-validez-del-criterio-de-regla_v1.0.md) |
| RC-10 | Coherencia del modo entre evento e incidente | [RC-10](modelo-datos/reglas-conceptuales-de-modelo/RC-10-coherencia-de-modo-evento-incidente_v1.0.md) |
| RC-11 | Tope de la ventana de borrado de una acción | [RC-11](modelo-datos/reglas-conceptuales-de-modelo/RC-11-tope-ventana-de-borrado_v1.0.md) |

## 7. Alcance deliberadamente fuera de la categoría 02

Coherente con el alcance excluido de 00 y con el intake: no se especifican el asistente de propuesta de configuración por modelo de lenguaje (frontera reservada, no construida en v1), la gestión de convivencia con el filtro nativo de la plataforma, la operación multi-servidor a escala masiva, la restauración de mensajes borrados (no posible por la plataforma), la moderación por reputación o historial, ni el anidamiento booleano más allá de dos niveles (grupo de reglas y combinación de grupos). La interacción multiusuario no aplica como concurrencia entre operadores, porque hay un único administrador; la concurrencia relevante es entre el bot (auditoría) y el panel (configuración) sobre la persistencia, tratada en cada CU y resuelta en 05.

## 8. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Índice maestro inicial con 16 CU, 16 RN, modelo de 13 entidades y 11 RC, derivado de las siete NB y del intake |
