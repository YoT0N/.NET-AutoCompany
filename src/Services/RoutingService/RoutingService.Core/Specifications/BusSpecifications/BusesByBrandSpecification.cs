using RoutingService.Domain.Entities;
using RoutingService.Domain.Specifications.Base;

namespace RoutingService.Domain.Specifications.BusSpecifications
{
    public class BusesByBrandSpecification : BaseSpecification<BusInfo>
    {
        public BusesByBrandSpecification(string brand, bool includeRouteSheets = false)
            : base(b => b.Brand != null && b.Brand.ToLower().Contains(brand.ToLower()))
        {
            if (includeRouteSheets)
            {
                AddInclude(b => b.RouteSheets);
            }

            ApplyOrderBy(b => b.CountryNumber);
        }
    }
}