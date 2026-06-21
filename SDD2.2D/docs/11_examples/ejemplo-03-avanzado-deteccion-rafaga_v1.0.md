# Ejemplo 03 — Detección de ráfaga distribuida con mensajes simulados y contención

**Proyecto:** discord-bots-admin
**Documento:** ejemplo-03-avanzado-deteccion-rafaga_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11)
**Nivel:** Avanzado
**Ubicación del código:** `/samples/03-avanzado-deteccion-rafaga/`

> Etapa de documentación. La estructura de `/samples/03-avanzado-deteccion-rafaga/` que se describe abajo es planificada: el código ejecutable, sus tests y su job de integración continua se materializan en la fase de codificación, posterior al handoff (`SOLUTION-INTAKE §16.1`). Este markdown es la especificación del sample.

## 1. Objetivo del sample

Demostrar el punto central de la solución: alimentar el motor de moderación con un lote de mensajes simulados que reproduce una ráfaga distribuida —un mismo usuario publicando en varios canales distintos dentro de una ventana corta— para que el sistema marque la condición, evalúe la política y ejecute la contención (baneo con borrado retroactivo y reporte). El sample corre el mismo lote en modo simulación y en ejecución real, de modo que el desarrollador observa la diferencia: en simulación se registra lo que se habría hecho sin ejecutarlo; en ejecución real se ejecuta la acción. Al terminar, el desarrollador entiende cómo el pipeline distingue el spam automatizado del uso legítimo y cómo el modo simulación calibra la regla sin afectar a nadie.

## 2. Nivel

Avanzado. Es el sample de integración del producto: ejercita el Motor de moderación, el Evaluador de conducta sobre el estado en memoria, el Evaluador de políticas y el Ejecutor de acciones (`../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md §3`). Reúne lo que los samples anteriores establecieron por separado: consume mensajes como el sample 01 y aplica un umbral configurado por descriptor como el sample 02, y agrega encima la detección con estado, la evaluación de políticas por prioridad y la ejecución de acciones contra un adaptador simulado. Es el más complejo de los tres y demuestra el valor diferencial de la solución.

## 3. Prerequisites

- Runtime y SDK del proyecto, versión mínima declarada en `SOLUTION-INTAKE §17 P.1` y `§17 P.9` (.NET 10).
- Sistema operativo de desarrollo: estación x64 con el SDK instalado. El target de producción es ARM de 32 bits (`SOLUTION-INTAKE §17 P.9`); este sample se ejecuta en la estación de desarrollo.
- Editor con soporte para el lenguaje del proyecto (opcional, para inspeccionar el lote y la configuración).
- No requiere token de la plataforma ni conexión a Discord: el sample usa un lote de mensajes simulados y un adaptador de acciones simulado que registra la orden de baneo y de borrado en lugar de ejecutarla contra la plataforma real. Esto lo hace reproducible en un entorno limpio y en integración continua sin afectar a ningún usuario real.

## 4. Cómo correrlo

1. Clonar el repositorio y entrar a la carpeta del sample: `cd samples/03-avanzado-deteccion-rafaga`.
2. Restaurar dependencias con el gestor de paquetes del ecosistema (`SOLUTION-INTAKE §17 P.1`).
3. Correr el lote en modo simulación (registra lo que haría, no ejecuta) con el comando de ejecución del stack declarado en `SOLUTION-INTAKE §17 P.1`: `dotnet run -- --modo simulacion`.
4. Correr el mismo lote en ejecución real contra el adaptador simulado (ejecuta la acción de baneo y borrado): `dotnet run -- --modo ejecucion`.
5. Comparar ambas salidas con el output esperado de §6.

## 5. Estructura del código

```
03-avanzado-deteccion-rafaga/
├── README.md                              # Resumen del sample y enlace a este markdown
├── src/
│   ├── Program.cs                         # Punto de entrada: arma el pipeline y elige modo simulacion/ejecucion
│   ├── MotorModeracion.cs                 # Orquesta el pipeline de evaluacion del mensaje hasta el incidente
│   ├── EvaluadorConducta.cs              # Predicado con estado: cuenta canales distintos en la ventana
│   ├── EstadoConductaEnMemoria.cs        # Ventana deslizante de actividad por usuario y contexto
│   ├── EjecutorAcciones.cs              # Ejecuta o simula reportar + banear con borrado retroactivo
│   ├── AdaptadorAccionesSimulado.cs     # Registra las ordenes de baneo/borrado en lugar de llamar a la plataforma
│   ├── config/regla-rafaga.json         # Umbral de canales, ventana y modo (simulacion/ejecucion)
│   └── data/lote-rafaga.json            # Lote simulado: usuario en N canales distintos dentro de la ventana
└── tests/
    └── DeteccionRafagaTests.cs           # Verifica deteccion, no ejecucion en simulacion y ejecucion en real
```

Los archivos `.cs` del árbol son código del lenguaje del stack declarado en `SOLUTION-INTAKE §17 P.1`. El sample respeta el flujo de ejecución del pipeline (`../05_arquitectura_tecnica/flujo-ejecucion_v1.0.md`): descarte de exentos, actualización de la actividad reciente, evaluación de la política por prioridad con primera coincidencia, toma de copia de mensajes antes de cualquier borrado y ejecución de las acciones en orden determinista. El adaptador de acciones simulado se enchufa por el contrato del Adaptador del gateway y de la API de la plataforma (capa de Infraestructura), demostrando el dominio aislado de la infraestructura.

