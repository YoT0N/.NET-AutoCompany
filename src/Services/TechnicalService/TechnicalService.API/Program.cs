using TechnicalService.Api.Extensions;
using TechnicalService.Api.Middleware;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Додати ServiceDefaults на початку
builder.AddServiceDefaults();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application Services з конфігурацією
builder.Services.AddApplicationServices(builder.Configuration);

// Configure Health Checks
builder.Services.AddHealthChecks();

// Configure CORS
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Додати CorrelationId middleware
app.UseCorrelationId();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Додати default endpoints
app.MapDefaultEndpoints();
app.MapControllers();

app.Run();