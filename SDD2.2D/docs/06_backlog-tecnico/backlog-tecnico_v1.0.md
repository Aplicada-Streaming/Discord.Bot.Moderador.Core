# Backlog técnico — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-20
**Autor:** Scrum Master (AG-06)
**Estimación:** Fibonacci (1, 2, 3, 5, 8, 13)

Vista del backlog desde la lente técnica, organizada por épicas técnicas y por tareas técnicas (BT-XX) con su fuente upstream en la categoría 05 (componentes de la vista lógica, ADR-01..ADR-13, modelo lógico y puntos de extensión). Cada BT declara al menos una US consumidora o se justifica como infraestructura compartida. El catálogo de historias y su priorización viven en `product-backlog_v1.0.md`. El piso para web-monolith es de 8 BT; este backlog declara 17, por debajo del umbral de 30 de §3.3 de las reglas, por lo que las BT viven inline en este documento.

## 1. Épicas técnicas

| EPT | Nombre | Objetivo | Alcance | Fuente upstream (05) | BT contenidas |
| --- | --- | --- | --- | --- | --- |
| EPT-01 | Fundaciones y persistencia | Establecer el esqueleto de capas y la base relacional embebida con su esquema versionado | Solución por capas, composición de dependencias, modelo lógico de 13 tablas, migración inicial, modo WAL | ADR-01, ADR-04, ADR-02; `modelo-datos-logico_v1.0.md`; componente Persistencia | BT-01, BT-02, BT-03 |
| EPT-02 | Motor de moderación (pipeline) | Construir el núcleo de dominio que evalúa cada mensaje hasta el incidente | Pipeline de 9 etapas, evaluadores de contenido y conducta, estado en memoria, evaluador de políticas, antirrebote, manejo de resultados | ADR-01, ADR-04, ADR-08, ADR-09; `flujo-ejecucion_v1.0.md`; componentes Motor de moderación, Evaluadores, Estado de conducta, Antirrebote | BT-04, BT-05, BT-06, BT-07, BT-08 |
| EPT-03 | Integración con la plataforma | Conectar al canal de eventos en tiempo real y ejecutar acciones de moderación por contexto | Adaptador del gateway y de la API, normalización de mensajes, ejecución de acciones, reconexión, una conexión por contexto | ADR-13, ADR-08; componentes Adaptador del gateway y de la API, Ejecutor de acciones | BT-09, BT-10, BT-11 |
| EPT-04 | Seguridad y secretos | Resguardar credenciales del administrador y cifrar los tokens en reposo | Hash robusto de contraseña, sesión y rol único, cifrado simétrico con clave maestra fuera de la base, marco legal | ADR-03, ADR-07, ADR-06; componentes Servicio de autenticación, Servicio de cifrado de tokens | BT-12, BT-13 |
| EPT-05 | Configuración dirigida por descriptores | Hacer del descriptor la fuente única de verdad de cada parámetro configurable | Registro de descriptores, validación genérica, superficie de extensión de nuevos descriptores/reglas/acciones | ADR-12; `extensibilidad_v1.0.md`; componentes Registro de descriptores, Servicio de configuración | BT-14, BT-15 |
| EPT-06 | Presentación del panel | Servir la cara de administración con render interactivo del lado servidor | Páginas de registro, configuración, incidentes y desbaneo; integración con los servicios de aplicación | ADR-01; componente Panel de administración | BT-16 |
| EPT-07 | Despliegue y operación | Empaquetar el artefacto self-contained y registrarlo como servicio del sistema | Publicación self-contained para la arquitectura objetivo, instalador y unidad de servicio, observabilidad al journal | ADR-05, ADR-11; `arquitectura-solucion_v1.0.md §5, §7`; `SOLUTION-INTAKE §17 P.8` | BT-17 |

## 2. BT por épica

Cada BT vive inline con su tipo, prioridad, estimación, fuente upstream, dependencias y criterios de aceptación. Las prioridades de las BT heredan la urgencia de las US que las consumen.

### EPT-01 Fundaciones y persistencia

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-01 | Esqueleto de capas y composición de dependencias | feature | Must | 5 | ADR-01, ADR-04; arquitectura §2 | — |
| BT-02 | Esquema relacional embebido y migración inicial | feature | Must | 5 | ADR-02; modelo-datos-logico §1-§5 (MIG-0001) | BT-01 |
| BT-03 | Modo WAL y concurrencia bot/panel sobre la persistencia | feature | Must | 3 | ADR-02; arquitectura §4 (concurrencia) | BT-02 |

