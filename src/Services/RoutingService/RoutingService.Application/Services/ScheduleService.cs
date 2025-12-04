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
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _unitOfWork.Schedules.GetAllAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);

            if (schedule == null)
                throw new EntityNotFoundException(nameof(Schedule), id);

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<ScheduleWithRouteDto>> GetSchedulesWithRouteInfoAsync()
        {
            var schedules = await _unitOfWork.Schedules
                .Query()
                .Include(s => s.Route)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ScheduleWithRouteDto>>(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByRouteAsync(int routeId)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == routeId);
            if (!routeExists)
                throw new EntityNotFoundException(nameof(Route), routeId);

            var schedules = await _unitOfWork.Schedules
                .FindAsync(s => s.RouteId == routeId);

            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<PagedResultDto<ScheduleWithRouteDto>> GetSchedulesPagedAsync(ScheduleFilterParameters parameters)
        {
            IQueryable<Schedule> query = _unitOfWork.Schedules
                .Query()
                .Include(s => s.Route);

            if (parameters.RouteId.HasValue)
            {
                query = query.Where(s => s.RouteId == parameters.RouteId.Value);
            }

            if (parameters.StartTime.HasValue)
            {
                query = query.Where(s => s.DepartureTime >= parameters.StartTime.Value);
            }

            if (parameters.EndTime.HasValue)
            {
                query = query.Where(s => s.DepartureTime <= parameters.EndTime.Value);
            }

            query = parameters.SortBy?.ToLower() switch
            {
                "route" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.Route.RouteNumber)
                    : query.OrderBy(s => s.Route.RouteNumber),
                "departure" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.DepartureTime)
                    : query.OrderBy(s => s.DepartureTime),
                "arrival" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(s => s.ArrivalTime)
                    : query.OrderBy(s => s.ArrivalTime),
                _ => query.OrderBy(s => s.DepartureTime)
            };

            var totalCount = await query.CountAsync();

            var schedules = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var scheduleDtos = _mapper.Map<IEnumerable<ScheduleWithRouteDto>>(schedules);

            return new PagedResultDto<ScheduleWithRouteDto>(
                scheduleDtos,
                parameters.Page,
                parameters.PageSize,
                totalCount);
        }

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId);
            if (!routeExists)
                throw new EntityNotFoundException(nameof(Route), dto.RouteId);

            if (dto.ArrivalTime <= dto.DepartureTime)
            {
                throw new ValidationException("Schedule",
                    new Dictionary<string, string[]>
                    {
                        { "ArrivalTime", new[] { "Arrival time must be after departure time" } }
                    });
            }

            var schedule = _mapper.Map<Schedule>(dto);

            await _unitOfWork.Schedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto dto)
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null)
                throw new EntityNotFoundException(nameof(Schedule), id);

            var newDeparture = dto.DepartureTime ?? schedule.DepartureTime;
            var newArrival = dto.ArrivalTime ?? schedule.ArrivalTime;

            if (newArrival <= newDeparture)
            {
                throw new ValidationException("Schedule",
                    new Dictionary<string, string[]>
                    {
                        { "ArrivalTime", new[] { "Arrival time must be after departure time" } }
                    });
            }

            _mapper.Map(dto, schedule);

            _unitOfWork.Schedules.Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null)
                throw new EntityNotFoundException(nameof(Schedule), id);

            _unitOfWork.Schedules.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}