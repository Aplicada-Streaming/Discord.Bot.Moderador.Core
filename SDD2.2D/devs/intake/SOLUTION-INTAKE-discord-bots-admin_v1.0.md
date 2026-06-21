# SOLUTION-INTAKE-discord-bots-admin

| Campo | Valor |
|---|---|
| Nombre de la solución | Administrador de Bots Moderador para Discord |
| Cliente / Stakeholder principal | Administrador propietario de los servidores de Discord a moderar (solicitante del sistema) |
| Repositorio | Discord.Bot.Moderador.Core |
| Lead técnico | Fernando — desarrollador full-stack, responsable técnico y de arquitectura |
| Documento | `SOLUTION-INTAKE-discord-bots-admin_v1.0.md` |
| Versión | 1.0 |
| Fecha | 2026-06-20 |
| Stack principal | .NET 10 / C# — Blazor Server (MudBlazor) + Discord.Net + SQLite (EF Core) |
| Estado | En revisión |

> Este documento captura qué quiere el cliente, cómo se compone la solución y cómo se construye cada proyecto.
> El orquestador deriva de §13 el `SOLUTION-MANIFEST` canónico; no se completa el manifiesto a mano.

---

# Parte A — Negocio de la solución

## §1 Idea y problema

Los servidores de Discord del solicitante sufren ataques de spam. El caso más común ocurre cuando se vulneran las credenciales de un usuario del servidor y un atacante usa esa cuenta para enviar ráfagas de spam —habitualmente imágenes adjuntas— a todos los canales, uno tras otro, en pocos segundos. La moderación manual no llega a tiempo: para cuando un humano reacciona, el spam ya inundó diez o veinte canales y hay que limpiarlos a mano.

Hoy el solicitante depende del filtro nativo de Discord (AutoMod) configurado con expresiones regulares, que tiene un comportamiento errático frente a este patrón y no corta la ráfaga distribuida entre canales. De hecho, el solicitante observa que llega un primer mensaje de spam y, al rato, llegan en ráfaga los mensajes a los demás canales.

Si no se construye la solución en los próximos meses, los servidores seguirán expuestos a inundaciones de spam ante cada cuenta comprometida, con la consiguiente carga de limpieza manual, degradación de la experiencia de los miembros y riesgo de pérdida de comunidad.

El disparador es concreto y actual: el solicitante está sufriendo estos incidentes y el mecanismo actual no los contiene.

## §2 Audiencia y stakeholders

| Rol | Nombre o cargo | Categoría | Responsabilidad principal |
|---|---|---|---|
| Dueño del problema | Administrador propietario de los servidores de Discord | Propietario | Aprueba el intake y opera el sistema |
| Usuario administrador | Administrador del sistema (rol único de la aplicación) | Beneficiario / operador | Registra servidores, configura reglas y revisa incidentes |
| Implementador | Fernando | Implementador | Construcción y mantenimiento, con asistencia de IA en el desarrollo |
| Miembros de los servidores | Comunidades de Discord moderadas | Beneficiario indirecto | Reciben un servidor libre de spam |

## §3 Propuesta de valor y diferenciación

Hoy el cliente confía la moderación al AutoMod nativo de Discord con expresiones regulares, que no corta de forma confiable las ráfagas de spam repartidas entre canales y se comporta de manera irregular.

La promesa central es detectar el patrón que de verdad delata al spam automatizado —el envío casi simultáneo a múltiples canales distintos, algo físicamente imposible para un humano— y cortarlo al instante baneando al emisor, lo que además limpia sus mensajes de todos los canales en una sola operación, con un reporte a un canal privado para revisar posibles falsos positivos.

Diferenciadores defendibles: un motor de reglas configurable de estilo firewall multi-contexto, adaptado a los patrones de spam reales del cliente; auto-hospedado en una Raspberry Pi propia, sin dependencia de servicios de terceros; capaz de operar varios servidores; con configuración dirigida por esquema (ayudas y ejemplos en pantalla) y una frontera reservada para que, a futuro, un asistente de IA proponga configuraciones.

## §4 Alcance funcional pretendido (MoSCoW)

