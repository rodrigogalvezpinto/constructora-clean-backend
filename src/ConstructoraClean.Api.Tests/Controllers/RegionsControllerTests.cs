using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConstructoraClean.Api.Controllers;
using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Api.DTOs;
using ConstructoraClean.Application.Queries;

namespace ConstructoraClean.Api.Tests.Controllers
{
    public class RegionsControllerTests
    {
        private readonly Mock<IRegionOverrunService> _mockService;
        private readonly RegionsController _controller;

        public RegionsControllerTests()
        {
            _mockService = new Mock<IRegionOverrunService>();
            _controller = new RegionsController(_mockService.Object);
        }

        [Fact]
        public async Task GetTopOverruns_ReturnsBadRequest_WhenRegionIdIsInvalid()
        {
            var result = await _controller.GetTopOverruns(0, 10);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El ID de región debe ser mayor que cero.", badRequest.Value);
        }

        [Fact]
        public async Task GetTopOverruns_ReturnsBadRequest_WhenLimitIsInvalid()
        {
            var result = await _controller.GetTopOverruns(1, 0);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El límite debe ser mayor que cero.", badRequest.Value);
        }

        [Fact]
        public async Task GetTopOverruns_ReturnsNotFound_WhenServiceReturnsNull()
        {
            _mockService.Setup(s => s.GetTopOverrunsAsync(It.IsAny<GetRegionOverrunsQuery>()))
                .ReturnsAsync((IEnumerable<RegionOverrunResult>?)null);
            var result = await _controller.GetTopOverruns(1, 10);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTopOverruns_ReturnsNotFound_WhenServiceReturnsEmptyList()
        {
            _mockService.Setup(s => s.GetTopOverrunsAsync(It.IsAny<GetRegionOverrunsQuery>()))
                .ReturnsAsync(new List<RegionOverrunResult>());
            var result = await _controller.GetTopOverruns(1, 10);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTopOverruns_ReturnsOk_WhenServiceReturnsList()
        {
            var serviceResults = new List<RegionOverrunResult> {
                new RegionOverrunResult(123, "Proyecto Test", 1000m, 1200m, 0.2m)
            };
            
            _mockService.Setup(s => s.GetTopOverrunsAsync(It.IsAny<GetRegionOverrunsQuery>()))
                .ReturnsAsync(serviceResults);
            
            var result = await _controller.GetTopOverruns(1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<RegionOverrunDto>>(okResult.Value);
            Assert.Single(value);
            var dto = value[0];
            Assert.Equal(123, dto.ProjectId);
            Assert.Equal("Proyecto Test", dto.Name);
            Assert.Equal(1000m, dto.Budget);
            Assert.Equal(1200m, dto.TotalCost);
            Assert.Equal(0.2m, dto.OverrunPct);
        }

        [Fact]
        public async Task GetTopOverruns_Returns500_WhenExceptionThrown()
        {
            _mockService.Setup(s => s.GetTopOverrunsAsync(It.IsAny<GetRegionOverrunsQuery>()))
                .ThrowsAsync(new System.Exception("fail"));
            var result = await _controller.GetTopOverruns(1, 10);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Error interno", statusResult.Value.ToString());
        }
    }
}
