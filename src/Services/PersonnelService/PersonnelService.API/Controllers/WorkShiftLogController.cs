using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.Interfaces;
using PersonnelService.Core.DTOs;

namespace PersonnelService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkShiftLogController : ControllerBase
    {
        private readonly IWorkShiftService _workShiftService;

        public WorkShiftLogController(IWorkShiftService workShiftService)
        {
            _workShiftService = workShiftService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetAll()
        {
            var workShifts = await _workShiftService.GetAllWorkShiftsAsync();
            return Ok(workShifts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkShiftDto>> GetById(string id)
        {
            var workShift = await _workShiftService.GetWorkShiftByIdAsync(id);
            if (workShift == null)
                return NotFound();

            return Ok(workShift);
        }

        [HttpGet("by-personnel/{personnelId}")]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetByPersonnelId(int personnelId)
        {
            var workShifts = await _workShiftService.GetWorkShiftsByPersonnelIdAsync(personnelId);
            return Ok(workShifts);
        }

        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var workShifts = await _workShiftService.GetWorkShiftsByDateRangeAsync(startDate, endDate);
            return Ok(workShifts);
        }

        [HttpGet("by-personnel-and-date")]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetByPersonnelAndDateRange(
            [FromQuery] int personnelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var workShifts = await _workShiftService.GetWorkShiftsByPersonnelAndDateRangeAsync(personnelId, startDate, endDate);
            return Ok(workShifts);
        }

        [HttpGet("by-bus/{busCountryNumber}")]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetByBusNumber(string busCountryNumber)
        {
            var workShifts = await _workShiftService.GetWorkShiftsByBusNumberAsync(busCountryNumber);
            return Ok(workShifts);
        }

        [HttpGet("by-route/{routeNumber}")]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetByRouteNumber(string routeNumber)
        {
            var workShifts = await _workShiftService.GetWorkShiftsByRouteNumberAsync(routeNumber);
            return Ok(workShifts);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<WorkShiftDto>>> GetByStatus(string status)
        {
            var workShifts = await _workShiftService.GetWorkShiftsByStatusAsync(status);
            return Ok(workShifts);
        }

        [HttpPost]
        public async Task<ActionResult<WorkShiftDto>> Create([FromBody] CreateWorkShiftDto createDto)
        {
            var created = await _workShiftService.CreateWorkShiftAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] UpdateWorkShiftDto updateDto)
        {
            var success = await _workShiftService.UpdateWorkShiftAsync(id, updateDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var success = await _workShiftService.DeleteWorkShiftAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("by-personnel/{personnelId}")]
        public async Task<ActionResult> DeleteByPersonnelId(int personnelId)
        {
            var success = await _workShiftService.DeleteWorkShiftsByPersonnelIdAsync(personnelId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(string id, [FromBody] string status)
        {
            var success = await _workShiftService.UpdateWorkShiftStatusAsync(id, status);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("statistics/total-distance")]
        public async Task<ActionResult<double>> GetTotalDistance(
            [FromQuery] int personnelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var totalDistance = await _workShiftService.GetTotalDistanceByPersonnelAsync(personnelId, startDate, endDate);
            return Ok(totalDistance);
        }

        [HttpGet("statistics/shift-count")]
        public async Task<ActionResult<int>> GetShiftCount(
            [FromQuery] int personnelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var shiftCount = await _workShiftService.GetShiftCountByPersonnelAsync(personnelId, startDate, endDate);
            return Ok(shiftCount);
        }
    }
}