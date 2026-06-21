# Wireframes — Primer ingreso y autenticación

**Proyecto:** discord-bots-admin
**Documento:** wireframes-primer-ingreso-y-autenticacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Pantalla y propósito

Superficie de entrada al sistema. Cubre dos situaciones que comparten el mismo lienzo centrado sin barra lateral: el alta de la cuenta única de administrador en el primer ingreso (first-run setup) y la autenticación en cada ingreso posterior. El administrador crea su cuenta una sola vez y, a partir de entonces, inicia sesión para acceder al panel. Es la puerta que protege toda función administrativa.

## 2. Layout

Lienzo centrado, sin shell de aplicación (no hay barra lateral hasta que la sesión está abierta). Tarjeta única centrada vertical y horizontalmente sobre el lienzo de página.

Variante A — Primer ingreso (no existe cuenta):

```
+-------------------------------------------------------------+
|                                                             |
|                   [ logo de marca (SVG) ]                   |
|                                                             |
|        +-----------------------------------------+          |
|        |  Crear la cuenta de administrador        |         |
|        |  Es la única cuenta del sistema.         |         |
|        |                                          |         |
|        |  Identificador                           |         |
|        |  [______________________________]        |         |
|        |                                          |         |
|        |  Contraseña                              |         |
|        |  [______________________________] [ojo] |         |
|        |  Medidor de robustez: [====----]         |         |
|        |  Requisitos: <derivados de la politica>  |         |
|        |                                          |         |
|        |  Confirmar contraseña                    |         |
|        |  [______________________________]        |         |
|        |                                          |         |
|        |                 [ Crear cuenta ]         |         |
|        +-----------------------------------------+          |
|                                                             |
+-------------------------------------------------------------+
```

Variante B — Autenticación (ya existe cuenta):

```
+-------------------------------------------------------------+
|                                                             |
|                   [ logo de marca (SVG) ]                   |
|                                                             |
|        +-----------------------------------------+          |
|        |  Iniciar sesión                          |         |
|        |                                          |         |
|        |  Identificador                           |         |
|        |  [______________________________]        |         |
|        |                                          |         |
|        |  Contraseña                              |         |
|        |  [______________________________] [ojo] |         |
|        |                                          |         |
|        |  [ banner de error inline si aplica ]    |         |
|        |                                          |         |
|        |                 [ Iniciar sesión ]       |         |
|        +-----------------------------------------+          |
|                                                             |
+-------------------------------------------------------------+
```

El sistema elige la variante según exista o no la cuenta única: si no hay cuenta, presenta A; si ya existe, presenta B. Intentar el alta cuando ya hay cuenta redirige a B; intentar autenticarse sin cuenta redirige a A.

## 3. Componentes principales

