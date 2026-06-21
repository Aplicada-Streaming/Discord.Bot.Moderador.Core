# Wireframes — Registro de servidor y prueba de configuración

**Proyecto:** discord-bots-admin
**Documento:** wireframes-registro-de-servidor-y-prueba_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)
**Variante:** UX/UI

---

## 1. Pantalla y propósito

Superficie de incorporación de un servidor a moderar. El administrador registra el servidor con su identificador y su token de bot, opcionalmente designa un canal de salida para los reportes, y ejecuta la prueba de configuración que verifica credencial, permisos, jerarquía de roles, recepción de eventos y existencia de canales antes de activar. La activación queda condicionada a que la prueba no arroje faltantes bloqueantes, para que el administrador no confíe en una protección que no existe.

## 2. Layout

Se usa el patrón wizard/stepper del catálogo con tres pasos: Credencial, Canal de salida y Prueba y activación. Indicador de pasos horizontal arriba, panel del paso activo en el medio, navegación al pie con contador "Paso X de 3".

```
+-----------+-------------------------------------------------------+
| barra     |  Registrar servidor                                   |
| lateral   |                                                       |
|           |  (1)Credencial --- (2)Canal salida --- (3)Prueba      |
|           |   activo            pendiente          pendiente      |
|           |                                                       |
|           |  +-------------------------------------------------+  |
|           |  | Paso 1 - Credencial                              | |
|           |  | Identificador del servidor                       | |
|           |  | [____________________]  (formato snowflake)      | |
|           |  | Token de bot                                     | |
|           |  | [____________________________] [ojo]            | |
|           |  | El token se guarda cifrado; nunca en texto claro.| |
|           |  +-------------------------------------------------+  |
|           |                                                       |
|           |  [ Anterior (deshab.) ]        [ Siguiente ]  Paso 1/3 |
+-----------+-------------------------------------------------------+
```

Paso 3 — Prueba y activación (panel de resultados):

```
+-------------------------------------------------------+
| Paso 3 - Prueba y activacion          [ Ejecutar      |
|                                          prueba ]      |
|                                                       |
|  Verificaciones                                       |
|  [ok] Credencial valida                               |
|  [ok] Permiso de baneo                                |
|  [ok] Permiso de borrar mensajes                      |
|  [!!] Permiso de gestionar roles  -> faltante         |
|  [ok] Recepcion de eventos                            |
|  [ i] Jerarquia: 1 rol por encima del bot (aviso)     |
|  [!!] Canal de salida: ausente -> designar o advertir |
|                                                       |
|  Resultado: bloqueada por 1 faltante                  |
|  Corregi el permiso de gestionar roles y volve a probar|
+-------------------------------------------------------+
| [ Anterior ]                      [ Activar (deshab.) ]|
+-------------------------------------------------------+
```

Cada verificación es una fila con un ícono de estado (superada, faltante bloqueante o advertencia no bloqueante), su etiqueta y, cuando aplica, la acción de corrección. El botón "Activar" del último paso solo se habilita cuando no hay faltantes bloqueantes.

## 3. Componentes principales

