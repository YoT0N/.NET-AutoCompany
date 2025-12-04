using Microsoft.EntityFrameworkCore;
using RoutingService.API.Middleware;
using RoutingService.Dal.Data;
using RoutingService.Dal.Data.Seeding;
using Serilog;

namespace RoutingService.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> ApplyMigrationsAndSeedAsync(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<RoutingDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                var configuration = services.GetRequiredService<IConfiguration>();
                var environment = services.GetRequiredService<IWebHostEnvironment>();

                // Apply migrations
                logger.LogInformation("Checking for pending database migrations...");

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation(
                        "Applying {Count} pending migrations: {Migrations}",
                        pendingMigrations.Count(),
                        string.Join(", ", pendingMigrations));

                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("Database is up to date, no migrations to apply");
                }

                // Seed data (only in Development or when explicitly configured)
                var enableSeeding = environment.IsDevelopment() ||
                                  configuration.GetValue<bool>("EnableSeeding");

                if (enableSeeding)
                {
                    logger.LogInformation("Starting database seeding...");
                    var seeder = services.GetRequiredService<DatabaseSeeder>();
                    await seeder.SeedAsync();
                    logger.LogInformation("Database seeding completed successfully");
                }
                else
                {
                    logger.LogInformation("Database seeding is disabled");
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database");

                if (services.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    throw;
                }

                logger.LogWarning("Application will continue despite database setup error");
            }

            return app;
        }

        public static IApplicationBuilder ConfigureMiddleware(this IApplicationBuilder app)
        {
            var environment = app.ApplicationServices
                .GetRequiredService<IWebHostEnvironment>();
            var configuration = app.ApplicationServices
                .GetRequiredService<IConfiguration>();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Routing Service API v1");
                    options.RoutePrefix = string.Empty; // Serve at root
                    options.DocumentTitle = "Routing Service API - Documentation";
                    options.DisplayRequestDuration();
                    options.EnableDeepLinking();
                    options.EnableFilter();
                    options.ShowExtensions();
                    options.EnableValidator();
                });
            }

            app.UseHttpsRedirection();

            app.UseResponseCompression();

            // Serilog request logging
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent",
                        httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("RemoteIpAddress",
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

                    if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID",
                        out var correlationId))
                    {
                        diagnosticContext.Set("CorrelationId", correlationId.ToString());
                    }

                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                    }
                };

                options.GetLevel = (httpContext, elapsed, ex) =>
                {
                    if (ex != null) return Serilog.Events.LogEventLevel.Error;
                    if (httpContext.Response.StatusCode >= 500)
                        return Serilog.Events.LogEventLevel.Error;
                    if (httpContext.Response.StatusCode >= 400)
                        return Serilog.Events.LogEventLevel.Warning;
                    return Serilog.Events.LogEventLevel.Information;
                };
            });

            app.UseRouting();

            var corsPolicy = environment.IsDevelopment() ? "AllowAll" : "Production";
            app.UseCors(corsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health")
                    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(
                        typeof(object), StatusCodes.Status200OK));

                endpoints.MapGet("/info", (IWebHostEnvironment env) =>
                    Results.Ok(new
                    {
                        application = "Routing Service API",
                        version = "1.0.0",
                        environment = env.EnvironmentName,
                        timestamp = DateTime.UtcNow
                    }))
                    .WithName("GetInfo")
                    .WithTags("System")
                    .Produces<object>(StatusCodes.Status200OK);
            });

            return app;
        }
    }
}