| Componente | Patrón del catálogo | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- | --- |
| Tarjeta de acceso | Tarjeta (4.2) sobre lienzo | Contener el formulario centrado | Título y subtítulo de la variante | Estática; un solo punto de foco de la pantalla |
| Campo identificador | Controles de formulario (4.6) | Capturar el identificador de cuenta | Label visible; placeholder de ejemplo | Validación inline al desenfocar |
| Campo contraseña | Controles de formulario (4.6) | Capturar la contraseña | Label visible; texto oculto con alternancia de visibilidad | Botón de mostrar/ocultar con rótulo accesible |
| Medidor de robustez (solo A) | Indicador inline | Reflejar el cumplimiento de la política mínima | Nivel de robustez y requisitos pendientes | Se actualiza al teclear; los requisitos provienen de la política de contraseña, no se hardcodean en la pantalla |
| Campo confirmar (solo A) | Controles de formulario (4.6) | Verificar la coincidencia de la contraseña | Label visible | Marca diferencia con la contraseña al desenfocar |
| Botón primario | Botones (4.9) | Crear la cuenta (A) o iniciar sesión (B) | Verbo exacto de la acción | Queda ocupado durante la operación para prevenir doble envío |
| Banner de error inline | Estados y feedback (5) | Comunicar fallo recuperable | Causa y acción de recuperación | Aparece sobre el botón; se asocia al campo cuando el error es de campo |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Crear cuenta | El administrador completa A y activa el botón primario | El sistema valida robustez y coincidencia, crea la cuenta única y deriva a la autenticación (B) | No existe cuenta; contraseña robusta; confirmación coincidente |
| Rechazo por contraseña débil | La contraseña no cumple la política mínima | Mensaje inline con el requisito incumplido; el foco vuelve al campo de contraseña | Variante A activa |
| Rechazo por confirmación distinta | La confirmación difiere de la contraseña | Mensaje inline que indica la diferencia; no se crea la cuenta | Variante A activa |
| Iniciar sesión | El administrador completa B y activa el botón primario | El sistema verifica las credenciales y abre la sesión hacia el panel de estado | Existe cuenta; primer ingreso completado |
| Rechazo por credenciales | El identificador o la contraseña no coinciden | Mensaje neutro de credenciales inválidas sin precisar cuál falló; el intento queda registrado | Variante B activa |
| Bloqueo por intentos | Se supera el límite de intentos fallidos consecutivos | Demora o bloqueo temporal de nuevos intentos con aviso del tiempo de espera | Variante B activa |
| Alternar visibilidad de contraseña | El administrador activa el control de visibilidad | El texto de la contraseña se muestra u oculta | Cualquier variante |
| Redirección por estado | Se intenta la variante que no corresponde al estado de cuenta | El sistema redirige a la variante correcta (A↔B) | — |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío (first-run) | No existe cuenta de administrador | Variante A con campos vacíos e invitación a crear la cuenta única |
| Con datos | El administrador escribió credenciales | Campos completos; botón primario habilitado cuando el formulario es válido |
| Cargando | Verificación de credenciales o creación de cuenta en curso | Botón primario ocupado; campos en solo lectura mientras dura la operación |
| Error de entrada | Contraseña débil o confirmación distinta (A); credenciales inválidas (B) | Mensaje inline asociado al campo (A) o banner neutro (B); foco devuelto |
| Error de conflicto | Se intenta el alta con cuenta ya existente (SETUP_YA_COMPLETADO) | Redirección a B con aviso de que la cuenta ya existe |
| Error de capacidad | Demasiados intentos (AUTH_DEMASIADOS_INTENTOS) o persistencia fallida (SETUP_PERSISTENCIA_FALLIDA) | Banner con el motivo y el tiempo de espera, o invitación a reintentar el alta conservando el estado de first-run |
| Sesión vencida | La sesión activa superó su vigencia (CU-09 5.B) | Redirección a B con aviso de que la sesión expiró |
| Éxito | Cuenta creada o sesión abierta | Transición al destino: de A a B; de B al panel de estado |

## 6. Versión móvil o responsive

La tarjeta única ya es de ancho acotado; en viewport angosto ocupa el ancho disponible con márgenes laterales y mantiene el centrado vertical. El contenido es legible sin scroll horizontal a 320px de ancho (reflow conforme al catálogo). El medidor de robustez y la lista de requisitos reflujan a ancho completo. No hay barra lateral que colapsar en esta superficie.

## 7. Notas de implementación

- Accesibilidad: encabezado principal por variante; cada campo con su label asociado; el botón de visibilidad de contraseña con rótulo accesible; el banner de error asociado al campo o anunciado; foco visible en todos los controles; el mensaje de credenciales no revela cuál dato falló, para no facilitar la enumeración de cuentas.
- La superficie no usa almacenamiento de navegador improvisado; el estado de sesión lo gobierna el servicio.
- Performance percibida: el botón primario previene el doble envío durante la verificación; sin animación ambiental.
- Internacionalización: los requisitos de la política de contraseña provienen de la configuración del servicio y se presentan como lista, tolerando expansión de texto.
- Los detalles de robustez, formato y resguardo de la contraseña son de 05; aquí solo se representa el feedback de cumplimiento.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Administrador del sistema, operador único (00) |
| CU origen | CU-08 (alta de credenciales en el primer ingreso); CU-09 (autenticar al administrador) |
| Reglas de negocio relevantes | RN-12 (autorización por rol administrador único); RN-13 (resguardo de credenciales y cuenta única) |
| Marco experiencia-de-uso aplicado | experiencia-de-uso_v1.0.md §3.1, §4, §5, §8 |
| US a generar | US a generar en 06 (first-run setup; política de contraseña; creación de cuenta única; autenticación; cierre y vencimiento de sesión; límite de intentos) |
| Tests previstos | Snapshot por estado (vacío first-run, con datos, cargando, error de entrada, error de conflicto, éxito); test de accesibilidad WCAG 2.2 AA (referencia tentativa a 08) |
| Catálogo de diseño aplicado | design-rules-web-generico_v1.0.md; design-rules-blazor-mudblazor_v1.0.md |
| Configuración dirigida por esquema aplicada | N/A (superficie sin parámetros de moderación) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Wireframe inicial de primer ingreso (first-run) y autenticación, con variantes A y B, componentes del catálogo, interacciones, estados mínimos vacío/cargando/con datos/error y los códigos de error de CU-08 y CU-09. |
