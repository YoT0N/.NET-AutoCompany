namespace TechnicalService.Bll.DTOs.Examination;

public class CreateExaminationDto
{
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string ExaminationResult { get; set; } = string.Empty;
    public bool SentForRepair { get; set; }
    public decimal RepairPrice { get; set; }
    public string? MechanicName { get; set; }
    public string? Notes { get; set; }

    // Запчастини, використані під час огляду/ремонту
    public List<CreateRepairPartDto>? RepairParts { get; set; }
}


public class UpdateExaminationDto
{
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string ExaminationResult { get; set; } = string.Empty;
    public bool SentForRepair { get; set; }
    public decimal RepairPrice { get; set; }
    public string? MechanicName { get; set; }
    public string? Notes { get; set; }
}

/// DTO для відображення технічного огляду
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
    public DateTime CreatedAt { get; set; }

    // Запчастини (якщо завантажені)
    public List<RepairPartDto>? RepairParts { get; set; }
}

/// DTO для запчастини при створенні огляду
public class CreateRepairPartDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}

/// DTO для відображення запчастини
public class RepairPartDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}