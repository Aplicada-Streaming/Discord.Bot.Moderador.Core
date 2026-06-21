# RC-09 — Validez del criterio de una regla según su clase

**Proyecto:** discord-bots-admin
**Documento:** RC-09-validez-del-criterio-de-regla_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

El criterio de una Regla debe ser coherente con su clase: una regla de contenido tiene una expresión regular válida o un conjunto no vacío de palabras o frases clave; una regla de conducta tiene un umbral y una ventana dentro de los límites de sus descriptores.

## 2. Entidades involucradas

Regla.

## 3. Tipo de restricción

Valor permitido (coherencia entre clase y criterio).

## 4. Mecanismo de verificación conceptual

Al guardar una regla se verifica que el criterio corresponde a la clase declarada y que es válido: la expresión regular compila o las palabras clave no están vacías para contenido, y el umbral y la ventana caen dentro de sus límites para conducta.

## 5. RN o CU que la justifican

RN-03, RN-10; CU-04, CU-11.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
