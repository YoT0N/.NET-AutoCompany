using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoutingService.Bll.DTOs;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.Interfaces;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Exceptions;
using RoutingService.Domain.Repositories;

namespace RoutingService.Bll.Services
{
    public class RouteStopService : IRouteStopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RouteStopService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteStopDto>> GetAllStopsAsync()
        {
            var stops = await _unitOfWork.RouteStops.GetAllAsync();
            return _mapper.Map<IEnumerable<RouteStopDto>>(stops);
        }

        public async Task<PagedResultDto<RouteStopDto>> GetStopsPagedAsync(RouteStopFilterParameters parameters)
        {
            var query = _unitOfWork.RouteStops.Query();

            if (!string.IsNullOrWhiteSpace(parameters.StopName))
            {
                query = query.Where(s => s.StopName.Contains(parameters.StopName));
            }

            if (parameters.MinLatitude.HasValue)
            {
                query = query.Where(s => s.Latitude >= parameters.MinLatitude.Value);
            }

            if (parameters.MaxLatitude.HasValue)
            {
                query = query.Where(s => s.Latitude <= parameters.MaxLatitude.Value);
            }

            if (parameters.MinLongitude.HasValue)
            {
                query = query.Where(s => s.Longitude >= parameters.MinLongitude.Value);
            }

            if (parameters.MaxLongitude.HasValue)
            {
                query = query.Where(s => s.Longitude <= parameters.MaxLongitude.Value);
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "name" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.StopName)
                    : query.OrderBy(s => s.StopName),
                "latitude" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.Latitude)
                    : query.OrderBy(s => s.Latitude),
                "longitude" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.Longitude)
                    : query.OrderBy(s => s.Longitude),
                _ => query.OrderBy(s => s.StopName)
            };

            var totalCount = await query.CountAsync();

            var stops = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var stopDtos = _mapper.Map<IEnumerable<RouteStopDto>>(stops);

            return new PagedResultDto<RouteStopDto>(
                stopDtos,
                parameters.Page,
                parameters.PageSize,
                totalCount);
        }

        public async Task<RouteStopDto?> GetStopByIdAsync(int id)
        {
            var stop = await _unitOfWork.RouteStops.GetByIdAsync(id);

            if (stop == null)
                throw new EntityNotFoundException(nameof(RouteStop), id);

            return _mapper.Map<RouteStopDto>(stop);
        }

        public async Task<RouteStopDto> CreateStopAsync(CreateRouteStopDto dto)
        {
            var stop = _mapper.Map<RouteStop>(dto);

            await _unitOfWork.RouteStops.AddAsync(stop);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RouteStopDto>(stop);
        }

        public async Task<RouteStopDto?> UpdateStopAsync(int id, UpdateRouteStopDto dto)
        {
            var stop = await _unitOfWork.RouteStops.GetByIdAsync(id);
            if (stop == null)
                throw new EntityNotFoundException(nameof(RouteStop), id);

            _mapper.Map(dto, stop);

            _unitOfWork.RouteStops.Update(stop);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RouteStopDto>(stop);
        }

        public async Task<bool> DeleteStopAsync(int id)
        {
            var stop = await _unitOfWork.RouteStops.GetByIdAsync(id);
            if (stop == null)
                throw new EntityNotFoundException(nameof(RouteStop), id);

            // Check if stop is assigned to any routes
            var hasAssignments = await _unitOfWork.RouteStopAssignments
                .Query()
                .AnyAsync(rsa => rsa.StopId == id);

            if (hasAssignments)
            {
                throw new ConflictException("RouteStop",
                    $"Cannot delete stop '{stop.StopName}' because it is assigned to one or more routes");
            }

            _unitOfWork.RouteStops.Delete(stop);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignStopToRouteAsync(AssignStopToRouteDto dto)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId);
            if (!routeExists)
                throw new EntityNotFoundException(nameof(Route), dto.RouteId);

            var stopExists = await _unitOfWork.RouteStops.ExistsAsync(s => s.StopId == dto.StopId);
            if (!stopExists)
                throw new EntityNotFoundException(nameof(RouteStop), dto.StopId);

            var existingAssignment = await _unitOfWork.RouteStopAssignments
                .Query()
                .FirstOrDefaultAsync(rsa => rsa.RouteId == dto.RouteId && rsa.StopId == dto.StopId);

            if (existingAssignment != null)
            {
                throw new ConflictException("RouteStopAssignment",
                    $"Stop {dto.StopId} is already assigned to route {dto.RouteId}");
            }

            // Check if StopOrder is already taken
            var orderExists = await _unitOfWork.RouteStopAssignments
                .Query()
                .AnyAsync(rsa => rsa.RouteId == dto.RouteId && rsa.StopOrder == dto.StopOrder);

            if (orderExists)
            {
                throw new ConflictException("RouteStopAssignment",
                    $"Stop order {dto.StopOrder} is already taken for route {dto.RouteId}");
            }

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

            if (assignment == null)
                throw new EntityNotFoundException("RouteStopAssignment", $"RouteId: {routeId}, StopId: {stopId}");

            _unitOfWork.RouteStopAssignments.Delete(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RouteStopInfoDto>> GetStopsByRouteAsync(int routeId)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == routeId);
            if (!routeExists)
                throw new EntityNotFoundException(nameof(Route), routeId);

            var assignments = await _unitOfWork.RouteStopAssignments
                .Query()
                .Include(rsa => rsa.RouteStop)
                .Where(rsa => rsa.RouteId == routeId)
                .OrderBy(rsa => rsa.StopOrder)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RouteStopInfoDto>>(assignments);
        }
    }
}