namespace TechnicalService.Core.Entities;

public class RepairPart
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? Supplier { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}