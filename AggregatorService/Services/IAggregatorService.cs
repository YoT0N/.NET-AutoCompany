using AggregatorService.DTOs;

namespace AggregatorService.Services;

public interface IAggregatorService
{
    Task<BusFullInfoDto?> GetBusFullInfoAsync(
        string countryNumber,
        CancellationToken cancellationToken = default);

    Task<PersonnelFullInfoDto?> GetPersonnelFullInfoAsync(
        int personnelId,
        CancellationToken cancellationToken = default);

    Task<DashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default);
}