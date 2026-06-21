# ADR-04 — Separación de capas con dominio independiente de infraestructura

**Proyecto:** discord-bots-admin
**Documento:** ADR-04-separacion-capas-dominio-independiente_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Estilo

## 1. Contexto

El módulo de detección de spam es el núcleo crítico del producto y exige una cobertura de pruebas ≥ 90 % (`SOLUTION-INTAKE §17 P.6`). El sistema integra con la plataforma de mensajería externa a través de una librería cliente del gateway y persiste mediante un ORM. Si la lógica de moderación se acopla a esas librerías de infraestructura, el dominio no se puede probar sin levantar la infraestructura, y el gate de cobertura crítica se vuelve frágil y lento. Lo motivan NB-01, todas las CU del pipeline (CU-01, CU-02, CU-04, CU-14, CU-16), las RN de evaluación (RN-04, RN-05, RN-07, RN-09) y el NFR de cobertura.

## 2. Decisión

Se adopta Clean Architecture con separación estricta de capas (Dominio, Aplicación, Infraestructura, Presentación) y dependencia unidireccional hacia el Dominio. El Dominio (motor de moderación, evaluadores, descriptores, estado de conducta y antirrebote) no depende de ninguna librería de infraestructura; la Aplicación orquesta; la Infraestructura (adaptador del gateway, persistencia, cifrado) y la Presentación (panel) dependen de las capas internas a través de puertos/abstracciones.

## 3. Estado

Aceptado el 2026-06-20. Derivada del estilo de capas de ADR-01 y de la variante web-monolith (§1.2 de las reglas de la categoría 05).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Clean Architecture con dominio independiente (elegida) | Dominio testeable sin infraestructura; reemplazo de adaptadores sin tocar la lógica; cumple el gate de cobertura crítica | Más abstracciones y archivos; curva inicial mayor |
| Capas planas con lógica acoplada a la infraestructura | Menos código al inicio | Imposible probar el dominio sin infraestructura; gate de cobertura frágil; acopla la moderación a la librería del gateway |
| Patrón transaccional por script (lógica en controladores) | Directo para CRUD | El pipeline de moderación no es CRUD; se vuelve inmantenible y no testeable |

## 5. Consecuencias positivas

1. El motor de detección se prueba con dobles de prueba, habilitando ≥ 90 % de cobertura del módulo crítico.
2. El adaptador del gateway y la persistencia se pueden sustituir o simular sin tocar el dominio (modo simulación, samples del intake §18).
3. Las reglas de evaluación (RN-04, RN-05, RN-07, RN-09) viven en el dominio, aisladas de la E/S.
4. Facilita la extensibilidad de reglas y acciones (ADR-12) sin filtrar detalles de infraestructura al dominio.

## 6. Consecuencias negativas y trade-offs

1. Mayor número de abstracciones y archivos: aceptado a cambio de testabilidad y mantenibilidad del núcleo crítico.
2. Costo de mapeo entre entidades de dominio y de persistencia: aceptado; el mapeo es localizado en Infraestructura.
3. Curva de entrada algo mayor para un solo desarrollador: aceptado; el beneficio en el módulo crítico lo justifica.

## 7. Implementación

Cuatro proyectos lógicos o carpetas por capa dentro del único proyecto físico. El Dominio define puertos (interfaces) para el cliente del gateway, la persistencia y el cifrado; la Infraestructura los implementa; la composición de dependencias los inyecta en el host web. El bot (servicio en segundo plano) consume el dominio a través de la Aplicación.

## 8. Métricas de validación

- Cobertura del módulo de detección ≥ 90 %; global líneas ≥ 75 %, branches ≥ 65 %.
- El dominio compila y se prueba sin referencias a las librerías de infraestructura.
- Pruebas unitarias del pipeline ejecutadas sin red ni base real.

## 9. Referencias

- NB-01.
- CU-01, CU-02, CU-04, CU-14, CU-16.
- RN-04, RN-05, RN-07, RN-09.
- NFR de cobertura (`arquitectura-solucion_v1.0.md §8`).
- `SOLUTION-INTAKE §17 P.2, P.6`.
- ADR-01 (estilo monolítico), ADR-12 (configuración dirigida por esquema).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
