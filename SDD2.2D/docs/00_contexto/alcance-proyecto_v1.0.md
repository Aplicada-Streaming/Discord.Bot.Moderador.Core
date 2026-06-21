# Alcance del Proyecto

**Proyecto:** discord-bots-admin
**Documento:** alcance-proyecto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Product Manager Senior (AG-00)
**Trazabilidad upstream:** SOLUTION-INTAKE §1, §4, §5, §6, §9, §10, §17 P.9
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 03_ux_ui_dx, 05_arquitectura_tecnica, 07_plan-sprint, 11_examples

## 1. Propósito

Este documento delimita qué construye el proyecto y qué deja explícitamente afuera, para que las categorías posteriores trabajen sobre un alcance estable y no generen funcionalidad por fuera de lo acordado. El propósito del proyecto es entregar un sistema de administración y moderación de servidores de Discord que detecte y corte automáticamente las ráfagas de spam distribuido, con un panel para configurar la moderación y revisar incidentes.

## 2. Descripción general

El sistema es una aplicación auto-hospedada que el administrador opera desde un panel web. El administrador registra un servidor de Discord con su credencial de acceso, prueba la configuración, define reglas y políticas de moderación, y deja al sistema vigilando los mensajes en tiempo real. Cuando un mensaje cumple una política, el sistema ejecuta las acciones configuradas, típicamente reportar a un canal privado y banear con borrado retroactivo de los mensajes del emisor, y registra el incidente para su revisión posterior.

El centro de la propuesta es la detección del patrón de ráfaga distribuida: un usuario que publica en varios canales distintos dentro de una ventana corta de tiempo. La métrica de canales distintos, en lugar de la cantidad de mensajes, es lo que distingue al spam automatizado de un miembro entusiasta que publica mucho en un solo canal.

## 3. Objetivos del proyecto

- Detectar y cortar automáticamente las ráfagas de spam distribuido apenas comienzan, sin intervención manual.
- Minimizar el costo de los errores de moderación mediante modo simulación previo, reporte a canal privado, revisión de incidentes y desbaneo.
- Permitir que el administrador configure la sensibilidad de la moderación desde el panel, con ayuda contextual, sin conocimiento técnico profundo.
- Operar de forma auto-hospedada en hardware propio del cliente, sin dependencia de servicios de terceros.

## 4. Alcance incluido

### 4.1 Capacidades

- Detección del patrón de ráfaga distribuida (un usuario que publica en N canales distintos dentro de una ventana corta) y baneo automático del emisor.
- Baneo con borrado retroactivo de los mensajes del usuario en todos los canales, con la ventana hacia atrás configurable hasta el tope que impone la plataforma (7 días).
- Detección de contenido no deseado en un mensaje mediante expresión regular, con baneo del emisor.
- Reporte a un canal privado de los mensajes que dispararon la acción y de los canales afectados, para revisión de falsos positivos.
- Panel web de administración para registrar un servidor con su credencial y administrar reglas, grupos de reglas, eventos y acciones.
- Usuario administrador único, con alta de credenciales en el primer ingreso.
- Umbrales y ventanas configurables por el administrador, con valor por defecto y ayuda contextual en pantalla (leyenda y ejemplos por parámetro).
- Reglas de conducta por volumen de mensajes de un usuario en un canal y reglas de contenido por palabras o frases clave.
- Acciones adicionales sobre el usuario: timeout, expulsión, asignar o quitar rol.
- Exenciones por rol, usuario o canal de confianza, por ejemplo el staff.
- Modo simulación por evento, antirrebote por usuario y prueba de configuración al registrar un servidor.
- Revisión de incidentes y mensajes accionados desde el panel, y reversión de un baneo (desbaneo) desde el panel.

### 4.2 Entregables

- Sistema instalable y operable de punta a punta sobre la plataforma de despliegue del cliente.
- Paquete de instalación que incluye todo lo necesario y deja el sistema corriendo como servicio.
- Documentación de las categorías 00 a 11 del proceso SDD.
- Samples demostrables de la solución (detallados en la categoría 11_examples).

### 4.3 Ambientes

- Ambiente de producción: instancia auto-hospedada en el hardware del cliente, operada por el administrador.
- Ambiente de desarrollo y validación: estación del implementador, con la integración cruzada de compilación hacia la plataforma de destino. El detalle de ambientes, compilación y publicación se define en la categoría 09 DevOps.

## 5. Alcance excluido

