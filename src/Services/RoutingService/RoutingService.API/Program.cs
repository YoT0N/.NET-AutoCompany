using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RoutingService.API.Extensions;
using RoutingService.API.Middleware;
using RoutingService.Bll.Interfaces;
using RoutingService.Bll.Mapping;
using RoutingService.Bll.Services;
using RoutingService.Dal.Data;
using RoutingService.Dal.Data.Seeding;
using RoutingService.Dal.Repositories;
using RoutingService.Domain.Repositories;
using Serilog;
using Serilog.Events;
using ServiceDefaults;

// Видалимо власну конфігурацію Serilog, ServiceDefaults це зробить
var builder = WebApplication.CreateBuilder(args);

// Додати ServiceDefaults на початку
builder.AddServiceDefaults();

// Add services using extension methods
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

await app.ApplyMigrationsAndSeedAsync();

// Додати CorrelationId middleware
app.UseCorrelationId();

app.ConfigureMiddleware();

// Додати default endpoints
app.MapDefaultEndpoints();

await app.RunAsync();