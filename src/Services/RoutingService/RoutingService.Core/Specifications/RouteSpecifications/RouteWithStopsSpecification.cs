using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System.Linq;

namespace RoutingService.Domain.Specifications.RouteSpecifications
{
    public class RouteWithStopsSpecification : BaseSpecification<Route>
    {
        public RouteWithStopsSpecification(int? routeId = null)
        {
            if (routeId.HasValue)
            {
                ApplyCriteria(r => r.RouteId == routeId.Value);
            }

            AddInclude(r => r.RouteStopAssignments);
            AddInclude("RouteStopAssignments.RouteStop");

            ApplyOrderBy(r => r.RouteNumber);
        }

        public RouteWithStopsSpecification(string routeNumber)
            : base(r => r.RouteNumber == routeNumber)
        {
            AddInclude(r => r.RouteStopAssignments);
            AddInclude("RouteStopAssignments.RouteStop");
        }
    }
}