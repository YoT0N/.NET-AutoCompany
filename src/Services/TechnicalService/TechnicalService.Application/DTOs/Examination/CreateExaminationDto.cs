namespace TechnicalService.Bll.DTOs.Examination;

/// <summary>
/// DTO для створення технічного огляду
/// </summary>
public class CreateExaminationDto
{
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string ExaminationResult { get; set; } = string.Empty; // "Passed", "Failed"
    public bool SentForRepair { get; set; }
    public decimal RepairPrice { get; set; }
    public string? MechanicName { get; set; }
    public string? Notes { get; set; }

    // Запчастини, використані під час огляду/ремонту
    public List<CreateRepairPartDto>? RepairParts { get; set; }
}

/// <summary>
/// DTO для оновлення технічного огляду
/// </summary>
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

/// <summary>
/// DTO для відображення технічного огляду
/// </summary>
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

/// <summary>
/// DTO для запчастини при створенні огляду
/// </summary>
public class CreateRepairPartDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO для відображення запчастини
/// </summary>
public class RepairPartDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}