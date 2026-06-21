# Visión de Producto

**Proyecto:** discord-bots-admin
**Documento:** vision-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Product Manager Senior (AG-00)
**Trazabilidad upstream:** SOLUTION-INTAKE §1, §2, §3, §8, §9, §10, §11, §12
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 03_ux_ui_dx, 05_arquitectura_tecnica, 07_plan-sprint, 11_examples

## 1. Problema de negocio

Los servidores de Discord del solicitante sufren ataques de spam. El caso más común ocurre cuando se vulneran las credenciales de un usuario del servidor y un atacante usa esa cuenta para enviar ráfagas de spam, habitualmente imágenes adjuntas, a todos los canales uno tras otro en pocos segundos. La moderación manual no llega a tiempo: para cuando un humano reacciona, el spam ya inundó diez o veinte canales y hay que limpiarlos a mano.

El mecanismo de defensa con el que cuenta hoy el solicitante es el filtro nativo de la plataforma configurado con expresiones regulares. Ese filtro tiene un comportamiento errático frente a este patrón y no corta la ráfaga repartida entre canales: llega un primer mensaje de spam y, al rato, llegan en ráfaga los mensajes a los demás canales.

Si no se construye la solución en los próximos meses, los servidores seguirán expuestos a inundaciones de spam ante cada cuenta comprometida, con la consiguiente carga de limpieza manual, degradación de la experiencia de los miembros y riesgo de pérdida de comunidad. El disparador es concreto y actual: el solicitante está sufriendo estos incidentes y el mecanismo actual no los contiene.

## 2. Audiencia y stakeholders

| Rol | Nombre o cargo | Categoría | Nivel de involucramiento | Responsabilidad principal |
| --- | --- | --- | --- | --- |
| Dueño del problema | Administrador propietario de los servidores de Discord | Propietario | Alto, decisor | Aprueba el alcance, define la sensibilidad de la moderación y opera el sistema en producción |
| Usuario administrador | Administrador del sistema (rol único de la aplicación) | Beneficiario / operador | Alto, uso diario | Registra servidores, configura reglas y eventos, revisa incidentes y revierte falsos positivos |
| Implementador | Fernando | Implementador | Alto, construcción | Construcción y mantenimiento del sistema, responsable técnico y de arquitectura, con asistencia de IA en el desarrollo |
| Miembros de los servidores | Comunidades de Discord moderadas | Beneficiario indirecto | Bajo, pasivo | Reciben un servidor libre de spam, sin interactuar con el sistema |

## 3. Propuesta de valor

Hoy el cliente confía la moderación a un filtro nativo configurado con expresiones regulares, que no corta de forma confiable las ráfagas de spam repartidas entre canales y se comporta de manera irregular.

La promesa central del producto es detectar el patrón que de verdad delata al spam automatizado, el envío casi simultáneo a múltiples canales distintos, algo físicamente imposible para un humano, y cortarlo al instante baneando al emisor. La misma operación limpia los mensajes del emisor en todos los canales de una sola vez y deja un reporte en un canal privado para revisar posibles falsos positivos.

Diferenciadores defendibles del producto:

- Un motor de reglas configurable de estilo firewall multi-contexto, adaptado a los patrones de spam reales del cliente.
- Auto-hospedado en hardware propio del cliente, sin dependencia de servicios de terceros y sin costo recurrente de infraestructura.
- Capaz de operar varios servidores de moderación desde una sola instancia.
- Configuración asistida en pantalla: cada parámetro trae su valor por defecto, su leyenda y ejemplos, de modo que el administrador ajusta la sensibilidad sin conocimiento técnico profundo.
- Una frontera reservada para que, a futuro, un asistente automatizado proponga configuraciones que el administrador previsualiza y confirma antes de aplicar.

## 4. Visión a 3 años

A tres años, el producto es la herramienta de moderación de cabecera del solicitante para sus comunidades de Discord: corta de forma confiable las ráfagas de spam distribuido apenas comienzan, mantiene a las comunidades libres de inundaciones sin esfuerzo manual y permite al administrador ajustar la moderación desde un panel sin tocar la configuración del servidor.

El motor de reglas evoluciona desde la detección de ráfaga distribuida hacia un catálogo de políticas de conducta y de contenido que el administrador combina, prueba en modo simulación y promueve a ejecución con confianza. La revisión de incidentes y la reversión de baneos se vuelven una rutina rápida que minimiza el costo de los falsos positivos.

Sobre la frontera reservada de propuesta de configuración asistida, en el horizonte se incorpora un asistente que sugiere ajustes a partir de los incidentes observados, siempre con previsualización y confirmación humana antes de aplicar. La operación se extiende de un servidor a varios servidores a medida que el hardware del cliente lo permita, manteniendo la promesa de auto-hospedaje sin dependencia de terceros.

## 5. Objetivos SMART

| Objetivo | Métrica | Target | Plazo | Responsable |
| --- | --- | --- | --- | --- |
| Cortar el spam de ráfaga sin intervención humana | Porcentaje de incidentes de ráfaga cortados automáticamente sobre el total de incidentes | ≥ 95% | Continuo, medido por incidente desde la puesta en producción | Administrador propietario |
| Mantener bajos los falsos positivos | Porcentaje de baneos revertidos por ser falso positivo sobre el total de baneos | ≤ 2% | Revisión mensual | Administrador del sistema |
| Limpiar la ráfaga de forma efectiva | Porcentaje de mensajes de la ráfaga eliminados dentro de los 10 segundos del incidente | ≥ 98% | Medido por incidente | Administrador propietario |

## 6. Métricas de éxito

