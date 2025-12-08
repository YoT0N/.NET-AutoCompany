using Microsoft.AspNetCore.Mvc;
using TechnicalService.Bll.Interfaces;

namespace TechnicalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IBusService _busService;
    private readonly IMaintenanceService _maintenanceService;
    private readonly IExaminationService _examinationService;
    private readonly IRepairPartService _repairPartService;

    public ReportsController(
        IBusService busService,
        IMaintenanceService maintenanceService,
        IExaminationService examinationService,
        IRepairPartService repairPartService)
    {
        _busService = busService;
        _maintenanceService = maintenanceService;
        _examinationService = examinationService;
        _repairPartService = repairPartService;
    }

    [HttpGet("bus-fleet-summary")]
    public async Task<ActionResult> GetBusFleetSummary()
    {
        var allBuses = await _busService.GetAllBusesAsync();
        var activeBuses = await _busService.GetActiveBusesAsync();

        var summary = new
        {
            TotalBuses = allBuses.Count(),
            ActiveBuses = activeBuses.Count(),
            InactiveBuses = allBuses.Count() - activeBuses.Count(),
            AverageAge = allBuses.Any() ? DateTime.Now.Year - allBuses.Average(b => b.YearOfManufacture) : 0,
            TotalMileage = allBuses.Sum(b => b.Mileage)
        };

        return Ok(summary);
    }

    [HttpGet("maintenance-cost-analysis")]
    public async Task<ActionResult> GetMaintenanceCostAnalysis([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var allMaintenance = await _maintenanceService.GetAllMaintenanceAsync();

        if (startDate.HasValue)
            allMaintenance = allMaintenance.Where(m => m.MaintenanceDate >= startDate.Value);

        if (endDate.HasValue)
            allMaintenance = allMaintenance.Where(m => m.MaintenanceDate <= endDate.Value);

        var analysis = new
        {
            TotalMaintenanceRecords = allMaintenance.Count(),
            TotalCost = allMaintenance.Sum(m => m.Cost),
            AverageCost = allMaintenance.Any() ? allMaintenance.Average(m => m.Cost) : 0,
            MaintenanceByType = allMaintenance.GroupBy(m => m.MaintenanceType)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    TotalCost = g.Sum(m => m.Cost)
                })
        };

        return Ok(analysis);
    }

    [HttpGet("examination-statistics")]
    public async Task<ActionResult> GetExaminationStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var allExaminations = await _examinationService.GetAllExaminationsAsync();

        if (startDate.HasValue)
            allExaminations = allExaminations.Where(e => e.ExaminationDate >= startDate.Value);

        if (endDate.HasValue)
            allExaminations = allExaminations.Where(e => e.ExaminationDate <= endDate.Value);

        var statistics = new
        {
            TotalExaminations = allExaminations.Count(),
            PassedExaminations = allExaminations.Count(e => e.ExaminationResult != "Failed"),
            FailedExaminations = allExaminations.Count(e => e.ExaminationResult == "Failed"),
            SentForRepair = allExaminations.Count(e => e.SentForRepair),
            TotalRepairCost = allExaminations.Sum(e => e.RepairPrice),
            AverageRepairCost = allExaminations.Any() ? allExaminations.Average(e => e.RepairPrice) : 0
        };

        return Ok(statistics);
    }

    [HttpGet("parts-inventory-status")]
    public async Task<ActionResult> GetPartsInventoryStatus()
    {
        var allParts = await _repairPartService.GetAllPartsAsync();
        var lowStockParts = await _repairPartService.GetLowStockPartsAsync(10);

        var status = new
        {
            TotalParts = allParts.Count(),
            TotalInventoryValue = allParts.Sum(p => p.UnitPrice * p.StockQuantity),
            LowStockParts = lowStockParts.Count(),
            OutOfStockParts = allParts.Count(p => p.StockQuantity == 0),
            PartsBySupplier = allParts.GroupBy(p => p.Supplier ?? "Unknown")
                .Select(g => new
                {
                    Supplier = g.Key,
                    PartCount = g.Count(),
                    TotalValue = g.Sum(p => p.UnitPrice * p.StockQuantity)
                })
        };

        return Ok(status);
    }

    [HttpGet("bus-detailed-report/{countryNumber}")]
    public async Task<ActionResult> GetBusDetailedReport(string countryNumber)
    {
        var bus = await _busService.GetBusWithStatusAsync(countryNumber);

        if (bus == null)
            return NotFound($"Bus with country number {countryNumber} not found");

        var maintenanceHistory = await _maintenanceService.GetMaintenanceByBusAsync(countryNumber);
        var examinations = await _examinationService.GetExaminationsByBusAsync(countryNumber);
        var totalMaintenanceCost = await _maintenanceService.GetTotalMaintenanceCostAsync(countryNumber);

        var report = new
        {
            BusInfo = bus,
            TotalMaintenanceCost = totalMaintenanceCost,
            MaintenanceRecords = maintenanceHistory.Count(),
            Examinations = examinations.Count(),
            FailedExaminations = examinations.Count(e => e.ExaminationResult == "Failed"),
            RecentMaintenance = maintenanceHistory.OrderByDescending(m => m.MaintenanceDate).Take(5),
            RecentExaminations = examinations.OrderByDescending(e => e.ExaminationDate).Take(5)
        };

        return Ok(report);
    }
}