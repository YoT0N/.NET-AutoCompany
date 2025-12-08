using Microsoft.AspNetCore.Mvc;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Bll.DTOs.Maintenance;

namespace TechnicalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceHistoryController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceHistoryController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaintenanceHistoryDto>>> GetAllMaintenance()
    {
        var maintenanceRecords = await _maintenanceService.GetAllMaintenanceAsync();
        return Ok(maintenanceRecords);
    }

    [HttpGet("{maintenanceId}")]
    public async Task<ActionResult<MaintenanceHistoryDto>> GetMaintenanceById(long maintenanceId)
    {
        var maintenance = await _maintenanceService.GetMaintenanceByIdAsync(maintenanceId);

        if (maintenance == null)
            return NotFound($"Maintenance record with ID {maintenanceId} not found");

        return Ok(maintenance);
    }

    [HttpGet("bus/{countryNumber}")]
    public async Task<ActionResult<IEnumerable<MaintenanceHistoryDto>>> GetMaintenanceByBus(string countryNumber)
    {
        var maintenanceRecords = await _maintenanceService.GetMaintenanceByBusAsync(countryNumber);
        return Ok(maintenanceRecords);
    }

    [HttpGet("bus/{countryNumber}/total-cost")]
    public async Task<ActionResult<decimal>> GetTotalMaintenanceCost(string countryNumber)
    {
        var totalCost = await _maintenanceService.GetTotalMaintenanceCostAsync(countryNumber);
        return Ok(new { CountryNumber = countryNumber, TotalCost = totalCost });
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MaintenanceHistoryDto>>> GetUpcomingMaintenance([FromQuery] DateTime? fromDate)
    {
        var date = fromDate ?? DateTime.Now;
        var maintenanceRecords = await _maintenanceService.GetUpcomingMaintenanceAsync(date);
        return Ok(maintenanceRecords);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMaintenance([FromBody] CreateMaintenanceDto createMaintenanceDto)
    {
        var result = await _maintenanceService.CreateMaintenanceAsync(createMaintenanceDto);

        if (result > 0)
            return CreatedAtAction(nameof(GetMaintenanceById), new { maintenanceId = result }, createMaintenanceDto);

        return BadRequest("Failed to create maintenance record");
    }

    [HttpPut("{maintenanceId}")]
    public async Task<ActionResult> UpdateMaintenance(long maintenanceId, [FromBody] CreateMaintenanceDto updateMaintenanceDto)
    {
        var result = await _maintenanceService.UpdateMaintenanceAsync(maintenanceId, updateMaintenanceDto);

        if (result == 0)
            return NotFound($"Maintenance record with ID {maintenanceId} not found");

        return NoContent();
    }

    [HttpDelete("{maintenanceId}")]
    public async Task<ActionResult> DeleteMaintenance(long maintenanceId)
    {
        var result = await _maintenanceService.DeleteMaintenanceAsync(maintenanceId);

        if (result == 0)
            return NotFound($"Maintenance record with ID {maintenanceId} not found");

        return NoContent();
    }
}