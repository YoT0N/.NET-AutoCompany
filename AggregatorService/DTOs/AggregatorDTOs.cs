using AggregatorService.Clients;

namespace AggregatorService.DTOs;

public class BusFullInfoDto
{
    public string CountryNumber { get; set; } = string.Empty;
    public string BoardingNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int PassengerCapacity { get; set; }
    public int YearOfManufacture { get; set; }
    public decimal Mileage { get; set; }
    public string? CurrentStatusName { get; set; }
    public List<RouteSheetDto> RouteSheets { get; set; } = new();
}

public class RouteSheetDto
{
    public int SheetId { get; set; }
    public int RouteId { get; set; }
    public int BusId { get; set; }
    public string SheetDate { get; set; } = string.Empty;
    public string RouteNumber { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string BusCountryNumber { get; set; } = string.Empty;
}

public class PersonnelFullInfoDto
{
    public int PersonnelId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<WorkShiftDto> WorkShifts { get; set; } = new();
    public int TotalShifts { get; set; }
    public double TotalDistance { get; set; }
}

public class DashboardSummaryDto
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalBuses { get; set; }
    public int TotalRoutes { get; set; }
    public int TotalPersonnel { get; set; }
}