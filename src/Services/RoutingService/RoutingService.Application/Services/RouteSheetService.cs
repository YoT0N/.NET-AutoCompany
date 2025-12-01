using System;
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
    public class RouteSheetService : IRouteSheetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RouteSheetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RouteSheetDto>> GetAllRouteSheetsAsync()
        {
            var sheets = await _unitOfWork.RouteSheets.GetAllAsync();
            return sheets.Select(MapToDto);
        }

        public async Task<RouteSheetDto?> GetRouteSheetByIdAsync(int id)
        {
            var sheet = await _unitOfWork.RouteSheets.GetByIdAsync(id);
            return sheet != null ? MapToDto(sheet) : null;
        }

        public async Task<RouteSheetDetailsDto?> GetRouteSheetDetailsAsync(int id)
        {
            var sheet = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .FirstOrDefaultAsync(rs => rs.SheetId == id);

            if (sheet == null) return null;

            return MapToDetailsDto(sheet);
        }

        public async Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByDateAsync(DateTime date)
        {
            var sheets = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .Where(rs => rs.SheetDate.Date == date.Date)
                .ToListAsync();

            return sheets.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByRouteAsync(int routeId)
        {
            var sheets = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .Where(rs => rs.RouteId == routeId)
                .ToListAsync();

            return sheets.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<RouteSheetDetailsDto>> GetRouteSheetsByBusAsync(int busId)
        {
            var sheets = await _unitOfWork.RouteSheets
                .Query()
                .Include(rs => rs.Route)
                .Include(rs => rs.BusInfo)
                .Where(rs => rs.BusId == busId)
                .ToListAsync();

            return sheets.Select(MapToDetailsDto);
        }

        public async Task<RouteSheetDto> CreateRouteSheetAsync(CreateRouteSheetDto dto)
        {
            var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId);
            if (!routeExists)
                throw new KeyNotFoundException($"Route with ID {dto.RouteId} not found");

            var busExists = await _unitOfWork.Buses.ExistsAsync(b => b.BusId == dto.BusId);
            if (!busExists)
                throw new KeyNotFoundException($"Bus with ID {dto.BusId} not found");

            var sheet = new RouteSheet
            {
                RouteId = dto.RouteId,
                BusId = dto.BusId,
                SheetDate = dto.SheetDate
            };

            await _unitOfWork.RouteSheets.AddAsync(sheet);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(sheet);
        }

        public async Task<RouteSheetDto?> UpdateRouteSheetAsync(int id, UpdateRouteSheetDto dto)
        {
            var sheet = await _unitOfWork.RouteSheets.GetByIdAsync(id);
            if (sheet == null) return null;

            if (dto.RouteId.HasValue)
            {
                var routeExists = await _unitOfWork.Routes.ExistsAsync(r => r.RouteId == dto.RouteId.Value);
                if (!routeExists)
                    throw new KeyNotFoundException($"Route with ID {dto.RouteId.Value} not found");
                sheet.RouteId = dto.RouteId.Value;
            }

            if (dto.BusId.HasValue)
            {
                var busExists = await _unitOfWork.Buses.ExistsAsync(b => b.BusId == dto.BusId.Value);
                if (!busExists)
                    throw new KeyNotFoundException($"Bus with ID {dto.BusId.Value} not found");
                sheet.BusId = dto.BusId.Value;
            }

            if (dto.SheetDate.HasValue) sheet.SheetDate = dto.SheetDate.Value;

            _unitOfWork.RouteSheets.Update(sheet);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(sheet);
        }

        public async Task<bool> DeleteRouteSheetAsync(int id)
        {
            var sheet = await _unitOfWork.RouteSheets.GetByIdAsync(id);
            if (sheet == null) return false;

            _unitOfWork.RouteSheets.Delete(sheet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static RouteSheetDto MapToDto(RouteSheet sheet)
        {
            return new RouteSheetDto
            {
                SheetId = sheet.SheetId,
                RouteId = sheet.RouteId,
                BusId = sheet.BusId,
                SheetDate = sheet.SheetDate
            };
        }

        private static RouteSheetDetailsDto MapToDetailsDto(RouteSheet sheet)
        {
            return new RouteSheetDetailsDto
            {
                SheetId = sheet.SheetId,
                RouteId = sheet.RouteId,
                BusId = sheet.BusId,
                SheetDate = sheet.SheetDate,
                RouteNumber = sheet.Route.RouteNumber,
                RouteName = sheet.Route.Name,
                BusCountryNumber = sheet.BusInfo.CountryNumber,
                BusBrand = sheet.BusInfo.Brand
            };
        }
    }
}