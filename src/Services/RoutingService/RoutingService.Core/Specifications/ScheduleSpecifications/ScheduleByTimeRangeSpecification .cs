using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System;

namespace RoutingService.Domain.Specifications.ScheduleSpecifications
{
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

            ApplyOrderBy(s => s.DepartureTime);
        }
    }
}