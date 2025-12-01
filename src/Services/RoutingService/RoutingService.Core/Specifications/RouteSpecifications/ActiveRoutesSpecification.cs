using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;
using System;
using System.Linq;

namespace RoutingService.Domain.Specifications.RouteSpecifications
{
    /// <summary>
    /// Specification for active routes (routes with schedules)
    /// Demonstrates complex filtering and includes
    /// </summary>
    public class ActiveRoutesSpecification : BaseSpecification<Route>
    {
        public ActiveRoutesSpecification(bool includeSchedules = true)
        {
            // Only routes that have schedules
            ApplyCriteria(r => r.Schedules.Any());

            if (includeSchedules)
            {
                AddInclude(r => r.Schedules);
            }

            // Order by route number
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