using Xunit;
using FluentAssertions;
using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Api.Tests.Models
{
    public class MaterialTests
    {
        [Fact]
        public void Material_ShouldHaveDefaultValues()
        {
            // Act
            var material = new Material();

            // Assert
            material.Id.Should().Be(0);
            material.Name.Should().BeNull();
        }

        [Fact]
        public void Material_ShouldSetAndGetProperties()
        {
            // Arrange
            var material = new Material();
            var expectedId = 100;
            var expectedName = "Cemento Portland";

            // Act
            material.Id = expectedId;
            material.Name = expectedName;

            // Assert
            material.Id.Should().Be(expectedId);
            material.Name.Should().Be(expectedName);
        }

        [Fact]
        public void Material_WithInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var material = new Material
            {
                Id = 200,
                Name = "Acero Estructural"
            };

            // Assert
            material.Id.Should().Be(200);
            material.Name.Should().Be("Acero Estructural");
        }

        [Theory]
        [InlineData("Cemento")]
        [InlineData("Acero corrugado")]
        [InlineData("Ladrillos rojos")]
        [InlineData("CONCRETO PREMEZCLADO")]
        [InlineData("Arena fina")]
        [InlineData("Material-con-guiones")]
        [InlineData("Material 123")]
        public void Material_ShouldAcceptValidNames(string name)
        {
            // Arrange & Act
            var material = new Material { Name = name };

            // Assert
            material.Name.Should().Be(name);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        [InlineData(int.MaxValue)]
        public void Material_ShouldAcceptValidIds(int id)
        {
            // Arrange & Act
            var material = new Material { Id = id };

            // Assert
            material.Id.Should().Be(id);
        }

        [Fact]
        public void Material_ShouldAllowNullName()
        {
            // Arrange & Act
            var material = new Material { Name = null! };

            // Assert
            material.Name.Should().BeNull();
        }
    }
} 