using System;

namespace RoutingService.Core.DTOs
{
    public class RouteSheetDto
    {
        public int SheetId { get; set; }
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public DateTime SheetDate { get; set; }
    }

    public class CreateRouteSheetDto
    {
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public DateTime SheetDate { get; set; }
    }

    public class UpdateRouteSheetDto
    {
        public int? RouteId { get; set; }
        public int? BusId { get; set; }
        public DateTime? SheetDate { get; set; }
    }

    public class RouteSheetDetailsDto : RouteSheetDto
    {
        public string RouteNumber { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string BusCountryNumber { get; set; } = string.Empty;
        public string? BusBrand { get; set; }
    }
}