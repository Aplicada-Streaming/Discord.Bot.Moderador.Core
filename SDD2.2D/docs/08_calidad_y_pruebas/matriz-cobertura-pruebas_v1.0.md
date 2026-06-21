# Matriz de cobertura de pruebas — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** matriz-cobertura-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

## 1. Propósito y alcance

Este documento es la bisagra de trazabilidad: relaciona los 16 casos de uso de 02 con sus tests (CU↔Tests), los NFR de `arquitectura-solucion_v1.0.md` §8 con su medición (NFR↔Tests) y las 16 reglas de negocio de 02 con su test (RN↔Tests), más la tabla de cobertura por capa. Los TC referenciados viven en `casos-prueba-referenciales_v1.0.md`. El estado refleja la etapa de diseño: todos los tests están especificados y pendientes de implementación; la matriz se actualiza al cierre de cada rebanada del mini-plan de 07 (anti-patrón de matriz desactualizada de la regla §4.10).

## 2. Trazabilidad CU↔Tests

Los 16 CU con su criterio Given/When/Then (CA-XX de 02), el TC que lo cubre y el estado. Cada CU-01..CU-16 tiene representados sus cuatro criterios de aceptación CA-01..CA-04 con al menos un TC cada uno (los 64 CA de los 16 CU están cubiertos). La columna CA indica el criterio de aceptación de 02 que cada fila representa; las filas marcadas como "excepción" cubren un código de excepción del CU (sección 6 del CU), no un criterio de aceptación, y se distinguen para no contarse como CA. Los TC de soporte de reglas de negocio sin un CA de CU directo se listan al pie de la tabla (TC de verificación de RN).