Must Have v1:
- Detectar el patrón de ráfaga distribuida: un usuario que postea en N canales distintos dentro de una ventana corta de tiempo, y banearlo automáticamente.
- Banear con borrado retroactivo de los mensajes del usuario en todos los canales, con la ventana hacia atrás configurable (tope de plataforma: 7 días).
- Detectar contenido no deseado en un mensaje mediante expresión regular y banear al emisor.
- Reportar a un canal privado los mensajes que dispararon la acción y los canales afectados, para revisión de falsos positivos.
- Panel web de administración para registrar el servidor y su token, y administrar reglas, grupos de reglas, eventos y acciones.
- Usuario administrador único con alta de credenciales en el primer ingreso.
- Umbrales y ventanas configurables por el administrador, con valor por defecto y ayuda contextual en pantalla (leyenda y ejemplos por parámetro).

Should Have v1:
- Regla de conducta por volumen de mensajes de un usuario en un canal.
- Reglas de contenido por palabras o frases clave.
- Acciones adicionales sobre el usuario: timeout, expulsión, asignar o quitar rol.
- Exenciones por rol, usuario o canal de confianza (p. ej. staff).
- Modo simulación por evento (registra lo que haría sin ejecutarlo) antes de pasar a ejecución real.
- Antirrebote por usuario para no repetir acciones durante una ráfaga.
- Prueba de configuración al registrar un servidor (validación de token, permisos, intents y canales).
- Revisar incidentes y mensajes accionados desde el panel.
- Revertir un baneo (desbanear) desde el panel.

Could Have v1:
- Explicación bidireccional de una regla en lenguaje natural a partir de su configuración.
- Divulgación progresiva de parámetros avanzados en el panel.
- Múltiples canales de salida con propósito lógico distinto (p. ej. reporte y volcado completo).

Won't Have v1:
- Motor de IA que proponga configuraciones por prompt (solo se reserva la frontera, no se construye).
- Operación multi-servidor a escala (v1 prevista para un servidor).
- Gestión de la convivencia con AutoMod de Discord.
- Anidamiento booleano de reglas más allá de dos niveles (grupo y combinación de grupos).
- Restauración de mensajes borrados (no es posible por la API de Discord).

## §5 Historias de usuario

- Como administrador, quiero que el bot banee automáticamente a quien postee en varios canales distintos en pocos segundos, para cortar el spam antes de que inunde el servidor.
- Como administrador, quiero configurar el umbral de canales y la ventana de tiempo, con un valor por defecto y ayuda en pantalla que explique cada parámetro con ejemplos, para ajustar la sensibilidad sin necesidad de conocimiento técnico profundo.
- Como administrador, quiero recibir en un canal privado los mensajes que dispararon una acción y la lista de canales afectados, para revisar si hubo un falso positivo.
- Como administrador, quiero revisar desde el panel los incidentes y los mensajes que activaron una acción y, si fue un falso positivo, desbanear al usuario, para corregir errores de moderación sin tener que entrar a Discord.
- Como administrador, quiero registrar un servidor con su token y probar la configuración antes de activarla, para no habilitar reglas que el bot no podrá ejecutar.

## §6 Flujos típicos

Flujo de moderación (en operación): llega un mensaje a un canal. El bot descarta primero a los usuarios exentos (staff y similares). Luego evalúa las reglas de contenido sobre el mensaje y actualiza la actividad reciente del usuario para las reglas de conducta. Si se cumple una política (por contenido o por ráfaga distribuida), toma una copia de los mensajes involucrados y ejecuta las acciones configuradas en orden —típicamente reportar al canal privado y banear con borrado retroactivo— y registra el incidente. Si la política está en modo simulación, registra lo que habría hecho sin ejecutarlo.

Flujo de configuración (administración): el administrador ingresa al panel, registra el servidor y su token, y ejecuta la prueba de configuración. Crea reglas (de contenido o de conducta), las agrupa, define un evento con sus acciones, apoyándose en las leyendas y ejemplos de cada parámetro. Deja el evento en modo simulación, revisa durante un tiempo los reportes y, cuando confía en la regla, la pasa a ejecución real.

Flujo de revisión de incidentes: el administrador abre el panel de incidentes, revisa la copia de los mensajes que dispararon una acción y los canales afectados, y decide si fue un falso positivo. Si lo fue, desbanea al usuario desde el panel. Los mensajes ya borrados no se restauran; el panel conserva solo la copia para la revisión.

## §7 Casos límite y "qué pasa si"

