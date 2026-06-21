@echo off
rem ============================================================
rem  build-all.bat
rem  Compila todos los servicios: descubre y llama a cada
rem  build-*.bat de esta carpeta (excepto a si mismo).
rem ============================================================
setlocal enabledelayedexpansion
echo ================================================
echo  build-all : compilando todos los servicios
echo ================================================
set "BATDIR=%~dp0"
set "TOTAL=0"
set "FALLOS=0"
for %%F in ("%BATDIR%build-*.bat") do (
  if /i not "%%~nxF"=="build-all.bat" (
    set /a TOTAL+=1
    echo.
    call "%%~fF"
    if not "!ERRORLEVEL!"=="0" set /a FALLOS+=1
  )
)
echo.
echo ------------------------------------------------
if "!FALLOS!"=="0" (
  echo [OK] build-all : !TOTAL! servicio^(s^) compilado^(s^)
) else (
  echo [ERROR] build-all : !FALLOS! de !TOTAL! servicio^(s^) con error
)
echo ------------------------------------------------
exit /b !FALLOS!