- BT-01 — Esqueleto de capas y composición de dependencias. Criterios: existe la separación Dominio, Aplicación, Infraestructura y Presentación con dependencia unidireccional hacia el Dominio; el Dominio compila y se prueba sin infraestructura; la composición de dependencias arranca el host web y el servicio en segundo plano en un solo proceso. Infraestructura compartida justificada por ADR-01 y ADR-04; consumida transversalmente por todas las US.
- BT-02 — Esquema relacional embebido y migración inicial. Criterios: la migración `MIG-0001-esquema-inicial` crea las 13 tablas con sus claves, restricciones de unicidad y check, y los índices del modelo lógico; el esquema es reconstruible en un entorno limpio; los snowflakes se almacenan como texto. Consumida por US-11, US-12, US-07.
- BT-03 — Modo WAL y concurrencia bot/panel. Criterios: la conexión habilita el registro de escritura anticipada; una escritura de auditoría del bot y una de configuración del panel coexisten sin bloqueo de lectores en una prueba de concurrencia. Consumida por US-06, US-12.

### EPT-02 Motor de moderación (pipeline)

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-04 | Pipeline de evaluación y orquestación de etapas | feature | Must | 8 | ADR-01, ADR-04; flujo-ejecucion §2 | BT-01 |
| BT-05 | Evaluador de reglas de contenido con tope de tiempo | feature | Must | 5 | ADR-04, ADR-08; componente Evaluador de contenido; RN-03 | BT-04 |
| BT-06 | Estado de conducta en memoria y evaluador de ráfaga | feature | Must | 8 | ADR-09; componentes Estado de conducta, Evaluador de conducta | BT-04 |
| BT-07 | Evaluador de políticas por prioridad con primera coincidencia | feature | Must | 5 | ADR-12; componente Evaluador de políticas; RN-04 | BT-04, BT-14 |
| BT-08 | Antirrebote por usuario y manejo de resultados de moderación | feature | Should | 5 | ADR-09, ADR-08; componentes Antirrebote, Motor de moderación | BT-04 |

- BT-04 — Pipeline de evaluación. Criterios: el motor recorre las nueve etapas del flujo (descarte de exentos, contenido, conducta, políticas, copia de evidencia, decisión de modo, antirrebote, ejecución, registro) en orden; cada etapa es probada de forma aislada; el dominio del pipeline alcanza el gate de cobertura del módulo de detección (≥ 90 %). Consumida por US-01, US-03, US-05, US-16.
- BT-05 — Evaluador de contenido. Criterios: evalúa expresión regular y palabras clave sobre un mensaje aislado; valida el patrón al guardar (RN-03) y aplica un tope de tiempo de evaluación para acotar el retroceso catastrófico. Consumida por US-05.
- BT-06 — Estado de conducta y evaluador de ráfaga. Criterios: mantiene ventanas deslizantes de actividad por usuario y por contexto en memoria; cuenta canales distintos en la ventana; se reconstruye tras un reinicio; la condición se marca al alcanzar el umbral. Consumida por US-01, US-02.
- BT-07 — Evaluador de políticas. Criterios: combina coincidencias por modo de grupo (todas, alguna, al menos N) y por combinación de grupos en un evento; evalúa por prioridad con primera coincidencia y respeta la bandera continuar (RN-04). Consumida por US-01, US-05, US-12.
- BT-08 — Antirrebote y manejo de resultados. Criterios: suprime acciones repetidas sobre el mismo usuario dentro de la ventana de antirrebote; expresa el resultado de moderación como conjunto cerrado (ejecutada, simulada, no accionable, fallida) y no aborta el pipeline ante una falla de acción (RN-01, ADR-08). Consumida por US-18, US-03.

### EPT-03 Integración con la plataforma

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Adaptador del canal de eventos y normalización de mensajes | feature | Must | 8 | ADR-13; componente Adaptador del gateway y de la API | BT-01 |
| BT-10 | Ejecutor de acciones de moderación en orden | feature | Must | 5 | ADR-08, ADR-13; componente Ejecutor de acciones; RN-05 | BT-09, BT-04 |
| BT-11 | Reconexión automática y estado de conexión por contexto | feature | Should | 5 | ADR-13; arquitectura §4; RN-16 | BT-09 |

