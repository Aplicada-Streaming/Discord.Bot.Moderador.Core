# RC-02 — Identidad de los snowflakes almacenados como texto

**Proyecto:** discord-bots-admin
**Documento:** RC-02-identidad-de-snowflakes_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Analista Funcional senior (AG-02)

## 1. Enunciado

Todo identificador de la plataforma (snowflake de servidor, canal, usuario o mensaje) presente en el modelo se conserva como texto y preserva su valor exacto de 64 bits; un Servidor se identifica de forma única por su snowflake.

## 2. Entidades involucradas

Servidor, CanalDeSalida, Exención, Incidente, MensajeAccionado, CanalAfectado.

## 3. Tipo de restricción

Identidad y valor permitido.

## 4. Mecanismo de verificación conceptual

Cada snowflake se valida contra el formato de identificador de la plataforma y se conserva como texto, evitando cualquier conversión a un entero que pueda desbordar; la unicidad del snowflake del servidor se comprueba al registrar un servidor nuevo.

## 5. RN o CU que la justifican

RN-08; CU-10, CU-15.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial del modelo conceptual |
