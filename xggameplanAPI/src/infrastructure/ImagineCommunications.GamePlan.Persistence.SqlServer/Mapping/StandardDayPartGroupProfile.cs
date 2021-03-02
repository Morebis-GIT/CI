using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class StandardDayPartGroupProfile : Profile
    {
        public StandardDayPartGroupProfile()
        {
            CreateMap<StandardDayPartGroup, Entities.Tenant.DayParts.StandardDayPartGroup>()
                .ForMember(d => d.Splits, o => o.MapFrom(src => src.Splits)).ReverseMap();
            CreateMap<StandardDayPartSplit, Entities.Tenant.DayParts.StandardDayPartSplit>().ReverseMap();
        }
    }
}
