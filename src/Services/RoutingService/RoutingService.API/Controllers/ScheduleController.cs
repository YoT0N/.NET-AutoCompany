using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingService.Bll.Interfaces;
using RoutingService.Bll.DTOs;

namespace RoutingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("with-route-info")]
        public async Task<ActionResult<IEnumerable<ScheduleWithRouteDto>>> GetSchedulesWithRouteInfo()
        {
            var schedules = await _scheduleService.GetSchedulesWithRouteInfoAsync();
            return Ok(schedules);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound($"Schedule with ID {id} not found");

            return Ok(schedule);
        }

        [HttpGet("route/{routeId}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetSchedulesByRoute(int routeId)
        {
            var schedules = await _scheduleService.GetSchedulesByRouteAsync(routeId);
            return Ok(schedules);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var schedule = await _scheduleService.CreateScheduleAsync(dto);
                return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.ScheduleId }, schedule);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ScheduleDto>> UpdateSchedule(int id, [FromBody] UpdateScheduleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var schedule = await _scheduleService.UpdateScheduleAsync(id, dto);
            if (schedule == null)
                return NotFound($"Schedule with ID {id} not found");

            return Ok(schedule);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result)
                return NotFound($"Schedule with ID {id} not found");

            return NoContent();
        }
    }
}