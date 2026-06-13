namespace FlightStatus.Api.Models
{
    /// <summary>
    /// RawFlightStatusDto represents the raw data structure of flight status information as it is loaded from the stub JSON files for each provider.
    /// </summary>
    public class RawFlightStatusDto
    {
        /// <summary>
        /// FlightNumber
        /// </summary>
        public string? FlightNumber { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// ScheduledDepartureUtc
        /// </summary>
        public DateTimeOffset? ScheduledDepartureUtc { get; set; }

        /// <summary>
        /// ScheduledArrivalUtc
        /// </summary>
        public DateTimeOffset? ScheduledArrivalUtc { get; set; }

        /// <summary>
        /// ActualDepartureUtc
        /// </summary>
        public DateTimeOffset? ActualDepartureUtc { get; set; }

        /// <summary>
        /// ActualArrivalUtc
        /// </summary>
        public DateTimeOffset? ActualArrivalUtc { get; set; }

        /// <summary>
        /// Terminal
        /// </summary>
        public string? Terminal { get; set; }

        /// <summary>
        /// Gate
        /// </summary>
        public string? Gate { get; set; }

        /// <summary>
        /// DelayReason
        /// </summary>
        public string? DelayReason { get; set; }

        /// <summary>
        /// LastUpdatedUtc
        /// </summary>
        public DateTimeOffset? LastUpdatedUtc { get; set; }

        /// <summary>
        /// SourceProvider
        /// </summary>
        public string? SourceProvider { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string? Message { get; set; }
    }
}
