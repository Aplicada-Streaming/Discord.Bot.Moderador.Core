# Arquitectura de solución — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)

## 1. Objetivo

Este documento describe la arquitectura técnica del servicio monolítico de administración y moderación de servidores de la plataforma de mensajería (proyecto `discord-bots-admin`, código `DiscordModeradorBot.Servicio`). Está dirigido a quienes construyen, prueban, despliegan y mantienen el sistema (categorías 06 a 11), y fija el cómo estructural —estilo, vistas lógica/procesos/despliegue/datos, decisiones transversales y atributos de calidad— sin descender al detalle de implementación de cada caso de uso. El stack concreto vive en `SOLUTION-INTAKE §17 P.1` y no se repite en el cuerpo; aquí se describe por capacidad y por rol.

## 2. Estilo arquitectónico

El estilo elegido es Clean Architecture por capas (Dominio, Aplicación, Infraestructura, Presentación) con dos características distintivas del dominio:

- Un pipeline de evaluación como núcleo del dominio: el motor de moderación procesa cada mensaje entrante a través de una secuencia ordenada de etapas (descarte de exentos, reglas de contenido, actualización de actividad reciente, evaluación de políticas por prioridad, ejecución de acciones, registro del incidente). El pipeline es la forma natural del problema: el flujo de moderación de §6 del intake describe una transformación lineal de un mensaje hasta un incidente.
- El dominio modelado como firewall multi-contexto: cada servidor registrado es un contexto independiente con su propia credencial de acceso, su propia conexión al canal de eventos de la plataforma, sus propias reglas, políticas, exenciones y estado de conducta. La operación de varios servidores se resuelve como multi-contexto dentro de una misma instancia, no como multi-tenant (un único administrador opera todo).

El bot corre como servicio en segundo plano dentro del mismo host web que sirve el panel; ambos comparten la persistencia y la composición de dependencias. El punto de entrada del administrador es el panel web con render interactivo del lado servidor; el punto de entrada de la moderación es el flujo de eventos del canal en tiempo real de la plataforma.

La dependencia entre capas es unidireccional hacia el Dominio: Presentación e Infraestructura dependen de Aplicación y Dominio; el Dominio no depende de ninguna capa externa. Esto permite probar el motor de moderación sin infraestructura (gate de cobertura ≥ 90 % del módulo de detección, `SOLUTION-INTAKE §17 P.6`).

### 2.1 Alternativas descartadas

| Alternativa | Pros | Contras | Motivo del descarte |
| --- | --- | --- | --- |
| Microservicios (panel, bot y motor como servicios autónomos) | Despliegue independiente por servicio; escalado horizontal por carga | Complejidad operativa alta; comunicación entre servicios; sobrecarga de memoria y proceso desproporcionada para el contexto de despliegue (hardware ARM de 32 bits, un operador) | Desproporcionado para un único operador desplegando en hardware de bajo consumo. Viola la restricción de memoria de `SOLUTION-INTAKE §10` y §17 P.12. Decisión cerrada en `SOLUTION-INTAKE §17 P.11` (ver ADR-01). |
| Bot y panel en procesos o servicios separados | Aislamiento de fallos entre la cara de administración y la de moderación | Requiere comunicación inter-proceso y un canal compartido de configuración; duplica la composición de dependencias; complica la instalación y el rollback en el dispositivo | Requisito explícito del cliente de monolito de un solo proyecto y de simplicidad de instalación y rollback (`SOLUTION-INTAKE §14`, §17 P.11). Ver ADR-01. |
| Capas planas sin separación de dominio (lógica de moderación acoplada a la infraestructura del cliente del canal de eventos) | Menos archivos al inicio | Imposible probar el dominio sin infraestructura; el motor de detección queda acoplado a la librería del gateway | No alcanza el gate de cobertura del módulo crítico ni la testabilidad exigida. Ver ADR-04. |

La tabla de criterios de elección (capas frente a pipeline, hexagonal, microservicios y event-driven) confirma la elección: equipo de 1 desarrollador, 1 dominio de negocio, sin requisito de deploy independiente, complejidad operativa baja y time-to-market rápido apuntan a capas con núcleo de pipeline.

## 3. Vista lógica

Componentes (módulos con responsabilidad cohesiva, no clases). Las dependencias son unidireccionales hacia el Dominio.

