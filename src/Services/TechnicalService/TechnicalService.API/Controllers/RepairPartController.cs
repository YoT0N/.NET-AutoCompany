using Microsoft.AspNetCore.Mvc;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Domain.Entities;

namespace TechnicalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepairPartController : ControllerBase
{
    private readonly IRepairPartService _repairPartService;

    public RepairPartController(IRepairPartService repairPartService)
    {
        _repairPartService = repairPartService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RepairPart>>> GetAllParts()
    {
        var parts = await _repairPartService.GetAllPartsAsync();
        return Ok(parts);
    }

    [HttpGet("{partId}")]
    public async Task<ActionResult<RepairPart>> GetPartById(int partId)
    {
        var part = await _repairPartService.GetPartByIdAsync(partId);

        if (part == null)
            return NotFound($"Part with ID {partId} not found");

        return Ok(part);
    }

    [HttpGet("by-part-number/{partNumber}")]
    public async Task<ActionResult<RepairPart>> GetPartByPartNumber(string partNumber)
    {
        var part = await _repairPartService.GetPartByPartNumberAsync(partNumber);

        if (part == null)
            return NotFound($"Part with part number {partNumber} not found");

        return Ok(part);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<RepairPart>>> GetLowStockParts([FromQuery] int threshold = 10)
    {
        var parts = await _repairPartService.GetLowStockPartsAsync(threshold);
        return Ok(parts);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePart([FromBody] RepairPart part)
    {
        var result = await _repairPartService.CreatePartAsync(part);

        if (result > 0)
            return CreatedAtAction(nameof(GetPartById), new { partId = part.PartId }, part);

        return BadRequest("Failed to create part");
    }

    [HttpPut("{partId}")]
    public async Task<ActionResult> UpdatePart(int partId, [FromBody] RepairPart part)
    {
        if (partId != part.PartId)
            return BadRequest("Part ID mismatch");

        var result = await _repairPartService.UpdatePartAsync(part);

        if (result == 0)
            return NotFound($"Part with ID {partId} not found");

        return NoContent();
    }

    [HttpPatch("{partId}/stock")]
    public async Task<ActionResult> UpdateStockQuantity(int partId, [FromBody] int quantity)
    {
        var result = await _repairPartService.UpdateStockQuantityAsync(partId, quantity);

        if (result == 0)
            return NotFound($"Part with ID {partId} not found");

        return NoContent();
    }

    [HttpDelete("{partId}")]
    public async Task<ActionResult> DeletePart(int partId)
    {
        var result = await _repairPartService.DeletePartAsync(partId);

        if (result == 0)
            return NotFound($"Part with ID {partId} not found");

        return NoContent();
    }
}