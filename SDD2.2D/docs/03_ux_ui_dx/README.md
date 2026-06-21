# 03 — UX / UI / DX

**Proyecto:** discord-bots-admin
**Sección:** 03_ux_ui_dx
**Variante aplicada:** UX/UI (tipo de proyecto web-monolith; el administrador recorre pantallas en navegador)
**Estado de la sección:** Propuesto
**Fecha:** 2026-06-20
**Autor:** UX/UI Designer + Frontend Lead (AG-03)

---

## 1. Propósito

Índice navegable de los artefactos de experiencia, layout y comportamiento del panel de administración de la moderación. Define cómo se siente el producto para su único operador, sin invadir el qué de 02 ni el cómo de 05. Caso degenerado de layout aplanado: los artefactos viven en `docs/03_ux_ui_dx/`, no bajo `proyectos/<kebab>/`.

## 2. Catálogo de diseño aplicado

Heredan tokens y patrones del catálogo de `devs/references/design/`, referenciados por nombre, sin tokens ad hoc:

- `design-rules-web-generico_v1.0.md` (base web genérico).
- `design-rules-blazor-mudblazor_v1.0.md` (especialización del stack declarado).
- `design-rules-config-esquema_v1.0.md` (extensión de capacidad, por las superficies de configuración).

## 3. Artefactos vigentes

| Artefacto | Propósito (una línea) | Variante | Estado |
| --- | --- | --- | --- |
| [experiencia-de-uso_v1.0.md](experiencia-de-uso_v1.0.md) | Marco de experiencia: audiencia, principios, flujos, estados, accesibilidad, i18n, performance y errores | UX/UI | Propuesto |
| [wireframes-primer-ingreso-y-autenticacion_v1.0.md](wireframes-primer-ingreso-y-autenticacion_v1.0.md) | Alta de la cuenta única en el primer ingreso y autenticación | UX/UI | Propuesto |
| [wireframes-panel-de-estado_v1.0.md](wireframes-panel-de-estado_v1.0.md) | Servidores registrados con estado de activación y de conexión | UX/UI | Propuesto |
| [wireframes-registro-de-servidor-y-prueba_v1.0.md](wireframes-registro-de-servidor-y-prueba_v1.0.md) | Registro de servidor con token y prueba de configuración antes de activar | UX/UI | Propuesto |
| [wireframes-configuracion-de-moderacion_v1.0.md](wireframes-configuracion-de-moderacion_v1.0.md) | Reglas, grupos, eventos, acciones y exenciones dirigidos por descriptores, con simulación y previsualización | UX/UI | Propuesto |
| [wireframes-revision-de-incidentes-y-desbaneo_v1.0.md](wireframes-revision-de-incidentes-y-desbaneo_v1.0.md) | Revisión de incidentes con evidencia y reversión de baneos | UX/UI | Propuesto |
| [glosario-ux_v1.0.md](glosario-ux_v1.0.md) | Terminología de presentación de la sección, sin duplicar el dominio de 02 | UX/UI | Propuesto |

## 4. Mapa wireframe → CU origen

| Wireframe | CU que lo anclan |
| --- | --- |
| Primer ingreso y autenticación | CU-08, CU-09 |
| Panel de estado | CU-13 |
| Registro de servidor y prueba | CU-10, CU-12 |
| Configuración de moderación | CU-11, CU-14, CU-15, CU-16 |
| Revisión de incidentes y desbaneo | CU-05, CU-06, CU-07 |

## 5. Trazabilidad de la sección

- Upstream: persona objetivo del administrador (00, `vision-producto_v1.0.md` §2); CU con interacción humana y RN que afectan la presentación (02, `especificacion-funcional_v1.0.md`).
- Downstream: US a generar en 06; tests de snapshot y de accesibilidad previstos en 08.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Índice inicial de la sección con el marco de experiencia, cinco wireframes que cubren las superficies reales del producto, el glosario UX y el catálogo de diseño aplicado. |
