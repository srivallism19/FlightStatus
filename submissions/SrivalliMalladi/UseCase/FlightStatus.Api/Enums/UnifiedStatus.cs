namespace FlightStatus.Api.Enums;

/// <summary>
/// Represents a normalized flight status used throughout the application.
/// </summary>
public enum UnifiedStatus
{
    /// <summary>
    /// Flight is scheduled to arrive/depart on time.
    /// </summary>
    OnTime = 1,

    /// <summary>
    /// Flight departure or arrival is delayed.
    /// </summary>
    Delayed = 2,

    /// <summary>
    /// Flight has been cancelled.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Flight has been diverted to an alternate airport.
    /// </summary>
    Diverted = 4,

    /// <summary>
    /// Status is unknown or not provided.
    /// </summary>
    Unknown = 5
}
