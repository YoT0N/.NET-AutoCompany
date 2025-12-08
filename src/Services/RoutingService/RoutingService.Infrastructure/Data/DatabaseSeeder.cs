using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoutingService.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Dal.Data.Seeding
{
    public class DatabaseSeeder
    {
        private readonly RoutingDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(RoutingDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                await SeedBusInfoAsync();
                await SeedRouteStopsAsync();
                await SeedRoutesAsync();
                await SeedRouteStopAssignmentsAsync();
                await SeedSchedulesAsync();
                await SeedRouteSheetsAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database seeding");
                throw;
            }
        }

        private async Task SeedBusInfoAsync()
        {
            if (await _context.Buses.AnyAsync())
            {
                _logger.LogInformation("BusInfo data already exists, skipping seed");
                return;
            }

            var buses = new[]
            {
                new BusInfo { CountryNumber = "AA1234BB", Brand = "Mercedes", Capacity = 50, YearOfManufacture = 2020 },
                new BusInfo { CountryNumber = "CC5678DD", Brand = "Volvo", Capacity = 45, YearOfManufacture = 2019 },
                new BusInfo { CountryNumber = "EE9012FF", Brand = "MAN", Capacity = 60, YearOfManufacture = 2021 },
                new BusInfo { CountryNumber = "GG3456HH", Brand = "Scania", Capacity = 55, YearOfManufacture = 2022 },
                new BusInfo { CountryNumber = "II7890JJ", Brand = "Iveco", Capacity = 40, YearOfManufacture = 2018 }
            };

            await _context.Buses.AddRangeAsync(buses);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} buses", buses.Length);
        }

        private async Task SeedRouteStopsAsync()
        {
            if (await _context.RouteStops.AnyAsync())
            {
                _logger.LogInformation("RouteStop data already exists, skipping seed");
                return;
            }

            var stops = new[]
            {
                new RouteStop { StopName = "Центральна площа", Latitude = 48.2920m, Longitude = 25.9358m },
                new RouteStop { StopName = "Вокзал", Latitude = 48.2891m, Longitude = 25.9444m },
                new RouteStop { StopName = "Ринок", Latitude = 48.2950m, Longitude = 25.9320m },
                new RouteStop { StopName = "Університет", Latitude = 48.2980m, Longitude = 25.9400m },
                new RouteStop { StopName = "Лікарня", Latitude = 48.2850m, Longitude = 25.9500m },
                new RouteStop { StopName = "Парк Шевченка", Latitude = 48.3000m, Longitude = 25.9280m },
                new RouteStop { StopName = "Театральна площа", Latitude = 48.2910m, Longitude = 25.9370m },
                new RouteStop { StopName = "Автовокзал", Latitude = 48.2880m, Longitude = 25.9480m }
            };

            await _context.RouteStops.AddRangeAsync(stops);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} route stops", stops.Length);
        }

        private async Task SeedRoutesAsync()
        {
            if (await _context.Routes.AnyAsync())
            {
                _logger.LogInformation("Route data already exists, skipping seed");
                return;
            }

            var now = DateTime.UtcNow;
            var routes = new[]
            {
                new Route { RouteNumber = "1", Name = "Центр - Вокзал", DistanceKm = 5.5m, CreatedAt = now, UpdatedAt = now },
                new Route { RouteNumber = "2", Name = "Ринок - Університет", DistanceKm = 8.2m, CreatedAt = now, UpdatedAt = now },
                new Route { RouteNumber = "3A", Name = "Парк - Лікарня", DistanceKm = 12.3m, CreatedAt = now, UpdatedAt = now },
                new Route { RouteNumber = "5", Name = "Кільцевий маршрут", DistanceKm = 15.7m, CreatedAt = now, UpdatedAt = now }
            };

            await _context.Routes.AddRangeAsync(routes);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} routes", routes.Length);
        }

        private async Task SeedRouteStopAssignmentsAsync()
        {
            if (await _context.RouteStopAssignments.AnyAsync())
            {
                _logger.LogInformation("RouteStopAssignment data already exists, skipping seed");
                return;
            }

            // Get existing routes and stops
            var routes = await _context.Routes.ToListAsync();
            var stops = await _context.RouteStops.ToListAsync();

            if (!routes.Any() || !stops.Any())
            {
                _logger.LogWarning("Cannot seed route stop assignments - routes or stops are missing");
                return;
            }

            var assignments = new[]
            {
                // Route 1: Центр - Вокзал
                new RouteStopAssignment { RouteId = routes[0].RouteId, StopId = stops[0].StopId, StopOrder = 1 },
                new RouteStopAssignment { RouteId = routes[0].RouteId, StopId = stops[6].StopId, StopOrder = 2 },
                new RouteStopAssignment { RouteId = routes[0].RouteId, StopId = stops[1].StopId, StopOrder = 3 },
                
                // Route 2: Ринок - Університет
                new RouteStopAssignment { RouteId = routes[1].RouteId, StopId = stops[2].StopId, StopOrder = 1 },
                new RouteStopAssignment { RouteId = routes[1].RouteId, StopId = stops[0].StopId, StopOrder = 2 },
                new RouteStopAssignment { RouteId = routes[1].RouteId, StopId = stops[3].StopId, StopOrder = 3 },
                
                // Route 3A: Парк - Лікарня
                new RouteStopAssignment { RouteId = routes[2].RouteId, StopId = stops[5].StopId, StopOrder = 1 },
                new RouteStopAssignment { RouteId = routes[2].RouteId, StopId = stops[0].StopId, StopOrder = 2 },
                new RouteStopAssignment { RouteId = routes[2].RouteId, StopId = stops[4].StopId, StopOrder = 3 },
                
                // Route 5: Кільцевий
                new RouteStopAssignment { RouteId = routes[3].RouteId, StopId = stops[0].StopId, StopOrder = 1 },
                new RouteStopAssignment { RouteId = routes[3].RouteId, StopId = stops[2].StopId, StopOrder = 2 },
                new RouteStopAssignment { RouteId = routes[3].RouteId, StopId = stops[3].StopId, StopOrder = 3 },
                new RouteStopAssignment { RouteId = routes[3].RouteId, StopId = stops[4].StopId, StopOrder = 4 },
                new RouteStopAssignment { RouteId = routes[3].RouteId, StopId = stops[0].StopId, StopOrder = 5 }
            };

            await _context.RouteStopAssignments.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} route stop assignments", assignments.Length);
        }

        private async Task SeedSchedulesAsync()
        {
            if (await _context.Schedules.AnyAsync())
            {
                _logger.LogInformation("Schedule data already exists, skipping seed");
                return;
            }

            var routes = await _context.Routes.ToListAsync();
            if (!routes.Any())
            {
                _logger.LogWarning("Cannot seed schedules - routes are missing");
                return;
            }

            var schedules = new[]
            {
                // Route 1
                new Schedule { RouteId = routes[0].RouteId, DepartureTime = new TimeSpan(6, 0, 0), ArrivalTime = new TimeSpan(6, 30, 0) },
                new Schedule { RouteId = routes[0].RouteId, DepartureTime = new TimeSpan(8, 0, 0), ArrivalTime = new TimeSpan(8, 30, 0) },
                new Schedule { RouteId = routes[0].RouteId, DepartureTime = new TimeSpan(12, 0, 0), ArrivalTime = new TimeSpan(12, 30, 0) },
                
                // Route 2
                new Schedule { RouteId = routes[1].RouteId, DepartureTime = new TimeSpan(7, 0, 0), ArrivalTime = new TimeSpan(7, 45, 0) },
                new Schedule { RouteId = routes[1].RouteId, DepartureTime = new TimeSpan(14, 0, 0), ArrivalTime = new TimeSpan(14, 45, 0) },
                
                // Route 3A
                new Schedule { RouteId = routes[2].RouteId, DepartureTime = new TimeSpan(6, 30, 0), ArrivalTime = new TimeSpan(7, 30, 0) },
                new Schedule { RouteId = routes[2].RouteId, DepartureTime = new TimeSpan(16, 0, 0), ArrivalTime = new TimeSpan(17, 0, 0) }
            };

            await _context.Schedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} schedules", schedules.Length);
        }

        private async Task SeedRouteSheetsAsync()
        {
            if (await _context.RouteSheets.AnyAsync())
            {
                _logger.LogInformation("RouteSheet data already exists, skipping seed");
                return;
            }

            var routes = await _context.Routes.ToListAsync();
            var buses = await _context.Buses.ToListAsync();

            if (!routes.Any() || !buses.Any())
            {
                _logger.LogWarning("Cannot seed route sheets - routes or buses are missing");
                return;
            }

            var today = DateTime.Today;
            var routeSheets = new[]
            {
                new RouteSheet { RouteId = routes[0].RouteId, BusId = buses[0].BusId, SheetDate = today },
                new RouteSheet { RouteId = routes[1].RouteId, BusId = buses[1].BusId, SheetDate = today },
                new RouteSheet { RouteId = routes[2].RouteId, BusId = buses[2].BusId, SheetDate = today },
                new RouteSheet { RouteId = routes[0].RouteId, BusId = buses[3].BusId, SheetDate = today.AddDays(1) }
            };

            await _context.RouteSheets.AddRangeAsync(routeSheets);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} route sheets", routeSheets.Length);
        }
    }
}