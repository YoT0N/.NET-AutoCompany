using Microsoft.EntityFrameworkCore;
using RoutingService.API.Extensions;
using RoutingService.API.Services;
using RoutingService.Dal.Data;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// ServiceDefaults
builder.AddServiceDefaults();

// Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Application Services
builder.Services.AddApplicationServices();

// API Services
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

// Memory Cache
builder.Services.AddMemoryCache();

// Redis через Aspire
builder.AddRedisClient("redis");

// gRPC Services
builder.Services.AddGrpc();

var app = builder.Build();

// Apply migrations and seed
await app.ApplyMigrationsAndSeedAsync();

// Configure middleware
app.ConfigureMiddleware();

// Map gRPC Service
app.MapGrpcService<RoutingGrpcService>();

// Enable gRPC reflection in development
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapDefaultEndpoints();

app.Run();