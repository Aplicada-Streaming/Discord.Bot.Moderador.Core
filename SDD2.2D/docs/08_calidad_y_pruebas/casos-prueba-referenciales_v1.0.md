# Casos de prueba referenciales — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** casos-prueba-referenciales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

Catálogo de casos de prueba TC-XX con numeración contigua de dos dígitos. Cada TC declara tipo, el CU/NFR/RN que cubre, setup, pasos en Given/When/Then derivados de los criterios de aceptación de 02, expected, actual y status. El estado "Pendiente" indica que el TC está especificado pero aún no implementado (no hay código en el repositorio en esta etapa de diseño); se actualiza al cierre de cada rebanada del mini-plan de 07. Los Given/When/Then provienen de los CA-XX de los CU de 02 y de las RN de 02. El tooling por nivel se ancla a `SOLUTION-INTAKE §17 P.6`.

Convención de tipos: Unit, Integration, E2E. El módulo de detección (CU-01, CU-04, CU-16 y las RN de evaluación) concentra los TC unitarios de mayor exigencia de cobertura (≥ 90 %, `estrategia-testing_v1.0.md` §2).

## Núcleo crítico: detección, contención y borrado

### TC-01 — deteccion-rafaga-distribuida-canales-distintos

- Tipo: Unit
- Cubre: CU-01 (CA-01), RN-08, NFR cobertura del módulo de detección
- Setup: evaluador de conducta con una regla de ráfaga de umbral 3 canales distintos y ventana de 2 s; estado de conducta en memoria vacío; reloj inyectable; usuario no exento con snowflake de prueba.
- Pasos: Given una regla de ráfaga con umbral de 3 canales distintos y ventana de 2 s, y un usuario no exento. When el usuario publica en los canales general, anuncios y memes en 1,5 s. Then el servicio marca la condición de ráfaga distribuida cumplida para ese usuario.
- Expected: la condición de ráfaga se marca como cumplida; la cuenta de canales distintos es 3.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-02 — rafaga-mismo-canal-no-dispara

- Tipo: Unit
- Cubre: CU-01 (CA-02)
- Setup: misma regla de ráfaga (umbral 3, ventana 2 s); usuario no exento.
- Pasos: Given la misma regla con umbral 3 canales y ventana 2 s. When el usuario publica 10 mensajes en el canal general en 1,5 s. Then el servicio no marca la condición porque la cuenta de canales distintos es 1.
- Expected: la condición no se marca; la cuenta de canales distintos es 1 (discrimina por canales, no por volumen).
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-03 — rafaga-ventana-ampliada-dispara

- Tipo: Unit
- Cubre: CU-01 (CA-03), RN-10
- Setup: regla de ráfaga con umbral 3 y ventana ampliada a 6 s; reloj inyectable.
- Pasos: Given una regla con umbral 3 canales y ventana ampliada a 6 s. When el usuario publica en 3 canales distintos a lo largo de 5 s. Then el servicio marca la condición de ráfaga distribuida cumplida.
- Expected: la condición se marca; la ventana mayor captura el fan-out más espaciado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-04 — exento-staff-descartado-antes-de-evaluar

- Tipo: Unit
- Cubre: CU-01 (CA-04), CU-15 (CA-02), RN-07
- Setup: pipeline con etapa de descarte de exentos; exención por rol staff vigente; usuario con rol staff.
- Pasos: Given una regla con umbral 3 canales y un usuario incluido en una exención por rol staff. When el usuario publica en 4 canales distintos en 1 s. Then el servicio descarta al emisor antes de evaluar y no marca la condición.
- Expected: el emisor se descarta en la etapa de exentos; ninguna regla se evalúa para él; no hay contención.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-05 — baneo-automatico-emisor-rol-inferior

- Tipo: Integration
- Cubre: CU-02 (CA-01), RN-04, RN-05
- Setup: pipeline ensamblado con doble del adaptador del gateway; política en ejecución real con acción de baneo; persistencia efímera; emisor con rol inferior al bot.
- Pasos: Given una política en ejecución real con acción de baneo y un emisor con rol inferior al bot. When la condición de ráfaga se marca para ese emisor. Then el servicio banea al emisor, registra el incidente y lo marca como accionado.
- Expected: el doble del adaptador recibe la orden de baneo; se persiste un incidente con estado accionado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-06 — baneo-bloqueado-por-jerarquia-superior

- Tipo: Integration
- Cubre: CU-02 (CA-02), RN-01
- Setup: política en ejecución real; emisor administrador con rol superior al bot; doble del adaptador que reporta jerarquía insuficiente; canal de incidencias designado.
- Pasos: Given una política en ejecución real y un emisor que es administrador del servidor con rol superior al bot. When la condición de ráfaga se marca para ese emisor. Then el servicio no banea, registra el incidente con código BANEO_JERARQUIA_INSUFICIENTE y lo reporta al canal de incidencias.
- Expected: no se ejecuta el baneo; el incidente queda no accionable con el código; el pipeline no se detiene (ADR-08).
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-07 — baneo-en-modo-simulacion-no-ejecuta

