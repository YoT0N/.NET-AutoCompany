using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.Interfaces;
using PersonnelService.Core.DTOs;

namespace PersonnelService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelController : ControllerBase
    {
        private readonly IPersonnelService _personnelService;

        public PersonnelController(IPersonnelService personnelService)
        {
            _personnelService = personnelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonnelDto>>> GetAll()
        {
            var personnel = await _personnelService.GetAllPersonnelAsync();
            return Ok(personnel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonnelDto>> GetById(string id)
        {
            var personnel = await _personnelService.GetPersonnelByIdAsync(id);
            if (personnel == null)
                return NotFound();

            return Ok(personnel);
        }

        [HttpGet("by-personnel-id/{personnelId}")]
        public async Task<ActionResult<PersonnelDto>> GetByPersonnelId(int personnelId)
        {
            var personnel = await _personnelService.GetPersonnelByPersonnelIdAsync(personnelId);
            if (personnel == null)
                return NotFound();

            return Ok(personnel);
        }

        [HttpGet("by-position/{position}")]
        public async Task<ActionResult<IEnumerable<PersonnelDto>>> GetByPosition(string position)
        {
            var personnel = await _personnelService.GetPersonnelByPositionAsync(position);
            return Ok(personnel);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<PersonnelDto>>> GetByStatus(string status)
        {
            var personnel = await _personnelService.GetPersonnelByStatusAsync(status);
            return Ok(personnel);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PersonnelDto>>> GetActive()
        {
            var personnel = await _personnelService.GetActivePersonnelAsync();
            return Ok(personnel);
        }

        [HttpPost]
        public async Task<ActionResult<PersonnelDto>> Create([FromBody] CreatePersonnelDto createDto)
        {
            var created = await _personnelService.CreatePersonnelAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] UpdatePersonnelDto updateDto)
        {
            var success = await _personnelService.UpdatePersonnelAsync(id, updateDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var success = await _personnelService.DeletePersonnelAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(string id, [FromBody] string status)
        {
            var success = await _personnelService.UpdatePersonnelStatusAsync(id, status);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/contacts")]
        public async Task<ActionResult> UpdateContacts(string id, [FromBody] PersonnelContactsDto contactsDto)
        {
            var success = await _personnelService.UpdatePersonnelContactsAsync(id, contactsDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/documents")]
        public async Task<ActionResult> AddDocument(string id, [FromBody] PersonnelDocumentInfoDto documentDto)
        {
            var success = await _personnelService.AddPersonnelDocumentAsync(id, documentDto);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}