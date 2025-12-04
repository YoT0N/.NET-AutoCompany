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
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRouteStopService, RouteStopService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IBusInfoService, BusInfoService>();
            services.AddScoped<IRouteSheetService, RouteSheetService>();
            services.AddScoped<ITripService, TripService>();

            return services;
        }

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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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

        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            // Controllers with custom configuration
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = false;
                });

            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = false;
            });

            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddEndpointsApiExplorer();

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

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.EnableAnnotations();

                options.DescribeAllParametersInCamelCase();
            });

            // Configure CORS
            services.AddCors(options =>
            {
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

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] =
                        context.HttpContext.TraceIdentifier;

                    context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;

                    context.ProblemDetails.Instance =
                        context.HttpContext.Request.Path;
                };
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddHealthChecks()
                .AddDbContextCheck<RoutingDbContext>("database");

            return services;
        }
    }
}