- Tipo: Unit
- Cubre: CU-02 (CA-03), CU-14 (CA-01), RN-09
- Setup: política en modo simulación con acción de baneo; doble del adaptador con verificación de no invocación.
- Pasos: Given una política en modo simulación con acción de baneo. When la condición de ráfaga se marca para un emisor. Then el servicio no banea y registra el incidente como simulado con la acción que se habría ejecutado.
- Expected: el adaptador no recibe ninguna orden; el incidente se registra como simulado con la acción que se habría ejecutado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-08 — antirrebote-suprime-baneo-repetido

- Tipo: Unit
- Cubre: CU-02 (CA-04), CU-16 (CA-01), RN-06
- Setup: antirrebote por usuario con ventana de supresión vigente; usuario ya baneado en la ráfaga; reloj inyectable.
- Pasos: Given un emisor ya baneado en la ráfaga vigente por la primera coincidencia. When llega un nuevo mensaje del mismo emisor dentro de la misma ráfaga. Then el servicio no repite el baneo por el antirrebote.
- Expected: la acción repetida se suprime; no se genera un incidente adicional.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-09 — borrado-retroactivo-multiples-canales

- Tipo: Integration
- Cubre: CU-03 (CA-01), RN-05
- Setup: acción de baneo con ventana de borrado de 1 día; emisor con mensajes en 5 canales en la última hora; doble del adaptador que registra los canales purgados; copia de mensajes tomada antes de borrar.
- Pasos: Given una acción de baneo con ventana de borrado de 1 día y un emisor con mensajes en 5 canales en la última hora. When se ejecuta la acción sobre el emisor. Then el servicio banea y remueve los mensajes del emisor en los 5 canales dentro del último día, y registra los 5 canales como afectados.
- Expected: el adaptador recibe la purga en los 5 canales; el incidente registra 5 canales afectados; la copia se tomó antes del borrado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-10 — borrado-ventana-fuera-de-rango-se-acota-a-7-dias

- Tipo: Unit
- Cubre: CU-03 (CA-02), RN-02
- Setup: acción configurada con ventana de borrado de 10 días.
- Pasos: Given una acción configurada con ventana de borrado de 10 días. When se ejecuta la acción sobre el emisor. Then el servicio acota la ventana a 7 días, ejecuta el borrado con ese tope y registra el ajuste con código BORRADO_VENTANA_FUERA_DE_RANGO.
- Expected: la ventana efectiva es 7 días; se registra el código de ajuste; la operación no se rechaza, solo se limita.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-11 — borrado-ventana-cero-no-remueve

- Tipo: Unit
- Cubre: CU-03 (CA-03), RN-02
- Setup: acción con ventana de borrado de 0 días.
- Pasos: Given una acción con ventana de borrado de 0 días. When se ejecuta la acción sobre el emisor. Then el servicio banea sin remover mensajes previos y registra el incidente sin canales afectados por borrado.
- Expected: se ejecuta el baneo; no se purga ningún mensaje; el incidente no registra canales afectados por borrado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-12 — borrado-cubre-mensajes-no-evaluados-en-ventana

- Tipo: Integration
- Cubre: CU-03 (CA-04), RN-02
- Setup: emisor con mensajes no evaluados por una caída del canal de eventos, todos dentro de la ventana de borrado; doble del adaptador.
- Pasos: Given un emisor con mensajes que el servicio no llegó a evaluar por una caída del canal de eventos, todos dentro de la ventana de borrado. When se ejecuta la acción sobre el emisor. Then el servicio remueve también esos mensajes por estar dentro de la ventana de borrado.
- Expected: el borrado retroactivo alcanza los mensajes no evaluados dentro de la ventana.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-13 — contenido-regex-dominio-de-estafa-contiene

- Tipo: Unit
- Cubre: CU-04 (CA-01), RN-04, NFR cobertura del módulo de detección
- Setup: regla de contenido con expresión regular que detecta enlaces de un dominio de estafa conocido; política en ejecución real; usuario no exento.
- Pasos: Given una regla de contenido con la expresión regular que detecta enlaces de un dominio de estafa conocido y una política en ejecución real. When un usuario no exento publica un mensaje con un enlace a ese dominio. Then el servicio marca la condición, contiene al emisor y registra el incidente con la copia del mensaje y el canal.
- Expected: coincidencia positiva; contención del emisor; incidente con copia del mensaje y canal.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-14 — contenido-patron-invalido-se-omite-y-continua

- Tipo: Unit
- Cubre: CU-04 (CA-03), RN-03
- Setup: una regla de contenido cuyo patrón no compila y otras reglas válidas en el conjunto.
- Pasos: Given una regla de contenido cuyo patrón no compila. When llega un mensaje cualquiera. Then el servicio omite esa regla, registra el patrón inválido con código CONTENIDO_PATRON_INVALIDO y evalúa las demás reglas.
- Expected: la regla inválida se omite con el código; las demás reglas se evalúan; el pipeline no se interrumpe.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-15 — contenido-regex-con-tope-de-tiempo

- Tipo: Unit
- Cubre: CU-04 (CA-01), RN-03, riesgo de regex costosa (arquitectura §9)
- Setup: regla de contenido con un patrón susceptible de retroceso catastrófico evaluado contra una entrada adversa; tope de tiempo de evaluación configurado.
- Pasos: Given una regla de contenido cuyo patrón puede provocar retroceso catastrófico. When se evalúa contra una entrada adversa que excede el tope de tiempo. Then el servicio aborta la evaluación de esa regla por el tope de tiempo sin colgar el pipeline y registra la condición.
- Expected: la evaluación se acota por el tope de tiempo; el pipeline continúa; no hay consumo ilimitado de CPU.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-16 — contenido-exento-no-contiene