- ¿Qué pasa si el usuario que dispara la regla es administrador del servidor o tiene un rol por encima del bot, y por lo tanto el bot no puede banearlo?
  → Respuesta: el bot no puede aplicar la acción sobre un usuario con rol superior al suyo; lo registra, lo reporta al canal de incidencias y la prueba de configuración advierte de la jerarquía de roles y los permisos faltantes antes de activar el servidor.
- ¿Qué pasa si la ráfaga llega espaciada y no entra completa dentro de la ventana de detección?
  → Respuesta: el borrado retroactivo del baneo limpia los mensajes previos dentro de la ventana de borrado; la ventana de detección es configurable para capturar fan-outs más espaciados, manteniendo como discriminador la cantidad de canales distintos.
- ¿Qué pasa si AutoMod de Discord bloquea parte de los mensajes y el bot no llega a verlos, dejando el conteo de canales incompleto?
  → Respuesta: se recomienda que AutoMod no pre-bloquee el contenido que cubre el bot, para que este vea el fan-out completo; la coordinación con AutoMod queda fuera de v1.
- ¿Qué pasa si un usuario legítimo postea muchos mensajes en un solo canal por entusiasmo y no por spam?
  → Respuesta: la métrica de canales distintos evita ese falso positivo; postear muchos mensajes en un solo canal no dispara la regla de ráfaga distribuida.
- ¿Qué pasa si se pierde la conexión del gateway de Discord en medio de una ráfaga?
  → Respuesta: el cliente reconecta automáticamente; los mensajes no recibidos durante la caída no se evalúan, pero el borrado retroactivo al banear cubre lo que haya quedado dentro de la ventana de borrado.
- ¿Qué pasa si el token del servidor se revoca o queda inválido?
  → Respuesta: el servidor queda marcado como desconectado en el panel; la prueba de configuración detecta el token inválido y bloquea la activación.
- ¿Qué pasa si dos políticas distintas coinciden sobre el mismo mensaje?
  → Respuesta: se evalúan por prioridad con primera coincidencia; una bandera "continuar" por política permite, si se desea, que un mismo mensaje dispare más de un evento.

## §8 Métricas de éxito desde el negocio

| Criterio | Métrica | Target | Plazo |
|---|---|---|---|
| Corte automático de spam | Porcentaje de incidentes de ráfaga cortados automáticamente, sin intervención manual | ≥ 95% | Continuo, por incidente, desde la puesta en producción |
| Falsos positivos | Porcentaje de baneos revertidos por ser falso positivo | ≤ 2% | Mensual |
| Limpieza efectiva | Porcentaje de mensajes de la ráfaga eliminados dentro de los 10 s del incidente | ≥ 98% | Por incidente |

Nota: los tiempos de la regla (ventana de detección, por defecto del orden de 2 s) y del baneo (borrado retroactivo configurable) son parámetros de configuración, no métricas; viven en la configuración de la regla y de la acción, y la latencia de procesamiento se mide como NFR en §17 P.10.

## §9 Lo que NO es esta solución (exclusiones)

- No reemplaza al AutoMod nativo de Discord ni gestiona su convivencia. Justificación: el motor propio se enfoca en el patrón de ráfaga distribuida que AutoMod no corta de forma confiable; la coordinación entre ambos queda fuera de v1.
- No incluye el motor de IA que propone configuraciones por prompt. Justificación: alcance acotado; v1 solo reserva la frontera para enchufarlo más adelante.
- No opera a escala multi-servidor masiva en v1. Justificación: límite de memoria de la Raspberry Pi de 32 bits; v1 se prevé para un servidor.
- No restaura mensajes borrados. Justificación: una vez que el baneo los purga, no se pueden recuperar por la API de Discord; el sistema conserva una copia solo para revisión, y el baneo sí es reversible (desbaneo).
- No implementa moderación por reputación, historial de usuario ni apelaciones automatizadas. Justificación: queda fuera del objetivo de cortar el spam.

## §10 Restricciones del cliente

- Plataforma de despliegue impuesta: Raspberry Pi con Raspbian / Raspberry Pi OS de 32 bits (armv7l), auto-hospedado, sin contenedores (sin Docker). Instalación mediante un paquete con todo lo necesario (publicación self-contained) que queda corriendo como servicio del sistema.
- Integración obligatoria: Discord, a través de su API y gateway, con un token de acceso por servidor que el solicitante obtiene de cada servidor a moderar.
- Plataforma tecnológica pre-fijada por el cliente: ver cabecera y §17 (Parte C); el cliente predefinió el stack de construcción.
- Presupuesto orientativo: sin presupuesto formal. Proyecto propio; el costo se reduce al hardware ya disponible (la Raspberry) más el tiempo de desarrollo.
- Fecha objetivo: sin fecha objetivo. Desarrollo incremental por sprints, sin compromiso contractual que justifique un deadline fijo.

