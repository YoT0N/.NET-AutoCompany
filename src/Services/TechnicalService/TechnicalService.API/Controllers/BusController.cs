using Microsoft.AspNetCore.Mvc;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Bll.DTOs.Bus;

namespace TechnicalService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BusController : ControllerBase
{
    private readonly IBusService _busService;
    private readonly ILogger<BusController> _logger;

    public BusController(IBusService busService, ILogger<BusController> logger)
    {
        _busService = busService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetAllBuses(CancellationToken cancellationToken)
    {
        var buses = await _busService.GetAllBusesAsync(cancellationToken);
        return Ok(buses);
    }

    [HttpGet("{countryNumber}")]
    [ProducesResponseType(typeof(BusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusDto>> GetBusByCountryNumber(
        string countryNumber,
        CancellationToken cancellationToken)
    {
        var bus = await _busService.GetBusByCountryNumberAsync(countryNumber, cancellationToken);
        return Ok(bus);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateBus(
        [FromBody] CreateBusDto createBusDto,
        CancellationToken cancellationToken)
    {
        var countryNumber = await _busService.CreateBusAsync(createBusDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetBusByCountryNumber),
            new { countryNumber },
            null);
    }

    [HttpPut("{countryNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateBus(
        string countryNumber,
        [FromBody] UpdateBusDto updateBusDto,
        CancellationToken cancellationToken)
    {
        await _busService.UpdateBusAsync(countryNumber, updateBusDto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{countryNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteBus(
        string countryNumber,
        CancellationToken cancellationToken)
    {
        await _busService.DeleteBusAsync(countryNumber, cancellationToken);
        return NoContent();
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<BusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetActiveBuses(CancellationToken cancellationToken)
    {
        var buses = await _busService.GetActiveBusesAsync(cancellationToken);
        return Ok(buses);
    }

    [HttpPatch("{countryNumber}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateBusStatus(
        string countryNumber,
        [FromBody] int newStatusId,
        CancellationToken cancellationToken)
    {
        await _busService.UpdateBusStatusAsync(countryNumber, newStatusId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{countryNumber}/mileage")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<decimal>> GetTotalMileage(
        string countryNumber,
        CancellationToken cancellationToken)
    {
        var mileage = await _busService.GetTotalMileageAsync(countryNumber, cancellationToken);
        return Ok(mileage);
    }
}