using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.RouteSpecifications
{
    /// <summary>
    /// Specification for filtering routes by distance range
    /// Demonstrates criteria building with optional parameters
    /// </summary>
    public class RoutesByDistanceRangeSpecification : BaseSpecification<Route>
    {
        public RoutesByDistanceRangeSpecification(
            decimal? minDistance = null,
            decimal? maxDistance = null,
            string? routeNumber = null)
        {
            // Build criteria based on provided parameters
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

            // Order by distance (shortest first)
            ApplyOrderBy(r => r.DistanceKm);
        }
    }
}