@echo off
rem ============================================================
rem  run-all.bat
rem  Levanta el entorno local en modo SIMULADO (demo/validacion,
rem  sin token ni red): descubre y llama a cada run-*.bat de esta
rem  carpeta, EXCEPTO a si mismo y a run-discord.bat.
rem
rem  Por que se excluye run-discord.bat: esta solucion es un
rem  monolito (un unico servicio) y run-discord.bat es el MISMO
rem  servicio en modo Discord real, en el MISMO puerto 5072. Si
rem  run-all lo lanzara tambien, las dos instancias pelearian por
rem  el puerto 5072 y el panel podria quedar respondiendo en
rem  Simulado. Para el modo Discord real, usar run-discord.bat
rem  directamente (NO run-all).
rem ============================================================
setlocal enabledelayedexpansion
echo ================================================
echo  run-all : iniciando el entorno local (modo Simulado)
echo ================================================
set "BATDIR=%~dp0"
set "TOTAL=0"
for %%F in ("%BATDIR%run-*.bat") do (
  if /i not "%%~nxF"=="run-all.bat" if /i not "%%~nxF"=="run-discord.bat" (
    set /a TOTAL+=1
    echo.
    call "%%~fF"
  )
)
echo.
echo ================================================
echo  run-all : !TOTAL! servicio^(s^) iniciado^(s^)
echo  Cada servicio corre en su propia ventana.
echo  Arriba esta el resumen de URLs y puertos.
echo ================================================
exit /b 0
