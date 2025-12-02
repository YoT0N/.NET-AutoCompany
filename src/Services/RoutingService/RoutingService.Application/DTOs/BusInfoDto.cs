namespace RoutingService.Bll.DTOs
{
    public class BusInfoDto
    {
        public int BusId { get; set; }
        public string CountryNumber { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public int? Capacity { get; set; }
        public int? YearOfManufacture { get; set; }
    }

    public class CreateBusInfoDto
    {
        public string CountryNumber { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public int? Capacity { get; set; }
        public int? YearOfManufacture { get; set; }
    }

    public class UpdateBusInfoDto
    {
        public string? CountryNumber { get; set; }
        public string? Brand { get; set; }
        public int? Capacity { get; set; }
        public int? YearOfManufacture { get; set; }
    }
}