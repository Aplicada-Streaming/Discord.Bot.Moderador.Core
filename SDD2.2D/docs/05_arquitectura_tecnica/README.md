# 05 Arquitectura técnica — discord-bots-admin

**Proyecto:** discord-bots-admin
**Código del proyecto:** DiscordModeradorBot.Servicio
**Tipo (D8):** web-monolith
**Versión:** 1.0
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)

Índice navegable de la arquitectura técnica del servicio monolítico de administración y moderación. La solución es de un único proyecto (caso degenerado): la vista de solución `_solucion/` se omite por completo y no existe carpeta huérfana.

## Documento maestro

- [arquitectura-solucion_v1.0.md](arquitectura-solucion_v1.0.md): estilo (capas con núcleo de pipeline y firewall multi-contexto), cuatro vistas (lógica, procesos, despliegue, datos), cross-cutting con seguridad reforzada, tabla de NFR con valores numéricos y trazabilidad de los 16 CU.

## Modelo de datos lógico

- [modelo-datos-logico_v1.0.md](modelo-datos-logico_v1.0.md): 13 tablas trazadas al modelo conceptual de 02, tipos físicos, índices, restricciones, migración inicial `MIG-0001-esquema-inicial`, snowflakes como texto, token cifrado en reposo, declaración no multi-tenant (multi-contexto).

## Flujo de ejecución

- [flujo-ejecucion_v1.0.md](flujo-ejecucion_v1.0.md): pipeline de evaluación de un mensaje en 9 etapas (descarte de exentos, contenido, conducta en memoria, políticas por prioridad, copia de mensajes, modo, antirrebote, ejecución de acciones, registro del incidente).

## Extensibilidad

- [extensibilidad_v1.0.md](extensibilidad_v1.0.md): superficie de configuración dirigida por descriptores (nuevos descriptores, tipos de regla y de acción) y frontera reservada de propuesta de configuración (no construida en v1).

## Decisiones de arquitectura (ADR)

- [decisiones-arquitectura_v1.0.md](decisiones-arquitectura_v1.0.md): índice navegable de los ADR con estado y fecha.

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-monolitico-capas-pipeline_v1.0.md) | Estilo monolítico de capas con núcleo de pipeline | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-persistencia-relacional-embebida-wal_v1.0.md) | Persistencia en base relacional embebida en modo WAL | Persistencia | Aceptado |
| [ADR-03](adrs/ADR-03-autenticacion-administrador-unico-hash-robusto_v1.0.md) | Autenticación de administrador único con hash robusto | Seguridad | Aceptado |
| [ADR-04](adrs/ADR-04-separacion-capas-dominio-independiente_v1.0.md) | Separación de capas con dominio independiente | Estilo | Aceptado |
| [ADR-05](adrs/ADR-05-despliegue-self-contained-arm-servicio-sistema_v1.0.md) | Despliegue self-contained para ARM con servicio del sistema | Despliegue | Aceptado |
| [ADR-06](adrs/ADR-06-compliance-ley-25326-proteccion-datos_v1.0.md) | Cumplimiento de la Ley 25.326 de Protección de Datos Personales | Seguridad | Aceptado |
| [ADR-07](adrs/ADR-07-cifrado-tokens-reposo-clave-maestra_v1.0.md) | Cifrado de tokens en reposo con clave maestra por variable de entorno | Seguridad | Aceptado |
| [ADR-08](adrs/ADR-08-manejo-errores-pipeline-resultados-moderacion_v1.0.md) | Manejo de errores del pipeline y resultados de moderación | Estilo | Aceptado |
| [ADR-09](adrs/ADR-09-estado-conducta-antirrebote-en-memoria_v1.0.md) | Estado de conducta y antirrebote en memoria, no persistidos | Persistencia | Aceptado |
| [ADR-10](adrs/ADR-10-omision-contratos-prompts-ai_v1.0.md) | Omisión de la categoría de contratos de prompts AI (sin LLM en v1) | Estilo | Aceptado |
| [ADR-11](adrs/ADR-11-colapso-developer-guide-en-readmes_v1.0.md) | Colapso de la developer guide en READMEs | Estilo | Aceptado |
| [ADR-12](adrs/ADR-12-configuracion-dirigida-por-esquema_v1.0.md) | Configuración dirigida por esquema (descriptores como fuente única) | Extensibilidad | Aceptado |
| [ADR-13](adrs/ADR-13-dominio-firewall-multi-contexto_v1.0.md) | Dominio como firewall multi-contexto | Estilo | Aceptado |

## NFR (resumen)

Valores de `SOLUTION-INTAKE §17 P.10`; detalle y mecanismo de medición en `arquitectura-solucion_v1.0.md §8`.

| NFR | Objetivo | ADR |
| --- | --- | --- |
| Latencia de procesamiento por mensaje | p95 < 200 ms | ADR-01, ADR-09 |
| Throughput sostenido | ≥ 50 mensajes/s | ADR-01, ADR-09 |
| Disponibilidad mensual | 99 % | ADR-05, ADR-13 |
| Memoria por conexión de gateway | ≤ 8 MB | ADR-08 |
| Cobertura del módulo de detección | ≥ 90 % | ADR-04 |

## Decisión sobre contratos externos

No se generan documentos `contratos-<area>`: web-monolith solo los genera si expone una API externa. Este proyecto integra con la plataforma de mensajería como cliente (gateway y API) y no expone ninguna API a terceros (`SOLUTION-INTAKE §17 P.3`). Por lo tanto, no hay contrato externo que documentar. La integración como cliente se describe en `arquitectura-solucion_v1.0.md §3` (componente Adaptador del gateway) y en ADR-13.

## Decisión sobre la vista de solución

La solución tiene un único proyecto (`SOLUTION-INTAKE §13`). La vista de solución `_solucion/` se omite por completo (caso degenerado); su contenido se reduce a esta arquitectura del único proyecto. No existe carpeta `_solucion/` huérfana.

## Trazabilidad

- Upstream: cada ADR referencia las NB/CU/RN/NFR que la motivan; cada componente de la vista lógica lista los CU que cubre; el modelo lógico referencia el modelo conceptual de 02 entidad por entidad.
- Downstream: alimenta 06 (US y backlog técnico), 07 (plan de sprint), 08 (tests previstos en las tablas de trazabilidad), 09 (DevOps y despliegue), 10 (colapsada en READMEs, ADR-11) y 11 (samples).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | README inicial de la sección 05 con índice del documento maestro, modelo lógico, flujo, extensibilidad, los 13 ADR y el resumen de NFR. |
