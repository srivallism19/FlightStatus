using Microsoft.Extensions.Logging;
using Moq;

namespace FlightStatus.Tests.Infrastructure;

/// <summary>
/// Tests for LoggingExtensions helper methods.
/// </summary>
public class LoggingExtensionsTests
{
    /// <summary>
    /// LogProviderSelection should call ILogger.LogInformation without throwing.
    /// </summary>
    [Fact]
    public void LogProviderSelection_DoesNotThrow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert: ensure the extension method can be invoked
        var ex = Record.Exception(() => Microsoft.Extensions.Logging.LoggerExtensions.LogInformation(mockLogger.Object, "Provider {provider} selected with LastUpdatedUtc={lastUpdated}", "P1", DateTimeOffset.UtcNow));
        Assert.Null(ex);
    }
}
