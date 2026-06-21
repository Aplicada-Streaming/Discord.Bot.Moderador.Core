# NB-01 — Corte automático de la ráfaga de spam distribuido

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-01-corte-automatico-rafaga-distribuida_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-01, CU-02 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

El negocio necesita que las inundaciones de spam que sufren los servidores dejen de propagarse antes de que un humano pueda reaccionar. El incidente típico nace cuando se vulnera la credencial de un miembro y un atacante usa esa cuenta para enviar mensajes, habitualmente imágenes adjuntas, a un canal tras otro en pocos segundos. La defensa actual depende de la moderación manual y del filtro nativo de la plataforma configurado con expresiones regulares, que se comporta de forma errática y no corta la ráfaga repartida entre canales.

La consecuencia hoy es que, para cuando alguien reacciona, el spam ya inundó diez o veinte canales y debe limpiarse a mano. Eso degrada la experiencia de los miembros, sobrecarga al administrador y pone en riesgo la permanencia de la comunidad. La necesidad central es disponer de un mecanismo que reconozca el patrón que de verdad delata al spam automatizado, el envío casi simultáneo a múltiples canales distintos, algo físicamente imposible para un humano, y que corte ese envío de inmediato sin esperar intervención.

Esta es la necesidad que justifica el proyecto: sin ella, las demás capacidades de configuración, reporte y revisión carecen de un disparador que las accione. El indicador que distingue al ataque del uso legítimo es la cantidad de canales distintos alcanzados en una ventana corta, no la cantidad de mensajes, porque un miembro entusiasta puede publicar mucho en un solo canal sin ser spam.

## 2. Ejemplo de uso desde la perspectiva del negocio

Una cuenta comprometida empieza a publicar la misma imagen promocional en el canal de bienvenida, después en el de anuncios, después en tres canales temáticos, todo en cuestión de segundos. El administrador, que está durmiendo o fuera de línea, no se entera hasta la mañana siguiente, cuando encuentra quince canales con la misma basura repetida y miembros quejándose. Lo que el negocio espera es que esa propagación se detenga sola en cuanto el emisor toca el segundo o tercer canal distinto, y que el responsable quede fuera del servidor sin que nadie haya tenido que mirar la pantalla.

## 3. Impacto

- La experiencia de los miembros deja de degradarse durante la franja en que no hay un moderador humano disponible.
- La carga operativa de limpieza manual del administrador se reduce a los casos excepcionales que el patrón no cubre.
- El riesgo de pérdida de comunidad por inundaciones recurrentes disminuye al cortarse el ataque en su inicio.
- El motor de detección de conducta y la integración con el canal de eventos en tiempo real quedan comprometidos como el núcleo del sistema.
- Si la necesidad no se resuelve, el resto de las capacidades del producto pierden razón de ser, porque ninguna reacción posterior recupera los canales ya inundados.

## 4. Problema específico que resuelve

- La moderación manual no llega a tiempo frente a una propagación que dura segundos.
- El filtro nativo de la plataforma no corta de forma confiable la ráfaga repartida entre canales.
- No existe hoy un criterio automatizado que distinga el spam distribuido del uso legítimo intenso en un solo canal.
- No hay forma de detener al emisor en el inicio del ataque sin presencia humana.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Corte automático de la ráfaga | Porcentaje de incidentes de ráfaga cortados automáticamente, sin intervención manual | ≥ 95 % | Continuo, por incidente, desde la puesta en producción |
| Distinción del uso legítimo | Porcentaje de actividad intensa en un solo canal que NO dispara la regla de ráfaga | 100 % | Por incidente, desde la puesta en producción |
| Cobertura de la franja sin moderador | Porcentaje de incidentes nocturnos o sin operador en línea contenidos automáticamente | ≥ 95 % | Mensual |
| Disponibilidad del servicio de detección | Disponibilidad mensual del servicio de moderación en operación | ≥ 99 % | Mensual |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Aprueba la prioridad y define el nivel de sensibilidad de la detección |
| Fernando | Implementador | Construye y mantiene el motor de detección y la integración con el canal de eventos |
| Administrador del sistema | Beneficiario | Valida que el corte automático reduce su carga de limpieza manual |
| Miembros de las comunidades moderadas | Beneficiario indirecto | Reciben un servidor que no se inunda durante la franja sin moderador |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-01 | CU-01 detectar ráfaga distribuida por canales distintos en ventana corta | a generar |
| NB-01 | CU-02 banear automáticamente al emisor de la ráfaga | a generar |

## 8. Dependencias con otras NB

Sin dependencias. Es la necesidad raíz del producto; las demás dependen de ésta o la complementan.

## 9. Prioridad MoSCoW

Must Have. Es el dolor central declarado en SOLUTION-INTAKE §1 y el objetivo principal del proyecto; sin esta NB no hay producto mínimo viable defendible.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del dolor central del intake y de la visión de producto |
