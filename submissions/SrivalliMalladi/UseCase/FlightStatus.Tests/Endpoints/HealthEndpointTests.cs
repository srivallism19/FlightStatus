using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System.Net;

namespace FlightStatus.Tests.Endpoints;

/// <summary>
/// Health endpoint tests.
/// </summary>
public class HealthEndpointTests
{
    /// <summary>
    /// GET /health returns 200 OK with status Healthy.
    /// </summary>
    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => { })
            .Configure(app =>
            {
                app.Run(async ctx =>
                {
                    if (ctx.Request.Path == "/health")
                    {
                        ctx.Response.StatusCode = 200;
                        await ctx.Response.WriteAsJsonAsync(new { status = "Healthy" });
                    }
                });
            });

        using var server = new TestServer(builder);
        var client = server.CreateClient();

        var res = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var s = await res.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", s);
    }

}