## §11 Riesgos detectados desde el negocio

| ID | Riesgo | Probabilidad | Impacto | Mitigación propuesta |
|---|---|---|---|---|
| R-01 | Falsos positivos: banear a un usuario legítimo | Media | Alto | Discriminar por canales distintos (no por cantidad de mensajes); exenciones de staff; modo simulación previo a la ejecución real; reporte a canal privado y panel de incidentes con desbaneo para revertir |
| R-02 | El atacante evade el patrón (randomiza nombres de archivo o espacia el envío) | Media | Medio | Discriminar por canales distintos, que es robusto al espaciado; borrado retroactivo que limpia lo previo; ventana de detección configurable |
| R-03 | El emisor del spam tiene un rol por encima del bot y no se lo puede banear | Media | Medio | La prueba de configuración advierte sobre jerarquía de roles y permisos faltantes antes de activar el servidor |
| R-04 | Caída del gateway de Discord o token revocado deja al servidor sin moderación | Baja | Alto | Reconexión automática del cliente; estado de conexión visible en el panel; prueba de token al registrar y re-validación |

## §12 Glosario del dominio del cliente

| Término | Definición | Sinónimos / Notas |
|---|---|---|
| Ráfaga distribuida (fan-out) | Envío casi simultáneo de mensajes a varios canales distintos, típico de un bot de spam o cuenta comprometida | "ráfaga", "fan-out" |
| Canal de salida | Canal de Discord designado con un nombre lógico (p. ej. "mod-log") al que el sistema envía reportes | — |
| Evento / política | Conjunto de grupos de reglas que, al cumplirse, dispara un conjunto de acciones | "política de moderación" |
| Grupo de reglas | Conjunto de reglas con un modo de coincidencia: todas, alguna, o al menos N | — |
| Regla de contenido | Predicado sin estado que evalúa un mensaje aislado (expresión regular o palabras clave) | — |
| Regla de conducta | Predicado con estado que evalúa la actividad reciente del usuario (frecuencia o canales distintos) | — |
| Exención | Rol, usuario o canal de confianza excluido de la moderación | "whitelist" |
| Modo simulación | Estado en que una política registra lo que haría sin ejecutar la acción | "log-only", "dry-run" |
| Borrado retroactivo | Borrado de los mensajes recientes del usuario, hacia atrás dentro de una ventana, al momento del baneo | "purga", "prune" |
| Desbaneo | Reversión de un baneo desde el panel; revierte el baneo pero no restaura los mensajes borrados | — |
| Incidente | Registro de un disparo de evento con su copia de mensajes, canales afectados y acción resultante | — |
| Token de bot | Credencial de la aplicación-bot de Discord que autoriza al servicio a operar en un servidor | — |
| Snowflake | Identificador de 64 bits de Discord para servidor, canal, usuario o mensaje | — |

---

# Parte B — Composición de la solución

## §13 Proyectos de la solución

La solución es un único servicio monolítico; por lo tanto §13 tiene una sola fila (caso degenerado válido). El servicio reúne el panel de administración, el bot de moderación y la persistencia en un solo proceso desplegable.

Perfil de convención de nombres de código:

| Parámetro | Valor | Notas |
|---|---|---|
| Forma del nombre de solución en código | PascalCase | `DiscordModeradorBot` |
| Separador de segmentos | `.` | Separa la raíz de la solución del sufijo de rol |
| Prefijo de paquetes redistribuibles | `Aplicada` | No se usa en v1: la solución no expone redistribuibles |

Tabla de proyectos (fuente del manifiesto derivado):

| `nombre-proyecto-kebab` | `project_type` (D8) | Rol en la solución | Dependencias | `redistribuible` |
|---|---|---|---|---|
| `discord-bots-admin` (principal) | `web-monolith` | Servicio monolítico: panel de administración Blazor Server + bot de moderación embebido + persistencia SQLite, en un solo proceso | — | false |

