using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.ScheduleSpecifications
{
    public class ScheduleByRouteSpecification : BaseSpecification<Schedule>
    {
        public ScheduleByRouteSpecification(int routeId, bool includeRoute = true)
            : base(s => s.RouteId == routeId)
        {
            if (includeRoute)
            {
                AddInclude(s => s.Route);
            }

            ApplyOrderBy(s => s.DepartureTime);
        }
    }
}