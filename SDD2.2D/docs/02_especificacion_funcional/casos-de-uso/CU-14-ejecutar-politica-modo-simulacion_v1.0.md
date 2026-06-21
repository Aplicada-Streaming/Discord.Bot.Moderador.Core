# CU-14 — Ejecutar una política en modo simulación registrando lo que haría sin ejecutarlo

**Proyecto:** discord-bots-admin
**Documento:** CU-14-ejecutar-politica-modo-simulacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Propósito

Permitir que una política se evalúe normalmente pero registre lo que habría hecho sin ejecutar la acción real sobre los usuarios, para que el administrador calibre la regla con datos del propio servidor antes de promoverla a ejecución, reduciendo los falsos positivos por diseño.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Servicio de moderación | Primario (sistema) | Evalúa la política y registra la acción que se habría ejecutado, sin ejecutarla |
| Administrador del sistema | Secundario | Activa el modo simulación, revisa los registros simulados y decide la promoción a ejecución |

## 3. Precondiciones

- Existe un evento o política con su modo configurado en simulación (RN-09).
- El servidor está activo y recibiendo mensajes.
- Se cumplen las condiciones de la política para un mensaje o usuario evaluado.

## 4. Flujo principal

1. El servicio evalúa una política cuyas condiciones se cumplen para un mensaje o usuario.
2. El servicio comprueba que la política está en modo simulación (RN-09).
3. El servicio toma la copia de los mensajes involucrados y la lista de canales afectados, igual que en ejecución real.
4. El servicio registra un incidente marcado como simulación, con la acción que se habría ejecutado y los sujetos que habría alcanzado.
5. El servicio no ejecuta ninguna acción real sobre los usuarios ni sus mensajes.
6. El servicio puede reportar el incidente simulado al canal de salida etiquetado como simulación (CU-05).

## 5. Flujos alternativos

- 5.A Promoción a ejecución real. Disparador: el administrador decide que la regla ya está calibrada. Acción: cambia el modo de la política a ejecución real (vía CU-11). Punto de retorno: a partir de ese cambio las coincidencias se procesan en ejecución real (CU-02, CU-03, CU-04).
- 5.B Coincidencia simultánea con política en ejecución real. Disparador: sobre el mismo mensaje, una política simulada y otra real coinciden. Acción: la simulada solo registra; la real ejecuta, según prioridad y bandera continuar (RN-04). Punto de retorno: cada política sigue su modo.
- 5.C Sin canal de salida. Disparador: no hay canal de salida designado para el reporte simulado. Acción: el incidente simulado igual se registra para revisión desde el panel. Punto de retorno: retorna al paso 4 conservando el registro.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| SIMULACION_REGISTRO_FALLIDO | No se pudo registrar el incidente simulado | Reintenta el registro; si persiste, deja constancia en el journal del servicio para no perder la observación |
| SIMULACION_MODO_INCONSISTENTE | El modo de la política no está definido de forma inequívoca como simulación o ejecución (RN-09) | Aplica el modo seguro por defecto (simulación) y señala la inconsistencia al administrador |
| SIMULACION_COPIA_NO_DISPONIBLE | No se pudo tomar la copia de los mensajes para el registro simulado | Registra el incidente simulado con los metadatos disponibles e indica que la copia no está completa |

## 7. Postcondiciones

- En caso de éxito: existe un incidente simulado con la acción que se habría ejecutado; ningún usuario ni mensaje real fue afectado.
- En caso de fallo: la observación simulada puede quedar incompleta; ningún usuario real es afectado en ningún caso.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una política en modo simulación con acción de baneo | Un usuario cumple las condiciones de la política | El servicio registra un incidente simulado con el baneo que se habría ejecutado y no banea al usuario |
| CA-02 | Una política en modo simulación con canal de salida designado | Se registra un incidente simulado | El servicio publica el reporte etiquetado como simulación en el canal de salida |
| CA-03 | Una política recién promovida a ejecución real | Un usuario cumple las condiciones después de la promoción | El servicio ejecuta la acción real sobre el usuario en lugar de solo registrarla |
| CA-04 | Una política cuyo modo quedó indefinido | Se cumplen sus condiciones | El servicio aplica el modo seguro de simulación con código SIMULACION_MODO_INCONSISTENTE y no ejecuta acción real |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 |
| Reglas de negocio aplicables | RN-04, RN-09 |
| Historias de usuario a generar | US a generar en 06 (modo simulación por política; reporte simulado; promoción a ejecución real) |
| Componentes esperados | Motor de evaluación con conmutador de modo; registro de incidentes simulados (referencia tentativa a 05) |
| Tests previstos | Pruebas de no ejecución en simulación; pruebas de registro simulado; pruebas de promoción a ejecución (referencia tentativa a 08) |

## 10. Notas y supuestos

- El modo simulación no altera la evaluación de las condiciones; solo suprime la ejecución de la acción real.
- Por defecto, un evento nuevo queda en modo simulación hasta que el administrador lo promueva, según RN-09.
- El valor de negocio es calibrar reglas con datos reales sin consecuencias para los miembros, bajando los falsos positivos por diseño.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de NB-07 y del riesgo R-01 del intake |
