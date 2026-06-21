@echo off
rem ============================================================
rem  run-all.bat
rem  Ejecuta todos los servicios: descubre y llama a cada
rem  run-*.bat de esta carpeta (excepto a si mismo). Cada
rem  servicio arranca en su propia ventana e imprime su URL.
rem ============================================================
setlocal enabledelayedexpansion
echo ================================================
echo  run-all : iniciando todos los servicios
echo ================================================
set "BATDIR=%~dp0"
set "TOTAL=0"
for %%F in ("%BATDIR%run-*.bat") do (
  if /i not "%%~nxF"=="run-all.bat" (
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
