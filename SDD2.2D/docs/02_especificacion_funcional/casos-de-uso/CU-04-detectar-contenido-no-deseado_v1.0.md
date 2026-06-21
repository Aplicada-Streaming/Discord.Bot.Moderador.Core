# CU-04 — Detectar contenido no deseado en un mensaje y contener al emisor

**Proyecto:** discord-bots-admin
**Documento:** CU-04-detectar-contenido-no-deseado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Evaluar el contenido de un mensaje aislado contra criterios definidos por el administrador (expresión regular o palabras o frases clave) y, cuando un mensaje cumple el criterio, disparar la contención del emisor, con independencia de cuántos canales toque. Aporta el eje de defensa por contenido, complementario al eje de conducta de CU-01.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Evalúa la regla de contenido sobre el mensaje y dispara la política correspondiente |
| Plataforma de mensajería | Sistema | Emite el mensaje a evaluar y recibe la acción de contención |
| Administrador del sistema | Secundario | Define los criterios de contenido (expresión regular, palabras o frases clave) |

## 3. Precondiciones

- Existe un servidor registrado y activo con su canal de eventos conectado.
- Existe al menos una regla de contenido configurada con su criterio (expresión regular o palabras o frases clave).
- El emisor no pertenece a un sujeto exento (RN-07).
- Existe un evento o política que incluye esa regla de contenido con su acción asociada.

## 4. Flujo principal

1. La plataforma de mensajería entrega un mensaje al servicio de moderación.
2. El servicio descarta a los emisores exentos antes de evaluar (RN-07).
3. El servicio evalúa la regla de contenido sobre el texto del mensaje aislado, sin observar la actividad acumulada del usuario.
4. Si el criterio se cumple (la expresión regular coincide o aparecen las palabras o frases clave), el servicio marca la condición de contenido no deseado para ese mensaje.
5. El servicio evalúa las políticas por prioridad con primera coincidencia (RN-04).
6. El servicio ejecuta las acciones de la política cumplida en el orden configurado (RN-05): típicamente reportar y contener al emisor.
7. El servicio registra el incidente con la copia del mensaje y el canal afectado.

## 5. Flujos alternativos

- 5.A Criterio no cumplido. Disparador: el contenido no coincide con ningún criterio configurado. Acción: el servicio no marca la condición. Punto de retorno: el flujo termina; el mensaje sigue siendo considerado para el eje de conducta (CU-01).
- 5.B Política de contenido en modo simulación. Disparador: la política aplicable está en modo simulación. Acción: el flujo se desvía a CU-14, que registra lo que se habría hecho sin ejecutarlo. Punto de retorno: termina sin contener; el incidente queda marcado como simulado.
- 5.C Coincidencia simultánea con la regla de ráfaga. Disparador: el mismo mensaje contribuye además a marcar la condición de ráfaga distribuida. Acción: la evaluación se resuelve por prioridad con primera coincidencia y bandera continuar (RN-04). Punto de retorno: retorna a la cadena de evaluación de políticas.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CONTENIDO_PATRON_INVALIDO | La expresión regular configurada no es válida o no compila | No evalúa esa regla; registra el patrón inválido para corrección del administrador y continúa con las demás reglas |
| CONTENIDO_JERARQUIA_INSUFICIENTE | El emisor tiene rol superior al bot o el bot carece de permisos para la acción (RN-01) | No ejecuta la acción de contención; registra el incidente como no accionable y lo reporta (CU-05) |
| CONTENIDO_EVALUACION_EXCEDE_TIEMPO | La evaluación del criterio sobre el mensaje supera el presupuesto de tiempo de procesamiento | Aborta la evaluación de esa regla, registra la situación y continúa, para no bloquear el procesamiento del flujo de mensajes |

## 7. Postcondiciones

- En caso de éxito: la condición de contenido no deseado queda registrada; si la política está en ejecución real, el emisor queda contenido y existe un incidente con la copia del mensaje y el canal afectado.
- En caso de fallo: la condición no produce contención; la situación de excepción queda registrada para revisión del administrador.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una regla de contenido con la expresión regular que detecta enlaces de un dominio de estafa conocido y una política en ejecución real | Un usuario no exento publica un mensaje con un enlace a ese dominio | El servicio marca la condición, contiene al emisor y registra el incidente con la copia del mensaje y el canal |
| CA-02 | Una regla de contenido por palabra clave vedada y un usuario incluido en una exención por usuario de confianza | El usuario exento publica un mensaje con esa palabra | El servicio descarta al emisor antes de evaluar y no contiene |
| CA-03 | Una regla de contenido cuyo patrón no compila | Llega un mensaje cualquiera | El servicio omite esa regla, registra el patrón inválido con código CONTENIDO_PATRON_INVALIDO y evalúa las demás reglas |
| CA-04 | Una regla de contenido con su política en modo simulación | Un usuario publica un mensaje que cumple el criterio | El servicio no contiene y registra el incidente como simulado con la acción que se habría ejecutado |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-03, RN-04, RN-05, RN-07, RN-09 |
| Historias de usuario a generar | US a generar en 06 (detección de contenido por expresión regular y por palabras clave; contención del emisor) |
| Componentes esperados | Evaluador de reglas de contenido sin estado; motor de evaluación; ejecutor de acciones (referencia tentativa a 05) |
| Tests previstos | Pruebas del evaluador de expresión regular y de palabras clave; pruebas de patrón inválido; pruebas de descarte de exentos (referencia tentativa a 08) |

## 10. Notas y supuestos

- La regla de contenido es un predicado sin estado: evalúa el mensaje en sí, no la actividad acumulada del usuario.
- La acción que sigue a la condición de contenido reutiliza el mismo mecanismo de contención que la ráfaga distribuida (baneo, y opcionalmente borrado retroactivo), parametrizable por la política.
- El comportamiento errático del filtro nativo de la plataforma es el motivo de negocio para que este control quede bajo administración directa del administrador.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-03 y de la propuesta de valor del intake |
