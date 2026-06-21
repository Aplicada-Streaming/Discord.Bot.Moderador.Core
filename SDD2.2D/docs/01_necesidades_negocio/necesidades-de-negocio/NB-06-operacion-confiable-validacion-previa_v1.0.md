# NB-06 — Operación confiable y validación previa de la moderación

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-06-operacion-confiable-validacion-previa_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §4, §7, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-12, CU-13 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

El negocio necesita la garantía de que, cuando deja un servidor bajo moderación, esa moderación realmente va a funcionar. Una herramienta que falla en silencio es peor que no tener herramienta, porque genera una falsa sensación de seguridad mientras el servidor queda expuesto. Los modos de falla son concretos: una credencial de acceso inválida o revocada, permisos insuficientes para contener usuarios, una jerarquía que impide actuar sobre quien tiene un rol superior, o una caída del canal de eventos en tiempo real que deja al servidor sin vigilancia.

Hoy no hay forma de validar por adelantado que el servidor quedó bien configurado ni de saber, mientras opera, si la moderación sigue activa. La necesidad tiene dos partes. La primera es una validación previa al activar un servidor que verifique la credencial, los permisos, la capacidad de recibir eventos y los canales necesarios, y advierta de los faltantes antes de que el administrador confíe en una protección que no existe. La segunda es la confiabilidad en operación: que el servicio se recupere por sí solo de las caídas transitorias del canal de eventos y que el estado de conexión de cada servidor sea visible para el administrador.

Esta necesidad es defensiva: nace de los casos límite y de los riesgos del negocio. Su valor no es agregar funcionalidad de moderación, sino asegurar que la moderación contratada sea efectivamente operativa y que sus fallas sean visibles en lugar de silenciosas.

## 2. Ejemplo de uso desde la perspectiva del negocio

El administrador registra un servidor nuevo y, antes de activarlo, el sistema le avisa que la credencial es válida pero que al bot le falta el permiso para banear y que su rol está por debajo de algunos roles del staff, por lo que no podría actuar sobre ellos. El administrador corrige los permisos y recién entonces activa el servidor, con la certeza de que la moderación funcionará. Semanas después, una caída momentánea de la conexión se resuelve sola y el panel muestra en todo momento si cada servidor está conectado o no.

## 3. Impacto

- Elimina la falsa sensación de seguridad de creer protegido un servidor que en realidad no puede ser moderado.
- Reduce los incidentes no contenidos por credenciales inválidas, permisos faltantes o jerarquía insuficiente.
- Mantiene la moderación activa frente a caídas transitorias del canal de eventos mediante recuperación automática.
- Da visibilidad del estado de conexión de cada servidor para que el administrador detecte una pérdida de protección.
- Si la necesidad no se resuelve, el sistema puede dejar de proteger sin que nadie lo note, anulando el valor de las demás NB.

## 4. Problema específico que resuelve

- No hay validación previa de credencial, permisos, recepción de eventos y canales antes de activar un servidor.
- La jerarquía de roles puede impedir contener al emisor sin que el administrador esté advertido de antemano.
- Una caída del canal de eventos deja al servidor sin moderación sin recuperación automática garantizada.
- El administrador no tiene visibilidad de si cada servidor sigue conectado y protegido durante la operación.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Adopción de la validación previa | Porcentaje de servidores activados que ejecutaron la prueba de configuración antes de la activación | 100 % | Por alta de servidor |
| Detección anticipada de bloqueos | Porcentaje de activaciones con credencial inválida, permisos o jerarquía faltantes detectadas antes de activar | 100 % | Por alta de servidor |
| Recuperación tras caída de conexión | Porcentaje de caídas transitorias del canal de eventos resueltas con reconexión automática | ≥ 99 % | Mensual |
| Visibilidad del estado de conexión | Tiempo de actualización del estado conectado o desconectado de un servidor en el panel | ≤ 60 s | Por cambio de estado |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Exige certeza de que un servidor activado queda efectivamente protegido |
| Fernando | Implementador | Construye la prueba de configuración, la reconexión automática y el estado de conexión |
| Administrador del sistema | Beneficiario | Valida que recibe advertencias antes de activar y ve el estado de cada servidor |
| Miembros de las comunidades moderadas | Beneficiario indirecto | No quedan desprotegidos por fallas silenciosas de la moderación |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-06 | CU-12 probar la configuración de un servidor antes de activarlo | a generar |
| NB-06 | CU-13 reconectar automáticamente y mostrar el estado de conexión de cada servidor | a generar |

## 8. Dependencias con otras NB

Depende de NB-05 (configuración autónoma de la moderación), porque la validación previa y el estado de conexión se ejecutan y muestran sobre los servidores registrados desde el panel.

## 9. Prioridad MoSCoW

Should Have. La prueba de configuración es Should Have en el intake; eleva fuertemente la confiabilidad y mitiga los riesgos R-03 y R-04, pero el corte de spam puede demostrarse sin ella en el primer recorte.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del alcance Should Have, los casos límite y los riesgos R-03 y R-04 del intake |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: alineación de IDs de CU en §7 con la numeración canónica consolidada del 02 (CU-11, CU-12 reasignadas a CU-12, CU-13) |
