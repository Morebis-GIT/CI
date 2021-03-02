using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping.SalesAreas
{
    public class SalesAreaProfile : Profile
    {
        public SalesAreaProfile()
        {
            CreateMap<SalesArea, Domain.Shared.SalesAreas.SalesArea>()
                .ForMember(d => d.ChannelGroup,
                           o => o.MapFrom(src => src.ChannelGroups
                                                 .Select(x => x.Name)))
                .ForMember(d => d.PublicHolidays,
                           o => o.MapFrom(src => src.Holidays.Where(x => x.Type == HolidayType.Public)
                                                 .Select(x => new DateRange(x.Start, x.End))))
                .ForMember(d => d.SchoolHolidays,
                           o => o.MapFrom(src => src.Holidays.Where(x => x.Type == HolidayType.School)
                                                 .Select(x => new DateRange(x.Start, x.End))))
                .ReverseMap()
                .ForMember(d => d.ChannelGroups,
                           o => o.MapFrom(src => src.ChannelGroup
                                                 .Select(x => new SalesAreasChannelGroup()
                                                 {
                                                     Name = x,
                                                     SalesAreaId = src.Id
                                                 })))
                .ForMember(d => d.Holidays,
                           o => o.MapFrom(src => src.PublicHolidays
                                                 .Select(x => new SalesAreasHoliday()
                                                 {
                                                     Start = x.Start,
                                                     End = x.End,
                                                     SalesAreaId = src.Id,
                                                     Type = HolidayType.Public
                                                 })
                                                 .Union(src.SchoolHolidays
                                                 .Select(x => new SalesAreasHoliday()
                                                 {
                                                     Start = x.Start,
                                                     End = x.End,
                                                     SalesAreaId = src.Id,
                                                     Type = HolidayType.School
                                                 }))));
        }
    }
}
