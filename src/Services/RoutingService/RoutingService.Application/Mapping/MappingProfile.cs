using AutoMapper;
using RoutingService.Bll.DTOs;
using RoutingService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoutingService.Bll.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BusInfo, BusInfoDto>();
            CreateMap<CreateBusInfoDto, BusInfo>()
                .ForMember(dest => dest.BusId, opt => opt.Ignore())
                .ForMember(dest => dest.RouteSheets, opt => opt.Ignore());
            CreateMap<UpdateBusInfoDto, BusInfo>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Route, RouteDto>();
            CreateMap<CreateRouteDto, Route>()
                .ForMember(dest => dest.RouteId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RouteStopAssignments, opt => opt.Ignore())
                .ForMember(dest => dest.Schedules, opt => opt.Ignore())
                .ForMember(dest => dest.RouteSheets, opt => opt.Ignore());
            CreateMap<UpdateRouteDto, Route>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Route, RouteWithStopsDto>()
                .ForMember(dest => dest.Stops, opt => opt.MapFrom(src => src.RouteStopAssignments));

            CreateMap<RouteStop, RouteStopDto>();
            CreateMap<CreateRouteStopDto, RouteStop>()
                .ForMember(dest => dest.StopId, opt => opt.Ignore())
                .ForMember(dest => dest.RouteStopAssignments, opt => opt.Ignore());
            CreateMap<UpdateRouteStopDto, RouteStop>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<RouteStopAssignment, RouteStopInfoDto>()
                .ForMember(dest => dest.StopId, opt => opt.MapFrom(src => src.StopId))
                .ForMember(dest => dest.StopName, opt => opt.MapFrom(src => src.RouteStop.StopName))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.RouteStop.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.RouteStop.Longitude))
                .ForMember(dest => dest.StopOrder, opt => opt.MapFrom(src => src.StopOrder));

            CreateMap<Schedule, ScheduleDto>();
            CreateMap<CreateScheduleDto, Schedule>()
                .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
                .ForMember(dest => dest.Route, opt => opt.Ignore());
            CreateMap<UpdateScheduleDto, Schedule>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Schedule, ScheduleWithRouteDto>()
                .ForMember(dest => dest.RouteNumber, opt => opt.MapFrom(src => src.Route.RouteNumber))
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route.Name));

            CreateMap<RouteSheet, RouteSheetDto>();
            CreateMap<CreateRouteSheetDto, RouteSheet>()
                .ForMember(dest => dest.SheetId, opt => opt.Ignore())
                .ForMember(dest => dest.Route, opt => opt.Ignore())
                .ForMember(dest => dest.BusInfo, opt => opt.Ignore())
                .ForMember(dest => dest.Trips, opt => opt.Ignore());
            CreateMap<UpdateRouteSheetDto, RouteSheet>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<RouteSheet, RouteSheetDetailsDto>()
                .ForMember(dest => dest.RouteNumber, opt => opt.MapFrom(src => src.Route.RouteNumber))
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route.Name))
                .ForMember(dest => dest.BusCountryNumber, opt => opt.MapFrom(src => src.BusInfo.CountryNumber))
                .ForMember(dest => dest.BusBrand, opt => opt.MapFrom(src => src.BusInfo.Brand));

            CreateMap<Trip, TripDto>();
            CreateMap<CreateTripDto, Trip>()
                .ForMember(dest => dest.TripId, opt => opt.Ignore())
                .ForMember(dest => dest.ActualDeparture, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.Ignore())
                .ForMember(dest => dest.RouteSheet, opt => opt.Ignore());
            CreateMap<UpdateTripDto, Trip>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Trip, TripDetailsDto>()
                .ForMember(dest => dest.SheetDate, opt => opt.MapFrom(src => src.RouteSheet.SheetDate))
                .ForMember(dest => dest.RouteNumber, opt => opt.MapFrom(src => src.RouteSheet.Route.RouteNumber))
                .ForMember(dest => dest.BusCountryNumber, opt => opt.MapFrom(src => src.RouteSheet.BusInfo.CountryNumber));
        }
    }
}