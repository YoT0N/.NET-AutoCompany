using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.API.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ApiControllerBase
    {
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IDocumentRepository _documentRepository;

        public ReportsController(
            IMediator mediator,
            IWorkShiftRepository workShiftRepository,
            IDocumentRepository documentRepository) : base(mediator)
        {
            _workShiftRepository = workShiftRepository;
            _documentRepository = documentRepository;
        }

        // GET: api/reports/personnel/{personnelId}/distance
        [HttpGet("personnel/{personnelId}/distance")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetPersonnelDistance(
            int personnelId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var totalDistance = await _workShiftRepository.GetTotalDistanceByPersonnelAsync(
                personnelId, startDate, endDate);

            var shiftCount = await _workShiftRepository.GetShiftCountByPersonnelAsync(
                personnelId, startDate, endDate);

            return Ok(new
            {
                personnelId,
                startDate,
                endDate,
                totalDistance,
                shiftCount,
                averageDistancePerShift = shiftCount > 0 ? totalDistance / shiftCount : 0
            });
        }

        // GET: api/reports/documents/statistics
        [HttpGet("documents/statistics")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetDocumentStatistics()
        {
            var documentCountByType = await _documentRepository.GetDocumentCountByTypeAsync();
            var expiringDocs = await _documentRepository.GetExpiringDocumentsAsync(30);
            var expiredDocs = await _documentRepository.GetExpiredDocumentsAsync();

            return Ok(new
            {
                documentsByType = documentCountByType,
                expiringCount = expiringDocs.Count,
                expiredCount = expiredDocs.Count
            });
        }
    }
}