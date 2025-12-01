using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoutingService.Application.Interfaces;
using RoutingService.Application.Services;
using RoutingService.Domain.Interfaces.Repositories;
using RoutingService.Infrastructure.Data;
using RoutingService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<RoutingDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 22))
    )
);
// Register repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register application services
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IRouteStopService, RouteStopService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IBusInfoService, BusInfoService>();
builder.Services.AddScoped<IRouteSheetService, RouteSheetService>();
builder.Services.AddScoped<ITripService, TripService>();

// Configure CORS (optional, adjust as needed)
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

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();