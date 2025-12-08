using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System;

namespace RoutingService.Domain.Specifications.RouteSheetSpecifications
{
    public class RouteSheetByDateSpecification : BaseSpecification<RouteSheet>
    {
        public RouteSheetByDateSpecification(
            DateTime date,
            bool includeRelatedData = true)
            : base(rs => rs.SheetDate.Date == date.Date)
        {
            if (includeRelatedData)
            {
                AddInclude(rs => rs.Route);
                AddInclude(rs => rs.BusInfo);
                AddInclude(rs => rs.Trips);
            }

            ApplyOrderBy(rs => rs.Route.RouteNumber);
        }

        public RouteSheetByDateSpecification(
            DateTime startDate,
            DateTime endDate,
            bool includeRelatedData = true)
            : base(rs => rs.SheetDate.Date >= startDate.Date && rs.SheetDate.Date <= endDate.Date)
        {
            if (includeRelatedData)
            {
                AddInclude(rs => rs.Route);
                AddInclude(rs => rs.BusInfo);
            }

            ApplyOrderBy(rs => rs.SheetDate);
        }
    }
}