# Modelo de datos lógico — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** modelo-datos-logico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)

Este documento traduce el modelo conceptual de 13 entidades de `02_especificacion_funcional/modelo-datos/modelo-conceptual_v1.0.md` a un modelo lógico con tipos físicos, nulabilidad, defaults, índices, restricciones y migración inicial, para la base de datos relacional embebida en modo WAL (ADR-02). Los tipos físicos se expresan de forma abstracta (TEXTO, ENTERO, BOOLEANO, MARCA_DE_TIEMPO) sin nombrar productos comerciales. Los identificadores de la plataforma (snowflakes) se almacenan como TEXTO (RN-08, RC-02, ADR-13) y el token de bot se almacena cifrado (RN-14, RC-07, ADR-07).

## 1. Tablas

Una subsección por tabla, con propósito y entidad conceptual de origen.

### 1.1 Administrador

Cuenta única que opera el sistema. Origen conceptual: Administrador (1.1). A lo sumo una fila (RC-06).

### 1.2 Servidor

Contexto del firewall multi-contexto: servidor a moderar con su credencial y estado. Origen conceptual: Servidor (1.2). El token se guarda cifrado (RC-07).

### 1.3 CanalDeSalida

Canal designado con propósito lógico al que el sistema reporta. Origen conceptual: CanalDeSalida (1.3). Depende de Servidor (RC-01).

### 1.4 Exencion

Sujeto de confianza (rol, usuario o canal) excluido de la moderación. Origen conceptual: Exención (1.4). Depende de Servidor (RC-01).

### 1.5 Regla

Predicado configurable de contenido (sin estado) o de conducta (con estado). Origen conceptual: Regla (1.5). Depende de Servidor (RC-01). El criterio es válido según la clase (RC-09).

### 1.6 GrupoDeReglas

Conjunto de reglas con un modo de coincidencia (todas, alguna, al menos N). Origen conceptual: GrupoDeReglas (1.6). Depende de Servidor (RC-01); composición mínima de una regla (RC-04).

### 1.7 GrupoRegla

Asociación muchos a muchos entre GrupoDeReglas y Regla. Origen conceptual: GrupoRegla (1.7). Ambos extremos deben existir (RC-03).

### 1.8 Evento

Política de moderación con prioridad, bandera continuar y modo. Origen conceptual: Evento (1.8). Depende de Servidor (RC-01); orden por prioridad (RC-05); modo simulación por defecto (RC-10).

### 1.9 EventoGrupo

Asociación muchos a muchos entre Evento y GrupoDeReglas. Origen conceptual: EventoGrupo (1.9). Ambos extremos deben existir (RC-03).

### 1.10 Accion

Operación de moderación de un evento, con orden de ejecución y parámetros. Origen conceptual: Acción (1.10). Depende de Evento; orden de ejecución (RC-05); ventana de borrado entre 0 y 7 días (RC-11).

### 1.11 Incidente

Registro de un disparo de evento con su modo, resultado y eventual reversión. Origen conceptual: Incidente (1.11). Depende de Servidor y de Evento (RC-01); modo coherente con el evento (RC-10).

### 1.12 MensajeAccionado

Copia de un mensaje involucrado en un incidente, conservada antes de cualquier remoción. Origen conceptual: MensajeAccionado (1.12). Depende de Incidente (RC-01, RN-11).

### 1.13 CanalAfectado

Canal en el que el incidente tuvo efecto. Origen conceptual: CanalAfectado (1.13). Depende de Incidente (RC-01, RN-11).

## 2. Atributos con tipo de dato físico

Tipos abstractos: TEXTO (cadena), ENTERO, BOOLEANO, MARCA_DE_TIEMPO. PK = clave primaria; FK = clave foránea. Los snowflakes son TEXTO (RC-02).

### 2.1 Administrador

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| identificador_cuenta | TEXTO | No | — | Único (RC-06) |
| resguardo_password | TEXTO | No | — | Hash PHC, nunca en claro (RN-13, RC-06, ADR-03) |
| creado_en | MARCA_DE_TIEMPO | No | momento de alta | Primer ingreso (CU-08) |

### 2.2 Servidor

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| snowflake_servidor | TEXTO | No | — | Snowflake como TEXTO; único (RN-08, RC-02) |
| token_cifrado | TEXTO | No | — | Token cifrado en reposo (RN-14, RC-07, ADR-07) |
| estado_conexion | TEXTO | No | 'desconectado' | Conjunto cerrado: conectado, desconectado |
| estado_activacion | TEXTO | No | 'inactivo' | Conjunto cerrado: inactivo, activo (activo solo con prueba superada, RC-08, RN-16) |
| nombre_descriptivo | TEXTO | Sí | — | Etiqueta para el panel |
| creado_en | MARCA_DE_TIEMPO | No | momento de alta | CU-10 |

