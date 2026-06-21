# RC-11 — Tope de la ventana de borrado de una acción

**Proyecto:** discord-bots-admin
**Documento:** RC-11-tope-ventana-de-borrado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

La ventana de borrado de una Acción que remueve mensajes está siempre dentro del rango de 0 a 7 días; no se admite una ventana de borrado por encima del tope de la plataforma.

## 2. Entidades involucradas

Acción.

## 3. Tipo de restricción

Valor permitido.

## 4. Mecanismo de verificación conceptual

Al configurar o ejecutar una acción con borrado se verifica que la ventana caiga en el rango de 0 a 7 días; un valor por encima se acota al tope de 7 días antes de ejecutar y el ajuste se registra.

## 5. RN o CU que la justifican

RN-02; CU-03, CU-11.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
