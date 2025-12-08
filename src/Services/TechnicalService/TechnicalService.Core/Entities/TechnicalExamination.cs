namespace TechnicalService.Domain.Entities;

public class TechnicalExamination
{
    public long ExaminationId { get; set; }
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string ExaminationResult { get; set; } = string.Empty;
    public bool SentForRepair { get; set; }
    public decimal RepairPrice { get; set; }
    public string? MechanicName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation properties
    public Bus? Bus { get; set; }
    public List<ExaminationRepairPart> RepairParts { get; set; } = new();
}