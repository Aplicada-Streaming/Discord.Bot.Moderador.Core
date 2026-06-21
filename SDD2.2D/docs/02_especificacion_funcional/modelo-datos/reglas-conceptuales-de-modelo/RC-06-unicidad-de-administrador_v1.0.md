# RC-06 — Unicidad y resguardo de la cuenta administrador

**Proyecto:** discord-bots-admin
**Documento:** RC-06-unicidad-de-administrador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Existe a lo sumo una instancia de Administrador en el modelo, con identificador de cuenta único, y su contraseña se representa solo por un resguardo no reversible, nunca como texto claro.

## 2. Entidades involucradas

Administrador.

## 3. Tipo de restricción

Identidad y valor permitido.

## 4. Mecanismo de verificación conceptual

Al crear la cuenta en el primer ingreso se verifica que no exista otra; el atributo de contraseña solo admite un verificador derivado por hash, y no se conserva ninguna representación reversible de la contraseña.

## 5. RN o CU que la justifican

RN-12, RN-13; CU-08, CU-09.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
