using TechnicalService.Core.DTOs;

namespace TechnicalService.Application.Interfaces;

public interface IBusService
{
    Task<IEnumerable<BusDto>> GetAllBusesAsync();
    Task<BusDto?> GetBusByCountryNumberAsync(string countryNumber);
    Task<BusDto?> GetBusWithStatusAsync(string countryNumber);
    Task<IEnumerable<BusDto>> GetBusesByStatusAsync(int statusId);
    Task<IEnumerable<BusDto>> GetActiveBusesAsync();
    Task<int> CreateBusAsync(CreateBusDto createBusDto);
    Task<int> UpdateBusAsync(string countryNumber, UpdateBusDto updateBusDto);
    Task<int> DeleteBusAsync(string countryNumber);
    Task<int> UpdateBusStatusAsync(string countryNumber, int newStatusId);
    Task<decimal> GetTotalMileageAsync(string countryNumber);
}