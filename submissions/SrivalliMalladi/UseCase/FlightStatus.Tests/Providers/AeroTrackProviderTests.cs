using FlightStatus.Api.Providers;
using FlightStatus.Api.Enums;

namespace FlightStatus.Tests.Providers;

/// <summary>
/// Tests for AeroTrackProvider mapping and GetStatusAsync behavior.
/// </summary>
public class AeroTrackProviderTests
{
    /// <summary>
    /// MapStatus should convert provider-specific "Late departure" to UnifiedStatus.Delayed.
    /// </summary>
    [Fact]
    public void MapStatus_LateDeparture_ReturnsDelayed()
    {
        // Arrange
        var method = typeof(AeroTrackProvider).GetMethod("MapStatus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        // Act
        var result = (UnifiedStatus)method.Invoke(null, new object[] { "Late departure" });

        // Assert
        Assert.Equal(UnifiedStatus.Delayed, result);
    }

    /// <summary>
    /// GetStatusAsync returns a FlightStatusResult when flight exists in stub data and null when not found.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_FoundAndNotFound()
    {
        // Arrange
        var loader = new StubDataLoader();
        var provider = new AeroTrackProvider(loader);

        // Act
        var found = await provider.GetStatusAsync("AB123", DateOnly.Parse("2026-06-01"));
        var notFound = await provider.GetStatusAsync("ZZ999", DateOnly.Parse("2026-06-01"));

        // Assert
        Assert.NotNull(found);
        Assert.Equal("AB123", found.FlightNumber, ignoreCase: true);
        Assert.Equal(UnifiedStatus.Delayed, found.Status);

        Assert.Null(notFound);
    }
}
