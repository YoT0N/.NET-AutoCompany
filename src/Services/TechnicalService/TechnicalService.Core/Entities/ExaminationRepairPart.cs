namespace TechnicalService.Domain.Entities;

public class ExaminationRepairPart
{
    public long ExaminationId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal? TotalPrice { get; set; }

    // Navigation properties
    public TechnicalExamination? Examination { get; set; }
    public RepairPart? Part { get; set; }
}