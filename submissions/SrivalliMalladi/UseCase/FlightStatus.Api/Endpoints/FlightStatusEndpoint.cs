using System.Globalization;
using FlightStatus.Api.Interfaces;

namespace FlightStatus.Api.Endpoints;

/// <summary>
/// Endpoints related to flight status retrieval.
/// This class maps minimal API endpoints and demonstrates query-parameter binding for flightNumber and date.
/// </summary>
public static class FlightStatusEndpoint
{
    /// <summary>
    /// Map the flight status endpoint.
    /// The endpoint binds query parameters by name: flightNumber (string) and date (string in yyyy-MM-dd).
    /// Although the service accepts a DateOnly, the endpoint validates the incoming date string using the exact format yyyy-MM-dd
    /// to provide clear error messages when parsing fails. Swagger will display both flightNumber and date as query parameters.
    /// </summary>
    /// <param name="app">Web application builder to register endpoints on.</param>
    public static void MapFlightStatusEndpoints(this WebApplication app)
    {
        app.MapGet("/flights/status", async (HttpContext http, string? flightNumber, string? date, IFlightStatusService service) =>
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(flightNumber))
            {
                errors.Add("Missing flightNumber");
            }

            if (string.IsNullOrWhiteSpace(date))
            {
                errors.Add("Missing date (yyyy-MM-dd)");
            }

            if (errors.Count > 0)
            {
                return Results.BadRequest(new { errors });
            }

            // Validate date format strictly as yyyy-MM-dd
            if (!DateOnly.TryParseExact(date!, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return Results.BadRequest(new { errors = new[] { "Invalid date format. Expect yyyy-MM-dd" } });
            }

            var normalizedFlight = flightNumber!.Trim().ToUpperInvariant();

            var result = await service.GetStatusAsync(normalizedFlight, parsedDate, http.RequestAborted);

            return Results.Ok(result);
        })
        .WithName("GetFlightStatus");
    }
}
