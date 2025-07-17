using Xunit;
using FluentAssertions;
using ConstructoraClean.Api.DTOs;

namespace ConstructoraClean.Api.Tests.DTOs
{
    public class HealthResponseTests
    {
        [Fact]
        public void HealthResponse_ShouldHaveDefaultValues()
        {
            // Act
            var dto = new HealthResponse();

            // Assert
            dto.Status.Should().BeNull();
            dto.Timestamp.Should().Be(default(DateTime));
        }

        [Fact]
        public void HealthResponse_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedStatus = "OK";
            var expectedTimestamp = DateTime.UtcNow;

            // Act
            var dto = new HealthResponse
            {
                Status = expectedStatus,
                Timestamp = expectedTimestamp
            };

            // Assert
            dto.Status.Should().Be(expectedStatus);
            dto.Timestamp.Should().Be(expectedTimestamp);
        }

        [Theory]
        [InlineData("OK")]
        [InlineData("FAIL")]
        [InlineData("ERROR")]
        [InlineData("HEALTHY")]
        [InlineData("UNHEALTHY")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Custom Status")]
        public void HealthResponse_ShouldAcceptValidStatusValues(string status)
        {
            // Act
            var dto = new HealthResponse { Status = status };

            // Assert
            dto.Status.Should().Be(status);
        }

        [Fact]
        public void HealthResponse_ShouldAllowNullStatus()
        {
            // Act
            var dto = new HealthResponse { Status = null! };

            // Assert
            dto.Status.Should().BeNull();
        }

        [Theory]
        [InlineData("2023-01-01T00:00:00Z")]
        [InlineData("2023-12-31T23:59:59Z")]
        [InlineData("1900-01-01T00:00:00Z")]
        [InlineData("9999-12-31T23:59:59Z")]
        public void HealthResponse_ShouldAcceptValidTimestamps(string timestampString)
        {
            // Arrange
            var timestamp = DateTime.Parse(timestampString);

            // Act
            var dto = new HealthResponse { Timestamp = timestamp };

            // Assert
            dto.Timestamp.Should().Be(timestamp);
        }

        [Fact]
        public void HealthResponse_WithMinDateTime_ShouldWork()
        {
            // Act
            var dto = new HealthResponse { Timestamp = DateTime.MinValue };

            // Assert
            dto.Timestamp.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void HealthResponse_WithMaxDateTime_ShouldWork()
        {
            // Act
            var dto = new HealthResponse { Timestamp = DateTime.MaxValue };

            // Assert
            dto.Timestamp.Should().Be(DateTime.MaxValue);
        }

        [Fact]
        public void HealthResponse_WithCurrentUtcTime_ShouldWork()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var dto = new HealthResponse { Timestamp = DateTime.UtcNow };
            var afterCreation = DateTime.UtcNow;

            // Assert
            dto.Timestamp.Should().BeAfter(beforeCreation.AddSeconds(-1));
            dto.Timestamp.Should().BeBefore(afterCreation.AddSeconds(1));
        }

        [Fact]
        public void HealthResponse_WithInitializer_ShouldSetAllProperties()
        {
            // Arrange
            var status = "HEALTHY";
            var timestamp = new DateTime(2023, 6, 15, 14, 30, 45);

            // Act
            var dto = new HealthResponse
            {
                Status = status,
                Timestamp = timestamp
            };

            // Assert
            dto.Status.Should().Be(status);
            dto.Timestamp.Should().Be(timestamp);
        }
    }
} 