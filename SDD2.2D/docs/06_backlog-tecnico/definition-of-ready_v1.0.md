# Definition of Ready — discord-bots-admin

**Proyecto:** discord-bots-admin
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-20
**Autor:** Scrum Master (AG-06)

La Definition of Ready (DoR) fija cuándo un ítem del backlog puede entrar a Sprint Planning. Describe el umbral de entrada al sprint, no el umbral de salida: la condición de terminado (Definition of Done) vive en la categoría 08 y no se solapa con este documento. Cada criterio se responde con sí o no de forma objetiva; un ítem que no cumple todos los criterios obligatorios no entra al sprint salvo excepción explícita (sección 3). El aprobador (sección 4) firma la entrada.

## 1. Criterios DoR para una historia de usuario

Una US está Ready cuando cumple los siete criterios siguientes:

1. La historia está redactada en formato `Como [rol], quiero [acción], para [valor]` con el valor para el rol explícito y no vacío.
2. Declara al menos un CU relacionado de la categoría 02 en su columna de trazabilidad; no hay US sin CU.
3. Tiene criterios de aceptación en formato Given/When/Then con al menos dos escenarios, uno de camino feliz y uno de borde, para toda US de prioridad Must o Should.
4. Está priorizada en MoSCoW con justificación y estimada en puntos de historia con la técnica Fibonacci declarada en el product-backlog.
5. No tiene dependencias bloqueantes sin resolver: las BT y las US prerequisito están identificadas y planificadas o ya cerradas.
6. Los parámetros configurables que la historia toca tienen un descriptor de origen identificado (default, límites, leyenda y ejemplos) cuando aplica, o se declara explícitamente que no toca configuración dirigida por descriptor.
7. Los datos o escenarios de prueba necesarios para verificar los criterios están disponibles o son construibles dentro del sprint (por ejemplo, mensajes simulados para el pipeline, servidor de prueba con su credencial de prueba).

## 2. Criterios DoR para una tarea técnica

Una BT está Ready cuando cumple los cinco criterios siguientes:

1. Tiene una fuente upstream declarada y verificable en la categoría 05 (componente de la vista lógica, ADR, modelo lógico o punto de extensión) o en una RN/CU de 02; no se admite BT sin justificación.
2. Declara al menos una US consumidora, o se justifica explícitamente como infraestructura compartida con la ADR que la respalda.
3. Tiene un alcance acotado a una tarea ejecutable en menos de un sprint, con criterios de aceptación técnicos verificables (compila, los tests previstos pasan, el contrato o la restricción declarada se respeta).
4. Tiene sus dependencias técnicas identificadas (otras BT o US que deben estar terminadas antes) y sin bloqueos externos sin resolver.
5. Está estimada con la técnica Fibonacci declarada; si es una spike, tiene caja temporal explícita y una pregunta de investigación concreta a responder.

## 3. Excepciones admitidas

- Spike exploratoria: una BT de tipo spike puede entrar sin criterios de aceptación cerrados de implementación, a cambio de declarar una pregunta de investigación, una caja temporal explícita y el entregable esperado (informe o recomendación). La aprueba el aprobador de la sección 4.
- Walking skeleton del primer sprint: las US y BT que componen la primera rebanada vertical end-to-end pueden entrar con criterios de aceptación reducidos al camino feliz demostrable, dado que su objetivo es validar el recorrido completo. La excepción se documenta en la US o BT y la firma el aprobador.
- Ítem dependiente de una decisión abierta a Sprint 0 (por ejemplo, elección puntual de familia de hash o de herramienta de versionado): puede entrar si la decisión no bloquea el camino feliz y se registra como supuesto a confirmar dentro del sprint.

Toda excepción se documenta en el ítem afectado y caduca al cierre del sprint en que se usó.

## 4. Aprobador

El rol responsable de validar que un ítem cumple la DoR antes de Sprint Planning es el Scrum Master (AG-06), que en este proyecto de un único desarrollador asume también la curaduría del backlog. Las revisiones acotadas de trazabilidad (AG-02 para US sin CU huérfanas) y de fuente técnica (AG-05 para BT sin fuente upstream) se incorporan al criterio del aprobador cuando hay duda sobre los criterios 2 (US) o 1 (BT). El aprobador deja registro del cumplimiento en el DoR check de cada US y en los criterios de cada BT.

## 5. Relación con la Definition of Done (08)

La DoR no enumera criterios de terminado: no habla de cobertura alcanzada, de revisión de código, de despliegue ni de aceptación final, que son competencia de la Definition of Done de la categoría 08. La DoR pregunta si el ítem está listo para empezar; la DoD pregunta si el ítem está listo para cerrar. Ambos filtros son complementarios y no comparten criterios.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | DoR inicial: siete criterios para US, cinco para BT, tres excepciones admitidas y aprobador declarado, sin solapamiento con la DoD de 08. |
