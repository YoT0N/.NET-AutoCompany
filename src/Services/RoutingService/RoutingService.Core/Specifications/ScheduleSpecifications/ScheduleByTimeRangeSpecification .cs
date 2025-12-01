using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System;

namespace RoutingService.Domain.Specifications.ScheduleSpecifications
{
    /// <summary>
    /// Specification for filtering schedules by time range
    /// Demonstrates time-based filtering
    /// </summary>
    public class ScheduleByTimeRangeSpecification : BaseSpecification<Schedule>
    {
        public ScheduleByTimeRangeSpecification(
            TimeSpan startTime,
            TimeSpan endTime,
            bool includeRoute = true)
            : base(s => s.DepartureTime >= startTime && s.DepartureTime <= endTime)
        {
            if (includeRoute)
            {
                AddInclude(s => s.Route);
            }

            // Order by departure time
            ApplyOrderBy(s => s.DepartureTime);
        }
    }
}