# Product Backlog — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-20
**Autor:** Scrum Master (AG-06)
**Estimación:** Fibonacci (1, 2, 3, 5, 8, 13)

Índice maestro priorizado de historias de usuario del servicio monolítico de administración y moderación. Cada historia traza a uno o más CU de la categoría 02 y se agrupa en una épica de capacidad funcional. La vista técnica complementaria, con las tareas técnicas y su fuente upstream en la categoría 05, vive en `backlog-tecnico_v1.0.md`; la condición de entrada al sprint vive en `definition-of-ready_v1.0.md`.

## 1. Objetivos del producto

Cortar de forma automática las ráfagas de spam distribuido entre canales —el patrón que la moderación manual y el filtro nativo de la plataforma no contienen a tiempo— baneando al emisor, limpiando sus mensajes hacia atrás y dejando registro para revisar falsos positivos. El MVP buscado es el conjunto de historias Must: detectar la ráfaga por canales distintos y banear automáticamente, banear con borrado retroactivo, contener contenido por patrón, reportar el incidente a un canal privado, y la superficie mínima de administración (cuenta administradora, registro de servidor con su credencial, y configuración de reglas, grupos, eventos y acciones).

## 2. Épicas

| EP | Nombre | Descripción | Sprints estimados |
| --- | --- | --- | --- |
| EP-01 | Detección de ráfaga y baneo automático | Reconoce el patrón de ráfaga distribuida por canales distintos en ventana corta y banea automáticamente al emisor. Núcleo de la solución. | 2 |
| EP-02 | Baneo con borrado retroactivo | Ejecuta el baneo limpiando los mensajes recientes del emisor hacia atrás dentro de una ventana configurable (tope de plataforma 7 días). | 1 |
| EP-03 | Contención de contenido por patrón | Detecta contenido no deseado en un mensaje aislado por expresión regular o palabras clave y contiene al emisor. | 1 |
| EP-04 | Incidentes, reporte y desbaneo | Reporta a un canal privado los mensajes accionados y los canales afectados, permite revisar incidentes desde el panel y revertir un baneo. | 2 |
| EP-05 | Autenticación y registro de servidor | Alta de credenciales del administrador en el primer ingreso, autenticación y registro de un servidor con su credencial de acceso cifrada. | 2 |
| EP-06 | Configuración dirigida por descriptores | Administra reglas, grupos, eventos, acciones y parámetros con ayuda contextual derivada de descriptores como fuente única de verdad. | 2 |
| EP-07 | Operación confiable y validación previa | Prueba la configuración de un servidor antes de activarlo y reconecta automáticamente mostrando el estado de conexión de cada contexto. | 1 |
| EP-08 | Exenciones y prudencia de la moderación | Modo simulación por evento, exenciones por rol, usuario o canal de confianza, y antirrebote por usuario para no repetir acciones durante una ráfaga. | 2 |

## 3. Historias por épica

Modo inline: el proyecto tiene 19 US (por debajo del umbral de 20 de §3.3 de las reglas), por lo que las historias viven inline en este documento con sus criterios, trazabilidad y DoR check. Cada US declara su CU relacionado de la categoría 02.

