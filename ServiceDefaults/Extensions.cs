using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

namespace ServiceDefaults;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.ConfigureSerilog();

        // Реєструємо DelegatingHandler для propagation CorrelationId
        builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: builder.Environment.ApplicationName,
                    serviceVersion: "1.0.0"))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation() // gRPC client tracing

                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnableConnectionLevelAttributes = true;
                    });

                // MongoDB instrumentation
                tracing.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");

                // gRPC server instrumentation
                tracing.AddSource("Grpc.Net.Client");
                tracing.AddSource("Grpc.AspNetCore.Server");
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(
            builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            // ВИПРАВЛЕННЯ: правильне використання OTLP exporter
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracerProvider =>
                tracerProvider.AddOtlpExporter());

            builder.Services.ConfigureOpenTelemetryMeterProvider(meterProvider =>
                meterProvider.AddOtlpExporter());
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }

    public static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
    {
        var logLevel = builder.Environment.IsDevelopment()
            ? Serilog.Events.LogEventLevel.Debug
            : Serilog.Events.LogEventLevel.Information;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(new CompactJsonFormatter())
            .CreateLogger();

        builder.Services.AddSerilog();

        return builder;
    }
}