using Xunit;
using Moq;
using FluentAssertions;
using ConstructoraClean.Infrastructure.Services;
using ConstructoraClean.Infrastructure.Data;
using ConstructoraClean.Api.Tests.Helpers;
using ConstructoraClean.Domain.Interfaces;
using ConstructoraClean.Application.Queries;
using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Api.Tests.Services
{
    public class ProjectCostServiceTests
    {
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly ProjectCostService _service;

        public ProjectCostServiceTests()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _service = new ProjectCostService(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithValidRepository_ShouldCreateService()
        {
            // Arrange
            var mockRepository = new Mock<IProjectRepository>();

            // Act
            var service = new ProjectCostService(mockRepository.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new ProjectCostService(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetProjectCostsAsync_WithValidQuery_ShouldReturnCorrectResult()
        {
            // Arrange
            var query = new GetProjectCostsQuery(1, DateTime.Now.AddDays(-30), DateTime.Now);
            var project = new Project { Id = 1, Name = "Test Project" };
            var topMaterials = new[] { new TopMaterial("Cemento", 1000m) };
            var monthlyBreakdown = new[] { new MonthlyBreakdown("2024-01", 1000m) };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
            _mockRepository.Setup(r => r.GetTotalCostAsync(1, query.From, query.To)).ReturnsAsync(1000m);
            _mockRepository.Setup(r => r.GetTopMaterialsAsync(1, query.From, query.To, 10)).ReturnsAsync(topMaterials);
            _mockRepository.Setup(r => r.GetMonthlyBreakdownAsync(1, query.From, query.To)).ReturnsAsync(monthlyBreakdown);

            // Act
            var result = await _service.GetProjectCostsAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.TotalCost.Should().Be(1000m);
            result.TopMaterials.Should().HaveCount(1);
            result.MonthlyBreakdown.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProjectCostsAsync_WithNonExistentProject_ShouldReturnNull()
        {
            // Arrange
            var query = new GetProjectCostsQuery(999, DateTime.Now.AddDays(-30), DateTime.Now);
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Project?)null);

            // Act
            var result = await _service.GetProjectCostsAsync(query);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetProjectCostsAsync_WithNullQuery_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await _service.Invoking(s => s.GetProjectCostsAsync(null!))
                .Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
