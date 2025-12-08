using RoutingService.API.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Додаємо ServiceDefaults
builder.AddServiceDefaults();

// Додаємо HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Реєстрація сервісів
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// Міграції та seed
await app.ApplyMigrationsAndSeedAsync();

// CorrelationId middleware
app.UseCorrelationId();

// Конфігурація middleware
app.ConfigureMiddleware();

// Мапимо health check endpoints
app.MapDefaultEndpoints();

app.Run();