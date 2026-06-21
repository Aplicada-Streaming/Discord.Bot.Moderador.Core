# DiscordModeradorBot

Servicio monolítico de administración y moderación para servidores de Discord. Detecta el patrón de spam de ráfaga distribuida (un usuario que postea en varios canales distintos en una ventana corta) y otros patrones de contenido, y actúa sobre el emisor (baneo con borrado retroactivo, timeout, expulsión, roles), con reporte a un canal privado, panel web de administración y revisión de incidentes.

Solución `Administrador de Bots Moderador para Discord`; proyecto único `DiscordModeradorBot.Servicio` (tipo `web-monolith`). Reúne en un solo proceso el panel (Blazor Server), el bot de moderación (servicio en segundo plano) y la persistencia (SQLite), pensado para auto-hospedarse en una Raspberry Pi.

## Stack

- .NET 10 / C#
- Blazor Server interactivo con MudBlazor (panel de administración)
- Discord.Net (gateway en tiempo real + API REST)
- EF Core con SQLite en modo WAL (persistencia)
- Publicación self-contained para `linux-arm` (armv7l), instalada como servicio systemd

## Estructura del repositorio

```
src/DiscordModeradorBot.Servicio/            Servicio (Dominio, Aplicacion, Infraestructura, Components/Blazor)
tests/DiscordModeradorBot.Servicio.Tests/    Unit + integración (xUnit, FluentAssertions, NSubstitute)
tests/DiscordModeradorBot.Servicio.E2E/      e2e del panel (Playwright; fuera de la solución)
scripts/servicio/                            Publicación, unidad systemd e instalador para la Raspberry
scripts/ci/                                  Gate de cobertura por umbrales
.github/workflows/                           CI (gates) y publicación por tag
SDD2.2D/docs/                                Documentación SDD 2.2 de la solución (categorías 00-11)
SDD2.2D/devs/intake/                         Intake unificado y manifiesto derivado
```

La arquitectura interna (Clean Architecture por capas con un pipeline de evaluación y el dominio modelado como firewall multi-contexto) está documentada en `SDD2.2D/docs/05_arquitectura_tecnica/`.

## Requisitos

- SDK de .NET 10 (`dotnet --version` >= 10.0).
- Para los e2e: navegador de Playwright (Chromium), que se instala una sola vez (ver más abajo).

## Compilar

```
dotnet build
```

## Ejecutar localmente

```
dotnet run --project src/DiscordModeradorBot.Servicio
```

Por defecto arranca en modo de gateway `Simulado` (sin red ni token): aplica las migraciones, levanta el panel y un servicio que inyecta escenarios de demostración del motor de moderación. El panel queda en la URL que informa la consola; las páginas relevantes son `/incidentes`, `/servidores`, `/configuracion` y `/exenciones`. El acceso es del administrador único: la primera vez se hace el alta de credenciales en `/configuracion-inicial` (first-run) y luego el ingreso en `/ingresar`.

En entorno `Development` se siembra un administrador de desarrollo si no existe (usuario `admin`, contraseña por defecto solo de desarrollo, configurable). Fuera de `Development` no se siembra nada: el alta es el first-run real.

## Configuración

Por configuración (`appsettings*.json`) o variables de entorno:

| Clave | Variable de entorno | Valores | Default |
| --- | --- | --- | --- |
| `Moderacion:Gateway` | `Moderacion__Gateway` | `Simulado` \| `Discord` | `Simulado` |
| `Seed:AdminDesarrollo:Habilitado` | `Seed__AdminDesarrollo__Habilitado` | `true` \| `false` | solo Development |
| (secreto) | `DISCORDMODERADOR_CLAVE_MAESTRA` | clave maestra de cifrado de tokens | requerida en producción |

Los tokens de bot se cifran en reposo con AES usando la clave maestra de `DISCORDMODERADOR_CLAVE_MAESTRA` (nunca se guardan en claro ni se loguean). No hay secretos en el repositorio.

## Probar

Suite unitaria + integración (corre sin red ni navegador):

```
dotnet test DiscordModeradorBot.slnx
```

Cobertura con los umbrales del proyecto (global líneas >= 75% / branches >= 65%; módulo de detección >= 90%):

```
dotnet test --settings coverlet.runsettings --results-directory TestResults
pwsh scripts/ci/verificar-cobertura.ps1
```

End-to-end del panel (proyecto Playwright, fuera de la solución para no afectar el gate unitario). Instalar el navegador una vez y luego correr por path:

```
dotnet build tests/DiscordModeradorBot.Servicio.E2E
pwsh tests/DiscordModeradorBot.Servicio.E2E/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test tests/DiscordModeradorBot.Servicio.E2E/DiscordModeradorBot.Servicio.E2E.csproj
```

Sin navegador instalado, los e2e se omiten (no fallan).

## Integración real con Discord

Para conectar el bot a un servidor real (en vez del modo `Simulado`), seguí `src/DiscordModeradorBot.Servicio/SMOKE-TEST-DISCORD.md`: crear la aplicación-bot, habilitar los intents privilegiados (MessageContent y GuildMembers), invitar el bot con permisos y subirlo en la jerarquía, exportar `DISCORDMODERADOR_CLAVE_MAESTRA`, poner `Moderacion__Gateway=Discord`, registrar el token del servidor por el panel (queda cifrado), correr la prueba de configuración y probar primero en modo simulación antes de pasar a ejecución.

## Publicar para la Raspberry (linux-arm)

```
pwsh scripts/servicio/publicar.ps1      # o: bash scripts/servicio/publicar.sh
```

Genera un paquete self-contained para `linux-arm` (zip + checksum) con el binario, el runtime embebido, la nativa de SQLite y la carpeta `servicio/` (instalador, unidad systemd y plantilla de entorno). En el dispositivo, `scripts/servicio/instalar.sh` instala el servicio y preserva el archivo de entorno y la clave maestra en reinstalaciones, de modo que los tokens cifrados siguen siendo válidos (rollback). Ver `scripts/servicio/README.md`.

## CI/CD

`.github/workflows/ci.yml` corre en cada PR y push a `main` con gates bloqueantes: formato (`dotnet format --verify-no-changes`), build en Release con analizadores y warnings como errores, tests, cobertura con umbrales, análisis de dependencias vulnerables y SBOM. Los e2e corren en un job separado que instala el navegador. `.github/workflows/publish.yml` publica el paquete self-contained linux-arm al crear un tag `v*`.

## Documentación

La documentación SDD 2.2 completa de la solución (visión, necesidades de negocio, casos de uso, arquitectura y ADR, backlog, plan, calidad, devops y ejemplos) vive en `SDD2.2D/docs/`. El punto de entrada es `SDD2.2D/docs/README.md`.

## Estado

v1 funcionalmente completo: los 16 casos de uso del intake implementados en siete rebanadas verticales, integración real con Discord.Net, CI/CD con quality gates, endurecimiento de seguridad y pruebas e2e.

Decisiones abiertas para Sprint 0 (documentadas en `SDD2.2D/docs/05_arquitectura_tecnica/` y `09_devops/`): elección de la herramienta de auto-versioning (GitVersion o Nerdbank.GitVersioning), y calibración de los valores por defecto de detección (umbral de canales y ventana) con datos reales.
