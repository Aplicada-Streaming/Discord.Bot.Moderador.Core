# Plan de pruebas — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** plan-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

## 1. Alcance del plan

El plan cubre el ciclo de vida v1 del servicio monolítico de administración y moderación, organizado por las siete rebanadas verticales R1..R7 del mini-plan de 07 (`mini-plan_v1.0.md`). Cada rebanada entrega valor demostrable end-to-end y el plan de pruebas se ejecuta por rebanada conforme se incorpora el módulo correspondiente.

Módulos incluidos:

- Núcleo del Dominio: motor de moderación (pipeline), evaluador de contenido, evaluador de conducta, ventana deslizante de actividad, evaluador de políticas, antirrebote, registro de descriptores.
- Aplicación: autenticación, registro de servidores, servicio de configuración, ejecutor de acciones, servicio de incidentes, prueba de configuración.
- Infraestructura: adaptador del gateway y de la API de la plataforma, persistencia embebida, cifrado de tokens.
- Presentación: panel del lado servidor (primer ingreso, autenticación, registro y prueba, configuración con ayuda contextual, incidentes y desbaneo).

Módulos excluidos del alcance de pruebas de v1:

- Frontera reservada de propuesta de configuración (`PropuestaDeConfiguracion`): no construida en v1 (ADR-10); su estrategia de testing futura se documenta en `guia-testing-extensibilidad_v1.0.md`.
- US-19 (segundo canal de salida lógico, Could, en Borrador): no comprometida en el mini-plan; fuera de alcance hasta su refinamiento.
- Coordinación con el filtro nativo de la plataforma: fuera de alcance de v1 (`SOLUTION-INTAKE §9`).

## 2. Criterios de entrada

Para ejecutar el plan de pruebas de una rebanada:

- El build de la rebanada compila sin errores (gate G1).
- Las US y BT de la rebanada cumplen la DoR de 06 (`definition-of-ready_v1.0.md`): la US tiene Given/When/Then con camino feliz y borde; la BT tiene fuente upstream en 05 y criterios técnicos verificables.
- Los datos de prueba sintéticos necesarios (mensajes simulados, snowflakes de prueba, fixtures de entidades) están disponibles o son construibles dentro de la rebanada (`estrategia-testing_v1.0.md` §6).
- El ambiente de testing está disponible: base relacional embebida efímera en archivo y, para e2e, instancia local del panel con datos sintéticos (`estrategia-testing_v1.0.md` §7).
- La clave maestra de cifrado y las credenciales de prueba no productivas están inyectadas por el ambiente de tests.

## 3. Criterios de salida

Para declarar el plan de la rebanada ejecutado con éxito:

- Cada criterio Given/When/Then de los CU que la rebanada hace avanzar tiene su TC en verde (matriz CU↔Tests).
- La cobertura por capa de los componentes tocados cumple su umbral de `estrategia-testing_v1.0.md` §2; si la rebanada toca el módulo de detección, ese módulo queda ≥ 90 % líneas; la cobertura global cumple líneas ≥ 75 % y branches ≥ 65 % (gate G3).
- Los NFR que la rebanada hace verificables tienen su medición dentro del SLA o una excepción con ADR.
- No hay defectos bloqueantes abiertos sobre la rebanada.
- Gates G1, G2, G4 y G5 en verde (build, tests, formato, análisis estático sin warnings nuevos).
- La rebanada es demostrable end-to-end por su camino declarado sin romper las anteriores; la DoD de sprint de `definition-of-done_v1.0.md` §1.3 se cumple.

## 4. Riesgos de calidad

Cada riesgo con impacto, probabilidad y mitigación, alineado con los riesgos arquitectónicos de `arquitectura-solucion_v1.0.md` §9 y los del intake §11.

| ID | Riesgo de calidad | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- | --- |
| RC-Q1 | Falsos positivos: banear a un usuario legítimo | Alto | Media | TC de discriminación por canales distintos (TC-01, TC-02), de exenciones (TC-04, TC-16, TC-27, TC-29) y de modo simulación (TC-07, TC-22, TC-24); cobertura del módulo de detección ≥ 90 % |
| RC-Q2 | Pérdida del estado de conducta y antirrebote ante reinicio deja una ráfaga sin cortar o repite una acción | Medio | Media | TC con reloj inyectable sobre ventana deslizante y antirrebote (TC-08, TC-20, TC-21); validar reconstrucción con tráfico y que el borrado retroactivo limpia lo previo (TC-12) |
| RC-Q3 | Expresión regular costosa o maliciosa en una regla de contenido (retroceso catastrófico) | Medio | Baja | TC de validación del patrón al guardar y de tope de tiempo de evaluación (TC-14, TC-15) |
| RC-Q4 | Latencia p95 inalcanzable en hardware de 32 bits | Medio | Media | TC-55 sobre pipeline instrumentado en ambiente equivalente; confirmación por benchmark en hardware real; tier deprioritizado aceptado |
| RC-Q5 | Filtración del token de bot por almacenamiento en claro | Alto | Baja | TC de cifrado en reposo y de ida y vuelta (TC-35, TC-38); verificar que el valor en la base no es legible como token |
| RC-Q6 | Caída del gateway o token revocado deja sin moderación de forma silenciosa | Alto | Baja | TC de reconexión automática y estado de conexión (TC-47, TC-48); TC de prueba de configuración que bloquea la activación con token inválido (TC-45) |
| RC-Q7 | Acción ejecutada sobre un usuario con jerarquía superior o sin permiso, deteniendo el pipeline | Alto | Media | TC de jerarquía y permisos que registran no accionable sin detener el pipeline (TC-06, TC-44, TC-53) |
| RC-Q8 | Pérdida de evidencia del incidente por borrar antes de copiar | Alto | Baja | TC de orden de acciones con copia antes de borrar (TC-19); TC de detalle de incidente con evidencia (TC-50) |
| RC-Q9 | Suite e2e del panel lenta y frágil en ARM, desbalanceando la pirámide | Medio | Media | Mantener e2e en el 10 %; cubrir lógica con unitarias; e2e solo para journeys críticos (TC-49, TC-50, TC-51, TC-54) |

