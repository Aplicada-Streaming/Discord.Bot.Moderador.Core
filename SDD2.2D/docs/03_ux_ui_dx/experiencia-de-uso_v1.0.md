# Experiencia de uso — Panel de administración de moderación

**Proyecto:** discord-bots-admin
**Documento:** experiencia-de-uso_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Audiencia y contexto de uso

Persona objetivo (de 00, §2): el administrador del sistema, rol único de la aplicación. Es el operador y, a la vez, el dueño del problema. Opera el panel desde un escritorio, en sesiones cortas y dispersas a lo largo del día, intercaladas con la atención del incidente real: registra servidores, ajusta la sensibilidad de la moderación, revisa los reportes que llegan y revierte falsos positivos. No es un developer ni un equipo: es una sola persona que necesita entender qué ajusta y con qué efecto, sin conocimiento técnico profundo del motor de moderación.

| Atributo del contexto | Descripción |
| --- | --- |
| Persona primaria | Administrador del sistema (operador único), dueño del problema de moderación |
| Frecuencia de uso | Diaria en ráfagas cortas; picos reactivos cuando llega un reporte de incidente |
| Duración típica de sesión | Minutos por tarea (revisar un incidente, ajustar un umbral, registrar un servidor) |
| Contexto físico | Escritorio con monitor amplio; sin requisito táctil; sin movilidad obligatoria |
| Contexto emocional | Tensión cuando hay un ataque en curso; calma al calibrar en modo simulación |
| Concurrencia | Sin concurrencia entre operadores (hay un único administrador). La concurrencia real es entre el motor de moderación y el panel sobre la misma persistencia, resuelta fuera de la presentación |
| Conocimiento previo | Maneja la plataforma de mensajería como usuario; no domina expresiones regulares ni el motor de reglas |

Consecuencia de diseño: el panel privilegia la comprensión sobre la densidad. Cada parámetro trae su leyenda y sus ejemplos; cada acción peligrosa pide confirmación; el estado de protección de cada servidor está siempre a la vista para que una pérdida de cobertura sea detectable de inmediato.

## 2. Principios de diseño

Heurísticas de Nielsen aplicadas (catálogo de diseño, marco común de la categoría):

| Heurística de Nielsen | Aplicación en el producto | Verificación |
| --- | --- | --- |
| Visibilidad del estado del sistema | Estado de conexión de cada servidor visible en el panel de estado; indicador de modo simulación en la cabecera de configuración; stepper en el registro y prueba de servidor | Inspección heurística por dos revisores sobre el panel de estado y la cabecera de configuración |
| Correspondencia entre el sistema y el mundo real | Vocabulario del dominio del administrador (servidor, regla, evento, exención, incidente, desbaneo), nunca términos del motor | Revisión de microcopy contra el glosario de 02 y el glosario UX |
| Control y libertad del usuario | Modo simulación como red de seguridad; cancelación disponible en toda confirmación; desbaneo para revertir un baneo | Recorrido de los flujos de configuración y de desbaneo |
| Consistencia y estándares (ley de Jakob) | Mismos patrones del catálogo en todas las superficies: shell con barra lateral, ABM de listado, formulario dirigido por descriptor, badge de estado | Auditoría de patrones por nombre contra el catálogo |
| Prevención de errores | Validación inline contra los límites del descriptor; bloqueo de activación de un servidor que no superó la prueba; previsualización antes de aplicar configuración | Pruebas de validación previstas en 08 |
| Reconocer antes que recordar | Leyenda y ejemplos colgados de cada parámetro; explicación en palabras de la configuración actual; previsualización con alcance afectado | Revisión de la ayuda contextual por superficie |
| Flexibilidad y eficiencia | Presets de configuración que precargan un conjunto coherente de valores; filtros en el listado de incidentes | Recorrido de la superficie de configuración |
| Estética y diseño minimalista | Jerarquía visual explícita, espacio en blanco sobre compresión, sin adorno; color de marca solo para acción y jerarquía | Inspección heurística |
| Ayudar a reconocer, diagnosticar y recuperarse de errores | Mensajes que dicen qué pasó, por qué y qué hacer; los faltantes de la prueba de configuración se enumeran con la acción de corrección | Catálogo de estados de error por superficie (§8) |
| Ayuda y documentación | Ayuda contextual por campo derivada del descriptor; sin necesidad de manual externo para configurar | Revisión de cobertura de ayuda por parámetro |

