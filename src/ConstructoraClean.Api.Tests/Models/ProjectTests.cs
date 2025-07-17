using Xunit;
using FluentAssertions;
using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Api.Tests.Models
{
    public class ProjectTests
    {
        [Fact]
        public void Project_ShouldHaveDefaultValues()
        {
            // Act
            var project = new Project();

            // Assert
            project.Id.Should().Be(0);
            project.Name.Should().BeNull();
            project.Budget.Should().Be(0m);
            project.RegionId.Should().Be(0);
        }

        [Fact]
        public void Project_ShouldSetAndGetProperties()
        {
            // Arrange
            var project = new Project();
            var expectedId = 123;
            var expectedName = "Proyecto Test";
            var expectedBudget = 500000.50m;
            var expectedRegionId = 5;

            // Act
            project.Id = expectedId;
            project.Name = expectedName;
            project.Budget = expectedBudget;
            project.RegionId = expectedRegionId;

            // Assert
            project.Id.Should().Be(expectedId);
            project.Name.Should().Be(expectedName);
            project.Budget.Should().Be(expectedBudget);
            project.RegionId.Should().Be(expectedRegionId);
        }

        [Fact]
        public void Project_WithInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var project = new Project
            {
                Id = 456,
                Name = "Construcción Edificio Central",
                Budget = 1250000.75m,
                RegionId = 10
            };

            // Assert
            project.Id.Should().Be(456);
            project.Name.Should().Be("Construcción Edificio Central");
            project.Budget.Should().Be(1250000.75m);
            project.RegionId.Should().Be(10);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void Project_ShouldAcceptValidIdValues(int id)
        {
            // Arrange & Act
            var project = new Project { Id = id };

            // Assert
            project.Id.Should().Be(id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Simple")]
        [InlineData("Proyecto con espacios")]
        [InlineData("Proyecto-con-guiones")]
        [InlineData("PROYECTO MAYÚSCULAS")]
        [InlineData("proyecto minúsculas")]
        [InlineData("123 Proyecto con números")]
        [InlineData("Proyecto con caracteres especiales !@#$%")]
        public void Project_ShouldAcceptValidNameValues(string name)
        {
            // Arrange & Act
            var project = new Project { Name = name };

            // Assert
            project.Name.Should().Be(name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(1)]
        [InlineData(1000)]
        [InlineData(999999.99)]
        [InlineData(-1)] // Permitir valores negativos si el modelo lo permite
        public void Project_ShouldAcceptValidBudgetValues(decimal budget)
        {
            // Arrange & Act
            var project = new Project { Budget = budget };

            // Assert
            project.Budget.Should().Be(budget);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void Project_ShouldAcceptValidRegionIdValues(int regionId)
        {
            // Arrange & Act
            var project = new Project { RegionId = regionId };

            // Assert
            project.RegionId.Should().Be(regionId);
        }

        [Fact]
        public void Project_ShouldAllowNullName()
        {
            // Arrange & Act
            var project = new Project { Name = null! };

            // Assert
            project.Name.Should().BeNull();
        }

        [Fact]
        public void Project_WithMaxDecimalBudget_ShouldWork()
        {
            // Arrange & Act
            var project = new Project { Budget = decimal.MaxValue };

            // Assert
            project.Budget.Should().Be(decimal.MaxValue);
        }

        [Fact]
        public void Project_WithMinDecimalBudget_ShouldWork()
        {
            // Arrange & Act
            var project = new Project { Budget = decimal.MinValue };

            // Assert
            project.Budget.Should().Be(decimal.MinValue);
        }
    }
} 