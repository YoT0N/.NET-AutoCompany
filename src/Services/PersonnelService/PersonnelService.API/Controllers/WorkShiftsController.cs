using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.TodoWorkShifts.Commands.CreateWorkShift;
using PersonnelService.Application.TodoWorkShifts.Commands.DeleteWorkShift;
using PersonnelService.Application.TodoWorkShifts.Commands.UpdateWorkShift;
using PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftById;
using PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByDateRange;
using PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByPersonnel;

namespace PersonnelService.API.Controllers
{
    [Route("api/workshifts")]
    [ApiController]
    public class WorkShiftsController : ApiControllerBase
    {
        public WorkShiftsController(IMediator mediator) : base(mediator) { }

        // GET: api/workshifts/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await Mediator.Send(new GetWorkShiftByIdQuery(id));
            return HandleResult(result);
        }

        // GET: api/workshifts/personnel/{personnelId}
        [HttpGet("personnel/{personnelId}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetByPersonnel(int personnelId)
        {
            var result = await Mediator.Send(new GetWorkShiftsByPersonnelQuery(personnelId));
            return Ok(result);
        }

        // GET: api/workshifts/daterange
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? personnelId = null)
        {
            var result = await Mediator.Send(new GetWorkShiftsByDateRangeQuery(startDate, endDate, personnelId));
            return Ok(result);
        }

        // POST: api/workshifts
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Create([FromBody] CreateWorkShiftCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT: api/workshifts/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateWorkShiftCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        // DELETE: api/workshifts/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await Mediator.Send(new DeleteWorkShiftCommand { Id = id });
            return NoContent();
        }
    }
}