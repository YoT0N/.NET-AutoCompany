using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System.Linq;

namespace RoutingService.Domain.Specifications.RouteSpecifications
{
    /// <summary>
    /// Specification for loading routes with their stops
    /// Demonstrates Include with ordering
    /// </summary>
    public class RouteWithStopsSpecification : BaseSpecification<Route>
    {
        public RouteWithStopsSpecification(int? routeId = null)
        {
            // Add criteria if routeId is provided
            if (routeId.HasValue)
            {
                ApplyCriteria(r => r.RouteId == routeId.Value);
            }

            // Include route stop assignments and their stops
            AddInclude(r => r.RouteStopAssignments);
            AddInclude("RouteStopAssignments.RouteStop");

            // Order by route number
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