# Ejemplo 01 — Conexión mínima al canal de eventos que loguea los mensajes entrantes

**Proyecto:** discord-bots-admin
**Documento:** ejemplo-01-basico-conexion-gateway_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11)
**Nivel:** Básico
**Ubicación del código:** `/samples/01-basico-conexion-gateway/`

> Etapa de documentación. La estructura de `/samples/01-basico-conexion-gateway/` que se describe abajo es planificada: el código ejecutable, sus tests y su job de integración continua se materializan en la fase de codificación, posterior al handoff (`SOLUTION-INTAKE §16.1`). Este markdown es la especificación del sample.

## 1. Objetivo del sample

Demostrar el ingreso de mensajes al pipeline de moderación: el sample levanta el adaptador del canal de eventos en tiempo real de la plataforma, recibe los mensajes entrantes, los normaliza a la forma que consume el dominio y los loguea por consola. Al terminar, el desarrollador entiende cómo un mensaje cruza la frontera de infraestructura y queda disponible para el motor de moderación, sin todavía evaluar ninguna regla.

## 2. Nivel

Básico. Es el punto de entrada absoluto y el primer eslabón del flujo de moderación (`../05_arquitectura_tecnica/flujo-ejecucion_v1.0.md`). No evalúa reglas, no toma acciones y no persiste nada: solo recibe, normaliza y loguea. Es la base sobre la que se apoyan el sample 02 (configuración de lo que se evalúa) y el sample 03 (detección y contención). Para que sea reproducible en un entorno limpio y en integración continua, el sample usa una fuente de mensajes simulados que reemplaza la conexión real a la plataforma a través del mismo contrato del adaptador; no requiere un token real.

## 3. Prerequisites

- Runtime y SDK del proyecto, versión mínima declarada en `SOLUTION-INTAKE §17 P.1` y `§17 P.9` (.NET 10). Sin runtime instalado en el sistema: el sample corre sobre el SDK de desarrollo en una estación de trabajo x64.
- Sistema operativo de desarrollo: estación x64 (Windows, Linux o macOS) con el SDK instalado. El target de producción es ARM de 32 bits (`SOLUTION-INTAKE §17 P.9`), pero este sample se ejecuta en la estación de desarrollo.
- Editor con soporte para el lenguaje del proyecto (opcional, para inspeccionar el código).
- No requiere token de la plataforma ni conexión de red: el sample usa la fuente de mensajes simulados incluida.

## 4. Cómo correrlo

1. Clonar el repositorio y entrar a la carpeta del sample: `cd samples/01-basico-conexion-gateway`.
2. Restaurar dependencias con el gestor de paquetes del ecosistema (`SOLUTION-INTAKE §17 P.1`).
3. Ejecutar el sample en modo simulado (fuente de mensajes simulados, sin token) con el comando de ejecución del stack declarado en `SOLUTION-INTAKE §17 P.1`: `dotnet run -- --fuente simulada`.
4. Observar en consola los mensajes entrantes normalizados a medida que la fuente los emite.
5. Comparar la salida con el output esperado de §6 y detener el proceso con Ctrl+C.

## 5. Estructura del código

```
01-basico-conexion-gateway/
├── README.md                         # Resumen del sample y enlace a este markdown
├── src/
│   ├── Program.cs                    # Punto de entrada: compone el adaptador y suscribe el log
│   ├── FuenteMensajesSimulada.cs     # Fuente que emite mensajes simulados por el contrato del adaptador
│   ├── AdaptadorGatewayMinimo.cs     # Suscripción al canal de eventos y normalización del mensaje
│   └── data/mensajes-entrantes.json  # Lote de mensajes simulados (usuario, canal, marca de tiempo, texto)
└── tests/
    └── ConexionGatewayTests.cs       # Verifica que cada mensaje del lote se normaliza y se loguea una vez
```

Los archivos `.cs` del árbol son código del lenguaje del stack declarado en `SOLUTION-INTAKE §17 P.1`. El adaptador respeta el contrato del componente Adaptador del gateway y de la API de la plataforma (capa de Infraestructura, `../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md §3`): recibe eventos del canal en tiempo real y los entrega como mensajes normalizados. La fuente simulada se enchufa por el mismo contrato, de modo que el resto del pipeline no distingue entre la fuente real y la simulada.

## 6. Qué esperar

Salida esperada en consola (un renglón por mensaje entrante normalizado del lote de ejemplo):

```
[conexion] canal de eventos conectado (fuente: simulada, contexto: servidor-demo)
[mensaje] usuario=1001 canal=general      ts=2026-06-20T12:00:00.100Z texto="hola a todos"
[mensaje] usuario=2002 canal=anuncios     ts=2026-06-20T12:00:00.350Z texto="mirá esto"
[mensaje] usuario=2002 canal=memes        ts=2026-06-20T12:00:00.600Z texto="mirá esto"
[mensaje] usuario=2002 canal=off-topic    ts=2026-06-20T12:00:00.850Z texto="mirá esto"
[resumen] mensajes normalizados: 4 | descartados por formato: 0
```

El sample solo loguea: no evalúa la condición de ráfaga ni ejecuta acciones. Que el usuario 2002 aparezca en tres canales distintos es el insumo que el sample 03 evaluará; aquí queda únicamente registrado.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Agregar mensajes al lote | Sumar entradas en `data/mensajes-entrantes.json` | El log refleja los nuevos mensajes y actualiza el contador del resumen |
| Mensaje con formato inválido | Insertar una entrada sin canal o sin usuario | El adaptador lo descarta y lo cuenta en "descartados por formato", sin caer |
| Cambiar la etiqueta de contexto | Ajustar el nombre del contexto del servidor de demostración | El log y el resumen reflejan el nuevo nombre de contexto (firewall multi-contexto, ADR-13) |
| Encadenar con el sample 03 | Usar el mismo lote como entrada del sample de detección de ráfaga | Sirve de puente al sample 03: los mismos mensajes que aquí se loguean, allá disparan la detección |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| [Ingreso de mensajes al pipeline (upstream de CU-01)](../02_especificacion_funcional/casos-de-uso/CU-01-detectar-rafaga-distribuida_v1.0.md) | Caso de uso (paso previo) | Materializa los pasos 1 y 3 del flujo principal de CU-01 (entrega del mensaje por el canal de eventos y su normalización), sin evaluar la regla |
| [Adaptador del gateway y de la API de la plataforma](../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md) | Componente de la vista lógica (Infraestructura) | Ejercita el componente real que recibe eventos del canal en tiempo real y entrega mensajes normalizados |
| [ADR-13 — Dominio firewall multi-contexto](../05_arquitectura_tecnica/adrs/ADR-13-dominio-firewall-multi-contexto_v1.0.md) | Decisión arquitectónica | El sample modela un contexto (servidor) con su propia conexión al canal de eventos |
| [ADR-04 — Separación de capas, dominio independiente](../05_arquitectura_tecnica/adrs/ADR-04-separacion-capas-dominio-independiente_v1.0.md) | Decisión arquitectónica | La fuente simulada se enchufa por el contrato del adaptador, demostrando el dominio aislado de la infraestructura |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial. Especificación del sample de nivel básico: conexión mínima al canal de eventos con fuente de mensajes simulados que normaliza y loguea los mensajes entrantes, como ingreso al pipeline (upstream de CU-01). |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: anclaje de `dotnet run`/`.cs` al stack de `SOLUTION-INTAKE §17 P.1` en su primera aparición. |
