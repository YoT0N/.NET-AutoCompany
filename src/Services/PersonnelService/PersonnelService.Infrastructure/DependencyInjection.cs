using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Infrastructure.Context;
using PersonnelService.Infrastructure.Repositories;
using PersonnelService.Infrastructure.Seeders;

namespace PersonnelService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register MongoDbContext
            services.AddSingleton<MongoDbContext>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new MongoDbContext(config);
            });

            // Register MongoDB mappings
            MongoDbMappings.RegisterClassMaps();

            // Register Repositories
            services.AddScoped<IPersonnelRepository, PersonnelRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IExaminationRepository, ExaminationRepository>();
            services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();

            // Register Seeders
            services.AddSingleton<PersonnelSeeder>();
            services.AddSingleton<DocumentSeeder>();
            services.AddSingleton<ExaminationSeeder>();
            services.AddSingleton<WorkShiftSeeder>();

            services.AddSingleton<IDataSeeder>(sp => sp.GetRequiredService<PersonnelSeeder>());
            services.AddSingleton<IDataSeeder>(sp => sp.GetRequiredService<DocumentSeeder>());
            services.AddSingleton<IDataSeeder>(sp => sp.GetRequiredService<ExaminationSeeder>());
            services.AddSingleton<IDataSeeder>(sp => sp.GetRequiredService<WorkShiftSeeder>());

            services.AddSingleton<SeedManager>();

            // Register Health Checks
            services.AddHealthChecks()
                .AddCheck<MongoDbHealthCheck>("mongodb");

            return services;
        }
    }
}