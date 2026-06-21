# RC-01 — Integridad referencial de las entidades dependientes

**Proyecto:** discord-bots-admin
**Documento:** RC-01-integridad-referencial-dependientes_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Toda entidad dependiente de un Servidor (CanalDeSalida, Exención, Regla, GrupoDeReglas, Evento, Incidente) debe referenciar un Servidor existente, y toda entidad dependiente de un Incidente (MensajeAccionado, CanalAfectado) debe referenciar un Incidente existente; no puede existir una entidad dependiente huérfana.

## 2. Entidades involucradas

Servidor, CanalDeSalida, Exención, Regla, GrupoDeReglas, Evento, Incidente, MensajeAccionado, CanalAfectado.

## 3. Tipo de restricción

Referencial.

## 4. Mecanismo de verificación conceptual

Cada instancia dependiente conserva la referencia a su entidad contenedora; al crear una dependiente se verifica que la contenedora exista, y al eliminar una contenedora se resuelve el destino de sus dependientes (eliminación en cascada o bloqueo) sin dejar referencias colgantes.

## 5. RN o CU que la justifican

RN-11; CU-05, CU-06, CU-10, CU-15.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