Leyes UX relevantes:

- Ley de Hick: la configuración separa lo común de lo avanzado con divulgación progresiva, para acotar las opciones simultáneas y acelerar la decisión del operador no técnico.
- Ley de Fitts: las acciones primarias y destructivas tienen área cómoda y posición consistente (primaria a la derecha del pie del formulario, destructiva diferenciada).
- Ley de Miller: no más de cinco a siete ítems de primer nivel por agrupación; la moderación se agrupa por servidor y por tipo de elemento (reglas, grupos, eventos, acciones, exenciones).
- Ley de Jakob: el panel se comporta como un panel de administración convencional con barra lateral y listados, para que el operador reutilice expectativas previas.

## 3. Flujos clave

### 3.1 Primer ingreso y autenticación (CU-08, CU-09)

Disparador: el administrador accede al sistema recién instalado. Pasos: el sistema detecta que no hay cuenta y presenta el alta de credenciales; el administrador define identificador y contraseña con confirmación; el sistema valida la robustez y crea la cuenta única; deriva a la autenticación; el administrador inicia sesión y entra al panel. Fricción anticipada: contraseña débil o confirmación que no coincide, resuelta con validación inline y mensaje accionable. Salida: sesión abierta en el panel de estado.

### 3.2 Registro de un servidor y prueba de configuración (CU-10, CU-12)

Disparador: el administrador quiere moderar un servidor nuevo. Pasos: registra el servidor con su identificador y su token de bot, opcionalmente designa un canal de salida; el sistema cifra el token y deja el servidor registrado pero inactivo; el administrador ejecuta la prueba de configuración, que verifica credencial, permisos, jerarquía de roles, recepción de eventos y existencia de canales; según el resultado, la activación se habilita o queda bloqueada con los faltantes enumerados. Fricción anticipada: permisos faltantes o jerarquía de roles por encima del bot; el resultado de la prueba los distingue entre bloqueantes y advertencias no bloqueantes. Salida: servidor activo y protegido, o servidor inactivo con la lista de correcciones.

### 3.3 Configuración de la moderación dirigida por descriptores (CU-11, CU-14, CU-15, CU-16)

Disparador: el administrador necesita ajustar la sensibilidad o crear una política. Pasos: abre la configuración del servidor; crea o edita reglas de contenido y de conducta cuyos parámetros traen default, leyenda, límites y ejemplos del descriptor; agrupa reglas con un modo de coincidencia; compone un evento con grupos, prioridad y bandera continuar; le asocia acciones en orden; define exenciones por rol, usuario o canal; configura ventanas como la de antirrebote y la de borrado retroactivo. Todo evento nuevo nace en modo simulación. Antes de aplicar, el administrador ve la explicación en palabras y el alcance afectado, y confirma. Fricción anticipada: valor fuera de límites, grupo sin reglas, eliminación de un elemento referenciado; cada uno con su mensaje y su corrección. Salida: configuración persistida y coherente, lista para calibrarse en simulación y promoverse a ejecución real.

### 3.4 Revisión de incidentes y desbaneo (CU-05, CU-06, CU-07)

Disparador: llega un reporte de incidente al canal privado, o el administrador hace una revisión periódica. Pasos: abre la sección de incidentes; ve la lista ordenada por fecha con emisor, tipo de regla, acción y modo; selecciona un incidente; revisa la copia de los mensajes accionados y los canales afectados; si concluye que fue un falso positivo de un baneo en ejecución real, inicia la reversión, confirma, y el sistema desbanea al usuario dejando constancia. Fricción anticipada: el detalle aclara que los mensajes removidos no se restauran y que un incidente simulado no ofrece reversión. Salida: usuario desbaneado y reversión auditada en el incidente, o evidencia revisada sin acción.

### 3.5 Monitoreo del estado de conexión (CU-13)

