using AutoMapper;
using TechnicalService.Bll.DTOs.Bus;
using TechnicalService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechnicalService.Bll.Mapping;

public class BusProfile : Profile
{
    public BusProfile()
    {
        CreateMap<Bus, BusDto>()
            .ForMember(dest => dest.CurrentStatusName,
                opt => opt.MapFrom(src => src.CurrentStatus != null ? src.CurrentStatus.StatusName : null));

        CreateMap<CreateBusDto, Bus>();

        CreateMap<UpdateBusDto, Bus>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}