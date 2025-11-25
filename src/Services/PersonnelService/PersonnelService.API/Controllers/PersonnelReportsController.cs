using Microsoft.AspNetCore.Mvc;
using PersonnelService.Application.Interfaces;

namespace PersonnelService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelReportsController : ControllerBase
    {
        private readonly IPersonnelService _personnelService;
        private readonly IWorkShiftService _workShiftService;
        private readonly IDocumentService _documentService;
        private readonly IExaminationService _examinationService;

        public PersonnelReportsController(
            IPersonnelService personnelService,
            IWorkShiftService workShiftService,
            IDocumentService documentService,
            IExaminationService examinationService)
        {
            _personnelService = personnelService;
            _workShiftService = workShiftService;
            _documentService = documentService;
            _examinationService = examinationService;
        }

        [HttpGet("personnel-summary")]
        public async Task<ActionResult> GetPersonnelSummary()
        {
            var allPersonnel = await _personnelService.GetAllPersonnelAsync();
            var activePersonnel = await _personnelService.GetActivePersonnelAsync();

            var summary = new
            {
                TotalPersonnel = allPersonnel.Count(),
                ActivePersonnel = activePersonnel.Count(),
                InactivePersonnel = allPersonnel.Count() - activePersonnel.Count(),
                ByPosition = allPersonnel.GroupBy(p => p.Position)
                    .Select(g => new { Position = g.Key, Count = g.Count() })
            };

            return Ok(summary);
        }

        [HttpGet("personnel-workload")]
        public async Task<ActionResult> GetPersonnelWorkload(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var personnel = await _personnelService.GetActivePersonnelAsync();
            var workloadData = new List<object>();

            foreach (var person in personnel)
            {
                var shifts = await _workShiftService.GetWorkShiftsByPersonnelAndDateRangeAsync(
                    person.PersonnelId, startDate, endDate);
                var totalDistance = await _workShiftService.GetTotalDistanceByPersonnelAsync(
                    person.PersonnelId, startDate, endDate);
                var shiftCount = await _workShiftService.GetShiftCountByPersonnelAsync(
                    person.PersonnelId, startDate, endDate);

                workloadData.Add(new
                {
                    PersonnelId = person.PersonnelId,
                    FullName = person.FullName,
                    Position = person.Position,
                    ShiftCount = shiftCount,
                    TotalDistanceKm = totalDistance
                });
            }

            return Ok(workloadData);
        }

        [HttpGet("document-expiration")]
        public async Task<ActionResult> GetDocumentExpiration([FromQuery] int days = 30)
        {
            var withinDate = DateTime.UtcNow.AddDays(days);
            var expiringDocuments = await _documentService.GetExpiringDocumentsAsync(withinDate);
            var expiredDocuments = await _documentService.GetExpiredDocumentsAsync(DateTime.UtcNow);

            var report = new
            {
                ExpiredCount = expiredDocuments.Count(),
                ExpiringCount = expiringDocuments.Count(),
                ExpiredDocuments = expiredDocuments,
                ExpiringDocuments = expiringDocuments
            };

            return Ok(report);
        }

        [HttpGet("health-status")]
        public async Task<ActionResult> GetHealthStatus()
        {
            var personnel = await _personnelService.GetActivePersonnelAsync();
            var healthData = new List<object>();

            foreach (var person in personnel)
            {
                var latestExam = await _examinationService.GetLatestExaminationByPersonnelIdAsync(person.PersonnelId);

                healthData.Add(new
                {
                    PersonnelId = person.PersonnelId,
                    FullName = person.FullName,
                    Position = person.Position,
                    LastExamDate = latestExam?.ExamDate,
                    LastExamResult = latestExam?.Result,
                    DaysSinceLastExam = latestExam != null
                        ? (DateTime.UtcNow - latestExam.ExamDate).Days
                        : (int?)null
                });
            }

            return Ok(healthData);
        }

        [HttpGet("personnel-details/{personnelId}")]
        public async Task<ActionResult> GetPersonnelDetails(int personnelId)
        {
            var personnel = await _personnelService.GetPersonnelByPersonnelIdAsync(personnelId);
            if (personnel == null)
                return NotFound();

            var documents = await _documentService.GetDocumentsByPersonnelIdAsync(personnelId);
            var examinations = await _examinationService.GetExaminationsByPersonnelIdAsync(personnelId);
            var latestExam = await _examinationService.GetLatestExaminationByPersonnelIdAsync(personnelId);

            var last30Days = DateTime.UtcNow.AddDays(-30);
            var recentShifts = await _workShiftService.GetWorkShiftsByPersonnelAndDateRangeAsync(
                personnelId, last30Days, DateTime.UtcNow);

            var details = new
            {
                Personnel = personnel,
                Documents = documents,
                ExaminationHistory = examinations,
                LatestExamination = latestExam,
                RecentShifts = recentShifts,
                Statistics = new
                {
                    TotalDocuments = documents.Count(),
                    TotalExaminations = examinations.Count(),
                    RecentShiftCount = recentShifts.Count()
                }
            };

            return Ok(details);
        }
    }
}