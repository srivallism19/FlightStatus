using System.Net;
using System.Text.Json;

namespace FlightStatus.Api.Middleware;

/// <summary>
/// ErrorHandlingMiddleware is a custom ASP.NET Core middleware that catches unhandled exceptions during request processing,
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Construct the ErrorHandlingMiddleware with the next RequestDelegate and a logger.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// InvokeAsync is called for each HTTP request. It wraps the next middleware in a try-catch block to handle any unhandled exceptions.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { error = "An unexpected error occurred" });
            await context.Response.WriteAsync(payload);
        }
    }
}
