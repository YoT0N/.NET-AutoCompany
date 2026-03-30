using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults (OpenTelemetry, Health Checks, Serilog)
builder.AddServiceDefaults();

// Add HttpContextAccessor for CorrelationId
builder.Services.AddHttpContextAccessor();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Add CorrelationId middleware
app.UseCorrelationId();

// Map reverse proxy
app.MapReverseProxy();

app.MapDefaultEndpoints();

app.Run();