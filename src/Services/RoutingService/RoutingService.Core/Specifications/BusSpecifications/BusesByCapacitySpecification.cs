// src/Services/RoutingService/RoutingService.Domain/Specifications/BusSpecifications/BusesByCapacitySpecification.cs
using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.BusSpecifications
{
    public class BusesByCapacitySpecification : BaseSpecification<BusInfo>
    {
        public BusesByCapacitySpecification(int minCapacity, int? maxCapacity = null)
        {
            AddAndCriteria(b => b.Capacity >= minCapacity);

            if (maxCapacity.HasValue)
            {
                AddAndCriteria(b => b.Capacity <= maxCapacity.Value);
            }

            ApplyOrderByDescending(b => b.Capacity!);
        }
    }
}