using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;
using RoutingService.Core.Entities;
using RoutingService.Domain.Interfaces.Repositories;

namespace RoutingService.Bll.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _unitOfWork.Schedules.GetAllAsync();
            return schedules.Select(MapToDto);
        }

        public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            return schedule != null ? MapToDto(schedule) : null;
        }

        public async Task<IEnumerable<ScheduleWithRouteDto>> GetSchedulesWithRouteInfoAsync()
        {
            var schedules = await _unitOfWork.Schedules
                .Query()
                .Include(s => s.Route)
                .ToListAsync();

            return schedules.Select(s => new ScheduleWithRouteDto
            {
                ScheduleId = s.ScheduleId,
                RouteId = s.RouteId,
                DepartureTime = s.DepartureTime,
                ArrivalTime = s.ArrivalTime,
                RouteNumber = s.Route.RouteNumber,
                RouteName = s.Route.Name
            });
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByRouteAsync(int routeId)
        {
            var schedules = await _unitOfWork.Schedules
                .FindAsync(s => s.RouteId == routeId);

            return schedules.Select(MapToDto);
        }

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId);
            if (!routeExists)
                throw new KeyNotFoundException($"Route with ID {dto.RouteId} not found");

            var schedule = new Schedule
            {
                RouteId = dto.RouteId,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };

            await _unitOfWork.Schedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(schedule);
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto dto)
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null) return null;

            if (dto.DepartureTime.HasValue) schedule.DepartureTime = dto.DepartureTime.Value;
            if (dto.ArrivalTime.HasValue) schedule.ArrivalTime = dto.ArrivalTime.Value;

            _unitOfWork.Schedules.Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null) return false;

            _unitOfWork.Schedules.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static ScheduleDto MapToDto(Schedule schedule)
        {
            return new ScheduleDto
            {
                ScheduleId = schedule.ScheduleId,
                RouteId = schedule.RouteId,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime
            };
        }
    }
}