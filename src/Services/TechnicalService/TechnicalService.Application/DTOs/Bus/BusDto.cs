namespace TechnicalService.Bll.DTOs.Bus;

public class BusDto
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
    public string? CurrentStatusName { get; set; }
}