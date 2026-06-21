# NB-02 — Limpieza retroactiva de los mensajes del incidente

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-02-limpieza-retroactiva-mensajes_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-03 (prevista en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

Cortar la ráfaga deteniendo al emisor no alcanza si los mensajes ya publicados quedan ensuciando los canales. El negocio necesita que, en el mismo acto en que se contiene al responsable, se eliminen los mensajes que alcanzó a dejar en todos los canales afectados, sin que el administrador tenga que recorrer canal por canal borrando a mano. Hoy la limpieza es completamente manual y consume el tiempo del operador justo después del incidente, cuando además debe revisar el resto del daño.

La necesidad importa porque el costo del ataque no termina con el corte: persiste mientras los mensajes sigan visibles para los miembros. Una propagación a quince canales deja quince tareas de limpieza que, hechas a mano, pueden llevar mucho más tiempo que el ataque mismo y dejan huecos si el operador se saltea un canal. El negocio quiere que la limpieza sea una sola operación, retroactiva sobre la actividad reciente del emisor, acotada por una ventana hacia atrás que el administrador pueda ajustar dentro del tope que impone la plataforma.

Esta necesidad es complementaria del corte automático: el corte detiene la propagación futura y la limpieza retroactiva remueve la propagación ya ocurrida. Se reconoce y acepta que los mensajes una vez removidos no se restauran; lo que el negocio pide es eficacia y completitud de la limpieza, no su reversibilidad.

## 2. Ejemplo de uso desde la perspectiva del negocio

Tras un ataque, el administrador entra al servidor y, en lugar de encontrar la misma imagen repetida en diez canales, encuentra los canales ya limpios: el sistema, al contener al emisor, removió en una sola pasada todo lo que ese usuario había publicado en los minutos previos en cada canal. El administrador solo necesita confirmar que no quedó nada pendiente, en vez de dedicar media hora a borrar mensajes uno por uno.

## 3. Impacto

- El tiempo de limpieza posterior al incidente se reduce de una tarea manual multi-canal a una verificación breve.
- Se elimina el riesgo de dejar canales sucios por error u omisión del operador.
- La experiencia de los miembros se recupera de inmediato, sin restos del ataque visibles.
- La acción de contención y la de limpieza quedan acopladas en una sola operación de negocio.
- Si la necesidad no se resuelve, el ahorro del corte automático se diluye por la carga manual de limpiar lo ya publicado.

## 4. Problema específico que resuelve

- La limpieza manual posterior al ataque consume tiempo del administrador y es propensa a omisiones.
- No existe hoy una forma de borrar en bloque la actividad reciente de un emisor en todos los canales a la vez.
- El daño visible persiste mientras los mensajes no se remuevan, aun después de contener al responsable.
- La ventana de tiempo hacia atrás a limpiar no es configurable hoy según la naturaleza del incidente.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Limpieza efectiva de la ráfaga | Porcentaje de mensajes de la ráfaga eliminados dentro de los 10 s del incidente | ≥ 98 % | Por incidente, desde la puesta en producción |
| Reducción de limpieza manual | Cantidad de canales que el administrador debe limpiar a mano por incidente | 0 | Por incidente |
| Cobertura de la ventana hacia atrás | Mensajes del emisor dentro de la ventana configurada que quedan sin remover | 0 | Por incidente |
| Configurabilidad de la ventana de borrado | Rango de ajuste de la ventana hacia atrás disponible para el administrador | de 0 a 7 días | Disponible en v1 |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Define hasta cuánto tiempo hacia atrás debe limpiar la operación de contención |
| Fernando | Implementador | Construye la acción de borrado retroactivo acoplada a la contención |
| Administrador del sistema | Beneficiario | Valida que ya no debe limpiar canales a mano tras un incidente |
| Miembros de las comunidades moderadas | Beneficiario indirecto | Dejan de ver los restos del ataque en los canales |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-02 | CU-03 banear con borrado retroactivo de los mensajes del emisor | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (corte automático de la ráfaga), porque la limpieza retroactiva se dispara como parte del acto de contención del emisor.

## 9. Prioridad MoSCoW

Must Have. Sin la limpieza retroactiva, el corte automático deja a los canales inundados y el negocio sigue cargando con la limpieza manual que motiva el proyecto.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del alcance Must Have del intake y de la visión de producto |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: alineación del título de CU-03 en §7 con el título canónico del 02 |