| Criterio | Métrica | Target | Plazo | Fuente del dato |
| --- | --- | --- | --- | --- |
| Corte automático de spam | Porcentaje de incidentes de ráfaga cortados automáticamente, sin intervención manual | ≥ 95% | Continuo, por incidente, desde la puesta en producción | Registro de incidentes del sistema |
| Falsos positivos | Porcentaje de baneos revertidos por ser falso positivo | ≤ 2% | Mensual | Registro de incidentes y de desbaneos |
| Limpieza efectiva | Porcentaje de mensajes de la ráfaga eliminados dentro de los 10 s del incidente | ≥ 98% | Por incidente | Registro de incidentes con canales afectados |
| Adopción de la prueba previa | Porcentaje de servidores activados que ejecutaron la prueba de configuración antes de la activación | 100% | Por alta de servidor | Registro de altas de servidor |

Nota: los tiempos de la regla (ventana de detección, por defecto del orden de 2 s) y del baneo (ventana de borrado retroactivo configurable) son parámetros de configuración, no métricas de éxito; viven en la configuración de la regla y de la acción. La latencia de procesamiento se trata como requisito no funcional y se define en la categoría 05 arquitectura técnica.

## 7. Restricciones

- Plataforma de despliegue impuesta por el cliente: hardware propio de bajo consumo del cliente, auto-hospedado, sin contenedores, instalado como servicio del sistema mediante un paquete que incluye todo lo necesario. El detalle de plataforma y runtime se define en `alcance-proyecto_v1.0.md` y en la categoría 09 DevOps.
- Integración obligatoria con Discord, a través de su interfaz de programación y su canal de eventos en tiempo real, con una credencial de acceso por servidor que el solicitante obtiene de cada servidor a moderar.
- Sin presupuesto formal: proyecto propio; el costo se reduce al hardware ya disponible más el tiempo de desarrollo.
- Sin fecha objetivo contractual: desarrollo incremental por sprints, sin deadline fijo.
- Tratamiento de datos personales conforme al marco legal local aplicable; residencia local de los datos en el dispositivo del cliente, sin terceros. El detalle normativo se trabaja en las categorías 05 arquitectura técnica y 08 calidad.

## 8. Riesgos

| ID | Riesgo | Probabilidad | Impacto | Mitigación | Responsable |
| --- | --- | --- | --- | --- | --- |
| R-01 | Falsos positivos: banear a un usuario legítimo | Media | Alto | Discriminar por canales distintos y no por cantidad de mensajes; exenciones de staff; modo simulación previo a la ejecución real; reporte a canal privado y panel de incidentes con desbaneo para revertir | Administrador del sistema |
| R-02 | El atacante evade el patrón randomizando nombres de archivo o espaciando el envío | Media | Medio | Discriminar por canales distintos, robusto al espaciado; borrado retroactivo que limpia lo previo; ventana de detección configurable | Implementador |
| R-03 | El emisor del spam tiene un rol por encima del bot y no se lo puede banear | Media | Medio | La prueba de configuración advierte sobre jerarquía de roles y permisos faltantes antes de activar el servidor; el sistema registra y reporta el caso | Administrador del sistema |
| R-04 | Caída del canal de eventos de Discord o credencial revocada deja al servidor sin moderación | Baja | Alto | Reconexión automática; estado de conexión visible en el panel; prueba de credencial al registrar y re-validación posterior | Implementador |

## 9. Glosario del dominio

| Término | Definición | Sinónimos o notas |
| --- | --- | --- |
| Ráfaga distribuida (fan-out) | Envío casi simultáneo de mensajes a varios canales distintos, típico de un bot de spam o de una cuenta comprometida | "ráfaga", "fan-out" |
| Evento o política | Conjunto de grupos de reglas que, al cumplirse, dispara un conjunto de acciones | "política de moderación" |
| Regla de contenido | Predicado que evalúa un mensaje aislado, por ejemplo por expresión regular o por palabras clave | — |
| Regla de conducta | Predicado que evalúa la actividad reciente del usuario, por ejemplo su frecuencia o sus canales distintos | — |
| Exención | Rol, usuario o canal de confianza excluido de la moderación, por ejemplo el staff | "whitelist" |
| Modo simulación | Estado en que una política registra lo que haría sin ejecutar la acción | "log-only", "dry-run" |
| Borrado retroactivo | Borrado de los mensajes recientes del usuario hacia atrás dentro de una ventana, al momento del baneo | "purga", "prune" |
| Desbaneo | Reversión de un baneo desde el panel; revierte el baneo pero no restaura los mensajes borrados | — |
| Incidente | Registro de un disparo de evento con su copia de mensajes, canales afectados y acción resultante | — |
| Canal de salida | Canal designado con un nombre lógico al que el sistema envía sus reportes, por ejemplo el registro de moderación | — |

## 10. Trazabilidad

- Upstream: SOLUTION-INTAKE §1 (idea y problema), §2 (audiencia y stakeholders), §3 (propuesta de valor y diferenciación), §8 (métricas de éxito de negocio), §9 (exclusiones), §10 (restricciones del cliente), §11 (riesgos de negocio), §12 (glosario del dominio del cliente).
- Downstream: alimenta 01_necesidades_negocio (necesidades NB-XX derivadas del problema y los objetivos), 02_especificacion_funcional (casos de uso y reglas de moderación), 03_ux_ui_dx (panel de administración y ayuda contextual), 05_arquitectura_tecnica (estilo, NFR y restricciones de plataforma), 07_plan-sprint (priorización y secuencia de entrega), 11_examples (samples demostrables de la solución).
