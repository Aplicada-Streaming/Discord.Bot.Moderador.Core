# DiscordModeradorBot.Servicio.Benchmarks

Harness de benchmark (BenchmarkDotNet) de los NFR de procesamiento del motor de moderación
(`MotorDeModeracion`): latencia por mensaje (**p95 < 200 ms**) y throughput sostenido
(**>= 50 mensajes/s**), con asignaciones por mensaje como proxy del NFR de memoria.

Trazabilidad: `SOLUTION-INTAKE §17 P.10`,
`SDD2.2D/docs/05_arquitectura_tecnica/arquitectura-solucion_v1.0.md` §8,
`SDD2.2D/docs/08_calidad_y_pruebas/matriz-cobertura-pruebas_v1.0.md` §3 (TC-55, TC-56).

## Fuera de la solución

Este proyecto está **deliberadamente FUERA de `DiscordModeradorBot.slnx`** (igual que el proyecto
e2e): es una app de consola de BenchmarkDotNet, no de tests. Así el `dotnet test` de la solución
(gate unitario + cobertura) no lo arrastra y no afecta el gate de cobertura. Se compila y corre por
path.

## Correr

Siempre en `-c Release` (BenchmarkDotNet exige optimizaciones):

```bash
# Microbenchmarks: latencia (media/mediana/p95) + throughput + memoria, corrida ACOTADA (rápida):
dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks

# Corrida COMPLETA (p95 más preciso, más lenta):
dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- full

# Loop cronometrado que imprime mensajes/s directamente:
dotnet run -c Release --project perf/DiscordModeradorBot.Servicio.Benchmarks -- throughput
```

## Qué mide

El hot path del motor con dependencias **en memoria / dobles**: repositorio de incidentes no-op,
adaptador de gateway no-op (sin red ni logging), estado de conducta y antirrebote en memoria, reloj
fijo. **Sin base de datos en disco, sin red, sin logging**, para medir el motor de evaluación y no
la E/S. Detalle completo y resultados en [`../RESULTADOS-BASELINE.md`](../RESULTADOS-BASELINE.md).

Los números de **aceptación** canónicos se obtienen corriendo este mismo harness en la
**Raspberry Pi 4 (ARM)**; este equipo x64 es solo baseline. Ver `RESULTADOS-BASELINE.md`.

## CI

Workflow manual `.github/workflows/benchmark.yml` (`workflow_dispatch`), **no bloqueante**: los
benchmarks son ruidosos y no se ponen como gate de PR. Se corre a demanda.
