using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.TodoDocuments.Commands.CreateDocument;
using PersonnelService.Application.TodoDocuments.Commands.DeleteDocument;
using PersonnelService.Application.TodoDocuments.Commands.UpdateDocument;
using PersonnelService.Application.TodoDocuments.Queries.GetDocumentById;
using PersonnelService.Application.TodoDocuments.Queries.GetDocumentsByPersonnel;
using PersonnelService.Application.TodoDocuments.Queries.GetExpiringDocuments;

namespace PersonnelService.API.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ApiControllerBase
    {
        public DocumentsController(IMediator mediator) : base(mediator) { }

        // GET: api/documents/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await Mediator.Send(new GetDocumentByIdQuery(id));
            return HandleResult(result);
        }

        // GET: api/documents/personnel/{personnelId}
        [HttpGet("personnel/{personnelId}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetByPersonnel(int personnelId)
        {
            var result = await Mediator.Send(new GetDocumentsByPersonnelQuery(personnelId));
            return Ok(result);
        }

        // GET: api/documents/expiring
        [HttpGet("expiring")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetExpiring([FromQuery] int daysThreshold = 30)
        {
            var result = await Mediator.Send(new GetExpiringDocumentsQuery(daysThreshold));
            return Ok(result);
        }

        // POST: api/documents
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Create([FromBody] CreateDocumentCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT: api/documents/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateDocumentCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        // DELETE: api/documents/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await Mediator.Send(new DeleteDocumentCommand { Id = id });
            return NoContent();
        }
    }
}