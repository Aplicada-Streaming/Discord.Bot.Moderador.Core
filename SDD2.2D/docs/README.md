# Administrador de Bots Moderador para Discord

| Campo | Valor |
| --- | --- |
| **Solución** | Administrador de Bots Moderador para Discord |
| **Versión del documento** | 1.0 |
| **Estado** | Propuesto |
| **Fecha** | 2026-06-20 |
| **Stack principal** | .NET 10 / C#, Blazor Server con MudBlazor, Discord.Net, SQLite con EF Core |
| **Composición** | 1 proyecto (caso degenerado; ver tabla de proyectos) |
| **Proyecto principal** | discord-bots-admin |
| **Documento** | README raíz de la solución |

Este README es el punto de entrada y el mapa navegable de toda la documentación SDD de la solución. Cada categoría numerada bajo `docs/` profundiza un aspecto; aquí se integra la vista global, se presenta el proyecto y se ordena la lectura por audiencia.

## 1. Identidad de la solución

Los servidores de Discord del solicitante sufren ataques de spam. El patrón más dañino aparece cuando se vulneran las credenciales de un miembro y un atacante usa esa cuenta para enviar ráfagas de spam —habitualmente imágenes adjuntas— a todos los canales, uno tras otro, en pocos segundos. La moderación manual no llega a tiempo: para cuando un humano reacciona, el spam ya inundó diez o veinte canales y hay que limpiarlos a mano. El filtro nativo de Discord, configurado con expresiones regulares, se comporta de forma errática frente a este patrón y no corta la ráfaga distribuida entre canales.

La propuesta de valor central es detectar el patrón que de verdad delata al spam automatizado —el envío casi simultáneo a múltiples canales distintos, algo físicamente imposible para un humano— y cortarlo al instante baneando al emisor, con borrado retroactivo de sus mensajes en todos los canales en una sola operación y un reporte a un canal privado para revisar falsos positivos. El motor de reglas es configurable al estilo firewall multi-contexto, auto-hospedado en una Raspberry Pi propia, sin dependencia de servicios de terceros, y dirigido por esquema con ayudas y ejemplos en pantalla.

La audiencia objetivo es el administrador propietario de los servidores de Discord, que aprueba el sistema y lo opera; el administrador del sistema (rol único de la aplicación), que registra servidores, configura reglas y revisa incidentes; y los miembros de las comunidades moderadas, beneficiarios indirectos de un servidor libre de spam. El lead técnico e implementador es Fernando.

El sistema se vive a través de tres flujos. En operación, llega un mensaje a un canal, el bot descarta primero a los usuarios exentos, evalúa las reglas de contenido sobre el mensaje y actualiza la actividad reciente del usuario para las reglas de conducta; si se cumple una política, toma una copia de los mensajes involucrados, ejecuta las acciones configuradas en orden y registra el incidente. En configuración, el administrador ingresa al panel, registra el servidor y su token, ejecuta la prueba de configuración, crea reglas, las agrupa, define eventos con sus acciones y las deja en modo simulación hasta confiar en ellas. En revisión, el administrador abre el panel de incidentes, examina la copia de los mensajes que dispararon una acción y, si fue un falso positivo, desbanea al usuario. El detalle de estos flujos vive en la categoría 02 (casos de uso) y en la categoría 03 (experiencia de uso).

## 2. Proyectos de la solución

La solución es el caso degenerado de un único proyecto. La tabla refleja el `SOLUTION-MANIFEST` sin divergencias.

| Proyecto | Tipo D8 | Rol | Dependencias | Redistribuible |
| --- | --- | --- | --- | --- |
| discord-bots-admin (principal) | web-monolith | Servicio monolítico: panel de administración Blazor Server, bot de moderación embebido y persistencia SQLite, en un solo proceso | — | false |

Nombre de código del proyecto: `DiscordModeradorBot.Servicio`. Al ser un único nodo, el grafo de dependencias es acíclico de forma trivial y no hay contratos entre proyectos que declarar. Por ser el caso degenerado, el layout de `docs/` está aplanado: las categorías 00 a 11 viven directo bajo `docs/<categoria>/`, sin subnivel `proyectos/<kebab>/` ni carpeta `_solucion/`.