### 3.1 Tabla resumen de historias

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Detectar ráfaga distribuida por canales distintos | Must | 8 | Ready | CU-01 | EP-01 |
| US-02 | Configurar umbral de canales y ventana de detección | Must | 3 | Ready | CU-01 | EP-01 |
| US-03 | Banear automáticamente al emisor de la ráfaga | Must | 5 | Ready | CU-02 | EP-01 |
| US-04 | Banear con borrado retroactivo de los mensajes | Must | 5 | Ready | CU-03 | EP-02 |
| US-05 | Contener contenido no deseado por patrón | Must | 5 | Ready | CU-04 | EP-03 |
| US-06 | Reportar el incidente a un canal privado | Must | 3 | Ready | CU-05 | EP-04 |
| US-07 | Revisar incidentes y mensajes accionados | Should | 5 | Ready | CU-06 | EP-04 |
| US-08 | Revertir un baneo (desbaneo) desde el panel | Should | 3 | Ready | CU-07 | EP-04 |
| US-09 | Alta de credenciales del administrador en el primer ingreso | Must | 3 | Ready | CU-08 | EP-05 |
| US-10 | Autenticar al administrador | Must | 3 | Ready | CU-09 | EP-05 |
| US-11 | Registrar un servidor con su credencial de acceso | Must | 5 | Ready | CU-10 | EP-05 |
| US-12 | Administrar reglas, grupos, eventos y acciones | Must | 8 | Ready | CU-11 | EP-06 |
| US-13 | Mostrar ayuda contextual por parámetro | Should | 3 | Ready | CU-11 | EP-06 |
| US-14 | Probar la configuración de un servidor antes de activarlo | Should | 5 | Ready | CU-12 | EP-07 |
| US-15 | Reconectar y mostrar el estado de conexión | Should | 5 | Ready | CU-13 | EP-07 |
| US-16 | Ejecutar una política en modo simulación | Should | 3 | Ready | CU-14 | EP-08 |
| US-17 | Definir exenciones por rol, usuario o canal | Should | 3 | Ready | CU-15 | EP-08 |
| US-18 | Evitar acciones repetidas durante una ráfaga (antirrebote) | Should | 3 | Ready | CU-16 | EP-08 |
| US-19 | Designar varios canales de salida con propósito lógico | Could | 3 | Borrador | CU-05, CU-11 | EP-04 |

### 3.2 Historias inline

#### US-01 — Detectar ráfaga distribuida por canales distintos
**Épica:** EP-01 · **MoSCoW:** Must · **SP:** 8 (Fibonacci) · **CU:** CU-01 · **NB:** NB-01

Como administrador, quiero que el sistema reconozca cuando un mismo usuario publica en una cantidad de canales distintos por encima de un umbral dentro de una ventana corta, para disponer del disparador que distingue el spam automatizado del uso legítimo intenso.

Criterios de aceptación:
- Given una regla de ráfaga con umbral de 3 canales distintos y ventana de 2 s y un usuario no exento, When el usuario publica en tres canales distintos en 1,5 s, Then el sistema marca la condición de ráfaga cumplida para ese usuario en ese servidor.
- Given la misma regla, When el usuario publica diez mensajes en un solo canal en 1,5 s, Then el sistema no marca la condición porque la cuenta de canales distintos es 1 (edge case del falso positivo legítimo).
- Given una regla con la ventana ampliada a 6 s, When el usuario publica en tres canales distintos a lo largo de 5 s, Then el sistema marca la condición (edge case de ráfaga espaciada).

DoR check: [x] valor explícito · [x] CU-01 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] sin dependencia bloqueante · [x] descriptor de umbral/ventana identificado · [x] mensajes simulados disponibles.

Notas: el discriminador es la cantidad de canales distintos, no la cantidad de mensajes. Los valores por defecto exactos quedan abiertos a calibración; la US es agnóstica al valor concreto y opera sobre el descriptor (US-02).

#### US-02 — Configurar umbral de canales y ventana de detección
**Épica:** EP-01 · **MoSCoW:** Must · **SP:** 3 (Fibonacci) · **CU:** CU-01 · **NB:** NB-01

Como administrador, quiero configurar el umbral de canales distintos y la ventana de tiempo con un valor por defecto y ayuda en pantalla, para ajustar la sensibilidad de la detección sin conocimiento técnico profundo.

Criterios de aceptación:
- Given el descriptor del umbral con default 3 y límites de 2 a 10, When el administrador guarda el valor 4, Then el sistema persiste 4 y la detección lo usa en la próxima evaluación.
- Given el mismo descriptor, When el administrador intenta guardar el valor 1, Then el sistema rechaza el valor por estar fuera de límite y ofrece el valor por defecto (edge case).

DoR check: [x] valor explícito · [x] CU-01 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende del descriptor (US-13) no bloqueante · [x] descriptor identificado · [x] datos de prueba disponibles.

Notas: la persistencia del criterio de la regla y su validación se apoyan en la configuración dirigida por descriptores (EP-06).

#### US-03 — Banear automáticamente al emisor de la ráfaga
**Épica:** EP-01 · **MoSCoW:** Must · **SP:** 5 (Fibonacci) · **CU:** CU-02 · **NB:** NB-01

