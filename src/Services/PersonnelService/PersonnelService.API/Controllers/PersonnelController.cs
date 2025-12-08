using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.DeletePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnelStatus;
using PersonnelService.Application.TodoPersonnel.Queries.GetAllPersonnel;
using PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelById;
using PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelByPosition;

namespace PersonnelService.API.Controllers
{
    [Route("api/personnel")]
    [ApiController]
    public class PersonnelController : ApiControllerBase
    {
        public PersonnelController(IMediator mediator) : base(mediator) { }

        // GET: api/personnel
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPersonnelQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        // GET: api/personnel/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await Mediator.Send(new GetPersonnelByIdQuery(id));
            return HandleResult(result);
        }

        // GET: api/personnel/position/{position}
        [HttpGet("position/{position}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetByPosition(string position)
        {
            var result = await Mediator.Send(new GetPersonnelByPositionQuery(position));
            return Ok(result);
        }

        // POST: api/personnel
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Create([FromBody] CreatePersonnelCommand command)
        {
            try
            {
                var id = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return UnprocessableEntity(new { message = ex.Message });
            }
        }

        // PUT: api/personnel/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePersonnelCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        // PUT: api/personnel/{id}/status
        [HttpPut("{id}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdatePersonnelStatusCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        // DELETE: api/personnel/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await Mediator.Send(new DeletePersonnelCommand { Id = id });
            return NoContent();
        }
    }
}