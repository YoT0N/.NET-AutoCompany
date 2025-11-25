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
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
        {
            var trips = await _unitOfWork.Trips.GetAllAsync();
            return trips.Select(MapToDto);
        }

        public async Task<TripDto?> GetTripByIdAsync(int id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            return trip != null ? MapToDto(trip) : null;
        }

        public async Task<TripDetailsDto?> GetTripDetailsAsync(int id)
        {
            var trip = await _unitOfWork.Trips
                .Query()
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .FirstOrDefaultAsync(t => t.TripId == id);

            if (trip == null) return null;

            return MapToDetailsDto(trip);
        }

        public async Task<IEnumerable<TripDetailsDto>> GetTripsByRouteSheetAsync(int sheetId)
        {
            var trips = await _unitOfWork.Trips
                .Query()
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .Where(t => t.SheetId == sheetId)
                .ToListAsync();

            return trips.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<TripDetailsDto>> GetCompletedTripsAsync()
        {
            var trips = await _unitOfWork.Trips
                .Query()
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .Where(t => t.Completed)
                .ToListAsync();

            return trips.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<TripDetailsDto>> GetPendingTripsAsync()
        {
            var trips = await _unitOfWork.Trips
                .Query()
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .Where(t => !t.Completed)
                .ToListAsync();

            return trips.Select(MapToDetailsDto);
        }

        public async Task<TripDto> CreateTripAsync(CreateTripDto dto)
        {
            var sheetExists = await _unitOfWork.RouteSheets.ExistsAsync(rs => rs.SheetId == dto.SheetId);
            if (!sheetExists)
                throw new KeyNotFoundException($"RouteSheet with ID {dto.SheetId} not found");

            var trip = new Trip
            {
                SheetId = dto.SheetId,
                ScheduledDeparture = dto.ScheduledDeparture,
                ActualDeparture = null,
                Completed = false
            };

            await _unitOfWork.Trips.AddAsync(trip);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(trip);
        }

        public async Task<TripDto?> UpdateTripAsync(int id, UpdateTripDto dto)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null) return null;

            if (dto.ScheduledDeparture.HasValue) trip.ScheduledDeparture = dto.ScheduledDeparture.Value;
            if (dto.ActualDeparture.HasValue) trip.ActualDeparture = dto.ActualDeparture.Value;
            if (dto.Completed.HasValue) trip.Completed = dto.Completed.Value;

            _unitOfWork.Trips.Update(trip);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(trip);
        }

        public async Task<bool> DeleteTripAsync(int id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null) return false;

            _unitOfWork.Trips.Delete(trip);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkTripAsCompletedAsync(int id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null) return false;

            trip.Completed = true;
            _unitOfWork.Trips.Update(trip);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static TripDto MapToDto(Trip trip)
        {
            return new TripDto
            {
                TripId = trip.TripId,
                SheetId = trip.SheetId,
                ScheduledDeparture = trip.ScheduledDeparture,
                ActualDeparture = trip.ActualDeparture,
                Completed = trip.Completed
            };
        }

        private static TripDetailsDto MapToDetailsDto(Trip trip)
        {
            return new TripDetailsDto
            {
                TripId = trip.TripId,
                SheetId = trip.SheetId,
                ScheduledDeparture = trip.ScheduledDeparture,
                ActualDeparture = trip.ActualDeparture,
                Completed = trip.Completed,
                SheetDate = trip.RouteSheet.SheetDate,
                RouteNumber = trip.RouteSheet.Route.RouteNumber,
                BusCountryNumber = trip.RouteSheet.BusInfo.CountryNumber
            };
        }
    }
}