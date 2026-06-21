# NB-03 — Contención de contenido no deseado por patrón

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-03-contencion-contenido-no-deseado_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §3, §4; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-04 (prevista en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

Además de la ráfaga distribuida, el negocio necesita contener mensajes individuales cuyo contenido es indeseado por sí mismo, con independencia de cuántos canales toque el emisor. Hay material que el administrador quiere bloquear apenas aparece, por ejemplo enlaces de estafa, lenguaje vedado o patrones de texto conocidos. La defensa actual descansa en el filtro nativo de la plataforma con expresiones regulares, que el solicitante describe como de comportamiento errático, por lo que no ofrece una contención confiable ni un control que el administrador sienta propio.

La necesidad importa porque no todo el spam llega como ráfaga: una parte se manifiesta como un único mensaje con contenido prohibido que igualmente daña la comunidad. El negocio quiere poder describir ese contenido indeseado mediante criterios que él controle y que, al cumplirse en un mensaje, disparen la contención del emisor de la misma forma confiable que la detección de ráfaga. Esto le da al administrador un segundo eje de defensa, basado en el contenido del mensaje aislado, complementario al eje de conducta basado en la actividad del usuario.

A diferencia de la ráfaga, esta necesidad evalúa el mensaje en sí, sin necesidad de observar la actividad acumulada del usuario. El valor para el negocio es tener un mecanismo de bloqueo de contenido que reemplace con confiabilidad el comportamiento irregular del filtro nativo y que quede bajo el control directo del administrador.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un miembro publica un enlace que el administrador sabe que es una estafa que circula entre comunidades. No es una ráfaga: es un solo mensaje en un solo canal. El administrador quiere que ese tipo de contenido se reconozca apenas se publica y que el emisor quede contenido sin tener que esperar a que un humano vea el mensaje y reaccione, y sin depender de un filtro nativo que a veces lo deja pasar.

## 3. Impacto

- El administrador gana control directo sobre qué contenido se considera indeseado, sin depender del comportamiento errático del filtro nativo.
- Se cubre el spam que no llega como ráfaga sino como mensaje individual con contenido prohibido.
- El motor de evaluación gana un segundo eje, basado en el contenido del mensaje, complementario al de conducta.
- La comunidad queda protegida de material dañino aun fuera de los ataques distribuidos.
- Si la necesidad no se resuelve, el contenido indeseado de un solo mensaje sigue dependiendo de una herramienta que el cliente considera poco confiable.

## 4. Problema específico que resuelve

- El filtro nativo de la plataforma deja pasar de forma irregular contenido que debería bloquearse.
- No hay hoy un control de contenido bajo administración directa del administrador del sistema.
- El spam de mensaje individual con contenido prohibido escapa a la detección de ráfaga distribuida.
- No existe un mecanismo confiable para contener al emisor de un contenido indeseado apenas se publica.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Contención de contenido indeseado | Porcentaje de mensajes con contenido prohibido configurado que disparan la contención | ≥ 95 % | Continuo, por incidente, desde la puesta en producción |
| Confiabilidad frente al filtro nativo | Porcentaje de patrones de contenido definidos que se aplican de forma consistente, sin comportamiento errático | 100 % | Mensual |
| Autonomía de gestión del contenido | Porcentaje de altas y cambios de criterios de contenido aplicados por el administrador sin asistencia técnica | 100 % | Continuo, por cambio |
| Cobertura del mensaje individual | Porcentaje de incidentes de contenido prohibido en mensaje único contenidos automáticamente | ≥ 95 % | Mensual |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Define qué contenido se considera indeseado en sus comunidades |
| Fernando | Implementador | Construye el eje de evaluación de contenido del mensaje aislado |
| Administrador del sistema | Beneficiario | Valida que el control de contenido es confiable y está bajo su gestión |
| Miembros de las comunidades moderadas | Beneficiario indirecto | Reciben un servidor sin material dañino de mensaje individual |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-03 | CU-04 detectar contenido no deseado en un mensaje y contener al emisor | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (corte automático de la ráfaga), porque reutiliza el mismo mecanismo de contención del emisor que NB-01 establece, aplicado a un eje de evaluación distinto.

## 9. Prioridad MoSCoW

Must Have. El intake declara la detección de contenido por patrón como capacidad Must Have v1 y constituye un eje de defensa complementario al de ráfaga distribuida.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del alcance Must Have del intake y de la propuesta de valor. Reformulación del criterio de éxito de control del contenido a una métrica de resultado de negocio (observación P2-01 del audit de Fase A). |
