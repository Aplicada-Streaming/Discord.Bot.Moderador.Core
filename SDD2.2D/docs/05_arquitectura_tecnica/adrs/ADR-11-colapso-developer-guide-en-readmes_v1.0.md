# ADR-11 — Colapso de la developer guide en READMEs (sin portal de developers)

**Proyecto:** discord-bots-admin
**Documento:** ADR-11-colapso-developer-guide-en-readmes_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Estilo
**Bandera:** tiene_portal_developers = false

## 1. Contexto

La cadena SDD prevé una categoría 10 (developer guide) con guías para integradores y, en proyectos con superficie pública, un portal de developers. Este sistema es un web-monolith de operador único que integra con la plataforma de mensajería como cliente y no expone ninguna API a terceros (`SOLUTION-INTAKE §17 P.3`); no es redistribuible (§17 identidad, `redistribuible = false`) ni publica un feed de paquetes (§17 P.7). No hay terceros que consuman una superficie pública, por lo que un portal de developers no tiene audiencia. Para evitar una omisión silenciosa, se registra la decisión de colapsar la developer guide en los READMEs del repositorio.

## 2. Decisión

Se colapsa la categoría 10 (developer guide) en los READMEs del repositorio y de las secciones, sin portal de developers (`tiene_portal_developers = false`). La documentación para el implementador (build, publicación, instalación como servicio, configuración por descriptores) vive en los READMEs de `/`, `/scripts/servicio/` y en los READMEs de las categorías de documentación, en lugar de un sitio o portal aparte.

## 3. Estado

Aceptado el 2026-06-20. Coherente con la ausencia de superficie pública (§17 P.3) y de redistribuibles (§17 P.7).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Colapsar 10 en READMEs, sin portal (elegida) | Proporcional a un operador único sin terceros; cero mantenimiento de un sitio; documentación junto al código | Menos vistoso que un portal; requiere disciplina de READMEs |
| Portal de developers completo | Experiencia rica para integradores | No hay integradores externos; esfuerzo desproporcionado; sin audiencia |
| No documentar para el desarrollador | Mínimo esfuerzo | Pierde la guía de build/instalación; perjudica el mantenimiento futuro |

## 5. Consecuencias positivas

1. La documentación de construcción, publicación e instalación queda junto al código, fácil de mantener.
2. No se invierte esfuerzo en un portal sin audiencia.
3. La decisión es explícita y auditable; no es un olvido de la categoría 10.

## 6. Consecuencias negativas y trade-offs

1. No hay un portal navegable para integradores: aceptado; no existen integradores externos.
2. La calidad de la guía depende de mantener los READMEs al día: aceptado; es responsabilidad del implementador único.

## 7. Implementación

La categoría 10 se materializa como READMEs: el README raíz del repositorio (build, publicación self-contained, instalación del servicio), el README de `/scripts/servicio/` (instalador y unidad de systemd) y los READMEs de cada categoría de documentación. No se genera un sitio ni un portal. La superficie de configuración para el operador se documenta a partir de los descriptores (ADR-12).

## 8. Métricas de validación

- Existen READMEs de build/publicación/instalación en el repositorio.
- No existe artefacto de portal de developers.
- Un implementador puede construir, publicar e instalar el servicio siguiendo solo los READMEs.

## 9. Referencias

- `SOLUTION-INTAKE §17 identidad (redistribuible = false), P.3, P.7`; `§16` (estructura de repositorio).
- ADR-05 (despliegue), ADR-12 (configuración por descriptores).
- README de la sección 05 (`README.md`).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
