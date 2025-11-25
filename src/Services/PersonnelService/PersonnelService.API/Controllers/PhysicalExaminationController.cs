using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.Interfaces;
using PersonnelService.Core.Models;

namespace PersonnelService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhysicalExaminationController : ControllerBase
    {
        private readonly IExaminationService _examinationService;

        public PhysicalExaminationController(IExaminationService examinationService)
        {
            _examinationService = examinationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetAll()
        {
            var examinations = await _examinationService.GetAllExaminationsAsync();
            return Ok(examinations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhysicalExamination>> GetById(string id)
        {
            var examination = await _examinationService.GetExaminationByIdAsync(id);
            if (examination == null)
                return NotFound();

            return Ok(examination);
        }

        [HttpGet("by-personnel/{personnelId}")]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetByPersonnelId(int personnelId)
        {
            var examinations = await _examinationService.GetExaminationsByPersonnelIdAsync(personnelId);
            return Ok(examinations);
        }

        [HttpGet("latest/{personnelId}")]
        public async Task<ActionResult<PhysicalExamination>> GetLatestByPersonnelId(int personnelId)
        {
            var examination = await _examinationService.GetLatestExaminationByPersonnelIdAsync(personnelId);
            if (examination == null)
                return NotFound();

            return Ok(examination);
        }

        [HttpGet("by-result/{result}")]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetByResult(string result)
        {
            var examinations = await _examinationService.GetExaminationsByResultAsync(result);
            return Ok(examinations);
        }

        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var examinations = await _examinationService.GetExaminationsByDateRangeAsync(startDate, endDate);
            return Ok(examinations);
        }

        [HttpGet("by-doctor/{doctorName}")]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetByDoctor(string doctorName)
        {
            var examinations = await _examinationService.GetExaminationsByDoctorAsync(doctorName);
            return Ok(examinations);
        }

        [HttpPost]
        public async Task<ActionResult<PhysicalExamination>> Create([FromBody] PhysicalExamination examination)
        {
            var created = await _examinationService.CreateExaminationAsync(examination);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] PhysicalExamination examination)
        {
            var success = await _examinationService.UpdateExaminationAsync(id, examination);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var success = await _examinationService.DeleteExaminationAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("by-personnel/{personnelId}")]
        public async Task<ActionResult> DeleteByPersonnelId(int personnelId)
        {
            var success = await _examinationService.DeleteExaminationsByPersonnelIdAsync(personnelId);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}