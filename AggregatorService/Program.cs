using AggregatorService.Clients;
using AggregatorService.Services;
using Projects;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();

// Register Typed HttpClients with Service Discovery
builder.Services.AddHttpClient<TechnicalServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://technicalservice-api");
})
.AddStandardResilienceHandler();

builder.Services.AddHttpClient<RoutingServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://routing-api");
})
.AddStandardResilienceHandler();

builder.Services.AddHttpClient<PersonnelServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://personnel-api");
})
.AddStandardResilienceHandler();

// Register Aggregator Service
builder.Services.AddScoped<IAggregatorService, AggregatorService.Services.AggregatorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCorrelationId();
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();