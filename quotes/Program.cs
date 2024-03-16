using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.ResourceDetectors.Container;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure telemetry.
if (Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") != null)
{
    var assembly = Assembly.GetExecutingAssembly();
    var serviceVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    if (string.IsNullOrEmpty(serviceVersion))
    {
        throw new ApplicationException("cannot get the service version");
    }
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(
                serviceName: builder.Environment.ApplicationName,
                serviceVersion: serviceVersion)
            .AddDetector(new ContainerResourceDetector()))
        .WithMetrics(metrics => metrics
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter())
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter()
            .AddConsoleExporter());
    builder.Logging
        .AddOpenTelemetry(options => options.AddOtlpExporter());
}

builder.Logging.Configure(options =>
    {
        Console.WriteLine($"Logging ActivityTrackingOptions: {options.ActivityTrackingOptions}");
    }
);

var app = builder.Build();

app.MapHealthChecks("/healthz/ready");

// enable the swagger document endpoint and swagger ui.
app.UseSwagger();   // make the swagger available at /swagger/v1/swagger.json
app.UseSwaggerUI(); // make the swagger UI available at /swagger

app.MapControllers();

app.Run();
