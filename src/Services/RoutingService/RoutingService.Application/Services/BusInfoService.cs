using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoutingService.Application.Interfaces;
using RoutingService.Core.DTOs;
using RoutingService.Core.Entities;
using RoutingService.Domain.Repositories;

namespace RoutingService.Bll.Services
{
    public class BusInfoService : IBusInfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BusInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BusInfoDto>> GetAllBusesAsync()
        {
            var buses = await _unitOfWork.Buses.GetAllAsync();
            return buses.Select(MapToDto);
        }

        public async Task<BusInfoDto?> GetBusByIdAsync(int id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            return bus != null ? MapToDto(bus) : null;
        }

        public async Task<BusInfoDto?> GetBusByCountryNumberAsync(string countryNumber)
        {
            var buses = await _unitOfWork.Buses.FindAsync(b => b.CountryNumber == countryNumber);
            var bus = buses.FirstOrDefault();
            return bus != null ? MapToDto(bus) : null;
        }

        public async Task<BusInfoDto> CreateBusAsync(CreateBusInfoDto dto)
        {
            var bus = new BusInfo
            {
                CountryNumber = dto.CountryNumber,
                Brand = dto.Brand,
                Capacity = dto.Capacity,
                YearOfManufacture = dto.YearOfManufacture
            };

            await _unitOfWork.Buses.AddAsync(bus);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(bus);
        }

        public async Task<BusInfoDto?> UpdateBusAsync(int id, UpdateBusInfoDto dto)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null) return null;

            if (dto.CountryNumber != null) bus.CountryNumber = dto.CountryNumber;
            if (dto.Brand != null) bus.Brand = dto.Brand;
            if (dto.Capacity.HasValue) bus.Capacity = dto.Capacity;
            if (dto.YearOfManufacture.HasValue) bus.YearOfManufacture = dto.YearOfManufacture;

            _unitOfWork.Buses.Update(bus);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(bus);
        }

        public async Task<bool> DeleteBusAsync(int id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null) return false;

            _unitOfWork.Buses.Delete(bus);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BusExistsAsync(int id)
        {
            return await _unitOfWork.Buses.ExistsAsync(b => b.BusId == id);
        }

        private static BusInfoDto MapToDto(BusInfo bus)
        {
            return new BusInfoDto
            {
                BusId = bus.BusId,
                CountryNumber = bus.CountryNumber,
                Brand = bus.Brand,
                Capacity = bus.Capacity,
                YearOfManufacture = bus.YearOfManufacture
            };
        }
    }
}