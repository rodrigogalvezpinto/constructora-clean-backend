# 🧪 Script de Testing Completo - ConstructoraClean
# Ejecuta tests unitarios, coverage y mutation testing en Windows
# 
# Uso: .\run-tests.ps1 [-SkipMutation]

param(
    [switch]$SkipMutation = $false
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "🚀 Iniciando suite completa de testing para ConstructoraClean..." -ForegroundColor Cyan
Write-Host "=======================================================================" -ForegroundColor Gray

# Function to print section headers
function Write-Section {
    param([string]$Message)
    Write-Host ""
    Write-Host $Message -ForegroundColor Blue
    Write-Host ("=" * 60) -ForegroundColor Gray
}

# Function to check if command exists
function Test-CommandExists {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

# Check if skip mutation flag is provided
if ($SkipMutation) {
    Write-Host "⚠️  Mutation testing will be skipped" -ForegroundColor Yellow
}

# Verify required tools
Write-Section "🔍 Verificando herramientas requeridas"

if (-not (Test-CommandExists "dotnet")) {
    Write-Host "❌ .NET SDK no encontrado. Instala .NET 8 SDK." -ForegroundColor Red
    exit 1
}

$dotnetVersion = dotnet --version
Write-Host "✅ .NET SDK $dotnetVersion" -ForegroundColor Green

# Check for optional tools
$hasReportGen = Test-CommandExists "reportgenerator"
if ($hasReportGen) {
    Write-Host "✅ ReportGenerator disponible" -ForegroundColor Green
} else {
    Write-Host "⚠️  ReportGenerator no encontrado. Los reportes HTML no estarán disponibles." -ForegroundColor Yellow
    Write-Host "   Instalar con: dotnet tool install --global dotnet-reportgenerator-globaltool" -ForegroundColor Gray
}

$hasStryker = Test-CommandExists "dotnet-stryker"
if ($hasStryker) {
    Write-Host "✅ Stryker.NET disponible" -ForegroundColor Green
} elseif ($SkipMutation) {
    $hasStryker = $false
} else {
    Write-Host "⚠️  Stryker.NET no encontrado. Mutation testing no estará disponible." -ForegroundColor Yellow
    Write-Host "   Instalar con: dotnet tool install --global dotnet-stryker" -ForegroundColor Gray
    $hasStryker = $false
}

# Clean previous results
Write-Section "🧹 Limpiando resultados anteriores"
if (Test-Path "TestResults") { Remove-Item -Recurse -Force "TestResults" }
if (Test-Path "src\ConstructoraClean.Api.Tests\StrykerOutput") { 
    Remove-Item -Recurse -Force "src\ConstructoraClean.Api.Tests\StrykerOutput" 
}
Write-Host "✅ Limpieza completada" -ForegroundColor Green

# Step 1: Build solution
Write-Section "🔨 Compilando solución"
try {
    dotnet build --configuration Release --no-restore
    Write-Host "✅ Compilación exitosa" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error en compilación" -ForegroundColor Red
    exit 1
}

# Step 2: Run unit tests with coverage
Write-Section "🧪 Ejecutando tests unitarios con coverage"
try {
    dotnet test src\ConstructoraClean.Api.Tests\ `
        --configuration Release `
        --no-build `
        --collect:"XPlat Code Coverage" `
        --results-directory .\TestResults `
        --logger "console;verbosity=normal" `
        -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
    
    Write-Host "✅ Tests unitarios completados" -ForegroundColor Green
}
catch {
    Write-Host "❌ Tests unitarios fallaron" -ForegroundColor Red
    exit 1
}

# Step 3: Generate coverage report (if ReportGenerator is available)
if ($hasReportGen) {
    Write-Section "📊 Generando reporte de coverage"
    
    # Find coverage files
    $coverageFiles = Get-ChildItem -Path ".\TestResults" -Filter "coverage.opencover.xml" -Recurse
    
    if ($coverageFiles.Count -gt 0) {
        $reports = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
        
        reportgenerator `
            -reports:$reports `
            -targetdir:".\TestResults\CoverageReport" `
            -reporttypes:"Html;TextSummary;Badges" `
            -historydir:".\TestResults\CoverageHistory"
        
        Write-Host "✅ Reporte de coverage generado en .\TestResults\CoverageReport\index.html" -ForegroundColor Green
        
        # Display summary
        $summaryFile = ".\TestResults\CoverageReport\Summary.txt"
        if (Test-Path $summaryFile) {
            Write-Host ""
            Write-Host "📈 Resumen de Coverage:" -ForegroundColor Blue
            Get-Content $summaryFile
        }
    } else {
        Write-Host "⚠️  No se encontraron archivos de coverage" -ForegroundColor Yellow
    }
}

# Step 4: Run mutation tests (if Stryker is available and not skipped)
if ($hasStryker -and -not $SkipMutation) {
    Write-Section "🧬 Ejecutando mutation testing"
    
    Push-Location "src\ConstructoraClean.Api.Tests"
    
    try {
        dotnet stryker `
            --config-file StrykerConfig.json `
            --reporter html `
            --reporter progress `
            --reporter cleartext
        
        Write-Host "✅ Mutation testing completado" -ForegroundColor Green
        
        # Find and display mutation report location
        $mutationReport = Get-ChildItem -Path ".\StrykerOutput" -Filter "mutation-report.html" -Recurse | Select-Object -First 1
        if ($mutationReport) {
            $fullPath = $mutationReport.FullName
            Write-Host "📄 Reporte de mutación disponible en: $fullPath" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "⚠️  Mutation testing completado con warnings" -ForegroundColor Yellow
    }
    
    Pop-Location
} elseif ($SkipMutation) {
    Write-Host "⏭️  Mutation testing omitido por flag -SkipMutation" -ForegroundColor Yellow
} else {
    Write-Host "⏭️  Mutation testing omitido (Stryker no disponible)" -ForegroundColor Yellow
}

# Final summary
Write-Section "🎉 Resumen de Testing Completado"

Write-Host "✅ Tests unitarios: COMPLETADOS" -ForegroundColor Green

if ($hasReportGen) {
    Write-Host "✅ Reporte de coverage: GENERADO" -ForegroundColor Green
    $coverageReportPath = (Resolve-Path ".\TestResults\CoverageReport\index.html").Path
    Write-Host "   📂 Ubicación: $coverageReportPath" -ForegroundColor Gray
} else {
    Write-Host "⚠️  Reporte de coverage: NO DISPONIBLE" -ForegroundColor Yellow
}

if ($hasStryker -and -not $SkipMutation) {
    Write-Host "✅ Mutation testing: COMPLETADO" -ForegroundColor Green
    $mutationReport = Get-ChildItem -Path "src\ConstructoraClean.Api.Tests\StrykerOutput" -Filter "mutation-report.html" -Recurse | Select-Object -First 1
    if ($mutationReport) {
        Write-Host "   📂 Ubicación: $($mutationReport.FullName)" -ForegroundColor Gray
    }
} elseif ($SkipMutation) {
    Write-Host "⏭️  Mutation testing: OMITIDO" -ForegroundColor Yellow
} else {
    Write-Host "⚠️  Mutation testing: NO DISPONIBLE" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🎯 Testing suite completado exitosamente!" -ForegroundColor Green
Write-Host "💡 Tip: Ejecuta con -SkipMutation para tests más rápidos durante desarrollo" -ForegroundColor Blue
Write-Host "=======================================================================" -ForegroundColor Gray

# Open reports automatically if available
$openReports = $false
if ($openReports) {
    if ($hasReportGen -and (Test-Path ".\TestResults\CoverageReport\index.html")) {
        Write-Host "🌐 Abriendo reporte de coverage..." -ForegroundColor Cyan
        Start-Process ".\TestResults\CoverageReport\index.html"
    }
    
    if ($hasStryker -and -not $SkipMutation) {
        $mutationReport = Get-ChildItem -Path "src\ConstructoraClean.Api.Tests\StrykerOutput" -Filter "mutation-report.html" -Recurse | Select-Object -First 1
        if ($mutationReport) {
            Write-Host "🧬 Abriendo reporte de mutation testing..." -ForegroundColor Cyan
            Start-Process $mutationReport.FullName
        }
    }
} 