### 2.3 CanalDeSalida

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| servidor_id | ENTERO | No | — | FK a Servidor (RC-01) |
| snowflake_canal | TEXTO | No | — | Snowflake como TEXTO (RC-02) |
| proposito_logico | TEXTO | No | — | Rol del canal, p. ej. reporte de incidentes |

### 2.4 Exencion

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| servidor_id | ENTERO | No | — | FK a Servidor (RC-01) |
| tipo_sujeto | TEXTO | No | — | Conjunto cerrado: rol, usuario, canal |
| snowflake_sujeto | TEXTO | No | — | Snowflake como TEXTO (RC-02) |

### 2.5 Regla

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| servidor_id | ENTERO | No | — | FK a Servidor (RC-01) |
| clase | TEXTO | No | — | Conjunto cerrado: contenido, conducta |
| criterio | TEXTO | No | — | Expresión regular o palabras clave (contenido); umbral/ventana serializados (conducta). Válido según la clase (RC-09, RN-03) |
| nombre | TEXTO | Sí | — | Etiqueta para el panel |

### 2.6 GrupoDeReglas

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| servidor_id | ENTERO | No | — | FK a Servidor (RC-01) |
| modo_coincidencia | TEXTO | No | — | Conjunto cerrado: todas, alguna, al_menos_n (RC-04, RN-15) |
| n_minimo | ENTERO | Sí | — | Requerido solo si modo = al_menos_n |
| nombre | TEXTO | Sí | — | Etiqueta para el panel |

### 2.7 GrupoRegla

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| grupo_id | ENTERO | No | — | FK a GrupoDeReglas (RC-03) |
| regla_id | ENTERO | No | — | FK a Regla (RC-03) |

### 2.8 Evento

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| servidor_id | ENTERO | No | — | FK a Servidor (RC-01) |
| prioridad | ENTERO | No | — | Orden de evaluación (RC-05, RN-04) |
| bandera_continuar | BOOLEANO | No | falso | Permite seguir evaluando tras coincidir (RN-04) |
| modo | TEXTO | No | 'simulacion' | Conjunto cerrado: simulacion, real (simulación por defecto, RC-10, RN-09) |
| nombre | TEXTO | Sí | — | Etiqueta para el panel |

### 2.9 EventoGrupo

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| evento_id | ENTERO | No | — | FK a Evento (RC-03) |
| grupo_id | ENTERO | No | — | FK a GrupoDeReglas (RC-03) |

### 2.10 Accion

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| evento_id | ENTERO | No | — | FK a Evento |
| tipo | TEXTO | No | — | Conjunto cerrado: reportar, banear, banear_con_borrado, desbanear, timeout, expulsar, asignar_rol, quitar_rol |
| orden_ejecucion | ENTERO | No | — | Posición dentro del evento (RC-05, RN-05) |
| ventana_borrado_dias | ENTERO | Sí | 0 | Entre 0 y 7 cuando aplica (RC-11, RN-02) |
| parametros | TEXTO | Sí | — | Parámetros serializados según el tipo (p. ej. snowflake de rol) |

### 2.11 Incidente

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| servidor_id | ENTERO | No | — | FK a Servidor (RC-01) |
| evento_id | ENTERO | No | — | FK a Evento |
| fecha | MARCA_DE_TIEMPO | No | momento del disparo | Obligatorio |
| snowflake_emisor | TEXTO | No | — | Snowflake como TEXTO (RC-02) |
| modo | TEXTO | No | — | Conjunto cerrado: real, simulacion (coherente con el evento, RC-10, RN-09) |
| resultado | TEXTO | No | — | Conjunto cerrado: ejecutada, simulada, no_accionable, fallida (RN-01) |
| reversion_autor_id | ENTERO | Sí | — | FK a Administrador; solo si el resultado fue baneo real (CU-07) |
| reversion_fecha | MARCA_DE_TIEMPO | Sí | — | Fecha del desbaneo, si lo hubo |

### 2.12 MensajeAccionado

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| incidente_id | ENTERO | No | — | FK a Incidente (RC-01, RN-11) |
| snowflake_mensaje | TEXTO | No | — | Snowflake como TEXTO (RC-02) |
| contenido_copiado | TEXTO | No | — | Copia tomada antes de la remoción (RN-11, RN-05) |
| snowflake_canal | TEXTO | Sí | — | Canal donde estaba el mensaje |

### 2.13 CanalAfectado

| Atributo | Tipo | Nulable | Default | Notas |
| --- | --- | --- | --- | --- |
| id | ENTERO | No | autoincremental | PK |
| incidente_id | ENTERO | No | — | FK a Incidente (RC-01, RN-11) |
| snowflake_canal | TEXTO | No | — | Snowflake como TEXTO (RC-02) |

