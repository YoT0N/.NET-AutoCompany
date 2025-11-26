using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IBusRepository : IRepository<Bus>
{
    Task<IEnumerable<Bus>> GetBusesByStatusAsync(int statusId);
    Task<Bus?> GetBusWithStatusAsync(string countryNumber);
    Task<IEnumerable<Bus>> GetActiveBusesAsync();
    Task<int> UpdateBusStatusAsync(string countryNumber, int newStatusId);
    Task<decimal> GetTotalMileageAsync(string countryNumber);
}