| CU | CA | Criterio Given-When-Then | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- | --- |
| CU-01 | CA-01 | Given regla umbral 3 canales y ventana 2 s, When publica en 3 canales en 1,5 s, Then marca ráfaga | TC-01 | Unit | Pendiente |
| CU-01 | CA-02 | Given misma regla, When publica 10 mensajes en 1 canal, Then no marca (1 canal distinto) | TC-02 | Unit | Pendiente |
| CU-01 | CA-03 | Given ventana ampliada a 6 s, When publica en 3 canales en 5 s, Then marca ráfaga | TC-03 | Unit | Pendiente |
| CU-01 | CA-04 | Given usuario exento por rol staff, When publica en 4 canales, Then descarta antes de evaluar | TC-04 | Unit | Pendiente |
| CU-02 | CA-01 | Given política real y emisor con rol inferior, When marca ráfaga, Then banea y registra incidente | TC-05 | Integration | Pendiente |
| CU-02 | CA-02 | Given emisor con rol superior, When marca ráfaga, Then no banea, código BANEO_JERARQUIA_INSUFICIENTE | TC-06 | Integration | Pendiente |
| CU-02 | CA-03 | Given política en simulación, When marca ráfaga, Then no banea y registra simulado | TC-07 | Unit | Pendiente |
| CU-02 | CA-04 | Given emisor ya baneado en la ráfaga, When llega nuevo mensaje, Then no repite por antirrebote | TC-08 | Unit | Pendiente |
| CU-03 | CA-01 | Given ventana borrado 1 día y mensajes en 5 canales, When ejecuta, Then purga 5 canales | TC-09 | Integration | Pendiente |
| CU-03 | CA-02 | Given ventana 10 días, When ejecuta, Then acota a 7 días, código BORRADO_VENTANA_FUERA_DE_RANGO | TC-10 | Unit | Pendiente |
| CU-03 | CA-03 | Given ventana 0 días, When ejecuta, Then banea sin remover mensajes | TC-11 | Unit | Pendiente |
| CU-03 | CA-04 | Given mensajes no evaluados dentro de la ventana, When ejecuta, Then también los remueve | TC-12 | Integration | Pendiente |
| CU-04 | CA-01 | Given regla regex de dominio de estafa y política real, When publica enlace, Then contiene y registra | TC-13 | Unit | Pendiente |
| CU-04 | CA-02 | Given usuario exento, When publica palabra vedada, Then descarta antes de evaluar | TC-16 | Unit | Pendiente |
| CU-04 | CA-03 | Given patrón que no compila, When llega mensaje, Then omite la regla, código CONTENIDO_PATRON_INVALIDO | TC-14 | Unit | Pendiente |
| CU-04 | CA-04 | Given regla de contenido en simulación, When publica mensaje que cumple, Then no contiene y registra simulado | TC-59 | Unit | Pendiente |
| CU-04 | excepción | Given patrón con retroceso catastrófico, When evalúa, Then aborta por tope de tiempo sin colgar, código CONTENIDO_EVALUACION_EXCEDE_TIEMPO | TC-15 | Unit | Pendiente |
| CU-05 | CA-01 | Given canal de salida y baneo real en 4 canales, When registra incidente, Then publica reporte con 4 canales | TC-25 | Integration | Pendiente |
| CU-05 | CA-02 | Given simulación con canal de salida, When registra simulado, Then publica reporte etiquetado | TC-22 | Integration | Pendiente |
| CU-05 | CA-03 | Given servidor sin canal de salida, When dispara política, Then conserva incidente, código REPORTE_CANAL_NO_DESIGNADO | TC-26 | Unit | Pendiente |
| CU-05 | CA-04 | Given incidente no accionable por jerarquía, When compone el reporte, Then incluye advertencia de jerarquía o permisos | TC-60 | Integration | Pendiente |
| CU-06 | CA-01 | Given administrador autenticado y 3 incidentes, When abre la sección, Then lista los 3 con sus atributos | TC-49 | E2E | Pendiente |
| CU-06 | CA-02 | Given incidente real con 6 mensajes en 4 canales, When abre detalle, Then muestra evidencia y ofrece revertir | TC-50 | E2E | Pendiente |
| CU-06 | CA-03 | Given incidente en simulación, When abre detalle, Then muestra acción y no ofrece reversión | TC-61 | E2E | Pendiente |
| CU-06 | CA-04 | Given sesión expirada, When abre la sección, Then redirige, código REVISION_SIN_AUTENTICACION | TC-51 | E2E | Pendiente |
| CU-07 | CA-01 | Given incidente con baneo real, When confirma reversión, Then desbanea y registra reversión | TC-52 | Integration | Pendiente |
| CU-07 | CA-02 | Given incidente en simulación, When abre detalle, Then no ofrece revertir | TC-54 | E2E | Pendiente |
| CU-07 | CA-03 | Given baneo real cuyo usuario ya fue desbaneado por otra vía, When solicita revertir, Then informa ya no baneado y marca revertido | TC-62 | Integration | Pendiente |
| CU-07 | CA-04 | Given bot sin permiso de desbaneo, When confirma reversión, Then no desbanea, código DESBANEO_SIN_PERMISO | TC-53 | Integration | Pendiente |
| CU-08 | CA-01 | Given sistema sin cuenta, When ingresa contraseña robusta confirmada, Then crea cuenta única con hash | TC-30 | Unit | Pendiente |
| CU-08 | CA-02 | Given first-run, When contraseña débil, Then rechaza, código SETUP_CONTRASENA_DEBIL | TC-31 | Unit | Pendiente |
| CU-08 | CA-03 | Given cuenta ya creada, When intenta alta, Then bloquea, código SETUP_YA_COMPLETADO | TC-32 | Unit | Pendiente |
| CU-08 | CA-04 | Given first-run, When confirmación no coincide, Then rechaza sin crear la cuenta | TC-63 | Unit | Pendiente |
| CU-09 | CA-01 | Given cuenta y credenciales correctas, When ingresa, Then abre sesión autorizada | TC-33 | Unit | Pendiente |
| CU-09 | CA-02 | Given cuenta, When contraseña incorrecta, Then no abre sesión, código AUTH_CREDENCIALES_INVALIDAS | TC-34 | Unit | Pendiente |
| CU-09 | CA-03 | Given sesión vencida, When intenta acción protegida, Then invalida y exige reautenticar | TC-64 | Unit | Pendiente |
| CU-09 | CA-04 | Given sistema sin cuenta, When intenta autenticarse, Then redirige a primer ingreso, código AUTH_SIN_CUENTA | TC-65 | Unit | Pendiente |
| CU-10 | CA-01 | Given administrador autenticado y servidor nuevo, When registra con token, Then persiste con token cifrado e inactivo | TC-35 | Integration | Pendiente |
| CU-10 | CA-02 | Given identificador ya registrado, When registra duplicado, Then rechaza, código SERVIDOR_YA_REGISTRADO | TC-36 | Unit | Pendiente |
| CU-10 | CA-03 | Given alta de servidor, When token vacío, Then rechaza, código SERVIDOR_TOKEN_VACIO | TC-37 | Unit | Pendiente |
| CU-10 | CA-04 | Given servidor con token revocado, When ingresa token nuevo, Then cifra y reemplaza y conserva la configuración | TC-66 | Integration | Pendiente |
| CU-11 | CA-01 | Given parámetro default 3 límites 2..10, When ingresa 4, Then acepta y muestra leyenda y ejemplo | TC-39 | Unit | Pendiente |
| CU-11 | CA-02 | Given límites 2..10, When ingresa 1, Then rechaza, código CONFIG_VALOR_FUERA_DE_LIMITE | TC-40 | Unit | Pendiente |
| CU-11 | CA-03 | Given grupo sin reglas, When guarda, Then rechaza, código CONFIG_GRUPO_SIN_REGLAS | TC-41 | Unit | Pendiente |
| CU-11 | CA-04 | Given grupo referenciado por un evento, When elimina, Then bloquea, código CONFIG_REFERENCIA_REQUERIDA | TC-42 | Integration | Pendiente |
| CU-12 | CA-01 | Given token válido y permisos completos, When prueba, Then todo superado y habilita activación | TC-43 | Integration | Pendiente |
| CU-12 | CA-02 | Given sin permiso de baneo, When prueba, Then bloquea, código PRUEBA_PERMISOS_FALTANTES | TC-44 | Integration | Pendiente |
| CU-12 | CA-03 | Given token revocado, When prueba, Then falla, código PRUEBA_TOKEN_INVALIDO, deja desconectado | TC-45 | Integration | Pendiente |
| CU-12 | CA-04 | Given roles de staff por encima del bot, When prueba, Then advierte jerarquía y permite activar | TC-46 | Integration | Pendiente |
| CU-13 | CA-01 | Given servidor conectado, When cae transitoriamente, Then reconecta automáticamente | TC-47 | Integration | Pendiente |
| CU-13 | CA-02 | Given servidor que pasa a desconectado, When cambia el estado, Then el panel lo refleja dentro del tiempo objetivo | TC-67 | Integration | Pendiente |
| CU-13 | CA-03 | Given token revocado en operación, When falla reconexión, Then desconectado, código CONEXION_TOKEN_INVALIDO | TC-48 | Integration | Pendiente |
| CU-13 | CA-04 | Given caída prolongada, When los reintentos no reconectan, Then mantiene desconectado visible y sigue reintentando | TC-68 | Integration | Pendiente |
| CU-14 | CA-01 | Given política en simulación, When usuario cumple, Then registra simulado y no banea | TC-07 | Unit | Pendiente |
| CU-14 | CA-02 | Given simulación con canal de salida, When registra simulado, Then publica reporte etiquetado | TC-22 | Integration | Pendiente |
| CU-14 | CA-03 | Given política promovida a real, When usuario cumple, Then ejecuta acción real | TC-23 | Integration | Pendiente |
| CU-14 | CA-04 | Given modo indefinido, When cumple, Then simulación segura, código SIMULACION_MODO_INCONSISTENTE | TC-24 | Unit | Pendiente |
| CU-15 | CA-01 | Given administrador y rol staff a eximir, When crea exención, Then persiste y excluye de moderación | TC-27 | Integration | Pendiente |
| CU-15 | CA-02 | Given exención por rol vigente, When usuario staff publica en 5 canales, Then descarta antes de evaluar | TC-04 | Unit | Pendiente |
| CU-15 | CA-03 | Given identificador no snowflake, When agrega exención, Then rechaza, código EXENCION_IDENTIFICADOR_INVALIDO | TC-28 | Unit | Pendiente |
| CU-15 | CA-04 | Given canal de confianza, When usuario publica contenido que cumpliría, Then excluye el canal | TC-29 | Unit | Pendiente |
| CU-16 | CA-01 | Given usuario ya baneado y ventana vigente, When llega nuevo mensaje, Then suprime acción repetida | TC-08 | Unit | Pendiente |
| CU-16 | CA-02 | Given ventana de antirrebote expirada, When vuelve a disparar, Then ejecuta nueva acción | TC-20 | Unit | Pendiente |
| CU-16 | CA-03 | Given ventana fuera de límites del descriptor, When evalúa, Then usa default, código ANTIRREBOTE_VENTANA_INVALIDA | TC-21 | Unit | Pendiente |
| CU-16 | CA-04 | Given usuario nunca accionado en la ráfaga, When dispara por primera vez, Then ejecuta la acción y lo marca accionado | TC-69 | Unit | Pendiente |

