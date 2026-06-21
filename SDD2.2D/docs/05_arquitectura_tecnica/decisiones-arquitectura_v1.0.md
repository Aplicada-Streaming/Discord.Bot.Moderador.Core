# Índice de decisiones de arquitectura — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)

## 1. Propósito

Índice navegable de los Architecture Decision Records (ADR) del proyecto. Cada ADR vive en un archivo individual bajo `adrs/` (convención crítica §3.3 de las reglas de la categoría 05); este índice no contiene el cuerpo de las decisiones, solo su identificador, título, categoría, estado y fecha. Las decisiones declaradas cerradas pre-Sprint 0 en `SOLUTION-INTAKE §17 P.11` figuran como `Aceptado`; las que el intake deja abiertas a Sprint 0 figuran como `Propuesto`.

## 2. Índice de ADR

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| ADR-01 | Estilo monolítico de capas con núcleo de pipeline | Estilo | Aceptado | 2026-06-20 |
| ADR-02 | Persistencia en base relacional embebida en modo WAL | Persistencia | Aceptado | 2026-06-20 |
| ADR-03 | Autenticación de administrador único con hash robusto | Seguridad | Aceptado | 2026-06-20 |
| ADR-04 | Separación de capas con dominio independiente de infraestructura | Estilo | Aceptado | 2026-06-20 |
| ADR-05 | Despliegue self-contained para ARM con servicio del sistema | Despliegue | Aceptado | 2026-06-20 |
| ADR-06 | Cumplimiento de la Ley 25.326 de Protección de Datos Personales | Seguridad | Aceptado | 2026-06-20 |
| ADR-07 | Cifrado de tokens en reposo con clave maestra por variable de entorno | Seguridad | Aceptado | 2026-06-20 |
| ADR-08 | Manejo de errores del pipeline y resultados de moderación | Estilo | Aceptado | 2026-06-20 |
| ADR-09 | Estado de conducta y antirrebote en memoria, no persistidos | Persistencia | Aceptado | 2026-06-20 |
| ADR-10 | Omisión de la categoría de contratos de prompts AI (sin LLM en v1) | Estilo | Aceptado | 2026-06-20 |
| ADR-11 | Colapso de la developer guide en READMEs (sin portal de developers) | Estilo | Aceptado | 2026-06-20 |
| ADR-12 | Configuración dirigida por esquema (descriptores como fuente única) | Extensibilidad | Aceptado | 2026-06-20 |
| ADR-13 | Dominio como firewall multi-contexto (un token y una conexión por servidor) | Estilo | Aceptado | 2026-06-20 |

## 3. Notas de estado

Todas las ADR de este catálogo están en estado `Aceptado` porque corresponden a decisiones cerradas pre-Sprint 0 según `SOLUTION-INTAKE §17 P.11` o a decisiones estructurales derivadas de ellas. Las elecciones puntuales que el intake deja abiertas a Sprint 0 (familia de hash exacta entre Argon2 y PBKDF2, herramienta de versionado, persistencia eventual de contadores de conducta, mecanismo de recarga en caliente) se documentan como trade-offs y consecuencias dentro de las ADR aceptadas que las contienen (ADR-03, ADR-09, ADR-12), sin abrir una ADR propia hasta que la elección se tome. Ninguna ADR está consolidada en otro documento; ninguna se editará en su cuerpo: si una decisión evoluciona, se creará una ADR nueva y la anterior pasará a `Superado por ADR-YY`.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Índice inicial con 13 ADR (mínimo de 5 para web-monolith más las requeridas por flags de compliance, omisión de categorías 04 y 10, despliegue, cifrado, firewall multi-contexto, estado en memoria y configuración dirigida por esquema). |