## 3. Índices

| Índice | Tabla | Columnas | Tipo | Motivación |
| --- | --- | --- | --- | --- |
| ux_administrador_cuenta | Administrador | identificador_cuenta | Único | Unicidad de la cuenta (RC-06) |
| ux_servidor_snowflake | Servidor | snowflake_servidor | Único | Unicidad del servidor por snowflake (RN-08, RC-02) |
| ix_canal_salida_servidor | CanalDeSalida | servidor_id | Compuesto/simple | Listar canales por servidor (CU-05, CU-10) |
| ix_exencion_servidor | Exencion | servidor_id | Simple | Descarte de exentos por servidor (RN-07) |
| ux_exencion_sujeto | Exencion | servidor_id, tipo_sujeto, snowflake_sujeto | Único | Evitar exenciones duplicadas |
| ix_regla_servidor | Regla | servidor_id | Simple | Cargar reglas por servidor (CU-11) |
| ux_gruporegla_par | GrupoRegla | grupo_id, regla_id | Único | Asociación única grupo-regla (RC-03) |
| ux_eventogrupo_par | EventoGrupo | evento_id, grupo_id | Único | Asociación única evento-grupo (RC-03) |
| ix_evento_servidor_prioridad | Evento | servidor_id, prioridad | Compuesto | Evaluación por prioridad (RC-05, RN-04) |
| ix_accion_evento_orden | Accion | evento_id, orden_ejecucion | Compuesto | Orden determinista de acciones (RC-05, RN-05) |
| ix_incidente_servidor_fecha | Incidente | servidor_id, fecha | Compuesto | Revisión de incidentes por fecha (CU-06) |
| ix_mensaje_accionado_incidente | MensajeAccionado | incidente_id | Simple | Recuperar evidencia del incidente (CU-06, RN-11) |
| ix_canal_afectado_incidente | CanalAfectado | incidente_id | Simple | Listar canales afectados (CU-06, RN-11) |

## 4. Restricciones

| Restricción | Tabla | Tipo | Definición |
| --- | --- | --- | --- |
| pk_* | todas | PK | Clave primaria por tabla (columna id) |
| fk_canal_salida_servidor | CanalDeSalida | FK | servidor_id → Servidor(id); borrado en cascada o bloqueado (RC-01) |
| fk_exencion_servidor | Exencion | FK | servidor_id → Servidor(id) (RC-01) |
| fk_regla_servidor | Regla | FK | servidor_id → Servidor(id) (RC-01) |
| fk_grupo_servidor | GrupoDeReglas | FK | servidor_id → Servidor(id) (RC-01) |
| fk_gruporegla_grupo / _regla | GrupoRegla | FK | grupo_id → GrupoDeReglas(id); regla_id → Regla(id) (RC-03) |
| fk_evento_servidor | Evento | FK | servidor_id → Servidor(id) (RC-01) |
| fk_eventogrupo_evento / _grupo | EventoGrupo | FK | evento_id → Evento(id); grupo_id → GrupoDeReglas(id) (RC-03) |
| fk_accion_evento | Accion | FK | evento_id → Evento(id) |
| fk_incidente_servidor / _evento | Incidente | FK | servidor_id → Servidor(id); evento_id → Evento(id) (RC-01) |
| fk_incidente_reversion_autor | Incidente | FK | reversion_autor_id → Administrador(id) |
| fk_mensaje_incidente | MensajeAccionado | FK | incidente_id → Incidente(id) (RC-01, RN-11) |
| fk_canal_afectado_incidente | CanalAfectado | FK | incidente_id → Incidente(id) (RC-01, RN-11) |
| ck_admin_unico | Administrador | Check | A lo sumo una fila (RC-06) |
| ck_servidor_estado_conexion | Servidor | Check | estado_conexion ∈ {conectado, desconectado} |
| ck_servidor_estado_activacion | Servidor | Check | estado_activacion ∈ {inactivo, activo} (RC-08, RN-16) |
| ck_exencion_tipo | Exencion | Check | tipo_sujeto ∈ {rol, usuario, canal} |
| ck_regla_clase | Regla | Check | clase ∈ {contenido, conducta} (RC-09) |
| ck_grupo_modo | GrupoDeReglas | Check | modo_coincidencia ∈ {todas, alguna, al_menos_n}; n_minimo presente si al_menos_n (RC-04, RN-15) |
| ck_evento_modo | Evento | Check | modo ∈ {simulacion, real} (RC-10, RN-09) |
| ck_accion_tipo | Accion | Check | tipo ∈ {reportar, banear, banear_con_borrado, desbanear, timeout, expulsar, asignar_rol, quitar_rol} |
| ck_accion_ventana | Accion | Check | ventana_borrado_dias entre 0 y 7 (RC-11, RN-02) |
| ck_incidente_modo | Incidente | Check | modo ∈ {real, simulacion} y coherente con el evento (RC-10) |
| ck_incidente_resultado | Incidente | Check | resultado ∈ {ejecutada, simulada, no_accionable, fallida} (RN-01) |
| ck_incidente_simulacion_no_ejecuta | Incidente | Check | si modo = simulacion ⇒ resultado ≠ ejecutada (RC-10, RN-09) |

