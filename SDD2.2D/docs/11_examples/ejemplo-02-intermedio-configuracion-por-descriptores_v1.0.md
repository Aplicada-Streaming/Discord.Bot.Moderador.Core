# Ejemplo 02 — Página mínima del panel dirigida por descriptores

**Proyecto:** discord-bots-admin
**Documento:** ejemplo-02-intermedio-configuracion-por-descriptores_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-20
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11)
**Nivel:** Intermedio
**Ubicación del código:** `/samples/02-intermedio-configuracion-por-descriptores/`

> Etapa de documentación. La estructura de `/samples/02-intermedio-configuracion-por-descriptores/` que se describe abajo es planificada: el código ejecutable, sus tests y su job de integración continua se materializan en la fase de codificación, posterior al handoff (`SOLUTION-INTAKE §16.1`). Este markdown es la especificación del sample.

## 1. Objetivo del sample

Demostrar la capa de configuración dirigida por descriptores: el sample sirve una página mínima del panel donde un parámetro configurable (el umbral de canales distintos) se renderiza enteramente a partir de su descriptor —valor por defecto, límites, leyenda, ejemplos y ayuda contextual— y se valida contra ese mismo descriptor al guardar. Al terminar, el desarrollador entiende cómo el Registro de descriptores actúa como fuente única de verdad y cómo la validación y la ayuda en pantalla se derivan sin escribir lógica específica por parámetro.

## 2. Nivel

Intermedio. Asume que el lector ya recorrió el sample 01 (ingreso de mensajes al pipeline) y agrega la cara de administración: una página interactiva del lado servidor con estado, validación y persistencia de configuración. Es más complejo que el sample 01 porque levanta el host web, una página interactiva y la capa de configuración con su descriptor; es más simple que el sample 03 porque no toca el motor de moderación ni ejecuta acciones. La presentación visual de la ayuda contextual corresponde a la categoría 03 (`../03_ux_ui_dx/wireframes-configuracion-de-moderacion_v1.0.md`); aquí se exige que el parámetro exponga su valor por defecto, su leyenda y sus ejemplos derivados del descriptor.

## 3. Prerequisites

- Runtime y SDK del proyecto, versión mínima declarada en `SOLUTION-INTAKE §17 P.1` y `§17 P.9` (.NET 10).
- Sistema operativo de desarrollo: estación x64 con el SDK instalado.
- Navegador evergreen para abrir la página del panel: últimas dos versiones de Chrome, Edge o Firefox, o Safari 16 o superior (`SOLUTION-INTAKE §17 P.9`).
- Almacén local para la configuración: archivo de base embebida creado por el propio sample en su primera ejecución (`SOLUTION-INTAKE §17 P.4`). No requiere instalar un motor de base aparte.
- No requiere token de la plataforma ni conexión a Discord: el sample opera enteramente sobre el panel local.

## 4. Cómo correrlo

1. Clonar el repositorio y entrar a la carpeta del sample: `cd samples/02-intermedio-configuracion-por-descriptores`.
2. Restaurar dependencias con el gestor de paquetes del ecosistema (`SOLUTION-INTAKE §17 P.1`).
3. Levantar el host web del sample con el comando de ejecución del stack declarado en `SOLUTION-INTAKE §17 P.1`: `dotnet run`.
4. Abrir en el navegador la dirección que imprime la consola (por ejemplo `http://localhost:5080/configuracion`).
5. Cambiar el valor del umbral, guardar y observar la validación contra el descriptor; comparar con el output esperado de §6.

## 5. Estructura del código

```
02-intermedio-configuracion-por-descriptores/
├── README.md                              # Resumen del sample y enlace a este markdown
├── src/
│   ├── Program.cs                         # Punto de entrada: compone el host web y la página
│   ├── Paginas/Configuracion.razor        # Página interactiva del lado servidor dirigida por el descriptor
│   ├── Descriptores/DescriptorUmbralCanales.cs  # Descriptor del parámetro (default, límites, leyenda, ejemplos)
│   ├── Servicios/ServicioConfiguracion.cs # Valida el valor contra el descriptor y lo persiste
│   └── data/                              # Carpeta donde el sample crea su base embebida en la primera corrida
└── tests/
    └── ConfiguracionDescriptoresTests.cs  # Verifica aceptación dentro de límites y rechazo fuera de límites
```