Cobertura de CU: los 16 CU (CU-01..CU-16) tienen al menos un TC; no hay CU huérfano. Los cuatro criterios de aceptación CA-01..CA-04 de cada uno de los 16 CU están representados con al menos un TC (64 CA cubiertos). La fila de CU-04 etiquetada como "excepción" cubre el código CONTENIDO_EVALUACION_EXCEDE_TIEMPO (sección 6 del CU-04), no un criterio de aceptación, y por eso no se cuenta como CA; el CA-04 propio de CU-04 (modo simulación) queda cubierto por TC-59.

TC de verificación de regla de negocio sin un CA de CU directo. Estos TC sostienen una RN de evaluación o de seguridad que no se materializa como un CA específico de ningún CU; aparecen en la tabla RN↔Tests (§4) y se anclan acá para que no queden como huecos silenciosos de trazabilidad.

| TC | RN que verifica | Motivo de no tener un CA de CU directo | Tipo |
| --- | --- | --- | --- |
| TC-17 | RN-04 | Evaluación por prioridad con primera coincidencia y bandera continuar desactivada; comportamiento transversal de RN-04, no un CA específico (el CA-01 de CU-11 lo cubre TC-39) | Unit |
| TC-18 | RN-04 | Variante de RN-04 con bandera continuar activada que dispara la segunda política; verificación de la regla, sin CA de CU propio | Unit |
| TC-19 | RN-05, RN-11 | Orden de ejecución de acciones y copia antes de borrar; invariante de ejecución compartida por CU-02 y CU-03, sin CA específico | Unit |
| TC-38 | RN-14 | Ida y vuelta de cifrado/descifrado del token; verificación de la RN-14 de seguridad, sin CA de CU directo | Unit |

