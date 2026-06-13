using System.Net;
using FlightStatus.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace FlightStatus.Tests.Middleware;

/// <summary>
/// Tests for the error handling middleware ensuring exceptions are converted to 500 responses.
/// </summary>
public class ErrorHandlingMiddlewareTests
{
    /// <summary>
    /// When downstream throws an exception, middleware should return 500 with generic error message.
    /// </summary>
    [Fact]
    public async Task Exception_ReturnsInternalServerError()
    {
        // Arrange
        var builder = new WebHostBuilder().Configure(app =>
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.Run(context => throw new InvalidOperationException("boom"));
        });

        using var server = new TestServer(builder);
        var client = server.CreateClient();

        // Act
        var res = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
        var content = await res.Content.ReadAsStringAsync();
        Assert.Contains("An unexpected error occurred", content);
    }
}
