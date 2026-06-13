using FlightStatus.Api.Interfaces;
using FlightStatus.Api.Models;
using FlightStatus.Api.Enums;

namespace FlightStatus.Api.Providers;

/// <summary>
/// Stub provider for AeroTrack. Loads stub data from the project's Data folder and maps provider-specific statuses to UnifiedStatus.
/// </summary>
public class AeroTrackProvider : IFlightStatusProvider
{
    private readonly IEnumerable<FlightStatusResult> _data;

    /// <summary>
    /// ProviderName uniquely identifies this provider and is used in the FlightStatusResult.SourceProvider field.
    /// </summary>
    public string ProviderName => "AeroTrack";

    /// <summary>
    /// Construct the AeroTrack provider and load stub data.
    /// The provider resolves the data file path using Directory.GetCurrentDirectory() and several
    /// fallback locations to support running via dotnet run, inside IDE, and when published in a container.
    /// Note: AppContext.BaseDirectory typically points to the runtime 'bin' output folder (e.g. /app or bin/Debug/net10.0),
    /// which may not contain the project's Data folder. Directory.GetCurrentDirectory() is safer for resolving
    /// project-relative paths during local development and can also work inside containers when the Data folder is copied into the container image in a known relative location.
    /// </summary>
    /// <param name="loader">StubDataLoader to parse JSON content and normalize status strings.</param>
    public AeroTrackProvider(StubDataLoader loader)
    {
        var path = ResolveDataFilePath("aerotrack-stub.json");
        _data = loader.Load(path, MapStatus);
    }

    /// <summary>
    /// Get status for a flight/date from the AeroTrack stub dataset.
    /// </summary>
    public Task<FlightStatusResult?> GetStatusAsync(string flightNumber, DateOnly date, CancellationToken cancellationToken = default)
    {
        var match = _data.FirstOrDefault(x => string.Equals(x.FlightNumber, flightNumber, StringComparison.OrdinalIgnoreCase) && x.Date == date);
        if (match is null)
        {
            return Task.FromResult<FlightStatusResult?>(null);
        }

        // ensure source provider
        var result = match with { SourceProvider = ProviderName };
        return Task.FromResult<FlightStatusResult?>(result);
    }

    /// <summary>
    /// Map AeroTrack provider-specific status strings to unified status enum values.
    /// Examples of AeroTrack statuses: "On schedule", "Late departure", "Delayed - maintenance", "Rerouted".
    /// This mapping ensures the API returns consistent unified statuses regardless of provider vocabulary.
    /// </summary>
    private static UnifiedStatus MapStatus(string raw)
    {
        var s = raw.Trim();
        // provider-specific vocabulary mapping
        if (s.Equals("On schedule", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.OnTime;
        if (s.Contains("Late departure", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Delayed;
        if (s.Contains("cancel", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Cancelled;
        if (s.Contains("rerouted", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Diverted;
        if (s.Equals("No data", StringComparison.OrdinalIgnoreCase) || s.Equals("Unknown", StringComparison.OrdinalIgnoreCase)) return UnifiedStatus.Unknown;
        // fallback
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
    /// <param name="fileName">File name to locate.</param>
    /// <returns>Resolved file path (may not exist).</returns>
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
