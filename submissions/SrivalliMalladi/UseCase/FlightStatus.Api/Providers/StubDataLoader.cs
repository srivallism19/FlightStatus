using System.Text.Json;
using System.Text.Json.Serialization;
using FlightStatus.Api.Models;
using FlightStatus.Api.Enums;

namespace FlightStatus.Api.Providers;

/// <summary>
/// Loads stub JSON data and normalizes provider-specific fields into the canonical FlightStatusResult model.
/// The stub JSON may contain provider-specific status strings (e.g. "On schedule", "Late departure").
/// To support that, callers may provide a mapping function that converts the provider-specific status string
/// into the unified <see cref="UnifiedStatus"/> enum. If no mapping is provided, the loader will attempt
/// a case-insensitive parse of known enum names and fallback to Unknown.
/// </summary>
public class StubDataLoader
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Construct the StubDataLoader with JSON serializer options that support case-insensitive property names and a custom converter for DateOnly values.
    /// </summary>
    public StubDataLoader()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };
    }

    /// <summary>
    /// Load stub data from a JSON file path and normalize entries to <see cref="FlightStatusResult"/>.
    /// </summary>
    /// <param name="filePath">Path to the JSON file.</param>
    /// <param name="mapStatus">Optional mapping function from provider-specific status string to UnifiedStatus.</param>
    /// <returns>Enumerable of normalized FlightStatusResult entries.</returns>
    public IEnumerable<FlightStatusResult> Load(string filePath, Func<string, UnifiedStatus>? mapStatus = null)
    {
        if (!File.Exists(filePath))
            return Enumerable.Empty<FlightStatusResult>();

        var json = File.ReadAllText(filePath);
        try
        {
            var rawItems = JsonSerializer.Deserialize<IEnumerable<RawFlightStatusDto>>(json, _options) ?? Enumerable.Empty<RawFlightStatusDto>();

            var results = rawItems.Select(raw =>
            {
                var status = NormalizeStatus(raw.Status, mapStatus);

                return new FlightStatusResult
                {
                    FlightNumber = raw.FlightNumber ?? string.Empty,
                    Date = raw.Date,
                    Status = status,
                    ScheduledDeparture = raw.ScheduledDepartureUtc,
                    ScheduledArrival = raw.ScheduledArrivalUtc,
                    ActualDeparture = raw.ActualDepartureUtc,
                    ActualArrival = raw.ActualArrivalUtc,
                    Terminal = raw.Terminal,
                    Gate = raw.Gate,
                    DelayReason = raw.DelayReason,
                    LastUpdatedUtc = raw.LastUpdatedUtc ?? DateTimeOffset.UtcNow,
                    SourceProvider = raw.SourceProvider ?? string.Empty,
                    Message = raw.Message
                };
            }).ToList();

            return results;
        }
        catch
        {
            return Enumerable.Empty<FlightStatusResult>();
        }
    }

    /// <summary>
    /// Normalize a provider-specific status string into the unified enum using the provided mapper or a fallback parser.
    /// </summary>
    private static UnifiedStatus NormalizeStatus(string? rawStatus, Func<string, UnifiedStatus>? mapStatus)
    {
        if (string.IsNullOrWhiteSpace(rawStatus)) return UnifiedStatus.Unknown;
        var s = rawStatus.Trim();
        if (mapStatus != null)
        {
            try
            {
                return mapStatus(s);
            }
            catch
            {
                // fallthrough to generic parse
            }
        }

        // Generic case-insensitive parse to UnifiedStatus by name
        if (Enum.TryParse<UnifiedStatus>(s, true, out var parsed)) return parsed;

        return UnifiedStatus.Unknown;
    }

    private class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString() ?? string.Empty;
            if (DateOnly.TryParse(s, out var d)) return d;
            // Fallback parse yyyy-MM-dd
            if (DateOnly.TryParseExact(s, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out d)) return d;
            throw new JsonException($"Unable to parse DateOnly value: {s}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
