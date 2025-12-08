using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.RouteSpecifications
{
    public class RoutesByDistanceRangeSpecification : BaseSpecification<Route>
    {
        public RoutesByDistanceRangeSpecification(
            decimal? minDistance = null,
            decimal? maxDistance = null,
            string? routeNumber = null)
        {
            if (minDistance.HasValue)
            {
                AddAndCriteria(r => r.DistanceKm >= minDistance.Value);
            }

            if (maxDistance.HasValue)
            {
                AddAndCriteria(r => r.DistanceKm <= maxDistance.Value);
            }

            if (!string.IsNullOrWhiteSpace(routeNumber))
            {
                AddAndCriteria(r => r.RouteNumber.Contains(routeNumber));
            }

            ApplyOrderBy(r => r.DistanceKm);
        }
    }
}