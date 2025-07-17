using Xunit;
using FluentAssertions;
using ConstructoraClean.Api.DTOs;

namespace ConstructoraClean.Api.Tests.DTOs
{
    public class RegionOverrunDtoTests
    {
        [Fact]
        public void RegionOverrunDto_ShouldHaveDefaultValues()
        {
            // Act
            var dto = new RegionOverrunDto();

            // Assert
            dto.ProjectId.Should().Be(0);
            dto.Name.Should().BeNull(); // Corregido, la propiedad es null por defecto
            dto.Budget.Should().Be(0m);
            dto.TotalCost.Should().Be(0m);
            dto.OverrunPct.Should().BeNull();
        }

        [Fact]
        public void RegionOverrunDto_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedProjectId = 150;
            var expectedName = "Proyecto Construcción Plaza";
            var expectedBudget = 500000m;
            var expectedTotalCost = 600000m;
            var expectedOverrunPct = 0.2m;

            // Act
            var dto = new RegionOverrunDto
            {
                ProjectId = expectedProjectId,
                Name = expectedName,
                Budget = expectedBudget,
                TotalCost = expectedTotalCost,
                OverrunPct = expectedOverrunPct
            };

            // Assert
            dto.ProjectId.Should().Be(expectedProjectId);
            dto.Name.Should().Be(expectedName);
            dto.Budget.Should().Be(expectedBudget);
            dto.TotalCost.Should().Be(expectedTotalCost);
            dto.OverrunPct.Should().Be(expectedOverrunPct);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        public void RegionOverrunDto_ShouldAcceptValidProjectIds(int projectId)
        {
            // Act
            var dto = new RegionOverrunDto { ProjectId = projectId };

            // Assert
            dto.ProjectId.Should().Be(projectId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Proyecto Simple")]
        [InlineData("PROYECTO MAYÚSCULAS")]
        [InlineData("proyecto minúsculas")]
        [InlineData("Proyecto-con-guiones")]
        [InlineData("Proyecto 123")]
        [InlineData("Proyecto con caracteres especiales !@#")]
        public void RegionOverrunDto_ShouldAcceptValidNames(string name)
        {
            // Act
            var dto = new RegionOverrunDto { Name = name };

            // Assert
            dto.Name.Should().Be(name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(1000)]
        [InlineData(500000.75)]
        [InlineData(-1000)] // Presupuestos negativos podrían ser válidos en algunos casos
        public void RegionOverrunDto_ShouldAcceptValidBudgets(decimal budget)
        {
            // Act
            var dto = new RegionOverrunDto { Budget = budget };

            // Assert
            dto.Budget.Should().Be(budget);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(1000)]
        [InlineData(750000.50)]
        [InlineData(-500)] // Costos negativos podrían representar reembolsos
        public void RegionOverrunDto_ShouldAcceptValidTotalCosts(decimal totalCost)
        {
            // Act
            var dto = new RegionOverrunDto { TotalCost = totalCost };

            // Assert
            dto.TotalCost.Should().Be(totalCost);
        }

        [Theory]
        [InlineData(null)]
        public void RegionOverrunDto_ShouldAcceptNullOverrunPercentage(decimal? overrunPct)
        {
            // Act
            var dto = new RegionOverrunDto { OverrunPct = overrunPct };

            // Assert
            dto.OverrunPct.Should().BeNull();
        }
        
        [Theory]
        [InlineData(0.0)]
        [InlineData(0.1)]
        [InlineData(-0.1)]
        [InlineData(1.5)]
        [InlineData(-0.5)]
        [InlineData(10.0)]
        public void RegionOverrunDto_ShouldAcceptDecimalOverrunPercentages(decimal overrunPct)
        {
            // Act
            var dto = new RegionOverrunDto { OverrunPct = overrunPct };

            // Assert
            dto.OverrunPct.Should().Be(overrunPct);
        }

        [Fact]
        public void RegionOverrunDto_WithNullOverrunPct_ShouldWork()
        {
            // Act
            var dto = new RegionOverrunDto
            {
                ProjectId = 1,
                Name = "Proyecto Sin Presupuesto",
                Budget = 0m,
                TotalCost = 10000m,
                OverrunPct = null
            };

            // Assert
            dto.OverrunPct.Should().BeNull();
        }

        [Fact]
        public void RegionOverrunDto_WithPositiveOverrun_ShouldWork()
        {
            // Act
            var dto = new RegionOverrunDto
            {
                ProjectId = 2,
                Name = "Proyecto Sobrecosto",
                Budget = 100000m,
                TotalCost = 120000m,
                OverrunPct = 0.2m // 20% sobrecosto
            };

            // Assert
            dto.OverrunPct.Should().Be(0.2m);
        }

        [Fact]
        public void RegionOverrunDto_WithNegativeOverrun_ShouldWork()
        {
            // Act
            var dto = new RegionOverrunDto
            {
                ProjectId = 3,
                Name = "Proyecto Eficiente",
                Budget = 100000m,
                TotalCost = 85000m,
                OverrunPct = -0.15m // 15% bajo presupuesto
            };

            // Assert
            dto.OverrunPct.Should().Be(-0.15m);
        }

        [Fact]
        public void RegionOverrunDto_WithMaxDecimalValues_ShouldWork()
        {
            // Act
            var dto = new RegionOverrunDto
            {
                ProjectId = int.MaxValue,
                Name = "Proyecto Gigante",
                Budget = decimal.MaxValue,
                TotalCost = decimal.MaxValue,
                OverrunPct = decimal.MaxValue
            };

            // Assert
            dto.ProjectId.Should().Be(int.MaxValue);
            dto.Budget.Should().Be(decimal.MaxValue);
            dto.TotalCost.Should().Be(decimal.MaxValue);
            dto.OverrunPct.Should().Be(decimal.MaxValue);
        }

        [Fact]
        public void RegionOverrunDto_WithMinDecimalValues_ShouldWork()
        {
            // Act
            var dto = new RegionOverrunDto
            {
                ProjectId = int.MinValue,
                Name = "Proyecto Mínimo",
                Budget = decimal.MinValue,
                TotalCost = decimal.MinValue,
                OverrunPct = decimal.MinValue
            };

            // Assert
            dto.ProjectId.Should().Be(int.MinValue);
            dto.Budget.Should().Be(decimal.MinValue);
            dto.TotalCost.Should().Be(decimal.MinValue);
            dto.OverrunPct.Should().Be(decimal.MinValue);
        }
    }
} 