using RoutingService.API.Extensions;
using RoutingService.API.Services;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Додати Service Defaults (OpenTelemetry, Health Checks, Serilog)
builder.AddServiceDefaults();

// Налаштувати інфраструктуру (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Додати сервіси додатку (Business Logic Layer)
builder.Services.AddApplicationServices();

// Додати API сервіси (Controllers, Swagger, Validation, CORS)
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

// Додати кешування
builder.Services.AddMemoryCache();

// Додати Redis Distributed Cache через Aspire
builder.AddRedisDistributedCache("redis");

// Додати gRPC
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

// Налаштувати Kestrel для HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoint для REST API
    options.ListenAnyIP(5062, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);

    // HTTP/2 endpoint для gRPC
    options.ListenAnyIP(5063, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

var app = builder.Build();

// Застосувати міграції та seed даних
await app.ApplyMigrationsAndSeedAsync();

// Налаштувати middleware pipeline
app.ConfigureMiddleware();

// Використати CorrelationId middleware
app.UseCorrelationId();

// Map default endpoints (health checks)
app.MapDefaultEndpoints();

// Map gRPC service
app.MapGrpcService<RoutingGrpcService>();

// gRPC reflection для тестування (тільки в Development)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();