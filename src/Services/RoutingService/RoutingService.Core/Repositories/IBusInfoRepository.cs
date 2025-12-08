using RoutingService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    public interface IBusInfoRepository : IRepository<BusInfo>
    {
        Task<BusInfo?> GetBusWithRouteSheetsAsync(int busId);

        Task<IEnumerable<BusInfo>> GetBusesByBrandAsync(string brand, int? minCapacity = null);

        Task<IEnumerable<BusInfo>> GetBusesByYearRangeAsync(int startYear, int endYear);

        Task<IEnumerable<BusInfo>> GetAvailableBusesAsync(System.DateTime date);

        Task<Dictionary<int, int>> GetBusTripStatisticsAsync();
    }
}