# CU-15 — Definir exenciones por rol, usuario o canal de confianza

**Proyecto:** discord-bots-admin
**Documento:** CU-15-definir-exenciones_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Permitir que el administrador declare sujetos exentos —por rol, por usuario o por canal de confianza— que deben quedar fuera de la moderación, para que el staff y los espacios legítimos nunca sean alcanzados por una acción. Las exenciones se descartan antes de evaluar cualquier regla.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Declara y mantiene las exenciones por rol, usuario o canal |
| Servicio de moderación | Sistema | Aplica las exenciones descartando a los sujetos exentos antes de evaluar (RN-07) |

## 3. Precondiciones

- El administrador está autenticado en el panel (CU-09).
- Existe un servidor registrado sobre el cual definir exenciones (CU-10).
- Los identificadores de rol, usuario o canal se manejan como snowflakes almacenados como texto (RN-08).

## 4. Flujo principal

1. El administrador abre la sección de exenciones de un servidor.
2. El administrador agrega una exención indicando su tipo (rol, usuario o canal) y el identificador correspondiente.
3. El servicio valida el identificador como snowflake y persiste la exención asociada al servidor (RN-08).
4. El servicio confirma que el sujeto exento queda fuera de la moderación a partir de ese momento.
5. Durante la operación, el servicio descarta a los sujetos exentos antes de evaluar cualquier regla de contenido o de conducta (RN-07).

## 5. Flujos alternativos

- 5.A Eliminación de una exención. Disparador: el administrador quita un sujeto de la lista de exentos. Acción: el servicio elimina la exención y el sujeto vuelve a estar sujeto a la moderación. Punto de retorno: retorna al paso 4 con la lista actualizada.
- 5.B Exención por canal de confianza. Disparador: el administrador declara un canal como de confianza. Acción: el servicio excluye la actividad de ese canal de la evaluación de reglas. Punto de retorno: retorna al paso 4 incluyendo el canal exento.
- 5.C Exención por rol que abarca varios usuarios. Disparador: el administrador exime un rol. Acción: el servicio exime a todos los usuarios que porten ese rol, sin enumerarlos uno por uno. Punto de retorno: retorna al paso 4 con la exención por rol vigente.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| EXENCION_IDENTIFICADOR_INVALIDO | El identificador de rol, usuario o canal no tiene formato de snowflake (RN-08) | Rechaza la exención e indica el formato esperado |
| EXENCION_DUPLICADA | Se intenta crear una exención que ya existe para el mismo sujeto y servidor | No crea un duplicado e informa que la exención ya existe |
| EXENCION_SIN_AUTORIZACION | Quien intenta definir la exención no es el administrador autenticado (RN-12) | Rechaza la operación y redirige a la autenticación (CU-09) |

## 7. Postcondiciones

- En caso de éxito: la exención queda persistida y los sujetos exentos quedan fuera de la evaluación de la moderación.
- En caso de fallo: la exención no se crea o no se modifica; los sujetos no exentos siguen sujetos a la moderación.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un administrador autenticado y un rol staff a eximir | El administrador crea una exención por ese rol | El servicio persiste la exención y los usuarios con ese rol quedan fuera de la moderación |
| CA-02 | Un servidor con una exención por rol staff vigente | Un usuario con rol staff publica en 5 canales distintos | El servicio descarta al usuario antes de evaluar y no lo contiene |
| CA-03 | Un administrador agregando una exención | El administrador ingresa un identificador que no es un snowflake válido | El servicio rechaza la exención con código EXENCION_IDENTIFICADOR_INVALIDO |
| CA-04 | Un canal declarado de confianza | Un usuario publica contenido que cumpliría una regla en ese canal | El servicio excluye la actividad de ese canal de la evaluación y no contiene |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 |
| Reglas de negocio aplicables | RN-07, RN-08, RN-12 |
| Historias de usuario a generar | US a generar en 06 (alta y baja de exenciones por rol, usuario y canal; aplicación previa a la evaluación) |
| Componentes esperados | Página de exenciones; servicio de exenciones; filtro de descarte previo en el pipeline (referencia tentativa a 05) |
| Tests previstos | Pruebas de exención por rol, usuario y canal; pruebas de descarte previo a la evaluación; pruebas de identificador inválido (referencia tentativa a 08) |

## 10. Notas y supuestos

- Las exenciones se descartan antes de evaluar cualquier regla, de modo que un sujeto exento nunca puede ser alcanzado por una acción (RN-07).
- La exención por rol cubre dinámicamente a quienes porten el rol, sin necesidad de enumerar usuarios.
- El indicador de negocio exige que ningún sujeto exento sea alcanzado por una acción de moderación.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-07 y de los casos límite del intake |
