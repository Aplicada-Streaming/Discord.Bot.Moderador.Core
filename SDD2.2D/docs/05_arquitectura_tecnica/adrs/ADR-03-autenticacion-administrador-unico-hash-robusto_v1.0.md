# ADR-03 — Autenticación de administrador único con hash robusto

**Proyecto:** discord-bots-admin
**Documento:** ADR-03-autenticacion-administrador-unico-hash-robusto_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Seguridad

## 1. Contexto

El panel de administración debe estar protegido por autenticación. El sistema tiene un único rol y una única cuenta administradora, cuyas credenciales se crean en el primer ingreso (first-run setup) y son las únicas autorizadas a registrar servidores, configurar moderación, revisar incidentes y desbanear (`SOLUTION-INTAKE §17 P.5`). La contraseña nunca debe almacenarse en texto claro. No hay proveedor de identidad externo; la autenticación es local. Lo motivan NB-05, CU-08, CU-09, RN-12, RN-13 y RC-06.

## 2. Decisión

Se adopta autenticación local con un único rol administrador y una única cuenta, creada en el primer ingreso. La contraseña se almacena con un hash robusto en formato PHC de la familia de derivación de clave con costo (Argon2 o PBKDF2). La autorización aplica un único rol que gobierna todas las operaciones administrativas. La elección puntual entre Argon2 y PBKDF2 queda abierta a Sprint 0 (`SOLUTION-INTAKE §17 P.11`).

## 3. Estado

Aceptado el 2026-06-20. La estrategia es decisión cerrada; la familia de hash exacta es la única dimensión abierta a Sprint 0.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Hash robusto PHC local, administrador único (elegida) | Sin dependencia de terceros; resistente a fuerza bruta por costo de derivación; formato PHC autodescriptivo (parámetros embebidos) | Requiere elegir parámetros de costo apropiados al hardware de 32 bits |
| Proveedor de identidad externo (OAuth/OIDC) | Delegación de la gestión de credenciales | Dependencia de un tercero; contradice el auto-hospedaje sin terceros (`SOLUTION-INTAKE §17 P.5`) |
| Hash de propósito general sin costo (familia SHA simple) | Cómputo barato | Vulnerable a fuerza bruta y a tablas precomputadas; no apto para contraseñas |
| Credencial en archivo de configuración sin hash | Trivial | Texto claro; viola RN-13 y RC-06 |

## 5. Consecuencias positivas

1. La contraseña nunca se almacena en texto claro (RN-13, RC-06).
2. El costo de derivación encarece la fuerza bruta, apropiado para una sola cuenta.
3. El formato PHC embebe los parámetros, lo que permite migrar parámetros de costo sin romper verificaciones existentes.
4. La autorización por rol único centraliza el control de acceso de todas las CU administrativas (RN-12).

## 6. Consecuencias negativas y trade-offs

1. Un costo de derivación alto consume CPU del dispositivo de 32 bits en cada autenticación: aceptado por baja frecuencia de login; los parámetros se calibran al hardware.
2. No hay recuperación de contraseña delegada a un tercero: aceptado; un operador único restablece la credencial por procedimiento local.
3. La elección entre Argon2 y PBKDF2 queda abierta: trade-off documentado; ambas son válidas en formato PHC y la decisión no altera el resto de la arquitectura.

## 7. Implementación

El Servicio de autenticación (Aplicación) gestiona el primer ingreso (RC-06 impone a lo sumo una cuenta), la verificación de hash y la sesión. La unicidad de la cuenta se garantiza por restricción en el modelo (`modelo-datos-logico_v1.0.md`). El rol administrador se exige en cada operación del panel (RN-12).

## 8. Métricas de validación

- Cero contraseñas en texto claro en la base (verificación de esquema y de datos).
- Tiempo de verificación de hash acotado en el hardware de referencia, sin degradar la experiencia de login.
- Pruebas unitarias de hashing/verificación y de imposibilidad de crear una segunda cuenta.

## 9. Referencias

- NB-05.
- CU-08, CU-09; consumida por CU-06, CU-07, CU-10, CU-11, CU-12, CU-15 (autorización).
- RN-12, RN-13.
- RC-06.
- `SOLUTION-INTAKE §17 P.5, P.11`.
- ADR-07 (cifrado de tokens, otra decisión de seguridad), ADR-06 (compliance).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
