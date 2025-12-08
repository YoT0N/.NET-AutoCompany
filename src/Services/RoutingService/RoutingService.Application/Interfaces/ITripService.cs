using System.Collections.Generic;
using System.Threading.Tasks;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.DTOs;

namespace RoutingService.Bll.Interfaces
{
    public interface ITripService
    {
        Task<IEnumerable<TripDto>> GetAllTripsAsync();
        Task<TripDto?> GetTripByIdAsync(int id);
        Task<TripDetailsDto?> GetTripDetailsAsync(int id);
        Task<IEnumerable<TripDetailsDto>> GetTripsByRouteSheetAsync(int sheetId);
        Task<IEnumerable<TripDetailsDto>> GetCompletedTripsAsync();
        Task<IEnumerable<TripDetailsDto>> GetPendingTripsAsync();
        Task<TripDto> CreateTripAsync(CreateTripDto dto);
        Task<TripDto?> UpdateTripAsync(int id, UpdateTripDto dto);
        Task<bool> DeleteTripAsync(int id);
        Task<bool> MarkTripAsCompletedAsync(int id);

        Task<PagedResultDto<TripDetailsDto>> GetTripsPagedAsync(TripFilterParameters parameters);
    }
}