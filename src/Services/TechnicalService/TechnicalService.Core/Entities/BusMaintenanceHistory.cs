namespace TechnicalService.Domain.Entities;

public class BusMaintenanceHistory
{
    public long MaintenanceId { get; set; }
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime MaintenanceDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Cost { get; set; }
    public string? MechanicName { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation property
    public Bus? Bus { get; set; }
}