- BT-09 — Adaptador del canal de eventos. Criterios: abre una conexión por contexto (firewall multi-contexto, ADR-13); normaliza el evento entrante en un Mensaje de dominio con sus snowflakes como texto; respeta el presupuesto de memoria por conexión (≤ 8 MB). Consumida por US-01, US-11.
- BT-10 — Ejecutor de acciones. Criterios: ejecuta reportar, banear, banear con borrado retroactivo (ventana 0..7 días), desbanear, timeout, expulsar y rol en el orden de ejecución; clasifica como no accionable la acción imposible por jerarquía de roles sin abortar; toma la copia de mensajes antes de cualquier borrado. Consumida por US-03, US-04, US-05, US-06, US-08.
- BT-11 — Reconexión y estado de conexión. Criterios: detecta la caída del canal, marca el contexto como desconectado, reintenta la reconexión y refleja el estado; ante credencial revocada no entra en reintento infinito ciego. Consumida por US-15.

### EPT-04 Seguridad y secretos

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-12 | Autenticación de administrador único con hash robusto | feature | Must | 5 | ADR-03, ADR-06; componente Servicio de autenticación; RN-12, RN-13 | BT-02 |
| BT-13 | Cifrado de tokens en reposo con clave maestra | feature | Must | 5 | ADR-07; componente Servicio de cifrado de tokens; RN-14 | BT-02 |

- BT-12 — Autenticación de administrador único. Criterios: alta de credenciales en el primer ingreso con contraseña en hash robusto en formato PHC; verificación en el ingreso; cuenta única (a lo sumo una fila) y rol administrador único; minimización y retención conforme al marco legal (ADR-06). Consumida por US-09, US-10, US-07.
- BT-13 — Cifrado de tokens en reposo. Criterios: el token se cifra con cifrado simétrico usando una clave maestra que vive fuera de la base (variable de entorno); nunca se persiste en texto claro; se descifra solo en memoria para operar; el rollback preserva la clave y los tokens cifrados siguen siendo válidos. Consumida por US-11, US-14.

### EPT-05 Configuración dirigida por descriptores

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-14 | Registro de descriptores y validación genérica por descriptor | feature | Must | 5 | ADR-12; extensibilidad §1.1; componente Registro de descriptores; RN-10 | BT-01 |
| BT-15 | CRUD de reglas, grupos, eventos, acciones y exenciones con integridad | feature | Must | 8 | ADR-12; componente Servicio de configuración; RN-15, RC-03, RC-04 | BT-14, BT-02 |

- BT-14 — Registro de descriptores. Criterios: cada parámetro configurable tiene un descriptor único (identificador, tipo, default, límites, leyenda, ejemplos) como fuente de verdad; la validación, el default y la ayuda en pantalla se derivan del descriptor; agregar un descriptor nuevo no toca la lógica genérica de validación (superficie de extensión §1.1). Consumida por US-02, US-13, US-12, US-18.
- BT-15 — CRUD de configuración con integridad. Criterios: alta, edición y baja de reglas, grupos, eventos, acciones y exenciones; rechaza grupo sin reglas (composición mínima), bloquea la eliminación de un elemento referenciado, garantiza unicidad de asociaciones y de exenciones; deja el evento nuevo por defecto en modo simulación. Consumida por US-12, US-17, US-16.

### EPT-06 Presentación del panel

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-16 | Páginas del panel: registro, configuración, incidentes y desbaneo | feature | Should | 8 | ADR-01; componente Panel de administración | BT-12, BT-15, BT-02 |

- BT-16 — Páginas del panel. Criterios: render interactivo del lado servidor para registrar servidor, configurar reglas con ayuda contextual, revisar incidentes con su evidencia y confirmar el desbaneo; toda página exige sesión autenticada y rol administrador; los errores de validación derivados de los descriptores se presentan al usuario. Consumida por US-07, US-08, US-12, US-13, US-19.

### EPT-07 Despliegue y operación

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias |
| --- | --- | --- | --- | --- | --- | --- |
| BT-17 | Empaquetado self-contained, servicio del sistema y observabilidad | devops | Should | 5 | ADR-05, ADR-11; arquitectura §5, §7; intake §17 P.8 | BT-01 |

