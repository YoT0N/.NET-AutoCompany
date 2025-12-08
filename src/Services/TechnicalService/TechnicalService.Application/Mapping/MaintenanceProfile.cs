using AutoMapper;
using TechnicalService.Bll.DTOs.Maintenance;
using TechnicalService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechnicalService.Bll.Mapping;

public class MaintenanceProfile : Profile
{
    public MaintenanceProfile()
    {
        CreateMap<CreateMaintenanceDto, BusMaintenanceHistory>()
            .ForMember(dest => dest.MaintenanceId, opt => opt.Ignore())
            .ForMember(dest => dest.MaintenanceDate, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<BusMaintenanceHistory, MaintenanceHistoryDto>();
    }
}