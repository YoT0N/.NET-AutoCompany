using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.ScheduleSpecifications
{
    /// <summary>
    /// Specification for getting schedules by route
    /// Demonstrates simple filtering with navigation property loading
    /// </summary>
    public class ScheduleByRouteSpecification : BaseSpecification<Schedule>
    {
        public ScheduleByRouteSpecification(int routeId, bool includeRoute = true)
            : base(s => s.RouteId == routeId)
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