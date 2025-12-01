using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IMaintenanceRepository : IRepository<BusMaintenanceHistory>
{
    Task<IEnumerable<BusMaintenanceHistory>> GetMaintenanceByBusAsync(string countryNumber, 
        CancellationToken cancellationToken = default);
    Task<decimal> GetTotalMaintenanceCostAsync(string countryNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusMaintenanceHistory>> GetUpcomingMaintenanceAsync(DateTime fromDate, 
        CancellationToken cancellationToken = default);
}