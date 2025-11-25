using PersonnelService.Application.Interfaces;
using PersonnelService.Core.DTOs;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;

namespace PersonnelService.Application.Services
{
    public class WorkShiftService : IWorkShiftService
    {
        private readonly IWorkShiftRepository _workShiftRepository;

        public WorkShiftService(IWorkShiftRepository workShiftRepository)
        {
            _workShiftRepository = workShiftRepository;
        }

        public async Task<IEnumerable<WorkShiftDto>> GetAllWorkShiftsAsync()
        {
            var workShifts = await _workShiftRepository.GetAllAsync();
            return workShifts.Select(MapToDto);
        }

        public async Task<WorkShiftDto?> GetWorkShiftByIdAsync(string id)
        {
            var workShift = await _workShiftRepository.GetByIdAsync(id);
            return workShift != null ? MapToDto(workShift) : null;
        }

        public async Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByPersonnelIdAsync(int personnelId)
        {
            var workShifts = await _workShiftRepository.GetByPersonnelIdAsync(personnelId);
            return workShifts.Select(MapToDto);
        }

        public async Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var workShifts = await _workShiftRepository.GetByDateRangeAsync(startDate, endDate);
            return workShifts.Select(MapToDto);
        }

        public async Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByPersonnelAndDateRangeAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            var workShifts = await _workShiftRepository.GetByPersonnelAndDateRangeAsync(personnelId, startDate, endDate);
            return workShifts.Select(MapToDto);
        }

        public async Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByBusNumberAsync(string busCountryNumber)
        {
            var workShifts = await _workShiftRepository.GetByBusNumberAsync(busCountryNumber);
            return workShifts.Select(MapToDto);
        }

        public async Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByRouteNumberAsync(string routeNumber)
        {
            var workShifts = await _workShiftRepository.GetByRouteNumberAsync(routeNumber);
            return workShifts.Select(MapToDto);
        }

        public async Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByStatusAsync(string status)
        {
            var workShifts = await _workShiftRepository.GetByStatusAsync(status);
            return workShifts.Select(MapToDto);
        }

        public async Task<WorkShiftDto> CreateWorkShiftAsync(CreateWorkShiftDto createDto)
        {
            var workShift = new WorkShiftLog
            {
                PersonnelId = createDto.PersonnelId,
                ShiftDate = createDto.ShiftDate,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                Bus = new BusInfo
                {
                    BusCountryNumber = createDto.Bus.BusCountryNumber,
                    Brand = createDto.Bus.Brand
                },
                Route = new RouteInfo
                {
                    RouteNumber = createDto.Route.RouteNumber,
                    DistanceKm = createDto.Route.DistanceKm
                },
                Status = createDto.Status
            };

            var created = await _workShiftRepository.CreateAsync(workShift);
            return MapToDto(created);
        }

        public async Task<bool> UpdateWorkShiftAsync(string id, UpdateWorkShiftDto updateDto)
        {
            var existing = await _workShiftRepository.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.ShiftDate = updateDto.ShiftDate;
            existing.StartTime = updateDto.StartTime;
            existing.EndTime = updateDto.EndTime;
            existing.Bus = new BusInfo
            {
                BusCountryNumber = updateDto.Bus.BusCountryNumber,
                Brand = updateDto.Bus.Brand
            };
            existing.Route = new RouteInfo
            {
                RouteNumber = updateDto.Route.RouteNumber,
                DistanceKm = updateDto.Route.DistanceKm
            };
            existing.Status = updateDto.Status;

            return await _workShiftRepository.UpdateAsync(id, existing);
        }

        public async Task<bool> DeleteWorkShiftAsync(string id)
        {
            return await _workShiftRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteWorkShiftsByPersonnelIdAsync(int personnelId)
        {
            return await _workShiftRepository.DeleteByPersonnelIdAsync(personnelId);
        }

        public async Task<bool> UpdateWorkShiftStatusAsync(string id, string status)
        {
            return await _workShiftRepository.UpdateStatusAsync(id, status);
        }

        public async Task<double> GetTotalDistanceByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            return await _workShiftRepository.GetTotalDistanceByPersonnelAsync(personnelId, startDate, endDate);
        }

        public async Task<int> GetShiftCountByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            return await _workShiftRepository.GetShiftCountByPersonnelAsync(personnelId, startDate, endDate);
        }

        private WorkShiftDto MapToDto(WorkShiftLog workShift)
        {
            return new WorkShiftDto
            {
                Id = workShift.Id,
                PersonnelId = workShift.PersonnelId,
                ShiftDate = workShift.ShiftDate,
                StartTime = workShift.StartTime,
                EndTime = workShift.EndTime,
                Bus = new BusInfoDto
                {
                    BusCountryNumber = workShift.Bus.BusCountryNumber,
                    Brand = workShift.Bus.Brand
                },
                Route = new RouteInfoDto
                {
                    RouteNumber = workShift.Route.RouteNumber,
                    DistanceKm = workShift.Route.DistanceKm
                },
                Status = workShift.Status
            };
        }
    }
}