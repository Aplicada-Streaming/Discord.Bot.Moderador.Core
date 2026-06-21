# RN-11 — Integridad de la evidencia del incidente

**Proyecto:** discord-bots-admin
**Documento:** RN-11-integridad-del-incidente_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado de la regla

Todo incidente registrado conserva la copia de los mensajes que dispararon la acción y la lista de canales afectados, tomada antes de cualquier remoción, y esa copia se mantiene disponible para revisión aunque los mensajes ya hayan sido removidos del servidor.

## 2. Justificación

La moderación automática debe ser auditable y reversible para que el negocio confíe en ella; la copia de mensajes es la única evidencia disponible una vez removidos. Conservarla permite revisar falsos positivos, justificar acciones y medir la calidad de la moderación.

## 3. Ámbito de aplicación

Se evalúa al registrar un incidente por una política disparada, en ejecución real o simulación, y al consultarlo o reportarlo posteriormente.

## 4. Consecuencia si se viola

Si un incidente no conservara la copia de mensajes y los canales afectados, no habría evidencia para revisar un falso positivo tras la remoción. La regla obliga a tomar y conservar esa copia; un incidente sin ella se marca como evidencia no disponible y se señala para revisión.

## 5. CU afectados

CU-05, CU-06.

## 6. Pruebas que la verifican

- Prueba de que un incidente conserva la copia de mensajes y los canales afectados tras la remoción (referencia a 08).
- Prueba de consulta de la evidencia de un incidente cuyos mensajes ya no están en el servidor (referencia a 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial derivada de la trazabilidad de incidentes del intake |
