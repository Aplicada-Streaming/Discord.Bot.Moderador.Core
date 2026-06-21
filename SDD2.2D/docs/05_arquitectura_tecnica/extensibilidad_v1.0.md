# Extensibilidad — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)

Este documento describe los puntos de extensión del sistema (`tiene_extensibilidad = true`): la superficie de extensión de la configuración dirigida por esquema y la frontera reservada de propuesta de configuración. Cada punto de extensión referencia la ADR que lo justifica y, cuando corresponde, el ejemplo previsto en la categoría 11.

## 1. Superficie de extensión: configuración dirigida por esquema

La configuración del sistema se dirige por descriptores: cada parámetro configurable se describe con un descriptor único que es fuente de verdad de su default, sus límites, su leyenda y sus ejemplos (ADR-12, RN-10). Esto define la superficie de extensión principal de v1.

### 1.1 Nuevos descriptores de parámetro

- Qué se extiende: agregar un parámetro configurable nuevo (por ejemplo, un nuevo umbral o una nueva ventana) registrando su descriptor.
- Contrato del punto de extensión: el descriptor declara identificador, tipo de valor, default, límites (mínimo/máximo o conjunto permitido), leyenda y ejemplos. La validación, la aplicación de default y la ayuda en pantalla se derivan del descriptor sin tocar la lógica genérica de validación.
- ADR que lo justifica: ADR-12.
- Impacto: el Servicio de configuración valida contra el nuevo descriptor; el panel deriva la ayuda automáticamente.

### 1.2 Nuevos tipos de regla

- Qué se extiende: agregar un tipo de regla (de contenido o de conducta) más allá de los iniciales (expresión regular, palabras clave, canales distintos, frecuencia).
- Contrato del punto de extensión: una regla nueva implementa el predicado correspondiente a su clase (sin estado para contenido; con estado sobre la ventana de actividad para conducta) y declara sus parámetros mediante descriptores (§1.1). Su criterio se valida según la clase (RC-09, RN-03).
- ADR que lo justifica: ADR-12 (descriptores), ADR-04 (el predicado vive en el Dominio, aislado de la infraestructura).
- Impacto: el motor de moderación evalúa el nuevo tipo en las etapas 2 o 3 del pipeline (`flujo-ejecucion_v1.0.md`).

### 1.3 Nuevos tipos de acción

- Qué se extiende: agregar un tipo de acción de moderación más allá de los iniciales (reportar, banear, banear con borrado, desbanear, timeout, expulsar, asignar/quitar rol).
- Contrato del punto de extensión: una acción nueva implementa la operación contra el Adaptador del gateway/API, declara su orden de ejecución y sus parámetros por descriptores, y devuelve un resultado del conjunto cerrado (ejecutada, no accionable, fallida) (RN-05, RN-01, ADR-08).
- ADR que lo justifica: ADR-12, ADR-08 (manejo de resultados), ADR-13 (ejecución por contexto).
- Impacto: el Ejecutor de acciones la incorpora en la etapa 8 del pipeline manteniendo el orden determinista.

## 2. Frontera reservada: propuesta de configuración (PropuestaDeConfiguracion)

Frontera reservada en v1 y no construida (ADR-10, `usa_llm = false`).

- Qué reserva: una capa de propuesta de configuración validable en la que un asistente automatizado (a futuro, eventualmente un modelo de lenguaje) propone una configuración, el sistema la valida contra los descriptores, el administrador la previsualiza, la prueba en modo simulación y la confirma antes de aplicarla. La IA propone; el sistema valida; el humano confirma.
- Estado en v1: solo se reserva la frontera; no se diseña ni se construye su forma final (ADR-10, `SOLUTION-INTAKE §17 P.11`, §4 Won't Have).
- Sustrato sobre el que se enchufa: la validación de configuración dirigida por descriptores (ADR-12) y el modo simulación del motor (RN-09, CU-14) son el contrato contra el cual una propuesta se validaría, previsualizaría y simularía antes de aplicarse.
- Contrato conceptual reservado: una `PropuestaDeConfiguracion` sería un conjunto de cambios de configuración candidatos que: (a) se validan contra los descriptores; (b) se previsualizan en el panel; (c) se ejercitan en modo simulación; (d) requieren confirmación humana explícita para aplicarse. Ninguna de estas piezas se implementa en v1.
- ADR que lo justifica: ADR-10 (omisión de contratos de prompts y reserva de la frontera), ADR-12 (validación por descriptores como sustrato).

## 3. Lo que no es extensible en v1

- No hay un sistema de plugins de terceros ni una superficie pública para integradores (web-monolith de operador único, ADR-11). Las extensiones de §1 son internas al proyecto.
- No se expone una API externa a terceros (`SOLUTION-INTAKE §17 P.3`); por eso no se generan contratos externos.
- La frontera de propuesta de configuración (§2) está reservada, no construida.

## 4. Ejemplo de extensión previsto en 11

El ejemplo de extensión previsto en la categoría 11 (`SOLUTION-INTAKE §16.1, §18`) es el sample (b): disparo de la detección de ráfaga con mensajes simulados que muestra el baneo, ejercitando el motor de evaluación y la acción de baneo, y el sample (c): página mínima del panel que ejercita la capa de configuración por descriptores (§1.1). Estos samples demuestran cómo un nuevo descriptor o un nuevo tipo de regla/acción se incorporaría a través de la superficie de extensión declarada.

## 5. Trazabilidad

| Punto de extensión | ADR que lo justifica | CU/RN relacionados | Ejemplo en 11 |
| --- | --- | --- | --- |
| Nuevos descriptores de parámetro | ADR-12 | CU-11, RN-10 | sample (c) |
| Nuevos tipos de regla | ADR-12, ADR-04 | CU-01, CU-04, RN-03, RC-09 | sample (b) |
| Nuevos tipos de acción | ADR-12, ADR-08, ADR-13 | CU-02, CU-03, RN-05, RN-01 | sample (b) |
| Frontera de propuesta de configuración (reservada) | ADR-10, ADR-12 | CU-11, CU-14, RN-09 | no aplica (no construida en v1) |

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Puntos de extensión iniciales: configuración dirigida por descriptores (nuevos descriptores, tipos de regla y de acción) y frontera reservada de propuesta de configuración, con trazabilidad a ADR y a los samples de 11. |
