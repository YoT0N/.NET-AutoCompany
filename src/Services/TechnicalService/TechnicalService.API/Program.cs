using Microsoft.OpenApi.Models;
using TechnicalService.Api.Middleware;
using TechnicalService.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Конфігурація Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/technicalservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Додавання сервісів
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Technical Service API",
        Version = "v1",
        Description = "API для управління технічним обслуговуванням автобусів",
        Contact = new OpenApiContact
        {
            Name = "Technical Service Team"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Реєстрація сервісів з Extension методу
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Technical Service API v1");
    });
}

// Глобальна обробка помилок
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Запуск Technical Service API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Застосунок завершився з помилкою");
}
finally
{
    Log.CloseAndFlush();
}