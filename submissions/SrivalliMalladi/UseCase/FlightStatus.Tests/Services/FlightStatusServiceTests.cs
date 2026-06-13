using FlightStatus.Api.Interfaces;
using FlightStatus.Api.Models;
using FlightStatus.Api.Enums;
using FlightStatus.Api.Services;
using Moq;

namespace FlightStatus.Tests.Services;

/// <summary>
/// Tests for FlightStatusService selection logic including lastUpdatedUtc tie-breaking and Unknown fallback.
/// </summary>
public class FlightStatusServiceTests
{
    /// <summary>
    /// When multiple providers return results, the one with the latest LastUpdatedUtc should be selected.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_SelectsLatestLastUpdated()
    {
        // Arrange
        var p1 = new Mock<IFlightStatusProvider>();
        var p2 = new Mock<IFlightStatusProvider>();

        p1.SetupGet(x => x.ProviderName).Returns("P1");
        p2.SetupGet(x => x.ProviderName).Returns("P2");

        var date = DateOnly.Parse("2026-06-01");

        p1.Setup(x => x.GetStatusAsync("AB123", date, default)).ReturnsAsync(new FlightStatusResult
        {
            FlightNumber = "AB123",
            Date = date,
            Status = UnifiedStatus.Delayed,
            LastUpdatedUtc = DateTimeOffset.Parse("2026-06-01T10:00:00Z"),
            SourceProvider = "P1"
        });

        p2.Setup(x => x.GetStatusAsync("AB123", date, default)).ReturnsAsync(new FlightStatusResult
        {
            FlightNumber = "AB123",
            Date = date,
            Status = UnifiedStatus.Delayed,
            LastUpdatedUtc = DateTimeOffset.Parse("2026-06-01T11:00:00Z"),
            SourceProvider = "P2"
        });

        var service = new FlightStatusService(new[] { p1.Object, p2.Object }, new Mock<Microsoft.Extensions.Logging.ILogger<FlightStatusService>>().Object);

        // Act
        var selected = await service.GetStatusAsync("AB123", date);

        // Assert
        Assert.Equal("P2", selected.SourceProvider);
        Assert.Equal(DateTimeOffset.Parse("2026-06-01T11:00:00Z"), selected.LastUpdatedUtc);
    }

    /// <summary>
    /// When no providers return data, service should return Unknown status fallback.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_NoProviders_ReturnsUnknown()
    {
        // Arrange
        var p1 = new Mock<IFlightStatusProvider>();
        p1.SetupGet(x => x.ProviderName).Returns("P1");
        p1.Setup(x => x.GetStatusAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), default)).ReturnsAsync((FlightStatusResult?)null);

        var service = new FlightStatusService(new[] { p1.Object }, new Mock<Microsoft.Extensions.Logging.ILogger<FlightStatusService>>().Object);

        // Act
        var result = await service.GetStatusAsync("ZZ999", DateOnly.Parse("2026-06-01"));

        // Assert
        Assert.Equal(UnifiedStatus.Unknown, result.Status);
        Assert.Equal("None", result.SourceProvider);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }
}
