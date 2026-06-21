# Estrategia de calidad — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** estrategia-calidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

## 1. Definición de calidad para el proyecto

El sistema tiene calidad cuando corta de forma confiable las ráfagas de spam distribuidas entre canales sin banear a usuarios legítimos, ejecuta cada acción de moderación con su evidencia auditada y reversible, y protege las credenciales del administrador y los tokens de bot en reposo. El perfil de riesgo del proyecto es asimétrico: un falso negativo deja un servidor inundado de spam, pero un falso positivo banea a un miembro legítimo; por eso la calidad se mide tanto por la efectividad de la detección (núcleo crítico) como por los resguardos contra la moderación errónea (modo simulación, exenciones, antirrebote, reporte y desbaneo). Al ser un servicio de un solo operador auto-hospedado en hardware de bajo consumo, la calidad incluye además que el artefacto se instale, sobreviva reinicios y reconecte sin intervención manual.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

Cada atributo declara su prioridad para v1, una justificación anclada al alcance funcional y, cuando corresponde, su métrica numérica con el NFR de origen de `arquitectura-solucion_v1.0.md` §8 (`SOLUTION-INTAKE §17 P.10`).

| Atributo ISO/IEC 25010 | Prioridad | Justificación | Métrica / NFR de origen |
| --- | --- | --- | --- |
| Funcionalidad (corrección, completitud, pertinencia) | Crítica | El valor central es detectar la ráfaga distribuida y contener al emisor sin alcanzar a exentos. Cubre los 16 CU y las 16 RN. | Atributo técnico verificado por la cobertura funcional de los 16 CU y las 16 RN (sin NFR numérico propio en `arquitectura-solucion_v1.0.md` §8). Las cifras de resultado asociadas —limpieza efectiva ≥ 98 % de mensajes eliminados dentro de los 10 s y corte automático ≥ 95 %— son métricas de negocio (vision §6 / intake §8), no NFR técnicos. |
| Fiabilidad (madurez, tolerancia a fallos, recuperabilidad) | Crítica | El servicio debe seguir moderando ante caídas transitorias del canal de eventos y sobrevivir reinicios reconstruyendo su estado en memoria. | Disponibilidad mensual (SLO) 99 % mensual (NFR disponibilidad); reconexión automática (CU-13) |
| Seguridad (confidencialidad, integridad, autenticidad) | Crítica | Token de bot cifrado en reposo, credenciales del administrador con hash robusto, autorización por rol único. Un token filtrado otorga control total del bot. | Token cifrado en reposo (RN-14); hash PHC del administrador (RN-13); autorización rol único (RN-12) |
| Eficiencia de desempeño (comportamiento temporal, uso de recursos) | Alta | El pipeline corre en hardware ARM de 32 bits; debe procesar cada mensaje con baja latencia y huella de memoria acotada por conexión. | Latencia p95 < 200 ms (NFR latencia); throughput ≥ 50 mensajes/s (NFR throughput); ≤ 8 MB por conexión de gateway activa (NFR memoria) |
| Mantenibilidad (modularidad, testabilidad, modificabilidad) | Alta | El dominio se prueba sin infraestructura para alcanzar el gate del módulo de detección; la configuración dirigida por descriptores aísla los puntos de extensión. | Cobertura del módulo de detección ≥ 90 % líneas; global líneas ≥ 75 %, branches ≥ 65 % (NFR cobertura) |
| Usabilidad (operabilidad, protección frente a errores del usuario) | Media | El administrador configura sin conocimiento técnico profundo apoyándose en leyendas y ejemplos derivados de descriptores; la validación rechaza valores fuera de límite. | Ayuda contextual por parámetro y validación por descriptor (CU-11, RN-10) |
| Compatibilidad (interoperabilidad) | Media | Interopera con el gateway y la API de la plataforma de mensajería y se sirve en navegadores evergreen. | Panel en navegadores evergreen (`SOLUTION-INTAKE §17 P.9`) |
| Portabilidad (adaptabilidad, instalabilidad, reemplazabilidad) | Media | Publicación self-contained para la arquitectura objetivo, instalable como servicio del sistema, con rollback que preserva la clave maestra. | Artefacto self-contained para la arquitectura objetivo, rollback que preserva clave (`SOLUTION-INTAKE §17 P.8`) |

## 3. Quality gates

