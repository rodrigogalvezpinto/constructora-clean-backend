using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ConstructoraClean.Api.Controllers;
using ConstructoraClean.Infrastructure.Data;
using ConstructoraClean.Api.Tests.Helpers;
using System.Data;
using System.Text.Json;

namespace ConstructoraClean.Api.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Mock<DapperContext> _mockContext;
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            _mockContext = DatabaseMockHelper.CreateMockContext();
            _controller = new HealthController(_mockContext.Object);
        }

        [Fact]
        public void Constructor_WithValidContext_ShouldCreateController()
        {
            // Arrange & Act
            var controller = new HealthController(_mockContext.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new HealthController(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Get_WhenDatabaseConnectionSucceeds_ShouldReturnOkWithBothStatusOk()
        {
            // Arrange
            var mockConnection = DatabaseMockHelper.SetupSuccessfulConnection();
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            var actionResult = _controller.Get();

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
            var resultValue = okResult.Subject.Value;
            resultValue.Should().NotBeNull();
            
            var json = JsonSerializer.Serialize(resultValue);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            dict.Should().NotBeNull();
            dict!["ApiStatus"].ToString().Should().Be("OK");
            dict["DbStatus"].ToString().Should().Be("OK");
            dict.Should().ContainKey("Timestamp");
        }

        [Fact]
        public void Get_WhenDatabaseConnectionFails_ShouldReturnOkWithDbStatusFail()
        {
            // Arrange
            var mockConnection = DatabaseMockHelper.SetupFailingConnection();
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            var actionResult = _controller.Get();

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
            var resultValue = okResult.Subject.Value;
            resultValue.Should().NotBeNull();
            
            var json = JsonSerializer.Serialize(resultValue);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            dict.Should().NotBeNull();
            dict!["ApiStatus"].ToString().Should().Be("OK");
            dict["DbStatus"].ToString().Should().Be("FAIL");
            dict.Should().ContainKey("Timestamp");
        }

        [Fact]
        public void Get_WhenCommandExecutionFails_ShouldReturnDbStatusFail()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            
            mockCommand.Setup(c => c.ExecuteScalar()).Throws(new InvalidOperationException("SQL execution failed"));
            mockCommand.SetupProperty(c => c.CommandText);
            mockConnection.Setup(c => c.Open());
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            var actionResult = _controller.Get();

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
            var resultValue = okResult.Subject.Value;
            resultValue.Should().NotBeNull();
            
            var json = JsonSerializer.Serialize(resultValue);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            dict!["DbStatus"].ToString().Should().Be("FAIL");
        }

        [Fact]
        public void Get_ShouldAlwaysReturnApiStatusOk()
        {
            // Arrange
            var mockConnection = DatabaseMockHelper.SetupFailingConnection();
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            var actionResult = _controller.Get();

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
            var resultValue = okResult.Subject.Value;
            resultValue.Should().NotBeNull();
            
            var json = JsonSerializer.Serialize(resultValue);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            dict!["ApiStatus"].ToString().Should().Be("OK");
        }

        [Fact]
        public void Get_ShouldIncludeTimestamp()
        {
            // Arrange
            var beforeCall = DateTime.UtcNow;
            var mockConnection = DatabaseMockHelper.SetupSuccessfulConnection();
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            var actionResult = _controller.Get();
            var afterCall = DateTime.UtcNow;

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
            var resultValue = okResult.Subject.Value;
            resultValue.Should().NotBeNull();
            
            var json = JsonSerializer.Serialize(resultValue);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            
            dict.Should().ContainKey("Timestamp");
            var timestampElement = dict!["Timestamp"];
            var timestamp = timestampElement.GetDateTime();
            
            timestamp.Should().BeAfter(beforeCall.AddSeconds(-1));
            timestamp.Should().BeBefore(afterCall.AddSeconds(1));
        }

        [Fact]
        public void Get_ShouldDisposeConnectionCorrectly()
        {
            // Arrange
            var mockConnection = DatabaseMockHelper.SetupSuccessfulConnection();
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            _controller.Get();

            // Assert
            _mockContext.Verify(c => c.CreateConnection(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Get_WhenExceptionDuringConnectionOpen_ShouldReturnDbStatusFail()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.Open()).Throws(new TimeoutException("Connection timeout"));
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            var actionResult = _controller.Get();

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
            var resultValue = okResult.Subject.Value;
            resultValue.Should().NotBeNull();
            
            var json = JsonSerializer.Serialize(resultValue);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            dict!["DbStatus"].ToString().Should().Be("FAIL");
        }

        [Fact]
        public void Get_WithVariousExceptions_ShouldAlwaysReturnDbStatusFail()
        {
            // Arrange
            var exceptions = new Exception[]
            {
                new SqlException("SQL Server error"),
                new TimeoutException("Timeout"),
                new UnauthorizedAccessException("Access denied"),
                new ArgumentException("Invalid argument"),
                new NullReferenceException("Null reference")
            };

            foreach (var exception in exceptions)
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(c => c.Open()).Throws(exception);
                _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

                // Act
                var actionResult = _controller.Get();

                // Assert
                var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>();
                var resultValue = okResult.Subject.Value;
                resultValue.Should().NotBeNull();
                
                var json = JsonSerializer.Serialize(resultValue);
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                
                dict!["DbStatus"].ToString().Should().Be("FAIL");
                dict["ApiStatus"].ToString().Should().Be("OK");
            }
        }

        [Fact]
        public void Get_ShouldUseCorrectSqlCommand()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            
            mockCommand.Setup(c => c.ExecuteScalar()).Returns(1);
            mockCommand.SetupProperty(c => c.CommandText);
            mockConnection.Setup(c => c.Open());
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            _mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Act
            _controller.Get();

            // Assert
            mockCommand.VerifySet(c => c.CommandText = "SELECT 1", Times.Once);
            mockCommand.Verify(c => c.ExecuteScalar(), Times.Once);
        }
    }

    // Helper class for SQL exceptions since SqlException constructor is internal
    public class SqlException : SystemException
    {
        public SqlException(string message) : base(message) { }
    }
}
