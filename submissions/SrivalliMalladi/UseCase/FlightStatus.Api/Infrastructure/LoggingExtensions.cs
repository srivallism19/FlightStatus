namespace FlightStatus.Api.Infrastructure;

/// <summary>
/// LoggingExtensions provides extension methods for structured logging related to flight status provider selection and operations.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// LogProviderSelection logs an informational message when a flight status provider is selected for a query, including the provider name and the last updated timestamp of the data used for the selection.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="provider"></param>
    /// <param name="lastUpdated"></param>
    public static void LogProviderSelection(this ILogger logger, string provider, DateTimeOffset lastUpdated)
    {
        logger.LogInformation("Provider {provider} selected with LastUpdatedUtc={lastUpdated}", provider, lastUpdated);
    }
}
