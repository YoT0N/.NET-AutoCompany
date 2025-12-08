using AggregatorService.DTOs;
using AggregatorService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AggregatorService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AggregatorController : ControllerBase
{
    private readonly IAggregatorService _aggregatorService;
    private readonly ILogger<AggregatorController> _logger;

    public AggregatorController(
        IAggregatorService aggregatorService,
        ILogger<AggregatorController> logger)
    {
        _aggregatorService = aggregatorService;
        _logger = logger;
    }

    /// <summary>
    /// Отримати повну інформацію про автобус з усіх сервісів
    /// </summary>
    [HttpGet("bus/{countryNumber}")]
    [ProducesResponseType(typeof(BusFullInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusFullInfoDto>> GetBusFullInfo(
        string countryNumber,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request for full bus info: {CountryNumber}",
            countryNumber);

        var result = await _aggregatorService.GetBusFullInfoAsync(
            countryNumber,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new { Message = $"Bus {countryNumber} not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Отримати повну інформацію про персонал з усіх сервісів
    /// </summary>
    [HttpGet("personnel/{personnelId}")]
    [ProducesResponseType(typeof(PersonnelFullInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonnelFullInfoDto>> GetPersonnelFullInfo(
        int personnelId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request for full personnel info: {PersonnelId}",
            personnelId);

        var result = await _aggregatorService.GetPersonnelFullInfoAsync(
            personnelId,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new { Message = $"Personnel {personnelId} not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Отримати загальну статистику з усіх сервісів
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request for dashboard summary");

        var result = await _aggregatorService.GetDashboardSummaryAsync(cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "AggregatorService",
            Timestamp = DateTime.UtcNow
        });
    }
}