| Componente | Capa | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- | --- |
| Panel de administración | Presentación | Interfaz web interactiva del lado servidor para configuración, registro de servidores, revisión de incidentes y desbaneo | Acciones del administrador (HTTP/render interactivo) | Comandos y consultas a Aplicación | Aplicación | CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12, CU-13, CU-15 |
| Servicio de autenticación | Aplicación | Alta de credenciales en primer ingreso, autenticación, gestión de sesión, rol administrador único | Credenciales | Sesión, decisión de autorización | Dominio, Persistencia | CU-08, CU-09 |
| Servicio de configuración dirigida por descriptores | Aplicación | Validación de parámetros contra descriptores (default, límites, leyenda, ejemplos), CRUD de reglas, grupos, eventos, acciones y exenciones | Comandos del panel | Entidades validadas, errores de validación | Dominio, Persistencia, Registro de descriptores | CU-11, CU-15 |
| Registro de descriptores de parámetro | Dominio | Fuente única de verdad de cada parámetro configurable (default, límites, leyenda, ejemplos) | — (definición estática) | Descriptores | Ninguna | CU-01, CU-11, CU-16 |
| Registro de servidores | Aplicación | Alta y administración de servidores (contextos del firewall), cifrado del token en reposo, estado de conexión y activación | Datos de registro, token | Servidor persistido (token cifrado) | Dominio, Persistencia, Servicio de cifrado | CU-10, CU-13 |
| Servicio de cifrado de tokens | Infraestructura | Cifra y descifra el token con clave maestra fuera de la base | Token en claro / cifrado | Token cifrado / en claro (solo en memoria) | Proveedor de clave maestra | CU-10, CU-12 |
| Prueba de configuración | Aplicación | Valida token, permisos, jerarquía de roles, intents y canales antes de activar; clasifica fallas bloqueantes y advertencias | Servidor registrado | Resultado de prueba, decisión de activación | Adaptador del gateway, Servicio de cifrado | CU-12 |
| Motor de moderación (pipeline) | Dominio | Núcleo: orquesta el pipeline de evaluación de un mensaje entrante hasta el incidente | Mensaje entrante normalizado | Decisión de acción / incidente / simulación | Evaluador de contenido, Evaluador de conducta, Evaluador de políticas, Estado de conducta, Antirrebote | CU-01, CU-02, CU-04, CU-14, CU-16 |
| Evaluador de reglas de contenido | Dominio | Predicado sin estado sobre un mensaje aislado (expresión regular o palabras clave) | Mensaje | Coincidencia booleana | Registro de descriptores | CU-04 |
| Evaluador de reglas de conducta | Dominio | Predicado con estado sobre la actividad reciente del usuario (canales distintos, frecuencia) | Mensaje, estado de conducta | Coincidencia booleana | Estado de conducta en memoria | CU-01 |
| Estado de conducta en memoria | Dominio (en memoria) | Ventanas deslizantes de actividad reciente por usuario y por contexto; no persistido | Eventos de actividad | Métricas de ventana | Ninguna | CU-01, CU-16 |
| Evaluador de políticas | Dominio | Evalúa eventos por prioridad con primera coincidencia y bandera continuar | Coincidencias de grupos | Evento(s) disparado(s) | Registro de descriptores | CU-01, CU-02, CU-04, CU-11, CU-14 |
| Antirrebote por usuario | Dominio (en memoria) | Suprime acciones repetidas sobre el mismo usuario durante una ráfaga; no persistido | Disparo de acción | Decisión de suprimir o ejecutar | Ninguna | CU-16 |
| Ejecutor de acciones | Aplicación | Ejecuta las acciones del evento en orden (reportar, banear, banear con borrado retroactivo, desbanear, timeout, expulsar, rol) o registra la simulación | Evento disparado, copia de mensajes | Llamadas al adaptador del gateway, incidente | Adaptador del gateway, Persistencia | CU-02, CU-03, CU-04, CU-05, CU-07, CU-14 |
| Servicio de incidentes | Aplicación | Toma de copia de mensajes y canales afectados, registro del incidente, reversión (desbaneo) | Resultado de acción | Incidente persistido | Persistencia | CU-05, CU-06, CU-07, CU-14 |
| Adaptador del gateway y de la API de la plataforma | Infraestructura | Cliente del canal de eventos en tiempo real y de las operaciones de moderación; reconexión automática; estado de conexión por contexto | Eventos del canal / comandos de acción | Mensajes normalizados / resultado de la operación | Librería cliente del gateway (intake §17 P.1) | CU-01, CU-02, CU-03, CU-04, CU-05, CU-07, CU-12, CU-13 |
| Persistencia | Infraestructura | Acceso a la base relacional embebida con migraciones versionadas (modo WAL) | Entidades | Filas; entidades materializadas | ORM con migraciones (intake §17 P.1) | CU-05, CU-06, CU-07, CU-10, CU-11, CU-15 |