La decisión de un solo proyecto es explícita del cliente y está justificada en la categoría 05: una arquitectura de microservicios se descartó por desproporcionada para un único operador desplegando en una Raspberry Pi, y separar el bot y el panel en procesos distintos se descartó por el requisito de monolito único y la simplicidad de instalación. El punto de entrada para el administrador es el panel web; el bot corre dentro del mismo proceso como servicio en segundo plano. El proyecto no es redistribuible: el artefacto de release es un paquete self-contained para Raspbian que se instala como servicio del sistema, sin feed de paquetes ni dependencia de un runtime instalado en el host.

## 3. Stack y composición

El stack está pre-fijado por el cliente. Las versiones provienen de la cabecera y de SOLUTION-INTAKE §17 P.1; las plataformas, de §17 P.9.

| Componente | Tecnología y versión | Rol en la solución |
| --- | --- | --- |
| Lenguaje y runtime | C# sobre .NET 10 (self-contained para `linux-arm`) | Base de ejecución sin runtime instalado en el host |
| Front-end del panel | Blazor Server interactivo con MudBlazor | Panel de administración web |
| Integración con Discord | Discord.Net (gateway WebSocket + API REST) | Recepción de eventos y acciones de moderación |
| Persistencia | SQLite con EF Core (modo WAL) | Configuración, auditoría e incidentes |
| Hospedaje del bot | Servicio en segundo plano dentro del host web | Motor de moderación en el mismo proceso |

El dominio se modela como un firewall multi-contexto: cada servidor de Discord registrado es un contexto con su propio token, sus reglas y sus políticas, y aporta su propia conexión de gateway. La configuración es dirigida por esquema, donde cada parámetro configurable se describe con un descriptor único que es fuente de verdad de su valor por defecto, sus límites, su leyenda y sus ejemplos en pantalla. El estado de conducta (ventanas deslizantes de actividad por usuario) vive en memoria, mientras que la configuración, la auditoría y los incidentes se persisten en SQLite. Estos conceptos se desarrollan en la categoría 05.

Plataformas soportadas (§17 P.9):

| Plataforma | Versión mínima | Observaciones |
| --- | --- | --- |
| Sistema operativo de despliegue | Raspbian / Raspberry Pi OS 32 bits (armv7l) | Chip ARMv7 o superior; no se soporta ARMv6 |
| Runtime publicado | .NET 10 self-contained `linux-arm` | Sin dependencia de runtime del sistema |
| Navegadores del panel | Chrome, Edge, Firefox (últimas dos versiones); Safari 16+ | Navegadores evergreen; otras combinaciones no soportadas |

Cadena de trazabilidad D6, como referencia conceptual de cómo se encadenan las categorías:

```text
Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline
```

Cada eslabón vive en su categoría: la Visión y el roadmap en 00, las necesidades de negocio (NB) en 01, los casos de uso (CU) y las reglas de negocio (RN) en 02, las decisiones de arquitectura (ADR) en 05, el backlog técnico (BT) en 06, el plan de sprint en 07, las pruebas en 08 y el pipeline en 09. Cada artefacto declara su trazabilidad upstream y downstream en su propia cabecera, de modo que la cadena es navegable en ambos sentidos sin pasar por este README.

El alcance de la primera versión está priorizado con MoSCoW en el intake y reflejado en el roadmap: la entrega es por rebanadas verticales, cada una completa y demostrable de punta a punta. El primer sprint es un walking skeleton que registra un servidor, recibe un mensaje del gateway, evalúa la regla de ráfaga distribuida y, en modo simulación, reporta la acción que se ejecutaría. Las rebanadas siguientes agregan el baneo con borrado retroactivo, las reglas de contenido por expresión regular, la revisión de incidentes y el desbaneo, las exenciones, las acciones adicionales y la configuración dirigida por descriptores. Quedan fuera de v1 el motor de IA que propone configuraciones por prompt (solo se reserva la frontera), la operación multi-servidor a escala y la restauración de mensajes borrados.

## 4. Mapa de la documentación

Una fila por categoría real bajo `docs/`. Los enlaces apuntan a carpetas existentes; las categorías omitidas se declaran con su motivo.