Como administrador, quiero que el sistema banee automáticamente al emisor cuando se cumple la condición de ráfaga, para cortar el spam antes de que inunde el servidor.

Criterios de aceptación:
- Given una política en modo real con la condición de ráfaga marcada y un bot con jerarquía suficiente, When se evalúa la política, Then el sistema ejecuta el baneo del emisor y registra el incidente con resultado ejecutada.
- Given un emisor con un rol por encima del bot, When se intenta el baneo, Then el sistema no aborta el pipeline, registra el incidente como no accionable y lo deja disponible para reporte (edge case de jerarquía).

DoR check: [x] valor explícito · [x] CU-02 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-01 (planificada) · [x] no toca configuración nueva · [x] escenarios de prueba disponibles.

Notas: el orden de las acciones y la supresión por antirrebote se cubren en US-16 y US-18; esta US asume política en modo real.

#### US-04 — Banear con borrado retroactivo de los mensajes
**Épica:** EP-02 · **MoSCoW:** Must · **SP:** 5 (Fibonacci) · **CU:** CU-03 · **NB:** NB-02

Como administrador, quiero que el baneo borre los mensajes recientes del emisor en todos los canales hacia atrás dentro de una ventana configurable, para limpiar el incidente en una sola operación.

Criterios de aceptación:
- Given una acción de baneo con borrado configurada a 1 día, When se banea al emisor, Then el sistema elimina los mensajes del emisor dentro de la ventana de borrado y registra los canales afectados.
- Given una acción configurada con una ventana mayor a 7 días, When el administrador la guarda, Then el sistema rechaza el valor por exceder el tope de plataforma de 7 días (edge case del tope).

DoR check: [x] valor explícito · [x] CU-03 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-03 · [x] descriptor de ventana de borrado identificado · [x] datos de prueba disponibles.

Notas: el borrado no es reversible; el baneo sí (US-08). La copia de los mensajes se toma antes del borrado para la evidencia (US-06).

#### US-05 — Contener contenido no deseado por patrón
**Épica:** EP-03 · **MoSCoW:** Must · **SP:** 5 (Fibonacci) · **CU:** CU-04 · **NB:** NB-03

Como administrador, quiero que el sistema detecte contenido no deseado en un mensaje mediante una expresión regular o palabras clave y contenga al emisor, para frenar contenido prohibido aunque no sea una ráfaga.

Criterios de aceptación:
- Given una regla de contenido con un patrón válido y una política en modo real, When llega un mensaje que coincide con el patrón, Then el sistema dispara la política y ejecuta las acciones configuradas.
- Given un patrón inválido al guardar la regla, When el administrador intenta persistirlo, Then el sistema rechaza el patrón por inválido y no lo guarda (edge case de validez del patrón).

DoR check: [x] valor explícito · [x] CU-04 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] sin dependencia bloqueante · [x] descriptor del patrón identificado · [x] mensajes de prueba disponibles.

Notas: la evaluación de contenido es sin estado y se evalúa con tope de tiempo para acotar el costo de la expresión regular.

#### US-06 — Reportar el incidente a un canal privado
**Épica:** EP-04 · **MoSCoW:** Must · **SP:** 3 (Fibonacci) · **CU:** CU-05 · **NB:** NB-04

Como administrador, quiero recibir en un canal privado los mensajes que dispararon una acción y la lista de canales afectados, para revisar si hubo un falso positivo.

Criterios de aceptación:
- Given una política que se disparó y un canal de salida designado, When se ejecuta la acción de reportar, Then el sistema envía al canal privado la copia de los mensajes accionados y la lista de canales afectados.
- Given un incidente cuyos mensajes serán borrados, When se procesa el incidente, Then la copia de los mensajes se toma antes del borrado y queda en la evidencia (edge case de orden evidencia/borrado).

DoR check: [x] valor explícito · [x] CU-05 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-04 · [x] descriptor de canal de salida identificado · [x] datos de prueba disponibles.

Notas: la integridad de la evidencia exige tomar la copia antes de cualquier remoción.

#### US-07 — Revisar incidentes y mensajes accionados
**Épica:** EP-04 · **MoSCoW:** Should · **SP:** 5 (Fibonacci) · **CU:** CU-06 · **NB:** NB-04

