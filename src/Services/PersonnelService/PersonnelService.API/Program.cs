using PersonnelService.API.Middlewares;
using PersonnelService.Application;
using PersonnelService.Infrastructure;
using PersonnelService.Infrastructure.Context;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Додаємо ServiceDefaults на самому початку
builder.AddServiceDefaults();

// Додаємо HttpContextAccessor для CorrelationId
builder.Services.AddHttpContextAccessor();

// Додаємо шари застосунку
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Додаємо контролери
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware для обробки винятків
app.UseMiddleware<GlobalExceptionMiddleware>();

// CorrelationId middleware
app.UseCorrelationId();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Мапимо health check endpoints
app.MapDefaultEndpoints();

// Ініціалізація бази даних та seed
using (var scope = app.Services.CreateScope())
{
    var seedManager = scope.ServiceProvider.GetRequiredService<SeedManager>();
    await seedManager.SeedAllAsync();
}

app.Run();