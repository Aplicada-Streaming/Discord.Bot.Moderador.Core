# ADR-08 — Manejo de errores del pipeline y resultados de moderación

**Proyecto:** discord-bots-admin
**Documento:** ADR-08-manejo-errores-pipeline-resultados-moderacion_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Estilo

## 1. Contexto

El pipeline de moderación procesa mensajes en tiempo real y ejecuta acciones contra una plataforma externa que puede fallar de modos esperables: el bot no puede accionar sobre un usuario con rol superior o sin los permisos requeridos (RN-01, `SOLUTION-INTAKE §7`), una expresión regular de una regla de contenido puede ser inválida o costosa (RN-03), o la API de la plataforma puede rechazar una operación. Una falla de acción no debe detener el pipeline ni perder la trazabilidad del incidente. El panel, por su parte, debe presentar errores de validación de configuración derivados de los descriptores (RN-10). Lo motivan CU-02, CU-03, CU-04, CU-05, CU-07, CU-10, CU-11, CU-15, RN-01, RN-03 y RN-11.

## 2. Decisión

Se adopta un manejo de errores con resultados de moderación como conjunto cerrado (ejecutada, simulada, no accionable, fallida) y la regla de que las fallas de acción sobre la plataforma no abortan el pipeline: se registran como incidente con el resultado correspondiente y se reportan al canal privado. Las expresiones regulares se validan al guardar y se evalúan con un tope de tiempo. Los errores de configuración del panel se derivan de los límites de los descriptores y se presentan al administrador como errores de validación, sin persistir valores inválidos.

## 3. Estado

Aceptado el 2026-06-20. Derivada del estilo de pipeline (ADR-01) y de las reglas de evaluación y evidencia de 02.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Resultados de dominio como conjunto cerrado, sin abortar el pipeline (elegida) | Trazabilidad completa de cada disparo; una falla de acción no afecta a otros mensajes; modela el caso de jerarquía insuficiente como incidente no accionable | Requiere disciplina para clasificar cada resultado y reportar las fallas |
| Excepciones propagadas que abortan el procesamiento | Simplicidad aparente | Un fallo de acción detendría el pipeline y perdería incidentes; inaceptable en tiempo real |
| Ignorar silenciosamente las fallas de acción | No interrumpe | Pérdida de trazabilidad; el operador no ve falsos negativos ni jerarquías insuficientes (viola RN-11) |

## 5. Consecuencias positivas

1. Cada disparo deja un incidente con un resultado clasificado, incluida la jerarquía insuficiente como no accionable (RN-01, RN-11).
2. Una falla de acción sobre la plataforma no interrumpe el procesamiento de otros mensajes.
3. La validación de patrones al guardar y el tope de tiempo de evaluación protegen de regex maliciosas o costosas (RN-03).
4. El panel rechaza configuración inválida en el origen, sin contaminar la base (RN-10).

## 6. Consecuencias negativas y trade-offs

1. La clasificación explícita de resultados añade lógica al pipeline: aceptado a cambio de trazabilidad.
2. El tope de tiempo de regex puede dejar un mensaje sin evaluar por una regla costosa: aceptado; se registra y se prefiere a un bloqueo del pipeline.
3. Reportar cada falla al canal privado puede generar ruido: mitigado por el antirrebote (ADR-09) y por la priorización de políticas (RN-04).

## 7. Implementación

El motor de moderación devuelve un resultado del conjunto cerrado; el Ejecutor de acciones traduce las fallas de la plataforma a resultados no accionable/fallida y delega el registro al Servicio de incidentes (RN-11). El Evaluador de contenido valida el patrón al guardar (RN-03) y evalúa con tope de tiempo. El Servicio de configuración rechaza valores fuera de los límites de los descriptores (RN-10). El logging del servicio va al journal del sistema (`arquitectura-solucion_v1.0.md §7`).

## 8. Métricas de validación

- Una falla de acción simulada no detiene el procesamiento de los mensajes siguientes (prueba de integración).
- Todo disparo produce un incidente con resultado clasificado (RN-11).
- Una regex inválida se rechaza al guardar; una costosa respeta el tope de tiempo.

## 9. Referencias

- CU-02, CU-03, CU-04, CU-05, CU-07, CU-10, CU-11, CU-15.
- RN-01, RN-03, RN-10, RN-11.
- `SOLUTION-INTAKE §7, §17 P.10`.
- ADR-01 (pipeline), ADR-09 (antirrebote), ADR-12 (descriptores).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