Nombre de código del proyecto: `DiscordModeradorBot.Servicio`. Grafo de dependencias: un único nodo, acíclico de forma trivial. No hay colisión de nombres. Proyecto principal único: `discord-bots-admin`.

## §14 Estilo arquitectónico de la solución

La solución es monolítica de un único proyecto, por decisión explícita del cliente (un solo proyecto que reúne front-end, back-end y bot). No hay contratos entre proyectos ni grafo de dependencias inter-proyecto que declarar.

La composición elegida se justifica frente a dos alternativas descartadas: una arquitectura de microservicios se descarta por desproporcionada para un único operador desplegando en una Raspberry Pi; separar el bot y el panel en dos procesos o servicios distintos se descarta por el requisito explícito de monolito de un solo proyecto y por la simplicidad de despliegue e instalación en la Pi. El punto de entrada para el administrador es el panel web; el bot corre dentro del mismo proceso como servicio en segundo plano.

## §15 Esquema de descomposición y delivery

Descomposición vertical (vertical slicing): cada sprint entrega una rebanada funcional completa que atraviesa panel, persistencia, bot y motor de evaluación, en lugar de construir por capas horizontales. El primer sprint es un walking skeleton end-to-end demostrable: registrar un servidor con su token, recibir un mensaje del gateway, evaluar la regla de ráfaga distribuida y, en modo simulación, reportar la acción que se ejecutaría.

Las rebanadas siguientes se agregan una por una, cada una completa y demostrable de punta a punta: la acción de baneo con borrado retroactivo; las reglas de contenido por expresión regular; la revisión de incidentes y el desbaneo desde el panel; las exenciones; las acciones de timeout, expulsión y rol; y la configuración dirigida por descriptores con ayuda contextual. Al ser un único proyecto, el orden topológico es trivial; el criterio rector y bloqueante es que cada rebanada entrega valor demostrable end-to-end y ninguna rompe el camino completo.

## §16 Estructura de repositorio de la solución

```text
discord-bots-admin/
├── src/
│   └── DiscordModeradorBot.Servicio/        # web-monolith (principal): panel + bot + persistencia
├── tests/
│   └── DiscordModeradorBot.Servicio.Tests/
├── samples/
├── docs/                                     # categorías 00-11 SDD
├── scripts/
│   └── servicio/                             # instalador y unidad systemd del servicio
└── devs/
    └── intake/                               # SOLUTION-INTAKE-discord-bots-admin_v1.0.md
```

### §16.1 Materialización de `/samples`

El proyecto principal es de tipo `web-monolith`; sus samples se materializan según la tabla de adaptabilidad del orquestador. Se construyen tres samples, todos sobre el proyecto `DiscordModeradorBot.Servicio` y ejercitando partes reales de `/src`: (a) conexión mínima al gateway que loguea los mensajes entrantes; (b) disparo de la detección de ráfaga con mensajes simulados, mostrando el baneo; (c) página mínima del panel que ejercita la capa de configuración por descriptores. Cada sample es autocontenido y reproducible en cinco pasos o menos.

---

# Parte C — Técnica por proyecto

## §17 Bloque técnico — `discord-bots-admin`

| Campo | Valor |
|---|---|
| `nombre-proyecto-kebab` | `discord-bots-admin` |
| `nombre-proyecto-codigo` | `DiscordModeradorBot.Servicio` |
| `project_type` (D8) | `web-monolith` |
| Rol | Servicio monolítico de administración y moderación (panel + bot + persistencia en un proceso) |
| `redistribuible` | false |

### §17.P.1 Stack tecnológico

Lenguaje C# sobre .NET 10. Runtime publicado en modo self-contained para `linux-arm` (ARM de 32 bits, armv7l), sin dependencia de un runtime instalado en el sistema. Front-end en Blazor Server interactivo con la librería de componentes MudBlazor. Integración con Discord mediante Discord.Net (gateway + acciones de moderación). Persistencia con SQLite a través de EF Core. El bot corre como servicio en segundo plano dentro del host web. Versión mínima de lenguaje y runtime: .NET 10. Dependencias core sin las que no compila: Discord.Net, MudBlazor, EF Core con su proveedor de SQLite.

### §17.P.2 Estilo arquitectónico del proyecto