Como administrador, quiero revisar desde el panel los incidentes con la copia de los mensajes que activaron una acción y los canales afectados, para decidir si fue un falso positivo sin entrar a la plataforma.

Criterios de aceptación:
- Given incidentes registrados, When el administrador autenticado abre el panel de incidentes, Then el sistema lista los incidentes por fecha con su modo, resultado, copia de mensajes y canales afectados.
- Given un usuario no autenticado, When intenta abrir el panel de incidentes, Then el sistema deniega el acceso por autorización de rol administrador único (edge case de autorización).

DoR check: [x] valor explícito · [x] CU-06 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-06, US-10 · [x] no toca descriptor · [x] fixtures de incidentes disponibles.

Notas: el panel conserva solo la copia para revisión; los mensajes borrados no se restauran.

#### US-08 — Revertir un baneo (desbaneo) desde el panel
**Épica:** EP-04 · **MoSCoW:** Should · **SP:** 3 (Fibonacci) · **CU:** CU-07 · **NB:** NB-04

Como administrador, quiero desbanear a un usuario desde el panel cuando un incidente fue un falso positivo, para corregir un error de moderación sin restaurar los mensajes.

Criterios de aceptación:
- Given un incidente con resultado de baneo real, When el administrador confirma el desbaneo, Then el sistema revierte el baneo y registra el autor y la fecha de la reversión en el incidente.
- Given un incidente en modo simulación o no accionable, When el administrador intenta desbanear, Then el sistema no ofrece la reversión porque no hubo baneo real que revertir (edge case de coherencia).

DoR check: [x] valor explícito · [x] CU-07 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-07 · [x] no toca descriptor · [x] datos de prueba disponibles.

Notas: el desbaneo es reversible vía la API de la plataforma; el borrado de mensajes no.

#### US-09 — Alta de credenciales del administrador en el primer ingreso
**Épica:** EP-05 · **MoSCoW:** Must · **SP:** 3 (Fibonacci) · **CU:** CU-08 · **NB:** NB-05

Como propietario del sistema, quiero crear las credenciales del administrador en el primer ingreso, para habilitar la única cuenta que opera el sistema con su contraseña resguardada.

Criterios de aceptación:
- Given un sistema sin administrador, When el propietario completa el alta de credenciales en el primer ingreso, Then el sistema crea la cuenta única con la contraseña almacenada con hash robusto y nunca en texto claro.
- Given un administrador ya creado, When alguien intenta repetir el alta de primer ingreso, Then el sistema rechaza la creación porque la cuenta es única (edge case de unicidad).

DoR check: [x] valor explícito · [x] CU-08 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] sin dependencia bloqueante · [x] no toca descriptor · [x] datos de prueba disponibles.

Notas: la elección puntual de familia de hash queda abierta a Sprint 0; la US es agnóstica al algoritmo concreto siempre que sea de la familia robusta.

#### US-10 — Autenticar al administrador
**Épica:** EP-05 · **MoSCoW:** Must · **SP:** 3 (Fibonacci) · **CU:** CU-09 · **NB:** NB-05

Como administrador, quiero autenticarme en el panel con mis credenciales, para acceder a la configuración y la revisión de incidentes de forma segura.

Criterios de aceptación:
- Given la cuenta administradora creada, When el administrador ingresa credenciales correctas, Then el sistema verifica el hash, abre la sesión y habilita las funciones del rol.
- Given credenciales incorrectas, When el administrador intenta ingresar, Then el sistema rechaza el acceso sin revelar cuál dato falló (edge case de credenciales inválidas).

DoR check: [x] valor explícito · [x] CU-09 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-09 · [x] no toca descriptor · [x] datos de prueba disponibles.

Notas: existe un único rol administrador; no hay autoservicio de recuperación en v1.

#### US-11 — Registrar un servidor con su credencial de acceso
**Épica:** EP-05 · **MoSCoW:** Must · **SP:** 5 (Fibonacci) · **CU:** CU-10 · **NB:** NB-05

Como administrador, quiero registrar un servidor con su credencial de acceso, para que el sistema pueda conectarse y moderarlo, guardando la credencial cifrada en reposo.

