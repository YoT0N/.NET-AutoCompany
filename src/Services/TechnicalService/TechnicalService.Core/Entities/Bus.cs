namespace TechnicalService.Domain.Entities;

public class Bus
{
    public string CountryNumber { get; set; } = string.Empty;
    public string BoardingNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int PassengerCapacity { get; set; }
    public int YearOfManufacture { get; set; }
    public decimal Mileage { get; set; }
    public DateTime DateOfReceipt { get; set; }
    public DateTime? WriteoffDate { get; set; }
    public int CurrentStatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation property
    public BusStatus? CurrentStatus { get; set; }
}
