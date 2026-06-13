using FlightStatus.Api.Models;

namespace FlightStatus.Api.Interfaces;

/// <summary>
/// Interface for flight status providers
/// </summary>
public interface IFlightStatusProvider
{
    /// <summary>
    /// ProviderName
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// GetStatusAsync
    /// </summary>
    /// <param name="flightNumber"></param>
    /// <param name="date"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FlightStatusResult?> GetStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default);
}