Estilo interno por capas con un pipeline de evaluación como núcleo del dominio (motor de moderación) y el bot como servicio en segundo plano; el dominio de moderación se modela como un firewall multi-contexto (cada servidor de Discord es un contexto con su token, sus reglas y sus políticas). La configuración es dirigida por esquema: cada parámetro configurable se describe con un descriptor único que es fuente de verdad de su default, sus límites, su leyenda y sus ejemplos. El proyecto expone superficies de configuración por parámetro (umbrales, ventanas, modos, opciones) administradas desde el panel. Alternativas descartadas: microservicios (desproporcionado para el contexto de despliegue) y separar bot y panel en procesos distintos (descartado por el requisito de monolito único y la simplicidad de despliegue).

### §17.P.3 Comunicación e integración

El proyecto integra con Discord como sistema externo: recibe eventos por el gateway (WebSocket) y ejecuta acciones contra la API REST de Discord (baneo con borrado de mensajes, desbaneo, timeout, expulsión, gestión de roles, envío de mensajes a canales). La autenticación contra Discord es por token de bot, uno por servidor registrado. Al ser un proyecto único, no expone contratos hacia otros proyectos de la solución, por lo que el versionado de contratos inter-proyecto no aplica. La política de cambios sobre la integración con Discord sigue la versión de su API y de Discord.Net.

### §17.P.4 Persistencia

Motor: SQLite embebida, accedida con EF Core, en modo WAL para tolerar escrituras concurrentes del bot (auditoría) y del panel (configuración). Guarda: servidores con su token cifrado, canales de salida lógicos, exenciones, reglas (de contenido y de conducta), grupos de reglas y su relación con reglas, eventos, acciones, registros de auditoría (incidentes con su copia de mensajes) y el administrador. Los identificadores de Discord (snowflakes) se almacenan como texto para evitar el desborde del entero con signo de 64 bits. El esquema se versiona con migraciones de EF Core. No es multi-tenant: hay un único administrador; la operación de varios servidores se modela como multi-contexto dentro de la misma instancia. El estado de conducta (ventanas deslizantes de actividad por usuario) vive en memoria, no en SQLite.

### §17.P.5 Seguridad y autenticación

Autenticación del panel: usuario administrador único, con credenciales creadas en el primer ingreso (first-run setup) y contraseña almacenada con hash robusto (formato PHC, Argon2 o PBKDF2). Autorización: un único rol administrador; solo el administrador puede registrar servidores, configurar reglas, eventos, acciones y parámetros, y desbanear usuarios. Identity Provider: local, sin proveedor externo. Secretos: los tokens de bot se cifran en reposo con AES usando una clave maestra que vive en una variable de entorno del servicio (archivo de entorno con permisos restringidos), nunca en la base. Datos: residencia local en el dispositivo, sin terceros; minimización (se guardan solo los identificadores necesarios) y retención acotada. Marco aplicable: Ley 25.326 de Protección de Datos Personales (autoridad de aplicación AAIP), con proyectos de reforma en trámite parlamentario que conviene monitorear; sin normativa sectorial específica.

### §17.P.6 Estrategia de testing

Pirámide con predominio de pruebas unitarias del motor de evaluación y de los evaluadores de reglas, pruebas de integración del pipeline y la persistencia, y un mínimo de pruebas end-to-end del panel. Cobertura mínima (gate del CI): líneas ≥ 75% y branches ≥ 65% a nivel global, con el módulo de detección de spam ≥ 90% por ser el núcleo crítico. Frameworks por nivel: unitario con xUnit + FluentAssertions + NSubstitute; integración con xUnit y WebApplicationFactory (Testcontainers no es necesario, la persistencia es SQLite en archivo); end-to-end del panel con Playwright. Pruebas de contrato hacia otros proyectos: no aplica (proyecto único).

### §17.P.7 Estrategia de versionado y release

Se adopta SemVer 2.0.0 y Conventional Commits sin excepciones. Versión calculada automáticamente a partir de tags de Git con GitVersion o Nerdbank.GitVersioning (elección puntual abierta para Sprint 0). Estrategia de branching: GitHub Flow (trunk con ramas de feature de vida corta), apropiada para un solo desarrollador. Canales y artefacto de release: el artefacto es un paquete self-contained para Raspbian (zip con todo lo necesario) que se instala como servicio; no hay feed de paquetes porque el proyecto no es redistribuible.

### §17.P.8 Pipeline CI/CD