- BT-17 — Empaquetado y servicio. Criterios: publicación self-contained para la arquitectura de CPU objetivo por compilación cruzada; instalador que registra el servicio en el supervisor del sistema con reinicio automático; el rollback conserva el archivo de entorno y la clave maestra; los eventos del servicio se registran al journal del sistema y cada incidente queda auditado en la base. Infraestructura compartida justificada por ADR-05 y ADR-11; habilita la operación de todas las US en el entorno objetivo y es consumida en particular por US-15 (estado de conexión observable) y US-11 (servicio operativo).

## 3. Trazabilidad BT↔US↔CU

Para cada BT, las US que la consumen y los CU upstream que esas US cubren. Las BT de infraestructura compartida (BT-01, BT-17) declaran su justificación de ADR y las US que habilitan.

| BT | Título | US consumidoras | CU upstream | Fuente upstream (05) |
| --- | --- | --- | --- | --- |
| BT-01 | Esqueleto de capas y composición | Todas (infraestructura compartida) | CU-01..CU-16 | ADR-01, ADR-04 |
| BT-02 | Esquema y migración inicial | US-11, US-12, US-07 | CU-10, CU-11, CU-06 | ADR-02; modelo lógico MIG-0001 |
| BT-03 | Modo WAL y concurrencia | US-06, US-12 | CU-05, CU-11 | ADR-02; arquitectura §4 |
| BT-04 | Pipeline de evaluación | US-01, US-03, US-05, US-16 | CU-01, CU-02, CU-04, CU-14 | ADR-01, ADR-04; flujo §2 |
| BT-05 | Evaluador de contenido | US-05 | CU-04 | ADR-04, ADR-08; Evaluador de contenido; RN-03 |
| BT-06 | Estado de conducta y ráfaga | US-01, US-02 | CU-01 | ADR-09; Estado de conducta, Evaluador de conducta |
| BT-07 | Evaluador de políticas | US-01, US-05, US-12 | CU-01, CU-04, CU-11 | ADR-12; Evaluador de políticas; RN-04 |
| BT-08 | Antirrebote y resultados | US-18, US-03 | CU-16, CU-02 | ADR-09, ADR-08; Antirrebote |
| BT-09 | Adaptador del canal de eventos | US-01, US-11 | CU-01, CU-10 | ADR-13; Adaptador del gateway |
| BT-10 | Ejecutor de acciones | US-03, US-04, US-05, US-06, US-08 | CU-02, CU-03, CU-04, CU-05, CU-07 | ADR-08, ADR-13; Ejecutor de acciones; RN-05 |
| BT-11 | Reconexión y estado | US-15 | CU-13 | ADR-13; arquitectura §4; RN-16 |
| BT-12 | Autenticación de administrador | US-09, US-10, US-07 | CU-08, CU-09, CU-06 | ADR-03, ADR-06; Servicio de autenticación; RN-12, RN-13 |
| BT-13 | Cifrado de tokens | US-11, US-14 | CU-10, CU-12 | ADR-07; Servicio de cifrado; RN-14 |
| BT-14 | Registro de descriptores | US-02, US-13, US-12, US-18 | CU-01, CU-11, CU-16 | ADR-12; extensibilidad §1.1; RN-10 |
| BT-15 | CRUD de configuración con integridad | US-12, US-17, US-16 | CU-11, CU-15, CU-14 | ADR-12; Servicio de configuración; RN-15 |
| BT-16 | Páginas del panel | US-07, US-08, US-12, US-13, US-19 | CU-06, CU-07, CU-11, CU-05 | ADR-01; Panel de administración |
| BT-17 | Empaquetado y servicio | Todas (infraestructura compartida); US-15, US-11 | CU-13, CU-10 | ADR-05, ADR-11; arquitectura §5, §7 |

Cobertura de US por BT: cada una de las 19 US tiene al menos una BT que la soporta directamente, salvo US-14 (prueba de configuración) que consume BT-13 (cifrado, para descifrar y validar la credencial) y se apoya en BT-09 y BT-16 para ejecutar la prueba y mostrarla; US-14 está cubierta por BT-13 y BT-16. Cobertura de CU: los 16 CU quedan referenciados en la columna CU upstream de esta matriz.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Backlog técnico inicial con 7 épicas técnicas y 17 BT inline, cada una con fuente upstream en 05 (ADR-01..ADR-13, componentes, modelo lógico, extensibilidad) y al menos una US consumidora o justificación de infraestructura compartida, más la matriz BT↔US↔CU completa. |
