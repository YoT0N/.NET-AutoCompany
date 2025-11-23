namespace TechnicalService.Core.DTOs;

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

public class CreateExaminationDto
{
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string ExaminationResult { get; set; } = string.Empty;
    public bool SentForRepair { get; set; }
    public decimal RepairPrice { get; set; }
    public string? MechanicName { get; set; }
    public string? Notes { get; set; }
    public List<ExaminationRepairPartDto>? RepairParts { get; set; }
}

public class RepairPartDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ExaminationRepairPartDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}