# Plantilla de instalador del add-on (Paso 8).
# Copia los binarios publicados a la carpeta de instalación y deja listo el .ard
# para registrarlo en SAP Business One (Administración > Add-Ons > Administración de add-ons).
#
# Requisitos en la estación: cliente SAP B1 10.0 + SDK, .NET Framework 4.8.
# Uso (PowerShell como administrador):
#   .\install.ps1 -PublishDir "..\..\src\addon\LN.Host\bin\Release\net48" -InstallFolder "C:\Program Files\LaNeta\AddOn"

param(
    [Parameter(Mandatory = $true)] [string] $PublishDir,
    [Parameter(Mandatory = $true)] [string] $InstallFolder,
    [string] $ArdTemplate = "$PSScriptRoot\..\ard\LN.AddOn.ard.template.xml",
    [string] $AddOnName = "Add-on LaNeta",
    [string] $AddOnVersion = "0.1.0"
)

$ErrorActionPreference = "Stop"

Write-Host "== Instalador Add-on LaNeta ==" -ForegroundColor Cyan

if (-not (Test-Path $PublishDir)) {
    throw "No existe el directorio de publicación: $PublishDir"
}

# 1. Crear carpeta de instalación y copiar binarios
New-Item -ItemType Directory -Force -Path $InstallFolder | Out-Null
Copy-Item -Path (Join-Path $PublishDir "*") -Destination $InstallFolder -Recurse -Force
Write-Host "Binarios copiados a $InstallFolder" -ForegroundColor Green

# 2. Generar el .ard final a partir de la plantilla
$exeName = "LN.AddOn.Host.exe"
$ard = Get-Content $ArdTemplate -Raw
$ard = $ard.Replace("{{ADDON_NAME}}", $AddOnName)
$ard = $ard.Replace("{{ADDON_VERSION}}", $AddOnVersion)
$ard = $ard.Replace("{{PARTNER_NAME}}", "LaNeta")
$ard = $ard.Replace("{{EXE_NAME}}", $exeName)
$ard = $ard.Replace("{{INSTALL_FOLDER}}", $InstallFolder)

$ardOut = Join-Path $InstallFolder "LN.AddOn.ard"
Set-Content -Path $ardOut -Value $ard -Encoding UTF8
Write-Host ".ard generado en $ardOut" -ForegroundColor Green

Write-Host ""
Write-Host "Siguiente paso MANUAL en SAP Business One:" -ForegroundColor Yellow
Write-Host "  Administracion > Add-Ons > Administracion de add-ons > Registrar > seleccionar:" -ForegroundColor Yellow
Write-Host "  $ardOut" -ForegroundColor Yellow
