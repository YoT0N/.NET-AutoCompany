namespace TechnicalService.Core.Entities;

public class BusStatus
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? StatusDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}