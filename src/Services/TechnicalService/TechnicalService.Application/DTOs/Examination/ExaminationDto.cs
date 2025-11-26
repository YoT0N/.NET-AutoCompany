namespace TechnicalService.Bll.DTOs.Examination;

public class ExaminationDto
{
    public long ExaminationId { get; set; }
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string ExaminationResult { get; set; } = string.Empty;
    public bool SentForRepair { get; set; }
    public decimal RepairPrice { get; set; }
    public string? MechanicName { get; set; }
    public string? Notes { get; set; }
    public List<RepairPartDto> RepairParts { get; set; } = new();
}

public class ExaminationRepairPartDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}