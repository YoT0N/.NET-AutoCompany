using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonnelService.API.Middlewares;
using PersonnelService.Application;
using PersonnelService.Infrastructure;
using PersonnelService.Infrastructure.Context;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Додати ServiceDefaults на початку конфігурації
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Personnel Service API", Version = "v1" });
});

// Configure Health Checks
builder.Services.AddHealthChecks();

// Add Application Services
builder.Services.AddApplication();

// Add Infrastructure Services - використовуємо connection string від Aspire
builder.Services.AddInfrastructure(builder.Configuration);

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

// Seed database
using (var scope = app.Services.CreateScope())
{
    var seedManager = scope.ServiceProvider.GetRequiredService<SeedManager>();
    await seedManager.SeedAllAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Personnel Service API v1");
        options.RoutePrefix = "swagger";
    });
}

// Додати CorrelationId middleware
app.UseCorrelationId();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Додати default endpoints для health checks
app.MapDefaultEndpoints();
app.MapControllers();

app.Run();