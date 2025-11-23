using Microsoft.EntityFrameworkCore;
using RoutingService.Core.Entities;
using RoutingService.Infrastructure.Data.Configurations;
using RoutingService.Infrastructure.Data.RoutingDbContext;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RoutingService.Infrastructure.Data
{
    public class RoutingDbContext : DbContext
    {
        public RoutingDbContext(DbContextOptions<RoutingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Route> Routes { get; set; } = null!;
        public DbSet<RouteStop> RouteStops { get; set; } = null!;
        public DbSet<RouteStopAssignment> RouteStopAssignments { get; set; } = null!;
        public DbSet<BusInfo> Buses { get; set; } = null!;
        public DbSet<RouteSheet> RouteSheets { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<Trip> Trips { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RouteConfiguration());
            modelBuilder.ApplyConfiguration(new RouteStopConfiguration());
            modelBuilder.ApplyConfiguration(new RouteStopAssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new BusInfoConfiguration());
            modelBuilder.ApplyConfiguration(new RouteSheetConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new TripConfiguration());
        }
    }
}