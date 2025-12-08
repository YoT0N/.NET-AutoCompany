using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace PersonnelService.Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery cacheableRequest)
                return await next();

            var cacheKey = cacheableRequest.CacheKey;

            if (_cache.TryGetValue(cacheKey, out TResponse? cachedValue))
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return cachedValue!;
            }

            var response = await next();

            _logger.LogInformation("Cache set for {CacheKey}", cacheKey);
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(cacheableRequest.CacheDurationMinutes));

            return response;
        }
    }

    public interface ICacheableQuery
    {
        string CacheKey { get; }
        int CacheDurationMinutes { get; }
    }
}