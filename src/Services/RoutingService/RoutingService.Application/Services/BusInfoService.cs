using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.Interfaces;
using RoutingService.Bll.DTOs;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Exceptions;
using RoutingService.Domain.Repositories;

namespace RoutingService.Bll.Services
{
    public class BusInfoService : IBusInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BusInfoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BusInfoDto>> GetAllBusesAsync()
        {
            var buses = await _unitOfWork.Buses.GetAllAsync();
            return _mapper.Map<IEnumerable<BusInfoDto>>(buses);
        }

        public async Task<PagedResultDto<BusInfoDto>> GetBusesPagedAsync(BusFilterParameters parameters)
        {
            var query = _unitOfWork.Buses.Query();

            if (!string.IsNullOrWhiteSpace(parameters.CountryNumber))
            {
                query = query.Where(b => b.CountryNumber.Contains(parameters.CountryNumber));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Brand))
            {
                query = query.Where(b => b.Brand != null && b.Brand.Contains(parameters.Brand));
            }

            if (parameters.MinCapacity.HasValue)
            {
                query = query.Where(b => b.Capacity >= parameters.MinCapacity.Value);
            }

            if (parameters.MaxCapacity.HasValue)
            {
                query = query.Where(b => b.Capacity <= parameters.MaxCapacity.Value);
            }

            if (parameters.MinYear.HasValue)
            {
                query = query.Where(b => b.YearOfManufacture >= parameters.MinYear.Value);
            }

            if (parameters.MaxYear.HasValue)
            {
                query = query.Where(b => b.YearOfManufacture <= parameters.MaxYear.Value);
            }

            query = parameters.SortBy?.ToLower() switch
            {
                "countrynumber" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(b => b.CountryNumber)
                    : query.OrderBy(b => b.CountryNumber),
                "brand" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(b => b.Brand)
                    : query.OrderBy(b => b.Brand),
                "capacity" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(b => b.Capacity)
                    : query.OrderBy(b => b.Capacity),
                "year" => parameters.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(b => b.YearOfManufacture)
                    : query.OrderBy(b => b.YearOfManufacture),
                _ => query.OrderBy(b => b.BusId)
            };

            var totalCount = await query.CountAsync();

            // Apply pagination
            var buses = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var busDtos = _mapper.Map<IEnumerable<BusInfoDto>>(buses);

            return new PagedResultDto<BusInfoDto>(
                busDtos,
                parameters.Page,
                parameters.PageSize,
                totalCount);
        }

        public async Task<BusInfoDto?> GetBusByIdAsync(int id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);

            if (bus == null)
                throw new EntityNotFoundException(nameof(BusInfo), id);

            return _mapper.Map<BusInfoDto>(bus);
        }

        public async Task<BusInfoDto?> GetBusByCountryNumberAsync(string countryNumber)
        {
            var buses = await _unitOfWork.Buses.FindAsync(b => b.CountryNumber == countryNumber);
            var bus = buses.FirstOrDefault();

            if (bus == null)
                return null;

            return _mapper.Map<BusInfoDto>(bus);
        }

        public async Task<BusInfoDto> CreateBusAsync(CreateBusInfoDto dto)
        {
            var existingBus = await _unitOfWork.Buses
                .FindAsync(b => b.CountryNumber == dto.CountryNumber);

            if (existingBus.Any())
            {
                throw new ConflictException("BusInfo",
                    $"A bus with country number '{dto.CountryNumber}' already exists");
            }

            var bus = _mapper.Map<BusInfo>(dto);

            await _unitOfWork.Buses.AddAsync(bus);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BusInfoDto>(bus);
        }

        public async Task<BusInfoDto?> UpdateBusAsync(int id, UpdateBusInfoDto dto)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);

            if (bus == null)
                throw new EntityNotFoundException(nameof(BusInfo), id);

            if (dto.CountryNumber != null && dto.CountryNumber != bus.CountryNumber)
            {
                var existingBus = await _unitOfWork.Buses
                    .FindAsync(b => b.CountryNumber == dto.CountryNumber);

                if (existingBus.Any())
                {
                    throw new ConflictException("BusInfo",
                        $"A bus with country number '{dto.CountryNumber}' already exists");
                }
            }

            _mapper.Map(dto, bus);

            _unitOfWork.Buses.Update(bus);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BusInfoDto>(bus);
        }

        public async Task<bool> DeleteBusAsync(int id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);

            if (bus == null)
                throw new EntityNotFoundException(nameof(BusInfo), id);

            _unitOfWork.Buses.Delete(bus);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BusExistsAsync(int id)
        {
            return await _unitOfWork.Buses.ExistsAsync(b => b.BusId == id);
        }
    }
}