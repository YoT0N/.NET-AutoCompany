using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.TripSpecifications
{
    /// <summary>
    /// Specification for filtering trips by completion status
    /// Demonstrates boolean filtering with includes
    /// </summary>
    public class TripsByCompletionStatusSpecification : BaseSpecification<Trip>
    {
        public TripsByCompletionStatusSpecification(
            bool completed,
            bool includeRouteSheet = true)
            : base(t => t.Completed == completed)
        {
            if (includeRouteSheet)
            {
                AddInclude(t => t.RouteSheet);
                AddInclude("RouteSheet.Route");
                AddInclude("RouteSheet.BusInfo");
            }

            // Order by scheduled departure
            ApplyOrderBy(t => t.ScheduledDeparture);
        }

        /// <summary>
        /// Constructor for pending trips (not completed)
        /// </summary>
        public static TripsByCompletionStatusSpecification PendingTrips(bool includeRouteSheet = true)
        {
            return new TripsByCompletionStatusSpecification(false, includeRouteSheet);
        }

        /// <summary>
        /// Constructor for completed trips
        /// </summary>
        public static TripsByCompletionStatusSpecification CompletedTrips(bool includeRouteSheet = true)
        {
            return new TripsByCompletionStatusSpecification(true, includeRouteSheet);
        }
    }
}