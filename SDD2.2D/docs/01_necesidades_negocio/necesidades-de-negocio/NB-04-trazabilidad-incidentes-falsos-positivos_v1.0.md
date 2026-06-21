# NB-04 — Trazabilidad de incidentes y control de falsos positivos

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-04-trazabilidad-incidentes-falsos-positivos_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §4, §8, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-05, CU-06, CU-07 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

Una moderación que actúa sola, sin supervisión humana, necesita ser auditable y reversible para que el negocio confíe en ella. Cuando el sistema contiene a un emisor de forma automática, existe el riesgo de que se trate de un usuario legítimo; ese es el riesgo de mayor impacto identificado para el negocio. La necesidad es que cada acción quede registrada con la evidencia que la motivó, los mensajes que la dispararon y los canales afectados, y que el administrador pueda revisarla y, si fue un error, revertirla.

Hoy no existe un registro consolidado de lo que la moderación hizo ni un canal de revisión que permita detectar a tiempo un falso positivo. Sin esa trazabilidad, el administrador no puede medir si la moderación está siendo justa, no puede explicar por qué se contuvo a un miembro, y no tiene forma ordenada de corregir un error. El negocio necesita dos superficies de control: un reporte inmediato a un canal privado para enterarse del evento apenas ocurre, y una revisión posterior desde el panel que conserve la copia de los mensajes accionados aun cuando ya hayan sido removidos del servidor.

La reversibilidad es parte de la necesidad: el negocio acepta que los mensajes removidos no se restauran, pero exige que la contención del usuario sí pueda revertirse cuando la revisión concluye que fue un falso positivo. Esta necesidad es la que hace tolerable la automatización, porque acota el costo de un error y da al administrador visibilidad y capacidad de corrección.

## 2. Ejemplo de uso desde la perspectiva del negocio

El sistema contiene a un usuario por una ráfaga, pero resulta que era un miembro probando un bot legítimo en varios canales con permiso del staff. En un canal privado de moderación aparece de inmediato el reporte con los mensajes que dispararon la acción y la lista de canales afectados. El administrador entra al panel, revisa la copia de esos mensajes, concluye que fue un falso positivo y revierte la contención del usuario desde la misma pantalla, sin tener que entrar al servidor a investigar.

## 3. Impacto

- Hace tolerable la automatización al acotar el costo y la visibilidad de los errores de moderación.
- Da al administrador la evidencia para explicar y justificar cada acción ante la comunidad.
- Permite medir la tasa de falsos positivos como indicador de calidad de la moderación.
- Conserva la copia de los mensajes accionados para revisión aun después de que se removieron del servidor.
- Si la necesidad no se resuelve, la moderación automática se vuelve riesgosa e injustificable, y el negocio no la adoptaría por miedo a banear miembros legítimos sin posibilidad de corregir.

## 4. Problema específico que resuelve

- No hay hoy un registro consolidado de las acciones de moderación ni de la evidencia que las motivó.
- El administrador no se entera a tiempo de una acción automática para detectar un falso positivo.
- No existe forma de revisar la evidencia de una acción una vez que los mensajes se removieron del servidor.
- No hay un mecanismo ordenado para revertir la contención de un usuario erróneamente moderado.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Tasa de falsos positivos controlada | Porcentaje de contenciones revertidas por ser falso positivo sobre el total | ≤ 2 % | Mensual |
| Cobertura de reporte de incidentes | Porcentaje de acciones de moderación reportadas al canal privado con sus mensajes y canales afectados | 100 % | Por incidente |
| Trazabilidad de la evidencia | Porcentaje de incidentes con copia de los mensajes accionados conservada para revisión | 100 % | Por incidente |
| Tiempo de reversión de un falso positivo | Minutos desde que el administrador abre el incidente hasta que revierte la contención | ≤ 3 min | Por revisión |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Exige que la moderación automática sea auditable y reversible para aprobarla |
| Fernando | Implementador | Construye el registro de incidentes, el reporte al canal privado y la reversión |
| Administrador del sistema | Beneficiario | Valida que puede revisar y corregir falsos positivos sin entrar al servidor |
| Miembros de las comunidades moderadas | Beneficiario indirecto | Reciben corrección rápida si fueron contenidos por error |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-04 | CU-05 reportar a un canal privado los mensajes accionados y los canales afectados | a generar |
| NB-04 | CU-06 revisar incidentes y mensajes accionados desde el panel | a generar |
| NB-04 | CU-07 revertir una contención (desbaneo) desde el panel | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (corte automático de la ráfaga) y de NB-05 (configuración y operación del panel), porque registra y reporta las acciones que NB-01 genera y se revisa desde el panel que NB-05 provee.

## 9. Prioridad MoSCoW

Must Have. El reporte de incidentes es Must Have en el intake y mitiga el riesgo de mayor impacto del negocio (R-01, falsos positivos); sin trazabilidad y reversión la automatización no se adopta.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del alcance Must Have, las métricas y el riesgo R-01 del intake |
