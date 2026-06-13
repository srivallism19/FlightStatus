using FlightStatus.Api.Interfaces;
using FlightStatus.Api.Models;
using FlightStatus.Api.Enums;

namespace FlightStatus.Api.Providers;

/// <summary>
/// Stub provider for QuickFlight. Loads stub data from the project's Data folder.
/// </summary>
public class QuickFlightProvider : IFlightStatusProvider
{
    private readonly IEnumerable<FlightStatusResult> _data;

    /// <summary>
    /// ProviderName uniquely identifies this provider and is used in the FlightStatusResult.SourceProvider field.
    /// </summary>
    public string ProviderName => "QuickFlight";

    /// <summary>
    /// Construct the QuickFlight provider and load stub data.
    /// </summary>
    public QuickFlightProvider(StubDataLoader loader)
    {
        var path = ResolveDataFilePath("quickflight-stub.json");
        _data = loader.Load(path, MapStatus);
    }

    /// <summary>
    /// Get status for a flight/date from the QuickFlight stub dataset.
    /// </summary>
    public Task<FlightStatusResult?> GetStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default)
    {
        var match = _data.FirstOrDefault(x => string.Equals(x.FlightNumber, flightNumber, StringComparison.OrdinalIgnoreCase) && x.Date == date);
        if (match is null)
        {
            return Task.FromResult<FlightStatusResult?>(null);
        }
        var result = match with { SourceProvider = ProviderName };
        return Task.FromResult<FlightStatusResult?>(result);
    }

    /// <summary>
    /// Map QuickFlight provider-specific status strings to unified status enum values.
    /// Examples: "On schedule", "Late departure", "Canceled", "Rerouted", "No data".
    /// </summary>
    private static UnifiedStatus MapStatus(string raw)
    {
        var s = raw.Trim();
        if (s.Equals("Ontime", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.OnTime;
        if (s.Contains("delay", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Delayed;
        if (s.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Cancelled;
        if (s.Contains("divert", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Diverted;
        if (s.Equals("No data", StringComparison.OrdinalIgnoreCase) || s.Equals("Unknown", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Unknown;
        return UnifiedStatus.Unknown;
    }

    /// <summary>
    /// Resolve the path to a data file by checking a set of candidate locations.
    /// Candidates include:
    ///  - {CurrentDirectory}/Data/{fileName}
    ///  - {CurrentDirectory}/src/FlightStatus.Api/Data/{fileName}
    ///  - {AppContext.BaseDirectory}/Data/{fileName}
    ///  - parent folders relative to AppContext.BaseDirectory
    /// The first existing candidate is returned. If none exist, the first candidate is returned as a fallback path.
    /// </summary>
    private static string ResolveDataFilePath(string fileName)
    {
        var cwd = Directory.GetCurrentDirectory();
        var baseDir = AppContext.BaseDirectory;

        var candidates = new[]
        {
            Path.Combine(cwd, "Data", fileName),
            Path.Combine(cwd, "src", "FlightStatus.Api", "Data", fileName),
            Path.Combine(baseDir, "Data", fileName),
            Path.Combine(baseDir, "..", "Data", fileName),
            Path.Combine(baseDir, "..", "..", "Data", fileName)
        };

        foreach (var c in candidates)
        {
            try
            {
                if (File.Exists(c)) return Path.GetFullPath(c);
            }
            catch
            {
                // ignore invalid candidate paths
            }
        }

        // fallback to the first candidate (project-local Data path)
        return Path.GetFullPath(candidates[0]);
    }
}
