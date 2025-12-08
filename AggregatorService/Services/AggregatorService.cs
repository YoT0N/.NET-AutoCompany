using AggregatorService.Clients;
using AggregatorService.DTOs;
using Microsoft.Extensions.Logging;

namespace AggregatorService.Services;

public class AggregatorService : IAggregatorService
{
    private readonly TechnicalServiceClient _technicalClient;
    private readonly RoutingServiceClient _routingClient;
    private readonly PersonnelServiceClient _personnelClient;
    private readonly ILogger<AggregatorService> _logger;

    public AggregatorService(
        TechnicalServiceClient technicalClient,
        RoutingServiceClient routingClient,
        PersonnelServiceClient personnelClient,
        ILogger<AggregatorService> logger)
    {
        _technicalClient = technicalClient;
        _routingClient = routingClient;
        _personnelClient = personnelClient;
        _logger = logger;
    }

    public async Task<BusFullInfoDto?> GetBusFullInfoAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Aggregating full information for bus {CountryNumber}",
            countryNumber);

        try
        {
            // Паралельні запити до всіх мікросервісів
            var busTask = _technicalClient.GetBusAsync(countryNumber, cancellationToken);
            var routeSheetsTask = _routingClient.GetRouteSheetsByBusAsync(countryNumber, cancellationToken);

            await Task.WhenAll(busTask, routeSheetsTask);

            var bus = await busTask;

            if (bus == null)
            {
                _logger.LogWarning("Bus {CountryNumber} not found", countryNumber);
                return null;
            }

            var routeSheets = await routeSheetsTask;

            var result = new BusFullInfoDto
            {
                CountryNumber = bus.CountryNumber,
                BoardingNumber = bus.BoardingNumber,
                Brand = bus.Brand,
                PassengerCapacity = bus.PassengerCapacity,
                YearOfManufacture = bus.YearOfManufacture,
                Mileage = bus.Mileage,
                CurrentStatusName = bus.CurrentStatusName,
                RouteSheets = routeSheets?.ToList() ?? new List<RouteSheetDto>()
            };

            _logger.LogInformation(
                "Successfully aggregated data for bus {CountryNumber}: {RouteSheetCount} route sheets",
                countryNumber,
                result.RouteSheets.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error aggregating data for bus {CountryNumber}",
                countryNumber);
            throw;
        }
    }

    public async Task<PersonnelFullInfoDto?> GetPersonnelFullInfoAsync(
        int personnelId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Aggregating full information for personnel {PersonnelId}",
            personnelId);

        try
        {
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

            _logger.LogInformation(
                "Successfully aggregated data for personnel {PersonnelId}: {ShiftCount} shifts, {TotalDistance} km",
                personnelId,
                result.TotalShifts,
                result.TotalDistance);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error aggregating data for personnel {PersonnelId}",
                personnelId);
            throw;
        }
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Aggregating dashboard summary data");

        try
        {
            // Можна зробити паралельні запити до всіх сервісів для загальної статистики
            // Для прикладу - базова реалізація

            var summary = new DashboardSummaryDto
            {
                Timestamp = DateTime.UtcNow,
                Message = "Dashboard aggregation - implement specific metrics as needed"
            };

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