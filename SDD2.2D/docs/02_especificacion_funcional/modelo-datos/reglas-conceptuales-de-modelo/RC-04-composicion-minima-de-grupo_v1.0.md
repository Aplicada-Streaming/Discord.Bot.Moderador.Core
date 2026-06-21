# RC-04 — Composición mínima y modo de coincidencia de un grupo

**Proyecto:** discord-bots-admin
**Documento:** RC-04-composicion-minima-de-grupo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Todo GrupoDeReglas tiene al menos una Regla asociada y un modo de coincidencia definido (todas, alguna, o al menos N); no existe un grupo vacío ni un grupo sin modo de coincidencia.

## 2. Entidades involucradas

GrupoDeReglas, GrupoRegla, Regla.

## 3. Tipo de restricción

Cardinalidad y valor permitido.

## 4. Mecanismo de verificación conceptual

Al guardar un grupo se verifica que tenga al menos una asociación GrupoRegla y un modo de coincidencia dentro del conjunto cerrado; un grupo que quede sin reglas tras una eliminación se bloquea o se marca como no evaluable.

## 5. RN o CU que la justifican

RN-15; CU-11.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
