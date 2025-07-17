using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ConstructoraClean.Api.Controllers;
using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Api.DTOs;
using ConstructoraClean.Application.Queries;
using ConstructoraClean.Api.Tests.Helpers;

namespace ConstructoraClean.Api.Tests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly Mock<IProjectCostService> _mockService;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
            _mockService = new Mock<IProjectCostService>();
            _controller = new ProjectsController(_mockService.Object);
        }

        [Fact]
        public void Constructor_WithValidService_ShouldCreateController()
        {
            // Arrange & Act
            var controller = new ProjectsController(_mockService.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new ProjectsController(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetProjectCosts_WithValidParameters_ShouldReturnOk()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);
            var expectedServiceResult = new ProjectCostsResult(
                1000m,
                new[] { new TopMaterialResult("Cemento", 500m) },
                new[] { new MonthlyBreakdownResult("2023-01", 1000m) }
            );

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ReturnsAsync(expectedServiceResult);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<ProjectCostsDto>().Subject;
            returnValue.TotalCost.Should().Be(1000m);
            returnValue.TopMaterials.Should().HaveCount(1);
            returnValue.MonthlyBreakdown.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProjectCosts_WhenServiceReturnsNull_ShouldReturnNotFound()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ReturnsAsync((ProjectCostsResult?)null);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetProjectCosts_WithInvalidProjectId_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidProjectId = 0;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);

            // Act
            var result = await _controller.GetProjectCosts(invalidProjectId, from, to);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("El ID de proyecto debe ser mayor que cero.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public async Task GetProjectCosts_WithNegativeProjectId_ShouldReturnBadRequest(int projectId)
        {
            // Arrange
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("El ID de proyecto debe ser mayor que cero.");
        }

        [Fact]
        public async Task GetProjectCosts_WithFromDateAfterToDate_ShouldReturnBadRequest()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 12, 31);
            var to = new DateTime(2023, 1, 1);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("La fecha inicial no puede ser mayor que la final.");
        }

        [Fact]
        public async Task GetProjectCosts_WithSameDates_ShouldReturnOk()
        {
            // Arrange
            var projectId = 1;
            var sameDate = new DateTime(2023, 6, 15);
            var expectedResult = new ProjectCostsResult(
                500m,
                new[] { new TopMaterialResult("Arena", 250m) },
                new[] { new MonthlyBreakdownResult("2023-06", 500m) }
            );

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == sameDate && q.To == sameDate)))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetProjectCosts(projectId, sameDate, sameDate);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetProjectCosts_WhenServiceThrowsException_ShouldReturn500()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);
            var exceptionMessage = "Database connection failed";

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusResult.StatusCode.Should().Be(500);
            statusResult.Value.Should().Be($"Error interno: {exceptionMessage}");
        }

        [Fact]
        public async Task GetProjectCosts_WithMaxValues_ShouldHandleCorrectly()
        {
            // Arrange
            var projectId = int.MaxValue;
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;
            var expectedResult = new ProjectCostsResult(
                decimal.MaxValue,
                new[] { new TopMaterialResult("MaxMaterial", decimal.MaxValue) },
                new[] { new MonthlyBreakdownResult("MAX-MONTH", decimal.MaxValue) }
            );

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetProjectCosts_WithMinValues_ShouldHandleCorrectly()
        {
            // Arrange
            var projectId = 1;
            var from = DateTime.MinValue;
            var to = DateTime.MinValue.AddDays(1);
            var expectedResult = new ProjectCostsResult(
                0m,
                new[] { new TopMaterialResult("Test", 0m) },
                new[] { new MonthlyBreakdownResult("Test", 0m) }
            );

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetProjectCosts_ShouldCallServiceOnce()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);
            var expectedResult = new ProjectCostsResult(
                1000m,
                new[] { new TopMaterialResult("Test", 500m) },
                new[] { new MonthlyBreakdownResult("2023-01", 1000m) }
            );

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ReturnsAsync(expectedResult);

            // Act
            await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            _mockService.Verify(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)), Times.Once);
        }

        [Fact]
        public async Task GetProjectCosts_WithEmptyResults_ShouldReturnOkWithEmptyData()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);
            var emptyResult = new ProjectCostsResult(
                0m,
                new List<TopMaterialResult>(),
                new List<MonthlyBreakdownResult>()
            );

            _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                q.ProjectId == projectId && q.From == from && q.To == to)))
                .ReturnsAsync(emptyResult);

            // Act
            var result = await _controller.GetProjectCosts(projectId, from, to);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<ProjectCostsDto>().Subject;
            returnValue.TotalCost.Should().Be(0m);
            returnValue.TopMaterials.Should().BeEmpty();
            returnValue.MonthlyBreakdown.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProjectCosts_WithDifferentExceptionTypes_ShouldReturn500()
        {
            // Arrange
            var projectId = 1;
            var from = new DateTime(2023, 1, 1);
            var to = new DateTime(2023, 12, 31);

            var exceptions = new Exception[]
            {
                new ArgumentException("Argument error"),
                new TimeoutException("Timeout error"),
                new UnauthorizedAccessException("Access denied"),
                new NullReferenceException("Null reference")
            };

            foreach (var exception in exceptions)
            {
                _mockService.Setup(s => s.GetProjectCostsAsync(It.Is<GetProjectCostsQuery>(q => 
                    q.ProjectId == projectId && q.From == from && q.To == to)))
                    .ThrowsAsync(exception);

                // Act
                var result = await _controller.GetProjectCosts(projectId, from, to);

                // Assert
                var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
                statusResult.StatusCode.Should().Be(500);
                statusResult.Value.Should().Be($"Error interno: {exception.Message}");
            }
        }
    }
} 