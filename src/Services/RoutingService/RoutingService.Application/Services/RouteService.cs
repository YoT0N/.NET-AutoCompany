using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;
using RoutingService.Core.Entities;
using RoutingService.Core.Interfaces;

namespace RoutingService.Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RouteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _unitOfWork.Routes.GetAllAsync();
            return routes.Select(MapToDto);
        }

        public async Task<RouteDto?> GetRouteByIdAsync(int id)
        {
            var route = await _unitOfWork.Routes.GetByIdAsync(id);
            return route != null ? MapToDto(route) : null;
        }

        public async Task<RouteWithStopsDto?> GetRouteWithStopsAsync(int id)
        {
            var route = await _unitOfWork.Routes.Query()
                .Include(r => r.RouteStopAssignments)
                .ThenInclude(rsa => rsa.RouteStop)
                .FirstOrDefaultAsync(r => r.RouteId == id);

            if (route == null) return null;

            var dto = new RouteWithStopsDto
            {
                RouteId = route.RouteId,
                RouteNumber = route.RouteNumber,
                Name = route.Name,
                DistanceKm = route.DistanceKm,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt,
                Stops = route.RouteStopAssignments
                    .OrderBy(rsa => rsa.StopOrder)
                    .Select(rsa => new RouteStopInfoDto
                    {
                        StopId = rsa.StopId,
                        StopName = rsa.RouteStop.StopName,
                        StopOrder = rsa.StopOrder,
                        Latitude = rsa.RouteStop.Latitude,
                        Longitude = rsa.RouteStop.Longitude
                    })
                    .ToList()
            };

            return dto;
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto dto)
        {
            var route = new Route
            {
                RouteNumber = dto.RouteNumber,
                Name = dto.Name,
                DistanceKm = dto.DistanceKm,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Routes.AddAsync(route);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(route);
        }

        public async Task<RouteDto?> UpdateRouteAsync(int id, UpdateRouteDto dto)
        {
            var route = await _unitOfWork.Routes.GetByIdAsync(id);
            if (route == null) return null;

            if (dto.RouteNumber != null) route.RouteNumber = dto.RouteNumber;
            if (dto.Name != null) route.Name = dto.Name;
            if (dto.DistanceKm.HasValue) route.DistanceKm = dto.DistanceKm.Value;

            route.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Routes.Update(route);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(route);
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var route = await _unitOfWork.Routes.GetByIdAsync(id);
            if (route == null) return false;

            _unitOfWork.Routes.Delete(route);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RouteExistsAsync(int id)
        {
            return await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == id);
        }

        private static RouteDto MapToDto(Route route)
        {
            return new RouteDto
            {
                RouteId = route.RouteId,
                RouteNumber = route.RouteNumber,
                Name = route.Name,
                DistanceKm = route.DistanceKm,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt
            };
        }
    }
}