Conjunto de criterios mecánicos que el pipeline de integración continua aplica antes de declarar un build, una rama o un release como aceptable. Bloqueantes para mergear (alineados con `SOLUTION-INTAKE §17 P.8` y los umbrales de §17 P.6). El tooling se describe por capacidad y se ancla por referencia; el producto concreto vive en `SOLUTION-INTAKE §17 P.6`/`P.8`. Estos gates se materializan como stages del pipeline en la categoría 09.

| Gate | Condición | Herramienta (por capacidad) | Consecuencia si falla |
| --- | --- | --- | --- |
| G1 Compilación | El artefacto compila sin errores para la configuración de release | Compilador del runtime objetivo en el pipeline (`SOLUTION-INTAKE §17 P.8`) | Bloquea el merge; no se ejecutan los gates siguientes |
| G2 Tests en verde | La suite de unitarias e integración pasa al 100 %; la e2e crítica del panel pasa | Corredores de tests por nivel (`SOLUTION-INTAKE §17 P.6`) | Bloquea el merge |
| G3 Cobertura por capa | Líneas ≥ 75 % y branches ≥ 65 % global; módulo de detección ≥ 90 % líneas; cobertura por capa según `estrategia-testing_v1.0.md` §2 | Recolector de cobertura del pipeline (`SOLUTION-INTAKE §17 P.6`) | Bloquea el merge |
| G4 Formato | El código respeta el formato canónico, sin diferencias | Verificador de formato del pipeline (`SOLUTION-INTAKE §17 P.8`) | Bloquea el merge |
| G5 Análisis estático | El análisis estático no introduce warnings nuevos respecto de la rama base | Analizador estático del pipeline (`SOLUTION-INTAKE §17 P.8`) | Bloquea el merge |

Excepción a un gate: solo se admite con ADR explícita y plan de remediación (ver `criterios-validacion_v1.0.md` §6). La política de no bajar cobertura sin ADR proviene de la regla §2.2 de 08.

## 4. Roles QA dentro del equipo

El proyecto lo construye, prueba y opera una sola persona (Fernando, con asistencia de IA), por lo que QA y SDET no son cargos separados sino disciplinas que la misma persona ejerce con artefactos versionados que suplen la separación de roles. La trazabilidad explícita de cada caso de prueba a CU/RN/NFR y los gates automáticos del pipeline son el sustituto de la revisión por pares.

| Actividad | Responsable | Apoyo |
| --- | --- | --- |
| Diseñar los casos de prueba y mantener la matriz | Desarrollador (rol SDET) | Catálogo `casos-prueba-referenciales_v1.0.md` |
| Ejecutar la suite | Pipeline de integración continua (automático) | Gates G1–G5 |
| Aprobar el release | Desarrollador (rol QA) contra `criterios-validacion_v1.0.md` | DoD de release de `definition-of-done_v1.0.md` |
| Validar trazabilidad CU↔test | Desarrollador (rol QA), revisión cruzada con la especificación funcional (02) | `matriz-cobertura-pruebas_v1.0.md` |

RACI reducido: el desarrollador es Responsable y Aprobador de todos los artefactos de calidad; el pipeline es el ejecutor mecánico que no admite criterio humano.

## 5. Cadencia de revisión

- La estrategia de calidad y sus umbrales se revisan al cierre de cada rebanada vertical (sprint) del mini-plan de 07, cuando se incorpora un módulo nuevo que puede exigir subir cobertura.
- Los umbrales de cobertura y los gates no se bajan sin ADR (regla §2.2 de 08). Subirlos no requiere ADR.
- Los valores por defecto de detección (umbral de canales, ventana) y la elección de familia de hash quedan abiertos a Sprint 0 (`SOLUTION-INTAKE §17 P.11`); cuando se cierren, los criterios de validación que dependan de ellos se recalibran en la versión correspondiente de `criterios-validacion_v1.0.md`.
- Cualquier cambio en los criterios versionables de la DoD se registra en §9 del propio `definition-of-done_v1.0.md` y se comunica en el cierre de rebanada siguiente.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial de la estrategia de calidad para `discord-bots-admin`: atributos ISO/IEC 25010 priorizados con su NFR de origen, cinco quality gates bloqueantes alineados con `SOLUTION-INTAKE §17 P.6/P.8`, roles QA para un único desarrollador y cadencia de revisión por rebanada. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: en §2, fila Funcionalidad, se separa la métrica de negocio (limpieza efectiva ≥ 98 %, corte automático ≥ 95 %) del atributo de calidad técnico y se reclasifica como métrica de negocio (vision §6 / intake §8), distinguiéndola de los NFR técnicos de §8. |
