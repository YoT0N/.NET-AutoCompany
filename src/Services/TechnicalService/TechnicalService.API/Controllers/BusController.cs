using Microsoft.AspNetCore.Mvc;
using TechnicalService.Application.Interfaces;
using TechnicalService.Core.DTOs;

namespace TechnicalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusController : ControllerBase
{
    private readonly IBusService _busService;

    public BusController(IBusService busService)
    {
        _busService = busService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetAllBuses()
    {
        var buses = await _busService.GetAllBusesAsync();
        return Ok(buses);
    }

    [HttpGet("{countryNumber}")]
    public async Task<ActionResult<BusDto>> GetBusByCountryNumber(string countryNumber)
    {
        var bus = await _busService.GetBusByCountryNumberAsync(countryNumber);

        if (bus == null)
            return NotFound($"Bus with country number {countryNumber} not found");

        return Ok(bus);
    }

    [HttpGet("{countryNumber}/with-status")]
    public async Task<ActionResult<BusDto>> GetBusWithStatus(string countryNumber)
    {
        var bus = await _busService.GetBusWithStatusAsync(countryNumber);

        if (bus == null)
            return NotFound($"Bus with country number {countryNumber} not found");

        return Ok(bus);
    }

    [HttpGet("by-status/{statusId}")]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetBusesByStatus(int statusId)
    {
        var buses = await _busService.GetBusesByStatusAsync(statusId);
        return Ok(buses);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetActiveBuses()
    {
        var buses = await _busService.GetActiveBusesAsync();
        return Ok(buses);
    }

    [HttpGet("{countryNumber}/mileage")]
    public async Task<ActionResult<decimal>> GetTotalMileage(string countryNumber)
    {
        var mileage = await _busService.GetTotalMileageAsync(countryNumber);
        return Ok(new { CountryNumber = countryNumber, TotalMileage = mileage });
    }

    [HttpPost]
    public async Task<ActionResult> CreateBus([FromBody] CreateBusDto createBusDto)
    {
        var result = await _busService.CreateBusAsync(createBusDto);

        if (result > 0)
            return CreatedAtAction(nameof(GetBusByCountryNumber), new { countryNumber = createBusDto.CountryNumber }, createBusDto);

        return BadRequest("Failed to create bus");
    }

    [HttpPut("{countryNumber}")]
    public async Task<ActionResult> UpdateBus(string countryNumber, [FromBody] UpdateBusDto updateBusDto)
    {
        var result = await _busService.UpdateBusAsync(countryNumber, updateBusDto);

        if (result == 0)
            return NotFound($"Bus with country number {countryNumber} not found");

        return NoContent();
    }

    [HttpPatch("{countryNumber}/status")]
    public async Task<ActionResult> UpdateBusStatus(string countryNumber, [FromBody] int newStatusId)
    {
        var result = await _busService.UpdateBusStatusAsync(countryNumber, newStatusId);

        if (result == 0)
            return NotFound($"Bus with country number {countryNumber} not found");

        return NoContent();
    }

    [HttpDelete("{countryNumber}")]
    public async Task<ActionResult> DeleteBus(string countryNumber)
    {
        var result = await _busService.DeleteBusAsync(countryNumber);

        if (result == 0)
            return NotFound($"Bus with country number {countryNumber} not found");

        return NoContent();
    }
}