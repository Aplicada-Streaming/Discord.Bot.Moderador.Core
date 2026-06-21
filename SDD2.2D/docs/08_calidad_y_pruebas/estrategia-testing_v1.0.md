# Estrategia de testing — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** estrategia-testing_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

## 1. Pirámide de testing deseada

El proyecto es de tipo web-monolith; adopta la pirámide clásica de la regla §2.2: 70 % unitario, 20 % integración, 10 % end-to-end. La distribución se justifica por la forma del problema: el núcleo de valor es el pipeline de moderación (motor, evaluador de contenido, evaluador de conducta, evaluador de políticas, antirrebote, estado de conducta), todos componentes del Dominio que dependen unidireccionalmente hacia adentro y se prueban sin infraestructura (ADR-04). La mayoría del riesgo —y de la cobertura exigida (≥ 90 % del módulo de detección)— se cubre con pruebas unitarias rápidas y deterministas sobre esos componentes.

| Nivel | Qué cubre | Tooling (por capacidad) | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unit | Lógica aislada del Dominio y de la Aplicación: evaluadores de contenido/conducta/políticas, ventana deslizante, antirrebote, hashing, cifrado, validación por descriptor | Framework de pruebas unitarias del runtime + librería de aserciones fluidas + librería de dobles de prueba (`SOLUTION-INTAKE §17 P.6`) | 70 % |
| Integration | Interacción entre componentes con persistencia real efímera y el pipeline ensamblado: registro de servidor, baneo, copia de mensajes antes de borrar, reconexión, prueba de configuración | Framework de integración con factory de aplicación web y base relacional embebida en archivo, sin contenedores (`SOLUTION-INTAKE §17 P.6`) | 20 % |
| E2E | Journeys críticos del panel del lado servidor: primer ingreso y autenticación, revisión de incidentes y desbaneo, registro y prueba de configuración | Framework end-to-end headless de navegador (`SOLUTION-INTAKE §17 P.6`) | 10 % |

Justificación contra anti-pirámides: no se adopta la pirámide invertida (e2e pesado) porque la suite headless del panel es lenta y frágil en hardware ARM y diagnostica mal las fallas del motor; el motor se valida con unitarias. No se adopta la pirámide aplanada (un único número global de cobertura) porque escondería 30 % en el dominio detrás de 100 % en getters; la cobertura se reporta por capa (§2). Snapshot no se usa como nivel propio: este proyecto no produce salida de CLI ni render que justifique baselines de snapshot; las verificaciones de payload de reporte y de configuración se hacen con aserciones explícitas dentro de unit/integración.

## 2. Cobertura mínima por capa

Valores numéricos por capa. El piso global del intake (líneas ≥ 75 %, branches ≥ 65 %) se reconcilia con la tabla por capa de la regla §2.2 para web-monolith (80 % aplicación, 70 % infraestructura, 60 % presentación) declarando el Dominio/detección ≥ 90 % por ser el núcleo crítico (`SOLUTION-INTAKE §17 P.6`, NFR cobertura de `arquitectura-solucion_v1.0.md` §8). Los porcentajes son piso, no techo (regla §2.2); no se bajan sin ADR.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio (motor de moderación, evaluadores, ventana deslizante, antirrebote, descriptores) | ≥ 90 | ≥ 80 | no exigido en v1 | 90 / 80 / — |
| Aplicación (autenticación, configuración, registro de servidores, ejecutor de acciones, incidentes, prueba de configuración) | ≥ 80 | ≥ 70 | no exigido en v1 | 80 / 70 / — |
| Infraestructura (adaptador del gateway, persistencia, cifrado de tokens) | ≥ 70 | ≥ 60 | no exigido en v1 | 70 / 60 / — |
| Presentación (panel del lado servidor) | ≥ 60 | ≥ 50 | no exigido en v1 | 60 / 50 / — |
| Global (gate de CI) | ≥ 75 | ≥ 65 | — | 75 / 65 / — |

El módulo de detección de spam (evaluador de conducta + ventana deslizante + evaluador de contenido + evaluador de políticas dentro del Dominio) es el núcleo crítico y se trata con el umbral más alto (≥ 90 % líneas). Mutation testing no se exige como gate en v1 (la tabla de la regla §2.2 solo lo exige para library); queda como mejora futura sin afectar el gate.

## 3. Tooling

El tooling se elige por capacidad y por nivel; los productos concretos por nivel viven en `SOLUTION-INTAKE §17 P.6` y no se nombran en el cuerpo (misma política que la categoría 05).

| Capacidad | Nivel donde se usa | Ancla |
| --- | --- | --- |
| Ejecución de pruebas unitarias del runtime | Unit | `SOLUTION-INTAKE §17 P.6` |
| Aserciones fluidas y legibles | Unit, Integration | `SOLUTION-INTAKE §17 P.6` |
| Dobles de prueba (sustitutos del adaptador del gateway y de colaboradores) | Unit, Integration | `SOLUTION-INTAKE §17 P.6` |
| Factory de aplicación web para integración en proceso | Integration | `SOLUTION-INTAKE §17 P.6` |
| Base relacional embebida efímera en archivo (sin contenedores) | Integration | `SOLUTION-INTAKE §17 P.4`, §17 P.6 |
| Conducción headless del navegador | E2E | `SOLUTION-INTAKE §17 P.6` |
| Recolección y reporte de cobertura por capa | Gate de CI | `SOLUTION-INTAKE §17 P.6` |

