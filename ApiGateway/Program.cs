using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// CorrelationId middleware - Ï‡∫ ·ÛÚË œ≈–≈ƒ MapReverseProxy
app.UseCorrelationId();

app.MapReverseProxy();

app.MapDefaultEndpoints();

app.Run();