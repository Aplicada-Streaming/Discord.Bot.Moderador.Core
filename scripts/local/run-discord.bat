@echo off
rem ============================================================
rem  run-discord.bat
rem  Ejecuta el servicio en modo GATEWAY DISCORD (envio REAL a
rem  Discord) en una ventana aparte e imprime el resumen.
rem
rem  Diferencia con run-servicio.bat: setea Moderacion__Gateway=Discord
rem  para que las acciones (incluido el "Enviar prueba" al canal de
rem  reportes) salgan de verdad a Discord en vez de solo registrarse.
rem
rem  NO define la clave maestra: en Development se usa la clave por
rem  defecto, con la que ya se cifraron los tokens guardados. Definir
rem  DISCORDMODERADOR_CLAVE_MAESTRA aca ROMPERIA el descifrado del
rem  token que ya cargaste (se cifro con la clave por defecto).
rem ============================================================
setlocal
set "PUERTO=5072"
set "URL=http://localhost:%PUERTO%"
echo === [run-discord] DiscordModeradorBot.Servicio (modo Discord) ===
rem Cierra cualquier instancia previa del servicio (p. ej. una lanzada desde VS en
rem modo Simulado) para que NO quede ocupando el puerto 5072. Si una instancia vieja
rem retiene el puerto, la nueva en modo Discord no podria arrancar y el panel seguiria
rem respondiendo en Simulado. Se ignora el error si no habia ninguna corriendo.
echo Cerrando instancias previas del servicio (si las hay)...
taskkill /F /IM DiscordModeradorBot.Servicio.exe >nul 2>&1
echo Iniciando en una ventana nueva (la primera vez compila)...
pushd "%~dp0..\.."
rem El modo Discord se fuerza por ARGUMENTO de linea de comandos (--Moderacion:Gateway=Discord),
rem que tiene prioridad sobre appsettings (que trae Simulado por defecto) y sobre el perfil de
rem launchSettings. La variable de entorno se mantiene como respaldo.
start "DiscordModeradorBot.Servicio (Discord) @ %URL%" cmd /k "set ASPNETCORE_ENVIRONMENT=Development&& set ASPNETCORE_URLS=%URL%&& set Moderacion__Gateway=Discord&& dotnet run --project src\DiscordModeradorBot.Servicio\DiscordModeradorBot.Servicio.csproj -c Debug -- --Moderacion:Gateway=Discord"
popd
echo.
echo --------------------------------------------------------
echo  Resumen del servicio (MODO DISCORD)
echo --------------------------------------------------------
echo  Servicio : DiscordModeradorBot.Servicio
echo  Estado   : iniciando en ventana aparte
echo  URL      : %URL%
echo  Puerto   : %PUERTO%
echo  Panel    : %URL%/servidores
echo  Login    : %URL%/ingresar
echo  Gateway  : Discord (envio REAL; el bot debe estar en el server con permiso de escritura)
echo  Clave    : usa la clave maestra por defecto de Development (NO definir DISCORDMODERADOR_CLAVE_MAESTRA)
echo  Prueba   : en %URL%/servidores, el boton "Enviar prueba" publica en el canal de reportes
echo --------------------------------------------------------
endlocal
exit /b 0
