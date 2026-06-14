using FlightStatus.Api.Enums;
using FlightStatus.Api.Interfaces;
using FlightStatus.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightStatus.Tests.Endpoints
{
    public class FlightStatusEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FlightStatusEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task MissingFlightNumber_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var res = await client.GetAsync("/api/v1/flights/status?date=2026-06-13");

            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            var body = await res.Content.ReadAsStringAsync();
            Assert.Contains("Missing flightNumber", body);
        }

        [Fact]
        public async Task MissingDate_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var res = await client.GetAsync("/api/v1/flights/status?flightNumber=AB123");

            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            var body = await res.Content.ReadAsStringAsync();
            Assert.Contains("Missing date", body);
        }

        [Fact]
        public async Task InvalidDateFormat_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var res = await client.GetAsync("/api/v1/flights/status?flightNumber=AB123&date=0601-2026");

            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            var body = await res.Content.ReadAsStringAsync();
            Assert.Contains("Invalid date format", body);
        }

        [Fact]
        public async Task ValidRequest_CallsServiceAndReturnsOk()
        {
            // Arrange: mock service
            var svcMock = new Mock<IFlightStatusService>();
            svcMock.Setup(s => s.GetStatusAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new FlightStatusResult
                   {
                       FlightNumber = "AB123",
                       Date = DateOnly.Parse("2026-06-13"),
                       Status = UnifiedStatus.OnTime,
                       SourceProvider = "TestProvider"
                   });

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IFlightStatusService>(svcMock.Object);
                });
            });

            var client = factory.CreateClient();

            // Act
            var res = await client.GetAsync("/api/v1/flights/status?flightNumber=AB123&date=2026-06-13");

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var result = await res.Content.ReadFromJsonAsync<FlightStatusResult>(options);
            Assert.NotNull(result);
            Assert.Equal("AB123", result!.FlightNumber);
            Assert.Equal(UnifiedStatus.OnTime, result.Status);
        }
    }
}
