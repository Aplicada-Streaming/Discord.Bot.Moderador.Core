# ADR-02 — Persistencia en base relacional embebida en modo WAL

**Proyecto:** discord-bots-admin
**Documento:** ADR-02-persistencia-relacional-embebida-wal_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-20
**Autor:** Arquitecto de Software Senior (AG-05)
**Categoría:** Persistencia

## 1. Contexto

El sistema necesita persistir la configuración de moderación (servidores, canales de salida, exenciones, reglas, grupos, eventos, acciones) y la auditoría de incidentes (con copia de mensajes y canales afectados) en un dispositivo auto-hospedado, sin servidor de base externo ni dependencia de terceros (`SOLUTION-INTAKE §10, §17 P.4`). Hay concurrencia entre el bot (escrituras de auditoría por incidente) y el panel (escrituras de configuración, baja frecuencia) sobre la misma base. Se requiere integridad referencial estricta entre entidades dependientes (RC-01, RC-03), restricciones de unicidad y check (RC-02..RC-11), y un esquema reconstruible y auditable. Lo motivan CU-05, CU-06, CU-10, CU-11, CU-15 y las reglas conceptuales del modelo de 02.

## 2. Decisión

Se adopta una base de datos relacional embebida en archivo como almacenamiento principal, accedida mediante un ORM con migraciones versionadas, operando en modo de registro de escritura anticipada (WAL) para tolerar escrituras concurrentes del bot y del panel sin bloquear lectores. El detalle de tablas, tipos físicos, índices, restricciones y migración inicial vive en `modelo-datos-logico_v1.0.md`.

## 3. Estado

Aceptado el 2026-06-20. Coherente con la decisión cerrada de stack de `SOLUTION-INTAKE §17 P.4, P.11`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Base relacional embebida en archivo, modo WAL (elegida) | Sin servidor externo; integridad referencial y restricciones declarativas; migraciones versionadas; WAL tolera concurrencia bot/panel; archivo único fácil de respaldar | El archivo es un punto único; el modo WAL agrega archivos auxiliares a respaldar de forma consistente |
| Almacenamiento en archivos planos o documentos | Flexibilidad de esquema | No garantiza integridad referencial; complica las restricciones de unicidad y check del modelo; reimplementa consistencia en aplicación |
| Servidor de base relacional externo | Concurrencia y escala robustas | Dependencia de un servicio adicional en el dispositivo; viola la simplicidad de despliegue y el auto-hospedaje sin terceros |
| Estado solo en memoria con volcado periódico | Latencia mínima | Pérdida de configuración e incidentes ante reinicio; inaceptable para auditoría (RN-11) |

## 5. Consecuencias positivas

1. La integridad referencial del modelo (RC-01, RC-03) se materializa con claves foráneas a nivel de motor.
2. Las restricciones de unicidad, check y rango (RC-02, RC-04, RC-06, RC-09, RC-11) se declaran en el esquema.
3. El modo WAL permite que el panel lea mientras el bot registra incidentes, sin bloqueo.
4. Las migraciones versionadas hacen el esquema reconstruible y auditable.

## 6. Consecuencias negativas y trade-offs

1. El archivo de base es un punto único de fallo: aceptado para un dispositivo auto-hospedado de un operador; mitigable con respaldo del archivo y sus archivos WAL de forma consistente.
2. El modo WAL exige cuidar el respaldo de los archivos auxiliares: aceptado a cambio de la concurrencia sin bloqueo.
3. Acopla el dominio al modelo relacional a través del repositorio: aceptado; el dominio permanece independiente del ORM gracias a la separación de capas (ADR-04).

## 7. Implementación

El acceso vive en la capa de Infraestructura mediante el componente Persistencia. Los snowflakes se almacenan como texto (RN-08, RC-02). El token se persiste cifrado (ADR-07). La migración inicial se referencia por identificador abstracto en `modelo-datos-logico_v1.0.md §5`. El modo WAL se habilita al iniciar la conexión.

## 8. Métricas de validación

- Cero violaciones de integridad referencial en las pruebas de integración de 08.
- Limpieza efectiva de la ráfaga ≥ 98 % de mensajes dentro de los 10 s (depende de que la copia de mensajes y el incidente se confirmen sin bloqueo de lectores).
- Reconstrucción del esquema desde la migración inicial en un entorno limpio.

## 9. Referencias

- CU-05, CU-06, CU-10, CU-11, CU-15.
- RN-08, RN-11.
- RC-01, RC-02, RC-03, RC-04, RC-05, RC-06, RC-09, RC-11.
- `modelo-datos-logico_v1.0.md`.
- `SOLUTION-INTAKE §10, §17 P.4, P.11`.
- ADR-04 (separación de capas), ADR-07 (cifrado de token), ADR-09 (estado en memoria).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Decisión inicial. Para una ADR aceptada, la única edición permitida es el cambio de estado a `Superado por ADR-YY`. |
