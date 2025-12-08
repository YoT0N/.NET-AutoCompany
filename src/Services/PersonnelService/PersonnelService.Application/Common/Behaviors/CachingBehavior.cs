using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PersonnelService.Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
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

            // L1 Cache (Memory) - перевірка
            if (_memoryCache.TryGetValue(cacheKey, out TResponse? cachedValue))
            {
                _logger.LogInformation("L1 Cache HIT for {CacheKey}", cacheKey);
                return cachedValue!;
            }

            _logger.LogInformation("L1 Cache MISS for {CacheKey}", cacheKey);

            // L2 Cache (Redis) - перевірка
            try
            {
                var cachedBytes = await _distributedCache.GetAsync(cacheKey, cancellationToken);
                if (cachedBytes != null)
                {
                    var cachedJson = System.Text.Encoding.UTF8.GetString(cachedBytes);
                    var distributedValue = JsonSerializer.Deserialize<TResponse>(cachedJson);

                    if (distributedValue != null)
                    {
                        _logger.LogInformation("L2 Cache (Redis) HIT for {CacheKey}", cacheKey);

                        // Записуємо в L1 для наступних запитів
                        var l1Options = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheableRequest.CacheDurationMinutes / 2),
                            Size = 1
                        };
                        _memoryCache.Set(cacheKey, distributedValue, l1Options);

                        return distributedValue;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis cache error for {CacheKey}, falling back to database", cacheKey);
            }

            _logger.LogInformation("L2 Cache (Redis) MISS for {CacheKey}", cacheKey);

            // Database query
            var response = await next();

            // Записуємо в обидва рівні кешу
            try
            {
                // L2 (Redis) - довший TTL
                var jsonString = JsonSerializer.Serialize(response);
                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                var distributedOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheableRequest.CacheDurationMinutes)
                };
                await _distributedCache.SetAsync(cacheKey, bytes, distributedOptions, cancellationToken);

                _logger.LogInformation("L2 Cache (Redis) SET for {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set Redis cache for {CacheKey}", cacheKey);
            }

            // L1 (Memory) - коротший TTL
            var memoryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheableRequest.CacheDurationMinutes / 2),
                Size = 1
            };
            _memoryCache.Set(cacheKey, response, memoryOptions);

            _logger.LogInformation("L1 Cache (Memory) SET for {CacheKey}", cacheKey);

            return response;
        }
    }

    public interface ICacheableQuery
    {
        string CacheKey { get; }
        int CacheDurationMinutes { get; }
    }
}