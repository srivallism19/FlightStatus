using FlightStatus.Api.Models;

namespace FlightStatus.Api.Interfaces;

/// <summary>
/// IFlightStatusService defines the contract for the main service that aggregates flight status information from multiple providers and applies business logic to determine the final status result returned by the API.
/// </summary>
public interface IFlightStatusService
{
    /// <summary>
    /// GetStatusAsync retrieves the flight status for a given flight number and date. It aggregates data from multiple providers, applies business logic to determine the final status, and returns a FlightStatusResult that includes the unified status and relevant details.
    /// </summary>
    /// <param name="flightNumber"></param>
    /// <param name="date"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FlightStatusResult> GetStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default);
}
