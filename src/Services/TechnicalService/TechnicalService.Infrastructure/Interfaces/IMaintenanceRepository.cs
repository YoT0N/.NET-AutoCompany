using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IMaintenanceRepository : IRepository<BusMaintenanceHistory>
{
    Task<IEnumerable<BusMaintenanceHistory>> GetMaintenanceByBusAsync(string countryNumber);
    Task<decimal> GetTotalMaintenanceCostAsync(string countryNumber);
    Task<IEnumerable<BusMaintenanceHistory>> GetUpcomingMaintenanceAsync(DateTime fromDate);
}