Pruebas de contrato hacia otros proyectos: no aplican; el proyecto es único (`SOLUTION-INTAKE §17 P.6`).

## 4. BDD a partir de los Given/When/Then de 02

Cada CU de la especificación funcional (02) declara cuatro criterios de aceptación CA-01..CA-04 en formato Given/When/Then. Esos criterios son la fuente de los casos de prueba: cada CA se materializa como un caso de prueba en `casos-prueba-referenciales_v1.0.md` con el mismo Given/When/Then, y la DoR de 06 exige Given/When/Then con al menos un camino feliz y un borde para toda US Must o Should, de modo que cada US es testeable desde su definición.

- No se introduce un runner de archivos `.feature` separado en v1: el Given/When/Then se expresa como nombre y estructura arrange/act/assert del propio test (Given = arrange/setup, When = act, Then = assert), conservando la trazabilidad textual al CA de origen en el nombre y en un comentario de referencia.
- La equivalencia es uno a uno verificable: cada CA-XX de cada CU debe tener su test asociado en la matriz (`matriz-cobertura-pruebas_v1.0.md`). Un CA sin test es un gap.

## 5. Mocks y fixtures

- Política de aislamiento: el Dominio se prueba sin dobles porque no depende de infraestructura (ADR-04); el único colaborador que se sustituye con un doble es el Adaptador del gateway y de la API de la plataforma, para no llamar a la plataforma real en unit ni en integración.
- Reuso: los dobles del adaptador y los constructores de entidades de prueba (servidor, regla, grupo, evento, acción, incidente) viven en un proyecto/carpeta de soporte de tests compartida, versionada con el código, para evitar duplicación entre casos.
- Versionado: los fixtures se versionan junto al código en el árbol de tests (`tests/` del repositorio, `SOLUTION-INTAKE §16`); cambiar un fixture compartido es un cambio revisable como cualquier otro.
- Control de duplicación: las fábricas de datos (builders) centralizan la construcción de entidades válidas con sobrescritura por caso, de modo que un nuevo campo obligatorio se agrega en un solo lugar.

## 6. Datos de prueba

- Origen: sintéticos. No se usan datos de producción ni snapshots de la plataforma. Los mensajes de prueba se construyen como mensajes simulados con autor, canal, contenido y marca de tiempo controlados.
- Identificadores de la plataforma: se usan snowflakes de prueba como texto (RN-08), generados de forma determinista por las fábricas, para preservar el valor de 64 bits sin desborde y para que las verificaciones sean reproducibles.
- Escenarios típicos sintéticos: ráfaga distribuida (mismo autor en N canales distintos dentro de la ventana), ráfaga concentrada en un solo canal (no debe disparar), mensaje con contenido que coincide con un patrón, patrón que no compila, sujeto exento, política en simulación, ventana de borrado fuera de rango.
- Versionado y regeneración: por ser deterministas, los datos se regeneran ejecutando las fábricas; no hay datasets binarios que mantener sincronizados. Cualquier cambio en un generador se revisa con el caso que lo usa.

## 7. Ambiente de testing

- Aislamiento entre tests: cada test de integración usa una base relacional embebida efímera propia (archivo temporal por clase/colección de tests), creada y migrada al inicio y descartada al final, de modo que ningún test depende del orden de ejecución ni del estado dejado por otro.
- Base efímera en archivo: la persistencia de pruebas es la misma tecnología de la producción (base relacional embebida en archivo, modo WAL), sin contenedores, conforme al intake (`SOLUTION-INTAKE §17 P.6` indica que no se requieren contenedores porque la base es embebida en archivo).
- Factory de aplicación web: las integraciones que ejercitan el panel o el ensamblado de dependencias arrancan la aplicación en proceso con su composición real salvo el adaptador del gateway, que se sustituye por un doble.
- Variables de entorno y secretos: la clave maestra de cifrado de tokens y las credenciales de prueba son valores no productivos inyectados por el ambiente de tests; nunca se usan secretos reales. El ambiente de e2e del panel corre contra una instancia local con datos sintéticos.
- Estado en memoria: el estado de conducta y el antirrebote viven en memoria (ADR-09); los tests que dependen de ventanas de tiempo controlan el reloj mediante una abstracción de tiempo inyectable para ser deterministas y no depender de pausas reales.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial de la estrategia de testing para `discord-bots-admin`: pirámide 70/20/10 justificada, cobertura por capa con Dominio/detección ≥ 90 %, tooling por capacidad anclado a `SOLUTION-INTAKE §17 P.6`, BDD derivado de los Given/When/Then de 02, política de mocks y fixtures, datos sintéticos con snowflakes de prueba y ambiente con base efímera en archivo. |