## 6. Qué esperar

Salida esperada en modo simulación (paso 3): la condición se marca, pero no se ejecuta ninguna acción real.

```
[deteccion] usuario=2002 canales distintos en ventana 2s: 3 (umbral 3) -> condicion de rafaga CUMPLIDA
[politica] evento "contencion-rafaga" coincide (prioridad 1, primera coincidencia)
[modo] SIMULACION: se registra lo que se habria hecho, no se ejecuta
[incidente] simulado | accion=banear+borrado-retroactivo | usuario=2002 | canales afectados=[general,anuncios,memes] | mensajes copiados=4
[resumen] incidentes simulados: 1 | acciones ejecutadas: 0
```

Salida esperada en ejecución real contra el adaptador simulado (paso 4): la acción se ejecuta y queda registrada.

```
[deteccion] usuario=2002 canales distintos en ventana 2s: 3 (umbral 3) -> condicion de rafaga CUMPLIDA
[politica] evento "contencion-rafaga" coincide (prioridad 1, primera coincidencia)
[modo] EJECUCION REAL
[copia] 4 mensajes copiados antes de borrar (evidencia del incidente)
[accion] reportar -> canal privado "mod-log"
[accion] banear usuario=2002 con borrado retroactivo (ventana de borrado dentro del tope de plataforma)
[antirrebote] usuario=2002 marcado como accionado en la rafaga vigente; no se repetira la accion
[incidente] ejecutado | accion=banear+borrado-retroactivo | usuario=2002 | canales afectados=[general,anuncios,memes] | mensajes copiados=4
[resumen] incidentes ejecutados: 1 | acciones ejecutadas: 2
```

En ambos modos, un segundo usuario que publica diez mensajes en un solo canal no dispara la condición (la cuenta de canales distintos permanece en 1), demostrando el discriminador que evita el falso positivo del usuario legítimo intenso.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Subir el umbral | Cambiar el umbral del descriptor de 3 a 5 en `config/regla-rafaga.json` | El lote de 3 canales deja de marcar la condición; ningún incidente se genera |
| Ráfaga espaciada | Separar las marcas de tiempo del lote más allá de la ventana | La condición no se marca hasta ampliar la ventana de detección (CU-01, flujo 5.B) |
| Emisor con rol superior | Marcar al emisor del lote como rol superior al bot | La acción no se ejecuta; se registra como incidente no accionable por jerarquía (RN-01, CU-02 BANEO_JERARQUIA_INSUFICIENTE) |
| Encadenar acciones | Agregar un timeout antes del baneo en la configuración | El ejecutor corre las acciones en el orden configurado (RN-05) |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| [CU-01 — Detectar ráfaga distribuida](../02_especificacion_funcional/casos-de-uso/CU-01-detectar-rafaga-distribuida_v1.0.md) | Caso de uso | Marca la condición de ráfaga contando canales distintos en la ventana (criterios CA-01, CA-02, CA-03) |
| [CU-02 — Banear al emisor de la ráfaga](../02_especificacion_funcional/casos-de-uso/CU-02-banear-emisor-rafaga_v1.0.md) | Caso de uso | Ejecuta la contención: copia de mensajes, baneo, registro del incidente y antirrebote (criterios CA-01, CA-04) |
| [CU-14 — Ejecutar una política en modo simulación](../02_especificacion_funcional/casos-de-uso/CU-14-ejecutar-politica-modo-simulacion_v1.0.md) | Caso de uso | Corre el mismo lote en simulación: registra lo que haría sin ejecutarlo (criterio CA-01) |
| [Motor de moderación (pipeline)](../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md) | Componente de la vista lógica (Dominio) | Orquesta el pipeline de evaluación del mensaje hasta el incidente |
| [Evaluador de reglas de conducta y Estado de conducta en memoria](../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md) | Componentes de la vista lógica (Dominio) | Cuentan los canales distintos sobre la ventana deslizante en memoria |
| [Ejecutor de acciones](../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md) | Componente de la vista lógica (Aplicación) | Ejecuta o simula reportar y banear con borrado retroactivo en orden determinista |
| [NFR de latencia de procesamiento por mensaje (p95 < 200 ms)](../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md) | Requisito no funcional (`SOLUTION-INTAKE §17 P.10`) | El sample puede instrumentar el pipeline de entrada a decisión para observar la latencia por mensaje del lote |
| [ADR-09 — Estado de conducta y antirrebote en memoria](../05_arquitectura_tecnica/adrs/ADR-09-estado-conducta-antirrebote-en-memoria_v1.0.md) | Decisión arquitectónica | El estado de conducta y el antirrebote viven en memoria; el sample los reconstruye con el lote |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial. Especificación del sample de nivel avanzado: disparo de la detección de ráfaga distribuida con mensajes simulados, corriendo el mismo lote en modo simulación y en ejecución real contra un adaptador simulado, mostrando la contención (baneo con borrado retroactivo y reporte). Ilustra CU-01, CU-02 y CU-14 y referencia el NFR de latencia. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: anclaje de `dotnet run`/`.cs` al stack de `SOLUTION-INTAKE §17 P.1` en su primera aparición. |