El archivo `Paginas/Configuracion.razor` es un componente del framework de UI declarado en `SOLUTION-INTAKE §17 P.1` (página interactiva del lado servidor que materializa la capacidad de panel); los archivos `.cs` del árbol son código del lenguaje del mismo stack declarado en `SOLUTION-INTAKE §17 P.1`. La página se renderiza a partir del descriptor del componente Registro de descriptores de parámetro (capa de Dominio) y la validación corre en el Servicio de configuración dirigida por descriptores (capa de Aplicación), ambos de la vista lógica de la arquitectura (`../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md §3`). El sample referencia la configuración por esquema de la categoría 03 (`design-rules-config-esquema_v1.0.md`, citado en `../03_ux_ui_dx/README.md §2`).

## 6. Qué esperar

Al abrir la página, el campo del umbral aparece con su valor por defecto, su leyenda y un ejemplo, todos provenientes del descriptor:

```
Umbral de canales distintos: [ 3 ]
Leyenda: cantidad mínima de canales distintos en los que un usuario debe publicar dentro de la ventana para marcar una ráfaga.
Ejemplo: con umbral 3, publicar en general, anuncios y memes dentro de la ventana marca la condición.
Límites permitidos: de 2 a 10.
```

Al guardar un valor dentro de límites (por ejemplo `4`), la consola y el panel confirman:

```
[config] umbral de canales distintos = 4 (valido, dentro de [2..10]) -> persistido
```

Al guardar un valor fuera de límites (por ejemplo `1`), la página muestra el error derivado del descriptor y no persiste:

```
[config] valor 1 rechazado: CONFIG_VALOR_FUERA_DE_LIMITE; permitido [2..10]; por defecto 3
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Dejar el campo vacío | Guardar sin ingresar valor | El sample aplica el valor por defecto del descriptor (3) en lugar de fallar |
| Endurecer los límites | Cambiar el rango del descriptor a `[2..5]` | La validación y la ayuda en pantalla reflejan el nuevo rango sin tocar la lógica de validación |
| Agregar un segundo parámetro | Registrar el descriptor de la ventana de detección | La misma página deriva la ayuda y la validación del nuevo descriptor automáticamente |
| Exención por rol (puente a CU-15) | Agregar una página mínima de exenciones | Sirve de puente al alcance de CU-15: declarar un sujeto exento que el pipeline descartará antes de evaluar |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| [CU-11 — Administrar reglas, grupos, eventos, acciones y parámetros](../02_especificacion_funcional/casos-de-uso/CU-11-administrar-reglas-grupos-eventos-acciones_v1.0.md) | Caso de uso | Ejecuta el paso 3 del flujo principal: presenta default, leyenda y ejemplos por parámetro y valida contra los límites del descriptor |
| [CU-15 — Definir exenciones por rol, usuario o canal](../02_especificacion_funcional/casos-de-uso/CU-15-definir-exenciones_v1.0.md) | Caso de uso (variación de §7) | La variación de exenciones materializa el alta de un sujeto exento que el pipeline descarta antes de evaluar (RN-07) |
| [Registro de descriptores de parámetro](../05_arquitectura_tecnica/arquitectura-solucion_v1.0.md) | Componente de la vista lógica (Dominio) | El parámetro se renderiza y se valida derivando todo del descriptor como fuente única de verdad |
| [ADR-12 — Configuración dirigida por esquema](../05_arquitectura_tecnica/adrs/ADR-12-configuracion-dirigida-por-esquema_v1.0.md) | Decisión arquitectónica | Materializa la configuración por descriptores como superficie de extensión (`../05_arquitectura_tecnica/extensibilidad_v1.0.md §1.1`) |
| [RN-10 — Configuración dirigida por descriptor](../02_especificacion_funcional/reglas-de-negocio/RN-10-configuracion-dirigida-por-descriptor_v1.0.md) | Regla de negocio | Aplica default y límites del descriptor; deriva la ayuda en pantalla del mismo descriptor |
| [Configuración por esquema (03)](../03_ux_ui_dx/wireframes-configuracion-de-moderacion_v1.0.md) | Artefacto UX/DX | Referencia la presentación de la ayuda contextual y la previsualización dirigidas por descriptores |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-20 | Versión inicial. Especificación del sample de nivel intermedio: página mínima del panel que renderiza y valida un parámetro a partir de su descriptor (default, límites, leyenda, ejemplos), ilustrando CU-11 y CU-15 y el Registro de descriptores. |
| 1.0 | 2026-06-20 | Limpieza de observaciones P2/P3 de los audits de fase: anclaje del archivo `.razor` al framework de UI de `SOLUTION-INTAKE §17 P.1` en §5, y de `dotnet run`/`.cs` al stack de `SOLUTION-INTAKE §17 P.1` en su primera aparición. |
