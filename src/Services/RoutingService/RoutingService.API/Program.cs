using Microsoft.EntityFrameworkCore;
using TechnicalService.Api.Extensions;
using TechnicalService.Api.Middleware;
using TechnicalService.API.GrpcServices;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// ServiceDefaults
builder.AddServiceDefaults();

// Memory Cache
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
});

// Redis
builder.AddRedisClient("redis");

// Application Services
builder.Services.AddApplicationServices(builder.Configuration);

// gRPC
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseAuthorization();

// gRPC endpoint
app.MapGrpcService<TechnicalGrpcServiceImpl>();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();