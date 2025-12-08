using PersonnelService.Application;
using PersonnelService.Infrastructure;
using PersonnelService.Infrastructure.Context;
using PersonnelService.API.Middlewares;
using PersonnelService.API.GrpcServices;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// ServiceDefaults
builder.AddServiceDefaults();

// Memory Cache
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
});

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Redis
builder.AddRedisClient("redis");

// gRPC
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

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
              .AllowAnyHeader()
              .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

var app = builder.Build();

// Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var seedManager = scope.ServiceProvider.GetRequiredService<SeedManager>();
    await seedManager.SeedAllAsync();
}

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors("AllowAll");
app.UseAuthorization();

// gRPC endpoint
app.MapGrpcService<PersonnelGrpcServiceImpl>();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();