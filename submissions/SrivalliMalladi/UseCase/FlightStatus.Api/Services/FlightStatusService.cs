using FlightStatus.Api.Enums;
using FlightStatus.Api.Interfaces;
using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

/// <summary>
/// FlightStatusService is the main service that aggregates flight status information from multiple providers and applies business logic to determine the final status result returned by the API.
/// </summary>
public class FlightStatusService : IFlightStatusService
{
    private readonly IEnumerable<IFlightStatusProvider> _providers;
    private readonly ILogger<FlightStatusService> _logger;

    /// <summary>
    /// Construct the FlightStatusService with a collection of IFlightStatusProvider implementations and a logger.
    /// </summary>
    /// <param name="providers"></param>
    /// <param name="logger"></param>
    public FlightStatusService(IEnumerable<IFlightStatusProvider> providers, ILogger<FlightStatusService> logger)
    {
        _providers = providers;
        _logger = logger;
    }


    /// <summary>
    /// GetStatusAsync queries all configured IFlightStatusProvider implementations in parallel for the given flight number and date, then applies business logic to select the most appropriate FlightStatusResult to return. If all providers fail or return no data, it returns a default FlightStatusResult with status Unknown.
    /// </summary>
    /// <param name="flightNumber"></param>
    /// <param name="date"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<FlightStatusResult> GetStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default)
    {
        var tasks = _providers.Select(p => ExecuteProviderSafe(p, flightNumber, date, cancellationToken)).ToArray();
        var results = await Task.WhenAll(tasks);

        var providers = results.Where(r => r is not null).Cast<FlightStatusResult>().ToList();
        if (providers.Count != 0)
        {
            var selectedProvider = providers.OrderByDescending(r => r.LastUpdatedUtc)
                                     .ThenBy(r => GetProviderPriority(r.SourceProvider))
                                     .First();
            _logger.LogInformation("Selected provider {provider} with lastUpdated {lastUpdated}", selectedProvider.SourceProvider, selectedProvider.LastUpdatedUtc);
            return selectedProvider;
        }

        return new FlightStatusResult
        {
            FlightNumber = flightNumber,
            Date = date,
            Status = UnifiedStatus.Unknown,
            LastUpdatedUtc = DateTimeOffset.UtcNow,
            SourceProvider = "None",
            Message = "No provider data; status unknown"
        };
    }

    private static int GetProviderPriority(string provider)
    {
        // lower value means higher priority in tie-breaker
        return provider switch
        {
            "AeroTrack" => 0,
            "QuickFlight" => 1,
            _ => 99
        };
    }

    private async Task<FlightStatusResult?> ExecuteProviderSafe(IFlightStatusProvider provider, string flightNumber, DateOnly date, CancellationToken cancellationToken)
    {
        try
        {
            var res = await provider.GetStatusAsync(flightNumber, date, cancellationToken);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider {provider} failed", provider.ProviderName);
            return null;
        }
    }
}