- Tipo: Unit
- Cubre: CU-04 (CA-02), RN-07
- Setup: regla de contenido por palabra clave vedada; usuario en una exención por usuario de confianza.
- Pasos: Given una regla de contenido por palabra clave vedada y un usuario incluido en una exención por usuario de confianza. When el usuario exento publica un mensaje con esa palabra. Then el servicio descarta al emisor antes de evaluar y no contiene.
- Expected: descarte previo del exento; sin contención.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-17 — politicas-por-prioridad-primera-coincidencia

- Tipo: Unit
- Cubre: CU-11 (CA-01 de evaluación), RN-04
- Setup: dos políticas con distinta prioridad que coinciden sobre el mismo mensaje; bandera continuar desactivada en la de mayor prioridad.
- Pasos: Given dos políticas que coinciden sobre el mismo mensaje, evaluadas por prioridad, con la bandera continuar desactivada. When llega el mensaje. Then se dispara solo la política de mayor prioridad y la evaluación se detiene en la primera coincidencia.
- Expected: solo un evento disparado; la segunda política no se evalúa.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-18 — politicas-bandera-continuar-dispara-segunda

- Tipo: Unit
- Cubre: RN-04
- Setup: dos políticas que coinciden; bandera continuar activada en la de mayor prioridad.
- Pasos: Given dos políticas que coinciden, con la bandera continuar activada en la de mayor prioridad. When llega el mensaje. Then ambas políticas disparan su evento.
- Expected: dos eventos disparados, en orden de prioridad.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-19 — orden-de-acciones-y-copia-antes-de-borrar

- Tipo: Unit
- Cubre: CU-02/CU-03 (ejecución), RN-05, RN-11
- Setup: evento con acciones reportar y banear-con-borrado en orden configurado; doble del adaptador que registra el orden de invocación.
- Pasos: Given un evento con varias acciones en el orden configurado por el administrador. When se ejecuta el evento. Then las acciones se ejecutan en ese orden y la copia de los mensajes se toma antes de cualquier borrado.
- Expected: el orden de invocación coincide con el configurado; la copia precede al borrado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-20 — antirrebote-ventana-expirada-permite-nueva-accion

- Tipo: Unit
- Cubre: CU-16 (CA-02), RN-06
- Setup: usuario accionado cuya ventana de antirrebote ya expiró; reloj inyectable.
- Pasos: Given un usuario accionado cuya ventana de antirrebote expiró. When el usuario vuelve a disparar una regla. Then el servicio trata el caso como una nueva acción permitida y la ejecuta.
- Expected: la acción se ejecuta de nuevo tras expirar la ventana.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-21 — antirrebote-ventana-invalida-usa-default

- Tipo: Unit
- Cubre: CU-16 (CA-03), RN-10
- Setup: ventana de antirrebote configurada fuera de los límites de su descriptor.
- Pasos: Given una ventana de antirrebote configurada fuera de los límites de su descriptor. When se evalúa una acción. Then el servicio aplica el valor por defecto con código ANTIRREBOTE_VENTANA_INVALIDA.
- Expected: se usa el default del descriptor; se registra el código.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Modo simulación, exenciones y reporte

### TC-22 — simulacion-publica-reporte-etiquetado

- Tipo: Integration
- Cubre: CU-14 (CA-02), CU-05 (CA-02), RN-09
- Setup: política en modo simulación con canal de salida designado; doble del adaptador.
- Pasos: Given una política en modo simulación con canal de salida designado. When se registra un incidente simulado. Then el servicio publica el reporte etiquetado como simulación en el canal de salida.
- Expected: el reporte se publica con la etiqueta de simulación; no se ejecuta acción real.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-23 — promocion-a-ejecucion-real-ejecuta

- Tipo: Integration
- Cubre: CU-14 (CA-03), RN-09
- Setup: política recién promovida de simulación a ejecución real; doble del adaptador.
- Pasos: Given una política recién promovida a ejecución real. When un usuario cumple las condiciones después de la promoción. Then el servicio ejecuta la acción real sobre el usuario en lugar de solo registrarla.
- Expected: el adaptador recibe la acción real tras la promoción.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-24 — modo-indefinido-aplica-simulacion-segura

- Tipo: Unit
- Cubre: CU-14 (CA-04), RN-09
- Setup: política cuyo modo quedó indefinido.
- Pasos: Given una política cuyo modo quedó indefinido. When se cumplen sus condiciones. Then el servicio aplica el modo seguro de simulación con código SIMULACION_MODO_INCONSISTENTE y no ejecuta acción real.
- Expected: se asume simulación segura; no hay acción real; se registra el código.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-25 — reporte-incidente-real-con-canales-afectados

- Tipo: Integration
- Cubre: CU-05 (CA-01), RN-11
- Setup: canal de salida privado designado; política en ejecución real que baneó a un emisor en 4 canales; doble del adaptador.
- Pasos: Given un canal de salida privado designado y una política en ejecución real que baneó a un emisor en 4 canales. When se registra el incidente. Then el servicio publica en el canal de salida un reporte con el emisor, los mensajes accionados y los 4 canales afectados.
- Expected: el reporte incluye emisor, mensajes accionados y 4 canales afectados.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-26 — reporte-sin-canal-designado-conserva-incidente