## 5. Migración inicial

La migración inicial se gestiona con el tooling de migración versionada del ORM (sin nombrar productos comerciales). Identificador abstracto: `MIG-0001-esquema-inicial`. Resumen del cambio: crea las 13 tablas (Administrador, Servidor, CanalDeSalida, Exencion, Regla, GrupoDeReglas, GrupoRegla, Evento, EventoGrupo, Accion, Incidente, MensajeAccionado, CanalAfectado) con sus claves primarias, foráneas, restricciones de unicidad y check, y los índices de §3; habilita el modo de registro de escritura anticipada (WAL) en la conexión (ADR-02). Las migraciones posteriores se versionan de forma incremental y conservan el historial. El esquema es reconstruible desde la migración inicial en un entorno limpio.

## 6. Estrategia multi-tenant

No multi-tenant (`multi_tenant = false`). Hay un único administrador (RC-06). La operación de varios servidores se modela como multi-contexto dentro de la misma instancia y la misma base: cada Servidor es un contexto del firewall (ADR-13), y todas las entidades dependientes se aíslan por `servidor_id` (RC-01). No hay columna discriminadora de inquilino, ni esquema por inquilino, ni base por inquilino. El aislamiento entre servidores es por la clave foránea a Servidor y por la partición en memoria del estado de conducta (ADR-09). En v1 se opera un solo servidor por el límite de memoria del proceso de 32 bits (`SOLUTION-INTAKE §17 P.12`).

## 7. Trazabilidad

Cada tabla lógica nace de una entidad conceptual de 02 y declara los CU que la consumen (coherente con la tabla §8 del modelo conceptual).

| Tabla lógica | Entidad conceptual de origen (02) | CU que la consumen | RC/RN aplicables |
| --- | --- | --- | --- |
| Administrador | Administrador (1.1) | CU-06, CU-07, CU-08, CU-09 | RC-06, RN-12, RN-13 |
| Servidor | Servidor (1.2) | CU-01, CU-10, CU-12, CU-13 | RC-02, RC-07, RC-08, RN-08, RN-14, RN-16 |
| CanalDeSalida | CanalDeSalida (1.3) | CU-05, CU-10 | RC-01, RC-02, RN-08 |
| Exencion | Exención (1.4) | CU-01, CU-04, CU-15 | RC-01, RC-02, RN-07, RN-08 |
| Regla | Regla (1.5) | CU-01, CU-04, CU-11 | RC-09, RN-03, RN-10, RN-15 |
| GrupoDeReglas | GrupoDeReglas (1.6) | CU-11 | RC-04, RN-15 |
| GrupoRegla | GrupoRegla (1.7) | CU-11 | RC-03, RN-15 |
| Evento | Evento (1.8) | CU-02, CU-04, CU-11, CU-14 | RC-05, RC-10, RN-04, RN-05, RN-09 |
| EventoGrupo | EventoGrupo (1.9) | CU-11 | RC-03, RN-15 |
| Accion | Acción (1.10) | CU-02, CU-03, CU-04, CU-11, CU-16 | RC-05, RC-11, RN-02, RN-05, RN-06 |
| Incidente | Incidente (1.11) | CU-02, CU-05, CU-06, CU-07, CU-14 | RC-10, RN-09, RN-11 |
| MensajeAccionado | MensajeAccionado (1.12) | CU-05, CU-06 | RC-01, RN-11 |
| CanalAfectado | CanalAfectado (1.13) | CU-03, CU-05, CU-06 | RC-01, RN-11 |

Nota sobre estado en memoria (no persistido): el estado de conducta (ventanas deslizantes de actividad reciente por usuario) y el estado de antirrebote por usuario no son tablas; viven en memoria, particionados por contexto, y se pierden ante un reinicio (ADR-09, `modelo-conceptual_v1.0.md` nota final, `SOLUTION-INTAKE §17 P.4, P.12`). Alimentan CU-01 y CU-16 sin persistirse.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Modelo lógico inicial con 13 tablas trazadas al modelo conceptual de 02, tipos físicos, índices, restricciones, migración inicial `MIG-0001-esquema-inicial` y declaración no multi-tenant. |
