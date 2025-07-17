using Xunit;
using FluentAssertions;
using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Api.Tests.Models
{
    public class PurchaseTests
    {
        [Fact]
        public void Purchase_ShouldHaveDefaultValues()
        {
            // Act
            var purchase = new Purchase();

            // Assert
            purchase.Id.Should().Be(0);
            purchase.ProjectId.Should().Be(0);
            purchase.MaterialId.Should().Be(0);
            purchase.SupplierId.Should().Be(0);
            purchase.TotalCost.Should().Be(0m);
        }

        [Fact]
        public void Purchase_ShouldSetAndGetProperties()
        {
            // Arrange
            var purchase = new Purchase();
            var expectedId = 1000;
            var expectedProjectId = 50;
            var expectedMaterialId = 25;
            var expectedSupplierId = 10;
            var expectedTotalCost = 15750.25m;

            // Act
            purchase.Id = expectedId;
            purchase.ProjectId = expectedProjectId;
            purchase.MaterialId = expectedMaterialId;
            purchase.SupplierId = expectedSupplierId;
            purchase.TotalCost = expectedTotalCost;

            // Assert
            purchase.Id.Should().Be(expectedId);
            purchase.ProjectId.Should().Be(expectedProjectId);
            purchase.MaterialId.Should().Be(expectedMaterialId);
            purchase.SupplierId.Should().Be(expectedSupplierId);
            purchase.TotalCost.Should().Be(expectedTotalCost);
        }

        [Fact]
        public void Purchase_WithInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var purchase = new Purchase
            {
                Id = 2000,
                ProjectId = 100,
                MaterialId = 50,
                SupplierId = 20,
                TotalCost = 32500.75m
            };

            // Assert
            purchase.Id.Should().Be(2000);
            purchase.ProjectId.Should().Be(100);
            purchase.MaterialId.Should().Be(50);
            purchase.SupplierId.Should().Be(20);
            purchase.TotalCost.Should().Be(32500.75m);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void Purchase_ShouldAcceptValidIds(int id)
        {
            // Arrange & Act
            var purchase = new Purchase { Id = id };

            // Assert
            purchase.Id.Should().Be(id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(500)]
        [InlineData(int.MaxValue)]
        public void Purchase_ShouldAcceptValidProjectIds(int projectId)
        {
            // Arrange & Act
            var purchase = new Purchase { ProjectId = projectId };

            // Assert
            purchase.ProjectId.Should().Be(projectId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(200)]
        [InlineData(int.MaxValue)]
        public void Purchase_ShouldAcceptValidMaterialIds(int materialId)
        {
            // Arrange & Act
            var purchase = new Purchase { MaterialId = materialId };

            // Assert
            purchase.MaterialId.Should().Be(materialId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(150)]
        [InlineData(int.MaxValue)]
        public void Purchase_ShouldAcceptValidSupplierIds(int supplierId)
        {
            // Arrange & Act
            var purchase = new Purchase { SupplierId = supplierId };

            // Assert
            purchase.SupplierId.Should().Be(supplierId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(1)]
        [InlineData(1000.50)]
        [InlineData(999999.99)]
        public void Purchase_ShouldAcceptValidTotalCosts(decimal totalCost)
        {
            // Arrange & Act
            var purchase = new Purchase { TotalCost = totalCost };

            // Assert
            purchase.TotalCost.Should().Be(totalCost);
        }

        [Fact]
        public void Purchase_WithMaxDecimalTotalCost_ShouldWork()
        {
            // Arrange & Act
            var purchase = new Purchase { TotalCost = decimal.MaxValue };

            // Assert
            purchase.TotalCost.Should().Be(decimal.MaxValue);
        }

        [Fact]
        public void Purchase_WithMinDecimalTotalCost_ShouldWork()
        {
            // Arrange & Act
            var purchase = new Purchase { TotalCost = decimal.MinValue };

            // Assert
            purchase.TotalCost.Should().Be(decimal.MinValue);
        }

        [Theory]
        [InlineData(-1, 1, 1, 1, 100)]
        [InlineData(1, -1, 1, 1, 100)]
        [InlineData(1, 1, -1, 1, 100)]
        [InlineData(1, 1, 1, -1, 100)]
        [InlineData(1, 1, 1, 1, -100)]
        public void Purchase_ShouldAcceptNegativeValues(int id, int projectId, int materialId, int supplierId, decimal totalCost)
        {
            // Arrange & Act
            var purchase = new Purchase
            {
                Id = id,
                ProjectId = projectId,
                MaterialId = materialId,
                SupplierId = supplierId,
                TotalCost = totalCost
            };

            // Assert
            purchase.Id.Should().Be(id);
            purchase.ProjectId.Should().Be(projectId);
            purchase.MaterialId.Should().Be(materialId);
            purchase.SupplierId.Should().Be(supplierId);
            purchase.TotalCost.Should().Be(totalCost);
        }
    }
} 