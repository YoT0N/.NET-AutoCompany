using AggregatorService.Clients;
using AggregatorService.Services;
using ServiceDefaults;
using RoutingService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults (OpenTelemetry, Health Checks, Serilog)
builder.AddServiceDefaults();

// Add HttpContextAccessor for CorrelationId
builder.Services.AddHttpContextAccessor();

// Memory Cache (L1)
builder.Services.AddMemoryCache();

// Redis Distributed Cache (L2) - через Aspire
builder.AddRedisClient("redis");

// Додаємо IDistributedCache реалізацію через StackExchange.Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = builder.Configuration.GetConnectionString("redis");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        options.Configuration = redisConnectionString;
    }
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HttpClients with CorrelationIdDelegatingHandler
builder.Services.AddHttpClient<TechnicalServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://technicalservice-api");
})
.AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

builder.Services.AddHttpClient<PersonnelServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://personnel-api");
})
.AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

builder.Services.AddGrpcClient<RoutingGrpcService.RoutingGrpcServiceClient>(options =>
{
    options.Address = new Uri("http://routing-api");
});

builder.Services.AddScoped<RoutingGrpcClient>();

builder.Services.AddScoped<IAggregatorService, AggregatorService.Services.AggregatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCorrelationId();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();