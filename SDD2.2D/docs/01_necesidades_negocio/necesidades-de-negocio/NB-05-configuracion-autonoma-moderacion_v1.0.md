# NB-05 — Configuración autónoma de la moderación por el administrador

| Campo | Valor |
| --- | --- |
| Proyecto | discord-bots-admin |
| Documento | NB-05-configuracion-autonoma-moderacion_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-20 |
| Autor | Analista de Negocio Senior (AG-01) |
| Trazabilidad upstream | SOLUTION-INTAKE §3, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-08, CU-09, CU-10, CU-11 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

El negocio necesita que el administrador pueda gobernar la moderación por sí mismo, ajustando la sensibilidad y las reglas sin depender de un técnico y sin conocimiento técnico profundo. La sensibilidad de la detección, los umbrales, las ventanas de tiempo, las reglas, sus agrupaciones y las acciones asociadas son decisiones del negocio que cambian según la comunidad y el momento. Si configurar el sistema exigiera intervención del implementador cada vez, la herramienta sería inutilizable para un operador único que la administra a diario.

Hoy el cliente confía la moderación a un filtro nativo configurado con expresiones regulares cuyos parámetros son crípticos y difíciles de calibrar. La necesidad es ofrecer una administración donde cada parámetro configurable traiga su valor por defecto, su explicación y ejemplos en pantalla, de modo que el administrador entienda qué está ajustando y con qué efecto. El negocio también necesita un punto de entrada seguro y único: una cuenta de administrador con sus credenciales creadas en el primer ingreso, desde la cual se registran los servidores con su credencial de acceso y se administran las reglas, los grupos, los eventos y las acciones.

Esta necesidad es la que vuelve operable y sostenible al producto en el día a día. Sin una configuración autónoma y guiada, las capacidades de detección y de contención no podrían ajustarse a la realidad cambiante de cada comunidad, y el administrador quedaría atado al implementador para cualquier cambio.

## 2. Ejemplo de uso desde la perspectiva del negocio

El administrador decide que su comunidad necesita una detección más estricta porque viene sufriendo ataques. Entra al panel con su cuenta, encuentra el parámetro de cantidad de canales distintos con su valor por defecto, una leyenda que explica qué significa y un ejemplo de cómo afecta la sensibilidad, lo baja un punto, crea un grupo de reglas y define la acción a tomar. Todo lo hace solo, en minutos, sin escribir una sola expresión técnica ni pedirle nada al implementador.

## 3. Impacto

- El administrador opera y ajusta la moderación de forma autónoma, sin depender del implementador para cada cambio.
- La sensibilidad de la detección se adapta a la realidad cambiante de cada comunidad.
- La curva de aprendizaje baja gracias a valores por defecto, leyendas y ejemplos por parámetro.
- El acceso queda acotado a una cuenta de administrador con credenciales propias, como punto de entrada único y seguro.
- Si la necesidad no se resuelve, el sistema no es operable por su usuario objetivo y las capacidades de detección quedan congeladas en su configuración inicial.

## 4. Problema específico que resuelve

- La configuración del filtro nativo es críptica y difícil de calibrar para un operador no técnico.
- No hay hoy un punto único y seguro desde el cual administrar la moderación de los servidores.
- El administrador no puede ajustar umbrales, reglas, grupos, eventos y acciones por sí mismo.
- Falta ayuda en pantalla que explique cada parámetro y su efecto sobre la sensibilidad.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Autonomía de configuración | Porcentaje de cambios de configuración que el administrador realiza sin asistencia técnica | 100 % | Mensual |
| Cobertura de ayuda contextual | Porcentaje de parámetros configurables con valor por defecto, leyenda y ejemplo en pantalla | 100 % | Disponible en v1 |
| Tiempo de ajuste de un parámetro | Minutos para localizar y cambiar un umbral o ventana desde el panel | ≤ 5 min | Por ajuste |
| Acceso administrado | Cantidad de cuentas de administrador con credenciales propias creadas en el primer ingreso | 1 | Disponible en v1 |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Administrador propietario de los servidores de Discord | Propietario | Exige poder gobernar la moderación por sí mismo y aprueba el modelo de acceso |
| Fernando | Implementador | Construye el panel, la configuración guiada por descriptores y el alta de la cuenta administradora |
| Administrador del sistema | Beneficiario | Valida que puede configurar reglas y umbrales sin conocimiento técnico profundo |
| Miembros de las comunidades moderadas | Beneficiario indirecto | Se benefician de una moderación calibrada a la realidad de su comunidad |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-05 | CU-08 dar de alta las credenciales del administrador en el primer ingreso | a generar |
| NB-05 | CU-09 autenticar al administrador | a generar |
| NB-05 | CU-10 registrar un servidor con su credencial de acceso | a generar |
| NB-05 | CU-11 administrar reglas, grupos, eventos, acciones y parámetros con ayuda contextual | a generar |

## 8. Dependencias con otras NB

Sin dependencias. Es una necesidad habilitante: provee la superficie de administración que el resto de las NB de detección, contención y revisión utilizan para ser configuradas y operadas.

## 9. Prioridad MoSCoW

Must Have. El panel de administración, la cuenta administradora única y la configuración con ayuda contextual son Must Have en el intake; sin ellos el sistema no es operable por su usuario objetivo.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada del alcance Must Have del intake y de la propuesta de valor |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: alineación de IDs y títulos de CU en §7 con la numeración canónica consolidada del 02 (incorporación de CU-09 autenticar al administrador y reasignación a CU-08, CU-09, CU-10, CU-11) |
