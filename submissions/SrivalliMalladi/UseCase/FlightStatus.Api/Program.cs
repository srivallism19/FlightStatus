using FlightStatus.Api.Endpoints;
using FlightStatus.Api.Interfaces;
using FlightStatus.Api.Middleware;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

/// <summary>
/// Program entry for the FlightStatus API. Configures the web application, services, middleware, and endpoints.
/// </summary>
public partial class Program
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure logging
        builder.Services.AddLogging();

        // Configure System.Text.Json options for minimal API and Http responses.
        // We add JsonStringEnumConverter so enum values can be serialized/deserialized as strings (e.g. "Delayed").
        builder.Services.ConfigureHttpJsonOptions(opts =>
        {
            // Allow case-insensitive property name matching (helps tolerant parsing of properties)
            opts.SerializerOptions.PropertyNameCaseInsensitive = true;

            // Add general string enum converter
            opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions);

        // Register services and providers for FlightStatus
        AddFlightStatusServices(builder.Services);

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlightStatus API", Version = "v1" });

            // Include XML comments from generated XML file if present
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        // Configure CORS to allow any origin, method, and header
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Enable swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightStatus API v1");
            c.RoutePrefix = string.Empty; // serve swagger at app root
        });

        // Use global error handling
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // Optional middleware
        app.UseHttpsRedirection();

        // Enable CORS
        app.UseCors();

        // Map endpoints (ensure they are discoverable by Swagger)
        app.MapHealthEndpoints();
        app.MapFlightStatusEndpoints();

        app.Run();
    }

    /// <summary>
    /// Register providers and services required by the FlightStatus API.
    /// </summary>
    /// <param name="services">Service collection to register into.</param>
    public static void AddFlightStatusServices(IServiceCollection services)
    {
        // Stub data loader for reading JSON fixtures
        services.AddSingleton<StubDataLoader>();

        services.AddScoped<IFlightStatusProvider, AeroTrackProvider>();

        services.AddScoped<IFlightStatusProvider, QuickFlightProvider>();

        services.AddScoped<IFlightStatusService, FlightStatusService>();
    }
}