| Componente | Patrón del catálogo | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- | --- |
| Stepper | Wizard/stepper (4.5) | Guiar el alta en tres pasos | Paso actual, completados y pendientes; contador | Anterior deshabilitado en el primer paso; el último paso vira a la acción de activación |
| Campo identificador | Controles de formulario (4.6) | Capturar el snowflake del servidor | Label y placeholder de ejemplo de formato | Valida formato de snowflake como texto al desenfocar |
| Campo token | Controles de formulario (4.6) | Capturar el token de bot | Label, texto oculto con alternancia de visibilidad | Obligatorio; se cifra en reposo; nunca se muestra el token guardado en claro |
| Selector de canal de salida | Controles de formulario (4.6) | Designar el canal de reportes | Canal por su propósito lógico | Opcional en el alta; puede definirse luego |
| Panel de verificaciones | Estados y feedback (5) | Mostrar el resultado de la prueba | Cada verificación con su estado y corrección | Se llena al ejecutar la prueba; distingue bloqueante de advertencia |
| Fila de verificación | Badge de estado (4.8) | Comunicar el resultado de cada chequeo | Etiqueta, estado (ok, faltante, aviso), acción de corrección | Estado por ícono más texto; el faltante enlaza a su corrección |
| Botón Ejecutar prueba | Botones (4.9) | Disparar la prueba contra la plataforma | Verbo de la acción | Queda ocupado y muestra progreso por verificación durante la prueba |
| Botón Activar | Botones (4.9) | Activar el servidor | Verbo de la acción | Deshabilitado mientras haya faltantes bloqueantes |
| Botón Volver a inicio | Botones (4.9, pill auxiliar) | Salir del alta sin activar | — | El servidor queda registrado e inactivo si se guardó |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Avanzar de paso | El administrador activa Siguiente | El stepper marca el paso como completado y muestra el siguiente | El paso actual es válido |
| Validar identificador | El administrador desenfoca el campo de identificador | Acepta si es snowflake con formato esperado; rechaza con el formato esperado si no (SERVIDOR_IDENTIFICADOR_INVALIDO) | Paso 1 activo |
| Rechazo por token vacío | Se intenta avanzar sin token | Mensaje inline de token obligatorio (SERVIDOR_TOKEN_VACIO) | Paso 1 activo |
| Rechazo por duplicado | Se registra un servidor con identificador ya existente | Mensaje de servidor ya registrado con opción de editar el existente (SERVIDOR_YA_REGISTRADO) | Paso 1 activo |
| Guardar sin activar | El administrador sale antes de probar | El servidor queda registrado e inactivo a la espera de la prueba (CU-10 5.C) | Credencial ingresada y cifrada |
| Designar canal de salida | El administrador elige un canal en el paso 2 | El canal de salida lógico queda asociado al servidor (CU-10 5.B) | Paso 2 activo |
| Ejecutar prueba | El administrador activa Ejecutar prueba en el paso 3 | El sistema verifica credencial, permisos, jerarquía, eventos y canales y compone el resultado (CU-12) | Servidor registrado |
| Reemplazar token | El administrador ingresa un token nuevo sobre uno revocado | El sistema cifra y reemplaza el token conservando el resto y vuelve a ofrecer la prueba (CU-10 5.A) | Servidor existente |
| Activar | El administrador activa Activar con la prueba superada | El servidor pasa a activo y queda protegido (RN-16) | Sin faltantes bloqueantes |
| Activar con advertencia | El administrador activa con una jerarquía no bloqueante | El servidor se activa dejando la advertencia visible (CU-12 5.C) | Solo advertencias no bloqueantes |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Formulario de registro recién abierto | Paso 1 con campos vacíos y placeholders de ejemplo |
| Con datos | Credencial ingresada | Campos completos; Siguiente habilitado cuando el paso es válido |
| Cargando | Cifrado del token o prueba contra la plataforma en curso | Botón Ejecutar prueba ocupado; progreso por verificación a medida que se resuelve |
| Con datos (prueba superada) | La prueba no arroja faltantes bloqueantes | Todas las verificaciones en estado superado o advertencia; botón Activar habilitado |
| Error de entrada | Identificador inválido o token vacío | Mensaje inline en el campo correspondiente; el paso no avanza |
| Error de conflicto | Servidor ya registrado | Mensaje con opción de editar el existente |
| Error de capacidad de moderar | Token inválido, permisos faltantes o canal de salida ausente (PRUEBA_TOKEN_INVALIDO, PRUEBA_PERMISOS_FALTANTES, PRUEBA_CANAL_SALIDA_AUSENTE) | Filas de verificación en faltante con su corrección; activación bloqueada; el servidor queda desconectado si el token es inválido |
| Advertencia no bloqueante | Jerarquía de roles por encima del bot sin impedir la operación general | Fila de verificación en aviso; activación habilitada dejando constancia |
| Éxito | Servidor activado | Confirmación y retorno al panel de estado con el servidor activo y conectado |

## 6. Versión móvil o responsive

El stepper horizontal reflujen a indicador vertical o compacto bajo el punto de quiebre del catálogo; el panel del paso ocupa el ancho disponible. Las filas de verificación apilan etiqueta, estado y corrección. La navegación del wizard se mantiene fija al pie. Contenido legible sin scroll horizontal a 320px.

## 7. Notas de implementación

- Accesibilidad: el stepper expone el paso actual y los completados a tecnologías asistivas; cada verificación combina ícono y texto; los faltantes enlazan a su corrección con foco manejable; el campo de token con alternancia de visibilidad rotulada; foco visible en todos los controles; navegación por teclado completa en el wizard.
- El estado de cada verificación nunca depende solo del color (superada, faltante, advertencia con ícono y texto).
- Performance percibida: la prueba cruza a la plataforma externa; se muestra progreso por verificación y el botón queda ocupado para prevenir doble disparo. Las acciones de moderación reales no se simulan como exitosas hasta confirmar.
- Seguridad de presentación: el token nunca se muestra en claro una vez guardado; el cifrado es de 05, aquí solo se representa el aviso de que se guarda cifrado.
- Internacionalización: las etiquetas de verificación y los mensajes de corrección toleran expansión; el formato de snowflake se ilustra como texto.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Administrador del sistema, operador único (00) |
| CU origen | CU-10 (registrar un servidor con su credencial); CU-12 (probar la configuración antes de activar) |
| Reglas de negocio relevantes | RN-01 (jerarquía de roles del bot); RN-08 (identidad de snowflakes); RN-12 (autorización por rol administrador único); RN-14 (cifrado del token en reposo); RN-16 (activación condicionada a la prueba) |
| Marco experiencia-de-uso aplicado | experiencia-de-uso_v1.0.md §3.2, §4, §7, §8 |
| US a generar | US a generar en 06 (registro de servidor; cifrado del token; designación de canal de salida; prueba de configuración; advertencia de permisos y jerarquía; bloqueo de activación) |
| Tests previstos | Snapshot por estado (vacío, con datos, cargando, prueba superada, faltante bloqueante, advertencia, error de conflicto); test de accesibilidad WCAG 2.2 AA (referencia tentativa a 08) |
| Catálogo de diseño aplicado | design-rules-web-generico_v1.0.md; design-rules-blazor-mudblazor_v1.0.md |
| Configuración dirigida por esquema aplicada | N/A en el alta de credencial; los umbrales y ventanas configurables viven en la superficie de configuración de moderación |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Wireframe inicial de registro de servidor y prueba de configuración con wizard de tres pasos, panel de verificaciones que distingue faltante bloqueante de advertencia, activación condicionada a la prueba, estados mínimos vacío/cargando/con datos/error y los códigos de CU-10 y CU-12. |
