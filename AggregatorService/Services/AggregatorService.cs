using AggregatorService.Clients;
using AggregatorService.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AggregatorService.Services;

public class AggregatorService : IAggregatorService
{
    private readonly TechnicalServiceClient _technicalClient;
    private readonly RoutingGrpcClient _routingGrpcClient; // gRPC замість HTTP
    private readonly PersonnelServiceClient _personnelClient;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<AggregatorService> _logger;

    public AggregatorService(
        TechnicalServiceClient technicalClient,
        RoutingGrpcClient routingGrpcClient,
        PersonnelServiceClient personnelClient,
        IDistributedCache distributedCache,
        ILogger<AggregatorService> logger)
    {
        _technicalClient = technicalClient;
        _routingGrpcClient = routingGrpcClient;
        _personnelClient = personnelClient;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<BusFullInfoDto?> GetBusFullInfoAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"aggregated:bus:{countryNumber}";

        try
        {
            // Перевірити кеш
            var cached = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
            {
                _logger.LogInformation("Cache HIT for aggregated bus info {CountryNumber}", countryNumber);
                return JsonSerializer.Deserialize<BusFullInfoDto>(cached);
            }

            _logger.LogInformation("Cache MISS - aggregating full information for bus {CountryNumber}", countryNumber);

            // Паралельні виклики через Task.WhenAll
            var busTask = _technicalClient.GetBusAsync(countryNumber, cancellationToken);
            // Для routeSheets потрібно спочатку отримати busId

            var bus = await busTask;
            if (bus == null)
            {
                _logger.LogWarning("Bus {CountryNumber} not found", countryNumber);
                return null;
            }

            // Тепер можна отримати route sheets через gRPC (потрібно BusId)
            // Припустимо, що ми можемо отримати BusId з іншого джерела або додати поле в BusDto
            // Для спрощення, просто викликаємо метод без route sheets або додамо логіку

            var result = new BusFullInfoDto
            {
                CountryNumber = bus.CountryNumber,
                BoardingNumber = bus.BoardingNumber,
                Brand = bus.Brand,
                PassengerCapacity = bus.PassengerCapacity,
                YearOfManufacture = bus.YearOfManufacture,
                Mileage = bus.Mileage,
                CurrentStatusName = bus.CurrentStatusName,
                RouteSheets = new List<RouteSheetDto>() // Тимчасово порожній список
            };

            // Зберегти в кеш на 30 секунд
            var serialized = JsonSerializer.Serialize(result);
            await _distributedCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                },
                cancellationToken);

            _logger.LogInformation("Successfully aggregated and cached data for bus {CountryNumber}", countryNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating data for bus {CountryNumber}", countryNumber);
            throw;
        }
    }

    public async Task<PersonnelFullInfoDto?> GetPersonnelFullInfoAsync(
        int personnelId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"aggregated:personnel:{personnelId}";

        try
        {
            // Перевірити кеш
            var cached = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
            {
                _logger.LogInformation("Cache HIT for aggregated personnel info {PersonnelId}", personnelId);
                return JsonSerializer.Deserialize<PersonnelFullInfoDto>(cached);
            }

            _logger.LogInformation("Cache MISS - aggregating full information for personnel {PersonnelId}", personnelId);

            // Паралельні запити
            var personnelTask = _personnelClient.GetPersonnelByIdAsync(personnelId, cancellationToken);
            var workShiftsTask = _personnelClient.GetWorkShiftsByPersonnelAsync(personnelId, cancellationToken);

            await Task.WhenAll(personnelTask, workShiftsTask);

            var personnel = await personnelTask;
            if (personnel == null)
            {
                _logger.LogWarning("Personnel {PersonnelId} not found", personnelId);
                return null;
            }

            var workShifts = await workShiftsTask;

            var result = new PersonnelFullInfoDto
            {
                PersonnelId = personnel.PersonnelId,
                FullName = personnel.FullName,
                Position = personnel.Position,
                Status = personnel.Status,
                WorkShifts = workShifts?.ToList() ?? new List<WorkShiftDto>(),
                TotalShifts = workShifts?.Count ?? 0,
                TotalDistance = workShifts?.Sum(w => w.DistanceKm) ?? 0
            };

            // Зберегти в кеш
            var serialized = JsonSerializer.Serialize(result);
            await _distributedCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(45)
                },
                cancellationToken);

            _logger.LogInformation("Successfully aggregated data for personnel {PersonnelId}", personnelId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating data for personnel {PersonnelId}", personnelId);
            throw;
        }
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        const string cacheKey = "aggregated:dashboard";

        try
        {
            // Перевірити кеш
            var cached = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
            {
                _logger.LogInformation("Cache HIT for dashboard summary");
                return JsonSerializer.Deserialize<DashboardSummaryDto>(cached)!;
            }

            _logger.LogInformation("Cache MISS - aggregating dashboard summary data");

            // Отримати всі маршрути через gRPC
            var routesTask = _routingGrpcClient.GetAllRoutesAsync(cancellationToken);

            var routes = await routesTask;

            var summary = new DashboardSummaryDto
            {
                Timestamp = DateTime.UtcNow,
                Message = "Dashboard aggregation via gRPC",
                TotalRoutes = routes.Count,
                TotalBuses = 0, // Можна додати виклик до TechnicalService
                TotalPersonnel = 0 // Можна додати виклик до PersonnelService
            };

            // Зберегти в кеш на 60 секунд
            var serialized = JsonSerializer.Serialize(summary);
            await _distributedCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                },
                cancellationToken);

            _logger.LogInformation("Dashboard summary aggregated successfully");

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating dashboard summary");
            throw;
        }
    }
}