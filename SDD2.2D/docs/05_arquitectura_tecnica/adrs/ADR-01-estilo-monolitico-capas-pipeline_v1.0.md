# ADR-01 — Estilo monolítico de capas con núcleo de pipeline

**Proyecto:** discord-bots-admin
**Documento:** ADR-01-estilo-monolitico-capas-pipeline_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Estilo

## 1. Contexto

El sistema debe reunir, en un solo artefacto desplegable sobre hardware de bajo consumo de 32 bits y sin contenedores, un panel de administración, un bot de moderación que opera en segundo plano y la persistencia, operados por un único administrador (`SOLUTION-INTAKE §13, §14, §10`). El núcleo del problema es un flujo de moderación que transforma un mensaje entrante en un incidente a través de etapas ordenadas (`SOLUTION-INTAKE §6`). Restricciones: memoria acotada del proceso de 32 bits (§17 P.12), simplicidad de instalación y rollback en el dispositivo, y un único dominio de negocio. Lo motivan NB-01 (corte de la ráfaga), todas las CU del pipeline (CU-01, CU-02, CU-04, CU-14, CU-16) y los NFR de latencia, throughput y memoria de §17 P.10.

## 2. Decisión

Se adopta un único proyecto monolítico con Clean Architecture por capas (Dominio, Aplicación, Infraestructura, Presentación) y un pipeline de evaluación como núcleo del dominio. El bot corre como servicio en segundo plano dentro del mismo host web que sirve el panel; ambos comparten persistencia y composición de dependencias en un solo proceso.

## 3. Estado

Aceptado el 2026-06-20. Decisión cerrada pre-Sprint 0 según `SOLUTION-INTAKE §17 P.11`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Monolito de capas con núcleo de pipeline (elegida) | Despliegue y rollback simples; un solo proceso dentro del presupuesto de memoria; dominio testeable; el pipeline modela el flujo natural de moderación | Sin deploy independiente de panel y bot; un fallo del proceso afecta a ambas caras |
| Microservicios | Deploy y escalado independientes | Complejidad operativa alta; comunicación entre servicios; sobrecarga de proceso y memoria desproporcionada para el dispositivo y para un operador |
| Bot y panel en procesos separados | Aislamiento de fallos | Comunicación inter-proceso; canal de configuración compartido; duplica composición; complica instalación y rollback |

## 5. Consecuencias positivas

1. La instalación es un único paquete self-contained y el rollback es una reinstalación atómica (ADR-05).
2. El dominio de moderación se prueba sin infraestructura, habilitando el gate de cobertura ≥ 90 % del módulo de detección (ADR-04, NFR cobertura).
3. El pipeline expresa el flujo de §6 del intake como etapas componibles, facilitando agregar reglas y acciones (ADR-12).
4. La huella de memoria se mantiene dentro del presupuesto de ≤ 8 MB por conexión al concentrar todo en un proceso sin runtime externo.

## 6. Consecuencias negativas y trade-offs

1. No hay deploy independiente del panel y del bot: aceptado, porque el operador es único y la simplicidad de despliegue prima (§17 P.12).
2. Un fallo del proceso detiene tanto la administración como la moderación: mitigado con reinicio automático del servicio del sistema (ADR-05, ADR-13).
3. ARM de 32 bits es un tier deprioritizado sin inversión de performance: trade-off aceptado por reutilizar el hardware existente (§17 P.12).

## 7. Implementación

Cuatro capas con dependencia unidireccional hacia el Dominio. El motor de moderación (Dominio) orquesta el pipeline descrito en `flujo-ejecucion_v1.0.md`. El adaptador del gateway y la persistencia viven en Infraestructura; el panel en Presentación; los servicios de orquestación en Aplicación. El bot se aloja como servicio en segundo plano del host web.

## 8. Métricas de validación

- Latencia p95 de procesamiento por mensaje < 200 ms en el hardware de referencia (§17 P.10).
- Throughput sostenido ≥ 50 mensajes/s (a confirmar por benchmark).
- Memoria ≤ 8 MB por conexión de gateway activa.
- Cobertura del módulo de detección ≥ 90 %.

## 9. Referencias

- NB-01.
- CU-01, CU-02, CU-04, CU-14, CU-16.
- RN-04, RN-05.
- NFR de latencia, throughput y memoria (`arquitectura-solucion_v1.0.md §8`).
- `SOLUTION-INTAKE §6, §13, §14, §17 P.2, P.10, P.11, P.12`.
- ADR-04 (separación de capas), ADR-05 (despliegue), ADR-13 (firewall multi-contexto).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