Criterios de aceptación:
- Given el administrador autenticado, When registra un servidor con su snowflake y su credencial de acceso, Then el sistema guarda el servidor con la credencial cifrada en reposo y el snowflake como texto.
- Given un snowflake de servidor ya registrado, When el administrador intenta registrarlo de nuevo, Then el sistema rechaza el duplicado por unicidad del servidor (edge case de unicidad).

DoR check: [x] valor explícito · [x] CU-10 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-10 · [x] no toca descriptor de regla · [x] credencial de prueba disponible.

Notas: la credencial nunca se guarda en texto claro; se descifra solo en memoria para operar. El servidor queda inactivo hasta superar la prueba de configuración (US-14).

#### US-12 — Administrar reglas, grupos, eventos y acciones
**Épica:** EP-06 · **MoSCoW:** Must · **SP:** 8 (Fibonacci) · **CU:** CU-11 · **NB:** NB-05

Como administrador, quiero crear y ajustar reglas de contenido y de conducta, agruparlas, definir eventos con sus acciones y configurar parámetros, para gobernar la moderación sin depender del implementador.

Criterios de aceptación:
- Given un servidor registrado, When el administrador crea un grupo con una regla, define un evento que lo combina con su prioridad y le asocia acciones en orden, Then el sistema valida y persiste la configuración y deja el evento por defecto en modo simulación.
- Given un grupo de reglas sin ninguna regla asociada, When el administrador intenta guardarlo, Then el sistema rechaza el grupo por composición mínima (edge case de composición de grupo).
- Given un grupo referenciado por un evento, When el administrador intenta eliminarlo, Then el sistema bloquea la eliminación e indica la referencia (edge case de integridad referencial).

DoR check: [x] valor explícito · [x] CU-11 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-11, US-13 · [x] descriptores de parámetros identificados · [x] datos de prueba disponibles.

Notas: el anidamiento booleano se limita a dos niveles (grupo y combinación de grupos). El orden de evaluación por prioridad y el orden de las acciones son deterministas.

#### US-13 — Mostrar ayuda contextual por parámetro
**Épica:** EP-06 · **MoSCoW:** Should · **SP:** 3 (Fibonacci) · **CU:** CU-11 · **NB:** NB-05

Como administrador, quiero ver por cada parámetro su valor por defecto, su leyenda y sus ejemplos derivados del descriptor, para entender qué configuro sin documentación externa.

Criterios de aceptación:
- Given un parámetro con su descriptor (default, límites, leyenda, ejemplos), When el administrador abre su configuración, Then el sistema muestra el valor por defecto, la leyenda y los ejemplos derivados del descriptor.
- Given un valor ingresado fuera de los límites del descriptor, When el administrador intenta guardarlo, Then el sistema muestra los límites permitidos y ofrece el valor por defecto (edge case de validación por descriptor).

DoR check: [x] valor explícito · [x] CU-11 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende del registro de descriptores (BT) · [x] descriptores identificados · [x] datos de prueba disponibles.

Notas: el descriptor es la fuente única de verdad; la presentación visual concreta corresponde a la categoría 03.

#### US-14 — Probar la configuración de un servidor antes de activarlo
**Épica:** EP-07 · **MoSCoW:** Should · **SP:** 5 (Fibonacci) · **CU:** CU-12 · **NB:** NB-06

Como administrador, quiero probar la configuración de un servidor antes de activarlo, para no habilitar reglas que el sistema no podrá ejecutar.

Criterios de aceptación:
- Given un servidor registrado con su credencial, When el administrador ejecuta la prueba de configuración, Then el sistema valida credencial, permisos, jerarquía de roles, intents y canales y, si todo es correcto, habilita la activación.
- Given una credencial inválida o permisos insuficientes, When el administrador ejecuta la prueba, Then el sistema clasifica la falla como bloqueante o advertencia y mantiene el servidor inactivo (edge case de activación condicionada).

DoR check: [x] valor explícito · [x] CU-12 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-11 · [x] no toca descriptor de regla · [x] servidor de prueba disponible.

Notas: la activación queda condicionada a una prueba superada; un servidor no probado no entra en operación.

#### US-15 — Reconectar y mostrar el estado de conexión
**Épica:** EP-07 · **MoSCoW:** Should · **SP:** 5 (Fibonacci) · **CU:** CU-13 · **NB:** NB-06

