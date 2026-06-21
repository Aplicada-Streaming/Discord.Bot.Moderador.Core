# CU-08 — Dar de alta las credenciales del administrador en el primer ingreso

**Proyecto:** discord-bots-admin
**Documento:** CU-08-alta-credenciales-administrador-primer-ingreso_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Crear, en el primer ingreso al sistema (first-run setup), la cuenta única de administrador con sus credenciales propias, para establecer el punto de entrada seguro desde el cual se opera toda la moderación. Es la condición que habilita el resto de las funciones administrativas.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del sistema | Primario | Define el identificador y la contraseña de la cuenta de administrador en el primer uso |
| Servicio de administración | Sistema | Verifica que no exista cuenta previa, valida y persiste las credenciales con resguardo |

## 3. Precondiciones

- El sistema está instalado y operativo.
- No existe todavía ninguna cuenta de administrador registrada (RN-13).
- El sistema está en estado de primer ingreso (first-run setup).

## 4. Flujo principal

1. El administrador accede al sistema por primera vez.
2. El servicio detecta que no existe ninguna cuenta de administrador y presenta el alta de credenciales.
3. El administrador ingresa el identificador de la cuenta y una contraseña, con su confirmación.
4. El servicio valida que la contraseña cumple la política mínima de robustez definida.
5. El servicio almacena la contraseña con un resguardo de hash robusto, nunca en texto claro (RN-13).
6. El servicio marca el primer ingreso como completado y queda con una única cuenta de administrador.
7. El servicio dirige al administrador a la autenticación (CU-09) para iniciar sesión.

## 5. Flujos alternativos

- 5.A Contraseña débil. Disparador: la contraseña no cumple la política mínima de robustez. Acción: el servicio rechaza el alta y explica el requisito incumplido. Punto de retorno: retorna al paso 3.
- 5.B Confirmación no coincide. Disparador: la contraseña y su confirmación difieren. Acción: el servicio rechaza el alta e indica la diferencia. Punto de retorno: retorna al paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| SETUP_YA_COMPLETADO | Ya existe una cuenta de administrador y se intenta acceder al alta de primer ingreso (RN-13) | Bloquea el alta y redirige a la autenticación (CU-09) |
| SETUP_CONTRASENA_DEBIL | La contraseña no cumple la política mínima de robustez | Rechaza el alta y solicita una contraseña que cumpla la política |
| SETUP_PERSISTENCIA_FALLIDA | No se pudo persistir la cuenta de administrador | No completa el primer ingreso; informa el fallo y mantiene el estado de first-run para reintentar |

## 7. Postcondiciones

- En caso de éxito: existe exactamente una cuenta de administrador con su contraseña resguardada; el primer ingreso queda completado y no vuelve a ofrecerse.
- En caso de fallo: no se crea cuenta; el sistema permanece en estado de primer ingreso.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un sistema recién instalado sin cuenta de administrador | El administrador ingresa un identificador y una contraseña robusta con su confirmación coincidente | El servicio crea la cuenta única, resguarda la contraseña con hash y completa el primer ingreso |
| CA-02 | Un sistema en first-run | El administrador ingresa una contraseña que no cumple la política mínima de robustez | El servicio rechaza el alta con código SETUP_CONTRASENA_DEBIL y solicita otra contraseña |
| CA-03 | Un sistema con una cuenta de administrador ya creada | Alguien intenta acceder al alta de primer ingreso | El servicio bloquea el alta con código SETUP_YA_COMPLETADO y redirige a la autenticación |
| CA-04 | Un sistema en first-run | El administrador ingresa una contraseña cuya confirmación no coincide | El servicio rechaza el alta e indica la diferencia, sin crear la cuenta |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-12, RN-13 |
| Historias de usuario a generar | US a generar en 06 (first-run setup; política de contraseña; creación de la cuenta única) |
| Componentes esperados | Flujo de primer ingreso; servicio de credenciales con resguardo de hash; persistencia del administrador (referencia tentativa a 05) |
| Tests previstos | Pruebas de creación única; pruebas de política de contraseña; pruebas de bloqueo cuando ya existe cuenta (referencia tentativa a 08) |

## 10. Notas y supuestos

- El sistema admite una única cuenta de administrador; no hay gestión de múltiples cuentas ni roles, según el alcance.
- El identity provider es local; no hay proveedor externo de identidad.
- El resguardo concreto de la contraseña (formato y algoritmo) se decide en la categoría 05; aquí solo se exige que nunca se almacene en texto claro y que cumpla una política de robustez.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-05 y del requisito de autenticación del intake |
