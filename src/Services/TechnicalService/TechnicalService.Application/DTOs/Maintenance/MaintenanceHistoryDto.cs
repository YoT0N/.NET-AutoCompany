namespace TechnicalService.Bll.DTOs.Maintenance;

public class MaintenanceHistoryDto
{
    public long MaintenanceId { get; set; }
    public string BusCountryNumber { get; set; } = string.Empty;
    public DateTime MaintenanceDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Cost { get; set; }
    public string? MechanicName { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}