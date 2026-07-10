# Dockerfile — discord-bots-admin
# Build multi-stage: compila en SDK y corre en runtime mínimo (linux/amd64).
# Contexto de build: raíz de Discord.Bot.Moderador.Core/
# La base SQLite se persiste en el volume /app/data (ADR-02, ADR-14).

# ── Etapa 1: build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restaurar dependencias primero para aprovechar la caché de capas Docker.
COPY ["src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj", \
      "src/DiscordModeradorBot.Servicio/"]
RUN dotnet restore "src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj"

# Copiar el resto del código y publicar.
COPY . .
RUN dotnet publish "src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Etapa 2: runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

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

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

ENTRYPOINT ["dotnet", "DiscordModeradorBot.Servicio.dll"]
