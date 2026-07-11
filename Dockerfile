# Dockerfile — discord-bots-admin
# Build multi-stage: compila en SDK y corre en runtime mínimo (linux/amd64).
# Contexto de build: raíz de Discord.Bot.Moderador.Core/
# La base SQLite se persiste en el volume /app/data (ADR-02, ADR-14).

# ── Etapa 1: build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Archivos de configuración de la raíz que MSBuild aplica a TODOS los proyectos. Deben estar
# presentes ANTES del restore para que restore y build sean consistentes; si faltan (como cuando se
# copia solo el .csproj), el restore genera un obj/ inconsistente y el publish posterior NO genera
# los static web assets del framework (wwwroot/_framework/blazor.web.js), rompiendo la interactividad.
# - Directory.Build.props: referencia Nerdbank.GitVersioning y los analyzers.
# - version.json: versión base para nbgv.  - global.json: versión fijada del SDK.
COPY ["Directory.Build.props", "version.json", "global.json", "./"]

# Restaurar dependencias (con las props ya presentes) para aprovechar la caché de capas Docker.
COPY ["src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj", \
      "src/DiscordModeradorBot.Servicio/"]
RUN dotnet restore "src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj"

# Copiar el resto del código y publicar. Se hace un restore completo en el publish (sin --no-restore)
# para garantizar que los targets de static web assets del framework Blazor corran con todo presente.
COPY . .
RUN dotnet publish "src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj" \
    -c Release \
    -o /app/publish

# ── Etapa 2: runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Versión de build (se inyecta desde el pipeline con el SHA/tag; ver docker-publish.yml).
ARG BUILD_VERSION=dev
ENV BUILD_VERSION=$BUILD_VERSION

# Usuario no-root (principio de mínimo privilegio, OWASP Top 10 A05).
RUN groupadd --system --gid 1001 appgroup \
 && useradd  --system --uid 1001 --gid 1001 --no-create-home appuser

# Directorio de datos para el volume SQLite; propietario no-root.
RUN mkdir -p /app/data && chown appuser:appgroup /app/data

COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

# Volume declarativo para la base SQLite (ADR-02, ADR-14).
# El docker-compose.yml monta un named volume sobre este punto.
VOLUME ["/app/data"]

# Configuración por defecto — se sobreescribe con variables de entorno en Portainer (ADR-07, 12-factor).
# Moderacion__Gateway=Simulado es el default seguro; en producción se pone Discord.
ENV Persistencia__RutaBase=/app/data/discordmoderador.db \
    Moderacion__Gateway=Simulado \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "DiscordModeradorBot.Servicio.dll"]
