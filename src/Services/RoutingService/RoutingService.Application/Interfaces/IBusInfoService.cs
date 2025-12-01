using System.Collections.Generic;
using System.Threading.Tasks;
using RoutingService.Core.DTOs;

namespace RoutingService.Bll.Interfaces
{
    public interface IBusInfoService
    {
        Task<IEnumerable<BusInfoDto>> GetAllBusesAsync();
        Task<BusInfoDto?> GetBusByIdAsync(int id);
        Task<BusInfoDto?> GetBusByCountryNumberAsync(string countryNumber);
        Task<BusInfoDto> CreateBusAsync(CreateBusInfoDto dto);
        Task<BusInfoDto?> UpdateBusAsync(int id, UpdateBusInfoDto dto);
        Task<bool> DeleteBusAsync(int id);
        Task<bool> BusExistsAsync(int id);
    }
}