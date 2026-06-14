namespace FlightStatus.Api.Endpoints;

/// <summary>
/// HealthEndpoint
/// </summary>
public static class HealthEndpoint
{
    /// <summary>
    /// MapHealthEndpoints maps a simple GET endpoint at /health that returns a 200 OK response with a JSON payload indicating the service is healthy.
    /// </summary>
    /// <param name="app"></param>
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/health", () => Results.Ok(new { status = "Healthy" }));
    }
}
