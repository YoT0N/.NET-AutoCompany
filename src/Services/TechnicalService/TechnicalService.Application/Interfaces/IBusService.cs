using TechnicalService.Bll.DTOs.Bus;

namespace TechnicalService.Bll.Interfaces;

public interface IBusService
{
    Task<IEnumerable<BusDto>> GetAllBusesAsync(CancellationToken cancellationToken = default);
    Task<BusDto> GetBusByCountryNumberAsync(string countryNumber, CancellationToken cancellationToken = default);
    Task<BusDto> GetBusWithStatusAsync(string countryNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusDto>> GetBusesByStatusAsync(int statusId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusDto>> GetActiveBusesAsync(CancellationToken cancellationToken = default);
    Task<string> CreateBusAsync(CreateBusDto createBusDto, CancellationToken cancellationToken = default);
    Task UpdateBusAsync(string countryNumber, UpdateBusDto updateBusDto, CancellationToken cancellationToken = default);
    Task DeleteBusAsync(string countryNumber, CancellationToken cancellationToken = default);
    Task UpdateBusStatusAsync(string countryNumber, int newStatusId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalMileageAsync(string countryNumber, CancellationToken cancellationToken = default);
}