- Tipo: Unit
- Cubre: CU-05 (CA-03), RN-11
- Setup: servidor sin canal de salida designado.
- Pasos: Given un servidor sin canal de salida designado. When se dispara una política. Then el servicio no envía reporte, conserva el incidente en el panel y registra código REPORTE_CANAL_NO_DESIGNADO.
- Expected: sin envío de reporte; el incidente queda disponible en el panel con el código.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-27 — exencion-por-rol-persistida-y-aplicada

- Tipo: Integration
- Cubre: CU-15 (CA-01), RN-07, RN-12
- Setup: administrador autenticado; rol staff a eximir; persistencia efímera.
- Pasos: Given un administrador autenticado y un rol staff a eximir. When el administrador crea una exención por ese rol. Then el servicio persiste la exención y los usuarios con ese rol quedan fuera de la moderación.
- Expected: la exención se persiste; los usuarios con el rol no son evaluados.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-28 — exencion-identificador-invalido-rechazada

- Tipo: Unit
- Cubre: CU-15 (CA-03), RN-08
- Setup: alta de exención con un identificador que no es un snowflake válido.
- Pasos: Given un administrador agregando una exención. When el administrador ingresa un identificador que no es un snowflake válido. Then el servicio rechaza la exención con código EXENCION_IDENTIFICADOR_INVALIDO.
- Expected: rechazo con el código; no se persiste la exención.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-29 — canal-de-confianza-excluido

- Tipo: Unit
- Cubre: CU-15 (CA-04), RN-07
- Setup: canal declarado de confianza; usuario que publica contenido que cumpliría una regla.
- Pasos: Given un canal declarado de confianza. When un usuario publica contenido que cumpliría una regla en ese canal. Then el servicio excluye la actividad de ese canal de la evaluación y no contiene.
- Expected: la actividad del canal de confianza se excluye; sin contención.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Autenticación, registro de servidor y seguridad

### TC-30 — alta-administrador-primer-ingreso-hash

- Tipo: Unit
- Cubre: CU-08 (CA-01), RN-13
- Setup: sistema recién instalado sin cuenta de administrador.
- Pasos: Given un sistema recién instalado sin cuenta de administrador. When el administrador ingresa un identificador y una contraseña robusta con su confirmación coincidente. Then el servicio crea la cuenta única, resguarda la contraseña con hash y completa el primer ingreso.
- Expected: una sola cuenta creada; la contraseña queda almacenada como hash en formato PHC, nunca en claro.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-31 — alta-contrasena-debil-rechazada

- Tipo: Unit
- Cubre: CU-08 (CA-02), RN-13
- Setup: sistema en first-run.
- Pasos: Given un sistema en first-run. When el administrador ingresa una contraseña que no cumple la política mínima de robustez. Then el servicio rechaza el alta con código SETUP_CONTRASENA_DEBIL y solicita otra contraseña.
- Expected: rechazo con el código; no se crea la cuenta.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-32 — segundo-alta-bloqueada-cuenta-unica

- Tipo: Unit
- Cubre: CU-08 (CA-03), RN-13
- Setup: sistema con una cuenta de administrador ya creada.
- Pasos: Given un sistema con una cuenta de administrador ya creada. When alguien intenta acceder al alta de primer ingreso. Then el servicio bloquea el alta con código SETUP_YA_COMPLETADO y redirige a la autenticación.
- Expected: alta bloqueada; a lo sumo una cuenta; redirección a autenticación.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-33 — autenticacion-credenciales-validas

- Tipo: Unit
- Cubre: CU-09 (CA-01), RN-13
- Setup: cuenta de administrador existente; credenciales correctas.
- Pasos: Given una cuenta de administrador existente y credenciales correctas. When el administrador ingresa identificador y contraseña válidos. Then el servicio abre una sesión autorizada y lo dirige al panel.
- Expected: sesión autorizada abierta; verificación de hash exitosa sin comparar en claro.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-34 — autenticacion-credenciales-invalidas

- Tipo: Unit
- Cubre: CU-09 (CA-02), RN-13
- Setup: cuenta de administrador existente; contraseña incorrecta.
- Pasos: Given una cuenta de administrador existente. When el administrador ingresa una contraseña incorrecta. Then el servicio no abre sesión, informa credenciales inválidas con código AUTH_CREDENCIALES_INVALIDAS y registra el intento.
- Expected: sin sesión; código de credenciales inválidas; intento registrado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-35 — registro-servidor-token-cifrado-en-reposo

- Tipo: Integration
- Cubre: CU-10 (CA-01), RN-14, RN-12
- Setup: administrador autenticado; servidor no registrado; clave maestra de prueba en variable de entorno; persistencia efímera.
- Pasos: Given un administrador autenticado y un servidor no registrado. When el administrador registra el servidor con un identificador válido y un token de bot. Then el servicio persiste el servidor con el token cifrado y lo deja inactivo, y ofrece la prueba de configuración.
- Expected: el servidor se persiste inactivo; el token en la base está cifrado (nunca en claro); se ofrece la prueba.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-36 — registro-servidor-duplicado-rechazado

