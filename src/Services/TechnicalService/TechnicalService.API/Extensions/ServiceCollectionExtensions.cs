using Microsoft.Extensions.DependencyInjection;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Bll.Mapping;
using TechnicalService.Bll.Services;
using TechnicalService.Dal.Data;
using TechnicalService.Dal.Implementations.AdoNet;
using TechnicalService.Dal.Implementations.Dapper;
using TechnicalService.Dal.Implementations;
using TechnicalService.Dal.Interfaces;

namespace TechnicalService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Реєстрація DapperContext
        services.AddSingleton<DapperContext>(provider =>
            new DapperContext(configuration));


        // Реєстрація репозиторіїв
        services.AddScoped<IBusRepository, BusRepositoryAdoNet>(); // Чистий ADO.NET
        services.AddScoped<IExaminationRepository, ExaminationRepository>(); // Dapper
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>(); // Dapper
        services.AddScoped<IRepairPartRepository, RepairPartRepository>(); // Dapper

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Business Logic сервіси
        services.AddScoped<IBusService, BusService>();
        services.AddScoped<IExaminationService, ExaminationService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IRepairPartService, RepairPartService>();

        // AutoMapper
        services.AddAutoMapper(typeof(BusProfile).Assembly);

        return services;
    }
}