Cobertura de los 16 CU: CU-01..CU-16 quedan cubiertos por al menos un componente (ver §10 Trazabilidad). Ningún CU queda huérfano.

## 4. Vista de procesos

El proceso único hospeda dos caras concurrentes sobre una misma persistencia:

- Cara de moderación (bot, servicio en segundo plano): por cada servidor registrado y activo se mantiene una conexión al canal de eventos en tiempo real de la plataforma. Cada mensaje entrante recorre el pipeline del motor de moderación. El pipeline es mayormente sin bloqueo; las únicas operaciones de E/S son las llamadas de acción a la API de la plataforma y el registro del incidente.
- Cara de administración (panel): atiende las interacciones del administrador mediante render interactivo del lado servidor. Lee y escribe configuración e incidentes.

Concurrencia y estado:

- Concurrencia bot/panel sobre la persistencia: la base relacional embebida opera en modo de registro de escritura anticipada (WAL) para tolerar escrituras concurrentes del bot (auditoría de incidentes) y del panel (configuración) sin bloqueo de lectores. Las escrituras del panel son de baja frecuencia (configuración); las del bot son por incidente.
- Estado de conducta en memoria: las ventanas deslizantes de actividad reciente por usuario viven en memoria, particionadas por contexto (servidor). No se persisten; se reconstruyen con el tráfico tras un reinicio (trade-off aceptado en `SOLUTION-INTAKE §17 P.12`, ver ADR-09).
- Antirrebote en memoria: el estado de antirrebote por usuario vive en memoria con una ventana de supresión; también se pierde ante un reinicio (ver ADR-09).
- Reconexión del gateway: el adaptador detecta la caída del canal de eventos y reconecta automáticamente con reintentos; mientras está caído, el contexto figura como desconectado en el panel y los mensajes no recibidos no se evalúan. El borrado retroactivo al banear cubre lo que haya quedado dentro de la ventana de borrado (`SOLUTION-INTAKE §7`, CU-13).
- Orden y atomicidad: las acciones de un evento se ejecutan en orden determinista (RN-05); la copia de mensajes se toma antes de cualquier borrado (RN-11). El registro del incidente se confirma como una unidad.
- Primera coincidencia: las políticas se evalúan por prioridad y se detienen en la primera coincidencia salvo que la bandera continuar esté activa (RN-04).

## 5. Vista de despliegue

- Unidad de despliegue: un único artefacto publicado en modo self-contained para la arquitectura de CPU objetivo (ARM de 32 bits), sin dependencia de un runtime instalado en el sistema operativo. El artefacto incluye el host web, el bot y la persistencia embebida en un solo proceso.
- Runtime objetivo: dispositivo de bajo consumo con sistema operativo de 32 bits (armv7l), auto-hospedado, sin contenedores. La publicación se genera por compilación cruzada desde una estación x64 (`SOLUTION-INTAKE §17 P.8`).
- Registro como servicio del sistema: el artefacto se instala mediante un paquete con todo lo necesario y queda corriendo como servicio gestionado por el supervisor de servicios del sistema (systemd), con reinicio automático.
- Dependencias de infraestructura: ninguna dependencia de terceros ni de red más allá del acceso al gateway y a la API de la plataforma de mensajería. La base es un archivo local en el dispositivo. La clave maestra de cifrado vive en una variable de entorno del servicio, en un archivo de entorno con permisos restringidos, fuera de la base (ver ADR-07).
- Rollback: la reinstalación de la publicación anterior conserva el archivo de entorno y la clave maestra, de modo que los tokens cifrados siguen siendo válidos (`SOLUTION-INTAKE §17 P.8`).
- Panel: accesible desde navegadores evergreen (`SOLUTION-INTAKE §17 P.9`).

