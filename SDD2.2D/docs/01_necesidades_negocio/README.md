# Necesidades de Negocio — discord-bots-admin

Esta sección contiene las necesidades de negocio (NB) del proyecto discord-bots-admin. El punto de entrada es el catálogo maestro [`necesidades-negocio_v1.0.md`](necesidades-negocio_v1.0.md), que mantiene la tabla resumen, el mapa de dependencias y la trazabilidad agregada. Cada NB se desarrolla en un archivo independiente bajo `necesidades-de-negocio/`.

Se incluye este README porque el proyecto tiene más de cinco NB (siete en total), conforme a la regla §3.4 de `01_rules_necesidades_negocio.md`.

## Tabla de necesidades

| NB-XX | Título | Impacto | Prioridad MoSCoW | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Corte automático de la ráfaga de spam distribuido | Detiene la propagación del spam en su inicio, sin intervención humana | Must | Propuesto | [archivo](necesidades-de-negocio/NB-01-corte-automatico-rafaga-distribuida_v1.0.md) |
| NB-02 | Limpieza retroactiva de los mensajes del incidente | Elimina en una operación los mensajes del emisor en todos los canales | Must | Propuesto | [archivo](necesidades-de-negocio/NB-02-limpieza-retroactiva-mensajes_v1.0.md) |
| NB-03 | Contención de contenido no deseado por patrón | Bloquea contenido prohibido de mensaje individual bajo control del administrador | Must | Propuesto | [archivo](necesidades-de-negocio/NB-03-contencion-contenido-no-deseado_v1.0.md) |
| NB-04 | Trazabilidad de incidentes y control de falsos positivos | Hace la moderación automática auditable y reversible | Must | Propuesto | [archivo](necesidades-de-negocio/NB-04-trazabilidad-incidentes-falsos-positivos_v1.0.md) |
| NB-05 | Configuración autónoma de la moderación por el administrador | Permite gobernar la moderación sin asistencia técnica | Must | Propuesto | [archivo](necesidades-de-negocio/NB-05-configuracion-autonoma-moderacion_v1.0.md) |
| NB-06 | Operación confiable y validación previa de la moderación | Asegura que la moderación activada sea efectivamente operativa | Should | Propuesto | [archivo](necesidades-de-negocio/NB-06-operacion-confiable-validacion-previa_v1.0.md) |
| NB-07 | Mitigación del riesgo de moderación errónea | Reduce los falsos positivos por diseño antes de que ocurran | Should | Propuesto | [archivo](necesidades-de-negocio/NB-07-mitigacion-moderacion-erronea_v1.0.md) |

## Mapa de dependencias

- NB-01: raíz, sin dependencias. Prerequisito de NB-02, NB-03, NB-04 y NB-07.
- NB-02: depende de NB-01.
- NB-03: depende de NB-01.
- NB-04: depende de NB-01 y NB-05.
- NB-05: raíz, sin dependencias. Prerequisito de NB-04, NB-06 y NB-07.
- NB-06: depende de NB-05.
- NB-07: depende de NB-01 y NB-05.

Las dependencias son acíclicas y ninguna NB depende de más de tres otras.

## Orden de lectura sugerido

1. NB-01 y NB-05 (necesidades raíz: corte y administración).
2. NB-02, NB-03 y NB-04 (limpieza, contenido y trazabilidad del núcleo).
3. NB-06 y NB-07 (confiabilidad operativa y prudencia de la moderación).

## RACI por NB

R: Responsable de ejecución. A: Aprobador. C: Consultado. I: Informado.

| NB-XX | Responsable (R) | Aprobador (A) | Consultado (C) | Informado (I) |
| --- | --- | --- | --- | --- |
| NB-01 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |
| NB-02 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |
| NB-03 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |
| NB-04 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |
| NB-05 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |
| NB-06 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |
| NB-07 | Fernando (implementador) | Administrador propietario | Administrador del sistema | Miembros de las comunidades |

El propietario del catálogo es el Analista de Negocio Senior (AG-01); el Product Manager (AG-00) y el Analista Funcional (AG-02) aportan revisión y validación, no autoría.
