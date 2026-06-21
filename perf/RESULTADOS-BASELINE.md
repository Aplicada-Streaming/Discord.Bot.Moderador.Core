# Resultados baseline del benchmark de NFR del motor de moderación

Harness de performance que mide los NFR de procesamiento del motor de moderación
(`MotorDeModeracion`) definidos en `SOLUTION-INTAKE §17 P.10`, recogidos en
`SDD2.2D/docs/05_arquitectura_tecnica/arquitectura-solucion_v1.0.md` §8 (tabla NFR) y trazados en
`SDD2.2D/docs/08_calidad_y_pruebas/matriz-cobertura-pruebas_v1.0.md` §3 (TC-55 latencia, TC-56
throughput). Cierra el "banco de pruebas de carga" que la matriz dejó como "a confirmar por
benchmark".

NFR medidos:

| NFR | Objetivo | TC (matriz §3) |
| --- | --- | --- |
| Latencia de procesamiento por mensaje | **p95 < 200 ms** | TC-55 |
| Throughput sostenido | **>= 50 mensajes/s** (en la Raspberry Pi 4) | TC-56 |
| Memoria por mensaje (proxy del NFR de memoria) | medida observada (sin SLA numérico de "por mensaje") | — |

## Cómo correr

El proyecto perf está **fuera de `DiscordModeradorBot.slnx`** (igual que el e2e): el `dotnet test`
de la solución no lo arrastra. Se corre explícitamente por path, siempre en `-c Release`
(BenchmarkDotNet exige optimizaciones; en Debug avisa y rehúsa medir).

```bash
# Microbenchmarks de BenchmarkDotNet (latencia con p95 + throughput + memoria), corrida ACOTADA:
dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks

# Corrida COMPLETA (más iteraciones -> p95 más preciso, más lenta):
dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- full

# Loop cronometrado que imprime mensajes/s directamente (verificación del NFR de throughput):
dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- throughput
```

La corrida por defecto es **acotada** (Job con 120 iteraciones, 10 de warmup) para producir números
en tiempo razonable (~12 s en total en este equipo). La corrida `full` (300 iteraciones, 2 launches)
reduce el margen de error del p95 a costa de tiempo y es la recomendada para números de aceptación
definitivos.

## Qué incluye y qué excluye la medición

**Se mide** el hot path del motor de evaluación con dependencias **en memoria / dobles**:

- `MotorDeModeracion.ProcesarAsync` completo (etapas 1 a 9 del flujo-ejecucion): descarte de
  exentos, evaluación de reglas de contenido, actualización del estado de conducta, evaluación de
  políticas por prioridad, y el camino de acciones (reportar + banear) cuando dispara.
- Estado de conducta y antirrebote **en memoria** (las clases reales del producto).
- Evaluadores reales (ráfaga distribuida, regla de contenido por regex con su matchTimeout,
  exenciones).
- Reloj fijo (`IReloj`) para una medición determinista.

**No se mide** (deliberadamente excluido, para aislar el motor y no la E/S):

- La base de datos SQLite: el repositorio de incidentes es un doble **no-op** (la etapa 9 no toca
  disco). El repositorio de servidores y de exenciones devuelven datos fijos en memoria.
- La red / Discord.Net: el adaptador del gateway es un doble **no-op** que devuelve
  `ResultadoAccion.Ejecutada` sin red, sin token y **sin logging** (a diferencia del
  `AdaptadorGatewaySimulado` del producto, que loguea cada acción; ese logging no es parte del
  costo del motor de decisión).
- El logging: se usa `NullLogger`.
- El panel Blazor y el host web.

