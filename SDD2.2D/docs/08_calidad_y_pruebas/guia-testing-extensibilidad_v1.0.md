# Guía de testing de extensibilidad — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** guia-testing-extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Ingeniero QA / SDET Senior (AG-08)

Esta guía describe cómo testear los puntos de extensión documentados en `extensibilidad_v1.0.md` (05) sin modificar el núcleo. El proyecto tiene `tiene_extensibilidad = true`: su superficie de extensión es la configuración dirigida por descriptores (nuevos descriptores, nuevos tipos de regla, nuevos tipos de acción) y tiene una frontera reservada, la propuesta de configuración (`PropuestaDeConfiguracion`), no construida en v1. Las extensiones son internas al proyecto (operador único, sin plugins de terceros, ADR-11); por eso esta guía es sobre cómo verificar que una extensión interna se incorpora a través de la superficie declarada sin tocar la lógica genérica.

## 1. Principio: testear la superficie, no reescribir el núcleo

El criterio rector es que agregar un descriptor, un tipo de regla o un tipo de acción no debe requerir modificar la lógica genérica de validación, ni el orquestador del pipeline, ni el ejecutor genérico. La prueba de una extensión consiste, entonces, en dos partes: (a) un test unitario del nuevo predicado/operación aislado, y (b) un test que demuestra que la extensión se descubre y opera a través de la superficie sin que cambie el código del núcleo. Si para agregar la extensión hubo que tocar la validación genérica o el orquestador, eso es un defecto de diseño que el test debe exponer.

## 2. Testear un nuevo descriptor de parámetro (extensibilidad §1.1)

Punto de extensión: agregar un parámetro configurable nuevo registrando su descriptor (identificador, tipo, default, límites, leyenda, ejemplos). ADR-12, RN-10. Sustrato de prueba: el mismo de TC-39/TC-40 (`casos-prueba-referenciales_v1.0.md`), reutilizado para el descriptor nuevo.

Casos a verificar sin modificar la validación genérica:

- Default: ausencia de valor resuelve al default del descriptor nuevo (Given un descriptor nuevo con default D; When no se ingresa valor; Then se aplica D).
- Límite inferior y superior: un valor por debajo del mínimo o por encima del máximo se rechaza con el código CONFIG_VALOR_FUERA_DE_LIMITE y se muestran los límites (mismo comportamiento que TC-40, ahora sobre el descriptor nuevo).
- Valor dentro de límites: se acepta y persiste, y la leyenda y el ejemplo del descriptor nuevo se derivan automáticamente en el panel (mismo comportamiento que TC-39).
- No regresión del núcleo: la validación genérica no se modificó; los TC existentes de validación por descriptor (TC-21, TC-39, TC-40) siguen verdes.

Tipo de pruebas: Unit (validación) y E2E mínima del panel para la derivación de ayuda (en línea con el sample (c) de 11). El test parametriza el descriptor de modo que un descriptor nuevo se cubre agregando un caso de datos, no código de validación.

## 3. Testear un nuevo tipo de regla (extensibilidad §1.2)

Punto de extensión: agregar un tipo de regla de contenido (sin estado) o de conducta (con estado sobre la ventana de actividad) más allá de los iniciales. ADR-12, ADR-04. El predicado vive en el Dominio, aislado de la infraestructura, lo que permite probarlo sin gateway.

Casos a verificar:

- Predicado aislado: el nuevo predicado de contenido se prueba como función pura sobre un mensaje (Given un mensaje sintético; When se evalúa el predicado; Then coincidencia booleana esperada), igual que TC-13 para el evaluador de contenido.
- Predicado con estado: el nuevo predicado de conducta se prueba sobre la ventana de actividad reciente con reloj inyectable (Given una secuencia de eventos en una ventana; When se evalúa; Then coincidencia esperada), igual que TC-01/TC-03 para el evaluador de conducta.
- Validez del criterio según su clase (RC-09, RN-03): un criterio inválido del nuevo tipo se omite con su código y no detiene el pipeline (mismo comportamiento que TC-14), sin tocar el manejo genérico de patrón inválido.
- Integración en el pipeline: el motor evalúa el nuevo tipo en la etapa correspondiente (2 contenido o 3 conducta de `flujo-ejecucion_v1.0.md`) sin que se modifique el orquestador; un test de integración del pipeline ejercita un evento que usa el tipo nuevo.
- Parámetros por descriptor: los parámetros del nuevo tipo se declaran mediante descriptores (§2 de esta guía); se prueban con los casos de descriptor.