TC-23 sí tiene fila de CU↔Tests propia: cubre el CA-03 de CU-14 (promoción de simulación a ejecución real) en el bloque de ese CU, además de aparecer en RN↔Tests (RN-09).

## 3. Trazabilidad NFR↔Tests

Los NFR con objetivo numérico de `arquitectura-solucion_v1.0.md` §8 (valores de `SOLUTION-INTAKE §17 P.10`), su SLA, el test o medición que lo valida y el tooling de medición (por capacidad).

| NFR | SLA | Test | Tooling de medición |
| --- | --- | --- | --- |
| Latencia de procesamiento por mensaje | p95 < 200 ms | TC-55 | Pipeline instrumentado + banco de mensajes simulados sobre hardware de referencia (`SOLUTION-INTAKE §17 P.10`) |
| Throughput sostenido | ≥ 50 mensajes/s en el dispositivo de referencia | TC-56 | Banco de pruebas de carga con mensajes simulados sobre el hardware real (a confirmar por benchmark) |
| Disponibilidad mensual (SLO) | 99 % mensual | Métrica observada en 09 (no test unitario) | Tiempo de servicio sobre tiempo total del mes, derivado del journal y del estado de conexión por contexto; reinicio automático |
| Memoria por conexión de gateway activa | ≤ 8 MB por conexión | TC-57 | Perfilado de huella de memoria por contexto activo en el dispositivo |
| Cobertura de tests del módulo de detección | ≥ 90 % líneas; global líneas ≥ 75 %, branches ≥ 65 % | Gate G3 del pipeline (ver §5) | Recolector de cobertura por capa en el pipeline (`SOLUTION-INTAKE §17 P.6`) |
| Limpieza efectiva de la ráfaga | ≥ 98 % de mensajes eliminados dentro de los 10 s del incidente | TC-58 | Registro de incidentes con canales afectados; medición en pruebas con mensajes simulados |

Cada NFR con objetivo numérico tiene un test o una medición observada asociada; ninguno queda sin verificación.

## 4. Trazabilidad RN↔Tests

Las 16 reglas de negocio de 02 con el TC que verifica su cumplimiento y el tipo.

| RN | Enunciado abreviado | TC | Tipo |
| --- | --- | --- | --- |
| RN-01 | Jerarquía de roles del bot para accionar | TC-06, TC-44, TC-53 | Integration |
| RN-02 | Tope del borrado retroactivo (0 a 7 días) | TC-10, TC-11, TC-12 | Unit / Integration |
| RN-03 | Validez del patrón de una regla de contenido | TC-14, TC-15 | Unit |
| RN-04 | Evaluación de políticas por prioridad con primera coincidencia | TC-17, TC-18 | Unit |
| RN-05 | Orden de ejecución de acciones y copia antes de borrar | TC-19 | Unit |
| RN-06 | Antirrebote por usuario durante una ráfaga | TC-08, TC-20 | Unit |
| RN-07 | Descarte previo de los sujetos exentos | TC-04, TC-16, TC-29 | Unit |
| RN-08 | Identidad de los snowflakes como texto | TC-28, TC-36 | Unit |
| RN-09 | El modo simulación no ejecuta acción real | TC-07, TC-24, TC-54 | Unit / E2E |
| RN-10 | Configuración dirigida por descriptor de parámetro | TC-21, TC-39, TC-40 | Unit |
| RN-11 | Integridad de la evidencia del incidente | TC-19, TC-25, TC-50 | Unit / Integration / E2E |
| RN-12 | Autorización por rol administrador único | TC-27, TC-35, TC-51, TC-52 | Integration / E2E |
| RN-13 | Resguardo de las credenciales del administrador y cuenta única | TC-30, TC-31, TC-32, TC-33, TC-34 | Unit |
| RN-14 | Cifrado del token de bot en reposo | TC-35, TC-37, TC-38, TC-45, TC-48 | Unit / Integration |
| RN-15 | Composición de un grupo de reglas | TC-41, TC-42 | Unit / Integration |
| RN-16 | Activación condicionada a la prueba de configuración | TC-43, TC-44, TC-45, TC-46, TC-47, TC-48 | Integration |

