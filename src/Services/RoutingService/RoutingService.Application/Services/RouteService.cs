using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoutingService.Application.DTOs.Common;
using RoutingService.Application.Interfaces;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Core.DTOs;
using RoutingService.Core.Entities;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Exceptions;
using RoutingService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RouteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _unitOfWork.Routes.GetAllAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<PagedResultDto<RouteDto>> GetRoutesPagedAsync(RouteFilterParameters parameters)
        {
            var query = _unitOfWork.Routes.Query();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(parameters.RouteNumber))
            {
                query = query.Where(r => r.RouteNumber.Contains(parameters.RouteNumber));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                query = query.Where(r => r.Name.Contains(parameters.Name));
            }

            if (parameters.MinDistance.HasValue)
            {
                query = query.Where(r => r.DistanceKm >= parameters.MinDistance.Value);
            }

            if (parameters.MaxDistance.HasValue)
            {
                query = query.Where(r => r.DistanceKm <= parameters.MaxDistance.Value);
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "routenumber" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(r => r.RouteNumber)
                    : query.OrderBy(r => r.RouteNumber),
                "name" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(r => r.Name)
                    : query.OrderBy(r => r.Name),
                "distance" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(r => r.DistanceKm)
                    : query.OrderBy(r => r.DistanceKm),
                _ => query.OrderBy(r => r.RouteId)
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var routes = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var routeDtos = _mapper.Map<IEnumerable<RouteDto>>(routes);

            return new PagedResultDto<RouteDto>(
                routeDtos,
                parameters.Page,
                parameters.PageSize,
                totalCount);
        }

        public async Task<RouteDto?> GetRouteByIdAsync(int id)
        {
            var route = await _unitOfWork.Routes.GetByIdAsync(id);

            if (route == null)
                throw new EntityNotFoundException(nameof(Route), id);

            return _mapper.Map<RouteDto>(route);
        }

        public async Task<RouteWithStopsDto?> GetRouteWithStopsAsync(int id)
        {
            // Use specific repository method with Eager Loading
            var route = await _unitOfWork.RouteRepository.GetRouteWithStopsAsync(id);

            if (route == null)
                throw new EntityNotFoundException(nameof(Route), id);

            return _mapper.Map<RouteWithStopsDto>(route);
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto dto)
        {
            // Check for duplicate route number
            var existingRoute = await _unitOfWork.Routes
                .FindAsync(r => r.RouteNumber == dto.RouteNumber);

            if (existingRoute.Any())
            {
                throw new ConflictException("Route",
                    $"A route with number '{dto.RouteNumber}' already exists");
            }

            var route = _mapper.Map<Route>(dto);
            route.CreatedAt = DateTime.UtcNow;
            route.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Routes.AddAsync(route);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RouteDto>(route);
        }

        public async Task<RouteDto?> UpdateRouteAsync(int id, UpdateRouteDto dto)
        {
            var route = await _unitOfWork.Routes.GetByIdAsync(id);

            if (route == null)
                throw new EntityNotFoundException(nameof(Route), id);

            // Check for duplicate route number if it's being changed
            if (dto.RouteNumber != null && dto.RouteNumber != route.RouteNumber)
            {
                var existingRoute = await _unitOfWork.Routes
                    .FindAsync(r => r.RouteNumber == dto.RouteNumber);

                if (existingRoute.Any())
                {
                    throw new ConflictException("Route",
                        $"A route with number '{dto.RouteNumber}' already exists");
                }
            }

            // Map only non-null properties
            _mapper.Map(dto, route);
            route.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Routes.Update(route);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RouteDto>(route);
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var route = await _unitOfWork.Routes.GetByIdAsync(id);

            if (route == null)
                throw new EntityNotFoundException(nameof(Route), id);

            _unitOfWork.Routes.Delete(route);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RouteExistsAsync(int id)
        {
            return await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == id);
        }
    }
}