Disparador: el administrador quiere confirmar que la protección está activa. Pasos: el panel de estado muestra cada servidor registrado con su estado de activación y su estado de conexión al canal de eventos; ante una caída, el estado cambia a desconectado y el sistema reconecta automáticamente; si el token quedó inválido, el estado lo indica y deriva a re-validar. Fricción anticipada: una caída silenciosa daría falsa sensación de seguridad; el estado siempre visible la evita. Salida: certeza de cobertura, o una alerta accionable de desconexión.

## 4. Estados y feedback

Mapa de estados por superficie clave, derivado de la tabla canónica del catálogo de diseño. Cada wireframe detalla su propia tabla; este es el marco común.

| Superficie | Vacío | Cargando | Con datos | Error | Sin conexión / sin permiso | Éxito |
| --- | --- | --- | --- | --- | --- | --- |
| Primer ingreso y autenticación | First-run sin cuenta: invitación a crear la cuenta única | Verificación de credenciales en curso (botón ocupado) | Formulario con credenciales ingresadas | Credenciales inválidas; contraseña débil; demasiados intentos | Sesión vencida redirige al ingreso | Cuenta creada / sesión abierta hacia el panel |
| Panel de estado | Sin servidores registrados: invitación a registrar el primero | Consulta del estado de los servidores | Lista de servidores con estado de activación y conexión | Falla al obtener el estado | Servidor desconectado; token inválido | Servidor recién activado tras prueba superada |
| Registro y prueba de servidor | Formulario de registro vacío | Cifrado del token / prueba contra la plataforma en curso | Resultado de la prueba con verificaciones | Token inválido; permisos faltantes; canal de salida ausente | Advertencia de jerarquía no bloqueante | Prueba superada: activación habilitada |
| Configuración de moderación | Servidor sin reglas, grupos ni eventos: invitación a crear el primero | Carga de descriptores y de la configuración | Reglas, grupos, eventos, acciones y exenciones | Valor fuera de límite; grupo sin reglas; referencia requerida | Modo simulación activo (no es error, es estado de seguridad) | Configuración aplicada tras confirmación |
| Incidentes y desbaneo | Sin incidentes registrados | Carga del listado / del detalle | Lista y detalle con evidencia | Incidente no encontrado; evidencia no disponible | Sesión expirada redirige al ingreso | Usuario desbaneado; reversión registrada |

Reglas de redacción de feedback (frontend writing del catálogo): voz activa; el verbo del botón coincide con el verbo del toast de confirmación; los errores no se disculpan ni son vagos; la pantalla vacía es una invitación a actuar. El estado nunca se comunica solo por color: siempre lleva etiqueta textual o ícono.

## 5. Accesibilidad

Compromiso explícito: WCAG 2.2 nivel AA como piso obligatorio, no como mejora opcional, en todas las superficies. Criterios prioritarios y verificables (heredados de la sección de accesibilidad del catálogo de diseño):

- Contraste de texto 4.5:1 (3:1 para texto grande); de componentes y de estados de foco, 3:1.
- Foco visible en todo elemento interactivo, con anillo de al menos 2px que no dependa solo del color.
- Navegación completa por teclado en el orden lógico de lectura, sin trampas de foco; objetivos de toque de al menos 24x24px.
- Semántica: un encabezado principal por vista; landmarks de navegación y de contenido; label asociado a cada control; rótulo accesible en los botones de solo ícono.
- El color nunca es el único canal de información: cada estado combina color con texto o ícono (estado de conexión, modo simulación, badge de estado, campo en error).
- Mensajes de error asociados a su campo y anunciados; el mensaje indica el rango admitido, no solo "valor inválido".
- Disclosure por teclado en la ayuda contextual de cada parámetro y en los expanders de opciones avanzadas, con anuncio de expandido o colapsado.
- La ranura del asistente de IA, hoy deshabilitada, anuncia su estado (deshabilitada, próximamente) a tecnologías asistivas, no solo de forma visual.
- Respeto de la preferencia de movimiento reducido: las animaciones no esenciales se desactivan.

Estos criterios se materializan como tests de accesibilidad previstos en 08 por cada wireframe.

## 6. Internacionalización

