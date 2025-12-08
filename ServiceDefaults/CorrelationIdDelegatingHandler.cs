using Microsoft.AspNetCore.Http;

namespace ServiceDefaults;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items[CorrelationIdHeader]?.ToString()
            ?? Guid.NewGuid().ToString();

        if (!request.Headers.Contains(CorrelationIdHeader))
        {
            request.Headers.Add(CorrelationIdHeader, correlationId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}