Plataforma: GitHub Actions. Stages con quality gates bloqueantes para mergear: build sin errores, tests en verde, umbral de cobertura cumplido, formato (`dotnet format`) y análisis estático sin warnings nuevos. La publicación `linux-arm` self-contained se genera por cross-compile desde el runner x64, sin necesidad de un runner ARM. Rollback de una publicación problemática: reinstalación de la publicación anterior conservando el archivo de entorno y la clave maestra (los tokens cifrados siguen siendo válidos).

### §17.P.9 Compatibilidad y plataformas target

Sistema operativo target: Raspbian / Raspberry Pi OS de 32 bits (armv7l), sobre chip ARMv7 o superior (Cortex-A7/A53 o equivalente). Runtime: .NET 10 publicado self-contained para `linux-arm`. No se soporta ARMv6 (Raspberry Pi Zero/1) ni arquitecturas no listadas. Panel web: últimas dos versiones de Chrome, Edge y Firefox, y Safari 16 o superior (navegadores evergreen). Toda combinación no listada se considera no soportada.

### §17.P.10 Requerimientos no funcionales (NFR)

- Latencia de procesamiento por mensaje: p95 < 200 ms.
- Throughput sostenido: ≥ 50 mensajes/s en Raspberry Pi 4 (a confirmar por benchmark en el hardware real).
- SLO de disponibilidad: 99% mensual (realista para auto-hospedado sin redundancia).
- Memoria: ≤ 8 MB por conexión de gateway activa.
- Observabilidad: registro de eventos del servicio al journal del sistema; auditoría de cada incidente (disparo, copia de mensajes, canales afectados y acción) en la base.

Capacidades con valor de plataforma ya acotadas: ventana de detección configurable (por defecto del orden de 2 s, a calibrar) y borrado retroactivo de mensajes limitado a 7 días por la API de Discord.

### §17.P.11 Decisiones técnicas pre-tomadas (pre-ADR)

Decisiones cerradas antes del Sprint 0:
- Monolito único en .NET 10 con panel Blazor Server, bot como servicio en segundo plano y SQLite, en un solo proceso. Alternativas evaluadas y descartadas: microservicios; bot y panel en procesos separados.
- Despliegue self-contained para `linux-arm` con instalador que registra el servicio en systemd. Alternativas descartadas: publicación dependiente del framework; contenedores (Docker).
- Dominio modelado como firewall multi-contexto; el token identifica una aplicación-bot por servidor, de modo que cada servidor registrado aporta su propio token y su propia conexión de gateway.
- Reglas con estado (conducta) versus sin estado (contenido); la métrica de canales distintos es el discriminador del patrón de ráfaga distribuida.
- Cifrado de los tokens en reposo con clave maestra provista por variable de entorno.
- Configuración dirigida por esquema (descriptores como fuente única de verdad), con modo simulación, antirrebote por usuario y prueba de configuración al registrar un servidor.
- Revisión de incidentes y desbaneo desde el panel: el baneo es reversible vía API; el borrado de mensajes no.
- Se reserva una frontera de propuesta de configuración validable, con previsualización, modo simulación y confirmación humana antes de aplicar, contra la cual se enchufará a futuro un asistente de IA que proponga configuraciones. La frontera no se diseña en v1: solo se reserva.

Decisiones deliberadamente abiertas para Sprint 0:
- Los valores por defecto exactos de detección (umbral de 2 vs 3 canales, ventana de 2 vs 4 s), a calibrar con datos reales.
- Si se persiste algún contador de conducta para sobrevivir reinicios (hoy decidido en memoria).
- El mecanismo exacto de recarga en caliente de la configuración (invalidación de caché).
- La elección puntual entre GitVersion y Nerdbank.GitVersioning, y entre Argon2 y PBKDF2.
- A futuro (no Sprint 0): una eventual capa de scoring o modelo de detección, fuera del alcance de v1.

### §17.P.12 Restricciones técnicas y trade-offs aceptados

