using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IBusRepository : IRepository<Bus>
{
    Task<IEnumerable<Bus>> GetBusesByStatusAsync(int statusId, CancellationToken cancellationToken = default);
    Task<Bus?> GetBusWithStatusAsync(string countryNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bus>> GetActiveBusesAsync(CancellationToken cancellationToken = default);
    Task<int> UpdateBusStatusAsync(string countryNumber, int newStatusId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalMileageAsync(string countryNumber, CancellationToken cancellationToken = default);
}