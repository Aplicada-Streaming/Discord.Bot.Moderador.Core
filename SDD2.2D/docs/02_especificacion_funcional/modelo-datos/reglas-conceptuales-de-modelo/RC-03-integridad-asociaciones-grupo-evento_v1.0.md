# RC-03 — Integridad de las asociaciones GrupoRegla y EventoGrupo

**Proyecto:** discord-bots-admin
**Documento:** RC-03-integridad-asociaciones-grupo-evento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Cada asociación GrupoRegla referencia un GrupoDeReglas y una Regla existentes, y cada asociación EventoGrupo referencia un Evento y un GrupoDeReglas existentes; no se admiten asociaciones con un extremo inexistente ni asociaciones duplicadas del mismo par.

## 2. Entidades involucradas

GrupoRegla, GrupoDeReglas, Regla, EventoGrupo, Evento.

## 3. Tipo de restricción

Referencial y cardinalidad (unicidad del par).

## 4. Mecanismo de verificación conceptual

Al crear una asociación se verifica que ambos extremos existan y que el par no esté ya registrado; al eliminar un extremo se eliminan o bloquean las asociaciones que lo referencian para no dejar vínculos colgantes ni duplicados.

## 5. RN o CU que la justifican

RN-15; CU-11.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
