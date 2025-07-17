using Xunit;
using Moq;
using FluentAssertions;
using ConstructoraClean.Infrastructure.Services;
using ConstructoraClean.Api.Tests.Helpers;
using ConstructoraClean.Domain.Interfaces;
using ConstructoraClean.Application.Queries;

namespace ConstructoraClean.Api.Tests.Services
{
    public class RegionOverrunServiceTests
    {
        private readonly Mock<IRegionRepository> _mockRepository;
        private readonly RegionOverrunService _service;

        public RegionOverrunServiceTests()
        {
            _mockRepository = new Mock<IRegionRepository>();
            _service = new RegionOverrunService(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithValidRepository_ShouldCreateService()
        {
            // Arrange
            var mockRepository = new Mock<IRegionRepository>();

            // Act
            var service = new RegionOverrunService(mockRepository.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new RegionOverrunService(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetTopOverrunsAsync_WithValidQuery_ShouldReturnCorrectResult()
        {
            // Arrange
            var query = new GetRegionOverrunsQuery(1, 5);
            var expectedOverruns = new[]
            {
                new RegionOverrun(1, "Project 1", 1000m, 1200m, 20m),
                new RegionOverrun(2, "Project 2", 2000m, 2100m, 5m)
            };

            _mockRepository.Setup(r => r.GetTopOverrunsAsync(1, 5))
                .ReturnsAsync(expectedOverruns);

            // Act
            var result = await _service.GetTopOverrunsAsync(query);

            // Assert
            result.Should().HaveCount(2);
            result.First().ProjectId.Should().Be(1);
            result.First().OverrunPct.Should().Be(20m);
        }

        [Fact]
        public async Task GetTopOverrunsAsync_WithNullQuery_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await _service.Invoking(s => s.GetTopOverrunsAsync(null!))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetTopOverrunsAsync_WithEmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var query = new GetRegionOverrunsQuery(999, 5);
            _mockRepository.Setup(r => r.GetTopOverrunsAsync(999, 5))
                .ReturnsAsync(new List<RegionOverrun>());

            // Act
            var result = await _service.GetTopOverrunsAsync(query);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