Ver ADR-05 para la decisión de despliegue self-contained con servicio del sistema.

## 6. Vista de datos

- Persistencia principal: base de datos relacional embebida en archivo, en modo WAL, accedida mediante un ORM con migraciones versionadas. Almacena servidores (con token cifrado), canales de salida, exenciones, reglas (de contenido y de conducta), grupos de reglas y su relación con reglas, eventos, su relación con grupos, acciones, incidentes con su copia de mensajes y canales afectados, y el administrador. El detalle de tablas, tipos físicos, índices, restricciones y migración inicial vive en `modelo-datos-logico_v1.0.md`.
- Identificadores de la plataforma (snowflakes): se almacenan como texto para preservar el valor exacto de 64 bits y evitar el desborde del entero con signo (RN-08, RC-02, ver ADR-02 de persistencia).
- Token de bot: cifrado en reposo; nunca en texto claro en la base (RN-14, RC-07, ver ADR-07).
- Cache y estado efímero: el estado de conducta y el antirrebote son caches en memoria, no entidades persistidas (ver ADR-09). No hay otra cache de datos.
- Particionamiento: no aplica sharding ni particionamiento; volumen acotado a un dispositivo y, en v1, a un servidor. No es multi-tenant; la operación multi-servidor es multi-contexto dentro de la misma base (`SOLUTION-INTAKE §17 P.4`, ver `modelo-datos-logico_v1.0.md`).
- Retención y minimización: se guardan solo los identificadores necesarios y una copia de mensajes para revisión; retención acotada conforme al marco legal local (ver §7 y ADR-06).

## 7. Cross-cutting concerns

Decisiones transversales centralizadas:

- Logging y observabilidad: registro de eventos del servicio al journal del sistema. Cada incidente queda auditado en la base (disparo, copia de mensajes, canales afectados, modo y acción resultante), lo que constituye el rastro de auditoría del dominio (RN-11). No hay tracing distribuido por ser un único proceso.
- Manejo de errores: el dominio expresa los resultados de moderación como un conjunto cerrado (ejecutada, simulada, no accionable, fallida). Las fallas de acción sobre la plataforma (jerarquía de roles insuficiente, permisos faltantes) no detienen el pipeline: se registran como incidente no accionable y se reportan al canal privado (RN-01, `SOLUTION-INTAKE §7`). El panel presenta los errores de validación de configuración derivados de los descriptores (RN-10). Política detallada en ADR-08.
- Configuración: dirigida por esquema. Cada parámetro configurable se describe con un descriptor único que es fuente de verdad de su default, sus límites, su leyenda y sus ejemplos (RN-10, ver ADR-12). La recarga en caliente de la configuración queda abierta a Sprint 0 (`SOLUTION-INTAKE §17 P.11`).
- Secretos: la clave maestra de cifrado de tokens vive en una variable de entorno del servicio, fuera de la base; el token se cifra en reposo con cifrado simétrico (AES) y se descifra solo en memoria para operar (RN-14, RC-07, ver ADR-07).
- Seguridad reforzada:
  - Autenticación del panel con administrador único, credenciales creadas en el primer ingreso y contraseña almacenada con hash robusto en formato PHC (familia Argon2 o PBKDF2; elección puntual abierta a Sprint 0). Nunca en texto claro (RN-13, RC-06, ver ADR-03).
  - Autorización por rol administrador único; solo el administrador registra servidores, configura y desbanea (RN-12).
  - Cifrado de tokens en reposo con clave maestra fuera de la base (RN-14, ver ADR-07).
  - Minimización y retención acotada de datos personales; residencia local sin terceros (ver ADR-06, marco Ley 25.326).
- Compliance: tratamiento de datos personales conforme a la Ley 25.326 de Protección de Datos Personales (residencia local, minimización, retención acotada). Decisión registrada en ADR-06.

## 8. Quality attributes (NFR)