- Tipo: Unit
- Cubre: CU-10 (CA-02), RN-08
- Setup: servidor ya registrado con un identificador dado.
- Pasos: Given un servidor ya registrado con un identificador dado. When el administrador intenta registrar otro servidor con el mismo identificador. Then el servicio rechaza el duplicado con código SERVIDOR_YA_REGISTRADO y ofrece editar el existente.
- Expected: rechazo del duplicado con el código.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-37 — registro-servidor-token-vacio-rechazado

- Tipo: Unit
- Cubre: CU-10 (CA-03), RN-14
- Setup: alta de servidor con el campo de token vacío.
- Pasos: Given un administrador registrando un servidor. When el administrador deja vacío el campo de token. Then el servicio rechaza el registro con código SERVIDOR_TOKEN_VACIO.
- Expected: rechazo con el código; no se persiste el servidor.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-38 — cifrado-descifrado-token-ida-y-vuelta

- Tipo: Unit
- Cubre: RN-14, NFR seguridad
- Setup: servicio de cifrado con clave maestra de prueba.
- Pasos: Given el servicio de cifrado con su clave maestra. When se cifra un token y luego se descifra. Then el resultado coincide con el token original y el valor en reposo difiere del token en claro.
- Expected: ida y vuelta correcta; el cifrado en reposo no es legible como el token original.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Configuración por descriptores

### TC-39 — config-valor-dentro-de-limites-aceptado

- Tipo: Unit
- Cubre: CU-11 (CA-01), RN-10
- Setup: parámetro de umbral de canales distintos con default 3 y límites de 2 a 10.
- Pasos: Given un parámetro de umbral de canales distintos con valor por defecto 3 y límites de 2 a 10. When el administrador ingresa el valor 4. Then el servicio acepta y persiste el valor 4 mostrando la leyenda y el ejemplo del parámetro.
- Expected: el valor 4 se persiste; se muestran leyenda y ejemplo derivados del descriptor.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-40 — config-valor-fuera-de-limite-rechazado

- Tipo: Unit
- Cubre: CU-11 (CA-02), RN-10
- Setup: el mismo parámetro con límites de 2 a 10.
- Pasos: Given el mismo parámetro con límites de 2 a 10. When el administrador ingresa el valor 1. Then el servicio rechaza el valor con código CONFIG_VALOR_FUERA_DE_LIMITE y muestra los límites permitidos.
- Expected: rechazo con el código; se muestran los límites permitidos.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-41 — config-grupo-sin-reglas-rechazado

- Tipo: Unit
- Cubre: CU-11 (CA-03), RN-15
- Setup: alta de un grupo de reglas sin ninguna regla asociada.
- Pasos: Given un administrador definiendo un grupo de reglas. When el administrador intenta guardar el grupo sin asociar ninguna regla. Then el servicio rechaza el grupo con código CONFIG_GRUPO_SIN_REGLAS.
- Expected: rechazo con el código; el grupo no se persiste.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-42 — config-eliminar-grupo-referenciado-bloqueado

- Tipo: Integration
- Cubre: CU-11 (CA-04), RN-15
- Setup: un evento que referencia un grupo de reglas; persistencia efímera.
- Pasos: Given un evento que referencia un grupo de reglas. When el administrador intenta eliminar ese grupo. Then el servicio bloquea la eliminación con código CONFIG_REFERENCIA_REQUERIDA e indica el evento que lo usa.
- Expected: eliminación bloqueada con el código; se indica el evento dependiente.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Prueba de configuración, reconexión y panel

### TC-43 — prueba-configuracion-todo-superado-habilita

- Tipo: Integration
- Cubre: CU-12 (CA-01), RN-16
- Setup: servidor registrado con token válido, permisos completos y canal de salida accesible; doble del adaptador que reporta verificaciones superadas.
- Pasos: Given un servidor registrado con token válido, permisos completos y canal de salida accesible. When el administrador ejecuta la prueba de configuración. Then el servicio reporta todas las verificaciones superadas y habilita la activación.
- Expected: todas las verificaciones superadas; la activación queda habilitada.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-44 — prueba-configuracion-permiso-faltante-bloquea

- Tipo: Integration
- Cubre: CU-12 (CA-02), RN-16, RN-01
- Setup: servidor con token válido pero sin permiso de baneo; doble del adaptador.
- Pasos: Given un servidor con token válido pero sin permiso de baneo. When el administrador ejecuta la prueba. Then el servicio reporta el permiso faltante con código PRUEBA_PERMISOS_FALTANTES y bloquea la activación.
- Expected: permiso faltante reportado con el código; activación bloqueada.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-45 — prueba-configuracion-token-invalido-bloquea

- Tipo: Integration
- Cubre: CU-12 (CA-03), RN-14, RN-16
- Setup: servidor cuyo token fue revocado; doble del adaptador que reporta token inválido.
- Pasos: Given un servidor cuyo token fue revocado. When el administrador ejecuta la prueba. Then el servicio marca la prueba fallida con código PRUEBA_TOKEN_INVALIDO, bloquea la activación y deja el servidor desconectado.
- Expected: prueba fallida con el código; activación bloqueada; servidor desconectado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-46 — prueba-configuracion-jerarquia-advertencia-permite-activar

