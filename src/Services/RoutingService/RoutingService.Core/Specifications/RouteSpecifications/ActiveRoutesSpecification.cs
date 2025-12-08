using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System;
using System.Linq;

namespace RoutingService.Domain.Specifications.RouteSpecifications
{
    public class ActiveRoutesSpecification : BaseSpecification<Route>
    {
        public ActiveRoutesSpecification(bool includeSchedules = true)
        {
            ApplyCriteria(r => r.Schedules.Any());

            if (includeSchedules)
            {
                AddInclude(r => r.Schedules);
            }

            ApplyOrderBy(r => r.RouteNumber);
        }

        public ActiveRoutesSpecification(int skip, int take)
        {
            ApplyCriteria(r => r.Schedules.Any());
            AddInclude(r => r.Schedules);
            ApplyOrderBy(r => r.RouteNumber);
            ApplyPaging(skip, take);
        }
    }
}