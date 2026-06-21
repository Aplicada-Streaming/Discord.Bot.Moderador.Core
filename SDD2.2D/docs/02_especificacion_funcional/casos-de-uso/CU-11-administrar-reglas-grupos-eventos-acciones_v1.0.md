# CU-11 — Administrar reglas, grupos, eventos, acciones y parámetros con ayuda contextual

**Proyecto:** discord-bots-admin
**Documento:** CU-11-administrar-reglas-grupos-eventos-acciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Permitir que el administrador cree y ajuste por sí mismo las reglas de contenido y de conducta, las agrupe, defina eventos con sus acciones y configure los parámetros de la moderación, apoyándose en el valor por defecto, la leyenda y los ejemplos de cada parámetro, para gobernar la sensibilidad sin conocimiento técnico profundo y sin depender del implementador.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Crea y ajusta reglas, grupos, eventos, acciones y parámetros |
| Servicio de administración | Sistema | Valida los valores contra los descriptores y persiste la configuración |

## 3. Precondiciones

- El administrador está autenticado en el panel (CU-09).
- Existe al menos un servidor registrado (CU-10) sobre el cual configurar la moderación.
- Cada parámetro configurable tiene un descriptor con su valor por defecto, sus límites, su leyenda y sus ejemplos (RN-10).

## 4. Flujo principal

1. El administrador abre la sección de configuración de la moderación de un servidor.
2. El administrador crea una regla de contenido (expresión regular o palabras clave) o de conducta (frecuencia o canales distintos) y configura sus parámetros.
3. El servicio presenta, por cada parámetro, su valor por defecto, su leyenda y sus ejemplos, y valida el valor ingresado contra los límites del descriptor (RN-10).
4. El administrador agrupa una o más reglas en un grupo de reglas y elige su modo de coincidencia: todas, alguna, o al menos N (RN-15).
5. El administrador define un evento o política asociando uno o más grupos de reglas y configura su prioridad y su bandera continuar (RN-04).
6. El administrador asocia al evento una o más acciones en el orden de ejecución deseado (RN-05).
7. El servicio valida la configuración completa y la persiste; el evento queda disponible, por defecto en modo simulación hasta que el administrador lo promueva (RN-09).

## 5. Flujos alternativos

- 5.A Edición de configuración existente. Disparador: el administrador modifica una regla, grupo, evento o acción ya creado. Acción: el servicio valida el cambio contra los descriptores y lo persiste. Punto de retorno: retorna al paso 7 con la configuración actualizada.
- 5.B Valor por defecto. Disparador: el administrador no ingresa un valor para un parámetro. Acción: el servicio aplica el valor por defecto del descriptor (RN-10). Punto de retorno: retorna al paso 3 con el valor por defecto cargado.
- 5.C Eliminación de un elemento de configuración. Disparador: el administrador elimina una regla, grupo, evento o acción. Acción: el servicio verifica que no rompa referencias requeridas antes de eliminar y persiste el cambio. Punto de retorno: retorna al paso 7.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CONFIG_VALOR_FUERA_DE_LIMITE | Un valor ingresado queda fuera de los límites del descriptor del parámetro (RN-10) | Rechaza el valor, muestra los límites permitidos y ofrece el valor por defecto |
| CONFIG_REFERENCIA_REQUERIDA | Se intenta eliminar un elemento referenciado por otro (por ejemplo un grupo usado por un evento activo) | Bloquea la eliminación e indica las referencias que deben resolverse primero |
| CONFIG_GRUPO_SIN_REGLAS | Se intenta definir un grupo de reglas sin ninguna regla asociada (RN-15) | Rechaza el grupo e indica que debe contener al menos una regla |

## 7. Postcondiciones

- En caso de éxito: la configuración de reglas, grupos, eventos, acciones y parámetros queda persistida y coherente con los descriptores; los eventos nuevos quedan por defecto en modo simulación.
- En caso de fallo: la configuración no se persiste o conserva su estado anterior; el motivo del rechazo queda informado.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un parámetro de umbral de canales distintos con valor por defecto 3 y límites de 2 a 10 | El administrador ingresa el valor 4 | El servicio acepta y persiste el valor 4 mostrando la leyenda y el ejemplo del parámetro |
| CA-02 | El mismo parámetro con límites de 2 a 10 | El administrador ingresa el valor 1 | El servicio rechaza el valor con código CONFIG_VALOR_FUERA_DE_LIMITE y muestra los límites permitidos |
| CA-03 | Un administrador definiendo un grupo de reglas | El administrador intenta guardar el grupo sin asociar ninguna regla | El servicio rechaza el grupo con código CONFIG_GRUPO_SIN_REGLAS |
| CA-04 | Un evento que referencia un grupo de reglas | El administrador intenta eliminar ese grupo | El servicio bloquea la eliminación con código CONFIG_REFERENCIA_REQUERIDA e indica el evento que lo usa |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-04, RN-05, RN-09, RN-10, RN-12, RN-15 |
| Historias de usuario a generar | US a generar en 06 (alta y edición de reglas, grupos, eventos y acciones; ayuda contextual por parámetro; modos de coincidencia de grupo) |
| Componentes esperados | Páginas de configuración; descriptores de parámetros como fuente de verdad; servicio de validación y persistencia (referencia tentativa a 05) |
| Tests previstos | Pruebas de validación contra descriptores; pruebas de modos de coincidencia; pruebas de integridad referencial al eliminar (referencia tentativa a 08) |

## 10. Notas y supuestos

- La configuración es dirigida por esquema: cada descriptor es la fuente única de verdad del valor por defecto, los límites, la leyenda y los ejemplos del parámetro.
- El anidamiento booleano se limita a dos niveles: grupo de reglas y combinación de grupos en un evento, según el alcance excluido.
- La presentación visual de la ayuda contextual corresponde a la categoría 03; aquí se exige que cada parámetro exponga su valor por defecto, su leyenda y sus ejemplos.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-05 y de la configuración dirigida por esquema del intake |
