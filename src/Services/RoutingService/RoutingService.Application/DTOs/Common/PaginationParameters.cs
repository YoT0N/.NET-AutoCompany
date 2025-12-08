namespace RoutingService.Bll.DTOs.Common
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";

        public int Skip => (Page - 1) * PageSize;
    }

    public class BusFilterParameters : PaginationParameters
    {
        public string? CountryNumber { get; set; }
        public string? Brand { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
    }

    public class RouteFilterParameters : PaginationParameters
    {
        public string? RouteNumber { get; set; }
        public string? Name { get; set; }
        public decimal? MinDistance { get; set; }
        public decimal? MaxDistance { get; set; }
    }

    public class ScheduleFilterParameters : PaginationParameters
    {
        public int? RouteId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    public class TripFilterParameters : PaginationParameters
    {
        public int? SheetId { get; set; }
        public bool? Completed { get; set; }
        public DateTime? Date { get; set; }
    }

    public class RouteSheetFilterParameters : PaginationParameters
    {
        public int? RouteId { get; set; }
        public int? BusId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class RouteStopFilterParameters : PaginationParameters
    {
        public string? StopName { get; set; }
        public decimal? MinLatitude { get; set; }
        public decimal? MaxLatitude { get; set; }
        public decimal? MinLongitude { get; set; }
        public decimal? MaxLongitude { get; set; }
    }
}