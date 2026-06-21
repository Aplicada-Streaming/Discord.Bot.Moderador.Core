# CU-01 — Detectar ráfaga distribuida por canales distintos en ventana corta

**Proyecto:** discord-bots-admin
**Documento:** CU-01-detectar-rafaga-distribuida_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Reconocer el patrón de ráfaga distribuida —un mismo usuario que publica en una cantidad de canales distintos por encima del umbral, dentro de una ventana corta de tiempo— para que el sistema disponga del disparador que activa la contención automática del emisor. Resuelve la necesidad central del producto: distinguir el spam automatizado del uso legítimo intenso en un solo canal.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Recibe los mensajes del canal de eventos, mantiene el estado de conducta por usuario y evalúa la regla de ráfaga distribuida |
| Plataforma de mensajería | Sistema | Emite los mensajes en tiempo real a través del canal de eventos |
| Administrador del sistema | Secundario | Define el umbral de canales distintos y la ventana de detección que la regla evalúa |

## 3. Precondiciones

- Existe un servidor registrado y activo, con su canal de eventos conectado.
- Existe al menos una regla de conducta de tipo ráfaga distribuida con su umbral de canales distintos y su ventana de detección configurados.
- El usuario emisor no pertenece a un sujeto exento (ver RN-07).
- El estado de conducta en memoria está disponible para el servidor (ventanas deslizantes de actividad por usuario).

## 4. Flujo principal

1. La plataforma de mensajería entrega un mensaje al servicio de moderación a través del canal de eventos.
2. El servicio descarta a los emisores exentos antes de evaluar (RN-07); si el emisor no está exento, continúa.
3. El servicio registra la actividad del emisor en su ventana deslizante: identificador de usuario, canal y marca de tiempo.
4. El servicio cuenta los canales distintos en los que ese usuario publicó dentro de la ventana de detección configurada.
5. El servicio compara la cuenta de canales distintos contra el umbral configurado.
6. Si la cuenta de canales distintos alcanza o supera el umbral dentro de la ventana, el servicio marca la condición de ráfaga distribuida como cumplida para ese usuario en ese servidor.
7. El servicio expone la condición cumplida como disparador para la evaluación de políticas (CU-02 y la cadena de acciones).

## 5. Flujos alternativos

- 5.A Actividad intensa en un solo canal. Disparador: el usuario publica muchos mensajes pero todos en el mismo canal. Acción: la cuenta de canales distintos permanece en 1 y no alcanza el umbral. Punto de retorno: el flujo termina sin marcar la condición; el mensaje se procesa por los demás ejes (CU-04) y la actividad queda registrada.
- 5.B Ráfaga espaciada que no entra completa en la ventana. Disparador: los mensajes llegan separados en el tiempo. Acción: la regla mantiene como discriminador la cantidad de canales distintos dentro de la ventana configurada; si la ventana fue ampliada por el administrador, captura el fan-out más espaciado. Punto de retorno: si dentro de la ventana se alcanza el umbral de canales distintos, retorna al paso 6; si no, el flujo termina sin marcar la condición.
- 5.C Coincidencia simultánea con otra política. Disparador: el mismo mensaje cumple además una regla de contenido. Acción: la evaluación de políticas se resuelve por prioridad con primera coincidencia y bandera continuar (RN-04). Punto de retorno: retorna a la cadena de evaluación de políticas con la condición de ráfaga ya marcada.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| DETECCION_ESTADO_NO_DISPONIBLE | El estado de conducta en memoria no está inicializado para el servidor (por ejemplo, tras un reinicio reciente del servicio) | El servicio reconstruye la ventana deslizante desde cero a partir del mensaje actual y registra la situación en el journal del servicio; los mensajes previos al reinicio no se contabilizan |
| DETECCION_GATEWAY_DESCONECTADO | El canal de eventos perdió la conexión y no se reciben mensajes | No se evalúa la regla mientras dure la caída; el servidor queda marcado como desconectado y la reconexión la gestiona CU-13 |
| DETECCION_PARAMETRO_INVALIDO | El umbral o la ventana quedaron fuera de los límites del descriptor del parámetro | El servicio aplica el valor por defecto del descriptor y registra la inconsistencia para revisión del administrador |

## 7. Postcondiciones

- En caso de éxito: la condición de ráfaga distribuida queda marcada para el par usuario-servidor y disponible como disparador; el estado de conducta del usuario refleja la actividad evaluada.
- En caso de fallo: la condición no se marca; la actividad del usuario, si pudo registrarse, permanece en la ventana deslizante; la situación de excepción queda en el journal del servicio.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una regla de ráfaga con umbral de 3 canales distintos y ventana de 2 s, y un usuario no exento | El usuario publica en los canales general, anuncios y memes en 1,5 s | El servicio marca la condición de ráfaga distribuida cumplida para ese usuario |
| CA-02 | La misma regla con umbral 3 canales y ventana 2 s | El usuario publica 10 mensajes en el canal general en 1,5 s | El servicio no marca la condición porque la cuenta de canales distintos es 1 |
| CA-03 | Una regla con umbral 3 canales y ventana ampliada a 6 s | El usuario publica en 3 canales distintos a lo largo de 5 s | El servicio marca la condición de ráfaga distribuida cumplida |
| CA-04 | Una regla con umbral 3 canales y un usuario incluido en una exención por rol staff | El usuario publica en 4 canales distintos en 1 s | El servicio descarta al emisor antes de evaluar y no marca la condición |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-04, RN-07, RN-08, RN-10 |
| Historias de usuario a generar | US a generar en 06 (detección de ráfaga por canales distintos; configuración de umbral y ventana) |
| Componentes esperados | Motor de evaluación y evaluador de conducta; servicio de estado en memoria (referencia tentativa a 05) |
| Tests previstos | Pruebas unitarias del evaluador de ráfaga con umbral y ventana; pruebas del descarte de exentos (referencia tentativa a 08) |

## 10. Notas y supuestos

- El estado de conducta vive en memoria; ante un reinicio del servicio se pierde la actividad acumulada previa, según el trade-off aceptado en el alcance. El borrado retroactivo del baneo (CU-03) cubre lo que haya quedado dentro de la ventana de borrado.
- El discriminador de canales distintos es la decisión de diseño que separa el spam del uso legítimo; no se cuenta cantidad de mensajes.
- Los valores por defecto exactos (umbral de 2 vs 3 canales, ventana de 2 vs 4 s) quedan abiertos para calibración; este CU es agnóstico al valor concreto y opera sobre lo que el descriptor del parámetro defina.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-01 y de los casos límite del intake |
