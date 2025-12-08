using AggregatorService.Clients;
using AggregatorService.Services;
using ServiceDefaults;
using RoutingService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Додати Service Defaults
builder.AddServiceDefaults();

// Додати контролери
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Додати кешування
builder.Services.AddMemoryCache();

// Додати Redis через Aspire
builder.AddRedisDistributedCache("redis");

// Додати HttpContextAccessor для CorrelationId
builder.Services.AddHttpContextAccessor();

// Зареєструвати HTTP clients (старі для інших сервісів)
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

// Зареєструвати gRPC client для RoutingService
builder.Services.AddGrpcClient<RoutingGrpcService.RoutingGrpcServiceClient>(options =>
{
    options.Address = new Uri("http://routing-api:5063"); // gRPC endpoint
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        EnableMultipleHttp2Connections = true
    };
});

// Зареєструвати gRPC client wrapper
builder.Services.AddScoped<RoutingGrpcClient>();

// Зареєструвати aggregator service
builder.Services.AddScoped<IAggregatorService, AggregatorService.Services.AggregatorService>();

var app = builder.Build();

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