## 5. Plan por sprint (rebanada)

El plan sigue las siete rebanadas del mini-plan de 07, en orden R1 → R7. Recursos: un desarrollador con asistencia de IA (ver §6).

| Rebanada (sprint) | Alcance de testing | CU que avanzan | TC en foco | Entregables de calidad |
| --- | --- | --- | --- | --- |
| R1 (Sprint 01, walking skeleton) | Unitarias del evaluador de ráfaga y ventana deslizante; integración mínima del pipeline y del registro de servidor con token cifrado; reporte en simulación | CU-10, CU-01, CU-14 | TC-01, TC-02, TC-03, TC-35, TC-07, TC-39, TC-40 | Suite unitaria del núcleo de detección verde; camino feliz end-to-end demostrable (excepción walking skeleton de la DoD §2) |
| R2 (Sprint 02) | Integración de baneo automático y borrado retroactivo; orden de acciones y copia antes de borrar; reporte real | CU-02, CU-03, CU-05 | TC-05, TC-06, TC-08, TC-09, TC-10, TC-11, TC-12, TC-19, TC-25, TC-26 | Cobertura del ejecutor de acciones y del servicio de incidentes en umbral; criterios de borde de R1 completados |
| R3 (Sprint 03) | Unitarias del evaluador de contenido; tope de tiempo de regex; patrón inválido | CU-04 | TC-13, TC-14, TC-15, TC-16 | Módulo de detección con contenido ≥ 90 % líneas |
| R4 (Sprint 04) | E2E del panel: primer ingreso, autenticación, revisión de incidentes y desbaneo; autorización | CU-08, CU-09, CU-06, CU-07 | TC-30, TC-31, TC-32, TC-33, TC-34, TC-49, TC-50, TC-51, TC-52, TC-53, TC-54 | Suite e2e crítica del panel verde; cobertura de presentación en umbral (≥ 60 %) |
| R5 (Sprint 05) | Unitarias e integración de exenciones por rol, usuario y canal; descarte previo | CU-15 | TC-04, TC-27, TC-28, TC-29 | Cobertura del filtro de exentos en umbral |
| R6 (Sprint 06) | Integración de acciones adicionales (timeout, expulsión, rol) en orden; antirrebote en ejecución | CU-11 (acciones), CU-02 | TC-19, TC-20, TC-23 | Catálogo de acciones cubierto; orden determinista verificado |
| R7 (Sprint 07) | Configuración dirigida por descriptores con ayuda contextual; prueba de configuración; reconexión; antirrebote | CU-11, CU-12, CU-13, CU-16 | TC-17, TC-18, TC-21, TC-41, TC-42, TC-43, TC-44, TC-45, TC-46, TC-47, TC-48 | Cobertura de configuración y de prueba de configuración en umbral; mediciones de NFR (TC-55..TC-58) ejecutadas |

Mediciones de NFR (TC-55..TC-58): se preparan desde R2 (cuando existe pipeline y acciones) y se consolidan en R7 con el banco de carga; latencia y throughput se confirman en hardware real cuando esté disponible.

## 6. Recursos

- Personas: un desarrollador (Fernando) con asistencia de IA, que ejerce los roles QA y SDET (`mini-plan_v1.0.md` §2; `estrategia-calidad_v1.0.md` §4). No hay ceremonias de equipo formales.
- Ambientes: ambiente local de desarrollo con base efímera en archivo; instancia local del panel para e2e con datos sintéticos; hardware de referencia (dispositivo ARM) para las mediciones de NFR de latencia, throughput y memoria.
- Datasets: datos de prueba sintéticos generados por fábricas deterministas (mensajes simulados, snowflakes de prueba, entidades de configuración); sin datos de producción (`estrategia-testing_v1.0.md` §6).
- Herramientas: corredores de tests por nivel, factory de aplicación web, base embebida efímera, conducción headless del navegador, recolector de cobertura, ancladas a `SOLUTION-INTAKE §17 P.6`. Pipeline de integración continua con los gates G1–G5 (materializado en 09).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Plan de pruebas inicial para `discord-bots-admin`: alcance por módulo, criterios de entrada y salida, nueve riesgos de calidad alineados con los riesgos arquitectónicos de 05 §9, plan por las siete rebanadas R1..R7 coherente con el mini-plan de 07 y recursos para un único desarrollador. |
