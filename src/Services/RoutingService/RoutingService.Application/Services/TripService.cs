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
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TripService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
        {
            var trips = await _unitOfWork.Trips.GetAllAsync();
            return _mapper.Map<IEnumerable<TripDto>>(trips);
        }

        public async Task<TripDto?> GetTripByIdAsync(int id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);

            if (trip == null)
                throw new EntityNotFoundException(nameof(Trip), id);

            return _mapper.Map<TripDto>(trip);
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

            if (trip == null)
                throw new EntityNotFoundException(nameof(Trip), id);

            return _mapper.Map<TripDetailsDto>(trip);
        }

        public async Task<IEnumerable<TripDetailsDto>> GetTripsByRouteSheetAsync(int sheetId)
        {
            var sheetExists = await _unitOfWork.RouteSheets.ExistsAsync(rs => rs.SheetId == sheetId);
            if (!sheetExists)
                throw new EntityNotFoundException(nameof(RouteSheet), sheetId);

            var trips = await _unitOfWork.Trips
                .Query()
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .Where(t => t.SheetId == sheetId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TripDetailsDto>>(trips);
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

            return _mapper.Map<IEnumerable<TripDetailsDto>>(trips);
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

            return _mapper.Map<IEnumerable<TripDetailsDto>>(trips);
        }

        public async Task<PagedResultDto<TripDetailsDto>> GetTripsPagedAsync(TripFilterParameters parameters)
        {
            IQueryable<Trip> query = _unitOfWork.Trips
                .Query()
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo);

            // Apply filters
            if (parameters.SheetId.HasValue)
            {
                query = query.Where(t => t.SheetId == parameters.SheetId.Value);
            }

            if (parameters.Completed.HasValue)
            {
                query = query.Where(t => t.Completed == parameters.Completed.Value);
            }

            if (parameters.Date.HasValue)
            {
                query = query.Where(t => t.RouteSheet.SheetDate.Date == parameters.Date.Value.Date);
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "scheduled" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.ScheduledDeparture)
                    : query.OrderBy(t => t.ScheduledDeparture),
                "actual" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.ActualDeparture)
                    : query.OrderBy(t => t.ActualDeparture),
                "completed" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.Completed)
                    : query.OrderBy(t => t.Completed),
                _ => query.OrderBy(t => t.ScheduledDeparture)
            };

            var totalCount = await query.CountAsync();

            // Apply pagination
            var trips = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var tripDtos = _mapper.Map<IEnumerable<TripDetailsDto>>(trips);

            return new PagedResultDto<TripDetailsDto>(
                tripDtos,
                parameters.Page,
                parameters.PageSize,
                totalCount);
        }

        public async Task<TripDto> CreateTripAsync(CreateTripDto dto)
        {
            var sheetExists = await _unitOfWork.RouteSheets.ExistsAsync(rs => rs.SheetId == dto.SheetId);
            if (!sheetExists)
                throw new EntityNotFoundException(nameof(RouteSheet), dto.SheetId);

            var trip = _mapper.Map<Trip>(dto);

            await _unitOfWork.Trips.AddAsync(trip);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<TripDto?> UpdateTripAsync(int id, UpdateTripDto dto)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null)
                throw new EntityNotFoundException(nameof(Trip), id);

            _mapper.Map(dto, trip);

            _unitOfWork.Trips.Update(trip);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<bool> DeleteTripAsync(int id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null)
                throw new EntityNotFoundException(nameof(Trip), id);

            _unitOfWork.Trips.Delete(trip);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkTripAsCompletedAsync(int id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null)
                throw new EntityNotFoundException(nameof(Trip), id);

            trip.Completed = true;
            if (!trip.ActualDeparture.HasValue)
            {
                trip.ActualDeparture = trip.ScheduledDeparture;
            }

            _unitOfWork.Trips.Update(trip);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}