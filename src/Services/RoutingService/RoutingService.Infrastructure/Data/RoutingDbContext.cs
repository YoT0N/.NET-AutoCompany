using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Dal.Data.Configurations;

namespace RoutingService.Dal.Data
{
    public class RoutingDbContext : DbContext
    {
        public RoutingDbContext(DbContextOptions<RoutingDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<RouteStopAssignment> RouteStopAssignments { get; set; }
        public DbSet<BusInfo> Buses { get; set; }
        public DbSet<RouteSheet> RouteSheets { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoutingDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<Route>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}