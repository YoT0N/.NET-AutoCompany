using Microsoft.AspNetCore.Mvc;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Bll.DTOs.Bus;
using TechnicalService.Domain.Exceptions;

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

    /// <summary>
    /// Отримати всі автобуси
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetAllBuses(CancellationToken cancellationToken)
    {
        var buses = await _busService.GetAllBusesAsync();
        return Ok(buses);
    }

    /// <summary>
    /// Отримати автобус за номером
    /// </summary>
    [HttpGet("{countryNumber}")]
    [ProducesResponseType(typeof(BusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusDto>> GetBusByCountryNumber(
        string countryNumber,
        CancellationToken cancellationToken)
    {
        var bus = await _busService.GetBusByCountryNumberAsync(countryNumber);

        if (bus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return Ok(bus);
    }

    /// <summary>
    /// Створити новий автобус
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateBus(
        [FromBody] CreateBusDto createBusDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _busService.CreateBusAsync(createBusDto);

        return CreatedAtAction(
            nameof(GetBusByCountryNumber),
            new { countryNumber = createBusDto.CountryNumber },
            null);
    }

    /// <summary>
    /// Оновити автобус
    /// </summary>
    [HttpPut("{countryNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateBus(
        string countryNumber,
        [FromBody] UpdateBusDto updateBusDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _busService.UpdateBusAsync(countryNumber, updateBusDto);

        if (result == 0)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return NoContent();
    }

    /// <summary>
    /// Видалити автобус (м'яке видалення)
    /// </summary>
    [HttpDelete("{countryNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBus(string countryNumber, CancellationToken cancellationToken)
    {
        var result = await _busService.DeleteBusAsync(countryNumber);

        if (result == 0)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return NoContent();
    }

    /// <summary>
    /// Отримати активні автобуси
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<BusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BusDto>>> GetActiveBuses(CancellationToken cancellationToken)
    {
        var buses = await _busService.GetActiveBusesAsync();
        return Ok(buses);
    }

    /// <summary>
    /// Оновити статус автобуса
    /// </summary>
    [HttpPatch("{countryNumber}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateBusStatus(
        string countryNumber,
        [FromBody] int newStatusId,
        CancellationToken cancellationToken)
    {
        var result = await _busService.UpdateBusStatusAsync(countryNumber, newStatusId);

        if (result == 0)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return NoContent();
    }
}