using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoutingService.Core.DTOs;

namespace RoutingService.Bll.Interfaces
{
    public interface IRouteSheetService
    {
        Task<IEnumerable<RouteSheetDto>> GetAllRouteSheetsAsync();
        Task<RouteSheetDto?> GetRouteSheetByIdAsync(int id);
        Task<RouteSheetDetailsDto?> GetRouteSheetDetailsAsync(int id);
        Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByDateAsync(DateTime date);
        Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByRouteAsync(int routeId);
        Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByBusAsync(int busId);
        Task<RouteSheetDto> CreateRouteSheetAsync(CreateRouteSheetDto dto);
        Task<RouteSheetDto?> UpdateRouteSheetAsync(int id, UpdateRouteSheetDto dto);
        Task<bool> DeleteRouteSheetAsync(int id);
    }
}