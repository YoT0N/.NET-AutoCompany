using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RoutingService.Bll.Interfaces;
using RoutingService.Bll.Mapping;
using RoutingService.Bll.Services;
using RoutingService.Dal.Data;
using RoutingService.Dal.Data.Seeding;
using RoutingService.Dal.Repositories;
using RoutingService.Domain.Repositories;
using System.Reflection;

namespace RoutingService.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Application/Business Logic Layer services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Register Application Services (BLL)
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRouteStopService, RouteStopService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IBusInfoService, BusInfoService>();
            services.AddScoped<IRouteSheetService, RouteSheetService>();
            services.AddScoped<ITripService, TripService>();

            return services;
        }

        /// <summary>
        /// Register Infrastructure/Data Access Layer services
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure DbContext
            services.AddDbContext<RoutingDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 22)),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    })
                    .EnableSensitiveDataLogging(false)
                    .EnableDetailedErrors(true);
            });

            // Register Unit of Work and Generic Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register Specific Repositories
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IRouteSheetRepository, RouteSheetRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IBusInfoRepository, BusInfoRepository>();
            services.AddScoped<IRouteStopRepository, RouteStopRepository>();
            services.AddScoped<ITripRepository, TripRepository>();

            // Register Database Seeder
            services.AddScoped<DatabaseSeeder>();

            return services;
        }

        /// <summary>
        /// Register API Layer services (Controllers, Validation, Swagger, etc.)
        /// </summary>
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            // Add Controllers with custom configuration
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Keep automatic 400 responses but customize them
                    options.SuppressModelStateInvalidFilter = false;
                });

            // Add FluentValidation
            services.AddFluentValidationAutoValidation(config =>
            {
                // Don't disable DataAnnotations - use both
                config.DisableDataAnnotationsValidation = false;
            });

            // Register all validators from the API assembly
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Add API Explorer for Swagger
            services.AddEndpointsApiExplorer();

            // Configure Swagger/OpenAPI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Routing Service API",
                    Version = "v1",
                    Description = @"RESTful API for managing bus routes, schedules, and trips. 
                    
Features:
- Comprehensive CRUD operations
- Advanced filtering and sorting
- Pagination with metadata
- Fluent validation
- Centralized error handling
- Structured logging",
                    Contact = new OpenApiContact
                    {
                        Name = "Development Team",
                        Email = "dev@routingservice.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Include XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Enable annotations
                options.EnableAnnotations();

                // Add response types documentation
                options.DescribeAllParametersInCamelCase();
            });

            // Configure CORS
            services.AddCors(options =>
            {
                // Development policy - allow all
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders(
                              "X-Pagination-Page",
                              "X-Pagination-PageSize",
                              "X-Pagination-TotalCount",
                              "X-Pagination-TotalPages");
                });

                // Production policy - restrictive
                var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
                    ?? Array.Empty<string>();

                options.AddPolicy("Production", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .WithExposedHeaders(
                              "X-Pagination-Page",
                              "X-Pagination-PageSize",
                              "X-Pagination-TotalCount",
                              "X-Pagination-TotalPages");
                });
            });

            // Add ProblemDetails support
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    // Add trace ID for debugging
                    context.ProblemDetails.Extensions["traceId"] =
                        context.HttpContext.TraceIdentifier;

                    // Add timestamp
                    context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;

                    // Add request path
                    context.ProblemDetails.Instance =
                        context.HttpContext.Request.Path;
                };
            });

            // Add Response Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Add Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<RoutingDbContext>("database");

            return services;
        }
    }
}