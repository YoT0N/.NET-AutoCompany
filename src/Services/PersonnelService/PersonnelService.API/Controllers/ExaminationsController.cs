using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.TodoExaminations.Commands.CreateExamination;
using PersonnelService.Application.TodoExaminations.Commands.DeleteExamination;
using PersonnelService.Application.TodoExaminations.Commands.UpdateExamination;
using PersonnelService.Application.TodoExaminations.Queries.GetExaminationById;
using PersonnelService.Application.TodoExaminations.Queries.GetExaminationsByPersonnel;
using PersonnelService.Application.TodoExaminations.Queries.GetLatestExamination;

namespace PersonnelService.API.Controllers
{
    [Route("api/examinations")]
    [ApiController]
    public class ExaminationsController : ApiControllerBase
    {
        public ExaminationsController(IMediator mediator) : base(mediator) { }

        // GET: api/examinations/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await Mediator.Send(new GetExaminationByIdQuery(id));
            return HandleResult(result);
        }

        // GET: api/examinations/personnel/{personnelId}
        [HttpGet("personnel/{personnelId}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetByPersonnel(int personnelId)
        {
            var result = await Mediator.Send(new GetExaminationsByPersonnelQuery(personnelId));
            return Ok(result);
        }

        // GET: api/examinations/personnel/{personnelId}/latest
        [HttpGet("personnel/{personnelId}/latest")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLatest(int personnelId)
        {
            var result = await Mediator.Send(new GetLatestExaminationQuery(personnelId));
            return HandleResult(result);
        }

        // POST: api/examinations
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Create([FromBody] CreateExaminationCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT: api/examinations/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateExaminationCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        // DELETE: api/examinations/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await Mediator.Send(new DeleteExaminationCommand { Id = id });
            return NoContent();
        }
    }
}