- Tipo: Integration
- Cubre: CU-12 (CA-04), RN-16, RN-01
- Setup: servidor con token y permisos correctos pero con roles de staff por encima del bot; doble del adaptador.
- Pasos: Given un servidor con token y permisos correctos pero con roles de staff por encima del bot. When el administrador ejecuta la prueba. Then el servicio advierte de la jerarquía que impide actuar sobre esos roles y permite activar dejando la advertencia visible.
- Expected: advertencia no bloqueante de jerarquía; la activación se permite con la advertencia visible.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-47 — reconexion-automatica-tras-caida-transitoria

- Tipo: Integration
- Cubre: CU-13 (CA-01), RN-16
- Setup: servidor activo conectado; doble del adaptador que simula una caída transitoria y una reconexión.
- Pasos: Given un servidor activo conectado al canal de eventos. When la conexión cae de forma transitoria. Then el servicio marca el servidor como desconectado, reconecta automáticamente y vuelve a marcarlo como conectado.
- Expected: transición conectado → desconectado → conectado sin intervención manual.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-48 — reconexion-token-revocado-marca-desconectado

- Tipo: Integration
- Cubre: CU-13 (CA-03), RN-14, RN-16
- Setup: servidor cuyo token se revoca durante la operación; doble del adaptador que falla la reconexión por credencial inválida.
- Pasos: Given un servidor cuyo token se revoca durante la operación. When la reconexión falla por credencial inválida. Then el servicio marca el servidor como desconectado por token inválido con código CONEXION_TOKEN_INVALIDO y solicita re-validación.
- Expected: estado desconectado por token inválido con el código; se solicita re-validación; no hay reintento ciego infinito.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-49 — panel-lista-incidentes-autenticado

- Tipo: E2E
- Cubre: CU-06 (CA-01), RN-12
- Setup: instancia local del panel con un administrador autenticado y 3 incidentes registrados; datos sintéticos.
- Pasos: Given un administrador autenticado y 3 incidentes registrados. When el administrador abre la sección de incidentes. Then el servicio lista los 3 incidentes con emisor, tipo de regla, acción y modo.
- Expected: los 3 incidentes se listan con sus atributos.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-50 — panel-detalle-incidente-real-ofrece-revertir

- Tipo: E2E
- Cubre: CU-06 (CA-02), RN-11
- Setup: incidente de baneo en ejecución real con copia de 6 mensajes en 4 canales.
- Pasos: Given un incidente de baneo en ejecución real con copia de 6 mensajes en 4 canales. When el administrador abre el detalle del incidente. Then el servicio muestra los 6 mensajes copiados y los 4 canales afectados, y ofrece la opción de revertir.
- Expected: detalle con 6 mensajes y 4 canales; opción de revertir presente.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-51 — panel-incidentes-sin-sesion-redirige

- Tipo: E2E
- Cubre: CU-06 (CA-04), RN-12
- Setup: sesión de administrador expirada.
- Pasos: Given una sesión de administrador expirada. When el administrador intenta abrir la sección de incidentes. Then el servicio redirige a la autenticación con código REVISION_SIN_AUTENTICACION y no muestra incidentes.
- Expected: redirección a autenticación con el código; sin incidentes mostrados.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-52 — desbaneo-falso-positivo-desde-panel

- Tipo: Integration
- Cubre: CU-07 (CA-01), RN-12, RN-01
- Setup: administrador autenticado; incidente con baneo real de un usuario aún baneado; doble del adaptador.
- Pasos: Given un administrador autenticado y un incidente con baneo real de un usuario que sigue baneado. When el administrador confirma la reversión. Then el servicio desbanea al usuario, registra la reversión con autor y fecha e informa que los mensajes no se restauran.
- Expected: desbaneo ejecutado; reversión registrada con autor y fecha; aviso de no restauración de mensajes.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-53 — desbaneo-sin-permiso-bloqueado

- Tipo: Integration
- Cubre: CU-07 (CA-04), RN-01
- Setup: bot sin permiso para revertir baneos; doble del adaptador que reporta permiso faltante.
- Pasos: Given un bot sin permiso para revertir baneos en el servidor. When el administrador confirma la reversión. Then el servicio no desbanea, informa el permiso faltante con código DESBANEO_SIN_PERMISO y registra el intento.
- Expected: sin desbaneo; código de permiso faltante; intento registrado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-54 — simulacion-no-ofrece-revertir

- Tipo: E2E
- Cubre: CU-07 (CA-02), CU-06 (CA-03), RN-09
- Setup: incidente en modo simulación.
- Pasos: Given un incidente en modo simulación. When el administrador abre su detalle. Then el servicio muestra la acción que se habría ejecutado y no ofrece reversión.
- Expected: detalle de simulación sin opción de revertir.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Mediciones de NFR

### TC-55 — latencia-pipeline-p95

- Tipo: Integration (medición de desempeño)
- Cubre: NFR latencia de procesamiento por mensaje
- Setup: pipeline instrumentado con marca de tiempo de entrada a salida de decisión; banco de mensajes simulados; medición sobre el hardware de referencia o ambiente equivalente.
- Pasos: Given el pipeline instrumentado y un banco de mensajes simulados. When se procesa el banco. Then el percentil 95 de latencia entrada-a-decisión es inferior a 200 ms.
- Expected: p95 < 200 ms.
- Actual: pendiente de ejecución (a confirmar por benchmark en hardware real).
- Status: Pendiente.