- Idioma soportado en v1: español rioplatense neutro técnico, idioma único de operación. La interfaz se redacta en sentence case, en voz activa.
- Expansión de texto: la leyenda y los ejemplos de cada parámetro pueden ser largos; los contenedores de ayuda contextual y de explicación en palabras reflujan sin truncar. Los botones toleran etiquetas que nombran el verbo completo de la acción.
- Dirección de lectura: izquierda a derecha.
- Formatos: fecha y hora en formato local del dispositivo, con orden año-mes-día en los listados de incidentes para orden cronológico inequívoco; números con separadores locales y cifras tabulares en columnas numéricas (umbrales, ventanas, contadores de canales afectados).
- Los textos de ayuda y de explicación en palabras provienen del descriptor o se generan por plantilla; cualquier traducción futura se aplica sobre esa fuente única, no sobre cadenas dispersas en la pantalla.

## 7. Performance percibida

- Tiempos tolerables: una acción local (validar un campo, abrir un detalle) se siente inmediata; una acción que viaja a la persistencia o a la plataforma de mensajería (prueba de configuración, desbaneo, registro) muestra feedback de carga apenas supera el umbral de espera perceptible.
- Skeletons para listas y tablas por encima de un umbral de espera breve; spinner o botón ocupado para acciones puntuales. La prueba de configuración, que consulta a la plataforma externa, muestra progreso por verificación a medida que se resuelve.
- Prevención de doble envío: los botones primarios quedan ocupados durante la operación que cruza a la persistencia o a la plataforma.
- Optimistic UI solo cuando la operación es reversible y local; las acciones de moderación reales (banear, desbanear) nunca son optimistas: esperan confirmación del resultado antes de declararse exitosas.
- Animación al servicio de la comprensión (transición de estado de conexión, avance del stepper, apertura de ayuda contextual), nunca movimiento ambiental permanente; transiciones breves con respeto de la preferencia de movimiento reducido.
- El estado de conexión de cada servidor se refresca dentro de un tiempo objetivo de refresco; si no logra refrescarse, el panel deja constancia en lugar de mostrar un estado obsoleto como si fuera vigente.

## 8. Errores y recuperación

Taxonomía de errores que el administrador verá, con tono y vía de recuperación. Los códigos provienen de los CU de 02; el tono sigue las reglas de redacción del catálogo (qué pasó, por qué, qué hacer; sin disculpas vagas, sin culpar al usuario).

| Categoría | Ejemplos (código de 02) | Tono y vía de recuperación |
| --- | --- | --- |
| Entrada inválida | SETUP_CONTRASENA_DEBIL, CONFIG_VALOR_FUERA_DE_LIMITE, SERVIDOR_IDENTIFICADOR_INVALIDO, EXENCION_IDENTIFICADOR_INVALIDO | Mensaje inline junto al campo, con el requisito o el rango admitido y el valor por defecto ofrecido; el foco vuelve al campo |
| Conflicto de estado | SETUP_YA_COMPLETADO, SERVIDOR_YA_REGISTRADO, EXENCION_DUPLICADA, CONFIG_REFERENCIA_REQUERIDA, CONFIG_GRUPO_SIN_REGLAS | Banner o inline que explica el conflicto y ofrece la salida (editar el existente, resolver las referencias, agregar una regla al grupo) |
| Autorización y sesión | AUTH_CREDENCIALES_INVALIDAS, AUTH_DEMASIADOS_INTENTOS, REVISION_SIN_AUTENTICACION, DESBANEO_SIN_AUTORIZACION | Mensaje neutro que no revela qué credencial falló; demora ante demasiados intentos; redirección al ingreso cuando la sesión expiró |
| Capacidad de moderar | PRUEBA_TOKEN_INVALIDO, PRUEBA_PERMISOS_FALTANTES, PRUEBA_CANAL_SALIDA_AUSENTE, DESBANEO_SIN_PERMISO, CONEXION_TOKEN_INVALIDO | Resultado de la prueba con cada faltante y su corrección; la activación queda bloqueada hasta resolver los bloqueantes; las advertencias no bloqueantes quedan visibles |
| Transitorio | DESBANEO_FALLA_PLATAFORMA, REPORTE_FALLA_ENVIO, CONEXION_RECONEXION_AGOTADA, CONEXION_ESTADO_NO_ACTUALIZA | Reintento automático con constancia; estado de desconexión visible; el incidente queda registrado aunque el reporte no se haya podido enviar |
| Evidencia y registro | INCIDENTE_NO_ENCONTRADO, EVIDENCIA_NO_DISPONIBLE | El incidente se muestra con los metadatos disponibles e indica que la copia no está accesible; no se inventa evidencia |

