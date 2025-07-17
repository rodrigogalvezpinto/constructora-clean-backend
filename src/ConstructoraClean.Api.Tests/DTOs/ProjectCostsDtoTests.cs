using Xunit;
using FluentAssertions;
using ConstructoraClean.Api.DTOs;

namespace ConstructoraClean.Api.Tests.DTOs
{
    public class ProjectCostsDtoTests
    {
        [Fact]
        public void ProjectCostsDto_ShouldHaveDefaultValues()
        {
            // Act
            var dto = new ProjectCostsDto();

            // Assert
            dto.TotalCost.Should().Be(0m);
            dto.TopMaterials.Should().NotBeNull().And.BeEmpty();
            dto.MonthlyBreakdown.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void ProjectCostsDto_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedTotalCost = 150000.75m;
            var expectedTopMaterials = new List<TopMaterialDto>
            {
                new() { Material = "Cemento", TotalCost = 50000m },
                new() { Material = "Acero", TotalCost = 30000m }
            };
            var expectedMonthlyBreakdown = new List<MonthlyBreakdownDto>
            {
                new() { Month = "2023-01", TotalCost = 75000m },
                new() { Month = "2023-02", TotalCost = 75000.75m }
            };

            // Act
            var dto = new ProjectCostsDto
            {
                TotalCost = expectedTotalCost,
                TopMaterials = expectedTopMaterials,
                MonthlyBreakdown = expectedMonthlyBreakdown
            };

            // Assert
            dto.TotalCost.Should().Be(expectedTotalCost);
            dto.TopMaterials.Should().BeEquivalentTo(expectedTopMaterials);
            dto.MonthlyBreakdown.Should().BeEquivalentTo(expectedMonthlyBreakdown);
        }

        [Fact]
        public void ProjectCostsDto_WithEmptyLists_ShouldWork()
        {
            // Act
            var dto = new ProjectCostsDto
            {
                TotalCost = 1000m,
                TopMaterials = new List<TopMaterialDto>(),
                MonthlyBreakdown = new List<MonthlyBreakdownDto>()
            };

            // Assert
            dto.TotalCost.Should().Be(1000m);
            dto.TopMaterials.Should().BeEmpty();
            dto.MonthlyBreakdown.Should().BeEmpty();
        }
    }

    public class TopMaterialDtoTests
    {
        [Fact]
        public void TopMaterialDto_ShouldHaveDefaultValues()
        {
            // Act
            var dto = new TopMaterialDto();

            // Assert
            dto.Material.Should().BeNull();
            dto.TotalCost.Should().Be(0m);
        }

        [Fact]
        public void TopMaterialDto_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedMaterial = "Cemento Portland";
            var expectedTotalCost = 25000.50m;

            // Act
            var dto = new TopMaterialDto
            {
                Material = expectedMaterial,
                TotalCost = expectedTotalCost
            };

            // Assert
            dto.Material.Should().Be(expectedMaterial);
            dto.TotalCost.Should().Be(expectedTotalCost);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Cemento")]
        [InlineData("Acero corrugado")]
        [InlineData("Material con espacios")]
        [InlineData("MATERIAL MAYÚSCULAS")]
        [InlineData("material minúsculas")]
        [InlineData("Material-123")]
        public void TopMaterialDto_ShouldAcceptValidMaterialNames(string material)
        {
            // Act
            var dto = new TopMaterialDto { Material = material };

            // Assert
            dto.Material.Should().Be(material);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(1000.50)]
        [InlineData(999999.99)]
        public void TopMaterialDto_ShouldAcceptValidTotalCosts(decimal totalCost)
        {
            // Act
            var dto = new TopMaterialDto { TotalCost = totalCost };

            // Assert
            dto.TotalCost.Should().Be(totalCost);
        }
    }

    public class MonthlyBreakdownDtoTests
    {
        [Fact]
        public void MonthlyBreakdownDto_ShouldHaveDefaultValues()
        {
            // Act
            var dto = new MonthlyBreakdownDto();

            // Assert
            dto.Month.Should().BeNull();
            dto.TotalCost.Should().Be(0m);
        }

        [Fact]
        public void MonthlyBreakdownDto_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedMonth = "2023-06";
            var expectedTotalCost = 45000.25m;

            // Act
            var dto = new MonthlyBreakdownDto
            {
                Month = expectedMonth,
                TotalCost = expectedTotalCost
            };

            // Assert
            dto.Month.Should().Be(expectedMonth);
            dto.TotalCost.Should().Be(expectedTotalCost);
        }

        [Theory]
        [InlineData("2023-01")]
        [InlineData("2023-12")]
        [InlineData("2024-06")]
        [InlineData("1999-01")]
        [InlineData("2030-12")]
        public void MonthlyBreakdownDto_ShouldAcceptValidMonthFormats(string month)
        {
            // Act
            var dto = new MonthlyBreakdownDto { Month = month };

            // Assert
            dto.Month.Should().Be(month);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(15000.75)]
        [InlineData(999999.99)]
        public void MonthlyBreakdownDto_ShouldAcceptValidTotalCosts(decimal totalCost)
        {
            // Act
            var dto = new MonthlyBreakdownDto { TotalCost = totalCost };

            // Assert
            dto.TotalCost.Should().Be(totalCost);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("invalid-format")]
        [InlineData("2023")]
        [InlineData("January 2023")]
        public void MonthlyBreakdownDto_ShouldAcceptAnyStringAsMonth(string month)
        {
            // Act
            var dto = new MonthlyBreakdownDto { Month = month };

            // Assert
            dto.Month.Should().Be(month);
        }
    }
} 