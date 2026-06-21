# Scripts .bat para pruebas locales (Windows)

Scripts de conveniencia para compilar y ejecutar los servicios de la solución de forma local en Windows (cmd). Pensados para probar el sistema a mano sin recordar comandos.

## Convención

- `build-<servicio>.bat`: compila un servicio.
- `run-<servicio>.bat`: ejecuta un servicio en una ventana aparte e imprime un resumen con su URL y puerto.
- `build-all.bat`: descubre y llama a todos los `build-*.bat` de esta carpeta (excepto a sí mismo).
- `run-all.bat`: descubre y llama a todos los `run-*.bat` de esta carpeta (excepto a sí mismo).

`build-all.bat` y `run-all.bat` no listan los servicios a mano: recorren los `build-*.bat` / `run-*.bat` existentes. Para agregar un servicio nuevo basta con crear su par `build-<x>.bat` / `run-<x>.bat` siguiendo el patrón; los `*-all` lo toman automáticamente.

## Servicios

La solución es un monolito: hay un único servicio desplegable.

| Servicio | Build | Run | URL | Puerto |
| --- | --- | --- | --- | --- |
| DiscordModeradorBot.Servicio (panel + bot + persistencia) | `build-servicio.bat` | `run-servicio.bat` | http://localhost:5072 | 5072 |

## Uso

Desde esta carpeta (o por ruta completa):

```bat
build-all.bat       :: compila todo
run-all.bat         :: levanta todo y muestra el resumen de URLs/puertos

build-servicio.bat  :: compila solo el servicio
run-servicio.bat    :: levanta solo el servicio
```

Cada `run-*.bat` abre el servicio en una ventana de consola nueva (la primera vez compila) y la ventana original imprime el resumen con la URL y el puerto. Para detener un servicio, cerrá su ventana o presioná Ctrl+C en ella.

## Notas

- Requisitos: SDK de .NET 10 en el PATH (`dotnet --version`).
- Modo de gateway: los scripts arrancan en `Simulado` (no requieren token ni red); el servicio corre escenarios de demostración del motor de moderación. Para conectar a Discord real, ver `src/DiscordModeradorBot.Servicio/SMOKE-TEST-DISCORD.md`.
- Entorno: los scripts fijan `ASPNETCORE_ENVIRONMENT=Development`, por lo que se siembra un administrador de desarrollo (`admin`, contraseña por defecto solo de desarrollo) si no existe; el alta real es el first-run en `/configuracion-inicial`.
- Puerto: fijo en 5072 vía `ASPNETCORE_URLS`, para que el resumen sea determinista.
- Estos scripts son para la cara web (servicios con URL/puerto). El proyecto de benchmark (`perf/`) y los e2e (`tests/DiscordModeradorBot.Servicio.E2E`) no son servicios y se corren por separado (ver el README raíz).