Cobertura de RN: las 16 RN (RN-01..RN-16) tienen al menos un TC; no hay RN huérfana.

## 5. Cobertura por capa

Umbrales de `estrategia-testing_v1.0.md` §2. Los valores observados se completan a medida que se implementan los tests; en esta etapa de diseño no hay corrida de cobertura, por lo que la columna observada figura como pendiente.

| Capa | Líneas observadas (%) | Branches observadas (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio (motor de moderación, evaluadores, ventana deslizante, antirrebote, descriptores) | pendiente | pendiente | no exigido en v1 | 90 / 80 / — |
| Aplicación (autenticación, configuración, registro de servidores, ejecutor de acciones, incidentes, prueba de configuración) | pendiente | pendiente | no exigido en v1 | 80 / 70 / — |
| Infraestructura (adaptador del gateway, persistencia, cifrado de tokens) | pendiente | pendiente | no exigido en v1 | 70 / 60 / — |
| Presentación (panel del lado servidor) | pendiente | pendiente | no exigido en v1 | 60 / 50 / — |
| Global (gate de CI) | pendiente | pendiente | — | 75 / 65 / — |

El gate de CI bloquea el merge si líneas < 75 % o branches < 65 % global, o si el módulo de detección queda por debajo de 90 % líneas (gate G3 de `estrategia-calidad_v1.0.md` §3).

## 6. Gaps identificados

- Cobertura de criterios de aceptación: no quedan CA sin cubrir. Los cuatro CA-01..CA-04 de cada uno de los 16 CU tienen al menos un TC en la tabla CU↔Tests (64 CA cubiertos), tras agregar TC-59 a TC-69 al cerrar el hallazgo H-01 del audit de Fase E.
- Cobertura observada por capa aún no medida: no hay código implementado en esta etapa de diseño; se completa al cierre de la rebanada 1 del mini-plan de 07 y en cada rebanada siguiente.
- NFR de latencia y throughput: dependen de benchmark en el hardware real (ARM de 32 bits), tier deprioritizado (`SOLUTION-INTAKE §17 P.12`); TC-55 y TC-56 se ejecutan en ambiente equivalente y se confirman en hardware real cuando esté disponible.
- Mutation testing no se exige como gate en v1 (la regla §2.2 solo lo pide para library); queda como mejora futura sin afectar el gate de cobertura.
- US-19 (segundo canal de salida lógico, Could, en Borrador) no está comprometida en ninguna rebanada del mini-plan; sus TC se incorporan cuando la US se refine.
- Disponibilidad mensual (SLO 99 %) se valida por métrica observada en 09, no por test de la suite; queda fuera del gate de CI por naturaleza.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Matriz inicial para `discord-bots-admin`: tabla CU↔Tests con los 16 CU, tabla NFR↔Tests con los 6 NFR de 05 §8, tabla RN↔Tests con las 16 RN, y tabla de cobertura por capa con umbrales de `estrategia-testing_v1.0.md`. 58 TC referenciados, todos en estado Pendiente por ser etapa de diseño. |
| 1.0 | 2026-06-20 | Corrección de los hallazgos P1 H-01 (11 CA sin TC) y H-02 (mapeo de TC-15 como excepción y anclaje de TC-17/18/19/23/38) del audit de Fase E. La tabla CU↔Tests incorpora la columna CA y las filas de TC-59 a TC-69, con lo que los 64 CA de los 16 CU quedan representados; TC-15 se reetiqueta como excepción del CU-04 (CONTENIDO_EVALUACION_EXCEDE_TIEMPO) y no como CA-04; TC-17, TC-18, TC-19 y TC-38 se anclan como TC de verificación de RN al pie de la tabla y se aclara que TC-23 ya tiene fila propia en el bloque CU-14 (CA-03). Total 69 TC referenciados. |
