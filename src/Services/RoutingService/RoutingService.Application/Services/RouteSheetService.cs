using System;
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
    public class RouteSheetService : IRouteSheetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RouteSheetService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteSheetDto>> GetAllRouteSheetsAsync()
        {
            var sheets = await _unitOfWork.RouteSheets.GetAllAsync();
            return _mapper.Map<IEnumerable<RouteSheetDto>>(sheets);
        }

        public async Task<RouteSheetDto?> GetRouteSheetByIdAsync(int id)
        {
            var sheet = await _unitOfWork.RouteSheets.GetByIdAsync(id);

            if (sheet == null)
                throw new EntityNotFoundException(nameof(RouteSheet), id);

            return _mapper.Map<RouteSheetDto>(sheet);
        }

        public async Task<RouteSheetDetailsDto?> GetRouteSheetDetailsAsync(int id)
        {
            var sheet = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .FirstOrDefaultAsync(rs => rs.SheetId == id);

            if (sheet == null)
                throw new EntityNotFoundException(nameof(RouteSheet), id);

            return _mapper.Map<RouteSheetDetailsDto>(sheet);
        }

        public async Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByDateAsync(DateTime date)
        {
            var sheets = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .Where(rs => rs.SheetDate.Date == date.Date)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RouteSheetDetailsDto>>(sheets);
        }

        public async Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByRouteAsync(int routeId)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == routeId);
            if (!routeExists)
                throw new EntityNotFoundException(nameof(Route), routeId);

            var sheets = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .Where(rs => rs.RouteId == routeId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RouteSheetDetailsDto>>(sheets);
        }

        public async Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByBusAsync(int busId)
        {
            var busExists = await _unitOfWork.Buses.ExistsAsync(b => b.BusId == busId);
            if (!busExists)
                throw new EntityNotFoundException(nameof(BusInfo), busId);

            var sheets = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .Where(rs => rs.BusId == busId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RouteSheetDetailsDto>>(sheets);
        }

        public async Task<PagedResultDto<RouteSheetDetailsDto>> GetRouteSheetsPagedAsync(RouteSheetFilterParameters parameters)
        {
            var query = _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo);

            // Apply filters
            if (parameters.RouteId.HasValue)
            {
                query = query.Where(rs => rs.RouteId == parameters.RouteId.Value);
            }

            if (parameters.BusId.HasValue)
            {
                query = query.Where(rs => rs.BusId == parameters.BusId.Value);
            }

            if (parameters.Date.HasValue)
            {
                query = query.Where(rs => rs.SheetDate.Date == parameters.Date.Value.Date);
            }

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(rs => rs.SheetDate.Date >= parameters.StartDate.Value.Date);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(rs => rs.SheetDate.Date <= parameters.EndDate.Value.Date);
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "date" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(rs => rs.SheetDate)
                    : query.OrderBy(rs => rs.SheetDate),
                "route" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(rs => rs.Route.RouteNumber)
                    : query.OrderBy(rs => rs.Route.RouteNumber),
                "bus" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(rs => rs.BusInfo.CountryNumber)
                    : query.OrderBy(rs => rs.BusInfo.CountryNumber),
                _ => query.OrderByDescending(rs => rs.SheetDate)
            };

            var totalCount = await query.CountAsync();

            var sheets = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var sheetDtos = _mapper.Map<IEnumerable<RouteSheetDetailsDto>>(sheets);

            return new PagedResultDto<RouteSheetDetailsDto>(
                sheetDtos,
                parameters.Page,
                parameters.PageSize,
                totalCount);
        }

        public async Task<RouteSheetDto> CreateRouteSheetAsync(CreateRouteSheetDto dto)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId);
            if (!routeExists)
                throw new EntityNotFoundException(nameof(Route), dto.RouteId);

            var busExists = await _unitOfWork.Buses.ExistsAsync(b => b.BusId == dto.BusId);
            if (!busExists)
                throw new EntityNotFoundException(nameof(BusInfo), dto.BusId);

            // Check for duplicate
            var existingSheet = await _unitOfWork.RouteSheets
                .Query()
                .FirstOrDefaultAsync(rs =>
                    rs.RouteId == dto.RouteId &&
                    rs.BusId == dto.BusId &&
                    rs.SheetDate.Date == dto.SheetDate.Date);

            if (existingSheet != null)
            {
                throw new ConflictException("RouteSheet",
                    $"A route sheet for route {dto.RouteId}, bus {dto.BusId} on {dto.SheetDate.ToShortDateString()} already exists");
            }

            var sheet = _mapper.Map<RouteSheet>(dto);

            await _unitOfWork.RouteSheets.AddAsync(sheet);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RouteSheetDto>(sheet);
        }

        public async Task<RouteSheetDto?> UpdateRouteSheetAsync(int id, UpdateRouteSheetDto dto)
        {
            var sheet = await _unitOfWork.RouteSheets.GetByIdAsync(id);
            if (sheet == null)
                throw new EntityNotFoundException(nameof(RouteSheet), id);

            if (dto.RouteId.HasValue)
            {
                var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId.Value);
                if (!routeExists)
                    throw new EntityNotFoundException(nameof(Route), dto.RouteId.Value);
            }

            if (dto.BusId.HasValue)
            {
                var busExists = await _unitOfWork.Buses.ExistsAsync(b => b.BusId == dto.BusId.Value);
                if (!busExists)
                    throw new EntityNotFoundException(nameof(BusInfo), dto.BusId.Value);
            }

            // Check for duplicate if key fields are being changed
            if (dto.RouteId.HasValue || dto.BusId.HasValue || dto.SheetDate.HasValue)
            {
                var newRouteId = dto.RouteId ?? sheet.RouteId;
                var newBusId = dto.BusId ?? sheet.BusId;
                var newDate = dto.SheetDate ?? sheet.SheetDate;

                var existingSheet = await _unitOfWork.RouteSheets
                    .Query()
                    .FirstOrDefaultAsync(rs =>
                        rs.SheetId != id &&
                        rs.RouteId == newRouteId &&
                        rs.BusId == newBusId &&
                        rs.SheetDate.Date == newDate.Date);

                if (existingSheet != null)
                {
                    throw new ConflictException("RouteSheet",
                        $"A route sheet for route {newRouteId}, bus {newBusId} on {newDate.ToShortDateString()} already exists");
                }
            }

            _mapper.Map(dto, sheet);

            _unitOfWork.RouteSheets.Update(sheet);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RouteSheetDto>(sheet);
        }

        public async Task<bool> DeleteRouteSheetAsync(int id)
        {
            var sheet = await _unitOfWork.RouteSheets.GetByIdAsync(id);
            if (sheet == null)
                throw new EntityNotFoundException(nameof(RouteSheet), id);

            _unitOfWork.RouteSheets.Delete(sheet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}