Esta delimitación es coherente con el mecanismo de medición de la arquitectura (§8: "instrumentación
del pipeline, marca de tiempo de entrada a salida de decisión") y con ADR-09 (estado en memoria): el
NFR de latencia/throughput aplica al **motor de evaluación**, no a la latencia de la plataforma
Discord ni a la E/S de persistencia.

## Números obtenidos en ESTE entorno (x64 baseline)

> Entorno de la corrida (reportado por BenchmarkDotNet):
> BenchmarkDotNet v0.15.8, Windows (VirtualBox), 12th Gen Intel Core i9-12900K, .NET SDK 10.0.301,
> runtime .NET 10.0.9 X64 RyuJIT. **Este NO es el hardware de aceptación**: es el baseline x64 (ver
> sección de la Raspberry). La VM/virtualización puede afectar la medición; BenchmarkDotNet lo
> advierte.

### Latencia por mensaje (Job acotado: 120 iteraciones)

| Caso | Media | Mediana | **p95** | Asignado/op |
| --- | ---: | ---: | ---: | ---: |
| No coincide (camino común) | 8.66 us | 8.55 us | **9.89 us** | 896 B |
| Ráfaga que dispara (reportar + banear) | 21.66 us | 20.55 us | **27.96 us** | 3752 B |

`us` = microsegundos. p95 del camino común = **9.89 us ≈ 0.0099 ms**; p95 del camino que dispara =
**27.96 us ≈ 0.028 ms**.

### Throughput (mensajes/s)

Dos mediciones independientes y coherentes:

- **Microbenchmark** (`Throughput (lote de mensajes benignos)`, lote de 1000 msg/invocación):
  **1.002 us por mensaje** -> **≈ 998.000 mensajes/s** (1e9 / 1002 ns).
- **Loop cronometrado** (`-- throughput`, 50.000 msg/ronda, 5 rondas medidas):
  promedio **≈ 485.800 mensajes/s**, mejor **≈ 534.600 mensajes/s**.

Las dos cifras difieren porque el loop usa un usuario y un mensaje distintos por cada mensaje (más
entradas en los diccionarios de estado, más asignaciones de snowflake), mientras que el
microbenchmark reusa un mismo emisor; ambas son varios órdenes de magnitud superiores al objetivo.

## Comparación contra los NFR

| NFR | Objetivo | Medido (x64 baseline) | Margen | Resultado |
| --- | --- | --- | --- | --- |
| Latencia p95 por mensaje (camino común) | < 200 ms | ≈ 0.0099 ms (9.89 us) | ~20.000x bajo el límite | **CUMPLE** |
| Latencia p95 por mensaje (ráfaga que dispara) | < 200 ms | ≈ 0.028 ms (27.96 us) | ~7.000x bajo el límite | **CUMPLE** |
| Throughput sostenido | >= 50 msg/s | ≈ 485.800 msg/s (loop) / ≈ 998.000 msg/s (micro) | ~9.700x sobre el mínimo | **CUMPLE** |
| Memoria por mensaje (proxy) | observada | 896 B (común) / 3752 B (dispara) | — | observado |

En x64 baseline el motor de evaluación cumple los tres NFR con un margen enorme. El NFR está
expresado para el **dispositivo de referencia** (Raspberry Pi 4, ARM); ver abajo.

## Objetivo canónico: Raspberry Pi 4 (ARM)

El objetivo de aceptación de los NFR es el **hardware de referencia, la Raspberry Pi 4** (intake §17
P.10/P.12; arquitectura §8: "banco de pruebas de carga con mensajes simulados sobre el hardware
real"). Este equipo x64 es solo el **baseline**. El ARM de 32 bits es un tier deprioritizado
aceptado (intake §17 P.12, ADR-01), pero los **números de aceptación** se obtienen corriendo el
**mismo harness** en el dispositivo.

Como referencia, la Raspberry Pi 4 es del orden de 10-40x más lenta que este i9 por núcleo en
trabajo de un solo hilo. Aun con un factor conservador de 50x, una latencia p95 de ~28 us pasaría a
~1.4 ms (muy por debajo de 200 ms) y un throughput de ~485.000 msg/s pasaría a ~9.700 msg/s (muy por
encima de 50 msg/s). La holgura es suficiente para esperar cumplimiento en ARM, pero **debe
confirmarse midiendo allí**.

### Cómo correr el harness en la Raspberry para los números de aceptación

1. Instalar el SDK de .NET 10 para `linux-arm` en el dispositivo (o cross-compilar y copiar; ver
   `scripts/servicio/` para el patrón self-contained usado por el servicio).
2. Copiar el repositorio (o al menos `src/DiscordModeradorBot.Servicio` y
   `perf/DiscordModeradorBot.Servicio.Benchmarks`) al dispositivo.
3. Correr la versión COMPLETA en Release para mayor precisión del p95:

   ```bash
   dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- full
   ```

   Y el throughput directo:

   ```bash
   dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- throughput
   ```

4. Pegar la tabla de BenchmarkDotNet (columna **P95** del bloque de latencia) y el throughput
   reportado en este documento como "Números de aceptación (Raspberry Pi 4)", comparando contra
   p95 < 200 ms y >= 50 msg/s.

> Nota: como `dotnet run` cross-compila/JIT-ea en el dispositivo, la primera corrida puede tardar;
> BenchmarkDotNet descuenta el JIT con su fase de warmup, de modo que las cifras reportadas no
> incluyen el costo de arranque.
