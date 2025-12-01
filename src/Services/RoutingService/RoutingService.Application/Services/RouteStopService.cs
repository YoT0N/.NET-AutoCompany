using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;
using RoutingService.Core.Entities;
using RoutingService.Domain.Repositories;

namespace RoutingService.Bll.Services
{
    public class RouteStopService : IRouteStopService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RouteStopService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RouteStopDto>> GetAllStopsAsync()
        {
            var stops = await _unitOfWork.RouteStops.GetAllAsync();
            return stops.Select(MapToDto);
        }

        public async Task<RouteStopDto?> GetStopByIdAsync(int id)
        {
            var stop = await _unitOfWork.RouteStops.GetByIdAsync(id);
            return stop != null ? MapToDto(stop) : null;
        }

        public async Task<RouteStopDto> CreateStopAsync(CreateRouteStopDto dto)
        {
            var stop = new RouteStop
            {
                StopName = dto.StopName,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            await _unitOfWork.RouteStops.AddAsync(stop);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(stop);
        }

        public async Task<RouteStopDto?> UpdateStopAsync(int id, UpdateRouteStopDto dto)
        {
            var stop = await _unitOfWork.RouteStops.GetByIdAsync(id);
            if (stop == null) return null;

            if (dto.StopName != null) stop.StopName = dto.StopName;
            if (dto.Latitude.HasValue) stop.Latitude = dto.Latitude;
            if (dto.Longitude.HasValue) stop.Longitude = dto.Longitude;

            _unitOfWork.RouteStops.Update(stop);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(stop);
        }

        public async Task<bool> DeleteStopAsync(int id)
        {
            var stop = await _unitOfWork.RouteStops.GetByIdAsync(id);
            if (stop == null) return false;

            _unitOfWork.RouteStops.Delete(stop);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignStopToRouteAsync(AssignStopToRouteDto dto)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId);
            var stopExists = await _unitOfWork.RouteStops.ExistsAsync(s => s.StopId == dto.StopId);

            if (!routeExists || !stopExists) return false;

            var existingAssignment = await _unitOfWork.RouteStopAssignments
                .Query()
                .FirstOrDefaultAsync(rsa => rsa.RouteId == dto.RouteId && rsa.StopId == dto.StopId);

            if (existingAssignment != null) return false;

            var assignment = new RouteStopAssignment
            {
                RouteId = dto.RouteId,
                StopId = dto.StopId,
                StopOrder = dto.StopOrder
            };

            await _unitOfWork.RouteStopAssignments.AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveStopFromRouteAsync(int routeId, int stopId)
        {
            var assignment = await _unitOfWork.RouteStopAssignments
                .Query()
                .FirstOrDefaultAsync(rsa => rsa.RouteId == routeId && rsa.StopId == stopId);

            if (assignment == null) return false;

            _unitOfWork.RouteStopAssignments.Delete(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RouteStopInfoDto>> GetStopsByRouteAsync(int routeId)
        {
            var assignments = await _unitOfWork.RouteStopAssignments
                .Query()
                .Include(rsa => rsa.RouteStop)
                .Where(rsa => rsa.RouteId == routeId)
                .OrderBy(rsa => rsa.StopOrder)
                .ToListAsync();

            return assignments.Select(rsa => new RouteStopInfoDto
            {
                StopId = rsa.StopId,
                StopName = rsa.RouteStop.StopName,
                StopOrder = rsa.StopOrder,
                Latitude = rsa.RouteStop.Latitude,
                Longitude = rsa.RouteStop.Longitude
            });
        }

        private static RouteStopDto MapToDto(RouteStop stop)
        {
            return new RouteStopDto
            {
                StopId = stop.StopId,
                StopName = stop.StopName,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude
            };
        }
    }
}