| Funcionalidad excluida | Justificación | Versión futura tentativa |
| --- | --- | --- |
| Reemplazo o gestión de convivencia con el filtro nativo de Discord | El motor propio se enfoca en el patrón de ráfaga distribuida que el filtro nativo no corta de forma confiable; la coordinación entre ambos queda fuera de v1 | No planificada |
| Asistente que propone configuraciones de moderación de forma automática | Alcance acotado; v1 solo reserva la frontera para enchufarlo más adelante, con previsualización y confirmación humana | Posterior a v1 |
| Operación multi-servidor a escala masiva | Límite de memoria del hardware de despliegue de 32 bits; v1 se prevé para un servidor | Posterior a v1, sujeta a hardware |
| Restauración de mensajes borrados | Una vez que el baneo los purga, no se pueden recuperar por la interfaz de Discord; el sistema conserva solo una copia para revisión y el baneo sí es reversible (desbaneo) | No posible por la plataforma |
| Moderación por reputación, historial de usuario o apelaciones automatizadas | Queda fuera del objetivo de cortar el spam | No planificada |
| Anidamiento booleano de reglas más allá de dos niveles | Se acota la complejidad de configuración a grupo de reglas y combinación de grupos | No planificada |

## 6. Supuestos

- El administrador puede obtener una credencial de acceso de bot por cada servidor a moderar y otorgarle los permisos necesarios.
- El bot opera con un rol con jerarquía suficiente para banear y borrar mensajes de los usuarios objetivo; los casos de jerarquía insuficiente se detectan y reportan.
- El hardware del cliente dispone de conectividad estable a Discord; las caídas transitorias se resuelven con reconexión automática.
- Hay un único administrador del sistema; no se requiere gestión de múltiples cuentas ni roles.
- El cliente acepta que el estado de conducta en curso pueda perderse ante un reinicio del servicio, según el trade-off declarado en el intake.

## 7. Restricciones

- Plataforma de despliegue impuesta por el cliente: hardware propio de bajo consumo, sistema operativo de 32 bits, auto-hospedado, sin contenedores, instalado como servicio del sistema. Las versiones de sistema operativo, runtime y navegadores soportados se documentan en la categoría 09 DevOps; los navegadores del panel son evergreen (últimas dos versiones de los principales más una versión mínima de Safari, según SOLUTION-INTAKE §17 P.9).
- Integración obligatoria con Discord, mediante su interfaz de programación y su canal de eventos en tiempo real, con una credencial por servidor.
- Sin presupuesto formal y sin fecha objetivo contractual; el desarrollo es incremental por sprints.
- El borrado retroactivo de mensajes está limitado a 7 días por la plataforma de Discord; restricción aceptada.
- Residencia local de los datos en el dispositivo del cliente, sin terceros, con minimización y retención acotada conforme al marco legal local aplicable.

## 8. Criterios de aceptación del proyecto

- El sistema detecta una ráfaga distribuida simulada y, según el modo configurado, reporta o ejecuta el baneo con borrado retroactivo.
- El administrador puede registrar un servidor con su credencial, ejecutar la prueba de configuración y recibir advertencias de permisos o jerarquía faltantes antes de activar.
- El administrador puede crear reglas, agruparlas, definir un evento con sus acciones y dejarlo en modo simulación, apoyándose en la ayuda contextual de cada parámetro.
- El sistema reporta a un canal privado los mensajes que dispararon una acción y los canales afectados.
- El administrador puede revisar un incidente desde el panel y revertir un baneo (desbaneo).
- Las exenciones por rol, usuario o canal de confianza excluyen efectivamente a esos sujetos de la moderación.
- El sistema se instala y queda corriendo como servicio sobre la plataforma de despliegue del cliente.

## 9. Gestión de cambios de alcance

Todo cambio de alcance se registra como una solicitud que el administrador propietario aprueba antes de incorporarse. Las exclusiones de la sección 5 solo se reincorporan al alcance mediante una nueva versión de este documento, con la justificación del cambio. Las capacidades de las categorías Should Have y Could Have del intake que se promuevan a v1, o las que se pospongan, se reflejan en `roadmap-producto_v1.0.md` y se sincronizan con el backlog (categoría 06) y el plan de sprint (categoría 07). Cualquier capacidad nueva que afecte la plataforma de despliegue se evalúa contra las restricciones de la sección 7 antes de aceptarse.

## 10. Trazabilidad

- Upstream: SOLUTION-INTAKE §1 (problema), §4 (alcance funcional MoSCoW), §5 (historias de usuario), §6 (flujos típicos), §9 (exclusiones), §10 (restricciones del cliente), §17 P.9 (compatibilidad y plataformas target).
- Downstream: alimenta 01_necesidades_negocio (necesidades de negocio derivadas del alcance), 02_especificacion_funcional (casos de uso dentro y fuera de alcance), 03_ux_ui_dx (alcance del panel y ayuda contextual), 05_arquitectura_tecnica (NFR, restricciones de plataforma y estilo), 07_plan-sprint (alcance por sprint), 11_examples (samples que ejercitan el alcance incluido).