### TC-56 — throughput-sostenido

- Tipo: Integration (medición de desempeño)
- Cubre: NFR throughput sostenido
- Setup: banco de pruebas de carga con mensajes simulados sobre el hardware de referencia o ambiente equivalente.
- Pasos: Given el banco de carga. When se sostiene la inyección de mensajes. Then el servicio sostiene al menos 50 mensajes por segundo.
- Expected: ≥ 50 mensajes/s.
- Actual: pendiente de ejecución (a confirmar por benchmark en hardware real).
- Status: Pendiente.

### TC-57 — memoria-por-conexion-de-gateway

- Tipo: Integration (medición de desempeño)
- Cubre: NFR memoria por conexión de gateway activa
- Setup: perfilado de la huella de memoria por contexto activo en el dispositivo o ambiente equivalente.
- Pasos: Given una conexión de gateway activa por contexto. When se perfila en carga. Then la huella por conexión no supera 8 MB.
- Expected: ≤ 8 MB por conexión.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-58 — limpieza-efectiva-dentro-de-10s

- Tipo: Integration
- Cubre: NFR limpieza efectiva de la ráfaga
- Setup: incidente de ráfaga con mensajes del emisor en varios canales; doble del adaptador que registra el tiempo y la cantidad de mensajes purgados.
- Pasos: Given un incidente de ráfaga con mensajes del emisor en varios canales. When se ejecuta el baneo con borrado retroactivo. Then al menos el 98 % de los mensajes de la ráfaga se eliminan dentro de los 10 s del incidente.
- Expected: ≥ 98 % de mensajes eliminados dentro de 10 s.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Cierre de criterios de aceptación por CU

Casos de prueba que completan la cobertura de un criterio de aceptación por cada CA-01..CA-04 de los 16 CU de 02, agregados al cerrar el hallazgo H-01 del audit de Fase E. Conservan la numeración contigua a partir de TC-58.

### TC-59 — contenido-en-simulacion-registra-simulado

- Tipo: Unit
- Cubre: CU-04 (CA-04), RN-09
- Setup: regla de contenido cuya política está en modo simulación; usuario no exento; doble del adaptador con verificación de no invocación.
- Pasos: Given una regla de contenido con su política en modo simulación. When un usuario publica un mensaje que cumple el criterio. Then el servicio no contiene y registra el incidente como simulado con la acción que se habría ejecutado.
- Expected: el adaptador no recibe ninguna orden de contención; el incidente queda registrado como simulado con la acción que se habría ejecutado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-60 — reporte-incidente-no-accionable-incluye-advertencia-jerarquia

- Tipo: Integration
- Cubre: CU-05 (CA-04), RN-01, RN-11
- Setup: canal de salida designado; incidente no accionable por jerarquía superior del emisor o permisos faltantes (RN-01); doble del adaptador.
- Pasos: Given un incidente no accionable por jerarquía superior del emisor. When se compone el reporte. Then el reporte incluye la advertencia de jerarquía o permisos faltantes.
- Expected: el reporte publicado contiene la advertencia de jerarquía o permisos faltantes y deja constancia de que la acción no se ejecutó.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-61 — panel-detalle-simulacion-no-ofrece-revertir

- Tipo: E2E
- Cubre: CU-06 (CA-03), RN-09
- Setup: instancia local del panel con un administrador autenticado y un incidente en modo simulación.
- Pasos: Given un incidente en modo simulación. When el administrador abre su detalle. Then el servicio muestra la acción que se habría ejecutado y no ofrece reversión.
- Expected: el detalle indica que la acción no se ejecutó y muestra lo que se habría hecho; no aparece la opción de revertir.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-62 — desbaneo-usuario-ya-desbaneado-marca-revertido

- Tipo: Integration
- Cubre: CU-07 (CA-03), RN-12
- Setup: administrador autenticado; incidente con baneo real cuyo usuario ya fue desbaneado por otra vía; doble del adaptador que reporta que el usuario no está baneado.
- Pasos: Given un incidente con baneo real cuyo usuario ya fue desbaneado por otra vía. When el administrador solicita revertir. Then el servicio informa que el usuario ya no está baneado y marca el incidente como revertido.
- Expected: no se ejecuta un nuevo desbaneo; el incidente queda marcado como revertido; se informa que el usuario ya no estaba baneado.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-63 — alta-confirmacion-no-coincide-rechazada

- Tipo: Unit
- Cubre: CU-08 (CA-04), RN-13
- Setup: sistema en first-run sin cuenta de administrador.
- Pasos: Given un sistema en first-run. When el administrador ingresa una contraseña cuya confirmación no coincide. Then el servicio rechaza el alta e indica la diferencia, sin crear la cuenta.
- Expected: rechazo del alta con indicación de la diferencia; no se crea ninguna cuenta; el sistema permanece en first-run.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-64 — sesion-vencida-exige-reautenticar

