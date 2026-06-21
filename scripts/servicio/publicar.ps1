#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publica el paquete self-contained linux-arm (armv7l) y lo empaqueta como zip.

.DESCRIPTION
    Reproduce localmente STAGE-11 (Package) del pipeline
    (pipeline-ci-cd_v1.0.md §2; guia-publicacion-paquete-self-contained-linux-arm_v1.0.md §2).
    Cross-compila desde x64 (no requiere runner ARM, §17 P.8 / ADR-05), incluye los
    artefactos de despliegue de scripts/servicio/ y genera el checksum SHA-256.

.PARAMETER Version
    Versión SemVer para el nombre del paquete (por defecto 1.0.0).

.PARAMETER Salida
    Carpeta de salida para el zip (por defecto ./artefactos).

.EXAMPLE
    pwsh scripts/servicio/publicar.ps1 -Version 1.0.0
#>
[CmdletBinding()]
param(
    [string]$Version = '1.0.0',
    [string]$Salida = './artefactos'
)
$ErrorActionPreference = 'Stop'

$rid = 'linux-arm'
$repo = (Resolve-Path "$PSScriptRoot/../..").Path
$proyecto = Join-Path $repo 'src/DiscordModeradorBot.Servicio/DiscordModeradorBot.Servicio.csproj'
$dirPublish = Join-Path $repo "publish/$rid"
$dirServicio = $PSScriptRoot
$nombrePaquete = "discord-bots-admin_${Version}_${rid}"
$dirSalida = Join-Path $repo ($Salida -replace '^\./','')
$zip = Join-Path $dirSalida "$nombrePaquete.zip"

Write-Host "==> Publicando self-contained $rid (cross-compile desde x64)"
if (Test-Path $dirPublish) { Remove-Item -Recurse -Force $dirPublish }
dotnet publish $proyecto -c Release -r $rid --self-contained true -o $dirPublish
if ($LASTEXITCODE -ne 0) { throw "dotnet publish falló (exit $LASTEXITCODE)" }

# Verificar que el output incluye el ejecutable y la nativa de SQLite para linux-arm.
$exe = Join-Path $dirPublish 'DiscordModeradorBot.Servicio'
$sqlite = Join-Path $dirPublish 'libe_sqlite3.so'
if (-not (Test-Path $exe)) { throw "No se generó el ejecutable del servicio: $exe" }
if (-not (Test-Path $sqlite)) { throw "No se generó la nativa de SQLite linux-arm: $sqlite" }
Write-Host "==> OK: ejecutable y libe_sqlite3.so presentes."

# Incluir los artefactos de despliegue (instalador, unidad systemd, plantilla de entorno, README).
$dstServicio = Join-Path $dirPublish 'servicio'
New-Item -ItemType Directory -Force -Path $dstServicio | Out-Null
Copy-Item -Force (Join-Path $dirServicio 'instalar.sh') $dstServicio
Copy-Item -Force (Join-Path $dirServicio 'discord-moderador-bot.service') $dstServicio
Copy-Item -Force (Join-Path $dirServicio 'discord-bots-admin.env.example') $dstServicio
Copy-Item -Force (Join-Path $dirServicio 'README.md') $dstServicio -ErrorAction SilentlyContinue

# Empaquetar.
New-Item -ItemType Directory -Force -Path $dirSalida | Out-Null
if (Test-Path $zip) { Remove-Item -Force $zip }
Write-Host "==> Empaquetando $zip"
Compress-Archive -Path "$dirPublish/*" -DestinationPath $zip -CompressionLevel Optimal

# Checksum SHA-256.
$hash = (Get-FileHash -Algorithm SHA256 -Path $zip).Hash.ToLower()
"$hash  $nombrePaquete.zip" | Out-File -FilePath "$zip.sha256" -Encoding ascii
Write-Host "==> Paquete: $zip"
Write-Host "==> SHA256 : $hash"
Write-Host "==> Listo. Adjuntar zip + .sha256 (+ SBOM + firma en release) según guia-publicacion §2."
