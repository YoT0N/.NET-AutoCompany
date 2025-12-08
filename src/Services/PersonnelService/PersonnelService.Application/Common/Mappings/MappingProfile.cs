using AutoMapper;
using PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel;
using PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnel;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.ValueObjects;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PersonnelService.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Personnel mappings
            CreateMap<CreatePersonnelCommand, Personnel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PersonnelId, opt => opt.Ignore())
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src =>
                    new PersonnelContactsVO(src.Phone, src.Email, src.Address)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore());

            CreateMap<UpdatePersonnelCommand, Personnel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PersonnelId, opt => opt.Ignore())
                .ForMember(dest => dest.Contacts, opt => opt.Condition(src =>
                    !string.IsNullOrEmpty(src.Phone) ||
                    !string.IsNullOrEmpty(src.Email) ||
                    !string.IsNullOrEmpty(src.Address)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore());

            // DTOs
            CreateMap<Personnel, PersonnelDto>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Contacts.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Contacts.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Contacts.Address))
                .ForMember(dest => dest.DocumentsCount, opt => opt.MapFrom(src => src.Documents.Count));

            CreateMap<PersonnelDocument, DocumentDto>();
            CreateMap<PhysicalExamination, ExaminationDto>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Metrics.Height))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Metrics.Weight))
                .ForMember(dest => dest.BloodPressure, opt => opt.MapFrom(src => src.Metrics.BloodPressure))
                .ForMember(dest => dest.Vision, opt => opt.MapFrom(src => src.Metrics.Vision));

            CreateMap<WorkShiftLog, WorkShiftDto>()
                .ForMember(dest => dest.BusNumber, opt => opt.MapFrom(src => src.Bus.BusCountryNumber))
                .ForMember(dest => dest.BusBrand, opt => opt.MapFrom(src => src.Bus.Brand))
                .ForMember(dest => dest.RouteNumber, opt => opt.MapFrom(src => src.Route.RouteNumber))
                .ForMember(dest => dest.DistanceKm, opt => opt.MapFrom(src => src.Route.DistanceKm));
        }
    }

    // DTOs
    public class PersonnelDto
    {
        public string Id { get; set; } = string.Empty;
        public int PersonnelId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int DocumentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class DocumentDto
    {
        public string Id { get; set; } = string.Empty;
        public int PersonnelId { get; set; }
        public string DocType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime? ValidUntil { get; set; }
    }

    public class ExaminationDto
    {
        public string Id { get; set; } = string.Empty;
        public int PersonnelId { get; set; }
        public DateTime ExamDate { get; set; }
        public string Result { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Weight { get; set; }
        public string BloodPressure { get; set; } = string.Empty;
        public string Vision { get; set; } = string.Empty;
    }

    public class WorkShiftDto
    {
        public string Id { get; set; } = string.Empty;
        public int PersonnelId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;
        public string BusBrand { get; set; } = string.Empty;
        public string RouteNumber { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}