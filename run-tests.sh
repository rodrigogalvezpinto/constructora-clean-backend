#!/bin/bash

# 🧪 Script de Testing Completo - ConstructoraClean
# Ejecuta tests unitarios, coverage y mutation testing
# 
# Uso: ./run-tests.sh [--skip-mutation]

set -e  # Exit on any error

echo "🚀 Iniciando suite completa de testing para ConstructoraClean..."
echo "======================================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if skip mutation flag is provided
SKIP_MUTATION=false
if [[ "$1" == "--skip-mutation" ]]; then
    SKIP_MUTATION=true
    echo -e "${YELLOW}⚠️  Mutation testing will be skipped${NC}"
fi

# Function to print section headers
print_section() {
    echo -e "\n${BLUE}$1${NC}"
    echo "$(printf '=%.0s' {1..60})"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Verify required tools
print_section "🔍 Verificando herramientas requeridas"

if ! command_exists dotnet; then
    echo -e "${RED}❌ .NET SDK no encontrado. Instala .NET 8 SDK.${NC}"
    exit 1
fi

echo -e "${GREEN}✅ .NET SDK $(dotnet --version)${NC}"

# Check for optional tools
if command_exists reportgenerator; then
    echo -e "${GREEN}✅ ReportGenerator disponible${NC}"
    HAS_REPORTGEN=true
else
    echo -e "${YELLOW}⚠️  ReportGenerator no encontrado. Los reportes HTML no estarán disponibles.${NC}"
    echo "   Instalar con: dotnet tool install --global dotnet-reportgenerator-globaltool"
    HAS_REPORTGEN=false
fi

if command_exists dotnet-stryker; then
    echo -e "${GREEN}✅ Stryker.NET disponible${NC}"
    HAS_STRYKER=true
elif $SKIP_MUTATION; then
    HAS_STRYKER=false
else
    echo -e "${YELLOW}⚠️  Stryker.NET no encontrado. Mutation testing no estará disponible.${NC}"
    echo "   Instalar con: dotnet tool install --global dotnet-stryker"
    HAS_STRYKER=false
fi

# Clean previous results
print_section "🧹 Limpiando resultados anteriores"
rm -rf TestResults/
rm -rf src/ConstructoraClean.Api.Tests/StrykerOutput/
echo -e "${GREEN}✅ Limpieza completada${NC}"

# Step 1: Build solution
print_section "🔨 Compilando solución"
dotnet build --configuration Release --no-restore
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Compilación exitosa${NC}"
else
    echo -e "${RED}❌ Error en compilación${NC}"
    exit 1
fi

# Step 2: Run unit tests with coverage
print_section "🧪 Ejecutando tests unitarios con coverage"
dotnet test src/ConstructoraClean.Api.Tests/ \
    --configuration Release \
    --no-build \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults \
    --logger "console;verbosity=normal" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Tests unitarios completados${NC}"
else
    echo -e "${RED}❌ Tests unitarios fallaron${NC}"
    exit 1
fi

# Step 3: Generate coverage report (if ReportGenerator is available)
if [ "$HAS_REPORTGEN" = true ]; then
    print_section "📊 Generando reporte de coverage"
    
    # Find coverage files
    COVERAGE_FILES=$(find ./TestResults -name "coverage.opencover.xml" -type f)
    
    if [ -n "$COVERAGE_FILES" ]; then
        reportgenerator \
            -reports:"$COVERAGE_FILES" \
            -targetdir:"./TestResults/CoverageReport" \
            -reporttypes:"Html;TextSummary;Badges" \
            -historydir:"./TestResults/CoverageHistory"
        
        echo -e "${GREEN}✅ Reporte de coverage generado en ./TestResults/CoverageReport/index.html${NC}"
        
        # Display summary
        if [ -f "./TestResults/CoverageReport/Summary.txt" ]; then
            echo -e "\n${BLUE}📈 Resumen de Coverage:${NC}"
            cat "./TestResults/CoverageReport/Summary.txt"
        fi
    else
        echo -e "${YELLOW}⚠️  No se encontraron archivos de coverage${NC}"
    fi
fi

# Step 4: Run mutation tests (if Stryker is available and not skipped)
if [ "$HAS_STRYKER" = true ] && [ "$SKIP_MUTATION" = false ]; then
    print_section "🧬 Ejecutando mutation testing"
    
    cd src/ConstructoraClean.Api.Tests
    
    dotnet stryker \
        --config-file StrykerConfig.json \
        --reporter html \
        --reporter progress \
        --reporter cleartext
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Mutation testing completado${NC}"
        
        # Find and display mutation report location
        MUTATION_REPORT=$(find ./StrykerOutput -name "mutation-report.html" -type f | head -1)
        if [ -n "$MUTATION_REPORT" ]; then
            echo -e "${GREEN}📄 Reporte de mutación disponible en: $MUTATION_REPORT${NC}"
        fi
    else
        echo -e "${YELLOW}⚠️  Mutation testing completado con warnings${NC}"
    fi
    
    cd ../../..
elif [ "$SKIP_MUTATION" = true ]; then
    echo -e "${YELLOW}⏭️  Mutation testing omitido por flag --skip-mutation${NC}"
else
    echo -e "${YELLOW}⏭️  Mutation testing omitido (Stryker no disponible)${NC}"
fi

# Final summary
print_section "🎉 Resumen de Testing Completado"

echo -e "${GREEN}✅ Tests unitarios: COMPLETADOS${NC}"

if [ "$HAS_REPORTGEN" = true ]; then
    echo -e "${GREEN}✅ Reporte de coverage: GENERADO${NC}"
    echo -e "   📂 Ubicación: ./TestResults/CoverageReport/index.html"
else
    echo -e "${YELLOW}⚠️  Reporte de coverage: NO DISPONIBLE${NC}"
fi

if [ "$HAS_STRYKER" = true ] && [ "$SKIP_MUTATION" = false ]; then
    echo -e "${GREEN}✅ Mutation testing: COMPLETADO${NC}"
    MUTATION_REPORT=$(find ./src/ConstructoraClean.Api.Tests/StrykerOutput -name "mutation-report.html" -type f | head -1)
    if [ -n "$MUTATION_REPORT" ]; then
        echo -e "   📂 Ubicación: $MUTATION_REPORT"
    fi
elif [ "$SKIP_MUTATION" = true ]; then
    echo -e "${YELLOW}⏭️  Mutation testing: OMITIDO${NC}"
else
    echo -e "${YELLOW}⚠️  Mutation testing: NO DISPONIBLE${NC}"
fi

echo -e "\n${GREEN}🎯 Testing suite completado exitosamente!${NC}"
echo -e "${BLUE}💡 Tip: Ejecuta con --skip-mutation para tests más rápidos durante desarrollo${NC}"
echo "=======================================================================" 