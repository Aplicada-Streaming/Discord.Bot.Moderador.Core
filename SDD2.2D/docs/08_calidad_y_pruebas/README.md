# 08 Calidad y pruebas — discord-bots-admin

**Proyecto:** discord-bots-admin
**Tipo (D8):** web-monolith
**Versión de la sección:** 1.0
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

Punto de entrada navegable de la disciplina de calidad y validación del servicio monolítico de administración y moderación. Esta sección ancla la estrategia de calidad, la pirámide de testing, el plan de pruebas, la matriz de cobertura, el catálogo de casos de prueba, los criterios de validación, la Definition of Done canónica y la guía de testing de extensibilidad. Recibe upstream de 02 (CU y RN), 05 (arquitectura y NFR), 06 (DoR, US, BT) y 07 (mini-plan); alimenta a 09 (quality gates como stages del pipeline), 10 (guía de testing del repo) y 11 (un test ejecutable por ejemplo).

## Documentos de la sección

| Documento | Estado | Contenido |
| --- | --- | --- |
| [estrategia-calidad_v1.0.md](estrategia-calidad_v1.0.md) | Propuesto | Definición de calidad, atributos ISO/IEC 25010 priorizados con su NFR de origen, cinco quality gates, roles QA y cadencia. |
| [estrategia-testing_v1.0.md](estrategia-testing_v1.0.md) | Propuesto | Pirámide 70/20/10, cobertura por capa con Dominio/detección ≥ 90 %, tooling por capacidad, BDD, mocks, datos sintéticos y ambiente efímero. |
| [plan-pruebas_v1.0.md](plan-pruebas_v1.0.md) | Propuesto | Alcance, criterios de entrada/salida, nueve riesgos de calidad, plan por las siete rebanadas R1..R7 y recursos. |
| [matriz-cobertura-pruebas_v1.0.md](matriz-cobertura-pruebas_v1.0.md) | Propuesto | Tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) y cobertura por capa. |
| [casos-prueba-referenciales_v1.0.md](casos-prueba-referenciales_v1.0.md) | Propuesto | Catálogo de 69 TC con tipo, cobertura, setup, pasos Given/When/Then, expected, actual y status. |
| [criterios-validacion_v1.0.md](criterios-validacion_v1.0.md) | Propuesto | Criterios funcionales por CU crítico, no funcionales con SLA, regresión, calidad de código y excepciones. |
| [definition-of-done_v1.0.md](definition-of-done_v1.0.md) | Propuesto | DoD canónica por capa (US, BT, sprint, release) con criterios mecánicos. Fuente única referenciada por 06 y 07. |
| [guia-testing-extensibilidad_v1.0.md](guia-testing-extensibilidad_v1.0.md) | Propuesto | Cómo testear descriptores, tipos de regla y de acción sin tocar el núcleo, y el sustrato de la frontera reservada. |

## Pirámide y cobertura

- Pirámide objetivo (web-monolith): 70 % unitario, 20 % integración, 10 % end-to-end.
- Cobertura por capa (piso, no techo; no se baja sin ADR): Dominio/detección ≥ 90 / 80, Aplicación ≥ 80 / 70, Infraestructura ≥ 70 / 60, Presentación ≥ 60 / 50; global ≥ 75 / 65 (gate de CI). Detalle en `estrategia-testing_v1.0.md` §2.
- Catálogo: 69 casos de prueba (38 Unit, 24 Integration, 7 E2E).

## Quality gates configurados

Bloqueantes para mergear (`estrategia-calidad_v1.0.md` §3, `SOLUTION-INTAKE §17 P.6/P.8`); se materializan como stages del pipeline en 09.

| Gate | Condición |
| --- | --- |
| G1 Compilación | Build sin errores |
| G2 Tests en verde | Suite unit + integración + e2e crítica al 100 % |
| G3 Cobertura por capa | Global líneas ≥ 75 %, branches ≥ 65 %; módulo de detección ≥ 90 % líneas |
| G4 Formato | Formato canónico sin diferencias |
| G5 Análisis estático | Sin warnings nuevos respecto de la rama base |

## DoD canónica

La Definition of Done canónica del proyecto vive en [definition-of-done_v1.0.md](definition-of-done_v1.0.md). Cubre cuatro capas (US, BT, sprint, release) con criterios verificables mecánicamente. La DoR de 06 y el mini-plan de 07 la referencian; ningún otro documento la redefine.

## Trazabilidad

- Upstream: 02 (CU-01..CU-16, RN-01..RN-16), 05 (arquitectura, NFR §8, ADR, extensibilidad), 06 (DoR, US-01..US-19, BT-01..BT-17), 07 (mini-plan, rebanadas R1..R7).
- Downstream: 09 (gates G1–G5 como stages), 10 (guía de testing del repositorio), 11 (un test ejecutable por sample).
- Cada TC referencia al menos un CU, RN o NFR; cada NFR numérico tiene test o medición; la matriz tiene las tres tablas obligatorias más cobertura por capa.