Como administrador, quiero que el sistema reconecte automáticamente y muestre el estado de conexión de cada servidor, para saber cuándo un contexto quedó sin moderación.

Criterios de aceptación:
- Given un servidor conectado, When se cae el canal de eventos, Then el sistema marca el servidor como desconectado, reintenta la reconexión automáticamente y refleja el estado en el panel.
- Given una credencial revocada, When el sistema intenta reconectar, Then el sistema mantiene el servidor desconectado y reporta la condición sin entrar en un ciclo de reintento infinito ciego (edge case de credencial inválida).

DoR check: [x] valor explícito · [x] CU-13 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-11 · [x] no toca descriptor · [x] entorno de prueba disponible.

Notas: los mensajes no recibidos durante la caída no se evalúan; el borrado retroactivo cubre lo que quede dentro de la ventana.

#### US-16 — Ejecutar una política en modo simulación
**Épica:** EP-08 · **MoSCoW:** Should · **SP:** 3 (Fibonacci) · **CU:** CU-14 · **NB:** NB-07

Como administrador, quiero dejar una política en modo simulación que registre lo que haría sin ejecutarlo, para validar una regla antes de pasarla a ejecución real.

Criterios de aceptación:
- Given un evento en modo simulación que se dispara, When se procesa, Then el sistema registra un incidente simulado con lo que habría hecho y no ejecuta ninguna acción sobre la plataforma.
- Given un incidente registrado en modo simulación, When se consulta su resultado, Then el resultado nunca es ejecutada porque la simulación no acciona (edge case de coherencia modo/resultado).

DoR check: [x] valor explícito · [x] CU-14 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-03 o US-05 · [x] no toca descriptor · [x] mensajes de prueba disponibles.

Notas: el modo simulación es el valor por defecto de un evento nuevo.

#### US-17 — Definir exenciones por rol, usuario o canal
**Épica:** EP-08 · **MoSCoW:** Should · **SP:** 3 (Fibonacci) · **CU:** CU-15 · **NB:** NB-07

Como administrador, quiero definir exenciones por rol, usuario o canal de confianza, para que el staff y los canales legítimos no sean moderados.

Criterios de aceptación:
- Given una exención por rol staff, When un usuario con ese rol publica en varios canales distintos, Then el sistema descarta al emisor antes de evaluar reglas y no genera incidente.
- Given una exención duplicada (mismo servidor, tipo y sujeto), When el administrador intenta crearla, Then el sistema rechaza el duplicado (edge case de unicidad de exención).

DoR check: [x] valor explícito · [x] CU-15 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-11 · [x] no toca descriptor de regla · [x] datos de prueba disponibles.

Notas: el descarte de exentos es la primera etapa del pipeline, previa a cualquier evaluación.

#### US-18 — Evitar acciones repetidas durante una ráfaga (antirrebote)
**Épica:** EP-08 · **MoSCoW:** Should · **SP:** 3 (Fibonacci) · **CU:** CU-16 · **NB:** NB-07

Como administrador, quiero que el sistema no repita acciones sobre el mismo usuario durante una ráfaga dentro de una ventana de supresión, para evitar incidentes duplicados y acciones redundantes.

Criterios de aceptación:
- Given que ya se accionó sobre un usuario dentro de la ventana de antirrebote, When llega otro disparo sobre el mismo usuario en esa ventana, Then el sistema suprime la acción repetida y no genera un nuevo incidente.
- Given que pasó la ventana de antirrebote, When llega un nuevo disparo sobre el mismo usuario, Then el sistema vuelve a accionar y registra un nuevo incidente (edge case de expiración de la ventana).

DoR check: [x] valor explícito · [x] CU-16 declarado · [x] Given/When/Then con happy + edge · [x] estimada Fibonacci · [x] depende de US-03 · [x] descriptor de ventana de antirrebote identificado · [x] datos de prueba disponibles.

Notas: el estado de antirrebote vive en memoria y se pierde ante un reinicio (trade-off aceptado).

#### US-19 — Designar varios canales de salida con propósito lógico
**Épica:** EP-04 · **MoSCoW:** Could · **SP:** 3 (Fibonacci) · **CU:** CU-05, CU-11 · **NB:** NB-04

