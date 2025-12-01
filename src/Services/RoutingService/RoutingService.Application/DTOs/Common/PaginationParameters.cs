using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Sorting
        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";

        // Calculated property for Skip
        public int Skip => (Page - 1) * PageSize;
    }

    /// <summary>
    /// Route-specific filter parameters
    /// </summary>
    public class RouteFilterParameters : PaginationParameters
    {
        public string? RouteNumber { get; set; }
        public string? Name { get; set; }
        public decimal? MinDistance { get; set; }
        public decimal? MaxDistance { get; set; }
    }

    /// <summary>
    /// Schedule-specific filter parameters
    /// </summary>
    public class ScheduleFilterParameters : PaginationParameters
    {
        public int? RouteId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    /// <summary>
    /// Trip-specific filter parameters
    /// </summary>
    public class TripFilterParameters : PaginationParameters
    {
        public int? SheetId { get; set; }
        public bool? Completed { get; set; }
        public DateTime? Date { get; set; }
    }
}
