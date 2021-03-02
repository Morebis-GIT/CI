using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class StandardDayPartProfile : Profile
    {
        public StandardDayPartProfile()
        {
            CreateMap<StandardDayPart, Entities.Tenant.DayParts.StandardDayPart>()
                .ForMember(d => d.Timeslices, o => o.MapFrom(src => src.Timeslices)).ReverseMap();
            CreateMap<StandardDayPartTimeslice, Entities.Tenant.DayParts.StandardDayPartTimeslice>().ReverseMap();
        }
    }
}
