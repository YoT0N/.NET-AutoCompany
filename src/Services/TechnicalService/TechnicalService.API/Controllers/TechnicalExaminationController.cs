using Microsoft.AspNetCore.Mvc;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Bll.DTOs.Examination;

namespace TechnicalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechnicalExaminationController : ControllerBase
{
    private readonly IExaminationService _examinationService;

    public TechnicalExaminationController(IExaminationService examinationService)
    {
        _examinationService = examinationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExaminationDto>>> GetAllExaminations()
    {
        var examinations = await _examinationService.GetAllExaminationsAsync();
        return Ok(examinations);
    }

    [HttpGet("{examinationId}")]
    public async Task<ActionResult<ExaminationDto>> GetExaminationById(long examinationId)
    {
        var examination = await _examinationService.GetExaminationByIdAsync(examinationId);

        if (examination == null)
            return NotFound($"Examination with ID {examinationId} not found");

        return Ok(examination);
    }

    [HttpGet("{examinationId}/with-parts")]
    public async Task<ActionResult<ExaminationDto>> GetExaminationWithParts(long examinationId)
    {
        var examination = await _examinationService.GetExaminationWithPartsAsync(examinationId);

        if (examination == null)
            return NotFound($"Examination with ID {examinationId} not found");

        return Ok(examination);
    }

    [HttpGet("bus/{countryNumber}")]
    public async Task<ActionResult<IEnumerable<ExaminationDto>>> GetExaminationsByBus(string countryNumber)
    {
        var examinations = await _examinationService.GetExaminationsByBusAsync(countryNumber);
        return Ok(examinations);
    }

    [HttpGet("failed")]
    public async Task<ActionResult<IEnumerable<ExaminationDto>>> GetFailedExaminations()
    {
        var examinations = await _examinationService.GetFailedExaminationsAsync();
        return Ok(examinations);
    }

    [HttpPost]
    public async Task<ActionResult> CreateExamination([FromBody] CreateExaminationDto createExaminationDto)
    {
        var examinationId = await _examinationService.CreateExaminationAsync(createExaminationDto);

        if (examinationId > 0)
            return CreatedAtAction(nameof(GetExaminationById), new { examinationId }, new { ExaminationId = examinationId });

        return BadRequest("Failed to create examination");
    }

    [HttpPut("{examinationId}")]
    public async Task<ActionResult> UpdateExamination(long examinationId, [FromBody] UpdateExaminationDto updateExaminationDto)
    {
        var result = await _examinationService.UpdateExaminationAsync(examinationId, updateExaminationDto);

        if (result == 0)
            return NotFound($"Examination with ID {examinationId} not found");

        return NoContent();
    }

    [HttpDelete("{examinationId}")]
    public async Task<ActionResult> DeleteExamination(long examinationId)
    {
        var result = await _examinationService.DeleteExaminationAsync(examinationId);

        if (result == 0)
            return NotFound($"Examination with ID {examinationId} not found");

        return NoContent();
    }
}