using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System.Linq;

namespace RoutingService.Domain.Specifications.TripSpecifications
{

    public class DelayedTripsSpecification : BaseSpecification<Trip>
    {
        public DelayedTripsSpecification(bool includeRouteSheet = true)
            : base(t => t.ActualDeparture != null && t.ActualDeparture > t.ScheduledDeparture)
        {
            if (includeRouteSheet)
            {
                AddInclude(t => t.RouteSheet);
                AddInclude("RouteSheet.Route");
                AddInclude("RouteSheet.BusInfo");
            }

            ApplyOrderByDescending(t => t.ActualDeparture);
        }
    }
}