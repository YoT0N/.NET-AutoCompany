using TechnicalService.Api.Extensions;
using TechnicalService.Api.Middleware;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Додаємо ServiceDefaults
builder.AddServiceDefaults();

// Додаємо HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Реєстрація сервісів
builder.Services.AddApplicationServices(builder.Configuration);

// Додаємо контролери
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware для обробки винятків
app.UseMiddleware<ExceptionHandlingMiddleware>();

// CorrelationId middleware
app.UseCorrelationId();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Мапимо health check endpoints
app.MapDefaultEndpoints();

app.Run();