| Sección | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto | Visión, alcance y roadmap del producto | AG-00 | [00_contexto](00_contexto/) |
| 01_necesidades_negocio | Necesidades de negocio (NB) derivadas del intake | AG-01 | [01_necesidades_negocio](01_necesidades_negocio/) |
| 02_especificacion_funcional | Casos de uso, reglas de negocio y modelo de datos conceptual | AG-02 | [02_especificacion_funcional](02_especificacion_funcional/) |
| 03_ux_ui_dx | Experiencia de uso, wireframes y glosario UX | AG-03 | [03_ux_ui_dx](03_ux_ui_dx/) |
| 04_prompts_ai | Omitida: `usa_llm=false` en v1; la frontera de IA solo se reserva (ver ADR-10) | — | [ADR-10](05_arquitectura_tecnica/adrs/ADR-10-omision-contratos-prompts-ai_v1.0.md) |
| 05_arquitectura_tecnica | Vista de arquitectura, ADRs, modelo lógico y extensibilidad | AG-05 | [05_arquitectura_tecnica](05_arquitectura_tecnica/) |
| 06_backlog-tecnico | Backlog técnico, product backlog y definición de listo | AG-06 | [06_backlog-tecnico](06_backlog-tecnico/) |
| 07_plan-sprint | Mini-plan de sprints y secuencia de rebanadas | AG-07 | [07_plan-sprint](07_plan-sprint/) |
| 08_calidad_y_pruebas | Estrategia de calidad y testing, matriz de cobertura y DoD | AG-08 | [08_calidad_y_pruebas](08_calidad_y_pruebas/) |
| 09_devops | Pipeline CI/CD, entornos, versionado y publicación self-contained | AG-09 | [09_devops](09_devops/) |
| 10_developer_guide | Omitida: colapsada en los READMEs de sección (ver ADR-11) | — | [ADR-11](05_arquitectura_tecnica/adrs/ADR-11-colapso-developer-guide-en-readmes_v1.0.md) |
| 11_examples | Ejemplos progresivos sobre `DiscordModeradorBot.Servicio` | AG-11 | [11_examples](11_examples/) |
| _audit | Auditorías de coherencia transversal por bloque documental | Orquestador SDD | [_audit](_audit/) |

No existen filas `_solucion/` ni `proyectos/<kebab>/`: ambas son inaplicables en el caso degenerado de un único proyecto. Cada categoría tiene su propio README de sección, que actúa como índice fino de sus documentos; este mapa enlaza la carpeta y delega ese detalle al README correspondiente para no duplicar contenido. La carpeta `_audit/` reúne las auditorías de coherencia transversal por bloque documental y es útil para verificar que la documentación se mantiene consistente entre categorías tras cada regeneración.

Qué se encuentra en cada categoría, a modo de orientación rápida:

- 00_contexto: visión del producto, alcance (qué entra y qué no) y roadmap por fases.
- 01_necesidades_negocio: las siete necesidades de negocio (NB-01 a NB-07) derivadas del problema.
- 02_especificacion_funcional: dieciséis casos de uso, las reglas de negocio (RN) y el modelo conceptual de datos con sus reglas.
- 03_ux_ui_dx: experiencia de uso, wireframes de cada pantalla del panel y el glosario UX completo.
- 05_arquitectura_tecnica: vista de arquitectura, las decisiones (ADR-01 a ADR-13), el modelo lógico, el flujo de ejecución y la extensibilidad.
- 06_backlog-tecnico: backlog técnico, product backlog y la definición de listo.
- 07_plan-sprint: el mini-plan que ordena las rebanadas verticales en sprints.
- 08_calidad_y_pruebas: estrategia de calidad y testing, matriz de cobertura, casos de prueba referenciales y la definición de hecho.
- 09_devops: pipeline CI/CD, entornos de despliegue, versionado, seguridad de la cadena de suministro y la guía de publicación self-contained para `linux-arm`.
- 11_examples: tres ejemplos progresivos (conexión al gateway, configuración por descriptores y detección de ráfaga) sobre partes reales de `/src`.

