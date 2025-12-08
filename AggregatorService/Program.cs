using AggregatorService.Clients;
using AggregatorService.Services;
using ServiceDefaults;
using RoutingService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// ServiceDefaults (Observability, Health Checks)
builder.AddServiceDefaults();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Memory Cache
builder.Services.AddMemoryCache();

// Redis Distributed Cache через Aspire
builder.AddRedisClient("redis");

// HTTP Clients
builder.Services.AddHttpClient<TechnicalServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://technicalservice-api");
});

builder.Services.AddHttpClient<PersonnelServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://personnel-api");
});

// gRPC Client для RoutingService
builder.Services.AddGrpcClient<RoutingGrpcService.RoutingGrpcServiceClient>(options =>
{
    options.Address = new Uri("http://routing-api");
});

// Register custom clients
builder.Services.AddScoped<RoutingGrpcClient>();

// Aggregator Service
builder.Services.AddScoped<IAggregatorService, AggregatorService.Services.AggregatorService>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCorrelationId();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();