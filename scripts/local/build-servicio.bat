@echo off
rem ============================================================
rem  build-servicio.bat
rem  Compila el servicio DiscordModeradorBot.Servicio (Debug).
rem ============================================================
setlocal
echo === [build-servicio] DiscordModeradorBot.Servicio ===
pushd "%~dp0..\.."
dotnet build "src\DiscordModeradorBot.Servicio\DiscordModeradorBot.Servicio.csproj" -c Debug --nologo
set "RC=%ERRORLEVEL%"
popd
if "%RC%"=="0" (
  echo [OK] build-servicio: compilacion correcta
) else (
  echo [ERROR] build-servicio: fallo con codigo %RC%
)
exit /b %RC%
