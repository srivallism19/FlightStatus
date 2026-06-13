using FlightStatus.Api.Providers;
using FlightStatus.Api.Enums;

namespace FlightStatus.Tests.Providers;

/// <summary>
/// Tests for QuickFlightProvider mapping behavior and null handling.
/// </summary>
public class QuickFlightProviderTests
{
    /// <summary>
    /// MapStatus should convert provider-specific "Delayed - aircraft late" to UnifiedStatus.Delayed.
    /// </summary>
    [Fact]
    public void MapStatus_DelayedAircraftLate_ReturnsDelayed()
    {
        // Arrange
        var method = typeof(QuickFlightProvider).GetMethod("MapStatus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        // Act
        var result = (UnifiedStatus)method.Invoke(null, new object[] { "Delayed - aircraft late" });

        // Assert
        Assert.Equal(UnifiedStatus.Delayed, result);
    }

    /// <summary>
    /// GetStatusAsync returns null for unknown flight and a result for known flight.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_FoundAndNotFound()
    {
        // Arrange
        var loader = new StubDataLoader();
        var provider = new QuickFlightProvider(loader);

        // Act
        var found = await provider.GetStatusAsync("AB123", DateOnly.Parse("2026-06-01"));
        var notFound = await provider.GetStatusAsync("YY000", DateOnly.Parse("2026-06-01"));

        // Assert
        Assert.NotNull(found);
        Assert.Equal(UnifiedStatus.Delayed, found.Status);

        Assert.Null(notFound);
    }
}