Como administrador, quiero designar más de un canal de salida con propósitos lógicos distintos (por ejemplo, reporte y volcado completo), para separar la información según su uso.

Criterios de aceptación:
- Given un servidor con dos canales de salida designados con propósitos distintos, When se dispara una política que reporta, Then el sistema envía cada salida al canal con el propósito lógico correspondiente.

DoR check: [x] valor explícito · [x] CU-05 y CU-11 declarados · [ ] Given/When/Then con segundo escenario pendiente de refinamiento (Could, no obligatorio aún) · [x] estimada Fibonacci · [x] depende de US-06, US-12 · [x] descriptor de canal de salida identificado · [ ] datos de prueba a construir.

Notas: historia Could de la categoría de alcance ampliado del intake (§4 Could Have). Queda en Borrador hasta su refinamiento; entra al backlog para no perderse, sin comprometer el MVP.

## 4. Métricas de avance

Resumen por prioridad sobre 19 US y 81 puntos de historia totales.

| Prioridad | Cantidad de US | Story points | % de SP |
| --- | --- | --- | --- |
| Must | 10 | 48 | 59 % |
| Should | 8 | 30 | 37 % |
| Could | 1 | 3 | 4 % |
| Won't (v1.0) | 0 | 0 | 0 % |

Nota de cálculo: Must suma US-01 (8), US-02 (3), US-03 (5), US-04 (5), US-05 (5), US-06 (3), US-09 (3), US-10 (3), US-11 (5), US-12 (8) = 48 SP. Should suma US-07 (5), US-08 (3), US-13 (3), US-14 (5), US-15 (5), US-16 (3), US-17 (3), US-18 (3) = 30 SP. Could suma US-19 (3). Total 81 SP; los porcentajes se calculan sobre ese total y suman 100. El MVP (Must) concentra el grueso del valor con el 59 % de los puntos.

| Métrica | Valor v1.0 |
| --- | --- |
| US totales | 19 |
| US cerradas (Done) | 0 |
| Porcentaje cerrado | 0 % |
| Deuda en backlog (Borrador, sin Ready) | 1 (US-19) |
| Épicas vigentes | 8 |

Cada NB-01..NB-07 tiene al menos una US y cada CU-01..CU-16 tiene al menos una US (ver matriz de la sección 3.1 y la matriz BT↔US↔CU del backlog técnico).

## 5. Refinamiento

- Cadencia: una sesión de Backlog Refinement por sprint, según el mínimo para web-monolith de las reglas de la categoría 06. Al ser un equipo de un único desarrollador (Fernando, con asistencia de IA), la sesión es de autocuraduría guiada: revisión de la siguiente rebanada vertical, de la trazabilidad a CU y de la estimación.
- Responsable: el Scrum Master (AG-06), que cura el backlog y firma la DoR. Las revisiones acotadas de trazabilidad (AG-02) y de fuente técnica (AG-05) se incorporan cuando hay duda sobre el CU de una US o la fuente upstream de una BT.
- Técnica de estimación: Fibonacci (1, 2, 3, 5, 8, 13), declarada en la cabecera y mantenida en todas las US y BT. Para un equipo sin historial de velocity todavía, la primera estimación es de calibración y se reajusta tras el walking skeleton.
- Formato: revisión de cada ítem contra INVEST y contra la DoR antes de Sprint Planning; los ítems que no pasan vuelven al backlog para refinarse.
- Orden de entrega: vertical slicing. El primer sprint es el walking skeleton end-to-end (registrar servidor, recibir mensaje, evaluar ráfaga, reportar en modo simulación), que toca US-11, US-01, US-02 y US-16 en su versión mínima; las rebanadas siguientes incorporan baneo, borrado retroactivo, contenido, incidentes y desbaneo, configuración con descriptores, prueba y reconexión, y exenciones con antirrebote.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Backlog inicial con 8 épicas, 19 historias inline (10 Must, 8 Should, 1 Could) trazadas a los 16 CU y las 7 NB, estimación Fibonacci, métricas de avance y política de refinement. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: §1 "Objetivos del producto" acortada al límite de 1-3 oraciones de §4.2, preservando el propósito y el MVP. |
