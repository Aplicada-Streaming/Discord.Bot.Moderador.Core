#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Verifica los umbrales de cobertura del gate G3 (pipeline-ci-cd_v1.0.md STAGE-07).

.DESCRIPTION
    Parsea un reporte Cobertura (coverage.cobertura.xml producido por coverlet con
    coverlet.runsettings) y aplica los umbrales del intake §17 P.6 y de
    estrategia-testing_v1.0.md §2:

      - Global   : líneas >= 75 %, branches >= 65 %.
      - Detección: el módulo crítico de detección (namespace por defecto
                   DiscordModeradorBot.Servicio.Dominio.Conducta) >= 90 % líneas.

    coverlet NO aplica umbrales por namespace, por eso el chequeo per-módulo se hace
    acá de forma robusta sobre el XML. El job de CI falla (exit 1) si algún umbral
    no se cumple.

.PARAMETER ReportePath
    Ruta al coverage.cobertura.xml. Si se omite, busca el más reciente bajo ./TestResults.

.PARAMETER UmbralLineasGlobal
    Umbral mínimo de líneas global (por defecto 75).

.PARAMETER UmbralBranchesGlobal
    Umbral mínimo de branches global (por defecto 65).

.PARAMETER UmbralLineasDeteccion
    Umbral mínimo de líneas del módulo de detección (por defecto 90).

.PARAMETER NamespaceDeteccion
    Prefijo de namespace del módulo de detección (por defecto Dominio.Conducta).

.EXAMPLE
    pwsh scripts/ci/verificar-cobertura.ps1
#>
[CmdletBinding()]
param(
    [string]$ReportePath,
    [double]$UmbralLineasGlobal = 75,
    [double]$UmbralBranchesGlobal = 65,
    [double]$UmbralLineasDeteccion = 90,
    [string]$NamespaceDeteccion = 'DiscordModeradorBot.Servicio.Dominio.Conducta'
)

$ErrorActionPreference = 'Stop'

if (-not $ReportePath) {
    $candidato = Get-ChildItem -Path './TestResults' -Recurse -Filter 'coverage.cobertura.xml' -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $candidato) {
        Write-Error "No se encontró coverage.cobertura.xml bajo ./TestResults. Ejecutá 'dotnet test --settings coverlet.runsettings' primero."
        exit 2
    }
    $ReportePath = $candidato.FullName
}

if (-not (Test-Path $ReportePath)) {
    Write-Error "No existe el reporte de cobertura: $ReportePath"
    exit 2
}

Write-Host "Reporte de cobertura: $ReportePath"
[xml]$xml = Get-Content -Path $ReportePath

function Get-CoberturaDeClases {
    param($Clases)
    $lc = 0; $lv = 0; $bc = 0; $bv = 0
    foreach ($cls in $Clases) {
        foreach ($l in $cls.SelectNodes('.//line')) {
            $lv++
            if ([int]$l.hits -gt 0) { $lc++ }
            if ($l.branch -eq 'true') {
                $cc = $l.'condition-coverage'
                if ($cc -match '\((\d+)/(\d+)\)') {
                    $bc += [int]$Matches[1]
                    $bv += [int]$Matches[2]
                }
            }
        }
    }
    [pscustomobject]@{
        LineasCubiertas   = $lc
        LineasValidas     = $lv
        BranchesCubiertos = $bc
        BranchesValidos   = $bv
        PctLineas         = if ($lv) { [math]::Round(100.0 * $lc / $lv, 2) } else { 100.0 }
        PctBranches       = if ($bv) { [math]::Round(100.0 * $bc / $bv, 2) } else { 100.0 }
    }
}

$todas = $xml.SelectNodes('//class')
$global = Get-CoberturaDeClases -Clases $todas

$clasesDeteccion = @($todas | Where-Object { $_.name -like "$NamespaceDeteccion*" })
if ($clasesDeteccion.Count -eq 0) {
    Write-Error "No se hallaron clases del módulo de detección ($NamespaceDeteccion) en el reporte. ¿Cambió el namespace o se excluyó por error?"
    exit 2
}
$deteccion = Get-CoberturaDeClases -Clases $clasesDeteccion

Write-Host ''
Write-Host '================ RESUMEN DE COBERTURA (gate G3) ================'
Write-Host ("Global    líneas  : {0,6}% ({1}/{2})   umbral >= {3}%" -f $global.PctLineas, $global.LineasCubiertas, $global.LineasValidas, $UmbralLineasGlobal)
Write-Host ("Global    branches: {0,6}% ({1}/{2})   umbral >= {3}%" -f $global.PctBranches, $global.BranchesCubiertos, $global.BranchesValidos, $UmbralBranchesGlobal)
Write-Host ("Detección líneas  : {0,6}% ({1}/{2})   umbral >= {3}%" -f $deteccion.PctLineas, $deteccion.LineasCubiertas, $deteccion.LineasValidas, $UmbralLineasDeteccion)
Write-Host '==============================================================='
Write-Host ''

$fallas = @()
if ($global.PctLineas -lt $UmbralLineasGlobal) { $fallas += "Global líneas $($global.PctLineas)% < $UmbralLineasGlobal%" }
if ($global.PctBranches -lt $UmbralBranchesGlobal) { $fallas += "Global branches $($global.PctBranches)% < $UmbralBranchesGlobal%" }
if ($deteccion.PctLineas -lt $UmbralLineasDeteccion) { $fallas += "Detección líneas $($deteccion.PctLineas)% < $UmbralLineasDeteccion%" }

# Resumen para el Job Summary de GitHub Actions, si está disponible.
if ($env:GITHUB_STEP_SUMMARY) {
    $estado = if ($fallas.Count -eq 0) { '✅ PASA' } else { '❌ FALLA' }
    @"
### Gate G3 — Cobertura $estado

| Métrica | Cobertura | Umbral |
| --- | --- | --- |
| Global líneas | $($global.PctLineas)% | >= $UmbralLineasGlobal% |
| Global branches | $($global.PctBranches)% | >= $UmbralBranchesGlobal% |
| Módulo de detección (líneas) | $($deteccion.PctLineas)% | >= $UmbralLineasDeteccion% |
"@ | Out-File -FilePath $env:GITHUB_STEP_SUMMARY -Append -Encoding utf8
}

if ($fallas.Count -gt 0) {
    Write-Host 'GATE G3 NO CUMPLIDO:' -ForegroundColor Red
    $fallas | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}

Write-Host 'GATE G3 CUMPLIDO: todos los umbrales de cobertura se satisfacen.' -ForegroundColor Green
exit 0