Valores numéricos tomados de `SOLUTION-INTAKE §17 P.10`.

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Latencia de procesamiento por mensaje | p95 < 200 ms | Instrumentación del pipeline (marca de tiempo de entrada a salida de decisión); percentil agregado sobre el journal y banco de pruebas de carga en el hardware real | ADR-01, ADR-09 |
| Throughput sostenido | ≥ 50 mensajes/s en el dispositivo de referencia | Banco de pruebas de carga con mensajes simulados sobre el hardware real (a confirmar por benchmark) | ADR-01, ADR-09 |
| Disponibilidad mensual (SLO) | 99 % mensual | Tiempo de servicio sobre tiempo total del mes, derivado del journal y del estado de conexión por contexto; reinicio automático del servicio | ADR-05, ADR-13 |
| Memoria por conexión de gateway activa | ≤ 8 MB por conexión | Medición de huella de memoria por contexto activo en el dispositivo; perfilado en carga | ADR-13 (firewall multi-contexto: una conexión por servidor) |
| Cobertura de tests del módulo de detección | ≥ 90 % (líneas); global líneas ≥ 75 %, branches ≥ 65 % | Gate de cobertura en el pipeline de integración continua (`SOLUTION-INTAKE §17 P.6`) | ADR-04 |
| Limpieza efectiva de la ráfaga | ≥ 98 % de mensajes eliminados dentro de los 10 s del incidente | Registro de incidentes con canales afectados; métrica de negocio (vision §6) verificada en pruebas | ADR-08, ADR-09 |

Nota: la ventana de detección (por defecto del orden de 2 s, a calibrar) y el tope de borrado retroactivo (7 días por la plataforma) son parámetros de configuración, no NFR; viven en los descriptores y en la acción (RC-11, RN-02).

## 9. Riesgos arquitectónicos

| Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- |
| Pérdida del estado de conducta y antirrebote ante un reinicio del servicio | Medio (una ráfaga en curso podría no cortarse hasta reconstruir la ventana; posible acción repetida) | Media | Estado reconstruido con el tráfico; el borrado retroactivo limpia lo previo dentro de la ventana; persistir contadores queda abierto a Sprint 0 (ADR-09, `SOLUTION-INTAKE §17 P.11`) |
| Sobrecarga de memoria del proceso de 32 bits al sumar contextos | Alto (puede exceder la memoria del dispositivo) | Media | v1 opera un solo servidor; presupuesto de ≤ 8 MB por conexión; firewall multi-contexto con una conexión por servidor (ADR-13, NFR memoria) |
| Filtración del token de bot | Alto (control total del bot) | Baja | Cifrado en reposo con clave maestra fuera de la base; archivo de entorno con permisos restringidos; el rollback preserva la clave (ADR-07, RN-14) |
| Falsos positivos de moderación | Alto (banear a un usuario legítimo) | Media | Discriminar por canales distintos; exenciones; modo simulación por defecto; reporte a canal privado; revisión y desbaneo desde el panel (ADR-09, RN-07, RN-09) |
| Caída del gateway o token revocado deja sin moderación | Alto | Baja | Reconexión automática; estado de conexión visible; prueba de configuración bloquea la activación con token inválido (CU-12, CU-13, RN-16) |
| Expresión regular maliciosa o costosa en una regla de contenido | Medio (consumo de CPU, retroceso catastrófico) | Baja | Validación del patrón al guardar (RN-03); evaluación con tope de tiempo; cobertura del evaluador de contenido (ADR-08) |
| Latencia p95 inalcanzable en hardware de 32 bits | Medio | Media | Pipeline mayormente sin bloqueo; estado en memoria; benchmark en hardware real; ARM de 32 bits es tier deprioritizado aceptado (`SOLUTION-INTAKE §17 P.12`, ADR-01) |

## 10. Trazabilidad

Cobertura de CU por componente y gobierno por ADR/RN. Tests previstos se materializan en la categoría 08.