- El estado de conducta se mantiene en memoria y se pierde ante un reinicio, a cambio de simplicidad y de no recargar SQLite en la Raspberry.
- v1 opera un solo servidor, a cambio de no exceder la memoria del proceso de 32 bits; cada token adicional implica una conexión de gateway concurrente.
- Publicación self-contained (paquete más grande) a cambio de no depender de un runtime instalado en la Pi.
- ARM de 32 bits es un tier deprioritizado de la plataforma (sin inversión de performance); trade-off aceptado por reutilizar la Raspberry existente.
- El borrado retroactivo de mensajes está limitado a 7 días por la plataforma; aceptado.
- El borrado de mensajes no es reversible (limitación de la API de Discord); el baneo sí lo es. Se acepta conservar solo una copia de los mensajes para revisión, sin capacidad de restaurarlos.
- Un token filtrado otorga control total del bot; riesgo aceptado y mitigado mediante cifrado en reposo con clave maestra fuera de la base.

---

## §18 Estrategia de demo / samples

Se construyen tres samples, todos sobre el proyecto único `DiscordModeradorBot.Servicio`: (a) conexión mínima al gateway que loguea los mensajes entrantes, que ejercita la capa de integración con Discord; (b) disparo de la detección de ráfaga con mensajes simulados, mostrando el baneo, que ejercita el motor de evaluación y la acción de baneo; (c) página mínima del panel que ejercita la capa de configuración por descriptores. Cada sample es autocontenido, reproducible en cinco pasos o menos, y referencia las partes reales de `/src` que ejercita. El sample (b) demuestra el punto central de la solución: la detección de ráfaga distribuida.

---

## §19 Checklist de completitud del intake

Negocio (Parte A):
- [x] La cabecera tiene nombre de solución, cliente, fecha y estado.
- [x] §1 describe un problema concreto y qué pasa si no se construye.
- [x] §2 tiene al menos un stakeholder por categoría con rol explícito.
- [x] §4 tiene al menos un ítem en cada categoría MoSCoW y el Must Have es el mínimo razonable.
- [x] §5 tiene al menos 3 historias en formato `Como/quiero/para`, cubriendo 2 roles si hay más de uno.
- [x] §7 lista al menos 5 casos límite con espacio para respuesta del cliente.
- [x] §8 tiene al menos 3 métricas SMART de negocio con target y plazo numéricos.
- [x] §9 lista al menos 3 exclusiones con justificación.
- [x] §10 declara presupuesto orientativo y fecha objetivo (o "sin fecha" justificado).
- [x] §11 lista al menos 3 riesgos con probabilidad, impacto y mitigación.
- [x] §12 define al menos 5 términos del dominio.

Composición (Parte B):
- [x] §13 enumera todos los proyectos, cada uno con uno de los 8 valores D8, señala el principal, y el grafo de dependencias es acíclico.
- [x] §13 declara el perfil de convención de nombres; no hay colisión de nombres de proyecto.
- [x] §14 describe la composición y los contratos entre proyectos.
- [x] §15 garantiza valor demostrable end-to-end en el primer sprint a través de la jerarquía.
- [x] §16 publica el árbol `tree` derivado de la jerarquía y de la convención de nombres, con §16.1.

Técnica por proyecto (Parte C):
- [x] §17 está completo para cada proyecto de §13 (identidad + P.1 a P.12).
- [x] Cada proyecto: P.6 declara cobertura mínima numérica; P.7 adopta SemVer y Conventional Commits; P.8 enumera quality gates bloqueantes; P.9 declara plataformas y versiones mínimas; P.10 expresa NFR con métricas numéricas.

General:
- [x] No hay vocabulario del dominio fuente del bootstrap ni stacks hardcodeados en el texto normativo (D7).
- [x] El control de cambios refleja la versión y fecha del documento.

---

## Trazabilidad downstream

| Sección del intake | Destino | Documento downstream típico |
|---|---|---|
| §1 a §12 (negocio) | `00_contexto/`, `01_necesidades_negocio/` | visión, alcance, NB-XX |
| §13 (proyectos) | `SOLUTION-MANIFEST` derivado; todas las categorías por proyecto | manifiesto canónico; selector de variantes D8 |
| §14 estilo de solución | `05_arquitectura_tecnica/` (vista de solución) | `arquitectura-solucion_v1.0.md` |
| §16 estructura | `05_arquitectura_tecnica/`, `10_developer_guide/` | árbol, README de carpeta |
| §17 P.x (técnica por proyecto) | `05`, `08`, `09`, `00` (según P) por proyecto | ADRs, estrategia testing, pipeline, NFR |
| §18 samples | `11_examples/` | `ejemplo-XX_v1.0.md` |

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-20 | Intake unificado inicial de la solución | Fernando |