## 5. Flujo de lectura recomendado por audiencia

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Administrador / Product Owner | 00 → 01 → 06 → 07 | Entiende la visión y el alcance, luego las necesidades de negocio y el orden de construcción por sprints, sin entrar al detalle técnico. |
| Desarrollador | 00 → 02 → 03 → 05 → 11 | Parte del contexto, sigue por la especificación funcional y la experiencia de uso, luego la arquitectura y finalmente los ejemplos ejecutables. |
| QA | 00 → 02 → 08 | Necesita el contexto y la especificación funcional para derivar pruebas, y la estrategia de calidad para la matriz de cobertura y los gates. |
| DevOps | 00 → 05 → 09 | Toma el contexto, la arquitectura de despliegue y luego el pipeline, los entornos y la publicación self-contained para ARM. |

El administrador y el product owner no necesitan el detalle técnico: les basta entender por qué existe el sistema, qué resuelve y en qué orden se construye. El desarrollador necesita el camino completo desde el problema hasta el código de ejemplo, pasando por qué debe hacer el sistema, cómo se presenta y cómo está estructurado por dentro. El QA deriva sus casos de los requisitos y reglas de negocio, por lo que entra a 02 antes de leer la estrategia de pruebas y los gates de cobertura. El DevOps se concentra en el despliegue: la arquitectura le dice qué se publica y dónde, y la categoría 09 le da el pipeline, los entornos y el procedimiento de publicación y rollback en la Raspberry Pi.

## 6. Cómo contribuir y cómo regenerar la documentación

Esta solución es `web-monolith`, por lo que no incluye `CONTRIBUTING.md`, `CHANGELOG.md` ni `LICENSE.md` (regla §2.1 de las reglas raíz): la comunicación es con un único equipo, no con consumidores externos. El equipo es de un solo desarrollador, por lo que las convenciones de versionado, branching y commits viven en el bloque técnico del intake y en la categoría 09 DevOps.

El versionado adopta SemVer 2.0.0 y Conventional Commits, con estrategia de branching GitHub Flow apropiada para un único desarrollador. El detalle operativo de estas convenciones se documenta en la categoría 09.

La documentación no se edita a mano de forma aislada: se regenera con el orquestador SDD 2.2, que coordina a los subagentes especializados. Cada subagente es propietario de su categoría:

- AG-ROOT: este README raíz de la solución.
- AG-00: contexto, visión, alcance y roadmap.
- AG-01: necesidades de negocio.
- AG-02: especificación funcional (casos de uso, reglas de negocio, modelo conceptual).
- AG-03: experiencia de uso, wireframes y glosario UX.
- AG-05: arquitectura técnica y decisiones (ADR).
- AG-06: backlog técnico.
- AG-07: plan de sprint.
- AG-08: calidad y pruebas.
- AG-09: DevOps y pipeline.
- AG-11: ejemplos progresivos.

El flujo de regeneración es: el orquestador lee el `SOLUTION-INTAKE` y deriva el `SOLUTION-MANIFEST` canónico; luego invoca a cada subagente con sus reglas de categoría para producir o actualizar sus documentos. Toda regeneración respeta el flujo de no-modificación de los upstream (intake y manifiesto), el árbol de categorías aplanado del caso degenerado y los criterios de aceptación de cada archivo de reglas. Antes de tocar un documento, se identifica el subagente propietario de su categoría y se regenera con él para mantener la coherencia transversal, los enlaces y la trazabilidad bidireccional. Las categorías 04 (prompts de IA) y 10 (developer guide) no se generan: están omitidas con motivo declarado en ADR-10 y ADR-11.

## 7. Estado actual y roadmap

Estado por categoría. El detalle de fases vive en el roadmap, que se enlaza y no se copia: [roadmap-producto_v1.0.md](00_contexto/roadmap-producto_v1.0.md).

| Categoría | Estado | Versión vigente |
| --- | --- | --- |
| 00_contexto | Propuesto | 1.0 |
| 01_necesidades_negocio | Propuesto | 1.0 |
| 02_especificacion_funcional | Propuesto | 1.0 |
| 03_ux_ui_dx | Propuesto | 1.0 |
| 05_arquitectura_tecnica | Propuesto | 1.0 |
| 06_backlog-tecnico | Propuesto | 1.0 |
| 07_plan-sprint | Propuesto | 1.0 |
| 08_calidad_y_pruebas | Propuesto | 1.0 |
| 09_devops | Propuesto | 1.0 |
| 11_examples | Propuesto | 1.0 |

