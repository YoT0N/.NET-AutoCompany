using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using RoutingService.Bll.Interfaces;
using RoutingService.Grpc;
using System.Text.Json;

namespace RoutingService.API.Services;

public class RoutingGrpcService : Grpc.RoutingGrpcService.RoutingGrpcServiceBase
{
    private readonly IRouteService _routeService;
    private readonly IRouteSheetService _routeSheetService;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RoutingGrpcService> _logger;

    public RoutingGrpcService(
        IRouteService routeService,
        IRouteSheetService routeSheetService,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<RoutingGrpcService> logger)
    {
        _routeService = routeService;
        _routeSheetService = routeSheetService;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public override async Task<RouteResponse> GetRouteById(
        GetRouteByIdRequest request,
        ServerCallContext context)
    {
        var cacheKey = $"route:{request.RouteId}";

        try
        {
            // L1 Cache - Memory
            if (_memoryCache.TryGetValue(cacheKey, out RouteResponse? cachedRoute))
            {
                _logger.LogInformation("Cache HIT (Memory) for route {RouteId}", request.RouteId);
                return cachedRoute!;
            }

            // L2 Cache - Redis
            var redisCached = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(redisCached))
            {
                _logger.LogInformation("Cache HIT (Redis) for route {RouteId}", request.RouteId);
                var route = JsonSerializer.Deserialize<RouteResponse>(redisCached);

                // Зберегти в L1
                _memoryCache.Set(cacheKey, route, TimeSpan.FromMinutes(5));

                return route!;
            }

            // Cache MISS - отримати з БД
            _logger.LogInformation("Cache MISS for route {RouteId}", request.RouteId);
            var routeDto = await _routeService.GetRouteByIdAsync(request.RouteId);

            var response = new RouteResponse
            {
                RouteId = routeDto.RouteId,
                RouteNumber = routeDto.RouteNumber,
                Name = routeDto.Name,
                DistanceKm = (double)routeDto.DistanceKm,
                CreatedAt = routeDto.CreatedAt.ToString("O"),
                UpdatedAt = routeDto.UpdatedAt.ToString("O")
            };

            // Зберегти в обох кешах
            _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

            var serialized = JsonSerializer.Serialize(response);
            await _distributedCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route {RouteId}", request.RouteId);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<RoutesListResponse> GetAllRoutes(
        GetAllRoutesRequest request,
        ServerCallContext context)
    {
        const string cacheKey = "routes:all";

        try
        {
            // Перевірити Memory Cache
            if (_memoryCache.TryGetValue(cacheKey, out RoutesListResponse? cachedRoutes))
            {
                _logger.LogInformation("Cache HIT (Memory) for all routes");
                return cachedRoutes!;
            }

            // Перевірити Redis
            var redisCached = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(redisCached))
            {
                _logger.LogInformation("Cache HIT (Redis) for all routes");
                var routes = JsonSerializer.Deserialize<RoutesListResponse>(redisCached);

                _memoryCache.Set(cacheKey, routes, TimeSpan.FromMinutes(3));

                return routes!;
            }

            // Cache MISS
            _logger.LogInformation("Cache MISS for all routes");
            var routeDtos = await _routeService.GetAllRoutesAsync();

            var response = new RoutesListResponse();
            foreach (var dto in routeDtos)
            {
                response.Routes.Add(new RouteResponse
                {
                    RouteId = dto.RouteId,
                    RouteNumber = dto.RouteNumber,
                    Name = dto.Name,
                    DistanceKm = (double)dto.DistanceKm,
                    CreatedAt = dto.CreatedAt.ToString("O"),
                    UpdatedAt = dto.UpdatedAt.ToString("O")
                });
            }

            // Зберегти в кешах
            _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(3));

            var serialized = JsonSerializer.Serialize(response);
            await _distributedCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all routes");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<RouteSheetsListResponse> GetRouteSheetsByBus(
        GetRouteSheetsByBusRequest request,
        ServerCallContext context)
    {
        var cacheKey = $"routesheets:bus:{request.BusId}";

        try
        {
            // L1 Cache
            if (_memoryCache.TryGetValue(cacheKey, out RouteSheetsListResponse? cached))
            {
                _logger.LogInformation("Cache HIT (Memory) for bus {BusId} route sheets", request.BusId);
                return cached!;
            }

            // L2 Cache
            var redisCached = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(redisCached))
            {
                _logger.LogInformation("Cache HIT (Redis) for bus {BusId} route sheets", request.BusId);
                var sheets = JsonSerializer.Deserialize<RouteSheetsListResponse>(redisCached);

                _memoryCache.Set(cacheKey, sheets, TimeSpan.FromMinutes(5));

                return sheets!;
            }

            // Cache MISS
            _logger.LogInformation("Cache MISS for bus {BusId} route sheets", request.BusId);
            var sheetDtos = await _routeSheetService.GetRouteSheetsByBusAsync(request.BusId);

            var response = new RouteSheetsListResponse();
            foreach (var dto in sheetDtos)
            {
                response.RouteSheets.Add(new RouteSheetResponse
                {
                    SheetId = dto.SheetId,
                    RouteId = dto.RouteId,
                    BusId = dto.BusId,
                    SheetDate = dto.SheetDate.ToString("O"),
                    RouteNumber = dto.RouteNumber,
                    RouteName = dto.RouteName,
                    BusCountryNumber = dto.BusCountryNumber
                });
            }

            // Зберегти в кешах
            _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

            var serialized = JsonSerializer.Serialize(response);
            await _distributedCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route sheets for bus {BusId}", request.BusId);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}