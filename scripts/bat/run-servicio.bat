@echo off
rem ============================================================
rem  run-servicio.bat
rem  Ejecuta el servicio DiscordModeradorBot.Servicio en una
rem  ventana aparte e imprime el resumen con URL y puerto.
rem  Modo de gateway: Simulado (no requiere token ni red).
rem ============================================================
setlocal
set "PUERTO=5072"
set "URL=http://localhost:%PUERTO%"
echo === [run-servicio] DiscordModeradorBot.Servicio ===
echo Iniciando en una ventana nueva (la primera vez compila)...
pushd "%~dp0..\.."
start "DiscordModeradorBot.Servicio @ %URL%" cmd /k "set ASPNETCORE_ENVIRONMENT=Development&& set ASPNETCORE_URLS=%URL%&& dotnet run --project src\DiscordModeradorBot.Servicio\DiscordModeradorBot.Servicio.csproj -c Debug"
popd
echo.
echo --------------------------------------------------------
echo  Resumen del servicio
echo --------------------------------------------------------
echo  Servicio : DiscordModeradorBot.Servicio
echo  Estado   : iniciando en ventana aparte
echo  URL      : %URL%
echo  Puerto   : %PUERTO%
echo  Panel    : %URL%/incidentes
echo  Login    : %URL%/ingresar
echo  Alta     : %URL%/configuracion-inicial   (primer ingreso)
echo  Gateway  : Simulado (sin token; ver SMOKE-TEST-DISCORD.md para Discord real)
echo  Dev      : se siembra el admin "admin" solo en Development (ver scripts/bat/README.md)
echo --------------------------------------------------------
endlocal
exit /b 0
