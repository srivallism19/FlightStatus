using System.Text.Json.Serialization;
using FlightStatus.Api.Enums;

namespace FlightStatus.Api.Models;

/// <summary>
/// FlightStatusResult class
/// </summary>
public record FlightStatusResult
{
    /// <summary>
    /// FlightNumber
    /// </summary>
    [JsonPropertyName("flightNumber")]
    public string FlightNumber { get; init; } = string.Empty;

    /// <summary>
    /// Date
    /// </summary>
    [JsonPropertyName("date")]
    public DateOnly Date { get; init; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonPropertyName("status")]
    public UnifiedStatus Status { get; init; }

    /// <summary>
    /// ScheduledDeparture
    /// </summary>
    [JsonPropertyName("scheduledDepartureUtc")]
    public DateTimeOffset? ScheduledDeparture { get; init; }

    /// <summary>
    /// ActualDeparture
    /// </summary>
    [JsonPropertyName("actualDepartureUtc")]
    public DateTimeOffset? ActualDeparture { get; init; }

    /// <summary>
    /// ScheduledArrival
    /// </summary>
    [JsonPropertyName("scheduledArrivalUtc")]
    public DateTimeOffset? ScheduledArrival { get; init; }

    /// <summary>
    /// ActualArrival
    /// </summary>
    [JsonPropertyName("actualArrivalUtc")]
    public DateTimeOffset? ActualArrival { get; init; }

    /// <summary>
    /// Terminal
    /// </summary>
    [JsonPropertyName("terminal")]
    public string? Terminal { get; init; }

    /// <summary>
    /// Gate
    /// </summary>
    [JsonPropertyName("gate")]
    public string? Gate { get; init; }

    /// <summary>
    /// DelayReason
    /// </summary>
    [JsonPropertyName("delayReason")]
    public string? DelayReason { get; init; }

    /// <summary>
    /// LastUpdatedUtc
    /// </summary>
    [JsonPropertyName("lastUpdatedUtc")]
    public DateTimeOffset LastUpdatedUtc { get; init; }

    /// <summary>
    /// SourceProvider
    /// </summary>
    [JsonPropertyName("sourceProvider")]
    public string SourceProvider { get; init; } = string.Empty;

    /// <summary>
    /// Message
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