| CU | Componente(s) que lo cubren | RN aplicables | ADR que lo gobiernan | Tests previstos (08) |
| --- | --- | --- | --- | --- |
| CU-01 | Motor de moderación, Evaluador de conducta, Estado de conducta, Evaluador de políticas, Adaptador del gateway | RN-04, RN-07, RN-08, RN-10 | ADR-01, ADR-04, ADR-08, ADR-09, ADR-12 | Unitarias del evaluador de conducta y de la ventana deslizante; integración del pipeline |
| CU-02 | Motor de moderación, Ejecutor de acciones, Adaptador del gateway, Servicio de incidentes | RN-01, RN-04, RN-05, RN-06, RN-07, RN-09 | ADR-01, ADR-08, ADR-09 | Unitarias de orden de acciones; integración de baneo |
| CU-03 | Ejecutor de acciones, Adaptador del gateway, Servicio de incidentes | RN-01, RN-02, RN-05 | ADR-02, ADR-08 | Unitarias de tope de ventana de borrado; integración |
| CU-04 | Motor de moderación, Evaluador de contenido, Evaluador de políticas, Ejecutor de acciones | RN-03, RN-04, RN-05, RN-07, RN-09 | ADR-01, ADR-04, ADR-08 | Unitarias del evaluador de contenido; tope de tiempo de regex |
| CU-05 | Servicio de incidentes, Ejecutor de acciones, Adaptador del gateway, Persistencia | RN-01, RN-09, RN-11 | ADR-02, ADR-08 | Integración de reporte; copia de mensajes antes de borrar |
| CU-06 | Panel de administración, Servicio de incidentes, Persistencia | RN-09, RN-11, RN-12 | ADR-02, ADR-03 | E2E del panel de incidentes; autorización |
| CU-07 | Panel de administración, Ejecutor de acciones, Adaptador del gateway, Servicio de incidentes | RN-01, RN-12 | ADR-03, ADR-08 | Integración de desbaneo; autorización |
| CU-08 | Panel de administración, Servicio de autenticación, Persistencia | RN-12, RN-13 | ADR-03 | Unitarias de hashing; primer ingreso |
| CU-09 | Panel de administración, Servicio de autenticación | RN-12, RN-13 | ADR-03 | Unitarias de verificación de hash; sesión |
| CU-10 | Panel de administración, Registro de servidores, Servicio de cifrado, Persistencia | RN-08, RN-12, RN-14 | ADR-02, ADR-07, ADR-08 | Unitarias de cifrado; integración de registro |
| CU-11 | Panel de administración, Servicio de configuración, Registro de descriptores, Persistencia | RN-04, RN-05, RN-09, RN-10, RN-12, RN-15 | ADR-02, ADR-03, ADR-12 | Unitarias de validación por descriptor; integridad de grupos |
| CU-12 | Prueba de configuración, Adaptador del gateway, Servicio de cifrado | RN-01, RN-12, RN-14, RN-16 | ADR-07, ADR-13 | Integración de prueba de token/permisos/jerarquía |
| CU-13 | Adaptador del gateway, Registro de servidores, Panel de administración | RN-14, RN-16 | ADR-07, ADR-13 | Integración de reconexión; estado de conexión |
| CU-14 | Motor de moderación, Evaluador de políticas, Servicio de incidentes | RN-04, RN-09 | ADR-09, ADR-08 | Unitarias de modo simulación (no ejecuta) |
| CU-15 | Panel de administración, Servicio de configuración, Persistencia, (filtro de exentos en el pipeline) | RN-07, RN-08, RN-12 | ADR-02, ADR-08 | Unitarias de descarte de exentos; integración |
| CU-16 | Motor de moderación, Antirrebote por usuario, Registro de descriptores | RN-06, RN-10 | ADR-09, ADR-12 | Unitarias de supresión por ventana |

NFR↔arquitectura↔ADR: la tabla §8 liga cada NFR a su mecanismo de medición y a la ADR que lo gobierna. RN upstream: las 16 RN quedan referenciadas en la columna de RN de esta tabla y en las ADR correspondientes. Entidades del modelo conceptual: trazadas entidad por entidad en `modelo-datos-logico_v1.0.md`.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial de la arquitectura de solución para `discord-bots-admin`: estilo de capas con núcleo de pipeline y firewall multi-contexto, cuatro vistas, cross-cutting con seguridad reforzada, tabla de NFR con valores de `SOLUTION-INTAKE §17 P.10` y trazabilidad de los 16 CU. Ajuste de atribuciones NFR↔ADR (memoria por conexión → ADR-13; limpieza efectiva → ADR-08/ADR-09) y alineación de la cobertura de CU del Servicio de cifrado con la tabla de trazabilidad §10 (observaciones P3-02/03/04 del audit de Fase C). |
