using RoutingService.API.Extensions;
using RoutingService.API.Services;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults (OpenTelemetry, Health Checks, Serilog)
builder.AddServiceDefaults();

// Add HttpContextAccessor for CorrelationId
builder.Services.AddHttpContextAccessor();

// Add Redis for caching
builder.AddRedisClient("redis");

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add database and repositories
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add application services
builder.Services.AddApplicationServices();

// Add API services (Controllers, Swagger, etc.)
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

// Register gRPC service
builder.Services.AddGrpc();

var app = builder.Build();

// Apply migrations and seed data
await app.ApplyMigrationsAndSeedAsync();

// Configure middleware pipeline
app.ConfigureMiddleware();

// Add CorrelationId middleware
app.UseCorrelationId();

// Map gRPC service
app.MapGrpcService<RoutingGrpcService>();

app.MapDefaultEndpoints();

app.Run();