Tipo de pruebas: Unit del predicado (mayoría) e Integration del pipeline con el tipo nuevo (en línea con el sample (b) de 11).

## 4. Testear un nuevo tipo de acción (extensibilidad §1.3)

Punto de extensión: agregar un tipo de acción de moderación más allá de los iniciales. ADR-12, ADR-08, ADR-13.

Casos a verificar:

- Operación contra el adaptador: la nueva acción se prueba con un doble del Adaptador del gateway/API que verifica la invocación correcta (Given un evento con la acción nueva; When se ejecuta; Then el adaptador recibe la operación esperada), sin llamar a la plataforma real.
- Orden determinista: la nueva acción respeta el orden de ejecución configurado y la copia de mensajes se toma antes de cualquier borrado (RN-05, RN-11), verificable con el mismo enfoque que TC-19, sin modificar el ejecutor genérico.
- Resultado del conjunto cerrado: la nueva acción devuelve un resultado del conjunto cerrado (ejecutada, no accionable, fallida) y una falla no detiene el pipeline (RN-01, ADR-08), igual que TC-06/TC-44; un test fuerza la condición de no accionable por jerarquía o permiso y verifica que el pipeline continúa.
- Modo simulación: en simulación, la nueva acción se registra sin ejecutarse (RN-09), verificable con el enfoque de TC-07/TC-22.

Tipo de pruebas: Unit del resultado y del orden; Integration de la ejecución contra el doble del adaptador.

## 5. Frontera reservada: propuesta de configuración (extensibilidad §2)

La frontera `PropuestaDeConfiguracion` está reservada, no construida en v1 (ADR-10, `usa_llm = false`). No se construyen tests de su forma final en v1. Lo que sí se asegura con tests es el sustrato contra el cual una propuesta se validaría, previsualizaría, simularía y confirmaría, de modo que la frontera quede testeable cuando se construya:

- Validación contra descriptores: la propuesta se validaría con la misma validación por descriptor; los TC-21/TC-39/TC-40 garantizan que ese sustrato funciona y rechaza valores fuera de límite.
- Previsualización: el panel deriva leyenda, ejemplos y límites del descriptor; cubierto por la E2E mínima del panel de configuración (§2 de esta guía).
- Modo simulación: una propuesta se ejercitaría en modo simulación antes de aplicarse; los TC-07, TC-22, TC-23, TC-24 garantizan que el modo simulación registra sin ejecutar y que la promoción a real funciona.
- Confirmación humana explícita: el contrato conceptual reservado exige confirmación humana antes de aplicar; cuando la frontera se construya, su test deberá verificar que ningún cambio se aplica sin una acción de confirmación del administrador y que un cambio no confirmado no toca la configuración vigente.

Criterio de cierre de v1 respecto de la frontera: basta con que los cuatro sustratos anteriores estén verdes; no se exige ningún test de la propuesta en sí, porque no se construye (alcance Won't Have v1, `SOLUTION-INTAKE §4`).

## 6. Trazabilidad de la guía

| Punto de extensión | ADR | CU/RN relacionados | TC de sustrato | Sample en 11 |
| --- | --- | --- | --- | --- |
| Nuevos descriptores de parámetro | ADR-12 | CU-11, RN-10 | TC-21, TC-39, TC-40 | sample (c) |
| Nuevos tipos de regla | ADR-12, ADR-04 | CU-01, CU-04, RN-03 | TC-01, TC-03, TC-13, TC-14 | sample (b) |
| Nuevos tipos de acción | ADR-12, ADR-08, ADR-13 | CU-02, CU-03, RN-05, RN-01 | TC-06, TC-19, TC-44 | sample (b) |
| Frontera de propuesta de configuración (reservada) | ADR-10, ADR-12 | CU-11, CU-14, RN-09 | TC-07, TC-22, TC-23, TC-24, TC-39 | no aplica (no construida en v1) |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Guía inicial de testing de extensibilidad para `discord-bots-admin`: cómo testear nuevos descriptores, tipos de regla y tipos de acción a través de la superficie de configuración dirigida por descriptores sin modificar el núcleo, y qué sustrato se asegura con tests para la frontera reservada de propuesta de configuración. |
