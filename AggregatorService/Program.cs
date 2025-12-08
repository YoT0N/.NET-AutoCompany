using AggregatorService.Clients;
using AggregatorService.Services;
using ServiceDefaults;
using RoutingService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults (OpenTelemetry, Health Checks, Serilog)
builder.AddServiceDefaults();

// Add HttpContextAccessor for CorrelationId
builder.Services.AddHttpContextAccessor();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis Distributed Cache
builder.AddRedisClient("redis");

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

// Register gRPC Client for RoutingService
builder.Services.AddGrpcClient<RoutingGrpcService.RoutingGrpcServiceClient>(options =>
{
    options.Address = new Uri("http://routing-api");
});

// Register custom clients
builder.Services.AddScoped<RoutingGrpcClient>();

// Register AggregatorService
builder.Services.AddScoped<IAggregatorService, AggregatorService.Services.AggregatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add CorrelationId middleware
app.UseCorrelationId();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();