Principio rector de recuperación: ninguna falla deja al administrador sin saber qué hacer; toda acción peligrosa pasa por confirmación; el modo simulación es la red de seguridad que permite probar antes de comprometer una política.

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Administrador del sistema, operador único (00, vision-producto_v1.0.md §2) |
| CU origen | CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12, CU-13, CU-14, CU-15, CU-16 (02) |
| Reglas de negocio relevantes | RN-01, RN-02, RN-04, RN-05, RN-06, RN-07, RN-08, RN-09, RN-10, RN-11, RN-12, RN-13, RN-14, RN-15, RN-16 (02) |
| Wireframes asociados | wireframes-primer-ingreso-y-autenticacion_v1.0.md; wireframes-panel-de-estado_v1.0.md; wireframes-registro-de-servidor-y-prueba_v1.0.md; wireframes-configuracion-de-moderacion_v1.0.md; wireframes-revision-de-incidentes-y-desbaneo_v1.0.md |
| US a generar | US a generar en 06 (first-run y autenticación; registro y prueba de servidor; configuración dirigida por descriptor con presets, simulación y previsualización; exenciones; antirrebote; estado de conexión; revisión de incidentes y desbaneo) |
| Tests previstos | Tests de snapshot por superficie y por estado, y tests de accesibilidad WCAG 2.2 AA por wireframe (referencia tentativa a 08) |
| Catálogo de diseño aplicado | design-rules-web-generico_v1.0.md; especialización de stack design-rules-blazor-mudblazor_v1.0.md; extensión de capacidad design-rules-config-esquema_v1.0.md |
| Configuración dirigida por esquema aplicada (descriptores, presets, modo simulación, ranura del asistente) | Sí (superficie de configuración de moderación; ventanas y umbrales en registro y prueba) |

## 10. Notas y supuestos

- Caso degenerado de layout aplanado: la solución es de un único proyecto; los artefactos viven en `docs/03_ux_ui_dx/`, no bajo `proyectos/<kebab>/`.
- Operación de un único administrador: no hay multiusuario concurrente entre operadores. La superficie no diseña roles ni permisos diferenciados entre personas; la autorización es por sesión del administrador único.
- La ayuda contextual, los defaults, los límites, las leyendas y los ejemplos de cada parámetro provienen del descriptor (fuente única). La pantalla nunca los escribe a mano; cualquier valor mostrado en los ejemplos de estos artefactos es ilustrativo de cómo se vería un descriptor, no un valor hardcodeado de pantalla.
- La explicación en palabras de una configuración se genera por plantilla a partir de descriptores y valores; no se redacta a mano por pantalla.
- La ranura del asistente de IA se reserva deshabilitada (forward-compat). La IA, cuando exista, llena una PropuestaDeConfiguracion que el administrador previsualiza y confirma y que el sistema valida; la IA propone, no ejecuta.
- El motor de validación, el registro de descriptores como servicio y la mecánica de aplicación son arquitectura técnica de 05; aquí solo se define la experiencia. La presentación no decide el stack: la única referencia admitida al stack es el nombre del documento del catálogo de diseño en la fila de trazabilidad.
- Supuesto de un servidor en v1 con modelo que admite varios contextos: el panel de estado y la configuración se diseñan por servidor para escalar a varios sin rediseño.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Marco de experiencia inicial. Audiencia operador único, principios de Nielsen y leyes UX, cinco flujos clave, mapa de estados por superficie, accesibilidad WCAG 2.2 AA como piso, i18n, performance percibida, taxonomía de errores con los códigos de 02, trazabilidad upstream y downstream, catálogo de diseño y configuración dirigida por esquema declarados. |
