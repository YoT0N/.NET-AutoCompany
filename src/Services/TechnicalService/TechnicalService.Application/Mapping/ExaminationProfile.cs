using AutoMapper;
using TechnicalService.Bll.DTOs.Examination;
using TechnicalService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechnicalService.Bll.Mapping;

public class ExaminationProfile : Profile
{
    public ExaminationProfile()
    {
        CreateMap<CreateExaminationDto, TechnicalExamination>()
            .ForMember(dest => dest.ExaminationId, opt => opt.Ignore())
            .ForMember(dest => dest.ExaminationDate, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<TechnicalExamination, ExaminationDto>();
    }
}