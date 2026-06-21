# NB-07 — Mitigación del riesgo de moderación errónea

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-07-mitigacion-moderacion-erronea_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §4, §7, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-14, CU-15, CU-16 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

El negocio necesita poder ganar confianza en una regla antes de dejar que actúe sola sobre los miembros, y necesita que la moderación no castigue a quienes no debe ni se ensañe innecesariamente durante un mismo ataque. El riesgo de mayor impacto es contener a un usuario legítimo; ese riesgo no se elimina solo con la revisión posterior, sino que se reduce mucho si el administrador puede observar el comportamiento de una regla sin consecuencias reales antes de activarla, si puede excluir de antemano a sujetos de confianza y si el sistema evita repetir acciones sobre el mismo usuario durante una ráfaga.

Hoy no hay forma de probar una regla en frío ni de proteger explícitamente al staff y a los canales de confianza, y una regla mal calibrada actuaría de inmediato sobre miembros reales. La necesidad tiene tres facetas complementarias. La primera es un modo de ensayo por política que registre qué habría hecho sin ejecutarlo, para que el administrador calibre con datos reales antes de pasar a ejecución. La segunda son las exenciones por rol, usuario o canal de confianza, para que el staff y los espacios legítimos nunca sean alcanzados. La tercera es un mecanismo que evite repetir la misma acción sobre un usuario varias veces durante un mismo ataque.

Esta necesidad complementa la trazabilidad de NB-04: mientras aquélla acota el costo del error después de ocurrido, ésta reduce la probabilidad de que el error ocurra. Su valor para el negocio es hacer la moderación automática prudente y adoptable, bajando la tasa de falsos positivos por diseño y no solo por corrección posterior.

## 2. Ejemplo de uso desde la perspectiva del negocio

El administrador crea una nueva regla de conducta pero no está seguro de su umbral. La deja en modo de ensayo una semana: el sistema le muestra a quiénes habría contenido sin haberlo hecho de verdad. Ve que la regla habría alcanzado a un moderador que usa varios canales por su trabajo, agrega a ese rol como exención y ajusta el umbral. Recién cuando los ensayos dejan de mostrar falsos positivos, pasa la regla a ejecución real, ahora con la tranquilidad de que el staff está protegido y de que un mismo atacante no recibirá la acción repetida durante su ráfaga.

## 3. Impacto

- Reduce por diseño la probabilidad de contener a usuarios legítimos antes de que el error ocurra.
- Permite calibrar reglas con datos reales del propio servidor sin consecuencias para los miembros.
- Protege explícitamente al staff y a los canales de confianza de ser alcanzados por la moderación.
- Evita acciones repetidas sobre el mismo usuario durante un único ataque, reduciendo ruido y reacciones desproporcionadas.
- Si la necesidad no se resuelve, cada regla nueva entra en producción a ciegas y el riesgo de falsos positivos crece, frenando la adopción.

## 4. Problema específico que resuelve

- No hay forma de observar el comportamiento de una regla antes de que actúe sobre miembros reales.
- El staff y los canales de confianza pueden ser alcanzados por la moderación sin una exclusión explícita.
- Un mismo usuario puede recibir la misma acción varias veces durante una sola ráfaga.
- El riesgo de falso positivo se gestiona hoy solo después del error, no antes de que suceda.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Calibración previa de reglas | Porcentaje de reglas nuevas que pasan por modo de ensayo antes de la ejecución real | 100 % | Por alta de regla |
| Protección de sujetos de confianza | Porcentaje de sujetos exentos alcanzados por una acción de moderación | 0 % | Por incidente |
| Reducción de falsos positivos por diseño | Porcentaje de contenciones revertidas por ser falso positivo sobre el total | ≤ 2 % | Mensual |
| Antirrebote por usuario durante una ráfaga | Acciones adicionales repetidas sobre el mismo usuario después de la primera durante una misma ráfaga | 0 | Por incidente |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Exige poder probar reglas y proteger al staff antes de activar la moderación |
| Fernando | Implementador | Construye el modo de ensayo, las exenciones y el antirrebote por usuario |
| Administrador del sistema | Beneficiario | Valida que puede calibrar reglas sin riesgo y que el staff nunca es alcanzado |
| Miembros de las comunidades moderadas | Beneficiario indirecto | Tienen menos probabilidad de ser contenidos por error |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-07 | CU-14 ejecutar una política en modo simulación registrando lo que haría sin ejecutarlo | a generar |
| NB-07 | CU-15 definir exenciones por rol, usuario o canal de confianza | a generar |
| NB-07 | CU-16 evitar acciones repetidas sobre el mismo usuario durante una ráfaga | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (corte automático de la ráfaga) y de NB-05 (configuración autónoma de la moderación), porque modula las acciones que NB-01 dispara y se configura desde el panel que NB-05 provee.

## 9. Prioridad MoSCoW

Should Have. El modo simulación, las exenciones y el antirrebote son Should Have en el intake; reducen los falsos positivos por diseño y refuerzan la mitigación de R-01, pero el corte de spam puede demostrarse sin ellos en el primer recorte.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del alcance Should Have, los casos límite y el riesgo R-01 del intake. Precisión del criterio de antirrebote a "0 acciones adicionales repetidas" para eliminar la ambigüedad del target anterior (observación P2-02 del audit de Fase A). |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: alineación de IDs y títulos de CU en §7 con la numeración canónica consolidada del 02 (CU-13, CU-14, CU-15 reasignadas a CU-14, CU-15, CU-16; título de la simulación alineado al del 02) |
