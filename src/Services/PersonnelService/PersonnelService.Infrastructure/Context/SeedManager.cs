using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Infrastructure.Context
{
    public class SeedManager
    {
        private readonly IEnumerable<IDataSeeder> _seeders;

        public SeedManager(IEnumerable<IDataSeeder> seeders)
        {
            _seeders = seeders;
        }

        public async Task SeedAllAsync()
        {
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync();
            }
        }
    }
}