using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.Interfaces;
using PersonnelService.Core.Models;

namespace PersonnelService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelDocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public PersonnelDocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonnelDocument>>> GetAll()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonnelDocument>> GetById(string id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        [HttpGet("by-personnel/{personnelId}")]
        public async Task<ActionResult<IEnumerable<PersonnelDocument>>> GetByPersonnelId(int personnelId)
        {
            var documents = await _documentService.GetDocumentsByPersonnelIdAsync(personnelId);
            return Ok(documents);
        }

        [HttpGet("by-type/{docType}")]
        public async Task<ActionResult<IEnumerable<PersonnelDocument>>> GetByDocType(string docType)
        {
            var documents = await _documentService.GetDocumentsByTypeAsync(docType);
            return Ok(documents);
        }

        [HttpGet("expired")]
        public async Task<ActionResult<IEnumerable<PersonnelDocument>>> GetExpired([FromQuery] DateTime? beforeDate)
        {
            var date = beforeDate ?? DateTime.UtcNow;
            var documents = await _documentService.GetExpiredDocumentsAsync(date);
            return Ok(documents);
        }

        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<PersonnelDocument>>> GetExpiring([FromQuery] int days = 30)
        {
            var withinDate = DateTime.UtcNow.AddDays(days);
            var documents = await _documentService.GetExpiringDocumentsAsync(withinDate);
            return Ok(documents);
        }

        [HttpPost]
        public async Task<ActionResult<PersonnelDocument>> Create([FromBody] PersonnelDocument document)
        {
            var created = await _documentService.CreateDocumentAsync(document);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] PersonnelDocument document)
        {
            var success = await _documentService.UpdateDocumentAsync(id, document);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var success = await _documentService.DeleteDocumentAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("by-personnel/{personnelId}")]
        public async Task<ActionResult> DeleteByPersonnelId(int personnelId)
        {
            var success = await _documentService.DeleteDocumentsByPersonnelIdAsync(personnelId);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}