Las categorías 04_prompts_ai y 10_developer_guide están omitidas; su motivo se declara en el mapa de la sección 4 (ADR-10 y ADR-11).

Restricciones de despliegue y NFR de referencia, a modo de orientación. El detalle y la verificación de estos objetivos viven en las categorías 05 (arquitectura) y 09 (DevOps); aquí se listan solo para enmarcar la lectura.

| Aspecto | Valor de referencia | Origen |
| --- | --- | --- |
| Plataforma de despliegue | Raspberry Pi con Raspbian 32 bits, auto-hospedado, sin contenedores | Restricción del cliente (§10) |
| Latencia de procesamiento por mensaje | p95 < 200 ms | NFR §17 P.10 |
| Throughput sostenido | ≥ 50 mensajes/s en Raspberry Pi 4 (a confirmar por benchmark) | NFR §17 P.10 |
| Disponibilidad | SLO de 99% mensual | NFR §17 P.10 |
| Cobertura de pruebas (gate de CI) | Líneas ≥ 75%, ramas ≥ 65%; módulo de detección ≥ 90% | Estrategia de testing §17 P.6 |
| Operación en v1 | Un servidor por instancia | Trade-off de memoria del proceso de 32 bits (§17 P.12) |

## 8. Glosario rápido

Términos esenciales del dominio. El glosario completo de la experiencia vive en [glosario-ux_v1.0.md](03_ux_ui_dx/glosario-ux_v1.0.md).

| Término | Definición breve |
| --- | --- |
| Ráfaga distribuida (fan-out) | Envío casi simultáneo de mensajes a varios canales distintos, típico de un bot de spam o cuenta comprometida. |
| Canal de salida | Canal de Discord designado con un nombre lógico al que el sistema envía reportes. |
| Evento / política | Conjunto de grupos de reglas que, al cumplirse, dispara un conjunto de acciones. |
| Grupo de reglas | Conjunto de reglas con un modo de coincidencia: todas, alguna, o al menos N. |
| Regla de contenido | Predicado sin estado que evalúa un mensaje aislado (expresión regular o palabras clave). |
| Regla de conducta | Predicado con estado que evalúa la actividad reciente del usuario (frecuencia o canales distintos). |
| Exención | Rol, usuario o canal de confianza excluido de la moderación. |
| Modo simulación | Estado en que una política registra lo que haría sin ejecutar la acción. |
| Borrado retroactivo | Borrado de los mensajes recientes del usuario, hacia atrás dentro de una ventana, al momento del baneo. |
| Desbaneo | Reversión de un baneo desde el panel; revierte el baneo pero no restaura los mensajes borrados. |
| Incidente | Registro de un disparo de evento con su copia de mensajes, canales afectados y acción resultante. |
| Token de bot | Credencial de la aplicación-bot de Discord que autoriza al servicio a operar en un servidor. |
| Snowflake | Identificador de 64 bits de Discord para servidor, canal, usuario o mensaje. |

## 9. Contacto y responsables

El proyecto es propio y de un solo desarrollador, por lo que la matriz de contacto es deliberadamente compacta. La propiedad del problema y la operación recaen en el administrador; la construcción y el mantenimiento, en el lead técnico, con asistencia de IA en el desarrollo.

| Rol | Responsable | Canal de comunicación |
| --- | --- | --- |
| Dueño del problema / Propietario | Administrador propietario de los servidores de Discord | Acuerdo directo con el lead técnico |
| Lead técnico e implementador | Fernando | Repositorio Discord.Bot.Moderador.Core (issues y commits) |
| Usuario administrador / operador | Administrador del sistema (rol único de la aplicación) | Panel de administración del servicio |

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | README raíz inicial de la solución generado por AG-ROOT. Cabecera, identidad, tabla de proyectos (caso degenerado), stack con versiones y plataformas, mapa de documentación aplanado (00, 01, 02, 03, 05, 06, 07, 08, 09, 11; 04 y 10 omitidas con motivo), flujo de lectura por audiencia, glosario rápido y control de cambios. |