- Tipo: Unit
- Cubre: CU-09 (CA-03), RN-12
- Setup: sesión de administrador activa cuyo tiempo de vigencia ya se superó; reloj inyectable.
- Pasos: Given una sesión activa que superó su tiempo de vigencia. When el administrador intenta una acción protegida. Then el servicio invalida la sesión y exige autenticarse de nuevo.
- Expected: la sesión vencida queda invalidada; la acción protegida no se ejecuta; se solicita nueva autenticación.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-65 — autenticacion-sin-cuenta-redirige-a-primer-ingreso

- Tipo: Unit
- Cubre: CU-09 (CA-04), RN-13
- Setup: sistema sin cuenta de administrador creada (first-run no completado).
- Pasos: Given un sistema sin cuenta de administrador creada. When el administrador intenta autenticarse. Then el servicio redirige al primer ingreso con código AUTH_SIN_CUENTA.
- Expected: redirección al alta de primer ingreso (CU-08) con código AUTH_SIN_CUENTA; no se intenta verificar credenciales.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-66 — registro-servidor-token-revocado-se-cifra-y-reemplaza

- Tipo: Integration
- Cubre: CU-10 (CA-04), RN-14
- Setup: servidor existente con token revocado; clave maestra de prueba en variable de entorno; persistencia efímera; resto de la configuración del servidor cargada.
- Pasos: Given un servidor existente con token revocado. When el administrador ingresa un token nuevo. Then el servicio cifra y reemplaza el token, conserva la configuración y vuelve a ofrecer la prueba de configuración.
- Expected: el nuevo token queda cifrado en reposo (nunca en claro) reemplazando al anterior; la configuración del servidor se conserva; se ofrece la prueba de configuración.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-67 — estado-conexion-se-refresca-dentro-del-tiempo-objetivo

- Tipo: Integration
- Cubre: CU-13 (CA-02), RN-16
- Setup: servidor que transita de conectado a desconectado; doble del adaptador que emite el cambio de estado; reloj inyectable para medir el refresco.
- Pasos: Given un servidor que pasa de conectado a desconectado. When cambia el estado de conexión. Then el panel refleja el nuevo estado dentro del tiempo objetivo de refresco.
- Expected: el estado visible se actualiza al nuevo valor dentro del tiempo objetivo de refresco.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-68 — caida-prolongada-mantiene-desconectado-y-reintenta

- Tipo: Integration
- Cubre: CU-13 (CA-04), RN-16
- Setup: servidor que sufre una caída prolongada; doble del adaptador que falla los reintentos de reconexión; política de reintento configurada.
- Pasos: Given un servidor que sufre una caída prolongada. When los reintentos no logran reconectar. Then el servicio mantiene el estado desconectado visible y continúa reintentando.
- Expected: el servidor permanece como desconectado con el estado visible para el administrador; el servicio sigue reintentando según su política sin detenerse.
- Actual: pendiente de ejecución.
- Status: Pendiente.

### TC-69 — antirrebote-primera-accion-se-ejecuta-y-marca

- Tipo: Unit
- Cubre: CU-16 (CA-04), RN-06
- Setup: política en ejecución real; usuario nunca accionado en la ráfaga vigente; ventana de antirrebote configurada; reloj inyectable.
- Pasos: Given un usuario nunca accionado en la ráfaga vigente. When el usuario dispara la regla por primera vez. Then el servicio ejecuta la acción y marca al usuario como accionado con su marca de tiempo.
- Expected: la acción se ejecuta una vez; el usuario queda marcado como accionado con su marca de tiempo dentro de la ventana de antirrebote.
- Actual: pendiente de ejecución.
- Status: Pendiente.

## Resumen del catálogo

- Total de casos de prueba: 69 (TC-01..TC-69).
- Distribución por nivel: 38 Unit, 24 Integration, 7 E2E. Esta distribución refleja la pirámide 70/20/10 con predominio unitario sobre el módulo de detección.
- Cobertura de CU críticos: el núcleo (CU-01, CU-02, CU-03, CU-04, CU-14, CU-15, CU-16) tiene varios TC cada uno; los 16 CU tienen al menos un TC (ver `matriz-cobertura-pruebas_v1.0.md` §2).
- Disponibilidad de NFR: los seis NFR numéricos tienen su TC de medición (TC-55..TC-58) o medición observada (cobertura como gate del pipeline, ver matriz §3).
- Política de regresión: todo bug cerrado agrega un TC nuevo o extiende uno existente; la numeración continúa a partir de TC-69.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Catálogo inicial de 58 casos de prueba referenciales para `discord-bots-admin`, derivados de los criterios Given/When/Then de los 16 CU de 02, de las 16 RN y de los NFR de 05 §8, con énfasis en el núcleo crítico (detección de ráfaga, baneo con borrado, contenido por regex con tope de tiempo, simulación, antirrebote, exenciones y prueba de configuración). |
| 1.0 | 2026-06-20 | Corrección de los hallazgos P1 H-01 (11 CA sin TC) y H-02 (mapeo de TC-15 como excepción y anclaje de TC-17/18/19/23/38) del audit de Fase E. Se agregan TC-59 a TC-69 para cubrir el CA-04 de CU-04, el CA-04 de CU-05, el CA-03 de CU-06, el CA-03 de CU-07, el CA-04 de CU-08, los CA-03 y CA-04 de CU-09, el CA-04 de CU-10, los CA-02 y CA-04 de CU-13 y el CA-04 de CU-16; total 69 TC (38 Unit, 24 Integration, 7 E2E). |
